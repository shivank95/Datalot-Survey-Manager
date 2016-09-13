using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Data.OleDb;

public partial class takeSurveyPublic : System.Web.UI.Page
{

    private String DBConnectionString = Configuration.ConnectionString;
    //"Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=dev.techfit;Integrated Security=SSPI";


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserID"] == null || ((string)Session["Type"] != "admin" && (string)Session["Type"] != "Teacher"))
        {
            Response.Redirect("DatalotLogin.aspx?err=You must log in to view student records");
        }
        //For Testing
        lblTester.Visible = false;

        getInfo();

        if (!IsPostBack)
        {

            
            

            //This method does everything including getting id from the old page, loading the entire survey and validation
            Session["surveyID"] = "";
            Session["groupID"] = "";
            Session["currentSurvey"] = "";
            Session["mcqArray"] = new List<MCQTake>();
            Session["saqArray"] = new List<SAQTake>();
            Session["currentQNum"] = 0;
            Session["questCnt"] = 0;

            Session["groupSurveyIDs"] = new List<string>();
            Session["currentSurveyNum"] = 0;

            Session["questionArray"] = new List<Object>();

            getSurveyID();

            //myTable.Visible = false;
        }

    }


    private void getInfo()
    {
        string text = Request.QueryString["getInfo"];
        lblResult.Text = text;
        ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblResult.ClientID + "').style.display='none'\",5000)</script>");
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
        //lblErr.Text = "GROUP ID: " + tempID;
        if (tempID == "" || tempID == null)
        {
            Response.Redirect("DatalotLogin.aspx?err=You must log in to view student records");
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
            List<Object> questionArray = new List<Object>();

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
                    questionArray.Add(currentSAQ);
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
                    questionArray.Add(currentMCQ);
                }
            }

            //Set the global session arrays of these questions to these temporary values
            Session["mcqArray"] = loadMCQ;
            Session["saqArray"] = loadSAQ;
            Session["questionArray"] = questionArray;

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

        Response.Redirect("findTakeSurveyPublic.aspx");
    }

    private bool addSAQAnswer(int currentQNum)
    {

        //Get Answer
        string response = txtResponse.Text;

        if (response == null || response == "")
        {
            return false;
        }
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

        return true;
    }

    protected void saveSAQAndNextQuestion(object sender, EventArgs e)
    {
        //Get currentQuestion
        int currentQNum = (int)Session["currentQNum"];


        bool answered = addSAQAnswer(currentQNum);

        if (answered == false)
        {
            lblResult.Text = "Please answer the question before moving on!";
            return;
        }

        if (currentQNum == (int)Session["questCnt"])
        {
            submitSurvey();
        }
        else
        {
            //Go to Next Question
            currentQNum++;
            Session["currentQNum"] = currentQNum;
            loadQuestion(currentQNum);
        }

    }

    private bool addMCQAnswer(int currentQNum)
    {
        //Get Response
        List<string> answers = new List<string>();

        bool answered = false;

        //Radio Buttons
        if (radioAnswers.Items.Count > 0)
        {
            
            for (int i = 0; i < radioAnswers.Items.Count; i++)
            {
                ListItem tempItem = radioAnswers.Items[i];
                if (tempItem.Selected == true)
                {
                    answers.Add(tempItem.Value);
                    answered = true;
                    //Testing
                    //lblResult.Text = tempItem.Value;

                    break;
                }
            }
        }
        else
        {
            

            for (int i = 0; i < ckbAnswers.Items.Count; i++)
            {
                ListItem tempItem = ckbAnswers.Items[i];
                if (tempItem.Selected == true)
                {
                    answers.Add(tempItem.Value);
                    answered = true;
                }
            }

        }

        if (answered == false)
        {
            return false;
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

        return true;
    }

    protected void saveMCQAndNextQuestion(object sender, EventArgs e)
    {
        //Get currentQuestion
        int currentQNum = (int)Session["currentQNum"];


        bool answered = addMCQAnswer(currentQNum);
        if (answered == false)
        {
            lblResult.Text = "Please answer the question before moving on!";
            return;
        }


        if (currentQNum == (int)Session["questCnt"])
        {
            submitSurvey();
        }
        else
        {
            //Go to Next Question
            currentQNum++;
            Session["currentQNum"] = currentQNum;
            loadQuestion(currentQNum);
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

    //Validate Answers
    private string validateAnswers ()
    {
        string valid = "true";

        

        return valid;
    }

    private string getSurveyGroupID (string surveyID, string groupID)
    {
        string sgID = "";

        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;

        try
        {
            //Open Connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();

            //Prepare sql
            string sql = "SELECT SurveyGroupID FROM spirit.DatalotAssociatedSurveyGroup WHERE SurveyID = " + surveyID + " AND GroupID = " + groupID + ";";


            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                sgID = (string)rdr["SurveyGroupID"].ToString();
            }

            return sgID;
        }

        catch (Exception findExc)
        {
            lblErr.Text = "Error! Unable to find Survey..." + findExc.Message + findExc.StackTrace;
        }

        finally
        {
            db.Close();
        }

        return sgID;
    }

    private string getGroupName(string groupID)
    {
        string groupName = "";

        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;

        try
        {
            //Open Connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();

            //Prepare sql
            string sql = "SELECT Name FROM spirit.DatalotSurveyGroup WHERE GroupID = " + groupID + ";";


            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                groupName = (string)rdr["Name"].ToString();
            }

            return groupName;
        }

        catch (Exception findExc)
        {
            lblErr.Text = "Error! Unable to find Survey..." + findExc.Message + findExc.StackTrace;
        }

        finally
        {
            db.Close();
        }

        return groupName;
    }

    private void saveResponse(string sgID, string groupID, string surveyID, string techfitID, string questionID, string answerText, string answerChoiceNum)
    {
        string sql = "INSERT INTO spirit.DatalotQuestionResponse (SurveyGroupID, GroupID, SurveyID, TechfitID, QuestionID, AnswerText, AnswerChoiceNumber) VALUES (" + sgID + ", " + groupID + ", " + surveyID + ", '" +
           techfitID + "', " + questionID + ", '" + answerText + "', '" + answerChoiceNum + "');" ;

        executeCommand(sql);
    }

    //Overloaded method without an answer choice number
    private void saveResponse(string sgID, string groupID, string surveyID, string techfitID, string questionID, string answerText)
    {
        string sql = "INSERT INTO spirit.DatalotQuestionResponse (SurveyGroupID, GroupID, SurveyID, TechfitID, QuestionID, AnswerText) VALUES (" + sgID + ", " + groupID + ", " + surveyID + ", '" +
           techfitID + "', " + questionID + ", '" + answerText + "');";

        executeCommand(sql);
    }

    //There shouldnt be more than one response from the same person for the same survey in the same group
    private void cleanResponseTable(string surveyID, string groupID, string techfitID)
    {
        string sql = "DELETE FROM spirit.DatalotQuestionResponse WHERE SurveyID = " + surveyID + " AND GroupID = " + groupID + " AND TechfitID = '" + techfitID + "'; ";

        //Execute Command
        executeCommand(sql);
    }

    private void executeCommand(string sql)
    {

        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;


        try
        {
            //Open Connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();


        }

        catch (Exception exec)
        {
            lblErr.Text = "Error! Unable to find Survey..." + exec.Message + exec.StackTrace;
        }

        finally
        {
            db.Close();
        }
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

    private void submitSurvey()
    {

        string valid = validateAnswers();

        if (valid != "true")
        {
            lblResult.Text = "All questions have not been answered. Please answer all questions before submitting the current Survey.";
            return;
        }

       

        //SUBMIT RESPONSE FROM SURVEY

        mySurvey3 currentSurvey = (mySurvey3)Session["currentSurvey"];

        string surveyID = currentSurvey.getID();
        string groupID = (string)Session["GroupID"];
        string surveyGroupID = getSurveyGroupID(surveyID, groupID);
        string techfitID = Session["UserID"].ToString();

        if (surveyGroupID == "")
        {
            lblErr.Text = "Something wrong";
        }
        else
        {
            lblErr.Text = "SurveyID: " + surveyID + " GID: " + groupID + " SGID: " + surveyGroupID + " techfitID: " + techfitID;

        }

        //Clean the Survey for integrity purposes.
        cleanResponseTable(surveyID, groupID, techfitID);


        List<SAQTake> tempSAQList = (List<SAQTake>)Session["saqarray"];
        List<MCQTake> tempMCQList = (List<MCQTake>)Session["mcqarray"];


        //Save to Database all the SAQs
        for (int i = 0; i < tempSAQList.Count; i++)
        {
            SAQTake currentSAQ = tempSAQList[i];
            string questionID = currentSAQ.getQuestionID();
            string answerText = currentSAQ.getAnswerResponse();
            answerText = escapeSingleQuote(answerText);
            saveResponse(surveyGroupID, groupID, surveyID, techfitID, questionID, answerText);
        }

        //Save to Database all the MCQs
        for (int i = 0; i < tempMCQList.Count; i++)
        {
            MCQTake currentMCQ = tempMCQList[i];
            string questionID = currentMCQ.getQuestionID();
            List<string> answerList = currentMCQ.getSelectedAnswers();
            string answerText = concatList(answerList);
            answerText = escapeSingleQuote(answerText);
            string answerChoiceNums = getAnswerChoiceNumbers(currentMCQ);


            saveResponse(surveyGroupID, groupID, surveyID, techfitID, questionID, answerText, answerChoiceNums);
        }

        //lblResult.Text = "Survey Submitted..";


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
            string temp = updateSurveyStatus(techfitID, groupID);
            Response.Redirect("FindTakeSurveyPublic.aspx?getInfo=Survey Response Submitted for " + temp);
        }
        
    }

    private string updateSurveyStatus(string techfitID, string groupID) 
    {
        string groupName = getGroupName(groupID);

        //string group1 = "first day of participation in summer professional development (PD) program";
        //string group2 = "Last day of summer PD program";
        //string group3 = "Last day of the 5th week of the after - school program,";
        //string group4 = "1 week after the finishing date of the after-school program implementation";

        string[] groups = { "first day of participation in summer professional development (PD) program", "Last day of summer PD program", "Last day of the 5th week of the after - school program,", "1 week after the finishing date of the after-school program implementation" };

        double high = 0.0;
        int highGroupNum = 0;
        string highGroupname = "";
        //Check which group this is similar to the most
        for (int i = 0; i < 4; i++)
        {
            double tempSim = CalculateSimilarity(groupName, groups[i]);

            if (tempSim > high)
            {
                high = tempSim;
                highGroupNum = i+1;
                highGroupname = groups[i];
            }
            
        }

        double offset = 0.50;

        if (high > offset)
        {
            generateSql(highGroupNum, techfitID);
        }


        return "" + highGroupNum + " " + high;
    }

    private void generateSql(int groupNumber, string techfitID)
    {
        string groupName = "Group" + groupNumber;

        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;

        try
        {
            //open connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();



            //See if the TechfitID exists in the table
            string sql = "SELECT TechfitID, Group1 FROM spirit.DatalotSurveyStatus WHERE TechfitID = '" + techfitID + "';";

            bool exists = false;

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                exists = true;
            }

            if (exists == true)
            {
                sql = "UPDATE spirit.DatalotSurveyStatus SET " + groupName + " = 1 WHERE TechfitID = '" + techfitID + "';";
            }
            else
            {
                sql = "INSERT INTO spirit.DatalotSurveyStatus (TechfitID, " + groupName + ") VALUES ('" + techfitID + "', 1);"; 
            }

            //Execute Command
            cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

        }

        catch (Exception exc)
        {
            lblResult.Text = "Error! Unable to Save Data to Survey Status..." + exc.Message + exc.StackTrace;

        }
        finally
        {
            db.Close();
        }
    }

    private int ComputeLevenshteinDistance(string source, string target)
    {
        if ((source == null) || (target == null)) return 0;
        if ((source.Length == 0) || (target.Length == 0)) return 0;
        if (source == target) return source.Length;

        int sourceWordCount = source.Length;
        int targetWordCount = target.Length;

        // Step 1
        if (sourceWordCount == 0)
            return targetWordCount;

        if (targetWordCount == 0)
            return sourceWordCount;

        int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

        // Step 2
        for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
        for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

        for (int i = 1; i <= sourceWordCount; i++)
        {
            for (int j = 1; j <= targetWordCount; j++)
            {
                // Step 3
                int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                // Step 4
                distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
            }
        }

        return distance[sourceWordCount, targetWordCount];
    }

    private double CalculateSimilarity(string source, string target)
    {
        if ((source == null) || (target == null)) return 0.0;
        if ((source.Length == 0) || (target.Length == 0)) return 0.0;
        if (source == target) return 1.0;

        int stepsToSame = ComputeLevenshteinDistance(source, target);
        return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));
    }

    private string concatList (List<string> answers)
    {

        if (answers.Count == 1)
        {
            return answers[0];
        }
        string ans = "";

        for (int i = 0; i < answers.Count; i++)
        {
            ans += "Answer" +(i+1) + ": '" + answers[i] + "',";
        }

        
        return ans.Substring(0, ans.Length - 1);
    }

    private string getAnswerChoiceNumbers(MCQTake currentMCQ)
    {
        List<string> answerChoices = currentMCQ.getAnswers();

        List<string> selectedAnswers = currentMCQ.getSelectedAnswers();

        string acn = "";

        for (int i = 0; i < selectedAnswers.Count; i++)
        {

            for (int j = 0; j < answerChoices.Count; j++)
            {
                if (selectedAnswers[i] == answerChoices[j])
                {
                    acn += "" + (j + 1) + ",";
                }
            }
        }

        return acn.Substring(0, acn.Length - 1);
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

/*
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

    public MCQTake(string q, bool ma, int qn, string qID)
    {
        this.question = q;
        answers = new List<string>();
        this.multipleAns = ma;
        this.questNo = qn;
        this.questionID = qID;

        this.selectedAnswer = new List<string>();
        
    }

    public string getQuestionID()
    {
        return this.questionID;
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
    public string getQuestionID()
    {
        return this.questionID;
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
*/