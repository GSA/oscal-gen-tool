﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Components.aspx.cs" Inherits="OSCALGenerator.PageSSP.SystemImplementation.Components" %>
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
           <div class="row"  width:80%" >
                <h2 style="align-content: center"> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Components</h2>
               <div class="col-md-6">
     
    <asp:Panel ID="AddRolePanel" runat="server"  Height="460px" style="background-color : whitesmoke;" Width="520px">
        <br />
        <table>
            <tr>
                <td>
                    <asp:Label ID="RoleIDLabel" runat="server" Text=""></asp:Label>
                </td>
                <td>
                   
                    <asp:DropDownList ID="RoleDropDownList" runat="server" Width="310px" AutoPostBack="True" OnSelectedIndexChanged="RoleDropDownList_SelectedIndexChanged"></asp:DropDownList>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label1" runat="server" Text=" Type"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TypeTextBox" runat="server" Width="311px"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label6" runat="server" Text=" Title"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TitleTextBox" runat="server" Width="311px"></asp:TextBox>
                </td>
            </tr>
           
            <tr>
                <td>
                    <asp:Label ID="DescriptionLabel" runat="server" Text="Description   "></asp:Label>
                </td>
                <td>
                    <textarea id="DescriptionTextArea" runat="server" cols="50" rows="2"></textarea>
                </td>
            </tr>

             <tr>
                <td>
                    <asp:Label ID="Label3" runat="server" Text=" Properties   "></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="PropertyDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="PropButton" runat="server" Width="141px" Text="Add/Edit Properties" OnClick="PropButton_Click" />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label4" runat="server" Text=" Annotations   "></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="AnnotationDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="AnnotationButton" runat="server" Width="141px" Text="Add/Edit Annotations" OnClick="AnnotationButton_Click" />
                </td>
            </tr>

            <tr>
                <td>
                    Links</td>
                <td>
                    <asp:DropDownList ID="LinkDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="LinkButton" runat="server" Width="141px" Text="Add/Edit Links" OnClick="LinkButton_Click" />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label2" runat="server" Text="Remarks    "></asp:Label>
                </td>
                <td>
                    <textarea id="RoleRemarksTextarea" runat="server" cols="50" rows="2"></textarea>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label8" runat="server" Text=" Status State"></asp:Label>
                </td>
                <td>
                   
                    <asp:DropDownList ID="StateDropDownList" runat="server" Width="311px">
                        <asp:ListItem Value="operational">Operational</asp:ListItem>
                        <asp:ListItem Value="under-development">Under-development</asp:ListItem>
                        <asp:ListItem Value="under-major-modification">Under-major-modification</asp:ListItem>
                        <asp:ListItem Value="disposition">Disposition</asp:ListItem>
                        <asp:ListItem Value="other">Other</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label7" runat="server" Text="Status Remarks    "></asp:Label>
                </td>
                <td>
                    <textarea id="StatusRemarksTextarea" runat="server" cols="50" rows="2"></textarea>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label9" runat="server" Text=" Responsible Roles   "></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ResponsibleRoleDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="AddResponsibleRoleButton" runat="server" Width="141px" Text="Add/Remove Role" OnClick="AddResponsibleRoleButton_Click" />
                </td>
            </tr>

        </table>
        <br />
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;
             <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
        &nbsp;&nbsp;&nbsp;&nbsp; <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" />
        <br />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Label ID="StatusLabel" runat="server"></asp:Label>
    </asp:Panel>
    
   </div>
   <div class="col-md-6">
    <asp:Panel ID="AddPropPanel" runat="server"  Height="224px" style="margin-left: 0px;background-color : whitesmoke;" Width="320px">
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
    </asp:Panel>
    
       <asp:Panel ID="AddResponsibleRolePanel" runat="server" Width ="350px" Height="70px" style="background-color : whitesmoke;">
         <table>  
             <tr>
                 <td>
                     <asp:Label ID="Label5" runat="server" Text="Responsible Role ID"></asp:Label> 
                 </td>
                 <td>
                     <asp:DropDownList ID="RespRoleDropDownList" runat="server" Width="200px"></asp:DropDownList>
                 </td>
             </tr>
         </table>   
           <br />
            &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;  <asp:Button ID="AddRoleButton" runat="server" Text="Add" OnClick="AddRoleButton_Click1" />
           &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;   <asp:Button ID="RemoveRoleButton" runat="server" Text="Remove" /> 
       </asp:Panel>

   </div>
   


   </div>
   </div>
   </div>
    <asp:Panel ID="GridviewPanel" runat="server">
         <h2 style="align-content: center"> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Components</h2>
            
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
        &nbsp;&nbsp;&nbsp<asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="EditButton" runat="server" Text="Add/Edit Components " OnClick="EditButton_Click" />
        &nbsp;&nbsp;
         <asp:Button ID="ContinueButton" runat="server" Text="Continue" OnClick="ContinueButton_Click" />

    </asp:Panel>

</asp:Content>
