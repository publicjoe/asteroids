using System;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace Publicjoe.Windows
{
  /// <summary>
  /// Description of HighScoreTable.
  /// </summary>
  public class HighScoreTable
  {
    private ArrayList table;

    private bool isLoaded;
  
    public HighScoreTable()
    {
      table = new ArrayList();
      isLoaded = false;
    }
  
    public void Load( string path )
    {
      if( File.Exists( path ) )
      {
        table = new ArrayList();
  
        using( StreamReader textStream = new StreamReader(path, Encoding.UTF8) )
        {
          string scoreLine;
  
          while( (scoreLine = textStream.ReadLine()) != null )
          {
            string[] scoreParts = scoreLine.Split(',');
  
            if( scoreParts.Length != 2 )
            {
              throw new ApplicationException("Score file corrupt!");
            }
            else
            {
              table.Add(new HighScoreEntry(scoreParts[0],Int32.Parse(scoreParts[1])));
            }
          }
          textStream.Close();
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
  
      using( StreamWriter textStream = new StreamWriter(path,false, Encoding.UTF8) )
      {
        foreach( object tableEntry in table )
        { 
          HighScoreEntry highScoreEntry = tableEntry as HighScoreEntry;
  
          textStream.WriteLine("{0},{1}",highScoreEntry.name,highScoreEntry.score );
        }
        textStream.Close();
      }
    }
  
    public int GetIndexOfScore( int score )
    {
      for( int index = 0; index < table.Count; index++ )
      {
        HighScoreEntry highScoreEntry = table[index] as HighScoreEntry;
  
        if( score > highScoreEntry.score && index < 10)
        {
          return index;
        }
      }
      return -1;
    }
  
    public void Update( string name, int score )
    {
      if( !isLoaded ) Load( Application.StartupPath+@"\score.txt" );
  
      int index = GetIndexOfScore( score );
  
      if( index > -1 )
      {
        if( table.Count > 9 ) table.RemoveAt( 9 );
        table.Insert( index, new HighScoreEntry( name, score ) );
        Save( Application.StartupPath+@"\score.txt" );
      }
    }
  
    public void Populate( ListView listView )
    {
      listView.Items.Clear();
  
      foreach( object entry in table )
      {
        HighScoreEntry highScoreEntry = entry as HighScoreEntry;
        listView.Items.Add( new ListViewItem( new string[] { highScoreEntry.name, highScoreEntry.score.ToString() } ));
      }
    }
  }
}
