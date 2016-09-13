<%@ Page Language="C#"  MasterPageFile="MasterPage.master" Title="Take Survey" AutoEventWireup="true" CodeFile="takeSurveyTeachers.aspx.cs" Inherits="takeSurveyTeachers" %>

	<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" Runat="Server">
    
    
        <asp:Label ID="lblTester" runat="server" Text="TestLabel"></asp:Label>
    <div class ="col-sm-6 col-sm-offset-3">
        
          <h1><asp:Label ID="lblGroupName" runat="server" ForeColor="Black" Font-Size="Large" Font-Bold="True"></asp:Label></h1>


        <h1><asp:Label ID="lblSurveyName" runat="server" ForeColor="Black" Font-Size="Large" Font-Bold="True"></asp:Label></h1>

        <p>
		<br />
            <asp:Label ID="lblResult" runat="server" ForeColor="Red" Font-Size="Small" Font-Bold="True"></asp:Label>
            
            <asp:Label ID="lblErr" runat="server" ForeColor="Red" Font-Size="Small" Font-Bold="True"></asp:Label>
        </p>

        <table ID ="myTable" border ="0" frame ="box" runat ="server" style ="background-color: rgba(255,255,255, 0.9)">

            </table>

        <br />

        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnSaveAndNext" class ="btn btn-warning btn-lg" runat="server" OnClick="submitSurvey" Text="Save and Next" Width="120px" Height="40px"  />

        

    </div>
        
	</asp:Content>
 