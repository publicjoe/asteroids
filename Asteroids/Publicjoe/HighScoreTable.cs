using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Publicjoe.Windows
{
  /// <summary>
  /// Description of HighScoreTable.
  /// </summary>
  public class HighScoreTable
  {
    private List<HighScoreEntry> table = new List<HighScoreEntry>();

    private bool isLoaded;
  
    public HighScoreTable()
    {
      isLoaded = false;
    }
  
    public void Load( string path )
    {
      if( File.Exists( path ) )
      {
        table = new List<HighScoreEntry>();

        using ( var textStream = new StreamReader(path, Encoding.UTF8) )
        {
          string scoreLine;
  
          while( (scoreLine = textStream.ReadLine()) != null )
          {
            var scoreParts = scoreLine.Split(',');
  
            if( scoreParts.Length != 2 )
            {
              throw new ApplicationException("Score file corrupt!");
            }
            else
            {
              table.Add(new HighScoreEntry(scoreParts[0],Int32.Parse(scoreParts[1])));
            }
          }
        }
      }
      else
      {
        for( int index = 0; index < 10; index++ )
        {
          table.Add( new HighScoreEntry("Nobody",0));
        }
        Save( path );
      }   
      isLoaded = true;
    }
  
    public void Save( string path )
    {
      if( File.Exists( path ) )
      {
        File.Delete( path );
      }
  
      using( var textStream = new StreamWriter(path,false, Encoding.UTF8) )
      {
        foreach (var highScoreEntry in table)
        {
          textStream.WriteLine($"{highScoreEntry.Name},{highScoreEntry.Score}");
        }
      }
    }
  
    public int GetIndexOfScore( int score )
    {
      for (int index = 0; index < table.Count; index++)
      {
        var highScoreEntry = table[index];
        if (score > highScoreEntry.Score && index < 10)
        {
          return index;
        }
      }
      return -1;
    }
  
    public void Update( string name, int score )
    {
      if (!isLoaded) Load(Application.StartupPath + @"\score.txt");

      int index = GetIndexOfScore(score);

      if (index > -1)
      {
        if (table.Count > 9) table.RemoveAt(9);
        table.Insert(index, new HighScoreEntry(name, score));
        Save(Application.StartupPath + @"\score.txt");
      }
    }
  
    public void Populate( ListView listView )
    {
      listView.Items.Clear();

      foreach (var highScoreEntry in table)
      {
        listView.Items.Add(new ListViewItem(new[] { highScoreEntry.Name, highScoreEntry.Score.ToString() }));
      }
    }
  }
}
