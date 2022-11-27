namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000005 RID: 5
	internal partial class AudioSettingDialog : global::System.Windows.Forms.Form
	{
		// Token: 0x06000020 RID: 32 RVA: 0x000029A4 File Offset: 0x00000BA4
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000029C4 File Offset: 0x00000BC4
		private void InitializeComponent()
		{
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::Microsoft.Virtualization.Client.InteractiveSession.AudioSettingDialog));
			this.m_ButtonsFlowLayoutPanel = new global::System.Windows.Forms.FlowLayoutPanel();
			this.m_CancelButton = new global::System.Windows.Forms.Button();
			this.m_OkButton = new global::System.Windows.Forms.Button();
			this.m_RemoteAudioPlayback = new global::System.Windows.Forms.GroupBox();
			this.m_PlayOnRemoteComputer = new global::System.Windows.Forms.RadioButton();
			this.m_DoNotPlay = new global::System.Windows.Forms.RadioButton();
			this.m_PlayOnThisComputer = new global::System.Windows.Forms.RadioButton();
			this.m_AudioIcon = new global::System.Windows.Forms.PictureBox();
			this.m_RemoteAudioRecording = new global::System.Windows.Forms.GroupBox();
			this.m_DoNotRecord = new global::System.Windows.Forms.RadioButton();
			this.m_RecordFromThisComputer = new global::System.Windows.Forms.RadioButton();
			this.m_MicIcon = new global::System.Windows.Forms.PictureBox();
			this.m_ButtonsFlowLayoutPanel.SuspendLayout();
			this.m_RemoteAudioPlayback.SuspendLayout();
			((global::System.ComponentModel.ISupportInitialize)this.m_AudioIcon).BeginInit();
			this.m_RemoteAudioRecording.SuspendLayout();
			((global::System.ComponentModel.ISupportInitialize)this.m_MicIcon).BeginInit();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.m_ButtonsFlowLayoutPanel, "m_ButtonsFlowLayoutPanel");
			this.m_ButtonsFlowLayoutPanel.Controls.Add(this.m_CancelButton);
			this.m_ButtonsFlowLayoutPanel.Controls.Add(this.m_OkButton);
			this.m_ButtonsFlowLayoutPanel.Name = "m_ButtonsFlowLayoutPanel";
			componentResourceManager.ApplyResources(this.m_CancelButton, "m_CancelButton");
			this.m_CancelButton.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.m_CancelButton.Name = "m_CancelButton";
			this.m_CancelButton.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.m_OkButton, "m_OkButton");
			this.m_OkButton.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.m_OkButton.Name = "m_OkButton";
			this.m_OkButton.UseVisualStyleBackColor = true;
			this.m_OkButton.Click += new global::System.EventHandler(this.m_OkButton_Click);
			this.m_RemoteAudioPlayback.Controls.Add(this.m_PlayOnRemoteComputer);
			this.m_RemoteAudioPlayback.Controls.Add(this.m_DoNotPlay);
			this.m_RemoteAudioPlayback.Controls.Add(this.m_PlayOnThisComputer);
			this.m_RemoteAudioPlayback.Controls.Add(this.m_AudioIcon);
			this.m_RemoteAudioPlayback.ForeColor = global::System.Drawing.SystemColors.WindowText;
			componentResourceManager.ApplyResources(this.m_RemoteAudioPlayback, "m_RemoteAudioPlayback");
			this.m_RemoteAudioPlayback.Name = "m_RemoteAudioPlayback";
			this.m_RemoteAudioPlayback.TabStop = false;
			componentResourceManager.ApplyResources(this.m_PlayOnRemoteComputer, "m_PlayOnRemoteComputer");
			this.m_PlayOnRemoteComputer.Name = "m_PlayOnRemoteComputer";
			this.m_PlayOnRemoteComputer.UseVisualStyleBackColor = true;
			this.m_PlayOnRemoteComputer.CheckedChanged += new global::System.EventHandler(this.m_PlayOnRemoteComputer_CheckedChanged);
			componentResourceManager.ApplyResources(this.m_DoNotPlay, "m_DoNotPlay");
			this.m_DoNotPlay.Name = "m_DoNotPlay";
			this.m_DoNotPlay.UseVisualStyleBackColor = true;
			this.m_DoNotPlay.CheckedChanged += new global::System.EventHandler(this.m_DoNotPlay_CheckedChanged);
			componentResourceManager.ApplyResources(this.m_PlayOnThisComputer, "m_PlayOnThisComputer");
			this.m_PlayOnThisComputer.Checked = true;
			this.m_PlayOnThisComputer.Name = "m_PlayOnThisComputer";
			this.m_PlayOnThisComputer.TabStop = true;
			this.m_PlayOnThisComputer.UseVisualStyleBackColor = true;
			this.m_PlayOnThisComputer.CheckedChanged += new global::System.EventHandler(this.m_PlayOnThisComputer_CheckedChanged);
			componentResourceManager.ApplyResources(this.m_AudioIcon, "m_AudioIcon");
			this.m_AudioIcon.Name = "m_AudioIcon";
			this.m_AudioIcon.TabStop = false;
			this.m_RemoteAudioRecording.Controls.Add(this.m_DoNotRecord);
			this.m_RemoteAudioRecording.Controls.Add(this.m_RecordFromThisComputer);
			this.m_RemoteAudioRecording.Controls.Add(this.m_MicIcon);
			this.m_RemoteAudioRecording.ForeColor = global::System.Drawing.SystemColors.WindowText;
			componentResourceManager.ApplyResources(this.m_RemoteAudioRecording, "m_RemoteAudioRecording");
			this.m_RemoteAudioRecording.Name = "m_RemoteAudioRecording";
			this.m_RemoteAudioRecording.TabStop = false;
			componentResourceManager.ApplyResources(this.m_DoNotRecord, "m_DoNotRecord");
			this.m_DoNotRecord.Name = "m_DoNotRecord";
			this.m_DoNotRecord.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.m_RecordFromThisComputer, "m_RecordFromThisComputer");
			this.m_RecordFromThisComputer.Checked = true;
			this.m_RecordFromThisComputer.Name = "m_RecordFromThisComputer";
			this.m_RecordFromThisComputer.TabStop = true;
			this.m_RecordFromThisComputer.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.m_MicIcon, "m_MicIcon");
			this.m_MicIcon.Name = "m_MicIcon";
			this.m_MicIcon.TabStop = false;
			base.AcceptButton = this.m_OkButton;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.m_CancelButton;
			base.Controls.Add(this.m_RemoteAudioRecording);
			base.Controls.Add(this.m_RemoteAudioPlayback);
			base.Controls.Add(this.m_ButtonsFlowLayoutPanel);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AudioSettingDialog";
			this.m_ButtonsFlowLayoutPanel.ResumeLayout(false);
			this.m_ButtonsFlowLayoutPanel.PerformLayout();
			this.m_RemoteAudioPlayback.ResumeLayout(false);
			this.m_RemoteAudioPlayback.PerformLayout();
			((global::System.ComponentModel.ISupportInitialize)this.m_AudioIcon).EndInit();
			this.m_RemoteAudioRecording.ResumeLayout(false);
			this.m_RemoteAudioRecording.PerformLayout();
			((global::System.ComponentModel.ISupportInitialize)this.m_MicIcon).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x0400000C RID: 12
		private global::System.ComponentModel.IContainer components;

		// Token: 0x0400000D RID: 13
		private global::System.Windows.Forms.FlowLayoutPanel m_ButtonsFlowLayoutPanel;

		// Token: 0x0400000E RID: 14
		private global::System.Windows.Forms.Button m_CancelButton;

		// Token: 0x0400000F RID: 15
		private global::System.Windows.Forms.Button m_OkButton;

		// Token: 0x04000010 RID: 16
		private global::System.Windows.Forms.GroupBox m_RemoteAudioPlayback;

		// Token: 0x04000011 RID: 17
		private global::System.Windows.Forms.GroupBox m_RemoteAudioRecording;

		// Token: 0x04000012 RID: 18
		private global::System.Windows.Forms.PictureBox m_AudioIcon;

		// Token: 0x04000013 RID: 19
		private global::System.Windows.Forms.RadioButton m_PlayOnRemoteComputer;

		// Token: 0x04000014 RID: 20
		private global::System.Windows.Forms.RadioButton m_DoNotPlay;

		// Token: 0x04000015 RID: 21
		private global::System.Windows.Forms.RadioButton m_PlayOnThisComputer;

		// Token: 0x04000016 RID: 22
		private global::System.Windows.Forms.PictureBox m_MicIcon;

		// Token: 0x04000017 RID: 23
		private global::System.Windows.Forms.RadioButton m_RecordFromThisComputer;

		// Token: 0x04000018 RID: 24
		private global::System.Windows.Forms.RadioButton m_DoNotRecord;
	}
}
