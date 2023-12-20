using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using SOMIOD_IS.Models;
using SOMIOD_IS.Properties;
using SOMIOD_IS.Controllers;
using System.ComponentModel;
using System.Data.Common;
using System.Reflection;
using Container = SOMIOD_IS.Models.Container;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace SOMIOD_IS.SqlHelpers
{
    public static class DbHelper
    {

        #region Metodos Genericos

        // Verifies if a child with the given name exists in the parent with the given name,
        // and if the parent itself exists.
        // This method should be used in Deletes/Updates, i.e., situations where the child already exists.
        // Returns the id of the child.
        private static int IsParentValid(SqlConnection db, string parentType, string parentName, string childType, string childName)
        {
            // Parameterized query to prevent SQL injection
            var query =
                $"SELECT c.Id FROM {childType} c JOIN {parentType} p ON (c.Parent = p.Id) WHERE p.Name=@ParentName AND c.Name=@ChildName";

            using (var cmd = new SqlCommand(query, db))
            {
                cmd.Parameters.AddWithValue("@ParentName", parentName.ToLower());
                cmd.Parameters.AddWithValue("@ChildName", childName.ToLower());

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new ModelNotFoundException(
                            $"Couldn't find {childType.ToLower()} '{childName}' in {parentType.ToLower()} '{parentName}'",
                            false);
                    }

                    int childId = reader.GetInt32(0);
                    return childId;
                }
            }
        }

        // Searches for the parent, and if it exists, returns its id.
        // Performs the existence check for the parent.
        // This method should be used in Creates where the child does not yet exist and requires the parent's id.
        private static int GetParentId(SqlConnection db, string parentType, string parentName)
        {
            // Parameterized query to prevent SQL injection
            var query = $"SELECT Id FROM {parentType} WHERE Name=@ParentName";

            using (var cmd = new SqlCommand(query, db))
            {
                cmd.Parameters.AddWithValue("@ParentName", parentName.ToLower());

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new ModelNotFoundException($"Couldn't find {parentType.ToLower()} e '{parentName}'", false);
                    }

                    int parentId = reader.GetInt32(0);
                    return parentId;
                }
            }
        }

        #endregion

        #region Application

        //Get all applications
        public static List<string> GetApplications()
        {
            List<string> data = new List<string> ();

            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                string query = "SELECT * FROM Application";
                using (SqlCommand command = new SqlCommand(query, db))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //int id = reader.GetInt32(reader.GetOrdinal("Id"));
                            string name = reader.GetString(reader.GetOrdinal("Name"));
                            //var time = reader.GetDateTime(reader.GetOrdinal("CreationDate"));

                            //data.Add(new Application(id,name,time));

                            data.Add(name);
                        }
                        reader.Close();
                    }
                }
                return data;
            }
        }

        public static Application GetApplication(string Appname)
        {
            using (var connection = new DbConnection())
            {
                var db = connection.Open();
                string query = "SELECT * FROM Application WHERE Name=@Name";
                using(SqlCommand command = new SqlCommand(query,db))
                {
                    command.Parameters.AddWithValue("@Name", Appname);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id = reader.GetInt32(reader.GetOrdinal("Id"));
                            string name = reader.GetString(reader.GetOrdinal("Name"));
                            var time = reader.GetDateTime(reader.GetOrdinal("CreationDate"));

                            return new Application(id,name,time);
                        }
                        return null;
                    }
                }
                
            }
        }
        public static void CreateApplication(string name)
        {
            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                string query = "INSERT INTO Application (Name, CreationDate) VALUES (@AppName, @CreationDate)";
                using (SqlCommand command = new SqlCommand(query, db))
                {
                    command.Parameters.AddWithValue("@AppName", name.ToLower());
                    command.Parameters.AddWithValue("@CreationDate", DateTime.Now);

                    // Execute the insert command
                    command.ExecuteNonQuery();

                    // Retrieve the last identity value for the Application table
                    string identityQuery = "SELECT IDENT_CURRENT('Application')";
                    SqlCommand identityCommand = new SqlCommand(identityQuery, db);
                    int newApplicationId = Convert.ToInt32(identityCommand.ExecuteScalar());
                }
            }
        }

        public static void UpdateApplication(string name, string newName)
        {
            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                string query = "UPDATE Application SET Name = @NewName WHERE Name = @Name";
                using (SqlCommand command = new SqlCommand(query, db))
                {
                    command.Parameters.AddWithValue("@Name", name.ToLower());
                    command.Parameters.AddWithValue("@NewName", newName.ToLower());
                    

                    command.ExecuteNonQuery();
                }
            }
        }


        public static bool DeleteApplication(string name)
        {
            using (var connection = new DbConnection())
            {
                var db = connection.Open();


                // Check if the application exists
                string checkQuery = "SELECT * FROM Container c JOIN Application a ON (c.Parent = a.Id) WHERE a.Name=@Name";
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, db))
                {
                    checkCommand.Parameters.AddWithValue("@Name", name.ToLower());
                    var reader = checkCommand.ExecuteReader();

                    if (reader.Read())
                    {
                        throw new Exception("Can not delete an application with containers");
                    }

                    reader.Close();

                    
                }

                // Delete the application
                string deleteQuery = "DELETE FROM Application WHERE Name = @Name";
                using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, db))
                {
                    deleteCommand.Parameters.AddWithValue("@Name", name.ToLower());
                    int rowsAffected = deleteCommand.ExecuteNonQuery();
                    if (rowsAffected != 1)
                        throw new ModelNotFoundException("Application");

                    return true;
                }
            }
        }


        #endregion

        #region Container

        private static int IsContainerParentValid(SqlConnection db, string appName, string ContainerName)
        {
            return IsParentValid(db, "Application", appName, "Container", ContainerName);
        }

        private static void ProcessSqlExceptionContainer(SqlException e)
        {
            switch (e.Number)
            {
                // Cannot insert duplicate key in object
                case 2627:
                    throw new UnprocessableEntityException("A container with that name already exists in that application");
                default:
                    throw new UntreatedSqlException(e);
            }
        }

        public static List<string> GetContainers(string appName)
        {
            var containers = new List<string>();

            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                string query = "SELECT * FROM Container c INNER JOIN Application a ON(c.Parent = a.Id) WHERE a.Name = @AppName";
                using (SqlCommand command = new SqlCommand(query, db))
                {
                    command.Parameters.AddWithValue("@AppName", appName.ToLower());

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //int id = reader.GetInt32(reader.GetOrdinal("Id"));
                            string name = reader.GetString(reader.GetOrdinal("Name"));
                            //var time = reader.GetDateTime(reader.GetOrdinal("CreationDate"));
                            //int parentid = reader.GetInt32(reader.GetOrdinal("Parent"));

                            containers.Add(name);
                        }
                        reader.Close();
                    }
                }
                return containers;
            }
        }

        public static Container GetContainer(string appName,string containerName)
        {
            using (var connection = new DbConnection())
            {
                var db = connection.Open();


                //verifica se o id da app coincide com o parent no container
                var containerId = IsContainerParentValid(db,appName,containerName);

                string query = "SELECT * FROM Container WHERE Id=@Id";

               
                using (SqlCommand command = new SqlCommand(query, db))
                {
                    command.Parameters.AddWithValue("@Id", containerId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id = reader.GetInt32(reader.GetOrdinal("Id"));
                            string name = reader.GetString(reader.GetOrdinal("Name"));
                            var time = reader.GetDateTime(reader.GetOrdinal("CreationDate"));
                            int parentid = reader.GetInt32(reader.GetOrdinal("Parent"));

                            //falta aqui a linha que vai buscar a data relativa a este container

                            return new Container(id, name, time, parentid);
                        }
                        else
                        {
                            throw new UntreatedSqlException();
                        }
                    }
                }

            }
        }

        public static void CreateContainer(string containername, string appName)
        {
            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                int parentID = GetParentId(db, "Application", appName);

                string query = "INSERT INTO Container (Name, CreationDate, Parent) VALUES (@Name, @CreationDate, @Parent);";
                using (SqlCommand command = new SqlCommand(query, db))
                {
                    command.Parameters.AddWithValue("@Name", containername.ToLower());
                    command.Parameters.AddWithValue("@CreationDate", DateTime.Now);
                    command.Parameters.AddWithValue("@Parent", parentID);

                    try
                    {
                        int rowChng = command.ExecuteNonQuery();

                        if (rowChng != 1)
                            throw new UntreatedSqlException();
                    }
                    catch (SqlException e)
                    {

                        ProcessSqlExceptionContainer(e);
                    }
                }
            }
        }

        public static void UpdateContainer(string appName, string containerName, string newName)
        {
            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                IsContainerParentValid(db, appName, containerName);

                string query = "UPDATE Container SET Name = @NewName WHERE Name = @Name";
                using (SqlCommand command = new SqlCommand(query, db))
                {
                    command.Parameters.AddWithValue("@Name", containerName.ToLower());
                    command.Parameters.AddWithValue("@NewName", newName);

                    try
                    {
                        int rowChng = command.ExecuteNonQuery();

                        if (rowChng != 1)
                            throw new ModelNotFoundException("Container");
                    }
                    catch (SqlException e)
                    {
                        ProcessSqlExceptionContainer(e);
                    }
                }
            }
        }

        public static bool DeleteContainer(string appName, string containerName)
        {
            List<int> subsIds = new List<int>();
            List<int> dataIds = new List<int>();

            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                IsContainerParentValid(db, appName, containerName);

                string checkQuery = "SELECT s.Id FROM Container m JOIN Subscription s ON (m.Id = s.Parent) WHERE m.Name=@Name";
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, db))
                {
                    checkCommand.Parameters.AddWithValue("@Name", containerName.ToLower());
                    var reader = checkCommand.ExecuteReader();

                    while (reader.Read())
                        subsIds.Add(reader.GetInt32(0));

                    reader.Close();
                }

                checkQuery = "SELECT d.Id FROM Container m JOIN Data d ON (m.Id = d.Parent) WHERE m.Name=@Name";
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, db))
                {
                    checkCommand.Parameters.AddWithValue("@Name", containerName.ToLower());
                    var reader = checkCommand.ExecuteReader();

                    while (reader.Read())
                        dataIds.Add(reader.GetInt32(0));

                    reader.Close();
                }

                foreach (int id in subsIds)
                {
                    string deleteQuery = "DELETE FROM Subscription WHERE Id=@Id";
                    using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, db))
                    {
                        deleteCommand.Parameters.AddWithValue("@Id", id);

                        int rowsAffected = deleteCommand.ExecuteNonQuery();
                        
                    }
                }

                foreach (int id in dataIds)
                {
                    string deleteQuery = "DELETE FROM Data WHERE Id=@Id";
                    using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, db))
                    {
                        deleteCommand.Parameters.AddWithValue("@Id", id);

                        int rowsAffected = deleteCommand.ExecuteNonQuery();
                        
                    }
                }

                string deleteQueryContainer = "DELETE FROM Container WHERE Name=@Name";
                using (SqlCommand deleteCommand = new SqlCommand(deleteQueryContainer, db))
                {
                    deleteCommand.Parameters.AddWithValue("@Name", containerName);

                    int rowsAffected = deleteCommand.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    

        #endregion

        #region Data

        private static void NotifySubscriptions(SqlConnection db, int parentId, string containerName, string eventType, string data)
        {
            try
            {
                var notification = new Notification(eventType, data);

                var cmdText = "SELECT * FROM Subscription WHERE Parent=@Parent AND (Event=@Event OR Event='BOTH')";
                using (var cmd = new SqlCommand(cmdText, db))
                {
                    cmd.Parameters.AddWithValue("@Parent", parentId);
                    cmd.Parameters.AddWithValue("@Event", eventType.ToUpper());

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BrokerHelper.FireNotification(reader.GetString(5), containerName, notification);
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                throw new BrokerException("An unknown database error (#" + e.Number + ") has happened while trying to notify subscriptions", e);
            }
        }

        public static List<string> GetDatas(string appName, string containerName)
        {
            var datas = new List<string>();

            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                int parentId = IsContainerParentValid(db, appName, containerName);

                string query = "SELECT * FROM Data d JOIN Container c ON d.Parent = c.Id JOIN Application a ON c.Parent = a.Id WHERE a.Name = @appName AND c.Name = @containerName";

                using (SqlCommand command = new SqlCommand(query, db))
                {
                    command.Parameters.AddWithValue("@appName", appName);
                    command.Parameters.AddWithValue("@containerName", containerName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader.GetString(reader.GetOrdinal("Name"));
                            
                            datas.Add(name);
                        }
                        reader.Close();
                    }
                }
                return datas;
            }
        }

        public static Data GetData(string appName, string containerName, string dataName)
        {
            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                int parentId = IsContainerParentValid(db, appName, containerName);

                string query = "SELECT * FROM Data WHERE Id=@Id";

                using (SqlCommand command = new SqlCommand(query, db))
                {
                    command.Parameters.AddWithValue("@Parent", parentId);
                    command.Parameters.AddWithValue("@Name", dataName.ToLower());

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id = reader.GetInt32(reader.GetOrdinal("Id"));
                            string name = reader.GetString(reader.GetOrdinal("Name"));
                            string content = reader.GetString(reader.GetOrdinal("Content"));
                            DateTime creationdate = reader.GetDateTime(reader.GetOrdinal("CreationDate"));
                            int parentid = reader.GetInt32(reader.GetOrdinal("Parent"));

                            return new Data(id, name, content, creationdate, parentid);
                        }
                        return null;
                    }
                }
            }
        }

        public static void CreateData(string appName, string containerName, string dataContent, string dataName)
        {
            using (var dbConn = new DbConnection())
            {
                var db = dbConn.Open();
                try
                {
                    int parentId = IsContainerParentValid(db, appName, containerName);

                    var cmdText = "INSERT INTO Data (Name, Content, CreationDate, Parent) VALUES (@Name, @Content, @CreationDate, @Parent)";
                    using (var cmd = new SqlCommand(cmdText, db))
                    {
                        cmd.Parameters.AddWithValue("@Name", dataName.ToLower());
                        cmd.Parameters.AddWithValue("@Content", dataContent);
                        cmd.Parameters.AddWithValue("@CreationDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Parent", parentId);

                        int rowChng = cmd.ExecuteNonQuery();

                        if (rowChng != 1)
                            throw new UntreatedSqlException();

                        NotifySubscriptions(db, parentId, containerName, "CREATE", dataContent);
                    }

                }
                catch (SqlException)
                {
                    throw new UntreatedSqlException();
                }
            }
        }

        public static void DeleteData(string appName, string containerName, string dataName)
        {
            using (var dbConn = new DbConnection())
            using (var db = dbConn.Open())
            using (var transaction = db.BeginTransaction())
            {
                try
                {
                    int parentId = IsContainerParentValid(db, appName, containerName);

                    var cmdText = "SELECT c.Id, d.Id, d.Content FROM Container c JOIN Data d ON (d.Parent = c.Id) WHERE d.Name=@DataName AND c.Name=@containerName";
                    string dataContent;

                    using (var cmd = new SqlCommand(cmdText, db, transaction))
                    {
                        cmd.Parameters.AddWithValue("@DataName", dataName);
                        cmd.Parameters.AddWithValue("@containerName", containerName.ToLower());

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                throw new ModelNotFoundException("A data resource with the Name #" + dataName + " does not exist in the container " + containerName, false);


                            dataContent = reader.GetString(2);
                        }
                    }

                    using (var deleteCmd = new SqlCommand("DELETE FROM Data WHERE Id=@Id", db, transaction))
                    {
                        deleteCmd.Parameters.AddWithValue("@Id", dataName);

                        int rowChng = deleteCmd.ExecuteNonQuery();
                        if (rowChng != 1)
                            throw new UntreatedSqlException();

                        NotifySubscriptions(db, parentId, containerName, "DELETE", dataContent);
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        #endregion

        #region Subscription

        private static void ProcessSqlExceptionSubscription(SqlException e)
        {
            switch (e.Number)
            {
                // Cannot insert duplicate key in object
                case 2627:
                    throw new UnprocessableEntityException("A subscription with that name already exists in that container");
                default:
                    throw new UntreatedSqlException(e);
            }
        }

        public static List<string> GetSubscriptions(string appName, string containerName)
        {
            var subscriptions = new List<string>();

            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                int parentId = IsContainerParentValid(db, appName, containerName);

                string query = "SELECT * FROM Subscription s JOIN Container c ON (s.Parent = c.Id) WHERE c.Name=@containerName";

                using (SqlCommand command = new SqlCommand(query, db))
                {
                    command.Parameters.AddWithValue("@Parent", parentId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //int id = reader.GetInt32(reader.GetOrdinal("Id"));
                            string name = reader.GetString(reader.GetOrdinal("Name"));
                            //var time = reader.GetDateTime(reader.GetOrdinal("CreationDate"));
                            //int parentid = reader.GetInt32(reader.GetOrdinal("Parent"));
                            //string @event = reader.GetString(reader.GetOrdinal("Event"));
                            //string endpoint = reader.GetString(reader.GetOrdinal("Endpoint"));

                            //subscriptions.Add(new Subscription(id, name, time, parentid, @event, endpoint));
                            subscriptions.Add(name);
                        }
                        reader.Close();
                    }
                }
                return subscriptions;
            }
        }

        public static Subscription GetSubscription(string appName, string containerName, string subscriptionName)
        {
            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                int parentId = IsContainerParentValid(db, appName, containerName);

                string query = "SELECT * FROM Subscription WHERE Id=@Id";

                using (SqlCommand command = new SqlCommand(query, db))
                {
                    command.Parameters.AddWithValue("@Parent", parentId);
                    command.Parameters.AddWithValue("@Name", subscriptionName.ToLower());

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id = reader.GetInt32(reader.GetOrdinal("Id"));
                            string name = reader.GetString(reader.GetOrdinal("Name"));
                            var time = reader.GetDateTime(reader.GetOrdinal("CreationDate"));
                            int parentid = reader.GetInt32(reader.GetOrdinal("Parent"));
                            string @event = reader.GetString(reader.GetOrdinal("Event"));
                            string endpoint = reader.GetString(reader.GetOrdinal("Endpoint"));

                            return new Subscription(id, name, time, parentid, @event, endpoint);
                        }
                        return null;
                    }
                }
            }
        }

        public static void CreateSubscription(string appName, string containerName, Subscription subscription)
        {
            using (var dbConn = new DbConnection())
            {
                var db = dbConn.Open();

                int parentId = IsContainerParentValid(db, appName, containerName);

                var cmd = new SqlCommand(
                    "INSERT INTO Subscription (Name, CreationDate, Parent, Event, Endpoint) VALUES (@Name, @CreationDate, @Parent, @Event, @Endpoint)",
                    db);
                cmd.Parameters.AddWithValue("@Name", subscription.Name.ToLower());
                cmd.Parameters.AddWithValue("@CreationDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@Parent", parentId);
                cmd.Parameters.AddWithValue("@Event", subscription.Event.ToUpper());
                cmd.Parameters.AddWithValue("@Endpoint", subscription.Endpoint.ToLower());

                try
                {
                    int rowChng = cmd.ExecuteNonQuery();

                    if (rowChng != 1)
                        throw new UntreatedSqlException();
                }
                catch (SqlException e)
                {
                    ProcessSqlExceptionSubscription(e);
                }
            }
        }

        public static void DeleteSubscription(string appName, string containerName, string subscriptionName)
        {
            using (var dbConn = new DbConnection())
            {
                var db = dbConn.Open();

                IsContainerParentValid(db, appName, containerName);

                int parentId = GetParentId(db, "Container", containerName);

                var cmd = new SqlCommand("DELETE FROM Subscription WHERE Name=@Name AND Parent=@Parent", db);
                cmd.Parameters.AddWithValue("@Name", subscriptionName.ToLower());
                cmd.Parameters.AddWithValue("@Parent", parentId);
                int rowChng = cmd.ExecuteNonQuery();

                if (rowChng != 1)
                    throw new ModelNotFoundException("Subscription");
            }
        }

        #endregion

        private class DbConnection : IDisposable
        {
            private readonly string _connStr = Settings.Default.ConnStr;
            private readonly SqlConnection _conn;
            
            public DbConnection()
            {
                _conn = new SqlConnection(_connStr);
            }

            public SqlConnection Open()
            {
                _conn.Open();
                return _conn;
            }

            public void Dispose()
            {
                _conn.Close();
            }
            
        }

    }

}