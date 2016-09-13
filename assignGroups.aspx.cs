using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Data.OleDb;

public partial class assignGroups : System.Web.UI.Page
{

    private String DBConnectionString = Configuration.ConnectionString;
    //"Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=dev.techfit;Integrated Security=SSPI";


    protected void Page_Load(object sender, EventArgs e)
    {

        //No More Testing
        lblTester.Visible = false;

        if (Session["UserID"] == null || (string)Session["Type"] != "admin")
        {
            Response.Redirect("DatalotAdminLogin.aspx?err=You must log in to view student records");
        }
        Page.MaintainScrollPositionOnPostBack = true;
        if (!IsPostBack)
        {

            //Group criticals
            Session["groupName"] = "";
            Session["assignedSurveyIDs"] = new List<string>();
            Session["assignedSurveys"] = new List<mySurvey2>();

            //New Date feature
            Session["dateReady"] = false;
            Session["activationDate"] = new DateTime();
            Session["endDate"] = new DateTime();
            Session["category"] = "Teachers";
            Session["statusCheck"] = false;

            //lblTester.Text = "joke";
            Session["buttonArray"] = new List<Button>();
            Session["surveyNameArray"] = new List<string>();
            Session["surveyArray"] = new List<mySurvey2>();
            myTable.Visible = false;
            surveyTable.Visible = false;
            lblDetails.Visible = false;
            //surveyTable.Visible = false;

            //string oldSName = (string)Session["search"];

            //if (oldSName != null)
            //{
            //    txtSurveyName.Text = "Old Search: " + oldSName;
            //    //lblErr.Visible = true;
            //}
        }

        if (Session["surveyNameArray"] == null)
        {
            //lblTester.Text = "joke";
            Session["buttonArray"] = new List<Button>();
            Session["surveyNameArray"] = new List<string>();
            Session["surveyArray"] = new List<mySurvey2>();
            myTable.Visible = false;
        }
        else
        {
            //lblTester.Text = "no kidding";

            //Load Table as controls get disposed
            List<string> surveyNameArray = (List<string>)Session["surveyNameArray"];
            myTable.Visible = true;

            myTable.Rows.Clear();

            //Add First Row
            //Create Row
            HtmlTableRow firstRow = new HtmlTableRow();
            firstRow.ID = "firstRow";
            //Cell1 (Label)
            HtmlTableCell cellFirst = new HtmlTableCell();
            cellFirst.ID = "cell00";
            cellFirst.Width = "222px";


            Label lblsn = new Label();
            lblsn.Text = "Survey Names";
            cellFirst.Controls.Clear();
            cellFirst.Controls.Add(lblsn);

            //Button Cell
            HtmlTableCell cellFirst2 = new HtmlTableCell();
            cellFirst2.ID = "cell0";
            cellFirst2.Width = "222px";

            firstRow.Cells.Add(cellFirst);
            firstRow.Cells.Add(cellFirst2);

            //Add row to table
            myTable.Rows.Add(firstRow);

            for (int i = 0; i < surveyNameArray.Count; i++)
            {
                //Create Row
                HtmlTableRow row = new HtmlTableRow();
                row.ID = "row" + i.ToString();

                //Cell1 (Label)
                HtmlTableCell cell = new HtmlTableCell();
                cell.ID = "cell11" + i.ToString();
                cell.Width = "200px";

                Label lblSurveyName = new Label();
                lblSurveyName.Text = surveyNameArray[i];

                cell.Controls.Clear();
                cell.Controls.Add(lblSurveyName);
                
                //Button Cell
                HtmlTableCell cell2 = new HtmlTableCell();
                cell2.ID = "cell22" + i.ToString();
                cell2.Width = "275px";

                Button btnViewSurvey = new Button();
                btnViewSurvey.ID = "btn00ID" + i.ToString();
                btnViewSurvey.Text = "View Survey";




                //*****************BUTTON METHOD********************************************************************************
                btnViewSurvey.Click += (s, e2) => {

                    //BUTTON FUNCTION
                    Button btnTemp = s as Button;

                    //Process of getting the index of the survey in the array
                    string btnID = btnTemp.ID;
                    int index = getIndex(btnID);
                    if (index == -1)
                    {
                        return;
                    }
                    

                    string loadType = "view";

                    List<mySurvey2> surveyArrayTemp = (List<mySurvey2>)Session["surveyArray"];

                    //Load Tables Function
                    loadTables(index, loadType);

                    //Testing
                    //txtSurveyName.Text = surveyArrayTemp[index].getName();

                    //Changed to seperate function
                };



                Button btnGroup = new Button();
                btnGroup.ID = "btn11ID" + i.ToString();
                btnGroup.Text = "Add to Group";
                btnGroup.Click += (s, e2) => { //your code;

                    //No need to describe button as Postback will take place and button function is defined there anyway
                    //BUTTON FUNCTION
                    Button btnTemp = s as Button;

                    //Process of getting the index of the survey in the array
                    string btnID = btnTemp.ID;
                    int index = getIndex(btnID);
                    if (index == -1)
                    {
                        return;
                    }

                    lblDetails.Visible = false;

                    List<mySurvey2> surveyArrayTemp = (List<mySurvey2>)Session["surveyArray"];
                    List<string> assignedSurveys = (List<string>)Session["assignedSurveyIDs"];
                    List<mySurvey2> AssignedSurveyObjects = (List<mySurvey2>)Session["assignedSurveys"];
                    if (assignedSurveys.Contains(surveyArrayTemp[index].getID()))
                    {
                        lblResult.Text = "This group already contains this Survey.";
                    }
                    else
                    {
                        //Add to global arrays
                        assignedSurveys.Add(surveyArrayTemp[index].getID());
                        AssignedSurveyObjects.Add(surveyArrayTemp[index]);

                        //Change Labels and update views
                        lblResult.Text = "Survey Added to Group.";
                        lblGroupInfo.Text = "This group currently has " + assignedSurveys.Count + " survey(s) assigned to it..";
                        updateGroupReview(lblGroupInfo.Text);
                    }
                    ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblResult.ClientID + "').style.display='none'\",3000)</script>");

                    

                    //Testing
                    //txtSurveyName.Text = surveyArrayTemp[index].getName();

                    //Reassign session variables
                    Session["assignedSurveyIDs"] = assignedSurveys;
                    Session["assignedSurveys"] = AssignedSurveyObjects;
                   


                };

                cell2.Controls.Clear();
                cell2.Controls.Add(btnViewSurvey);
                cell2.Controls.Add(btnGroup);
                

                //Add cell to row
                row.Cells.Add(cell);
                row.Cells.Add(cell2);

                //Add row to table
                myTable.Rows.Add(row);


            }

            if (surveyNameArray.Count == 0)
            {
                myTable.Visible = false;
            }

        }

        //For Testing
        // lblTester.Visible = false;
    }

    private string escapeSingleQuote(string text)
    {
        string newText = "";

        //If there are no single quotes, all is well
        if (!text.Contains("'"))
        {
            return text;
        }

        //Otherwise we need to work on this
        newText = text.Replace("'", "''");

        lblTester.Text = newText;

        return newText;
    }

    public void updateGroupReview(string text)
    {
        List<mySurvey2> assignedSurveyObjects = (List<mySurvey2>)Session["assignedSurveys"];

        if (text == "Clear")
        {
            lblGroupInfo.Text = "This group currently has 0 surveys assigned to it..";
            return;
        }

        lblGroupInfo.Text += "<br /><br /> <strong>Group Review: </strong><br />";
        lblGroupInfo.Text += "<br />Surveys Assigned";
        for (int i = 0; i < assignedSurveyObjects.Count; i++)
        {
            string name = assignedSurveyObjects[i].getName();
            lblGroupInfo.Text += "<br />" + (i+1) + ". " + name;
        }

    }

    protected void clearGroup(object sender, EventArgs e)
    {
        List<mySurvey2> assignedSurveyObjects = (List<mySurvey2>)Session["assignedSurveys"];
        assignedSurveyObjects.Clear();
        List<string> assignedSurveys = (List<string>)Session["assignedSurveyIDs"];
        assignedSurveys.Clear();

        updateGroupReview("Clear");
    }

    protected void updateCategory(object sender, EventArgs e)
    {

        string category = ddlCategoryList.SelectedValue;
        Session["category"] = category;

        //Testing
        //lblDetails.Visible = true;
        //lblDetails.Text = category;
    }


    protected void updateDate(object sender, EventArgs e)
    {
        bool complete = true;

        int startYear = 2015;
        int startMonth = 1;
        int startDay = 1;
        int startHour = 12;
        int startMin = 0;

        int endYear = 2015;
        int endMonth = 12;
        int endDay = 1;
        int endHour = 12;
        int endMin = 0;

        //SURVEY START DATES

        if (ddlStartYear.SelectedIndex < 1)
        {
            complete = false;
        }
        else
        {
            startYear = 2015 + ddlStartYear.SelectedIndex;
        }

        if (ddlStartMonth.SelectedIndex < 1)
        {
            complete = false;
        }
        else
        {
            startMonth = ddlStartMonth.SelectedIndex;
        }

        if (ddlStartDay.SelectedIndex < 1)
        {
            complete = false;
        }
        else
        {
            startDay = ddlStartDay.SelectedIndex;
        }

        if (ddlStartHour.SelectedIndex < 1)
        {
            complete = false;
        }
        else
        {
            startHour = ddlStartHour.SelectedIndex;
            if (startHour == 24)
            {
                startHour = 0;
            }
        }

        if (ddlStartMinutes.SelectedIndex < 1)
        {
            complete = false;
        }
        else
        {
            startMin = Convert.ToInt32(ddlStartMinutes.SelectedValue);
        }


        //SURVEY END DATES
        if (ddlEndYear.SelectedIndex < 1)
        {
            complete = false;
        }
        else
        {
            endYear = 2015 + ddlEndYear.SelectedIndex;
        }

        if (ddlEndMonth.SelectedIndex < 1)
        {
            complete = false;
        }
        else
        {
            endMonth = ddlEndMonth.SelectedIndex;
        }

        if (ddlEndDay.SelectedIndex < 1)
        {
            complete = false;
        }
        else
        {
            endDay = ddlEndDay.SelectedIndex;
        }

        if (ddlEndHour.SelectedIndex < 1)
        {
            complete = false;
        }
        else
        {
            endHour = ddlEndHour.SelectedIndex;
            if (endHour == 24)
            {
                endHour = 0;
            }
        }

        if (ddlEndMinutes.SelectedIndex < 1)
        {
            complete = false;
        }
        else
        {
            endMin = Convert.ToInt32(ddlEndMinutes.SelectedValue);
        }


        //Update Session Variable
        bool dateReady = (bool)Session["dateReady"];
        dateReady = complete;
        Session["dateReady"] = dateReady;

        //lblResult.Text = "Start Date: " + startDay + " " + startMonth + " " + startYear + " " + "<br /> End Date: " +
        //    endDay + " " + endMonth + " " + endYear + " " +
        //    Session["dateReady"];


        //If all dates have been entered. Add them to the session variables
        if (complete == true)
        {

           
            try
            {
                DateTime tempActDate = new DateTime(startYear, startMonth, startDay, startHour, startMin, 0);
                Session["activationDate"] = tempActDate;

                DateTime tempEndDate = new DateTime(endYear, endMonth, endDay, endHour, endMin, 0);
                Session["endDate"] = tempEndDate;

                //Compare dates for validity
                int dateCompare = DateTime.Compare(tempActDate, tempEndDate);
                if (dateCompare >= 0)
                {
                    complete = false;
                    Session["dateReady"] = complete;
                    lblResult.Text = "Not a valid Date..";
                }
                else
                {
                    lblResult.Text = "";
                    //Testing
                    //lblResult.Text = "Activation Date: " + ((DateTime)Session["activationDate"]).ToString() +
                        //"<br /> End Date: " + ((DateTime)Session["endDate"]).ToString();
                }
            }
            catch (Exception excp)
            {
                complete = false;
                Session["dateReady"] = complete;
                lblResult.Text = "Not a valid Date..";
            }
        }
    }

    private bool validateGroup()
    {
        string groupName = txtGroup.Text;
        groupName = escapeSingleQuote(groupName);
        //Validate
        if (groupName == "")
        {
            lblResult.Text = "Please Enter a Group Name!";
            return false;
        }

        else if ((bool)Session["dateReady"] == false)
        {
            lblResult.Text = "Please Enter a valid Activation Date and an End Date for the Survey";
            return false;
        }

        List<mySurvey2> assignedSurveyObjects = (List<mySurvey2>)Session["assignedSurveys"];

        if (assignedSurveyObjects.Count < 1)
        {
            lblResult.Text = "There must be atleast 1 survey assigned to this group!";
            return false;
        }


        return true;
    }

    private int getIndex(string btnID)
    {
        int index = 0;

        string[] parts1 = btnID.Split(new string[] { "ID" }, StringSplitOptions.None);

        //This will happen if something is really wrong
        if (parts1.Length < 1)
        {
            return -1;
        }

        string tempIndex = parts1[parts1.Length - 1];

        //Convert string to int

        index = Convert.ToInt32(tempIndex);


        //Testing
        lblTester.Text = "" + index;


        return index;
    }


    protected void submitGroups(object sender, EventArgs e)
    {
        if (validateGroup() == false)
        {
            return;
        }

        string groupName = txtGroup.Text;
        groupName = escapeSingleQuote(groupName);
        List<mySurvey2> assignedSurveyObjects = (List<mySurvey2>)Session["assignedSurveys"];

        //Create a group
        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;

        //Initiate Database Queries
        try
        {
            //Open Connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();

            bool statusCheck = addToStatusCheck.Checked;

            string sql = "INSERT INTO spirit.DatalotSurveyGroup (Name, GrpActivationDate, GrpEndDate, Category, StatusCheck) VALUES ('" + groupName + "', '" + (DateTime)Session["activationDate"] + 
                "', '" + (DateTime)Session["endDate"] + "', '" + (string) Session["category"] + "', + '" + statusCheck + "'); ";

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            //Get this groupID
            sql = "SELECT MAX(GroupID) AS MaxGroupID FROM spirit.DatalotSurveyGroup";
            cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();
            string groupID = "";
            while (rdr.Read())
            {
                groupID = (string)rdr["MaxGroupID"].ToString();
            }

            //Now we assign this groupID to the various surveys
            for (int i = 0; i < assignedSurveyObjects.Count; i++)
            {
                string currentSurveyID = assignedSurveyObjects[i].getID();

                //sql = "UPDATE spirit.DatalotSurvey SET GroupID = " + groupID + " WHERE SurveyID = " + currentSurveyID + ";";
                sql = "INSERT INTO spirit.DatalotAssociatedSurveyGroup (GroupID, SurveyID, SurveyNum) VALUES ('" + groupID + "', '" + currentSurveyID + "', '" + (i + 1) + "'); ";

                cmd = new OleDbCommand(sql, db);
                rdr = cmd.ExecuteReader();
            }

            lblResult.Text = "Group Saved!";
            ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblResult.ClientID + "').style.display='none'\",2000)</script>");

            groupName = groupName.Replace("''", "'");
            Response.Redirect("Datalot.aspx?getInfo=Group '" + groupName + "' Created!");
        }
        catch (Exception submitGroupexc)
        {
            lblErr.Text = "Error! Unable to submit Survey..." + submitGroupexc.Message + submitGroupexc.StackTrace;
            //ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblResult.ClientID + "').style.display='none'\",2000)</script>");
        }
        finally
        {
            //close db connection
            db.Close();
        }
    }

    public void loadTables(int index, string loadType)
    {

        List<mySurvey2> surveyArrayTemp = (List<mySurvey2>)Session["surveyArray"];
        surveyTable.Rows.Clear();

        //Row1 (SURVEY NAME)
        HtmlTableRow surveyNameRow = new HtmlTableRow();
        surveyNameRow.ID = "surveyNameRow";
        //Cell1 (Label)
        HtmlTableCell cellSurveyName = new HtmlTableCell();
        cellSurveyName.Width = "222px";

        Label lblSurveyTable1 = new Label();
        lblSurveyTable1.Text = "Name: ";

        cellSurveyName.Controls.Clear();
        cellSurveyName.Controls.Add(lblSurveyTable1);

        HtmlTableCell cellSurveyNameValue = new HtmlTableCell();
        cellSurveyNameValue.Width = "222px";

        Label lblSurveyTable1Value = new Label();
        lblSurveyTable1Value.Text = surveyArrayTemp[index].getName();
        
        cellSurveyNameValue.Controls.Clear();
        cellSurveyNameValue.Controls.Add(lblSurveyTable1Value);

        surveyNameRow.Cells.Add(cellSurveyName);
        surveyNameRow.Cells.Add(cellSurveyNameValue);

        surveyTable.Rows.Add(surveyNameRow);
        //Row1 END

        ////Row2 (SURVEY Group)
        //HtmlTableRow surveyGroupRow = new HtmlTableRow();
        //surveyGroupRow.ID = "surveyGroupRow";
        ////Cell1 (Label)
        //HtmlTableCell cellGroupName = new HtmlTableCell();
        //cellGroupName.Width = "222px";

        //Label lblSurveyTable2 = new Label();
        //lblSurveyTable2.Text = "Group Name: ";
        
        //cellGroupName.Controls.Clear();
        //cellGroupName.Controls.Add(lblSurveyTable2);

        //HtmlTableCell cellSurveyGroupValue = new HtmlTableCell();
        //cellSurveyGroupValue.Width = "222px";

        //Label lblSurveyTable2Value = new Label();
        //string groupName = getGroupName(surveyArrayTemp[index].getSurveyGroup());
        //lblSurveyTable2Value.Text = groupName;
        //cellSurveyGroupValue.Controls.Clear();
        //cellSurveyGroupValue.Controls.Add(lblSurveyTable2Value);

        //surveyGroupRow.Cells.Add(cellGroupName);
        //surveyGroupRow.Cells.Add(cellSurveyGroupValue);

        //surveyTable.Rows.Add(surveyGroupRow);
        ////Row2 END

        //Row3 (SURVEY PARTICIPANTS)
        HtmlTableRow surveyParticipantsRow = new HtmlTableRow();
        surveyParticipantsRow.ID = "surveyParticipantsRow";
        //Cell1 (Label)
        HtmlTableCell cellParticipants = new HtmlTableCell();
        cellParticipants.Width = "222px";

        Label lblSurveyTable3 = new Label();
        lblSurveyTable3.Text = "Participants: ";
        cellParticipants.Controls.Clear();
        cellParticipants.Controls.Add(lblSurveyTable3);

        HtmlTableCell cellParticipantsValue = new HtmlTableCell();
        cellParticipantsValue.Width = "222px";

        Label lblSurveyTable3Value = new Label();
        lblSurveyTable3Value.Text = surveyArrayTemp[index].getParticipants();
        cellParticipantsValue.Controls.Clear();
        cellParticipantsValue.Controls.Add(lblSurveyTable3Value);

        surveyParticipantsRow.Cells.Add(cellParticipants);
        surveyParticipantsRow.Cells.Add(cellParticipantsValue);

        surveyTable.Rows.Add(surveyParticipantsRow);
        //Row3 END


        //Row4 (SURVEY Activation Date)
        HtmlTableRow surveyActivationDateRow = new HtmlTableRow();
        surveyActivationDateRow.ID = "surveyActivationDateRow";
        //Cell1 (Label)
        HtmlTableCell cellActivationDate = new HtmlTableCell();
        cellActivationDate.Width = "222px";

        Label lblSurveyTable4 = new Label();
        lblSurveyTable4.Text = "Activation Date (yyyy-mm-dd): ";
        cellActivationDate.Controls.Clear();
        cellActivationDate.Controls.Add(lblSurveyTable4);

        HtmlTableCell cellActivationDateValue = new HtmlTableCell();
        cellActivationDateValue.Width = "222px";

        Label lblSurveyTable4Value = new Label();
        lblSurveyTable4Value.Text = surveyArrayTemp[index].getActivationDate();
        cellActivationDateValue.Controls.Clear();
        cellActivationDateValue.Controls.Add(lblSurveyTable4Value);

        surveyActivationDateRow.Cells.Add(cellActivationDate);
        surveyActivationDateRow.Cells.Add(cellActivationDateValue);

        surveyTable.Rows.Add(surveyActivationDateRow);
        //Row4 END

        //Row5 (SURVEY End Date)
        HtmlTableRow surveyEndDateRow = new HtmlTableRow();
        surveyEndDateRow.ID = "surveyEndDateRow";
        //Cell1 (Label)
        HtmlTableCell cellEndDate = new HtmlTableCell();
        cellEndDate.Width = "222px";

        Label lblSurveyTable5 = new Label();
        lblSurveyTable5.Text = "End Date (yyyy-mm-dd): ";
        cellEndDate.Controls.Clear();
        cellEndDate.Controls.Add(lblSurveyTable5);

        HtmlTableCell cellEndDateValue = new HtmlTableCell();
        cellEndDateValue.Width = "222px";

        Label lblSurveyTable5Value = new Label();
        lblSurveyTable5Value.Text = surveyArrayTemp[index].getEndDate();
        cellEndDateValue.Controls.Clear();
        cellEndDateValue.Controls.Add(lblSurveyTable5Value);

        surveyEndDateRow.Cells.Add(cellEndDate);
        surveyEndDateRow.Cells.Add(cellEndDateValue);

        surveyTable.Rows.Add(surveyEndDateRow);
        //Row5 END

        //Row6 (SURVEY Creation Date)
        HtmlTableRow surveyCreationDateRow = new HtmlTableRow();
        surveyCreationDateRow.ID = "surveyCreationDateRow";
        //Cell1 (Label)
        HtmlTableCell cellCreationDate = new HtmlTableCell();
        cellCreationDate.Width = "222px";

        Label lblSurveyTable6 = new Label();
        lblSurveyTable6.Text = "Creation Date (yyyy-mm-dd): ";
        cellCreationDate.Controls.Clear();
        cellCreationDate.Controls.Add(lblSurveyTable6);

        HtmlTableCell cellCreationDateValue = new HtmlTableCell();
        cellCreationDateValue.Width = "222px";

        Label lblSurveyTable6Value = new Label();
        lblSurveyTable6Value.Text = surveyArrayTemp[index].getCreationDate();
        cellCreationDateValue.Controls.Clear();
        cellCreationDateValue.Controls.Add(lblSurveyTable6Value);

        surveyCreationDateRow.Cells.Add(cellCreationDate);
        surveyCreationDateRow.Cells.Add(cellCreationDateValue);

        surveyTable.Rows.Add(surveyCreationDateRow);
        

        surveyTable.Visible = true;
        lblDetails.Visible = true;
    }

    private void displayResult(string result)
    {
        lblResult2.Visible = true;
        lblResult2.Text = result;
        ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblResult2.ClientID + "').style.display='none'\",2000)</script>");
    }

    protected void backButton(object sender, EventArgs e)
    {
        Response.Redirect("manageGroups.aspx");
    }

    //private string getGroupName(string groupID)
    //{

    //    string currentGroupName = "";
    //    //Initiate OleDb
    //    OleDbConnection db = null;
    //    OleDbDataReader rdr = null;

    //    try
    //    {
    //        //open connection
    //        db = new OleDbConnection();
    //        db.ConnectionString = DBConnectionString;
    //        db.Open();

    //        string sql = "SELECT Name FROM spirit.DatalotSurveyGroup WHERE GroupID = " + groupID + ";";

    //        //Execute Command
    //        OleDbCommand cmd = new OleDbCommand(sql, db);
    //        rdr = cmd.ExecuteReader();

    //        while (rdr.Read())
    //        {
    //            currentGroupName = (string)rdr["Name"].ToString();
    //        }

    //        return currentGroupName;
    //    }

    //    catch (Exception groupNameExc)
    //    {

    //    }
    //    finally
    //    {
    //        db.Close();
    //    }

    //    return "Not Assigned";


    //}



    protected void tableButtonClick(object sender, EventArgs e)
    {
        //Button button = sender as Button;
        //lblErr.Text = "Button clicked";
        //lblTester.Text = "Button Clicked";
        //lblErr.Visible = true;
        //displayResult("Button clicked");

        //txtSurveyName.Text = "Yolo";

    }

    private void search()
    {
        surveyTable.Visible = false;
        lblDetails.Visible = false;
        string surveyName = txtSurveyName.Text;
        //displayResult("Searching '" + surveyName + "'...");
        Session["search"] = surveyName;
        //Validate

        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;
        //Initiate Database Queries

        try
        {
            //Open Connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();

            //Prepare sql
            String sql = "SELECT SurveyID,Name,ActivationDate,EndDate,Participants,CreationDate FROM spirit.DatalotSurvey S WHERE S.Name LIKE '%" + surveyName + "%';";

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();


            String surveyNames = "";

            List<string> surveys = new List<string>();
            //Read
            int surveyCnt = 0;

            List<string> surveyNameArray = (List<string>)Session["surveyNameArray"];

            List<mySurvey2> currentSurveyArray = (List<mySurvey2>)Session["surveyArray"];

            surveyNameArray.Clear();
            currentSurveyArray.Clear();


            //Temp Variables to build Survey classes
            string sname = "";
            string parts = "";
            string activationDate = "";
            string endDate = "";
            string creationDate = "";
            string sID = "";

            while (rdr.Read())
            {
                surveyCnt++;
                surveyNames += (string)rdr["Name"].ToString() + "\t";
                surveys.Add((string)rdr["Name"].ToString());

                //Add to temp array
                surveyNameArray.Add((string)rdr["Name"].ToString());

                //Build Survey Class
                sname = (string)rdr["Name"].ToString();
                parts = (string)rdr["Participants"].ToString();
                activationDate = rdr["ActivationDate"].ToString();
                endDate = (string)rdr["EndDate"].ToString();
                creationDate = (string)rdr["CreationDate"].ToString();
                sID = (string)rdr["SurveyID"].ToString();
                mySurvey2 tempSurvey = new mySurvey2(sname, activationDate, endDate, parts, creationDate, sID);

                currentSurveyArray.Add(tempSurvey);
            }

            Session["surveyNameArray"] = surveyNameArray;
            Session["surveyArray"] = currentSurveyArray;

            //Clear all rows
            myTable.Rows.Clear();

            //Add First Row
            //Create Row
            HtmlTableRow firstRow = new HtmlTableRow();
            firstRow.ID = "firstRow";
            //Cell1 (Label)
            HtmlTableCell cellFirst = new HtmlTableCell();
            cellFirst.ID = "cell00";
            cellFirst.Width = "222px";

            Label lblsn = new Label();
            lblsn.Text = "Survey Names";
            cellFirst.Controls.Clear();
            cellFirst.Controls.Add(lblsn);

            HtmlTableCell cellFirst2 = new HtmlTableCell();
            cellFirst2.ID = "cell0";
            cellFirst2.Width = "222px";

            firstRow.Cells.Add(cellFirst);
            firstRow.Cells.Add(cellFirst2);

            //Add row to table
            myTable.Rows.Add(firstRow);


            for (int i = 0; i < surveyCnt; i++)
            {
                //Create Row
                HtmlTableRow row = new HtmlTableRow();
                row.ID = "row0" + i.ToString();

                //Cell1 (Label)
                HtmlTableCell cell = new HtmlTableCell();
                cell.ID = "cell11" + i.ToString();
                cell.Width = "200px";

                Label lblSurveyName = new Label();
                lblSurveyName.Text = surveyNameArray[i];
                cell.Controls.Clear();
                cell.Controls.Add(lblSurveyName);

                //Button Cell
                HtmlTableCell cell2 = new HtmlTableCell();
                cell2.ID = "cell22" + i.ToString();
                cell2.Width = "275px";

                Button btnViewSurvey = new Button();
                btnViewSurvey.ID = "btn00ID" + i.ToString();
                btnViewSurvey.Text = "View Survey";



                //*****************BUTTON METHOD************************************************************
                btnViewSurvey.Click += (s, e2) => { //your code;

                    //No need to describe button as Postback will take place and button function is defined there anyway

                };

                Button btnGroup = new Button();
                btnGroup.ID = "btn11ID" + i.ToString();
                btnGroup.Text = "Add to Group";
                btnGroup.Click += (s, e2) => { //your code;

                    //No need to describe button as Postback will take place and button function is defined there anyway

                };

                //Add Buttons
                cell2.Controls.Clear();
                cell2.Controls.Add(btnViewSurvey);
                cell2.Controls.Add(btnGroup);
                

                //Add cell to row
                row.Cells.Add(cell);
                row.Cells.Add(cell2);

                //Add row to table
                myTable.Rows.Add(row);


            }



            if (surveyCnt == 0)
            {
                displayResult("No Surveys Found");
                myTable.Visible = false;
                surveyTable.Visible = false;
                lblDetails.Visible = false;
                txtSurveyName.Focus();
            }
            else
            {
                displayResult("Found " + surveyCnt + " record(s)..");
                myTable.Visible = true;
            }





        }
        catch (Exception findExc)
        {
            lblErr.Text = "Error! Unable to find Survey..." + findExc.Message + findExc.StackTrace;
        }

        finally
        {
            db.Close();
        }
    }


    protected void searchButton(object sender, EventArgs e)
    {



        search();
        //Prepare sql

        //Execute Command


    }
}

internal class mySurvey2
{

    //private string question;
    //private List<string> answers;
    //private bool multipleAns;
    //private int questNo;

    private string name;
    private string activationDate;
    private string endDate;
    private string participants;
    private string creationDate;
    private string sID;

    public mySurvey2(string n, string ad, string ed, string p, string cd, string id)
    {
        this.name = n;
        this.activationDate = ad;
        this.endDate = ed;
        this.participants = p;
        this.creationDate = cd;
        this.sID = id;
    }

    //public void addAnswer(string a)
    //{
    //    this.answers.Add(a);
    //}

    public string getName()
    {
        return this.name;
    }

    public string getID()
    {
        return this.sID;
    }
    public string getActivationDate()
    {
        return this.activationDate;
    }

    public string getEndDate()
    {
        return this.endDate;
    }
    public string getCreationDate()
    {
        return this.creationDate;
    }
    public string getParticipants()
    {
        return this.participants;
    }
    //public List<string> getAnswers()
    //{
    //    return answers;
    //}
    //public bool getMultOpt()
    //{
    //    return this.multipleAns;
    //}
}