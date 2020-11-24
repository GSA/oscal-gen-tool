<%@ Page Title="" Language="C#" MasterPageFile="~/SiteSAR.Master" AutoEventWireup="true" CodeBehind="Risks.aspx.cs" Inherits="OSCALGenerator.PagesSAR.Results.Risks" %>
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
                <h2 style="align-content: center"> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Risk</h2>
               <div class="col-md-6">
     
    <asp:Panel ID="AddRolePanel" runat="server"  Height="500px" style="background-color : whitesmoke;" Width="520px">
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
                    
                    Risk Metrics

                </td>
                <td>
                    <asp:DropDownList ID="RiskMetricsDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="RiskMetricsButton" runat="server" Width="141px" Text="Add/Edit Risk Metric" OnClick="RiskMetricsButton_Click"   />
                </td>
            </tr>

             <tr>
                <td>
                    
                    Mitigating Factors

                </td>
                <td>
                    <asp:DropDownList ID="MitigatingFactorsDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="MitigatingFactorButton" runat="server" Width="141px" Text="Add/Edit Factor" OnClick="MitigatingFactorButton_Click"   />
                </td>
            </tr>

             <tr>
                <td>
                    
                    Remediations

                </td>
                <td>
                    <asp:DropDownList ID="RemediationDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="RemediationButton" runat="server" Width="141px" Text="Add/Edit Remediation" OnClick="RemediationButton_Click"  />
                </td>
            </tr>

              <tr>
                <td>
                    
                    Risk Status

                </td>
                <td>
                    <asp:DropDownList ID="RiskStatusDropDownList" runat="server" Width="180px">
                        <asp:ListItem Value="open">Open</asp:ListItem>
                        <asp:ListItem Value="closed">Closed</asp:ListItem>
                    </asp:DropDownList> 
                </td>
            </tr>

             <tr>
                <td>
                    
                    Remediation Tracking

                </td>
                <td>
                    <asp:DropDownList ID="TrackingDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="RemediationTrackingButton" runat="server" Width="141px" Text="Add/Edit Tracking"  />
                </td>
            </tr>

             <tr>
                <td>
                    
                    Party UUIDs</td>
                <td>
                    <asp:DropDownList ID="RelevantEvidenceDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="AddPartyButton" runat="server" Width="141px" Text="Add/Edit Party"  />
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
                <td style="width: 80px; height: 55px;"><asp:Label ID="RemarksLabel" runat="server" Text=""></asp:Label></td>
                <td style="height: 55px">
                    <textarea runat="server" id="RemarksTextArea" rows="3" style="width: 200px"></textarea>
                </td>
            </tr>
        </table>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="AddEditButton" runat="server" Text="Add/Edit" OnClick="AddEditButton_Click" />
        &nbsp;&nbsp;<asp:Button ID="RemoveButton" runat="server" Text="Delete" />
        &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;
    </asp:Panel>
    
       <asp:Panel ID="AddAuxPanel" runat="server" Width ="316px" Height="220px" style="background-color : whitesmoke;">
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
           <asp:Label ID="AuxLabel" runat="server" Text=""></asp:Label>
         <br />
           <table>  
             <tr>
                 <td>
                     <asp:Label ID="KeyLabel" runat="server" Width="75px"></asp:Label> 
                 </td>
                 <td>
                     <asp:DropDownList ID="AuxDropDownList" runat="server" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="AuxDropDownList_SelectedIndexChanged"></asp:DropDownList>
                 </td>
             </tr>
                <tr>
                   <td>
                       <asp:Label ID="FirstLabel" runat="server" Text=""></asp:Label>
                   </td>
                   <td>
                       <asp:TextBox ID="FirstTextBox" runat="server" Width="195px"></asp:TextBox>
                   </td>
                    
               </tr>
               <tr>
                   <td>
                       <asp:Label ID="SecondLabel" runat="server" Text=""></asp:Label>
                   </td>
                   <td>
                       <asp:TextBox ID="SecondTextBox" runat="server" Width="195px"></asp:TextBox>
                      <asp:DropDownList ID="TypeDropDownList" runat="server" Width="195px">
                           <asp:ListItem Value="component">Component</asp:ListItem>
                           <asp:ListItem Value="inventory-item">Inventory Item</asp:ListItem>
                           <asp:ListItem Value="location">Location</asp:ListItem>
                           <asp:ListItem Value="party">Party</asp:ListItem>
                           <asp:ListItem Value="user">User</asp:ListItem>
                           <asp:ListItem Value="resource">Resource</asp:ListItem>
                       </asp:DropDownList>
                       
                       <asp:DropDownList ID="OTypeDropDownList" runat="server" Width="195px">
                           
                           <asp:ListItem Value="tool">Tool</asp:ListItem>
                           <asp:ListItem Value="test-method">Test Method</asp:ListItem>
                           <asp:ListItem Value="task">Task</asp:ListItem>
                           <asp:ListItem Value="included-activity">Included Activity</asp:ListItem>
                           <asp:ListItem Value="other">Other</asp:ListItem>
                           
                       </asp:DropDownList>
                   </td>
                    
               </tr>
               <tr>
                   <td>
                       <asp:Label ID="ThirdLabel" runat="server" Text=""></asp:Label>
                   </td>
                   <td>
                       <asp:TextBox ID="ThirdTextBox" runat="server" Width="195px"></asp:TextBox>
                   </td>
                    
               </tr>
               <tr>
                   <td>
                       <asp:Label ID="FourLabel" runat="server" Text=""></asp:Label>
                   </td>
                   <td>
                       <asp:TextBox ID="FourTextBox" runat="server" Width="195px"></asp:TextBox>
                   </td>
                    
               </tr>
         </table>   
           <br />
            &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  <asp:Button ID="AddAuxButton" runat="server" Text="Add" OnClick="AddAuxButton_Click" />
           &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;   <asp:Button ID="RemoveRoleButton" runat="server" Text="Remove" /> 
       </asp:Panel>

       <asp:Panel ID="FactorPanel" runat="server" Width="520px" Height="250px" style="background-color : whitesmoke;">
           <h3> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; <asp:Label ID="Label1" runat="server" Text="Mitigating Factor"></asp:Label>  </h3>
           <table>
                <tr>
                   <td>
                       &nbsp; Mitigatin Factor ID&nbsp;</td>
                   <td>
                       <asp:DropDownList ID="AddFactorDropDownList" runat="server" Width="332px" AutoPostBack="True" Height="19px" OnSelectedIndexChanged="AddFactorDropDownList_SelectedIndexChanged" ></asp:DropDownList>
                   </td>
               </tr>
               <tr>
                   <td>
                       <asp:Label ID="Label5" runat="server" Text="Description" Width="100px"></asp:Label>
                   </td>
                   <td>
                       <textarea runat="server" id="FactoDescTextarea" style="width: 333px; height: 79px;"></textarea>
                   </td>
               </tr>

               <tr>
                <td>
                    
                    Subject References

                </td>
                <td>
                    <asp:DropDownList ID="RefDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="Button4" runat="server" Width="155px" Text="Add/Edit Reference" OnClick="SusbjectReferenceButton_Click"  />
                </td>
            </tr>

               </table>
           <br />
              &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<asp:Button ID="SaveFactorButton" runat="server" Text="Save" OnClick="SaveFactorButton_Click" />  &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="DeleteFactorButton" runat="server" Text="Delete" />
       </asp:Panel>

       <asp:Panel ID="AddRemediationPanel" runat="server" Width="430px" Height="420px" style="background-color : whitesmoke;">
           <h3> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; <asp:Label ID="A1Label" runat="server" Text="Remediation"></asp:Label>  </h3>
           <table>
                <tr>
                   <td>
                       &nbsp; ID&nbsp;</td>
                   <td>
                       <asp:DropDownList ID="AddRemediationDropDownList" runat="server" Width="250px" AutoPostBack="True" OnSelectedIndexChanged="AddRemediationDropDownList_SelectedIndexChanged" ></asp:DropDownList>
                   </td>
               </tr>

               <tr>
                   <td>
                       &nbsp;<asp:Label ID="A2Label" runat="server" Text="Title"></asp:Label> &nbsp;</td>
                   <td>
                       <asp:TextBox ID="TitleRemediationTextBox" runat="server" Width="245px"></asp:TextBox>
                   </td>
               </tr>
                <tr>
                <td>
                    
                    Type

                </td>
                <td>
                    <asp:DropDownList ID="RemedTypeDropDownList" runat="server" Width="250px">
                        <asp:ListItem Value="recommendation">Recommended</asp:ListItem>
                        <asp:ListItem Value="planned">Planned</asp:ListItem>
                    </asp:DropDownList> 
                </td>
            </tr>
               <tr>
                   <td>
                       <asp:Label ID="A4Label" runat="server" Text="Description" Width="100px"></asp:Label>
                   </td>
                   <td>
                       <textarea runat="server" id="RemedDescTextArea" rows="3" style="width: 250px"></textarea>
                   </td>
               </tr>
                <tr>
                <td>
                    Properties</td>
                <td>
                    <asp:DropDownList ID="RemedPropDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="RemedPropButton" runat="server" Width="141px" Text="Add/Edit Property" OnClick="RemedPropButton_Click" />
                </td>
            </tr>
             <tr>
                <td>
                    Annotations</td>
                <td>
                    <asp:DropDownList ID="RemedAnnDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="RemedAnnButton" runat="server" Width="141px" Text="Add/Edit Annotation" OnClick="RemedAnnButton_Click"  />
                </td>
            </tr>
            <tr>
                <td>                   
                    Origin
                </td>
                <td>
                    <asp:DropDownList ID="OriginDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="OriginButton" runat="server" Width="141px" Text="Add/Edit Origin" OnClick="OriginButton_Click"  />
                </td>
            </tr>

            <tr>
                <td>                   
                    Required
                </td>
                <td>
                    <asp:DropDownList ID="RequiredDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="RequiredButton" runat="server" Width="141px" Text="Add/Edit Required"  />
                </td>
            </tr>
            
            <tr>
                <td>                   
                    Schedule
                </td>
                <td>
                    <asp:DropDownList ID="ScheduleDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="ScheduleButton" runat="server" Width="141px" Text="Add/Edit Schedule"  />
                </td>
            </tr>

               <tr>
                   <td>
                       <asp:Label ID="Label10" runat="server" Text="Remarks" Width="100px"></asp:Label>
                   </td>
                   <td>
                       <textarea runat="server" id="RemedRemarkTextArea" rows="3" style="width: 250px"></textarea>
                   </td>
               </tr>
           </table>
           <br />
              &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<asp:Button ID="SaveTestStepButton" runat="server" Text="Save" OnClick="SaveTestStepButton_Click" />  &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="DeleteFunctionButton" runat="server" Text="Delete" />
       </asp:Panel>


   </div>
   


   </div>
   </div>
   </div>

</asp:Content>
