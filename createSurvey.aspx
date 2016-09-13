<%@ Page Language="C#"  MasterPageFile="MasterPage.master" Title="Create Survey" AutoEventWireup="true" CodeFile="createSurvey.aspx.cs" Inherits="createSurvey" %>


	<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" Runat="Server">
    
    
        <asp:Label ID="lblTester" runat="server" Text=""></asp:Label>
    <div class ="col-sm-6 col-sm-offset-3">
        
        <p>
		<br />
            <asp:Button ID="btnBack" class ="btn btn-warning btn-lg" runat="server" onClientClick = "javascript:return confirm('Are you sure you want to go back? Your progress will be lost!');" onClick="backButton" Text="Back" Width="87px" />
        </p>

        <h1>Create Surveys</h1>

       
            <asp:Label ID="lblSurveyName" runat="server" Text="Survey Name:     "></asp:Label>

            <asp:TextBox ID="txtSurveyName" runat="server"></asp:TextBox>
             
            <br />

            
            
         <p>
		    <asp:Label ID="lblQCnt" runat="server" Text="Number of Questions: 0"></asp:Label></p>

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

                     <td><asp:CheckBox id="allowChcks" runat="server" Text="Allow more than 1 choice" AutoPostBack="False"/></td>
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
                
                
            
             <asp:Label ID="lblSurveyReview" runat="server" Text=""></asp:Label></p>

            

            <br /><br />
            <asp:Button ID="btnMCQ" class ="btn btn-warning btn-lg" runat="server" OnClick="addMCQ" Text="Add MCQ" Width="87px" />
            <asp:Button ID="btnSAQ" class ="btn btn-warning btn-lg" runat="server" OnClick="addSAQ" Text="Add SAQ" Width="87px" />

           
            
            <p style = "padding-left: 15cm">
            <asp:Button ID="btnSaveSurvey" class ="btn btn-warning btn-lg" runat="server" onClientClick = "javascript:return confirm('Submit Survey to Database?');" OnClick="submitSurvey" Text="Submit Survey" Width="120px" Height ="40px"/>
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
 