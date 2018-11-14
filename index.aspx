<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="ValueAddDemo.index" %>

<!DOCTYPE html>
<html>

<head>
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="title" content="Impact Assessment Landing page" />
    <meta name="description" content="Impact Assessment Landing Page" />

    <!-- Include Twitter Bootstrap and jQuery: -->
    <%--    <link href="scripts/css/bootstrap.min.css" rel="stylesheet" type="text/css" />--%>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
    <script type="text/javascript" src="scripts/js/jquery.min.js"></script>
    <script type="text/javascript" src="scripts/js/bootstrap.min.js"></script>
    <script src="scripts/js/jquery.cookie.js"></script>
    <!-- Custom Fonts -->
    <link href="scripts/css/style.css" rel="stylesheet" type="text/css" />
    <title>ImpactAssessmentTool Landing Page</title>

    <script type="text/javascript">
        $(document).ready(function () {
            $(".main_container").css("height", ($(window).height()));
            var header_height = $(".bg_header").height();
            var footer_height = $(".footer_section").height();
            $(".bgBack").css("height", ($(window).height()) - (header_height + footer_height));
        });

        function selectPlatform() {
            var selectedPltValue = $('#ddl3').val();
            var selectedEnvtValue = $('#ddl4').val();
            var selected = document.getElementById("ddl3");
            $.cookie('platformValue', selected.value, { expires: 1, path: '/' });
            if (selectedPltValue != "0" && selectedEnvtValue != "0") {
                window.location.assign("/search.aspx");
            }                
            else if (selectedPltValue == "Github")
            {
                $.cookie('envtValue', "Dev", { expires: 1, path: '/' });
                window.location.assign("/search.aspx");
            }

        }
        function selectEnvironment() {
            var selectedPltValue = $('#ddl3').val();
            var selectedEnvtValue = $('#ddl4').val();
            var selected = $('#ddl4 :selected').text();
            $.cookie('envtValue', selected, { expires: 1, path: '/' });
            if (selectedPltValue == "0") {
                alert("Please select a value from platform");
            }
            else if (selectedPltValue != "0" && selectedEnvtValue != "0") {
                window.location.assign("/search.aspx");
            }
        }


    </script>
</head>

<body>
    <form id="form1" runat="server" width="1000px">
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
                <!-- LOGOUT -->
            <div class="row">
                
                <span class="logout">
                    <asp:Button ID="LogoutButton" runat="server" Text="Logout" OnClick="LogoutButton_Click" />               
                <img src="image/logout.png" alt="Logout">
            </span>
            </div>

                <div class="row">
                    <!--************************************-->
                    <div class="index-secttion col-xs-offset-1 col-sm-10 col-lg-offset-3 col-lg-6 col-md-offset-1 col-md-10 " id="index">
                        <div class="row">
                            <label class="col-sm-6  ">Select Platform</label>
                            <label class="col-sm-6">Select Environment</label>
                        </div>
                        <!--Info buttons with dropdown menu-->
                        <div class="buttons row">
                            <div class="btn-group col-sm-6 dropButtons">
                                <select class="btn dropdown-toggle ddn_bg" id="ddl3" runat="server" data-toggle="dropdown" data-default-value="0" onchange="selectPlatform();">
                                    <option style="background-color: white" value="0">Please select Platform</option>
                                    <option style="background-color: white" value="SQL">SQL</option>
                                    <option style="background-color: white" value="Informatica">Informatica</option>
                                    <option style="background-color: white" value="Linux">Linux</option>
                                    <option style="background-color: white" value="Github">Github</option>
                                </select>

                            </div>
                            <!--Info buttons with dropdown menu-->
                            <div class="btn-group col-sm-6 dropButtons">
                                <select class="btn ddn_bg dropdown-toggle" id="ddl4" multiple="false" runat="server" data-default-value="0" onchange="selectEnvironment();">
                                    <option style="background-color: white" value="0">Please select Environment</option>
                                    <option style="background-color: white" value="Dev">Dev</option>
                                    <option style="background-color: white" value="QA">QA</option>
                                    <option style="background-color: white" value="Prod">Prod</option>
                                </select>

                            </div>

                        </div>
                    </div>
                </div>


            </div>


            <footer class="footer_section" id="contact">
                <div class="container">
                </div>
                <div class="container">
                    <div class="footer_bottom">
                        <%--<a class="help-text" href="http://vm-acellywd-038:52052/webform2.aspx">HELP</a>
                        <span>© All rights reserved by TATA CONSULTANCY SERVICES LTD</span>--%>
                        <div class="credits"></div>
                    </div>
                </div>
            </footer>
        </div>


    </form>
</body>

</html>
