<%@ Page Language="C#"  MasterPageFile="MasterPage.master" Title="Datalot" AutoEventWireup="true" CodeFile="datalot.aspx.cs" Inherits="Datalot" %>


	<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" Runat="Server">
    
    
        <asp:Label ID="lblTester" runat="server" Text="TestLabel"></asp:Label>
    <div class ="col-sm-6 col-sm-offset-3">

            <asp:Button ID="btnLogout" class ="btn btn-warning btn-lg" runat="server" OnClick="logout" Text="Logout" Width="100px" />
        
        <h1>Welcome to Datalot!</h1>
        
        <p>
		<br />
            <asp:Button ID="btnCreateSurvey" class ="btn btn-warning btn-lg" runat="server" OnClick="createSurvey" Text="Create Surveys" Width="150px" Height ="40px" />
            <asp:Button ID="btnOpenSurvey" class ="btn btn-warning btn-lg" runat="server" OnClick="openSurvey" Text="Find/Edit Surveys" Width="150px" Height ="40px" />
            <asp:Button ID="btnAssignGroups" class ="btn btn-warning btn-lg" runat="server" OnClick="assignGroups" Text="Manage Survey Groups" Width="150px" Height ="40px" />
            <asp:Button ID="btnTakeSurvey" class ="btn btn-warning btn-lg" runat="server" OnClick="takeSurvey" Text="Take a Mock Survey" Width="150px" Height ="40px" Visible ="false" />
            <br />
            <br />
            <asp:Button ID="btnSurveyStatus" class ="btn btn-warning btn-lg" runat="server" OnClick="surveyStatus" Text="Check Survey Status" Width="150px" Height ="40px" />
            <asp:Button ID="btnFormManager" class ="btn btn-warning btn-lg" runat="server" OnClick="formStatus" Text="Manage Forms" Width="150px" Height ="40px" />
        </p>

        <asp:Label ID="lblResult" runat="server" ForeColor="Red" Font-Size="Small" Font-Bold="True"></asp:Label>
            
         <p>
		<br />
            
        </p>

    </div>
        
	</asp:Content>
 