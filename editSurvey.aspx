<%@ Page Language="C#"  MasterPageFile="MasterPage.master" Title="Edit Survey" AutoEventWireup="true" CodeFile="editSurvey.aspx.cs" Inherits="editSurvey" %>


	<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" Runat="Server">
    
    
        <asp:Label ID="lblTester" runat="server" Text=""></asp:Label>
    <div class ="col-sm-6 col-sm-offset-3">
        
        <p>
		<br />
            <asp:Button ID="btnBack" class ="btn btn-warning btn-lg" runat="server" onClientClick = "javascript:return confirm('Are you sure you want to go back? Your progress will be lost!');" onClick="backButton" Text="Back" Width="87px" />
        </p>

        <h1>Edit Surveys</h1>

            <asp:Label ID="lblBTW" runat="server" Text="Note: Editing Surveys will lead to the deletion of all previous responses for that Survey!"></asp:Label>

        <br />
        <br />

            <asp:Label ID="lblSurveyName" runat="server" Text="Survey Name:     "></asp:Label>

            <asp:TextBox ID="txtSurveyName" runat="server"></asp:TextBox>
             
            <br />
            <p>
            

            <asp:Label ID="lblParticipants" runat="server" Text="     Participants:     "></asp:Label>

            <asp:DropDownList ID="ddlParticipants" runat="server" AutoPostBack ="True" OnSelectedIndexChanged ="getPartGrp">
                            <asp:ListItem Text="--Participants--" Value="" />
                            <asp:ListItem Text = "All" Value="All">All</asp:ListItem>
                            <asp:ListItem Text="Students" Value="Students">Students</asp:ListItem>
                            <asp:ListItem Text="Teachers" Value="Teachers">Teachers</asp:ListItem>
                        </asp:DropDownList>

            <br />
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
                            
                            <asp:ListItem Value ="1">1-Jan</asp:ListItem>
                            <asp:ListItem Value ="2">2-Feb</asp:ListItem>
                            <asp:ListItem Value ="3">3-Mar</asp:ListItem>
                            <asp:ListItem Value ="4">4-Apr</asp:ListItem>
                            <asp:ListItem Value ="5">5-May</asp:ListItem>
                            <asp:ListItem Value ="6">6-June</asp:ListItem>
                            <asp:ListItem Value ="7">7-July</asp:ListItem>
                            <asp:ListItem Value ="8">8-Aug</asp:ListItem>
                            <asp:ListItem Value ="9">9-Sept</asp:ListItem>
                            <asp:ListItem Value ="10">10-Oct</asp:ListItem>
                            <asp:ListItem Value ="11">11-Nov</asp:ListItem>
                            <asp:ListItem Value ="12">12-Dec</asp:ListItem>
                            
                            
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
                            <asp:ListItem Value ="1">1-Jan</asp:ListItem>
                            <asp:ListItem Value ="2">2-Feb</asp:ListItem>
                            <asp:ListItem Value ="3">3-Mar</asp:ListItem>
                            <asp:ListItem Value ="4">4-Apr</asp:ListItem>
                            <asp:ListItem Value ="5">5-May</asp:ListItem>
                            <asp:ListItem Value ="6">6-June</asp:ListItem>
                            <asp:ListItem Value ="7">7-July</asp:ListItem>
                            <asp:ListItem Value ="8">8-Aug</asp:ListItem>
                            <asp:ListItem Value ="9">9-Sept</asp:ListItem>
                            <asp:ListItem Value ="10">10-Oct</asp:ListItem>
                            <asp:ListItem Value ="11">11-Nov</asp:ListItem>
                            <asp:ListItem Value ="12">12-Dec</asp:ListItem>
                            
                            
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

            <br />
            </p>

            

            
            
         <p>
		    <asp:Label ID="lblQCnt" runat="server" Text="Number of Questions: 0"></asp:Label></p>

        
		    <asp:Label ID="lblEditQuestion" runat="server" Text="Edit Question: "></asp:Label>

             <asp:DropDownList ID="ddlEditQuestions" runat="server" AutoPostBack ="true" Width ="350px" OnSelectedIndexChanged="updateQuestions"> 
                 <asp:ListItem Text="--Select a Question--" Value="" />
                  </asp:DropDownList>

            <p> <asp:Button ID="btnUndo" class ="btn btn-warning btn-lg" runat="server" OnClick="undo" Text="Undo" Width="87px" /></p>
            <br />
            <asp:Label ID="lblResult" runat="server" ForeColor="Red" Font-Size="Small" Font-Bold="True"></asp:Label>
            <asp:Label ID="lblErr" runat="server" ForeColor="Red" Font-Size="Small" Font-Bold="True"></asp:Label>
            <p>
           


             <%--TABLE FOR SAQ --%>
            
             <table ID ="myTable" frame ="box" runat ="server" style ="background-color: rgba(255,255,255, 0.9)">
               
                <tr>
                    <td style="width: 60px"><strong> Question: </strong>
                    </td>
                    
                    <td><asp:TextBox ID="txtQuest" runat="server" TextMode = "MultiLine" Width = "400px"></asp:TextBox></td>

                    <td><asp:Button ID="Button1" runat="server" Text="Add Question" OnClick="saveQuestion" /></td>
                </tr>
        
            </table>

            <%--TABLE FOR SAQ EDIT--%>
            
             <table ID ="mySAQEditTable" frame ="box" runat ="server" style ="background-color: rgba(255,255,255, 0.9)">
               
                <tr>
                    <td style="width: 60px"><strong> Question: </strong>
                    </td>
                    
                    <td><asp:TextBox ID="txtSAQEdit" runat="server" TextMode = "MultiLine" Width = "400px"></asp:TextBox></td>

                    <td><asp:Button ID="btnEditSAQ" runat="server" Text="Edit Question" OnClick="saveEditQuestion" /></td>
                    <td><asp:Button ID="btnDeleteSAQ" runat="server" Text="Delete Question" OnClick="deleteQuestion" /></td>
                </tr>
        
            </table>

            
            <%--TABLE FOR MULTIPLE CHOICE--%>

             <table ID ="myTable2" frame ="box" runat ="server" style ="background-color: rgba(255,255,255, 0.9)">
               
                <tr>
                    <td style="width: 60px"><strong> Question: </strong>
                    </td>
                    
                    <td><asp:TextBox ID="txtMCQQuest" runat="server" TextMode = "MultiLine" Width = "400px"></asp:TextBox></td>

                    <td><asp:Button ID="btnMCQSave" runat="server" Text="Add Question" OnClick="saveMCQ" /></td>
                </tr>
                 <tr>
                     <td><br /></td>
                 </tr>
                 <tr>
                     
                     <td style="width: 60px"><strong> Choice 1: </strong>
                    </td>
                    
                    <td><asp:TextBox ID="txtCh1" runat="server" TextMode = "MultiLine" Width = "400px" Height = "20px"></asp:TextBox></td>
                 </tr>

                 <tr>
                     <td style="width: 60px"><strong> Choice 2: </strong>
                    </td>
                    
                    <td><asp:TextBox ID="txtCh2" runat="server" TextMode = "MultiLine" Width = "400px" Height = "20px"></asp:TextBox></td>

                     <td><asp:CheckBox ID="allowChcks" runat="server" Text="Allow more than 1 choice" AutoPostBack="False"/></td>
                 </tr>

                 <tr>
                     <td style="width: 60px"><strong> Choice 3: </strong>
                    </td>
                    
                    <td><asp:TextBox ID="txtCh3" runat="server" TextMode = "MultiLine" Width = "400px" Height = "20px"></asp:TextBox></td>
                     
                     <td><asp:DropDownList ID="listOptions" runat="server" AutoPostBack ="True" OnSelectedIndexChanged ="updateOptions">
                            <asp:ListItem Text="--Common Options--" Value="" />
                            <asp:ListItem>Agree - Disagree</asp:ListItem>
                            <asp:ListItem>1 - 5</asp:ListItem>
                            <asp:ListItem>Yes, No</asp:ListItem>
                            <asp:ListItem>Strongly Agree</asp:ListItem>
                            <asp:ListItem>Not at all</asp:ListItem>
                            <asp:ListItem>Item 6</asp:ListItem>
                            
                        </asp:DropDownList></td>
                     
                 </tr>

                 <tr>
                     <td style="width: 60px"><strong> Choice 4: </strong>
                    </td>
                    
                    <td><asp:TextBox ID="txtCh4" runat="server" TextMode = "MultiLine" Width = "400px" Height = "20px"></asp:TextBox></td>

                     <td><asp:Button ID="btnClearTextBoxes" runat="server" Text="Clear Options" OnClientClick = "false" OnClick="clearTxtBoxes" /></td>

                 </tr>

                 <tr>
                     <td style="width: 60px"><strong> Choice 5: </strong>
                    </td>
                    
                    <td><asp:TextBox ID="txtCh5" runat="server" TextMode = "MultiLine" Width = "400px" Height = "20px"></asp:TextBox></td>

                      <td><asp:Button ID="txtHideTable" runat="server" Text="Hide Table" OnClientClick = "false" OnClick="hideMCQTable" /></td>
                     
                 </tr>
        
            </table>


               <%--TABLE FOR MULTIPLE CHOICE EDIT--%>

             <table ID ="myMCQEditTable" frame ="box" runat ="server" style ="background-color: rgba(255,255,255, 0.9)">
               
                <tr>
                    <td style="width: 60px"><strong> Question: </strong>
                    </td>
                    
                    <td><asp:TextBox ID="txtMCQEditQuest" runat="server" TextMode = "MultiLine" Width = "400px"></asp:TextBox></td>

                    <td><asp:Button ID="btnEditMCQ" runat="server" Text="Edit Question" OnClick="saveEditQuestion" /></td>
                </tr>
                 <tr>
                     <td><br /></td>
                 </tr>
                 <tr>
                     
                     <td style="width: 60px"><strong> Choice 1: </strong>
                    </td>
                    
                    <td><asp:TextBox ID="txtChEdit1" runat="server" TextMode = "MultiLine" Width = "400px" Height = "20px"></asp:TextBox></td>
                 </tr>

                 <tr>
                     <td style="width: 60px"><strong> Choice 2: </strong>
                    </td>
                    
                    <td><asp:TextBox ID="txtChEdit2" runat="server" TextMode = "MultiLine" Width = "400px" Height = "20px"></asp:TextBox></td>

                     <td><asp:CheckBox ID="ckbAllowMultEdit" runat="server" Text="Allow more than 1 choice" AutoPostBack="False"/></td>
                 </tr>

                 <tr>
                     <td style="width: 60px"><strong> Choice 3: </strong>
                    </td>
                    
                    <td><asp:TextBox ID="txtChEdit3" runat="server" TextMode = "MultiLine" Width = "400px" Height = "20px"></asp:TextBox></td>
                     
                     
                     
                 </tr>

                 <tr>
                     <td style="width: 60px"><strong> Choice 4: </strong>
                    </td>
                    
                    <td><asp:TextBox ID="txtChEdit4" runat="server" TextMode = "MultiLine" Width = "400px" Height = "20px"></asp:TextBox></td>

                    

                 </tr>

                 <tr>
                     <td style="width: 60px"><strong> Choice 5: </strong>
                    </td>
                    
                    <td><asp:TextBox ID="txtChEdit5" runat="server" TextMode = "MultiLine" Width = "400px" Height = "20px"></asp:TextBox></td>

                      <td><asp:Button ID="btnDeleteMCQEdit" runat="server" Text="Delete Question" OnClientClick = "false" OnClick="deleteQuestion" /></td>
                     
                 </tr>
        
            </table>
                
                
            
             <asp:Label ID="lblSurveyReview" runat="server" Text=""></asp:Label></p>

            

            <br /><br />
            <asp:Button ID="btnMCQ" class ="btn btn-warning btn-lg" runat="server" OnClick="addMCQ" Text="Add MCQ" Width="87px" />
            <asp:Button ID="btnSAQ" class ="btn btn-warning btn-lg" runat="server" OnClick="addSAQ" Text="Add SAQ" Width="87px" />

           
            
            <p style = "padding-left: 15cm">
            <asp:Button ID="btnSaveSurvey" class ="btn btn-warning btn-lg" runat="server" onClientClick = "javascript:return confirm('Submit Survey to Database?');" OnClick="submitSurvey" Text="Submit Survey" Width="120px"/>
            </p>
	<%--<br />
        Select School:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:DropDownList ID="schoolList" runat="server" Height="20px" Width="204px">
            <asp:ListItem Text="Bremen Public School" Value="0" />
            <asp:ListItem Text="Carver Edisto Middle School" Value="0" />
            <asp:ListItem Text = "H.E. McCracken Middle School" Value="0" />
            <asp:ListItem Text = "Hilton Head Island Middle School" Value="0" />
            <asp:ListItem Text = "Hughes Academy of Science and Technology" Value = "0" />
            <asp:ListItem Text = "Ladys Island Middle School" Value="0" />
            <asp:ListItem Text = "Lafayette Sunnyside Intermediate School" Value ="0" />
            <asp:ListItem Text = "Northview Middle School" Value ="0" />
            <asp:ListItem Text = "Robert E. Howard Middle School" Value ="0" />
            <asp:ListItem Text = "Winamac Middle School" Value ="0" />
            <asp:ListItem Text = "Woodlan Jr/Sr High School" Value ="0" />
            
        </asp:DropDownList>--%>
        
        

    </div>
        
	</asp:Content>
 