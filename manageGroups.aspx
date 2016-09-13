<%@ Page Language="C#"  MasterPageFile="MasterPage.master" Title="Datalot" AutoEventWireup="true" CodeFile="manageGroups.aspx.cs" Inherits="manageGroups" %>


	<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" Runat="Server">
    
    
        <asp:Label ID="lblTester" runat="server" Text="TestLabel"></asp:Label>
    <div class ="col-sm-6 col-sm-offset-3">

        <p>
		<br />
            <asp:Button ID="btnBack" class ="btn btn-warning btn-lg" runat="server" onClick="backButton" Text="Back" Width="87px" />
        </p>
        
        <h1>Manage Survey Groups</h1>
        
        <p>
		<br />
            
            <asp:Button ID="btnAssignGroups" class ="btn btn-warning btn-lg" runat="server" OnClick="assignGroups" Text="Create Survey Groups" Width="150px" Height ="40px" />
            <asp:Button ID="btnOpenGroup" class ="btn btn-warning btn-lg" runat="server" OnClick="openGroups" Text="Find/Delete Groups" Width="150px" Height ="40px"/>
           
        </p>

        <asp:Label ID="lblResult" runat="server" ForeColor="Red" Font-Size="Small" Font-Bold="True"></asp:Label>
            
         <p>
		<br />
            
        </p>

    </div>
        
	</asp:Content>
 