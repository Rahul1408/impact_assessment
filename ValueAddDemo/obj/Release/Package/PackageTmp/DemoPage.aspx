<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DemoPage.aspx.cs" Inherits="ValueAddDemo.DemoPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="assets/css/bootstrap-theme.min.css" rel="stylesheet" />    
    <link href="assets/css/bootstrap.min.css" rel="stylesheet" />
    <style>
    #theme-content{padding-bottom:75px;padding-bottom:10px;}
.theme-dropdown .dropdown-menu {
  position: static;
  display: block;
  margin-bottom: 20px;
  background-color: burlywood;
}

.theme-showcase > p > .btn {
  margin: 5px 0;
}

.theme-showcase .navbar .container {
  width: auto;
}
.form-gccp::after, .form-gccp::before {
    content: " ";
    display: table; clear:both;
}

.form-gccp ,.form-gccp .form-control, .form-gccp-nav .form-control{font-size:13px !important; line-height:15px; padding:0px 5px; 
}
    .form-gccp .form-control, .form-gccp-nav .form-control {height:25px;
    }
    .form-gccp .radio {
    margin-bottom: 3px;   
}
     .form-gccp .btn {
    margin-top:15px;
            height: 24px;
            width: 106px;
        }

    .form-gccp .col-xs-1 {
    width: 10.7%; padding-left:5px; padding-right:5px;
}

    .form-gccp .col-xs-2 {
    width: 14.3%;padding-left:5px; padding-right:5px;
}
    </style>
</head>
<body style="height: 383px">
    <form id="form1" runat="server"> 
        <div style="padding-left:45px" class="form-gccp">
            <div id="theme-content">
                <div style="min-height: 550px;">
                    <div class="container theme-showcase" role="main" style="width:100%">
                        <div class="page-header">
                            <div style="padding-left:4px" class="form-gccp">
                                <div class="col-xs-1 text-left">
                                    <div class="form-group form-group-sm">
                                        <table style="width:1000px">  
                                            <tr>
                                                <td style="width:250px"></td>
                                                <td colspan="2"  style="width:500px; padding-left:100px">
                                                    <select class="form-control" id="ddl1" style="width:200px;">
                                                        <option value="">Please select an application</option>
                                                        <option value="GCO">GCO</option>
                                                        <option value="GCCP">GCCP</option>
                                                    </select>
                                                </td>
                                                <td  style="width:250px"></td>
                                            </tr>                                          
                                            <tr>
                                                <td class="radio" style="width:250px">
                                                    <input type="radio" name="optradio" id="RdSmartSearch" checked="checked"/><label id="lblrd1" data-label="SMART_SEARCH" style="padding-left:0px !important">Smart Search</label>
                                                </td>
                                                <td colspan="2"  style="width:500px">
                                                    <select class="form-control" id="ddl2" style="width:400px;">
                                                        <option value="">Please select a value</option>
                                                        <option value="Mapping">Mapping</option>
                                                        <option value="Folder">Folder</option>
                                                    </select>
                                                </td>
                                                <td  style="width:250px">
                                                    <input class="form-control" name="Name" id="Txt1" />                                                
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="radio"  style="width:250px">                              
                                                    <input type="radio" name="optradio" id="RdObjectSearch"/><label id="lblrd2" data-label="OBJECT_SEARCH" style="padding-left:0px !important">Object Search</label>
                                                </td>
                                                <td  style="width:250px">
                                                    <select class="form-control" id="ddl3">
                                                        <option value="">Please select a value</option>
                                                        <option value="SQL">SQL</option>
                                                        <option value="Informatica">Informatica</option>
                                                        <option value="Shell">Shell</option>
                                                    </select>
                                                </td>
                                                <td  style="width:250px">
                                                    <select class="form-control" id="ddl3">
                                                        <option value="">Please select a value</option>
                                                        <option value="Connection">Connection</option>
                                                        <option value="Table">Table</option>
                                                        <option value="Column">Column</option>
                                                        <option value="Query">Query</option>
                                                        <option value="Referential integrity">Referential integrity</option>
                                                    </select>
                                                </td>
                                                <td  style="width:250px">
                                                    <input class="form-control" name="Name" id="Txt2" />
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                                <div class="col-xs-1" style="width:75% !important">
                                    <div class="form-group" align="center">                                       
                                        <button type="button" class="btn btn-sm btn-primary" onclick="SearchData()"><label data-label="SEARCH">Search </label></button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div>
                    <div class="over-f-auto">
                        <div class="table-responsive">
                            <asp:GridView ID="GridView1" Class="table table-striped table-bordered table-hover table-condensed" runat="server" AllowSorting="True" CellPadding="4" ForeColor="#333333" GridLines="None" AutoGenerateColumns="False" ShowHeaderWhenEmpty="false" EmptyDataText="No records found for selected criteria">
                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                <EditRowStyle BackColor="#999999" />
                                <FooterStyle BackColor="#286090" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#286090" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#fff" CssClass="footer-style" ForeColor="White" HorizontalAlign="Center" />
                                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                <SortedAscendingCellStyle BackColor="#E9E7E2" />
                                <SortedAscendingHeaderStyle BackColor="#506C8C" />
                                <SortedDescendingCellStyle BackColor="#FFFDF8" />
                                <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                                <Columns>
                                    <asp:BoundField ReadOnly="True" HeaderText="Folder Name"
                                        InsertVisible="False" 
                                        SortExpression="FolderName" />                                    
                                    <asp:BoundField ReadOnly="True" HeaderText="Mapping Name"
                                        InsertVisible="False" 
                                        SortExpression="MappingName" />
                                    <asp:BoundField ReadOnly="True" HeaderText="Transformation Type"
                                        InsertVisible="False" 
                                        SortExpression="TransformationType" />
                                    <asp:BoundField ReadOnly="True" HeaderText="Transformation Name"
                                        InsertVisible="False" 
                                        SortExpression="TransformationName" />
                                    <asp:BoundField ReadOnly="True" HeaderText="Attribute Name"
                                        InsertVisible="False" 
                                        SortExpression="AttributeName" />                                    
                                </Columns>
                            </asp:GridView>                                                                                              
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
