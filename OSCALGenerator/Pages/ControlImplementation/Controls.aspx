<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Controls.aspx.cs" Inherits="OSCALGenerator.Pages.ControlImplementation.Controls" %>
<%@ Register src="../../CustomControls/Parameters.ascx" tagname="Parameters" tagprefix="uc1" %>
<%@ Register src="../../CustomControls/Statements.ascx" tagname="Statements" tagprefix="uc2" %>
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

     <h4 style="align-content: center"> Controls</h4>
<asp:Panel ID="AddControlPanel" runat="server"  Height="650px" style="margin-left: 10px;background-color : whitesmoke;" Width="1000px">
        <br />
    <asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Horizontal" CellSpacing="10" AutoPostBack="True" CellPadding="10" OnSelectedIndexChanged="RadioButtonList1_SelectedIndexChanged" Visible="False">
         <asp:ListItem>Low</asp:ListItem>
         <asp:ListItem>Moderate</asp:ListItem>
        <asp:ListItem>High</asp:ListItem>
        </asp:RadioButtonList>
    <asp:Label ID="SensitivityLevelLabel" runat="server" Text="Security Sensitivity Level:"></asp:Label>
    <br />  
    
    <table runat="server">
            <tr>
                <td>
                    <asp:Label ID="ControlIDLabel" runat="server" Text="Control ID:"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ControlDropDownList" runat="server" Width="360px" AutoPostBack="True" OnSelectedIndexChanged="ControlDropDownList_SelectedIndexChanged"></asp:DropDownList>
                </td>
            </tr>
            
            <tr>
                <td>
                    <asp:Label ID="RoleIDLabel" runat="server" Text="Responsible Role:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="ResponsibleRoleTextBox" runat="server" Width="350px"></asp:TextBox>
                    &nbsp;&nbsp;&nbsp;
                    <asp:Button ID="RespRoleButton" runat="server" OnClick="RespRoleButton_Click" Text="Update Responsible Role" Width="194px" />
                    &nbsp;
                    <asp:DropDownList ID="RoleDropDownList" runat="server" Width="178px" Visible="False" AutoPostBack="True" OnSelectedIndexChanged="RoleDropDownList_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
            </tr>
       
           <tr>
            <td> Parameters </td>
            <td>
            <asp:DropDownList ID="ParameterDropDownList" runat="server" Width="360px" OnSelectedIndexChanged="ParameterDropDownList_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
            <br />
            <textarea  runat="server" id="ParameterTextArea" cols="150" style="max-width: 800px; font-family:monospace;" rows="3"></textarea>
            <br />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;<asp:Button ID="SaveParameterButton" runat="server" Text="Save Parameter" OnClick="SaveParameterButton_Click" />
       </td>
               </tr>


            <tr>
                <td>
                    Implementation Status
                    <br />
                    (Check all that apply)
                </td>
                <td>
                    <table>
                        <tr>
                            <td>
                               &nbsp;&nbsp; &nbsp;<asp:CheckBox ID="implemented" Height="20px" Width="20px" runat="server" /><asp:Label ID="ImplementedLabel" runat="server" Text="Implemented"></asp:Label>
                            </td>
                            <td>
                                &nbsp;&nbsp; &nbsp;<asp:CheckBox ID="partiallyimplemented" Height="20px" Width="20px" runat="server" /><asp:Label ID="Label1" runat="server" Text="Partially implemented"></asp:Label>
                
                            </td>
                            <td>
                                &nbsp;&nbsp; &nbsp;<asp:CheckBox ID="planned" Height="20px" Width="20px" runat="server" /><asp:Label ID="Label2" runat="server" Text="Planned"></asp:Label>
                
                            </td>
                        </tr>
                        <tr>
                            <td>
                               &nbsp;&nbsp; &nbsp;<asp:CheckBox ID="alternativeimplementation" Height="20px" Width="20px" runat="server" /><asp:Label ID="Label3" runat="server" Text="Alternative implementation"></asp:Label>
                            </td>
                            <td>
                                &nbsp;&nbsp; &nbsp;<asp:CheckBox ID="notapplicable" Height="20px" Width="20px" runat="server" /><asp:Label ID="Label4" runat="server" Text="Not applicable"></asp:Label>
                
                            </td>
                            <td>
                               <%-- &nbsp;&nbsp; &nbsp;<asp:CheckBox ID="CheckBox5" Height="20px" Width="20px" runat="server" /><asp:Label ID="Label5" runat="server" Text="Implemented"></asp:Label>
                --%>
                            </td>
                        </tr>
                    </table>
                </td>

            </tr>

            <tr>
                <td>

                  Control Origination
                    <br />
                    (check all that apply):
                </td>
                <td>

                      <table>
                        <tr>
                            <td>
                               &nbsp;&nbsp; &nbsp;<asp:CheckBox ID="serviceprovidercorporate" Height="20px" Width="20px" runat="server" /><asp:Label ID="Label5" runat="server" Text="Service Provider Corporate"></asp:Label>
                            </td>
                            <td>
                                &nbsp;&nbsp; &nbsp;<asp:CheckBox ID="serviceprovidersystemspecific" Height="20px" Width="20px" runat="server" /><asp:Label ID="Label6" runat="server" Text="Service Provider System Specific"></asp:Label>
                
                            </td>
                            
                        </tr>
                        <tr>
                            <td>
                                &nbsp;&nbsp; &nbsp;<asp:CheckBox ID="serviceproviderhybrid" Height="20px" Width="20px" runat="server" /><asp:Label ID="Label7" runat="server" Text="Service Provider Hybrid (Corporate and System Specific)"></asp:Label>
                
                            </td>

                            <td>
                               &nbsp;&nbsp; &nbsp;<asp:CheckBox ID="configuredbycustomer" Height="20px" Width="20px" runat="server" /><asp:Label ID="Label8" runat="server" Text="Configured by Customer (Customer System Specific) "></asp:Label>
                            </td>
                            </tr>
                          <tr>
                            <td>
                                &nbsp;&nbsp; &nbsp;<asp:CheckBox ID="providedbycustomer" Height="20px" Width="20px" runat="server" /><asp:Label ID="Label9" runat="server" Text="Provided by Customer (Customer System Specific) "></asp:Label>
                
                            </td>
                            <td>
                                &nbsp;&nbsp; &nbsp;<asp:CheckBox ID="shared" Height="20px" Width="20px" runat="server" /><asp:Label ID="Label10" runat="server" Text="Shared (Service Provider and Customer Responsibility)"></asp:Label>
                
                            </td>
                        </tr>
                        <tr>

                            <td>
                                &nbsp;&nbsp; &nbsp;<asp:CheckBox ID="inherited" Height="20px" Width="20px" runat="server" /><asp:Label ID="Label11" runat="server" Text="Inherited from pre-existing FedRAMP Authorization"></asp:Label>
                
                            </td>
                            <td>
                                &nbsp;&nbsp; &nbsp;<asp:Label ID="Label12" runat="server" Text="Enter the Date of Authorization "></asp:Label> <asp:TextBox ID="AuthorizationDateTextBox" runat="server"></asp:TextBox>
                            </td>

                        </tr>
                        

                    </table>

                </td>

            </tr>
           <%-- </table>
            <table style="width:490px; background-color : azure;">--%>
                  <tr>
                   <td> Statements </td>
                    <td>
                        <asp:Label ID="StatementLabel" runat="server" Width="800px" Text=""></asp:Label>
                        <asp:DropDownList ID="StatementDropDownList" runat="server" Width="900px" OnSelectedIndexChanged="StatementDropDownList_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                           <br />
                               <textarea runat="server" id="StatementTextArea" cols="150" style="max-width: 800px; font-family:monospace;" rows="6"></textarea>
                             
                           <br />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp;&nbsp;<asp:Button ID="SaveStatementButton" runat="server" Text="Save Statement" OnClick="SaveStatementButton_Click" />
                        </td>
                    </tr>
         </table>
            <br />
           
    
          &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   <asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" />   &nbsp;&nbsp;&nbsp;&nbsp; <asp:Button ID="SaveButton" runat="server" OnClick="UpdateControlButton_Click" Text="Update Control" />
            &nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="ContinueButton" runat="server" Text="Continue" OnClick="ContinueButton_Click"/>
        
    <asp:Label ID="StatusLabel" runat="server" Text=""></asp:Label>
    </asp:Panel>
    

</asp:Content>
