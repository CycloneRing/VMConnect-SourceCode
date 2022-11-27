using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Virtualization.Client.Management;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000022 RID: 34
	internal class VmisStatusStrip : StatusStrip
	{
		// Token: 0x060001D2 RID: 466 RVA: 0x0000F844 File Offset: 0x0000DA44
		public VmisStatusStrip(Form form)
		{
			base.SuspendLayout();
			base.Name = "$this";
			base.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
			base.ShowItemToolTips = true;
			this.AutoSize = true;
			this.m_VMStateLabel = new ToolStripStatusLabel
			{
				Name = "VMStateLabel",
				AutoSize = true
			};
			this.m_ProgressLabel = new ToolStripStatusLabel
			{
				Name = "ProgressLabel",
				AutoSize = true
			};
			this.m_InformationLabel = new ToolStripStatusLabel
			{
				Name = "InformationLabel",
				AutoSize = true
			};
			this.m_ProgressBar = new ToolStripProgressBar
			{
				Name = "ProgressBar",
				AutoSize = false,
				Minimum = 0,
				Maximum = 100,
				Step = 1,
				Value = 0
			};
			this.m_MouseCapturedIcon = new ToolStripStatusLabel
			{
				Name = "MouseCapturedIcon",
				AutoSize = true,
				Alignment = ToolStripItemAlignment.Right,
				AutoToolTip = false,
				DisplayStyle = ToolStripItemDisplayStyle.Image,
				ImageScaling = ToolStripItemImageScaling.None
			};
			this.m_KeyboardCapturedIcon = new ToolStripStatusLabel
			{
				Name = "KeyboardCapturedIcon",
				AutoSize = true,
				Alignment = ToolStripItemAlignment.Right,
				AutoToolTip = false,
				DisplayStyle = ToolStripItemDisplayStyle.Image,
				ImageScaling = ToolStripItemImageScaling.None,
				Padding = new Padding(0, 0, 5, 0)
			};
			this.m_LockIcon = new ToolStripStatusLabel
			{
				Name = "LockIcon",
				AutoSize = true,
				Alignment = ToolStripItemAlignment.Right,
				AutoToolTip = false,
				DisplayStyle = ToolStripItemDisplayStyle.Image,
				ImageScaling = ToolStripItemImageScaling.None
			};
			this._resources = new ComponentResourceManager(typeof(VmisStatusStrip));
			this._resources.ApplyResources(this, base.Name);
			this._resources.ApplyResources(this.m_VMStateLabel, this.m_VMStateLabel.Name);
			this._resources.ApplyResources(this.m_ProgressLabel, this.m_ProgressLabel.Name);
			this._resources.ApplyResources(this.m_InformationLabel, this.m_InformationLabel.Name);
			this._resources.ApplyResources(this.m_MouseCapturedIcon, this.m_MouseCapturedIcon.Name);
			this._resources.ApplyResources(this.m_KeyboardCapturedIcon, this.m_KeyboardCapturedIcon.Name);
			this._resources.ApplyResources(this.m_LockIcon, this.m_LockIcon.Name);
			this.m_MouseCaptureToolTips = new string[]
			{
				this._resources.GetString("MouseNotCapturedToolTip"),
				this._resources.GetString("MouseCapturedToolTip")
			};
			this.m_KeyboardCaptureToolTips = new string[]
			{
				this._resources.GetString("KeyboardNotCapturedToolTip"),
				this._resources.GetString("KeyboardCapturedToolTip")
			};
			this.m_LockToolTips = new string[]
			{
				this._resources.GetString("LockToolTip_None"),
				this._resources.GetString("LockToolTip_Certificate"),
				this._resources.GetString("LockToolTip_Kerberos"),
				this._resources.GetString("LockToolTip_CertificateAndKerberos")
			};
			this.DoLayout();
			this.Items.Add(this.m_VMStateLabel);
			this.Items.Add(this.m_InformationLabel);
			this.Items.Add(this.m_LockIcon);
			this.Items.Add(this.m_MouseCapturedIcon);
			this.Items.Add(this.m_KeyboardCapturedIcon);
			base.ResumeLayout(false);
			(form as IDpiForm).DpiChanged += this.OnDpiChanged;
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060001D3 RID: 467 RVA: 0x0000FBD1 File Offset: 0x0000DDD1
		public bool IsDisplayingTaskProgress
		{
			get
			{
				return this.Items.Contains(this.m_ProgressLabel) && !this.m_DisplayingTaskResult;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060001D4 RID: 468 RVA: 0x0000FBF1 File Offset: 0x0000DDF1
		public bool IsDisplayingTaskResults
		{
			get
			{
				return this.Items.Contains(this.m_ProgressLabel) && this.m_DisplayingTaskResult;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060001D5 RID: 469 RVA: 0x0000FC0E File Offset: 0x0000DE0E
		public ClientCreatedTask DisplayedTask
		{
			get
			{
				if (this.IsDisplayingTaskProgress || this.IsDisplayingTaskResults)
				{
					return (ClientCreatedTask)this.m_ProgressBar.Tag;
				}
				return null;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060001D6 RID: 470 RVA: 0x0000FC32 File Offset: 0x0000DE32
		public string DisplayedInformation
		{
			get
			{
				return this.m_InformationLabel.Text;
			}
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000FC3F File Offset: 0x0000DE3F
		private void HandlePercentCompleteChanged(object sender, EventArgs ea)
		{
			if (this.IsDisplayingTaskProgress && this.m_ProgressBar.Tag == sender)
			{
				this.m_ProgressBar.Value = ((ClientCreatedTask)sender).PercentComplete;
			}
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0000FC70 File Offset: 0x0000DE70
		private void HandleTaskCompleted(object sender, EventArgs ea)
		{
			if (this.IsDisplayingTaskProgress && this.m_ProgressBar.Tag == sender)
			{
				ClientCreatedTask clientCreatedTask = (ClientCreatedTask)sender;
				VMTaskStatus status = clientCreatedTask.Status;
				this.CleanupTask(clientCreatedTask);
				this.DisplayTaskResult(status);
			}
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000FCB0 File Offset: 0x0000DEB0
		public void InformVMState(VMComputerSystemState state, bool isMigrating)
		{
			string arg;
			if (isMigrating)
			{
				arg = VMISResources.StatusBar_Migrating;
			}
			else
			{
				arg = new VMComputerSystemStateConverter().ConvertToString(null, CultureInfo.CurrentUICulture, state);
			}
			this.m_VMStateLabel.Text = string.Format(CultureInfo.CurrentCulture, VMISResources.StatusBar_VMStateDisplay, arg);
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000FCFA File Offset: 0x0000DEFA
		public void InformNoVM()
		{
			this.m_VMStateLabel.Text = string.Empty;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000FD0C File Offset: 0x0000DF0C
		public void InformConnectedState(bool connected, RdpAuthenticationType autheticationType)
		{
			this.m_Connected = connected;
			if (this.m_Connected)
			{
				this.m_MouseCapturedIcon.ImageIndex = 0;
				this.m_MouseCapturedIcon.ToolTipText = this.m_MouseCaptureToolTips[0];
				this.m_KeyboardCapturedIcon.ImageIndex = 3;
				this.m_KeyboardCapturedIcon.ToolTipText = this.m_KeyboardCaptureToolTips[1];
				if (autheticationType == RdpAuthenticationType.None)
				{
					this.m_LockIcon.ImageIndex = 4;
				}
				else
				{
					this.m_LockIcon.ImageIndex = 5;
				}
				if (autheticationType >= RdpAuthenticationType.None && autheticationType < (RdpAuthenticationType)this.m_LockToolTips.Length)
				{
					this.m_LockIcon.ToolTipText = this.m_LockToolTips[(int)autheticationType];
				}
			}
			else
			{
				this.m_MouseCapturedIcon.ImageIndex = -1;
				this.m_MouseCapturedIcon.ToolTipText = string.Empty;
				this.m_KeyboardCapturedIcon.ImageIndex = -1;
				this.m_KeyboardCapturedIcon.ToolTipText = string.Empty;
				this.m_LockIcon.ImageIndex = -1;
				this.m_LockIcon.ToolTipText = string.Empty;
			}
			this.m_MouseCapturedIcon.AccessibleName = this.m_MouseCapturedIcon.ToolTipText;
			this.m_KeyboardCapturedIcon.AccessibleName = this.m_KeyboardCapturedIcon.ToolTipText;
			this.m_LockIcon.AccessibleName = this.m_LockIcon.ToolTipText;
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000FE44 File Offset: 0x0000E044
		public void InformMouseCapturedState(bool captured)
		{
			if (this.m_Connected)
			{
				this.m_MouseCapturedIcon.ImageIndex = (captured ? 1 : 0);
				string text = captured ? this.m_MouseCaptureToolTips[1] : this.m_MouseCaptureToolTips[0];
				this.m_MouseCapturedIcon.ToolTipText = text;
				this.m_MouseCapturedIcon.AccessibleName = text;
			}
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000FE9C File Offset: 0x0000E09C
		public void InformKeyboardInputCapturedState(bool captured)
		{
			if (this.m_Connected)
			{
				this.m_KeyboardCapturedIcon.ImageIndex = (captured ? 3 : 2);
				string text = captured ? this.m_KeyboardCaptureToolTips[1] : this.m_KeyboardCaptureToolTips[0];
				this.m_KeyboardCapturedIcon.ToolTipText = text;
				this.m_KeyboardCapturedIcon.AccessibleName = text;
			}
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000FEF1 File Offset: 0x0000E0F1
		public void DisplayInformation(string information, Image informationImage)
		{
			this.m_InformationLabel.Image = informationImage;
			this.m_InformationLabel.Text = information;
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000FF0B File Offset: 0x0000E10B
		public void DisplayInformation(string information)
		{
			this.DisplayInformation(information, null);
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000FF15 File Offset: 0x0000E115
		public void ClearInformationDisplay()
		{
			this.m_InformationLabel.Image = null;
			this.m_InformationLabel.Text = string.Empty;
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000FF34 File Offset: 0x0000E134
		public void ClearProgressDisplay()
		{
			if (this.IsDisplayingTaskResults || this.IsDisplayingTaskProgress)
			{
				this.m_DisplayingTaskResult = false;
				this.m_ProgressLabel.Image = null;
				this.m_ProgressLabel.Text = string.Empty;
				this.m_ProgressLabel.Tag = null;
				this.Items.Remove(this.m_ProgressLabel);
				this.m_ProgressBar.Value = 0;
				if (this.Items.Contains(this.m_ProgressBar))
				{
					ClientCreatedTask task = (ClientCreatedTask)this.m_ProgressBar.Tag;
					this.CleanupTask(task);
					this.m_ProgressBar.Tag = null;
					this.Items.Remove(this.m_ProgressBar);
				}
				this.Items.Add(this.m_InformationLabel);
			}
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000FFFC File Offset: 0x0000E1FC
		public void DisplayTaskProgress(ClientCreatedTask task, string taskName, string taskCompleteMessage, string taskErrorMessage, string taskCanceledMessage)
		{
			if (task == null)
			{
				throw new ArgumentNullException("task", "task can not be null.");
			}
			this.ClearProgressDisplay();
			this.Items.Remove(this.m_InformationLabel);
			this.m_ProgressLabel.Width = this.GetStatusLabelWidth(this.m_ProgressLabel.Font, new string[]
			{
				taskName,
				taskCompleteMessage,
				taskErrorMessage,
				taskCanceledMessage
			});
			this.m_ProgressLabel.Text = taskName;
			this.m_ProgressLabel.Tag = new VmisStatusStrip.TaskCompleteMessage(taskCompleteMessage, taskErrorMessage, taskCanceledMessage);
			this.m_ProgressBar.Tag = task;
			this.Items.Add(this.m_ProgressLabel);
			this.Items.Add(this.m_ProgressBar);
			task.PercentCompleteChanged += this.HandlePercentCompleteChanged;
			task.Completed += this.HandleTaskCompleted;
			this.m_ProgressBar.Value = task.PercentComplete;
			if (task.IsCompleted)
			{
				this.HandleTaskCompleted(task, EventArgs.Empty);
			}
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x00010104 File Offset: 0x0000E304
		private void DoLayout()
		{
			int deviceDpi = this.DeviceDpi();
			int num = this.LogicalToDeviceUnits(16);
			Size size = new Size(num, num);
			base.SuspendLayout();
			if (this.m_VMStateLabelLogicalFontSizeInPoints == 0f)
			{
				this.m_VMStateLabelLogicalFontSizeInPoints = this.DeviceToLogicalUnits(this.m_VMStateLabel.Font.SizeInPoints);
			}
			if (this.m_InformationLabelLogicalFontSizeInPoints == 0f)
			{
				this.m_InformationLabelLogicalFontSizeInPoints = this.DeviceToLogicalUnits(this.m_InformationLabel.Font.SizeInPoints);
			}
			if (this.m_ProgressLabelLogicalFontSizeInPoints == 0f)
			{
				this.m_ProgressLabelLogicalFontSizeInPoints = this.DeviceToLogicalUnits(this.m_ProgressLabel.Font.SizeInPoints);
			}
			if (this.m_ProgressBarLogicalSize == Size.Empty)
			{
				this.m_ProgressBarLogicalSize = this.DeviceToLogicalUnits(this.m_ProgressBar.Size);
				this.m_ProgressBar.AutoSize = false;
			}
			this.m_VMStateLabel.Font = new Font(this.m_VMStateLabel.Font.FontFamily, this.LogicalToDeviceUnits(this.m_VMStateLabelLogicalFontSizeInPoints));
			this.m_InformationLabel.Font = new Font(this.m_InformationLabel.Font.FontFamily, this.LogicalToDeviceUnits(this.m_InformationLabelLogicalFontSizeInPoints));
			this.m_ProgressLabel.Font = new Font(this.m_ProgressLabel.Font.FontFamily, this.LogicalToDeviceUnits(this.m_ProgressLabelLogicalFontSizeInPoints));
			this.m_ProgressBar.Size = this.LogicalToDeviceUnits(this.m_ProgressBarLogicalSize);
			base.ImageList = new ImageList
			{
				ColorDepth = ColorDepth.Depth32Bit,
				ImageSize = size
			};
			base.ImageScalingSize = size;
			base.ImageList.Images.Add(this._resources.GetImage("MouseNotCapturedImage", deviceDpi));
			base.ImageList.Images.Add(this._resources.GetImage("MouseCapturedImage", deviceDpi));
			base.ImageList.Images.Add(this._resources.GetImage("KeyboardNotCapturedImage", deviceDpi));
			base.ImageList.Images.Add(this._resources.GetImage("KeyboardCapturedImage", deviceDpi));
			base.ImageList.Images.Add(this._resources.GetImage("Unlock", deviceDpi));
			base.ImageList.Images.Add(this._resources.GetImage("Lock", deviceDpi));
			base.ResumeLayout(false);
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x00010369 File Offset: 0x0000E569
		private void OnDpiChanged(object sender, System.Windows.Forms.DpiChangedEventArgs e)
		{
			if (e.DeviceDpiOld == e.DeviceDpiNew)
			{
				return;
			}
			this.DoLayout();
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x00010380 File Offset: 0x0000E580
		private void CleanupTask(ClientCreatedTask task)
		{
			task.PercentCompleteChanged -= this.HandlePercentCompleteChanged;
			task.Completed -= this.HandleTaskCompleted;
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x000103A8 File Offset: 0x0000E5A8
		private void DisplayTaskResult(VMTaskStatus taskCompletedStatus)
		{
			if (!this.IsDisplayingTaskProgress)
			{
				return;
			}
			this.m_DisplayingTaskResult = true;
			this.m_ProgressBar.Value = 100;
			VmisStatusStrip.TaskCompleteMessage taskCompleteMessage = (VmisStatusStrip.TaskCompleteMessage)this.m_ProgressLabel.Tag;
			if (taskCompletedStatus == VMTaskStatus.CompletedSuccessfully)
			{
				this.m_ProgressLabel.Text = taskCompleteMessage.Success;
			}
			else if (taskCompletedStatus == VMTaskStatus.CompletedWithErrors)
			{
				int scaledSize = this.LogicalToDeviceUnits(16);
				this.m_ProgressLabel.Image = CommonResources.ResourceManager.GetImageFromIcon("ErrorIcon", scaledSize);
				this.m_ProgressLabel.Text = taskCompleteMessage.Error;
			}
			else
			{
				int scaledSize2 = this.LogicalToDeviceUnits(16);
				this.m_ProgressLabel.Image = CommonResources.ResourceManager.GetImageFromIcon("WarningIcon", scaledSize2);
				this.m_ProgressLabel.Text = taskCompleteMessage.Canceled;
			}
			DelayedUIInvoker delayedUIInvoker = new DelayedUIInvoker();
			delayedUIInvoker.DelayTime = TimeSpan.FromMilliseconds(5000.0);
			delayedUIInvoker.Invoked += delegate(object o, EventArgs e)
			{
				if (!this.IsDisplayingTaskProgress)
				{
					this.ClearProgressDisplay();
				}
			};
			delayedUIInvoker.Invoke();
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0001049C File Offset: 0x0000E69C
		private int GetStatusLabelWidth(Font font, params string[] texts)
		{
			int num = 0;
			for (int i = 0; i < texts.Length; i++)
			{
				int width = TextRenderer.MeasureText(texts[i], font).Width;
				if (width > num)
				{
					num = width;
				}
			}
			int num2 = this.LogicalToDeviceUnits(16);
			num += num2;
			return num + this.LogicalToDeviceUnits(5);
		}

		// Token: 0x04000128 RID: 296
		private const int gm_TaskResultDisplayDelay = 5000;

		// Token: 0x04000129 RID: 297
		private const int gm_ProgressLabelSpacing = 5;

		// Token: 0x0400012A RID: 298
		private ToolStripStatusLabel m_VMStateLabel;

		// Token: 0x0400012B RID: 299
		private ToolStripStatusLabel m_ProgressLabel;

		// Token: 0x0400012C RID: 300
		private ToolStripStatusLabel m_InformationLabel;

		// Token: 0x0400012D RID: 301
		private ToolStripProgressBar m_ProgressBar;

		// Token: 0x0400012E RID: 302
		private ToolStripStatusLabel m_MouseCapturedIcon;

		// Token: 0x0400012F RID: 303
		private string[] m_MouseCaptureToolTips;

		// Token: 0x04000130 RID: 304
		private ToolStripStatusLabel m_KeyboardCapturedIcon;

		// Token: 0x04000131 RID: 305
		private string[] m_KeyboardCaptureToolTips;

		// Token: 0x04000132 RID: 306
		private ToolStripStatusLabel m_LockIcon;

		// Token: 0x04000133 RID: 307
		private string[] m_LockToolTips;

		// Token: 0x04000134 RID: 308
		private float m_VMStateLabelLogicalFontSizeInPoints;

		// Token: 0x04000135 RID: 309
		private float m_InformationLabelLogicalFontSizeInPoints;

		// Token: 0x04000136 RID: 310
		private float m_ProgressLabelLogicalFontSizeInPoints;

		// Token: 0x04000137 RID: 311
		private Size m_ProgressBarLogicalSize;

		// Token: 0x04000138 RID: 312
		private bool m_DisplayingTaskResult;

		// Token: 0x04000139 RID: 313
		private bool m_Connected;

		// Token: 0x0400013A RID: 314
		private ComponentResourceManager _resources;

		// Token: 0x0200004B RID: 75
		private class TaskCompleteMessage
		{
			// Token: 0x060003EE RID: 1006 RVA: 0x00015B9A File Offset: 0x00013D9A
			public TaskCompleteMessage(string success, string error, string canceled)
			{
				this.Success = success;
				this.Error = error;
				this.Canceled = canceled;
			}

			// Token: 0x040001C6 RID: 454
			public string Success;

			// Token: 0x040001C7 RID: 455
			public string Error;

			// Token: 0x040001C8 RID: 456
			public string Canceled;
		}

		// Token: 0x0200004C RID: 76
		private enum VmisStatusStripImage
		{
			// Token: 0x040001CA RID: 458
			None = -1,
			// Token: 0x040001CB RID: 459
			MouseNotCaptured,
			// Token: 0x040001CC RID: 460
			MouseCaptured,
			// Token: 0x040001CD RID: 461
			KeyboardNotCaptured,
			// Token: 0x040001CE RID: 462
			KeyboardCaptured,
			// Token: 0x040001CF RID: 463
			Unlock,
			// Token: 0x040001D0 RID: 464
			Lock
		}

		// Token: 0x0200004D RID: 77
		private enum VmisToolTipIndex
		{
			// Token: 0x040001D2 RID: 466
			NotCaptured,
			// Token: 0x040001D3 RID: 467
			Captured
		}
	}
}
