// LazyTextBox.cs
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
    public class LazyTextBox : TextBox
    {
        #region Events

        public event EventHandler ParentSizeChanged;

        #endregion Events

        #region Variables

        private Object lastParent;
        private Boolean dockOnCenter;

        #endregion Variables

        #region Constructors

        public LazyTextBox()
        {
            this.ParentSizeChanged += OnParentSizeChanged;
        }

        #endregion Constructors

        #region Methods

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

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
            if (this.DesignMode == false && this.dockOnCenter == true)
                this.Location = new Point((this.Parent.Size.Width / 2) - (this.Width / 2), (this.Parent.Size.Height / 2) - (this.Height / 2));
        }

        #endregion Methods

        #region Properties

        public Boolean DockOnCenter
        {
            get { return this.dockOnCenter; }
            set { this.dockOnCenter = value; }
        }

        #endregion Properties
    }
}