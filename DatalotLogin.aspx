<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="DatalotLogin.aspx.cs" Inherits="DatalotLogin" Title="TECHFIT Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" Runat="Server">
<h1>Take Surveys</h1>

        <br />
        <asp:Panel ID="pnlMessage" runat="server" GroupingText="Messages From App"
            Height="100px" Style="z-index: 101; left: 0px;
            position: relative; top: 0px" Width="690px" BorderColor="Red"
            BorderStyle="Solid" BorderWidth="1px" Font-Bold="True" ForeColor="Red"
            BackColor="White" Visible="False">
			<asp:Label ID="lblMessage" runat="server" Font-Bold="False"
				Font-Size="Small" Font-Color="Red" Text="Message from App"></asp:Label>
        </asp:Panel>
        <asp:Label ID="lblResult" runat="server" ForeColor="Red" Font-Size="Small" Font-Bold="True"></asp:Label>
        <asp:Panel ID="Pnlcontact" runat="server" GroupingText="Login"
            Height="200px" Font-Bold Font-Size="Medium">

            <table style="width: 85%">
                <tr>
                    <td style="height: 25px; width: 259px;">
                        <asp:Label ID="lblTeacherID" runat="server" Font-Bold="False"
                            Font-Size="Small" Text="*Your 10-digit TECHFIT ID:"></asp:Label>
                    </td>
                    <td style="height: 25px; width: 456px">
                        <asp:TextBox ID="txtTeacherID" runat="server"
                            Width="205px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="width: 259px; height: 25px;">
                        <asp:Label ID="lblSchoolName" runat="server" Font-Bold="False"
                            Font-Size="Small" Text="*Name of your School:"></asp:Label>
                    </td>
                    <td style="height: 25px; width: 456px">

                        <asp:DropDownList ID="txtSchoolName" runat="server">
                            <asp:ListItem Value="0">Please Select</asp:ListItem>
                            <asp:ListItem>Bremen Public School</asp:ListItem>
                            <asp:ListItem>Carver Edisto Middle School</asp:ListItem>
                            <asp:ListItem>Hilton Head Island Middle School</asp:ListItem>
                            <asp:ListItem>Hughes Academy of Science and Technology</asp:ListItem>
                            <asp:ListItem>Lafayette Sunnyside Intermediate School</asp:ListItem>
                            <asp:ListItem>Robert E. Howard Middle School</asp:ListItem>
                            <asp:ListItem>Winamac Middle School</asp:ListItem>
                            <asp:ListItem>Woodlan Jr/Sr High School</asp:ListItem>
					    </asp:DropDownList>

                    </td>
                </tr>
                <tr>
                    <td style="height: 30px" colspan="3" align="center">
                     <br />
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" Width="103px" OnClick="btnSubmit_Click" />
                        <br />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlStudentInfo" runat="server" GroupingText="Student Information"
            Height="1250px" Style="z-index: 101; left: 0px;
            position: relative; top: 0px" Width="690px" BorderColor="Blue"
            BorderStyle="Solid" BorderWidth="1px" Font-Bold="True" ForeColor="Blue"
            BackColor="White" Visible="False">
			<asp:Label ID="lblDetails" runat="server" Font-Bold="False"
				Font-Size="Small" Font-Color="Blue" Text=""></asp:Label>
        </asp:Panel>

</asp:Content>
