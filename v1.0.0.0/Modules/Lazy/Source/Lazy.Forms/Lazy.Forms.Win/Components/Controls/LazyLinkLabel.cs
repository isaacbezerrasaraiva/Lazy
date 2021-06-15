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
        }

        #endregion Constructors

        #region Methods

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (this.AutoOpenUrl == true)
                Process.Start(new ProcessStartInfo(this.Url) { UseShellExecute = true });
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            this.linkColorTemp = this.LinkColor;
            this.LinkColor = this.LinkHover;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            this.LinkColor = this.linkColorTemp;
        }

        #endregion Methods

        #region Properties

        public Color LinkHover { get; set; }

        public Boolean AutoOpenUrl { get; set; }

        public String Url { get; set; }

        #endregion Properties
    }
}