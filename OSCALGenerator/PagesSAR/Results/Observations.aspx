<%@ Page Title="" Language="C#" MasterPageFile="~/SiteSAR.Master" AutoEventWireup="true" CodeBehind="Observations.aspx.cs" Inherits="OSCALGenerator.PagesSAR.Results.Observations" %>
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
                <h2 style="align-content: center"> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Observation</h2>
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
                    
                    Observartion Methods

                </td>
                <td>
                    <asp:DropDownList ID="ObsMethodDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="ObsMethodButton" runat="server" Width="141px" Text="Add/Edit Obs Method" OnClick="ObsMethodButton_Click"  />
                </td>
            </tr>

             <tr>
                <td>
                    
                    Observartion Types

                </td>
                <td>
                    <asp:DropDownList ID="ObsTypeDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="ObsTypeButton" runat="server" Width="141px" Text="Add/Edit Obs Type" OnClick="ObsTypeButton_Click"  />
                </td>
            </tr>

             <tr>
                <td>
                    
                    Assessors

                </td>
                <td>
                    <asp:DropDownList ID="AssessorDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="AssessorButton" runat="server" Width="141px" Text="Add/Edit Assessor" OnClick="AssessorButton_Click"  />
                </td>
            </tr>

              <tr>
                <td>
                    
                    Subject References

                </td>
                <td>
                    <asp:DropDownList ID="SubjectReferenceDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="SusbjectReferenceButton" runat="server" Width="141px" Text="Add/Edit Reference" OnClick="SusbjectReferenceButton_Click"  />
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
                    
                    Relevant Evidence

                </td>
                <td>
                    <asp:DropDownList ID="RelevantEvidenceDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="EvidenceButton" runat="server" Width="141px" Text="Add/Edit Evidence" OnClick="EvidenceButton_Click"  />
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
    
       <asp:Panel ID="AddAuxPanel" runat="server" Width ="316px" Height="150px" style="background-color : whitesmoke;">
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
           <asp:Label ID="AuxLabel" runat="server" Text=""></asp:Label>
         <br />
           <table>  
             <tr>
                 <td>
                     <asp:Label ID="KeyLabel" runat="server" Width="75px"></asp:Label> 
                 </td>
                 <td>
                     <asp:DropDownList ID="AuxDropDownList" runat="server" Width="200px"></asp:DropDownList>
                 </td>
             </tr>
                <tr>
                   <td>
                       <asp:Label ID="TypeLabel" runat="server" Text=""></asp:Label>
                   </td>
                   <td>
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
                       <asp:Label ID="OtherLabel" runat="server" Text=""></asp:Label>
                   </td>
                   <td>
                       <asp:TextBox ID="AuxTextBox" runat="server" Width="195px"></asp:TextBox>
                   </td>
                    
               </tr>

         </table>   
           <br />
            &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  <asp:Button ID="AddAuxButton" runat="server" Text="Add" OnClick="AddAuxButton_Click" />
           &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;   <asp:Button ID="RemoveRoleButton" runat="server" Text="Remove" /> 
       </asp:Panel>

       <asp:Panel ID="AddAuthorizedFunctPanel" runat="server" Width="430px" Height="350px" style="background-color : whitesmoke;">
           <h3> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; Relevant Evidence</h3>
           <table>
                <tr>
                   <td>
                       &nbsp;Evidence ID&nbsp;</td>
                   <td>
                       <asp:DropDownList ID="EvidenceDropDownList" runat="server" Width="250px" AutoPostBack="True" OnSelectedIndexChanged="EvidenceDropDownList_SelectedIndexChanged" ></asp:DropDownList>
                   </td>
               </tr>

               <tr>
                   <td>
                       Href</td>
                   <td>
                       <asp:TextBox ID="HrefTextBox" runat="server" Width="250px"></asp:TextBox>
                   </td>
               </tr>
                
               <tr>
                   <td>
                       <asp:Label ID="Label8" runat="server" Text="Description" Width="100px"></asp:Label>
                   </td>
                   <td>
                       <textarea runat="server" id="EvidenceDescTextArea" rows="3" style="width: 250px"></textarea>
                   </td>
               </tr>
                <tr>
                <td>
                    Properties</td>
                <td>
                    <asp:DropDownList ID="EviPropDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="EvidencePropButton" runat="server" Width="141px" Text="Add/Edit Property" OnClick="EvidencePropButton_Click"  />
                </td>
            </tr>
             <tr>
                <td>
                    Annotations</td>
                <td>
                    <asp:DropDownList ID="EviAnnDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="EvidenceAnnButton" runat="server" Width="141px" Text="Add/Edit Annotation" OnClick="EvidenceAnnButton_Click"  />
                </td>
            </tr>
               <tr>
                   <td>
                       <asp:Label ID="Label10" runat="server" Text="Remarks" Width="100px"></asp:Label>
                   </td>
                   <td>
                       <textarea runat="server" id="EvidenceRemarkTextArea" rows="3" style="width: 250px"></textarea>
                   </td>
               </tr>
           </table>
           <br />
              &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<asp:Button ID="SaveRelevantEvidenceButton" runat="server" Text="Save" OnClick="SaveRelevantEvidenceButton_Click"  />  &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="DeleteFunctionButton" runat="server" Text="Delete" />
       </asp:Panel>


   </div>
   


   </div>
   </div>
   </div>
   

</asp:Content>
