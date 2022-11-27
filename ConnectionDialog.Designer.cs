namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000006 RID: 6
	internal partial class ConnectionDialog : global::System.Windows.Forms.Form
	{
		// Token: 0x06000023 RID: 35 RVA: 0x00002FE2 File Offset: 0x000011E2
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			if (disposing)
			{
				this.m_ProgressAnimationTimer.Stop();
				this.m_ProgressAnimationTimer.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00004420 File Offset: 0x00002620
		private void InitializeComponent()
		{
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::Microsoft.Virtualization.Client.InteractiveSession.ConnectionDialog));
			this.m_HeaderTableLayoutPanel = new global::System.Windows.Forms.TableLayoutPanel();
			this.m_RightPanel = new global::System.Windows.Forms.Panel();
			this.m_TitleLabel1 = new global::System.Windows.Forms.Label();
			this.m_TitleLabel2 = new global::System.Windows.Forms.Label();
			this.m_LeftPanel = new global::System.Windows.Forms.Panel();
			this.m_ProgressPanel = new global::System.Windows.Forms.Panel();
			this.m_ProgressPanelLeft = new global::System.Windows.Forms.Panel();
			this.m_ProgressPanelRight = new global::System.Windows.Forms.Panel();
			this.m_ContentTableLayoutPanel = new global::System.Windows.Forms.TableLayoutPanel();
			this.m_MachineComboBox = new global::System.Windows.Forms.ComboBox();
			this.m_ServerComboBox = new global::System.Windows.Forms.ComboBox();
			this.m_MachineLabel = new global::System.Windows.Forms.Label();
			this.m_ServerLabel = new global::System.Windows.Forms.Label();
			this.m_MessageTableLayoutPanel = new global::System.Windows.Forms.TableLayoutPanel();
			this.m_WarningPictureBox = new global::System.Windows.Forms.PictureBox();
			this.m_MessageLabel = new global::System.Windows.Forms.Label();
			this.m_ButtonsFlowLayoutPanel = new global::System.Windows.Forms.FlowLayoutPanel();
			this.m_CancelButton = new global::System.Windows.Forms.Button();
			this.m_OkButton = new global::System.Windows.Forms.Button();
			this.m_SetUserButton = new global::System.Windows.Forms.Button();
			this.m_ConnectAsCheckbox = new global::System.Windows.Forms.CheckBox();
			this.m_HeaderTableLayoutPanel.SuspendLayout();
			this.m_RightPanel.SuspendLayout();
			this.m_ProgressPanel.SuspendLayout();
			this.m_ContentTableLayoutPanel.SuspendLayout();
			this.m_MessageTableLayoutPanel.SuspendLayout();
			((global::System.ComponentModel.ISupportInitialize)this.m_WarningPictureBox).BeginInit();
			this.m_ButtonsFlowLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.m_HeaderTableLayoutPanel, "m_HeaderTableLayoutPanel");
			this.m_HeaderTableLayoutPanel.Controls.Add(this.m_RightPanel, 1, 0);
			this.m_HeaderTableLayoutPanel.Controls.Add(this.m_LeftPanel, 0, 0);
			this.m_HeaderTableLayoutPanel.Name = "m_HeaderTableLayoutPanel";
			this.m_RightPanel.BackgroundImage = global::Microsoft.Virtualization.Client.InteractiveSession.Resources.ConnectionResources.PanelRight96;
			componentResourceManager.ApplyResources(this.m_RightPanel, "m_RightPanel");
			this.m_RightPanel.Controls.Add(this.m_TitleLabel1);
			this.m_RightPanel.Controls.Add(this.m_TitleLabel2);
			this.m_RightPanel.Name = "m_RightPanel";
			componentResourceManager.ApplyResources(this.m_TitleLabel1, "m_TitleLabel1");
			this.m_TitleLabel1.BackColor = global::System.Drawing.Color.Transparent;
			this.m_TitleLabel1.ForeColor = global::System.Drawing.Color.White;
			this.m_TitleLabel1.Name = "m_TitleLabel1";
			componentResourceManager.ApplyResources(this.m_TitleLabel2, "m_TitleLabel2");
			this.m_TitleLabel2.BackColor = global::System.Drawing.Color.Transparent;
			this.m_TitleLabel2.ForeColor = global::System.Drawing.Color.White;
			this.m_TitleLabel2.Name = "m_TitleLabel2";
			this.m_LeftPanel.BackgroundImage = global::Microsoft.Virtualization.Client.InteractiveSession.Resources.ConnectionResources.PanelLeft96;
			componentResourceManager.ApplyResources(this.m_LeftPanel, "m_LeftPanel");
			this.m_LeftPanel.Name = "m_LeftPanel";
			this.m_ProgressPanel.Controls.Add(this.m_ProgressPanelLeft);
			this.m_ProgressPanel.Controls.Add(this.m_ProgressPanelRight);
			componentResourceManager.ApplyResources(this.m_ProgressPanel, "m_ProgressPanel");
			this.m_ProgressPanel.Name = "m_ProgressPanel";
			this.m_ProgressPanelLeft.BackgroundImage = global::Microsoft.Virtualization.Client.InteractiveSession.Resources.ConnectionResources.ProgressBar96;
			componentResourceManager.ApplyResources(this.m_ProgressPanelLeft, "m_ProgressPanelLeft");
			this.m_ProgressPanelLeft.Name = "m_ProgressPanelLeft";
			this.m_ProgressPanelRight.BackgroundImage = global::Microsoft.Virtualization.Client.InteractiveSession.Resources.ConnectionResources.ProgressBar96;
			componentResourceManager.ApplyResources(this.m_ProgressPanelRight, "m_ProgressPanelRight");
			this.m_ProgressPanelRight.Name = "m_ProgressPanelRight";
			componentResourceManager.ApplyResources(this.m_ContentTableLayoutPanel, "m_ContentTableLayoutPanel");
			this.m_ContentTableLayoutPanel.Controls.Add(this.m_MachineComboBox, 1, 1);
			this.m_ContentTableLayoutPanel.Controls.Add(this.m_ServerComboBox, 1, 0);
			this.m_ContentTableLayoutPanel.Controls.Add(this.m_MachineLabel, 0, 1);
			this.m_ContentTableLayoutPanel.Controls.Add(this.m_ServerLabel, 0, 0);
			this.m_ContentTableLayoutPanel.Name = "m_ContentTableLayoutPanel";
			this.m_MachineComboBox.AutoCompleteMode = global::System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.m_MachineComboBox.AutoCompleteSource = global::System.Windows.Forms.AutoCompleteSource.ListItems;
			this.m_MachineComboBox.FormattingEnabled = true;
			componentResourceManager.ApplyResources(this.m_MachineComboBox, "m_MachineComboBox");
			this.m_MachineComboBox.Name = "m_MachineComboBox";
			this.m_MachineComboBox.DropDown += new global::System.EventHandler(this.OnMachineComboBoxDropDown);
			this.m_MachineComboBox.SelectedIndexChanged += new global::System.EventHandler(this.OnMachineComboBoxSelectedIndexChanged);
			this.m_MachineComboBox.DropDownClosed += new global::System.EventHandler(this.OnMachineComboBoxDropDownClosed);
			this.m_MachineComboBox.TextChanged += new global::System.EventHandler(this.OnMachineComboBoxTextChanged);
			this.m_MachineComboBox.Enter += new global::System.EventHandler(this.OnMachineComboBoxEnter);
			this.m_ServerComboBox.AutoCompleteMode = global::System.Windows.Forms.AutoCompleteMode.Append;
			this.m_ServerComboBox.AutoCompleteSource = global::System.Windows.Forms.AutoCompleteSource.CustomSource;
			this.m_ServerComboBox.FormattingEnabled = true;
			componentResourceManager.ApplyResources(this.m_ServerComboBox, "m_ServerComboBox");
			this.m_ServerComboBox.Name = "m_ServerComboBox";
			this.m_ServerComboBox.SelectedIndexChanged += new global::System.EventHandler(this.OnServerNameSelectedIndexChanged);
			this.m_ServerComboBox.DropDownClosed += new global::System.EventHandler(this.OnServerComboBoxDropDownClosed);
			this.m_ServerComboBox.TextChanged += new global::System.EventHandler(this.OnServerComboBoxTextChanged);
			this.m_ServerComboBox.Leave += new global::System.EventHandler(this.OnServerComboBoxLeave);
			componentResourceManager.ApplyResources(this.m_MachineLabel, "m_MachineLabel");
			this.m_MachineLabel.Name = "m_MachineLabel";
			componentResourceManager.ApplyResources(this.m_ServerLabel, "m_ServerLabel");
			this.m_ServerLabel.Name = "m_ServerLabel";
			componentResourceManager.ApplyResources(this.m_MessageTableLayoutPanel, "m_MessageTableLayoutPanel");
			this.m_MessageTableLayoutPanel.BackColor = global::System.Drawing.SystemColors.Control;
			this.m_MessageTableLayoutPanel.Controls.Add(this.m_WarningPictureBox, 0, 0);
			this.m_MessageTableLayoutPanel.Controls.Add(this.m_MessageLabel, 1, 0);
			this.m_MessageTableLayoutPanel.Name = "m_MessageTableLayoutPanel";
			componentResourceManager.ApplyResources(this.m_WarningPictureBox, "m_WarningPictureBox");
			this.m_WarningPictureBox.Name = "m_WarningPictureBox";
			this.m_WarningPictureBox.TabStop = false;
			componentResourceManager.ApplyResources(this.m_MessageLabel, "m_MessageLabel");
			this.m_MessageLabel.BackColor = global::System.Drawing.SystemColors.Control;
			this.m_MessageLabel.Name = "m_MessageLabel";
			componentResourceManager.ApplyResources(this.m_ButtonsFlowLayoutPanel, "m_ButtonsFlowLayoutPanel");
			this.m_ButtonsFlowLayoutPanel.Controls.Add(this.m_CancelButton);
			this.m_ButtonsFlowLayoutPanel.Controls.Add(this.m_OkButton);
			this.m_ButtonsFlowLayoutPanel.Name = "m_ButtonsFlowLayoutPanel";
			componentResourceManager.ApplyResources(this.m_CancelButton, "m_CancelButton");
			this.m_CancelButton.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.m_CancelButton.Name = "m_CancelButton";
			this.m_CancelButton.UseVisualStyleBackColor = true;
			this.m_CancelButton.Click += new global::System.EventHandler(this.OnCancelClicked);
			componentResourceManager.ApplyResources(this.m_OkButton, "m_OkButton");
			this.m_OkButton.Name = "m_OkButton";
			this.m_OkButton.UseVisualStyleBackColor = true;
			this.m_OkButton.Click += new global::System.EventHandler(this.OnOkClicked);
			componentResourceManager.ApplyResources(this.m_SetUserButton, "m_SetUserButton");
			this.m_SetUserButton.Name = "m_SetUserButton";
			this.m_SetUserButton.UseVisualStyleBackColor = true;
			this.m_SetUserButton.Click += new global::System.EventHandler(this.OnSetUserButtonClick);
			componentResourceManager.ApplyResources(this.m_ConnectAsCheckbox, "m_ConnectAsCheckbox");
			this.m_ConnectAsCheckbox.Name = "m_ConnectAsCheckbox";
			this.m_ConnectAsCheckbox.UseVisualStyleBackColor = true;
			this.m_ConnectAsCheckbox.CheckedChanged += new global::System.EventHandler(this.OnConnectAsCheckboxCheckedChanged);
			base.AcceptButton = this.m_OkButton;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.m_CancelButton;
			base.Controls.Add(this.m_SetUserButton);
			base.Controls.Add(this.m_ConnectAsCheckbox);
			base.Controls.Add(this.m_ButtonsFlowLayoutPanel);
			base.Controls.Add(this.m_MessageTableLayoutPanel);
			base.Controls.Add(this.m_ContentTableLayoutPanel);
			base.Controls.Add(this.m_ProgressPanel);
			base.Controls.Add(this.m_HeaderTableLayoutPanel);
			this.DoubleBuffered = true;
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ConnectionDialog";
			base.ShowInTaskbar = false;
			base.SizeGripStyle = global::System.Windows.Forms.SizeGripStyle.Hide;
			this.m_HeaderTableLayoutPanel.ResumeLayout(false);
			this.m_RightPanel.ResumeLayout(false);
			this.m_RightPanel.PerformLayout();
			this.m_ProgressPanel.ResumeLayout(false);
			this.m_ContentTableLayoutPanel.ResumeLayout(false);
			this.m_ContentTableLayoutPanel.PerformLayout();
			this.m_MessageTableLayoutPanel.ResumeLayout(false);
			this.m_MessageTableLayoutPanel.PerformLayout();
			((global::System.ComponentModel.ISupportInitialize)this.m_WarningPictureBox).EndInit();
			this.m_ButtonsFlowLayoutPanel.ResumeLayout(false);
			this.m_ButtonsFlowLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x04000020 RID: 32
		private global::System.Windows.Forms.Timer m_ProgressAnimationTimer = new global::System.Windows.Forms.Timer();

		// Token: 0x04000028 RID: 40
		private global::System.ComponentModel.IContainer components;

		// Token: 0x04000029 RID: 41
		private global::System.Windows.Forms.TableLayoutPanel m_HeaderTableLayoutPanel;

		// Token: 0x0400002A RID: 42
		private global::System.Windows.Forms.Panel m_LeftPanel;

		// Token: 0x0400002B RID: 43
		private global::System.Windows.Forms.Panel m_RightPanel;

		// Token: 0x0400002C RID: 44
		private global::System.Windows.Forms.Label m_TitleLabel1;

		// Token: 0x0400002D RID: 45
		private global::System.Windows.Forms.Label m_TitleLabel2;

		// Token: 0x0400002E RID: 46
		private global::System.Windows.Forms.Panel m_ProgressPanel;

		// Token: 0x0400002F RID: 47
		private global::System.Windows.Forms.Panel m_ProgressPanelLeft;

		// Token: 0x04000030 RID: 48
		private global::System.Windows.Forms.Panel m_ProgressPanelRight;

		// Token: 0x04000031 RID: 49
		private global::System.Windows.Forms.TableLayoutPanel m_ContentTableLayoutPanel;

		// Token: 0x04000032 RID: 50
		private global::System.Windows.Forms.Label m_ServerLabel;

		// Token: 0x04000033 RID: 51
		private global::System.Windows.Forms.ComboBox m_ServerComboBox;

		// Token: 0x04000034 RID: 52
		private global::System.Windows.Forms.Label m_MachineLabel;

		// Token: 0x04000035 RID: 53
		private global::System.Windows.Forms.ComboBox m_MachineComboBox;

		// Token: 0x04000036 RID: 54
		private global::System.Windows.Forms.TableLayoutPanel m_MessageTableLayoutPanel;

		// Token: 0x04000037 RID: 55
		private global::System.Windows.Forms.PictureBox m_WarningPictureBox;

		// Token: 0x04000038 RID: 56
		private global::System.Windows.Forms.Label m_MessageLabel;

		// Token: 0x04000039 RID: 57
		private global::System.Windows.Forms.FlowLayoutPanel m_ButtonsFlowLayoutPanel;

		// Token: 0x0400003A RID: 58
		private global::System.Windows.Forms.Button m_CancelButton;

		// Token: 0x0400003B RID: 59
		private global::System.Windows.Forms.Button m_OkButton;

		// Token: 0x0400003C RID: 60
		private global::System.Windows.Forms.Button m_SetUserButton;

		// Token: 0x0400003D RID: 61
		private global::System.Windows.Forms.CheckBox m_ConnectAsCheckbox;
	}
}
