<%@ Page Language="C#" MasterPageFile="MasterPage.master" Title="TECHFIT Home Page" AutoEventWireup="true" CodeFile="FormManager.aspx.cs" Inherits="FormManager" %>




<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" Runat="Server">

        <asp:Button ID="btnSelectSchool" class ="btn btn-warning btn-lg" runat="server" OnClick="btnSelectSchool_Click" style="margin-bottom: 0" Text="&lt;- Back" Width="100px" />

        <h1>Student Form Manager</h1>

	<asp:Label ID="lblSaved" runat="server" Text="CHANGES SAVED!.." Visible="False"></asp:Label>

        <br />
            <asp:Label ID="lblRowCnt" runat="server" Text="Row Count: "></asp:Label>
            <br />

             <asp:Label ID="lblSchoolName" runat="server" Text=" "></asp:Label>
            <br />

            <asp:Label ID="lblStudentCnt" runat="server" Text="Number of Students: "></asp:Label>
            <br />

            <asp:Label ID="lblOfficialTechfit" runat="server" Text="Number of Students: "></asp:Label>
            <br />
            <br />


            <table ID ="myTable" border ="1" runat ="server" style ="background-color: rgba(255,255,255, 0.9)">
        <tr>
            <td style="width: 159px"><strong> Student ID&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;         </strong>      <br />
            </td>
            <td style="width: 57px"> <strong> Student Name&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;        </strong>            </td>
            <td style="width: 134px"> <strong> Parent
                <br />
                Consent Form&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</strong> </td>
            <td style="width: 132px"><strong>Student
                <br />
                Assent<br />
                Form&nbsp;</strong> </td>
            <td style="width: 132px"><strong>PreProgram
                <br />

                Surveys&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</strong> </td>

            <td style="width: 162px"> <strong>PostProgram<br />Surveys</strong></td>



            <td style="width: 132px"><strong>Final
                <br />
                Survey3Mos
                </strong> </td>


        </tr>

    </table>

        <br />
        <p>
        <asp:Button ID="Button2" class ="btn btn-warning btn-lg" runat="server" OnClick="Button2_Click" Text="Save" Width="85px"  />
        </p>

    </asp:Content>
