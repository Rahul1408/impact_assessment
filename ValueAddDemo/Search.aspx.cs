using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web.UI;
using System.Web.UI.WebControls;
using Renci.SshNet.Channels;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization.Json;
using System.Reflection;
using System.ComponentModel;



namespace ValueAddDemo
{
    public partial class Search : System.Web.UI.Page
    {
        OracleConnection objConn;
        OracleCommand objCmd;
        SqlConnection sqlConn;
        SqlCommand sqlCmd;
        OleDbConnection oldbCon;
        OleDbCommand oldbCmd;
        string envtValue = string.Empty;
        string platformValue = string.Empty;
        // Define the name and type of the client scripts on the page.
        String csname1 = "ProjectScript";
        String csname2 = "DatabaseScript";
        String csname3 = "EmailScript";
        String csname4 = "ShellScript";
        String csname5 = "SQLScript";
        String csname6 = "RequestOut";
        String userIDTxt = string.Empty;
        String passwordTxt = string.Empty;
        String port = string.Empty;
        String host = string.Empty;
        String SIDName = string.Empty;
        String strSQL = string.Empty;
        String strSearch = string.Empty;
        String folderList = string.Empty;
        String schemaName = string.Empty;
        String selectedDB = string.Empty;
        String selectedServer = string.Empty;
        private string GridViewSortDirection
        {
            get
            {
                return ViewState["SortDirection"] as string ?? "ASC";
            }
            set
            {
                ViewState["SortDirection"] = value;
            }
        }
        private string GridViewSortExpression
        {
            get
            {
                return ViewState["SortExpression"] as string ?? string.Empty;
            }
            set
            {
                ViewState["SortExpression"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["authenticated"] == null)
            {
                Logger.LogMessage("User not authenticated, redirected to LogIn page from Search page");
                Response.Redirect("~/login.aspx");
            }

            else if (Session["authenticated"].ToString() == "true")
            {
                //Logger.LogMessage("Successfully LogIn to IA Tool by User--" + Session["LoginUser"].ToString());
                if (Request.Cookies["platformValue"] != null)
                {
                    platformValue = Request.Cookies["platformValue"].Value;
                }

                if (Request.Cookies["envtValue"] != null)
                {
                    envtValue = Request.Cookies["envtValue"].Value;
                }
                if (!Page.IsPostBack)
                {
                    if (platformValue.ToLower() == "informatica")
                    {
                        Logger.LogMessage("Platform--Informatica");
                        serverDropdownLst.Items.Insert(0, "Please Select Server");
                        BindInformaticaServerList();
                        ScriptManager.RegisterStartupScript(this, typeof(System.Web.UI.Page), "ModalPop4", "openInformaticaAuthenticationModal();", true);
                    }
                    else if (platformValue.ToLower() == "sql")
                    {
                        Logger.LogMessage("Platform---SQL");
                        dbDrpdwnDiv.Style.Add("display", "block");
                        contextDiv.Style.Add("display", "block");
                        dbDrpdnHeading.Style.Add("display", "block");
                        dbSchemaHeading.Style.Add("display", "block");
                        dbContextHeading.Style.Add("display", "block");
                        keywordHeading.Style.Add("display", "block");
                        dbDropDwnLst.Items.Insert(0, "Please Select Database");
                        BindDatabaseList();
                        schemaDrpdwnDiv.Style.Add("display", "block");
                    }
                    else if (platformValue.ToLower() == "linux")
                    {
                        Logger.LogMessage("Platform---Linux");
                        serverDrpdwnDiv.Style.Add("display", "block");
                        dbApplicationHeading.Style.Add("display", "block");
                        dbServerHeading.Style.Add("display", "block");
                        dirHeading.Style.Add("display", "block");
                        keywordHeading.Style.Add("display", "block");
                        appDrpdwnDiv.Style.Add("display", "block");
                        serverDropDwnLst.Items.Insert(0, "Please Select Server");
                        appDropDwnLst.Items.Insert(0, "Please Select Project");
                        BindServerList();
                        directoryDiv.Style.Add("display", "block");
                    }
                    else if (platformValue.ToLower() == "github")
                    {
                        Logger.LogMessage("Platform---Github");
                        dbRepositoryHeading.Style.Add("display", "block");
                        dbApplicationHeading.Style.Add("display", "block");
                        keywordHeading.Style.Add("display", "block");
                        appDrpdwnDiv.Style.Add("display", "block");
                        repoDrpdwnDiv.Style.Add("display", "block");
                        appDropDwnLst.Items.Insert(0, "Please Select Project");
                        BindAppList();
                    }


                }
            }
            if (Session["state"] != null)
            {
                Session["state"] = ddl4.Value.ToString();
                contextValue.Value = Session["state"].ToString();
            }

        }
        /// <summary>
        /// Generic method to get JSON object
        /// </summary>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        public JsonResponse.RootObject ReadAndParseJson(string jsonPath)
        {
            string json = String.Empty;
            DataTable dt = new DataTable();
            string keywordCollection = String.Empty;
            try
            {
                using (System.Net.WebClient webClient = new System.Net.WebClient())
                {
                    json = webClient.DownloadString(System.Web.HttpContext.Current.Server.MapPath("/") + jsonPath);
                }
                System.Web.Script.Serialization.JavaScriptSerializer obj = new System.Web.Script.Serialization.JavaScriptSerializer();
                JsonResponse.RootObject result = obj.Deserialize<JsonResponse.RootObject>(json);
                return result;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Generic method to get previous and current value of database
        /// </summary>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        public int CurrentDatabaseValue(string value)
        {
            string hiddenDBValue = hdnPreviousDBValue.Value;
            string currentDBValue = value;
            int flag = 0;
            if (hiddenDBValue != "")
            {
                if (hiddenDBValue.ToUpper() == currentDBValue.ToUpper())
                    flag = 1;
                else
                {
                    hdnPreviousDBValue.Value = value;
                    flag = 0;
                }
            }
            else
            {
                hdnPreviousDBValue.Value = value;
                flag = 0;
            }
            return flag;
        }
        /// <summary>
        /// To bind database list with dropdown
        /// </summary>
        public void BindDatabaseList()
        {
            int count = 0;
            var result = ReadAndParseJson("SqlPropertyFile.json");
            foreach (var item in result.root.Environment)
            {
                if (item.Name.ToLower() == envtValue.ToLower())
                {
                    foreach (var team in item.Team)
                    {
                        foreach (var db in team.Details)
                        {
                            count++;
                            dbDropDwnLst.Items.Add(new ListItem(db.Database.ToString(), db.Database.ToString()));

                        }
                    }
                    break;
                }

            }

        }
        /// <summary>
        /// To bind server list with dropdown
        /// </summary>
        void BindServerList()
        {
            int count = 0;
            var result = ReadAndParseJson("LinuxPropertyFile.json");
            foreach (var item in result.root.Environment)
            {
                if (item.Name.ToLower() == envtValue.ToLower())
                {
                    foreach (var team in item.Team)
                    {
                        foreach (var db in team.ServerDetails)
                        {
                            count++;
                            serverDropDwnLst.Items.Add(new ListItem(db.Name.ToString(), Convert.ToString(count)));

                        }
                    }
                    break;
                }

            }

        }
        /// <summary>
        /// To bind application list with dropdown-Github
        /// </summary>
        void BindAppList()
        {
            int count = 0;
            var result = ReadAndParseJson("GithubPropertyFile.json");
            foreach (var item in result.root.Environment)
            {
                if (item.Name.ToLower() == envtValue.ToLower())
                {
                    foreach (var team in item.Team)
                    {
                        count++;
                        appDropDwnLst.Items.Add(new ListItem(team.Name.ToString(), team.Name.ToString()));
                    }
                    break;
                }

            }

        }
        /// <summary>
        /// To bind Informatica server lists with dropdown
        /// </summary>
        void BindInformaticaServerList()
        {
            int count = 0;
            var result = ReadAndParseJson("InformaticaPropertyFile.json");
            foreach (var item in result.root.Environment)
            {
                if (item.Name.ToLower() == envtValue.ToLower())
                {
                    foreach (var team in item.Team)
                    {
                        foreach (var server in team.Details)
                        {
                            count++;
                            serverDropdownLst.Items.Add(new ListItem(server.Database.ToString(), Convert.ToString(count)));

                        }
                    }
                    break;
                }

            }

        }
        /// <summary>
        /// To bind schema list with dropdown
        /// </summary>
        /// <param name="selectedKey"></param>
        public void BindSchemaList(string selectedKey)
        {
            int count = 0;
            var result = ReadAndParseJson("SqlPropertyFile.json");
            foreach (var item in result.root.Environment)
            {
                if (item.Name.ToLower() == envtValue.ToLower())
                {
                    foreach (var team in item.Team)
                    {
                        foreach (var db in team.Details)
                        {
                            if (db.Database.ToLower() == selectedKey.ToLower())
                            {
                                foreach (var schema in db.Schema)
                                {
                                    count++;
                                    schemaDrpdwnLst.Items.Add(new ListItem(schema.Name.ToString(), Convert.ToString(count)));
                                }
                                break;
                            }
                        }
                    }
                    break;

                }

            }

        }

        /// <summary>
        /// To bind path list with dropdown
        /// </summary>
        /// <param name="selectedKey"></param>
        public void BindDirPathList(string selectedServer, string selectedApp)
        {
            int count = 0;
            var result = ReadAndParseJson("LinuxPropertyFile.json");
            foreach (var item in result.root.Environment)
            {
                if (item.Name.ToLower() == envtValue.ToLower())
                {
                    foreach (var team in item.Team)
                    {
                        foreach (var server in team.ServerDetails)
                        {
                            if (server.Name.ToLower() == selectedServer.ToLower())
                            {
                                foreach (var app in server.ApplicationDetails)
                                {
                                    if (app.Name.ToLower() == selectedApp.ToLower())
                                    {
                                        foreach (var path in app.Paths)
                                        {
                                            count++;
                                            dirDrpdwnList.Items.Add(new ListItem(path.ToString(), Convert.ToString(count)));
                                        }
                                        break;
                                    }

                                }

                            }
                        }
                    }
                    break;
                }

            }

        }
        /// <summary>
        /// To bind Repo list with dropdown
        /// </summary>
        /// <param name="selectedKey"></param>
        public void BindRepoPathList(string selectedApp)
        {
            int count = 0;
            var result = ReadAndParseJson("GithubPropertyFile.json");
            foreach (var item in result.root.Environment)
            {
                if (item.Name.ToLower() == envtValue.ToLower())
                {
                    foreach (var team in item.Team)
                    {
                        if (team.Name.ToLower() == selectedApp.ToLower())
                        {
                            foreach (var repo in team.RepoDetails)
                            {
                                count++;
                                repoDropDownList.Items.Add(new ListItem(repo.Name.ToString(), repo.Path.ToString()));
                            }
                            break;
                        }
                        
                    }

                }

            }

        }
        /// <summary>
        /// To bind application list with dropdown
        /// </summary>
        /// <param name="selectedKey"></param>
        public void BindApplicationList(string selectedKey)
        {
            int count = 0;
            var result = ReadAndParseJson("LinuxPropertyFile.json");
            foreach (var item in result.root.Environment)
            {
                if (item.Name.ToLower() == envtValue.ToLower())
                {
                    foreach (var team in item.Team)
                    {
                        foreach (var server in team.ServerDetails)
                        {
                            if (server.Name.ToLower() == selectedKey.ToLower())
                            {
                                foreach (var app in server.ApplicationDetails)
                                {
                                    count++;
                                    appDropDwnLst.Items.Add(new ListItem(app.Name.ToString(), Convert.ToString(count)));
                                }
                                break;
                            }
                        }
                    }
                    break;

                }

            }

        }
        /// <summary>
        /// To get the database details for a selected database
        /// </summary>
        /// <param name="selectedDatabase"></param>
        /// <param name="databaseType"></param>
        /// <param name="server"></param>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        public void GetDatabaseDetails(string selectedDatabase, out string databaseType, out string server, out string userID, out string password, out string port, out string databaseValue)
        {
            databaseType = string.Empty;
            userID = string.Empty;
            password = string.Empty;
            server = string.Empty;
            port = string.Empty;
            databaseValue = string.Empty;
            var result = ReadAndParseJson("SqlPropertyFile.json");
            foreach (var item in result.root.Environment)
            {
                if (item.Name.ToLower() == envtValue.ToLower())
                {
                    foreach (var team in item.Team)
                    {
                        foreach (var db in team.Details)
                        {
                            if (db.Database.ToLower() == selectedDatabase.ToLower())
                            {
                                databaseType = db.Type.ToString();
                                //userID = db.UserID.ToString();
                                //password = db.Password.ToString();
                                server = db.Server.ToString();
                                if (databaseType == "Oracle")
                                {
                                    port = db.Port.ToString();
                                }
                                if (databaseType == "SQL")
                                {
                                    ddl4.Items.Remove(ddl4.Items.FindByValue("Sequence"));
                                    ddl4.Items.Remove(ddl4.Items.FindByValue("Synonym"));
                                    ddl4.Items.Remove(ddl4.Items.FindByValue("MaterializedView"));
                                }
                                if (databaseType == "Netezza")
                                {
                                    databaseValue = db.DatabaseValue.ToString();
                                }
                                break;
                            }

                        }
                    }
                    break;
                }

            }

        }
        /// <summary>
        /// To remove droddown items
        /// </summary>
        /// <param name="selectedDatabase"></param>
        /// <param name="databaseType"></param>        
        public void RemoveDrodwnList(string selectedDatabase)
        {
            var result = ReadAndParseJson("SqlPropertyFile.json");
            foreach (var item in result.root.Environment)
            {
                if (item.Name.ToLower() == envtValue.ToLower())
                {
                    foreach (var team in item.Team)
                    {
                        foreach (var db in team.Details)
                        {
                            if (db.Database.ToLower() == selectedDatabase.ToLower())
                            {
                                dbTypeValue.Value = db.Type;
                                break;
                            }

                        }
                    }
                    break;
                }

            }

        }
        /// <summary>
        /// To get the userID for a selected server
        /// </summary>
        /// <param name="selectedDatabase"></param>
        /// <param name="databaseType"></param>
        /// <param name="server"></param>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        public void GetServerDetails(string selectedServer, out string userID, out string teamName)
        {
            userID = string.Empty;
            teamName = string.Empty;
            var result = ReadAndParseJson("LinuxPropertyFile.json");
            foreach (var item in result.root.Environment)
            {
                if (item.Name.ToLower() == envtValue.ToLower())
                {
                    foreach (var team in item.Team)
                    {
                        foreach (var db in team.ServerDetails)
                        {
                            if (db.Name.ToLower() == selectedServer.ToLower())
                            {
                                teamName = team.Name;
                                break;
                            }

                        }
                    }
                    break;
                }

            }

        }
        /// <summary>
        /// To get the informatica server details for a selected server
        /// </summary>
        /// <param name="selectedServer"></param>        
        public void GetInformaticaServerDetails(string selectedServer)
        {
            var result = ReadAndParseJson("InformaticaPropertyFile.json");
            foreach (var item in result.root.Environment)
            {
                if (item.Name.ToLower() == envtValue.ToLower())
                {
                    foreach (var team in item.Team)
                    {
                        foreach (var db in team.Details)
                        {
                            if (db.Database.ToLower() == selectedServer.ToLower())
                            {
                                port = db.Port.ToString();
                                host = db.Server.ToString();
                                foreach (var schema in db.Schema)
                                {
                                    schemaName = schema.Name.ToString();
                                }
                                break;
                            }
                        }
                    }
                    break;
                }

            }

        }
        protected void informaticaGridview_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            DataTable myDataTable = (DataTable)Session["SessionData"];
            informaticaGridview.DataSource = this.SortDataTable(myDataTable, true);
            informaticaGridview.PageIndex = e.NewPageIndex;
            informaticaGridview.DataBind();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop5", "openModal();", true);
        }
        protected void sqlGridview_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            DataTable myDataTable = (DataTable)Session["SessionData1"];
            sqlGridview.DataSource = this.SortDataTable(myDataTable, true);
            sqlGridview.PageIndex = e.NewPageIndex;
            sqlGridview.DataBind();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop6", "openModal();", true);
        }
        protected void shellGridview_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            DataTable myDataTable = (DataTable)Session["SessionData2"];
            shellGridview.DataSource = this.SortDataTable(myDataTable, true);
            shellGridview.PageIndex = e.NewPageIndex;
            shellGridview.DataBind();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop7", "openModal();", true);
        }
        protected void githubGridview_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            DataTable myDataTable = (DataTable)Session["SessionData3"];
            githubGridview.DataSource = this.SortDataTable(myDataTable, true);
            githubGridview.PageIndex = e.NewPageIndex;
            githubGridview.DataBind();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop8", "openModal();", true);
        }
        protected void informaticaGridview_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable myDataTable = (DataTable)Session["SessionData"];
            this.GridViewSortExpression = e.SortExpression;

            int iPageIndex = informaticaGridview.PageIndex;
            informaticaGridview.DataSource = this.SortDataTable(myDataTable, false);
            informaticaGridview.DataBind();
            informaticaGridview.PageIndex = iPageIndex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop8", "openModal();", true);
        }
        protected void sqlGridview_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable myDataTable = (DataTable)Session["SessionData1"];
            this.GridViewSortExpression = e.SortExpression;

            int iPageIndex = sqlGridview.PageIndex;
            sqlGridview.DataSource = this.SortDataTable(myDataTable, false);
            sqlGridview.DataBind();
            sqlGridview.PageIndex = iPageIndex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop9", "openModal();", true);
        }
        protected void shellGridview_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable myDataTable = (DataTable)Session["SessionData2"];
            this.GridViewSortExpression = e.SortExpression;

            int iPageIndex = shellGridview.PageIndex;
            shellGridview.DataSource = this.SortDataTable(myDataTable, false);
            shellGridview.DataBind();
            shellGridview.PageIndex = iPageIndex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop10", "openModal();", true);
        }
        protected void githubGridview_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable myDataTable = (DataTable)Session["SessionData3"];
            this.GridViewSortExpression = e.SortExpression;

            int iPageIndex = shellGridview.PageIndex;
            githubGridview.DataSource = this.SortDataTable(myDataTable, false);
            githubGridview.DataBind();
            githubGridview.PageIndex = iPageIndex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop11", "openModal();", true);
        }

        protected DataView SortDataTable(DataTable myDataTable, bool isPageIndexChanging)
        {
            if (myDataTable != null)
            {
                DataView myDataView = new DataView(myDataTable);
                if (this.GridViewSortExpression != string.Empty)
                {
                    if (isPageIndexChanging)
                    {
                        myDataView.Sort = string.Format("{0} {1}",
                        this.GridViewSortExpression, this.GridViewSortDirection);
                    }
                    else
                    {
                        myDataView.Sort = string.Format("{0} {1}",
                        this.GridViewSortExpression, this.GetSortDirection());
                    }
                }
                return myDataView;
            }
            else
            {

                return new DataView();
            }
        }

        private string GetSortDirection()
        {
            switch (this.GridViewSortDirection)
            {
                case "ASC":
                    this.GridViewSortDirection = "DESC";
                    break;
                case "DESC":
                    this.GridViewSortDirection = "ASC";
                    break;
            }
            return this.GridViewSortDirection;
        }

        protected void btnSighIn_Click(object sender, EventArgs e)
        {
            Type cstype = this.GetType();
            strSearch = Txt2.Value;
            string databaseType = string.Empty;
            string server = string.Empty;
            string team = string.Empty;
            string schemaList = string.Empty;
            string oradb = string.Empty;
            string DBValue = string.Empty;
            //Session["state"] = ddl4.Value.ToString();
            //contextValue.Value = Session["state"].ToString();   
            userIDTxt = UserID.Text;
            passwordTxt = Password.Text;
            ViewState["UserID"] = userIDTxt;
            ViewState["Password"] = passwordTxt;
            ViewState["Port"] = port;
            String userID = string.Empty;
            String password = string.Empty;
            // Get a ClientScriptManager reference from the Page class.
            ClientScriptManager cs = Page.ClientScript;
            //If the selected platform is SQL
            if (platformValue.ToLower() == "informatica")
            {
                try
                {
                    userIDTxt = loginID.Text;
                    passwordTxt = loginPassword.Text;
                    ViewState["UserID"] = userIDTxt;
                    ViewState["Password"] = passwordTxt;
                    projectDrpdwnDiv.Style.Add("display", "block");
                    contextDiv.Style.Add("display", "block");
                    dbProjectHeading.Style.Add("display", "block");
                    dbContextHeading.Style.Add("display", "block");
                    keywordHeading.Style.Add("display", "block");
                    dbSchemaHeading.Visible = false;
                    GetInformaticaServerDetails(serverDropdownLst.SelectedItem.Text);
                    SIDName = serverDropdownLst.SelectedItem.Text;
                    ViewState["Host"] = host;
                    ViewState["Port"] = port;
                    ViewState["schemaName"] = schemaName;
                    oradb = @"Data Source=  (DESCRIPTION =              (ADDRESS_LIST =                  (ADDRESS =                      (HOST =" + host + ")                      (PROTOCOL = TCP) (PORT = " + port + ")                 )              )              (CONNECT_DATA = (SID = " + SIDName + ")             )         ) ;User Id=" + userIDTxt + ";Password=" + passwordTxt + ";Persist Security Info=True;";
                    //oradb = @"Data Source=  (DESCRIPTION =              (ADDRESS_LIST =                  (ADDRESS =                      (HOST =lazermark.am.lilly.com)                      (PROTOCOL = TCP) (PORT = 1530)                 )              )              (CONNECT_DATA = (SID = prd101)             )         ) ;User Id=" + userIDTxt + ";Password=" + passwordTxt + ";Persist Security Info=True;";
                    objConn = new OracleConnection(oradb);
                    objConn.Open();
                    strSQL = "SELECT DISTINCT subject_area AS Folder FROM " + schemaName + ".REP_ALL_MAPPINGS";
                    objCmd = new OracleCommand(strSQL, objConn);
                    objCmd.CommandType = CommandType.Text;
                    OracleDataAdapter da = new OracleDataAdapter(objCmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    objConn.Close();
                    Session["FolderData"] = dt;
                    BindFolderList(dt);
                    Logger.LogMessage("Platform---Informatica--LogIn by user--" + userIDTxt);
                }
                catch (OracleException Orex)
                {
                    Logger.LogException(Orex);
                    if (!cs.IsStartupScriptRegistered(cstype, csname2))
                    {
                        String cstext1 = "alert('Login Failed!!');";
                        cs.RegisterStartupScript(cstype, csname2, cstext1, true);
                    }
                    ScriptManager.RegisterStartupScript(this, typeof(System.Web.UI.Page), "ModalPop5", "openInformaticaAuthenticationModal();", true);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    Response.Redirect("~/TechnicalError.aspx");
                }

            }
            else if (platformValue.ToLower() == "sql")
            {
                for (int i = 0; i < schemaDrpdwnLst.Items.Count; i++)
                {
                    if (schemaList != "" && schemaDrpdwnLst.Items[i].Selected.Equals(true))
                    {
                        schemaList += ",";
                    }
                    if (schemaDrpdwnLst.Items[i].Selected.Equals(true))
                    {
                        schemaList += "'" + schemaDrpdwnLst.Items[i].Text + "'";
                    }
                }
                if (Session["state"] != null)
                {
                    ddl4.Value = (string)Session["state"];

                }
                //Logger.LogMessage("Platform---SQL");
                GetDatabaseDetails(dbDropDwnLst.SelectedItem.Text.ToString(), out databaseType, out server, out userID, out password, out port, out DBValue);
                try
                {
                    if (databaseType.ToUpper().Equals("ORACLE"))
                    {
                        oradb = @"Data Source=  (DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (HOST = " + server + ") (PROTOCOL = TCP) (PORT = " + port.ToString() + ")) ) (CONNECT_DATA = (SID =" + dbDropDwnLst.SelectedValue.ToString() + "))); User Id=" + userIDTxt + ";Password=" + passwordTxt + ";Persist Security Info=True;";
                        //string oradb = ConfigurationManager.ConnectionStrings["strOracleConnectionString"].ConnectionString;
                        objConn = new OracleConnection(oradb);
                        objConn.Open();
                        if (schemaList != "" && dbDropDwnLst.SelectedIndex != 0 && ddl4.Value != "0")
                        {
                            if (ddl4.Value.ToUpper().Equals("TABLE"))
                            {
                                strSQL = "SELECT DISTINCT OWNER AS Entity_Schema, Table_Name AS Entity_Name, 'Table' AS Entity_Type, 'NA' AS Entity_Subtype, 'NA' AS Entity_Definition FROM all_tables WHERE owner IN (" + schemaList + ") AND UPPER (TABLE_NAME) LIKE UPPER ('%" + strSearch + "%') UNION SELECT DISTINCT OWNER AS Entity_Schema, SYNONYM_NAME AS Entity_Name, 'SYNONYM' AS Entity_Type, 'NA' AS Entity_Subtype, 'NA' AS Entity_Definition FROM ALL_SYNONYMS WHERE owner IN (" + schemaList + ") AND (UPPER (TABLE_NAME) LIKE UPPER ('%" + strSearch + "%') OR UPPER (SYNONYM_NAME) LIKE UPPER ('%" + strSearch + "%')) UNION SELECT OWNER AS Entity_Schema, VIEW_NAME AS Entity_Name, 'View' AS Entity_Type, 'NA' AS Entity_Subtype, Text_vc AS Entity_Definition FROM all_views WHERE owner IN (" + schemaList + ") AND (UPPER (Text_vc) LIKE UPPER ('%" + strSearch + "%') OR UPPER (VIEW_NAME) LIKE UPPER ('%" + strSearch + "%')) UNION SELECT OWNER AS Entity_Schema, NAME AS Entity_Name, 'Routine' AS Entity_Type, 'NA' AS Entity_Subtype, 'NA' AS Entity_Definition FROM all_Source WHERE owner IN (" + schemaList + ") AND (UPPER (TEXT) LIKE UPPER ('%" + strSearch + "%') OR UPPER (NAME) LIKE UPPER ('%" + strSearch + "%')) ORDER BY 1, 2, 3";
                            }
                            else if (ddl4.Value.ToUpper().Equals("COLUMN"))
                            {
                                strSQL = "SELECT DISTINCT OWNER AS Entity_Schema, Table_Name AS Entity_Name, 'Table' AS Entity_Type, 'Column' AS Entity_Subtype, Data_type AS Entity_DataType, CASE WHEN DATA_TYPE = 'NUMBER' THEN DATA_PRECISION || ',' || DATA_SCALE WHEN DATA_TYPE = 'VARCHAR2' OR DATA_TYPE = 'CHAR' THEN TO_CHAR (DATA_LENGTH / 4) END DATA_LENGTH, COLUMN_NAME AS Entity_Definition FROM all_tab_columns WHERE owner IN (" + schemaList + ") AND UPPER (COLUMN_NAME) LIKE UPPER ('%" + strSearch + "%') UNION SELECT OWNER AS Entity_Schema, VIEW_NAME AS Entity_Name, 'View' AS Entity_Type, 'NA' AS Entity_Subtype, 'NA' AS Entity_DataType, null DATA_LENGTH, Text_vc AS Entity_Definition FROM all_views WHERE owner IN (" + schemaList + ") AND (UPPER (Text_vc) LIKE UPPER ('%" + strSearch + "%') OR UPPER (VIEW_NAME) LIKE UPPER ('%" + strSearch + "%')) UNION SELECT OWNER AS Entity_Schema, NAME AS Entity_Name, TYPE AS Entity_Type, 'NA' AS Entity_Subtype, 'NA' AS Entity_DataType, null DATA_LENGTH, 'NA' AS Entity_Definition FROM all_Source WHERE owner IN (" + schemaList + ") AND (   UPPER (TEXT) LIKE UPPER ('%" + strSearch + "%') OR UPPER (NAME) LIKE UPPER ('%" + strSearch + "%')) ORDER BY 1, 2, 3";
                            }
                            else if (ddl4.Value.ToUpper().Equals("ROUTINE"))
                            {
                                strSQL = "select DISTINCT OWNER as Entity_Schema, NAME AS Entity_Name, Type as Entity_Type, 'NA' as Entity_Subtype, 'NA' as Entity_Definition FROM all_Source where owner in (" + schemaList + ") and (TYPE='PROCEDURE' OR TYPE='FUNCTION') AND (UPPER(TEXT) LIKE UPPER ('%" + strSearch + "%') OR UPPER(NAME) LIKE UPPER ('%" + strSearch + "%'))";
                            }
                            else if (ddl4.Value.ToUpper().Equals("VIEW"))
                            {
                                strSQL = "select DISTINCT OWNER as Entity_Schema, VIEW_NAME AS Entity_Name, 'View' as Entity_Type, 'NA' as Entity_Subtype, Text_vc as Entity_Definition from all_views where OWNER in (" + schemaList + ") AND (UPPER(VIEW_NAME) LIKE UPPER('%" + strSearch + "%') OR UPPER(TEXT_VC) LIKE UPPER('%" + strSearch + "%'))";
                            }
                            objCmd = new OracleCommand(strSQL, objConn);
                            objCmd.CommandType = CommandType.Text;
                            OracleDataAdapter da1 = new OracleDataAdapter(objCmd);
                            DataTable dt = new DataTable();
                            da1.Fill(dt);
                            sqlGridview.DataSource = dt.DefaultView;
                            sqlGridview.DataBind();
                            objConn.Close();
                            GridviewDiv.Style.Add("display", "block");
                            sqlGridview.Style.Add("display", "block");
                            sqlGridview.UseAccessibleHeader = true;
                            sqlGridview.HeaderRow.TableSection = TableRowSection.TableHeader;
                            Session["SessionData1"] = dt;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop1", "openModal();", true);
                            noResultsFound.Style.Add("display", "none");
                            Logger.LogMessage("DB---Oracle--LogIn by user--" + userIDTxt + "--Server--" + server + "--Database--" + dbDropDwnLst.SelectedValue.ToString());
                        }
                        else
                        {
                            GridviewDiv.Style.Add("display", "none");
                            noResultsFound.Style.Add("display", "none");
                            // Check to see if the startup script is already registered.
                            if (!cs.IsStartupScriptRegistered(cstype, csname2))
                            {
                                String cstext1 = "alert('Please check mandatory fields!!');";
                                cs.RegisterStartupScript(cstype, csname2, cstext1, true);
                            }
                        }
                    }
                    else if (databaseType.ToUpper().Equals("SQL"))
                    {
                        oradb = @"Data Source=" + server + ";Initial Catalog=" + dbDropDwnLst.SelectedItem.Text.ToString() + ";User ID=" + userIDTxt + ";Password=" + passwordTxt + ";";
                        sqlConn = new SqlConnection(oradb);
                        sqlConn.Open();
                        if (schemaList != "" && dbDropDwnLst.SelectedIndex != 0 && ddl4.Value != "0")
                        {
                            if (ddl4.Value.ToUpper().Equals("COLUMN"))
                            {
                                strSQL = "select DISTINCT Table_Schema as Entity_Schema, TABLE_NAME as Entity_Name, 'Table' as Entity_Type, 'Column' as Entity_Subtype,Data_type as Entity_DataType, COLUMN_NAME as Entity_Definition from information_schema.columns where TABLE_SCHEMA in (" + schemaList + ") and UPPER(COLUMN_NAME) LIKE UPPER('%" + strSearch + "%') UNION select Routine_schema as Entity_Schema,routine_name as Entity_Name ,'Routine' as Entity_Type, Routine_type as Entity_Subtype,'NA' as Entity_DataType,ROUTINE_DEFINITION as Entity_Definition from information_schema.ROUTINES where ROUTINE_SCHEMA in (" + schemaList + ") and UPPER(ROUTINE_DEFINITION) LIKE UPPER('%" + strSearch + "%') UNION select TABLE_SCHEMA as Entity_Schema,TABLE_NAME as Entity_Name, 'View' as Entity_Type, 'NA' as Entity_Subtype, 'NA' as Entity_DataType,VIEW_DEFINITION as Entity_Definition from information_schema.VIEWS where TABLE_SCHEMA in (" + schemaList + ") and Upper(VIEW_DEFINITION) LIKE UPPER('%" + strSearch + "%')";
                            }
                            else if (ddl4.Value.ToUpper().Equals("TABLE"))
                            {
                                strSQL = "select DISTINCT Routine_schema as Entity_Schema,routine_name as Entity_Name ,'Routine' as Entity_Type, Routine_type as Entity_Subtype,ROUTINE_DEFINITION as Entity_Definition from information_schema.ROUTINES  where ROUTINE_SCHEMA in (" + schemaList + ") and upper(ROUTINE_DEFINITION) LIKE UPPER('%" + strSearch + "%') UNION select TABLE_SCHEMA as Entity_Schema,TABLE_NAME as Entity_Name, 'Table' as Entity_Type, 'NA' as Entity_Subtype, 'NA' as Entity_Definition from information_schema.tables where TABLE_SCHEMA in (" + schemaList + ") and TABLE_NAME LIKE UPPER('%" + strSearch + "%') and TABLE_TYPE='BASE TABLE' UNION select TABLE_SCHEMA as Entity_Schema,TABLE_NAME as Entity_Name, 'View' as Entity_Type, 'NA' as Entity_Subtype, VIEW_DEFINITION as Entity_Definition from information_schema.VIEWS where TABLE_SCHEMA in (" + schemaList + ") and Upper(VIEW_DEFINITION) LIKE UPPER('%" + strSearch + "%')";

                            }
                            else if (ddl4.Value.ToUpper().Equals("ROUTINE"))
                            {
                                strSQL = "select DISTINCT Routine_schema as Entity_Schema,routine_name as Entity_Name ,'Routine' as Entity_Type, Routine_type as Entity_Subtype,ROUTINE_DEFINITION as Entity_Definition from information_schema.ROUTINES where ROUTINE_SCHEMA in (" + schemaList + ") and upper(ROUTINE_NAME) LIKE UPPER('%" + strSearch + "%')";

                            }
                            else if (ddl4.Value.ToUpper().Equals("VIEW"))
                            {
                                strSQL = "select DISTINCT TABLE_SCHEMA as Entity_Schema,TABLE_NAME as Entity_Name, 'View' as Entity_Type, 'NA' as Entity_Subtype, VIEW_DEFINITION as Entity_Definition from information_schema.VIEWS where TABLE_SCHEMA in (" + schemaList + ") and (upper(VIEW_DEFINITION) LIKE UPPER('%" + strSearch + "%') OR upper(TABLE_NAME) LIKE UPPER('%" + strSearch + "%'))";
                            }

                            sqlCmd = new SqlCommand(strSQL, sqlConn);
                            sqlCmd.CommandType = CommandType.Text;
                            SqlDataAdapter da1 = new SqlDataAdapter(sqlCmd);
                            DataTable dt = new DataTable();
                            da1.Fill(dt);
                            sqlGridview.DataSource = dt.DefaultView;
                            sqlGridview.DataBind();
                            sqlConn.Close();
                            GridviewDiv.Style.Add("display", "block");
                            sqlGridview.Style.Add("display", "block");
                            sqlGridview.UseAccessibleHeader = true;
                            sqlGridview.HeaderRow.TableSection = TableRowSection.TableHeader;
                            Session["SessionData1"] = dt;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop2", "openModal();", true);
                            noResultsFound.Style.Add("display", "none");
                            Logger.LogMessage("DB---SQL Server--LogIn by user--" + userIDTxt + "--Server--" + server + "--Database--" + dbDropDwnLst.SelectedValue.ToString());
                        }
                        else
                        {
                            GridviewDiv.Style.Add("display", "none");
                            noResultsFound.Style.Add("display", "none");
                            if (!cs.IsStartupScriptRegistered(cstype, csname5))
                            {
                                String cstext1 = "alert('Please check mandatory fields!!');";
                                cs.RegisterStartupScript(cstype, csname5, cstext1, true);
                            }
                        }
                    }
                    else if (databaseType.ToUpper().Equals("NETEZZA"))
                    {
                        schemaList = string.Empty;

                        oradb = @"Provider=NZOLEDB;Password=" + passwordTxt + ";User ID=" + userIDTxt + ";Data Source=" + server + ";Initial Catalog=" + DBValue + ";Persist Security Info=True;";
                        //oradb = @"Provider=NetezzaSQL;Data Source=" + server + ";Initial Catalog=" + dbDropDwnLst.SelectedItem.Text.ToString() + ";User ID=" + userID + ";Password=" + password + ";";
                        oldbCon = new OleDbConnection(oradb);
                        oldbCon.Open();
                        if (ddl4.Value.ToUpper().Equals("TABLE"))
                        {
                            for (int i = 0; i < schemaDrpdwnLst.Items.Count; i++)
                            {
                                if (schemaList != "" && schemaDrpdwnLst.Items[i].Selected.Equals(true))
                                {
                                    schemaList += " UNION ALL ";
                                }
                                if (schemaDrpdwnLst.Items[i].Selected.Equals(true))
                                {
                                    schemaList += "select DISTINCT DATABASE AS ENTITY_SCHEMA, TABLENAME AS ENTITY_NAME, OBJTYPE AS Entity_Type, 'NA' as Entity_Subtype, 'NA' as Entity_Definition from " + schemaDrpdwnLst.Items[i].Text + ".._v_table where upper(ENTITY_NAME) LIKE UPPER('%" + strSearch + "%') UNION  ALL select  DATABASE AS ENTITY_SCHEMA, VIEWNAME AS ENTITY_NAME, OBJTYPE AS Entity_Type, 'NA' as Entity_Subtype, DEFINITION as Entity_Definition  from  " + schemaDrpdwnLst.Items[i].Text + ".._v_view where upper(DEFINITION) LIKE UPPER('%" + strSearch + "%') UNION  select  DATABASE AS ENTITY_SCHEMA, PROCEDURE AS ENTITY_NAME, 'Routine' as Entity_Type, OBJTYPE as Entity_Subtype, PROCEDURESOURCE as Entity_Definition from  " + schemaDrpdwnLst.Items[i].Text + ".._v_procedure where upper(PROCEDURESOURCE) LIKE UPPER('%" + strSearch + "%') UNION  select  DATABASE AS ENTITY_SCHEMA, SYNONYM_NAME AS ENTITY_NAME, OBJTYPE AS Entity_Type, 'NA' as Entity_Subtype, 'NA' as Entity_Definition  from  " + schemaDrpdwnLst.Items[i].Text + ".._v_synonym where UPPER(REFOBJNAME) LIKE UPPER('%" + strSearch + "%')";
                                }
                            }
                            strSQL = schemaList;
                            //                                strSQL = "select DISTINCT SCHEMA AS ENTITY_SCHEMA, TABLENAME AS ENTITY_NAME, OBJTYPE AS Entity_Type, 'NA' as Entity_Subtype, 'NA' as Entity_Definition from  _v_table where SCHEMA in (" + schemaList + ") AND upper(ENTITY_NAME) LIKE UPPER('%" + strSearch + "%') UNION  ALL select  SCHEMA AS ENTITY_SCHEMA, VIEWNAME AS ENTITY_NAME, OBJTYPE AS Entity_Type, 'NA' as Entity_Subtype, DEFINITION as Entity_Definition  from  _v_view where SCHEMA in (" + schemaList + ") AND upper(DEFINITION) LIKE UPPER('%" + strSearch + "%') UNION  select  SCHEMA AS ENTITY_SCHEMA, PROCEDURE AS ENTITY_NAME, 'Routine' as Entity_Type, OBJTYPE as Entity_Subtype, PROCEDURESOURCE as Entity_Definition from  _v_procedure where SCHEMA in (" + schemaList + ") AND upper(PROCEDURESOURCE) LIKE UPPER('%" + strSearch + "%') UNION  select  SCHEMA AS ENTITY_SCHEMA, SYNONYM_NAME AS ENTITY_NAME, OBJTYPE AS Entity_Type, 'NA' as Entity_Subtype, 'NA' as Entity_Definition  from  _v_synonym where SCHEMA in (" + schemaList + ") AND UPPER(REFOBJNAME) LIKE UPPER('%" + strSearch + "%')";
                        }
                        else if (ddl4.Value.ToUpper().Equals("COLUMN"))
                        {
                            for (int i = 0; i < schemaDrpdwnLst.Items.Count; i++)
                            {
                                if (schemaList != "" && schemaDrpdwnLst.Items[i].Selected.Equals(true))
                                {
                                    schemaList += " UNION ALL ";
                                }
                                if (schemaDrpdwnLst.Items[i].Selected.Equals(true))
                                {
                                    schemaList += "select DISTINCT DATABASE AS ENTITY_SCHEMA, TABLE_NAME AS ENTITY_NAME, 'Table' AS Entity_Type, 'Column' as Entity_Subtype,Type_name as Entity_DataType,Column_size as Entity_Size, COLUMN_NAME as Entity_Definition from " + schemaDrpdwnLst.Items[i].Text + ".._v_sys_columns where UPPER(COLUMN_NAME) LIKE UPPER('%" + strSearch + "%') UNION select  DATABASE AS ENTITY_SCHEMA, VIEWNAME AS ENTITY_NAME, OBJTYPE AS Entity_Type, 'NA' as Entity_Subtype,'NA' as Entity_DataType,'0' as Entity_Size, DEFINITION as Entity_Definition from " + schemaDrpdwnLst.Items[i].Text + ".._v_view where upper(DEFINITION) LIKE UPPER('%" + strSearch + "%') UNION select DATABASE AS ENTITY_SCHEMA, PROCEDURE AS ENTITY_NAME, 'Routine' as Entity_Type, OBJTYPE as Entity_Subtype, 'NA' as Entity_DataType,'0' as Entity_Size,PROCEDURESOURCE as Entity_Definition from " + schemaDrpdwnLst.Items[i].Text + ".._v_procedure where (upper(PROCEDURESOURCE) LIKE UPPER('%" + strSearch + "%') OR upper(PROCEDURE) LIKE UPPER('%" + strSearch + "%'))";
                                }
                            }
                            strSQL = schemaList;

                            //strSQL = "select DISTINCT SCHEMA AS ENTITY_SCHEMA, TABLE_NAME AS ENTITY_NAME, 'Table' AS Entity_Type, 'Column' as Entity_Subtype,Type_name as Entity_DataType,Column_size as Entity_Size, COLUMN_NAME as Entity_Definition from _v_sys_columns where SCHEMA in (" + schemaList + ") AND UPPER(COLUMN_NAME) LIKE UPPER('%" + strSearch + "%') UNION  select  SCHEMA AS ENTITY_SCHEMA, VIEWNAME AS ENTITY_NAME, OBJTYPE AS Entity_Type, 'NA' as Entity_Subtype,'NA' as Entity_DataType,'0' as Entity_Size, DEFINITION as Entity_Definition from _v_view where SCHEMA in (" + schemaList + ") AND upper(DEFINITION) LIKE UPPER('%" + strSearch + "%') UNION select SCHEMA AS ENTITY_SCHEMA, PROCEDURE AS ENTITY_NAME, 'Routine' as Entity_Type, OBJTYPE as Entity_Subtype, 'NA' as Entity_DataType,'0' as Entity_Size,PROCEDURESOURCE as Entity_Definition from _v_procedure where SCHEMA in (" + schemaList + ") AND (upper(PROCEDURESOURCE) LIKE UPPER('%" + strSearch + "%') OR upper(PROCEDURE) LIKE UPPER('%" + strSearch + "%'))";
                        }
                        else if (ddl4.Value.ToUpper().Equals("ROUTINE"))
                        {
                            for (int i = 0; i < schemaDrpdwnLst.Items.Count; i++)
                            {
                                if (schemaList != "" && schemaDrpdwnLst.Items[i].Selected.Equals(true))
                                {
                                    schemaList += " UNION ALL ";
                                }
                                if (schemaDrpdwnLst.Items[i].Selected.Equals(true))
                                {
                                    schemaList += "select DISTINCT DATABASE AS ENTITY_SCHEMA, PROCEDURE AS ENTITY_NAME, 'Routine' as Entity_Type, OBJTYPE as Entity_Subtype, PROCEDURESOURCE as Entity_Definition  from  " + schemaDrpdwnLst.Items[i].Text + ".._v_procedure where (upper(PROCEDURESOURCE) LIKE UPPER('%" + strSearch + "%') OR upper(PROCEDURE) LIKE UPPER('%" + strSearch + "%'))";
                                }
                            }
                            strSQL = schemaList;
                            //strSQL = "select DISTINCT SCHEMA AS ENTITY_SCHEMA, PROCEDURE AS ENTITY_NAME, 'Routine' as Entity_Type, OBJTYPE as Entity_Subtype, PROCEDURESOURCE as Entity_Definition  from  _v_procedure where SCHEMA in (" + schemaList + ") AND (upper(PROCEDURESOURCE) LIKE UPPER('%" + strSearch + "%') OR upper(PROCEDURE) LIKE UPPER('%" + strSearch + "%'))";

                        }
                        else if (ddl4.Value.ToUpper().Equals("VIEW"))
                        {
                            for (int i = 0; i < schemaDrpdwnLst.Items.Count; i++)
                            {
                                if (schemaList != "" && schemaDrpdwnLst.Items[i].Selected.Equals(true))
                                {
                                    schemaList += " UNION ALL ";
                                }
                                if (schemaDrpdwnLst.Items[i].Selected.Equals(true))
                                {
                                    schemaList += "select DISTINCT DATABASE AS ENTITY_SCHEMA, VIEWNAME AS ENTITY_NAME, OBJTYPE AS Entity_Type, 'NA' as Entity_Subtype, DEFINITION as Entity_Definition  from  " + schemaDrpdwnLst.Items[i].Text + ".._v_view where (upper(DEFINITION) LIKE UPPER('%" + strSearch + "%') OR upper(VIEWNAME) LIKE UPPER('%" + strSearch + "%'))";
                                }
                            }
                            strSQL = schemaList;
                            //strSQL = "select DISTINCT SCHEMA AS ENTITY_SCHEMA, VIEWNAME AS ENTITY_NAME, OBJTYPE AS Entity_Type, 'NA' as Entity_Subtype, DEFINITION as Entity_Definition  from  _v_view where SCHEMA in (" + schemaList + ") AND (upper(DEFINITION) LIKE UPPER('%" + strSearch + "%') OR upper(VIEWNAME) LIKE UPPER('%" + strSearch + "%'))";
                            //UNION  select  SCHEMA AS ENTITY_SCHEMA, PROCEDURE AS ENTITY_NAME, 'Routine' as Entity_Type, OBJTYPE as Entity_Subtype, PROCEDURESOURCE as Entity_Definition from  _v_procedure where SCHEMA in (" + schemaList + ") AND PROCEDURESOURCE LIKE UPPER('%" + strSearch + "%')
                        }
                        if (schemaList != "" && dbDropDwnLst.SelectedIndex != 0 && ddl4.Value != "0")
                        {

                            //OleDbDataReader dreader;                        
                            oldbCmd = new OleDbCommand(strSQL, oldbCon);
                            oldbCmd.CommandType = CommandType.Text;
                            OleDbDataAdapter da1 = new OleDbDataAdapter(oldbCmd);
                            DataTable dt = new DataTable();
                            da1.Fill(dt);
                            sqlGridview.DataSource = dt.DefaultView;
                            sqlGridview.DataBind();
                            oldbCon.Close();
                            GridviewDiv.Style.Add("display", "block");
                            sqlGridview.Style.Add("display", "block");
                            sqlGridview.UseAccessibleHeader = true;
                            sqlGridview.HeaderRow.TableSection = TableRowSection.TableHeader;
                            Session["SessionData1"] = dt;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop3", "openModal();", true);
                            noResultsFound.Style.Add("display", "none");
                            Logger.LogMessage("DB---Netezza--LogIn by user--" + userIDTxt + "--Serve--" + server + "--Database--" + DBValue);
                        }
                        else
                        {
                            GridviewDiv.Style.Add("display", "none");
                            noResultsFound.Style.Add("display", "none");
                            // Check to see if the startup script is already registered.
                            if (!cs.IsStartupScriptRegistered(cstype, csname5))
                            {
                                String cstext1 = "alert('Please check mandatory fields!!');";
                                cs.RegisterStartupScript(cstype, csname5, cstext1, true);
                            }
                        }
                    }
                }
                catch (SqlException sqEx)
                {
                    Logger.LogException(sqEx);
                    if (!cs.IsStartupScriptRegistered(cstype, csname2))
                    {
                        String cstext1 = "alert('Login Failed!!');";
                        cs.RegisterStartupScript(cstype, csname2, cstext1, true);
                    }
                    ScriptManager.RegisterStartupScript(this, typeof(System.Web.UI.Page), "ModalPop6", "openAuthenticationModal();", true);
                }
                catch (OracleException orEx)
                {
                    Logger.LogException(orEx);
                    if (!cs.IsStartupScriptRegistered(cstype, csname2))
                    {
                        String cstext1 = "alert('Login Failed!!');";
                        cs.RegisterStartupScript(cstype, csname2, cstext1, true);
                    }
                    ScriptManager.RegisterStartupScript(this, typeof(System.Web.UI.Page), "ModalPop7", "openAuthenticationModal();", true);
                }
                catch (OleDbException oldbEx)
                {
                    Logger.LogException(oldbEx);
                    if (!cs.IsStartupScriptRegistered(cstype, csname2))
                    {
                        String cstext1 = "alert('Login Failed!!');";
                        cs.RegisterStartupScript(cstype, csname2, cstext1, true);
                    }
                    ScriptManager.RegisterStartupScript(this, typeof(System.Web.UI.Page), "ModalPop8", "openAuthenticationModal();", true);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    noResultsFound.Style.Add("display", "block");
                    //Response.Redirect("~/TechnicalError.aspx");
                }

            }
            else if (platformValue.ToLower() == "linux")
            {
                string test="da";
                try
                {
                    string strDirectory = string.Empty;
                    for (int i = 0; i < dirDrpdwnList.Items.Count; i++)
                    {
                        if (strDirectory != "" && dirDrpdwnList.Items[i].Selected.Equals(true))
                        {
                            strDirectory += ",";
                        }
                        if (dirDrpdwnList.Items[i].Selected.Equals(true))
                        {
                            strDirectory += "'" + dirDrpdwnList.Items[i].Text + "'";
                        }
                    }
                    if (serverDropDwnLst.SelectedIndex != 0 && !string.IsNullOrEmpty(strDirectory))
                    {
                        GetServerDetails(serverDropDwnLst.SelectedItem.Text.ToString(), out userID, out team);

                        using (var client = new SshClient(serverDropDwnLst.SelectedItem.Text.ToString(), userIDTxt, passwordTxt))
                        {
                            client.Connect();
                            if (team == "Team1")
                            {
                                //client.RunCommand("mkdir \\IAToolLinuxService1");                                
                                //client.RunCommand("touch /home/" + userIDTxt.ToString() + "/IAToolLinuxService1/main_test2.sh");
                                //client.RunCommand("chmod 775 /home/" + userIDTxt.ToString() + "/IAToolLinuxService1/main_test2.sh");
                                //client.RunCommand("echo "+test+" > /home/" + userIDTxt.ToString() + "/IAToolLinuxService1/main_test2.sh");
                                //client.RunCommand("cp C://Users//c201573//Documents//Visual Studio 2012//Projects//ValueAddDemo\ValueAddDemo \\home\\" + userIDTxt.ToString() + "\\IAToolLinuxService");
                                client.RunCommand("/bin/ksh /home/" + userIDTxt.ToString() + "/IAToolLinuxService/main1.sh " + strDirectory.Trim().ToString() + " " + userIDTxt.ToString() + " " + serverDropDwnLst.SelectedItem.Text.ToString() + " " + strSearch.Trim().ToString());
                                //client.RunCommand("/bin/ksh /home/" +"c181012" + "/IAToolLinuxService/main1.sh " + strDirectory.Trim().ToString() + " " + userIDTxt.ToString() + " " + serverDropDwnLst.SelectedItem.Text.ToString() + " " + strSearch.Trim().ToString());
                                using (var sftp = new SftpClient(serverDropDwnLst.SelectedItem.Text.ToString(), userIDTxt, passwordTxt))
                                {
                                    sftp.Connect();
                                    //Reading from the SSH channel                                      
                                    try
                                    {
                                        using (var FileStream = new FileStream("C:\\users\\public\\datafile.txt", FileMode.Create, FileAccess.Write, FileShare.None))
                                        {
                                            sftp.DownloadFile("/home/" + userIDTxt.ToString() + "/IAToolLinuxService/IA_tool_output_file.csv", FileStream, null);
                                            //sftp.DownloadFile("/home/" + "c181012" + "/IAToolLinuxService/IA_tool_output_file.csv", FileStream, null);
                                        }
                                        client.RunCommand("/bin/ksh /home/" + userIDTxt.ToString() + "/IAToolLinuxService/archive_script.sh");
                                        //client.RunCommand("/bin/ksh /home/" + "c181012" + "/IAToolLinuxService/archive_script.sh");
                                        client.Disconnect();
                                        sftp.Disconnect();
                                        BindCSVData("C:\\users\\public\\datafile.txt");
                                    }
                                    catch (Exception ex)
                                    {
                                        noResultsFound.Style.Add("display", "block");
                                        //Response.Redirect("~/TechnicalError.aspx");
                                    }
                                }
                            }
                            else if (team == "Team2")
                            {
                                client.RunCommand("/bin/ksh /home/" + userIDTxt.ToString() + "/IAToolLinuxService/main1.sh " + strDirectory.Trim().ToString() + " " + userIDTxt.ToString() + " " + serverDropDwnLst.SelectedItem.Text.ToString() + " " + strSearch.Trim().ToString());
                                using (var sftp = new SftpClient(serverDropDwnLst.SelectedItem.Text.ToString(), userIDTxt, passwordTxt))
                                {
                                    sftp.Connect();
                                    try
                                    {
                                        //Reading from the SSH channel                                      
                                        using (var FileStream = new FileStream("C:\\users\\public\\datafile.txt", FileMode.Create, FileAccess.Write, FileShare.None))
                                        {
                                            sftp.DownloadFile("/home/" + userIDTxt.ToString() + "/IAToolLinuxService/IA_tool_output_file.csv", FileStream, null);
                                        }
                                        client.RunCommand("/bin/ksh /home/" + userIDTxt.ToString() + "/IAToolLinuxService/archive_script.sh");
                                        client.Disconnect();
                                        sftp.Disconnect();
                                        BindCSVData("C:\\users\\public\\datafile.txt");
                                    }
                                    catch (Exception ex)
                                    {
                                        noResultsFound.Style.Add("display", "block");
                                        //Response.Redirect("~/TechnicalError.aspx");
                                    }
                                }

                            }
                            Logger.LogMessage("Platform---Linux--LogIn by user--" + userIDTxt + "--Server--" + serverDropDwnLst.SelectedItem.Text.ToString() + "--Directory--" + strDirectory.Trim().ToString());
                        }


                    }

                    else
                    {
                        GridviewDiv.Style.Add("display", "none");
                        noResultsFound.Style.Add("display", "none");
                        // Check to see if the startup script is already registered.
                        if (!cs.IsStartupScriptRegistered(cstype, csname4))
                        {
                            String cstext1 = "alert('Please check mandatory fields!!');";
                            cs.RegisterStartupScript(cstype, csname4, cstext1, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    //Response.Redirect("~/TechnicalError.aspx");
                    if (!cs.IsStartupScriptRegistered(cstype, csname6))
                    {
                        String cstext1 = "alert('Please try again');";
                        cs.RegisterStartupScript(cstype, csname6, cstext1, true);
                    }
                    ScriptManager.RegisterStartupScript(this, typeof(System.Web.UI.Page), "ModalPop2", "openAuthenticationModal();", true);
                }

            }
            else if (platformValue.ToLower() == "github")
            {
                try
                {
                    Page<JsonResponse.Item> response = null;
                    List<JsonResponse.Item> lsts = null;                    
                    DataTable dt = new DataTable();
                    if (appDropDwnLst.SelectedIndex != 0 && repoDropDownList.SelectedIndex != 0)
                    {
                        response = GetGithubResults(userIDTxt, passwordTxt, strSearch.Trim().ToString(), repoDropDownList.SelectedValue.ToString());
                        if (response.total_count > 0)
                        {
                            dt.Columns.Add("File_Name", typeof(string));
                            dt.Columns.Add("File_Path", typeof(string));
                            dt.Columns.Add("Repository_Name", typeof(string));
                            dt.Columns.Add("Repository_Path", typeof(string));
                            lsts = response.Items;
                            foreach (JsonResponse.Item item in lsts)
                            {
                                DataRow row = dt.NewRow();
                                row["File_Name"] = item.name;
                                row["File_Path"] = item.path;
                                row["Repository_Name"] = item.repository.full_name;
                                row["Repository_Path"] = item.repository.html_url;
                                dt.Rows.Add(row);
                            }

                            githubGridview.DataSource = dt.DefaultView;
                            githubGridview.DataBind();
                            GridviewDiv.Style.Add("display", "block");
                            githubGridview.Style.Add("display", "block");
                            githubGridview.UseAccessibleHeader = true;
                            githubGridview.HeaderRow.TableSection = TableRowSection.TableHeader;
                            Session["SessionData3"] = dt;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop8", "openModal();", true);
                            noResultsFound.Style.Add("display", "none");
                            Logger.LogMessage("DB---Github--LogIn by user--" + userIDTxt + "--Repository--" + repoDropDownList.SelectedValue.ToString());
                        }
                        else
                            noResultsFound.Style.Add("display", "block");
                    }
                    else
                    {
                        GridviewDiv.Style.Add("display", "none");
                        noResultsFound.Style.Add("display", "none");
                        // Check to see if the startup script is already registered.
                        if (!cs.IsStartupScriptRegistered(cstype, csname4))
                        {
                            String cstext1 = "alert('Please check mandatory fields!!');";
                            cs.RegisterStartupScript(cstype, csname4, cstext1, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    //Response.Redirect("~/TechnicalError.aspx");
                    if (!cs.IsStartupScriptRegistered(cstype, csname6))
                    {
                        String cstext1 = "alert('Please enter the correct UserID and Password');";
                        cs.RegisterStartupScript(cstype, csname6, cstext1, true);
                    }
                    ScriptManager.RegisterStartupScript(this, typeof(System.Web.UI.Page), "ModalPop2", "openAuthenticationModal();", true);
                }

            }
        }
        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }
        protected void search_Results_Click(object sender, EventArgs e)
        {
            Type cstype = this.GetType();
            strSearch = Txt2.Value;
            string databaseType = string.Empty;
            string server = string.Empty;
            string team = string.Empty;
            string schemaList = string.Empty;
            string oradb = string.Empty;
            string DBValue = string.Empty;
            //Session["state"] = ddl4.Value.ToString();
            //contextValue.Value = Session["state"].ToString();   
            userIDTxt = ViewState["UserID"].ToString();
            passwordTxt = ViewState["Password"].ToString();
            String userID = string.Empty;
            String password = string.Empty;
            // Get a ClientScriptManager reference from the Page class.
            ClientScriptManager cs = Page.ClientScript;
            //If the selected platform is SQL
            if (platformValue.ToLower() == "informatica")
            {
                try
                {
                    userIDTxt = loginID.Text;
                    passwordTxt = loginPassword.Text;
                    ViewState["UserID"] = userIDTxt;
                    ViewState["Password"] = passwordTxt;
                    projectDrpdwnDiv.Style.Add("display", "block");
                    contextDiv.Style.Add("display", "block");
                    dbProjectHeading.Style.Add("display", "block");
                    dbContextHeading.Style.Add("display", "block");
                    keywordHeading.Style.Add("display", "block");
                    dbSchemaHeading.Visible = false;
                    GetInformaticaServerDetails(serverDropdownLst.SelectedItem.Text);
                    SIDName = serverDropdownLst.SelectedItem.Text;
                    ViewState["Host"] = host;
                    ViewState["Port"] = port;
                    ViewState["schemaName"] = schemaName;
                    oradb = @"Data Source=  (DESCRIPTION =              (ADDRESS_LIST =                  (ADDRESS =                      (HOST =" + host + ")                      (PROTOCOL = TCP) (PORT = " + port + ")                 )              )              (CONNECT_DATA = (SID = " + SIDName + ")             )         ) ;User Id=" + userIDTxt + ";Password=" + passwordTxt + ";Persist Security Info=True;";
                    //oradb = @"Data Source=  (DESCRIPTION =              (ADDRESS_LIST =                  (ADDRESS =                      (HOST =lazermark.am.lilly.com)                      (PROTOCOL = TCP) (PORT = 1530)                 )              )              (CONNECT_DATA = (SID = prd101)             )         ) ;User Id=" + userIDTxt + ";Password=" + passwordTxt + ";Persist Security Info=True;";
                    objConn = new OracleConnection(oradb);
                    objConn.Open();
                    strSQL = "SELECT DISTINCT subject_area AS Folder FROM " + schemaName + ".REP_ALL_MAPPINGS";
                    objCmd = new OracleCommand(strSQL, objConn);
                    objCmd.CommandType = CommandType.Text;
                    OracleDataAdapter da = new OracleDataAdapter(objCmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    objConn.Close();
                    Session["FolderData"] = dt;
                    BindFolderList(dt);
                    Logger.LogMessage("Platform---Informatica--LogIn by user--" + userIDTxt);
                }
                catch (OracleException orEx)
                {
                    Logger.LogException(orEx);
                    if (!cs.IsStartupScriptRegistered(cstype, csname2))
                    {
                        String cstext1 = "alert('Login Failed!!');";
                        cs.RegisterStartupScript(cstype, csname2, cstext1, true);
                    }
                    ScriptManager.RegisterStartupScript(this, typeof(System.Web.UI.Page), "ModalPop5", "openInformaticaAuthenticationModal();", true);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    Response.Redirect("~/TechnicalError.aspx");
                }

            }
            else if (platformValue.ToLower() == "sql")
            {
                for (int i = 0; i < schemaDrpdwnLst.Items.Count; i++)
                {
                    if (schemaList != "" && schemaDrpdwnLst.Items[i].Selected.Equals(true))
                    {
                        schemaList += ",";
                    }
                    if (schemaDrpdwnLst.Items[i].Selected.Equals(true))
                    {
                        schemaList += "'" + schemaDrpdwnLst.Items[i].Text + "'";
                    }
                }
                if (Session["state"] != null)
                {
                    ddl4.Value = (string)Session["state"];

                }

                GetDatabaseDetails(dbDropDwnLst.SelectedItem.Text.ToString(), out databaseType, out server, out userID, out password, out port, out DBValue);
                try
                {
                    if (databaseType.ToUpper().Equals("ORACLE"))
                    {
                        //port = ViewState["Port"].ToString();
                        oradb = @"Data Source=  (DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (HOST = " + server + ") (PROTOCOL = TCP) (PORT = " + port + ")) ) (CONNECT_DATA = (SID =" + dbDropDwnLst.SelectedValue.ToString() + "))); User Id=" + userIDTxt + ";Password=" + passwordTxt + ";Persist Security Info=True;";
                        //string oradb = ConfigurationManager.ConnectionStrings["strOracleConnectionString"].ConnectionString;
                        objConn = new OracleConnection(oradb);
                        objConn.Open();
                        if (schemaList != "" && dbDropDwnLst.SelectedIndex != 0 && ddl4.Value != "0")
                        {
                            if (ddl4.Value.ToUpper().Equals("TABLE"))
                            {
                                strSQL = "SELECT DISTINCT OWNER AS Entity_Schema, Table_Name AS Entity_Name, 'Table' AS Entity_Type, 'NA' AS Entity_Subtype, 'NA' AS Entity_Definition FROM all_tables WHERE owner IN (" + schemaList + ") AND UPPER (TABLE_NAME) LIKE UPPER ('%" + strSearch + "%') UNION SELECT DISTINCT OWNER AS Entity_Schema, SYNONYM_NAME AS Entity_Name, 'SYNONYM' AS Entity_Type, 'NA' AS Entity_Subtype, 'NA' AS Entity_Definition FROM ALL_SYNONYMS WHERE owner IN (" + schemaList + ") AND (UPPER (TABLE_NAME) LIKE UPPER ('%" + strSearch + "%') OR UPPER (SYNONYM_NAME) LIKE UPPER ('%" + strSearch + "%')) UNION SELECT OWNER AS Entity_Schema, VIEW_NAME AS Entity_Name, 'View' AS Entity_Type, 'NA' AS Entity_Subtype, Text_vc AS Entity_Definition FROM all_views WHERE owner IN (" + schemaList + ") AND (UPPER (Text_vc) LIKE UPPER ('%" + strSearch + "%') OR UPPER (VIEW_NAME) LIKE UPPER ('%" + strSearch + "%')) UNION SELECT OWNER AS Entity_Schema, NAME AS Entity_Name, 'Routine' AS Entity_Type, 'NA' AS Entity_Subtype, 'NA' AS Entity_Definition FROM all_Source WHERE owner IN (" + schemaList + ") AND (UPPER (TEXT) LIKE UPPER ('%" + strSearch + "%') OR UPPER (NAME) LIKE UPPER ('%" + strSearch + "%')) ORDER BY 1, 2, 3";
                            }
                            else if (ddl4.Value.ToUpper().Equals("COLUMN"))
                            {
                                strSQL = "SELECT DISTINCT OWNER AS Entity_Schema, Table_Name AS Entity_Name, 'Table' AS Entity_Type, 'Column' AS Entity_Subtype, Data_type AS Entity_DataType, CASE WHEN DATA_TYPE = 'NUMBER' THEN DATA_PRECISION || ',' || DATA_SCALE WHEN DATA_TYPE = 'VARCHAR2' OR DATA_TYPE = 'CHAR' THEN TO_CHAR (DATA_LENGTH / 4) END DATA_LENGTH, COLUMN_NAME AS Entity_Definition FROM all_tab_columns WHERE owner IN (" + schemaList + ") AND UPPER (COLUMN_NAME) LIKE UPPER ('%" + strSearch + "%') UNION SELECT OWNER AS Entity_Schema, VIEW_NAME AS Entity_Name, 'View' AS Entity_Type, 'NA' AS Entity_Subtype, 'NA' AS Entity_DataType, null DATA_LENGTH, Text_vc AS Entity_Definition FROM all_views WHERE owner IN (" + schemaList + ") AND (UPPER (Text_vc) LIKE UPPER ('%" + strSearch + "%') OR UPPER (VIEW_NAME) LIKE UPPER ('%" + strSearch + "%')) UNION SELECT OWNER AS Entity_Schema, NAME AS Entity_Name, TYPE AS Entity_Type, 'NA' AS Entity_Subtype, 'NA' AS Entity_DataType, null DATA_LENGTH, 'NA' AS Entity_Definition FROM all_Source WHERE owner IN (" + schemaList + ") AND (   UPPER (TEXT) LIKE UPPER ('%" + strSearch + "%') OR UPPER (NAME) LIKE UPPER ('%" + strSearch + "%')) ORDER BY 1, 2, 3";
                            }
                            else if (ddl4.Value.ToUpper().Equals("ROUTINE"))
                            {
                                strSQL = "select DISTINCT OWNER as Entity_Schema, NAME AS Entity_Name, Type as Entity_Type, 'NA' as Entity_Subtype, 'NA' as Entity_Definition FROM all_Source where owner in (" + schemaList + ") and (TYPE='PROCEDURE' OR TYPE='FUNCTION') AND (UPPER(TEXT) LIKE UPPER ('%" + strSearch + "%') OR UPPER(NAME) LIKE UPPER ('%" + strSearch + "%'))";
                            }
                            else if (ddl4.Value.ToUpper().Equals("VIEW"))
                            {
                                strSQL = "select DISTINCT OWNER as Entity_Schema, VIEW_NAME AS Entity_Name, 'View' as Entity_Type, 'NA' as Entity_Subtype, Text_vc as Entity_Definition from all_views where OWNER in (" + schemaList + ") AND (UPPER(VIEW_NAME) LIKE UPPER('%" + strSearch + "%') OR UPPER(TEXT_VC) LIKE UPPER('%" + strSearch + "%'))";
                            }
                            objCmd = new OracleCommand(strSQL, objConn);
                            objCmd.CommandType = CommandType.Text;
                            OracleDataAdapter da1 = new OracleDataAdapter(objCmd);
                            DataTable dt = new DataTable();
                            da1.Fill(dt);
                            sqlGridview.DataSource = dt.DefaultView;
                            sqlGridview.DataBind();
                            objConn.Close();
                            GridviewDiv.Style.Add("display", "block");
                            sqlGridview.Style.Add("display", "block");
                            sqlGridview.UseAccessibleHeader = true;
                            sqlGridview.HeaderRow.TableSection = TableRowSection.TableHeader;
                            Session["SessionData1"] = dt;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop1", "openModal();", true);
                            noResultsFound.Style.Add("display", "none");
                            Logger.LogMessage("DB---Oracle--LogIn by user--" + userIDTxt + "--Server--" + server + "--Database--" + dbDropDwnLst.SelectedValue.ToString());
                        }
                        else
                        {
                            GridviewDiv.Style.Add("display", "none");
                            noResultsFound.Style.Add("display", "none");
                            // Check to see if the startup script is already registered.
                            if (!cs.IsStartupScriptRegistered(cstype, csname2))
                            {
                                String cstext1 = "alert('Please check mandatory fields!!');";
                                cs.RegisterStartupScript(cstype, csname2, cstext1, true);
                            }
                        }
                    }
                    else if (databaseType.ToUpper().Equals("SQL"))
                    {
                        oradb = @"Data Source=" + server + ";Initial Catalog=" + dbDropDwnLst.SelectedItem.Text.ToString() + ";User ID=" + userIDTxt + ";Password=" + passwordTxt + ";";
                        sqlConn = new SqlConnection(oradb);
                        sqlConn.Open();
                        if (schemaList != "" && dbDropDwnLst.SelectedIndex != 0 && ddl4.Value != "0")
                        {
                            if (ddl4.Value.ToUpper().Equals("COLUMN"))
                            {
                                strSQL = "select DISTINCT Table_Schema as Entity_Schema, TABLE_NAME as Entity_Name, 'Table' as Entity_Type, 'Column' as Entity_Subtype,Data_type as Entity_DataType, COLUMN_NAME as Entity_Definition from information_schema.columns where TABLE_SCHEMA in (" + schemaList + ") and UPPER(COLUMN_NAME) LIKE UPPER('%" + strSearch + "%') UNION select Routine_schema as Entity_Schema,routine_name as Entity_Name ,'Routine' as Entity_Type, Routine_type as Entity_Subtype,'NA' as Entity_DataType,ROUTINE_DEFINITION as Entity_Definition from information_schema.ROUTINES where ROUTINE_SCHEMA in (" + schemaList + ") and UPPER(ROUTINE_DEFINITION) LIKE UPPER('%" + strSearch + "%') UNION select TABLE_SCHEMA as Entity_Schema,TABLE_NAME as Entity_Name, 'View' as Entity_Type, 'NA' as Entity_Subtype, 'NA' as Entity_DataType,VIEW_DEFINITION as Entity_Definition from information_schema.VIEWS where TABLE_SCHEMA in (" + schemaList + ") and Upper(VIEW_DEFINITION) LIKE UPPER('%" + strSearch + "%')";
                            }
                            else if (ddl4.Value.ToUpper().Equals("TABLE"))
                            {
                                strSQL = "select DISTINCT Routine_schema as Entity_Schema,routine_name as Entity_Name ,'Routine' as Entity_Type, Routine_type as Entity_Subtype,ROUTINE_DEFINITION as Entity_Definition from information_schema.ROUTINES  where ROUTINE_SCHEMA in (" + schemaList + ") and upper(ROUTINE_DEFINITION) LIKE UPPER('%" + strSearch + "%') UNION select TABLE_SCHEMA as Entity_Schema,TABLE_NAME as Entity_Name, 'Table' as Entity_Type, 'NA' as Entity_Subtype, 'NA' as Entity_Definition from information_schema.tables where TABLE_SCHEMA in (" + schemaList + ") and TABLE_NAME LIKE UPPER('%" + strSearch + "%') and TABLE_TYPE='BASE TABLE' UNION select TABLE_SCHEMA as Entity_Schema,TABLE_NAME as Entity_Name, 'View' as Entity_Type, 'NA' as Entity_Subtype, VIEW_DEFINITION as Entity_Definition from information_schema.VIEWS where TABLE_SCHEMA in (" + schemaList + ") and Upper(VIEW_DEFINITION) LIKE UPPER('%" + strSearch + "%')";

                            }
                            else if (ddl4.Value.ToUpper().Equals("ROUTINE"))
                            {
                                strSQL = "select DISTINCT Routine_schema as Entity_Schema,routine_name as Entity_Name ,'Routine' as Entity_Type, Routine_type as Entity_Subtype,ROUTINE_DEFINITION as Entity_Definition from information_schema.ROUTINES where ROUTINE_SCHEMA in (" + schemaList + ") and upper(ROUTINE_NAME) LIKE UPPER('%" + strSearch + "%')";

                            }
                            else if (ddl4.Value.ToUpper().Equals("VIEW"))
                            {
                                strSQL = "select DISTINCT TABLE_SCHEMA as Entity_Schema,TABLE_NAME as Entity_Name, 'View' as Entity_Type, 'NA' as Entity_Subtype, VIEW_DEFINITION as Entity_Definition from information_schema.VIEWS where TABLE_SCHEMA in (" + schemaList + ") and (upper(VIEW_DEFINITION) LIKE UPPER('%" + strSearch + "%') OR upper(TABLE_NAME) LIKE UPPER('%" + strSearch + "%'))";
                            }

                            sqlCmd = new SqlCommand(strSQL, sqlConn);
                            sqlCmd.CommandType = CommandType.Text;
                            SqlDataAdapter da1 = new SqlDataAdapter(sqlCmd);
                            DataTable dt = new DataTable();
                            da1.Fill(dt);
                            sqlGridview.DataSource = dt.DefaultView;
                            sqlGridview.DataBind();
                            sqlConn.Close();
                            GridviewDiv.Style.Add("display", "block");
                            sqlGridview.Style.Add("display", "block");
                            sqlGridview.UseAccessibleHeader = true;
                            sqlGridview.HeaderRow.TableSection = TableRowSection.TableHeader;
                            Session["SessionData1"] = dt;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop2", "openModal();", true);
                            noResultsFound.Style.Add("display", "none");
                            Logger.LogMessage("DB---SQL Server--LogIn by user--" + userIDTxt + "--Server--" + server + "--Database--" + dbDropDwnLst.SelectedValue.ToString());
                        }
                        else
                        {
                            GridviewDiv.Style.Add("display", "none");
                            noResultsFound.Style.Add("display", "none");
                            if (!cs.IsStartupScriptRegistered(cstype, csname5))
                            {
                                String cstext1 = "alert('Please check mandatory fields!!');";
                                cs.RegisterStartupScript(cstype, csname5, cstext1, true);
                            }
                        }
                    }
                    else if (databaseType.ToUpper().Equals("NETEZZA"))
                    {
                        oradb = @"Provider=NZOLEDB;Password=" + passwordTxt + ";User ID=" + userIDTxt + ";Data Source=" + server + ";Initial Catalog=" + DBValue + ";Persist Security Info=True;";
                        //oradb = @"Provider=NetezzaSQL;Data Source=" + server + ";Initial Catalog=" + dbDropDwnLst.SelectedItem.Text.ToString() + ";User ID=" + userID + ";Password=" + password + ";";
                        oldbCon = new OleDbConnection(oradb);
                        oldbCon.Open();
                        schemaList = string.Empty;
                        if (ddl4.Value.ToUpper().Equals("TABLE"))
                        {
                            for (int i = 0; i < schemaDrpdwnLst.Items.Count; i++)
                            {
                                if (schemaList != "" && schemaDrpdwnLst.Items[i].Selected.Equals(true))
                                {
                                    schemaList += " UNION ALL ";
                                }
                                if (schemaDrpdwnLst.Items[i].Selected.Equals(true))
                                {
                                    schemaList += "select DISTINCT DATABASE AS ENTITY_SCHEMA, TABLENAME AS ENTITY_NAME, OBJTYPE AS Entity_Type, 'NA' as Entity_Subtype, 'NA' as Entity_Definition from " + schemaDrpdwnLst.Items[i].Text + ".._v_table where upper(ENTITY_NAME) LIKE UPPER('%" + strSearch + "%') UNION  ALL select  DATABASE AS ENTITY_SCHEMA, VIEWNAME AS ENTITY_NAME, OBJTYPE AS Entity_Type, 'NA' as Entity_Subtype, DEFINITION as Entity_Definition  from  " + schemaDrpdwnLst.Items[i].Text + ".._v_view where upper(DEFINITION) LIKE UPPER('%" + strSearch + "%') UNION  select  DATABASE AS ENTITY_SCHEMA, PROCEDURE AS ENTITY_NAME, 'Routine' as Entity_Type, OBJTYPE as Entity_Subtype, PROCEDURESOURCE as Entity_Definition from  " + schemaDrpdwnLst.Items[i].Text + ".._v_procedure where upper(PROCEDURESOURCE) LIKE UPPER('%" + strSearch + "%') UNION  select  DATABASE AS ENTITY_SCHEMA, SYNONYM_NAME AS ENTITY_NAME, OBJTYPE AS Entity_Type, 'NA' as Entity_Subtype, 'NA' as Entity_Definition  from  " + schemaDrpdwnLst.Items[i].Text + ".._v_synonym where UPPER(REFOBJNAME) LIKE UPPER('%" + strSearch + "%')";
                                }
                            }
                            strSQL = schemaList;
                            //                                strSQL = "select DISTINCT SCHEMA AS ENTITY_SCHEMA, TABLENAME AS ENTITY_NAME, OBJTYPE AS Entity_Type, 'NA' as Entity_Subtype, 'NA' as Entity_Definition from  _v_table where SCHEMA in (" + schemaList + ") AND upper(ENTITY_NAME) LIKE UPPER('%" + strSearch + "%') UNION  ALL select  SCHEMA AS ENTITY_SCHEMA, VIEWNAME AS ENTITY_NAME, OBJTYPE AS Entity_Type, 'NA' as Entity_Subtype, DEFINITION as Entity_Definition  from  _v_view where SCHEMA in (" + schemaList + ") AND upper(DEFINITION) LIKE UPPER('%" + strSearch + "%') UNION  select  SCHEMA AS ENTITY_SCHEMA, PROCEDURE AS ENTITY_NAME, 'Routine' as Entity_Type, OBJTYPE as Entity_Subtype, PROCEDURESOURCE as Entity_Definition from  _v_procedure where SCHEMA in (" + schemaList + ") AND upper(PROCEDURESOURCE) LIKE UPPER('%" + strSearch + "%') UNION  select  SCHEMA AS ENTITY_SCHEMA, SYNONYM_NAME AS ENTITY_NAME, OBJTYPE AS Entity_Type, 'NA' as Entity_Subtype, 'NA' as Entity_Definition  from  _v_synonym where SCHEMA in (" + schemaList + ") AND UPPER(REFOBJNAME) LIKE UPPER('%" + strSearch + "%')";
                        }
                        else if (ddl4.Value.ToUpper().Equals("COLUMN"))
                        {
                            for (int i = 0; i < schemaDrpdwnLst.Items.Count; i++)
                            {
                                if (schemaList != "" && schemaDrpdwnLst.Items[i].Selected.Equals(true))
                                {
                                    schemaList += " UNION ALL ";
                                }
                                if (schemaDrpdwnLst.Items[i].Selected.Equals(true))
                                {
                                    schemaList += "select DISTINCT DATABASE AS ENTITY_SCHEMA, TABLE_NAME AS ENTITY_NAME, 'Table' AS Entity_Type, 'Column' as Entity_Subtype,Type_name as Entity_DataType,Column_size as Entity_Size, COLUMN_NAME as Entity_Definition from " + schemaDrpdwnLst.Items[i].Text + ".._v_sys_columns where UPPER(COLUMN_NAME) LIKE UPPER('%" + strSearch + "%') UNION select  DATABASE AS ENTITY_SCHEMA, VIEWNAME AS ENTITY_NAME, OBJTYPE AS Entity_Type, 'NA' as Entity_Subtype,'NA' as Entity_DataType,'0' as Entity_Size, DEFINITION as Entity_Definition from " + schemaDrpdwnLst.Items[i].Text + ".._v_view where upper(DEFINITION) LIKE UPPER('%" + strSearch + "%') UNION select DATABASE AS ENTITY_SCHEMA, PROCEDURE AS ENTITY_NAME, 'Routine' as Entity_Type, OBJTYPE as Entity_Subtype, 'NA' as Entity_DataType,'0' as Entity_Size,PROCEDURESOURCE as Entity_Definition from " + schemaDrpdwnLst.Items[i].Text + ".._v_procedure where (upper(PROCEDURESOURCE) LIKE UPPER('%" + strSearch + "%') OR upper(PROCEDURE) LIKE UPPER('%" + strSearch + "%'))";
                                }
                            }
                            strSQL = schemaList;

                            //strSQL = "select DISTINCT SCHEMA AS ENTITY_SCHEMA, TABLE_NAME AS ENTITY_NAME, 'Table' AS Entity_Type, 'Column' as Entity_Subtype,Type_name as Entity_DataType,Column_size as Entity_Size, COLUMN_NAME as Entity_Definition from _v_sys_columns where SCHEMA in (" + schemaList + ") AND UPPER(COLUMN_NAME) LIKE UPPER('%" + strSearch + "%') UNION  select  SCHEMA AS ENTITY_SCHEMA, VIEWNAME AS ENTITY_NAME, OBJTYPE AS Entity_Type, 'NA' as Entity_Subtype,'NA' as Entity_DataType,'0' as Entity_Size, DEFINITION as Entity_Definition from _v_view where SCHEMA in (" + schemaList + ") AND upper(DEFINITION) LIKE UPPER('%" + strSearch + "%') UNION select SCHEMA AS ENTITY_SCHEMA, PROCEDURE AS ENTITY_NAME, 'Routine' as Entity_Type, OBJTYPE as Entity_Subtype, 'NA' as Entity_DataType,'0' as Entity_Size,PROCEDURESOURCE as Entity_Definition from _v_procedure where SCHEMA in (" + schemaList + ") AND (upper(PROCEDURESOURCE) LIKE UPPER('%" + strSearch + "%') OR upper(PROCEDURE) LIKE UPPER('%" + strSearch + "%'))";
                        }
                        else if (ddl4.Value.ToUpper().Equals("ROUTINE"))
                        {
                            for (int i = 0; i < schemaDrpdwnLst.Items.Count; i++)
                            {
                                if (schemaList != "" && schemaDrpdwnLst.Items[i].Selected.Equals(true))
                                {
                                    schemaList += " UNION ALL ";
                                }
                                if (schemaDrpdwnLst.Items[i].Selected.Equals(true))
                                {
                                    schemaList += "select DISTINCT DATABASE AS ENTITY_SCHEMA, PROCEDURE AS ENTITY_NAME, 'Routine' as Entity_Type, OBJTYPE as Entity_Subtype, PROCEDURESOURCE as Entity_Definition  from  " + schemaDrpdwnLst.Items[i].Text + ".._v_procedure where (upper(PROCEDURESOURCE) LIKE UPPER('%" + strSearch + "%') OR upper(PROCEDURE) LIKE UPPER('%" + strSearch + "%'))";
                                }
                            }
                            strSQL = schemaList;
                            //strSQL = "select DISTINCT SCHEMA AS ENTITY_SCHEMA, PROCEDURE AS ENTITY_NAME, 'Routine' as Entity_Type, OBJTYPE as Entity_Subtype, PROCEDURESOURCE as Entity_Definition  from  _v_procedure where SCHEMA in (" + schemaList + ") AND (upper(PROCEDURESOURCE) LIKE UPPER('%" + strSearch + "%') OR upper(PROCEDURE) LIKE UPPER('%" + strSearch + "%'))";

                        }
                        else if (ddl4.Value.ToUpper().Equals("VIEW"))
                        {
                            for (int i = 0; i < schemaDrpdwnLst.Items.Count; i++)
                            {
                                if (schemaList != "" && schemaDrpdwnLst.Items[i].Selected.Equals(true))
                                {
                                    schemaList += " UNION ALL ";
                                }
                                if (schemaDrpdwnLst.Items[i].Selected.Equals(true))
                                {
                                    schemaList += "select DISTINCT DATABASE AS ENTITY_SCHEMA, VIEWNAME AS ENTITY_NAME, OBJTYPE AS Entity_Type, 'NA' as Entity_Subtype, DEFINITION as Entity_Definition  from  " + schemaDrpdwnLst.Items[i].Text + ".._v_view where (upper(DEFINITION) LIKE UPPER('%" + strSearch + "%') OR upper(VIEWNAME) LIKE UPPER('%" + strSearch + "%'))";
                                }
                            }
                            strSQL = schemaList;
                            //strSQL = "select DISTINCT SCHEMA AS ENTITY_SCHEMA, VIEWNAME AS ENTITY_NAME, OBJTYPE AS Entity_Type, 'NA' as Entity_Subtype, DEFINITION as Entity_Definition  from  _v_view where SCHEMA in (" + schemaList + ") AND (upper(DEFINITION) LIKE UPPER('%" + strSearch + "%') OR upper(VIEWNAME) LIKE UPPER('%" + strSearch + "%'))";
                            //UNION  select  SCHEMA AS ENTITY_SCHEMA, PROCEDURE AS ENTITY_NAME, 'Routine' as Entity_Type, OBJTYPE as Entity_Subtype, PROCEDURESOURCE as Entity_Definition from  _v_procedure where SCHEMA in (" + schemaList + ") AND PROCEDURESOURCE LIKE UPPER('%" + strSearch + "%')
                        }
                        if (schemaList != "" && dbDropDwnLst.SelectedIndex != 0 && ddl4.Value != "0")
                        {
                            //OleDbDataReader dreader;                        
                            oldbCmd = new OleDbCommand(strSQL, oldbCon);
                            oldbCmd.CommandType = CommandType.Text;
                            OleDbDataAdapter da1 = new OleDbDataAdapter(oldbCmd);
                            DataTable dt = new DataTable();
                            da1.Fill(dt);
                            sqlGridview.DataSource = dt.DefaultView;
                            sqlGridview.DataBind();
                            oldbCon.Close();
                            GridviewDiv.Style.Add("display", "block");
                            sqlGridview.Style.Add("display", "block");
                            sqlGridview.UseAccessibleHeader = true;
                            sqlGridview.HeaderRow.TableSection = TableRowSection.TableHeader;
                            Session["SessionData1"] = dt;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop3", "openModal();", true);
                            noResultsFound.Style.Add("display", "none");
                            Logger.LogMessage("DB---Netezza--LogIn by user--" + userIDTxt + "--Server--" + server + "--Database--" + DBValue);
                        }
                        else
                        {
                            GridviewDiv.Style.Add("display", "none");
                            noResultsFound.Style.Add("display", "none");
                            // Check to see if the startup script is already registered.
                            if (!cs.IsStartupScriptRegistered(cstype, csname5))
                            {
                                String cstext1 = "alert('Please check mandatory fields!!');";
                                cs.RegisterStartupScript(cstype, csname5, cstext1, true);
                            }
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    Logger.LogException(sqlEx);
                    if (!cs.IsStartupScriptRegistered(cstype, csname2))
                    {
                        String cstext1 = "alert('Login Failed!!');";
                        cs.RegisterStartupScript(cstype, csname2, cstext1, true);
                    }
                    ScriptManager.RegisterStartupScript(this, typeof(System.Web.UI.Page), "ModalPop6", "openAuthenticationModal();", true);
                }
                catch (OracleException orEx)
                {
                    Logger.LogException(orEx);
                    if (!cs.IsStartupScriptRegistered(cstype, csname2))
                    {
                        String cstext1 = "alert('Login Failed!!');";
                        cs.RegisterStartupScript(cstype, csname2, cstext1, true);
                    }
                    ScriptManager.RegisterStartupScript(this, typeof(System.Web.UI.Page), "ModalPop7", "openAuthenticationModal();", true);
                }
                catch (OleDbException oldbEx)
                {
                    Logger.LogException(oldbEx);
                    if (!cs.IsStartupScriptRegistered(cstype, csname2))
                    {
                        String cstext1 = "alert('Login Failed!!');";
                        cs.RegisterStartupScript(cstype, csname2, cstext1, true);
                    }
                    ScriptManager.RegisterStartupScript(this, typeof(System.Web.UI.Page), "ModalPop8", "openAuthenticationModal();", true);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    noResultsFound.Style.Add("display", "block");
                    //Response.Redirect("~/TechnicalError.aspx");
                }

            }
            else if (platformValue.ToLower() == "linux")
            {
                try
                {
                    string strDirectory = string.Empty;
                    for (int i = 0; i < dirDrpdwnList.Items.Count; i++)
                    {
                        if (strDirectory != "" && dirDrpdwnList.Items[i].Selected.Equals(true))
                        {
                            strDirectory += ",";
                        }
                        if (dirDrpdwnList.Items[i].Selected.Equals(true))
                        {
                            strDirectory += "'" + dirDrpdwnList.Items[i].Text + "'";
                        }
                    }
                    if (serverDropDwnLst.SelectedIndex != 0 && !string.IsNullOrEmpty(strDirectory))
                    {
                        GetServerDetails(serverDropDwnLst.SelectedItem.Text.ToString(), out userID, out team);

                        using (var client = new SshClient(serverDropDwnLst.SelectedItem.Text.ToString(), userIDTxt, passwordTxt))
                        {
                            client.Connect();
                            if (team == "Team1")
                            {
                                client.RunCommand("/bin/ksh /home/" + "userIDTxt.ToString()" + "/IAToolLinuxService/main1.sh " + strDirectory.Trim().ToString() + " " + userIDTxt.ToString() + " " + serverDropDwnLst.SelectedItem.Text.ToString() + " " + strSearch.Trim().ToString());
                                using (var sftp = new SftpClient(serverDropDwnLst.SelectedItem.Text.ToString(), userIDTxt, passwordTxt))
                                {
                                    sftp.Connect();
                                    try
                                    {
                                        //Reading from the SSH channel                                      
                                        using (var FileStream = new FileStream("C:\\users\\public\\datafile.txt", FileMode.Create, FileAccess.Write, FileShare.None))
                                        {
                                            sftp.DownloadFile("/home/" + "userIDTxt.ToString()" + "/IAToolLinuxService/IA_tool_output_file.csv", FileStream, null);
                                        }
                                        client.RunCommand("/bin/ksh /home/" + "userIDTxt.ToString()" + "/IAToolLinuxService/archive_script.sh");
                                        client.Disconnect();
                                        sftp.Disconnect();
                                        BindCSVData("C:\\users\\public\\datafile.txt");
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.LogException(ex);
                                        noResultsFound.Style.Add("display", "block");
                                        //Response.Redirect("~/TechnicalError.aspx");
                                    }

                                }
                            }
                            else if (team == "Team2")
                            {
                                client.RunCommand("/bin/ksh /home/" + userIDTxt.ToString() + "/IAToolLinuxService/main1.sh " + strDirectory.Trim().ToString() + " " + userIDTxt.ToString() + " " + serverDropDwnLst.SelectedItem.Text.ToString() + " " + strSearch.Trim().ToString());
                                using (var sftp = new SftpClient(serverDropDwnLst.SelectedItem.Text.ToString(), userIDTxt, passwordTxt))
                                {
                                    sftp.Connect();
                                    try
                                    {
                                        //Reading from the SSH channel                                      
                                        using (var FileStream = new FileStream("C:\\users\\public\\datafile.txt", FileMode.Create, FileAccess.Write, FileShare.None))
                                        {
                                            sftp.DownloadFile("/home/" + userIDTxt.ToString() + "/IAToolLinuxService/IA_tool_output_file.csv", FileStream, null);
                                        }
                                        client.RunCommand("/bin/ksh /home/" + userIDTxt.ToString() + "/IAToolLinuxService/archive_script.sh");
                                        client.Disconnect();
                                        sftp.Disconnect();
                                        BindCSVData("C:\\users\\public\\datafile.txt");
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.LogException(ex);
                                        noResultsFound.Style.Add("display", "block");
                                        //Response.Redirect("~/TechnicalError.aspx");
                                    }
                                }
                            }
                        }


                    }

                    else
                    {
                        GridviewDiv.Style.Add("display", "none");
                        noResultsFound.Style.Add("display", "none");
                        // Check to see if the startup script is already registered.
                        if (!cs.IsStartupScriptRegistered(cstype, csname4))
                        {
                            String cstext1 = "alert('Please check mandatory fields!!');";
                            cs.RegisterStartupScript(cstype, csname4, cstext1, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    //Response.Redirect("~/TechnicalError.aspx");
                    if (!cs.IsStartupScriptRegistered(cstype, csname6))
                    {
                        String cstext1 = "alert('Please try again');";
                        cs.RegisterStartupScript(cstype, csname6, cstext1, true);
                    }
                    ScriptManager.RegisterStartupScript(this, typeof(System.Web.UI.Page), "ModalPop2", "openAuthenticationModal();", true);
                }

            }
            else if (platformValue.ToLower() == "github")
            {
                try
                {
                    Page<JsonResponse.Item> response = null;
                    List<JsonResponse.Item> lsts = null;
                    DataTable dt = new DataTable();
                    if (appDropDwnLst.SelectedIndex != 0 && repoDropDownList.SelectedIndex != 0)
                    {
                        response = GetGithubResults(userIDTxt, passwordTxt, strSearch.Trim().ToString(), repoDropDownList.SelectedValue.ToString());
                        if (response.total_count > 0)
                        {
                            dt.Columns.Add("File_Name", typeof(string));
                            dt.Columns.Add("File_Path", typeof(string));
                            dt.Columns.Add("Repository_Name", typeof(string));
                            dt.Columns.Add("Repository_Path", typeof(string));
                            lsts = response.Items;
                            foreach (JsonResponse.Item item in lsts)
                            {
                                DataRow row = dt.NewRow();
                                row["File_Name"] = item.name;
                                row["File_Path"] = item.path;
                                row["Repository_Name"] = item.repository.full_name;
                                row["Repository_Path"] = item.repository.html_url;
                                dt.Rows.Add(row);
                            }
                            githubGridview.DataSource = dt.DefaultView;
                            githubGridview.DataBind();
                            GridviewDiv.Style.Add("display", "block");
                            githubGridview.Style.Add("display", "block");
                            githubGridview.UseAccessibleHeader = true;
                            githubGridview.HeaderRow.TableSection = TableRowSection.TableHeader;
                            Session["SessionData3"] = dt;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop8", "openModal();", true);
                            noResultsFound.Style.Add("display", "none");
                            Logger.LogMessage("DB---Github--LogIn by user--" + userIDTxt + "--Repository--" + repoDropDownList.SelectedValue.ToString());
                        }
                        else
                            noResultsFound.Style.Add("display", "block");
                    }

                    else
                    {
                        GridviewDiv.Style.Add("display", "none");
                        noResultsFound.Style.Add("display", "none");
                        // Check to see if the startup script is already registered.
                        if (!cs.IsStartupScriptRegistered(cstype, csname4))
                        {
                            String cstext1 = "alert('Please check mandatory fields!!');";
                            cs.RegisterStartupScript(cstype, csname4, cstext1, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    //Response.Redirect("~/TechnicalError.aspx");
                    if (!cs.IsStartupScriptRegistered(cstype, csname6))
                    {
                        String cstext1 = "alert('Please enter correct UserID and Password');";
                        cs.RegisterStartupScript(cstype, csname6, cstext1, true);
                    }
                    ScriptManager.RegisterStartupScript(this, typeof(System.Web.UI.Page), "ModalPop2", "openAuthenticationModal();", true);
                }

            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Type cstype1 = this.GetType();
            // Get a ClientScriptManager reference from the Page class.
            ClientScriptManager cs1 = Page.ClientScript;
            Session["state"] = ddl4.Value.ToString();
            contextValue.Value = Session["state"].ToString();
            strSearch = Txt2.Value;
            for (int i = 0; i < lstFolders.Items.Count; i++)
            {
                if (folderList != "" && lstFolders.Items[i].Selected.Equals(true))
                {
                    folderList += ",";
                }
                if (lstFolders.Items[i].Selected.Equals(true))
                {
                    folderList += "'" + lstFolders.Items[i].Text + "'";
                }
            }
            //If the selected platform is INFORMATICA                        
            if (platformValue.ToLower() == "informatica")
            {
                try
                {
                    userIDTxt = ViewState["UserID"].ToString();
                    passwordTxt = ViewState["Password"].ToString();
                    SIDName = serverDropdownLst.SelectedItem.Text;
                    host = ViewState["Host"].ToString();
                    port = ViewState["Port"].ToString();
                    schemaName = ViewState["schemaName"].ToString();
                    //string oradb = ConfigurationManager.ConnectionStrings["strConnectionString"].ConnectionString;
                    //string oradb = @"Data Source=  (DESCRIPTION =              (ADDRESS_LIST =                  (ADDRESS =                      (HOST =lazermark.am.lilly.com)                      (PROTOCOL = TCP) (PORT = 1530)                 )              )              (CONNECT_DATA = (SID = prd154)             )         ) ;User Id=" + userIDTxt + ";Password=" + passwordTxt + ";Persist Security Info=True;";                                            
                    string oradb = @"Data Source=  (DESCRIPTION =(ADDRESS_LIST =                  (ADDRESS =                      (HOST = " + host + ")                      (PROTOCOL = TCP) (PORT = " + port + ")                 )              )              (CONNECT_DATA = (SID = " + SIDName + ")             )         ) ;User Id=" + userIDTxt + ";Password=" + passwordTxt + ";Persist Security Info=True;";
                    objConn = new OracleConnection(oradb);
                    objConn.Open();

                    if (folderList != "" && ddl4.Value != "0")
                    {
                        //if (ddl4.Value.Equals("0"))
                        //{
                        //    strSQL = "SELECT DISTINCT d.subject_area AS Folder, d.mapping_name AS Mapping,a.widget_type_name AS Transformation_Type, a.instance_name as Transformation_Name,b.attr_name as Attribute_Name FROM in9_prd_repo.REP_WIDGET_INST a, in9_prd_repo.REP_WIDGET_ATTR b,in9_prd_repo.REP_LOAD_SESSIONS c, in9_prd_repo.REP_ALL_MAPPINGS d WHERE b.widget_id = a. widget_id AND b.widget_type = a.widget_type AND b.widget_type in (3, 11) AND c.mapping_id = a.mapping_id AND d.mapping_id = a.mapping_id AND b.attr_id= 1 AND d.subject_area in (" + folderList + ") AND b.attr_datatype=2 and b.attr_type=3 ORDER BY d.subject_area, d.mapping_name";
                        //}                        
                        if (ddl4.Value.ToUpper().Equals("CONNECTION"))
                        {
                            strSQL = "SELECT WF.SUBJECT_AREA AS FOLDER_NAME,WF.WORKFLOW_NAME AS WORKFLOW_NAME,T.INSTANCE_NAME AS SESSION_NAME,T.TASK_TYPE_NAME,C.CNX_NAME AS CONNECTION_NAME,V.CONNECTION_SUBTYPE,V.HOST_NAME,V.USER_NAME,C.INSTANCE_NAME,C.READER_WRITER_TYPE FROM " + schemaName + ".REP_TASK_INST T," + schemaName + ".REP_SESS_WIDGET_CNXS C," + schemaName + ".REP_WORKFLOWS WF," + schemaName + ".V_IME_CONNECTION V WHERE T.TASK_ID = C.SESSION_ID AND WF.WORKFLOW_ID = T.WORKFLOW_ID AND C.CNX_NAME = V.CONNECTION_NAME AND (UPPER (C.CNX_NAME) LIKE UPPER ('%" + strSearch + "%') OR UPPER (V.HOST_NAME) LIKE UPPER ('%" + strSearch + "%')) AND WF.SUBJECT_AREA IN (" + folderList + ")";
                        }
                        else if (ddl4.Value.ToUpper().Equals("KEYWORD"))
                        {
                            //strSQL = "SELECT DISTINCT d.subject_area AS Folder, d.mapping_name AS Mapping, a.widget_type_name AS Transformation_Type, a.instance_name as Transformation_Name, b.attr_name as Attribute_Name FROM "+schemaName+".REP_WIDGET_INST a, "+schemaName+".REP_WIDGET_ATTR b, "+schemaName+".REP_LOAD_SESSIONS c, "+schemaName+".REP_ALL_MAPPINGS d WHERE b.widget_id = a. widget_id AND b.widget_type = a. widget_type AND b.widget_type in (3, 11) AND c.mapping_id = a.mapping_id AND d.mapping_id = a.mapping_id AND b.attr_id= 1 and UPPER(b.attr_value) like UPPER('%" + strSearch + "%') AND d.subject_area in (" + folderList + ") AND b.attr_datatype=2 and b.attr_type=3 ORDER BY d.subject_area, d.mapping_name";
                            //strSQL = "SELECT d.subject_area AS Folder,d.mapping_name AS Mapping,a.widget_type_name AS Transformation_Type,a.instance_name AS Transformation_Name,b.attr_name AS Attribute_Name FROM " + schemaName + ".REP_WIDGET_INST a, " + schemaName + ".REP_WIDGET_ATTR b, " + schemaName + ".REP_LOAD_SESSIONS c, " + schemaName + ".REP_ALL_MAPPINGS d WHERE b.widget_id = a.widget_id AND b.widget_type = a.widget_type AND b.widget_type IN (3, 11) AND c.mapping_id = a.mapping_id AND d.mapping_id = a.mapping_id AND b.attr_id = 1 AND UPPER (b.attr_value) LIKE UPPER ('%" + strSearch + "%') AND d.subject_area IN (" + folderList + ") AND b.attr_datatype = 2 AND b.attr_type = 3 UNION SELECT rep_all_mappings.subject_area AS Folder, rep_all_mappings.mapping_name, rep_widget_inst.widget_type_name AS transformation_type, rep_widget_inst.instance_name AS transformation_name, rep_widget_field.field_name AS port_name FROM " + schemaName + ".rep_widget_inst, " + schemaName + ".rep_widget_field, " + schemaName + ".rep_all_mappings WHERE rep_widget_inst.widget_id = rep_widget_field.widget_id AND rep_widget_inst.mapping_id = rep_all_mappings.mapping_id AND UPPER (rep_widget_field.field_name) LIKE UPPER ('%" + strSearch + "%') AND rep_all_mappings.subject_area IN (" + folderList + ") UNION SELECT subject_area AS Folder, mapping_name, 'SOURCE DEFINITION' AS transformation_type, 'SOURCE' AS Transformation_Name, SOURCE_NAME FROM " + schemaName + ".REP_SRC_MAPPING WHERE UPPER (SOURCE_NAME) LIKE UPPER ('%" + strSearch + "%') AND subject_area IN (" + folderList + ") UNION SELECT subject_area AS Folder, mapping_name, 'TARGET DEFINITION' AS transformation_type, 'TARGET' AS Transformation_Name, TARGET_NAME FROM " + schemaName + ".REP_TARG_MAPPING WHERE UPPER (TARGET_NAME) LIKE UPPER ('%" + strSearch + "%') AND subject_area IN (" + folderList + ")";
                            strSQL= "SELECT d.subject_area AS Folder, d.mapping_name AS Mapping, a.widget_type_name AS Transformation_Type, a.instance_name AS Transformation_Name, b.attr_name AS Attribute_Name, null AS datatype, null as prec, null as scale FROM " + schemaName + ".REP_WIDGET_INST a, "+schemaName+".REP_WIDGET_ATTR b, "+schemaName+".REP_LOAD_SESSIONS c, "+schemaName+".REP_ALL_MAPPINGS d WHERE b.widget_id = a.widget_id AND b.widget_type = a.widget_type AND b.widget_type IN (3, 11) AND c.mapping_id = a.mapping_id AND d.mapping_id = a.mapping_id AND b.attr_id = 1 AND UPPER (b.attr_value) LIKE UPPER ('%" + strSearch + "%') AND d.subject_area IN (" + folderList + ") AND b.attr_datatype = 2 AND b.attr_type = 3 UNION SELECT rep_all_mappings.subject_area AS Folder, rep_all_mappings.mapping_name, rep_widget_inst.widget_type_name AS transformation_type, rep_widget_inst.instance_name AS transformation_name, rep_widget_field.field_name AS port_name, rep_widget_field.datatype AS datatype, rep_widget_field.WGT_PREC as prec, rep_widget_field.WGT_SCALE as scale FROM "+schemaName+".rep_widget_inst, "+schemaName+".rep_widget_field, "+schemaName+".rep_all_mappings WHERE     rep_widget_inst.widget_id = rep_widget_field.widget_id AND rep_widget_inst.mapping_id = rep_all_mappings.mapping_id AND UPPER (rep_widget_field.field_name) LIKE UPPER ('%" + strSearch + "%') AND rep_all_mappings.subject_area IN (" + folderList + ") UNION SELECT subject_area AS Folder, mapping_name, 'SOURCE DEFINITION' AS transformation_type, 'SOURCE' AS Transformation_Name, SOURCE_NAME, null AS datatype, null as prec, null as scale FROM "+schemaName+".REP_SRC_MAPPING WHERE UPPER (SOURCE_NAME) LIKE UPPER ('%" + strSearch + "%') AND subject_area IN (" + folderList + ") UNION SELECT subject_area AS Folder, mapping_name, 'TARGET DEFINITION' AS transformation_type, 'TARGET' AS Transformation_Name, TARGET_NAME, null AS datatype, null as prec, null as scale FROM "+schemaName+".REP_TARG_MAPPING WHERE UPPER (TARGET_NAME) LIKE UPPER ('%" + strSearch + "%') AND subject_area IN (" + folderList + ")";
                        }
                        else if (ddl4.Value.ToUpper().Equals("FUNCTION"))
                        {
                            strSQL = "SELECT DISTINCT REP_ALL_MAPPINGS.SUBJECT_AREA AS Folder, REP_ALL_MAPPINGS.MAPPING_NAME AS Mapping,REP_WIDGET_INST.WIDGET_TYPE_NAME AS Transformation_Type,REP_WIDGET_INST.INSTANCE_NAME AS Transformation_Name,REP_WIDGET_FIELD.FIELD_NAME AS Port_Name,CASE WHEN REP_WIDGET_FIELD.PORTTYPE = 1 THEN 'I' WHEN REP_WIDGET_FIELD.PORTTYPE = 2 THEN 'O' WHEN REP_WIDGET_FIELD.PORTTYPE = 3 THEN 'IO' WHEN REP_WIDGET_FIELD.PORTTYPE = 32 THEN 'V' END AS PORT_TYPE, REP_WIDGET_FIELD.EXPRESSION FROM " + schemaName + ".REP_WIDGET_INST, " + schemaName + ".REP_WIDGET_FIELD, " + schemaName + ".REP_ALL_MAPPINGS WHERE REP_WIDGET_INST.WIDGET_ID = REP_WIDGET_FIELD.WIDGET_ID AND REP_WIDGET_INST.MAPPING_ID = REP_ALL_MAPPINGS.MAPPING_ID AND REP_WIDGET_INST.WIDGET_TYPE = 5 AND UPPER(REP_WIDGET_FIELD.EXPRESSION) LIKE UPPER('%" + strSearch + "%') AND REP_ALL_MAPPINGS.SUBJECT_AREA in (" + folderList + ") ORDER BY 1";
                        }

                        objCmd = new OracleCommand(strSQL, objConn);
                        objCmd.CommandType = CommandType.Text;
                        OracleDataAdapter da1 = new OracleDataAdapter(objCmd);
                        DataTable dt1 = new DataTable();
                        da1.Fill(dt1);
                        informaticaGridview.DataSource = dt1.DefaultView;
                        informaticaGridview.DataBind();
                        GridviewDiv.Style.Add("display", "block");
                        informaticaGridview.Style.Add("display", "block");
                        informaticaGridview.UseAccessibleHeader = true;
                        informaticaGridview.HeaderRow.TableSection = TableRowSection.TableHeader;
                        Session["SessionData"] = dt1;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
                        noResultsFound.Style.Add("display", "none");
                        Logger.LogMessage("Platform---Informatica--LogIn by user--" + userIDTxt + "--Host--" + host + "--Database--" + SIDName);

                    }
                    else
                    {
                        noResultsFound.Style.Add("display", "none");
                        //Check to see if the startup script is already registered.
                        if (!cs1.IsStartupScriptRegistered(cstype1, csname1))
                        {
                            String cstext1 = "alert('Please select all the mandatory fields!!');";
                            cs1.RegisterStartupScript(cstype1, csname1, cstext1, true);
                        }

                    }
                }
                catch
                {
                    //Response.Redirect("~/TechnicalError.aspx");
                    //Response.Write();
                    noResultsFound.Style.Add("display", "block");
                }
            }
            //If the selected platform is SQL
            else if (platformValue.ToLower() == "sql")
            {
                int flag = CurrentDatabaseValue(dbDropDwnLst.SelectedItem.Text.ToUpper().ToString());
                if (flag == 0)
                    ScriptManager.RegisterStartupScript(this, typeof(System.Web.UI.Page), "ModalPop1", "openAuthenticationModal();", true);
                else
                    search_Results_Click(sender, e);
            }
            //If the selected platform is SHELL
            else if (platformValue.ToLower() == "linux")
            {
                int flag = CurrentDatabaseValue(serverDropDwnLst.SelectedItem.Text.ToUpper().ToString());
                if (flag == 0)
                    ScriptManager.RegisterStartupScript(this, typeof(System.Web.UI.Page), "ModalPop2", "openAuthenticationModal();", true);
                else
                    search_Results_Click(sender, e);
            }
            else if (platformValue.ToLower() == "github")
            {
                int flag = CurrentDatabaseValue(appDropDwnLst.SelectedItem.Text.ToUpper().ToString());
                if (flag == 0)
                    ScriptManager.RegisterStartupScript(this, typeof(System.Web.UI.Page), "ModalPop3", "openAuthenticationModal();", true);
                else
                    search_Results_Click(sender, e);
            }
        }
        /// <summary>
        /// To bind CSV data in a gridview
        /// </summary>
        /// <param name="path"></param>
        protected void BindCSVData(string path)
        {
            //Creating object of datatable  
            DataTable tblcsv = new DataTable();
            //creating columns  
            tblcsv.Columns.Add("Permission");
            tblcsv.Columns.Add("User");
            tblcsv.Columns.Add("Group");
            tblcsv.Columns.Add("File Size (Bytes)");
            tblcsv.Columns.Add("Modified Date");
            tblcsv.Columns.Add("File Name");
            //getting full file path of Uploaded file                          
            string ReadCSV = File.ReadAllText(path);
            //Reading All text  
            //spliting row after new line  
            foreach (string csvRow in ReadCSV.Split('\n'))
            {
                if (!string.IsNullOrEmpty(csvRow))
                {
                    //Adding each row into datatable  
                    tblcsv.Rows.Add();
                    int count = 0;
                    foreach (string FileRec in csvRow.Split(','))
                    {
                        tblcsv.Rows[tblcsv.Rows.Count - 1][count] = FileRec;
                        count++;
                    }
                }

            }
            //Calling Bind Grid Functions  
            shellGridview.DataSource = tblcsv;
            shellGridview.DataBind();
            GridviewDiv.Style.Add("display", "block");
            shellGridview.Style.Add("display", "block");
            shellGridview.UseAccessibleHeader = true;
            shellGridview.HeaderRow.TableSection = TableRowSection.TableHeader;
            Session["SessionData2"] = tblcsv;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop4", "openModal();", true);
            noResultsFound.Style.Add("display", "none");
        }


        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Confirms that an HtmlForm control is rendered for the specified ASP.NET
               server control at run time. */
        }
        /// <summary>
        /// To insert the Gridview data into a excel sheet
        /// </summary>
        /// <returns></returns>
        protected StringWriter GridviewToExcel()
        {
            DateTime now = DateTime.Now;
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=IA_" + platformValue + "_" + envtValue + "_" + Session["state"].ToString() + "_" + ViewState["UserID"] + "_" + strSearch + "_" + now.ToString("dd-MMM-yyyy") + ".xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            if (platformValue.ToLower() == "informatica")
            {
                informaticaGridview.AllowPaging = false;
                informaticaGridview.DataSource = ((DataTable)Session["SessionData"]).DefaultView;
                informaticaGridview.DataBind();
                informaticaGridview.UseAccessibleHeader = true;
                informaticaGridview.HeaderRow.TableSection = TableRowSection.TableHeader;

                //Change the Header Row back to white color
                informaticaGridview.HeaderRow.Style.Add("background-color", "#FFFFFF");

                //Apply style to Individual Cells            
                for (int i = 0; i < informaticaGridview.HeaderRow.Cells.Count; i++)
                {
                    informaticaGridview.HeaderRow.Cells[i].Style.Add("background-color", "#286090");
                    informaticaGridview.FooterRow.Cells[i].Style.Add("background-color", "#286090");
                    LinkButton headerRowCell = (LinkButton)informaticaGridview.HeaderRow.Cells[i].Controls[0];
                    headerRowCell.Enabled = false;
                }
                foreach (GridViewRow row in informaticaGridview.Rows)
                {
                    row.BackColor = Color.White;
                    foreach (TableCell cell in row.Cells)
                    {
                        if (row.RowIndex % 2 == 0)
                        {
                            cell.BackColor = informaticaGridview.AlternatingRowStyle.BackColor;
                        }
                        else
                        {
                            cell.BackColor = informaticaGridview.RowStyle.BackColor;
                        }
                        cell.CssClass = "textmode";
                    }
                }

                informaticaGridview.RenderControl(hw);
            }
            else if (platformValue.ToLower() == "sql")
            {

                sqlGridview.AllowPaging = false;
                sqlGridview.DataSource = ((DataTable)Session["SessionData1"]).DefaultView;
                sqlGridview.DataBind();
                sqlGridview.UseAccessibleHeader = true;
                sqlGridview.HeaderRow.TableSection = TableRowSection.TableHeader;

                //Change the Header Row back to white color
                sqlGridview.HeaderRow.Style.Add("background-color", "#FFFFFF");

                //Apply style to Individual Cells            
                for (int i = 0; i < sqlGridview.HeaderRow.Cells.Count; i++)
                {
                    sqlGridview.HeaderRow.Cells[i].Style.Add("background-color", "#286090");
                    sqlGridview.FooterRow.Cells[i].Style.Add("background-color", "#286090");
                    LinkButton headerRowCell = (LinkButton)sqlGridview.HeaderRow.Cells[i].Controls[0];
                    headerRowCell.Enabled = false;
                }

                foreach (GridViewRow row in sqlGridview.Rows)
                {
                    row.BackColor = Color.White;
                    foreach (TableCell cell in row.Cells)
                    {
                        if (row.RowIndex % 2 == 0)
                        {
                            cell.BackColor = sqlGridview.AlternatingRowStyle.BackColor;
                        }
                        else
                        {
                            cell.BackColor = sqlGridview.RowStyle.BackColor;
                        }
                        cell.CssClass = "textmode";
                    }
                }

                sqlGridview.RenderControl(hw);
            }
            else if (platformValue.ToLower() == "linux")
            {

                shellGridview.AllowPaging = false;
                shellGridview.DataSource = ((DataTable)Session["SessionData2"]).DefaultView;
                shellGridview.DataBind();
                shellGridview.UseAccessibleHeader = true;
                shellGridview.HeaderRow.TableSection = TableRowSection.TableHeader;

                //Change the Header Row back to white color
                shellGridview.HeaderRow.Style.Add("background-color", "#FFFFFF");

                //Apply style to Individual Cells            
                for (int i = 0; i < shellGridview.HeaderRow.Cells.Count; i++)
                {
                    shellGridview.HeaderRow.Cells[i].Style.Add("background-color", "#286090");
                    shellGridview.FooterRow.Cells[i].Style.Add("background-color", "#286090");
                    LinkButton headerRowCell = (LinkButton)shellGridview.HeaderRow.Cells[i].Controls[0];
                    headerRowCell.Enabled = false;
                }

                foreach (GridViewRow row in shellGridview.Rows)
                {
                    row.BackColor = Color.White;
                    foreach (TableCell cell in row.Cells)
                    {
                        if (row.RowIndex % 2 == 0)
                        {
                            cell.BackColor = shellGridview.AlternatingRowStyle.BackColor;
                        }
                        else
                        {
                            cell.BackColor = shellGridview.RowStyle.BackColor;
                        }
                        cell.CssClass = "textmode";
                    }
                }

                shellGridview.RenderControl(hw);
            }
            else if (platformValue.ToLower() == "github")
            {

                githubGridview.AllowPaging = false;
                githubGridview.DataSource = ((DataTable)Session["SessionData3"]).DefaultView;
                githubGridview.DataBind();
                githubGridview.UseAccessibleHeader = true;
                githubGridview.HeaderRow.TableSection = TableRowSection.TableHeader;

                //Change the Header Row back to white color
                githubGridview.HeaderRow.Style.Add("background-color", "#FFFFFF");

                //Apply style to Individual Cells            
                for (int i = 0; i < githubGridview.HeaderRow.Cells.Count; i++)
                {
                    githubGridview.HeaderRow.Cells[i].Style.Add("background-color", "#286090");
                    githubGridview.FooterRow.Cells[i].Style.Add("background-color", "#286090");
                    LinkButton headerRowCell = (LinkButton)githubGridview.HeaderRow.Cells[i].Controls[0];
                    headerRowCell.Enabled = false;
                }

                foreach (GridViewRow row in githubGridview.Rows)
                {
                    row.BackColor = Color.White;
                    foreach (TableCell cell in row.Cells)
                    {
                        if (row.RowIndex % 2 == 0)
                        {
                            cell.BackColor = githubGridview.AlternatingRowStyle.BackColor;
                        }
                        else
                        {
                            cell.BackColor = githubGridview.RowStyle.BackColor;
                        }
                        cell.CssClass = "textmode";
                    }
                }

                githubGridview.RenderControl(hw);
            }
            return sw;

        }

        /// <summary>
        /// Button to export the Gridview data to excel sheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                StringWriter sw = GridviewToExcel();
                //style to format numbers to string
                string style = @"<style> .textmode { mso-number-format:\@; } </style>";
                //Writetherenderedcontenttoafile.            
                Response.Write(style);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
                Logger.LogMessage("Excel sheet downloaded by user--" + ViewState["UserID"]);
            }
            catch
            {
                Response.Redirect("~/TechnicalError.aspx");
            }
        }


        /// <summary>
        /// Email Button to send the excel sheet attached with the Email
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEmail_Click(object sender, EventArgs e)
        {
            Type cstype = this.GetType();
            DateTime now = DateTime.Now;
            // Get a ClientScriptManager reference from the Page class.
            ClientScriptManager cs = Page.ClientScript;
            bool emailSent = false;
            try
            {
                if (!string.IsNullOrEmpty(hiddenNameField.Value))
                {
                    var fromAddress = new MailAddress("mittal_rahul@network.lilly.com");
                    var toAddress = new MailAddress(hiddenNameField.Value.ToString());
                    const string subject = "Impacted Components Details: Attached Excel file";
                    const string body = @"Please find the attached file.";
                    StringWriter sw = GridviewToExcel();
                    string renderedGridView = sw.ToString();
                    string attachmentName = "IA_" + platformValue + "_" + envtValue + "_" + Session["state"].ToString() + "_" + ViewState["UserID"] + "_" + strSearch + "_" + now.ToString("dd-MMM-yyyy") + ".xls";
                    System.IO.File.WriteAllText(@"C:\\users\\public\\" + attachmentName, renderedGridView);
                    var smtp = new SmtpClient
                    {
                        Host = "smtp-z2-nomx.lilly.com"
                    };
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body,
                    })
                    {
                        if (toAddress.ToString().Contains("network.lilly.com"))
                        {
                            message.Attachments.Add(new Attachment("C:\\users\\public\\" + attachmentName));
                            emailSent = true;
                            smtp.Send(message);
                        }
                        if (!cs.IsStartupScriptRegistered(cstype, csname3))
                        {
                            if (emailSent == true)
                            {
                                String cstext1 = "alert('Email has been sent to specified address!!');";
                                cs.RegisterStartupScript(cstype, csname3, cstext1, true);
                            }
                            else
                            {
                                String cstext1 = "alert('Not able to sent the Email. Please enter correct Email-ID!!');";
                                cs.RegisterStartupScript(cstype, csname3, cstext1, true);
                            }
                        }

                    }
                    Logger.LogMessage("Email sent by user--" + ViewState["UserID"] + "--to--" + toAddress);
                    // Check to see if the startup script is already registered.
                    

                }
            }
            catch
            {
                Response.Redirect("~/TechnicalError.aspx");
            }

        }

        protected void BindFolderList(DataTable folderDataTable)
        {
            int i = 1;
            foreach (DataRow dr in folderDataTable.Rows)
            {
                lstFolders.Items.Add(new ListItem(dr[0].ToString(), Convert.ToString(i)));
                i++;
            }
        }
        /// <summary>
        /// Reset Button to go back to previous page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnReset_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/index.aspx");
        }

        protected void lstFolders_SelectedIndexChanged(object sender, EventArgs e)
        {

            schemaDrpdwnLst.Items.Clear();
            BindSchemaList(dbDropDwnLst.SelectedItem.Text.ToString());
            RemoveDrodwnList(dbDropDwnLst.SelectedItem.Text.ToString());
        }
        protected void lstServer_SelectedIndexChanged(object sender, EventArgs e)
        {

            appDropDwnLst.Items.Clear();
            appDropDwnLst.Items.Insert(0, "Please Select Project");
            BindApplicationList(serverDropDwnLst.SelectedItem.Text.ToString());
            //RemoveDrodwnList(dbDropDwnLst.SelectedItem.Text.ToString());
        }
        protected void listApp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (platformValue.ToLower() == "linux")
            {
                dirDrpdwnList.Items.Clear();
                BindDirPathList(serverDropDwnLst.SelectedItem.Text.ToString(), appDropDwnLst.SelectedItem.Text.ToString());
            }
            else if (platformValue.ToLower() == "github")
            {
                repoDropDownList.Items.Clear();
                repoDropDownList.Items.Insert(0, "Please Select Repository");
                BindRepoPathList(appDropDwnLst.SelectedItem.Text.ToString());
            }
            //RemoveDrodwnList(dbDropDwnLst.SelectedItem.Text.ToString());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private HttpClient getClient(string username, string password)
        {
            try
            {

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;                 
                //ADD BASIC AUTH
                var authByteArray = Encoding.ASCII.GetBytes(username + ":" + password);
                var authString = Convert.ToBase64String(authByteArray);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", " " + authString);
                return httpClient;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="affiliate">Country</param>
        /// <param name="id">Document ID</param>
        /// <param name="environment">Document environment</param>
        /// <returns></returns>
        public Page<JsonResponse.Item> GetGithubResults(string username, string password, string searchKeyword, string repoPath)
        {
            try
            {
                Page<JsonResponse.Item> results = null;                
                using (var client = getClient(username, password))
                {                        
                    var stringTask = client.GetAsync(new Uri("https://api.github.com/search/code?page=1&per_page=100&order=desc&q=" + searchKeyword + "+repo:" + repoPath)).Result;
                    var responseString = stringTask.Content.ReadAsStringAsync().Result;
                    results = JsonConvert.DeserializeObject<Page<JsonResponse.Item>>(responseString);                    
                    //Response.Write(responseString.ToString());
                    return results;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        protected void LogoutButton_Click(object sender, EventArgs e)
        {
            Session["authenticated"] = null;
            Session.RemoveAll(); //Removes all session variables
            Response.Redirect("~/login.aspx");
        }



        /// <summary>
        /// Method to encrypt a string
        /// </summary>
        /// <param name="plainStr">String value</param>
        /// <returns>Base64 encoded string</returns>
        //public static string Encrypt(string plainStr)
        //{
        //    byte[] key = null;
        //    byte[] iv = null;
        //    byte[] plainText = null;
        //    byte[] cipherText = null;
        //    int count = 0;

        //    try
        //    {
        //        using (RijndaelManaged aesEncryption = new RijndaelManaged())
        //        {
        //            aesEncryption.KeySize = 256;
        //            aesEncryption.BlockSize = 128;
        //            aesEncryption.Mode = CipherMode.CBC;
        //            aesEncryption.Padding = PaddingMode.PKCS7;
        //            key = Enumerable.Repeat((byte)0, 16).ToArray();
        //            foreach (byte b in Encoding.UTF8.GetBytes("encrytedkey"))
        //            {
        //                key[count] = b;
        //                count++;
        //            }
        //            aesEncryption.Key = key;
        //            iv = Enumerable.Repeat((byte)0, 16).ToArray();
        //            aesEncryption.IV = iv;
        //            plainText = System.Text.Encoding.UTF8.GetBytes(plainStr);
        //            ICryptoTransform crypto = aesEncryption.CreateEncryptor();
        //            cipherText = crypto.TransformFinalBlock(plainText, 0, plainText.Length);
        //            return Convert.ToBase64String(cipherText);
        //        }

        //    }
        //    catch (Exception exception)
        //    {
        //        throw exception;
        //    }
        //    finally
        //    {
        //        key = null;
        //        iv = null;
        //        plainText = null;
        //        cipherText = null;
        //    }
        //}

    }


}
