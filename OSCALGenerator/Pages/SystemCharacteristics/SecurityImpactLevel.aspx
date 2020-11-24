<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SecurityImpactLevel.aspx.cs" Inherits="OSCALGenerator.Pages.SystemCharacteristics.SecurityImpactLevel" %>
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
    <div class="container-fluid">
        <div class ="container">
  <div class="row" style="margin-left: 0px;background-color : whitesmoke; width:80%" >
      <div class="col-md-4">
       <h3 style="align-content: center"> Security Impact Level</h3>
    <asp:Panel ID="Panel1" runat="server"  Height="135px" style="margin-left: 0px;" Width="250px">
     <br />
     <table>
   <tr>
       <td>
    <asp:Label ID="ConfidentialityLabel" runat="server" Text="Confidentiality"></asp:Label>
    </td>
       <td>
          
           <asp:DropDownList ID="ConfidentialityDropDownList" runat="server">
               
               <asp:ListItem>fips-199-low</asp:ListItem>
               <asp:ListItem>fips-199-moderate</asp:ListItem>
               <asp:ListItem>fips-199-high</asp:ListItem>
               
           </asp:DropDownList>

       </td>
     </tr>
     <tr>
         <td>
     <asp:Label ID="IntegrityLabel" runat="server" Text="Integrity">
     </asp:Label>
       </td>
         <td>
           
             <asp:DropDownList ID="IntegrityDropDownList" runat="server">

                 
                 <asp:ListItem>fips-199-low</asp:ListItem>
                 <asp:ListItem>fips-199-moderate</asp:ListItem>
                 <asp:ListItem>fips-199-high</asp:ListItem>

                 
             </asp:DropDownList>
      </td>       
      </tr>
    
        <tr>
        <td>
     <asp:Label ID="AvailabilityLabel" runat="server" Text="Availability">
     </asp:Label>
       </td>
      <td>
          <asp:DropDownList ID="AvailabilityDropDownList" runat="server">
              
              <asp:ListItem>fips-199-low</asp:ListItem>
              <asp:ListItem>fips-199-moderate</asp:ListItem>
              <asp:ListItem>fips-199-high</asp:ListItem>
              
          </asp:DropDownList>
     </td>
    </tr>
  
   
        </table>
    
   <br />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <br />
</asp:Panel>
          </div>
          <div class="col-md-4">

               <h3 style="align-content: center"> Security eAuthentification</h3>
    <asp:Panel ID="Panel2" runat="server"  Height="150px" style="margin-left: 0px;" Width="258px">
     <br />
     <table>
   <tr>
       <td>
    <asp:Label ID="Label1" runat="server" Text="Security-Auth-Ial"></asp:Label>
    </td>
       <td>
          
           <asp:DropDownList ID="SecurityAuthIalDropDownList" runat="server">
               
               <asp:ListItem>low</asp:ListItem>
               <asp:ListItem>moderate</asp:ListItem>
               <asp:ListItem>high</asp:ListItem>
               
           </asp:DropDownList>

       </td>
     </tr>
     <tr>
         <td>
     <asp:Label ID="Label2" runat="server" Text="Security-Auth-Aal">
     </asp:Label>
       </td>
         <td>
           
             <asp:DropDownList ID="SecurityAuthAalDropDownList" runat="server">

                 
                 <asp:ListItem>low</asp:ListItem>
                 <asp:ListItem>moderate</asp:ListItem>
                 <asp:ListItem>high</asp:ListItem>

                 
             </asp:DropDownList>
      </td>       
      </tr>
    
        <tr>
        <td>
     <asp:Label ID="Label3" runat="server" Text="Security-Auth-Fal">
     </asp:Label>
       </td>
      <td>
          <asp:DropDownList ID="SecurityAuthFalDropDownList" runat="server">
              
              <asp:ListItem>low</asp:ListItem>
              <asp:ListItem>moderate</asp:ListItem>
              <asp:ListItem>high</asp:ListItem>
              
          </asp:DropDownList>
     </td>
    </tr>

         <tr>
        <td>
     <asp:Label ID="Label4" runat="server" Text="Security-Eauth-Level">
     </asp:Label>
       </td>
      <td>
          <asp:DropDownList ID="SecurityEauthLevelDropDownList" runat="server">
              
              <asp:ListItem>low</asp:ListItem>
              <asp:ListItem>moderate</asp:ListItem>
              <asp:ListItem>high</asp:ListItem>
              
          </asp:DropDownList>
     </td>
    </tr>
       
   
        </table>
    
   <br />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <br />
</asp:Panel>
                
      </div>

       <div class="col-md-4">


              <h3 style="align-content: center"> System Status </h3>
    <asp:Panel ID="Panel3" runat="server"  Height="165px" style="margin-left: 0px;" Width="327px">
     <br />
     <table>
   <tr>
       <td>
    <asp:Label ID="SystemStatusLabel" runat="server" Text="System State"></asp:Label>
    </td>
       <td>
          
          
           <asp:DropDownList ID="StateDropDownList" runat="server"  Width="142px" Height="18px">
               <asp:ListItem Value="operational">Operational</asp:ListItem>
               <asp:ListItem Value="under-development">Under Development</asp:ListItem>
               <asp:ListItem Value="under-major-modification">Under Major Modification</asp:ListItem>
               <asp:ListItem Value="disposition">Disposition</asp:ListItem>
               <asp:ListItem Value="other">Other</asp:ListItem>
           </asp:DropDownList>
       </td>
     </tr>
     <tr>
         <td>
     <asp:Label ID="DeploymentModelTypeLabel" runat="server" Text="Deployment Model Type">
     </asp:Label>
       </td>
         <td>
           
             <asp:TextBox ID="DeploymentModelTypeTextBox" runat="server" Width="140px"></asp:TextBox>
         </td>       
      </tr>
    
        <tr>
        <td>
     <asp:Label ID="ServiceModelTypeLabel" runat="server" Text="Service Model Type">
     </asp:Label>
       </td>
      <td>
          <asp:TextBox ID="ServiceModelTypeTextBox" runat="server" Width="140px"></asp:TextBox>
     </td>
    </tr>
     <tr>
        <td>
     <asp:Label ID="Label5" runat="server" Text="remarks">
     </asp:Label>
       </td>
      <td>
          <textarea id="RemarkTextArea" runat="server" style="width: 142px; height: 46px"></textarea>
     </td>
    </tr>
  
   
        </table>
    
   <br />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <br />
</asp:Panel>


        </div>
           
     </div>
            <asp:Label ID="StatusLabel" runat="server" Text=""></asp:Label>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp<asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
        &nbsp;&nbsp; &nbsp
            <asp:Button ID="ContinueButton" runat="server" Text="Continue" OnClick="ContinueButton_Click" />

        </div>
       </div>
</asp:Content>
