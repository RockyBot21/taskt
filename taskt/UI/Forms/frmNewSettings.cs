﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace taskt.UI.Forms
{
    public partial class frmNewSettings : ThemedForm
    {
        Core.ApplicationSettings newAppSettings;
        frmScriptBuilder scriptBuilderForm;

        private string prevPage = "";

        // Metric
        private Label lblMetrics = null;
        private TreeView tvExecutionTimes = null;
        private Button btnClearMetrics = null;

        // Local Listener
        private Button btnStartListening = null;
        private Button btnStopListening = null;
        private Label lblListeningState = null;

        // Server
        private Label lblSocketState = null;
        private Label lblSocketException = null;

        private string[] keysList;

        private enum FontSize
        {
            Small,
            Normal,
            NormalBold,
            Large
        }

        #region form events
        public frmNewSettings(frmScriptBuilder fm)
        {
            InitializeComponent();
            this.scriptBuilderForm = fm;

            var keys = Enum.GetValues(typeof(Keys));
            keysList = new string[keys.Length];
            int i = 0;
            foreach (var key in keys)
            {
                keysList[i++] = key.ToString();
            }
        }

        private void frmNewSettings_Load(object sender, EventArgs e)
        {
            newAppSettings = new Core.ApplicationSettings();
            newAppSettings = newAppSettings.GetOrCreateApplicationSettings();

            // Network -> Server
            Core.Server.LocalTCPListener.ListeningStarted += AutomationTCPListener_ListeningStarted;
            Core.Server.LocalTCPListener.ListeningStopped += AutomationTCPListener_ListeningStopped;

            tvSettingsMenu.ExpandAll();
        }
        #endregion

        #region footer buttons
        private void uiBtnOpen_Click(object sender, EventArgs e)
        {
            //Keys key = (Keys)Enum.Parse(typeof(Keys), cboCancellationKey.Text);
            //newAppSettings.EngineSettings.CancellationKey = key;
            newAppSettings.Save(newAppSettings);
            Core.Server.SocketClient.LoadSettings();
            this.Close();
        }

        private void uiCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region tvSettingMenu
        private void tvSettingsMenu_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode rootNode = tvSettingsMenu.SelectedNode;
            while (rootNode.Parent != null)
            {
                rootNode = rootNode.Parent;
            }

            flowLayoutSettings.SuspendLayout();

            string newPage = rootNode.Text + " - " + tvSettingsMenu.SelectedNode.Text;

            switch (newPage)
            {
                case "Application - Debug":
                    showApplicationDebugSettings();
                    break;
                case "Application - Folder":
                    showApplicationFolderSettings();
                    break;
                case "Application - Other":
                    showApplicationOtherSettings();
                    break;
                case "Application - Script File":
                    showApplicationScriptFileSettings();
                    break;
                case "Application - Script Metric":
                    showApplicationMetricSettings();
                    break;
                case "Application - Start Up":
                    showApplicationStartUpSetting();
                    break;

                case "Automation Engine - Engine":
                    showAutomationEngineEngineSettings();
                    break;
                case "Automation Engine - Keyword":
                    showAutomationEngineKeywordSettings();
                    break;
                case "Automation Engine - Log":
                    showAutomationEngineLogSettings();
                    break;
                case "Automation Engine - Parser":
                    showAutomationEngineParserSettings();
                    break;
                case "Automation Engine - Variable":
                    showAutomationEngineVariableSettings();
                    break;

                case "Documents - Command Reference":
                    showDocumentsCommandReferenceSettings();
                    break;

                case "Editor - Command List":
                    showEditorCommandListSettings();
                    break;
                case "Editor - Command Search":
                    showEditorCommandSearchSettings();
                    break;
                case "Editor - Indent":
                    showEditorIndentSettings();
                    break;
                case "Editor - Instance":
                    showEditorInstanceSettings();
                    break;
                case "Editor - Insert Command":
                    showEditorInsertCommandSettings();
                    break;
                case "Editor - Mini Map":
                    showEditorMiniMapSettings();
                    break;
                case "Editor - Menu Bar":
                    showEditorMenuBarSettings();
                    break;
                case "Editor - Validation":
                    showEditorValidationSettings();
                    break;
                case "Editor - Variable":
                    showEditorVariableSettings();
                    break;

                case "Network - Local Listener":
                    showNetworkLocalListerSettings();
                    break;
                case "Network - Server":
                    showNetworkServerSettings();
                    break;

                case "Update - Check Update":
                    showUpdateCheckUpdateSettings();
                    break;

                case "VM - Display Manager":
                    showVMDisplayManagerSettings();
                    break;

                default:
                    newPage = prevPage;
                    break;
            }
            updatePrevPage(newPage);

            flowLayoutSettings.ResumeLayout();
        }

        private void updatePrevPage(string currentPage)
        {
            if (currentPage == prevPage)
            {
                return;
            }
            else
            {
                switch (prevPage)
                {
                    case "Application - Script Metric":
                        lblMetrics = null;
                        tvExecutionTimes = null;
                        btnClearMetrics = null;
                        break;

                    case "Network - Local Listener":
                        btnStartListening = null;
                        btnStopListening = null;
                        break;

                    case "Network - Server":
                        lblSocketState = null;
                        lblSocketException = null;
                        break;
                }
            }
            prevPage = currentPage;
        }

        #endregion

        #region Application
        private void showApplicationStartUpSetting()
        {
            removeSettingControls();

            createLabel("lblTitie", "Start Up", FontSize.Large, true);
            createCheckBox("chkAntiIdle", "Anti-Idle (while app is open)", newAppSettings.ClientSettings, "AntiIdleWhileOpen", true);
            CheckBox chkPre = createCheckBox("chkPreLoadCommands", "Load Commands at Startup (Reduces Flicker)", newAppSettings.ClientSettings, "PreloadBuilderCommands", true);
            chkPre.Visible = false;
            createLabel("lblStartMode", "Start Mode:", FontSize.Normal, false);
            ComboBox cmbStart = createComboBox("cmbStartMode", new string[] { "Builder Mode","Attended Task Mode"}, 200, newAppSettings.ClientSettings, "StartupMode", true);
            cmbStart.SelectionChangeCommitted += (sender, e) => cmbStartUpMode_SelectionChangeCommitted(sender, e);

            Button btnAttended = createButton("btnLunchAttended", "Launch Attended Mode", 240, true);
            btnAttended.Click += (sender, e) => btnLaunchAttendedMode_Click(sender, e);
        }

        private void showApplicationFolderSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Folder", FontSize.Large, true);
            
            createLabel("lblRootFolder", "taskt Root Folder", FontSize.Small, true);
            TextBox txtAppFolder = createTextBox("txtAppFolderPath", 440, newAppSettings.ClientSettings, "RootFolder", false);
            Button rootButton = createButton("btnSelectRootFolder", "...", 42, true);
            rootButton.Click += (sender, e) => btnSelectRootFolder_Click(sender, e, txtAppFolder);

            createLabel("lblTaskFolder", "Attended Tasks Folder (Default Folder for saving Script Files)", FontSize.Small, true);
            TextBox txtTasksFolder = createTextBox("txtAttendedTasksFolder", 440, newAppSettings.ClientSettings, "AttendedTasksFolder", false);
            Button tasksFolder = createButton("btnSelectTasksFolder", "...", 42, true);
            tasksFolder.Click += (sender, e) => btnSelectAttendedTaskFolder_Click(sender, e, txtTasksFolder);
        }
        private void showApplicationDebugSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Debug", FontSize.Large, true);

            createCheckBox("chkShowDebug", "Show Debug Window when Script Execute", newAppSettings.EngineSettings, "ShowDebugWindow", true);
            createCheckBox("chkAutoCloseDebugWindow", "Automatically Close Debug Window", newAppSettings.EngineSettings, "AutoCloseDebugWindow", true);
            createCheckBox("chkShowAdvancedDebug", "Show Advanced Debug Logs During Execution", newAppSettings.EngineSettings, "ShowAdvancedDebugOutput", true);
        }
        private void showApplicationMetricSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Script Metric", FontSize.Large, true);

            createCheckBox("chkTrackMetrics", "Track Execution Metrics", newAppSettings.EngineSettings, "TrackExecutionMetrics", true);

            createLabel("lblTitleMetrics", "Script Execution Metrics (Last 10 per Script)", FontSize.Small, true);
            lblMetrics = createLabel("lblMetrics", "Getting Metrics...", FontSize.Normal, true);

            TreeView tv = new TreeView();
            tv.Name = "tvExecutionTimes";
            tv.Size = new Size(500, 120);
            tv.Font = new Font("Segoe UI", 12);
            flowLayoutSettings.Controls.Add(tv);

            tvExecutionTimes = tv;

            btnClearMetrics = createButton("btnClearMetrics", "Clear Metrics", 200, true);
            btnClearMetrics.Click += (sender, e) => btnClearMetrics_Click(sender, e);

            //get metrics
            bgwMetrics.RunWorkerAsync();
        }
        private void showApplicationOtherSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Other", FontSize.Large, true);

            createCheckBox("chkMinimizeToTary", "Minimize to System Tray", newAppSettings.ClientSettings, "MinimizeToTray", true);

            createLabel("lblResourceTitle", "Resources Folder", FontSize.NormalBold, true);
            createLabel("lblResource", "If you want to update the WebDriver (chromedriver.exe etc),\nopen the Resources folder and overwrite the file.", FontSize.Small, true);
            Button btnRes = createButton("btnOpenResources", "Open Resources Folder", 280, true);
            btnRes.Click += (sender, e) => btnShowRecoures_Click(sender, e);
        }
        private void showApplicationScriptFileSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Script File", FontSize.Large, true);

            createCheckBox("lblScriptIntermediate", "Export Intermediate Script File", newAppSettings.EngineSettings, "ExportIntermediateXML", true);
        }
        #endregion

        #region Automation Engine
        private void showAutomationEngineParserSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Parser", FontSize.Large, true);

            createLabel("lblStartMarker", "Start Marker:", FontSize.Normal, false);
            TextBox txtStart = createTextBox("txtStartMarker", 40, newAppSettings.EngineSettings, "VariableStartMarker", false);
            createLabel("lblEndMarker", "End Marker:", FontSize.Normal, false);
            TextBox txtEnd = createTextBox("txtEndMarker", 40, newAppSettings.EngineSettings, "VariableEndMarker", true);
            Label lblNotice = createLabel("lblMarkerNotice", "If Start Maker and End Marker are the same,\nthe variable may not expand properly.", FontSize.Small, false);
            lblNotice.Padding = new Padding(0, 4, 0, 0);
            Label lblExample = createLabel("lblVariableExample", newAppSettings.EngineSettings.VariableStartMarker + "VariableName" + newAppSettings.EngineSettings.VariableEndMarker, FontSize.NormalBold, true);

            txtStart.TextChanged += (sender, e) => VariableMarker_TextChanged(sender, e, txtStart, txtEnd, lblExample);
            txtEnd.TextChanged += (sender, e) => VariableMarker_TextChanged(sender, e, txtStart, txtEnd, lblExample);

            createCheckBox("chkCalculateAutomatically", "Calculate Automatically", newAppSettings.EngineSettings, "AutoCalcVariables", true);

            createCheckBox("chkUserNewParser", "Use New Parser (beta)", newAppSettings.EngineSettings, "UseNewParser", true);
            createCheckBox("chkIgnoreFirstMarker", "Ignore First Variable Marker In Output Parameter (Check is strongly recommended)", newAppSettings.EngineSettings, "IgnoreFirstVariableMarkerInOutputParameter", true);
        }
        private void showAutomationEngineKeywordSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Keyword", FontSize.Large, true);

            createLabel("lblWindowKeyword", "Window Keyword", FontSize.NormalBold, true);

            createLabel("lblCurrentWindow", "Current Window Keyword", FontSize.Small, true);
            createTextBox("txtCurrentWindow", 400, newAppSettings.EngineSettings, "CurrentWindowKeyword", true);

            createLabel("lblExcelKeyword", "Excel Keyword", FontSize.NormalBold, true);
            createLabel("lblCurrentSheet", "Current Worksheet Keyword", FontSize.Small, true);
            createTextBox("txtCurrentSheet", 400, newAppSettings.EngineSettings, "CurrentWorksheetKeyword", true);
            createLabel("lblNextSheet", "Next Worksheet Keyword", FontSize.Small, true);
            createTextBox("txtNextSheet", 400, newAppSettings.EngineSettings, "NextWorksheetKeyword", true);
            createLabel("lblPreviousSheet", "Previous Worksheet Keyword", FontSize.Small, true);
            createTextBox("txtPreviousSheet", 400, newAppSettings.EngineSettings, "PreviousWorksheetKeyword", true);
        }

        private void showAutomationEngineEngineSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Engine", FontSize.Large, true);

            createCheckBox("chkOverrideAppInstance", "Override App Instances", newAppSettings.EngineSettings, "OverrideExistingAppInstances", true);

            createLabel("lblCommandDelay", "Default delay between executing commands (ms):", FontSize.Normal, false);
            createTextBox("txtCommandDelay", 80, newAppSettings.EngineSettings, "DelayBetweenCommands", true);

            createLabel("lblCancelKey", "End Script Hotkey:", FontSize.Normal, false);
            ComboBox cmb =createComboBox("cmbCancellationKey", keysList, 240, newAppSettings.EngineSettings, "CancellationKey", true);
            cmb.SelectionChangeCommitted += (sender, e) => cmdCancellationButton_SelectionChangeCommitted(sender, e);
        }
        private void showAutomationEngineVariableSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Variable", FontSize.Large, true);

            createCheckBox("chkCreateMissingVariable", "Create Missing Variables During Execution", newAppSettings.EngineSettings, "CreateMissingVariablesDuringExecution", true);
        }
        private void showAutomationEngineLogSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Log", FontSize.Large, true);

            createCheckBox("chkEnableLogging", "Enable Diagnostic Logging", newAppSettings.EngineSettings, "EnableDiagnosticLogging", true);
        }
        #endregion

        #region Documents
        private void showDocumentsCommandReferenceSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Command Reference", FontSize.Large, true);

            Button btn = createButton("btnCreateCommandRef", "Create Command Reference", 300, true);
            btn.Click += (sender, e) => btnCreateCommandReference_Click(sender, e);
        }
        #endregion

        #region Editor
        private void showEditorMenuBarSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Menu Bar", FontSize.Large, true);

            createCheckBox("chkUseSlimBar", "Use Slim Menu Bar (Restart required)", newAppSettings.ClientSettings, "UseSlimActionBar", true);
        }
        private void showEditorCommandListSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Command List", FontSize.Large, true);

            createLabel("lblGrouping", "Command List", FontSize.NormalBold, true);
            createCheckBox("chkGroupBySubgroup", "Gruoping by subgroup (Restart required)", newAppSettings.ClientSettings, "GroupingBySubgroup", true);

            createLabel("lblSearch", "Command Search", FontSize.NormalBold, true);

            createCheckBox("chkMakeGroupNameSearchTarget", "Make Group Name a Search Target", newAppSettings.ClientSettings, "SearchTargetGroupName", true);
            createCheckBox("chkGreedlyGroupName", "Show All Commands if Group Name Matches", newAppSettings.ClientSettings, "SearchGreedlyGroupName", true);
            createCheckBox("chkMakeSubGroupNameSearchTarget", "Make SubGroup Name a Search Target", newAppSettings.ClientSettings, "SearchTargetSubGroupName", true);
            createCheckBox("chkGreedlySubGroupName", "Show All Commands if SubGroup Name Matches", newAppSettings.ClientSettings, "SearchGreedlySubGroupName", true);
        }
        private void showEditorInstanceSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Instance", FontSize.Large, true);

            createLabel("lblSortHeader", "Instance Sort", FontSize.NormalBold, true);
            createLabel("lblSortOrder", "Instance Name Sort Order:", FontSize.Normal, false);
            ComboBox cmbSort = createComboBox("cmbSortOrder", new string[] { "Creation Frequently", "By Name", "Frequency Of Use", "No Sorting" }, 240, newAppSettings.ClientSettings, "InstanceNameOrder", true);
            cmbSort.Text = newAppSettings.ClientSettings.InstanceNameOrder;
            cmbSort.SelectionChangeCommitted += (sender, e) => cmbInstanceSortOrder_SelectionChangeCommitted(sender, e);

            createLabel("lblDefaultInstance", "Default Instance Name", FontSize.NormalBold, true);
            createLabel("lblDefaultDatabase", "Default Database Instance Name", FontSize.Small, true);
            createTextBox("txtDefaultDatabase", 400, newAppSettings.ClientSettings, "DefaultDBInstanceName", true);
            createLabel("lblDefaultExcel", "Default Excel Instance Name", FontSize.Small, true);
            createTextBox("txtDefaultExcel", 400, newAppSettings.ClientSettings, "DefaultExcelInstanceName", true);
            createLabel("lblDefaultNLG", "Default NLG Instance Name", FontSize.Small, true);
            createTextBox("txtDefaultNLG", 400, newAppSettings.ClientSettings, "DefaultNLGInstanceName", true);
            createLabel("lblDefaultStopWatch", "Default StopWatch Instance Name", FontSize.Small, true);
            createTextBox("txtDefaultStopWatch", 400, newAppSettings.ClientSettings, "DefaultStopWatchInstanceName", true);
            createLabel("lblDefaultWebBrowser", "Default WebBrowser Instance Name", FontSize.Small, true);
            createTextBox("txtDefaultWebBrowser", 400, newAppSettings.ClientSettings, "DefaultBrowserInstanceName", true);
            createLabel("lblDefaultWord", "Default Word Instance Name", FontSize.Small, true);
            createTextBox("txtDefaultWord", 400, newAppSettings.ClientSettings, "DefaultWordInstanceName", true);
        }
        private void showEditorInsertCommandSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Insert Command", FontSize.Large, true);

            createCheckBox("chkInsertCommandsInline", "New Commands Insert Below Selected Command", newAppSettings.ClientSettings, "InsertCommandsInline", true);
            createCheckBox("chkSequenceDragDrop", "Allow Drag and Drop into Sequence Commands", newAppSettings.ClientSettings, "EnableSequenceDragDrop", true);
            createCheckBox("chkInsertElse", "Insert Else when BeginIf command inserted", newAppSettings.ClientSettings, "InsertElseAutomatically", true);
            createCheckBox("chkInsertCommentIfLoop", "Insert Comment above If, Loop, Try", newAppSettings.ClientSettings, "InsertCommentIfLoopAbove", true);
        }
        private void showEditorMiniMapSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Mini Map", FontSize.Large, true);

            createCheckBox("chkShowMiniMap", "Show Script Mini Map (beta)", newAppSettings.ClientSettings, "ShowScriptMiniMap", true);
        }
        private void showEditorIndentSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Indent", FontSize.Large, true);

            createCheckBox("chkShowIndentLine", "Show Indent Line", newAppSettings.ClientSettings, "ShowIndentLine", true);
            createLabel("lblIndentWidth", "Indent Width (1 to 32):", FontSize.Normal, false);
            createTextBox("txtIndentWidth", 60, newAppSettings.ClientSettings, "IndentWidth", true);
        }
        private void showEditorVariableSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Variable", FontSize.Large, true);

            createCheckBox("chkInsertVariablePosition", "Insert variable at cursor position(Textbox / Combobox)", newAppSettings.ClientSettings, "InsertVariableAtCursor", true);
        }
        private void showEditorValidationSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Validation", FontSize.Large, true);

            createCheckBox("chkSilentValidation", "Don't show Script Command Validation Message", newAppSettings.ClientSettings, "DontShowValidationMessage", true);
        }
        private void showEditorCommandSearchSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Command Search", FontSize.Large, true);

            createCheckBox("chkShowCommandSearch", "Show Command Search Box when taskt is started (required restart)", newAppSettings.ClientSettings, "ShowCommandSearchBar", true);
        }
        #endregion

        #region Network
        private void showNetworkLocalListerSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Local Listener (Beta)", FontSize.Large, true);

            createLabel("lbmMessage", "Enable this functionality to allow this computer to accept script execution requests from \nother taskt or REST-capable clients.", FontSize.Small, true);

            createCheckBox("chkAutoStartListener", "Start Listening on Startup", newAppSettings.ListenerSettings, "StartListenerOnStartup", true);
            createCheckBox("chkEnableListening", "Local Listening Enabled", newAppSettings.ListenerSettings, "LocalListeningEnabled", true);

            createLabel("lblListeningPort", "Listening Port:", FontSize.Normal, false);
            TextBox txtListeningPort = createTextBox("txtListeningPort", 120, newAppSettings.ListenerSettings, "ListeningPort", true);

            createCheckBox("chkRequireListenerKey", "Require Authentication Key", newAppSettings.ListenerSettings, "RequireListenerAuthenticationKey", true);
            createLabel("lblAuthenicationKey", "Authentication Key", FontSize.Small, true);
            createTextBox("txtAuthenicationKey", 480, newAppSettings, "ListenerSettings.AuthKey", true);

            createCheckBox("chkEnableWhitelit", "Enable IP Verification (Seperate with comma)", newAppSettings.ListenerSettings, "EnableWhitelist", true);
            TextBox txtWhite = createTextBox("txtWhitelist", 480, newAppSettings.ListenerSettings, "IPWhiteList", true);
            txtWhite.Multiline = true;
            txtWhite.ScrollBars = ScrollBars.Vertical;
            txtWhite.Height = 80;

            btnStartListening = createButton("btnStartListening", "Start Listening", 140, false);
            btnStopListening = createButton("btnEndListening", "Stop Listening", 140, true);

            btnStartListening.Click += (sender, e) => btnStartListening_Click(sender, e, txtListeningPort);
            btnStopListening.Click += (sender, e) => btnStopListening_Click(sender, e);

            lblListeningState = createLabel("lblListeningState", "Listening on {}", FontSize.Large, true);

            SetupListeningUI();
        }
        private void showNetworkServerSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Server", FontSize.Large, true);

            createLabel("lblMessage", "Enable this functionality to connect to a local instance of taskt server for workforce \nmanagement. After testing the connection, the client will be assigned a new GUID \nwhich must be approved by an administrator in the server.", FontSize.Small, true);

            createCheckBox("chkServerEnabled", "Server Connection Enabled", newAppSettings.ServerSettings, "ServerConnectionEnabled", true);
            createCheckBox("chkAutomaticallyConnect", "Check In On Startup", newAppSettings.ServerSettings, "ConnectToServerOnStartup", true);

            createLabel("lblServerURL", "HTTPS Server URL", FontSize.NormalBold, true);
            createLabel("lblServerURLex", "Enter the location of the taskt server (ex. https://localhost:60281", FontSize.Normal, true);
            TextBox txtAddress = createTextBox("txtHttpsAddress", 480, newAppSettings.ServerSettings, "HTTPServerURL", true);
            Button btnTestConnection = createButton("btnTestConnection", "Test Connection", 240, true);

            createLabel("lblClientGUID", "Client GUID", FontSize.NormalBold, true);
            createLabel("lblClientGUIDex", "Indicates the GUID the client will use when connecting to taskt server", FontSize.Normal, true);
            createTextBox("txtGUID", 480, newAppSettings.ServerSettings, "HTTPGuid", true);
            Button btnPublishTask = createButton("btnPublishTask", "Publish Task", 240, true);

            lblSocketState = createLabel("lblSocketState", "Socket Status", FontSize.Large, true);
            lblSocketException = createLabel("lblSocketException", "Socket Exception", FontSize.Normal, true);

            btnTestConnection.Click += (sender, e) => btnTestConnection_Click(sender, e, txtAddress);
            btnPublishTask.Click += (sender, e) => btnPublishTask_Click(sender, e);
        }
        #endregion

        #region Update
        private void showUpdateCheckUpdateSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Check Update", FontSize.Large, true);

            Button btn = createButton("btnCheckUpdate", "Check For Updates", 200, true);
            btn.Click += (sender, e) => btnCheckUpdate_Click(sender, e);
        }
        #endregion

        #region VM
        private void showVMDisplayManagerSettings()
        {
            removeSettingControls();

            createLabel("lblTitle", "Display Manager", FontSize.Large, true);

            Button btn = createButton("btnDisplayManager", "Launch Display Manager", 200, true);
            btn.Click += (sender, e) => btnLaunchDisplayManager_Click(sender, e);
        }
        #endregion

        #region Create Controls
        private void removeSettingControls()
        {
            flowLayoutSettings.Controls.Clear();
        }
        private Label createLabel(string name, string text, FontSize fontSize = FontSize.Normal, bool isBreak = false)
        {
            Label lbl = new Label();
            lbl.Name = name;
            lbl.Text = text;

            lbl.AutoSize = true;

            switch (fontSize)
            {
                case FontSize.Small:
                    lbl.Font = new Font("Segoe UI Semilight", (Single)9.75);
                    lbl.ForeColor = Color.SlateGray;
                    lbl.Height = 16;
                    break;
                case FontSize.Normal:
                    lbl.Font = new Font("Segoe UI Light", 12);
                    lbl.ForeColor = Color.SteelBlue;
                    lbl.Height = 24;
                    break;
                case FontSize.NormalBold:
                    lbl.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                    lbl.ForeColor = Color.SteelBlue;
                    lbl.Height = 24;
                    break;
                case FontSize.Large:
                    lbl.Font = new Font("Segoe UI Light", (Single)15.75);
                    lbl.ForeColor = Color.SteelBlue;
                    break;
            }

            flowLayoutSettings.Controls.Add(lbl);
            flowLayoutSettings.SetFlowBreak(lbl, isBreak);

            return lbl;
        }

        private TextBox createTextBox(string name, int width, object source, string memberName, bool isBreak = false)
        {
            TextBox txt = new TextBox();
            txt.Name = name;
            txt.Width = width;
            txt.Height = 29;
            txt.Font = new Font("Segoe UI", 12);

            txt.DataBindings.Add("Text", source, memberName, false, DataSourceUpdateMode.OnPropertyChanged);

            flowLayoutSettings.Controls.Add(txt);
            flowLayoutSettings.SetFlowBreak(txt, isBreak);

            return txt;
        }

        private CheckBox createCheckBox(string name, string text, object source, string memberName, bool isBreak = false)
        {
            CheckBox chk = new CheckBox();
            chk.Name = name;
            chk.AutoSize = true;
            chk.Text = text;
            chk.Font = new Font("Segoe UI Semilight", (Single)11.25);
            chk.ForeColor = Color.SteelBlue;

            chk.DataBindings.Add("Checked", source, memberName, false, DataSourceUpdateMode.OnPropertyChanged);

            flowLayoutSettings.Controls.Add(chk);
            flowLayoutSettings.SetFlowBreak(chk, isBreak);

            return chk;
        }

        private ComboBox createComboBox(string name, string[] items, int width, object source, string memberName, bool isBreak = false)
        {
            ComboBox cmb = new ComboBox();
            cmb.Name = name;
            cmb.Font = new Font("Segoe UI", 12);
            cmb.DropDownStyle = ComboBoxStyle.DropDownList;

            cmb.BeginUpdate();
            cmb.Items.AddRange(items);
            cmb.EndUpdate();
            cmb.Width = width;

            cmb.DataBindings.Add("Text", source, memberName, false, DataSourceUpdateMode.OnPropertyChanged);

            flowLayoutSettings.Controls.Add(cmb);
            flowLayoutSettings.SetFlowBreak(cmb, isBreak);

            return cmb;
        }
        private Button createButton(string name, string text, int width, bool isBreak = false)
        {
            Button btn = new Button();
            btn.Name = name;
            btn.Text = text;
            btn.Width = width;
            btn.Height = 29;
            btn.Font = new Font("Segoe UI", (Single)9.75);

            flowLayoutSettings.Controls.Add(btn);
            flowLayoutSettings.SetFlowBreak(btn, isBreak);

            return btn;
        }
        #endregion

        #region StartUp Events
        private void btnLaunchAttendedMode_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Close Settings form to launch Attended Mode.\nIf you have changed the settings, click the 'OK' button to save the changes.\nLaunch Attended Mode now ?", "Settings", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                scriptBuilderForm.showAttendedModeFormProcess();
                this.Close();
            }
        }
        private void cmbStartUpMode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            newAppSettings.ClientSettings.StartupMode = ((ComboBox)sender).Text;
        }
        #endregion

        #region Folder Events
        private void btnSelectRootFolder_Click(object sender, EventArgs e, TextBox txt)
        {
            string currentFolerPath = newAppSettings.ClientSettings.RootFolder;

            //prompt user to confirm they want to select a new folder
            var updateFolderRequest = 
                MessageBox.Show(
                    "Would you like to change the default root folder that taskt uses to store tasks and information? " + Environment.NewLine + Environment.NewLine +
                    "Current Root Folder: " + currentFolerPath, 
                    "Change Default Root Folder", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            //if user does not want to update folder then exit
            if (updateFolderRequest == DialogResult.No)
            {
                return;
            }

            //user folder browser to let user select top level folder
            using (var fbd = new FolderBrowserDialog())
            {
                //check if user selected a folder
                if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    //create references to old and new root folders
                    var oldRootFolder = currentFolerPath;
                    var newRootFolder = System.IO.Path.Combine(fbd.SelectedPath, "taskt");

                    //ask user to confirm
                    var confirmNewFolderSelection = 
                        MessageBox.Show(
                            "Please confirm the changes below:" + Environment.NewLine + Environment.NewLine +
                            "Old Root Folder: " + oldRootFolder + Environment.NewLine + Environment.NewLine +
                            "New Root Folder: " + newRootFolder, 
                            "Change Default Root Folder", 
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                    //handle if user decides to cancel
                    if (confirmNewFolderSelection == DialogResult.Cancel)
                    {
                        return;
                    }

                    //ask if we should migrate the data
                    var migrateCopyData = 
                        MessageBox.Show(
                            "Would you like to attempt to move the data from the old folder to the new folder?  Please note, depending on how many files you have, this could take a few minutes.", 
                            "Migrate Data?", 
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    //check if user wants to migrate data
                    if (migrateCopyData == DialogResult.Yes)
                    {
                        try
                        {
                            //find and copy files
                            foreach (string dirPath in System.IO.Directory.GetDirectories(oldRootFolder, "*", System.IO.SearchOption.AllDirectories))
                            {
                                System.IO.Directory.CreateDirectory(dirPath.Replace(oldRootFolder, newRootFolder));
                            }
                            foreach (string newPath in System.IO.Directory.GetFiles(oldRootFolder, "*.*", System.IO.SearchOption.AllDirectories))
                            {
                                System.IO.File.Copy(newPath, newPath.Replace(oldRootFolder, newRootFolder), true);
                            }

                            MessageBox.Show("Data Migration Complete", "Data Migration Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                        catch (Exception ex)
                        {
                            //handle any unexpected errors
                            MessageBox.Show("An Error Occured during Data Migration Copy: " + ex.ToString());
                        }
                    }

                    //update textbox which will be updated once user selects "Ok"
                    newAppSettings.ClientSettings.RootFolder = newRootFolder;
                }
            }
        }

        private void btnSelectAttendedTaskFolder_Click(object sender, EventArgs e, TextBox txt)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    var newAttendedTaskFolder = System.IO.Path.Combine(fbd.SelectedPath);
                    txt.Text = newAttendedTaskFolder;
                }
            }
        }
        #endregion

        #region Parser Events
        private void VariableMarker_TextChanged(object sender,EventArgs e, TextBox startMarker, TextBox endMaker, Label exampleLabel)
        {
            exampleLabel.Text = startMarker.Text + "VariableName" + endMaker.Text;
        }
        #endregion

        #region Network Events
        private void btnTestConnection_Click(object sender, EventArgs e, TextBox txtAddress)
        {
            var successfulConnection = Core.Server.HttpServerClient.TestConnection(txtAddress.Text);

            if (successfulConnection)
            {
                var pulledNewGUID = Core.Server.HttpServerClient.GetGuid();

                if (pulledNewGUID)
                {
                    newAppSettings = new Core.ApplicationSettings();
                    newAppSettings = newAppSettings.GetOrCreateApplicationSettings();
                    txtAddress.Text = newAppSettings.ServerSettings.HTTPGuid.ToString();
                    MessageBox.Show("Connected Successfully!\nGUID will be reloaded automatically the next time settings is loaded!", "Taskt", MessageBoxButtons.OK);
                }
                MessageBox.Show("Connected Successfully!", "Taskt", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("Unable To Connect!", "Taskt", MessageBoxButtons.OK);
            }
        }
        private void btnPublishTask_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(scriptBuilderForm.ScriptFilePath))
            {
                Core.Server.HttpServerClient.PublishScript(scriptBuilderForm.ScriptFilePath, Core.Server.PublishedScript.PublishType.ServerReference);
            }
            else
            {
                MessageBox.Show("Please open the task in order to publish it.", "Taskt", MessageBoxButtons.OK);
            }
        }
        private void tmrGetSocketStatus_Tick(object sender, EventArgs e)
        {
            if ((lblSocketState == null) || (lblSocketException == null))
            {
                return;
            }
            lblSocketState.Text = "Socket Status: " + Core.Server.SocketClient.GetSocketState();
            if (Core.Server.SocketClient.connectionException != string.Empty)
            {
                lblSocketException.Show();
                lblSocketException.Text = Core.Server.SocketClient.connectionException;
            }
            else
            {
                lblSocketException.Hide();
            }
        }
        private void btnStartListening_Click(object sender, EventArgs e, TextBox txtPort)
        {
            if (int.TryParse(txtPort.Text, out var portNumber))
            {
                DisableListenerButtons();
                Core.Server.LocalTCPListener.StartListening(portNumber);
            }
        }
        private void btnStopListening_Click(object sender, EventArgs e)
        {
            DisableListenerButtons();
            Core.Server.LocalTCPListener.StopAutomationListener();
        }
        private void DisableListenerButtons()
        {
            if ((btnStartListening == null) || (btnStopListening == null))
            {
                return;
            }
            btnStartListening.Enabled = false;
            btnStopListening.Enabled = false;
        }
        private void SetupListeningUI()
        {
            if ((btnStartListening == null) || (btnStopListening == null) || (lblListeningState == null))
            {
                return;
            }

            if (Core.Server.LocalTCPListener.IsListening)
            {
                lblListeningState.Text = $"Client is Listening at Endpoint '{Core.Server.LocalTCPListener.GetListeningAddress()}'.";
                btnStartListening.Enabled = false;
                btnStopListening.Enabled = true;
            }
            else
            {
                lblListeningState.Text = $"Client is Not Listening!";
                btnStartListening.Enabled = true;
                btnStopListening.Enabled = false;
            }
            lblListeningState.Show();
        }
        #endregion

        #region LocalListener Events
        public delegate void AutomationTCPListener_StartedDelegate(object sender, EventArgs e);
        public delegate void AutomationTCPListener_StoppedDelegate(object sender, EventArgs e);
        private void AutomationTCPListener_ListeningStopped(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                var stoppedDelegate = new AutomationTCPListener_StoppedDelegate(AutomationTCPListener_ListeningStopped);
                Invoke(stoppedDelegate, new object[] { sender, e });
            }
            else
            {
                SetupListeningUI();
            }
        }
        private void AutomationTCPListener_ListeningStarted(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                var startedDelegate = new AutomationTCPListener_StoppedDelegate(AutomationTCPListener_ListeningStarted);
                Invoke(startedDelegate, new object[] { sender, e });
            }
            else
            {
                SetupListeningUI();
            }
        }

        #endregion

        #region Metrics Events
        private void btnClearMetrics_Click(object sender, EventArgs e)
        {
            new Core.Metrics().ClearExecutionMetrics();
            bgwMetrics.RunWorkerAsync();
        }
        private void bgwMetrics_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = new Core.Metrics().ExecutionMetricsSummary();
        }
        private void bgwMetrics_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((lblMetrics == null) || (tvExecutionTimes == null) || (btnClearMetrics == null))
            {
                return;
            }

            if (e.Error != null)
            {
                if (e.Error is System.IO.FileNotFoundException)
                {
                    lblMetrics.Text = "Metrics Unavailable - Metrics are only available after running tasks which will generate metrics logs";
                }
                else
                {
                    lblMetrics.Text = "Metrics Unavailable: " + e.Error.ToString();
                }
            }
            else
            {
                var metricsSummary = (List<Core.ExecutionMetric>)(e.Result);

                if (metricsSummary.Count == 0)
                {
                    lblMetrics.Text = "No Metrics Found";
                    lblMetrics.Show();
                    tvExecutionTimes.Hide();
                    btnClearMetrics.Hide();
                }
                else
                {
                    lblMetrics.Hide();
                    tvExecutionTimes.Show();
                    btnClearMetrics.Show();
                }

                foreach (var metric in metricsSummary)
                {
                    var rootNode = new TreeNode();
                    rootNode.Text = metric.FileName + " [" + metric.AverageExecutionTime + " avg.]";

                    foreach (var metricItem in metric.ExecutionData)
                    {
                        var subNode = new TreeNode();
                        subNode.Text = string.Join(" - ", metricItem.LoggedOn.ToString("MM/dd/yy hh:mm"), metricItem.ExecutionTime);
                        rootNode.Nodes.Add(subNode);
                    }

                    tvExecutionTimes.Nodes.Add(rootNode);
                }
            }
        }
        #endregion

        #region Update Events
        private void btnCheckUpdate_Click(object sender, EventArgs e)
        {
            taskt.Core.ApplicationUpdate updater = new Core.ApplicationUpdate();
            Core.UpdateManifest manifest = new Core.UpdateManifest();
            try
            {
                manifest = updater.GetManifest();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting manifest: " + ex.ToString());
                return;
            }

            if (manifest.RemoteVersionNewer)
            {
                Supplement_Forms.frmUpdate frmUpdate = new Supplement_Forms.frmUpdate(manifest);
                if (frmUpdate.ShowDialog() == DialogResult.OK)
                {

                    //move update exe to root folder for execution
                    var updaterExecutionResources = Application.StartupPath + "\\resources\\taskt-updater.exe";
                    var updaterExecutableDestination = Application.StartupPath + "\\taskt-updater.exe";

                    if (!System.IO.File.Exists(updaterExecutionResources))
                    {
                        MessageBox.Show("taskt-updater.exe not found in resources directory!");
                        return;
                    }
                    else
                    {
                        System.IO.File.Copy(updaterExecutionResources, updaterExecutableDestination);
                    }

                    var updateProcess = new System.Diagnostics.Process();
                    updateProcess.StartInfo.FileName = updaterExecutableDestination;
                    updateProcess.StartInfo.Arguments = manifest.PackageURL;

                    updateProcess.Start();
                    Application.Exit();
                }
            }
            else
            {
                MessageBox.Show("The application is currently up-to-date!", "No Updates Available", MessageBoxButtons.OK);
            }
        }
        #endregion

        #region VM Events
        private void btnLaunchDisplayManager_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Close Settings form to launch Display Manager.\nIf you have changed the settings, click the 'OK' button to save the changes.\nLaunch Display Manager now ?", "Settings", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Supplemental.frmDisplayManager displayManager = new Supplemental.frmDisplayManager();
                displayManager.Show();
                this.Close();
            }   
        }
        #endregion

        #region Documents Events
        private void btnCreateCommandReference_Click(object sender, EventArgs e)
        {
            Core.DocumentationGeneration docGeneration = new Core.DocumentationGeneration();
            var docsRoot = docGeneration.GenerateMarkdownFiles();
            System.Diagnostics.Process.Start(docsRoot);
        }
        #endregion

        #region Engine Events
        private void cmdCancellationButton_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Keys key = (Keys)Enum.Parse(typeof(Keys), ((ComboBox)sender).Text);
            newAppSettings.EngineSettings.CancellationKey = key;
        }
        #endregion

        #region Editor Events
        private void cmbInstanceSortOrder_SelectionChangeCommitted(object sender, EventArgs e)
        {
            newAppSettings.ClientSettings.InstanceNameOrder = ((ComboBox)sender).Text;
        }
        #endregion

        #region Application Events
        private void btnShowRecoures_Click(object sender, EventArgs e)
        {
            var myAssembly = System.Reflection.Assembly.GetEntryAssembly();
            string path = System.IO.Path.GetDirectoryName(myAssembly.Location) + "\\Resources";
            System.Diagnostics.Process.Start(path);
        }
        #endregion
    }
}