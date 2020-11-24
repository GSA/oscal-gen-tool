<%@ Page Title="" Language="C#" MasterPageFile="~/SiteSAR.Master" AutoEventWireup="true" CodeBehind="Findings.aspx.cs" Inherits="OSCALGenerator.PagesSAR.Results.Findings" %>
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
                <h2 style="align-content: center"> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Finding</h2>
               <div class="col-md-6">
     
    <asp:Panel ID="AddFindingPanel" runat="server"  Height="670px" style="background-color : whitesmoke;" Width="520px">
        <br />
        <table>
            <tr>
                <td>
                    <asp:Label ID="RoleIDLabel" runat="server" Text=""></asp:Label>
                </td>
                <td>
                   
                    <asp:DropDownList ID="RoleDropDownList" runat="server" Width="344px" AutoPostBack="True" OnSelectedIndexChanged="RoleDropDownList_SelectedIndexChanged"></asp:DropDownList>
                </td>
            </tr>

            

            <tr>
                <td>
                    <asp:Label ID="Label6" runat="server" Text=" Title"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TitleTextBox" runat="server" Width="341px"></asp:TextBox>
                </td>
            </tr>

            
           
            <tr>
                <td>
                    <asp:Label ID="DescriptionLabel" runat="server" Text="Description   "></asp:Label>
                </td>
                <td>
                    <textarea id="DescriptionTextArea" runat="server" style="height: 56px; width: 345px"></textarea>
                </td>
            </tr>

             <tr>
                <td>
                    <asp:Label ID="Label3" runat="server" Text=" Properties   "></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="PropertyDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="PropButton" runat="server" Width="177px" Text="Add/Edit Properties" OnClick="PropButton_Click" />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label4" runat="server" Text=" Annotations   "></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="AnnotationDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="AnnotationButton" runat="server" Width="172px" Text="Add/Edit Annotations" OnClick="AnnotationButton_Click" />
                </td>
            </tr>

            

            <tr>
                <td>
                    <asp:Label ID="Label1" runat="server" Text="date-time-stamp "></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="StartTextBox" runat="server" Width="230px"></asp:TextBox>
                    <asp:Button ID="Date1Button" runat="server" Text="Get Date" OnClick="Date1Button_Click" Width="118px" />
                </td>
            </tr>
            <tr>
               <td> </td>  
               <td>
                  <asp:Calendar ID="StartDateCalendar" runat="server" Visible="false" OnSelectionChanged="StartDateCalendar_SelectionChanged" ></asp:Calendar>
                </td>
             </tr>
            
            <tr>
                <td>
                    &nbsp;Objective Status
                </td>
                <td>
                    <asp:Button ID="ObjectiveButton" runat="server" Text="Fill Objective Status Parameters" OnClick="ObjectiveButton_Click" Width="356px" />
                </td>
            </tr>

            
            <tr>
                <td>
                    <asp:Label ID="Label7" runat="server" Text=" Observations  "></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ObservationDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="AddObservationButton" runat="server" Width="179px" Text="Add/Remove Observation" OnClick="AddObservationButton_Click"/>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label14" runat="server" Text=" Threat  IDs  "></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ThreatIDDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="AddThreatIDButton" runat="server" Width="179px" Text="Add/Remove Threat ID" OnClick="AddThreatIDButton_Click" />
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="Label13" runat="server" Text=" Risks  "></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="RiskDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="AddRiskButton" runat="server" Width="179px" Text="Add/Remove Risk" OnClick="AddRiskButton_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label16" runat="server" Text=" Parties  "></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="PartyDropDownList" runat="server" Width="180px"></asp:DropDownList> <asp:Button ID="AddPartyButton" runat="server" Width="179px" Text="Add/Remove Party" OnClick="AddPartyButton_Click1" />
                </td>
            </tr>
             <tr>
                <td>
                    <asp:Label ID="Label2" runat="server" Text="Remarks    "></asp:Label>
                </td>
                <td>
                    <textarea id="RoleRemarksTextarea" runat="server" style="height: 82px; width: 350px"></textarea>
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
    
       <asp:Panel ID="AddResponsibleRolePanel" runat="server" Width ="316px" Height="70px" style="background-color : whitesmoke;">
         <table>  
             <tr>
                 <td>
                     <asp:Label ID="KeyLabel" runat="server" Width="75px"></asp:Label> 
                 </td>
                 <td>
                     <asp:DropDownList ID="RespRoleDropDownList" runat="server" Width="200px"></asp:DropDownList>
                 </td>
             </tr>
         </table>   
           <br />
            &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;  <asp:Button ID="ResponsibleRoleButton" runat="server" Text="Add" OnClick="SaveResponsibleRoleButton_Click" />
           &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;   <asp:Button ID="RemoveRoleButton" runat="server" Text="Remove" /> 
       </asp:Panel>

       <asp:Panel ID="AddAuthorizedFunctPanel" runat="server" Width="430px" Height="450px" style="background-color : whitesmoke;">
           <h3> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Objective Status</h3>
           <table>
                <tr>
                   <td>
                     
                       <asp:Label ID="AuthLabel" runat="server" Text=""></asp:Label>
                   </td>
                   <td style="width: 334px">
                       <asp:DropDownList ID="ControlIDDropDownList" runat="server" Width="250px" AutoPostBack="True" OnSelectedIndexChanged="ControlIDDropDownList_SelectedIndexChanged" ></asp:DropDownList>
                   </td>
               </tr>

               <tr>
                   <td>
                       
                       <asp:Label ID="SecondAuthLabel" runat="server" Text=""></asp:Label>
                   </td>
                   <td style="width: 334px">
                       <asp:TextBox ID="SecondAuthTextBox" runat="server" Width="250px"></asp:TextBox>
                   </td>
               </tr>
                <tr>
                   <td>
                       <asp:Label ID="ThirdAuthLabel" runat="server" Text=""></asp:Label>

                   </td>
                   <td style="width: 334px">
                       <asp:TextBox ID="ThirdAuthTextBox" runat="server" Width="250px"></asp:TextBox>
                   </td>
               </tr>
               <tr>
                   <td>
                       <asp:Label ID="Label8" runat="server" Text="Description" Width="100px"></asp:Label>
                   </td>
                   <td style="width: 334px">
                       <textarea runat="server" id="AuthDescTextArea" rows="3" style="width: 250px"></textarea>
                   </td>
               </tr>
            <tr>
                <td>
                    <asp:Label ID="Label5" runat="server" Text=" Result System  "></asp:Label>
                </td>
                <td style="width: 334px">
                    <asp:TextBox ID="FourthAuthTextBox" runat="server" Width="247px"></asp:TextBox>
                </td>
            </tr>
             <tr>
                <td>
                    <asp:Label ID="Label12" runat="server" Text=" Result Value  "></asp:Label>
                </td>
                <td style="width: 334px">
                     <asp:TextBox ID="FifthAuthTextBox" runat="server" Width="247px"></asp:TextBox>
                </td>
            </tr>
            
           <tr>
                <td>
                    <asp:Label ID="Label9" runat="server" Text=" Implementation Status System  "></asp:Label>
                </td>
                <td style="width: 334px">
                    <asp:TextBox ID="SixthAuthTextBox" runat="server" Width="247px"></asp:TextBox>
                </td>
            </tr>
             <tr>
                <td>
                    <asp:Label ID="Label11" runat="server" Text=" Implementation Status Value  "></asp:Label>
                </td>
                <td style="width: 334px">
                     <asp:TextBox ID="SeventhAuthTextBox" runat="server" Width="242px"></asp:TextBox>
                </td>
            </tr>


               <tr>
                   <td>
                       <asp:Label ID="Label10" runat="server" Text="Remarks" Width="100px"></asp:Label>
                   </td>
                   <td style="width: 334px">
                       <textarea runat="server" id="RemarkStepTextArea" rows="3" style="width: 250px"></textarea>
                   </td>
               </tr>
           </table>
           <br />
              &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<asp:Button ID="SaveObjStatusButton" runat="server" Text="Save" OnClick="SaveObjStatusButton_Click"   />  &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="DeleteFunctionButton" runat="server" Text="Delete" />
       </asp:Panel>

        <asp:Panel ID="AddAuxPanel" runat="server" Width ="316px" Height="180px" style="background-color : whitesmoke;">
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
           <asp:Label ID="AuxLabel" runat="server" Text=""></asp:Label>
         <br />
           <table>  
             <tr>
                 <td>
                     <asp:Label ID="Label15" runat="server" Width="75px"></asp:Label> 
                 </td>
                 <td>
                     <asp:DropDownList ID="AuxDropDownList" runat="server" Width="200px" OnSelectedIndexChanged="Aux2DropDownList_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                 </td>
             </tr>
                <tr>
                   <td>
                       <asp:Label ID="TypeLabel" runat="server" Text="System"></asp:Label>
                   </td>
                   <td>
                       <asp:TextBox ID="SystemTextBox" runat="server" Width="195px"></asp:TextBox>
                   </td>
                    
               </tr>
               <tr>
                   <td>
                       <asp:Label ID="OtherLabel" runat="server" Text="Uri"></asp:Label>
                   </td>
                   <td>
                       <asp:TextBox ID="UriTextBox" runat="server" Width="195px"></asp:TextBox>
                   </td>
                    
               </tr>
               <tr>
                   <td>
                       <asp:Label ID="Label17" runat="server" Text="Threat"></asp:Label>
                   </td>
                   <td>
                       <asp:TextBox ID="ThreatTextBox" runat="server" Width="195px"></asp:TextBox>
                   </td>
                    
               </tr>

         </table>   
           <br />
            &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  <asp:Button ID="SaveAuxButton" runat="server" Text="Add" OnClick="SaveAuxButton_Click" />
           &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;   <asp:Button ID="Button2" runat="server" Text="Remove" /> 
       </asp:Panel>

   </div>
   


   </div>
   </div>
   </div>
   <%-- <asp:Panel ID="GridviewPanel" runat="server">
         <h2 style="align-content: center"> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Findings</h2>
            
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
         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="EditButton" runat="server" Text="Add/Edit Findings" OnClick="EditButton_Click" />
        &nbsp;&nbsp;
         
    </asp:Panel>--%>

</asp:Content>
