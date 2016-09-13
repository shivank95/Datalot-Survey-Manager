using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;

public partial class DatalotFormSS : System.Web.UI.Page
{
    String DBConnectionString = Configuration.ConnectionString;
    //private static string userTableName = "spirit.TECHFITTeacherApps2016";
    private static string userTableName = "spirit.TECHFITTeacherApps2015";

    protected void Page_Load(object sender, EventArgs e)
    {
        //For Testing
        lblTester.Visible = false;

        if (Session["UserID"] == null || Session["Type"] == null)
        {
            Response.Redirect("DatalotAdminLogin.aspx?err=You must log in to view student records");
        }

        if (!IsPostBack)
        {

            if (!Session["UserID"].Equals("40") && !Session["Type"].Equals("admin"))
            {
                Response.Redirect("Login.aspx?err=You must log in to view student records" + Session["UserID"] + " " + Session["Type"]);
            }

            addSchoolNamesDynamically(schoolList);

            //schoolList.Items.Clear();
            //schoolList.Items.Add(new ListItem("Bremen Public School", "0"));
            //schoolList.Items.Add(new ListItem("Carver Edisto Middle School", "1"));
            //schoolList.Items.Add(new ListItem("H.E. McCracken Middle School", "2"));
            //schoolList.Items.Add(new ListItem("Hilton Head Island Middle School", "3"));
            //schoolList.Items.Add(new ListItem("Hughes Academy of Science and Technology", "4"));
            //schoolList.Items.Add(new ListItem("Ladys Island Middle School", "5"));
            //schoolList.Items.Add(new ListItem("Lafayette Sunnyside Intermediate School", "6"));
            //schoolList.Items.Add(new ListItem("Northview Middle School", "7"));
            //schoolList.Items.Add(new ListItem("Robert E. Howard Middle School", "8"));
            //schoolList.Items.Add(new ListItem("Winamac Middle School", "9"));
            //schoolList.Items.Add(new ListItem("Woodlan Jr/Sr High School", "10"));

        }
    }

    private void addSchoolNamesDynamically(DropDownList myddl)
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

    protected void updateCategory(object sender, EventArgs e)
    {

        string category = ddlCategoryList.SelectedValue;
        Session["category"] = category;

        //Testing
        //lblDetails.Visible = true;
        //lblDetails.Text = category;
    }

    private List<string> getSchoolList()
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

            while (rdr.Read())
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

    protected void Button1_Click(object sender, EventArgs e)
    {
        lblTester.Text = schoolList.SelectedItem.Text;

        Response.Redirect("DatalotFormEditor.aspx?SchoolNamer=" + schoolList.SelectedItem.Text + "&Category=" + ddlCategoryList.SelectedItem.Text);


    }

    protected void backButton(object sender, EventArgs e)
    {

        Response.Redirect("Datalot.aspx");
    }
}