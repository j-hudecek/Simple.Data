﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Simple.Data.SqlTest
{
    internal static class DatabaseHelper
    {
        public static readonly string ConnectionString = GetConnectionString();

        private static string GetConnectionString()
        {
#if(MONO)
			return "Data Source=10.37.129.4;Initial Catalog=SimpleTest;User ID=SimpleUser;Password=SimplePassword";
#else
            return Environment.GetEnvironmentVariable("SIMPLETESTDB") ?? "Data Source=localhost;Initial Catalog=SimpleTest;Integrated Security=True";
#endif
        }
		
        public static dynamic Open()
        {
            return Database.Opener.OpenConnection(ConnectionString);
        }

        public static void Reset()
        {
            try
            {
                var provider = new SqlServer.SqlConnectionProvider();
                using (var cn = new SqlConnection(ConnectionString))
                {
                    cn.Open();
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "TestReset";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
