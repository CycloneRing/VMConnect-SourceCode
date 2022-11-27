using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Security.Permissions;
using System.Windows.Forms;
using Microsoft.Virtualization.Client.InteractiveSession.Resources;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x0200000A RID: 10
	internal partial class EnhancedConnectionDialog : Form, IDpiForm
	{
		// Token: 0x0600005F RID: 95 RVA: 0x000057F8 File Offset: 0x000039F8
		public EnhancedConnectionDialog(Screen targetScreen, RdpOptions options, string virtualMachine)
		{
			this.InitializeComponent();
			base.SuspendLayout();
			this._oldDpi = this.DeviceDpi();
			this.m_WindowInterceptors = new NativeWindowDoubleClickInterceptor(new List<Control>
			{
				this.m_Printer,
				this.m_Clipboard,
				this.m_UseAllMonitorBox,
				this.m_SaveSwitch
			});
			this.m_Options.Hide();
			this.m_OptionShown = false;
			this.UpdateButtonImage();
			this.m_RdpOptions = options;
			Resolution defaultResolution = new Resolution(this.m_RdpOptions.DesktopSize.Width, this.m_RdpOptions.DesktopSize.Height);
			this.m_Display = new DisplayResolution(defaultResolution);
			this.InitResolutionBar(targetScreen);
			if (Screen.AllScreens.Length == 1)
			{
				this.m_UseAllMonitorBox.Enabled = false;
				this.m_RdpOptions.UseAllMonitors = false;
			}
			if (!this.m_RdpOptions.EnablePrinterRedirection)
			{
				this.m_Printer.Checked = false;
				this.m_Printer.Enabled = false;
			}
			else
			{
				this.m_Printer.Checked = this.m_RdpOptions.PrinterRedirection;
			}
			this.m_Clipboard.Checked = this.m_RdpOptions.ClipboardRedirection;
			this.m_UseAllMonitorBox.Checked = this.m_RdpOptions.UseAllMonitors;
			this.m_SaveSwitch.Checked = this.m_RdpOptions.SaveButtonChecked;
			if (this.m_RdpOptions.FullScreen)
			{
				this.m_DisplayResolution.Value = this.m_DisplayResolution.Maximum;
			}
			this.ChangeResolution(this.m_DisplayResolution.Value);
			this.m_DisplayResolution.Focus();
			this.Text = string.Format(CultureInfo.CurrentCulture, ConnectionResources.EnhancedFormTitle, virtualMachine);
			this.FixupSaveSwitch();
			this.FixupDisplayConfigurationLayout();
			this.FixupGeneralOptionsLayout();
			this.SetDefaultControlHeights();
			base.ClientSize = new Size(base.ClientSize.Width, this.m_ShrunkHeight);
			this.FixupPanelControlLocations();
			base.ResumeLayout(false);
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000060 RID: 96 RVA: 0x00005A04 File Offset: 0x00003C04
		// (remove) Token: 0x06000061 RID: 97 RVA: 0x00005A3C File Offset: 0x00003C3C
		public new event System.Windows.Forms.DpiChangedEventHandler DpiChanged;

		// Token: 0x06000062 RID: 98 RVA: 0x00005A71 File Offset: 0x00003C71
		public void OnDpiChanged(System.Windows.Forms.DpiChangedEventArgs e)
		{
			DpiUtilities.SetWindowPosition(base.Handle, e.SuggestedRectangle);
			if (this._dpiBeforeResize == 0)
			{
				this.FixupFormLayout();
			}
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00005A94 File Offset: 0x00003C94
		private void FixupFormLayout()
		{
			this.FixupDisplayConfigurationLayout();
			this.FixupGeneralOptionsLayout();
			this.SetDefaultControlHeights();
			base.ClientSize = new Size(this.m_ButtonsFlowLayoutPanel.Width + 10 + this.LogicalToDeviceUnits(10), this.m_OptionShown ? this.m_ExpandedHeight : this.m_ShrunkHeight);
			this.FixupPanelControlLocations();
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00005AF4 File Offset: 0x00003CF4
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 736)
			{
				Rectangle suggestedRectangle = DpiUtilities.GetSuggestedRectangle(m.LParam);
				int num = this.NewDpi(ref m);
				System.Windows.Forms.DpiChangedEventArgs e = new System.Windows.Forms.DpiChangedEventArgs(this._oldDpi, num, suggestedRectangle);
				System.Windows.Forms.DpiChangedEventHandler dpiChanged = this.DpiChanged;
				if (dpiChanged != null)
				{
					dpiChanged(this, e);
				}
				this.OnDpiChanged(e);
				this._oldDpi = num;
			}
			base.WndProc(ref m);
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00005B58 File Offset: 0x00003D58
		protected override void OnResizeBegin(EventArgs e)
		{
			base.OnResizeBegin(e);
			this._dpiBeforeResize = DpiUtilities.DeviceDpi(base.Handle);
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00005B72 File Offset: 0x00003D72
		protected override void OnResizeEnd(EventArgs e)
		{
			base.OnResizeEnd(e);
			if (this._dpiBeforeResize != this.DeviceDpi())
			{
				this.FixupFormLayout();
			}
			this._dpiBeforeResize = 0;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00005B98 File Offset: 0x00003D98
		private void FixupDisplayConfigurationLayout()
		{
			this.m_SmallResolution.Location = new Point(this.m_SmallResolution.Location.X, this.m_ChooseResolutionLabel.Location.Y + this.m_ChooseResolutionLabel.Height + 5);
			this.m_DisplayResolution.Location = new Point(this.m_SmallResolution.Location.X + this.m_SmallResolution.Width + 5, this.m_ChooseResolutionLabel.Location.Y + this.m_ChooseResolutionLabel.Height);
			this.m_LargeResolution.Location = new Point(this.m_DisplayResolution.Location.X + this.m_DisplayResolution.Width + 5, this.m_ChooseResolutionLabel.Location.Y + this.m_ChooseResolutionLabel.Height + 5);
			this.m_ResolutionLabel.Location = new Point(this.m_ResolutionLabel.Location.X, this.m_DisplayResolution.Location.Y + this.m_DisplayResolution.Height - 5);
			this.m_UseAllMonitorBox.Location = new Point(this.m_UseAllMonitorBox.Location.X, this.m_ResolutionLabel.Location.Y + this.m_ResolutionLabel.Height + 5);
			this.m_DisplayConfiguration.Height = this.m_UseAllMonitorBox.Bottom + 5;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00005D2C File Offset: 0x00003F2C
		private void FixupGeneralOptionsLayout()
		{
			int height = Math.Max(this.m_DisplayConfiguration.Height + this.m_SaveSwitch.Height + 10, this.m_DriveTab.Height);
			this.m_GeneralOption.Height = height;
			this.m_DriveTab.Height = height;
			this.m_SaveSwitch.Top = this.m_GeneralOption.Height - 5 - this.m_SaveSwitch.Height;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00005DA0 File Offset: 0x00003FA0
		private void FixupSaveSwitch()
		{
			Size size = TextRenderer.MeasureText(this.m_SaveSwitch.Text, this.m_SaveSwitch.Font);
			this.m_SaveSwitch.Height = this.m_SaveSwitch.Height * (int)Math.Ceiling((double)size.Width / (double)this.m_SaveSwitch.Width);
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00005DFC File Offset: 0x00003FFC
		private void SetDefaultControlHeights()
		{
			int num = this.LogicalToDeviceUnits(10);
			int num2 = 3 * num;
			int height = this.m_ButtonsFlowLayoutPanel.Height;
			this.m_ShrunkHeight = this.m_DisplayConfiguration.Height + height + num2;
			this.m_ExpandedHeight = this.m_Options.Height + height + num2;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00005E4C File Offset: 0x0000404C
		private void FixupPanelControlLocations()
		{
			if (this.m_OptionShown)
			{
				this.m_ButtonsFlowLayoutPanel.Top = this.m_Options.Bottom + this.DeviceToLogicalUnits(10);
				return;
			}
			this.m_ButtonsFlowLayoutPanel.Top = this.m_DisplayConfiguration.Bottom + this.DeviceToLogicalUnits(10);
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00005EA0 File Offset: 0x000040A0
		private void InitResolutionBar(Screen targetScreen)
		{
			this.m_Display.CreateResolutionList(targetScreen);
			this.m_DisplayResolution.Minimum = 0;
			this.m_DisplayResolution.Maximum = this.m_Display.AllResolutions.Count;
			this.m_DisplayResolution.Value = this.m_Display.DefaultResolutionIndex;
			this.ChangeResolution(this.m_DisplayResolution.Value);
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00005F08 File Offset: 0x00004108
		private void ChangeResolution(int idx)
		{
			this.m_MaxResolutionSelected = false;
			if (idx > this.m_DisplayResolution.Maximum || idx < this.m_DisplayResolution.Minimum)
			{
				return;
			}
			if (idx == this.m_DisplayResolution.Maximum)
			{
				this.m_ResolutionLabel.Text = ConnectionResources.FullScreen;
				this.m_SelectedResolution = this.m_Display.AllResolutions[this.m_Display.FullScreenResolutionIndex];
				this.m_MaxResolutionSelected = true;
				return;
			}
			this.m_ResolutionLabel.Text = string.Format(CultureInfo.CurrentCulture, ConnectionResources.ResolutionFormat, this.m_Display.AllResolutions[idx].Width, this.m_Display.AllResolutions[idx].Height);
			this.m_SelectedResolution = this.m_Display.AllResolutions[idx];
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00005FE7 File Offset: 0x000041E7
		private void DisplayResolution_Scroll(object sender, EventArgs e)
		{
			this.ChangeResolution(this.m_DisplayResolution.Value);
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00005FFA File Offset: 0x000041FA
		private void UpdateButtonImage()
		{
			this.m_ShowOption.Image = DpiUtilities.GetImage(ConnectionResources.ResourceManager, this.m_OptionShown ? "up" : "down");
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00006028 File Offset: 0x00004228
		private void FinishToggleButton()
		{
			if (this.m_OptionShown)
			{
				this.m_ShowOption.Text = ConnectionResources.HideOptions;
				this.m_Options.Show();
				this.m_DisplayResolution.BackColor = SystemColors.Window;
				base.Controls.Remove(this.m_DisplayConfiguration);
				this.m_GeneralOption.Controls.Add(this.m_DisplayConfiguration);
				this.m_DisplayConfiguration.Location = new Point(5, 5);
				this.m_DisplayConfiguration.Width -= 20;
			}
			this.FixupPanelControlLocations();
			this.m_Expanding = false;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000060C4 File Offset: 0x000042C4
		private void ShowOption_Click(object sender, EventArgs e)
		{
			if (!this.m_Expanding)
			{
				this.m_Expanding = true;
				this.m_Options.SuspendLayout();
				this.m_GeneralOption.SuspendLayout();
				this.m_DisplayConfiguration.SuspendLayout();
				this.m_OptionShown = !this.m_OptionShown;
				if (!this.m_OptionShown)
				{
					this.m_ShowOption.Text = ConnectionResources.ShowOptions;
					this.m_GeneralOption.Controls.Remove(this.m_DisplayConfiguration);
					base.Controls.Add(this.m_DisplayConfiguration);
					this.m_DisplayConfiguration.Location = new Point(10, 10);
					this.m_DisplayConfiguration.Width += 20;
					this.m_DisplayResolution.BackColor = SystemColors.Control;
					this.m_Options.Hide();
				}
				int value = base.RectangleToScreen(base.ClientRectangle).Top - base.Top;
				AnimatedFormResizer.Resize(this, new Size(base.Width, (this.m_OptionShown ? this.m_ExpandedHeight : this.m_ShrunkHeight) + this.DeviceToLogicalUnits(value)), TimeSpan.FromMilliseconds(200.0), new Action(this.FinishToggleButton), AnimationMode.Linear);
				if (this.m_UseAllMonitorBox.Checked)
				{
					this.m_UseAllMonitorBox.Focus();
				}
				else
				{
					this.m_DisplayResolution.Focus();
				}
				this.UpdateButtonImage();
				this.m_DisplayConfiguration.ResumeLayout();
				this.m_GeneralOption.ResumeLayout();
				this.m_Options.ResumeLayout();
			}
		}

		// Token: 0x06000072 RID: 114 RVA: 0x0000624C File Offset: 0x0000444C
		private void m_UseAllMonitorBox_CheckedChanged(object sender, EventArgs e)
		{
			if (this.m_UseAllMonitorBox.Checked)
			{
				this.m_DisplayResolution.Enabled = false;
				this.m_LastChosenResolution = this.m_DisplayResolution.Value;
				this.m_DisplayResolution.Value = this.m_DisplayResolution.Maximum;
				this.m_ResolutionLabel.Text = ConnectionResources.FullScreen;
			}
			else
			{
				this.m_DisplayResolution.Enabled = true;
				this.m_DisplayResolution.Value = this.m_LastChosenResolution;
			}
			this.ChangeResolution(this.m_DisplayResolution.Value);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000062D9 File Offset: 0x000044D9
		private void m_AudioSettings_Click(object sender, EventArgs e)
		{
			if (this.m_RdpOptions != null)
			{
				this.m_AudioSettingsDlg = new AudioSettingDialog(this.m_RdpOptions);
				this.m_AudioSettingsDlg.StartPosition = FormStartPosition.CenterParent;
				this.m_AudioSettingsDlg.ShowDialog(this);
				this.m_AudioSettingsDlg.Dispose();
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00006318 File Offset: 0x00004518
		private void m_MoreDevices_Click(object sender, EventArgs e)
		{
			if (this.m_RdpOptions != null)
			{
				this.m_LocalResource = new LocalResourcesDialog(this.m_RdpOptions);
				this.m_LocalResource.StartPosition = FormStartPosition.CenterParent;
				this.m_LocalResource.ShowDialog(this);
				this.m_LocalResource.Dispose();
			}
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00006358 File Offset: 0x00004558
		private void m_ConnectButton_Click(object sender, EventArgs e)
		{
			if (this.m_RdpOptions.EnablePrinterRedirection)
			{
				this.m_RdpOptions.PrinterRedirection = this.m_Printer.Checked;
			}
			this.m_RdpOptions.ClipboardRedirection = this.m_Clipboard.Checked;
			this.m_RdpOptions.UseAllMonitors = this.m_UseAllMonitorBox.Checked;
			this.m_RdpOptions.FullScreen = this.m_MaxResolutionSelected;
			this.m_RdpOptions.EnhancedModeDialogScreen = (this.m_MaxResolutionSelected ? Screen.FromControl(this) : null);
			this.m_RdpOptions.DesktopSize = new Size(this.m_SelectedResolution.Width, this.m_SelectedResolution.Height);
			if (this.m_SaveSwitch.Checked)
			{
				this.m_RdpOptions.Save();
			}
			else
			{
				this.m_RdpOptions.Revert();
			}
			base.Close();
			base.Dispose();
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00006438 File Offset: 0x00004638
		private void m_HelpButton_Click(object sender, EventArgs e)
		{
			new Process
			{
				EnableRaisingEvents = false,
				StartInfo = 
				{
					FileName = "https://go.microsoft.com/fwlink/?LinkId=279826"
				}
			}.Start();
		}

		// Token: 0x06000077 RID: 119 RVA: 0x0000645C File Offset: 0x0000465C
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Escape)
			{
				base.Close();
				base.Dispose();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		// Token: 0x04000055 RID: 85
		private int _oldDpi = 96;

		// Token: 0x04000056 RID: 86
		private int _dpiBeforeResize;

		// Token: 0x04000058 RID: 88
		private RdpOptions m_RdpOptions;

		// Token: 0x04000059 RID: 89
		private LocalResourcesDialog m_LocalResource;

		// Token: 0x0400005A RID: 90
		private DisplayResolution m_Display;

		// Token: 0x0400005B RID: 91
		private bool m_MaxResolutionSelected;

		// Token: 0x0400005C RID: 92
		private Resolution m_SelectedResolution;

		// Token: 0x0400005D RID: 93
		private AudioSettingDialog m_AudioSettingsDlg;

		// Token: 0x0400005E RID: 94
		private int m_ExpandedHeight;

		// Token: 0x0400005F RID: 95
		private int m_ShrunkHeight;

		// Token: 0x04000060 RID: 96
		private bool m_Expanding;

		// Token: 0x04000061 RID: 97
		private NativeWindowDoubleClickInterceptor m_WindowInterceptors;

		// Token: 0x0400006D RID: 109
		private bool m_OptionShown;

		// Token: 0x0400006E RID: 110
		private int m_LastChosenResolution;
	}
}
