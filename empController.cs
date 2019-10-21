using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using emptest.Models;
using System.Data.OracleClient;
using System.Data;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;

namespace emptest.Controllers
{
    public class empController : Controller
    {
      
        // GET: emp
        /* public ActionResult Index(int id)
         {
             string connection = ConfigurationManager.ConnectionStrings["hrConn"].ToString();
             var con = new OracleConnection(connection);
             OracleCommand cmd = new OracleCommand();
             OracleDataAdapter objAdapter = new OracleDataAdapter();
             DataTable dt = new DataTable();
             DataSet ds = new DataSet();
             //emp e = new emp();

            // List<emp> e =.toList();
           //  e=(ds.empid==id)
            // DbSet<emp> emps
             var query = "select empname from EMP where EMPID="+id;
             try
             {
                 con.Open();
                 cmd.CommandText = query;
                 cmd.Connection = con;
                 objAdapter.SelectCommand = cmd;
                 objAdapter.Fill(dt);
                 if (dt.Rows.Count > 0)
                 {
                     ds.Tables.Add( dt);
                 }
             }
             catch (Exception ex)
             {
                 con.Close();
             }
             finally
             {
                 dt.Dispose();
                 cmd.Dispose();
                 objAdapter.Dispose();
                 con.Close();


             }
             return View();
             }

     */
       
        //INSERT------------------------------------------------------------------------------------------------------------------------------------------

        public ActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        public ActionResult Index(emp e)
        {
            string constr = ConfigurationManager.ConnectionStrings["hrConn"].ToString();
            using (OracleConnection con = new OracleConnection(constr))
            {
                if (e.empid.ToString() != null && e.empname != null)
                {
                    string query = "select empid from emp where empid=" + e.empid;
                    OracleCommand cmd = new OracleCommand(query);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    DataTable dt = new DataTable();
                    //Da
                    OracleDataAdapter objAdapter = new OracleDataAdapter(query, con);

                    // DataSet ds = new DataSet();        

                    objAdapter.Fill(dt);
                    
                    con.Close();


                    if (dt.Rows.Count == 0)
                    {
                        string query1 = "INSERT INTO emp(empid,empname, gender,age) VALUES(" + e.empid + ",'" + e.empname + "','" + e.gender + "'," + e.age + ")";
                        OracleCommand cmd1 = new OracleCommand(query1);
                        cmd1.Connection = con;
                        con.Open();
                        cmd1.ExecuteNonQuery();
                        con.Close();
                        /*using (OracleCommand cmd = new OracleCommand(query))
                        {
                            cmd.Connection = con;
                            con.Open();
                            cmd.Parameters.AddWithValue("@empid", e.empid);
                            cmd.Parameters.AddWithValue("@empname", e.empname);
                            cmd.Parameters.AddWithValue("@gender", e.gender);
                            cmd.Parameters.AddWithValue("@age", e.age);
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }*/


                        TempData["msg"] = "<script>alert('successfully inserted');</script>";
                        return RedirectToAction("index","main");
                    }
                    else
                    {
                        TempData["msg"] = "<script>alert('already exist this employee id');</script>";
                        return View();
                    }
                }
                
                else
                {
                    TempData["msg"] = "<script>alert('please completely fill details');</script>";
                    return View();
                }
                
            }
        }
        //---------------DISPLAY--------------------------------------------------------------------------------------------------------------------

        [HttpGet]
        public ActionResult display()
        {
            string connection = ConfigurationManager.ConnectionStrings["hrConn"].ToString();
            IList<string> ep = new List<string>();
           // 
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            var con = new OracleConnection(connection);
            con.Open();
            var query = "select * from EMP ";

            OracleDataAdapter objAdapter = new OracleDataAdapter(query, con);
            objAdapter.Fill(dt);
           // ViewBag.ct = dt.Rows.Count;
            con.Close();
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;

            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    //row.Add(col.ColumnName.Replace("_", ""), dr[col]);
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }


            ViewBag.data = serializer.Serialize(rows);
         //  ViewBag.Inserted = "false";
            return View("display");


            // DataSet ds = new DataSet();        

            // objAdapter.Fill(dt);


        }

        //export to excel



        public IList<login> GetEmployeeList()
        {
            
            IList<login> ep = new List<login>();
            string connection = ConfigurationManager.ConnectionStrings["hrConn"].ToString();
            var query = "select * from EMP order by empid asc ";
            var con = new OracleConnection(connection);
            OracleCommand cmd = new OracleCommand(query);
           
                cmd.Connection = con;
                con.Open();



                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
               
                    var ws = new login();
                   
                ws.empid = Convert.ToInt32(reader["empid"]);
                    ws.empname = reader["empname"].ToString();
                    ws.gender = reader["gender"].ToString();
                    ws.age = Convert.ToInt32(reader["age"]);

                    ep.Add(ws);
                }
                reader.Close();
            


          //  TempData["e"] = ep;
           
                con.Close();
           




            return ep;
        }
        public ActionResult export()
        {
            return View(this.GetEmployeeList());
        }

        public ActionResult ExportToExcel()
        {
           
            
                var gv = new GridView();
                gv.DataSource = this.GetEmployeeList();
                gv.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
                Response.ContentType = "application/ms-excel";

                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);

                gv.RenderControl(objHtmlTextWriter);


                Response.Output.Write(objStringWriter.ToString());
            
            
                Response.Flush();
                Response.End();
           
          

            return View();

        }




    }
}  


    
