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
    /// Defines a game object, including it's shape, position, 
    /// movement and rotation. It also can detemine if two objects collide.
    /// </summary>
    public class Sprite
    {
        private static bool JVM_SUX = true;

        /// <summary>Dimensions of the graphics area.</summary>
        public static Size Screen;

        /// <summary>Initial sprite shape, centered at the origin (0,0).</summary>
        public GraphicsPath shape;

        /// <summary>Active flag.</summary>
        public bool active;

        /// <summary>Current angle of rotation.</summary>
        public double angle;

        /// <summary>Amount to change the rotation angle.</summary>
        public double deltaAngle;

        /// <summary>Current position on screen.</summary>
        public double currentX, currentY;

        /// <summary>Amount to change the screen position.</summary>
        public double deltaX, deltaY;

        private Matrix location = new Matrix();
        private Region space = new Region();

        /// <summary>
        /// Final location and shape of sprite after applying rotation and
        /// moving to screen position. Used for drawing on the screen and
        /// in detecting collisions.
        /// </summary>
        public GraphicsPath sprite;

        public Sprite()
        {
            this.shape = new GraphicsPath();
            this.active = false;
            this.angle = 0.0;
            this.deltaAngle = 0.0;
            this.currentX = 0.0;
            this.currentY = 0.0;
            this.deltaX = 0.0;
            this.deltaY = 0.0;
            this.sprite = new GraphicsPath();
        }

        /// <summary>
        /// Update the rotation and position of the sprite based 
        /// on the delta values. If the sprite moves off the edge 
        /// of the screen, it is wrapped around to the other side.
        /// </summary>
        public void advance()
        {
            this.angle += this.deltaAngle;
            if (this.angle < 0)
                this.angle += 2 * Math.PI;
            if (this.angle > 2 * Math.PI)
                this.angle -= 2 * Math.PI;
            this.currentX += this.deltaX;
            if (this.currentX < -Screen.Width / 2)
                this.currentX += Screen.Width;
            if (this.currentX > Screen.Width / 2)
                this.currentX -= Screen.Width;
            this.currentY -= this.deltaY;
            if (this.currentY < -Screen.Height / 2)
                this.currentY += Screen.Height;
            if (this.currentY > Screen.Height / 2)
                this.currentY -= Screen.Height;
        }

        /// <summary>
        /// Render the sprite.
        /// </summary>
        public void render()
        {
            try
            {
                if (JVM_SUX)
                {
                    location.Reset();
                    float x = (float)Math.Round(this.currentX) + Screen.Width / 2;
                    float y = (float)Math.Round(this.currentY) + Screen.Height / 2;
                    double theta = this.angle * (180 / Math.PI); //* (float)(2 * Math.PI)

                    location.Rotate((float)theta, MatrixOrder.Append);
                    location.Translate(x, y, MatrixOrder.Append);

                    this.sprite.Reset();
                    this.sprite.AddPath(this.shape, true);
                    this.sprite.CloseFigure();
                    this.sprite.Transform(location);
                }
                else
                {
                    Point[] points = new Point[this.shape.PathPoints.Length];
                    for (int i = 0; i < this.shape.PathPoints.Length; i++)
                    {
                        float x_pt = this.shape.PathPoints[i].X;
                        float y_pt = this.shape.PathPoints[i].Y;
                        double cos = Math.Cos(this.angle);
                        double sin = -Math.Sin(this.angle);
                        points[i].X = (int)(Math.Round(x_pt * cos + y_pt * sin) + Math.Round(this.currentX) + Screen.Width / 2);
                        points[i].Y = (int)(Math.Round(y_pt * cos - x_pt * sin) + Math.Round(this.currentY) + Screen.Height / 2);
                    }
                    this.sprite.Reset();
                    this.sprite.AddPolygon(points);
                }
            }
            catch (Exception e)
            {
                Debug.Write(e.ToString());
            }
        }

        /// <summary>
        /// Determine if one sprite overlaps with another, i.e., 
        /// if any vertice of one sprite lands inside the other.
        /// </summary>
        public bool isColliding(Sprite s)
        {
            try
            {
                if (JVM_SUX)
                {
                    lock (Form1.offGraphics)
                    {
                        space.MakeInfinite();
                        space.Intersect(sprite);
                        space.Intersect(s.sprite);
                        if (!space.IsEmpty(Form1.offGraphics))
                            return true;
                    }
                }
                else
                {
                    GraphicsPath path1 = s.sprite;
                    GraphicsPath path2 = this.sprite;
                    PointF[] polygon1 = s.sprite.PathPoints;
                    PointF[] polygon2 = this.sprite.PathPoints;

                    for (int i = 0; i < polygon1.Length; i++)
                        if (path2.IsVisible(polygon1[i].X, polygon1[i].Y))
                            return true;
                    for (int i = 0; i < polygon2.Length; i++)
                        if (path1.IsVisible(polygon2[i].X, polygon2[i].Y))
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

