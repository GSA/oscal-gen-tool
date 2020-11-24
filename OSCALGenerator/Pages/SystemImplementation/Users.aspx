<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="OSCALGenerator.Pages.SystemImplementation.Users" %>
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

     <h2 style="align-content: center"> Users</h2>
<asp:Panel ID="AddUserPanel" runat="server"  Height="350px" style="margin-left: 10px;background-color : whitesmoke;" Width="450px">
        <br />
        <table>
            <tr>
                <td>
                    <asp:Label ID="UserIDLabel" runat="server" Text="User ID:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="UserIDTextBox" runat="server" Width="350px"></asp:TextBox>
                </td>
            </tr>
            
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
                    <asp:Label ID="TitleLabel" runat="server" Text="Title:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TitleTextBox" runat="server" Width="350px"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="NSLabel" runat="server" Text="NS:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="NSTextBox" runat="server" Width="350px"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="ExternalLabel" runat="server" Text="External:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="ExternalTextBox" runat="server" Width="350px"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="AccessLabel" runat="server" Text="Access:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="AccessTextBox" runat="server" Width="350px"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="SensitivityLabel2" runat="server" Text="Sensitivity Level:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="SensitivityTextBox" runat="server" Width="350px"></asp:TextBox>
                </td>
            </tr>

             <tr>
                <td>
                    <asp:Label ID="PrivilegeLabel" runat="server" Text="Authorized Privilege:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="PrivilegeTextBox" runat="server" Width="350px"></asp:TextBox>
                </td>
            </tr>

             <tr>
                <td>
                    <asp:Label ID="FunctionLabel" runat="server" Text="Function Performed:"></asp:Label>
                </td>
                <td>
                     <textarea runat="server" id="FunctionPerformedTextArea" cols="60" rows="3"></textarea>
                
                </td>
            </tr>
           
             

        </table>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
             <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
    </asp:Panel>
    <asp:Panel ID="FormviewPanel" runat="server" style="margin-left: 10px;background-color : whitesmoke;" BackColor="WhiteSmoke" Width="480px">

        <asp:FormView ID="FormView1" runat="server" AllowPaging="True" CellPadding="4" ForeColor="#333333" BackColor="#3366FF" Width="484px" OnPageIndexChanging="FormView1_PageIndexChanging">
            <EditRowStyle BackColor="#2461BF" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <ItemTemplate>
                User ID: <%# Eval("ID") %> 
                <br />
                Role ID:  <%# Eval("RoleID") %> 
                <br />
                Title: <%# Eval("Title") %> 
                <br />
                NS:  <%# Eval("NS") %> 
                <br />
                External:  <%# Eval("External") %>  
                <br />
                Access:&nbsp; <%# Eval("Access") %> 
                <br />
                Sensitivity Level:  <%# Eval("SensitivityLevel") %> 
                <br /> 
                Authorized Privilege: <%# Eval("AuthorizedPrivilegeName") %> 
                <br />
                Function Performed:  <%# Eval("FunctionPerformed") %>
                <br />
            </ItemTemplate>
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#EFF3FB" />
        </asp:FormView>

         &nbsp;&nbsp;&nbsp;<asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" />  &nbsp;&nbsp;&nbsp; <asp:Button ID="AddUserButton" runat="server" Text="Add a User" OnClick="AddUserButton_Click" /> &nbsp;&nbsp;&nbsp;
          <asp:Button ID="ContinueButton" runat="server" Text="Continue" OnClick="ContinueButton_Click" />
    </asp:Panel>

</asp:Content>
