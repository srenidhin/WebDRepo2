using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using WebApplication17.Models;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;

namespace WebApplication11.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username,string password)
        {
            string conn = "server=LAPTOP-2A1SL8A9\\SQLSERVER;Integrated security=true;database=Nidhin";
            SqlConnection cn = new SqlConnection(conn);
            cn.Open();
            String que = "Select password from UserTable where username='{0}'";
            que = String.Format(que, username);
            SqlCommand cmd = new SqlCommand(que, cn);
            SqlDataReader dr = cmd.ExecuteReader();
            ViewBag.Status = string.Empty;
            if (dr.Read())
            {
                string pass = dr[0].ToString();
                pass = pass.TrimEnd(' ');
                if (password == pass)
                {
                    Session["username"] = username;
                    return RedirectToAction("DashBoard");
                }
            }
            ViewBag.Status = "Invalid Credentials";
            return View();
        }
        public ActionResult UR()
        {
            DataTable dt1 = new DataTable();
            DataTable dt = new DataTable();
            string conn = "server=LAPTOP-2A1SL8A9\\SQLSERVER;Integrated security=true;database=Nidhin";
            SqlConnection cn = new SqlConnection(conn);
            cn.Open();
            String que = "Select * from UserTable";
            SqlCommand cmd = new SqlCommand(que, cn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            ViewBag.Data = dt;
            que = "Select * from UserDetails";
            SqlCommand cmd1 = new SqlCommand(que, cn);
            SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
            da1.Fill(dt1);
            ViewBag.Data1 = dt1;
            return View();
        }
        public ActionResult FR()
        {
            DataTable dt = new DataTable();
            string conn = "server=LAPTOP-2A1SL8A9\\SQLSERVER;Integrated security=true;database=Nidhin";
            SqlConnection cn = new SqlConnection(conn);
            cn.Open();
            String que = "Select * from FileTable";
            SqlCommand cmd = new SqlCommand(que, cn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            ViewBag.Data = dt;
            return View();
        }
        public ActionResult UserMan()
        {
            DataTable dt = new DataTable();
            string conn = "server=LAPTOP-2A1SL8A9\\SQLSERVER;Integrated security=true;database=Nidhin";
            SqlConnection cn = new SqlConnection(conn);
            cn.Open();
            String que = "Select * from UserTable";
            SqlCommand cmd = new SqlCommand(que, cn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            ViewBag.Data = dt;
            return View();
        }
        public ActionResult deleteUM(string un)
        {
            DataTable dt = new DataTable();
            string conn = "server=LAPTOP-2A1SL8A9\\SQLSERVER;Integrated security=true;database=Nidhin";
            SqlConnection cn = new SqlConnection(conn);
            cn.Open();
            String que = "delete from UserTable where username='{0}'";
            que = String.Format(que, un);
            SqlCommand cmd = new SqlCommand(que, cn);
            int i = cmd.ExecuteNonQuery();
            que = "delete from UserDetails where username='{0}'";
            que = String.Format(que, un);
            cmd = new SqlCommand(que, cn);
            i = cmd.ExecuteNonQuery();
            cn.Close();
            return RedirectToAction("UserMan","Home");
        }
        public ActionResult DashBoard()
        {
            int[] a=new int[12];
            string conn = "server=LAPTOP-2A1SL8A9\\SQLSERVER;Integrated security=true;database=Nidhin";
            SqlConnection cn = new SqlConnection(conn);
            cn.Open();
            String que = "Select count(*) from FileTable";
            SqlCommand cmd = new SqlCommand(que, cn);
            SqlDataReader dr = cmd.ExecuteReader();
            if(dr.Read())
            {
                ViewBag.Val1 = dr[0].ToString();
            }
            dr.Close();
            que = "Select count(*) from FileTable where [Created By] = '{0}'";
            que = String.Format(que, Session["username"]);
            cmd = new SqlCommand(que, cn);
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                ViewBag.Val2 = dr[0].ToString();
            }
            dr.Close();
            que = "Select count(*) from UserTable";
            cmd = new SqlCommand(que, cn);
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                ViewBag.Val3 = dr[0].ToString();
            }
            dr.Close();
            que = "Select count(*) from UserTable where status='active'";
            cmd = new SqlCommand(que, cn);
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                ViewBag.Val4 = dr[0].ToString();
            }
            dr.Close();
            que = "Select count(*) from UserTable where status='deactivated'";
            cmd = new SqlCommand(que, cn);
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                ViewBag.Val5 = dr[0].ToString();
            }
            dr.Close();
            que = "chart";
            cmd = new SqlCommand(que, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            dr = cmd.ExecuteReader();
            while(dr.Read())
            {
                int x = Convert.ToInt32(dr[0]);
                a[x-1] = Convert.ToInt32(dr[1]);
            }
            string ad = "";
            for (int i = 0; i < 12; i++)
                ad += a[i] + ",";
            ad = ad.TrimEnd(',');
            ViewBag.chart2 = ad;
            return View();
        }
        public void EditUserMan(string head,string sal)
        {
            string[] y = head.Split('$');
            string[] x = sal.Split('$');
            string conn = "server=LAPTOP-2A1SL8A9\\SQLSERVER;Integrated security=true;database=Nidhin";
            SqlConnection cn = new SqlConnection(conn);
            cn.Open();
            string que = "update UserTable set {0} where {1}";
            string param = "";
            string cond = "";
            for(int i=1;i<y.Length-1;i++)
            {
                string temp = y[i] + "='" + x[i] + "',";
                param += temp;
            }
            param = param.TrimEnd(',');
            cond = y[0] + "='" + x[0] + "'";
            que = String.Format(que, param, cond);
            SqlCommand cmd = new SqlCommand(que, cn);
            int ai = cmd.ExecuteNonQuery();
            string yy = Session["username"].ToString();
            que = "Update UserDetails set [Modified By]='"+yy+ "',[Modified At]=getdate() where [Username]='"+x[0]+"'";
            SqlCommand cmd1 = new SqlCommand(que, cn);
            int ai2 = cmd1.ExecuteNonQuery();
        }
        public void AddUserMan(string sal)
        {
            string[] x = sal.Split('$');
            string conn = "server=LAPTOP-2A1SL8A9\\SQLSERVER;Integrated security=true;database=Nidhin";
            SqlConnection cn = new SqlConnection(conn);
            cn.Open();
            string que = "Insert into UserTable values({0})";
            string param = "";
            for (int i = 0; i < x.Length - 1; i++)
            {
                string temp ="'" + x[i] + "',";
                param += temp;
            }
            param = param.TrimEnd(',');
            que = String.Format(que, param);
            SqlCommand cmd = new SqlCommand(que, cn);
            int ai = cmd.ExecuteNonQuery();
            string yy = Session["username"].ToString();
            que = "Insert into UserDetails values('"+x[0]+"','"+yy +"',Getdate(),'"+Session["username"]+"',Getdate())";
            SqlCommand cmd1 = new SqlCommand(que, cn);
            int ai2 = cmd1.ExecuteNonQuery();
        }
        public ActionResult PropertiesModal()
        {
            return View();
        }
        public ActionResult AddFile()
        {
            return View("AddFile");
        }
        private IEnumerable<SelectListItem> GetRoles()
        {
            var files = new Files();
            var roles = files.GetRoles().Select(x => new SelectListItem{
                                                        Value = x.FilePath,
                                                        Text = x.Filename
                                                        });
            return new SelectList(roles, "Value", "Text");
        }

        public ActionResult EditTable()
        {
            var model = new FileNameViewModel
            {
                Files = GetRoles()
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult AddFile(HttpPostedFileBase postedFile)
        {
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Content/uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);
                string conString = string.Empty;
                switch (extension)
                {
                    case ".xls":
                        conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                        break;
                    case ".xlsx":
                        conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                        break;
                }

                conString = string.Format(conString, filePath);
                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            DataTable dt = new DataTable();
                            cmdExcel.Connection = connExcel;
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                            ViewBag.Data = dt;
                        }
                    }
                }
            }
            return View("AddFile");
        }
        
        [HttpPost]
        public ActionResult TablePartial(string fPath)
        {
            string conString = string.Empty;
            conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
            conString = string.Format(conString, fPath);
            using (OleDbConnection connExcel = new OleDbConnection(conString))
            {
                using (OleDbCommand cmdExcel = new OleDbCommand())
                {
                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                    {
                        DataTable dt = new DataTable();
                        cmdExcel.Connection = connExcel;
                        connExcel.Open();
                        DataTable dtExcelSchema;
                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        string sheetName = dtExcelSchema.Rows[1]["TABLE_NAME"].ToString();
                        connExcel.Close();
                        connExcel.Open();
                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                        odaExcel.SelectCommand = cmdExcel;
                        odaExcel.Fill(dt);
                        connExcel.Close();
                        ViewBag.Data = dt;
                    }
                }
            }
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            //List<CustomerModel> customers = new List<CustomerModel>();
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Content/uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);
                string conString = string.Empty;
                switch (extension)
                {
                    case ".xls":
                        conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                        break;
                    case ".xlsx":
                        conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                        break;
                }

                conString = string.Format(conString, filePath);
                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            DataTable dt = new DataTable();
                            cmdExcel.Connection = connExcel;
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                            ViewBag.Data = dt;
                        }
                    }
                }
            }
            return View();
        }
        public void Deleter(int row,string fPath)
        {
            Excel.Application xlApp = new Excel.Application();
            string x = modifier(fPath);
            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(x);
            Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            string password = "excel";
            xlWorkSheet.Unprotect(password);
            Excel.Range xlRange = xlWorkSheet.UsedRange;
            row += 2;
            Excel.Range range = xlWorkSheet.get_Range("A" + row);
            Excel.Range eR = range.EntireRow;
            eR.Delete(Excel.XlDirection.xlUp);
            xlApp.DisplayAlerts = false;
            xlWorkBook.Save();
            xlWorkBook.Close();
        }
        public void StatusChange(string us,string status)
        {
            string conn = "server=LAPTOP-2A1SL8A9\\SQLSERVER;Integrated security=true;database=Nidhin";
            SqlConnection cn = new SqlConnection(conn);
            cn.Open();
            string que = "update UserTable set status='{0}' where username='{1}'";
            que = String.Format(que, status,us);
            SqlCommand cmd = new SqlCommand(que, cn);
            int ai = cmd.ExecuteNonQuery();
            string yy = Session["username"].ToString();
            que = "Update UserDetails set [Modified By]='" + yy + "',[Modified At]=getdate() where [Username]='" + us + "'";
            SqlCommand cmd1 = new SqlCommand(que, cn);
            int ai2 = cmd1.ExecuteNonQuery();
        }
        public void Inserter(string vals,string fPath)
        {
            try
            {
                string[] rowers = vals.Split('$');
                string cmdText = "";
                for (int i = 0; i < rowers.Count() - 1; i++)
                {
                    cmdText += "'" + rowers[i] + "',";
                }
                string pathOfFileToCreate = fPath;
                string conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                conString = String.Format(conString, pathOfFileToCreate);
                OleDbConnection conn = new OleDbConnection(conString);
                OleDbCommand cmd = new OleDbCommand();
                conn.Open();
                cmd.Connection = conn;
                cmdText = cmdText.Substring(0, cmdText.Length - 1);
                cmd.CommandText = String.Format("INSERT INTO [Sheet1$] values({0})", cmdText);
                int pp = cmd.ExecuteNonQuery();
                conn.Close();
            }catch(Exception ae)
            {

            }
        }
        public string modifier(string name)
        {
            string x = "";
            string conn = "server=LAPTOP-2A1SL8A9\\SQLSERVER;Integrated security=true;database=Nidhin";
            SqlConnection cn = new SqlConnection(conn);
            cn.Open();
            String que = "Select [File Path] from FileTable where [File Name]='{0}'";
            que = String.Format(que, name);
            SqlCommand cmd = new SqlCommand(que, cn);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                x = dr[0].ToString();
            }
            dr.Close();
            que = "Update FileTable set [Last Modified By]='{0}',[Last Modified At]=GETDATE() where [File Name]='{1}'";
            que = String.Format(que, Session["username"],name);
            SqlCommand cmd1 = new SqlCommand(que, cn);
            int i = cmd1.ExecuteNonQuery();
            return x;
        }
        [HttpPost]
        public void Updater(string heads, string rows, string fPath)
        {
            fPath = fPath.TrimEnd('\n');
            string[] headers = heads.Split('$');
            string[] rowers = rows.Split('$');
            try
            {
                string val = string.Empty;
                for (int i=1;i<headers.Count()-1;i++)
                {
                    val += headers[i] + "='" + rowers[i] + "',";
                }
                val = val.Substring(0, val.Length - 1);
                val = val + " WHERE " + headers[0] + " = '" + rowers[0]+"'";            
                string pathOfFileToCreate = modifier(fPath);
                string conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                conString = String.Format(conString, pathOfFileToCreate);
                OleDbConnection conn = new OleDbConnection(conString);
                OleDbCommand cmd = new OleDbCommand();
                conn.Open();
                cmd.Connection = conn;
                string query = "Update [Sheet1$] set {0}";
                query = String.Format(query, val);
                cmd.CommandText = query;
                int pp = cmd.ExecuteNonQuery();
                if (pp == 0)
                {
                    conn.Close();
                    Inserter(rows, pathOfFileToCreate);
                }
                else
                    conn.Close();
            }catch(Exception ae)
            { }
        }
        [HttpPost]
        public void XWriter(string heads, string rows,string FileName)
        {
            string[] headers = heads.Split('$');
            string[] rowers = rows.Split('$');

            string createHeaders = string.Empty;
            for(int i=0;i<headers.Count()-1;i++)
            {
                createHeaders += headers[i] + " nvarchar(40),";
            }
            createHeaders = createHeaders.Substring(0, createHeaders.Length - 1);
            System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection();
            try
            {
                string pathOfFileToCreate = "\\\\LAPTOP-2A1SL8A9\\New folder\\{0}.xls";
                pathOfFileToCreate = String.Format(pathOfFileToCreate, FileName);
                string conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                conString = String.Format(conString, pathOfFileToCreate);
                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;
                            connExcel.Open();
                            var cmd = connExcel.CreateCommand();
                            string cmdText = "CREATE TABLE sheet1 ({0})";
                            cmdText = String.Format(cmdText, createHeaders);
                            cmd.CommandText = cmdText;
                            cmd.ExecuteNonQuery();
                            int i = headers.Count() - 1;
                            cmdText = "";
                            for(int j=0;j<rowers.Length;j++)
                            {
                                if (j % i == 0 && j != 0)
                                {
                                    cmdText = cmdText.Substring(0, cmdText.Length - 1);
                                    cmd.CommandText = String.Format("INSERT INTO sheet1 values({0})", cmdText);
                                    cmd.ExecuteNonQuery();
                                    cmdText = "";
                                }
                                cmdText += "'"+rowers[j]+"',";
                            }
                        }
                    }
                }
                string conn2 = "server=LAPTOP-2A1SL8A9\\SQLSERVER;Integrated security=true;database=Nidhin";
                SqlConnection cn = new SqlConnection(conn2);
                cn.Open();
                String que = "Insert into FileTable values ('{0}','{1}','{2}',GETDATE(),'{2}',GETDATE())";
                que = String.Format(que, FileName, pathOfFileToCreate,Session["username"]);
                SqlCommand cmd2 = new SqlCommand(que, cn);
                cmd2.ExecuteNonQuery();
                cn.Close();
            }
            catch(Exception ae)
            {
                //DataTable table = new DataTable();
                //int i;
                //for (i = 0; i < headers.Count() - 2; i++)
                //{
                //    table.Columns.Add(headers[i]);
                //}
                //DataRow dr = table.NewRow();
                //for (int j = 0; j < rowers.Count(); j++)
                //{
                //    if (j % i == 0 && j != 0)
                //    {
                //        table.Rows.Add(dr);
                //        dr = table.NewRow();
                //    }
                //    dr[j % i] = rowers[j];
                //}
                //GridView excel = new GridView();
                //excel.DataSource = table;
                //excel.DataBind();

                //Response.ClearContent();
                //Response.AppendHeader("content-disposition", "attachment; filename=ExcelFile.xls");
                //Response.ContentType = "application/excel";
                //StringWriter strw = new StringWriter();
                //HtmlTextWriter htmltw = new HtmlTextWriter(strw);
                //excel.RenderControl(htmltw);
                //Response.Write(strw.ToString());
                //Response.End();
                //return "Done";
            }
        }
    }
}