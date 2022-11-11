using System;

namespace Publicjoe.Windows
{
  /// <summary>
  /// Description of HighScoreEntry.
  /// </summary>
  public class HighScoreEntry
  {
    public string name;
    public int score;
    
    public HighScoreEntry( string name, int score )
    {
      this.name = name;
      this.score = score;
    }
  }
}
