<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="DatalotAdminLogin.aspx.cs" Inherits="DatalotAdminLogin" Title="TECHFIT Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" Runat="Server">
    <h1>
        Datalot - Login</h1>
    <span>Please enter your user name and password to gain
                    access to secured parts of this website.<br />
    </span>
    <br />
    <table style="width: 230px; text-align: center; border-right: gray thin solid; border-top: gray thin solid; border-left: gray thin solid; border-bottom: gray thin solid;">
        <tr style="font-size: 12pt">
            <td colspan="4" rowspan="1"
                style="text-align: center; background-color: #671514; color: #FFFFFF;">
                <span style="font-size: 10pt">Log in to Datalot</span></td>
        </tr>
        <tr style="font-size: 12pt">
            <td colspan="3" style="text-align: left;">
                <span style="font-size: 10pt">
                Username:</span>
            </td>
            <td colspan="1" style="text-align: left">
                <asp:TextBox ID="txtUsername" runat="server" Width="100px" Font-Size="Small"></asp:TextBox></td>
        </tr>
        <tr style="font-size: 12pt">
            <td colspan="3" style="height: 26px; text-align: left;">
                <span style="font-size: 10pt">
                Password:</span>
            </td>
            <td colspan="1" style="height: 26px; text-align: left">
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" Width="100px" Font-Size="Small"></asp:TextBox></td>
        </tr>
        <tr style="font-size: 12pt">
            <td colspan="3" rowspan="1" style="height: 26px; text-align: left">
            </td>
            <td colspan="1" rowspan="1" style="height: 26px; text-align: left">
                <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click"
                    Font-Size="Smaller" BorderColor="#BDBECD" BorderStyle="Solid"
                    BorderWidth="2px" /></td>
        </tr>
    </table>
    <br />
                <asp:Label ID="lblResult" runat="server" ForeColor="Red" Font-Size="Small" Font-Bold="True"></asp:Label>
    <br />
                <asp:Label ID="lblErr" runat="server" ForeColor="Brown" Font-Size="Small" Font-Bold="True"></asp:Label><br />

</asp:Content>

