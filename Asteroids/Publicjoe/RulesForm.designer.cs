namespace Publicjoe.Windows
{
  partial class RulesForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RulesForm));
      this.richTextBox1 = new System.Windows.Forms.RichTextBox();
      this.SuspendLayout();
      // 
      // richTextBox1
      // 
      this.richTextBox1.BackColor = System.Drawing.Color.White;
      this.richTextBox1.Location = new System.Drawing.Point(12, 12);
      this.richTextBox1.Name = "richTextBox1";
      this.richTextBox1.ReadOnly = true;
      this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
      this.richTextBox1.Size = new System.Drawing.Size(655, 444);
      this.richTextBox1.TabIndex = 0;
      this.richTextBox1.Text = "";
      // 
      // RulesForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(679, 468);
      this.Controls.Add(this.richTextBox1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.Name = "RulesForm";
      this.Text = "RulesForm";
      this.Load += new System.EventHandler(this.Rules_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.RichTextBox richTextBox1;
  }
}