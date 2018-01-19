namespace Server
{
    partial class Main
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
            this.components = new System.ComponentModel.Container();
            this.clientList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.disconnectClientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chatWithClientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtReceive = new System.Windows.Forms.TextBox();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // clientList
            // 
            this.clientList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.clientList.ContextMenuStrip = this.menu;
            this.clientList.FullRowSelect = true;
            this.clientList.GridLines = true;
            this.clientList.Location = new System.Drawing.Point(224, 12);
            this.clientList.Name = "clientList";
            this.clientList.Size = new System.Drawing.Size(503, 293);
            this.clientList.TabIndex = 0;
            this.clientList.UseCompatibleStateImageBehavior = false;
            this.clientList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "IP Adress";
            this.columnHeader1.Width = 152;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Nickname";
            this.columnHeader2.Width = 199;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Status";
            this.columnHeader3.Width = 257;
            // 
            // menu
            // 
            this.menu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.disconnectClientToolStripMenuItem,
            this.chatWithClientToolStripMenuItem});
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(194, 52);
            // 
            // disconnectClientToolStripMenuItem
            // 
            this.disconnectClientToolStripMenuItem.Name = "disconnectClientToolStripMenuItem";
            this.disconnectClientToolStripMenuItem.Size = new System.Drawing.Size(193, 24);
            this.disconnectClientToolStripMenuItem.Text = "Disconnect Client";
            this.disconnectClientToolStripMenuItem.Click += new System.EventHandler(this.disconnectClientToolStripMenuItem_Click);
            // 
            // chatWithClientToolStripMenuItem
            // 
            this.chatWithClientToolStripMenuItem.Name = "chatWithClientToolStripMenuItem";
            this.chatWithClientToolStripMenuItem.Size = new System.Drawing.Size(193, 24);
            this.chatWithClientToolStripMenuItem.Text = "Chat with client";
            this.chatWithClientToolStripMenuItem.Click += new System.EventHandler(this.chatWithClientToolStripMenuItem_Click);
            // 
            // txtReceive
            // 
            this.txtReceive.HideSelection = false;
            this.txtReceive.Location = new System.Drawing.Point(12, 12);
            this.txtReceive.Multiline = true;
            this.txtReceive.Name = "txtReceive";
            this.txtReceive.ReadOnly = true;
            this.txtReceive.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtReceive.Size = new System.Drawing.Size(206, 319);
            this.txtReceive.TabIndex = 2;
            this.txtReceive.TextChanged += new System.EventHandler(this.txtReceive_TextChanged);
            // 
            // txtInput
            // 
            this.txtInput.Location = new System.Drawing.Point(224, 311);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(395, 22);
            this.txtInput.TabIndex = 3;
            this.txtInput.Text = "Admin Says: Hey";
            this.txtInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyDown);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(625, 311);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(102, 24);
            this.btnSend.TabIndex = 4;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(739, 341);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.txtReceive);
            this.Controls.Add(this.clientList);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TCP Chat Server";
            this.Load += new System.EventHandler(this.Main_Load);
            this.menu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ContextMenuStrip menu;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ToolStripMenuItem disconnectClientToolStripMenuItem;
        public System.Windows.Forms.ListView clientList;
        public System.Windows.Forms.TextBox txtReceive;
        public System.Windows.Forms.TextBox txtInput;
        public System.Windows.Forms.Button btnSend;
        public System.Windows.Forms.ToolStripMenuItem chatWithClientToolStripMenuItem;
    }
}

