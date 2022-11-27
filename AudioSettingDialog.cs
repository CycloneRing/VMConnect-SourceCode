using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000005 RID: 5
	internal partial class AudioSettingDialog : Form
	{
		// Token: 0x0600001A RID: 26 RVA: 0x00002824 File Offset: 0x00000A24
		public AudioSettingDialog(RdpOptions options)
		{
			this.InitializeComponent();
			this.m_RdpOptions = options;
			if (this.m_RdpOptions.AudioPlaybackRedirectionMode == RdpOptions.AudioPlaybackRedirectionType.AUDIO_MODE_REDIRECT)
			{
				this.m_PlayOnThisComputer.Checked = true;
			}
			else if (this.m_RdpOptions.AudioPlaybackRedirectionMode == RdpOptions.AudioPlaybackRedirectionType.AUDIO_MODE_PLAY_ON_SERVER)
			{
				this.m_PlayOnRemoteComputer.Checked = true;
			}
			else
			{
				this.m_DoNotPlay.Checked = true;
			}
			this.m_RecordFromThisComputer.Checked = this.m_RdpOptions.AudioCaptureRedirectionMode;
			this.m_DoNotRecord.Checked = !this.m_RecordFromThisComputer.Checked;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000028B8 File Offset: 0x00000AB8
		private void SaveAll()
		{
			this.m_RdpOptions.AudioCaptureRedirectionMode = this.m_RecordFromThisComputer.Checked;
			if (this.m_PlayOnThisComputer.Checked)
			{
				this.m_RdpOptions.AudioPlaybackRedirectionMode = RdpOptions.AudioPlaybackRedirectionType.AUDIO_MODE_REDIRECT;
				return;
			}
			if (this.m_PlayOnRemoteComputer.Checked)
			{
				this.m_RdpOptions.AudioPlaybackRedirectionMode = RdpOptions.AudioPlaybackRedirectionType.AUDIO_MODE_PLAY_ON_SERVER;
				this.m_RdpOptions.AudioCaptureRedirectionMode = false;
				return;
			}
			this.m_RdpOptions.AudioPlaybackRedirectionMode = RdpOptions.AudioPlaybackRedirectionType.AUDIO_MODE_NONE;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002927 File Offset: 0x00000B27
		private void m_OkButton_Click(object sender, EventArgs e)
		{
			this.SaveAll();
		}

		// Token: 0x0600001D RID: 29 RVA: 0x0000292F File Offset: 0x00000B2F
		private void m_PlayOnRemoteComputer_CheckedChanged(object sender, EventArgs e)
		{
			if (this.m_PlayOnRemoteComputer.Checked)
			{
				this.m_RecordFromThisComputer.Enabled = false;
				this.m_DoNotRecord.Enabled = false;
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002956 File Offset: 0x00000B56
		private void m_PlayOnThisComputer_CheckedChanged(object sender, EventArgs e)
		{
			if (this.m_PlayOnThisComputer.Checked)
			{
				this.m_RecordFromThisComputer.Enabled = true;
				this.m_DoNotRecord.Enabled = true;
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x0000297D File Offset: 0x00000B7D
		private void m_DoNotPlay_CheckedChanged(object sender, EventArgs e)
		{
			if (this.m_DoNotPlay.Checked)
			{
				this.m_RecordFromThisComputer.Enabled = true;
				this.m_DoNotRecord.Enabled = true;
			}
		}

		// Token: 0x0400000B RID: 11
		private RdpOptions m_RdpOptions;
	}
}
