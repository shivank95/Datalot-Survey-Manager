using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Data.OleDb;

using System.Collections;

public partial class DatalotFormEditor : System.Web.UI.Page
{

    String currentSchoolName = "Carver Edisto Middle School";
    String currentCategory = "Teachers";

    String tableName = "";
    string studentTable = "spirit.TECHFITStudentApps2015";

    string teacherTable = "spirit.TECHFITTeacherApps2015";

    string statusCheckTable = "spirit.DatalotFormStatus";
    string columnCountTable = "spirit.DatalotManualForms";

    //String DBConnectionString = "Provider=SQLOLEDB;Server=techwebdev.ecn.purdue.edu;Database=dev.techfit;uid=dev.techfit.usr01;pwd=ECNdev.techfitpa55;";

    String DBConnectionString = Configuration.ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["UserList"] = new List<StatusUserForms>();
            Session["RowCount"] = 0;
            Session["ColCount"] = 0;
            Session["SchoolName"] = "Carver Edisto Middle School";
            Session["Category"] = "Teachers";
            Session["GroupList"] = new List<UserForm>();
        }
        if (Session["UserID"] == null || Session["Type"] == null)
        {
            Response.Redirect("DatalotAdminLogin.aspx?err=You must log in to view student records");
        }

        if (!Session["UserID"].Equals("40") && !Session["Type"].Equals("admin"))
        {
            Response.Redirect("DatalotAdminLogin.aspx?err=You must log in to view student records");
        }
        //Used for Testing
        //lblRowCnt.Visible = false;

        //Get URL stuff
        getSchoolName();
        getCategoryName();

        if (currentCategory == "Teachers")
        {
            tableName = teacherTable;
        }
        //Student Category
        else
        {
            tableName = studentTable;
        }


        int rowCnt = setRowCnt();
        int colCnt = setColumnCount();

        //Old Code
        //createRows(rowCnt);

        //New Create Rows
        createRowsNew(rowCnt, colCnt);

        //Get Data
        int index = 0;
        int offStudentCount = 0;

        OleDbConnection db = null;
        OleDbDataReader myReader = null;
        try
        {
            //myReader = myCommand.ExecuteReader();
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();

            currentSchoolName = (string)Session["SchoolName"];
            string sql = "SELECT ID, FirstName, LastName, SchoolName, ApplicationID FROM " + tableName + " WHERE SchoolName = '" + currentSchoolName + "' ORDER BY LastName, FirstName;";

            OleDbCommand cmd = new OleDbCommand(sql, db);
            myReader = cmd.ExecuteReader();



            String ID = "";
            String firstName = "";
            String lastName = "";

            //New Code
            List<StatusUserForms> userList = (List<StatusUserForms>)Session["UserList"];
            userList.Clear();


            while (myReader.Read() && index < rowCnt)
            {
                ID = myReader["ID"].ToString();
                firstName = myReader["FirstName"].ToString();
                lastName = myReader["LastName"].ToString();

                int cCnt = 1;

                //New Code
                StatusUserForms tempUser = new StatusUserForms((firstName + " " + lastName), ID);
                userList.Add(tempUser);

                HtmlTableRow currentRow = (HtmlTableRow)myTable.FindControl("row" + index);

                //First Cell
                HtmlTableCell cell1 = (HtmlTableCell)currentRow.FindControl(index + "cell" + cCnt++);
                TextBox txtID = (TextBox)cell1.Controls[0] as TextBox;

                txtID.Text = ID;


                //Second Cell
                HtmlTableCell cell2 = (HtmlTableCell)currentRow.FindControl(index + "cell" + cCnt++);
                TextBox txtName = (TextBox)cell2.Controls[0] as TextBox;

                txtName.Text = firstName + " " + lastName;

                //Increment Index for ID
                index++;

            }

            //Old Update Table
            //updateTable();

            //New Update Table
            updateTableNew();

        }
        catch (Exception e3)
        {
            lblErr.Text = "Error in getting Data..." + e3.Message + " " + e3.StackTrace;
            return;
        }
        finally
        {
            db.Close();
        }

        //Update total student Count
        lblStudentCnt.Text = "Total Number of " + currentCategory + ": " + index;
        lblCategory.Text = "Editing Form statuses for: " + currentCategory;

        //string[] groups = { "first day of participation in summer professional development (PD) program", "Last day of summer PD program", "Last day of the 5th week of the after - school program,", "1 week after the finishing date of the after-school program implementation" };

        //lblGroupDetails.Text = " <br /> Group 1: " + groups[0] + " <br /> Group 2: " + groups[1] + " <br /> Group 3: " + groups[2] + " <br /> Group 4: " + groups[3];   



    }

    private string getGroupName(string groupID)
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

            string sql = "SELECT FormName FROM spirit.DatalotManualForms WHERE FormID = " + groupID + ";";

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                return (string)rdr["FormName"].ToString();
            }

            return null;

        }

        catch (Exception exc)
        {
            lblResult.Text = "Error! Unable to get form names..." + exc.Message + exc.StackTrace;
        }

        finally
        {
            db.Close();
        }

        return null;

    }

    public string getColumnsGroupID(string index)
    {
        List<UserForm> tmpGroup = (List<UserForm>)Session["GroupList"];

        for (int i = 0; i < tmpGroup.Count; i++)
        {

            UserForm tmp = tmpGroup[i];
            if (index == tmp.getColumnIndex())
            {
                return tmp.getID();
            }
        }

        return "" + tmpGroup.Count;

    }
    public void createPreRows(int colCnt)
    {
        //Techfit ID Row
        HtmlTableRow row = new HtmlTableRow();
        row.ID = "staticRow";

        HtmlTableCell cell = new HtmlTableCell();
        cell.ID = "staticCell1";
        cell.Align = "center";
        Label lblTechfitID = new Label();
        lblTechfitID.ID = "staticTechfitIDLabel";
        lblTechfitID.Text = "TechfitID";
        cell.Controls.Add(lblTechfitID);

        HtmlTableCell cell2 = new HtmlTableCell();
        cell2.ID = "staticCell2";
        cell2.Align = "center";
        Label lblName = new Label();
        lblName.ID = "staticNameLabel";
        lblName.Text = "Name";
        cell2.Controls.Add(lblName);

        row.Cells.Add(cell);
        row.Cells.Add(cell2);

        lblGroupLegend.Text = "Forms: <br />";

        for (int i = 0; i < colCnt; i++)
        {

            HtmlTableCell cell3 = new HtmlTableCell();
            cell3.ID = "staticCell" + (i + 3);
            cell3.Width = "40px";
            cell3.Align = "center";

            Label lblGroup = new Label();
            lblGroup.ID = "staticGroup" + (i + 3) + "Label";

            string groupID = getColumnsGroupID("" + (i + 1));

            lblGroup.Text = " <p> " + (i + 1) + "   </p>" ;

            lblGroupLegend.Text += (i + 1) + ": " + getGroupName(groupID) + " <br />";

            cell3.Controls.Add(lblGroup);

            row.Cells.Add(cell3);

        }

        myTable.Rows.Add(row);
    }

    public void createRowsNew(int rowCnt, int colCnt)
    {
        //Clear the table
        myTable.Rows.Clear();

        //PreRows
        createPreRows(colCnt);



        for (int i = 0; i < rowCnt; i++)
        {

            //First 2 columns exists no matter what
            HtmlTableRow row = new HtmlTableRow();
            //row.ID = i.ToString();
            //The ID
            int currentCol = 1;

            row.ID = "row" + i.ToString();

            HtmlTableCell cell = new HtmlTableCell();
            cell.ID = i.ToString() + "cell" + currentCol++;

            TextBox txtID = new TextBox();
            txtID.Enabled = false;
            txtID.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            txtID.ID = "txt1" + i.ToString();
            txtID.Style["text-align"] = "center";
            cell.Controls.Add(txtID);

            //The Name
            HtmlTableCell cell2 = new HtmlTableCell();
            cell2.ID = i.ToString() + "cell" + currentCol++;

            TextBox txtName = new TextBox();
            txtName.Enabled = false;
            txtName.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            txtName.Style["text-align"] = "center";
            txtID.ID = "txt2" + i.ToString();
            cell2.Controls.Add(txtName);

            //Add to ROW
            row.Cells.Add(cell);
            row.Cells.Add(cell2);

            if (currentCategory == "Student")
            {
                //Add 2 more columns
            }

            for (int j = 0; j < colCnt; j++)
            {
                //Add the new column Groups
                HtmlTableCell cellSurveyGroup = new HtmlTableCell();
                cellSurveyGroup.ID = i.ToString() + "cell" + currentCol++;
                cellSurveyGroup.Width = "45px";
                cellSurveyGroup.Align = "center";
                CheckBox tempCkb = new CheckBox();
                tempCkb.ID = i.ToString() + "ckb" + currentCol;
                tempCkb.Enabled = true;
                cellSurveyGroup.Controls.Add(tempCkb);

                row.Cells.Add(cellSurveyGroup);

            }

            myTable.Rows.Add(row);
        }


    }

    public void updateTableNew()
    {
        int rowCount = (int)Session["RowCount"];
        int colCount = (int)Session["ColCount"];
        List<StatusUserForms> userList = (List<StatusUserForms>)Session["UserList"];
        List<UserForm> groupList = (List<UserForm>)Session["GroupList"];


        //Update the User List
        updateUsers();

        for (int i = 0; i < rowCount; i++)
        {

            //Get User details
            StatusUserForms currentUser = userList[i];
            List<string> statusList = currentUser.getStatusList();

            for (int j = 0; j < colCount; j++)
            {
                HtmlTableRow currentRow = (HtmlTableRow)myTable.FindControl("row" + i);

                //Third Cell
                HtmlTableCell cell3 = (HtmlTableCell)currentRow.FindControl(i.ToString() + "cell" + (j + 3));
                CheckBox ckbPCF = (CheckBox)cell3.Controls[0] as CheckBox;

                ckbPCF.Checked = Boolean.Parse(statusList[j]);


            }
        }

    }

    public void updateUsers()
    {
        List<StatusUserForms> userList = (List<StatusUserForms>)Session["UserList"];
        List<UserForm> groupList = (List<UserForm>)Session["GroupList"];

        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;

        try
        {
            //open connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();

            for (int i = 0; i < userList.Count; i++)
            {
                StatusUserForms tempUser = userList[i];
                string currentTechfitID = tempUser.getID();

                for (int j = 0; j < groupList.Count; j++)
                {
                    string currentGroupID = groupList[j].getID();

                    string sql = "SELECT * FROM " + statusCheckTable + " WHERE TechfitID = " + currentTechfitID + " AND FormID = " + currentGroupID + ";";

                    //Execute Command
                    OleDbCommand cmd = new OleDbCommand(sql, db);
                    rdr = cmd.ExecuteReader();
                    int flag = 0;
                    while (rdr.Read())
                    {
                        string techfitID = (string)rdr["TechfitID"].ToString();
                        string groupID = (string)rdr["FormID"].ToString();
                        //string takeCount = (string)rdr["TakeCount"].ToString();
                        string category = (string)rdr["Category"].ToString();
                        tempUser.addStatus("True");
                        flag = 1;
                    }

                    if (flag == 0)
                    {
                        tempUser.addStatus("False");
                    }


                }
                userList[i] = tempUser;

                //Testing
                //lblResult.Text += "<br /> ID " + tempUser.getID() + " " + tempUser.getStatusList()[1] + " Count: " + userList.Count;
            }

            Session["UserList"] = userList;

        }

        catch (Exception exc)
        {
            lblResult.Text = "Error! Unable to load Survey Status Data to get updated lists..." + exc.Message + exc.StackTrace;
        }

        finally
        {
            db.Close();
        }
    }

    //Getting the School Name from the URL

    public void getSchoolName()
    {
        currentSchoolName = Request.QueryString["SchoolNamer"];

        //Default school name
        if (currentSchoolName == null)
        {
            currentSchoolName = "Carver Edisto Middle School";
        }

        lblSchoolName.Text = "School: " + currentSchoolName;

        Session["SchoolName"] = currentSchoolName;
    }

    //Getting the Category / Participants from the URL

    public void getCategoryName()
    {
        currentCategory = Request.QueryString["Category"];

        //Default school name
        if (currentCategory == null)
        {
            currentCategory = "Teachers";
        }

        lblCategory.Text = "Category: " + currentCategory;

        Session["Category"] = currentCategory;
    }

    private int setColumnCount()
    {
        int colCnt = 0;
        OleDbConnection db = null;
        try
        {
            //myReader = myCommand.ExecuteReader();
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();
            OleDbDataReader myReader = null;

            currentSchoolName = (string)Session["SchoolName"];
            currentCategory = (string)Session["Category"];
            List<UserForm> groupList = (List<UserForm>)Session["GroupList"];

            string sql = "SELECT DISTINCT FormID  FROM " + columnCountTable + " WHERE (Category LIKE '%" + currentCategory + "%' OR Category = 'Public' OR Category = 'Anonymous');";

            OleDbCommand cmd = new OleDbCommand(sql, db);
            myReader = cmd.ExecuteReader();

            while (myReader.Read())
            {
                colCnt++;
                string ID = (string)myReader["FormID"].ToString();
                string columnIndex = "" + colCnt;

                UserForm tempGroup = new UserForm(ID, columnIndex);
                groupList.Add(tempGroup);
            }
            //Testing
            if (colCnt > 0)
            {

                lblResult.Text = "Number of Form(s) found: " + colCnt;
            }
            else
            {
                lblResult.Text = "No Forms found..";
            }

            Session["ColCount"] = colCnt;
            Session["GroupList"] = groupList;
            return colCnt;
        }
        catch (Exception e)
        {
            lblErr.Text = "Error in getting column count.." + e.Message + " " + e.StackTrace;
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('There was a Problem..\n" + e.StackTrace.ToString() + "')", true);

        }
        finally
        {
            db.Close();
        }
        return colCnt;
    }


    private int setRowCnt()
    {

        int rowCnt = 0;
        OleDbConnection db = null;
        try
        {
            //myReader = myCommand.ExecuteReader();
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();
            OleDbDataReader myReader = null;

            currentSchoolName = (string)Session["SchoolName"];

            string sql = "SELECT ID, FirstName, LastName, SchoolName, ApplicationID FROM " + tableName + " WHERE SchoolName = '" + currentSchoolName + "' ORDER BY FirstName;";

            OleDbCommand cmd = new OleDbCommand(sql, db);
            myReader = cmd.ExecuteReader();

            while (myReader.Read())
            {
                rowCnt++;
            }

            Session["RowCount"] = rowCnt;
            return rowCnt;
        }
        catch (Exception e)
        {
            lblErr.Text = "Error in getting row count.." + e.Message + " " + e.StackTrace;
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('There was a Problem..\n" + e.StackTrace.ToString() + "')", true);

        }
        finally
        {
            db.Close();
        }
        return rowCnt;
    }
    protected void btnSelectSchool_Click(object sender, EventArgs e)
    {
        Response.Redirect("DatalotFormSS.aspx");
    }

    //Save button
    protected void saveFormData(object sender, EventArgs e)
    {

        int rowCount = (int)Session["RowCount"];
        int colCount = (int)Session["ColCount"];
        List<StatusUserForms> userList = (List<StatusUserForms>)Session["UserList"];
        


        //Update the User List
        //updateUsers();

        for (int i = 0; i < rowCount; i++)
        {

            //Get User details
            StatusUserForms currentUser = userList[i];
            List<string> statusList = currentUser.getStatusList();

            for (int j = 0; j < colCount; j++)
            {
                HtmlTableRow currentRow = (HtmlTableRow)myTable.FindControl("row" + i);

                //Third Cell
                HtmlTableCell cell3 = (HtmlTableCell)currentRow.FindControl(i.ToString() + "cell" + (j + 3));
                CheckBox ckbPCF = (CheckBox)cell3.Controls[0] as CheckBox;

                bool currentCheckbox = ckbPCF.Checked;

                statusList[j] = currentCheckbox.ToString();

                

                //Testing
                //if (currentCheckbox == true)
                //{
                //    //lblResult.Text += "(i,j): (" + i + ", " + j + ") <br />";
                //    lblResult.Text += "<br /> " + currentUser.getID() + " " + statusList[j];
                //}

            }

            //Update the status list
            currentUser.setStatusList(statusList);
            userList[i] = currentUser;

        }

        Session["UserList"] = userList;

        //Save to DB
        saveFormToDB();

    }

    private void saveFormToDB()
    {

        int rowCount = (int)Session["RowCount"];
        int colCount = (int)Session["ColCount"];
        List<StatusUserForms> userList = (List<StatusUserForms>)Session["UserList"];
        List<UserForm> groupList = (List<UserForm>)Session["GroupList"];

        //Initiate OleDb
        OleDbConnection db = null;
        OleDbDataReader rdr = null;

        //Update the User List
        //updateUsers();

        try
        {

            //open connection
            db = new OleDbConnection();
            db.ConnectionString = DBConnectionString;
            db.Open();


            for (int i = 0; i < rowCount; i++)
            {

                //Get User details
                StatusUserForms currentUser = userList[i];
                List<string> statusList = currentUser.getStatusList();

                string techfitID = currentUser.getID();
                string sql = "DELETE FROM " + statusCheckTable + " WHERE TechfitID = '" + techfitID + "';";

                //Execute Command
                OleDbCommand cmd = new OleDbCommand(sql, db);
                rdr = cmd.ExecuteReader();

                for (int j = 0; j < colCount; j++)
                {

                    //This is a form id in this case
                    string groupID = getColumnsGroupID("" + (j + 1));

                    string val = statusList[j];

                    string currentCategory = (string)Session["Category"];

                    //Testing
                    //if (statusList[j] == "True")
                    //{

                    //    lblResult.Text += " <br /> " + currentUser.getID() + " " + statusList[j] + " J: " + j + "Form ID: " + groupID;
                    //}

                    if (statusList[j] == "True")
                    {
                        //Generate sql
                        sql = "INSERT INTO " + statusCheckTable + " VALUES ('" + techfitID + "'," + groupID + ", '" + currentCategory + "');";

                        //Execute Command
                        cmd = new OleDbCommand(sql, db);
                        rdr = cmd.ExecuteReader();
                    }
                    
                }


            }

            lblSaved.Visible = true;
            lblSaved.Text = "Changes Saved!";
            ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", "<script type=\"text/javascript\">setTimeout(\"document.getElementById('" + lblSaved.ClientID + "').style.display='none'\",2000)</script>");

        }

        catch (Exception exc)
        {
            lblResult.Text = "Error! Unable to save form Status Data..." + exc.Message + exc.StackTrace;
        }

        finally
        {
            db.Close();
        }


    }

}

internal class StatusUserForms
{
    private string name;
    private string ID;
    List<string> groupStatusList;

    public StatusUserForms(string n, string i)
    {
        this.name = n;
        this.ID = i;
        groupStatusList = new List<string>();
    }

    public string getName()
    {
        return this.name;
    }

    public string getID()
    {
        return this.ID;
    }
    public void addStatus(string status)
    {
        groupStatusList.Add(status);
    }

    public List<string> getStatusList()
    {
        return this.groupStatusList;
    }
    public void setStatusList(List<string> statusList)
    {
        this.groupStatusList = statusList;
    }
}

internal class UserForm
{
    private string ID;
    private string columnIndex;

    public UserForm(string i, string ci)
    {
        this.ID = i;
        this.columnIndex = ci;
    }

    public string getID()
    {
        return this.ID;
    }

    public string getColumnIndex()
    {
        return this.columnIndex;
    }

}