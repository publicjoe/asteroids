/*
 * Downloaded from www.publicjoe.co.uk
 *
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the author(s) be held liable for any damages arising from
 * the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely.
 *  
 * This class was written by Michael Lambert, Copyright 2000. Which is Based 
 * on Asteroids, Copyright 1998 by Mike Hall. All Rights Reserved.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace Asteroids
{
  /// <summary>
  /// Defines a game object, including it's Shape, position, 
  /// movement and rotation. It also can detemine if two objects collide.
  /// </summary>
  public class Sprite
  {
    private static bool JVM_SUX = true;

    /// <summary>Dimensions of the graphics area.</summary>
    public static Size Screen;

    /// <summary>Initial TransformedPath Shape, centered at the origin (0,0).</summary>
    public GraphicsPath Shape = new GraphicsPath();

    /// <summary>Active flag.</summary>
    public bool Active { get; set; }

    /// <summary>Current RotationAngle of rotation.</summary>
    public double RotationAngle { get; set; }

    /// <summary>Amount to change the rotation RotationAngle.</summary>
    public double DeltaAngle { get; set; }

    /// <summary>Current position on screen.</summary>
    public double CurrentX { get; set; }
    public double CurrentY { get; set; }

    /// <summary>Amount to change the screen position.</summary>
    public double DeltaX { get; set; }
    public double DeltaY { get; set; }

    private Matrix _location = new Matrix();
    private Region _space = new Region();

    /// <summary>
    /// Final _location and _shape of TransformedPath after applying rotation and
    /// moving to screen position. Used for drawing on the screen and
    /// in detecting collisions.
    /// </summary>
    public GraphicsPath TransformedPath { get; private set; } = new GraphicsPath();

    public Sprite()
    {
    }

    /// <summary>
    /// Update the rotation and position of the TransformedPath based 
    /// on the delta values. If the TransformedPath moves off the edge 
    /// of the screen, it is wrapped around to the other side.
    /// </summary>
    public void Advance()
    {
      RotationAngle += DeltaAngle;
      if (RotationAngle < 0)
        RotationAngle += 2 * Math.PI;
      if (RotationAngle > 2 * Math.PI)
        RotationAngle -= 2 * Math.PI;

      CurrentX += DeltaX;
      if (CurrentX < -Screen.Width / 2)
        CurrentX += Screen.Width;
      if (CurrentX > Screen.Width / 2)
        CurrentX -= Screen.Width;

      CurrentY -= DeltaY;
      if (CurrentY < -Screen.Height / 2)
        CurrentY += Screen.Height;
      if (CurrentY > Screen.Height / 2)
        CurrentY -= Screen.Height;
    }

    /// <summary>
    /// Render the TransformedPath.
    /// </summary>
    public void render()
    {
      try
      {
        if (JVM_SUX)
        {
          _location.Reset();
          float x = (float)Math.Round(CurrentX) + Screen.Width / 2;
          float y = (float)Math.Round(CurrentY) + Screen.Height / 2;
          double theta = RotationAngle * (180 / Math.PI);

          _location.Rotate((float)theta, MatrixOrder.Append);
          _location.Translate(x, y, MatrixOrder.Append);

          TransformedPath.Reset();
          TransformedPath.AddPath(Shape, true);
          TransformedPath.CloseFigure();
          TransformedPath.Transform(_location);
        }
        else
        {
          var points = new Point[Shape.PathPoints.Length];
          for (int i = 0; i < Shape.PathPoints.Length; i++)
          {
            float x_pt = Shape.PathPoints[i].X;
            float y_pt = Shape.PathPoints[i].Y;
            double cos = Math.Cos(RotationAngle);
            double sin = -Math.Sin(RotationAngle);
            points[i].X = (int)(Math.Round(x_pt * cos + y_pt * sin) + Math.Round(CurrentX) + Screen.Width / 2);
            points[i].Y = (int)(Math.Round(y_pt * cos - x_pt * sin) + Math.Round(CurrentY) + Screen.Height / 2);
          }
          TransformedPath.Reset();
          TransformedPath.AddPolygon(points);
        }
      }
      catch (Exception e)
      {
        Debug.Write(e.ToString());
      }
    }

    /// <summary>
    /// Determine if one TransformedPath overlaps with another, i.e., 
    /// if any vertice of one TransformedPath lands inside the other.
    /// </summary>
    public bool isColliding(Sprite s)
    {
      try
      {
        if (JVM_SUX)
        {
          lock (Form1.offGraphics)
          {
            _space.MakeInfinite();
            _space.Intersect(TransformedPath);
            _space.Intersect(s.TransformedPath);
            if (!_space.IsEmpty(Form1.offGraphics))
              return true;
          }
        }
        else
        {
          var path1 = s.TransformedPath;
          var path2 = TransformedPath;
          var polygon1 = s.TransformedPath.PathPoints;
          var polygon2 = TransformedPath.PathPoints;

          foreach (var pt in polygon1)
            if (path2.IsVisible(pt.X, pt.Y))
              return true;
          foreach (var pt in polygon2)
            if (path1.IsVisible(pt.X, pt.Y))
              return true;
        }
      }
      catch (Exception e)
      {
        Debug.Write(e.ToString());
      }

      return false;
    }
  }
}

