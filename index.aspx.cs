
using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ValueAddDemo
{
    public partial class index : System.Web.UI.Page
    {       

        protected void Page_Load(object sender, EventArgs e)
        {
            //If not authenticated, redirect the user back to login page
            if (Session["authenticated"] == null)
            {
                Logger.LogMessage("User not authneticated, redirected to LogIn page from index page");
                Response.Redirect("/login.aspx");
            }
        }

        protected void LogoutButton_Click(object sender, EventArgs e)
        {
            Session["authenticated"] = null;
            Logger.LogMessage("Successfully Logged Out from IA Tool by User--" + Session["LoginUser"].ToString());
            Response.Redirect("~/login.aspx");
        }

        
      
       


        
    }
}