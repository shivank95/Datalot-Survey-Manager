using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Data.OleDb;
using System.Data;
using System.Reflection;
using System.Text;
using System.IO;

public partial class findGroups : System.Web.UI.Page
{

    private String DBConnectionString = Configuration.ConnectionString;
    //"Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=dev.techfit;Integrated Security=SSPI";


    protected void Page_Load(object sender, EventArgs e)
    {
        //NO testing
        lblTester.Visible = false;

        if (Session["UserID"] == null || (string)Session["Type"] != "admin")
        {
            Response.Redirect("DatalotAdminLogin.aspx?err=You must log in to view student records");
        }
        Page.MaintainScrollPositionOnPostBack = true;
        if (!IsPostBack)
        {

            Session["buttonArray"] = new List<Button>();
            Session["groupNameArray"] = new List<string>();
            Session["groupArray"] = new List<myGroupEdit>();
            myTable.Visible = false;
            surveyTable.Visible = false;
            myResponseTable.Visible = false;
            lblDetails.Visible = false;

        }

        if (Session["groupArray"] == null)
        {
            //lblTester.Text = "joke";
            Session["buttonArray"] = new List<Button>();
            Session["groupNameArray"] = new List<string>();
            Session["groupArray"] = new List<myGroupEdit>();
            myTable.Visible = false;
        }
        else
        {
            //lblTester.Text = "no kidding";

            //Load Table as controls get disposed
            List<string> groupNameArray = (List<string>)Session["groupNameArray"];
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
            lblsn.Text = "Group Names";
            cellFirst.Controls.Clear();
            cellFirst.Controls.Add(lblsn);

            //Button Cell
            HtmlTableCell cellFirst2 = new HtmlTableCell();
            cellFirst2.ID = "cell0";
            cellFirst2.Width = "350px";

            firstRow.Cells.Add(cellFirst);
            firstRow.Cells.Add(cellFirst2);

            //Add row to table
            myTable.Rows.Add(firstRow);

            for (int i = 0; i < groupNameArray.Count; i++)
            {
                //Create Row
                HtmlTableRow row = new HtmlTableRow();
                row.ID = "row" + i.ToString();

                //Cell1 (Label)
                HtmlTableCell cell = new HtmlTableCell();
                cell.ID = "cell11" + i.ToString();
                cell.Width = "200px";

                Label lblSurveyName = new Label();
                lblSurveyName.Text = groupNameArray[i];

                cell.Controls.Clear();
                cell.Controls.Add(lblSurveyName);

                //Button Cell
                HtmlTableCell cell2 = new HtmlTableCell();
                cell2.ID = "cell22" + i.ToString();
                cell2.Width = "350px";

                Button btnViewSurvey = new Button();
                btnViewSurvey.ID = "btn00ID" + i.ToString();
                btnViewSurvey.Text = "View Group";




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

                    //Load Tables Function
                    loadTables(index, loadType);
                };


                Button btnGetResponse = new Button();
                btnGetResponse.ID = "btn11ID" + i.ToString();
                btnGetResponse.Text = "Get Responses";
                btnGetResponse.Click += (s, e2) =>
                { //your code;

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

                    List<myGroupEdit> groupArrayTemp = (List<myGroupEdit>)Session["groupArray"];

                    string currentGroupID = groupArrayTemp[index].getGroupID();
                    string groupName = groupArrayTemp[index].getName();


                    //Get the Data
                    List<SurveyResponse> surveyResponseList = new List<SurveyResponse>(); ;
                    surveyResponseList = getData(currentGroupID);

                    Table oTable = getResponseTable(surveyResponseList, groupName);


                    oTable.BorderStyle = BorderStyle.Solid;

                    string fileName = groupName + " Response " + DateTime.Now.ToString();
                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ".xls");
                    Response.Charset = "hahaha";
                    this.EnableViewState = false;
                    System.IO.StringWriter oStringWriter = new System.IO.StringWriter();
                    System.Web.UI.Html32TextWriter oHtmlTextWriter = new System.Web.UI.Html32TextWriter(oStringWriter);
                    oTable.RenderControl(oHtmlTextWriter);
                    Response.Write(oStringWriter.ToString());
                    Response.End();

                };


                Button btnDeleteSurvey = new Button();
                btnDeleteSurvey.ID = "btn22ID" + i.ToString();
                btnDeleteSurvey.Text = "Delete Group";
                btnDeleteSurvey.OnClientClick = "javascript: return confirm('Are you sure you want to delete the Survey?'); ";
                btnDeleteSurvey.Click += (s, e2) => { //your code;

                    //BUTTON FUNCTION
                    Button btnTemp = s as Button;

                    //Process of getting the index of the survey in the array
                    string btnID = btnTemp.ID;
                    int index = getIndex(btnID);
                    if (index == -1)
                    {
                        return;
                    }

                    List<myGroupEdit> groupArrayTemp = (List<myGroupEdit>)Session["groupArray"];

                    string currentGroupID = groupArrayTemp[index].getGroupID();
                    //Initiate OleDb
                    OleDbConnection db = null;
                    OleDbDataReader rdr = null;

                    try
                    {
                        //open connection
                        db = new OleDbConnection();
                        db.ConnectionString = DBConnectionString;
                        db.Open();

                        //Delete FROM Survey Statuses
                        string sql = "DELETE FROM spirit.DatalotSurveyStatusNew WHERE GroupID = " + currentGroupID + ";";

                        //Execute Command
                        OleDbCommand cmd = new OleDbCommand(sql, db);
                        rdr = cmd.ExecuteReader();

                        //Delete Question Responses First
                        sql = "DELETE FROM spirit.DatalotQuestionResponse WHERE GroupID = " + currentGroupID + ";";

                        //Execute Command
                        cmd = new OleDbCommand(sql, db);
                        rdr = cmd.ExecuteReader();


                        sql = "DELETE FROM spirit.DatalotAssociatedSurveyGroup WHERE GroupID = " + currentGroupID + ";";

                        //Execute Command
                        cmd = new OleDbCommand(sql, db);
                        rdr = cmd.ExecuteReader();

                        sql = "DELETE FROM spirit.DatalotSurveyGroup WHERE GroupID = " + currentGroupID + ";";

                        cmd = new OleDbCommand(sql, db);
                        rdr = cmd.ExecuteReader();

                        //Refresh
                        search();

                    }

                    catch (Exception delExc)
                    {
                        lblErr.Text = "Error! Unable to submit Survey..." + delExc.Message + delExc.StackTrace;
                    }

                    finally
                    {
                        db.Close();
                    }


                };

                cell2.Controls.Clear();
                cell2.Controls.Add(btnViewSurvey);
                cell2.Controls.Add(btnGetResponse);
                cell2.Controls.Add(btnDeleteSurvey);


                //Add cell to row
                row.Cells.Add(cell);
                row.Cells.Add(cell2);

                //Add row to table
                myTable.Rows.Add(row);


            }

            if (groupNameArray.Count == 0)
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

    private List<string> getTechfitIDList(string groupID, string surveyID)
    {
        List<string> TechfitIDList = new List<string>();

        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;

        try
        {
            //open connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();

            string sql = "SELECT DISTINCT TechfitID FROM spirit.DatalotQuestionResponse WHERE GroupID = " + groupID + " AND SurveyID = " + surveyID + ";";

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                TechfitIDList.Add((string)rdr["TechfitID"].ToString());
            }

            return TechfitIDList;
        }

        catch (Exception exc)
        {
            lblErr.Text = "Error! Unable to load data for techfitID responses..." + exc.Message + exc.StackTrace;
        }
        finally
        {
            db.Close();
        }

        return TechfitIDList;

    }

    private List<string> getSurveyIDList(string groupID)
    {

        List<string> surveyIDList = new List<string>();

        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;

        try
        {
            //open connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();

            string sql = "SELECT SurveyID FROM spirit.DatalotAssociatedSurveyGroup WHERE GroupID = " + groupID + ";";

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                surveyIDList.Add((string)rdr["SurveyID"].ToString());
            }

            return surveyIDList;
        }

        catch (Exception exc)
        {
            lblErr.Text = "Error! Unable to load data for surveyID in responses..." + exc.Message + exc.StackTrace;
        }
        finally
        {
            db.Close();
        }

        return surveyIDList;


    }
    private List<SurveyResponse> getData(string groupID)
    {
        List<SurveyResponse> surveyResponseList = new List<SurveyResponse>();
        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;


        List<string> SurveyIDList = getSurveyIDList(groupID);

        if (SurveyIDList.Count == 0)
        {
            lblResult.Text = "This group has no Survey(s) attached to it!";
            return surveyResponseList;
        }


        try
        {
            //open connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();

            lblTester.Text = "";

            for (int i = 0; i < SurveyIDList.Count; i++)
            {

                List<string> techfitIDList = new List<string>();
                List<myRecord> recordList = new List<myRecord>();

                techfitIDList = getTechfitIDList(groupID, SurveyIDList[i]);

                for (int j = 0; j < techfitIDList.Count; j++)
                {

                    string sql = "SELECT QR.GroupID, QR.SurveyID, QR.TechfitID, QR.QuestionID, Q.Question, Q.QNumber, QR.AnswerText, QR.AnswerChoiceNumber FROM spirit.DatalotQuestionResponse QR Left join spirit.DatalotQuestion Q ON " +
                    "QR.QuestionID = Q.QuestionID WHERE QR.GroupID = " + groupID + " AND QR.SurveyID = " + SurveyIDList[i] + " AND QR.TechfitID = '" + techfitIDList[j] + "' ORDER BY QR.GroupID, QR.SurveyID, QR.TechfitID, Q.QNumber; ";

                    //Execute Command
                    OleDbCommand cmd = new OleDbCommand(sql, db);
                    rdr = cmd.ExecuteReader();

                    List<string> answerChoiceList = new List<string>();

                    while (rdr.Read())
                    {

                        string questionID = (string)rdr["QuestionID"].ToString();

                        string answerChoiceNum = (string)rdr["AnswerChoiceNumber"].ToString();

                        if (answerChoiceNum == null || answerChoiceNum == "")
                        {
                            answerChoiceList.Add((string)rdr["AnswerText"].ToString());
                        }
                        else
                        {
                            answerChoiceList.Add(answerChoiceNum);
                        }



                        //Testing
                        lblTester.Visible = true;
                        lblTester.Text += "<br /> GroupID: " + groupID + " SURVEY ID: " + SurveyIDList[i] + " TECHFIT ID: " + techfitIDList[j] + " Question ID: " + questionID + " <br /> ";



                    }

                    //We have a techfit ID and a bunch of answers choices. Add these as records.
                    myRecord currentRecord = new myRecord(techfitIDList[j], answerChoiceList);
                    recordList.Add(currentRecord);

                    lblTester.Text += "<br /> <br />";
                }

                SurveyResponse currentSurveyResponses = new SurveyResponse(SurveyIDList[i], recordList);
                surveyResponseList.Add(currentSurveyResponses);


                //Testing
                lblTester.Text += "<br /> <br />";
                lblTester.Text = "";

            }

        }

        catch (Exception exc)
        {
            lblErr.Text = "Error! Unable to load data for responses..." + exc.Message + exc.StackTrace;
        }

        finally
        {
            db.Close();
        }

        return surveyResponseList;

    }

    private void populateResponseLabel(List<SurveyResponse> surveyResponseList)
    {
        lblResponse.Text = "";
        for (int i = 0; i < surveyResponseList.Count; i++)
        {

            SurveyResponse currentSurvey = surveyResponseList[i];

            List<myRecord> currentRecords = currentSurvey.getRecordList();


            lblResponse.Text += "SURVEY ID: " + currentSurvey.getSurveyID() + "<br />";
            lblResponse.Text += "TECHFITID &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Q1";

            for (int j = 0; j < currentRecords.Count; j++)
            {
                lblResponse.Text += "<br /> " + currentRecords[j].getTechfitID();
                List<string> answerChoices = currentRecords[j].getAnswerList();

                for (int k = 0; k < answerChoices.Count; k++)
                {
                    lblResponse.Text += " " + answerChoices[k] + " ";
                }

                lblResponse.Text += " <br />";
            }

            lblResponse.Text += "<br /> <br />";
        }
    }

    private string getSurveyName(string surveyID)
    {
        string name = "";


        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;

        try
        {
            //open connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();

            string sql = "SELECT Name FROM spirit.DatalotSurvey WHERE SurveyID = " + surveyID + ";";

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                name = rdr["Name"].ToString();
            }

            return name;

        }

        catch (Exception exc)
        {
            lblResult.Text = "Error! Unable to get assigned Survey..." + exc.Message + exc.StackTrace;
        }

        finally
        {
            db.Close();
        }

        return "DELETED SURVEY";
    }

    private int getQuestionCnt(string surveyID)
    {
        string name = "";

        int qCnt = 0;

        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;

        try
        {
            //open connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();

            string sql = "SELECT Question FROM spirit.DatalotQuestion WHERE SurveyID = " + surveyID + ";";

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                name = rdr["Question"].ToString();
                qCnt++;
            }

            return qCnt;

        }

        catch (Exception exc)
        {
            lblResult.Text = "Error! Unable to get assigned Survey..." + exc.Message + exc.StackTrace;
        }

        finally
        {
            db.Close();
        }

        return 0;
    }


    private Table getResponseTable(List<SurveyResponse> surveyResponseList, string groupName)
    {
        Table myResponseTable = new Table();

        //Create Group Row
        TableRow groupRow = new TableRow();

        TableCell groupNameCell = new TableCell();
        groupNameCell.HorizontalAlign = HorizontalAlign.Left;
        groupNameCell.VerticalAlign = VerticalAlign.Top;
        groupNameCell.Width = 100;

        Label lblGroupName = new Label();
        lblGroupName.Font.Bold = true;
        lblGroupName.Text = "SurveyGroup Name: " + groupName;

        groupNameCell.Controls.Clear();
        groupNameCell.Controls.Add(lblGroupName);

        groupRow.Cells.Add(groupNameCell);

        myResponseTable.Rows.Add(groupRow);



        for (int i = 0; i < surveyResponseList.Count; i++)
        {

            SurveyResponse currentSurvey = surveyResponseList[i];

            List<myRecord> currentRecords = currentSurvey.getRecordList();

            string currentSurveyID = currentSurvey.getSurveyID();

            string currentSurveyName = getSurveyName(currentSurveyID);

            //Create Row
            TableRow surveyRow = new TableRow();
            //surveyRow.ID = "SurveyNamerow" + i.ToString();

            //Cell1 (Label)
            TableCell surveyIDcell = new TableCell();
            surveyIDcell.HorizontalAlign = HorizontalAlign.Left;
            surveyIDcell.VerticalAlign = VerticalAlign.Top;
            surveyIDcell.Width = 100;

            Label lblSurveyName = new Label();
            lblSurveyName.Font.Bold = true;
            lblSurveyName.Text = "SurveyName: " + currentSurveyName;


            //Add label to cell
            surveyIDcell.Controls.Clear();
            surveyIDcell.Controls.Add(lblSurveyName);

            //Add cell to row
            surveyRow.Cells.Add(surveyIDcell);

            //Add row to table
            myResponseTable.Rows.Add(surveyRow);

            //Add blank row
            myResponseTable.Rows.Add(new TableRow());

            //Question Count and First Rows
            int qCnt = getQuestionCnt(currentSurveyID);

            TableRow firstRow = new TableRow();

            for (int z = 0; z <= qCnt; z++)
            {

                TableCell cell = new TableCell();
                cell.HorizontalAlign = HorizontalAlign.Left;
                cell.VerticalAlign = VerticalAlign.Top;
                cell.Width = 100;

                Label lblFirst = new Label();
                if (z == 0)
                {
                    lblFirst.Text = "TECHFIT ID";
                }
                else
                {
                    lblFirst.Text = "Q" + z;
                }

                cell.Controls.Add(lblFirst);

                firstRow.Cells.Add(cell);
            }

            myResponseTable.Rows.Add(firstRow);

            for (int j = 0; j < currentRecords.Count; j++)
            {
                string currentTechfitID = currentRecords[j].getTechfitID();

                //Create Row
                TableRow recordRow = new TableRow();

                //TechfitID cell
                TableCell techfitIDcell = new TableCell();
                techfitIDcell.HorizontalAlign = HorizontalAlign.Left;
                techfitIDcell.VerticalAlign = VerticalAlign.Top;
                //TechfitID label
                Label lblTechfitId = new Label();
                lblTechfitId.Text = currentTechfitID;

                //Add label to cell
                techfitIDcell.Controls.Clear();
                techfitIDcell.Controls.Add(lblTechfitId);

                //Add cell to row
                recordRow.Cells.Add(techfitIDcell);

                List<string> answerChoices = currentRecords[j].getAnswerList();

                for (int k = 0; k < answerChoices.Count; k++)
                {
                    string currentAnswerChoice = answerChoices[k];

                    //Create cell
                    TableCell answerChoiceCell = new TableCell();
                    answerChoiceCell.Width = 100;
                    //Create Label
                    Label lblChoice = new Label();
                    lblChoice.Text = currentAnswerChoice;

                    //Add Label to cell
                    answerChoiceCell.Controls.Clear();
                    answerChoiceCell.HorizontalAlign = HorizontalAlign.Left;
                    answerChoiceCell.VerticalAlign = VerticalAlign.Top;
                    answerChoiceCell.Controls.Add(lblChoice);

                    //Add cell to row
                    recordRow.Cells.Add(answerChoiceCell);
                }

                //Add row to table
                myResponseTable.Rows.Add(recordRow);

            }

            //Create a blank row
            //Add blank row
            myResponseTable.Rows.Add(new TableRow());

        }


        return myResponseTable;
    }

    private void loadResponseTable(List<SurveyResponse> surveyResponseList)
    {
        myResponseTable.Rows.Clear();

        //Maximum questions
        //int maxCellCnt = 20;

        for (int i = 0; i < surveyResponseList.Count; i++)
        {

            SurveyResponse currentSurvey = surveyResponseList[i];

            List<myRecord> currentRecords = currentSurvey.getRecordList();

            string currentSurveyID = currentSurvey.getSurveyID();

            //Create Row
            HtmlTableRow surveyRow = new HtmlTableRow();
            //surveyRow.ID = "SurveyNamerow" + i.ToString();

            //Cell1 (Label)
            HtmlTableCell surveyIDcell = new HtmlTableCell();
            //surveyIDcell.ID = "SurveyNamecell" + i.ToString();
            surveyIDcell.Width = "200px";

            Label lblSurveyName = new Label();
            lblSurveyName.Text = "Survey ID: " + currentSurveyID;

            //Add label to cell
            surveyIDcell.Controls.Clear();
            surveyIDcell.Controls.Add(lblSurveyName);

            //Add cell to row
            surveyRow.Cells.Add(surveyIDcell);

            //Add row to table
            myResponseTable.Rows.Add(surveyRow);

            for (int j = 0; j < currentRecords.Count; j++)
            {
                string currentTechfitID = currentRecords[j].getTechfitID();

                //Create Row
                HtmlTableRow recordRow = new HtmlTableRow();

                //TechfitID cell
                HtmlTableCell techfitIDcell = new HtmlTableCell();

                //TechfitID label
                Label lblTechfitId = new Label();
                lblTechfitId.Text = currentTechfitID;

                //Add label to cell
                techfitIDcell.Controls.Clear();
                techfitIDcell.Controls.Add(lblTechfitId);

                //Add cell to row
                recordRow.Cells.Add(techfitIDcell);

                List<string> answerChoices = currentRecords[j].getAnswerList();

                for (int k = 0; k < answerChoices.Count; k++)
                {
                    string currentAnswerChoice = answerChoices[k];

                    //Create cell
                    HtmlTableCell answerChoiceCell = new HtmlTableCell();

                    //Create Label
                    Label lblChoice = new Label();
                    lblChoice.Text = currentAnswerChoice;

                    //Add Label to cell
                    answerChoiceCell.Controls.Clear();
                    answerChoiceCell.Controls.Add(lblChoice);

                    //Add cell to row
                    recordRow.Cells.Add(answerChoiceCell);
                }

                //Add row to table
                myResponseTable.Rows.Add(recordRow);

            }

            //Create a blank row
        }

    }

    private void getResponses(string groupID, string groupName)
    {
        //Get the Data
        List<SurveyResponse> surveyResponseList = new List<SurveyResponse>(); ;
        surveyResponseList = getData(groupID);

        //Construct the table
        loadResponseTable(surveyResponseList);

        //Testing
        populateResponseLabel(surveyResponseList);

        surveyTable.Visible = false;
        lblDetails.Visible = false;
        myResponseTable.Visible = true;
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


    public void loadTables(int index, string loadType)
    {

        //List<mySurvey> surveyArrayTemp = (List<mySurvey>)Session["surveyArray"];

        List<myGroupEdit> groupArrayTemp = (List<myGroupEdit>)Session["groupArray"];

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
        lblSurveyTable1Value.Text = groupArrayTemp[index].getName();

        cellSurveyNameValue.Controls.Clear();
        cellSurveyNameValue.Controls.Add(lblSurveyTable1Value);

        surveyNameRow.Cells.Add(cellSurveyName);
        surveyNameRow.Cells.Add(cellSurveyNameValue);

        surveyTable.Rows.Add(surveyNameRow);
        //Row1 END



        //Row2 (SURVEY LIST for Group)
        HtmlTableRow surveyCreationDateRow = new HtmlTableRow();
        surveyCreationDateRow.ID = "surveydRow";
        //Cell1 (Label)
        HtmlTableCell cellCreationDate = new HtmlTableCell();
        cellCreationDate.Width = "222px";

        Label lblSurveyTable6 = new Label();
        lblSurveyTable6.Text = "Assigned Surveys: ";
        cellCreationDate.Controls.Clear();
        cellCreationDate.Controls.Add(lblSurveyTable6);

        HtmlTableCell cellCreationDateValue = new HtmlTableCell();
        cellCreationDateValue.Width = "222px";

        Label lblSurveyTable6Value = new Label();
        //Get Survey List Method
        string surveyList = getSurveyList(groupArrayTemp[index].getGroupID());
        lblSurveyTable6Value.Text = surveyList;
        cellCreationDateValue.Controls.Clear();
        cellCreationDateValue.Controls.Add(lblSurveyTable6Value);

        surveyCreationDateRow.Cells.Add(cellCreationDate);
        surveyCreationDateRow.Cells.Add(cellCreationDateValue);

        surveyTable.Rows.Add(surveyCreationDateRow);
        //Row2 Creation


        //Row3 (Activation Date for Surveys Group
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
        lblSurveyTable4Value.Text = groupArrayTemp[index].getActivationDate();
        cellActivationDateValue.Controls.Clear();
        cellActivationDateValue.Controls.Add(lblSurveyTable4Value);

        surveyActivationDateRow.Cells.Add(cellActivationDate);
        surveyActivationDateRow.Cells.Add(cellActivationDateValue);

        surveyTable.Rows.Add(surveyActivationDateRow);
        //Row3 END

        //Row4 (SURVEY End Date)
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
        lblSurveyTable5Value.Text = groupArrayTemp[index].getEndDate();
        cellEndDateValue.Controls.Clear();
        cellEndDateValue.Controls.Add(lblSurveyTable5Value);

        surveyEndDateRow.Cells.Add(cellEndDate);
        surveyEndDateRow.Cells.Add(cellEndDateValue);

        surveyTable.Rows.Add(surveyEndDateRow);
        //Row4 END

        //ROW5 (Survey Group Category)
        HtmlTableRow surveyCategoryRow = new HtmlTableRow();
        surveyCategoryRow.ID = "surveyCategoryRow";
        //Cell1 (Label)
        HtmlTableCell cellCategory = new HtmlTableCell();
        cellCategory.Width = "222px";

        Label lblSurveyTableCategory = new Label();
        lblSurveyTableCategory.Text = "Category / Participants: ";
        cellCategory.Controls.Clear();
        cellCategory.Controls.Add(lblSurveyTableCategory);

        HtmlTableCell cellCategoryValue = new HtmlTableCell();
        cellCategoryValue.Width = "222px";

        Label lblSurveyTableCategoryValue = new Label();
        lblSurveyTableCategoryValue.Text = groupArrayTemp[index].getCategory();
        cellCategoryValue.Controls.Clear();
        cellCategoryValue.Controls.Add(lblSurveyTableCategoryValue);

        surveyCategoryRow.Cells.Add(cellCategory);
        surveyCategoryRow.Cells.Add(cellCategoryValue);

        surveyTable.Rows.Add(surveyCategoryRow);
        //ROW5 END


        surveyTable.Visible = true;
        lblDetails.Visible = true;
        myResponseTable.Visible = false;
    }

    private string getSurveyList(string groupID)
    {
        string surveys = "";

        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;

        try
        {
            //open connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();

            string sql = "SELECT SurveyID FROM spirit.DatalotAssociatedSurveyGroup WHERE GroupID = " + groupID + ";";

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            int surveyCnt = 0;

            List<string> surveyIDList = new List<string>();

            while (rdr.Read())
            {
                surveyCnt++;
                surveyIDList.Add((string)rdr["SurveyID"].ToString());
            }

            for (int i = 0; i < surveyIDList.Count; i++)
            {
                sql = "SELECT Name FROM spirit.DatalotSurvey WHERE SurveyID = " + surveyIDList[i] + ";";

                //Execute Command
                cmd = new OleDbCommand(sql, db);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    surveys += (i + 1) + ". " + (string)rdr["Name"].ToString() + " <br />";
                }

            }


            //surveys += surveyCnt + ". " + (string)rdr["Name"].ToString() + " <br />";
            if (surveyCnt == 0)
            {
                surveys = "No assigned Surveys...";
            }
            return surveys;
        }

        catch (Exception delExc)
        {
            lblErr.Text = "Error! Unable to get assigned Survey..." + delExc.Message + delExc.StackTrace;
        }

        finally
        {
            db.Close();

        }

        return surveys;

    }

    private void displayResult(string result)
    {
        lblResult.Visible = true;
        lblResult.Text = result;
        ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblResult.ClientID + "').style.display='none'\",2000)</script>");
    }

    protected void backButton(object sender, EventArgs e)
    {
        Response.Redirect("manageGroups.aspx");
    }

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
        string groupName = txtGroupName.Text;

        //Escape Single Quote
        groupName = escapeSingleQuote(groupName);

        //displayResult("Searching '" + surveyName + "'...");
        Session["search"] = groupName;
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
            String sql = "SELECT Name,GroupID,GrpActivationDate,GrpEndDate,Category FROM spirit.DatalotSurveyGroup S WHERE S.Name LIKE '%" + groupName + "%';";

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            String surveyNames = "";

            List<string> surveys = new List<string>();
            //Read
            int surveyCnt = 0;

            List<string> groupNameArray = (List<string>)Session["groupNameArray"];

            //List<mySurvey> currentSurveyArray = (List<mySurvey>)Session["surveyArray"];

            List<myGroupEdit> currentGroupArray = (List<myGroupEdit>)Session["groupArray"];

            groupNameArray.Clear();
            currentGroupArray.Clear();



            //Temp Variables to build Survey classes
            string gName = "";
            string gID = "";

            string actDate = "";
            string endDate = "";
            string category = "";


            while (rdr.Read())
            {


                //Build Survey Class
                gName = (string)rdr["Name"].ToString();
                gID = (string)rdr["GroupID"].ToString();
                actDate = (string)rdr["GrpActivationDate"].ToString();
                endDate = (string)rdr["GrpEndDate"].ToString();
                category = (string)rdr["Category"].ToString();

                DateTime aDate = Convert.ToDateTime(actDate);
                DateTime eDate = Convert.ToDateTime(endDate);

                //Testing
                //lblTester.Text = "ADATE: " + aDate;

                DateTime today = DateTime.Now;

                if (eDate.CompareTo(today) < 0)
                {
                    gName += " (Expired)";
                }
                else if (aDate.CompareTo(today) > 0)
                {
                    gName += " (Not Active Yet)";
                }
                else
                {
                    gName += "(Active)";
                }

                //Make sure data is clean
                if (category == null || category == "")
                {
                    category = "Public";
                }


                myGroupEdit tempGroup = new myGroupEdit(gID, gName, actDate, endDate, category);

                currentGroupArray.Add(tempGroup);

                surveyCnt++;
                surveyNames += gName + "\t";
                surveys.Add(gName);

                //Add to temp array
                groupNameArray.Add(gName);
            }

            Session["groupNameArray"] = groupNameArray;
            Session["groupArray"] = currentGroupArray;

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
            lblsn.Text = "Group Names";
            cellFirst.Controls.Clear();
            cellFirst.Controls.Add(lblsn);

            HtmlTableCell cellFirst2 = new HtmlTableCell();
            cellFirst2.ID = "cell0";
            cellFirst2.Width = "350px";

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
                lblSurveyName.Text = groupNameArray[i];
                cell.Controls.Clear();
                cell.Controls.Add(lblSurveyName);

                //Button Cell
                HtmlTableCell cell2 = new HtmlTableCell();
                cell2.ID = "cell22" + i.ToString();
                cell2.Width = "275px";

                Button btnViewSurvey = new Button();
                btnViewSurvey.ID = "btn00ID" + i.ToString();
                btnViewSurvey.Text = "View Group";



                //*****************BUTTON METHOD************************************************************
                btnViewSurvey.Click += (s, e2) => { //your code;

                    //No need to describe button as Postback will take place and button function is defined there anyway

                };


                Button btnDeleteSurvey = new Button();
                btnDeleteSurvey.ID = "btn22ID" + i.ToString();
                btnDeleteSurvey.Text = "Delete Group";
                btnDeleteSurvey.OnClientClick = "javascript: return confirm('Are you sure you want to delete the Survey?'); ";
                btnDeleteSurvey.Click += (s, e2) => { //your code;

                    //No need to describe button as Postback will take place and button function is defined there anyway

                };

                Button btnGetResponse = new Button();
                btnGetResponse.ID = "btn11ID" + i.ToString();
                btnGetResponse.Text = "Get Responses";
                btnGetResponse.Click += (s, e2) =>
                { //your code;

                };


                //Add Buttons
                cell2.Controls.Clear();
                cell2.Controls.Add(btnViewSurvey);
                cell2.Controls.Add(btnGetResponse);
                cell2.Controls.Add(btnDeleteSurvey);

                //Add cell to row
                row.Cells.Add(cell);
                row.Cells.Add(cell2);

                //Add row to table
                myTable.Rows.Add(row);


            }



            if (surveyCnt == 0)
            {
                displayResult("No Groups Found");
                myTable.Visible = false;
                surveyTable.Visible = false;
                lblDetails.Visible = false;
                txtGroupName.Focus();
            }
            else
            {
                displayResult("Found " + surveyCnt + " record(s)..");
                myTable.Visible = true;
            }





        }
        catch (Exception findExc)
        {
            lblErr.Text = "Error! Unable to find Survey Group..." + findExc.Message + findExc.StackTrace;
        }

        finally
        {
            db.Close();
        }
    }


    protected void searchButton(object sender, EventArgs e)
    {


        search();


    }
}

internal class myRecord
{
    private string techfitID;
    private List<string> answerChoices;

    public myRecord(string tid, List<string> ac)
    {
        this.techfitID = tid;
        this.answerChoices = new List<string>();
        this.answerChoices = ac;
    }

    public string getTechfitID()
    {
        return this.techfitID;
    }

    public List<string> getAnswerList()
    {
        return this.answerChoices;
    }
}

internal class SurveyResponse
{
    private List<myRecord> surveyResponses;
    private string surveyID;
    private string surveyName;

    public SurveyResponse(string sid, List<myRecord> sr)
    {
        this.surveyID = sid;
        this.surveyResponses = new List<myRecord>();
        this.surveyResponses = sr;
    }
    public List<myRecord> getRecordList()
    {
        return this.surveyResponses;
    }
    public string getSurveyID()
    {
        return this.surveyID;
    }
}


internal class myGroupEdit
{
    private string ID;
    private string name;
    private string activationDate;
    private string endDate;
    private string category;

    public myGroupEdit(string i, string n)
    {
        this.ID = i;
        this.name = n;
    }

    public myGroupEdit(string i, string n, string a, string e, string c)
    {
        this.ID = i;
        this.name = n;
        this.activationDate = a;
        this.endDate = e;
        this.category = c;
    }

    public string getGroupID()
    {
        return this.ID;
    }

    public string getName()
    {
        return this.name;
    }

    public string getActivationDate()
    {
        return this.activationDate;
    }

    public string getEndDate()
    {
        return this.endDate;
    }
    public string getCategory()
    {
        return this.category;

    }
}
