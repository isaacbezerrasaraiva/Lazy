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

        private Int32 lastWidth;
        private Int32 lastHeight;
        private Int32 lastLocationX;
        private Int32 lastLocationY;
        private Boolean drawAsCircle;

        #endregion Variables

        #region Constructors

        public LazyPictureBox()
        {
            this.drawAsCircle = false;

            this.lastWidth = 0;
            this.lastHeight = 0;
            this.lastLocationX = this.Location.X;
            this.lastLocationY = this.Location.Y;
        }

        #endregion Constructors

        #region Methods

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.drawAsCircle == true)
            {
                if (this.Location.X != this.lastLocationX)
                {
                    this.Height = this.Width;
                }
                else if (this.Location.Y != this.lastLocationY)
                {
                    this.Width = this.Height;
                }
                else
                {
                    if (this.Width != this.lastWidth)
                        this.Height = this.Width;

                    if (this.Height != this.lastHeight)
                        this.Width = this.Height;
                }

                this.lastWidth = this.Width;
                this.lastHeight = this.Height;
                this.lastLocationX = this.Location.X;
                this.lastLocationY = this.Location.Y;

                GraphicsPath graphicsPath = new GraphicsPath();
                graphicsPath.AddEllipse(0, 0, this.Width - 2, this.Height - 2);
                this.Region = new Region(graphicsPath);
            }
        }

        #endregion Methods

        #region Properties

        public Boolean DrawAsCircle
        {
            get { return this.drawAsCircle; }
            set 
            {
                this.drawAsCircle = value;
                
                if (this.drawAsCircle == true)
                {
                    OnResize(new EventArgs());
                }
                else
                {
                    this.lastWidth = 0;
                    this.lastHeight = 0;
                    this.lastLocationX = this.Location.X;
                    this.lastLocationY = this.Location.Y;

                    this.Region = null;
                }
            }
        }

        #endregion Properties
    }
}