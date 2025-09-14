using Publicjoe.Windows;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Asteroids
{
  public partial class Form1 : Form
  {
    private HighScoreTable highScoreTable = new HighScoreTable();

    public Form1()
    {
      InitializeComponent();

      // Load high score table...
      highScoreTable.Load(Application.StartupPath + @"\score.txt");

      init();

      Start();
    }

    private void highScoresToolStripMenuItem_Click(object sender, EventArgs e)
    {
      HighScoreForm HighScoreForm = new HighScoreForm(highScoreTable);
      HighScoreForm.StartPosition = FormStartPosition.CenterScreen;
      HighScoreForm.Show();
    }

    private void rulesToolStripMenuItem_Click(object sender, EventArgs e)
    {
      (new RulesForm()).ShowDialog();
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Close();
    }

    public static Random random = new Random();

    //
    // Thread control variables.
    //
    private static bool CREATE_THREAD = false;
    Thread loopThread;
    bool gameLoop = !CREATE_THREAD;
    float T0 = 0;
    float T = 0;

    /**************************** Constants ****************************/

    /// <summary>Milliseconds between screen updates.</summary>
    public const int DELAY = 20; //10

    /// <summary>Starting number of ships per game.</summary>
    public const int MAX_SHIPS = 5;

    /// <summary>Maximum number of sprites for photons, asteroids, and explosions.</summary>
    public const int MAX_SHOTS = 8;
    public const int MAX_ROCKS = 8;
    public const int MAX_SCRAP = 20;

    /// <summary>Counter starting values.</summary>
    public const int SCRAP_COUNT = 30;
    public const int HYPER_COUNT = 60;
    public const int STORM_PAUSE = 30;
    public const int UFO_PASSES = 3;

    /// <summary>Asteroid Shape and size ranges.</summary>
    public const int MIN_ROCK_SIDES = 8;
    public const int MAX_ROCK_SIDES = 12;
    public const int MIN_ROCK_SIZE = 20;
    public const int MAX_ROCK_SIZE = 40;
    public const int MIN_ROCK_SPEED = 2;
    public const int MAX_ROCK_SPEED = 12;

    /// <summary>Points for shooting different objects.</summary>
    public const int BIG_POINTS = 25;
    public const int SMALL_POINTS = 50;
    public const int UFO_POINTS = 250;
    public const int MISSLE_POINTS = 500;

    /// <summary>Number of points needed to earn a new ship.</summary>
    public const int NEW_SHIP_POINTS = 5000;

    /// <summary>Number of points between flying saucers.</summary>
    public const int NEW_UFO_POINTS = 2750;

    /// <summary>Background stars.</summary>
    Point[] stars;
    int numStars;

    /**************************** Game data ****************************/

    int score;
    int highScore;
    int newShipScore;
    int newUfoScore;

    bool loaded = false;
    bool paused = false;
    bool playing = false;
    bool sound = false;
    bool detail = false;

    //
    // Key flags.
    //
    bool left = false;
    bool right = false;
    bool up = false;
    bool down = false;

    //
    // Sprite objects.
    //
    Sprite ship;
    Sprite ufo;
    Sprite missle;
    Sprite[] photons = new Sprite[MAX_SHOTS];
    Sprite[] asteroids = new Sprite[MAX_ROCKS];
    Sprite[] explosions = new Sprite[MAX_SCRAP];

    /**************************** Ship data ****************************/

    /// <summary>Number of ships left to play, including current one.</summary>
    int shipsLeft;

    /// <summary>Time counter for ship explosion.</summary>
    int shipCounter;

    /// <summary>Time counter for hyperspace.</summary>
    int hyperCounter;

    /**************************** Photon data ****************************/

    /// <summary>Time counter for life of a photon.</summary>
    int[] photonCounter = new int[MAX_SHOTS];

    /// <summary>Next available photon TransformedPath.</summary>
    int photonIndex;

    /**************************** Flying saucer data ****************************/

    /// <summary>Number of flying saucer passes.</summary>
    int ufoPassesLeft;

    /// <summary>Time counter for each pass.</summary>
    int ufoCounter;

    /**************************** Missile data ****************************/

    /// <summary>Counter for life of missle.</summary>
    int missleCounter;

    /**************************** Asteroid data ****************************/

    /// <summary>Asteroid size flag.</summary>
    bool[] asteroidIsSmall = new bool[MAX_ROCKS];

    /// <summary>Break-time counter.</summary>
    int asteroidsCounter;

    /// <summary>Asteroid speed.</summary>
    int asteroidsSpeed;

    /// <summary>Number of Active asteroids.</summary>
    int asteroidsLeft;

    /**************************** Explosion data ****************************/

    /// <summary>Time counters for explosions.</summary>
    int[] explosionCounter = new int[MAX_SCRAP];

    /// <summary>Next available explosion TransformedPath.</summary>
    int explosionIndex;

    //
    // Values for the offscreen image.
    //

    Size offDimension;
    Image offImage;
    public static Graphics offGraphics;

    // Font data.
    Font font = new Font("Helvetica", 12);

    // Applet information.
    public static string[] Info = new string[]
    {
      "Original concept, 1998 by Mike Hall",
      "C# Asteroids, 2000 by Michael Lambert",
      "Updated:, 2006 by Publicjoe"
    };

    // Applet information.
    public static string[] Help = new string[]
    {
      "S\t\t- Start Game\n",
      "P\t\t- Pause Game\n",
      "Cursor Up\t- Fire Thrusters\n",
      "Cursor Down\t- Fire Retro Thrusters\n",
      "Cursor Left\t- Rotate Left\n",
      "Cursor Right\t- Rotate Right\n",
      "Spacebar\t- Fire Cannon\n",
      "H\t\t- Hyperspace\n",
      "M\t\t- Toggle Sound\n",
      "D\t\t- Toggle Graphics Detail\n"
    };

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
      Stop();
    }

    public void init()
    {
      int i;

      // Take credit.
      foreach (string s in Info)
        System.Console.WriteLine(s);

      // Find the size of the screen and set the values for sprites.
      Graphics g = this.CreateGraphics();
      Size d = this.Size;
      Sprite.Screen.Width = this.ClientRectangle.Width;
      Sprite.Screen.Height = this.ClientRectangle.Width;

      // Generate starry background.
      GenerateStarfield();

      // Create Shape for the ship TransformedPath.
      ship = new Sprite();
      PointF[] ship_pts = 
      {
        new PointF(0, -10),
        new PointF(7, 10),
        new PointF(-7, 10)
      };
      ship.Shape.AddPolygon(ship_pts);
      ship.Shape.CloseFigure();

      // Create Shape for the photon sprites.
      for (i = 0; i < MAX_SHOTS; i++)
      {
        photons[i] = new Sprite();
        PointF[] photon_pts = 
        {
          new PointF(1, 1),
          new PointF(1, -1),
          new PointF(-1, 1),
          new PointF(-1, -1)
          
        };
        photons[i].Shape.AddPolygon(photon_pts);
        photons[i].Shape.CloseFigure();
      }

      // Create Shape for the flying saucer.
      ufo = new Sprite();
      PointF[] ufo_pts = 
      {
        new PointF(-15, 0),
        new PointF(-10, -5),
        new PointF(-5, -5),
        new PointF(-5, -9),
        new PointF(5, -9),
        new PointF(5, -5),
        new PointF(10, -5),
        new PointF(15, 0),
        new PointF(10, 5),
        new PointF(-10, 5)
      };
      ufo.Shape.AddPolygon(ufo_pts);
      ufo.Shape.CloseFigure();

      // Create Shape for the guided missle.
      missle = new Sprite();
      PointF[] missle_pts = 
      {
        new PointF(0, -4),
        new PointF(1, -3),
        new PointF(1, 3),
        new PointF(2, 4),
        new PointF(-2, 4),
        new PointF(-1, 3),
        new PointF(-1, -3)
      };

      missle.Shape.AddPolygon(missle_pts);
      missle.Shape.CloseFigure();

      // Create asteroid sprites.
      for (i = 0; i < MAX_ROCKS; i++)
        asteroids[i] = new Sprite();

      // Create explosion sprites.
      for (i = 0; i < MAX_SCRAP; i++)
        explosions[i] = new Sprite();

      // Initialize game data and put us in 'game over' mode.
      highScore = 0;
      sound = true;
      detail = true;
      initGame();
      endGame();
      g.Dispose();
    }

    private void GenerateStarfield()
    {
      numStars = Sprite.Screen.Width * Sprite.Screen.Height / 5000;
      stars = new Point[numStars];
      for (int i = 0; i < numStars; i++)
        stars[i] = new Point((int)(random.NextDouble() * Sprite.Screen.Width), (int)(random.NextDouble() * Sprite.Screen.Height));
    }

    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
      // Eliminating Flicker
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      Size d = this.Size;
      int i;
      int c;
      String s;
      SizeF stringSize;
      Pen penRGB = new Pen(Color.White, 1);

      // Create the offscreen graphics context, if no good one exists.
      if (offGraphics == null || d.Width != offDimension.Width || d.Height != offDimension.Height)
      {
        offDimension = d;
        offImage = new Bitmap(offDimension.Width, offDimension.Height, e.Graphics);
        offGraphics = Graphics.FromImage(offImage);
      }

      lock (offGraphics)
      {
        // Fill in background and stars.
        offGraphics.FillRectangle(Brushes.Black, offGraphics.ClipBounds);
        if (detail)
        {
          lock (stars)
          {
            foreach (Point pt in stars)
              offGraphics.DrawEllipse(Pens.White, pt.X, pt.Y, 1, 1);
          }
        }

        // Draw photon bullets.
        lock (photons)
        {
          for (i = 0; i < MAX_SHOTS; i++)
            if (photons[i].Active)
              offGraphics.DrawPath(Pens.White, photons[i].TransformedPath);
        }

        // Draw the guided missle, counter is used to quickly fade color to black when near expiration.
        c = Math.Min(missleCounter * 24, 255);
        penRGB.Color = Color.FromArgb(c, c, c);
        lock (missle)
        {
          if (missle.Active)
            offGraphics.DrawPath(penRGB, missle.TransformedPath);
        }

        // Draw the asteroids.
        lock (asteroids)
        {
          foreach (Sprite asteroid in asteroids)
          {
            if (asteroid.Active)
            {
              if (detail)
                offGraphics.FillPath(Brushes.Black, asteroid.TransformedPath);

              offGraphics.DrawPath(Pens.White, asteroid.TransformedPath);
            }
          }
        }

        // Draw the flying saucer.
        if (ufo.Active)
        {
          if (detail)
            offGraphics.DrawPath(Pens.Black, ufo.TransformedPath);

          offGraphics.DrawPath(Pens.White, ufo.TransformedPath);
        }

        // Draw the ship, counter is used to fade color to white on hyperspace.
        c = 255 - (255 / HYPER_COUNT) * hyperCounter;
        lock (ship)
        {
          if (ship.Active)
          {
            if (detail && hyperCounter == 0)
              offGraphics.DrawPath(Pens.Black, ship.TransformedPath);

            penRGB.Color = Color.FromArgb(c, c, c);
            offGraphics.DrawPath(penRGB, ship.TransformedPath);
          }
        }

        // Draw any explosion debris, counters are used to fade color to black.
        lock (explosions)
        {
          for (i = 0; i < MAX_SCRAP; i++)
          {
            if (explosions[i].Active)
            {
              c = (255 / SCRAP_COUNT) * explosionCounter[i];

              penRGB.Color = Color.FromArgb(c, c, c);
              offGraphics.DrawPath(penRGB, explosions[i].TransformedPath);
            }
          }
        }

        // Display status and messages.
        SizeF fontSize = offGraphics.MeasureString("W", font, offDimension.Width);
        offGraphics.DrawString("Score: " + score, font, Brushes.White, fontSize.Width, font.Height + 20 );
        offGraphics.DrawString("Ships: " + shipsLeft, font, Brushes.White, fontSize.Width, (offDimension.Height - font.Height * 5) + 20 );

        s = "High: " + highScore;
        stringSize = offGraphics.MeasureString(s, font, offDimension.Width);
        offGraphics.DrawString(s, font, Brushes.White, offDimension.Width - (fontSize.Width + stringSize.Width), font.Height + 20);

        if (!sound)
        {
          s = "Mute";
          stringSize = offGraphics.MeasureString(s, font, offDimension.Width);
          offGraphics.DrawString(s, font, Brushes.White,
            offDimension.Width - (fontSize.Width + stringSize.Width),
            offDimension.Height - font.Height);
        }

        if (!playing)
        {
          s = "A S T E R O I D S";
          stringSize = offGraphics.MeasureString(s, font, offDimension.Width);
          offGraphics.DrawString(s, font, Brushes.White, (offDimension.Width - stringSize.Width) / 2, offDimension.Height / 2);

          //for (int info = 0; info < Info.Length; info++)
          //{
          //  stringSize = offGraphics.MeasureString(Info[info], font, offDimension.Width);
          //  offGraphics.DrawString(Info[info], font, Brushes.White, (offDimension.Width - stringSize.Width) / 2, offDimension.Height / 2 + font.Height * (info + 1));
          //}

          if (!loaded)
          {
            s = "Loading sounds...";
            stringSize = offGraphics.MeasureString(s, font, offDimension.Width);
            offGraphics.DrawString(s, font, Brushes.White, (offDimension.Width - stringSize.Width) / 2, offDimension.Height / 4);
          }
          else
          {
            s = "Game Over";
            stringSize = offGraphics.MeasureString(s, font, offDimension.Width);
            offGraphics.DrawString(s, font, Brushes.White, (offDimension.Width - stringSize.Width) / 2, offDimension.Height / 4);
            s = "'S' to Start\n'P' to Pause";
            stringSize = offGraphics.MeasureString(s, font, offDimension.Width);
            offGraphics.DrawString(s, font, Brushes.White, (offDimension.Width - stringSize.Width) / 2, offDimension.Height / 4 + font.Height);
          }
        }
        else if (paused)
        {
          s = "Game Paused";
          stringSize = offGraphics.MeasureString(s, font, offDimension.Width);
          offGraphics.DrawString(s, font, Brushes.White, (offDimension.Width - stringSize.Width) / 2, offDimension.Height / 4);
          s = "Keyboard Controls";
          stringSize = offGraphics.MeasureString(s, font, offDimension.Width);
          offGraphics.DrawString(s, font, Brushes.White, (offDimension.Width - stringSize.Width) / 2, offDimension.Height / 3);

          for (int info = 0; info < Help.Length; info++)
          {
            stringSize = offGraphics.MeasureString(Help[info], font, offDimension.Width);
            offGraphics.DrawString(Help[info], font, Brushes.White, offDimension.Width / 4, offDimension.Height / 3 + font.Height * (info + 2));
          }

        }

        // Copy the off screen buffer to the screen.
        e.Graphics.DrawImage(offImage, 0, 0);

        penRGB.Dispose();
      }
    }

    public void initGame()
    {
      // Initialize game data and sprites.
      score = 0;
      shipsLeft = MAX_SHIPS;
      asteroidsSpeed = MIN_ROCK_SPEED;
      newShipScore = NEW_SHIP_POINTS;
      newUfoScore = NEW_UFO_POINTS;

      initShip();
      initPhotons();
      stopUfo();
      stopMissle();
      initAsteroids();
      initExplosions();

      playing = true;
      paused = false;
    }

    public void endGame()
    {
      // Stop ship, flying saucer, guided missle and associated sounds.
      playing = false;

      stopShip();
      stopUfo();
      stopMissle();
      CheckHighScore();
    }

    public void initAsteroids()
    {
      int i, j;
      int s;
      double theta, r;

      // Create random shapes, positions and movements for each asteroid.

      for (i = 0; i < MAX_ROCKS; i++)
      {
        // Create a jagged Shape for the asteroid and give it a random rotation.

        s = MIN_ROCK_SIDES + (int)(random.NextDouble() * (MAX_ROCK_SIDES - MIN_ROCK_SIDES));
        //s = random.Next(MIN_ROCK_SIDES, MAX_ROCK_SIDES);
        Point[] pts = new Point[s];
        for (j = 0; j < s; j++)
        {
          theta = 2 * Math.PI / s * j;
          r = (MIN_ROCK_SIZE + (int)(random.NextDouble() * (MAX_ROCK_SIZE - MIN_ROCK_SIZE))) / 2;
          //r = random.Next(MIN_ROCK_SIDES, MAX_ROCK_SIDES)/2;
          pts[j].X = (int)-Math.Round(r * Math.Sin(theta));
          pts[j].Y = (int)Math.Round(r * Math.Cos(theta));
        }

        asteroids[i].Shape.Reset();// = new GraphicsPath();
        //asteroids[i].Shape.addPoint(x, y);
        asteroids[i].Shape.AddPolygon(pts);
        asteroids[i].Active = true;
        asteroids[i].RotationAngle = 0.0;
        asteroids[i].DeltaAngle = (random.NextDouble() - 0.5) / 10;

        // Place the asteroid at one edge of the screen.

        if (random.NextDouble() < 0.5)
        {
          asteroids[i].CurrentX = -Sprite.Screen.Width / 2;
          if (random.NextDouble() < 0.5)
            asteroids[i].CurrentX = Sprite.Screen.Width / 2;
          //asteroids[i].CurrentY = random.Next() * Sprite.Screen.Height;
          asteroids[i].CurrentY = random.Next(Sprite.Screen.Height);
        }
        else
        {
          //asteroids[i].CurrentX = random.Next() * Sprite.Screen.Width;
          asteroids[i].CurrentX = random.Next(Sprite.Screen.Width);
          asteroids[i].CurrentY = -Sprite.Screen.Height / 2;
          if (random.Next() < 0.5)
            asteroids[i].CurrentY = Sprite.Screen.Height / 2;
        }

        // Set a random motion for the asteroid.

        // asteroids[i].DeltaX = random.Next() * asteroidsSpeed;
        asteroids[i].DeltaX = random.Next(1, asteroidsSpeed);
        if (random.NextDouble() < 0.5)
          asteroids[i].DeltaX = -asteroids[i].DeltaX;
        // asteroids[i].DeltaY = random.Next() * asteroidsSpeed;
        asteroids[i].DeltaY = random.Next(1, asteroidsSpeed);
        if (random.NextDouble() < 0.5)
          asteroids[i].DeltaY = -asteroids[i].DeltaY;

        asteroids[i].render();
        asteroidIsSmall[i] = false;
      }

      asteroidsCounter = STORM_PAUSE;
      asteroidsLeft = MAX_ROCKS;
      if (asteroidsSpeed < MAX_ROCK_SPEED)
        asteroidsSpeed++;
    }

    public void updateAsteroids()
    {
      int i, j;

      // Move any Active asteroids and check for collisions.

      for (i = 0; i < MAX_ROCKS; i++)
      {
        if (asteroids[i].Active)
        {
          asteroids[i].Advance();
          asteroids[i].render();

          // If hit by photon, kill asteroid and Advance score. If asteroid is large,
          // make some smaller ones to replace it.

          for (j = 0; j < MAX_SHOTS; j++)
            if (photons[j].Active && asteroids[i].Active && asteroids[i].isColliding(photons[j]))
            {
              asteroidsLeft--;
              asteroids[i].Active = false;
              photons[j].Active = false;
              explode(asteroids[i]);
              if (!asteroidIsSmall[i])
              {
                score += BIG_POINTS;
                initSmallAsteroids(i);
              }
              else
                score += SMALL_POINTS;
            }

          // If the ship is not in hyperspace, see if it is hit.

          if (ship.Active && hyperCounter <= 0 && asteroids[i].Active && asteroids[i].isColliding(ship))
          {
            explode(ship);
            stopShip();
            stopUfo();
            stopMissle();
          }

        }
      }
    }

    public void initExplosions()
    {
      for (int i = 0; i < MAX_SCRAP; i++)
      {
        //explosions[i].Shape = new GraphicsPath();
        explosions[i].Shape.Reset();
        //explosions[i].Shape = new Polygon();
        explosions[i].Active = false;
        explosionCounter[i] = 0;
      }
      explosionIndex = 0;
    }

    public void explode(Sprite s)
    {
      int c, i, j, k;

      // Create sprites for explosion animation. The each individual line segment of the given TransformedPath
      // is used to create a new TransformedPath that will move outward  from the TransformedPath's original position
      // with a random rotation.

      s.render();
      c = 2;
      if (detail || s.TransformedPath.PathPoints.Length < 6)
        c = 1;

      for (i = 0; i < s.TransformedPath.PathPoints.Length; i += c)
      {
        explosionIndex++;
        if (explosionIndex >= MAX_SCRAP)
          explosionIndex = 0;
        explosions[explosionIndex].Active = true;

        j = i + 1;
        if (j >= s.TransformedPath.PathPoints.Length)
          j -= s.TransformedPath.PathPoints.Length;
        k = j + 1;
        if (k >= s.TransformedPath.PathPoints.Length)
          k -= s.TransformedPath.PathPoints.Length;

        PointF[] points =
        {
          new PointF( s.Shape.PathPoints[i].X, s.Shape.PathPoints[i].Y ),
          new PointF( s.Shape.PathPoints[j].X, s.Shape.PathPoints[j].Y ),
          new PointF( s.Shape.PathPoints[k].X, s.Shape.PathPoints[k].Y )
        };

        //explosions[explosionIndex].Shape = new GraphicsPath();
        explosions[explosionIndex].Shape.Reset();
        explosions[explosionIndex].Shape.AddPolygon(points);
        explosions[explosionIndex].RotationAngle = s.RotationAngle;
        explosions[explosionIndex].DeltaAngle = (random.NextDouble() * 2 * Math.PI - Math.PI) / 15;
        explosions[explosionIndex].CurrentX = s.CurrentX;
        explosions[explosionIndex].CurrentY = s.CurrentY;

        explosions[explosionIndex].DeltaX = -s.Shape.PathPoints[i].X / 5;
        explosions[explosionIndex].DeltaY = -s.Shape.PathPoints[i].Y / 5;
        //explosions[explosionIndex].DeltaX = -s.Shape.xpoints[i] / 5;
        //explosions[explosionIndex].DeltaY = -s.Shape.ypoints[i] / 5;
        explosionCounter[explosionIndex] = SCRAP_COUNT;
      }
    }

    public void updateExplosions()
    {
      // Move any Active explosion debris. Stop explosion when its counter has expired.
      for (int i = 0; i < MAX_SCRAP; i++)
      {
        if (explosions[i].Active)
        {
          explosions[i].Advance();
          explosions[i].render();
          if (--explosionCounter[i] < 0)
            explosions[i].Active = false;
        }
      }
    }

    public void initShip()
    {
      ship.Active = true;
      ship.RotationAngle = 0.0;
      ship.DeltaAngle = 0.0;
      ship.CurrentX = 0.0;
      ship.CurrentY = 0.0;
      ship.DeltaX = 0.0;
      ship.DeltaY = 0.0;
      ship.render();
      hyperCounter = 0;
    }

    public void updateShip()
    {
      double dx, dy, limit;

      if (!playing)
        return;

      // Rotate the ship if left or right cursor key is down.

      if (right)
      {
        ship.RotationAngle += Math.PI / 32.0;
        if (ship.RotationAngle > 2 * Math.PI)
          ship.RotationAngle -= 2 * Math.PI;
      }
      if (left)
      {
        ship.RotationAngle -= Math.PI / 32.0;
        if (ship.RotationAngle < 0)
          ship.RotationAngle += 2 * Math.PI;
      }

      // Fire thrusters if up or down cursor key is down. Don't let ship go past
      // the speed limit.

      dx = Math.Sin(ship.RotationAngle);
      dy = Math.Cos(ship.RotationAngle);
      limit = 0.8 * MIN_ROCK_SIZE;
      if (up)
      {
        if (ship.DeltaX + dx > -limit && ship.DeltaX + dx < limit)
          ship.DeltaX += dx;
        if (ship.DeltaY + dy > -limit && ship.DeltaY + dy < limit)
          ship.DeltaY += dy;
      }
      if (down)
      {
        if (ship.DeltaX - dx > -limit && ship.DeltaX - dx < limit)
          ship.DeltaX -= dx;
        if (ship.DeltaY - dy > -limit && ship.DeltaY - dy < limit)
          ship.DeltaY -= dy;
      }

      // Move the ship. If it is currently in hyperspace, Advance the countdown.

      if (ship.Active)
      {
        ship.Advance();
        ship.render();
        if (hyperCounter > 0)
          hyperCounter--;
      }

      // Ship is exploding, Advance the countdown or create a new ship if it is
      // done exploding. The new ship is added as though it were in hyperspace.
      // (This gives the player time to move the ship if it is in imminent danger.)
      // If that was the last ship, end the game.

      else
      {
        if (--shipCounter <= 0)
        {
          if (shipsLeft > 0)
          {
            initShip();
            hyperCounter = HYPER_COUNT;
          }
          else
            endGame();
        }
      }
    }

    public void stopShip()
    {
      ship.Active = false;
      shipCounter = SCRAP_COUNT;
      if (shipsLeft > 0)
        shipsLeft--;
    }

    public void initPhotons()
    {
      int i;

      for (i = 0; i < MAX_SHOTS; i++)
      {
        photons[i].Active = false;
        photonCounter[i] = 0;
      }
      photonIndex = 0;
    }

    public void updatePhotons()
    {
      int i;

      // Move any Active photons. Stop it when its counter has expired.

      for (i = 0; i < MAX_SHOTS; i++)
      {
        if (photons[i].Active)
        {
          photons[i].Advance();
          photons[i].render();
          if (--photonCounter[i] < 0)
            photons[i].Active = false;
        }
      }
    }

    public void initUfo()
    {
      // Randomly set flying saucer at left or right edge of the screen.
      ufo.Active = true;
      ufo.CurrentX = -Sprite.Screen.Width / 2;
      ufo.CurrentY = random.NextDouble() * Sprite.Screen.Height;
      ufo.DeltaX = MIN_ROCK_SPEED + random.NextDouble() * (MAX_ROCK_SPEED - MIN_ROCK_SPEED);
      if (random.NextDouble() < 0.5)
      {
        ufo.DeltaX = -ufo.DeltaX;
        ufo.CurrentX = Sprite.Screen.Width / 2;
      }
      ufo.DeltaY = MIN_ROCK_SPEED + random.NextDouble() * (MAX_ROCK_SPEED - MIN_ROCK_SPEED);
      if (random.NextDouble() < 0.5)
        ufo.DeltaY = -ufo.DeltaY;
      ufo.render();

      // Set counter for this pass.

      ufoCounter = (int)Math.Floor(Sprite.Screen.Width / Math.Abs(ufo.DeltaX));
    }

    public void updateUfo()
    {
      int i, d;

      // Move the flying saucer and check for collision with a photon. Stop it when its
      // counter has expired.

      if (ufo.Active)
      {
        ufo.Advance();
        ufo.render();
        if (--ufoCounter <= 0)
          if (--ufoPassesLeft > 0)
            initUfo();
          else
            stopUfo();
        else
        {
          for (i = 0; i < MAX_SHOTS; i++)
            if (photons[i].Active && ufo.isColliding(photons[i]))
            {
              explode(ufo);
              stopUfo();
              score += UFO_POINTS;
            }

          // On occassion, fire a missle at the ship if the saucer is not
          // too close to it.

          d = (int)Math.Max(Math.Abs(ufo.CurrentX - ship.CurrentX), Math.Abs(ufo.CurrentY - ship.CurrentY));
          if (ship.Active && hyperCounter <= 0 && ufo.Active && !missle.Active &&
            d > 4 * MAX_ROCK_SIZE && random.NextDouble() < .03)
            initMissle();
        }
      }
    }

    public void stopUfo()
    {
      ufo.Active = false;
      ufoCounter = 0;
      ufoPassesLeft = 0;
    }

    public void initMissle()
    {
      missle.Active = true;
      missle.RotationAngle = 0.0;
      missle.DeltaAngle = 0.0;
      missle.CurrentX = ufo.CurrentX;
      missle.CurrentY = ufo.CurrentY;
      missle.DeltaX = 0.0;
      missle.DeltaY = 0.0;
      missle.render();
      missleCounter = 3 * Math.Max(Sprite.Screen.Width, Sprite.Screen.Height) / MIN_ROCK_SIZE;
    }

    public void updateMissle()
    {
      int i;

      // Move the guided missle and check for collision with ship or photon. Stop it when its
      // counter has expired.

      if (missle.Active)
      {
        if (--missleCounter <= 0)
          stopMissle();
        else
        {
          guideMissle();
          missle.Advance();
          missle.render();
          for (i = 0; i < MAX_SHOTS; i++)
            if (photons[i].Active && missle.isColliding(photons[i]))
            {
              explode(missle);
              stopMissle();
              score += MISSLE_POINTS;
            }
          if (missle.Active && ship.Active && hyperCounter <= 0 && ship.isColliding(missle))
          {
            explode(ship);
            stopShip();
            stopUfo();
            stopMissle();
          }
        }
      }
    }

    public void guideMissle()
    {
      double dx, dy, angle;

      if (!ship.Active || hyperCounter > 0)
        return;

      // Find the RotationAngle needed to hit the ship.

      dx = ship.CurrentX - missle.CurrentX;
      dy = ship.CurrentY - missle.CurrentY;
      if (dx == 0 && dy == 0)
        angle = 0;
      if (dx == 0)
      {
        if (dy < 0)
          angle = -Math.PI / 2;
        else
          angle = Math.PI / 2;
      }
      else
      {
        angle = Math.Atan(Math.Abs(dy / dx));
        if (dy > 0)
          angle = -angle;
        if (dx < 0)
          angle = Math.PI - angle;
      }

      // Adjust RotationAngle for screen coordinates.

      missle.RotationAngle = angle - Math.PI / 2;

      // Change the missle's RotationAngle so that it points toward the ship.

      missle.DeltaX = MIN_ROCK_SIZE / 3 * Math.Sin(missle.RotationAngle);
      missle.DeltaY = MIN_ROCK_SIZE / 3 * Math.Cos(missle.RotationAngle);
    }

    public void stopMissle()
    {
      missle.Active = false;
      missleCounter = 0;
    }

    public void initSmallAsteroids(int n)
    {
      int count;
      int i, j;
      int s;
      double tempX, tempY;
      double theta, r;

      // Create one or two smaller asteroids from a larger one using inactive asteroids. The new
      // asteroids will be placed in the same position as the old one but will have a new, smaller
      // Shape and new, randomly generated movements.

      count = 0;
      i = 0;
      tempX = asteroids[n].CurrentX;
      tempY = asteroids[n].CurrentY;
      do
      {
        if (!asteroids[i].Active)
        {
          s = MIN_ROCK_SIDES + (int)(random.NextDouble() * (MAX_ROCK_SIDES - MIN_ROCK_SIDES));
          //s = random.Next(MIN_ROCK_SIDES, MAX_ROCK_SIDES);
          PointF[] points = new PointF[s];
          for (j = 0; j < s; j++)
          {
            theta = 2 * Math.PI / s * j;
            r = (MIN_ROCK_SIZE + (int)(random.NextDouble() * (MAX_ROCK_SIZE - MIN_ROCK_SIZE))) / 4;
            //r = random.Next(MIN_ROCK_SIZE, MAX_ROCK_SIZE)/4;
            points[j].X = (int)-Math.Round(r * Math.Sin(theta));
            points[j].Y = (int)Math.Round(r * Math.Cos(theta));
            //asteroids[i].Shape.addPoint(x, y);
          }
          //asteroids[i].Shape = new GraphicsPath();
          asteroids[i].Shape.Reset();
          asteroids[i].Shape.AddPolygon(points);
          asteroids[i].Active = true;
          asteroids[i].RotationAngle = 0.0;
          asteroids[i].DeltaAngle = (random.NextDouble() - 0.5) / 10;
          asteroids[i].CurrentX = tempX;
          asteroids[i].CurrentY = tempY;
          asteroids[i].DeltaX = random.NextDouble() * 2 * asteroidsSpeed - asteroidsSpeed;
          asteroids[i].DeltaY = random.NextDouble() * 2 * asteroidsSpeed - asteroidsSpeed;
          asteroids[i].render();
          asteroidIsSmall[i] = true;
          count++;
          asteroidsLeft++;
        }
        i++;
      } while (i < MAX_ROCKS && count < 2);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      // public bool keyDown(Event e, int key) {

      // Check if any cursor keys have been pressed and set flags.

      if (e.KeyCode == Keys.Left)
        left = true;
      if (e.KeyCode == Keys.Right)
        right = true;
      if (e.KeyCode == Keys.Up)
        up = true;
      if (e.KeyCode == Keys.Down)
        down = true;

      // Spacebar: fire a photon and start its counter.

      if (e.KeyCode == Keys.Space && ship.Active)
      {
        photonIndex++;
        if (photonIndex >= MAX_SHOTS)
          photonIndex = 0;
        photons[photonIndex].Active = true;
        photons[photonIndex].CurrentX = ship.CurrentX;
        photons[photonIndex].CurrentY = ship.CurrentY;

        photons[photonIndex].DeltaX = MIN_ROCK_SIZE * Math.Sin(ship.RotationAngle);
        photons[photonIndex].DeltaY = MIN_ROCK_SIZE * Math.Cos(ship.RotationAngle);
        photonCounter[photonIndex] = Math.Min(Sprite.Screen.Width, Sprite.Screen.Height) / MIN_ROCK_SIZE;
      }

      // 'H' key: warp ship into hyperspace by moving to a random _location and starting counter.

      if (e.KeyCode == Keys.H && ship.Active && hyperCounter <= 0)
      {
        ship.CurrentX = random.NextDouble() * Sprite.Screen.Width;
        ship.CurrentX = random.NextDouble() * Sprite.Screen.Height;
        hyperCounter = HYPER_COUNT;
      }

      // 'P' key: toggle pause mode and start or stop any Active looping sound clips.

      if (e.KeyCode == Keys.P)
      {
        paused = !paused;
      }

      // 'M' key: toggle sound on or off and stop any looping sound clips.

      if (e.KeyCode == Keys.M && loaded)
      {
        sound = !sound;
      }

      // 'D' key: toggle graphics detail on or off.

      if (e.KeyCode == Keys.D)
        detail = !detail;

      // 'S' key: start the game, if not already in progress.

      if (e.KeyCode == Keys.S && loaded && !playing)
        initGame();

      base.OnKeyDown(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
      // Check if any cursor keys where released and set flags.
      if (e.KeyCode == Keys.Left)
        left = false;
      if (e.KeyCode == Keys.Right)
        right = false;
      if (e.KeyCode == Keys.Up)
        up = false;
      if (e.KeyCode == Keys.Down)
        down = false;

      base.OnKeyUp(e);
    }

    protected override void OnResize(EventArgs e)
    {
      paused = true;
      Sprite.Screen.Width = this.Size.Width;
      Sprite.Screen.Height = this.Size.Height;
      GenerateStarfield();
      base.OnResize(e);
    }

    public void Start()
    {
      if (CREATE_THREAD)
      {
        if (loopThread == null)
        {
          loopThread = new Thread(new ThreadStart(this.Run));
          loopThread.Start();
        }
      }
      else
      {
        gameLoop = true;
        ThreadPool.QueueUserWorkItem(new WaitCallback(this.WorkItemSleep), null);
      }
    }

    public void Stop()
    {
      if (CREATE_THREAD)
      {
        loopThread.Abort();

        loopThread.Join();

        loopThread = null;
      }
      else
        gameLoop = false;
    }

    /// <summary>
    /// This is the main loop. 
    /// </summary>
    private void Run()
    {
      try
      {
        loaded = true;

        while (true)
        {
          try
          {

            T0 = Environment.TickCount;

            if (!paused)
            {
              ProcessSprites();
              ProcessStats();
            }

            //
            // Update the screen and go to sleep until the next loop.
            //
            Invalidate(Region);

            Thread.Sleep(DELAY);

            T = Environment.TickCount - T0;
          }
          catch (ThreadInterruptedException)
          {
            break;
          }
        }
      }
      catch (Exception e)
      {
        Debug.Write(e.ToString());
      }
    }

    private void WorkItemSleep(object state)
    {
      loaded = true;

      T0 = Environment.TickCount;

      Thread.Sleep(DELAY);

      ThreadPool.QueueUserWorkItem(new WaitCallback(this.WorkItemCalc), null);
    }

    private void WorkItemCalc(object state)
    {
      if (!paused)
      {
        ProcessSprites();
        ProcessStats();
      }

      ThreadPool.QueueUserWorkItem(new WaitCallback(this.WorkItemPaint), null);
    }

    private void WorkItemPaint(object state)
    {
      //
      // Update the screen and goto sleep until the next state.
      //
      Invalidate(Region);

      T = Environment.TickCount - T0;

      ThreadPool.QueueUserWorkItem(new WaitCallback(this.WorkItemSleep), null);
    }

    /// <summary>Move and process all sprites.</summary>
    private void ProcessSprites()
    {
      lock (ship)
        updateShip();

      lock (photons)
        updatePhotons();

      lock (ufo)
        updateUfo();

      lock (missle)
        updateMissle();

      lock (asteroids)
        updateAsteroids();

      lock (explosions)
        updateExplosions();
    }

    /// <summary>
    /// Check the score and Advance high score, add a new ship 
    /// or start the flying saucer as necessary.
    /// </summary>
    private void ProcessStats()
    {
      if (score > highScore)
        highScore = score;

      if (score > newShipScore)
      {
        newShipScore += NEW_SHIP_POINTS;
        shipsLeft++;
      }

      if (playing && score > newUfoScore && !ufo.Active)
      {
        newUfoScore += NEW_UFO_POINTS;
        ufoPassesLeft = UFO_PASSES;
        initUfo();
      }

      // If all asteroids have been destroyed create a new batch.
      if (asteroidsLeft <= 0)
        if (--asteroidsCounter <= 0)
          initAsteroids();
    }

    private void CheckHighScore()
    {
      highScoreTable.Load(Application.StartupPath + @"\score.txt");

      int scoreIndex = highScoreTable.GetIndexOfScore(highScore);

      if (scoreIndex > -1)
      {
        string name = "";
        using (EntryForm aForm = new EntryForm())
        {
          aForm.StartPosition = FormStartPosition.CenterScreen;

          if (aForm.ShowDialog() == DialogResult.OK)
          {
            name = aForm.textBox1.Text;

            highScoreTable.Update(name, highScore);
          }
        }
      }
    }
  }
}
