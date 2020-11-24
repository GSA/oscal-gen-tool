<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Resources.aspx.cs" Inherits="OSCALGenerator.Pages.BackMatter.Resources" %>
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
     <h4 style="align-content: center"> Resources</h4>
    <asp:Panel ID="ResourcePanel" runat="server"  Height="146px" style="margin-left: 0px;background-color : whitesmoke;" Width="420px">
        <br />
        <table>
            <tr>
                <td>
                    <asp:Label ID="Label1" runat="server" Text="Resource ID:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="ResourceIDTextBox" runat="server" Width="320px"></asp:TextBox>
                </td>
            </tr>
           
            <tr>
                <td>
                    <asp:Label ID="Label2" runat="server" Text="Description"></asp:Label>
                </td>
                <td>
                    <textarea id="DescTextarea" runat="server" cols="50" rows="1"></textarea>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label3" runat="server" Text="Filename"></asp:Label>
                </td>
                <td>
                    <asp:FileUpload ID="FileUpload1" runat="server" />
                </td>
            </tr>
        </table>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<br /> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;
             <asp:Button ID="SaveResourceButton" runat="server" Text="Save" OnClick="SaveResourceButton_Click" />
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="CancelResourceButton" runat="server" Text="Cancel" OnClick="CancelResourceButton_Click"  />
    </asp:Panel>
    <asp:Panel ID="GridviewPanel" runat="server"  Width="420px" Height="186px">

        <asp:GridView ID="ResourceGridView" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" Width="377px" AllowCustomPaging="True" AllowPaging="True" AllowSorting="True">
           
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
         &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;  <asp:Button ID="AddResourceButton" runat="server" Text="Add Resource" OnClick="AddResourceButton_Click"   />&nbsp;&nbsp;&nbsp
            <br />   
    </asp:Panel>  
    <br />
    <br />
     &nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" /> 

                  &nbsp;&nbsp;&nbsp;  <asp:Button ID="ContinueButton" runat="server" Text="Continue" OnClick="ContinueButton_Click" />   &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;
 
    <asp:Label ID="StatusLabel" runat="server" Text=""></asp:Label>
</asp:Content>

