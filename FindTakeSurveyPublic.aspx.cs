using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Data.OleDb;

public partial class FindTakeSurveyPublic : System.Web.UI.Page
{

    private String DBConnectionString = Configuration.ConnectionString;
    //"Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=dev.techfit;Integrated Security=SSPI";


    protected void Page_Load(object sender, EventArgs e)
    {
        lblTester.Visible = false;
        if (Session["UserID"] == null || ((string)Session["Type"] != "admin" && (string)Session["Type"] != "Teachers" && (string)Session["Type"] != "Students"))
        {
            Response.Redirect("DatalotLogin.aspx?err=You must log in to view student records");
        }
        Page.MaintainScrollPositionOnPostBack = true;

        getInfo();

        if (!IsPostBack)
        {
            Session["groupArray"] = new List<myGroupPublic>();
            Session["surveyArray"] = new List<mySurveyPublic>();
            myTable.Visible = false;
            surveyTable.Visible = false;
            myGroupTable.Visible = false;
            lblDetails.Visible = false;
            
        }

        if (Session["groupArray"] == null)
        {
            //lblTester.Text = "joke";
            Session["groupArray"] = new List<myGroupPublic>();
            myTable.Visible = false;
            myGroupTable.Visible = false;

            //Get the current active groups...
            getActiveGroups();
        }
        else
        {

            //Get the current active groups...
            getActiveGroups();

            myTable.Rows.Clear();
            myTable.Visible = false;


            //TIME TO ADD THE GROUPS*************************************************
            List<myGroupPublic> tempGroupArray = (List<myGroupPublic>)Session["groupArray"];

            myGroupTable.Visible = true;

            myGroupTable.Rows.Clear();

            //Add First Row
            //Create Row
            HtmlTableRow firstGroupRow = new HtmlTableRow();
            firstGroupRow.ID = "firstGroupRow";
            //Cell1 (Label)
            HtmlTableCell cellFirstGroup = new HtmlTableCell();
            cellFirstGroup.ID = "cell00G";
            cellFirstGroup.Width = "200px";


            Label lblgn = new Label();
            lblgn.Text = "Survey Names";
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
                btnViewSurvey.ID = "btn00" + i.ToString() + "G";
                btnViewSurvey.Width = 100;

                //string btnText = getBtnText(tempGroupArray[i].getGroupID());

                string btnText = hasTakenSurvey(tempGroupArray[i].getGroupID());

                btnViewSurvey.Text = btnText;

                

                //*****************GROUP METHOD**************
                btnViewSurvey.Click += (s, e2) => { //your code;

                    //BUTTON FUNCTION
                    Button btnTemp = s as Button;

                    //Get index for the surveyArray
                    char c = btnTemp.ID[btnTemp.ID.Length - 2];
                    int index = Int32.Parse(c.ToString());

                    List<myGroupPublic> tempGroupArrayTake = (List<myGroupPublic>)Session["groupArray"];

                    Response.Redirect("takeSurveyTeachers.aspx?GroupID=" + tempGroupArrayTake[index].getGroupID());

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

            if (tempGroupArray.Count == 0)
            {
                myGroupTable.Visible = false;
            }

        }
    }

    private string hasTakenSurvey(string groupID)
    {
        string userID = (string)Session["UserID"];

        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;

        try
        {
            //open connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();


            string sql = "SELECT * FROM spirit.DatalotSurveyStatusNew S WHERE S.GroupID = " + groupID + " AND S.TechfitID = " + userID;

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                return "Retake Survey";

            }

            return "Start Survey";
        }
        catch (Exception loadExc)
        {
            lblErr.Text = "Error! Unable to find Survey Status..." + loadExc.Message + loadExc.StackTrace;
        }
        finally
        {
            db.Close();
        }

        return "Start Survey";
    }

    private void getInfo()
    {
        string text = Request.QueryString["getInfo"];
        lblGetInfo.Text = text;
        ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblGetInfo.ClientID + "').style.display='none'\",5000)</script>");
    }

    private void displayResult(string result)
    {
        lblResult.Visible = true;
        lblResult.Text = result;
        ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblResult.ClientID + "').style.display='none'\",2000)</script>");
    }

    protected void logoutButton(object sender, EventArgs e)
    {
        Session["Type"] = null;
        Session["Name"] = null;
        Session["UserID"] = null;

        Response.Redirect("DatalotLogin.aspx");
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

    private void getActiveGroups ()
    {

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



            //GROUP CODE START
            string groupID = "";
            string groupName = "";
            string grpActivationDate = "";
            string grpEndDate = "";
            string category = "";
            string statusCheck = "";

            List<myGroupPublic> tempGroupArray = (List<myGroupPublic>)Session["groupArray"];
            tempGroupArray.Clear();

            //Today's Date
            DateTime today = DateTime.Now;

            //sql = "SELECT GroupID, Name, GrpActivationDate, GrpEndDate FROM spirit.DatalotSurveyGroup SG WHERE SG.Name LIKE '%" + surveyName + "%';";


            string sql = "SELECT GroupID, Name, GrpActivationDate, GrpEndDate, Category, StatusCheck FROM spirit.DatalotSurveyGroup SG WHERE SG.GrpActivationDate <= '" + today + "' AND SG.GrpEndDate >= '" + today + "';";
            lblResult.Text = sql;

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();


            int groupCnt = 0;
            while (rdr.Read())
            {
                

                groupName = (string)rdr["Name"].ToString();
                groupID = (string)rdr["GroupID"].ToString();
                grpActivationDate = (string)rdr["GrpActivationDate"].ToString();
                grpEndDate = (string)rdr["GrpEndDate"].ToString();
                category = (string)rdr["Category"].ToString();
                statusCheck = (string)rdr["StatusCheck"].ToString();

                string userCategory = (string)Session["Type"];
                myGroupPublic currentGrp = new myGroupPublic(groupID, groupName, grpActivationDate, grpEndDate, category, statusCheck);

                bool valid = false;
                

                if (category.Contains(userCategory) || category == "Public" || category == "Anonymous")
                {
                    groupCnt++;
                    tempGroupArray.Add(currentGrp);
                }
              
            }


            //Group
            Session["groupArray"] = tempGroupArray;


            //Clear all rows and remove visibility for survey table as we won't be using it
            myTable.Rows.Clear();
            myTable.Visible = false;

            myGroupTable.Visible = true;

            //Clear all Rows
            myGroupTable.Rows.Clear();

            //Add First Row
            //Create Row
            HtmlTableRow firstGroupRow = new HtmlTableRow();
            firstGroupRow.ID = "firstGroupRow";
            //Cell1 (Label)
            HtmlTableCell cellFirstGroup = new HtmlTableCell();
            cellFirstGroup.ID = "cell00G";
            cellFirstGroup.Width = "222px";


            Label lblgn = new Label();
            lblgn.Text = "Survey Names";
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
                btnViewSurvey.ID = "btn00" + i.ToString() + "G";
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
                myGroupTable.Rows.Add(row);
            }



            if (groupCnt == 0)
            {
                displayResult("No Active Surveys Found");
                myTable.Visible = false;
                surveyTable.Visible = false;
                myGroupTable.Visible = false;
                lblDetails.Visible = false;
            }
            else
            {
                displayResult("Found " + (groupCnt) + " Active Surveys..");
                myGroupTable.Visible = true;
            }
        }
        catch (Exception getGroupsExc)
        {
            lblErr.Text = "Error! Unable to find Survey..." + getGroupsExc.Message + getGroupsExc.StackTrace;
        }
        finally
        {
            db.Close();
        }
    }

    private void search()
    {
        lblDetails.Visible = false;

        getActiveGroups();
    }


    protected void searchButton(object sender, EventArgs e)
    {
        search();
    }
}

internal class myGroupPublic
{
    private string groupID;
    private string groupName;
    private string grpActivationDate;
    private string grpEndDate;
    private string category;
    private string statusCheck;

    public myGroupPublic(string i, string n, string ad, string ed, string c, string sc)
    {
        groupID = i;
        groupName = n;
        grpActivationDate = ad;
        grpEndDate = ed;
        category = c;
        statusCheck = sc;
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
    public string getCategory()
    {
        return this.category;
    }
    public string getStatusCheck()
    {
        return this.statusCheck;
    }
}

internal class mySurveyPublic
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

    public mySurveyPublic(string n, string ad, string ed, string p, string cd, string id)
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