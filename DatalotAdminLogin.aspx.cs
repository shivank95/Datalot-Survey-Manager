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
using System.Data.OleDb;

public partial class DatalotAdminLogin : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string msg = Request.QueryString["err"];
        lblErr.Text = msg;

        string msg2 = Request.QueryString["msg"];
        lblResult.Text = msg2;

        string msg3 = Request.QueryString["logout"];

        if (msg3 == "true")
        {
            ClearAll();
        }
    }

    protected void  btnLogin_Click(object sender, EventArgs e)
    {
		string sql = "";
        lblErr.Text = "";

        // Set up a regular expression to filter out invalid name characters.
        Regex myFilter = new Regex("[0-9a-zA-Z-_]");

        string user = txtUsername.Text;

        if (user == "")
        {
            lblResult.Text = "ERROR: User name is required.";
            txtUsername.Focus();
            return;
        }


        if (!myFilter.IsMatch(user))
        {
            lblResult.Text = "The username you entered has an invalid character, so the request cannot be processed.";
            return;
        }

        string pass = txtPassword.Text;

        if (pass == "")
        {
            lblResult.Text = "ERROR: Password is required.";
            txtPassword.Focus();
            return;
        }

        myFilter = new Regex("[0-9a-zA-Z'-_!]*");

        if (!myFilter.IsMatch(pass))
        {
            lblResult.Text = "The password you entered has an invalid character, so the request cannot be processed.";
            return;
        }

        // Look for user in DB
        OleDbConnection db = new OleDbConnection();

        /*CHANGE 1 */

        db.ConnectionString = Configuration.ConnectionString;
        //db.ConnectionString = "Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=dev.techfit;Integrated Security=SSPI";


        try
        {
            db.Open();

            sql = "SELECT * FROM spirit.Judges WHERE [Username] = " + toSQL(user) + " AND [Password] = " + toSQL(pass);

            OleDbCommand cmd = new OleDbCommand(sql, db);
            OleDbDataReader rdr = cmd.ExecuteReader();

            // If a match is found, assign the session variable to signify authorized.
            if (rdr.Read() == true)
            {
                Session["UserID"] = rdr["UserID"].ToString();
				//Response.Redirect("RevLogin.aspx?msg=" + rdr["FirstName"].ToString());
                Session["Name"] = rdr["FirstName"].ToString() + " " + rdr["LastName"].ToString();
                Session["Type"] = rdr["UserType"].ToString();
				// Only if authorized, reroute to the homepage
                if (Session["Type"].ToString() == "admin")
                {
                    //Response.Redirect("Results.aspx");
                    Response.Redirect("Datalot.aspx?getInfo=Login Successful.");
                }
                Response.Redirect("DatalotAdminLogin.aspx?msg=" + user + ", your name is not recognized. Please contact the webmaster of this site.");
            }
            else
            {
                ClearAll();
                Response.Redirect("DatalotAdminLogin.aspx?msg=" + user + ", your name is not recognized. Please contact the webmaster of this site. Login Failed");
            }

            rdr.Close();
        }
        catch (Exception err)
        {
            lblResult.Text = "ERROR: There was a problem - " + err.Message + err.StackTrace;
        }
        finally
        {
            db.Close();
        }
    }

    private void ClearAll()
    {
        Session["UserID"] = null;
        Session["Name"] = null;
        Session["Type"] = null;
    }

    // Helper method to filter the user's input
    private string toSQL(string stringValue)
    {
        string temp = stringValue.Replace("'", "''");
        temp = temp.Replace("<", "");
        temp = temp.Replace(">", "");
        temp = temp.Replace(";", " ");
        temp = temp.Replace(" OR ", " ");

        return "'" + temp + "'";
    }
}
