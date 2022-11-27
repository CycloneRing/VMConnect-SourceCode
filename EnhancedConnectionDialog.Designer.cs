namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x0200000A RID: 10
	internal partial class EnhancedConnectionDialog : global::System.Windows.Forms.Form, global::System.Windows.Forms.IDpiForm
	{
		// Token: 0x06000078 RID: 120 RVA: 0x00006479 File Offset: 0x00004679
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00006498 File Offset: 0x00004698
		private void InitializeComponent()
		{
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::Microsoft.Virtualization.Client.InteractiveSession.EnhancedConnectionDialog));
			this.m_DisplayConfiguration = new global::System.Windows.Forms.GroupBox();
			this.m_UseAllMonitorBox = new global::System.Windows.Forms.CheckBox();
			this.m_ResolutionLabel = new global::System.Windows.Forms.Label();
			this.m_LargeResolution = new global::System.Windows.Forms.Label();
			this.m_SmallResolution = new global::System.Windows.Forms.Label();
			this.m_DisplayResolution = new global::System.Windows.Forms.TrackBar();
			this.m_ChooseResolutionLabel = new global::System.Windows.Forms.Label();
			this.m_DisplayIcon = new global::System.Windows.Forms.PictureBox();
			this.m_Options = new global::System.Windows.Forms.TabControl();
			this.m_GeneralOption = new global::System.Windows.Forms.TabPage();
			this.m_SaveSwitch = new global::System.Windows.Forms.CheckBox();
			this.m_DriveTab = new global::System.Windows.Forms.TabPage();
			this.m_RemoteAudio = new global::System.Windows.Forms.GroupBox();
			this.m_AudioIcon = new global::System.Windows.Forms.PictureBox();
			this.m_AudioSettings = new global::System.Windows.Forms.Button();
			this.m_AudioLabel = new global::System.Windows.Forms.Label();
			this.m_DriveList = new global::System.Windows.Forms.GroupBox();
			this.pictureBox1 = new global::System.Windows.Forms.PictureBox();
			this.m_Clipboard = new global::System.Windows.Forms.CheckBox();
			this.m_Printer = new global::System.Windows.Forms.CheckBox();
			this.m_ChooseDevices = new global::System.Windows.Forms.Label();
			this.m_MoreDevices = new global::System.Windows.Forms.Button();
			this.m_ShowOption = new global::System.Windows.Forms.Button();
			this.m_HelpButton = new global::System.Windows.Forms.Button();
			this.m_ConnectButton = new global::System.Windows.Forms.Button();
			this.m_ButtonsFlowLayoutPanel = new global::System.Windows.Forms.Panel();
			this.m_DisplayConfiguration.SuspendLayout();
			((global::System.ComponentModel.ISupportInitialize)this.m_DisplayResolution).BeginInit();
			((global::System.ComponentModel.ISupportInitialize)this.m_DisplayIcon).BeginInit();
			this.m_Options.SuspendLayout();
			this.m_GeneralOption.SuspendLayout();
			this.m_DriveTab.SuspendLayout();
			this.m_RemoteAudio.SuspendLayout();
			((global::System.ComponentModel.ISupportInitialize)this.m_AudioIcon).BeginInit();
			this.m_DriveList.SuspendLayout();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
			this.m_ButtonsFlowLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.m_DisplayConfiguration.BackColor = global::System.Drawing.Color.Transparent;
			this.m_DisplayConfiguration.Controls.Add(this.m_UseAllMonitorBox);
			this.m_DisplayConfiguration.Controls.Add(this.m_ResolutionLabel);
			this.m_DisplayConfiguration.Controls.Add(this.m_LargeResolution);
			this.m_DisplayConfiguration.Controls.Add(this.m_SmallResolution);
			this.m_DisplayConfiguration.Controls.Add(this.m_DisplayResolution);
			this.m_DisplayConfiguration.Controls.Add(this.m_ChooseResolutionLabel);
			this.m_DisplayConfiguration.Controls.Add(this.m_DisplayIcon);
			this.m_DisplayConfiguration.Cursor = global::System.Windows.Forms.Cursors.Default;
			this.m_DisplayConfiguration.ForeColor = global::System.Drawing.SystemColors.WindowText;
			componentResourceManager.ApplyResources(this.m_DisplayConfiguration, "m_DisplayConfiguration");
			this.m_DisplayConfiguration.Name = "m_DisplayConfiguration";
			this.m_DisplayConfiguration.TabStop = false;
			componentResourceManager.ApplyResources(this.m_UseAllMonitorBox, "m_UseAllMonitorBox");
			this.m_UseAllMonitorBox.Name = "m_UseAllMonitorBox";
			this.m_UseAllMonitorBox.UseVisualStyleBackColor = true;
			this.m_UseAllMonitorBox.CheckedChanged += new global::System.EventHandler(this.m_UseAllMonitorBox_CheckedChanged);
			this.m_ResolutionLabel.ForeColor = global::System.Drawing.SystemColors.WindowText;
			componentResourceManager.ApplyResources(this.m_ResolutionLabel, "m_ResolutionLabel");
			this.m_ResolutionLabel.Name = "m_ResolutionLabel";
			componentResourceManager.ApplyResources(this.m_LargeResolution, "m_LargeResolution");
			this.m_LargeResolution.ForeColor = global::System.Drawing.SystemColors.WindowText;
			this.m_LargeResolution.Name = "m_LargeResolution";
			componentResourceManager.ApplyResources(this.m_SmallResolution, "m_SmallResolution");
			this.m_SmallResolution.ForeColor = global::System.Drawing.SystemColors.WindowText;
			this.m_SmallResolution.Name = "m_SmallResolution";
			this.m_DisplayResolution.AllowDrop = true;
			this.m_DisplayResolution.BackColor = global::System.Drawing.SystemColors.Control;
			this.m_DisplayResolution.LargeChange = 2;
			componentResourceManager.ApplyResources(this.m_DisplayResolution, "m_DisplayResolution");
			this.m_DisplayResolution.Maximum = 15;
			this.m_DisplayResolution.Minimum = 1;
			this.m_DisplayResolution.Name = "m_DisplayResolution";
			this.m_DisplayResolution.Value = 6;
			this.m_DisplayResolution.Scroll += new global::System.EventHandler(this.DisplayResolution_Scroll);
			componentResourceManager.ApplyResources(this.m_ChooseResolutionLabel, "m_ChooseResolutionLabel");
			this.m_ChooseResolutionLabel.ForeColor = global::System.Drawing.SystemColors.WindowText;
			this.m_ChooseResolutionLabel.Name = "m_ChooseResolutionLabel";
			this.m_DisplayIcon.BackColor = global::System.Drawing.Color.Transparent;
			componentResourceManager.ApplyResources(this.m_DisplayIcon, "m_DisplayIcon");
			this.m_DisplayIcon.Name = "m_DisplayIcon";
			this.m_DisplayIcon.TabStop = false;
			this.m_Options.Controls.Add(this.m_GeneralOption);
			this.m_Options.Controls.Add(this.m_DriveTab);
			componentResourceManager.ApplyResources(this.m_Options, "m_Options");
			this.m_Options.Name = "m_Options";
			this.m_Options.SelectedIndex = 0;
			this.m_GeneralOption.BackColor = global::System.Drawing.SystemColors.Window;
			this.m_GeneralOption.Controls.Add(this.m_SaveSwitch);
			componentResourceManager.ApplyResources(this.m_GeneralOption, "m_GeneralOption");
			this.m_GeneralOption.Name = "m_GeneralOption";
			componentResourceManager.ApplyResources(this.m_SaveSwitch, "m_SaveSwitch");
			this.m_SaveSwitch.ForeColor = global::System.Drawing.SystemColors.WindowText;
			this.m_SaveSwitch.Name = "m_SaveSwitch";
			this.m_SaveSwitch.UseVisualStyleBackColor = true;
			this.m_DriveTab.BackColor = global::System.Drawing.SystemColors.Window;
			this.m_DriveTab.Controls.Add(this.m_RemoteAudio);
			this.m_DriveTab.Controls.Add(this.m_DriveList);
			componentResourceManager.ApplyResources(this.m_DriveTab, "m_DriveTab");
			this.m_DriveTab.Name = "m_DriveTab";
			this.m_RemoteAudio.Controls.Add(this.m_AudioIcon);
			this.m_RemoteAudio.Controls.Add(this.m_AudioSettings);
			this.m_RemoteAudio.Controls.Add(this.m_AudioLabel);
			this.m_RemoteAudio.ForeColor = global::System.Drawing.SystemColors.WindowText;
			componentResourceManager.ApplyResources(this.m_RemoteAudio, "m_RemoteAudio");
			this.m_RemoteAudio.Name = "m_RemoteAudio";
			this.m_RemoteAudio.TabStop = false;
			componentResourceManager.ApplyResources(this.m_AudioIcon, "m_AudioIcon");
			this.m_AudioIcon.Name = "m_AudioIcon";
			this.m_AudioIcon.TabStop = false;
			this.m_AudioSettings.ForeColor = global::System.Drawing.SystemColors.ControlText;
			componentResourceManager.ApplyResources(this.m_AudioSettings, "m_AudioSettings");
			this.m_AudioSettings.Name = "m_AudioSettings";
			this.m_AudioSettings.UseVisualStyleBackColor = true;
			this.m_AudioSettings.Click += new global::System.EventHandler(this.m_AudioSettings_Click);
			componentResourceManager.ApplyResources(this.m_AudioLabel, "m_AudioLabel");
			this.m_AudioLabel.ForeColor = global::System.Drawing.SystemColors.WindowText;
			this.m_AudioLabel.Name = "m_AudioLabel";
			this.m_DriveList.Controls.Add(this.pictureBox1);
			this.m_DriveList.Controls.Add(this.m_Clipboard);
			this.m_DriveList.Controls.Add(this.m_Printer);
			this.m_DriveList.Controls.Add(this.m_ChooseDevices);
			this.m_DriveList.Controls.Add(this.m_MoreDevices);
			this.m_DriveList.ForeColor = global::System.Drawing.SystemColors.WindowText;
			componentResourceManager.ApplyResources(this.m_DriveList, "m_DriveList");
			this.m_DriveList.Name = "m_DriveList";
			this.m_DriveList.TabStop = false;
			componentResourceManager.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			componentResourceManager.ApplyResources(this.m_Clipboard, "m_Clipboard");
			this.m_Clipboard.Name = "m_Clipboard";
			this.m_Clipboard.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.m_Printer, "m_Printer");
			this.m_Printer.Name = "m_Printer";
			this.m_Printer.UseVisualStyleBackColor = true;
			this.m_ChooseDevices.ForeColor = global::System.Drawing.SystemColors.WindowText;
			componentResourceManager.ApplyResources(this.m_ChooseDevices, "m_ChooseDevices");
			this.m_ChooseDevices.Name = "m_ChooseDevices";
			this.m_MoreDevices.ForeColor = global::System.Drawing.SystemColors.ControlText;
			componentResourceManager.ApplyResources(this.m_MoreDevices, "m_MoreDevices");
			this.m_MoreDevices.Name = "m_MoreDevices";
			this.m_MoreDevices.UseVisualStyleBackColor = true;
			this.m_MoreDevices.Click += new global::System.EventHandler(this.m_MoreDevices_Click);
			this.m_ShowOption.AutoSize = true;
			componentResourceManager.ApplyResources(this.m_ShowOption, "m_ShowOption");
			this.m_ShowOption.BackColor = global::System.Drawing.SystemColors.Control;
			this.m_ShowOption.FlatAppearance.BorderColor = global::System.Drawing.SystemColors.Control;
			this.m_ShowOption.FlatAppearance.BorderSize = 0;
			this.m_ShowOption.FlatAppearance.MouseDownBackColor = global::System.Drawing.SystemColors.GradientActiveCaption;
			this.m_ShowOption.FlatAppearance.MouseOverBackColor = global::System.Drawing.SystemColors.GradientInactiveCaption;
			this.m_ShowOption.Name = "m_ShowOption";
			this.m_ShowOption.UseVisualStyleBackColor = true;
			this.m_ShowOption.Click += new global::System.EventHandler(this.ShowOption_Click);
			componentResourceManager.ApplyResources(this.m_HelpButton, "m_HelpButton");
			this.m_HelpButton.Name = "m_HelpButton";
			this.m_HelpButton.UseVisualStyleBackColor = true;
			this.m_HelpButton.Click += new global::System.EventHandler(this.m_HelpButton_Click);
			componentResourceManager.ApplyResources(this.m_ConnectButton, "m_ConnectButton");
			this.m_ConnectButton.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.m_ConnectButton.Name = "m_ConnectButton";
			this.m_ConnectButton.UseVisualStyleBackColor = true;
			this.m_ConnectButton.Click += new global::System.EventHandler(this.m_ConnectButton_Click);
			componentResourceManager.ApplyResources(this.m_ButtonsFlowLayoutPanel, "m_ButtonsFlowLayoutPanel");
			this.m_ButtonsFlowLayoutPanel.Controls.Add(this.m_HelpButton);
			this.m_ButtonsFlowLayoutPanel.Controls.Add(this.m_ShowOption);
			this.m_ButtonsFlowLayoutPanel.Controls.Add(this.m_ConnectButton);
			this.m_ButtonsFlowLayoutPanel.Name = "m_ButtonsFlowLayoutPanel";
			base.AcceptButton = this.m_ConnectButton;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.m_ButtonsFlowLayoutPanel);
			base.Controls.Add(this.m_Options);
			base.Controls.Add(this.m_DisplayConfiguration);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "EnhancedConnectionDialog";
			this.m_DisplayConfiguration.ResumeLayout(false);
			this.m_DisplayConfiguration.PerformLayout();
			((global::System.ComponentModel.ISupportInitialize)this.m_DisplayResolution).EndInit();
			((global::System.ComponentModel.ISupportInitialize)this.m_DisplayIcon).EndInit();
			this.m_Options.ResumeLayout(false);
			this.m_GeneralOption.ResumeLayout(false);
			this.m_DriveTab.ResumeLayout(false);
			this.m_RemoteAudio.ResumeLayout(false);
			this.m_RemoteAudio.PerformLayout();
			((global::System.ComponentModel.ISupportInitialize)this.m_AudioIcon).EndInit();
			this.m_DriveList.ResumeLayout(false);
			this.m_DriveList.PerformLayout();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
			this.m_ButtonsFlowLayoutPanel.ResumeLayout(false);
			this.m_ButtonsFlowLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x04000062 RID: 98
		private global::System.ComponentModel.IContainer components;

		// Token: 0x04000063 RID: 99
		private global::System.Windows.Forms.GroupBox m_DisplayConfiguration;

		// Token: 0x04000064 RID: 100
		private global::System.Windows.Forms.Label m_ChooseResolutionLabel;

		// Token: 0x04000065 RID: 101
		private global::System.Windows.Forms.PictureBox m_DisplayIcon;

		// Token: 0x04000066 RID: 102
		private global::System.Windows.Forms.CheckBox m_UseAllMonitorBox;

		// Token: 0x04000067 RID: 103
		private global::System.Windows.Forms.Label m_ResolutionLabel;

		// Token: 0x04000068 RID: 104
		private global::System.Windows.Forms.Label m_LargeResolution;

		// Token: 0x04000069 RID: 105
		private global::System.Windows.Forms.Label m_SmallResolution;

		// Token: 0x0400006A RID: 106
		private global::System.Windows.Forms.TrackBar m_DisplayResolution;

		// Token: 0x0400006B RID: 107
		private global::System.Windows.Forms.TabControl m_Options;

		// Token: 0x0400006C RID: 108
		private global::System.Windows.Forms.Button m_ShowOption;

		// Token: 0x0400006F RID: 111
		private global::System.Windows.Forms.TabPage m_DriveTab;

		// Token: 0x04000070 RID: 112
		private global::System.Windows.Forms.GroupBox m_DriveList;

		// Token: 0x04000071 RID: 113
		private global::System.Windows.Forms.Button m_MoreDevices;

		// Token: 0x04000072 RID: 114
		private global::System.Windows.Forms.Label m_ChooseDevices;

		// Token: 0x04000073 RID: 115
		private global::System.Windows.Forms.Button m_HelpButton;

		// Token: 0x04000074 RID: 116
		private global::System.Windows.Forms.Button m_ConnectButton;

		// Token: 0x04000075 RID: 117
		private global::System.Windows.Forms.GroupBox m_RemoteAudio;

		// Token: 0x04000076 RID: 118
		private global::System.Windows.Forms.CheckBox m_Clipboard;

		// Token: 0x04000077 RID: 119
		private global::System.Windows.Forms.CheckBox m_Printer;

		// Token: 0x04000078 RID: 120
		private global::System.Windows.Forms.Label m_AudioLabel;

		// Token: 0x04000079 RID: 121
		private global::System.Windows.Forms.Button m_AudioSettings;

		// Token: 0x0400007A RID: 122
		private global::System.Windows.Forms.CheckBox m_SaveSwitch;

		// Token: 0x0400007B RID: 123
		private global::System.Windows.Forms.PictureBox m_AudioIcon;

		// Token: 0x0400007C RID: 124
		private global::System.Windows.Forms.PictureBox pictureBox1;

		// Token: 0x0400007D RID: 125
		private global::System.Windows.Forms.TabPage m_GeneralOption;

		// Token: 0x0400007E RID: 126
		private global::System.Windows.Forms.Panel m_ButtonsFlowLayoutPanel;
	}
}
