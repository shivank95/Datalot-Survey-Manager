using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Data.OleDb;

public partial class takeSurvey : System.Web.UI.Page
{

    private String DBConnectionString = "Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=dev.techfit;Integrated Security=SSPI";


    protected void Page_Load(object sender, EventArgs e)
    {
        //No More Testing
        lblTester.Visible = false;

        if (Session["UserID"] == null || (string)Session["Type"] != "admin")
        {
            Response.Redirect("DatalotAdminLogin.aspx?err=You must log in to view student records");
        }
        //For Testing
        lblTester.Visible = false;
        //if (Session["UserID"] == null || Session["Type"] == null)
        //{
        //    Response.Redirect("Login.aspx?err=You must log in to view student records");
        //}

        if (!IsPostBack)
        {

            //if (!Session["UserID"].Equals("40") && !Session["Type"].Equals("admin"))
            //{
            //    Response.Redirect("Login.aspx?err=You must log in to view student records" + Session["UserID"] + " " + Session["Type"]);
            //}


            //This method does everything including getting id from the old page, loading the entire survey and validation
            Session["surveyID"] = "";
            Session["currentSurvey"] = "";
            Session["mcqArray"] = new List<MCQTake>();
            Session["saqArray"] = new List<SAQTake>();
            Session["currentQNum"] = 0;
            Session["questCnt"] = 0;

            Session["groupSurveyIDs"] = new List<string>();
            Session["currentSurveyNum"] = 0;
            getSurveyID();

            //myTable.Visible = false;

            

        }
    }


    private void getSurveyID()
    {
        Session["SurveyID"] = Request.QueryString["surveyID"];
        string tempID = (string)Session["surveyID"];
        if (tempID == "" || tempID == null)
        {
            getGroupID();
        }
        else
        {
            loadSurvey(tempID);
        }
    }

    private void getGroupID()
    {
        Session["GroupID"] = Request.QueryString["GroupID"];
        string tempID = (string)Session["GroupID"];
        lblErr.Text = "GROUP ID: " + tempID;
        if (tempID == "" || tempID == null)
        {
            Response.Redirect("DatalotAdminLogin.aspx?err=You must log in to view student records");
        }
        loadGroup(tempID);
    }

    private void loadGroup(string groupID)
    {
        //Based on Group ID GET all survey IDs
        string currentSurveyID = "";
        int surveyCnt = 0;

        List<string> tempSurveyArray = (List<string>)Session["groupSurveyIDs"];

        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;

        try
        {
            //open connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();


            string sql = "SELECT SurveyID FROM spirit.DatalotAssociatedSurveyGroup S WHERE S.GroupID = " + groupID + " ORDER BY S.SurveyNum;";

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                surveyCnt++;
                currentSurveyID = (string)rdr["SurveyID"].ToString();
                tempSurveyArray.Add(currentSurveyID);
            }

            if (surveyCnt == 0)
            {
                lblResult.Text = "There are no surveys associated with this group. Please try again...";
                myTable.Visible = false;
                myTable2.Visible = false;
            }
            else
            {
                loadSurvey(tempSurveyArray[0]);
                Session["currentSurveyNum"] = 0;
            }
            //If there are no surveys then tell that to the user
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

    //Assigns Session variable currentSurvey
    private void loadSurvey(string surveyID)
    {

        //Get Survey Details
        string surveyName = "";
        string participants = "";
        DateTime activationDate = new DateTime();
        DateTime endDate = new DateTime();


        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;

        try
        {
            //open connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();

            string sql = "SELECT Name, Participants, ActivationDate, EndDate, CreationDate FROM spirit.DatalotSurvey S WHERE S.SurveyID = " + surveyID + ";";

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                surveyName = (string)rdr["Name"].ToString();
                participants = (string)rdr["Participants"].ToString();
                string tempDate = (string)rdr["ActivationDate"].ToString();
                activationDate = Convert.ToDateTime(tempDate);
                tempDate = (string)rdr["EndDate"].ToString();
                endDate = Convert.ToDateTime(tempDate);
                
            }
            
            //*******8Validate if Survey Can be Taken Here Later******************************88

            //Save in Session variable for future use
            mySurvey3 currentSurvey = new mySurvey3(surveyName, activationDate.ToString(), endDate.ToString(), participants, surveyID);
            Session["currentSurvey"] = currentSurvey;

            // Change header
            lblSurveyName.Text = "Survey: " + surveyName;

            //Indicate Survey Number if exists
            List<string> tempGroupIDs = (List<string>) Session["groupSurveyIDs"];
            if (tempGroupIDs != null && tempGroupIDs.Count > 0)
            {
                int surveyNum = (int)Session["currentSurveyNum"];

                // Modify header
                lblSurveyName.Text = "Survey (" + (surveyNum + 1) + " of " + tempGroupIDs.Count + ") : " + surveyName;
            }


            //Time to get the questions
            loadAllQuestions(surveyID);


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

    private void loadAllQuestions(string surveyID)
    {

        

        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;

        try
        {
            //open connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();

            string sql = "SELECT QuestionID, Question, Qtype, QNumber, Choice1, Choice2, Choice3, Choice4, Choice5, Choice6, Choice7, Choice8, Choice9, Choice10 FROM spirit.DatalotQuestion WHERE SurveyID = " + surveyID + " ORDER BY QNumber ASC;";

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            //Attributes
            string qID;
            string question;
            string qType;
            string qNumber;
            List<string> choices = new List<string>();

            List<SAQTake> loadSAQ = new List<SAQTake>();
            List<MCQTake> loadMCQ = new List<MCQTake>();

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


                if (qType == "SAQ")
                {
                    int currentQNumber = Int32.Parse(qNumber);
                    SAQTake currentSAQ = new SAQTake(question, currentQNumber, qID);
                    loadSAQ.Add(currentSAQ);

                }
                else
                {
                    int currentQNumber = Int32.Parse(qNumber);
                    MCQTake currentMCQ;
                    if (qType == "MCQ")
                    {
                        currentMCQ = new MCQTake(question, false, currentQNumber, qID);
                    }
                    else
                    {
                        currentMCQ = new MCQTake(question, true, currentQNumber, qID);
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
            }

            //Set the global session arrays of these questions to these temporary values
            Session["mcqArray"] = loadMCQ;
            Session["saqArray"] = loadSAQ;

            Session["questCnt"] = qCountInt;

            Session["currentQNum"] = 1;


            //Start LoadingQuestions from Question 1
            loadQuestion(1);


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

    private void loadQuestion(int questNumber)
    {

        //Update current Question Number
        Session["currentQNum"] = questNumber;
        

        List<SAQTake> tempSAQList = (List<SAQTake>)Session["saqarray"];
        List<MCQTake> tempMCQList = (List<MCQTake>)Session["mcqarray"];


        bool found = false;

        //Find the Question
        for (int i = 0; i < tempSAQList.Count; i++)
        {
            
            int tempCnt = tempSAQList[i].getQuestNo();
            if (tempCnt == questNumber)
            {
                //Found
                found = true;
                
                SAQTake currentSAQ = tempSAQList[i];

                myTable.Visible = true;
                myTable2.Visible = false;
                lblQuestion.Text = "Question " + questNumber + ": " + currentSAQ.getQuestion();

                if (currentSAQ.getAnswerResponse() != "")
                {
                    txtResponse.Text = currentSAQ.getAnswerResponse();
                }

                Button tempButton = (Button) myTable.FindControl("btnSaveAndNext");
                
                if (questNumber == (int) Session["questCnt"])
                {
                    tempButton.Text = "Submit Survey";
                }
                else
                {
                    tempButton.Text = "Save and Next ->";
                }

                break;
            }

        }

        //If its an MCQ
        for (int i = 0; found == false && i < tempMCQList.Count; i++)
        {
            int tempCnt = tempMCQList[i].getQuestNo();
            if (tempCnt == questNumber)
            {
                //Found
                found = true;
                MCQTake currentMCQ = tempMCQList[i];
                found = true;

                //get answers
                List<string> tempAnswers = currentMCQ.getAnswers();

                //Clear pre existing options
                radioAnswers.Items.Clear();
                ckbAnswers.Items.Clear();

                //Radio Button List
                if (currentMCQ.getMultOpt() == false)
                {
                    List<string> oldAnswers = currentMCQ.getSelectedAnswers();

                    for (int j = 0; j < tempAnswers.Count; j++)
                    {
                        ListItem tempItem = new ListItem(tempAnswers[j], tempAnswers[j]);
                        if (oldAnswers.Count > 0)
                        {
                            if (oldAnswers[0] == tempAnswers[j])
                            {
                                tempItem.Selected = true;
                            }
                           
                        }
                        radioAnswers.Items.Add(tempItem);
                    }

                }

                //CheckBoxes
                else
                {
                    List<string> oldAnswers = currentMCQ.getSelectedAnswers();

                    for (int j = 0; j < tempAnswers.Count; j++)
                    {
                        ListItem tempItem = new ListItem(tempAnswers[j], tempAnswers[j]);

                        if (oldAnswers.Count > 0)
                        {

                            for (int z = 0; z < oldAnswers.Count; z++)
                            {
                                if (oldAnswers[z] == tempAnswers[j])
                                {
                                    tempItem.Selected = true;
                                }
                            }

                        }

                        ckbAnswers.Items.Add(tempItem);
                    }
                }
                

                myTable2.Visible = true;
                myTable.Visible = false;

                lblMCQQuest.Text = "Question " + questNumber + ": " + currentMCQ.getQuestion();

                Button tempButton = (Button)myTable.FindControl("btnSaveAndNextMCQ");

                if (questNumber == (int)Session["questCnt"])
                {
                    tempButton.Text = "Submit Survey";
                }
                else
                {
                    tempButton.Text = "Save and Next ->";
                }

                break;
            }
        }





                //update currentQuestionNumber

            }

    protected void backButton(object sender, EventArgs e)
    {

        Response.Redirect("findTakeSurvey.aspx");
    }

    private void addSAQAnswer(int currentQNum)
    {

        //Get Answer
        string response = txtResponse.Text;
        txtResponse.Text = "";

        List<SAQTake> tempSAQList = (List<SAQTake>)Session["saqarray"];
        List<MCQTake> tempMCQList = (List<MCQTake>)Session["mcqarray"];

        for (int i = 0; i < tempSAQList.Count; i++)
        {
            SAQTake currentSAQ = tempSAQList[i];
            if (currentSAQ.getQuestNo() == currentQNum)
            {

                //Found question

                //Add Answer
                currentSAQ.addAnswer(response);
            }

        }

        Session["saqArray"] = tempSAQList;


        //Update Review
        updateReview(tempMCQList, tempSAQList);
    }

    protected void saveSAQAndNextQuestion(object sender, EventArgs e)
    {
        //Get currentQuestion
        int currentQNum = (int)Session["currentQNum"];


        addSAQAnswer(currentQNum);
        

        if (currentQNum == (int)Session["questCnt"])
        {
            submitSurvey();
        }
        else
        {
            //Go to Next Question
            loadQuestion(currentQNum + 1);
        }

    }

    private void addMCQAnswer(int currentQNum)
    {
        //Get Response
        List<string> answers = new List<string>();
        if (radioAnswers.Items.Count > 0)
        {
            lblResult.Text = "radio";
            for (int i = 0; i < radioAnswers.Items.Count; i++)
            {
                ListItem tempItem = radioAnswers.Items[i];
                if (tempItem.Selected == true)
                {
                    answers.Add(tempItem.Value);

                    //Testing
                    lblResult.Text = tempItem.Value;

                    break;
                }
            }
        }
        else
        {
            lblResult.Text = "ckb";

            for (int i = 0; i < ckbAnswers.Items.Count; i++)
            {
                ListItem tempItem = ckbAnswers.Items[i];
                if (tempItem.Selected == true)
                {
                    answers.Add(tempItem.Value);
                }
            }

        }

        List<SAQTake> tempSAQList = (List<SAQTake>)Session["saqarray"];
        List<MCQTake> tempMCQList = (List<MCQTake>)Session["mcqarray"];



        for (int i = 0; i < tempMCQList.Count; i++)
        {
            MCQTake currentMCQ = tempMCQList[i];
            if (currentMCQ.getQuestNo() == currentQNum)
            {

                //Found question
                //Add Answer
                currentMCQ.setAnswerResponses(answers);
            }

        }

        Session["mcqArray"] = tempMCQList;

        //Update Survey Review
        updateReview(tempMCQList, tempSAQList);
    }

    protected void saveMCQAndNextQuestion(object sender, EventArgs e)
    {
        //Get currentQuestion
        int currentQNum = (int)Session["currentQNum"];


        addMCQAnswer(currentQNum);
       

        if (currentQNum == (int)Session["questCnt"])
        {
            submitSurvey();
        }
        else
        {
            //Go to Next Question
            loadQuestion(currentQNum + 1);
        }
        
    }

    protected void lastQuestion(object sender, EventArgs e)
    {

        //Get currentQuestion
        int currentQNum = (int)Session["currentQNum"];

        if (currentQNum == 1)
        {
            return;
        }

        if (myTable.Visible == true)
        {
            addSAQAnswer(currentQNum);
        }
        else
        {
            addMCQAnswer(currentQNum);
        }

       
        //Go to Last Question
        loadQuestion(currentQNum - 1);

    }

    private void submitSurvey()
    {
        lblResult.Text = "Survey Submitted..";


        //Check if group has any more surveys
        List<string> tempSurveyArray = (List<string>)Session["groupSurveyIDs"];
        int currentSurveyNum = (int)Session["currentSurveyNum"];
        currentSurveyNum++;
        if (currentSurveyNum < tempSurveyArray.Count)
        {
            Session["currentSurveyNum"] = currentSurveyNum;
            loadSurvey(tempSurveyArray[currentSurveyNum]);
        }
        else
        {
            Response.Redirect("Datalot.aspx?getInfo=Survey Response Submitted!");
        }
        
    }

    /*
    Updates the survey review label as users keep adding questions. It takes data from the current Session arrays to keep the review updated.
    */
    private void updateReview(List<MCQTake> arr1, List<SAQTake> arr2)
    {
        lblSurveyReview.Text = "";
        if (arr2.Count > 0)
        {
            lblSurveyReview.Text += "<br /><strong>Short Answer Question(s): <br /> </strong>";
            for (int i = 0; i < arr2.Count; i++)
            {
                if (arr2[i].getAnswerResponse() != "")
                {
                    lblSurveyReview.Text += "<br /> Q" + arr2[i].getQuestNo() + ": " + arr2[i].getQuestion() + " <br /> Response: " + arr2[i].getAnswerResponse();
                    lblSurveyReview.Text += "<br /> <br />";
                }
            }
        }

        if (arr1.Count > 0)
        {
            lblSurveyReview.Text += "<br /><br /><strong> Multiple Choice Question(s): <br /></strong>";
            for (int i = 0; i < arr1.Count; i++)
            {
                List<string> selectedAnswers = arr1[i].getAnswerResponses();

                if (selectedAnswers.Count > 0)
                {
                    lblSurveyReview.Text += "<br />  Q" + arr1[i].getQuestNo() + ": " + arr1[i].getQuestion() + "";

                    for (int j = 0; j < selectedAnswers.Count; j++)
                    {
                        lblSurveyReview.Text += "<br /> Response: " + selectedAnswers[j];
                    }
                    lblSurveyReview.Text += "<br /> <br />";
                }
            }
        }


    }


}

internal class mySurvey3
{

    //private string question;
    //private List<string> answers;
    //private bool multipleAns;
    //private int questNo;

    private string name;
    private string activationDate;
    private string endDate;
    private string participants;
    private string sID;

    public mySurvey3(string n,string ad, string ed, string p, string id)
    {
        this.name = n;
        this.activationDate = ad;
        this.endDate = ed;
        this.participants = p;
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
    public string getParticipants()
    {
        return this.participants;
    }
}

internal class MCQTake
{

    private string question;
    private List<string> answers;
    //Array for answer(s)
    private List<string> selectedAnswer;

    private bool multipleAns;
    private int questNo;
    private string questionID = "";

    public MCQTake(string q, bool ma, int qn)
    {
        this.question = q;
        answers = new List<string>();

        this.selectedAnswer = new List<string>();

        this.multipleAns = ma;
        this.questNo = qn;
    }

    public MCQTake(string q, bool ma, int qn, string qID)
    {
        this.question = q;
        answers = new List<string>();
        this.multipleAns = ma;
        this.questNo = qn;
        this.questionID = qID;

        this.selectedAnswer = new List<string>();
    }
    public void removeAnswers()
    {
        this.answers.Clear();
    }
    public void addAnswer(string a)
    {
        this.answers.Add(a);
    }

    public void reportAnswerResponse(string a)
    {
        this.selectedAnswer.Add(a);
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
    public List<string> getSelectedAnswers()
    {
        return this.selectedAnswer;
    }
    public List<string> getAnswers()
    {
        return answers;
    }

    public void setAnswerResponses(List<string> responses)
    {
        selectedAnswer.Clear();
        this.selectedAnswer = responses;
    }

    public List<string> getAnswerResponses()
    {
        return this.selectedAnswer;
    }

    public bool getMultOpt()
    {
        return this.multipleAns;
    }


}

internal class SAQTake
{
    private string questionID = "";
    private string question;
    private string answerResponse;
    private int questNo;

    public SAQTake(string q, int qn, string qID)
    {
        this.question = q;
        answerResponse = "";
        this.questNo = qn;
        this.questionID = qID;
    }


    public void setQuestion(string newQuest)
    {
        this.question = newQuest;
    }

    public string getQuestion()
    {
        return this.question;
    }

    public string getAnswerResponse()
    {
        return this.answerResponse;
    }

    public void setAnswerResponse(string a)
    {
        this.answerResponse = a;
    }

    public int getQuestNo()
    {
        return this.questNo;
    }
    public void setQuestNo(int qNum)
    {
        this.questNo = qNum;
    }

 
    public void addAnswer(string a)
    {
        this.answerResponse = a;
    }
}