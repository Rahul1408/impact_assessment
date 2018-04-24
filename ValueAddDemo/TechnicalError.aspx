<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TechnicalError.aspx.cs" Inherits="ValueAddDemo.TechnicalError" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8"/>
    <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
    <meta name="viewport" content="width=device-width, initial-scale=1"/>
    <meta name="title" content="Impact Assessment Technical Error Page"/>
        <meta name="description" content="Impact Assessment Technical Error Page"/>

     <!-- Include Twitter Bootstrap and jQuery: -->
<%--    <link href="scripts/css/bootstrap.min.css" rel="stylesheet" type="text/css" />--%>
      <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
    <script type="text/javascript" src="scripts/js/jquery.min.js"></script>
    <script type="text/javascript" src="scripts/js/bootstrap.min.js"></script>      
    <!-- Custom Fonts -->
    <link href="scripts/css/style.css" rel="stylesheet" type="text/css" />        
    <title>ImpactAssessmentTool Technical Error Page</title>
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
                <div class="row error-image">
                    <div class="col-xs-10 col-xs-offset-1 bg_custom">
                        <div class="row" id="try_again">
                            <h2><a href="search.aspx">Click here</a> to go to Home page.</h2>
                        </div>
                    </div>
                </div>
            </div>        
              
            <footer class="footer_section" id="contact">
                <div class="container">
                </div>
                <div class="container">
                    <div class="footer_bottom">
                        <span>© All rights reserved by TATA CONSULTANCY SERVICES LTD</span>
                        <div class="credits"></div>
                    </div>
                </div>
            </footer>
        </div>


    </form>
    
        
    
</body>
</html>
