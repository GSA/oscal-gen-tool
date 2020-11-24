<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Roles.aspx.cs" Inherits="OSCALGenerator.Pages.Metadata.Roles" %>
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
    <asp:SqlDataSource ID="SqlDataSource1" runat="server"></asp:SqlDataSource>
      <h2 style="align-content: center"> Metadata: Roles</h2>
    <asp:Panel ID="AddRolePanel" runat="server"  Height="224px" style="margin-left: 0px;background-color : whitesmoke;" Width="380px">
        <br />
        <table>
            <tr>
                <td>
                    <asp:Label ID="RoleIDLabel" runat="server" Text="Role ID:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="RoleIDTextBox" runat="server" Width="195px"></asp:TextBox>
                    <asp:DropDownList ID="RoleDropDownList" runat="server" Width="100px" AutoPostBack="True" OnSelectedIndexChanged="RoleDropDownList_SelectedIndexChanged"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="TitleLabel" runat="server" Text="Title:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TitleTextBox" runat="server" Width="298px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="DescriptionLabel" runat="server" Text="Description:    "></asp:Label>
                </td>
                <td>
                    <textarea id="TextArea1" runat="server" cols="50" rows="5"></textarea>
                </td>
            </tr>
        </table>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
        &nbsp;&nbsp;&nbsp;&nbsp; <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" />
    </asp:Panel>
    <asp:Panel ID="GridviewPanel" runat="server" BackColor="WhiteSmoke">

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
        &nbsp;&nbsp;&nbsp<asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" />&nbsp;&nbsp;&nbsp; <asp:Button ID="AddRoleButton" runat="server" Text="Add Role" OnClick="AddRoleButton_Click" />   &nbsp;&nbsp;&nbsp;<asp:Button ID="EditButton" runat="server" Text="Edit Role" OnClick="EditButton_Click" />
        &nbsp;&nbsp;
         <asp:Button ID="ContinueButton" runat="server" Text="Continue" OnClick="ContinueButton_Click" />

    </asp:Panel>


</asp:Content>
