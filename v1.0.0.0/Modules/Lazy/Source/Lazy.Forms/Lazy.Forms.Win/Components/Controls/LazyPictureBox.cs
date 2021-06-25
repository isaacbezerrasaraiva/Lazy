// LazyPictureBox.cs
//
// This file is integrated part of Ark project
// Licensed under "Gnu General Public License Version 3"
//
// Created by Isaac Bezerra Saraiva
// Created on 2021, June 14

using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace Lazy.Forms.Win
{
    public class LazyPictureBox : PictureBox
    {
        #region Variables

        private Int32 lastX;
        private Int32 lastY;
        private Int32 lastWidth;
        private Int32 lastHeight;
        private Boolean drawAsSquare;
        private Boolean drawAsEllipse;

        #endregion Variables

        #region Constructors

        public LazyPictureBox()
        {
            this.LocationChanged += OnLocationChanged;
            this.SizeChanged += OnSizeChanged;
        }

        #endregion Constructors

        #region Methods

        private void OnLocationChanged(Object sender, EventArgs e)
        {
            this.lastX = this.Location.X;
            this.lastY = this.Location.Y;
        }

        private void OnSizeChanged(Object sender, EventArgs e)
        {
            if (this.drawAsSquare == true)
            {
                if (this.Location.X < this.lastX)
                {
                    this.Size = new Size(this.Size.Width, this.Size.Width);
                    this.Location = new Point(this.Location.X, this.Location.Y - (this.lastX - this.Location.X));
                }
                else if (this.Location.X > this.lastX)
                {
                    this.Size = new Size(this.Size.Width, this.Size.Width);
                    this.Location = new Point(this.Location.X, this.Location.Y + (this.Location.X - this.lastX));
                }
                else if (this.Location.Y < this.lastY)
                {
                    this.Size = new Size(this.Size.Height, this.Size.Height);
                    this.Location = new Point(this.Location.X - (this.lastY - this.Location.Y), this.Location.Y);
                }
                else if (this.Location.Y > this.lastY)
                {
                    this.Size = new Size(this.Size.Height, this.Size.Height);
                    this.Location = new Point(this.Location.X + (this.Location.Y - this.lastY), this.Location.Y);
                }
                else if (this.lastWidth != this.Size.Width)
                {
                    this.Size = new Size(this.Size.Width, this.Size.Width);
                }
                else if (this.lastHeight != this.Size.Height)
                {
                    this.Size = new Size(this.Size.Height, this.Size.Height);
                }

                this.lastWidth = this.Size.Width;
                this.lastHeight = this.Size.Height;
            }

            if (this.drawAsEllipse == true)
            {
                GraphicsPath graphicsPath = new GraphicsPath();
                graphicsPath.AddEllipse(0, 0, this.Width - 2, this.Height - 2);
                this.Region = new Region(graphicsPath);
            }
        }

        #endregion Methods

        #region Properties

        public Boolean DrawAsSquare
        {
            get { return this.drawAsSquare; }
            set
            {
                this.drawAsSquare = value;

                if (this.drawAsSquare == true)
                    OnSizeChanged(this, null);
            }
        }

        public Boolean DrawAsEllipse
        {
            get { return this.drawAsEllipse; }
            set
            {
                this.drawAsEllipse = value;

                if (this.drawAsEllipse == true)
                {
                    GraphicsPath graphicsPath = new GraphicsPath();
                    graphicsPath.AddEllipse(0, 0, this.Width - 2, this.Height - 2);
                    this.Region = new Region(graphicsPath);
                }
                else
                {
                    this.Region = null;
                }
            }
        }

        #endregion Properties
    }
}