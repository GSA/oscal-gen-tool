<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Citations.aspx.cs" Inherits="OSCALGenerator.Pages.BackMatter.Citations" %>
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

     <h3 style="align-content: center"> &nbsp;Citations&nbsp;</h3>
   
    <table>  <tr>  <td> 
    <asp:Panel ID="CitationPanel" runat="server"  Height="180px" style="margin-left: 0px;background-color : whitesmoke;" Width="437px">
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <br />
        <table>
            <tr>
                <td>
                    <asp:Label ID="CitIDLabel" runat="server" Text="Citation ID:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="CitationIDTextBox" runat="server" Width="371px"></asp:TextBox>
                </td>
            </tr>
           
            <tr>
                <td>
                    <asp:Label ID="TargetLabel" runat="server" Text="Target"></asp:Label>
                </td>
                <td>
                    <textarea id="TargetTextArea" runat="server" cols="60" rows="3"></textarea>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="TitleLabel" runat="server" Text="Title:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TitleTextBox" runat="server" Width="371px"></asp:TextBox>
                </td>
            </tr>
        </table>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
             <asp:Button ID="SaveCitButton" runat="server" Text="Save" OnClick="SaveCitButton_Click"  />
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="CancelCitButton" runat="server" Text="Cancel" OnClick="CancelCitButton_Click"  />
    </asp:Panel>
    <asp:Panel ID="GidviewPanel" runat="server"  Width="720px">

        <asp:GridView ID="CitationsGridView" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" Width="715px" >
           
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
         &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;  <asp:Button ID="AddCitationButton" runat="server" Text="Add Citation" OnClick="AddCitationButton_Click" />&nbsp;&nbsp;&nbsp
               
    </asp:Panel>
    </td>

     <td>
     </td>

      </tr>

    </table>
    <br />
    <br />

         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;  <asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" /> 

                 &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;   <asp:Button ID="ContinueButton" runat="server" Text="Continue" OnClick="ContinueButton_Click" />
 
    <asp:Label ID="StatusLabel" runat="server" Text=""></asp:Label>
</asp:Content>
