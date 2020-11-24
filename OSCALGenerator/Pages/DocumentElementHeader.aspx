<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DocumentElementHeader.aspx.cs" Inherits="OSCALGenerator.Pages.DocumentHeaderElement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div>  
        <br />
        <asp:Panel ID="MainPanel" runat="server" Height="567px" Width="1082px">

            <asp:Label ID="ElementListLabel" runat="server" Text="Element List" Font-Size="X-Large"></asp:Label>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
            <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" AutoGenerateEditButton="True" CellPadding="4" DataSourceID="SqlDataSource1" ForeColor="#333333" GridLines="None" OnRowUpdating="GridView1_RowUpdating" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" OnRowCreated="GridView1_RowCreated" Width="1055px" OnSelectedIndexChanging="GridView1_SelectedIndexChanging">
                <AlternatingRowStyle BackColor="White" />
                <Columns>
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:BoundField DataField="DEID" HeaderText="DEID" InsertVisible="False" ReadOnly="True" SortExpression="DEID" />
                    <asp:BoundField DataField="DOID" HeaderText="DOID" SortExpression="DOID" />
                    <asp:BoundField DataField="ElementTypeID" HeaderText="ElementTypeID" SortExpression="ElementTypeID" />
                    <asp:BoundField DataField="ElementName" HeaderText="ElementName" SortExpression="ElementName" />
                    <asp:BoundField DataField="ElementTag" HeaderText="ElementTag" SortExpression="ElementTag" />
                    <asp:BoundField DataField="ElementDesc" HeaderText="ElementDesc" SortExpression="ElementDesc" />
                    <asp:BoundField DataField="Active" HeaderText="Active" SortExpression="Active" />
                    <asp:BoundField DataField="CreatedDate" HeaderText="CreatedDate" SortExpression="CreatedDate" />
                    <asp:BoundField DataField="CreatedBy" HeaderText="CreatedBy" SortExpression="CreatedBy" />
                    <asp:BoundField DataField="ModifiedDate" HeaderText="ModifiedDate" SortExpression="ModifiedDate" />
                    <asp:BoundField DataField="ModifiedBy" HeaderText="ModifiedBy" SortExpression="ModifiedBy" />
                </Columns>
                <EditRowStyle BackColor="#2461BF" />
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#EFF3FB" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                <SortedDescendingHeaderStyle BackColor="#4870BE" />
            </asp:GridView>
            <asp:Button ID="AddButton" runat="server" OnClick="AddButton_Click" Text="Add" />
            &nbsp;&nbsp;
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Panel ID="RemovePanel" runat="server" Height="64px" Width="399px" BackColor="#E7E7E7" style="margin-left: 216px">
                &nbsp;&nbsp;&nbsp;&nbsp;<br /> &nbsp;&nbsp;<asp:TextBox ID="TextBox2" runat="server" Width="249px">Do you want to remove this row?</asp:TextBox>
                &nbsp;&nbsp;
                <asp:Button ID="YesButton" runat="server" OnClick="YesButton_Click" Text="Yes" />
                &nbsp;
                <asp:Button ID="NoButton" runat="server" Text="No" OnClick="NoButton_Click" />
            </asp:Panel>
            &nbsp;&nbsp;<asp:Panel ID="Panel1" runat="server" BackColor="#E7E7E7" Height="224px" style="margin-left: 217px" Width="329px">
                &nbsp;<br /> &nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Element Name:"></asp:Label>
                &nbsp;<asp:TextBox ID="EltNameTextbox" runat="server" Width="140px"></asp:TextBox>
                <br />
                &nbsp;&nbsp;<asp:Label ID="Label2" runat="server" Text="Element Type: "></asp:Label>
                &nbsp;&nbsp;
                <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="SqlDataSource2" DataTextField="ElementTypeAbrev" DataValueField="ID" Width="138px">
                </asp:DropDownList>
                <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:OWT_DEVConnectionString %>" SelectCommand="SELECT * FROM [DocElementTypeLU]"></asp:SqlDataSource>
                &nbsp;&nbsp;<br />&nbsp;&nbsp;
                <asp:Label ID="Label3" runat="server" Text="Element Tag:"></asp:Label>
                &nbsp;&nbsp;&nbsp;
                <asp:TextBox ID="EltTagTextbox" runat="server" Width="140px"></asp:TextBox>
                <br />
                &nbsp; Element Description:<br /> &nbsp;<textarea id="EltDescTextArea" runat="server" name="S1" style=" width:auto; height:auto"></textarea><br />
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="SaveButton" runat="server" OnClick="SaveButton_Click" Text="Save" Width="66px" />
                &nbsp;<asp:Button ID="CancelButton" runat="server" Text="Cancel" Width="65px" OnClick="CancelButton_Click" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            </asp:Panel>
            
            <br />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <br />
        </asp:Panel>
        <br />
     &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  
      
  </div>

<%--    <asp:DropDownList ID="ElementList" runat="server" Height="34px" Width="290px">
        <asp:ListItem>Add</asp:ListItem>
        <asp:ListItem>Remove</asp:ListItem>
    </asp:DropDownList>--%>

    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;

    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:OWT_DEVConnectionString %>" SelectCommand="SELECT * FROM [DocElementHeader] Where Active =1" InsertCommand="INSERT INTO [DocElementHeader] ([DOID], [ElementTypeID], [ElementName], [ElementTag], [ElementDesc], [Active], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (@DOID, @ElementTypeID, @ElementName, @ElementTag, @ElementDesc, @Active, @CreatedDate, @CreatedBy, @ModifiedDate, @ModifiedBy)" 
        UpdateCommand="UPDATE DocElementHeader SET ElementTypeID = @ElementTypeID, ElementName = @ElementName, ElementTag = @ElementTag, ElementDesc = @ElementDesc, Active = @Active, ModifiedDate = GETDATE(), ModifiedBy = @UID WHERE (DEID = @DEID)">
        <InsertParameters>
            <asp:Parameter Name="DOID" Type="Int32" />
            <asp:Parameter Name="ElementTypeID" Type="Int32" />
            <asp:Parameter Name="ElementName" Type="String" />
            <asp:Parameter Name="ElementTag" Type="String" />
            <asp:Parameter Name="ElementDesc" Type="String" />
            <asp:Parameter Name="Active" Type="Int32" />
            <asp:Parameter Name="CreatedDate" Type="DateTime" />
            <asp:Parameter Name="CreatedBy" Type="Int32" />
            <asp:Parameter Name="ModifiedDate" Type="DateTime" />
            <asp:Parameter Name="ModifiedBy" Type="Int32" />
            <asp:Parameter Name="DEID" Type="Int32" />
            <asp:Parameter Name="NEWID" Type="Int32" />
            <asp:Parameter Name="UID" Type="Int32" />
        </InsertParameters>

         <UpdateParameters>
            <asp:Parameter Name="DOID" Type="Int32" />
            <asp:Parameter Name="ElementTypeID" Type="Int32" />
            <asp:Parameter Name="ElementName" Type="String" />
            <asp:Parameter Name="ElementTag" Type="String" />
            <asp:Parameter Name="ElementDesc" Type="String" />
            <asp:Parameter Name="Active" Type="Int32" />
            <asp:Parameter Name="CreatedDate" Type="DateTime" />
            <asp:Parameter Name="CreatedBy" Type="Int32" />
            <asp:Parameter Name="ModifiedDate" Type="DateTime" />
            <asp:Parameter Name="ModifiedBy" Type="Int32" />
            <asp:Parameter Name="DEID" Type="Int32" />
            <asp:Parameter Name="NEWID" Type="Int32" />
            <asp:Parameter Name="UID" Type="Int32" />
        </UpdateParameters>



    </asp:SqlDataSource>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <br />
    <asp:Panel ID="ErrorPanel" runat="server">
        <asp:TextBox ID="ErrorTextBox" runat="server" Width="1176px"></asp:TextBox>
    </asp:Panel>
<br />
</asp:Content>
