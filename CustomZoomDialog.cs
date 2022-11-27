using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000007 RID: 7
	public partial class CustomZoomDialog : Form
	{
		// Token: 0x06000046 RID: 70 RVA: 0x00004D3C File Offset: 0x00002F3C
		public CustomZoomDialog()
		{
			this.InitializeComponent();
			for (Control nextControl = this.radioButton1; nextControl != null; nextControl = base.GetNextControl(nextControl, true))
			{
				RadioButton radioButton = nextControl as RadioButton;
				if (radioButton != null)
				{
					this.radioButtons.Add(radioButton);
				}
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000047 RID: 71 RVA: 0x00004D8B File Offset: 0x00002F8B
		// (set) Token: 0x06000048 RID: 72 RVA: 0x00004D93 File Offset: 0x00002F93
		public uint DisplayZoom { get; set; }

		// Token: 0x06000049 RID: 73 RVA: 0x00004D9C File Offset: 0x00002F9C
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			this.InitControls();
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00004DAB File Offset: 0x00002FAB
		private void InitControls()
		{
			this.customUpDown.Value = Math.Min(this.DisplayZoom, (uint)this.customUpDown.Maximum);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00004DD8 File Offset: 0x00002FD8
		private void CheckCorrectRadioButton()
		{
			int num = (int)this.customUpDown.Value;
			for (int i = 0; i < this.radioButtons.Count; i++)
			{
				bool @checked = num == CustomZoomDialog.DefaultZoomValues[i];
				this.radioButtons[i].Checked = @checked;
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00004E29 File Offset: 0x00003029
		private void RadioButtonCheckedChanged(object sender, EventArgs e)
		{
			if (((RadioButton)sender).Checked)
			{
				this.customUpDown.Value = CustomZoomDialog.DefaultZoomValues[this.radioButtons.IndexOf((RadioButton)sender)];
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00004E5F File Offset: 0x0000305F
		private void ButtonOK_Click(object sender, EventArgs e)
		{
			this.DisplayZoom = (uint)this.customUpDown.Value;
			base.DialogResult = DialogResult.OK;
			base.Close();
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004E84 File Offset: 0x00003084
		private void CustomUpDown_ValueChanged(object sender, EventArgs e)
		{
			this.CheckCorrectRadioButton();
		}

		// Token: 0x0400003E RID: 62
		private static readonly int[] DefaultZoomValues = new int[]
		{
			25,
			50,
			75,
			100,
			125,
			150,
			200
		};

		// Token: 0x0400003F RID: 63
		private List<RadioButton> radioButtons = new List<RadioButton>();
	}
}
