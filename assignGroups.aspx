<%@ Page Language="C#"  MasterPageFile="MasterPage.master" Title="Assign Groups" AutoEventWireup="true" CodeFile="assignGroups.aspx.cs" Inherits="assignGroups" %>


	<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" Runat="Server">
    
    
        <asp:Label ID="lblTester" runat="server" Text="TestLabel"></asp:Label>
    <div class ="col-sm-6 col-sm-offset-3">
        
        <p>
		<br />
            <asp:Button ID="btnBack" class ="btn btn-warning btn-lg" runat="server" onClick="backButton" Text="Back" Width="87px" />
        </p>

        <h1>Create Survey Groups</h1>

        <p>
              <asp:Label ID="lblGroup" runat="server" Text = "Group Name: "></asp:Label>
             <asp:TextBox ID="txtGroup" runat="server"></asp:TextBox>&nbsp;&nbsp;
             <asp:Button ID="btnSubmitGroups" class ="btn btn-warning btn-lg" runat="server" onClientClick = "javascript:return confirm('Are you sure you want to Submit?');" OnClick="submitGroups" Text="Submit Group" Width="120px" Height ="40px" />
            <br /> <br />
            <asp:Label ID="lblGroupInfo" runat="server" Text = "This group currently has 0 surveys assigned to it.."></asp:Label>

        </p>

        <p>
            <asp:Label ID="lblActTime" runat="server" Text="     Activation Date:     "></asp:Label>

            <asp:DropDownList ID="ddlStartDay" runat="server" AutoPostBack ="true" OnSelectedIndexChanged="updateDate">
                            <asp:ListItem Text="--Day--" Value="" />
                            <asp:ListItem>1</asp:ListItem>
                            <asp:ListItem>2</asp:ListItem>
                            <asp:ListItem>3</asp:ListItem>
                            <asp:ListItem>4</asp:ListItem>
                            <asp:ListItem>5</asp:ListItem>
                            <asp:ListItem>6</asp:ListItem>
                            <asp:ListItem>7</asp:ListItem>
                            <asp:ListItem>8</asp:ListItem>
                            <asp:ListItem>9</asp:ListItem>
                            <asp:ListItem>10</asp:ListItem>
                            <asp:ListItem>11</asp:ListItem>
                            <asp:ListItem>12</asp:ListItem>
                            <asp:ListItem>13</asp:ListItem>
                            <asp:ListItem>14</asp:ListItem>
                            <asp:ListItem>15</asp:ListItem>
                            <asp:ListItem>16</asp:ListItem>
                            <asp:ListItem>17</asp:ListItem>
                            <asp:ListItem>18</asp:ListItem>
                            <asp:ListItem>19</asp:ListItem>
                            <asp:ListItem>20</asp:ListItem>
                            <asp:ListItem>21</asp:ListItem>
                            <asp:ListItem>22</asp:ListItem>
                            <asp:ListItem>23</asp:ListItem>
                            <asp:ListItem>24</asp:ListItem>
                            <asp:ListItem>25</asp:ListItem>
                            <asp:ListItem>26</asp:ListItem>
                            <asp:ListItem>27</asp:ListItem>
                            <asp:ListItem>28</asp:ListItem>
                            <asp:ListItem>29</asp:ListItem>
                            <asp:ListItem>30</asp:ListItem>
                            <asp:ListItem>31</asp:ListItem>
                            
                        </asp:DropDownList>

                 <asp:DropDownList ID="ddlStartMonth" runat="server" AutoPostBack ="true" OnSelectedIndexChanged="updateDate">
                            <asp:ListItem Text="--Month--" Value="" />
                            <asp:ListItem>1-Jan</asp:ListItem>
                            <asp:ListItem>2-Feb</asp:ListItem>
                            <asp:ListItem>3-Mar</asp:ListItem>
                            <asp:ListItem>4-Apr</asp:ListItem>
                            <asp:ListItem>5-May</asp:ListItem>
                            <asp:ListItem>6-June</asp:ListItem>
                            <asp:ListItem>7-July</asp:ListItem>
                            <asp:ListItem>8-Aug</asp:ListItem>
                            <asp:ListItem>9-Sept</asp:ListItem>
                            <asp:ListItem>10-Oct</asp:ListItem>
                            <asp:ListItem>11-Nov</asp:ListItem>
                            <asp:ListItem>12-Dec</asp:ListItem>
                            
                            
                        </asp:DropDownList>

                 <asp:DropDownList ID="ddlStartYear" runat="server" AutoPostBack ="true" OnSelectedIndexChanged="updateDate">
                            <asp:ListItem Text="--Year--" Value="" />
                            <asp:ListItem>2016</asp:ListItem>
                            <asp:ListItem>2017</asp:ListItem>
                            <asp:ListItem>2018</asp:ListItem>
                            <asp:ListItem>2019</asp:ListItem>
                            <asp:ListItem>2020</asp:ListItem>
                            <asp:ListItem>2021</asp:ListItem>
                            <asp:ListItem>2022</asp:ListItem>
                            <asp:ListItem>2023</asp:ListItem>
                            <asp:ListItem>2024</asp:ListItem>
                            <asp:ListItem>2025</asp:ListItem>
                            
                        </asp:DropDownList>

                <asp:DropDownList ID="ddlStartHour" runat="server" AutoPostBack ="true" OnSelectedIndexChanged="updateDate">
                            <asp:ListItem Text="--Hour--" Value="" />
                            <asp:ListItem>1</asp:ListItem>
                            <asp:ListItem>2</asp:ListItem>
                            <asp:ListItem>3</asp:ListItem>
                            <asp:ListItem>4</asp:ListItem>
                            <asp:ListItem>5</asp:ListItem>
                            <asp:ListItem>6</asp:ListItem>
                            <asp:ListItem>7</asp:ListItem>
                            <asp:ListItem>8</asp:ListItem>
                            <asp:ListItem>9</asp:ListItem>
                            <asp:ListItem>10</asp:ListItem>
                            <asp:ListItem>11</asp:ListItem>
                            <asp:ListItem>12</asp:ListItem>
                            <asp:ListItem>13</asp:ListItem>
                            <asp:ListItem>14</asp:ListItem>
                            <asp:ListItem>15</asp:ListItem>
                            <asp:ListItem>16</asp:ListItem>
                            <asp:ListItem>17</asp:ListItem>
                            <asp:ListItem>18</asp:ListItem>
                            <asp:ListItem>19</asp:ListItem>
                            <asp:ListItem>20</asp:ListItem>
                            <asp:ListItem>21</asp:ListItem>
                            <asp:ListItem>22</asp:ListItem>
                            <asp:ListItem>23</asp:ListItem>
                            <asp:ListItem>00</asp:ListItem>    
                        </asp:DropDownList>

                <asp:DropDownList ID="ddlStartMinutes" runat="server" AutoPostBack ="true" OnSelectedIndexChanged="updateDate">
                            <asp:ListItem Text="--Minutes--" Value="" />
                            <asp:ListItem Value = "0">00</asp:ListItem>
                            <asp:ListItem Value = "15">15</asp:ListItem>
                            <asp:ListItem Value = "30">30</asp:ListItem>
                            <asp:ListItem Value =" 45">45</asp:ListItem>
                        </asp:DropDownList>

               
                   
                
                
            <br />
            </p>

            <asp:Label ID="Label1" runat="server" Text="     End Date:            "></asp:Label>

            <asp:DropDownList ID="ddlEndDay" runat="server" AutoPostBack ="true" OnSelectedIndexChanged="updateDate">
                            <asp:ListItem Text="--Day--" Value="" />
                            <asp:ListItem>1</asp:ListItem>
                            <asp:ListItem>2</asp:ListItem>
                            <asp:ListItem>3</asp:ListItem>
                            <asp:ListItem>4</asp:ListItem>
                            <asp:ListItem>5</asp:ListItem>
                            <asp:ListItem>6</asp:ListItem>
                            <asp:ListItem>7</asp:ListItem>
                            <asp:ListItem>8</asp:ListItem>
                            <asp:ListItem>9</asp:ListItem>
                            <asp:ListItem>10</asp:ListItem>
                            <asp:ListItem>11</asp:ListItem>
                            <asp:ListItem>12</asp:ListItem>
                            <asp:ListItem>13</asp:ListItem>
                            <asp:ListItem>14</asp:ListItem>
                            <asp:ListItem>15</asp:ListItem>
                            <asp:ListItem>16</asp:ListItem>
                            <asp:ListItem>17</asp:ListItem>
                            <asp:ListItem>18</asp:ListItem>
                            <asp:ListItem>19</asp:ListItem>
                            <asp:ListItem>20</asp:ListItem>
                            <asp:ListItem>21</asp:ListItem>
                            <asp:ListItem>22</asp:ListItem>
                            <asp:ListItem>23</asp:ListItem>
                            <asp:ListItem>24</asp:ListItem>
                            <asp:ListItem>25</asp:ListItem>
                            <asp:ListItem>26</asp:ListItem>
                            <asp:ListItem>27</asp:ListItem>
                            <asp:ListItem>28</asp:ListItem>
                            <asp:ListItem>29</asp:ListItem>
                            <asp:ListItem>30</asp:ListItem>
                            <asp:ListItem>31</asp:ListItem>
                            
                        </asp:DropDownList>

                 <asp:DropDownList ID="ddlEndMonth" runat="server" AutoPostBack ="true" OnSelectedIndexChanged="updateDate">
                            <asp:ListItem Text="--Month--" Value="" />
                            <asp:ListItem>1-Jan</asp:ListItem>
                            <asp:ListItem>2-Feb</asp:ListItem>
                            <asp:ListItem>3-Mar</asp:ListItem>
                            <asp:ListItem>4-Apr</asp:ListItem>
                            <asp:ListItem>5-May</asp:ListItem>
                            <asp:ListItem>6-June</asp:ListItem>
                            <asp:ListItem>7-July</asp:ListItem>
                            <asp:ListItem>8-Aug</asp:ListItem>
                            <asp:ListItem>9-Sept</asp:ListItem>
                            <asp:ListItem>10-Oct</asp:ListItem>
                            <asp:ListItem>11-Nov</asp:ListItem>
                            <asp:ListItem>12-Dec</asp:ListItem>
                            
                            
                        </asp:DropDownList>

                 <asp:DropDownList ID="ddlEndYear" runat="server" AutoPostBack ="true" OnSelectedIndexChanged="updateDate">
                            <asp:ListItem Text="--Year--" Value="" />
                            <asp:ListItem>2016</asp:ListItem>
                            <asp:ListItem>2017</asp:ListItem>
                            <asp:ListItem>2018</asp:ListItem>
                            <asp:ListItem>2019</asp:ListItem>
                            <asp:ListItem>2020</asp:ListItem>
                            <asp:ListItem>2021</asp:ListItem>
                            <asp:ListItem>2022</asp:ListItem>
                            <asp:ListItem>2023</asp:ListItem>
                            <asp:ListItem>2024</asp:ListItem>
                            <asp:ListItem>2025</asp:ListItem>
                            
                        </asp:DropDownList>
                       
                   <asp:DropDownList ID="ddlEndHour" runat="server" AutoPostBack ="true" OnSelectedIndexChanged="updateDate">
                            <asp:ListItem Text="--Hour--" Value="" />
                            <asp:ListItem>1</asp:ListItem>
                            <asp:ListItem>2</asp:ListItem>
                            <asp:ListItem>3</asp:ListItem>
                            <asp:ListItem>4</asp:ListItem>
                            <asp:ListItem>5</asp:ListItem>
                            <asp:ListItem>6</asp:ListItem>
                            <asp:ListItem>7</asp:ListItem>
                            <asp:ListItem>8</asp:ListItem>
                            <asp:ListItem>9</asp:ListItem>
                            <asp:ListItem>10</asp:ListItem>
                            <asp:ListItem>11</asp:ListItem>
                            <asp:ListItem>12</asp:ListItem>   
                            <asp:ListItem>13</asp:ListItem>
                            <asp:ListItem>14</asp:ListItem>
                            <asp:ListItem>15</asp:ListItem>
                            <asp:ListItem>16</asp:ListItem>
                            <asp:ListItem>17</asp:ListItem>
                            <asp:ListItem>18</asp:ListItem>
                            <asp:ListItem>19</asp:ListItem>
                            <asp:ListItem>20</asp:ListItem>
                            <asp:ListItem>21</asp:ListItem>
                            <asp:ListItem>22</asp:ListItem>
                            <asp:ListItem>23</asp:ListItem>
                            <asp:ListItem>00</asp:ListItem>  
                        </asp:DropDownList>

                <asp:DropDownList ID="ddlEndMinutes" runat="server" AutoPostBack ="true" OnSelectedIndexChanged="updateDate">
                            <asp:ListItem Text="--Minutes--" Value="" />
                            <asp:ListItem Value = "0">00</asp:ListItem>
                            <asp:ListItem Value = "15">15</asp:ListItem>
                            <asp:ListItem Value = "30">30</asp:ListItem>
                            <asp:ListItem Value ="45">45</asp:ListItem>
                        </asp:DropDownList>

                

            </p>

        Select Category:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:DropDownList ID="ddlCategoryList" runat="server" Height="20px" Width="170px" AutoPostBack ="true" OnSelectedIndexChanged="updateCategory">
            <asp:ListItem Text="Teachers" Value="Teachers" />
            <asp:ListItem Text="Students" Value="Students" />
            <asp:ListItem Text = "Teachers and Students" Value="Teachers and Students" />
            <asp:ListItem Text = "Public" Value="Public" />
            <asp:ListItem Text = "Anonymous" Value = "Anonymous" />
            
        </asp:DropDownList>

        <br />
        <br />

        <asp:CheckBox id="addToStatusCheck" runat="server" Text="Check this box to add this group for status checking." AutoPostBack="False"/>

        <br />
        <br />

         <asp:Label ID="lblResult" runat="server" ForeColor="Red" Font-Size="Small" Font-Bold="True"></asp:Label>
        <br />
        <br />
        <asp:Button ID="btnClearGroup" class ="btn btn-warning btn-lg" runat="server" onClick="clearGroup" Text="Clear Assigned Surveys" Width="155px" />


        <h3>Find Surveys</h3>
	
        
        <p>
            <asp:Panel ID="p" runat="server" DefaultButton="btnSubmit">
            Survey Name:&nbsp;&nbsp;
             <asp:TextBox ID="txtSurveyName" runat="server"></asp:TextBox>&nbsp;&nbsp;
            <asp:Button ID="btnSubmit" class ="btn btn-warning btn-lg" runat="server" OnClick="searchButton" Text="Search" Width="87px" />
            </asp:Panel>
        </p>

         <asp:Label ID="lblResult2" runat="server" ForeColor="Red" Font-Size="Small" Font-Bold="True"></asp:Label>
        <asp:Label ID="lblErr" runat="server" ForeColor="Red" Font-Size="Small" Font-Bold="True"></asp:Label>

        <br />
        <br />



         <table ID ="myTable" border ="1" runat ="server" style ="background-color: rgba(255,255,255, 0.9)">
               
                <tr>
                    
                    
                     <td style="width: 222px"><strong>Survey Name</strong> </td>

                    <td style="width: 162px"> <strong></strong></td>

                </tr>
        
            </table>

        <br />
        <br />
        <br />


         <asp:Label ID="lblDetails" runat="server" ForeColor="Black" Font-Size="Small" Font-Bold="True" Text="Survey Details:"></asp:Label>
        <br />
        <br />
        <table ID ="surveyTable" border ="1" runat ="server" style ="background-color: rgba(255,255,255, 0.9)">
               
              
        
            </table>

    </div>
        
	</asp:Content>
 