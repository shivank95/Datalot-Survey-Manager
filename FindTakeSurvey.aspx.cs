using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Data.OleDb;

public partial class FindTakeSurvey : System.Web.UI.Page
{

    private String DBConnectionString = Configuration.ConnectionString;
    //"Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=dev.techfit;Integrated Security=SSPI";


    protected void Page_Load(object sender, EventArgs e)
    {
        //No More Testing
        lblTester.Visible = false;

        if (Session["UserID"] == null || ((string)Session["Type"] != "admin" && (string)Session["Type"] != "Teacher"))
        {
            Response.Redirect("DatalotAdminLogin.aspx?err=You must log in to view student records");
        }


        Page.MaintainScrollPositionOnPostBack = true;
        if (!IsPostBack)
        {

            //lblTester.Text = "joke";
            Session["buttonArray"] = new List<Button>();
            Session["surveyNameArray"] = new List<string>();
            Session["groupArray"] = new List<myGroup>();
            Session["surveyArray"] = new List<mySurvey>();
            myTable.Visible = false;
            surveyTable.Visible = false;
            myGroupTable.Visible = false;
            lblDetails.Visible = false;
            
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
                btnViewSurvey.Text = "Take Survey";




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

                    List<mySurvey> surveyArrayTemp = (List<mySurvey>)Session["surveyArray"];

                    Response.Redirect("takeSurvey.aspx?SurveyID=" + surveyArrayTemp[index].getID());

                };

                cell2.Controls.Clear();
                cell2.Controls.Add(btnViewSurvey);


                //Add cell to row
                row.Cells.Add(cell);
                row.Cells.Add(cell2);

                //Add row to table
                myTable.Rows.Add(row);


            }


            //TIME TO ADD THE GROUPS*************************************************
            List<myGroup> tempGroupArray = (List<myGroup>)Session["groupArray"];

            myGroupTable.Visible = true;

            myGroupTable.Rows.Clear();

            //Add First Row
            //Create Row
            HtmlTableRow firstGroupRow = new HtmlTableRow();
            firstRow.ID = "firstGroupRow";
            //Cell1 (Label)
            HtmlTableCell cellFirstGroup = new HtmlTableCell();
            cellFirstGroup.ID = "cell00G";
            cellFirstGroup.Width = "200px";


            Label lblgn = new Label();
            lblgn.Text = "Group Names";
            cellFirstGroup.Controls.Clear();
            cellFirstGroup.Controls.Add(lblgn);

            //Button Cell
            HtmlTableCell cellFirst2Group = new HtmlTableCell();
            cellFirst2Group.ID = "cell0G";
            cellFirst2Group.Width = "275px";

            firstGroupRow.Cells.Add(cellFirstGroup);
            firstGroupRow.Cells.Add(cellFirst2Group);

            //Add row to table
            myGroupTable.Rows.Add(firstGroupRow);
            for (int i = 0; i < tempGroupArray.Count; i++)
            {
                //Create Row
                HtmlTableRow row = new HtmlTableRow();
                row.ID = "row0" + i.ToString() + "G";

                //Cell1 (Label)
                HtmlTableCell cell = new HtmlTableCell();
                cell.ID = "cell11" + i.ToString() + "G";
                cell.Width = "200px";

                Label lblGroupName = new Label();
                lblGroupName.Text = tempGroupArray[i].getGroupName();
                cell.Controls.Clear();
                cell.Controls.Add(lblGroupName);

                //Button Cell
                HtmlTableCell cell2 = new HtmlTableCell();
                cell2.ID = "cell22" + i.ToString() + "G";
                cell2.Width = "275px";

                Button btnViewSurvey = new Button();
                btnViewSurvey.ID = "btn00ID" + i.ToString() + "G";
                btnViewSurvey.Text = "Take Survey";



                //*****************GROUP METHOD**************
                btnViewSurvey.Click += (s, e2) => { //your code;

                    //BUTTON FUNCTION
                    Button btnTemp = s as Button;

                    //Process of getting the index of the survey in the array
                    string btnID = btnTemp.ID;
                    btnID = btnID.Substring(0, btnID.Length - 1);
                    int index = getIndex(btnID);
                    if (index == -1)
                    {
                        return;
                    }
                    
                    List<myGroup> tempGroupArrayTake = (List<myGroup>)Session["groupArray"];

                    Response.Redirect("takeSurvey.aspx?GroupID=" + tempGroupArrayTake[index].getGroupID());

                };

                //Add Buttons
                cell2.Controls.Clear();
                cell2.Controls.Add(btnViewSurvey);
                

                //Add cell to row
                row.Cells.Add(cell);
                row.Cells.Add(cell2);

                //Add row to table
                myGroupTable.Rows.Add(row);
            }



            if (surveyNameArray.Count == 0)
            {
                myTable.Visible = false;
            }
            if (tempGroupArray.Count == 0)
            {
                myGroupTable.Visible = false;
            }

        }
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
        //lTester.Text = "" + index;


        return index;
    }

    private void getActiveGroups()
    {

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

            //GROUP CODE START
            string groupID = "";
            string groupName = "";
            string grpActivationDate = "";
            string grpEndDate = "";

            List<myGroup> tempGroupArray = (List<myGroup>)Session["groupArray"];
            tempGroupArray.Clear();

            sql = "SELECT GroupID, Name, GrpActivationDate, GrpEndDate FROM spirit.DatalotSurveyGroup SG WHERE SG.Name LIKE '%" + surveyName + "%';";

            //Execute Command
            cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();


            int groupCnt = 0;
            while (rdr.Read())
            {
                groupCnt++;

                groupName = (string)rdr["Name"].ToString();
                groupID = (string)rdr["GroupID"].ToString();
                grpActivationDate = (string)rdr["GrpActivationDate"].ToString();
                grpEndDate = (string)rdr["GrpEndDate"].ToString();

                myGroup currentGrp = new myGroup(groupID, groupName, grpActivationDate, grpEndDate);

                tempGroupArray.Add(currentGrp);

                //Testing
                //lblTester.Text += "<br /> " + groupName;
            }


            //Group
            Session["groupArray"] = tempGroupArray;

            
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
                btnViewSurvey.Text = "Take Survey";



                //*****************BUTTON METHOD************************************************************
                btnViewSurvey.Click += (s, e2) => { //your code;

                    //No need to describe button as Postback will take place and button function is defined there anyway

                };


                //Add Buttons
                cell2.Controls.Clear();
                cell2.Controls.Add(btnViewSurvey);
                //cell2.Controls.Add(btnEditSurvey);
                //cell2.Controls.Add(btnDeleteSurvey);

                //Add cell to row
                row.Cells.Add(cell);
                row.Cells.Add(cell2);

                //Add row to table
                myTable.Rows.Add(row);


            }


            //TIME TO ADD THE GROUPS*************************************************************************************
            //List<myGroup> tempGroupArray = (List<myGroup>)Session["groupArray"];

            myGroupTable.Visible = true;

            //Clear all Rows
            myGroupTable.Rows.Clear();

            //Add First Row
            //Create Row
            HtmlTableRow firstGroupRow = new HtmlTableRow();
            firstRow.ID = "firstGroupRow";
            //Cell1 (Label)
            HtmlTableCell cellFirstGroup = new HtmlTableCell();
            cellFirstGroup.ID = "cell00G";
            cellFirstGroup.Width = "222px";


            Label lblgn = new Label();
            lblgn.Text = "Group Names";
            cellFirstGroup.Controls.Clear();
            cellFirstGroup.Controls.Add(lblgn);

            //Button Cell
            HtmlTableCell cellFirst2Group = new HtmlTableCell();
            cellFirst2Group.ID = "cell0G";
            cellFirst2Group.Width = "222px";

            firstGroupRow.Cells.Add(cellFirstGroup);
            firstGroupRow.Cells.Add(cellFirst2Group);

            //Add row to table
            myGroupTable.Rows.Add(firstGroupRow);

            for (int i = 0; i < tempGroupArray.Count; i++)
            {
                //Create Row
                HtmlTableRow row = new HtmlTableRow();
                row.ID = "row0" + i.ToString() + "G";

                //Cell1 (Label)
                HtmlTableCell cell = new HtmlTableCell();
                cell.ID = "cell11" + i.ToString() + "G";
                cell.Width = "200px";

                Label lblGroupName = new Label();
                lblGroupName.Text = tempGroupArray[i].getGroupName();
                cell.Controls.Clear();
                cell.Controls.Add(lblGroupName);

                //Button Cell
                HtmlTableCell cell2 = new HtmlTableCell();
                cell2.ID = "cell22" + i.ToString() + "G";
                cell2.Width = "275px";

                Button btnViewSurvey = new Button();
                btnViewSurvey.ID = "btn00ID" + i.ToString() + "G";
                btnViewSurvey.Text = "Take Survey";



                //*****************BUTTON METHOD************************************************************
                btnViewSurvey.Click += (s, e2) => { //your code;

                    //No need to describe button as Postback will take place and button function is defined there anyway

                };

                //Add Buttons
                cell2.Controls.Clear();
                cell2.Controls.Add(btnViewSurvey);

                //Add cell to row
                row.Cells.Add(cell);
                row.Cells.Add(cell2);

                //Add row to table
                myGroupTable.Rows.Add(row);
            }



            if (surveyCnt == 0 && groupCnt == 0)
            {
                displayResult("No Surveys/Groups Found");
                myTable.Visible = false;
                surveyTable.Visible = false;
                myGroupTable.Visible = false;
                lblDetails.Visible = false;
                txtSurveyName.Focus();
            }
            else
            {
                displayResult("Found " + (surveyCnt+groupCnt) + " record(s)..");
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

internal class myGroup
{
    private string groupID;
    private string groupName;
    private string grpActivationDate;
    private string grpEndDate;

    public myGroup(string i, string n, string ad, string ed)
    {
        groupID = i;
        groupName = n;
        grpActivationDate = ad;
        grpEndDate = ed;
    }

    public string getGroupID()
    {
        return this.groupID;
    }

    public string getActivationDate()
    {
        return this.grpActivationDate;
    }

    public string getEndDate()
    {
        return this.grpEndDate;
    }

    public string getGroupName()
    {
        return this.groupName;
    }
}

internal class mySurvey
{

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