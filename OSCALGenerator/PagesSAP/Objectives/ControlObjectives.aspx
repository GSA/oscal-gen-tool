<%@ Page Title="" Language="C#" MasterPageFile="~/SiteSAP.Master" AutoEventWireup="true" CodeBehind="ControlObjectives.aspx.cs" Inherits="OSCALGenerator.PagesSAP.Objectives.ControlObjectives" %>
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
           <div class="row" >
                <h2 style="align-content: center"> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Control Objectives</h2>
               <div class="col-md-6">
               
    <asp:Panel ID="AddDescriptionPanel" runat="server"  Height="350px" style="background-color : whitesmoke;" Width="470px">
       
       
        <br />
        <table>
           
            <tr>
                <td>
                    <asp:Label ID="Label6" runat="server" Text="Baseline"></asp:Label>
                </td>
                <td>
                     <asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Horizontal" CellSpacing="10" AutoPostBack="True" CellPadding="10" OnSelectedIndexChanged="RadioButtonList1_SelectedIndexChanged">
                       <asp:ListItem>Low</asp:ListItem>
                       <asp:ListItem>Moderate</asp:ListItem>
                       <asp:ListItem>High</asp:ListItem>
                     </asp:RadioButtonList>
                </td>

            </tr>

            <tr>
                <td>
                    <asp:Label ID="DescriptionLabel" runat="server" Text="Description   "></asp:Label>
                </td>
                <td style="width: 331px">
                    <textarea id="DescriptionTextArea" runat="server" cols="50" rows="5"></textarea>
                </td>
            </tr>

             <tr>
                <td>
                    <asp:Label ID="Label3" runat="server" Text=" Properties   "></asp:Label>
                </td>
                <td style="width: 331px">
                    <asp:DropDownList ID="PropertyDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="PropButton" runat="server" Width="141px" Text="Add/Edit Properties" OnClick="PropButton_Click" />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label4" runat="server" Text=" Annotations   "></asp:Label>
                </td>
                <td style="width: 331px">
                    <asp:DropDownList ID="AnnotationDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="AnnotationButton" runat="server" Width="141px" Text="Add/Edit Annotations" OnClick="AnnotationButton_Click" />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label1" runat="server" Text=" All "></asp:Label>
                </td>
                <td style="width: 331px">
                    <asp:TextBox ID="AllTextBox" runat="server" Width="301px"></asp:TextBox>
                   
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label2" runat="server" Text=" Include-Objective   "></asp:Label>
                </td>
                <td style="width: 331px">
                    <asp:DropDownList ID="IncludeControlsDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="IncludeControlsButton" runat="server" Width="141px" Text="Add/Edit Controls" OnClick="IncludeControlsButton_Click" />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label5" runat="server" Text=" Exclude-Objective   "></asp:Label>
                </td>
                <td style="width: 331px">
                    <asp:DropDownList ID="ExcludeControlsDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="ExcludeControlsButton" runat="server" Width="141px" Text="Add/Edit Controls" OnClick="ExcludeControlsButton_Click" />
                </td>
            </tr>

        </table>
        <br />
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="BackButton" runat="server" OnClick="BackButton_Click" Text="Back" />
        &nbsp;&nbsp;&nbsp;
             <asp:Button ID="SaveButton" runat="server" Text="Save and Continue" OnClick="SaveButton_Click" />
        &nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp; 
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
  
  <asp:Panel ID="AddControlPanel" runat="server"  style="margin-left: 0px;background-color : whitesmoke;" Width="300px">
    <table runat="server">
            <tr>
                <td>

                </td>
                <td>
                    <asp:Label ID="ControlLabel" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 22px">
                    <asp:Label ID="ControlIDLabel" runat="server" Text="Control ID:"></asp:Label>
                </td>
                <td style="height: 22px">
                    <asp:DropDownList ID="ControlDropDownList" runat="server" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="ControlDropDownList_SelectedIndexChanged"></asp:DropDownList>
                </td>
            </tr>
           <tr>
                <td style="height: 22px">
                    <asp:Label ID="Label7" runat="server" Text="Statement:"></asp:Label>
                </td>
                <td style="height: 22px">
                    <asp:TextBox ID="StatementTextBox" runat="server" Width="200px"></asp:TextBox>
                </td>
            </tr>
        </table>
        &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<asp:Button ID="AddButton" runat="server" Text="Add" OnClick="AddButton_Click" />
         &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp<asp:Button ID="DeleteButton" runat="server" Text="Remove" />

  </asp:Panel>

   </div>
   


   </div>
   </div>
   </div>

</asp:Content>
