using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Data.OleDb;

public partial class createSurvey : System.Web.UI.Page
{

    private String surveyName = "";
    private static String surveyGrp = "";
    private String DBConnectionString = Configuration.ConnectionString;
    //"Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=dev.techfit;Integrated Security=SSPI";


    protected void Page_Load(object sender, EventArgs e)
    {

        if (Session["UserID"] == null || (string)Session["Type"] != "admin")
        {
            Response.Redirect("DatalotAdminLogin.aspx?err=You must log in to view student records");
        }

        //myTable.Visible = false;
        lblTester.Visible = false;
        //myTable2.Visible = false;
        Page.MaintainScrollPositionOnPostBack = true;
        
        
        if (!IsPostBack)
        {
            //Initial Values
            //questCnt = 0;
            myTable.Visible = false;
            myTable2.Visible = false;
            //Initializing session variables
            Session["questCnt"] = 0;
            Session["mcqArray"] = new List<MCQ>();
            Session["saqArray"] = new List<SAQ>();
            Session["lastQuestionType"] = new List<String>();
            Session["dateReady"] = false;
            Session["activationDate"] = new DateTime();
            Session["endDate"] = new DateTime();
            Session["participants"] = "";
            Session["surveyGroup"] = "";
        }
    }
    protected void backButton(object sender, EventArgs e)
    {

        Response.Redirect("Datalot.aspx");
    }

    protected void addMCQ(object sender, EventArgs e)
    {
        //lblTester.Text = schoolList.SelectedItem.Text;

        //Response.Redirect("Datalot.aspx");
        myTable.Visible = false;
        myTable2.Visible = true;
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

    protected void addSAQ(object sender, EventArgs e)
    {
        //lblTester.Text = schoolList.SelectedItem.Text;

        //Response.Redirect("Datalot.aspx");
        myTable2.Visible = false;
        myTable.Visible = true;
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
    private void updateReview(List<MCQ> arr1, List<SAQ> arr2)
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

        List<String> tempUndo = (List<String>)Session["lastQuestionType"];

        

        //Otherwise, check the last question
        if (tempUndo[tempUndo.Count - 1] == "SAQ")
        {
            
            //Remove last element from the SAQ array
            List<SAQ> tempSAQ = (List<SAQ>)Session["saqArray"];
            tempSAQ.RemoveAt(tempSAQ.Count - 1);
            Session["saqArray"] = tempSAQ;
            List<MCQ> tempMCQ = (List<MCQ>)Session["mcqArray"];


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
        else if (tempUndo[tempUndo.Count - 1] == "MCQ")
        {

            //Remove last element from the SAQ array
            List<MCQ> tempMCQ = (List<MCQ>)Session["mcqArray"];
            tempMCQ.RemoveAt(tempMCQ.Count - 1);
            Session["mcqArray"] = tempMCQ;
            List<SAQ> tempSAQ = (List<SAQ>)Session["saqArray"];

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


        


        SAQ curQuest = new SAQ(txtQuest.Text, tempCnt);
        //saqArray.Add(curQuest);
        
        //Using Session Variables
        List<SAQ> saqTemp = (List<SAQ>)Session["saqArray"];
        saqTemp.Add(curQuest);
        Session["saqArray"] = saqTemp;

        //TEST
        List<MCQ> mcqTemp = (List<MCQ>)Session["mcqArray"];
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
        allowChcks.Checked = false;
        MCQ curQuest = new MCQ(txtMCQQuest.Text,multChoices,tempCnt);

        
        //Add Answer Choices to the MCQ Object
        for (int i = 0; i < choiceArr.Length; i++)
        {
            if (choiceArr[i] != "")
            {
                curQuest.addAnswer(choiceArr[i]);
            }
        }

        //New Session Way. Add to temporary array
        List<MCQ> mcqTemp = (List<MCQ>)Session["mcqArray"];
        mcqTemp.Add(curQuest);

        List<SAQ> saqTemp = (List<SAQ>)Session["saqArray"];

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
    //protected void updateDate(object sender, EventArgs e)
    //{
    //    bool complete = true;

    //    int startYear = 2015;
    //    int startMonth = 1;
    //    int startDay = 1;
    //    int endYear = 2015;
    //    int endMonth = 12;
    //    int endDay = 1;

    //    //SURVEY START DATES

    //    if (ddlStartYear.SelectedIndex < 1)
    //    {
    //        complete = false;
    //    }
    //    else
    //    {
    //        startYear = 2015 + ddlStartYear.SelectedIndex;
    //    }

    //    if (ddlStartMonth.SelectedIndex < 1)
    //    {
    //        complete = false;
    //    }
    //    else
    //    {
    //        startMonth = ddlStartMonth.SelectedIndex;
    //    }

    //    if (ddlStartDay.SelectedIndex < 1)
    //    {
    //        complete = false;
    //    }
    //    else
    //    {
    //        startDay = ddlStartDay.SelectedIndex;
    //    }


    //    //SURVEY END DATES
    //    if (ddlEndYear.SelectedIndex < 1)
    //    {
    //        complete = false;
    //    }
    //    else
    //    {
    //        endYear = 2015 + ddlEndYear.SelectedIndex;
    //    }

    //    if (ddlEndMonth.SelectedIndex < 1)
    //    {
    //        complete = false;
    //    }
    //    else
    //    {
    //        endMonth = ddlEndMonth.SelectedIndex;
    //    }

    //    if (ddlEndDay.SelectedIndex < 1)
    //    {
    //        complete = false;
    //    }
    //    else
    //    {
    //        endDay = ddlEndDay.SelectedIndex;
    //    }


    //    //Update Session Variable
    //    bool dateReady = (bool)Session["dateReady"];
    //    dateReady = complete;
    //    Session["dateReady"] = dateReady;

       


    //    //If all dates have been entered. Add them to the session variables
    //    if (complete == true)
    //    {

            
    //        try
    //        {
    //            DateTime tempActDate = new DateTime(startYear, startMonth, startDay);
    //            Session["activationDate"] = tempActDate;

    //            DateTime tempEndDate = new DateTime(endYear, endMonth, endDay);
    //            Session["endDate"] = tempEndDate;

    //            //Compare dates for validity
    //            int dateCompare = DateTime.Compare(tempActDate, tempEndDate);
    //            if (dateCompare >= 0)
    //            {
    //                complete = false;
    //                Session["dateReady"] = complete;
    //                lblResult.Text = "Not a valid Date..";
    //            }
    //            else
    //            {
    //                lblResult.Text = "";
    //                //Testing
    //                //lblResult.Text = "Activation Date: " + ((DateTime)Session["activationDate"]).ToString() +
    //                    //"<br /> End Date: " + ((DateTime)Session["endDate"]).ToString();
    //            }
    //        }
    //        catch (Exception excp)
    //        {
    //            complete = false;
    //            Session["dateReady"] = complete;
    //            lblResult.Text = "Not a valid Date..";
    //        }
          
    //    }

    //}


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

    //protected void getPartGrp(object sender, EventArgs e)
    //{
    //    String participants = "";
    //    if (ddlParticipants.SelectedIndex == 1)
    //    {
    //        participants = "All";
    //    }
    //    else if (ddlParticipants.SelectedIndex == 2)
    //    {
    //        participants = "Students";
    //    }
    //    else if (ddlParticipants.SelectedIndex == 3)
    //    {
    //        participants = "Teachers";
    //    }

    //    Session["participants"] = participants;
    //}

    private string saveAndValidate()
    {
        this.surveyName = txtSurveyName.Text;
        string valid = "valid";

        //Validate surveyName
        if (txtSurveyName.Text == "")
        {
            valid = "Enter a Survey Name..";
        }

        //Validate number of questions
        else if ((int)Session["questCnt"] < 1)
        {
            valid = "The Survey has to have atleast 1 question!";
        }

        
        return valid;
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
                String participants = "All";

                //Escape the Single Quote
                this.surveyName = escapeSingleQuote(this.surveyName);

                //Prepare Query
                /*String sql = "INSERT INTO spirit.DatalotSurvey (Name, ActivationDate, EndDate, Participants, CreationDate) VALUES ('"+ this.surveyName+"', '" + (DateTime)Session["activationDate"] + 
                    "', '" + (DateTime)Session["endDate"] + "', '" + (string)Session["participants"] + "', '" + creationDate + "');";*/

                String sql = "INSERT INTO spirit.DatalotSurvey (Name, ActivationDate, EndDate, Participants, CreationDate) VALUES ('" + this.surveyName + "', '" + creationDate +
                   "', '" + creationDate + "', '" + participants + "', '" + creationDate + "');";

                //Execute Command
                OleDbCommand cmd = new OleDbCommand(sql, db);
                rdr = cmd.ExecuteReader();

                //Get last SurveyID
                sql = "SELECT MAX(SurveyID) AS MaxSurveyID FROM spirit.DatalotSurvey";
                cmd = new OleDbCommand(sql, db);
                rdr = cmd.ExecuteReader();
                string surveyID = "";
                while (rdr.Read())
                {
                    surveyID = (string)rdr["MaxSurveyID"].ToString();
                }
                //Convert to int
                int surveyIDInt = Int32.Parse(surveyID);

                //Add Questions to question table
                List<MCQ> mcqTemp = (List<MCQ>)Session["mcqArray"];
                List<SAQ> saqTemp = (List<SAQ>)Session["saqArray"];

                //ADD SAQ
                for (int i = 0; i < saqTemp.Count; i++)
                {
                    SAQ temp = saqTemp[i];
                    string tempQuestion = temp.getQuestion();
                    tempQuestion = escapeSingleQuote(tempQuestion);
                    string tempType = "SAQ";
                    int tempQuestNo = temp.getQuestNo();
                    string choice1 = "temp";
                    string choice2 = "temp";

                    sql = "INSERT INTO spirit.DatalotQuestion (SurveyID, Question, Qtype, QNumber, Choice1, Choice2) VALUES ('" + surveyIDInt + "', '" + tempQuestion + "', '" + tempType + "', '" +
                        tempQuestNo + "', '" + choice1 + "', '" + choice2 + "');";
                    cmd = new OleDbCommand(sql, db);
                    rdr = cmd.ExecuteReader();

                }

                //ADD MCQ
                for (int i = 0; i < mcqTemp.Count; i++)
                {
                    MCQ temp = mcqTemp[i];
                    string tempQuestion = temp.getQuestion();
                    tempQuestion = escapeSingleQuote(tempQuestion);
                    string tempType = "MCQ";

                    if (temp.getMultOpt() == true)
                    {
                        tempType = "MCQ2";
                    }

                    int tempQuestNo = temp.getQuestNo();
                    List<string> tempAnswers = temp.getAnswers();
                    string choice1 = tempAnswers[0];
                    choice1 = escapeSingleQuote(choice1);
                    string choice2 = tempAnswers[1];
                    choice2 = escapeSingleQuote(choice2);
                    int answerCnt = tempAnswers.Count;

                    //Prepare an incomplete sql statement
                    sql = "INSERT INTO spirit.DatalotQuestion (SurveyID, Question, Qtype, QNumber, Choice1, Choice2, Choice3, Choice4, Choice5, Choice6, Choice7, Choice8, Choice9, Choice10) VALUES ('" + surveyIDInt + "', '"
                        + tempQuestion + "', '" + tempType + "', '" +
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
                lblResult.Text = "Success! Survey Submitted! SurveyID: " + surveyID;
                ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblResult.ClientID + "').style.display='none'\",2000)</script>");

                surveyName = surveyName.Replace("''", "'");
                Response.Redirect("Datalot.aspx?getInfo=Survey '" + surveyName + "' Created!");
            }
            catch (Exception exc)
            {
                lblErr.Text = "Error! Unable to submit Survey..." + exc.Message + exc.StackTrace;
                //ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblResult.ClientID + "').style.display='none'\",2000)</script>");
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

internal class MCQ
{

    private string question;
    private List<string> answers;
    private bool multipleAns;
    private int questNo;

    public MCQ()
    {

    }

    public MCQ(string q, bool ma, int qn)
    {
        this.question = q;
        answers = new List<string>();
        this.multipleAns = ma;
        this.questNo = qn;
    }

    public void addAnswer(string a)
    {
        this.answers.Add(a);
    }

    public string getQuestion()
    {
        return this.question;
    }

    public int getQuestNo()
    {
        return this.questNo;
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

internal class SAQ
{

    private string question;
    private string answer;
    private int questNo;

    public SAQ()
    {

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

    public SAQ(String q, int qn)
    {
        this.question = q;
        answer = "";
        this.questNo = qn;
    }

    public void addAnswer(String a)
    {
        this.answer = a;
    }
}

/*
        questCnt++;
        
        //Create the Question row
        HtmlTableRow row1 = new HtmlTableRow();
        row1.ID = "row" + questCnt;

        HtmlTableCell cell1 = new HtmlTableCell();
        cell1.ID = "cell2" + questCnt;

        Label quest = new Label();
        quest.Text = "Question: ";
        quest.ID = "question" + questCnt;

        cell1.Controls.Add(quest);

        HtmlTableCell cell2 = new HtmlTableCell();
        cell2.ID = "cell2" + questCnt;

        TextBox txtQuest = new TextBox();
        txtQuest.TextMode = TextBoxMode.MultiLine;
        txtQuest.Width = 410;
        txtQuest.ID = "txtQuest" + questCnt;

        cell2.Controls.Add(txtQuest);

        //Add cells to the row
        row1.Cells.Add(cell1);
        row1.Cells.Add(cell2);

        //Create Answer Row
        HtmlTableRow row2 = new HtmlTableRow();
        row2.ID = "row2" + questCnt;

        HtmlTableCell cellSave = new HtmlTableCell();
        cellSave.ID = "cellSave" + questCnt;

        Button saveQuest = new Button();
        saveQuest.Text = "Add Question";
        saveQuest.ID = "save" + questCnt;
        saveQuest.ControlStyle.CssClass = "button";
        saveQuest.Click += new EventHandler(saveQuestion);
        saveQuest.Visible = true;

        cellSave.Controls.Add(saveQuest);

        row2.Cells.Add(cellSave);


        //Add row to table
        myTable.Rows.Add(row1);
        myTable.Rows.Add(row2);

        myTable.Visible = true;
        */
