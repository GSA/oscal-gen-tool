<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SystemInfoTypes.aspx.cs" Inherits="OSCALGenerator.Pages.SystemCharacteristics.SystemInfoTypes" %>
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

    <h3 style="align-content: center"> System Characteristics: System Information Types</h3>
    <asp:Panel ID="Panel1" runat="server"  Height="400px" style="margin-left: 0px;background-color : whitesmoke;" Width="510px">
        <br />
        <table>
            <tr>
                <td>
                    <asp:Label ID="InfoTypeNameLabel" runat="server" Text="Information Type Name:"></asp:Label>
                </td>
                <td style="width: 342px">
                    <asp:TextBox ID="InfoTypeNameTextBox" runat="server" Width="300px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="InfoTypeIDLabel" runat="server" Text="Information Type ID:"></asp:Label>
                </td> 
                <td style="width: 342px">
                       <asp:Label ID="InfoTypeIDValueLabel" runat="server" Text=""></asp:Label>
                 </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label1" runat="server" Text="Information Sys Type ID:"></asp:Label>
                </td>
                <td style="width: 342px">
                    <asp:TextBox ID="SysTypeIDTextBox" runat="server" Width="300px"></asp:TextBox>
                </td>
             </tr>
            <tr>
                <td>
                    <asp:Label ID="InfoTypeSystemLabel" runat="server" Text="Description:"></asp:Label>
                </td>
                <td style="width: 342px">
                    <textarea id="DescTextarea" runat="server" style="width: 299px; height: 51px"></textarea>
                    <%--<asp:TextBox ID="InfoTypeSystemTextBox" runat="server" Width="300px"></asp:TextBox>--%>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="ConfImpactBaseLabel" runat="server" Text="Confidentiality Impact Base: "></asp:Label>
                </td>
                <td style="width: 342px">
                    <asp:DropDownList ID="ConfImpactBaseDropDownList" runat="server" Height="38px" Width="300px">
                        <asp:ListItem>fips-199-low</asp:ListItem>
                        <asp:ListItem>fips-199-moderate</asp:ListItem>
                        <asp:ListItem>fips-199-high</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="ConfImpactSelectedLabel" runat="server" Text="Confidentiality Impact Selected: "></asp:Label>
                </td>
                <td style="width: 342px">
                    <asp:DropDownList ID="ConfImpactSelectedDropDownList" runat="server" Height="38px" Width="300px">
                        <asp:ListItem>fips-199-low</asp:ListItem>
                        <asp:ListItem>fips-199-moderate</asp:ListItem>
                        <asp:ListItem>fips-199-high</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
             <tr>
                <td>
                    <asp:Label ID="IntegrityImpactBaseLabel" runat="server" Text="Integrity Impact Base: "></asp:Label>
                </td>
                <td style="width: 342px">
                    <asp:DropDownList ID="IntegrityImpactBaseDropDownList" runat="server" Height="38px" Width="300px">
                        <asp:ListItem>fips-199-low</asp:ListItem>
                        <asp:ListItem>fips-199-moderate</asp:ListItem>
                        <asp:ListItem>fips-199-high</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="IntegrityImpactSelectedLabel" runat="server" Text="Integrity Impact Selected: "></asp:Label>
                </td>
                <td style="width: 342px">
                    <asp:DropDownList ID="IntegrityImpactSelectedDropDownList" runat="server" Height="38px" Width="300px">
                        <asp:ListItem>fips-199-low</asp:ListItem>
                        <asp:ListItem>fips-199-moderate</asp:ListItem>
                        <asp:ListItem>fips-199-high</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="AvailabilityImpactBaseLabel" runat="server" Text="Availability Impact Base: "></asp:Label>
                </td>
                <td style="width: 342px">
                    <asp:DropDownList ID="AvailabilityImpactBaseDropDownList" runat="server" Height="38px" Width="300px">
                        <asp:ListItem>fips-199-low</asp:ListItem>
                        <asp:ListItem>fips-199-moderate</asp:ListItem>
                        <asp:ListItem>fips-199-high</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="AvailabilityImpactSeletedLabel" runat="server" Text="Availability Impact Selected: "></asp:Label>
                </td>
                <td style="width: 342px">
                    <asp:DropDownList ID="AvailabilityImpactSeletedDropDownList" runat="server" Height="38px" Width="300px">
                        <asp:ListItem>fips-199-low</asp:ListItem>
                        <asp:ListItem>fips-199-moderate</asp:ListItem>
                        <asp:ListItem>fips-199-high</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
             <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
    </asp:Panel>
    <asp:Panel ID="FormviewPanel" runat="server" BackColor="WhiteSmoke" Width="450px">

         <asp:FormView ID="FormView1" runat="server" AllowPaging="True" CellPadding="4" ForeColor="#333333" BackColor="#3366FF" Width="400px" OnPageIndexChanging="FormView1_PageIndexChanging">
            <EditRowStyle BackColor="#2461BF" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <ItemTemplate>
                Information Type Name: <%# Eval("Name") %> 
                <br />
                Information Type ID:  <%# Eval("InfoId") %> 
                <br />
                Information Type NIST ID:  <%# Eval("InfoTypeSytemId") %> 
                <br />
                 Description:  <%# Eval("Description") %> 
                <br />
                Confidentiality Impact Base:  <%# Eval("ConfidentialityImpactBase") %> 
                <br />
                Confidentiality Impact Selected:  <%# Eval("ConfidentialityImpactSelected") %>
                <br />
                Integrity Impact Base:  <%# Eval("IntegrityImpactBase") %> 
                <br />
                Integrity Impact Selected:  <%# Eval("IntegrityImpactSelected") %> 
                <br /> 
                Availability Impact Base:   <%# Eval("AvailabilityImpactBase") %> 
                <br />
                Availability Impact Selected: <%# Eval("AvailabilityImpactSelected") %>
                <br />
            </ItemTemplate>
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#EFF3FB" />
        </asp:FormView>
         &nbsp;&nbsp;&nbsp   <asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" />   <asp:Button ID="AddSysInfoTypeButton" runat="server" Text="Add System Information Type" OnClick="AddSysInfoTypeButton_Click" />  &nbsp;&nbsp;&nbsp
         <asp:Button ID="ContinueButton" runat="server" Text="Continue" OnClick="ContinueButton_Click" />
        <asp:Label ID="StatusLabel" runat="server" Text=""></asp:Label>
    
    </asp:Panel>

</asp:Content>
