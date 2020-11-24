<%@ Page Title="" Language="C#" MasterPageFile="~/SiteSAP.Master" AutoEventWireup="true" CodeBehind="Parties.aspx.cs" Inherits="OSCALGenerator.PagesSAP.Metadata.Parties" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

 <table style="width:100%">
   <tr>
       <td>
          <asp:Label ID="CorpNameLabel" runat="server" Text="" ForeColor="#0067ac"></asp:Label>
      </td>
       <td>
          <asp:Label ID="SystemNameLabel" runat="server" Text="" ForeColor="#0067ac"></asp:Label>
           
       </td>
       <td>
          <asp:Label ID="DocNameLabel" runat="server" Text=""  ForeColor="#0067ac"></asp:Label>
           
       </td>
     </tr>

     </table>
    <h2 style="align-content: center"> Metadata: Parties</h2>

     <table>
            <tr>
                <td>

    <asp:Panel ID="MainPartyPanel" runat="server"  Height="480px" style="margin-left: 10px;background-color : whitesmoke;" Width="550px">
           
        <table>
            <tr>
                <td>
                    <asp:Label ID="Label1" runat="server" Text="Party ID:"></asp:Label>
                </td>
                <td>
                    
                    <asp:DropDownList ID="PartyDropDownList" runat="server" Width="300px" AutoPostBack="True" OnSelectedIndexChanged="PartyDropDownList_SelectedIndexChanged" ></asp:DropDownList>
                </td>
            </tr>
          <tr>
                <td>
                    <asp:Label ID="Label6" runat="server" Text="Party Name"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="PartyNameTextBox" runat="server" Width="310px"></asp:TextBox>
                </td>
            </tr>
          <tr>
                <td>
                    <asp:Label ID="PersonOrgLabel" runat="server" Text="Party Short Name"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="PartyShortNameTextBox" runat="server" Width="310px"></asp:TextBox>
                    
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label2" runat="server" Text="Party Type"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="PartyTypeDropDownList" runat="server" Width="310px">
                        <asp:ListItem Value="person">Person</asp:ListItem>
                        <asp:ListItem Value="organization">Organization</asp:ListItem>
                    </asp:DropDownList>
                  
                    
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label7" runat="server" Text="External ID"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="ExternalIDTextBox" runat="server" Width="310px"></asp:TextBox>
                    
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label13" runat="server" Text="External Type"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="ExternalTypeTextBox" runat="server" Width="310px"></asp:TextBox>
                    
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp; Addresses&nbsp;

                </td>
                <td>
                    <asp:DropDownList ID="AddressDropDownList" runat="server" Width="150px"  ></asp:DropDownList> <asp:Button ID="AddAddressButton" runat="server" Width="171px" Text="Add/Edit Address" OnClick="AddAddressButton_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp; Phones &nbsp;

                </td>
                <td>
                    <asp:DropDownList ID="PhoneDropDownList" runat="server" Width="150px"  ></asp:DropDownList> <asp:Button ID="AddPhoneButton" runat="server" Width="171px" Text="Add/Edit Phone" OnClick="AddPhoneButton_Click" />
                </td>
            </tr>

            <tr>
                <td>
                    &nbsp; Emails&nbsp;</td>
                <td>
                    <asp:DropDownList ID="EmailDropDownList" runat="server" Width="150px" ></asp:DropDownList> <asp:Button ID="AddEmailButton" runat="server" Width="171px" Text="Add/Edit Email" OnClick="AddEmailButton_Click"  />
                </td>
            </tr>

             <tr>
                <td>
                    &nbsp; Member of Organization&nbsp;</td>
                <td>
                    <asp:DropDownList ID="MemberOrgDropDownList1" runat="server" Width="150px" ></asp:DropDownList> <asp:Button ID="AddMemberOrgButton" runat="server" Width="171px" Text="Add/Edit Member of Org" OnClick="AddMemberOrgButton_Click"  />
                </td>
            </tr>

            
             <tr>
                <td>
                    &nbsp; Location UUID&nbsp;</td>
                <td>
                    <asp:DropDownList ID="LocationUUIDDropDownList" runat="server" Width="150px" ></asp:DropDownList> <asp:Button ID="LocationIDButton" runat="server" Width="171px" Text="Add/Edit Location UUID" OnClick="LocationIDButton_Click" />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label8" runat="server" Text=" Properties   "></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="PropertyDropDownList" runat="server" Width="180px"  ></asp:DropDownList> 
                    <asp:Button ID="MainPropButton" runat="server" Width="141px" Text="Add/Edit Properties" OnClick="MainPropButton_Click"  />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label9" runat="server" Text=" Annotations   "></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="AnnotationDropDownList" runat="server" Width="180px"  ></asp:DropDownList> 
                    <asp:Button ID="MainAnnotationButton" runat="server" Width="141px" Text="Add/Edit Annotations" OnClick="MainAnnotationButton_Click"  />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label10" runat="server" Text=" Link   "></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="LinkDropDownList" runat="server" Width="180px" ></asp:DropDownList>
                    <asp:Button ID="MainLinkButton" runat="server" Width="141px" Text="Add/Edit Links" OnClick="MainLinkButton_Click"  />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label11" runat="server" Text="Remarks    "></asp:Label>
                </td>
                <td>
                    <textarea id="MainPartyTextarea" runat="server" cols="50" rows="2"></textarea>
                </td>
            </tr>

          </table>
        
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             <asp:Button ID="SavePartyButton" runat="server" Text="Save" OnClick="SavePartyButton_Click" style="height: 26px"  />
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="CancelPartyButton" runat="server" Text="Cancel" OnClick="CancelPartyButton_Click"  />
        &nbsp;&nbsp;
        <asp:Button ID="DeletePartyButton" runat="server" OnClick="DeletePartyButton_Click" Text="Delete" />

    </asp:Panel>
    </td>
  
        <td>

    <asp:Panel ID="AddPropPanel" runat="server"  Height="234px" style="margin-left: 10px;background-color : whitesmoke;" Width="320px">
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
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;<asp:Button ID="AddMainButton" runat="server" Text="Add/Edit" OnClick="AddEditMainButton_Click" />
        &nbsp;&nbsp;&nbsp;<asp:Button ID="RemoveButton" runat="server" Text="Delete" OnClick="RemoveButton_Click1" style="height: 26px" />
        &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;
        <br />
        <asp:Label ID="PropStatusLabel" runat="server" Text=""></asp:Label>
    </asp:Panel>

       <asp:Panel ID="AddAuxPanel" runat="server" Width="318px" Height="105px"  style="margin-left: 10px;background-color : whitesmoke;">
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
                       <asp:Label ID="FirstLabel" runat="server" Text="Phone Type" Width="110px"></asp:Label>
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
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:Button ID="AddAuxButton" runat="server" Text="Add" OnClick="AddAuxButton_Click" />
           &nbsp;&nbsp;&nbsp;
           <asp:Button ID="RemoveAuxButton" runat="server" Text="Remove" OnClick="RemoveAuxButton_Click" style="height: 26px" />
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
       </asp:Panel>
       
       <asp:Panel ID="AddAddressPanel" runat="server" Width="450px" Height="260px"  style="margin-left: 10px;background-color : whitesmoke;">
           <table style="width: 364px">
             <tr>
                <td>
                    <asp:Label ID="AddressIDLabel" runat="server" Width="120px"></asp:Label>
                </td>
                <td>                
                    <asp:DropDownList ID="AddAddressDropDownList" runat="server" Width="310px" AutoPostBack="True" OnSelectedIndexChanged="AddAddressDropDownList_SelectedIndexChanged"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;&nbsp; Address Type</td>
                <td>
                    <asp:TextBox ID="AddressTypeTextBox" runat="server" Width="310px" OnTextChanged="AddressTypeTextBox_TextChanged"></asp:TextBox>
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
                      &nbsp;&nbsp;&nbsp; Country&nbsp;</td>
                <td>
                    <asp:TextBox ID="CountryTextBox" runat="server" Width="310px"></asp:TextBox>
                </td>
            </tr>

           </table>
            <br />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<asp:Button ID="SaveAddressButton" runat="server" Text="Add/Edit" OnClick="SaveAddressButton_Click" /> 
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
           <asp:Button ID="RemoveAddressButton" runat="server" Text="Remove" OnClick="RemoveAddressButton_Click" />
       </asp:Panel>


  </td>
 </tr>
</table>
    <asp:Panel ID="FormviewPanel" runat="server" style="margin-left: 10px;background-color : whitesmoke;" BackColor="WhiteSmoke" Width="400px">

        <asp:FormView ID="FormView1" runat="server" AllowPaging="True" CellPadding="4" ForeColor="#333333" BackColor="#3366FF" Width="400px" OnPageIndexChanging="FormView1_PageIndexChanging" OnItemUpdating="FormView1_ItemUpdating">
            <EditRowStyle BackColor="#2461BF" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <ItemTemplate>
                Party Id: <%# Eval("PartyID") %> 
                <br />
                  Party UUId: <%# Eval("UUID") %> 
                <br />
                 Name:  <%# Eval("Name") %> 
                <br />
                Short Name:  <%# Eval("ShortName") %> 
                 <br />
                Party Type:  <%# Eval("PartyType") %> 
                <br />
                External ID:  <%# Eval("ExternalID") %> 
                <br />
                External Type:  <%# Eval("ExternalType")%> 
             
                <br />
                Remarks:  <%# Eval("Remarks") %> 
                <br /> 
            </ItemTemplate>
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#EFF3FB" />
        </asp:FormView>

         &nbsp;&nbsp;&nbsp; <asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" />  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="EditButton" runat="server" OnClick="EditButton_Click" Text="Add/Edit Party" />
&nbsp;
    <asp:Button ID="ContinueButton" runat="server" Text="Continue" OnClick="ContinueButton_Click" />

    </asp:Panel>

         <asp:Label ID="StatusLabel" runat="server"></asp:Label>
    <br />

</asp:Content>
