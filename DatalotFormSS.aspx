<%@ Page Language="C#"  MasterPageFile="MasterPage.master" Title="Select School" AutoEventWireup="true" CodeFile="DatalotFormSS.aspx.cs" Inherits="DatalotFormSS" %>


	<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" Runat="Server">
    
    
        <asp:Label ID="lblTester" runat="server" Text="TestLabel"></asp:Label>
                    <asp:Button ID="btnBack" class ="btn btn-warning btn-lg" runat="server" onClick="backButton" Text="<- Back" Width="87px" />

    <div class ="col-sm-6 col-sm-offset-3">
        
        <h1>Edit Form Status</h1>
	<br />
        
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
            
        </asp:DropDownList>
        <br />
        <br />
        
        Select Category:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:DropDownList ID="ddlCategoryList" runat="server" Height="20px" Width="170px" AutoPostBack ="true" OnSelectedIndexChanged="updateCategory">
            <asp:ListItem Text="Teachers" Value="Teachers" />
            <asp:ListItem Text="Students" Value="Students" />
            
        </asp:DropDownList>
        
        
           &nbsp;&nbsp;&nbsp;&nbsp; <asp:Button ID="btnSubmit" class ="btn btn-warning btn-lg" runat="server" OnClick="Button1_Click" Text="Next ->" Width="87px" />
        
        <p>
            <asp:Label ID="lblResult" runat="server" ForeColor="Red" Font-Size="Small" Font-Bold="True"></asp:Label>
            <asp:Label ID="lblErr" runat="server" ForeColor="Red" Font-Size="Small" Font-Bold="True"></asp:Label>
            <br />
            
        </p>
        <p>
        To create new manual forms ->
        <asp:Button ID="Button1" class ="btn btn-warning btn-lg" runat="server" OnClick="Button1_Click" Text="Create New Forms" Width="160px" Height ="45px" Enabled = "false" /></p>
        

    </div>
        
	</asp:Content>
 