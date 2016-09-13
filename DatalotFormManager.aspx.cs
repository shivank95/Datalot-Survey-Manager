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

public partial class DatalotFormManager : System.Web.UI.Page
{
    
    String currentSchoolName = "Carver Edisto Middle School";
    String currentCategory = "Teachers";

    String tableName = "";
    string studentTable = "spirit.TECHFITStudentApps2015";

    string teacherTable = "spirit.TECHFITTeacherApps2015";

    string statusCheckTable = "spirit.DatalotSurveyStatusNew";
    string columnCountTable = "spirit.DatalotSurveyGroup";

    //String DBConnectionString = "Provider=SQLOLEDB;Server=techwebdev.ecn.purdue.edu;Database=dev.techfit;uid=dev.techfit.usr01;pwd=ECNdev.techfitpa55;";

    String DBConnectionString = Configuration.ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["TeacherList"] = new List<Teacher>();
            Session["UserList"] = new List<StatusUser>();
            Session["RowCount"] = 0;
            Session["ColCount"] = 0;
            Session["SchoolName"] = "Carver Edisto Middle School";
            Session["Category"] = "Teachers";
            Session["GroupList"] = new List<UserGroup>();
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

            List<Teacher> myTeacherList = (List<Teacher>)Session["TeacherList"];
            myTeacherList.Clear();

            //New Code
            List<StatusUser> userList = (List<StatusUser>)Session["UserList"];
            userList.Clear();
            

            while (myReader.Read() && index < rowCnt)
            {
                ID = myReader["ID"].ToString();
                firstName = myReader["FirstName"].ToString();
                lastName = myReader["LastName"].ToString();

                int cCnt = 1;

                Teacher tempTeacher = new Teacher(ID, (firstName + " " + lastName), false, false, false, false);
                myTeacherList.Add(tempTeacher);

                //New Code
                StatusUser tempUser = new StatusUser((firstName + " " + lastName), ID);
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

            Session["TeacherList"] = myTeacherList;

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
        lblCategory.Text = "Checking Survey statuses for: " + currentCategory;

        //string[] groups = { "first day of participation in summer professional development (PD) program", "Last day of summer PD program", "Last day of the 5th week of the after - school program,", "1 week after the finishing date of the after-school program implementation" };

        //lblGroupDetails.Text = " <br /> Group 1: " + groups[0] + " <br /> Group 2: " + groups[1] + " <br /> Group 3: " + groups[2] + " <br /> Group 4: " + groups[3];   



    }
    public string getColumnsGroupID(string index)
    {
        List<UserGroup> tmpGroup = (List<UserGroup>)Session["GroupList"];

        for (int i = 0; i < tmpGroup.Count; i++)
        {

            UserGroup tmp = tmpGroup[i];
            
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

        Label lblTechfitID = new Label();
        lblTechfitID.ID = "staticTechfitIDLabel";
        lblTechfitID.Text = "TechfitID";
        cell.Controls.Add(lblTechfitID);
        cell.Align = "Center";

        HtmlTableCell cell2 = new HtmlTableCell();
        cell2.ID = "staticCell2";

        Label lblName = new Label();
        lblName.ID = "staticNameLabel";
        lblName.Text = "Name";
        cell2.Controls.Add(lblName);
        cell2.Align = "Center";

        row.Cells.Add(cell);
        row.Cells.Add(cell2);

        lblGroupLegend.Text = "Groups: <br />";
        for (int i =0; i < colCnt; i++)
        {

            HtmlTableCell cell3 = new HtmlTableCell();
            cell3.ID = "staticCell" + (i +3);
            cell3.Width = "45px";
            cell3.Align = "Center";

            Label lblGroup = new Label();
            lblGroup.ID = "staticGroup" + (i + 3) + "Label";

            string groupID = getColumnsGroupID("" + (i+1));

            lblGroup.Text = " <p> " + (i + 1) + "   </p>";

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
            txtID.ID = "txt2" + i.ToString();
            txtName.Style["text-align"] = "center";
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
                //pcf.Checked = students.ElementAt(i).parentConsent;
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
        List<StatusUser> userList = (List<StatusUser>)Session["UserList"];
        List<UserGroup> groupList = (List<UserGroup>)Session["GroupList"];


        //Update the User List
        updateUsers();

        for (int i = 0; i < rowCount; i++)
        {

            //Get User details
            StatusUser currentUser = userList[i];
            List<string> statusList = currentUser.getStatusList();

            for (int j = 0; j < colCount; j++)
            {
                HtmlTableRow currentRow = (HtmlTableRow)myTable.FindControl("row" + i);

                //Third Cell
                HtmlTableCell cell3 = (HtmlTableCell)currentRow.FindControl(i.ToString() + "cell" + (j+3));
                CheckBox ckbPCF = (CheckBox)cell3.Controls[0] as CheckBox;

                ckbPCF.Checked = Boolean.Parse(statusList[j]);

                ckbPCF.Enabled = false;

            }
        }

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

            string sql = "SELECT Name FROM spirit.DatalotSurveyGroup WHERE GroupID = " + groupID + ";";

            //Execute Command
            OleDbCommand cmd = new OleDbCommand(sql, db);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                return (string)rdr["Name"].ToString();
            }

            return null;

        }

        catch (Exception exc)
        {
            lblResult.Text = "Error! Unable to get group names..." + exc.Message + exc.StackTrace;
        }

        finally
        {
            db.Close();
        }

        return null;

    }

    public void updateUsers()
    {
        List<StatusUser> userList = (List<StatusUser>)Session["UserList"];
        List<UserGroup> groupList = (List<UserGroup>)Session["GroupList"];

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
                StatusUser tempUser = userList[i];
                string currentTechfitID = tempUser.getID();

                for (int j = 0; j < groupList.Count; j++)
                {
                    string currentGroupID = groupList[j].getID();

                    string sql = "SELECT * FROM spirit.DatalotSurveyStatusNew WHERE TechfitID = " + currentTechfitID + " AND GroupID = " + currentGroupID + ";";

                    //Execute Command
                    OleDbCommand cmd = new OleDbCommand(sql, db);
                    rdr = cmd.ExecuteReader();
                    int flag = 0;
                    while (rdr.Read())
                    {
                        string techfitID = (string)rdr["TechfitID"].ToString();
                        string groupID = (string)rdr["GroupID"].ToString();
                        string takeCount = (string)rdr["TakeCount"].ToString();
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
            List<UserGroup> groupList = (List<UserGroup>)Session["GroupList"];

            string sql = "SELECT DISTINCT GroupID FROM " + columnCountTable + " WHERE StatusCheck = 1 AND (Category LIKE '%" + currentCategory + "%' OR Category = 'Public' OR Category = 'Anonymous');";

            OleDbCommand cmd = new OleDbCommand(sql, db);
            myReader = cmd.ExecuteReader();

            while (myReader.Read())
            {
                colCnt++;
                string ID = (string)myReader["GroupID"].ToString();
                string columnIndex = "" + colCnt;

                UserGroup tempGroup = new UserGroup(ID, columnIndex);
                groupList.Add(tempGroup);
            }
            //Testing
            if (colCnt > 0)
            {

                lblResult.Text = "Number of Group(s) found: " + colCnt;
            }
            else
            {
                lblResult.Text = "No Groups found..";
            }

            Session["ColCount"] = colCnt;
            Session["GroupList"] = groupList;
            return colCnt;
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
        Response.Redirect("DatalotSelectSchool.aspx");
    }
}

internal class StatusUser
{
    private string name;
    private string ID;
    List<string> groupStatusList;

    public StatusUser(string n, string i)
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
}

internal class UserGroup
{
    private string ID;
    private string columnIndex;

    public UserGroup(string i, string ci)
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


internal class Teacher {

    private string name;
    private string ID;
    private bool g1;
    private bool g2;
    private bool g3;
    private bool g4;

    public Teacher(string ID, string name, bool a, bool b, bool c, bool d)
    {
        this.ID = ID;
        this.name = name;
        this.g1 = a;
        this.g2 = b;
        this.g3 = c;
        this.g4 = d;
    }

    public string getID ()
    {
        return this.ID;
    }

    public string getName()
    {
        return this.name;
    }

    public bool getG1()
    {
        return this.g1;
    }

    public bool getG2()
    {
        return this.g2;
    }

    public bool getG3()
    {
        return this.g3;
    }

    public bool getG4()
    {
        return this.g4;
    }

    public void setGroups(bool v1, bool v2, bool v3, bool v4)
    {
        this.g1 = v1;
        this.g2 = v2;
        this.g3 = v3;
        this.g4 = v4;
    }


}
