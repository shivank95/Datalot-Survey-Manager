using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Data.OleDb;


public partial class DatalotLogin : System.Web.UI.Page
{

    //Change Teacher Name here if needed
    private static string userTableName = "spirit.TECHFITTeacherApps2015";
    private static string studentTableName = "spirit.TECHFITStudentApps2015";

    private String DBConnectionString = Configuration.ConnectionString;
    //"Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=dev.techfit;Integrated Security=SSPI";


    protected void Page_Load(object sender, EventArgs e)
    {
        string msg = Request.QueryString["err"];
        lblMessage.Text = msg;

        string msg2 = Request.QueryString["msg"];
        lblMessage.Text = msg2;


        if (!IsPostBack)
        {
            addSchoolNamesDynamically(txtSchoolName);
        }

    }


    private void addSchoolNamesDynamically (DropDownList myddl)
    {
        //Call Get schoolList to get a List of all school names
        List<string> schoolList = getSchoolList();

        //Clear dropdownList
        myddl.Items.Clear();

        for (int i = 0; i < schoolList.Count; i++)
        {
            ListItem currentSchool = new ListItem(schoolList[i], schoolList[i]);
            myddl.Items.Add(currentSchool);
        }
    }

    private List<string> getSchoolList ()
    {
        List<string> schoolList = new List<string>();


        //Test
        //schoolList.Add("LMB");
        //schoolList.Add("Blah");

        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;

        try
        {
            //open connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();

            string sql = "SELECT DISTINCT SchoolName FROM " + userTableName + " ORDER BY SchoolName;";

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                schoolList.Add((string)rdr["SchoolName"].ToString());
            }
        }

        catch (Exception exc)
        {
            lblResult.Text = "Error! Unable to load School List..." + exc.Message + exc.StackTrace;
        }

        finally
        {
            db.Close();
        }



       return schoolList;
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        OleDbConnection db = null;
        OleDbDataReader rdr = null;
        OleDbDataReader rdr2 = null;
        string appid = "";
        string sql = "";
        string details = "";
        try
        {
            // Get the form's input values.

            string teacherID = txtTeacherID.Text.Trim();
            string highSchoolName = txtSchoolName.Text.Trim();

            // Format the SQL statement to search for a match

            sql = "SELECT * FROM " + userTableName + " WHERE ID = '" + teacherID +
                "' AND schoolname = '" + highSchoolName + "';";

            string sql2 = "SELECT * FROM " + studentTableName + " WHERE ID = '" + teacherID +
               "' AND SchoolName = '" + highSchoolName + "';";

            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;

            db.Open();

            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            //No read from earlier table select try students table
            OleDbCommand cmd2 = new OleDbCommand(sql2, db);
            rdr2 = cmd2.ExecuteReader();


            int readCnt = 0;

            if (rdr2.Read())
            {
                readCnt++;

                Session["UserID"] = rdr2["ID"].ToString();

                Session["Name"] = rdr2["FirstName"].ToString() + " " + rdr2["LastName"].ToString();
                Session["Type"] = "Students";

                lblResult.Text = "Login Successful" + (string)Session["Type"];

                Response.Redirect("FindTakeSurveyPublic.aspx");

            }

            else if (rdr.Read())
            {
                readCnt++;

                Session["UserID"] = rdr["ID"].ToString();
                //Response.Redirect("RevLogin.aspx?msg=" + rdr["FirstName"].ToString());
                Session["Name"] = rdr["FirstName"].ToString() + " " + rdr["LastName"].ToString();
                Session["Type"] = "Teachers";

                lblResult.Text = "Login Successful" + (string)Session["Type"];

                Response.Redirect("FindTakeSurveyPublic.aspx");

            }

            

            else
            {
                // No match found so ID and school did not match
                lblResult.Text = "There was no match of the provided ID and selected school. Please try again or contact the administrator for assistance.";
            }
        }
        catch (Exception err)
        {
            
            lblResult.Text = "ERROR: There was a problem - " + err.Message + err.StackTrace;
        }
        finally
        {
            // Close the database connection.

            if (db != null)
            {
                db.Close();
            }
        }
    }

}
