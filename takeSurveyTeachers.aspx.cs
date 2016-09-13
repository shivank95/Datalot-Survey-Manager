using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Data.OleDb;

public partial class takeSurveyTeachers : System.Web.UI.Page
{

    private String DBConnectionString = Configuration.ConnectionString;
    //"Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=dev.techfit;Integrated Security=SSPI";


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserID"] == null || ((string)Session["Type"] != "admin" && (string)Session["Type"] != "Teachers" && (string)Session["Type"] != "Students"))
        {
            Response.Redirect("DatalotLogin.aspx?err=User must log in to view student records");
        }
        //For Testing
        lblTester.Text = "";
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
        else
        {
            string surveyID = (string)Session["surveyID"];
            loadSurvey(surveyID);
        }

    }


    private void getInfo()
    {
        string text = Request.QueryString["getInfo"];
        lblResult.Text = text;
        ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblResult.ClientID + "').style.display='none'\",6000)</script>");
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
        tempSurveyArray = new List<string>();

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

            Session["groupSurveyIDs"] = tempSurveyArray;

            if (surveyCnt == 0)
            {
                lblResult.Text = "There are no surveys associated with this group. Please try again...";
                //myTable.Visible = false;
                //myTable2.Visible = false;
            }
            else
            {
                loadSurvey(tempSurveyArray[0]);
                Session["currentSurveyNum"] = 0;

                if ((int)Session["currentSurveyNum"] == tempSurveyArray.Count - 1)
                {
                    btnSaveAndNext.Text = "Submit Survey";
                }

            }
            //If there are no surveys then tell that to the user
         }
        catch (Exception loadExc)
        {
            lblErr.Text = "Error! Unable to load Group..." + loadExc.Message + loadExc.StackTrace;
        }
        finally
        {
            db.Close();
        }
    }

    //Assigns Session variable currentSurvey
    private void loadSurvey(string surveyID)
    {

        Session["surveyID"] = surveyID;

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


            myTable.Rows.Clear();
            //Start LoadingQuestions from Question 1
            for (int i = 1; i <= qCountInt; i++)
            {
                loadQuestion(i);
            }

        }

        catch (Exception loadExc)
        {
            lblErr.Text = "Error! Unable to load All Questions..." + loadExc.Message + loadExc.StackTrace;
        }
        finally
        {
            db.Close();
        }

    }

    private void addQuestionToTable(string question, int questionNumber, Object tempQuestion)
    {
        string surveyID = (string)Session["surveyID"];

        //ID protocol
        surveyID += "QN";

        //Create New Row
        HtmlTableRow newRow = new HtmlTableRow();
        newRow.ID = "row" + surveyID + questionNumber;

        //lblTester.Text = "NEW ROW ID: " + newRow.ID;

        //Create New Cell
        HtmlTableCell newCell = new HtmlTableCell();
        newCell.ID = "cell" + surveyID + questionNumber;
        newCell.Width = "675px";

        //lblTester.Visible = true;
        //lblTester.Text = "Current SAQ: " + " Current MCQ: " + tempMCQ.ToString();

        if (tempQuestion.GetType().ToString() == "SAQTake")         //Short Answer Question
        {
            //Create New Label
            Label lblQuestion = new Label();
            lblQuestion.Text = "<br /><br />Question " + questionNumber + ": " + question + "<br /><br />";

            //Add Label to Cell
            newCell.Controls.Add(lblQuestion);


            TextBox txtAnswer = new TextBox();
            txtAnswer.ID = "txt" + surveyID + questionNumber;
            txtAnswer.TextMode = TextBoxMode.MultiLine;
            txtAnswer.Width = 400;

            newCell.Controls.Add(txtAnswer);

        }
        else if (tempQuestion.GetType().ToString() == "MCQTake")   //Multiple Choice
        {
            MCQTake tempMCQ = (MCQTake)tempQuestion;

            //Create New Label
            Label lblQuestion = new Label();
            lblQuestion.Text = "<br /><br />Question " + questionNumber + ": " + question;

            //Add Label to Cell
            newCell.Controls.Add(lblQuestion);


            //Get answers
            List<string> tempAnswers = tempMCQ.getAnswers();
            bool multOpt = tempMCQ.getMultOpt();

            if (multOpt == false)
            {
                RadioButtonList radioList = new RadioButtonList();
                radioList.ID = "radio" + surveyID + questionNumber;

                for (int j = 0; j < tempAnswers.Count; j++)
                {
                    ListItem tempItem = new ListItem(tempAnswers[j], tempAnswers[j]);

                    radioList.Items.Add(tempItem);
                }

                newCell.Controls.Add(radioList);

            }

            else
            {
                CheckBoxList ckbList = new CheckBoxList();
                ckbList.ID = "ckbList" + surveyID + questionNumber;

                for (int j = 0; j < tempAnswers.Count; j++)
                {
                    ListItem tempItem = new ListItem(tempAnswers[j], tempAnswers[j]);

                    ckbList.Items.Add(tempItem);
                }

                newCell.Controls.Add(ckbList);
            }
        }

        

        //Add Cell to Row
        newRow.Cells.Add(newCell);

        //Add Row to Table
        myTable.Rows.Add(newRow);
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

                

                List<Object> questionArray = (List<Object>)Session["questionArray"];
                

                addQuestionToTable(currentSAQ.getQuestion(), questNumber, questionArray[questNumber - 1]);

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



                List<Object> questionArray = (List<Object>)Session["questionArray"];


                addQuestionToTable(currentMCQ.getQuestion(), questNumber, questionArray[questNumber - 1]);



                break;

                /*//get answers
                List<string> tempAnswers = currentMCQ.getAnswers();

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
                        //radioAnswers.Items.Add(tempItem);
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

                        //ckbAnswers.Items.Add(tempItem);
                    }
                }*/

            }
        }





                //update currentQuestionNumber

            }
    private void saveAnswers()
    {
        List<SAQTake> tempSAQList = (List<SAQTake>)Session["saqarray"];
        List<MCQTake> tempMCQList = (List<MCQTake>)Session["mcqarray"];

        int questCnt = (int)Session["questCnt"];

        string surveyID = (string) Session["surveyID"];

        for (int i = 1; i <= questCnt; i++)
        {
            getAnswers(i, surveyID, tempSAQList, tempMCQList);
        }
    }

    private void getAnswers(int questNumber, string surveyID, List<SAQTake> tempSAQList, List<MCQTake> tempMCQList)
    {

        surveyID += "QN";

        bool found = false;

        lblTester.Text += "Cnt: " + myTable.Rows.Count;



        //Get the row and cell based on Protocol
        HtmlTableRow currentRow = (HtmlTableRow)myTable.FindControl("row" + surveyID + questNumber);

        
        HtmlTableCell currentCell = (HtmlTableCell)currentRow.FindControl("cell" + surveyID + questNumber);


        //Find the Question
        for (int i = 0; i < tempSAQList.Count; i++)
        {

            int tempCnt = tempSAQList[i].getQuestNo();
            if (tempCnt == questNumber)
            {
                //Found
                found = true;

                SAQTake currentSAQ = tempSAQList[i];

                //Find the Control Based on Protocol
                TextBox txtAnswer = (TextBox)currentCell.FindControl("txt" + surveyID + questNumber);
                string answer = txtAnswer.Text;

                currentSAQ.addAnswer(answer);

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

                List<string> answers = new List<string>();

                //Find Control Based on Protocol
                if (currentMCQ.getMultOpt() == false)
                {
                    RadioButtonList radioAnswers = (RadioButtonList)currentCell.FindControl("radio" + surveyID + questNumber);

                    if (radioAnswers.Items.Count > 0)
                    {
                        for (int j = 0; j < radioAnswers.Items.Count; j++)
                        {
                            ListItem tempItem = radioAnswers.Items[j];
                            if (tempItem.Selected == true)
                            {
                                answers.Add(tempItem.Value);

                                //Testing
                                //lblTester.Text = tempItem.Value;

                                break;
                            }
                        }
                    }
                }

                else
                {
                    CheckBoxList ckbAnswers = (CheckBoxList)currentCell.FindControl("ckbList" + surveyID + questNumber);

                    if (ckbAnswers.Items.Count > 0)
                    {
                        for (int j = 0; j < ckbAnswers.Items.Count; j++)
                        {
                            ListItem tempItem = ckbAnswers.Items[j];
                            if (tempItem.Selected == true)
                            {
                                answers.Add(tempItem.Value);
                            }
                        }
                    }

                }

                currentMCQ.setAnswerResponses(answers);

                //Testing
                for (int z = 0; z < answers.Count; z++)
                {
                    lblTester.Text += answers[z] + "<br /> <br />";
                }
                

                break;
            }
        }


    }

    protected void submitSurvey(object sender, EventArgs e)
    {
       
        //Save Survey
        saveAnswers();

        //Validate
        string valid = validateAnswers();

        if (valid != "true")
        {
            lblResult.Text = valid;
            return;
        }

        //SUBMIT RESPONSE FROM SURVEY

        //Get criticals
        mySurvey3 currentSurvey = (mySurvey3)Session["currentSurvey"];

        string surveyID = currentSurvey.getID();
        string groupID = (string)Session["GroupID"];
        string surveyGroupID = getSurveyGroupID(surveyID, groupID);
        string techfitID = Session["UserID"].ToString();

        if (surveyGroupID == "")
        {
            //lblErr.Text = "Something wrong";
        }
        else
        {
            //lblErr.Text = "SurveyID: " + surveyID + " GID: " + groupID + " SGID: " + surveyGroupID + " techfitID: " + techfitID;

        }

        //Clean the Survey for integrity issues.
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

        //Toggle to next if there is
        List<string> tempSurveyArray = (List<string>)Session["groupSurveyIDs"];
        int currentSurveyNum = (int)Session["currentSurveyNum"];
        currentSurveyNum++;
        if (currentSurveyNum < tempSurveyArray.Count)
        {
            Session["currentSurveyNum"] = currentSurveyNum;
            lblResult.Text = "Survey Response Saved!";

            if (currentSurveyNum == tempSurveyArray.Count - 1)
            {
                btnSaveAndNext.Text = "Submit Survey";
            }

           loadSurvey(tempSurveyArray[currentSurveyNum]);
        }
        else
        {
            //string temp = updateSurveyStatus(techfitID, groupID);

            //Check if the group's status needs to be tracked
            string check = getGroupStatusCheck(groupID);
            if (check == "True" || check.Contains("T"))
            {
                updateSurveyStatusNew(techfitID);
            }

       
            Response.Redirect("FindTakeSurveyPublic.aspx?getInfo=Survey Response Submitted! ");
        }


    }

        //Validate Answers
    private string validateAnswers ()
    {
        string valid = "true";

        int notAnswered = 0;

        List<SAQTake> tempSAQList = (List<SAQTake>)Session["saqarray"];
        List<MCQTake> tempMCQList = (List<MCQTake>)Session["mcqarray"];

        //Validate Short Answer Questions
        for (int i = 0; i < tempSAQList.Count; i++)
        {

            SAQTake currentSAQ = tempSAQList[i];

            if (currentSAQ.getAnswerResponse() == "")
            {
                valid = "Please answer all questions before moving on.";
                notAnswered++;
            }

        }

        for (int i = 0; i < tempMCQList.Count; i++)
        {
            MCQTake currentMCQ = tempMCQList[i];

            List<string> answers = currentMCQ.getAnswerResponses();

            if (answers.Count == 0)
            {
                valid = "Please answer all questions before moving on.";
                notAnswered++;
            }
        }

        if (valid != "true")
        {
            valid += " " + notAnswered + " unanswered question(s).";
        }

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

    private string getGroupStatusCheck(string groupID)
    {
        string groupCheck = "";

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
            string sql = "SELECT StatusCheck FROM spirit.DatalotSurveyGroup WHERE GroupID = " + groupID + ";";


            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                groupCheck = (string)rdr["StatusCheck"].ToString();
            }

            return groupCheck;
        }

        catch (Exception findExc)
        {
            lblErr.Text = "Error! Unable to find Survey..." + findExc.Message + findExc.StackTrace;
        }

        finally
        {
            db.Close();
        }

        return groupCheck;
    }

    private string getGroupCategory(string groupID)
    {
        string groupCat = "";

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
            string sql = "SELECT Category FROM spirit.DatalotSurveyGroup WHERE GroupID = " + groupID + ";";


            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                groupCat = (string)rdr["Category"].ToString();
            }

            return groupCat;
        }

        catch (Exception findExc)
        {
            lblErr.Text = "Error! Unable to find Survey..." + findExc.Message + findExc.StackTrace;
        }

        finally
        {
            db.Close();
        }

        return groupCat;
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

    private void updateSurveyStatusNew(string techfitID)
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

            bool exists = false;

            //NEW CODE************* FALL 2016
            string currentGroupID = (string)Session["GroupID"];
            string sql2 = "SELECT * FROM spirit.DatalotSurveyStatusNew WHERE TechfitID = '" + techfitID + "' AND GroupID = '" + currentGroupID + "';";

            OleDbCommand cmd = new OleDbCommand(sql2, db);
            rdr = cmd.ExecuteReader();
            string sql = "";

            string category = getGroupCategory(currentGroupID);
            //exists = false;

            while (rdr.Read())
            {
                exists = true;
                string takenCount = (string)rdr["TakeCount"].ToString();
                int takenCountInt = Int32.Parse(takenCount);
                takenCountInt++;
                sql = "UPDATE spirit.DatalotSurveyStatusNew SET TakeCount = " + takenCountInt + " WHERE TechfitID = '" + techfitID + "' AND GroupID = '" + currentGroupID + "';";
            }

            if (exists == false)
            {
                sql = "INSERT INTO spirit.DatalotSurveyStatusNew (TechfitID, GroupID, TakeCount, Category) VALUES ('" + techfitID + "', '" + currentGroupID + "', 1, '" + category + "');";
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