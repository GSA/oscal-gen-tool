<%@ Page Title="" Language="C#" MasterPageFile="~/SiteSAP.Master" AutoEventWireup="true" CodeBehind="Title.aspx.cs" Inherits="OSCALGenerator.PagesSAP.Metadata.Title" %>
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
    <h2 style="align-content: center"> Metadata Title  </h2>
    <table style="width:60%">
   <tr>
       <td>
    <asp:Label ID="DocumentTilteLabel" runat="server" Text="System Assessment Plan Title"></asp:Label>
    </td>
       <td>
          
           <textarea id="TitleTextArea" runat="server" cols="50" rows="3"></textarea>

       </td>
     </tr>

        <tr>
        <td>
     <asp:Label ID="VersionDateLabel" runat="server" Text="Published On">
     </asp:Label>
            </td>
           <td>  <asp:TextBox ID="VersionDateTextBox" runat="server" Width="557px"></asp:TextBox>
        <asp:Button ID="VersionDateButton" runat="server" Text="GetDate" OnClick="VersionDateButton_Click"/>
               </td>
    </tr>
    
    <tr>
      <td> </td>  
        <td>
           <asp:Calendar ID="VersionDateCalendar" runat="server" Visible="false" OnSelectionChanged="VersionDateCalendar_SelectionChanged"></asp:Calendar>
      
       </td>
       </tr>

        <tr>
        <td>
     <asp:Label ID="Label1" runat="server" Text="Last Modified Date">
     </asp:Label>
            </td>
           <td>  <asp:TextBox ID="LastModifiedTextBox" runat="server" Width="557px"></asp:TextBox>
        <asp:Button ID="LastModifiedButton" runat="server" Text="GetDate" OnClick="LastModifiedButton_Click"/>
               </td>
    </tr>
        <tr>
      <td> </td>  
        <td>
           <asp:Calendar ID="LastModifiedCalendar" runat="server" Visible="false" OnSelectionChanged="LastModifiedCalendar_SelectionChanged"></asp:Calendar>
      
       </td>
       </tr>

  
  
    <tr>
        <td>
     <asp:Label ID="VersionLabel" runat="server" Text="Version">
     </asp:Label>
            </td>
        <td>
    <asp:TextBox ID="VersionTextBox" runat="server" Width="800px"></asp:TextBox>
        </td>

     </tr>
    <tr>
        <td>
     <asp:Label ID="OSCALLabel1" runat="server" Text="OSCAL Version">
     </asp:Label>
        </td>
        <td>
    <asp:TextBox ID="OSCALVersionTextBox" runat="server" Width="800px"></asp:TextBox>

     </td>
     </tr>
     <tr>
                <td>
                    <asp:Label ID="Label2" runat="server" Text="Remarks    "></asp:Label>
                </td>
                <td>
                    <textarea id="RemarksTextarea" runat="server" style="height: 142px; width: 798px;"></textarea>
                </td>
            </tr>
        </table>
    
   <br />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
    <asp:Button ID="SaveButton" runat="server" Text="Save and Continue" OnClick="SaveButton_Click" />
    <br />

    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:Label ID="StatusLabel" runat="server" Text=""></asp:Label>

</asp:Content>
