namespace API.GGP.GeneralGamePlayingNS
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.gameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectGameFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectRecentGameFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.startGameFromHistoryFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.nextTurnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.queryGamePrologEngineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dumpPrologEngineUserClausesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveCollapsedParseTreesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveParseTreesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveHornClausesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.freeRunningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.milliSecondsBetweenFreeRunningTermsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useOriginalMoveGenerationAndApplicationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allowFreeRunningOfTurnsWithNoPlayerMovesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_PrevTurn = new System.Windows.Forms.ToolStripButton();
            this.tsb_NextTurn = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gameToolStripMenuItem,
            this.playerToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(950, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // gameToolStripMenuItem
            // 
            this.gameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectGameFileToolStripMenuItem,
            this.selectRecentGameFileToolStripMenuItem,
            this.startGameToolStripMenuItem,
            this.toolStripSeparator3,
            this.startGameFromHistoryFileToolStripMenuItem,
            this.toolStripSeparator1,
            this.nextTurnToolStripMenuItem,
            this.toolStripSeparator4,
            this.queryGamePrologEngineToolStripMenuItem,
            this.dumpPrologEngineUserClausesToolStripMenuItem,
            this.toolStripSeparator2,
            this.saveCollapsedParseTreesToolStripMenuItem,
            this.saveParseTreesToolStripMenuItem,
            this.saveHornClausesToolStripMenuItem});
            this.gameToolStripMenuItem.Name = "gameToolStripMenuItem";
            this.gameToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.gameToolStripMenuItem.Text = "Game";
            // 
            // selectGameFileToolStripMenuItem
            // 
            this.selectGameFileToolStripMenuItem.Name = "selectGameFileToolStripMenuItem";
            this.selectGameFileToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.selectGameFileToolStripMenuItem.Text = "Select Game File";
            this.selectGameFileToolStripMenuItem.Click += new System.EventHandler(this.selectGameFileToolStripMenuItem_Click);
            // 
            // selectRecentGameFileToolStripMenuItem
            // 
            this.selectRecentGameFileToolStripMenuItem.Name = "selectRecentGameFileToolStripMenuItem";
            this.selectRecentGameFileToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.selectRecentGameFileToolStripMenuItem.Text = "Select Recent Game File";
            // 
            // startGameToolStripMenuItem
            // 
            this.startGameToolStripMenuItem.Name = "startGameToolStripMenuItem";
            this.startGameToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.startGameToolStripMenuItem.Text = "Start Game";
            this.startGameToolStripMenuItem.Click += new System.EventHandler(this.startGameToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(250, 6);
            // 
            // startGameFromHistoryFileToolStripMenuItem
            // 
            this.startGameFromHistoryFileToolStripMenuItem.Name = "startGameFromHistoryFileToolStripMenuItem";
            this.startGameFromHistoryFileToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.startGameFromHistoryFileToolStripMenuItem.Text = "Start Game From History File";
            this.startGameFromHistoryFileToolStripMenuItem.Click += new System.EventHandler(this.startGameFromHistoryFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(250, 6);
            // 
            // nextTurnToolStripMenuItem
            // 
            this.nextTurnToolStripMenuItem.Name = "nextTurnToolStripMenuItem";
            this.nextTurnToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.nextTurnToolStripMenuItem.Text = "Next Turn";
            this.nextTurnToolStripMenuItem.Click += new System.EventHandler(this.tsb_NextTurn_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(250, 6);
            // 
            // queryGamePrologEngineToolStripMenuItem
            // 
            this.queryGamePrologEngineToolStripMenuItem.Name = "queryGamePrologEngineToolStripMenuItem";
            this.queryGamePrologEngineToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.queryGamePrologEngineToolStripMenuItem.Text = "Query Game Prolog Engine";
            this.queryGamePrologEngineToolStripMenuItem.Click += new System.EventHandler(this.queryGamePrologEngineToolStripMenuItem_Click);
            // 
            // dumpPrologEngineUserClausesToolStripMenuItem
            // 
            this.dumpPrologEngineUserClausesToolStripMenuItem.Name = "dumpPrologEngineUserClausesToolStripMenuItem";
            this.dumpPrologEngineUserClausesToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.dumpPrologEngineUserClausesToolStripMenuItem.Text = "Dump Prolog Engine User Clauses";
            this.dumpPrologEngineUserClausesToolStripMenuItem.Click += new System.EventHandler(this.dumpPrologEngineUserClausesToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(250, 6);
            // 
            // saveCollapsedParseTreesToolStripMenuItem
            // 
            this.saveCollapsedParseTreesToolStripMenuItem.Name = "saveCollapsedParseTreesToolStripMenuItem";
            this.saveCollapsedParseTreesToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.saveCollapsedParseTreesToolStripMenuItem.Text = "Save Collapsed Parse Trees";
            this.saveCollapsedParseTreesToolStripMenuItem.Click += new System.EventHandler(this.saveCollapsedParseTreesToolStripMenuItem_Click);
            // 
            // saveParseTreesToolStripMenuItem
            // 
            this.saveParseTreesToolStripMenuItem.Name = "saveParseTreesToolStripMenuItem";
            this.saveParseTreesToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.saveParseTreesToolStripMenuItem.Text = "Save Parse Trees";
            this.saveParseTreesToolStripMenuItem.Click += new System.EventHandler(this.saveParseTreesToolStripMenuItem_Click);
            // 
            // saveHornClausesToolStripMenuItem
            // 
            this.saveHornClausesToolStripMenuItem.Enabled = false;
            this.saveHornClausesToolStripMenuItem.Name = "saveHornClausesToolStripMenuItem";
            this.saveHornClausesToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.saveHornClausesToolStripMenuItem.Text = "Save Horn Clauses";
            // 
            // playerToolStripMenuItem
            // 
            this.playerToolStripMenuItem.Name = "playerToolStripMenuItem";
            this.playerToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.playerToolStripMenuItem.Text = "Player";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoStartToolStripMenuItem,
            this.freeRunningToolStripMenuItem,
            this.milliSecondsBetweenFreeRunningTermsToolStripMenuItem,
            this.useOriginalMoveGenerationAndApplicationToolStripMenuItem,
            this.allowFreeRunningOfTurnsWithNoPlayerMovesToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // autoStartToolStripMenuItem
            // 
            this.autoStartToolStripMenuItem.Checked = true;
            this.autoStartToolStripMenuItem.CheckOnClick = true;
            this.autoStartToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoStartToolStripMenuItem.Name = "autoStartToolStripMenuItem";
            this.autoStartToolStripMenuItem.Size = new System.Drawing.Size(346, 22);
            this.autoStartToolStripMenuItem.Text = "Auto Start";
            // 
            // freeRunningToolStripMenuItem
            // 
            this.freeRunningToolStripMenuItem.CheckOnClick = true;
            this.freeRunningToolStripMenuItem.Name = "freeRunningToolStripMenuItem";
            this.freeRunningToolStripMenuItem.Size = new System.Drawing.Size(346, 22);
            this.freeRunningToolStripMenuItem.Text = "Free Running";
            this.freeRunningToolStripMenuItem.Click += new System.EventHandler(this.freeRunningToolStripMenuItem_Click);
            // 
            // milliSecondsBetweenFreeRunningTermsToolStripMenuItem
            // 
            this.milliSecondsBetweenFreeRunningTermsToolStripMenuItem.Name = "milliSecondsBetweenFreeRunningTermsToolStripMenuItem";
            this.milliSecondsBetweenFreeRunningTermsToolStripMenuItem.Size = new System.Drawing.Size(346, 22);
            this.milliSecondsBetweenFreeRunningTermsToolStripMenuItem.Text = "MilliSeconds Between Free Running Terms";
            this.milliSecondsBetweenFreeRunningTermsToolStripMenuItem.Click += new System.EventHandler(this.milliSecondsBetweenFreeRunningTermsToolStripMenuItem_Click);
            // 
            // useOriginalMoveGenerationAndApplicationToolStripMenuItem
            // 
            this.useOriginalMoveGenerationAndApplicationToolStripMenuItem.CheckOnClick = true;
            this.useOriginalMoveGenerationAndApplicationToolStripMenuItem.Name = "useOriginalMoveGenerationAndApplicationToolStripMenuItem";
            this.useOriginalMoveGenerationAndApplicationToolStripMenuItem.Size = new System.Drawing.Size(346, 22);
            this.useOriginalMoveGenerationAndApplicationToolStripMenuItem.Text = "Use Original Move Generation and Application";
            this.useOriginalMoveGenerationAndApplicationToolStripMenuItem.Click += new System.EventHandler(this.useOriginalMoveGenerationAndApplicationToolStripMenuItem_Click);
            // 
            // allowFreeRunningOfTurnsWithNoPlayerMovesToolStripMenuItem
            // 
            this.allowFreeRunningOfTurnsWithNoPlayerMovesToolStripMenuItem.Checked = true;
            this.allowFreeRunningOfTurnsWithNoPlayerMovesToolStripMenuItem.CheckOnClick = true;
            this.allowFreeRunningOfTurnsWithNoPlayerMovesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.allowFreeRunningOfTurnsWithNoPlayerMovesToolStripMenuItem.Name = "allowFreeRunningOfTurnsWithNoPlayerMovesToolStripMenuItem";
            this.allowFreeRunningOfTurnsWithNoPlayerMovesToolStripMenuItem.Size = new System.Drawing.Size(346, 22);
            this.allowFreeRunningOfTurnsWithNoPlayerMovesToolStripMenuItem.Text = "Allow Free Running Of Turns With No Player Moves";
            this.allowFreeRunningOfTurnsWithNoPlayerMovesToolStripMenuItem.Click += new System.EventHandler(this.allowFreeRunningOfTurnsWithNoPlayerMovesToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_PrevTurn,
            this.tsb_NextTurn});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(950, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsb_PrevTurn
            // 
            this.tsb_PrevTurn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsb_PrevTurn.Image = ((System.Drawing.Image)(resources.GetObject("tsb_PrevTurn.Image")));
            this.tsb_PrevTurn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_PrevTurn.Name = "tsb_PrevTurn";
            this.tsb_PrevTurn.Size = new System.Drawing.Size(34, 22);
            this.tsb_PrevTurn.Text = "Prev";
            this.tsb_PrevTurn.Click += new System.EventHandler(this.tsb_PrevTurn_Click);
            // 
            // tsb_NextTurn
            // 
            this.tsb_NextTurn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsb_NextTurn.Image = ((System.Drawing.Image)(resources.GetObject("tsb_NextTurn.Image")));
            this.tsb_NextTurn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_NextTurn.Name = "tsb_NextTurn";
            this.tsb_NextTurn.Size = new System.Drawing.Size(35, 22);
            this.tsb_NextTurn.Text = "Next";
            this.tsb_NextTurn.Click += new System.EventHandler(this.tsb_NextTurn_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 714);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(950, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 736);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "General Game Playing";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem gameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem queryGamePrologEngineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dumpPrologEngineUserClausesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectGameFileToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_NextTurn;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem freeRunningToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem saveCollapsedParseTreesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveParseTreesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveHornClausesToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem milliSecondsBetweenFreeRunningTermsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectRecentGameFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoStartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextTurnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useOriginalMoveGenerationAndApplicationToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem startGameFromHistoryFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton tsb_PrevTurn;
        private System.Windows.Forms.ToolStripMenuItem allowFreeRunningOfTurnsWithNoPlayerMovesToolStripMenuItem;
    }
}

