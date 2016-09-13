<%@ Page Language="C#"  MasterPageFile="MasterPage.master" Title="Find Survey" AutoEventWireup="true" CodeFile="FindTakeSurvey.aspx.cs" Inherits="FindTakeSurvey" %>


	<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" Runat="Server">
    
    
        <asp:Label ID="lblTester" runat="server" Text="TestLabel"></asp:Label>
    <div class ="col-sm-6 col-sm-offset-3">
        
        <p>
		<br />
            <asp:Button ID="btnBack" class ="btn btn-warning btn-lg" runat="server" onClick="backButton" Text="Back" Width="87px" />
        </p>

        <h1>Find a Survey to take</h1>
	
        
        <p>
            <asp:Panel ID="p" runat="server" DefaultButton="btnSubmit">
            Survey Name:&nbsp;&nbsp;
             <asp:TextBox ID="txtSurveyName" runat="server"></asp:TextBox>&nbsp;&nbsp;
            <asp:Button ID="btnSubmit" class ="btn btn-warning btn-lg" runat="server" OnClick="searchButton" Text="Search" Width="87px" />
            </asp:Panel>
        </p>

        <asp:Label ID="lblResult" runat="server" ForeColor="Red" Font-Size="Small" Font-Bold="True"></asp:Label>
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

        <table ID ="myGroupTable" border ="1" runat ="server" style ="background-color: rgba(255,255,255, 0.9)">
               
                <tr>
                    
                    
                     <td style="width: 222px"><strong>Group Name</strong> </td>

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
 