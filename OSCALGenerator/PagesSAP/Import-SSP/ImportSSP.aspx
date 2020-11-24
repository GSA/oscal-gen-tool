<%@ Page Title="" Language="C#" MasterPageFile="~/SiteSAP.Master" AutoEventWireup="true" CodeBehind="ImportSSP.aspx.cs" Inherits="OSCALGenerator.PagesSAP.Import_SSP.ImportSSP" %>
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
   
    <div id="MainDiv"  runat="server" class="container-fluid">
        <div class ="container">
           <div class="row"  >
                <h2 style="align-content: center"> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Import-SSP</h2>
               <div class="col-md-6">
     
    <asp:Panel ID="AddLocationPanel" runat="server"  Height="230px" style="background-color : whitesmoke;" Width="433px">
        <br />
        <table>
            
            

            

            <tr>
                <td>
                    <asp:Label ID="Label5" runat="server" Text=" Href   "></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="HrefTextBox" runat="server" Width ="310px"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label2" runat="server" Text="Remarks    "></asp:Label>
                </td>
                <td>
                    <textarea id="RoleRemarksTextarea" runat="server" cols="50" style="height: 142px"></textarea>
                </td>
            </tr>

        </table>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="BackButton" runat="server" OnClick="BackButton_Click" Text="Back" />
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="SaveButton" runat="server" OnClick="SaveButton_Click" Text="Save and Continue" />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;
        <br />
        </asp:Panel>
                   </div>
               </div>
            </div>
        </div>
    
    <asp:Label ID="StatusLabel" runat="server" Text=""></asp:Label>
</asp:Content>
