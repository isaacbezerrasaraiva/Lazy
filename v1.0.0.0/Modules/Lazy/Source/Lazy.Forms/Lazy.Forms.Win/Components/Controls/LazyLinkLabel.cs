// LazyLinkLabel.cs
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
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace Lazy.Forms.Win
{
    public class LazyLinkLabel : LinkLabel
    {
        #region Variables

        private Color linkColorTemp;

        #endregion Variables

        #region Constructors

        public LazyLinkLabel()
        {
            this.linkColorTemp = this.LinkColor;
            this.LinkHover = this.LinkColor;

            this.MouseEnter += OnMouseEnter;
            this.MouseLeave += OnMouseLeave;
            this.Click += OnClick;
        }

        #endregion Constructors

        #region Methods

        private void OnMouseEnter(Object sender, EventArgs e)
        {
            this.linkColorTemp = this.LinkColor;
            this.LinkColor = this.LinkHover;
        }

        private void OnMouseLeave(Object sender, EventArgs e)
        {
            this.LinkColor = this.linkColorTemp;
        }

        private void OnClick(Object sender, EventArgs e)
        {
            if (this.AutoOpenUrl == true)
                Process.Start(new ProcessStartInfo(this.Url) { UseShellExecute = true });
        }

        #endregion Methods

        #region Properties

        public Color LinkHover { get; set; }

        public Boolean AutoOpenUrl { get; set; }

        public String Url { get; set; }

        #endregion Properties
    }
}