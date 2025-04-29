using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Drawing;

namespace WebDatDoAnOnline
{
    public class ConnectionSQL
    {
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["cs"].ConnectionString;
        }
    }

    public class DashboardCount
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader sdr;
        public int Count(string tableName)
        {
            int count = 0;

            con = new SqlConnection(ConnectionSQL.GetConnectionString());
            cmd = new SqlCommand("Dashboard", con);
            cmd.Parameters.AddWithValue("@Action", tableName);
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();
            sdr = cmd.ExecuteReader();

            while (sdr.Read())
            {
                if (sdr[0] == DBNull.Value)
                {
                    count = 0;
                }
                else
                {
                    count = Convert.ToInt32(sdr[0]);
                }
            }

            sdr.Close();
            con.Close();

            return count;
        }
    }

}