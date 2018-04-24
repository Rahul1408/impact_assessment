<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="ValueAddDemo.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="title" content="Impact Assessment Logon page" />
    <meta name="description" content="Impact Assessment Logon page" />

    <!-- Include Twitter Bootstrap and jQuery: -->
<%--    <link href="scripts/css/bootstrap.min.css" rel="stylesheet" type="text/css" />--%>
      <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
    <script type="text/javascript" src="scripts/js/jquery.min.js"></script>
    <script type="text/javascript" src="scripts/js/bootstrap.min.js"></script>      
    <!-- Custom Fonts -->
    <link href="scripts/css/style.css" rel="stylesheet" type="text/css" />        
    <title>ImpactAssessmentTool Logon Page</title>
    <script type="text/javascript">
        $(document).ready(function () {
            $(".main_container").css("height", ($(window).height()));
            var header_height = $(".bg_header").height();
            var footer_height = $(".footer_section").height();
            $(".bgBack").css("height", ($(window).height()) - (header_height + footer_height));            
        });

    </script>

</head>
<body>
    <form id="form1" runat="server">        
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
                
                <div class="login-secttion col-lg-offset-4 col-lg-4 col-md-offset-2 col-md-8 col-sm-8 col-sm-offset-2" id="login">

                    <div class="login-box">
                        <h1>Sign In</h1>
                        <form>
                            <div class="form-group ">
                                <label class="control-label " for="email">
   User Id:
  </label>
                                <div class="input-group">
                                    <div class="input-group-addon">
                                        <span class="glyphicon glyphicon-user"></span>
                                    </div>
                                     <asp:TextBox ID="UserEmail" class="form-control" runat="server" placeholder="user id" />                                    
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
                                    <asp:TextBox ID="UserPass" class="form-control" TextMode="Password" runat="server" placeholder="password" />                                    
                                </div>
                            </div>
                             <asp:Button ID="btnSignIn" OnClick="Login_Click" Text="Submit" runat="server" class="btn btn_style btn_bg_blue btn_common" />
                            
                        </form>
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
