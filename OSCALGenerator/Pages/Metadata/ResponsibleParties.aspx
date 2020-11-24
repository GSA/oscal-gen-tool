<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ResponsibleParties.aspx.cs" Inherits="OSCALGenerator.Pages.Metadata.ResponsibleParties" %>
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
     <h2 style="align-content: center"> Metadata: Responsible Parties</h2>
<asp:Panel ID="Panel1" runat="server"  Height="120px" style="margin-left: 0px;background-color : whitesmoke;" Width="420px">
        <br />
        <table>
            <tr>
                <td>
                    <asp:Label ID="RoleIDLabel" runat="server" Text="Role ID:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="RoleIDTextBox" runat="server" Width="350px"></asp:TextBox>
                </td>
            </tr>
           
            <tr>
                <td>
                    <asp:Label ID="PartyIdLabel" runat="server" Text="Enter Party Id:    "></asp:Label>
                </td>
                <td>
                    <textarea id="TextArea1" runat="server" cols="50" rows="1"></textarea>
                </td>
            </tr>
        </table>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
        &nbsp;
        <asp:Button ID="Delete" runat="server" Text="Delete" />
    </asp:Panel>
    <asp:Panel ID="GidviewPanel" runat="server"  Width="420px">

        <asp:GridView ID="RolesGridView" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" >
           
            <AlternatingRowStyle BackColor="White" />
            <EditRowStyle BackColor="#2461BF" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#EFF3FB" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#F5F7FB" />
            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
            <SortedDescendingCellStyle BackColor="#E9EBEF" />
            <SortedDescendingHeaderStyle BackColor="#4870BE" />
           
        </asp:GridView>
         &nbsp;&nbsp;&nbsp  <asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" /> &nbsp;&nbsp;&nbsp  <asp:Button ID="AddRoleButton" runat="server" Text="Add Role" OnClick="AddRoleButton_Click" />&nbsp;&nbsp;&nbsp;<asp:Button ID="EditButton" runat="server" Text="Edit Button" />
        &nbsp;&nbsp;&nbsp;&nbsp;
           <asp:Button ID="ContinueButton" runat="server" Text="Continue" OnClick="ContinueButton_Click" />
    
        <br />
        &nbsp;&nbsp;
        <asp:Label ID="StatusLabel" runat="server" Visible="False"></asp:Label>
    
    </asp:Panel>
</asp:Content>
