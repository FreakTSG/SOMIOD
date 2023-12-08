using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using SOMIOD_IS.Models;
using SOMIOD_IS.Properties;

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
                        throw new ModelNotFoundException($"Couldn't find {parentType.ToLower()} '{parentName}'", false);
                    }

                    int parentId = reader.GetInt32(0);
                    return parentId;
                }
            }
        }

        #endregion

        #region Application

        //Get all applications
        public static List<Application> GetApplications()
        {
            List<Application> data = new List<Application> ();

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
                            int id = reader.GetInt32(reader.GetOrdinal("Id"));
                            string name = reader.GetString(reader.GetOrdinal("Name"));
                            var time = reader.GetDateTime(reader.GetOrdinal("CreationDate"));

                            data.Add(new Application(id,name,time));
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
                    command.Parameters.AddWithValue("@NewName", newName);
                    

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

        public static List<Container> GetContainers(string appName)
        {
            List<Container> data = new List<Container>();

            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                string query = "SELECT * FROM Container c JOIN Application a ON (c.Parent = a.Id) WHERE a.Name=@AppName";
                using (SqlCommand command = new SqlCommand(query, db))
                {


                    command.Parameters.AddWithValue("@AppName", appName.ToLower());

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(reader.GetOrdinal("Id"));
                            string name = reader.GetString(reader.GetOrdinal("Name"));
                            var time = reader.GetDateTime(reader.GetOrdinal("CreationDate"));
                            int parentid = reader.GetInt32(reader.GetOrdinal("Parent"));

                            data.Add(new Container(id, name, time, parentid));
                        }
                        reader.Close();
                    }
                }
                return data;
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
                        return null;
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

                string query = "INSERT INTO Container (Name, CreationDate, Parent) VALUES (@ContainerName, @CreationDate, @Parent); SELECT SCOPE_IDENTITY();";
                using (SqlCommand command = new SqlCommand(query, db))
                {
                    command.Parameters.AddWithValue("@Name", containername.ToLower());
                    command.Parameters.AddWithValue("@CreationDate", DateTime.Now);
                    command.Parameters.AddWithValue("@Parent", parentID);

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateContainer(string name, string newName)
        {
            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                string query = "UPDATE Container SET Name = @NewName WHERE Name = @Name";
                using (SqlCommand command = new SqlCommand(query, db))
                {
                    command.Parameters.AddWithValue("@Name", name.ToLower());
                    command.Parameters.AddWithValue("@NewName", newName);

                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool DeleteContainer(int containerId)
        {
            using (var connection = new DbConnection())
            {
                var db = connection.Open();

                string checkQuery = "SELECT COUNT(1) FROM Container WHERE Id = @Id";
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, db))
                {
                    checkCommand.Parameters.AddWithValue("@Id", containerId);

                    int exists = (int)checkCommand.ExecuteScalar();
                    if (exists == 0)
                    {
                        return false;
                    }
                }
 
                string deleteQuery = "DELETE FROM Container WHERE Id = @Id";
                using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, db))
                {
                    deleteCommand.Parameters.AddWithValue("@Id", containerId);

                    int rowsAffected = deleteCommand.ExecuteNonQuery();
                    return rowsAffected > 0; 
                }
            }
        }

        #endregion

        #region Data

        private static List<Data> GetDataResourcesForContainer(int parentId)
        {
            var dataRes = new List<Data>();

            using (var dbConn = new DbConnection())
            using (var db = dbConn.Open())
            {
                var cmdText = "SELECT * FROM Data WHERE Parent=@Parent";
                using (var cmd = new SqlCommand(cmdText, db))
                {
                    cmd.Parameters.AddWithValue("@Parent", parentId);

                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dataRes.Add(new Data(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3), reader.GetInt32(4)));
                            }
                        }
                    }
                    catch (SqlException e)
                    {
                        // Handle or log the exception as needed
                        throw new ApplicationException("Database operation failed", e);
                    }
                }
            }

            return dataRes;
        }

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

        public static void CreateData(string appName, string containerName, string dataContent)
        {
            using (var dbConn = new DbConnection())
            using (var db = dbConn.Open())
            using (var transaction = db.BeginTransaction())
            {
                try
                {
                    int parentId = IsContainerParentValid(db, appName, containerName);

                    var cmdText = "INSERT INTO Data (Name, Content, CreationDate, Parent) VALUES (@Name, @Content, @CreationDate, @Parent)";
                    using (var cmd = new SqlCommand(cmdText, db, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Name", containerName.ToLower());
                        cmd.Parameters.AddWithValue("@Content", dataContent);
                        cmd.Parameters.AddWithValue("@CreationDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Parent", parentId);

                        int rowChng = cmd.ExecuteNonQuery();

                        if (rowChng != 1)
                            throw new UntreatedSqlException();

                        NotifySubscriptions(db, parentId, containerName, "CREATE", dataContent);
                    }

                    transaction.Commit();
                }
                catch (SqlException)
                {
                    transaction.Rollback();
                    throw new UntreatedSqlException();
                }
            }
        }

        public static void DeleteData(string appName, string containerName, int dataId)
        {
            using (var dbConn = new DbConnection())
            using (var db = dbConn.Open())
            using (var transaction = db.BeginTransaction())
            {
                try
                {
                    int parentId = IsContainerParentValid(db, appName, containerName);

                    var cmdText = "SELECT c.Id, d.Id, d.Content FROM Container c JOIN Data d ON (d.Parent = c.Id) WHERE d.Id=@DataId AND c.Name=@containerName";
                    string dataContent;

                    using (var cmd = new SqlCommand(cmdText, db, transaction))
                    {
                        cmd.Parameters.AddWithValue("@DataId", dataId);
                        cmd.Parameters.AddWithValue("@containerName", containerName.ToLower());

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                throw new ModelNotFoundException("A data resource with the Id #" + dataId + " does not exist in the container " + containerName, false);

                            dataContent = reader.GetString(2);
                        }
                    }

                    using (var deleteCmd = new SqlCommand("DELETE FROM Data WHERE Id=@Id", db, transaction))
                    {
                        deleteCmd.Parameters.AddWithValue("@Id", dataId);

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