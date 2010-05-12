//
// DO NOT REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
//
// @Authors:
//       timop
//
// Copyright 2004-2010 by OM International
//
// This file is part of OpenPetra.org.
//
// OpenPetra.org is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// OpenPetra.org is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with OpenPetra.org.  If not, see <http://www.gnu.org/licenses/>.
//
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Ict.Common.Controls
{
    /// <summary>
    /// This special statusbar will monitor the active control
    /// and displays the help text for the active control in the statusbar
    /// see also http://msdn.microsoft.com/en-us/library/ms229066.aspx
    /// and http://www.vb-helper.com/howto_net_focus_status.html
    /// </summary>
    public class TExtStatusBarHelp : System.Windows.Forms.StatusStrip, IExtenderProvider
    {
        private Hashtable FControlTexts;
        private System.Windows.Forms.Control FActiveControl;
        private System.Windows.Forms.ToolStripStatusLabel FStatusLabel;

        /// <summary>
        /// constructor
        /// </summary>
        public TExtStatusBarHelp()
        {
            this.FStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.SuspendLayout();

            this.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                    this.FStatusLabel
                });

            this.FStatusLabel.Name = "FStatusLabel";
            this.FStatusLabel.Size = new System.Drawing.Size(109, 17);

            this.ResumeLayout(false);
            this.PerformLayout();

            FControlTexts = new Hashtable();
        }

        bool IExtenderProvider.CanExtend(object target)
        {
            if (target is Control
                && !(target is TExtStatusBarHelp))
            {
                return true;
            }

            return false;
        }

        //
        // <doc>
        // <desc>
        //      This is an event handler that responds to the OnControlEnter
        //      event.  We attach this to each control we are providing help
        //      text for.
        // </desc>
        // </doc>
        //
        private void OnControlEnter(object sender, EventArgs e)
        {
            FActiveControl = (Control)sender;
            UpdateLabel();
        }

        //
        // <doc>
        // <desc>
        //      This is an event handler that responds to the OnControlLeave
        //      event.  We attach this to each control we are providing help
        //      text for.
        // </desc>
        // </doc>
        //
        private void OnControlLeave(object sender, EventArgs e)
        {
            if (sender == FActiveControl)
            {
                FActiveControl = null;
                UpdateLabel();
            }
        }

        /// <summary>
        /// add a control and the text that should be displayed in the statusbar when the control is focused
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        public void SetHelpText(Control control, string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }

            if (value.Length == 0)
            {
                FControlTexts.Remove(control);

                control.Enter -= new EventHandler(OnControlEnter);
                control.Leave -= new EventHandler(OnControlLeave);
            }
            else
            {
                FControlTexts[control] = value;

                control.Enter += new EventHandler(OnControlEnter);
                control.Leave += new EventHandler(OnControlLeave);
            }

            if (control == FActiveControl)
            {
                UpdateLabel();
            }
        }

        void UpdateLabel()
        {
            if (FActiveControl != null)
            {
                string text = (string)FControlTexts[FActiveControl];

                if ((text != null) && (text != this.FStatusLabel.Text))
                {
                    this.FStatusLabel.Text = text;
                }
            }
            else
            {
                this.FStatusLabel.Text = "";
            }
        }

        /// show a message in the status bar, independent of the selected control
        public void ShowMessage(string msg)
        {
            FStatusLabel.Text = msg;
        }
    }
}