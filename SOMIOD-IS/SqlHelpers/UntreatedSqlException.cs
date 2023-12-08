using System;
using System.Runtime.Serialization;
using System.Data.SqlClient;

namespace SOMIOD_IS.SqlHelpers
{
    [Serializable]
    internal class UntreatedSqlException : Exception
    {
        public UntreatedSqlException() : base("Erro desconhecido na database")
        {
        }

        public UntreatedSqlException(SqlException e) : base("Database erro nao tratado (#" + e.Number + ")")
        {
        }

    }
}