using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Datalot : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        lblTester.Visible = false;
        if (Session["UserID"] == null || (string)Session["Type"] != "admin")
        {
            Response.Redirect("DatalotAdminLogin.aspx?err=You must log in to view student records");
        }

        getInfo();
    }

    private void getInfo()
    {
        string text = Request.QueryString["getInfo"];
        lblResult.Text = text;
        ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblResult.ClientID + "').style.display='none'\",5000)</script>");
    }

    protected void createSurvey(object sender, EventArgs e)
    {
        //lblTester.Text = schoolList.SelectedItem.Text;

        Response.Redirect("createSurvey.aspx");


    }
    protected void logout(object sender, EventArgs e)
    {
        Session["UserID"] = null;
        Session["Type"] = null;
        Response.Redirect("DatalotAdminLogin.aspx?err=Logout Successful");
    }



    protected void openSurvey(object sender, EventArgs e)
    {
        //lblTester.Text = schoolList.SelectedItem.Text;

        Response.Redirect("findSurvey.aspx");

    }

    protected void assignGroups(object sender, EventArgs e)
    {
        //lblTester.Text = schoolList.SelectedItem.Text;

        Response.Redirect("manageGroups.aspx");

    }

    protected void takeSurvey(object sender, EventArgs e)
    {
        //lblTester.Text = schoolList.SelectedItem.Text;

        Response.Redirect("FindTakeSurvey.aspx");

    }

    protected void surveyStatus(object sender, EventArgs e)
    {
        //lblTester.Text = schoolList.SelectedItem.Text;

        Response.Redirect("DatalotSelectSchool.aspx");

    }

    protected void formStatus(object sender, EventArgs e)
    {
        //lblTester.Text = schoolList.SelectedItem.Text;

        Response.Redirect("DatalotFormSS.aspx");

    }

}