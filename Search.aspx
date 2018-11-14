<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="ValueAddDemo.Search" MaintainScrollPositionOnPostback="true" EnableViewState="true"  %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head>
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="title" content="Impact Assessment Search Page" />
    <meta name="description" content="Impact Assessment Search" />
    <!-- Include Twitter Bootstrap and jQuery: -->
    <%--    <link href="scripts/css/bootstrap.min.css" rel="stylesheet" type="text/css" />--%>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
    <script type="text/javascript" src="scripts/js/jquery.min.js"></script>
    <script type="text/javascript" src="scripts/js/bootstrap.min.js"></script>
    <script src="scripts/js/jquery.cookie.js"></script>
    <!-- Custom Fonts -->
    <script type="text/javascript" src="scripts/js/bootstrap-multiselect.js"></script>
    <link rel="stylesheet" href="scripts/css/bootstrap-multiselect.css" type="text/css" />
    <link href="scripts/css/style.css" rel="stylesheet" type="text/css" />


    <title>ImpactAssessmentTool Search Page</title>

    <script type="text/javascript">
        $(document).ready(function () {
            $(".main_container").css("height", ($(window).height()));
            var header_height = $(".bg_header").height();
            var footer_height = $(".footer_section").height();
            $(".bgBack").css("height", ($(window).height()) - (header_height + footer_height));


            $('#lstFolders').multiselect({
                enableFiltering: true,
                includeSelectAllOption: true,
                maxHeight: 200,
                buttonWidth: '200px',
                numberDisplayed: 1,
                buttonClass: 'btn ddn_color dropdown-toggle',
                enableCaseInsensitiveFiltering: true
            });
            $('#schemaDrpdwnLst').multiselect({
                enableFiltering: true,
                includeSelectAllOption: true,
                maxHeight: 200,
                buttonWidth: '200px',
                numberDisplayed: 1,
                buttonClass: 'btn ddn_color dropdown-toggle',
                enableCaseInsensitiveFiltering: true
            });
            $('#dirDrpdwnList').multiselect({
                enableFiltering: true,
                includeSelectAllOption: true,
                maxHeight: 200,
                buttonWidth: '200px',
                numberDisplayed: 1,
                buttonClass: 'btn ddn_color dropdown-toggle',
                enableCaseInsensitiveFiltering: true
            });

            $("#ddl4").attr("runat", "server");

            var select = $('#ddl4');

            var selected = $.cookie('platformValue');
            if (selected == 'SQL') {
                $("#ddl4").empty();
                $("#ddl4").removeAttr("disabled");
                $("#Txt2").removeAttr("disabled");
                select.append(
                    $('<option style="background-color:white" ></option>').val("0").html("Please Select Context")
                );
                select.append(
                    $('<option style="background-color:white" ></option>').val("Table").html("Table")
                );
                select.append(
                    $('<option style="background-color:white" ></option>').val("Column").html("Column")
                );
                select.append(
                   $('<option style="background-color:white" ></option>').val("View").html("View")
               );
                select.append(
                   $('<option style="background-color:white" ></option>').val("Routine").html("Routine")
               );
              //  select.append(
              //     $('<option style="background-color:white" ></option>').val("Index").html("Index")
              // );
              //  select.append(
              //     $('<option style="background-color:white" ></option>').val("Trigger").html("Trigger")
              // );
              //  select.append(
              //     $('<option style="background-color:white" ></option>').val("Constraint").html("Constraint")
              // );
              //  select.append(
              //    $('<option style="background-color:white" ></option>').val("Sequence").html("Sequence")
              //);
              //  select.append(
              //    $('<option style="background-color:white" ></option>').val("Synonym").html("Synonym")
              //);
              //  select.append(
              //    $('<option style="background-color:white" ></option>').val("MaterializedView").html("Materialized View")
              //);
              //  select.append(
              //     $('<option style="background-color:white" ></option>').val("DatabaseLink").html("Database Link")
              // );
            }
            else if (selected == 'Informatica') {
                $("#ddl4").empty();
                $("#ddl4").removeAttr("disabled");
                $("#Txt2").removeAttr("disabled");
                select.append(
                    $('<option style="background-color:white" ></option>').val("0").html("Please Select Context")
                );
              //  select.append(
              //    $('<option style="background-color:white" ></option>').val("Table").html("Table")
              //);
                select.append(
                  $('<option style="background-color:white" ></option>').val("Connection").html("Connection")
              );
                //  select.append(
                //    $('<option style="background-color:white" ></option>').val("Column").html("Column")
                //);
                select.append(
                  $('<option style="background-color:white" ></option>').val("Keyword").html("Keyword")
              );
                select.append(
                  $('<option style="background-color:white" ></option>').val("Function").html("Function")
              );
            }
            else if (selected == 'Linux') {
                $("#ddl4").removeAttr("disabled");
                $("#Txt2").removeAttr("disabled");
            }
            else if (selected == 'Github') {                
                $("#Txt2").removeAttr("disabled");
            }
            else {
                $("#ddl4").attr("disabled", true);
                $("#Txt2").attr("disabled", true);
            }
        });

        function showprogress() {
            $("#progress").css("display", "block");
        }

        function hideprogress() {
            $("#progress").css("display", "none");
        }
        function emailClick() {
            var txt;
            var person = prompt("Please enter your Email address:", "example@network.lilly.com");
            if (person != null && person != "") {
                $(".hiddenNameField").val(person);
            }
            else { $(".hiddenNameField").val(""); }

        }
        function openModal() {
            $('#myModal').modal('show');
        }
        function openAuthenticationModal() {
            $('#authenticationPopup').modal('show');
        }
        function openInformaticaAuthenticationModal() {
            $('#informaticaAuthenticationPopup').modal('show');
        }
        
        $(document).ready(function () {
            $('#Txt2').keypress(function (e) {
                if (e.keyCode == 13) {
                    $('#btnSearch').click();
                    return false;
                }
            });
        });
        $(document).ready(function () {
            $('#Password').keypress(function (e) {
                if (e.keyCode == 13) {
                    $('#btnSignIn').click();
                    return false;
                }
            });
        });        
        $(document).ready(function () {
            $('#directoryText').keypress(function (e) {
                if (e.keyCode == 13) {
                    $('#btnSearch').click();
                    return false;
                }
            });
            if ($(".contextValue").val() != "") {

                $("#ddl4").val($(".contextValue").val());
            }
            if ($(".dbTypeValue").val() != "") {
                if ($(".dbTypeValue").val() == "SQL") {
                    $("#ddl4 option[value='Sequence']").remove();
                    $("#ddl4 option[value='Synonym']").remove();
                    $("#ddl4 option[value='MaterializedView']").remove();
                }
            }

        });
        
    </script>
</head>

<body onload="javascript:hideprogress()">
    <form id="form1" runat="server" width="1000px">
         <div class="loading" id="progress">
    </div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <input type="hidden" id="hiddenNameField" class="hiddenNameField" runat="server" value="" />
        <input type="hidden" name="contextValue" id="contextValue" class="contextValue" runat="server"/>
        <input type="hidden" name="dbTypeValue" id="dbTypeValue" class="dbTypeValue" runat="server"/>
        <input type="hidden" id="hdnPreviousDBValue" name="hdnPreviousDBValue" runat="server"/>
        <div class="main_container">
            <div class="container-fluid bg_header">
                <!-- Brand and toggle get grouped for better mobile display -->
                <div class="navbar-header">

                    <img src="image/ic_logo_tata.svg" alt="tcs-logo" class="tcs_logo img-responsive">
                </div>

                <!-- Collect the nav links, forms, and other content for toggling -->
                <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
                    <img src="/scripts/image/Impact_logo.png" alt="site-logo" class="impact_logo img-responsive pull-right">
                </div>
                <!-- /.navbar-collapse -->
            </div>
            <div class="container-fluid bgBack">
                <div class="row">
                    
                    <a href="/index.aspx">
                        <img class="back-img" src="/image/return-button.png" alt="Return button">
                    </a>
                    <span class="logout">
                        <asp:Button ID="LogoutButton" runat="server" Text="Logout" OnClick="LogoutButton_Click" />
                        <img src="image/logout.png" alt="Logout">
                    </span>
                </div>
                <div class="row">
                    <!--************************************-->
                    <div class="index-secttion col-xs-offset-1 col-sm-9 col-lg-offset-3 col-lg-6 col-md-offset-1 col-md-9" id="search">
                        <div class="row">
                            <asp:Label ID="dbDrpdnHeading" runat="server" Class="col-sm-6 headingLabel" runat="server" Style="display: none;">Select Database
                                            <span class="mandatory">*</span>
                            </asp:Label>
                            <asp:Label ID="dbServerHeading" runat="server" Class="col-sm-6 headingLabel" runat="server" Style="display: none;">Select Server
                                            <span class="mandatory">*</span>
                            </asp:Label>
                            <asp:Label ID="dbProjectHeading" runat="server" Class="col-sm-6 headingLabel" runat="server" Style="display: none;">Select Project
                                            <span class="mandatory">*</span>
                            </asp:Label>
                              <asp:Label ID="dbApplicationHeading" runat="server" Class="col-sm-6 headingLabel" runat="server" Style="display: none;">Select Project
                                            <span class="mandatory">*</span>
                            </asp:Label>
                           <%-- <asp:Label ID="dirHeading" runat="server" Class="col-sm-6 headingLabel" runat="server" Style="display: none;">Enter Directory
                                            <span class="mandatory">*</span>
                            </asp:Label>--%>
                            <asp:Label ID="dbContextHeading" runat="server" Class="col-sm-6 headingLabel" runat="server" Style="display: none;">Select Context
                                            <span class="mandatory">*</span>
                            </asp:Label>
                            <asp:Label ID="dbRepositoryHeading" runat="server" Class="col-sm-6 headingLabel" runat="server" Style="display: none;">Select Repository
                                            <span class="mandatory">*</span>
                            </asp:Label>
                        </div>
                        <!--Info buttons with dropdown menu-->
                        <div class="buttons row">
                            <div class="btn-group col-sm-6 dropButtons" id="dbDrpdwnDiv" runat="server" style="display: none;">
                                <asp:DropDownList AutoPostBack="true" ID="dbDropDwnLst" runat="server" class="btn btn-info dropdown-toggle" OnSelectedIndexChanged="lstFolders_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                            <!--Info buttons with dropdown menu-->

                            <div class="btn-group col-sm-6 dropButtons" id="projectDrpdwnDiv" runat="server" style="display: none;">
                                <select class="btn width ddn_color dropdown-toggle" id="lstFolders" multiple="true" runat="server">
                                </select>

                            </div>
                            <div class="btn-group col-sm-6 dropButtons" id="contextDiv" runat="server" style="display: none;">
                                <select class="btn dropdown-toggle ddn_bg  width" id="ddl4" runat="server" data-toggle="dropdown">
                                    <option style="background-color: white" value="0">Please Select Context</option>
                                    <option style="background-color: white" value="Connection">Connection</option>
                                    <option style="background-color: white" value="Table">Table</option>
                                    <option style="background-color: white" value="Column">Column</option>
                                    <option style="background-color: white" value="Procedure">Procedure</option>
                                    <option style="background-color: white" value="Keyword">Keyword</option>                                    
                                    <option style="background-color: white" value="Routine">Routine</option>
                                    <option style="background-color: white" value="View">View</option>
                                    <option style="background-color: white" value="Constraint">Constraint</option>
                                    <option style="background-color: white" value="Trigger">Trigger</option>
                                    <option style="background-color: white" value="Index">Index</option>
                                    <option style="background-color: white" value="Sequence">Sequence</option>
                                     <option style="background-color: white" value="Synonym">Synonym</option>
                                    <option style="background-color: white" value="DatabaseLink">Database Link</option>                                   
                                    <option style="background-color: white" value="MaterializedView">Materialized View</option>
                                     <option style="background-color: white" value="Function">Function</option>
                                </select>

                            </div>
                            <div class="btn-group col-sm-6 dropButtons" id="serverDrpdwnDiv" runat="server" style="display: none;">
                                <asp:DropDownList ID="serverDropDwnLst" runat="server" class="btn dropdown-toggle ddn_bg" AutoPostBack="true" OnSelectedIndexChanged="lstServer_SelectedIndexChanged"></asp:DropDownList>

                            </div>
                             <div class="btn-group col-sm-6 dropButtons" id="appDrpdwnDiv" runat="server" style="display: none;">
                                <asp:DropDownList ID="appDropDwnLst" runat="server" class="btn dropdown-toggle ddn_bg" AutoPostBack="true" OnSelectedIndexChanged="listApp_SelectedIndexChanged"></asp:DropDownList>

                            </div>
                            <div class="btn-group col-sm-6 dropButtons" id="repoDrpdwnDiv" runat="server" style="display: none;">
                                <asp:DropDownList ID="repoDropDownList" runat="server" class="btn btn-info dropdown-toggle">
                                </asp:DropDownList>
                            </div>
                            <%--<div class="btn-group col-sm-6 dropButtons" id="directoryDiv" runat="server" style="display: none;">
                                <input class="keyword form-control" runat="server" placeholder="directory path" id="directoryText" type="text" />
                            </div>--%>
                            <div class="row marginTop12">
                                <asp:Label ID="dbSchemaHeading" runat="server" Class="col-sm-6 headingLabel" runat="server" Style="display: none;">Select Schema
                                            <span class="mandatory">*</span>
                                </asp:Label>
                                 <asp:Label ID="dirHeading" runat="server" Class="col-sm-6 headingLabel" runat="server" Style="display: none;">Select Path
                                            <span class="mandatory">*</span>
                            </asp:Label>
                                <asp:Label ID="keywordHeading" runat="server" Class="col-sm-6 headingLabel" runat="server" Style="display: none;">Enter Keyword                                            
                                </asp:Label>
                            </div>
                            <div class="btn-group col-sm-6 dropButtons" id="schemaDrpdwnDiv" runat="server" style="display: none;">
                                <select class="btn width ddn_color dropdown-toggle" id="schemaDrpdwnLst" multiple="true" runat="server">
                                </select>
                            </div>
                            <div class="btn-group col-sm-6 dropButtons" id="directoryDiv" runat="server" style="display: none;">
                                <select class="btn width ddn_color dropdown-toggle" id="dirDrpdwnList" multiple="true" runat="server">
                                </select>                               
                            </div>
                            <div class="btn-group col-sm-6 dropButtons">
                                <input class="keyword form-control" runat="server" id="Txt2" type="text" placeholder="Search text">
                            </div>
                            <asp:Button ID="btnSearch" Text="Search" runat="server" OnClientClick="showprogress();" OnClick="btnSearch_Click" class="btn btn_style btn_bg_blue btn_common" />

                        </div>                        
                        <asp:Label ID="noResultsFound" runat="server" Text="No records found for the search criteria" Style="font-weight: bold; display: none;"></asp:Label>
                    </div>
                    <!--**********************************************-->
                </div>


            </div>



            <footer class="footer_section" id="contact">
                <div class="container">
                </div>
                <div class="container">
                    <div class="footer_bottom">
                       <%-- <a class="help-text" href="http://vm-acellywd-038:52052/webform2.aspx">HELP</a>
                        <span>© All rights reserved by TATA CONSULTANCY SERVICES LTD</span>--%>
                        <div class="credits"></div>
                    </div>
                </div>
            </footer>
        </div>

        <!-- Modal -->

        <div class="modal fade" id="myModal" role="dialog">
            <div class="modal-dialog">

                <!-- Modal content-->
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                        <h4 class="modal-title">Impacted Components Detail</h4>
                    </div>
                    <div class="modal-body">

                        <div class="row" runat="server" id="GridviewDiv" style="display: none;">
                            <div class="col-xs-10 col-xs-offset-1 bg_custom">
                                <div class="row table-responsive" style="text-align: center">
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>

                                            <asp:GridView ID="informaticaGridview" Style="border: 1px; display: none" UseAccessibleHeader="true" CssClass="mydatagrid"
                                                PagerStyle-CssClass="pager" HeaderStyle-CssClass="header" RowStyle-CssClass="rows"
                                                BorderColor="Black" runat="server" AllowSorting="True" CellPadding="3" AllowPaging="True"
                                                PageSize="10" OnSorting="informaticaGridview_Sorting" OnPageIndexChanging="informaticaGridview_PageIndexChanging"
                                                GridLines="Vertical" AutoGenerateColumns="true" EmptyDataText="No records found for selected criteria">
                                                <AlternatingRowStyle BackColor="LightGray" ForeColor="#284775" />
                                                <EditRowStyle BackColor="#999999" Width="100%" />
                                                <FooterStyle BackColor="#286090" Font-Bold="True" ForeColor="White" />
                                                <HeaderStyle BackColor="#286090" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" />
                                                <PagerStyle BackColor="#286090" CssClass="footer-style" ForeColor="White" HorizontalAlign="Center" />
                                                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" />
                                                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                                <SortedAscendingCellStyle BackColor="#E9E7E2" />
                                                <SortedAscendingHeaderStyle BackColor="#506C8C" />
                                                <SortedDescendingCellStyle BackColor="#FFFDF8" />
                                                <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                                               <%-- <Columns>
                                                    <asp:BoundField HeaderStyle-Width="20%" HeaderStyle-HorizontalAlign="Center" ReadOnly="True" HeaderText="Folder" InsertVisible="False"
                                                        DataField="Folder" SortExpression="Folder" />
                                                    <asp:BoundField HeaderStyle-Width="20%" ReadOnly="True" HeaderText="Mapping" InsertVisible="False" DataField="Mapping" SortExpression="Mapping" />
                                                    <asp:BoundField HeaderStyle-Width="20%" ReadOnly="True" HeaderText="Transformation Type" InsertVisible="False" DataField="Transformation_Type"
                                                        SortExpression="Transformation_Type" />
                                                    <asp:BoundField HeaderStyle-Width="20%" ReadOnly="True" HeaderText="Transformation Name" InsertVisible="False" DataField="Transformation_Name"
                                                        SortExpression="Transformation_Name" />
                                                    <asp:BoundField HeaderStyle-Width="20%" ReadOnly="True" HeaderText="Attribute Name" InsertVisible="False" DataField="Attribute_Name"
                                                        SortExpression="Attribute_Name" />
                                                </Columns>--%>
                                            </asp:GridView>
                                            <asp:GridView ID="sqlGridview" Style="border: 1px; display: none" UseAccessibleHeader="true" CssClass="mydatagrid" PagerStyle-CssClass="pager"
                                                HeaderStyle-CssClass="header" RowStyle-CssClass="rows" BorderColor="Black" runat="server"
                                                AllowSorting="True" CellPadding="3" AllowPaging="True" PageSize="10" OnSorting="sqlGridview_Sorting"
                                                OnPageIndexChanging="sqlGridview_PageIndexChanging" GridLines="Vertical" AutoGenerateColumns="True"
                                                EmptyDataText="No records found for selected criteria">
                                                <AlternatingRowStyle BackColor="LightGray" ForeColor="#284775" />
                                                <EditRowStyle BackColor="#999999" Width="100%" />
                                                <FooterStyle BackColor="#286090" Font-Bold="True" ForeColor="White" />
                                                <HeaderStyle BackColor="#286090" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" Width="20%" />
                                                <PagerStyle BackColor="#286090" CssClass="footer-style" ForeColor="White" HorizontalAlign="Center" />
                                                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" Width="20%" />
                                                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                                <SortedAscendingCellStyle BackColor="#E9E7E2" />
                                                <SortedAscendingHeaderStyle BackColor="#506C8C" />
                                                <SortedDescendingCellStyle BackColor="#FFFDF8" />
                                                <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                                            </asp:GridView>
                                            <asp:GridView ID="shellGridview" Style="border: 1px; display: none" UseAccessibleHeader="true" CssClass="mydatagrid" PagerStyle-CssClass="pager"
                                                HeaderStyle-CssClass="header" RowStyle-CssClass="rows" BorderColor="Black" runat="server"
                                                AllowSorting="True" CellPadding="3" AllowPaging="True" PageSize="10" OnSorting="shellGridview_Sorting"
                                                OnPageIndexChanging="shellGridview_PageIndexChanging" GridLines="Vertical" AutoGenerateColumns="True"
                                                EmptyDataText="No records found for selected criteria">
                                                <AlternatingRowStyle BackColor="LightGray" ForeColor="#284775" />
                                                <EditRowStyle BackColor="#999999" Width="100%" />
                                                <FooterStyle BackColor="#286090" Font-Bold="True" ForeColor="White" />
                                                <HeaderStyle BackColor="#286090" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" Width="20%" />
                                                <PagerStyle BackColor="#286090" CssClass="footer-style" ForeColor="White" HorizontalAlign="Center" />
                                                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" Width="20%" />
                                                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                                <SortedAscendingCellStyle BackColor="#E9E7E2" />
                                                <SortedAscendingHeaderStyle BackColor="#506C8C" />
                                                <SortedDescendingCellStyle BackColor="#FFFDF8" />
                                                <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                                            </asp:GridView>
                                            <asp:GridView ID="githubGridview" Style="border: 1px; display: none" UseAccessibleHeader="true" CssClass="mydatagrid" PagerStyle-CssClass="pager"
                                                HeaderStyle-CssClass="header" RowStyle-CssClass="rows" BorderColor="Black" runat="server"
                                                AllowSorting="True" CellPadding="3" AllowPaging="True" PageSize="10" OnSorting="githubGridview_Sorting"
                                                OnPageIndexChanging="githubGridview_PageIndexChanging" GridLines="Vertical" AutoGenerateColumns="True"
                                                EmptyDataText="No records found for selected criteria">
                                                <AlternatingRowStyle BackColor="LightGray" ForeColor="#284775" />
                                                <EditRowStyle BackColor="#999999" Width="100%" />
                                                <FooterStyle BackColor="#286090" Font-Bold="True" ForeColor="White" />
                                                <HeaderStyle BackColor="#286090" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" Width="20%" />
                                                <PagerStyle BackColor="#286090" CssClass="footer-style" ForeColor="White" HorizontalAlign="Center" />
                                                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center" Width="20%" />
                                                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                                <SortedAscendingCellStyle BackColor="#E9E7E2" />
                                                <SortedAscendingHeaderStyle BackColor="#506C8C" />
                                                <SortedDescendingCellStyle BackColor="#FFFDF8" />
                                                <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                                            </asp:GridView>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                        </div>
                    </div>


                    <div class="modal-footer">
                        <asp:Button ID="btnEmail" Text="Email" class="btn btn-success" runat="server" OnClick="btnEmail_Click" OnClientClick="emailClick();" />
                        <asp:Button ID="btnExportToExcel" Text="Export To Excel" class="btn btn-warning" runat="server" OnClick="btnExportToExcel_Click" />
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>


        <!-- Modal -->
        <div class="modal fade" id="authenticationPopup" role="dialog">
            <div class="modal-dialog">

                <!-- Modal content-->
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                        <h4 class="modal-title">Platform Login Details</h4>
                    </div>
                    <div class="modal-body">
                        <div class="form-group ">
                            <label class="control-label " for="email">
                                User Id:
                            </label>
                            <div class="input-group">
                                <div class="input-group-addon">
                                    <span class="glyphicon glyphicon-user"></span>
                                </div>
                                <asp:TextBox ID="UserID" class="form-control" runat="server" placeholder="user id" />
                            </div>
                        </div>

                        <div class="form-group ">
                            <label class="control-label " for="email">
                                Password:
                            </label>
                            <div class="input-group">
                                <div class="input-group-addon">
                                    <span class="glyphicon glyphicon-asterisk"></span>
                                </div>
                                <asp:TextBox ID="Password" class="form-control" TextMode="Password" runat="server" placeholder="password" />
                            </div>
                        </div>
                        

                        <asp:Button ID="btnSignIn" Text="Submit" runat="server" class="btn btn_style btn_bg_blue btn_common" OnClientClick="showprogress();"  OnClick="btnSighIn_Click" />
                                            
                    </div>
                </div>
            </div>
        </div>
        <div class="modal fade" id="informaticaAuthenticationPopup" role="dialog">
            <div class="modal-dialog">

                <!-- Modal content-->
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                        <h4 class="modal-title">Server Login Details</h4>
                    </div>
                    <div class="modal-body">
                        <div class="form-group ">
                            <label class="control-label " for="email">
                                User Id:
                            </label>
                            <div class="input-group">
                                <div class="input-group-addon">
                                    <span class="glyphicon glyphicon-user"></span>
                                </div>
                                <asp:TextBox ID="loginID" class="form-control" runat="server" placeholder="user id" />
                            </div>
                        </div>

                        <div class="form-group ">
                            <label class="control-label " for="email">
                                Password:
                            </label>
                            <div class="input-group">
                                <div class="input-group-addon">
                                    <span class="glyphicon glyphicon-asterisk"></span>
                                </div>
                                <asp:TextBox ID="loginPassword" class="form-control" TextMode="Password" runat="server" placeholder="password" />
                            </div>
                        </div>
                        <div class="form-group ">
                            <label class="control-label " for="email">
                                Select Server:
                            </label>
                            <div class="input-group">
                                <asp:DropDownList ID="serverDropdownLst" runat="server" class="btn btn-info dropdown-toggle">
                                </asp:DropDownList>                                
                            </div>
                        </div>                                                

                        <asp:Button ID="loginButton" Text="Submit" runat="server" class="btn btn_style btn_bg_blue btn_common" OnClientClick="showprogress();" OnClick="btnSighIn_Click" />
                                            
                    </div>
                </div>
            </div>
        </div>

    </form>
</body>
</html>
