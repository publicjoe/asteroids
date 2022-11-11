using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Publicjoe.Windows
{
  public partial class HighScoreForm : Form
  {
    private HighScoreTable highScoreTable = null;

    public HighScoreForm()
    {
      InitializeComponent();
    }

    public HighScoreForm(HighScoreTable highScoreTableReference)
    {
      InitializeComponent();
      highScoreTable = highScoreTableReference;
    }

    private void HighScore_Load(object sender, EventArgs e)
    {
      highScoreTable.Load(Application.StartupPath + @"\score.txt");
      highScoreTable.Populate(listView1);
    }
  }
}