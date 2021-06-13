// LazyUserControl.cs
//
// This file is integrated part of Ark project
// Licensed under "Gnu General Public License Version 3"
//
// Created by Isaac Bezerra Saraiva
// Created on 2021, June 12

using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace Lazy.Forms.Win
{
    public partial class LazyUserControl : UserControl
    {
        #region Events

        public event EventHandler ParentSizeChanged;

        #endregion Events

        #region Variables

        private Object lastParent;
        private Boolean keepCenteredToParent;

        #endregion Variables

        #region Constructors

        public LazyUserControl()
        {
            InitializeComponent();

            this.ParentSizeChanged += OnParentSizeChanged;
        }

        #endregion Constructors

        #region Methods

        private void OnParentChanged(Object sender, EventArgs args)
        {
            if (this.lastParent != null)
            {
                if (this.lastParent != this.Parent)
                {
                    if (this.lastParent is Form)
                        ((Form)this.lastParent).SizeChanged -= this.ParentSizeChanged;
                    else if (this.lastParent is Control)
                        ((Control)this.lastParent).SizeChanged -= this.ParentSizeChanged;
                }
            }

            if (this.Parent != null)
            {
                this.Parent.SizeChanged += this.ParentSizeChanged;
                this.ParentSizeChanged(this, new EventArgs());
            }

            this.lastParent = this.Parent;
        }

        private void OnParentSizeChanged(Object sender, EventArgs e)
        {
            if (this.DesignMode == false && this.keepCenteredToParent == true)
                this.Location = new Point((this.Parent.Size.Width / 2) - (this.Width / 2), (this.Parent.Size.Height / 2) - (this.Height / 2));
        }

        #endregion Methods

        #region Properties

        public Boolean KeepCenteredToParent
        {
            get { return this.keepCenteredToParent; }
            set { this.keepCenteredToParent = value; }
        }

        #endregion Properties
    }
}
