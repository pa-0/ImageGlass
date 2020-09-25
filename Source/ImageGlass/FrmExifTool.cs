﻿/*
ImageGlass Project - Image viewer for Windows
Copyright (C) 2020 DUONG DIEU PHAP
Project homepage: http://imageglass.org

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageGlass.Library.Image;
using ImageGlass.Settings;
using ImageGlass.UI.Renderers;

namespace ImageGlass {
    public partial class FrmExifTool: Form {
        public FrmExifTool() {
            InitializeComponent();
        }

        private ExifToolWrapper exifTool = new ExifToolWrapper(Configs.ExifToolExePath);

        private void btnClose_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void FrmExif_Load(object sender, EventArgs e) {
            SystemRenderer.ApplyTheme(lvExifItems);
            Local_OnImageChanged(null, null);
            

            // Load config
            // Windows Bound (Position + Size)
            Bounds = Configs.FrmExifToolWindowBound;

            // windows state
            WindowState = Configs.FrmExifToolWindowState;

            Local.OnImageChanged += Local_OnImageChanged;
        }

        private void FrmExif_FormClosing(object sender, FormClosingEventArgs e) {
            Local.OnImageChanged -= Local_OnImageChanged;

            // Save config
            if (WindowState == FormWindowState.Normal) {
                // Windows Bound
                Configs.FrmExifToolWindowBound = Bounds;
            }

            Configs.FrmExifToolWindowState = WindowState;
        }

        private void FrmExifTool_Activated(object sender, EventArgs e) {
            LoadLanguage();
        }

        private void FrmExif_KeyDown(object sender, KeyEventArgs e) {
            // close dialog
            if (e.KeyCode == Keys.Escape && !e.Control && !e.Shift && !e.Alt) {
                Close();

                return;
            }

            // copy value
            if (e.KeyCode == Keys.C && e.Control && !e.Shift && !e.Alt) {
                btnCopyValue.PerformClick();
            }
        }


        private void LoadLanguage() {
            var _lang = Configs.Language.Items;

            lnkSelectExifTool.Text = _lang[$"{nameof(frmSetting)}.lnkSelectExifTool"];
            lblNotFound.Text = string.Format(
                    _lang[$"{nameof(frmSetting)}.lnkSelectExifTool._NotFound"],
                    Configs.ExifToolExePath);

            clnProperty.Text = _lang[$"{Name}.{nameof(clnProperty)}"];
            clnValue.Text = _lang[$"{Name}.{nameof(clnValue)}"];

            btnCopyValue.Text = _lang[$"{Name}.{nameof(btnCopyValue)}"];
            btnExport.Text = _lang[$"{Name}.{nameof(btnExport)}"];
            btnClose.Text = _lang[$"{Name}.{nameof(btnClose)}"];
        }


        private void SetUIVisibility(bool isNotFound) {
            if (isNotFound) {
                this.Text = Configs.Language.Items["frmMain.mnuExifTool"];
                this.Icon = Icon.ExtractAssociatedIcon(Base.App.IGExePath);

                panNotFound.Visible = true;
                lvExifItems.Visible = false;
            }
            else {
                this.Text = Path.GetFileName(Configs.ExifToolExePath);
                this.Icon = Icon.ExtractAssociatedIcon(Configs.ExifToolExePath);

                panNotFound.Visible = false;
                lvExifItems.Visible = true;
            }
        }


        private async void Local_OnImageChanged(object sender, EventArgs e) {
            SetFormState(false);

            // check if exif tool exists
            this.exifTool.ToolPath = Configs.ExifToolExePath;
            if (!this.exifTool.CheckExists()) {
                SetUIVisibility(true);

                return;
            }

            SetUIVisibility(false);
            await Task.Run(() => {
                var filename = Local.ImageList.GetFileName(Local.CurrentIndex);
                this.exifTool.LoadExifData(filename);
            });

            lvExifItems.Items.Clear();
            lvExifItems.Groups.Clear();

            // get groups
            var groups = this.exifTool.GroupBy(i => i.Group)
                .Select(group => new { Group = group.Key })
                .Distinct()
                .ToList();

            foreach (var item in groups) {
                lvExifItems.Groups.Add(item.Group, item.Group);
            }

            // load items
            for (var i = 0; i < this.exifTool.Count; i++) {
                var item = this.exifTool[i];
                var li = new ListViewItem((i + 1).ToString());

                li.SubItems.Add(item.Name);
                li.SubItems.Add(item.Value);
                li.Group = lvExifItems.Groups[item.Group];

                lvExifItems.Items.Add(li);
            }

            SetFormState(true);
        }

        private void btnCopyValue_Click(object sender, EventArgs e) {
            if (lvExifItems.SelectedItems.Count > 0) {
                Clipboard.SetText(lvExifItems.SelectedItems[0].SubItems[2].Text);
            }
        }

        private void SetFormState(bool enabled = true) {
            btnCopyValue.Enabled =
                btnExport.Enabled =
                lblNotFound.Enabled = enabled;
        }

        private void lnkSelectExifTool_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var ofd = new OpenFileDialog() {
                CheckFileExists = true,
                Filter = "exiftool.exe file|*.exe",
            };

            if (ofd.ShowDialog() == DialogResult.OK) {
                var exif = new ExifToolWrapper(ofd.FileName);

                if (!exif.CheckExists()) {
                    lblNotFound.Text = string.Format(
                        Configs.Language.Items[$"{nameof(frmSetting)}.lnkSelectExifTool._NotFound"],
                        ofd.FileName);
                }
                else {
                    Configs.ExifToolExePath = ofd.FileName;
                    Local_OnImageChanged(null, null);
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e) {
            var sfd = new SaveFileDialog();

            if (sfd.ShowDialog() == DialogResult.OK) {
                var imgFile = Local.ImageList.GetFileName(Local.CurrentIndex);
                // todo:
                // exiftool.exe "C:\Users\d2pha\Desktop\_photo\f.webp" -fast > aaaaa.txt
                var errorMsg = exifTool.SaveExifData(imgFile, sfd.FileName);

                if (errorMsg.Length > 0) {
                    MessageBox.Show(errorMsg);
                }
            }
        }
    }
}
