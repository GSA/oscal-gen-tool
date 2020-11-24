<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Remarks.aspx.cs" Inherits="OSCALGenerator.PageSSP.SystemImplementation.Remarks" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

        <table style="width:100%">
   <tr>
       <td>
          <asp:Label ID="OrgNameLabel" runat="server" Text="" ForeColor="#0067ac"></asp:Label>
      </td>
       <td>
          <asp:Label ID="SysNameLabel" runat="server" Text="" ForeColor="#0067ac"></asp:Label>
           
       </td>
       <td>
          <asp:Label ID="DocNameLabel" runat="server" Text=""  ForeColor="#0067ac"></asp:Label>
           
       </td>
     </tr>

     </table>
    <asp:Panel ID="AddRolePanel" runat="server"  Height="350px" style="background-color : whitesmoke;" Width="526px">
        <br />
           <h2 style="align-content: center"> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;System Implementation: Remarks</h2>
     
        <table>
         

            <tr>
                <td>
                    <asp:Label ID="Label2" runat="server" Text="Remarks    "></asp:Label>
                </td>
                <td>
                    <textarea id="RemarksTextarea" runat="server" cols="70" style="height: 146px"></textarea>
                </td>
            </tr>

        </table>
        <br />
          &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
       
         <asp:Button ID="ContinueButton" runat="server" Text="Save and Continue" OnClick="ContinueButton_Click"  />

        <br />
        <asp:Label ID="StatusLabel" runat="server" Text=""></asp:Label>

        </asp:Panel>
</asp:Content>
