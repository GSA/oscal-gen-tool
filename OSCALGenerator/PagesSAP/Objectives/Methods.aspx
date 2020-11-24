<%@ Page Title="" Language="C#" MasterPageFile="~/SiteSAP.Master" AutoEventWireup="true" CodeBehind="Methods.aspx.cs" Inherits="OSCALGenerator.PagesSAP.Objectives.Methods" %>
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
                <h2 style="align-content: center"> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Method</h2>
               <div class="col-md-6">
               
    <asp:Panel ID="AddDescriptionPanel" runat="server"  Height="365px" style="background-color : whitesmoke;" Width="470px">
       
       
        <br />
        <table style="height: 234px">
           <tr>
               <td>
                   Method&nbsp; Id
               </td>
               <td>
                   <asp:DropDownList ID="ControlDropDownList" runat="server" Height="25px" Width="317px" AutoPostBack="True" OnSelectedIndexChanged="ControlDropDownList_SelectedIndexChanged"></asp:DropDownList>
               </td>
           </tr>
            <tr>
                <td>
                    <asp:Label ID="DescriptionLabel" runat="server" Text="Description   "></asp:Label>
                </td>
                <td style="width: 331px">
                    <textarea id="DescriptionTextArea" runat="server" cols="50" rows="3"></textarea>
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
                    Part</td>
                <td style="width: 331px">
                     <asp:Button ID="AddPartButton" runat="server" Width="325px" Text="Add/Edit Part" OnClick="AddPartButton_Click" />
                </td>
            </tr>

           

            <tr>
                <td>
                    &nbsp;Remarks&nbsp;</td>
                <td style="width: 331px">
                       <textarea id="OBJRemarkTextarea" runat="server" cols="50" rows="3"></textarea>
                   
                </td>
            </tr>

        </table>
        <br />
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;
             <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
        &nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp; 
        <asp:Button ID="MainDeleteButton" runat="server" OnClick="MainDeleteButton_Click" Text="Delete" />
        <br />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Label ID="StatusLabel" runat="server"></asp:Label>
    </asp:Panel>
    
   </div>
   <div class="col-md-6">
    <asp:Panel ID="AddPropPanel" runat="server"  Height="234px" style="margin-left: 0px;background-color : whitesmoke;" Width="320px">
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
        &nbsp;&nbsp;<asp:Button ID="RemoveButton" runat="server" Text="Delete" OnClick="RemoveButton_Click" />
        &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Label ID="PropStatusLabel" runat="server" Text=""></asp:Label>
    </asp:Panel>
  
       <asp:Panel ID="AddPartPanel" runat="server" Width="430px" Height="350px" style="background-color : whitesmoke;">
           <h3> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Part</h3>
           <table>
                <tr>
                   <td>
                       &nbsp;Name&nbsp;</td>
                   <td>
                       <asp:TextBox ID="PartNameTextBox" runat="server" Width="250px"></asp:TextBox>
                   </td>
               </tr>
                <tr>
                   <td>
                       &nbsp;NS&nbsp;</td>
                   <td>
                       <asp:TextBox ID="PartNSTextBox" runat="server" Width="250px"></asp:TextBox>
                   </td>
               </tr>

                <tr>
                   <td>
                       &nbsp;Class&nbsp;</td>
                   <td>
                       <asp:TextBox ID="PartClassTextBox" runat="server" Width="250px"></asp:TextBox>
                   </td>
               </tr>

               <tr>
                   <td>
                       &nbsp;Title&nbsp;</td>
                   <td>
                       <asp:TextBox ID="PartTitleTextBox" runat="server" Width="250px"></asp:TextBox>
                   </td>
               </tr>
                
               <tr>
                   <td>
                       <asp:Label ID="Label8" runat="server" Text="Description" Width="100px"></asp:Label>
                   </td>
                   <td>
                       <textarea runat="server" id="PrivilegeDescTextArea" rows="3" style="width: 250px"></textarea>
                   </td>
               </tr>
               <tr>
                <td>
                    <asp:Label ID="Label1" runat="server" Text=" Properties   "></asp:Label>
                </td>
                <td style="width: 331px">
                    <asp:DropDownList ID="PropPartDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="PropPartButton" runat="server" Width="141px" Text="Add/Edit Properties" OnClick="PropPartButton_Click" />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label2" runat="server" Text=" Links   "></asp:Label>
                </td>
                <td style="width: 331px">
                    <asp:DropDownList ID="LinkPartDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="LinkPartButton" runat="server" Width="141px" Text="Add/Edit Links" OnClick="LinkPartButton_Click"/>
                </td>
            </tr>

            
            <tr>
                <td>
                    Parts</td>
                <td style="width: 331px">
                    <asp:DropDownList ID="RealPartDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="RealPartButton" runat="server" Width="141px" Text="Add/Edit Part" OnClick="RealPartButton_Click"  />
                </td>
            </tr>

               
           </table>
           <br />
              &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<asp:Button ID="SaveTestStepButton" runat="server" Text="Save" OnClick="SaveTestStepButton_Click"  />  &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
           <br />
               <asp:Label ID="PartStatusLabel" runat="server" Text=""></asp:Label>
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
                    <asp:Label ID="ControlIDLabel" runat="server" Text="Part ID:"></asp:Label>
                </td>
                <td style="height: 22px">
                    <asp:DropDownList ID="PartsDropDownList" runat="server" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="PartsDropDownList_SelectedIndexChanged"></asp:DropDownList>
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
         &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp<asp:Button ID="DeleteButton" runat="server" Text="Remove" OnClick="DeleteButton_Click" />

  </asp:Panel>

   </div>
   


   </div>
   </div>
   </div>
     <asp:Panel ID="GridviewPanel" runat="server">
         <h2 style="align-content: center"> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Methods</h2>
              
        <asp:GridView ID="MethodGridView" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" >
           
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
          &nbsp;&nbsp;&nbsp<asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" />&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="EditButton" runat="server" Text="Add/Edit Methods" OnClick="EditButton_Click" />
        &nbsp;&nbsp;
         <asp:Button ID="ContinueButton" runat="server" Text="Continue" OnClick="ContinueButton_Click" />

        </asp:Panel>

</asp:Content>
