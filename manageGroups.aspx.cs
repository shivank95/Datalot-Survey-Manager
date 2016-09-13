using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class manageGroups : System.Web.UI.Page
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



    protected void openGroups(object sender, EventArgs e)
    {

        Response.Redirect("findGroups.aspx");

    }

    protected void backButton(object sender, EventArgs e)
    {

        Response.Redirect("Datalot.aspx");

    }


    protected void assignGroups(object sender, EventArgs e)
    {
        //lblTester.Text = schoolList.SelectedItem.Text;

        Response.Redirect("assignGroups.aspx");

    }

}