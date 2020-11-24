<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AuthorizationBoundary.aspx.cs" Inherits="OSCALGenerator.Pages.SystemCharacteristics.AuthorizationBoundary" %>
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



  <table>
    <tr style="margin-left: 0px;background-color : whitesmoke; width:100%">
 
    <td>
       <h3 style="align-content: center"> Authorization Boundary</h3>
    <asp:Panel ID="Panel1" runat="server"  Height="132px" style="margin-left: 0px;" Width="310px">
     <br />
     <table>
   <tr>
       <td style="height: 20px">
    <asp:Label ID="DescriptionLabel" runat="server" Text="Description"></asp:Label>
    </td>
       <td style="height: 20px">
          
           <textarea  runat="server"  id="AuthDescTextArea" rows="2" style="width: 185px"></textarea>

       </td>
     </tr>
    
    
        <tr>
        <td>
     <asp:Label ID="LinkLabel" runat="server" Text="Link href">
     </asp:Label>
       </td>
      <td>
          <textarea  runat="server" id="AuthLinkTextArea" rows="2" style="width: 185px"></textarea>
     </td>
    </tr>
        </table >
        <br />

</asp:Panel>
          
        </td>
         <td>
        

               <h3 style="align-content: center"> Network Architecture</h3>
    <asp:Panel ID="Panel2" runat="server"  Height="124px" style="margin-left: 0px;" Width="310px">
     <br />
         <table>
   <tr>
       <td style="height: 20px">
    <asp:Label ID="Label1" runat="server" Text="Description"></asp:Label>
    </td>
       <td style="height: 20px">
          
           <textarea runat="server" id="NetDescTextArea" rows="2" style="width: 185px"></textarea>

       </td>
     </tr>
     
    
        <tr>
        <td>
     <asp:Label ID="Label3" runat="server" Text="Link href">
     </asp:Label>
       </td>
      <td>
          <textarea  runat="server" id="NetLinkTextArea" rows="2" style="width: 185px"></textarea>
     </td>
    </tr>
  
   
        </table>

    
   <br />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <br />
</asp:Panel>
                
      </td>
      <td>

  


              <h3 style="align-content: center"> Data Flow</h3>
    <asp:Panel ID="Panel3" runat="server"  Height="135px" style="margin-left: 0px;" Width="310px">
     <br />
         <table>
   <tr>
       <td style="height: 20px">
    <asp:Label ID="Label4" runat="server" Text="Description"></asp:Label>
    </td>
       <td style="height: 20px">
          
           <textarea runat ="server" id="DataDescTextArea" rows="2" style="width: 185px"></textarea>

       </td>
     </tr>
     
    
        <tr>
        <td>
     <asp:Label ID="Label6" runat="server" Text="Link href">
     </asp:Label>
       </td>
      <td>
          <textarea  runat="server" id="DataLinkTextArea" rows="2" style="width: 185px"></textarea>
     </td>
    </tr>
  
   
        </table>

    
   <br />

</asp:Panel>

          </td>  
            
 
     </tr>
   </table>


      <br />
 <asp:Label ID="StatusLabel" runat="server" Text=""></asp:Label>
         &nbsp<asp:Button ID="Button1" runat="server" Text="Back" OnClick="BackButton_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Button ID="Button2" runat="server" Text="Save" OnClick="SaveButton_Click" />
        &nbsp;&nbsp; &nbsp
            <asp:Button ID="Button3" runat="server" Text="Continue" OnClick="ContinueButton_Click" />     
         

     
   
    
  
    

</asp:Content>
