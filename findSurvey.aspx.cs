using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Data.OleDb;

public partial class findSurvey : System.Web.UI.Page
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

            //lblTester.Text = "joke";
            Session["buttonArray"] = new List<Button>();
            Session["surveyNameArray"] = new List<string>();
            Session["surveyArray"] = new List<mySurvey>();
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
            Session["surveyArray"] = new List<mySurvey>();
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
                    int newIndex = getIndex(btnID);
                    if (newIndex == -1)
                    {
                        return;
                    }
                    
                    //Load Tables Function
                    string loadType = "view";
                    loadTables(newIndex, loadType);

                    
                };



                Button btnEditSurvey = new Button();
                btnEditSurvey.ID = "btn11ID" + i.ToString();
                btnEditSurvey.Text = "Edit Survey";
                btnEditSurvey.Click += (s, e2) => { //your code;

                    //No need to describe button as Postback will take place and button function is defined there anyway
                    //BUTTON FUNCTION
                    Button btnTemp = s as Button;

                    //Process of getting the index of the survey in the array
                    string btnID = btnTemp.ID;
                    int newIndex = getIndex(btnID);
                    if (newIndex == -1)
                    {
                        return;
                    }

                    List<mySurvey> surveyArrayTemp = (List<mySurvey>)Session["surveyArray"];
                    Response.Redirect("editSurvey.aspx?SurveyID="+surveyArrayTemp[newIndex].getID());


                };

                Button btnDeleteSurvey = new Button();
                btnDeleteSurvey.ID = "btn22ID" + i.ToString();
                btnDeleteSurvey.Text = "Delete Survey";
                btnDeleteSurvey.OnClientClick = "javascript: return confirm('Are you sure you want to delete the Survey?'); ";
                btnDeleteSurvey.Click += (s, e2) => { //your code;

                    lblQuestions.Visible = false;

                    //BUTTON FUNCTION
                    Button btnTemp = s as Button;

                    //Process of getting the index of the survey in the array
                    string btnID = btnTemp.ID;
                    int newIndex = getIndex(btnID);
                    if (newIndex == -1)
                    {
                        return;
                    }

                    //Get index for the surveyArray
                    //char c = btnTemp.ID[btnTemp.ID.Length - 1];
                    //int index = Int32.Parse(c.ToString());

                    List<mySurvey> surveyArrayTemp = (List<mySurvey>)Session["surveyArray"];

                    string currentSurveyID = surveyArrayTemp[newIndex].getID();

                    //Initiate OleDb
                    OleDbConnection db = null;
                    OleDbDataReader rdr = null;

                    try
                    {
                        //open connection
                        db = new OleDbConnection();
                        db.ConnectionString = DBConnectionString;
                        db.Open();

                        //Delete Question Responses First
                        string sql = "DELETE FROM spirit.DatalotQuestionResponse WHERE SurveyID = " + currentSurveyID + ";";

                        //Execute Command
                        OleDbCommand cmd = new OleDbCommand(sql, db);
                        rdr = cmd.ExecuteReader();

                        //DELETE AssociatedSurveyGroups
                        sql = "DELETE FROM spirit.DatalotAssociatedSurveyGroup WHERE SurveyID = " + currentSurveyID + ";";

                        //Execute Command
                        cmd = new OleDbCommand(sql, db);
                        rdr = cmd.ExecuteReader();

                        
                        //Delete Questions First
                        sql = "DELETE FROM spirit.DatalotQuestion WHERE SurveyID =" + currentSurveyID + ";";

                        //Execute Command
                        cmd = new OleDbCommand(sql, db);
                        rdr = cmd.ExecuteReader();

                        sql = "DELETE FROM spirit.DatalotSurvey WHERE SurveyID =" + currentSurveyID + ";";

                        //Execute Command
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
                cell2.Controls.Add(btnEditSurvey);
                cell2.Controls.Add(btnDeleteSurvey);
                

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

    private string getGroupList(string surveyID)
    {

        string currentGroupNames = "";
        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;

        try
        {
            //open connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();

            string sql = "SELECT GroupID FROM spirit.DatalotAssociatedSurveyGroup WHERE SurveyID = " + surveyID + ";";

            List<string> groupIDList = new List<string>();

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            int groupCnt = 0;

            while (rdr.Read())
            {
                groupCnt++;
                groupIDList.Add((string)rdr["GroupID"].ToString());
            }

            for (int i = 0; i < groupIDList.Count; i++)
            {
                sql = "SELECT Name FROM spirit.DatalotSurveyGroup WHERE GroupID = " + groupIDList[i] + ";";

                //Execute Command
                cmd = new OleDbCommand(sql, db);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    currentGroupNames += (i + 1) + ". " + (string)rdr["Name"].ToString() + " <br />";
                }

            }
            
            if (groupCnt == 0)
            {
                currentGroupNames = "No Groups Assigned Yet..";
            }

            return currentGroupNames;
        }

        catch (Exception groupNameExc)
        {
            lblResult.Text = "Error! Unable to get assigned Survey..." + groupNameExc.Message + groupNameExc.StackTrace;
        }
        finally
        {
            db.Close();
        }

        return "Not Assigned..";


    }

    public void loadTables(int index, string loadType)
    {

        List<mySurvey> surveyArrayTemp = (List<mySurvey>)Session["surveyArray"];
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

        //Row2 (SURVEY Group)
        HtmlTableRow surveyGroupRow = new HtmlTableRow();
        surveyGroupRow.ID = "surveyGroupRow";
        //Cell1 (Label)
        HtmlTableCell cellGroupName = new HtmlTableCell();
        cellGroupName.Width = "222px";

        Label lblSurveyTable2 = new Label();
        lblSurveyTable2.Text = "Group Name(s): ";
        cellGroupName.Controls.Clear();
        cellGroupName.Controls.Add(lblSurveyTable2);

        HtmlTableCell cellSurveyGroupValue = new HtmlTableCell();
        cellSurveyGroupValue.Width = "222px";

        Label lblSurveyTable2Value = new Label();
        string groupName = getGroupList(surveyArrayTemp[index].getID());
        lblSurveyTable2Value.Text = groupName;
        cellSurveyGroupValue.Controls.Clear();
        cellSurveyGroupValue.Controls.Add(lblSurveyTable2Value);

        surveyGroupRow.Cells.Add(cellGroupName);
        surveyGroupRow.Cells.Add(cellSurveyGroupValue);

        surveyTable.Rows.Add(surveyGroupRow);
        //Row2 END

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
        //Row6 Creation


        //View Questions
        viewQuestions(surveyArrayTemp[index].getID());

        surveyTable.Visible = true;
        lblDetails.Visible = true;
    }

    private void viewQuestions(string surveyID)
    {
        lblQuestions.Visible = true;

        lblQuestions.Text = "Survey Questions: <br />";

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

            string sql = "Select Question, Qtype, Choice1, Choice2, Choice3, Choice4, Choice5 FROM spirit.DatalotQuestion WHERE SurveyID = " + surveyID + " ORDER BY QNumber;";

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            int cnt = 0;
            while (rdr.Read())
            {
                cnt++;
                string type = (string)rdr["QType"].ToString();
                if (type.Contains("MCQ"))
                {
                    lblQuestions.Text += " <br /> Question " + cnt + " (MCQ): " + (string)rdr["Question"].ToString() + "";
                    string ch1 = (string)rdr["Choice1"].ToString();
                    string ch2 = (string)rdr["Choice2"].ToString();
                    string ch3 = (string)rdr["Choice3"].ToString();
                    string ch4 = (string)rdr["Choice4"].ToString();
                    string ch5 = (string)rdr["Choice5"].ToString();

                    lblQuestions.Text += evaluateChoice(ch1, 1);
                    lblQuestions.Text += evaluateChoice(ch2, 2);
                    lblQuestions.Text += evaluateChoice(ch3, 3);
                    lblQuestions.Text += evaluateChoice(ch4, 4);
                    lblQuestions.Text += evaluateChoice(ch5, 5);



                }
                else
                {
                    lblQuestions.Text += " <br /> Question " + cnt + " (SAQ): " + (string)rdr["Question"].ToString() + "";
                }


                lblQuestions.Text += "<br />";
            }
        }

        catch (Exception exc)
        {
            lblErr.Text = "Error! Unable to find Survey..." + exc.Message + exc.StackTrace;
        }

        finally
        {
            db.Close();
        }

    }

    private string evaluateChoice(string choice, int cnt)
    {
        if (choice == null || choice == "")
        {
            return "";
        }
        else
        {
            return "<br />    " + "Choice " + cnt + ": " + choice;
        }
    }

    private void displayResult(string result)
    {
        lblResult.Visible = true;
        lblResult.Text = result;
        ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblResult.ClientID + "').style.display='none'\",2000)</script>");
    }

    protected void backButton(object sender, EventArgs e)
    {
        Response.Redirect("Datalot.aspx");
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
        lblQuestions.Visible = false;
        string surveyName = txtSurveyName.Text;

        //Escape Single Quote
        surveyName = escapeSingleQuote(surveyName);

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

            List<mySurvey> currentSurveyArray = (List<mySurvey>)Session["surveyArray"];

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
                mySurvey tempSurvey = new mySurvey(sname, activationDate, endDate, parts, creationDate, sID);

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

                Button btnEditSurvey = new Button();
                btnEditSurvey.ID = "btn11ID" + i.ToString();
                btnEditSurvey.Text = "Edit Survey";
                btnEditSurvey.Click += (s, e2) => { //your code;

                    //No need to describe button as Postback will take place and button function is defined there anyway

                };

                Button btnDeleteSurvey = new Button();
                btnDeleteSurvey.ID = "btn22ID" + i.ToString();
                btnDeleteSurvey.Text = "Delete Survey";
                btnDeleteSurvey.OnClientClick = "javascript: return confirm('Are you sure you want to delete the Survey?'); ";
                btnDeleteSurvey.Click += (s, e2) => { //your code;

                    //No need to describe button as Postback will take place and button function is defined there anyway

                };


                //Add Buttons
                cell2.Controls.Clear();
                cell2.Controls.Add(btnViewSurvey);
                cell2.Controls.Add(btnEditSurvey);
                cell2.Controls.Add(btnDeleteSurvey);

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

internal class mySurvey
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

    public mySurvey(string n, string ad, string ed, string p, string cd, string id)
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