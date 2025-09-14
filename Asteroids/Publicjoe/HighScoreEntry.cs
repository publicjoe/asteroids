namespace Publicjoe.Windows
{
  /// <summary>
  /// Description of HighScoreEntry.
  /// </summary>
  public class HighScoreEntry
  {
    public string Name { get; set; }
    public int Score { get; set; }

    public HighScoreEntry( string name, int score )
    {
      Name = name;
      Score = score;
    }
  }
}
