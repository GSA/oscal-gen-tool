<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SystemIdentification.aspx.cs" Inherits="OSCALGenerator.Pages.SystemCharacteristics.SystemIdentification" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

      <table style="width:100%">
   <tr>
       <td>
          <asp:Label ID="CorpNameLabel" runat="server" Text="" ForeColor="#0067ac"></asp:Label>
      </td>
       <td>
          <asp:Label ID="SytemInfoNameLabel" runat="server" Text="" ForeColor="#0067ac"></asp:Label>
           
       </td>
       <td>
          <asp:Label ID="DocNameLabel" runat="server" Text=""  ForeColor="#0067ac"></asp:Label>
           
       </td>
     </tr>

     </table>

     <div id="MainDiv"  runat="server" class="container-fluid">
        <div class ="container">
           <div class="row"  >
                 <h2 style="align-content: center"> System Identifications  </h2>
               <div class="col-md-6">


    
      <asp:Panel ID="Panel1" runat="server"  Height="500px" style="margin-left: 0px;background-color : whitesmoke;" Width="673px">
    <table style="width:100%; ">
   <tr>
       <td>
    <asp:Label ID="SystemIdLabel" runat="server" Text="System ID"></asp:Label>
    </td>
       <td>
                   
           
            <asp:DropDownList ID="SystemIDDropDownList" runat="server" Width="200px"  ></asp:DropDownList> <asp:Button ID="SysIDButton" runat="server" Width="145px" Text="Add/Edit System ID" OnClick="SysIDButton_Click" />
               
       </td>
     </tr>
     <tr>
         <td>
     <asp:Label ID="SystemNameLabel" runat="server" Text="System Name">
     </asp:Label>
       </td>
         <td>
      <asp:TextBox ID="SystemNameTextBox" runat="server" Width="350px"></asp:TextBox>
        
      </td>       
      </tr>
    
        <tr>
        <td>
     <asp:Label ID="SystemNameShortLabel" runat="server" Text="System Name Abbreviation">
     </asp:Label>
       </td>
      <td>
    <asp:TextBox ID="SystemNameShortTextBox" runat="server" Width="350px"></asp:TextBox>
     </td>
    </tr>
  <tr>
                <td>
                    <asp:Label ID="Label11" runat="server" Text="Description    "></asp:Label>
                </td>
                <td>
                    <textarea id="DescTextarea" runat="server" style="width: 344px; height: 56px"></textarea>
                </td>
            </tr>
    <tr>
        <td>
     <asp:Label ID="SecuritySensitivityLevelLabel" runat="server" Text="Security Sensitivity Level">
     </asp:Label>
            </td>
        <td>
            <asp:DropDownList ID="DropDownList1" runat="server" Height="25px" Width="350px">
                <asp:ListItem>low</asp:ListItem>
                <asp:ListItem>moderate</asp:ListItem>
                <asp:ListItem>high</asp:ListItem>
            </asp:DropDownList>
            &nbsp;&nbsp;
        </td>

     </tr>
     <tr>
                <td>
                    <asp:Label ID="Label3" runat="server" Text=" Properties   "></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="PropertyDropDownList" runat="server" Width="180px"  ></asp:DropDownList> <asp:Button ID="PropButton" runat="server" Width="141px" Text="Add/Edit Properties" OnClick="PropButton_Click" />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label4" runat="server" Text=" Annotations   "></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="AnnotationDropDownList" runat="server" Width="180px"  ></asp:DropDownList> <asp:Button ID="AnnotationButton" runat="server" Width="141px" Text="Add/Edit Annotations" OnClick="AnnotationButton_Click" />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label5" runat="server" Text=" Link   "></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="LinkDropDownList" runat="server" Width="180px" ></asp:DropDownList> <asp:Button ID="LinkButton" runat="server" Width="141px" Text="Add/Edit Links" OnClick="LinkButton_Click" />
                </td>
            </tr>
                 <tr>
        <td>
     <asp:Label ID="Label1" runat="server" Text="Date Authorized">
     </asp:Label>
            </td>
           <td>  <asp:TextBox ID="DateTextBox" runat="server" Width="344px"></asp:TextBox>
        <asp:Button ID="DateButton" runat="server" Text="GetDate" OnClick="DateButton_Click" />
               </td>
    </tr>
        <tr>
      <td> </td>  
        <td>
           <asp:Calendar ID="Calendar" runat="server" Visible="false" OnSelectionChanged="Calendar_SelectionChanged"></asp:Calendar>
      
       </td>
       </tr>
        
        </table>
        
          <br />
          &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
          <asp:Button ID="BackButton" runat="server" OnClick="BackButton_Click" Text="Back" />
          &nbsp;&nbsp;&nbsp;
                   <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />&nbsp;&nbsp;&nbsp;
          <asp:Button ID="ContinueButton" runat="server" Text="Continue" OnClick="ContinueButton_Click" />
  
          <br />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:Label ID="StatusLabel" runat="server" Text=""></asp:Label>
          <br />
      </asp:Panel>

        </div>
            <div class="col-md-6">
    <asp:Panel ID="AddPropPanel" runat="server"  Height="235px" style="margin-left: 0px;background-color : whitesmoke;" Width="320px">
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<asp:Label ID="PropLabel" runat="server" Text=""></asp:Label>
        <br />
        <table>
            <tr>
                <td style="width: 80px">
                    <asp:Label ID="MainLabel" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    
                    <asp:DropDownList ID="PropDropDownList" runat="server" AutoPostBack="True"  Width="200px" Height="20px" OnSelectedIndexChanged="PropDropDownList_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="width: 80px"> <asp:Label ID="NameLabel" runat="server" Text=""></asp:Label></td>
                <td>
                    <asp:TextBox ID="NameTextBox" runat="server" Width="200px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="width: 80px">    <asp:Label ID="NSLabel" runat="server" Text=""></asp:Label>&nbsp; </td>
                <td>
                    <asp:TextBox ID="NSTextBox" runat="server" Width="200px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="width: 80px">    <asp:Label ID="ClassLabel" runat="server" Text=""></asp:Label></td>
                <td><asp:TextBox ID="ClassTextBox" runat="server" Width="200px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="width: 80px"><asp:Label ID="ValueLabel" runat="server" Text=""></asp:Label></td>
                <td><asp:TextBox ID="ValueTextBox" runat="server" Width="200px"></asp:TextBox>
                </td>
            </tr>
             <tr>
                <td style="width: 80px"><asp:Label ID="RemarksLabel" runat="server" Text=""></asp:Label></td>
                <td>
                    <textarea runat="server" id="RemarksTextArea" rows="3" style="width: 200px"></textarea>
                </td>
            </tr>
        </table>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="AddEditButton" runat="server" Text="Add/Edit" OnClick="AddEditButton_Click" />
        &nbsp;&nbsp;<asp:Button ID="RemoveButton" runat="server" Text="Delete" />
        &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;
        <br />
        <asp:Label ID="PropStatusLabel" runat="server" Text=""></asp:Label>
    </asp:Panel>
  
<asp:Panel ID="AddAuxPanel" runat="server" Width="380px" Height="105px"  style="margin-left: 10px;background-color : whitesmoke;">
           <table>
               <tr>
                   <td>
                       &nbsp;</td>
                   <td>
                       <asp:DropDownList ID="AuxDropDownList" runat="server" Width="200px"></asp:DropDownList>
                   </td>
               </tr>
               <tr>
                   <td>
                       <asp:Label ID="FirstLabel" runat="server" Text="Identification Type" Width="150px"></asp:Label>
                   </td>
                   <td>
                       <asp:DropDownList ID="TypeDropDownList" runat="server"  Width="200px">
                           <asp:ListItem Value="https://fedramp.gov"></asp:ListItem>
                           <asp:ListItem Value="https://ietf.org/rfc/rfc4122"></asp:ListItem>
                       </asp:DropDownList>
                      
                   </td>
               </tr>
               <tr>
                   <td>
                       <asp:Label ID="SecondLabel" runat="server" Text="System Identification"></asp:Label>                    
                   </td>
                   <td>
                       <asp:TextBox ID="SecondTextBox" runat="server" Width="200px"></asp:TextBox>
                   </td>
               </tr>
                    
           </table>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:Button ID="AddAuxButton" runat="server" Text="Add" OnClick="AddAuxButton_Click" />
           &nbsp;&nbsp;&nbsp;
           <asp:Button ID="RemoveAuxButton" runat="server" Text="Remove" />
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
       </asp:Panel>
       

   </div>


               </div>
            </div>
         </div>


    
</asp:Content>
