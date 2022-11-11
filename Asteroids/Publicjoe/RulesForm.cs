using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Publicjoe.Windows
{
  public partial class RulesForm : Form
  {
    public RulesForm()
    {
      InitializeComponent();
    }

    private void Rules_Load(object sender, EventArgs e)
    {
      try
      {
        richTextBox1.LoadFile(Application.StartupPath + @"\rules.rtf", RichTextBoxStreamType.RichText);
      }
      catch
      {
        MessageBox.Show("Rules not found");
      }
    }
  }
}