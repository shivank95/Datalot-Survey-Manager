using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Data.OleDb;

using System.Collections;

// background-image: url("https://source.unsplash.com/kOqBCFsGTs8/1600x3400");

public partial class FormManager : System.Web.UI.Page
{
    int rowCnt = 0;
    String currentSchoolName = "Carver Edisto Middle School";

    String tableName = "spirit.TECHFITStudentApps2015";
    //String DBConnectionString = "Provider=SQLOLEDB;Server=techwebdev.ecn.purdue.edu;Database=dev.techfit;uid=dev.techfit.usr01;pwd=ECNdev.techfitpa55;";

    String DBConnectionString = Configuration.ConnectionString;
    //String DBConnectionString = "Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=dev.techfit;Integrated Security=SSPI";

    public void getSchoolName()
    {
        currentSchoolName = Request.QueryString["SchoolNamer"];



        //Default school name
        if (currentSchoolName == null)
        {
            currentSchoolName = "Carver Edisto Middle School";
        }

        lblSchoolName.Text = "School: " + currentSchoolName;
    }

    private void setRowCnt()
    {


        OleDbConnection db = null;
        try
        {
            //myReader = myCommand.ExecuteReader();
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();
            OleDbDataReader myReader = null;

            String sql = "SELECT * FROM " + tableName + " WHERE SchoolName = '" + currentSchoolName + "';";
            OleDbCommand cmd = new OleDbCommand(sql, db);
            myReader = cmd.ExecuteReader();

            while (myReader.Read())
            {
                rowCnt++;
            }
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ROW COUNT: " + rowCnt + "')" + "')", true);


        }
        catch (Exception e)
        {
            lblRowCnt.Text = "Error: " + e.StackTrace;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('There was a Problem..\n" + e.StackTrace.ToString() + "')", true);
            return;
        }
        finally
        {
            db.Close();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        if (Session["UserID"] == null || Session["Type"] == null)
        {
            Response.Redirect("Login.aspx?err=You must log in to view student records");
        }

        if (!Session["UserID"].Equals("40") && !Session["Type"].Equals("admin"))
        {
            Response.Redirect("Login.aspx?err=You must log in to view student records");
        }
        //Used for Testing
        lblRowCnt.Visible = false;

        getSchoolName();
        setRowCnt();



        //Create Rows
        for (int i = 0; i < rowCnt; i++)
        {
            HtmlTableRow row = new HtmlTableRow();
            //row.ID = i.ToString();
            //The ID
            row.ID = "row" + i.ToString();

            HtmlTableCell cell = new HtmlTableCell();
            cell.ID = "cell" + i.ToString();

            TextBox txtID = new TextBox();
            txtID.Enabled = false;
	    txtID.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            txtID.ID = "txt1" + i.ToString();
            cell.Controls.Add(txtID);

            //The Name
            HtmlTableCell cell2 = new HtmlTableCell();
            cell2.ID = "cell222" + i.ToString();

            TextBox txtName = new TextBox();
            txtName.Enabled = false;
	    txtName.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            txtID.ID = "txt2" + i.ToString();
            cell2.Controls.Add(txtName);

            //The PCF
            HtmlTableCell cell3 = new HtmlTableCell();
            cell3.ID = "cell333" + i.ToString();
            cell3.Align = "center";

            CheckBox pcf = new CheckBox();
            pcf.ID = "ckb1" + i;
            //pcf.Checked = students.ElementAt(i).parentConsent;
            cell3.Controls.Add(pcf);

            //The Student ASSENT
            HtmlTableCell cell4 = new HtmlTableCell();
            cell4.ID = "cell444" + i.ToString();
            cell4.Align = "center";

            CheckBox sa = new CheckBox();
            //sa.Checked = students.ElementAt(i).studentAssent;
            sa.ID = "ckb2" + i;
            cell4.Controls.Add(sa);

            //PreProgram Surveys
            HtmlTableCell cell5 = new HtmlTableCell();
            cell5.ID = "cell555" + i.ToString();
            cell5.Align = "center";

            CheckBox pps = new CheckBox();
            //pps.Checked = students.ElementAt(i).preprogramSurvey;
            pps.ID = "ckb3" + i;
            cell5.Controls.Add(pps);

            //HtmlTableCell cell6 = new HtmlTableCell();
            //cell6.ID = "cell666" + i.ToString();

            ////School Name
     //       TextBox txtSchool = new TextBox();
     //       txtSchool.Enabled = false;
	    //txtSchool.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");

     //       cell6.Controls.Add(txtSchool);

            HtmlTableCell cell7 = new HtmlTableCell();
            cell7.ID = "cell777" + i.ToString();
            TextBox txtApplicationID = new TextBox();
            txtApplicationID.Enabled = false;

            cell7.Controls.Add(txtApplicationID);

            cell7.Visible = false;

            //PostProgramSurveys
            HtmlTableCell cell8 = new HtmlTableCell();
            cell8.ID = "cell888" + i.ToString();
            cell8.Align = "center";
            CheckBox postSurveys = new CheckBox();
            postSurveys.ID = "ckb4" + i;
            cell8.Controls.Add(postSurveys);

            /*********CHANGE 2**********/
            //FinalSurvey3Mos
            HtmlTableCell cell9 = new HtmlTableCell();
            cell9.ID = "cell999" + i.ToString();
            cell9.Align = "center";
            CheckBox finalSurveyMos = new CheckBox();
            finalSurveyMos.ID = "ckb5" + i;
            cell9.Controls.Add(finalSurveyMos);

            /******CHANGE 2 END*********/

            //Store something for application ID

            //Add to ROW
            row.Cells.Add(cell);
            row.Cells.Add(cell2);
            row.Cells.Add(cell3);
            row.Cells.Add(cell4);
            row.Cells.Add(cell5);
            row.Cells.Add(cell7);
            row.Cells.Add(cell8);

            /*********CHANGE 3**********/
            row.Cells.Add(cell9);
            /******CHANGE 3 END*********/

            myTable.Rows.Add(row);
        }

        //Get Data

        OleDbConnection db = null;
        OleDbDataReader myReader = null;
        try
        {
            //myReader = myCommand.ExecuteReader();
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();
            //OleDbDataReader myReader = null;

            String sql = "Select* from " + tableName + " WHERE SchoolName = '" + currentSchoolName + "' ORDER BY SchoolName, FirstName;";
            OleDbCommand cmd = new OleDbCommand(sql, db);
            myReader = cmd.ExecuteReader();


            //try {
            //myReader = myCommand.ExecuteReader();
        }
        catch (Exception e3)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('There was a Problem..\n" + e3.StackTrace.ToString() + "')", true);
            return;
        }


        String ID = "";
        String firstName = "";
        String lastName = "";
        bool parentConsent;
        bool studentAssent;
        bool preprogramSurvey;
        bool postprogramSurvey;

        /******CHANGE 4*********/
        bool finalSurveyCheck;
        String FS = "";
        /******CHANGE 4 END*********/

        String schoolName = "";
        String applicationID = "";

        String PC = "";
        String SA = "";
        String PPS = "";
        String PPS2 = "";



        int index = 0;
        int offStudentCount = 0;

        while (myReader.Read() && index < rowCnt)
        {
            ID = myReader["ID"].ToString();
            firstName = myReader["FirstName"].ToString();
            lastName = myReader["LastName"].ToString();

            if (!myReader["ParentConsentForm"].Equals(null))
            {
                PC = myReader["ParentConsentForm"].ToString();

            }
            else
            {
                PC = "false";
            }
            if (!myReader["StudentAssentForm"].Equals(null))
            {
                SA = myReader["StudentAssentForm"].ToString();

            }
            else
            {
                SA = "false";
            }
            //SA = myReader["StudentAssentForm"].ToString() ;
            if (!myReader["PreProgramSurvey"].Equals(null))
            {
                PPS = myReader["PreProgramSurvey"].ToString();

            }
            else
            {
                PPS = "false";
            }

            if (!myReader["PostProgramSurvey"].Equals(null))
            {
                PPS2 = myReader["PostProgramSurvey"].ToString();

            }
            else
            {
                PPS2 = "false";
            }

            /******CHANGE 5*********/
            if (!myReader["FinalSurvey3Mos"].Equals(null))
            {
                FS = myReader["FinalSurvey3Mos"].ToString();
            }
            else
            {
                FS = "false";
            }
            /******CHANGE 5 END*********/



            //PPS = myReader["PreProgramSurveys"].ToString();
            schoolName = myReader["SchoolName"].ToString();
            applicationID = myReader["ApplicationID"].ToString();

            //To calculate official techfit student count
            int offCount = 0;

            if (PC != null && PC.Contains("t") || PC.Contains("T"))
            {
                parentConsent = true;
                offCount++;
            }
            else
            {
                parentConsent = false;
            }

            if (SA != null && SA.Contains("t") || SA.Contains("T"))
            {
                studentAssent = true;
                offCount++;
            }
            else
            {
                studentAssent = false;
            }

            if (PPS != null && PPS.Contains("t") || PPS.Contains("T"))
            {
                preprogramSurvey = true;
                offCount++;
            }
            else
            {
                preprogramSurvey = false;
            }

            if (PPS2 != null && PPS2.Contains("t") || PPS2.Contains("T"))
            {
                postprogramSurvey = true;
                offCount++;
            }
            else
            {
                postprogramSurvey = false;
            }

            /******CHANGE 6*********/
            if (FS != null && FS.Contains("t") || FS.Contains("T"))
            {
                finalSurveyCheck = true;
                //offCount++;
            }
            else
            {
                finalSurveyCheck = false;
            }
            /******CHANGE 6 END*********/



            if (offCount == 4)
            {
                offStudentCount++;

            }
                HtmlTableRow currentRow = (HtmlTableRow)myTable.FindControl("row" + index);



            //First Cell
            HtmlTableCell cell1 = (HtmlTableCell)currentRow.FindControl("cell" + index);
            TextBox txtID = (TextBox)cell1.Controls[0] as TextBox;

            txtID.Text = ID;


            //Second Cell
            HtmlTableCell cell2 = (HtmlTableCell)currentRow.FindControl("cell222" + index);
            TextBox txtName = (TextBox)cell2.Controls[0] as TextBox;

            txtName.Text = firstName + " " + lastName;

            if (offCount != 4)
            {
                currentRow.BgColor = "LightBlue";
                txtID.BackColor = System.Drawing.Color.LightBlue;
                txtName.BackColor = System.Drawing.Color.LightBlue;
            }

            //Third Cell
            HtmlTableCell cell3 = (HtmlTableCell)currentRow.FindControl("cell333" + index);
            CheckBox ckbPCF = (CheckBox)cell3.Controls[0] as CheckBox;

            ckbPCF.Checked = parentConsent;

            //Fourth Cell
            HtmlTableCell cell4 = (HtmlTableCell)currentRow.FindControl("cell444" + index);
            CheckBox ckbSA = (CheckBox)cell4.Controls[0] as CheckBox;

            ckbSA.Checked = studentAssent;

            //Fifth Cell
            HtmlTableCell cell5 = (HtmlTableCell)currentRow.FindControl("cell555" + index);
            CheckBox ckbPPF = (CheckBox)cell5.Controls[0] as CheckBox;

            ckbPPF.Checked = preprogramSurvey;

            //6th Cell
            //HtmlTableCell cell6 = (HtmlTableCell)currentRow.FindControl("cell666" + index);
            //TextBox txtSchool = (TextBox)cell6.Controls[0] as TextBox;

            //txtSchool.Text = schoolName;

            //7th cell
            HtmlTableCell cell7 = (HtmlTableCell)currentRow.FindControl("cell777" + index);
            TextBox txtAppID = (TextBox)cell7.Controls[0] as TextBox;

            txtAppID.Text = applicationID;

            //8th cell
            HtmlTableCell cell8 = (HtmlTableCell)currentRow.FindControl("cell888" + index);
            CheckBox ckbPPF2 = (CheckBox)cell8.Controls[0] as CheckBox;

            ckbPPF2.Checked = postprogramSurvey;

            /******CHANGE 7*********/
            //9th cell
            HtmlTableCell cell9 = (HtmlTableCell)currentRow.FindControl("cell999" + index);
            CheckBox ckbFS = (CheckBox)cell9.Controls[0] as CheckBox;

            ckbFS.Checked = finalSurveyCheck;
            /******CHANGE 7 END*********/

            //Increment Index for ID
            index++;

        }

        //Update total student Count
        lblStudentCnt.Text = "Total Number of Students: " + index;

        lblOfficialTechfit.Text = "Number of Official Techfit Students: " + offStudentCount;

        //conn.Close();
    }

    //Save button
    protected void Button2_Click(object sender, EventArgs e)
    {

        OleDbConnection db = null;
        OleDbDataReader rdr = null;

        int index = 0;
        int offStudentCnt = 0;

        //Get Changed Data
        while (index < rowCnt)
        {


            HtmlTableRow currentRow = (HtmlTableRow)myTable.FindControl("row" + index);

            //First Cell
            HtmlTableCell cell1 = (HtmlTableCell)currentRow.FindControl("cell" + index);
            TextBox txtID = (TextBox)cell1.Controls[0] as TextBox;

            String ID = txtID.Text;

            //Name Cell for Color Changes
            //Second Cell
            HtmlTableCell cell2 = (HtmlTableCell)currentRow.FindControl("cell222" + index);
            TextBox txtName = (TextBox)cell2.Controls[0] as TextBox;

            //7th Cell
            HtmlTableCell cell7 = (HtmlTableCell)currentRow.FindControl("cell777" + index);
            TextBox txtAppID = (TextBox)cell7.Controls[0] as TextBox;

            String applicationID = txtAppID.Text;


            //Third Cell
            HtmlTableCell cell3 = (HtmlTableCell)currentRow.FindControl("cell333" + index);
            CheckBox ckbPCF = (CheckBox)cell3.Controls[0] as CheckBox;

            String newPCF = ckbPCF.Checked.ToString();

            //Fourth Cell
            HtmlTableCell cell4 = (HtmlTableCell)currentRow.FindControl("cell444" + index);
            CheckBox ckbSA = (CheckBox)cell4.Controls[0] as CheckBox;

            String newSA = ckbSA.Checked.ToString();

            //Fifth Cell
            HtmlTableCell cell5 = (HtmlTableCell)currentRow.FindControl("cell555" + index);
            CheckBox ckbPPF = (CheckBox)cell5.Controls[0] as CheckBox;

            String newPPS = ckbPPF.Checked.ToString();

            //Eigth Cell for PostProgramSurveys
            HtmlTableCell cell8 = (HtmlTableCell)currentRow.FindControl("cell888" + index);
            CheckBox ckbPPF2 = (CheckBox)cell8.Controls[0] as CheckBox;

            String newPPS2 = ckbPPF2.Checked.ToString();

            /******CHANGE 8*********/
            //Ninth Cell for FinalSurveys
            HtmlTableCell cell9 = (HtmlTableCell)currentRow.FindControl("cell999" + index);
            CheckBox ckbFS = (CheckBox)cell9.Controls[0] as CheckBox;

            String newFS = ckbFS.Checked.ToString();
            /******CHANGE 8 END*********/


            if (newPCF == "True" && newSA == "True" && newPPS == "True" && newPPS2 == "True")
            {
                offStudentCnt++;
                currentRow.BgColor = "White";
                txtID.BackColor = System.Drawing.Color.White;
                txtName.BackColor = System.Drawing.Color.White;
            }
            else
            {
                currentRow.BgColor = "LightBlue";
                txtID.BackColor = System.Drawing.Color.LightBlue;
                txtName.BackColor = System.Drawing.Color.LightBlue;
            }

            //Next Row
            index++;



            //Update Command

            try
            {
                //myReader = myCommand.ExecuteReader();

                db = new OleDbConnection();
                db.ConnectionString = DBConnectionString;
                db.Open();
                String sql = "UPDATE " + tableName + " SET ParentConsentForm = '" + newPCF + "', StudentAssentForm = '" +
                    newSA + "', PreProgramSurvey = '" + newPPS + "', PostProgramSurvey = '" + newPPS2 + "' WHERE ApplicationID =  '" +
                    applicationID + "';";

                /******CHANGE 9*********/
                //Update the new sql query with final Surveys
                sql = "UPDATE " + tableName + " SET ParentConsentForm = '" + newPCF + "', StudentAssentForm = '" +
                    newSA + "', PreProgramSurvey = '" + newPPS + "', PostProgramSurvey = '" + newPPS2 + "', FinalSurvey3Mos = '" + newFS + "' WHERE ApplicationID =  '" +
                    applicationID + "';";
                /******CHANGE 9 END*********/

                OleDbCommand cmd = new OleDbCommand(sql, db);
                rdr = cmd.ExecuteReader();
            }
            catch (Exception e2)
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('There was a Problem..\n" + e2.StackTrace.ToString() + "')", true);
		lblSaved.Visible = true;
		lblSaved.Text = "Error While Saving..";
        	ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblSaved.ClientID + "').style.display='none'\",2000)</script>");

            }
            finally
            {
                //conn.Close();
                db.Close();
            }

        }

        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Changes saved!')", true);

	lblSaved.Visible = true;
	lblSaved.Text = "CHANGES SAVED!..";
        ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblSaved.ClientID + "').style.display='none'\",2000)</script>");

        lblOfficialTechfit.Text = "Number of Official Techfit Students: " + offStudentCnt;

    }
    protected void btnSelectSchool_Click(object sender, EventArgs e)
    {
        Response.Redirect("SelectSchool.aspx");
    }
}
