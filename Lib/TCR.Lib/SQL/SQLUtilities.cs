using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TCR.Lib.SQL
{
    public class SqlUtilities
    {
        public static string ReadResourceScript(Assembly assembly, string path)
        {
            string result = string.Empty;

            using (var stream = assembly.GetManifestResourceStream(path))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            return result;
        }

        public static DataSet FetchData(string connectionString, string sqlCommand, List<SqlParameter> parameters)
        {
            DataSet result = new DataSet();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                try
                {
                    result = FetchData(conn, sqlCommand, parameters);
                }
                finally
                {
                    conn.Close();
                }
            }
            return result;
        }

        public static DataSet FetchData(SqlConnection connection, string sqlCommand, List<SqlParameter> parameters)
        {
            DataSet result = new DataSet();

            using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
            {
                cmd.CommandTimeout = 600;
                foreach (var parm in parameters)
                    cmd.Parameters.Add(parm);
                using (SqlDataAdapter adaptor = new SqlDataAdapter(cmd))
                {
                    adaptor.Fill(result);
                }
            }
            return result;
        }

        public static void ExecuteCommand(string connectionString, string sqlCommand, List<SqlParameter> parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                try
                {
                    ExecuteCommand(conn, sqlCommand, parameters);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static void ExecuteCommand(SqlConnection connection, string sqlCommand, List<SqlParameter> parameters, SqlTransaction transaction = null)
        {
            using (SqlCommand cmd = new SqlCommand(sqlCommand, connection, transaction))
            {
                cmd.CommandTimeout = 1800;  // 30 min
                foreach (var parm in parameters)
                    cmd.Parameters.Add(parm);
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateTableStatistics(SqlConnection connection, string sqlTableName)
        {
            using (SqlCommand cmd = new SqlCommand("UPDATE STATISTICS " + sqlTableName, connection))
            {
                cmd.CommandTimeout = 1200; //20 minutes
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateAllStatistics(SqlConnection connection)
        {
            using (SqlCommand cmd = new SqlCommand("EXEC sp_updatestats", connection))
            {
                cmd.CommandTimeout = 1200; //20 minutes
                cmd.ExecuteNonQuery();
            }
        }
    }
}
