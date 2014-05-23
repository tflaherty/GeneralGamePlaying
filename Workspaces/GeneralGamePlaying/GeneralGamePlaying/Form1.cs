using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Threading;
using System.Windows.Forms;
using API.GGP.GeneralGameManagerNS;
using API.GGP.GeneralGameNS;
using API.UtilitiesAndExtensions;
using MRUManager_dotNet4_example;
using API.SWIProlog.SWIPrologServiceLibrary;


namespace API.GGP.GeneralGamePlayingNS
{
    public partial class Form1 : Form
    {
        private ServiceHost host1;
        private ServiceEndpoint endpoint1;
        private NetTcpBinding tcpBinding1;

        private ServiceHost host2;
        private ServiceEndpoint endpoint2;
        private NetTcpBinding tcpBinding2;

        private ServiceHost host3;
        private ServiceEndpoint endpoint3;
        private NetTcpBinding tcpBinding3;

        private ServiceHost host4;
        private ServiceEndpoint endpoint4;
        private NetTcpBinding tcpBinding4;

        static public readonly string GameManagerWindowConfigStringPrefix = "GameManagerWindow";
        static public readonly string PlayerWindowConfigStringPrefix = "PlayerWindow";
        static public readonly string GameStateWindowConfigString = "GameStateWindow";

        private MRUManager mruManager;
        public GeneralGameManager GeneralGameManager;
        private string KIFFilePath;
        private AutoResetEvent startNextTurnEvent = new AutoResetEvent(false);
        private AutoResetEvent prevTurnEvent = new AutoResetEvent(false);

        private Form managerOutputForm;
        private Form htmlOutputForm;
        private readonly List<Form> outputForms = new List<Form>();
        private IEWebBrowserListener htmlOutputFormListener;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            foreach (Form outputForm in outputForms)
            {
                WindowGeometryPersistence.SaveFormsGeometry(Properties.Settings.Default.WindowGeometry, outputForm);
            }
           Properties.Settings.Default.Save();

            if (GeneralGameManager != null)
            {
                GeneralGameManager.Dispose();
            }
        }

        protected void StartSWIPrologService1()
        {
            host1 = new ServiceHost(typeof(SWIPrologService));
            tcpBinding1 = new NetTcpBinding();
            tcpBinding1.MaxBufferPoolSize = 524288;
            tcpBinding1.MaxBufferSize = 262144;
            tcpBinding1.MaxReceivedMessageSize = 262144;
            tcpBinding1.MaxConnections = 1000;
            tcpBinding1.OpenTimeout = new TimeSpan(0, 1, 0, 0);
            tcpBinding1.ReceiveTimeout = new TimeSpan(0, 1, 0, 0);
            tcpBinding1.SendTimeout = new TimeSpan(0, 1, 0, 0);
            tcpBinding1.ReliableSession.InactivityTimeout = new TimeSpan(0, 1, 0, 0);
            tcpBinding1.Security.Mode = SecurityMode.None;
            tcpBinding1.Security.Message.AlgorithmSuite = SecurityAlgorithmSuite.Basic256;
            tcpBinding1.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
            tcpBinding1.Security.Transport.ProtectionLevel = ProtectionLevel.None;
            tcpBinding1.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            tcpBinding1.ReaderQuotas.MaxArrayLength = 524288;
            tcpBinding1.ReaderQuotas.MaxBytesPerRead = 524288;
            tcpBinding1.ReaderQuotas.MaxDepth = 524288;
            tcpBinding1.ReaderQuotas.MaxNameTableCharCount = 524288;
            tcpBinding1.ReaderQuotas.MaxStringContentLength = 524288;
            endpoint1 = host1.AddServiceEndpoint(typeof(ISWIPrologService), tcpBinding1, "net.tcp://localhost/Design_Time_Addresses/SWIPrologService/SWIPrologService1");

            try
            {
                host1.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }

            MessageBox.Show(host1.State.ToString());            
        }

        protected void StartSWIPrologService2()
        {
            host2 = new ServiceHost(typeof(SWIPrologService));
            tcpBinding2 = new NetTcpBinding();
            tcpBinding2.MaxBufferPoolSize = 524288;
            tcpBinding2.MaxBufferSize = 262144;
            tcpBinding2.MaxReceivedMessageSize = 262144;
            tcpBinding2.MaxConnections = 1000;
            tcpBinding2.OpenTimeout = new TimeSpan(0, 1, 0, 0);
            tcpBinding2.ReceiveTimeout = new TimeSpan(0, 1, 0, 0);
            tcpBinding2.SendTimeout = new TimeSpan(0, 1, 0, 0);
            tcpBinding2.ReliableSession.InactivityTimeout = new TimeSpan(0, 1, 0, 0);
            tcpBinding2.Security.Mode = SecurityMode.None;
            tcpBinding2.Security.Message.AlgorithmSuite = SecurityAlgorithmSuite.Basic256;
            tcpBinding2.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
            tcpBinding2.Security.Transport.ProtectionLevel = ProtectionLevel.None;
            tcpBinding2.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            tcpBinding2.ReaderQuotas.MaxArrayLength = 524288;
            tcpBinding2.ReaderQuotas.MaxBytesPerRead = 524288;
            tcpBinding2.ReaderQuotas.MaxDepth = 524288;
            tcpBinding2.ReaderQuotas.MaxNameTableCharCount = 524288;
            tcpBinding2.ReaderQuotas.MaxStringContentLength = 524288;
            endpoint2 = host2.AddServiceEndpoint(typeof(ISWIPrologService), tcpBinding2, "net.tcp://localhost:810/Design_Time_Addresses/SWIPrologService2/SWIPrologService1");

            try
            {
                host2.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }

            MessageBox.Show(host2.State.ToString());            
        }

        protected void StartSWIPrologService3()
        {
            host3 = new ServiceHost(typeof(SWIPrologService));
            tcpBinding3 = new NetTcpBinding();
            tcpBinding3.MaxBufferPoolSize = 524288;
            tcpBinding3.MaxBufferSize = 262144;
            tcpBinding3.MaxReceivedMessageSize = 262144;
            tcpBinding3.MaxConnections = 1000;
            tcpBinding3.OpenTimeout = new TimeSpan(0, 1, 0, 0);
            tcpBinding3.ReceiveTimeout = new TimeSpan(0, 1, 0, 0);
            tcpBinding3.SendTimeout = new TimeSpan(0, 1, 0, 0);
            tcpBinding3.ReliableSession.InactivityTimeout = new TimeSpan(0, 1, 0, 0);
            tcpBinding3.Security.Mode = SecurityMode.None;
            tcpBinding3.Security.Message.AlgorithmSuite = SecurityAlgorithmSuite.Basic256;
            tcpBinding3.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
            tcpBinding3.Security.Transport.ProtectionLevel = ProtectionLevel.None;
            tcpBinding3.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            tcpBinding3.ReaderQuotas.MaxArrayLength = 524288;
            tcpBinding3.ReaderQuotas.MaxBytesPerRead = 524288;
            tcpBinding3.ReaderQuotas.MaxDepth = 524288;
            tcpBinding3.ReaderQuotas.MaxNameTableCharCount = 524288;
            tcpBinding3.ReaderQuotas.MaxStringContentLength = 524288;
            endpoint3 = host3.AddServiceEndpoint(typeof(ISWIPrologService), tcpBinding3, "net.tcp://localhost:812/Design_Time_Addresses/SWIPrologService3/SWIPrologService1");

            try
            {
                host3.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }

            MessageBox.Show(host3.State.ToString());
        }

        protected void StartSWIPrologService4()
        {
            host4 = new ServiceHost(typeof(SWIPrologService));
            tcpBinding4 = new NetTcpBinding();
            tcpBinding4.MaxBufferPoolSize = 524288;
            tcpBinding4.MaxBufferSize = 262144;
            tcpBinding4.MaxReceivedMessageSize = 262144;
            tcpBinding4.MaxConnections = 1000;
            tcpBinding4.OpenTimeout = new TimeSpan(0, 1, 0, 0);
            tcpBinding4.ReceiveTimeout = new TimeSpan(0, 1, 0, 0);
            tcpBinding4.SendTimeout = new TimeSpan(0, 1, 0, 0);
            tcpBinding4.ReliableSession.InactivityTimeout = new TimeSpan(0, 1, 0, 0);
            tcpBinding4.Security.Mode = SecurityMode.None;
            tcpBinding4.Security.Message.AlgorithmSuite = SecurityAlgorithmSuite.Basic256;
            tcpBinding4.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
            tcpBinding4.Security.Transport.ProtectionLevel = ProtectionLevel.None;
            tcpBinding4.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            tcpBinding4.ReaderQuotas.MaxArrayLength = 524288;
            tcpBinding4.ReaderQuotas.MaxBytesPerRead = 524288;
            tcpBinding4.ReaderQuotas.MaxDepth = 524288;
            tcpBinding4.ReaderQuotas.MaxNameTableCharCount = 524288;
            tcpBinding4.ReaderQuotas.MaxStringContentLength = 524288;
            endpoint4 = host4.AddServiceEndpoint(typeof(ISWIPrologService), tcpBinding4, "net.tcp://localhost:8732/Design_Time_Addresses/SWIPrologService3/SWIPrologService1");

            try
            {
                host4.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }

            MessageBox.Show(host4.State.ToString());
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Space)
            {
                if (tsb_NextTurn.Enabled)
                    startNextTurnEvent.Set();
            }
            else if (e.KeyCode == Keys.Back)
            {
                if (tsb_PrevTurn.Enabled)
                    prevTurnEvent.Set();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            DebugAndTraceHelper.CustomLocation = Properties.Settings.Default.TempFilePath;

            mruManager = new MRUManager(selectRecentGameFileToolStripMenuItem, Application.ProductName,
                                        recentFileGotClicked, null);

            freeRunningToolStripMenuItem.Enabled = false;

            tsb_NextTurn.Enabled = false;
            tsb_PrevTurn.Enabled = false;

            startGameToolStripMenuItem.Enabled = false;

            saveCollapsedParseTreesToolStripMenuItem.Enabled = false;
            saveParseTreesToolStripMenuItem.Enabled = false;
            saveHornClausesToolStripMenuItem.Enabled = false;

            queryGamePrologEngineToolStripMenuItem.Enabled = false;
            dumpPrologEngineUserClausesToolStripMenuItem.Enabled = false;

            var dc = new DecoupledConsole();
            DebugAndTraceHelper.AddListener(new DecoupledConsoleTraceListener(dc));
 
            DebugAndTraceHelper.AddListener(new ToolStripStatusLabelListener(this.toolStripStatusLabel1), DebugAndTraceHelper.StatusStripChannelId);

            managerOutputForm = new Form()
                                    {
                                        MdiParent = this, 
                                        Text = DebugAndTraceHelper.ManagerChannelId, 
                                        StartPosition = FormStartPosition.Manual, 
                                        Location = new Point(0, 0),
                                        Width = this.ClientRectangle.Width - 5,
                                        Tag = GameManagerWindowConfigStringPrefix
                                    };
            var managerOutputTextBox = new TextBox() { Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Both };
            managerOutputForm.Controls.Add(managerOutputTextBox);
            DebugAndTraceHelper.AddListener(new TextboxTraceListener(managerOutputTextBox), DebugAndTraceHelper.ManagerChannelId);
            try
            {
                WindowGeometryPersistence.SetFormsGeometry(Properties.Settings.Default.WindowGeometry, managerOutputForm);
            }
            catch (Exception)
            {
                ;
            }
            managerOutputForm.Show();
            outputForms.Add(managerOutputForm);

            htmlOutputForm = new Form()
                                 {
                                     MdiParent = this, 
                                     StartPosition = FormStartPosition.Manual, 
                                     Location = new Point(0, managerOutputForm.Height), 
                                     Width = this.ClientRectangle.Width - 5,
                                     Tag = GameStateWindowConfigString
                                 };
            var webBrowser = new System.Windows.Forms.WebBrowser() {Dock = DockStyle.Fill, ScrollBarsEnabled = true};
            htmlOutputForm.Controls.Add(webBrowser);
            DebugAndTraceHelper.WriteTraceLine("WebBrowser version: " + webBrowser.Version, DebugAndTraceHelper.ManagerChannelId);
            htmlOutputFormListener = new IEWebBrowserListener(webBrowser);
            DebugAndTraceHelper.AddListener(htmlOutputFormListener, DebugAndTraceHelper.StateChannelId);
            WindowGeometryPersistence.SetFormsGeometry(Properties.Settings.Default.WindowGeometry, htmlOutputForm);
            htmlOutputForm.Show();
            outputForms.Add(htmlOutputForm);

            GeneralGame.UseOldWayOfGeneratingAndApplyingMoves =
                useOriginalMoveGenerationAndApplicationToolStripMenuItem.Checked;

            if (!System.Diagnostics.Debugger.IsAttached && false)
            {
                Thread t;
                t = new Thread(StartSWIPrologService1);
                t.IsBackground = true;
                t.Start();

                Thread t2;
                t2 = new Thread(StartSWIPrologService2);
                t2.IsBackground = true;
                t2.Start();

                Thread t3;
                t3 = new Thread(StartSWIPrologService3);
                t3.IsBackground = true;
                t3.Start();                
            }


#if PrologEngineTest
            PrologEngine.PrologEngine foo = new PrologEngine.PrologEngine();
            foo.Assert("cell(1,1,b)");
            foo.Assert("cell(2,2,x)");
            foreach (List<SWIPrologServiceLibrary.SolutionVariable> solutionVariables in foo.GetSolutionVariables("cell(X,Y,Z)"))
            {
                ;
            }

            PrologEngine.PrologEngine foo2 = new PrologEngine.PrologEngine();
            foo2.Assert("cell(2,2,o)");
            foo2.Assert("cell(3,3,x)");
            foreach (List<SWIPrologServiceLibrary.SolutionVariable> solutionVariables in foo2.GetSolutionVariables("cell(X,Y,Z)"))
            {
                ;
            }

            PrologEngine.PrologEngine foo3 = new PrologEngine.PrologEngine();
            foo3.Assert("cell(1,2,b)");
            foo3.Assert("cell(2,3,o)");
            foreach (List<SWIPrologServiceLibrary.SolutionVariable> solutionVariables in foo3.GetSolutionVariables("cell(X,Y,Z)"))
            {
                ;
            }

            foreach (List<SWIPrologServiceLibrary.SolutionVariable> solutionVariables in foo2.GetSolutionVariables("cell(X,Y,Z)"))
            {
                ;
            }

            foreach (List<SWIPrologServiceLibrary.SolutionVariable> solutionVariables in foo.GetSolutionVariables("cell(X,Y,Z)"))
            {
                ;
            }
#endif
        }

        private void recentFileGotClicked(object obj, EventArgs e)
        {
            OpenKIFFile((obj as ToolStripMenuItem).Text);
            DebugAndTraceHelper.WriteTraceLine("Game file loaded, please select Start Game to continue...", DebugAndTraceHelper.StatusStripChannelId);
            managerOutputForm.Text = GeneralGameManager.TheGeneralGame.Name;

            if (autoStartToolStripMenuItem.Checked)
            {
                startGameToolStripMenuItem_Click(null, null);
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void queryGamePrologEngineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new PrologQuery(GeneralGameManager.TheGeneralGame.PrologEngine);
            dialog.Show(this);
        }

        private void dumpPrologEngineUserClausesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ToDo:  What do I do with this??
            //GeneralGameManager.TheGeneralGame.PrologEngine.Ps.ListAll(null, -1, false, true);
        }

        private void startGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsb_NextTurn.Enabled = true;
            freeRunningToolStripMenuItem.Enabled = true;
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += Play;
            backgroundWorker.RunWorkerAsync();
        }

        private void Play(object sender, DoWorkEventArgs e)
        {
            GeneralGameManager.Play();
        }

        private void selectGameFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
                                     {
                                         Multiselect = false,
                                         FileName = Properties.Settings.Default.InitialGameFileName,
                                         InitialDirectory = Properties.Settings.Default.InitialGameFileLocationDirectory,
                                         RestoreDirectory = true,
                                         Filter = "GDL files (*.kif;*.gdl)|*.kif;*.gdl|All files (*.*)|*.*",
                                         FilterIndex = 0
                                     };

            var dlgResult = openFileDialog.ShowDialog(this);
            if (dlgResult == DialogResult.Cancel)
            {
                return;
            }

            OpenKIFFile(openFileDialog.FileName);
            DebugAndTraceHelper.WriteTraceLine("Game file loaded, please select Start Game to continue...", DebugAndTraceHelper.StatusStripChannelId);
            managerOutputForm.Text = GeneralGameManager.TheGeneralGame.Name;

            if (autoStartToolStripMenuItem.Checked)
            {
                startGameToolStripMenuItem_Click(null, null);
            }
        }

        private void OpenKIFFile(string kifFilePath)
        {
            KIFFilePath = kifFilePath;
            mruManager.AddRecentFile(KIFFilePath);
            GeneralGameManager = new GeneralGameManager(KIFFilePath, 10, 10, Properties.Settings.Default.WcfSvcHostExecutable, Properties.Settings.Default.TempFilePath) {StartNextMoveEvent = startNextTurnEvent, PrevMoveEvent = prevTurnEvent};
            GeneralGame.UseOldWayOfGeneratingAndApplyingMoves =
                useOriginalMoveGenerationAndApplicationToolStripMenuItem.Checked;

            playerToolStripMenuItem.DropDownItems.Clear();
            int playerOutputWindowX = 0;
            int playerOutputWindowY = htmlOutputForm.Location.Y + htmlOutputForm.Height;
            int playerOutputFormWidth = (this.ClientRectangle.Width - 5)/(GeneralGameManager.TheGeneralGame.FindRoles().Count() == 0 ? 1 : GeneralGameManager.TheGeneralGame.FindRoles().Count());
            int playerOutputFormHeight = this.ClientRectangle.Height - 25 - this.statusStrip1.Height - managerOutputForm.Height - htmlOutputForm.Height - (2 * SystemInformation.HorizontalScrollBarHeight);
            int playerFormCount = 0;
            foreach (string role in GeneralGameManager.TheGeneralGame.FindRoles())
            {
                var playerOutputForm = new Form()
                                           {
                                               MdiParent = this,
                                               Text = role,
                                               StartPosition = FormStartPosition.Manual,
                                               Location = new Point(playerOutputWindowX, playerOutputWindowY),
                                               Width = playerOutputFormWidth,
                                               Height = playerOutputFormHeight,
                                               Tag = PlayerWindowConfigStringPrefix + playerFormCount
                                           };
                var playerOutputTextBox = new TextBox() {Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Both};
                playerOutputForm.Controls.Add(playerOutputTextBox);
                DebugAndTraceHelper.AddListener(new TextboxTraceListener(playerOutputTextBox), role);
                playerOutputWindowX += playerOutputForm.Bounds.Width;
                WindowGeometryPersistence.SetFormsGeometry(Properties.Settings.Default.WindowGeometry, playerOutputForm);
                playerOutputForm.Show();
                outputForms.Add(playerOutputForm);

                var mi = new ToolStripMenuItem(role);
                playerToolStripMenuItem.DropDownItems.Add(mi);

                var submi = new ToolStripMenuItem("Query Player Prolog Engine");
                mi.DropDownItems.Add(submi);
                submi.Click += (o, k) =>
                                   {
                                       var dialog = new PrologQuery(GeneralGameManager.Players.Where(n => n.Role == role).First().GetPrologEngine());
                                       dialog.Show(this);
                                   };

                playerFormCount++;
            }

            startGameToolStripMenuItem.Enabled = true;

            saveCollapsedParseTreesToolStripMenuItem.Enabled = true;
            saveParseTreesToolStripMenuItem.Enabled = true;
            saveHornClausesToolStripMenuItem.Enabled = true;

            queryGamePrologEngineToolStripMenuItem.Enabled = true;
            dumpPrologEngineUserClausesToolStripMenuItem.Enabled = true;
        }

        private void tsb_NextTurn_Click(object sender, EventArgs e)
        {
            startNextTurnEvent.Set();
        }

        private void tsb_PrevTurn_Click(object sender, EventArgs e)
        {
            prevTurnEvent.Set();
        }

        private void freeRunningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GeneralGameManager.StartNextMoveEvent = freeRunningToolStripMenuItem.Checked ? null : startNextTurnEvent;
        }

        private void saveCollapsedParseTreesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.InitialDirectory = Properties.Settings.Default.TempFilePath;
            sfd.FileName = Path.GetFileNameWithoutExtension(GeneralGameManager.TheGeneralGame.KIFFilePath);
            //sfd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            sfd.Filter = "All files (*.*)|*.*";
            sfd.RestoreDirectory = true;
            var dlgResult = sfd.ShowDialog();
            if (dlgResult == DialogResult.OK)
            {                
                GeneralGameManager.TheGeneralGame.SaveCollapsedParseTrees(Path.GetDirectoryName(sfd.FileName), Path.GetFileNameWithoutExtension(sfd.FileName), FileMode.Create);
            }
        }

        private void saveParseTreesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.InitialDirectory = Properties.Settings.Default.TempFilePath;
            sfd.FileName = Path.GetFileNameWithoutExtension(GeneralGameManager.TheGeneralGame.KIFFilePath);
            //sfd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            sfd.Filter = "All files (*.*)|*.*";
            sfd.RestoreDirectory = true;
            var dlgResult = sfd.ShowDialog();
            if (dlgResult == DialogResult.OK)
            {
                GeneralGameManager.TheGeneralGame.SaveParseTrees(Path.GetDirectoryName(sfd.FileName),  Path.GetFileNameWithoutExtension(sfd.FileName), FileMode.Create);                
            }
        }

        private void allowFreeRunningOfTurnsWithNoPlayerMovesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GeneralGameManager.AllowFreeRunningOfTurnsWithNoPlayerMoves = allowFreeRunningOfTurnsWithNoPlayerMovesToolStripMenuItem.Checked;
        }

        private void milliSecondsBetweenFreeRunningTermsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string input;
            var dialogResult = InputBox.ShowDialog("",
                                                   "Please enter number of milliseconds",
                                                   GeneralGameManager.MilliSecondsBetweenFreeRunningMoves.ToString(),
                                                   out input,
                                                   InputBoxResultType.Int32,
                                                   false,
                                                   this);

            if (dialogResult != DialogResult.Cancel)
            {
                GeneralGameManager.MilliSecondsBetweenFreeRunningMoves = Int32.Parse(input);
            }
        }

        private void useOriginalMoveGenerationAndApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GeneralGame.UseOldWayOfGeneratingAndApplyingMoves =
                useOriginalMoveGenerationAndApplicationToolStripMenuItem.Checked;
        }

        private void startGameFromHistoryFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                Multiselect = false,
                FileName = Properties.Settings.Default.InitialGameFileName,
                InitialDirectory = Properties.Settings.Default.InitialGameFileLocationDirectory,
                RestoreDirectory = true,
                Filter = "GDL files (*.kif;*.gdl)|*.kif;*.gdl|All files (*.*)|*.*",
                FilterIndex = 0
            };

            var dlgResult = openFileDialog.ShowDialog(this);
            if (dlgResult == DialogResult.Cancel)
            {
                return;
            }

            var gameFileName = openFileDialog.FileName;
            OpenKIFFile(gameFileName);
            //DebugAndTraceHelper.WriteTraceLine("Game file loaded, please select Start Game to continue...", DebugAndTraceHelper.StatusStripChannelId);
            managerOutputForm.Text = GeneralGameManager.TheGeneralGame.Name;

            //if (autoStartToolStripMenuItem.Checked)
            //{
            //    startGameToolStripMenuItem_Click(null, null);
            //}


            openFileDialog = new OpenFileDialog()
            {
                Multiselect = false,
                InitialDirectory = Properties.Settings.Default.InitialGameFileLocationDirectory,
                RestoreDirectory = true,
                Filter = "GDL files (*.his)|*.his|All files (*.*)|*.*",
                FilterIndex = 0
            };

            dlgResult = openFileDialog.ShowDialog(this);
            if (dlgResult == DialogResult.Cancel)
            {
                return;
            }

            var historyFileName = openFileDialog.FileName;

            tsb_NextTurn.Enabled = true;
            tsb_PrevTurn.Enabled = true;
            freeRunningToolStripMenuItem.Enabled = true;
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (o, args) => GeneralGameManager.PlayFromHistoryFile(historyFileName);
            backgroundWorker.RunWorkerAsync();
        }
    }
}
