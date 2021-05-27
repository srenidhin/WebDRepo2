using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace WebApplication17.Models
{
    public class File
    {
        public string Filename { get; set; }
        public string FilePath { get; set; }
    }
    public class Files
    {
        public List<File> GetRoles()
        {
            List<File> Locations = new List<File>();
            string conn = "server=LAPTOP-2A1SL8A9\\SQLSERVER;Integrated security=true;database=Nidhin";
            SqlConnection cn = new SqlConnection(conn);
            cn.Open();
            String que = "Select * from FileTable";
            SqlCommand cmd = new SqlCommand(que, cn);
            SqlDataReader dr = cmd.ExecuteReader();
            while(dr.Read())
            {
                File ur = new File();
                ur.Filename = dr[0].ToString();
                ur.FilePath = dr[1].ToString();
                ur.Filename = ur.Filename.TrimEnd(' ');
                ur.FilePath = ur.FilePath.TrimEnd(' ');
                Locations.Add(ur);
            }
            cn.Close();
            return Locations;
        }
    }
}