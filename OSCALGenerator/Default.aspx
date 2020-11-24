<%@ Page Title="Home Page" Language="C#"   Debug="true" MasterPageFile ="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="OSCALGenerator._Default" %>
 
    

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server"  >
 



    <div class="container" id="maindivcon" align="center">
        <asp:Label ID="OrgNameLabel" runat="server" Text=""></asp:Label>
    </div>
     <br />
    <div runat="server" id="SysDiv"  align="center">

        <table style="border: medium groove #C0C0C0; width:867px; padding-left:0px" >
        

        <tr>
            <td style="border: thin solid #C0C0C0">
                  
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;New System Name&nbsp;<asp:TextBox ID="SystemTextBox" runat="server" Width="222px"></asp:TextBox>
                    <br />
                      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;New System ID&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<asp:TextBox ID="SysIDTextBox" runat="server" Width="222px"></asp:TextBox>
                
                  </td>
             <td style="border: thin solid #C0C0C0">
                
                 &nbsp;&nbsp;&nbsp;Select an Existing System &nbsp;  <asp:DropDownList ID="SystemDropDownList" runat="server" Width="280px"></asp:DropDownList>
                
            </td>
         </tr>
          </table>
           <br />
           <br />
        <asp:Button ID="SaveSystemButton" runat="server" Text="Save and Continue " Width="150px" OnClick="SaveSystemButton_Click" />
          


     <br />
    <br />
    <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
       
  </div>



   



 <div runat="server" id="DocDiv"  visible="false" align="center">

  <table style="border-width: medium; border-color: #C0C0C0; border-style: groove;">
        <tr>
            <td style="border: thin solid #C0C0C0">

                   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Generate a New SSP

            </td>
            <td style="border: thin solid #C0C0C0; width: 312px;">
                 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Update an Existing SSP
            </td>

             <td style="border: thin solid #C0C0C0; width: 312px;">
                 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Import an Existing OSCAL Document
            </td>

        </tr>

        <tr>
            <td style="border: thin solid #C0C0C0">
                 <table>
                     <tr>
                         <td> Full Name  </td> <td> <asp:TextBox ID="DocFullNameTextBox" runat="server"></asp:TextBox> </td>
                     </tr>
                     <tr>
                         <td> Short Name  </td> <td>  <asp:TextBox ID="DocShortNameTextBox" runat="server"></asp:TextBox> </td>
                     </tr>
                </table>         
             </td>
             <td style="border: thin solid #C0C0C0; width: 312px;">
                
                 &nbsp;&nbsp;&nbsp;Select File&nbsp;  <asp:DropDownList ID="DocumentDropDownList" runat="server" Width="180px" OnSelectedIndexChanged="DocumentDropDownList_SelectedIndexChanged" ></asp:DropDownList>
                
            </td>
            <td  style="border: thin solid #C0C0C0; width: 312px;">
                   &nbsp;&nbsp;&nbsp;&nbsp;  &nbsp;<asp:FileUpload ID="FileUpload1" Width="250px" runat="server" /></td>

         </tr>
          </table>
     <br />

     <table>
         <tr>
             <td> </td>
         <td>  &nbsp;<asp:Button ID="StartButton" runat="server" Text="Start " OnClick="StartButton_Click" Width="135px" />
             <br />
          
             <br />
          
             </td>
             <td></td>
         </tr>
        <tr>

    <td>
    
    
     <asp:Button ID="OpenFileButton" runat="server" Text="Open File" OnClick="OpenFileButton_Click" ForeColor="#33CC33" />
       &nbsp;&nbsp;&nbsp;  &nbsp;&nbsp;&nbsp;
    </td>
     
             <td> 
    <asp:Button ID="OSCALButton" runat="server" OnClick="OSCALButton_Click" Text="Render OSCAL SSP" Width="147px" /> 
               &nbsp;&nbsp;&nbsp;     
             </td>
           
             <td>   &nbsp;&nbsp;&nbsp; <asp:Button ID="WordDocButton" runat="server" Text="Render Word Doc" Width="147px" OnClick="WordDocButton_Click" Visible="False" />
                 </td>
     
     </tr>
    </table>
        <br />
     <asp:Label ID="StatusLabel" runat="server" Text=""></asp:Label>
   
        </div> 
        
           <p id="realmessage"></p>
           <div class="progress" >
             <div  id ="mainbar" class="progress-bar" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0% ">
                    <span id = "realbar" class="sr-only">0% Complete</span>
             </div>
             <div  runat="server" id ="mainbarServer" class="progress-bar" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0% ">
             
                 <span id = "otherrealbar" class="sr-only">0% Complete</span>
             </div>
           </div>
         

  </asp:Content>
