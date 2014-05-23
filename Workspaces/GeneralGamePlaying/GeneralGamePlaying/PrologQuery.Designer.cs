namespace API.GGP.GeneralGamePlayingNS
{
    partial class PrologQuery
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txb_Query = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_Submit = new System.Windows.Forms.Button();
            this.btn_ListAll = new System.Windows.Forms.Button();
            this.btn_ListAllNonAlphabetically = new System.Windows.Forms.Button();
            this.btn_ListAllFacts = new System.Windows.Forms.Button();
            this.btn_SaveResults = new System.Windows.Forms.Button();
            this.txb_QueryResults = new System.Windows.Forms.TextBox();
            this.btn_Copy = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txb_QueryResults);
            this.splitContainer1.Size = new System.Drawing.Size(697, 828);
            this.splitContainer1.SplitterDistance = 353;
            this.splitContainer1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.txb_Query, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(697, 353);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // txb_Query
            // 
            this.txb_Query.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txb_Query.Location = new System.Drawing.Point(8, 8);
            this.txb_Query.Multiline = true;
            this.txb_Query.Name = "txb_Query";
            this.txb_Query.Size = new System.Drawing.Size(594, 337);
            this.txb_Query.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.btn_Submit);
            this.flowLayoutPanel1.Controls.Add(this.btn_ListAll);
            this.flowLayoutPanel1.Controls.Add(this.btn_ListAllNonAlphabetically);
            this.flowLayoutPanel1.Controls.Add(this.btn_ListAllFacts);
            this.flowLayoutPanel1.Controls.Add(this.btn_SaveResults);
            this.flowLayoutPanel1.Controls.Add(this.btn_Copy);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(608, 8);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(81, 337);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btn_Submit
            // 
            this.btn_Submit.Location = new System.Drawing.Point(3, 3);
            this.btn_Submit.Name = "btn_Submit";
            this.btn_Submit.Size = new System.Drawing.Size(75, 23);
            this.btn_Submit.TabIndex = 0;
            this.btn_Submit.Text = "Submit";
            this.btn_Submit.UseVisualStyleBackColor = true;
            this.btn_Submit.Click += new System.EventHandler(this.btn_Submit_Click);
            // 
            // btn_ListAll
            // 
            this.btn_ListAll.Location = new System.Drawing.Point(3, 32);
            this.btn_ListAll.Name = "btn_ListAll";
            this.btn_ListAll.Size = new System.Drawing.Size(75, 23);
            this.btn_ListAll.TabIndex = 1;
            this.btn_ListAll.Text = "List All";
            this.btn_ListAll.UseVisualStyleBackColor = true;
            this.btn_ListAll.Click += new System.EventHandler(this.btn_ListAll_Click);
            // 
            // btn_ListAllNonAlphabetically
            // 
            this.btn_ListAllNonAlphabetically.Location = new System.Drawing.Point(3, 61);
            this.btn_ListAllNonAlphabetically.Name = "btn_ListAllNonAlphabetically";
            this.btn_ListAllNonAlphabetically.Size = new System.Drawing.Size(75, 23);
            this.btn_ListAllNonAlphabetically.TabIndex = 4;
            this.btn_ListAllNonAlphabetically.Text = "List All Orig Order";
            this.btn_ListAllNonAlphabetically.UseVisualStyleBackColor = true;
            this.btn_ListAllNonAlphabetically.Click += new System.EventHandler(this.btn_ListAllNonAlphabetically_Click);
            // 
            // btn_ListAllFacts
            // 
            this.btn_ListAllFacts.Location = new System.Drawing.Point(3, 90);
            this.btn_ListAllFacts.Name = "btn_ListAllFacts";
            this.btn_ListAllFacts.Size = new System.Drawing.Size(75, 23);
            this.btn_ListAllFacts.TabIndex = 3;
            this.btn_ListAllFacts.Text = "All Facts";
            this.btn_ListAllFacts.UseVisualStyleBackColor = true;
            this.btn_ListAllFacts.Click += new System.EventHandler(this.btn_ListAllFacts_Click);
            // 
            // btn_SaveResults
            // 
            this.btn_SaveResults.Location = new System.Drawing.Point(3, 119);
            this.btn_SaveResults.Name = "btn_SaveResults";
            this.btn_SaveResults.Size = new System.Drawing.Size(75, 23);
            this.btn_SaveResults.TabIndex = 2;
            this.btn_SaveResults.Text = "Save Results";
            this.btn_SaveResults.UseVisualStyleBackColor = true;
            this.btn_SaveResults.Click += new System.EventHandler(this.btn_SaveResults_Click);
            // 
            // txb_QueryResults
            // 
            this.txb_QueryResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txb_QueryResults.Location = new System.Drawing.Point(0, 0);
            this.txb_QueryResults.Multiline = true;
            this.txb_QueryResults.Name = "txb_QueryResults";
            this.txb_QueryResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txb_QueryResults.Size = new System.Drawing.Size(697, 471);
            this.txb_QueryResults.TabIndex = 0;
            // 
            // btn_Copy
            // 
            this.btn_Copy.Location = new System.Drawing.Point(3, 148);
            this.btn_Copy.Name = "btn_Copy";
            this.btn_Copy.Size = new System.Drawing.Size(75, 23);
            this.btn_Copy.TabIndex = 5;
            this.btn_Copy.Text = "Copy to Clipboard";
            this.btn_Copy.UseVisualStyleBackColor = true;
            this.btn_Copy.Click += new System.EventHandler(this.btn_Copy_Click);
            // 
            // PrologQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(697, 828);
            this.Controls.Add(this.splitContainer1);
            this.KeyPreview = true;
            this.Name = "PrologQuery";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PrologQuery";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PrologQuery_KeyDown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txb_QueryResults;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox txb_Query;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btn_Submit;
        private System.Windows.Forms.Button btn_ListAll;
        private System.Windows.Forms.Button btn_SaveResults;
        private System.Windows.Forms.Button btn_ListAllFacts;
        private System.Windows.Forms.Button btn_ListAllNonAlphabetically;
        private System.Windows.Forms.Button btn_Copy;

    }
}