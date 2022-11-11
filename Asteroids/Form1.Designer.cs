namespace Asteroids
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
          this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.highScoresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.rulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
          this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.menuStrip1.SuspendLayout();
          this.SuspendLayout();
          // 
          // menuStrip1
          // 
          this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
          this.menuStrip1.Location = new System.Drawing.Point(0, 0);
          this.menuStrip1.Name = "menuStrip1";
          this.menuStrip1.Size = new System.Drawing.Size(490, 24);
          this.menuStrip1.TabIndex = 0;
          this.menuStrip1.Text = "menuStrip1";
          // 
          // fileToolStripMenuItem
          // 
          this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.highScoresToolStripMenuItem,
            this.rulesToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
          this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
          this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
          this.fileToolStripMenuItem.Text = "&File";
          // 
          // highScoresToolStripMenuItem
          // 
          this.highScoresToolStripMenuItem.Name = "highScoresToolStripMenuItem";
          this.highScoresToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
          this.highScoresToolStripMenuItem.Text = "&High Scores";
          this.highScoresToolStripMenuItem.Click += new System.EventHandler(this.highScoresToolStripMenuItem_Click);
          // 
          // rulesToolStripMenuItem
          // 
          this.rulesToolStripMenuItem.Name = "rulesToolStripMenuItem";
          this.rulesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
          this.rulesToolStripMenuItem.Text = "&Rules";
          this.rulesToolStripMenuItem.Click += new System.EventHandler(this.rulesToolStripMenuItem_Click);
          // 
          // toolStripMenuItem1
          // 
          this.toolStripMenuItem1.Name = "toolStripMenuItem1";
          this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
          // 
          // exitToolStripMenuItem
          // 
          this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
          this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
          this.exitToolStripMenuItem.Text = "&Exit";
          this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
          // 
          // Form1
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.ClientSize = new System.Drawing.Size(490, 456);
          this.Controls.Add(this.menuStrip1);
          this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
          this.MainMenuStrip = this.menuStrip1;
          this.MinimizeBox = false;
          this.Name = "Form1";
          this.Text = "C# Asteroids";
          this.menuStrip1.ResumeLayout(false);
          this.menuStrip1.PerformLayout();
          this.ResumeLayout(false);
          this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem highScoresToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rulesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
      private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    }
}

