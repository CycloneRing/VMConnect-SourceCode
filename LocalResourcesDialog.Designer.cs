namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x0200000C RID: 12
	internal partial class LocalResourcesDialog : global::System.Windows.Forms.Form
	{
		// Token: 0x060000CC RID: 204 RVA: 0x00009AE0 File Offset: 0x00007CE0
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00009B00 File Offset: 0x00007D00
		private void InitializeComponent()
		{
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::Microsoft.Virtualization.Client.InteractiveSession.LocalResourcesDialog));
			this.m_DriveList = new global::System.Windows.Forms.GroupBox();
			this.m_ResourceList = new global::System.Windows.Forms.TreeView();
			this.m_ChooseDevices = new global::System.Windows.Forms.Label();
			this.m_ButtonsFlowLayoutPanel = new global::System.Windows.Forms.FlowLayoutPanel();
			this.m_CancelButton = new global::System.Windows.Forms.Button();
			this.m_OkButton = new global::System.Windows.Forms.Button();
			this.m_DriveList.SuspendLayout();
			this.m_ButtonsFlowLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.m_DriveList.Controls.Add(this.m_ResourceList);
			this.m_DriveList.Controls.Add(this.m_ChooseDevices);
			this.m_DriveList.ForeColor = global::System.Drawing.SystemColors.WindowText;
			componentResourceManager.ApplyResources(this.m_DriveList, "m_DriveList");
			this.m_DriveList.Name = "m_DriveList";
			this.m_DriveList.TabStop = false;
			this.m_ResourceList.CheckBoxes = true;
			componentResourceManager.ApplyResources(this.m_ResourceList, "m_ResourceList");
			this.m_ResourceList.Name = "m_ResourceList";
			this.m_ResourceList.ShowLines = false;
			componentResourceManager.ApplyResources(this.m_ChooseDevices, "m_ChooseDevices");
			this.m_ChooseDevices.ForeColor = global::System.Drawing.SystemColors.WindowText;
			this.m_ChooseDevices.Name = "m_ChooseDevices";
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
			base.AcceptButton = this.m_OkButton;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.m_CancelButton;
			base.Controls.Add(this.m_ButtonsFlowLayoutPanel);
			base.Controls.Add(this.m_DriveList);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "LocalResourcesDialog";
			this.m_DriveList.ResumeLayout(false);
			this.m_ButtonsFlowLayoutPanel.ResumeLayout(false);
			this.m_ButtonsFlowLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x0400009D RID: 157
		private global::System.ComponentModel.IContainer components;

		// Token: 0x0400009E RID: 158
		private global::System.Windows.Forms.GroupBox m_DriveList;

		// Token: 0x0400009F RID: 159
		private global::System.Windows.Forms.Label m_ChooseDevices;

		// Token: 0x040000A0 RID: 160
		private global::System.Windows.Forms.TreeView m_ResourceList;

		// Token: 0x040000A1 RID: 161
		private global::System.Windows.Forms.FlowLayoutPanel m_ButtonsFlowLayoutPanel;

		// Token: 0x040000A2 RID: 162
		private global::System.Windows.Forms.Button m_CancelButton;

		// Token: 0x040000A3 RID: 163
		private global::System.Windows.Forms.Button m_OkButton;
	}
}
