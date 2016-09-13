<%@ Page Language="C#" MasterPageFile="MasterPage.master" Title="TECHFIT Home Page" AutoEventWireup="true" CodeFile="DatalotFormManager.aspx.cs" Inherits="DatalotFormManager" %>




<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" Runat="Server">

        <asp:Button ID="btnSelectSchool" class ="btn btn-warning btn-lg" runat="server" OnClick="btnSelectSchool_Click" style="margin-bottom: 0" Text="<- Back" Width="100px" />
        
        <h2>Survey Status</h2>

	<asp:Label ID="lblSaved" runat="server" Text="CHANGES SAVED!.." Visible="False"></asp:Label>    
            
             <asp:Label ID="lblSchoolName" runat="server" Text=" "></asp:Label>
            <br />
             <br />

             <asp:Label ID="lblCategory" runat="server" Text=" "></asp:Label>
            <br />
            <br />
            
            <asp:Label ID="lblStudentCnt" runat="server" Text="Number of Students: "></asp:Label>
            <br />
            <br />
            <asp:Label ID="lblGroupLegend" runat="server" Text=""></asp:Label>
            <br />
            <br />

             <asp:Label ID="lblResult" runat="server"></asp:Label>
    <br />
            <asp:Label ID="lblErr" runat="server" ForeColor="Red" Font-Size="Small" Font-Bold="True"></asp:Label>

    <br />
            <table ID ="myTable" border ="1" runat ="server" style ="background-color: rgba(255,255,255, 0.9)">
        <tr>
            <td style="width: 159px"><strong> Techfit ID&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;         </strong>      <br />
            </td>
            <td style="width: 57px"> <strong> Teacher Name&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;        </strong>            </td>
            <td style="width: 134px"> <strong> 
                Group1&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</strong> </td>
            <td style="width: 132px"><strong>Group2&nbsp;</strong> </td>
            <td style="width: 132px"><strong>Group3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</strong> </td>

            


            <td style="width: 162px"> <strong>Group4</strong></td>
            
        </tr>
        
    </table>

        
    
    </asp:Content>
