<%@ Page Title="" Language="C#" MasterPageFile="~/SiteSAP.Master" AutoEventWireup="true" CodeBehind="LocationsSAP.aspx.cs" Inherits="OSCALGenerator.PagesSAP.Metadata.LocationsSAP" %>
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
           <div class="row" style="background-color : whitesmoke; width:80%" >
                <h2 style="align-content: center"> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Location</h2>
               <div class="col-md-6">
     
    <asp:Panel ID="AddLocationPanel" runat="server"  Height="530px" style="background-color : whitesmoke;" Width="433px">
        <br />
        <table>
            <tr>
                <td>
                    <asp:Label ID="RevisionIDLabel" runat="server" Text=""></asp:Label>
                </td>
                <td>                
                    <asp:DropDownList ID="LocationDropDownList" runat="server" Width="310px" AutoPostBack="True" OnSelectedIndexChanged="LocationDropDownList_SelectedIndexChanged"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;&nbsp; Address Type</td>
                <td>
                    
                    <asp:DropDownList ID="AddressTypeDropDownList" runat="server"  Width="310px">
                        <asp:ListItem Value="office">Office</asp:ListItem>
                        <asp:ListItem Value="home">Home</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;&nbsp; Address Line</td>
                <td>
                    <asp:TextBox ID="AddressLine1TextBox" runat="server" Width="310px"></asp:TextBox>
                </td>
            </tr>
            <tr>
               <td> &nbsp;&nbsp; Address Line</td>  
               <td>
                   <asp:TextBox ID="AddressLine2TextBox" runat="server" Width="310px"></asp:TextBox>
                </td>
             </tr>
             <tr>
                <td>
                    &nbsp;&nbsp; City</td>
                <td>
                    <asp:TextBox ID="CityTextBox" runat="server" Width="310px"></asp:TextBox>
                </td>
            </tr>

             <tr>
               <td>   &nbsp;&nbsp; State</td>  
               <td>
                   <asp:TextBox ID="StateTextBox" runat="server" Width="310px"></asp:TextBox>
                </td>
             </tr>
            <tr>
                <td style="height: 22px">
                      &nbsp;&nbsp; Postal Code</td>
                <td style="height: 22px">
                    <asp:TextBox ID="PostalCodeTextBox" runat="server" Width="310px"></asp:TextBox>
                </td>
            </tr>

             <tr>
                <td>
                      &nbsp;Country&nbsp;</td>
                <td>
                    <asp:TextBox ID="CountryTextBox" runat="server" Width="310px"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td>
                    &nbsp; Phones&nbsp;</td>
                <td>
                    <asp:DropDownList ID="PhoneDropDownList" runat="server" Width="180px"  ></asp:DropDownList> <asp:Button ID="AddPhonesButton" runat="server" Width="141px" Text="Add/Edit Phones" OnClick="AddPhonesButton_Click"  />
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp; Emails&nbsp;</td>
                <td>
                    <asp:DropDownList ID="EmailDropDownList" runat="server" Width="180px" ></asp:DropDownList> <asp:Button ID="AddEmailsButton" runat="server" Width="141px" Text="Add/Edit Emails" OnClick="AddEmailsButton_Click"  />
                </td>
            </tr>

            <tr>
                <td>
                    &nbsp; Url&nbsp;</td>
                <td>
                    <asp:DropDownList ID="UrlDropDownList" runat="server" Width="180px" ></asp:DropDownList> <asp:Button ID="AddUrlButton" runat="server" Width="141px" Text="Add/Edit Url" OnClick="AddUrlButton_Click" />
                </td>
            </tr>

             <tr>
                <td>
                    <asp:Label ID="Label3" runat="server" Text=" Properties   "></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="PropertyDropDownList" runat="server" Width="180px" OnSelectedIndexChanged="PropertyDropDownList_SelectedIndexChanged" ></asp:DropDownList> <asp:Button ID="PropButton" runat="server" Width="141px" Text="Add/Edit Properties" OnClick="PropButton_Click" />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label4" runat="server" Text=" Annotations   "></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="AnnotationDropDownList" runat="server" Width="180px" OnSelectedIndexChanged="AnnotationDropDownList_SelectedIndexChanged" ></asp:DropDownList> <asp:Button ID="AnnotationButton" runat="server" Width="141px" Text="Add/Edit Annotations" OnClick="AnnotationButton_Click" />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label5" runat="server" Text=" Link   "></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="LinkDropDownList" runat="server" Width="180px" OnSelectedIndexChanged="LinkDropDownList_SelectedIndexChanged" ></asp:DropDownList> <asp:Button ID="LinkButton" runat="server" Width="141px" Text="Add/Edit Links" OnClick="LinkButton_Click" />
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
  

       <asp:Panel ID="AddAuxPanel" runat="server" Width="318px" Height="105px">
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
                       <asp:Label ID="FirstLabel" runat="server" Text="Phone Type"></asp:Label>
                   </td>
                   <td>
                       <asp:TextBox ID="FirstTextBox" runat="server" Width="200px"></asp:TextBox>
                   </td>
               </tr>
               <tr>
                   <td>
                       <asp:Label ID="SecondLabel" runat="server" Text="Phone Number"></asp:Label>                    
                   </td>
                   <td>
                       <asp:TextBox ID="SecondTextBox" runat="server" Width="200px"></asp:TextBox>
                   </td>
               </tr>
                    
           </table>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:Button ID="AddAuxButton" runat="server" Text="Add" OnClick="AddAuxButton_Click" />
           &nbsp;&nbsp;&nbsp;
           <asp:Button ID="RemoveAuxButton" runat="server" Text="Remove" />
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
       </asp:Panel>

   </div>
   


   </div>
   </div>
   </div>
    <asp:Panel ID="GridviewPanel" runat="server">
         <h2 style="align-content: center"> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Locations</h2>
              
        <asp:GridView ID="LocationGridView" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" >
           
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
        &nbsp;&nbsp;&nbsp<asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" />&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="EditButton" runat="server" Text="Add/Edit Location" OnClick="EditButton_Click" />
        &nbsp;&nbsp;
         <asp:Button ID="ContinueButton" runat="server" Text="Continue" OnClick="ContinueButton_Click" />

    </asp:Panel> 
  
  

</asp:Content>
