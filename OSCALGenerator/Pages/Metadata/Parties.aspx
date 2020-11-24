<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Parties.aspx.cs" Inherits="OSCALGenerator.Pages.Metadata.Parties" %>
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
<asp:Panel ID="AddPartyPanel" runat="server"  Height="409px" style="margin-left: 10px;background-color : whitesmoke;" Width="480px">
        <br />
        <table>
            <tr>
                <td>
                    <asp:Label ID="PatyIDLabel" runat="server" Text="Party ID:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="PartyIDTextBox" runat="server" Width="175px"></asp:TextBox>
                    <asp:DropDownList ID="PartyIdDropDownList" runat="server" Width="175px" AutoPostBack="True" OnSelectedIndexChanged="PartyIdDropDownList_SelectedIndexChanged"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="TypeLabel" runat="server" Text="Party Type:"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="DropDownList1" runat="server" Height="21px" Width="297px" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                        <asp:ListItem>Organization</asp:ListItem>
                        <asp:ListItem>Person</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="OrgNameLabel" runat="server" Text="Organization Name:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="OrgNameTextBox" runat="server" Width="350px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="ShortNameLabel" runat="server" Text="Organization Short Name:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="ShortNameTextBox" runat="server" Width="350px"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="OrgIdLabel" runat="server" Text="Organization ID:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="OrgIDTextBox" runat="server" Width="350px"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="PersonNameLabel" runat="server" Text="Person Name:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="PersonNameTextBox" runat="server" Width="350px"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="AddressLabel" runat="server" Text="Address Line:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="AddressLine1TextBox" runat="server" Width="350px"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="AddressLineLabel2" runat="server" Text="Address Line:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="AddressLine2TextBox" runat="server" Width="350px"></asp:TextBox>
                </td>
            </tr>

             <tr>
                <td>
                    <asp:Label ID="CityLabel1" runat="server" Text="City:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="CityTextBox" runat="server" Width="350px"></asp:TextBox>
                </td>
            </tr>

             <tr>
                <td>
                    <asp:Label ID="StateLabel" runat="server" Text="State:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="StateTextBox" runat="server" Width="350px"></asp:TextBox>
                </td>
            </tr>
           
             <tr>
                <td>
                    <asp:Label ID="PostalCodeLabel" runat="server" Text="Postal Code:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="PostalCodeTextBox" runat="server" Width="350px"></asp:TextBox>
                </td>
            </tr>

             <tr>
                <td>
                    <asp:Label ID="ContryLabel" runat="server" Text="Country:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="CountryTextBox" runat="server" Width="350px"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="PhoneLabel" runat="server" Text="Phone:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="PhoneTextBox" runat="server" Width="350px"></asp:TextBox>
                </td>
            </tr>

             <tr>
                <td>
                    <asp:Label ID="EmailLabel1" runat="server" Text="Email:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="EmailTextBox" runat="server" Width="350px"></asp:TextBox>
                </td>
            </tr>

        </table>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
             <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
        &nbsp;&nbsp;
        <asp:Button ID="DeleteButton" runat="server" OnClick="DeleteButton_Click" Text="Delete" />
    </asp:Panel>
    <asp:Panel ID="FormviewPanel" runat="server" style="margin-left: 10px;background-color : whitesmoke;" BackColor="WhiteSmoke" Width="400px">

        <asp:FormView ID="FormView1" runat="server" AllowPaging="True" CellPadding="4" ForeColor="#333333" BackColor="#3366FF" Width="400px" OnPageIndexChanging="FormView1_PageIndexChanging" OnItemUpdating="FormView1_ItemUpdating">
            <EditRowStyle BackColor="#2461BF" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <ItemTemplate>
                Party Id: <%# Eval("PartyID") %> 
                <br />
                Organization Name:  <%# Eval("OrgName") %> 
                <br />
                Organization Short Name:  <%# Eval("ShortName") %> 
                <br />
                Person Name:  <%# Eval("PersonName") %> 
                <br />
                Address:  <%# Eval("AddressLine1") %>  <%# Eval("AddressLine2") %> 
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <%# Eval("City") %>  <%# Eval("State") %>   <%# Eval("PostalCode") %>  <%# Eval("Country") %>.   
                <br />
                Phone:  <%# Eval("Phone") %> 
                <br /> 
                email:   <%# Eval("Email") %> 
                <br />
                <br />
            </ItemTemplate>
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#EFF3FB" />
        </asp:FormView>

         &nbsp;&nbsp;&nbsp; <asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" />  &nbsp;&nbsp;&nbsp;<asp:Button ID="AddPartyButton" runat="server" Text="Add Party" OnClick="AddPartyButton_Click" />&nbsp;&nbsp;&nbsp;<asp:Button ID="EditButton" runat="server" OnClick="EditButton_Click" Text="Edit Party" />
&nbsp;
    <asp:Button ID="ContinueButton" runat="server" Text="Continue" OnClick="ContinueButton_Click" />

    </asp:Panel>

</asp:Content>
