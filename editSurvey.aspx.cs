using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Data.OleDb;

public partial class editSurvey : System.Web.UI.Page
{

    private String surveyName = "";
    private static String surveyGrp = "";
    private String DBConnectionString = Configuration.ConnectionString;

    //"Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=dev.techfit;Integrated Security=SSPI";
    //private string[] lastQuestionType;
    //public static int questCnt = 0;

    //static List<MCQ> mcqArray = new List<MCQ>();

    //static List<SAQ> saqArray = new List<SAQ>();

    bool needSave = false;

   

    protected void Page_Load(object sender, EventArgs e)
    {

        //No more Testing
        lblTester.Visible = false;

        //May use this in the future
        btnUndo.Visible = false;

        if (Session["UserID"] == null || (string)Session["Type"] != "admin")
        {
            Response.Redirect("DatalotAdminLogin.aspx?err=You must log in to view student records");
        }
        //myTable.Visible = false;
        lblTester.Visible = false;
        //myTable2.Visible = false;
        Page.MaintainScrollPositionOnPostBack = true;


        btnUndo.Enabled = false;
        
        if (!IsPostBack)
        {
            //Initial Values
            //questCnt = 0;
            myTable.Visible = false;
            myTable2.Visible = false;
            mySAQEditTable.Visible = false;
            myMCQEditTable.Visible = false;
            //Initializing session variables
            Session["questCnt"] = 0;
            Session["mcqArray"] = new List<MCQEdit>();
            Session["saqArray"] = new List<SAQEdit>();
            Session["lastQuestionType"] = new List<string>();

            //Survey Criticals
            Session["dateReady"] = false;
            Session["activationDate"] = new DateTime();
            Session["endDate"] = new DateTime();
            Session["participants"] = "";
            Session["surveyGroup"] = "";
            Session["SurveyID"] = "";

            //For Editing individual questions
            Session["listQuestions"] = new List<ListItem>();
            Session["EditingQuestionNum"] = 0;

            //Get the SurveyID to edit and load everything
            getSurveyID();
        }
    }

    private void updateEditQuestionsList()
    {
        if (Session["listQuestions"] != null)
        {
            List<ListItem> tempList = (List<ListItem>)Session["listQuestions"];


            ddlEditQuestions.Items.Clear();
            ddlEditQuestions.Items.Insert(0, new ListItem("--Select a Question--", ""));
            for (int i = 0; i < tempList.Count; i++)
            {
                ddlEditQuestions.Items.Add(tempList[i]);
            }

        }
    }

    public void getSurveyID()
    {
        Session["SurveyID"] = Request.QueryString["surveyID"];
        string tempID = (string)Session["surveyID"];
        loadSurvey(tempID);
    }

    //Method to load all the details of the exisitng survey and populate the various text boxes that lie within this page
    public void loadSurvey(string tempID)
    {
        //txtSurveyName.Text = tempID;
        String currentSurveyName = "";
        String currentParticipants = "";
        DateTime activationDate = new DateTime();
        DateTime endDate = new DateTime();

        List<ListItem> tempList = (List<ListItem>)Session["listQuestions"];

        ddlEditQuestions.Items.Clear();
        ddlEditQuestions.Items.Insert(0, new ListItem("--Select a Question--", ""));

        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;

        //Get Survey Details
        try
        {
            //Open Connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();

            String sql = "SELECT Name, Participants, ActivationDate, EndDate, CreationDate FROM spirit.DatalotSurvey S WHERE S.SurveyID = " + tempID + ";";

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                currentSurveyName = (string)rdr["Name"].ToString();
                currentParticipants = (string)rdr["Participants"].ToString();
                string tempDate = (string)rdr["ActivationDate"].ToString();
                activationDate = Convert.ToDateTime(tempDate);
                tempDate = (string)rdr["EndDate"].ToString();
                endDate = Convert.ToDateTime(tempDate);

                

            }

            //Change Controls
            //Survey Name
            txtSurveyName.Text = currentSurveyName;
            //Participants
            loadParticipants(currentParticipants);
            //Date lists
            updateDateLists(activationDate, endDate);



            //Time to Get the Questions
            sql = "SELECT QuestionID, Question, Qtype, QNumber, Choice1, Choice2, Choice3, Choice4, Choice5, Choice6, Choice7, Choice8, Choice9, Choice10 FROM spirit.DatalotQuestion WHERE SurveyID = " + tempID + " ORDER BY QNumber ASC;";
            
            //Execute Command
            cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            //Attributes
            string qID;
            string question;
            string qType;
            string qNumber;
            List<string> choices = new List<string>();

            List<SAQEdit> loadSAQ = new List<SAQEdit>();
            List<MCQEdit> loadMCQ = new List<MCQEdit>();

            List<string> tempType = new List<string>();

            int qCountInt = 0;
            while (rdr.Read())
            {
                qCountInt++;
                qID = (string)rdr["QuestionID"].ToString();
                question = (string)rdr["Question"].ToString();
                qType = (string)rdr["Qtype"].ToString();
                qNumber = (string)rdr["QNumber"].ToString();
                choices.Clear();
                for (int i = 1; i <= 10; i++)
                {
                    string attrTemp = "Choice" + i.ToString();

                    string tempChoice = (string)rdr[attrTemp].ToString();

                    choices.Add(tempChoice);
                }

                


                //LAST EDIT
                ddlEditQuestions.Items.Insert(ddlEditQuestions.Items.Count, new ListItem(question, qNumber));
                tempList.Add(new ListItem(question, qNumber));


                if (qType == "SAQ")
                {
                    int currentQNumber = Int32.Parse(qNumber);
                    SAQEdit currentSAQ = new SAQEdit(question, currentQNumber, qID);
                    loadSAQ.Add(currentSAQ);
                    
                }
                else
                {
                    int currentQNumber = Int32.Parse(qNumber);
                    MCQEdit currentMCQ;
                    if (qType == "MCQ")
                    {
                        currentMCQ = new MCQEdit(question, false, currentQNumber, qID);
                    }
                    else
                    {
                        currentMCQ = new MCQEdit(question, true, currentQNumber, qID);
                    }
                    currentMCQ.removeAnswers();                        
                    //Add Answers
                    for (int i = 0; i < choices.Count; i++)
                    {
                        if (choices[i] != null && choices[i] != "" && choices[i] != "temp")
                        {
                            currentMCQ.addAnswer(choices[i]);
                        }
                    }

                    loadMCQ.Add(currentMCQ);           
                }

                //Add to beginning of list for undo function
                tempType.Add(qType);

            }

            //Update Edit Questions List
            Session["listQuestions"] = tempList;

            //Use Old function to update Reviewer
            updateReview(loadMCQ, loadSAQ);

            //Set the global session arrays of these questions to these temporary values
            Session["mcqArray"] = loadMCQ;
            Session["saqArray"] = loadSAQ;

            Session["lastQuestionType"] = tempType;

            Session["questCnt"] = qCountInt;
            lblQCnt.Text = "Number of Questions: " + qCountInt;



        }

        catch (Exception loadExc)
        {
            lblErr.Text = "Error! Unable to load Survey..." + loadExc.Message + loadExc.StackTrace;
        }

        finally
        {
            db.Close();
        }
    }

    private void loadParticipants(string parts)
    {
        ddlParticipants.SelectedValue = parts;
        Session["participants"] = parts;
    }

    private void updateDateLists(DateTime aDate, DateTime eDate)
    {
        //Decode activation date mm/dd/yyyy

        string currentDate = aDate.ToString();

        char[] delimiterChars = { '/', ' '};

        string[] separatedDate = currentDate.Split(delimiterChars);

        string month = separatedDate[0];
        string day = separatedDate[1];
        string year = separatedDate[2];

        ddlStartDay.SelectedValue = day;
        ddlStartMonth.SelectedValue = month;
        ddlStartYear.SelectedValue = year;

        //End Date mm/dd/yyyy
        currentDate = eDate.ToString();
        separatedDate = currentDate.Split(delimiterChars);

        //Testing
        //for (int i = 0; i < separatedDate.Length; i++)
        //{
        //    lblErr.Text += separatedDate[i] + "--";
        //}

        month = separatedDate[0];
        day = separatedDate[1];
        year = separatedDate[2];

        ddlEndDay.SelectedValue = day;
        ddlEndMonth.SelectedValue = month;
        ddlEndYear.SelectedValue = year;

        //Update Session Variables for these Dates
        Session["activationDate"] = aDate;
        Session["endDate"] = eDate;
        Session["dateReady"] = true;


    }

    private void updateLabels()
    {
        //lblQCnt.Text = "Number of Questions: " + saqArray.Count;
    }

    protected void backButton(object sender, EventArgs e)
    {

        Response.Redirect("findSurvey.aspx");
    }

    //Method that updates views based on a drop down list, question type and populates the views with existing values
    protected void updateQuestions(object sender, EventArgs e)
    {

        //The Selected Index will always be the question number
        int questNumber = ddlEditQuestions.SelectedIndex;

        if (questNumber == 0)
        {
            myTable.Visible = false;
            myTable2.Visible = false;
            mySAQEditTable.Visible = false;
            myMCQEditTable.Visible = false;

            return;
        }

        //Testing
        //lblResult.Text = "Quest Num: " + questNumber;

        //Save this for future use becuase editing will take place based on question number and surveyid both of which will be unique for any particular question in a given survey.
        Session["EditingQuestionNum"] = questNumber;
        

        //Find the Question
        List<SAQEdit> tempSAQList = (List<SAQEdit>)Session["saqarray"];
        List<MCQEdit> tempMCQList = (List<MCQEdit>)Session["mcqarray"];

        //bool found = false;
        //string type = "";
        
        List<string> currentAnswers = new List<string>();
        bool multOpts = false;

        for (int i = 0; i < tempSAQList.Count; i++)
        {
            int tempCnt = tempSAQList[i].getQuestNo();
            if (tempCnt == questNumber)
            {
                //Found
                SAQEdit currentSAQ = tempSAQList[i];

                //Update Views
                mySAQEditTable.Visible = true;
                myMCQEditTable.Visible = false;
                myTable.Visible = false;
                myTable2.Visible = false;

                //Update Textbox
                txtSAQEdit.Text = currentSAQ.getQuestion();
                txtQuest.Focus();
                break;
            }
        }

        for (int i = 0; i < tempMCQList.Count; i++)
        {
            int tempCnt = tempMCQList[i].getQuestNo();
            if (tempCnt == questNumber)
            {
                //Found
                MCQEdit currentMCQ = tempMCQList[i];

                myTable.Visible = false;
                myTable2.Visible = false;
                mySAQEditTable.Visible = false;

                myMCQEditTable.Visible = true;


                //Question
                txtMCQEditQuest.Text = currentMCQ.getQuestion();

                ////Answers
                currentAnswers = currentMCQ.getAnswers();
                int len = currentAnswers.Count;
                if (len >= 2)
                {
                    txtChEdit1.Text = currentAnswers[0];
                    txtChEdit2.Text = currentAnswers[1];

                   if (len > 2 && currentAnswers[2] != null)
                    {
                        txtChEdit3.Text = currentAnswers[2];
                    }
                   else
                    {
                        txtChEdit3.Text = "";
                    }

                    if (len > 3 && currentAnswers[3] != null)
                    {
                        txtChEdit4.Text = currentAnswers[3];
                    }
                    else
                    {
                        txtChEdit4.Text = "";
                    }

                    if (len > 4 && currentAnswers[4] != null)
                    {
                        txtChEdit5.Text = currentAnswers[4];
                    }
                    else
                    {
                        txtChEdit5.Text = "";
                    }
                }

                ckbAllowMultEdit.Checked = currentMCQ.getMultOpt();

                break;

            }
        }

    }
    private string getType(int qnumber)
    {
        //Type can be found by seeing which editing table we have visible
        if (mySAQEditTable.Visible == true)
        {
            return "SAQ";
        }
        return "MCQ";
    }
    protected void saveEditQuestion(object sender, EventArgs e)
    {
        //Testing
        //lblResult.Text = "EditingQuestNumber: " + editingQNum;

        int editingQNum = (int)Session["EditingQuestionNum"];
        string qType = getType(editingQNum);

        //Testing
        //lblResult.Text = "EditingQuestType: " + qType + " Question Number" + editingQNum;

        //NEED TO VALIDATE QUESTION


        //Now we have the question TYPE, the question number and the survey ID. This is all we need to edit a question.
        if (qType == "SAQ")
        {
            //Find the question in the SAQ array
            List<SAQEdit> tempSAQ = (List<SAQEdit>)Session["saqArray"];
            List<MCQEdit> tempMCQ = (List<MCQEdit>)Session["mcqArray"];
            for (int i = 0; i < tempSAQ.Count; i++)
            {
                SAQEdit currentSAQ = tempSAQ[i];
                if (editingQNum == currentSAQ.getQuestNo())
                {
                    //Question Found
                    currentSAQ.setQuestion(txtSAQEdit.Text);
                    updateReview(tempMCQ, tempSAQ);
                }
            }
        }

        else if (qType == "MCQ")
        {
            //Find the question in the SAQ array
            List<SAQEdit> tempSAQ = (List<SAQEdit>)Session["saqArray"];
            List<MCQEdit> tempMCQ = (List<MCQEdit>)Session["mcqArray"];

            for (int i = 0; i < tempMCQ.Count; i++)
            {
                MCQEdit currentMCQ = tempMCQ[i];
                if (editingQNum == currentMCQ.getQuestNo())
                {
                    //Question Found
                    currentMCQ.setQuestion(txtMCQEditQuest.Text);

                    //Remove all current answers
                    currentMCQ.removeAnswers();

                    //Add New Answers
                    string[] choiceArr = { txtChEdit1.Text, txtChEdit2.Text, txtChEdit3.Text, txtChEdit4.Text, txtChEdit5.Text };

                    for (int j = 0; j < choiceArr.Length; j++)
                    {
                        currentMCQ.addAnswer(choiceArr[j]);
                    }

                    //Lastly Update the Multiple Options Choice
                    if (ckbAllowMultEdit.Checked)
                    {
                        currentMCQ.setMult(true);
                    }
                    else
                    {
                        currentMCQ.setMult(false);
                    }

                    //Update the review
                    updateReview(tempMCQ, tempSAQ);


                }
            }

        }
    }

    protected void deleteQuestion(object sender, EventArgs e)
    {

        

        //First get the question number and question type
        int editingQNum = (int)Session["EditingQuestionNum"];

        string qType = getType(editingQNum);

        //Testing
        //lblResult.Text = "DeletingQuestType: " + qType + " Question Number" + editingQNum;


        string currentSurveyID = (string)Session["SurveyID"];
       

        ////////Update the questions list
        List<ListItem> tempList = (List<ListItem>)Session["listQuestions"];
        tempList.RemoveAt(editingQNum - 1);
        updateEditQuestionsList();
        Session["listQuestions"] = tempList;

        //Update arrays and Question Numbers for all other questions
        //Add Questions to question table
        List<MCQEdit> mcqTemp = (List<MCQEdit>)Session["mcqArray"];
        List<SAQEdit> saqTemp = (List<SAQEdit>)Session["saqArray"];
        int i;
        if (qType == "SAQ")
        {
            for (i = 0; i < saqTemp.Count; i++)
            {
                SAQEdit currentQuest = saqTemp[i];
                if (currentQuest.getQuestNo() == editingQNum)
                {
                    lblResult.Text = "Question Deleted!";
                    saqTemp.RemoveAt(i);
                    break;
                }
            }
            
        }
        else
        {
            for (i = 0; i < mcqTemp.Count; i++)
            {
                MCQEdit currentQuest = mcqTemp[i];
                if (currentQuest.getQuestNo() == editingQNum)
                {
                    lblResult.Text = "Question Deleted!";
                    mcqTemp.RemoveAt(i);
                    break;
                }
            }
            
        }

        ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblResult.ClientID + "').style.display='none'\",5000)</script>");

        //Update Question Numbers
        for (i = 0; i < saqTemp.Count; i++)
        {
            SAQEdit currentQuest = saqTemp[i];
            if (currentQuest.getQuestNo() > editingQNum)
            {
                
                int currentQNum = currentQuest.getQuestNo();

                //Set new question Number
                currentQuest.setQuestNo(currentQNum - 1);
            }
        }
        //Update Question Numbers
        for (i = 0; i < mcqTemp.Count; i++)
        {
            MCQEdit currentQuest = mcqTemp[i];
            if (currentQuest.getQuestNo() > editingQNum)
            {

                int currentQNum = currentQuest.getQuestNo();

                //Set new question Number
                currentQuest.setQuestNo(currentQNum - 1);
            }
        }

        //Update Question Count
        int tempCnt = (int)Session["questCnt"];
        tempCnt--;
        lblQCnt.Text = "Number of Questions: " + tempCnt;
        Session["questCnt"] = tempCnt;


        Session["mcqArray"] = mcqTemp;
        Session["saqArray"] = saqTemp;

        //Update Session["EditingQuestionNum"]
        Session["EditingQuestionNum"] = 0;

        //Update Undo function ["Last Question Type"]

        //Update the Survey Reviewer
        updateReview(mcqTemp,saqTemp);
        //Update the Views
        myMCQEditTable.Visible = false;
        mySAQEditTable.Visible = false;
    }
    protected void addMCQ(object sender, EventArgs e)
    {
        //lblTester.Text = schoolList.SelectedItem.Text;

        //Response.Redirect("Datalot.aspx");
        myTable.Visible = false;
        myTable2.Visible = true;
        mySAQEditTable.Visible = false;
        myMCQEditTable.Visible = false;

        txtQuest.Focus();
        lblResult.Text = "";

    }

    protected void clearTxtBoxes(object sender, EventArgs e)
    {
        //lblTester.Text = schoolList.SelectedItem.Text;

        //Response.Redirect("Datalot.aspx");

        txtCh1.Text = "";
        txtCh2.Text = "";
        txtCh3.Text = "";
        txtCh4.Text = "";
        txtCh5.Text = "";
        myTable2.Visible = true;
        txtQuest.Focus();

    }

    protected void hideMCQTable(object sender, EventArgs e)
    {
       
        myTable2.Visible = false;

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

    protected void addSAQ(object sender, EventArgs e)
    {
        //lblTester.Text = schoolList.SelectedItem.Text;

        //Response.Redirect("Datalot.aspx");
        myTable2.Visible = false;
        myTable.Visible = true;
        mySAQEditTable.Visible = false;
        myMCQEditTable.Visible = false;
        txtQuest.Focus();
        lblResult.Text = "";
        

    }

    private string validateQuest()
    {
        //Validate all the inputs
        
        string valid = "valid";
        //Question is blank
        if (txtMCQQuest.Text == "")
        {
            valid = "Enter a question!";
        }

        else if (txtCh1.Text == "" || txtCh2.Text == "")
        {
            valid = "Enter atleast 2 choices for a multiple choice question.";
        }
        return valid;
    }



    /*
    Updates the survey review label as users keep adding questions. It takes data from the current Session arrays to keep the review updated.
    */
    private void updateReview(List<MCQEdit> arr1, List<SAQEdit> arr2)
    {
        lblSurveyReview.Text = "";
        if (arr2.Count > 0)
        {
            lblSurveyReview.Text += "<br /><strong>Short Answer Question(s): <br /> </strong>";
            for (int i = 0; i < arr2.Count; i++)
            {
                lblSurveyReview.Text += "<br /> Q" + arr2[i].getQuestNo() + ": " + arr2[i].getQuestion() + "";
            }
        }

        if (arr1.Count > 0)
        {
            lblSurveyReview.Text += "<br /><br /><strong> Multiple Choice Question(s): <br /></strong>";
            for (int i = 0; i < arr1.Count; i++)
            {
                lblSurveyReview.Text += "<br />  Q" + arr1[i].getQuestNo() + ": " + arr1[i].getQuestion() + "";
                List<string> answers = arr1[i].getAnswers();
                for (int j = 0; j < answers.Count; j++)
                {
                    lblSurveyReview.Text += "<br />  Choice " + (j+1) + ":" + answers[j];
                }
                lblSurveyReview.Text += "<br /> More than 1 choice option: " + arr1[i].getMultOpt() + "<br /> <br />";
            }
        }


    }

    protected void undo(object sender, EventArgs e)
    {

        int flag = 0;
        //If there are no questions, there is nothing to undo
        if ((int)Session["questCnt"] == 0)
        {
            lblResult.Text = "No Question to Remove..";
            return;
        }

        List<string> tempUndo = (List<string>)Session["lastQuestionType"];

        

        //Otherwise, check the last question
        if (tempUndo[tempUndo.Count - 1] == "SAQ")
        {
            
            //Remove last element from the SAQ array
            List<SAQEdit> tempSAQ = (List<SAQEdit>)Session["saqArray"];
            tempSAQ.RemoveAt(tempSAQ.Count - 1);
            Session["saqArray"] = tempSAQ;
            List<MCQEdit> tempMCQ = (List<MCQEdit>)Session["mcqArray"];


            //Update Survey Review
            updateReview(tempMCQ, tempSAQ);

            //Update questCnt
            int tempCnt = (int)Session["questCnt"];
            tempCnt--;
            Session["questCnt"] = tempCnt;
            lblQCnt.Text = "Number of Questions: " + tempCnt;
            flag = 1;

            //Remove from Last question from stack
            tempUndo.RemoveAt(tempUndo.Count - 1);
        }
        //else if (tempUndo[tempUndo.Count - 1] == "MCQ" || tempUndo[tempUndo.Count - 1] == "MCQ2")
        else if (tempUndo[tempUndo.Count - 1] == "MCQ" || tempUndo[tempUndo.Count - 1] == "MCQ2")
        {

            //Remove last element from the SAQ array
            List<MCQEdit> tempMCQ = (List<MCQEdit>)Session["mcqArray"];
            tempMCQ.RemoveAt(tempMCQ.Count - 1);
            Session["mcqArray"] = tempMCQ;
            List<SAQEdit> tempSAQ = (List<SAQEdit>)Session["saqArray"];

            //Update Survey Review
            updateReview(tempMCQ, tempSAQ);

            //Update questCnt
            int tempCnt = (int)Session["questCnt"];
            tempCnt--;
            Session["questCnt"] = tempCnt;
            flag = 1;
            lblQCnt.Text = "Number of Questions: " + tempCnt;

            //Remove from Last question from stack
            tempUndo.RemoveAt(tempUndo.Count - 1);
        }
        Session["lastQuestionType"] = tempUndo;

        if (flag == 1)
        {
            lblResult.Text = "Last Question Removed!";
        }

    }


    //SAVES SAQ TYPE Question
    protected void saveQuestion(object sender, EventArgs e)
    {

        //Validate
        string valid = "";
        if (txtQuest.Text == "")
        {
            valid = "Enter a question";
            lblResult.Text = valid;
            return;
        }
        else
        {
            lblResult.Text = "";
        }


        //questCnt++;
        int tempCnt = (int)Session["questCnt"];

        tempCnt++;
        lblQCnt.Text = "Number of Questions: " + tempCnt; 
        
        lblSurveyReview.Text += "<br /> <strong>Short Answer Question " + tempCnt + ": " + txtQuest.Text + "</strong>";



        //Edit Questions List
        List<ListItem> tempList = (List<ListItem>)Session["listQuestions"];
        tempList.Add(new ListItem(txtQuest.Text, tempCnt.ToString()));
        //Edit Questions List Update
        Session["listQuestions"] = tempList;
        updateEditQuestionsList();

        SAQEdit curQuest = new SAQEdit(txtQuest.Text, tempCnt);
        //saqArray.Add(curQuest);
        
        //Using Session Variables
        List<SAQEdit> saqTemp = (List<SAQEdit>)Session["saqArray"];
        saqTemp.Add(curQuest);
        Session["saqArray"] = saqTemp;

        //TEST
        List<MCQEdit> mcqTemp = (List<MCQEdit>)Session["mcqArray"];
        updateReview(mcqTemp, saqTemp);

        lblTester.Visible = true;
        lblTester.Text = "QUESTION SAVED!";

        ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblTester.ClientID + "').style.display='none'\",2000)</script>");

        txtQuest.Text = "";
        
        myTable.Visible = true;
        myTable2.Visible = false;

        Session["questCnt"] = tempCnt;

        List<String> tempUndo = (List<String>)Session["lastQuestionType"];
        tempUndo.Add("SAQ");
        Session["lastQuestionType"] = tempUndo;

        

    }

    protected void saveMCQ(object sender, EventArgs e)
    {
        //Validate Question
        string valid = validateQuest();
        if (valid != "valid")
        {
            lblResult.Text = valid;
            return;
        }
        else
        {
            lblResult.Text = "";
        }

        //Update Question Count
        //questCnt++;

        int tempCnt = (int)Session["questCnt"];

        tempCnt++;
        lblQCnt.Text = "Number of Questions: " + tempCnt;
        //Get choices
        string[] choiceArr = { txtCh1.Text, txtCh2.Text, txtCh3.Text, txtCh4.Text, txtCh5.Text };

        

        lblSurveyReview.Text += "<br />";

        //Save Question in MCQ array
        bool multChoices = allowChcks.Checked;
        MCQEdit curQuest = new MCQEdit(txtMCQQuest.Text,multChoices,tempCnt);

        //Edit Questions List
        List<ListItem> tempList = (List<ListItem>)Session["listQuestions"];
        tempList.Add(new ListItem(txtMCQQuest.Text, tempCnt.ToString()));
        //Edit Questions List Update
        Session["listQuestions"] = tempList;
        updateEditQuestionsList();

        //Add Answer Choices to the MCQ Object
        for (int i = 0; i < choiceArr.Length; i++)
        {
            if (choiceArr[i] != "")
            {
                curQuest.addAnswer(choiceArr[i]);
            }
        }

        //New Session Way. Add to temporary array
        List<MCQEdit> mcqTemp = (List<MCQEdit>)Session["mcqArray"];
        mcqTemp.Add(curQuest);

        List<SAQEdit> saqTemp = (List<SAQEdit>)Session["saqArray"];

        //Update Survey Review
        updateReview(mcqTemp, saqTemp);

        Session["mcqArray"] = mcqTemp;

        //mcqArray.Add(curQuest);

        lblResult.Visible = true;
        lblResult.Text = "Question Saved!";
        ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblResult.ClientID + "').style.display='none'\",2000)</script>");

        txtMCQQuest.Text = "";

        myTable.Visible = false;
        myTable2.Visible = true;

        Session["questCnt"] = tempCnt;

        //Save Last Question
        List<String> tempUndo = (List<String>)Session["lastQuestionType"];
        tempUndo.Add("MCQ");
        Session["lastQuestionType"] = tempUndo;


    }
    protected void updateDate(object sender, EventArgs e)
    {
        bool complete = true;

        int startYear = 2015;
        int startMonth = 1;
        int startDay = 1;
        int endYear = 2015;
        int endMonth = 12;
        int endDay = 1;

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


        //Update Session Variable
        bool dateReady = (bool)Session["dateReady"];
        dateReady = complete;
        Session["dateReady"] = dateReady;

      

        //If all dates have been entered. Add them to the session variables
        if (complete == true)
        {

            
            try
            {
                DateTime tempActDate = new DateTime(startYear, startMonth, startDay);
                Session["activationDate"] = tempActDate;

                DateTime tempEndDate = new DateTime(endYear, endMonth, endDay);
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
                    lblResult.Text = "Activation Date: " + ((DateTime)Session["activationDate"]).ToString() +
                        "<br /> End Date: " + ((DateTime)Session["endDate"]).ToString();
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


    protected void updateOptions(object sender, EventArgs e)
    {
        txtCh1.Text = "";
        txtCh2.Text = "";
        txtCh3.Text = "";
        txtCh4.Text = "";
        txtCh5.Text = "";
        if (listOptions.SelectedIndex == 1)
        {
            txtCh1.Text = "Agree";
            txtCh2.Text = "Disagree";
        }

        if(listOptions.SelectedIndex == 2)
        {
            txtCh1.Text = "1";
            txtCh2.Text = "2";
            txtCh3.Text = "3";
            txtCh4.Text = "4";
            txtCh5.Text = "5";
        }

        if (listOptions.SelectedIndex == 3)
        {
            txtCh1.Text = "Yes";
            txtCh2.Text = "No";
        }

        if (listOptions.SelectedIndex == 4)
        {
            txtCh1.Text = "Strongly Disagree";
            txtCh2.Text = "Disagree";
            txtCh3.Text = "Agree";
            txtCh4.Text = "Strongly Agree";
        }

        if (listOptions.SelectedIndex == 5)
        {
            txtCh1.Text = "Not at all";
            txtCh2.Text = "Some-what";
            txtCh3.Text = "A Fair amount";
            txtCh4.Text = "A Great Deal";
        }

        myTable2.Visible = true;
        txtQuest.Focus();
    }

    protected void getPartGrp(object sender, EventArgs e)
    {
        String participants = "";
        if (ddlParticipants.SelectedIndex == 1)
        {
            participants = "All";
        }
        else if (ddlParticipants.SelectedIndex == 2)
        {
            participants = "Students";
        }
        else if (ddlParticipants.SelectedIndex == 3)
        {
            participants = "Teachers";
        }

        Session["participants"] = participants;
    }

    private string saveAndValidate()
    {
        this.surveyName = txtSurveyName.Text;
        string valid = "valid";

        //Validate surveyName
        if (txtSurveyName.Text == "")
        {
            valid = "Enter a Survey Name..";
        }

        else if ((bool)Session["dateReady"] == false)
        {
            valid = "Please Enter a valid Activation Date and an End Date for the Survey";
        }

        //Validate Participants
        else if ((string)Session["participants"] == "")
        {
            valid = "Please Select a participants group!";
        }

        //Validate number of questions
        else if ((int)Session["questCnt"] < 1)
        {
            valid = "The Survey has to have atleast 1 question!";
        }

        
        return valid;
    }

    protected void submitSurvey(object sender, EventArgs e)
    {
        string valid = saveAndValidate();
        if (valid != "valid")
        {
            lblResult.Text = valid;
            return;
        }
        else
        {
            //Valid Survey
            lblResult.Visible = true;
            lblResult.Text = "Submitting...";
            ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblResult.ClientID + "').style.display='none'\",2000)</script>");

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

                //Creation Date
                DateTime creationDate = DateTime.Today;

                //Escape the Single Quote
                this.surveyName = escapeSingleQuote(this.surveyName);

                //Prepare Query
                //String sql = "INSERT INTO spirit.DatalotSurvey (Name, SGroup, ActivationDate, EndDate, Participants, CreationDate) VALUES ('"+ this.surveyName+"', '" + (string)Session["surveyGroup"] + "', '" + (DateTime)Session["activationDate"] + 
                //    "', '" + (DateTime)Session["endDate"] + "', '" + (string)Session["participants"] + "', '" + creationDate + "');";

                String sql = "UPDATE spirit.DatalotSurvey SET Name='" + this.surveyName + "', ActivationDate='" + (DateTime)Session["activationDate"] + "', EndDate='" + (DateTime)Session["endDate"] + 
                    "', Participants='" + (string)Session["participants"] + "', CreationDate='" + creationDate + "' WHERE SurveyID=" + (string)Session["SurveyID"] + ";";
                //Execute Command
                OleDbCommand cmd = new OleDbCommand(sql, db);
                rdr = cmd.ExecuteReader();
                

                //Add Questions to question table
                List<MCQEdit> mcqTemp = (List<MCQEdit>)Session["mcqArray"];
                List<SAQEdit> saqTemp = (List<SAQEdit>)Session["saqArray"];

                sql = "DELETE FROM spirit.DatalotQuestionResponse WHERE SurveyID=" + (string)Session["SurveyID"] + ";";
                //Execute Command
                cmd = new OleDbCommand(sql, db);
                rdr = cmd.ExecuteReader();

                //Remove existing questions from this survey
                sql = "DELETE FROM spirit.DatalotQuestion WHERE SurveyID=" + (string)Session["SurveyID"] + ";";
                //Execute Command
                cmd = new OleDbCommand(sql, db);
                rdr = cmd.ExecuteReader();


                //ADD SAQ
                for (int i = 0; i < saqTemp.Count; i++)
                {
                    SAQEdit temp = saqTemp[i];
                    string tempQuestion = temp.getQuestion();
                    tempQuestion = escapeSingleQuote(tempQuestion);
                    string tempType = "SAQ";
                    int tempQuestNo = temp.getQuestNo();
                    string choice1 = "temp";
                    string choice2 = "temp";

                    sql = "INSERT INTO spirit.DatalotQuestion (SurveyID, Question, Qtype, QNumber, Choice1, Choice2) VALUES (" + (string)Session["SurveyID"] + ", '" + tempQuestion + "', '" + tempType + "', '" +
                        tempQuestNo + "', '" + choice1 + "', '" + choice2 + "');";

                  
                    cmd = new OleDbCommand(sql, db);
                    rdr = cmd.ExecuteReader();

                }

                //ADD MCQ
                for (int i = 0; i < mcqTemp.Count; i++)
                {
                    MCQEdit temp = mcqTemp[i];
                    string tempQuestion = temp.getQuestion();
                    tempQuestion = escapeSingleQuote(tempQuestion);
                    int tempQuestNo = temp.getQuestNo();
                    List<string> tempAnswers = temp.getAnswers();

                    string choice1 = tempAnswers[0];
                    choice1 = escapeSingleQuote(choice1);
                    string choice2 = tempAnswers[1];
                    choice2 = escapeSingleQuote(choice2);

                    int answerCnt = tempAnswers.Count;

                    bool multOpts = temp.getMultOpt();

                    string type = "MCQ";
                    if (multOpts == true)
                    {
                        type = "MCQ2";
                    }

                    //Prepare an incomplete sql statement
                    sql = "INSERT INTO spirit.DatalotQuestion (SurveyID, Question, Qtype, QNumber, Choice1, Choice2, Choice3, Choice4, Choice5, Choice6, Choice7, Choice8, Choice9, Choice10) VALUES (" + (string)Session["SurveyID"] + ", '"
                        + tempQuestion + "', '" + type + "', '" +
                        tempQuestNo + "', '" + choice1 + "', '" + choice2 + "', ";

                    for (int j = 2; j < answerCnt; j++)
                    {
                        tempAnswers[j] = escapeSingleQuote(tempAnswers[j]);
                        if (answerCnt == 10 && j == answerCnt-1) {
                            sql += "'" + tempAnswers[j] + "'";
                        }
                        else
                        {
                            sql += "'" + tempAnswers[j] + "', ";
                        }
                    }

                    if (answerCnt < 10) {
                        for (int j = answerCnt; j < 10; j++)
                        {
                            if (j != 9)
                            {
                                sql += "'', ";
                            }
                            else
                            {
                                sql += "''";
                            }
                        }

                    }

                    sql +=  "); ";

                    //Testing
                    //lblErr.Text = sql;

                    //Execute Command
                    cmd = new OleDbCommand(sql, db);
                    rdr = cmd.ExecuteReader();


                }

                



                //Lastly, Confirm
                lblResult.Text = "Success! Survey Modified! SurveyID: " + (string)Session["SurveyID"];
                ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblResult.ClientID + "').style.display='none'\",2000)</script>");

                //Redirect to datalot with info
                surveyName = surveyName.Replace("''", "'");
                Response.Redirect("Datalot.aspx?getInfo=Survey '" + surveyName + "' has been modified..");
            }
            catch (Exception exc)
            {
                lblErr.Text = "Error! Unable to submit Survey..." + exc.Message + exc.StackTrace;
            }
            finally
            {
                //close db connection
                db.Close();
            }
        }

        //Start Submit Process

    }
}

internal class MCQEdit
{

    private string question;
    private List<string> answers;
    private bool multipleAns;
    private int questNo;
    private string questionID = "";

    public MCQEdit()
    {

    }

    public MCQEdit(string q, bool ma, int qn)
    {
        this.question = q;
        answers = new List<string>();
        this.multipleAns = ma;
        this.questNo = qn;
    }

    public MCQEdit(string q, bool ma, int qn, string qID)
    {
        this.question = q;
        answers = new List<string>();
        this.multipleAns = ma;
        this.questNo = qn;
        this.questionID = qID;
    }
    public void removeAnswers()
    {
        this.answers.Clear();
    }
    public void addAnswer(string a)
    {
        this.answers.Add(a);
    }

    public void setMult(bool multOpts)
    {
        this.multipleAns = multOpts;
    }

    public void setQuestion(string newQuest)
    {
        this.question = newQuest;
    }

    public string getQuestion()
    {
        return this.question;
    }

    public int getQuestNo()
    {
        return this.questNo;
    }
    public void setQuestNo(int qNum)
    {
        this.questNo = qNum;
    }
    public List<string> getAnswers()
    {
        return answers;
    }
    public bool getMultOpt()
    {
        return this.multipleAns;
    }


}

internal class SAQEdit
{
    private string questionID = "";
    private string question;
    private string answer;
    private int questNo;

    public SAQEdit()
    {

    }
    public void setQuestion(string newQuest)
    {
        this.question = newQuest;
    }

    public string getQuestion()
    {
        return this.question;
    }

    public string getanswer()
    {
        return this.answer;
    }
    public int getQuestNo()
    {
        return this.questNo;
    }
    public void setQuestNo(int qNum)
    {
        this.questNo = qNum;
    }

    public SAQEdit(String q, int qn)
    {
        this.question = q;
        answer = "";
        this.questNo = qn;
    }

    public SAQEdit(String q, int qn, string qID)
    {
        this.question = q;
        answer = "";
        this.questNo = qn;
        this.questionID = qID;
    }

    public void addAnswer(String a)
    {
        this.answer = a;
    }
}