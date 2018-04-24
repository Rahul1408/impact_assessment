using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ValueAddDemo
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Login_Click(object sender, EventArgs e)
        {
            Type cstype = this.GetType();
            // Get a ClientScriptManager reference from the Page class.
            ClientScriptManager cs = Page.ClientScript;
            //To authenticate the user if credentials are correct
            if (FormsAuthentication.Authenticate(UserEmail.Text, UserPass.Text))
            {
                FormsAuthentication.RedirectFromLoginPage(UserEmail.Text, true);
                Session["authenticated"] = "true";
                Session["LoginUser"] = UserEmail.Text;
                Logger.LogMessage("Successfully LogIn to IA Tool by User--" + UserEmail.Text);
                Response.Redirect("/index.aspx");
            }
            else
            {
                // Check to see if the startup script is already registered.
                if (!cs.IsStartupScriptRegistered(cstype, "loginScript"))
                {
                    String cstext1 = "alert('Authentication Failed!!');";
                    cs.RegisterStartupScript(cstype, "loginScript", cstext1, true);
                }
                Logger.LogMessage("Login to IA tool Failed, UserID entered--"+UserEmail.Text);
            }
        }
       
    }
}