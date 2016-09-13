<%@ Page Language="C#"  MasterPageFile="MasterPage.master" Title="Take Survey" AutoEventWireup="true" CodeFile="takeSurveyPublic.aspx.cs" Inherits="takeSurveyPublic" %>

	<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" Runat="Server">
    
    
        <asp:Label ID="lblTester" runat="server" Text="TestLabel"></asp:Label>
    <div class ="col-sm-6 col-sm-offset-3">
        
         <p>
		<br />
            <asp:Button ID="btnBack" class ="btn btn-warning btn-lg" runat="server" onClientClick = "javascript:return confirm('Are you sure you want to go back? Your progress will be lost!');" onClick="backButton" Text="Back" Width="87px" />
        </p>
        <br />

          <h1><asp:Label ID="lblGroupName" runat="server" ForeColor="Black" Font-Size="Large" Font-Bold="True"></asp:Label></h1>


        <h1><asp:Label ID="lblSurveyName" runat="server" ForeColor="Black" Font-Size="Large" Font-Bold="True"></asp:Label></h1>
	<br />

        <p>
		<br />
            <asp:Label ID="lblResult" runat="server" ForeColor="Red" Font-Size="Small" Font-Bold="True"></asp:Label>
            
            <asp:Label ID="lblErr" runat="server" ForeColor="Red" Font-Size="Small" Font-Bold="True"></asp:Label>
        </p>

         <%--TABLE FOR SAQ --%>
        <table ID ="myTable" frame ="box" runat ="server" style ="background-color: rgba(255,255,255, 0.9)">
               
                <tr>
                    <td>
                        <asp:Label ID="lblQuestion" runat="server" ForeColor="Black" Font-Size="Small" Font-Bold="True"></asp:Label>
                        <br />
                        <br />
                        
                    </td>

                    </tr>

                <tr>
                    
                    <td><asp:TextBox ID="txtResponse" runat="server" TextMode = "MultiLine" Width = "675px" Height = "75px"></asp:TextBox>

                        <br />
                        <br />
                    </td>
                    
                </tr>

            <tr>
                 
                 <td>   <asp:Button ID="btnLastQuestionSAQ" runat="server" Text="<-Back" OnClick="lastQuestion" />
                     <asp:Button ID="btnSaveAndNext" runat="server" Text="Save and Next ->" OnClick="saveSAQAndNextQuestion" /> 
                     <br /></td>

                     


            </tr>
        
            </table>

         <%--TABLE FOR MCQ --%>
        <table ID ="myTable2" frame ="box" runat = "server" style ="background-color: rgba(255,255,255, 0.9)">
             
                <tr>
                    <td>
                        <asp:Label ID="lblMCQQuest" runat="server" ForeColor="Black" Font-Size="Small" Width = "675px" Font-Bold="True"></asp:Label>
                        <br />
                        <br />
                    </td>

                </tr>

                <tr>
                    <td>
                    <asp:radioButtonList ID="radioAnswers" runat="server" AutoPostBack="true">
                        
                    </asp:radioButtonList>
                    </td>
                </tr>

            <tr>

                <td>
                    <asp:CheckBoxList ID="ckbAnswers" runat="server" AutoPostBack="true">

                    </asp:CheckBoxList>
                </td>

            </tr>

                <tr>
                     <td> <asp:Button ID="btnLastQuestionMCQ" runat="server" Text="<-Back" OnClick="lastQuestion" />
                         <asp:Button ID="btnSaveandNextMCQ" runat="server" Text="Save and Next ->" OnClick="saveMCQAndNextQuestion" /></td>
                    

                </tr>
        
        </table>
        
                     <asp:Label ID="lblSurveyReview" runat="server" Text=""></asp:Label></p>


        

    </div>
        
	</asp:Content>
 