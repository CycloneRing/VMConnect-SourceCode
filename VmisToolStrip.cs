using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Virtualization.Client.Management;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000023 RID: 35
	internal class VmisToolStrip : ToolStrip
	{
		// Token: 0x060001E9 RID: 489 RVA: 0x00010500 File Offset: 0x0000E700
		public VmisToolStrip(Form form, IMenuActionTarget actionTarget)
		{
			base.Name = "$this";
			base.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
			base.GripStyle = ToolStripGripStyle.Hidden;
			this.AutoSize = false;
			this.m_ActionTarget = actionTarget;
			this.m_Performer = new VMConnectUserActionPerformer(actionTarget);
			this.DoLayout();
			(form as IDpiForm).DpiChanged += this.OnDpiChanged;
		}

		// Token: 0x060001EA RID: 490 RVA: 0x00010564 File Offset: 0x0000E764
		internal void OnButtonClicked(object sender, EventArgs ea)
		{
			if (sender == this.m_VMSendSasButton)
			{
				this.m_Performer.OnSendSasTaskTriggered();
				return;
			}
			if (sender == this.m_VMSnapshotButton)
			{
				this.m_Performer.OnSnapshotTaskTrigged();
				return;
			}
			if (sender == this.m_VMEnhancedButton)
			{
				this.m_Performer.OnEnhancedTriggered();
				return;
			}
			if (sender == this.m_VMShareButton)
			{
				this.m_Performer.OnShareTaskTriggered();
				return;
			}
			VmisToolStripButton vmisToolStripButton = sender as VmisToolStripButton;
			this.m_Performer.OnStateChangeTaskTrigged(vmisToolStripButton.Action);
		}

		// Token: 0x060001EB RID: 491 RVA: 0x000105E0 File Offset: 0x0000E7E0
		private void DoLayout()
		{
			int deviceDpi = this.DeviceDpi();
			int num = this.LogicalToDeviceUnits(16);
			Size size = new Size(num, num);
			base.SuspendLayout();
			this.Items.Clear();
			base.ImageList = new ImageList
			{
				ColorDepth = ColorDepth.Depth32Bit,
				ImageSize = size
			};
			base.ImageScalingSize = size;
			base.ImageList.Images.Add(CommonResources.ResourceManager.GetImage("Start", deviceDpi));
			base.ImageList.Images.Add(CommonResources.ResourceManager.GetImage("Turnoff", deviceDpi));
			base.ImageList.Images.Add(CommonResources.ResourceManager.GetImage("Shutdown", deviceDpi));
			base.ImageList.Images.Add(CommonResources.ResourceManager.GetImage("PauseImage", deviceDpi));
			base.ImageList.Images.Add(CommonResources.ResourceManager.GetImage("Resume", deviceDpi));
			base.ImageList.Images.Add(CommonResources.ResourceManager.GetImage("Snapshot", deviceDpi));
			base.ImageList.Images.Add(CommonResources.ResourceManager.GetImage("RevertImage", deviceDpi));
			base.ImageList.Images.Add(CommonResources.ResourceManager.GetImage("Reset", deviceDpi));
			base.ImageList.Images.Add(CommonResources.ResourceManager.GetImage("SaveState", deviceDpi));
			base.ImageList.Images.Add(CommonResources.ResourceManager.GetImage("SendCtrlAltDelete", deviceDpi));
			base.ImageList.Images.Add(CommonResources.ResourceManager.GetImage("EnableEnhanced", deviceDpi));
			base.ImageList.Images.Add(CommonResources.ResourceManager.GetImage("DisableEnhanced", deviceDpi));
			base.ImageList.Images.Add(CommonResources.ResourceManager.GetImage("CompressVirtualMachine", deviceDpi));
			this.m_VMSendSasButton = this.InitToolBarButton("SendSasButton", VMISResources.ToolStripToolTip_Sas, VmisToolStrip.ButtonImageIndex.SendSas, VMControlAction.None);
			this.Items.Add(new ToolStripSeparator());
			this.m_VMStartButton = this.InitToolBarButton("StartButton", VMISResources.ToolStripToolTip_Start, VmisToolStrip.ButtonImageIndex.Start, VMControlAction.StartTurnOff);
			this.m_VMTurnOffButton = this.InitToolBarButton("TurnOffButton", VMISResources.ToolStripToolTip_TurnOff, VmisToolStrip.ButtonImageIndex.TurnOff, VMControlAction.StartTurnOff);
			this.m_VMShutdownButton = this.InitToolBarButton("ShutDownButton", VMISResources.ToolStripToolTip_Shutdown, VmisToolStrip.ButtonImageIndex.Shutdown, VMControlAction.Shutdown);
			this.m_VMSaveButton = this.InitToolBarButton("SaveButton", VMISResources.ToolStripToolTip_Save, VmisToolStrip.ButtonImageIndex.Save, VMControlAction.Save);
			this.Items.Add(new ToolStripSeparator());
			this.m_VMPauseResumeButton = this.InitToolBarButton("PauseResumeButton", VMISResources.ToolStripToolTip_Pause, VmisToolStrip.ButtonImageIndex.Pause, VMControlAction.PauseResume);
			this.m_VMResetButton = this.InitToolBarButton("ResetButton", VMISResources.ToolStripToolTip_Reset, VmisToolStrip.ButtonImageIndex.Reset, VMControlAction.Reset);
			this.Items.Add(new ToolStripSeparator());
			this.m_VMSnapshotButton = this.InitToolBarButton("SnapshotButton", VMISResources.ToolStripToolTip_Snapshot, VmisToolStrip.ButtonImageIndex.Snapshot, VMControlAction.Snapshot);
			this.m_VMRevertButton = this.InitToolBarButton("RevertButton", VMISResources.ToolStripToolTip_Revert, VmisToolStrip.ButtonImageIndex.Revert, VMControlAction.Revert);
			this.Items.Add(new ToolStripSeparator());
			this.m_VMEnhancedButton = this.InitToolBarButton("EnhancedButton", VMISResources.ToolStripToolTip_EnableEnhanced, VmisToolStrip.ButtonImageIndex.EnableEnhanced, VMControlAction.Enhanced);
			this.m_VMShareButton = this.InitToolBarButton("ShareButton", VMISResources.ToolStripToolTip_Share, VmisToolStrip.ButtonImageIndex.Share, VMControlAction.Share);
			this.UpdateEnabledState();
			base.ResumeLayout(false);
		}

		// Token: 0x060001EC RID: 492 RVA: 0x00010935 File Offset: 0x0000EB35
		private void OnDpiChanged(object sender, System.Windows.Forms.DpiChangedEventArgs e)
		{
			if (e.DeviceDpiOld == e.DeviceDpiNew)
			{
				return;
			}
			this.DoLayout();
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0001094C File Offset: 0x0000EB4C
		private VmisToolStripButton InitToolBarButton(string controlName, string toolTipString, VmisToolStrip.ButtonImageIndex imageIndex, VMControlAction action)
		{
			VmisToolStripButton vmisToolStripButton = new VmisToolStripButton();
			vmisToolStripButton.Name = controlName;
			vmisToolStripButton.AccessibleName = toolTipString;
			vmisToolStripButton.Click += this.OnButtonClicked;
			vmisToolStripButton.ToolTipText = toolTipString;
			vmisToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			vmisToolStripButton.ImageIndex = (int)imageIndex;
			vmisToolStripButton.ImageScaling = ToolStripItemImageScaling.SizeToFit;
			vmisToolStripButton.AutoSize = true;
			vmisToolStripButton.Action = action;
			vmisToolStripButton.Padding = new Padding(2);
			this.Items.Add(vmisToolStripButton);
			return vmisToolStripButton;
		}

		// Token: 0x060001EE RID: 494 RVA: 0x000109C4 File Offset: 0x0000EBC4
		internal void UpdateEnabledState()
		{
			if (this.m_ActionTarget.VirtualMachine == null)
			{
				this.m_VMSendSasButton.Enabled = false;
				this.m_VMStartButton.Enabled = false;
				this.m_VMTurnOffButton.Enabled = false;
				this.m_VMShutdownButton.Enabled = false;
				this.m_VMSnapshotButton.Enabled = false;
				this.m_VMRevertButton.Enabled = false;
				this.m_VMSaveButton.Enabled = false;
				this.m_VMResetButton.Enabled = false;
				this.m_VMPauseResumeButton.Enabled = false;
				this.m_VMEnhancedButton.Enabled = false;
				this.m_VMShareButton.Enabled = false;
				this.m_VMPauseResumeButton.ToolTipText = VMISResources.ToolStripToolTip_Pause;
				this.m_VMPauseResumeButton.ImageIndex = 3;
				this.m_VMEnhancedButton.ToolTipText = VMISResources.ToolStripToolTip_EnableEnhanced;
				this.m_VMEnhancedButton.ImageIndex = 10;
				return;
			}
			VMComputerSystemState state = this.m_ActionTarget.VirtualMachine.State;
			bool hasSnapshots = this.m_ActionTarget.HasSnapshots;
			this.m_VMSendSasButton.Enabled = (state == VMComputerSystemState.Running && this.m_ActionTarget.IsRdpConnected && this.m_ActionTarget.IsRdpEnabled);
			this.m_VMStartButton.Enabled = (state == VMComputerSystemState.PowerOff || state == VMComputerSystemState.Saved || state == VMComputerSystemState.Hibernated);
			this.m_VMTurnOffButton.Enabled = (state == VMComputerSystemState.Running || state == VMComputerSystemState.Paused);
			this.m_VMShutdownButton.Enabled = (state == VMComputerSystemState.Running && !this.m_ShutdownInProgress);
			this.m_VMSnapshotButton.Enabled = this.m_ActionTarget.VirtualMachine.IsSnapshotAvailable();
			this.m_VMRevertButton.Enabled = ((state == VMComputerSystemState.Running || state == VMComputerSystemState.Paused || state == VMComputerSystemState.PowerOff || state == VMComputerSystemState.Saved) && hasSnapshots);
			this.m_VMSaveButton.Enabled = (state == VMComputerSystemState.Running || state == VMComputerSystemState.Paused);
			this.m_VMResetButton.Enabled = (state == VMComputerSystemState.Running);
			this.m_VMPauseResumeButton.Enabled = (state == VMComputerSystemState.Running || state == VMComputerSystemState.Paused);
			bool flag = ExportInvoker.ShouldShowShareAction(this.m_ActionTarget.VirtualMachine);
			this.m_VMShareButton.Visible = flag;
			this.m_VMShareButton.Enabled = flag;
			if (state != VMComputerSystemState.Running)
			{
				this.m_VMEnhancedButton.ToolTipText = VMISResources.ToolStripToolTip_EnableEnhanced;
				this.m_VMEnhancedButton.ImageIndex = 10;
				this.m_VMEnhancedButton.Enabled = false;
			}
			else
			{
				switch (this.m_ActionTarget.RdpMode)
				{
				case RdpConnectionMode.InTransition:
				case RdpConnectionMode.Basic:
					this.m_VMEnhancedButton.ToolTipText = VMISResources.ToolStripToolTip_EnableEnhanced;
					this.m_VMEnhancedButton.ImageIndex = 10;
					this.m_VMEnhancedButton.Enabled = false;
					break;
				case RdpConnectionMode.EnhancedAvailable:
					this.m_VMEnhancedButton.ToolTipText = VMISResources.ToolStripToolTip_EnableEnhanced;
					this.m_VMEnhancedButton.ImageIndex = 10;
					this.m_VMEnhancedButton.Enabled = true;
					break;
				case RdpConnectionMode.Enhanced:
					this.m_VMEnhancedButton.ToolTipText = VMISResources.ToolStripToolTip_DisableEnhanced;
					this.m_VMEnhancedButton.ImageIndex = 11;
					this.m_VMEnhancedButton.Enabled = !this.m_ActionTarget.IsShieldedVm();
					this.m_VMSendSasButton.Enabled = false;
					break;
				}
			}
			if (state == VMComputerSystemState.Paused)
			{
				this.m_VMPauseResumeButton.ToolTipText = VMISResources.ToolStripToolTip_Resume;
				this.m_VMPauseResumeButton.ImageIndex = 4;
				return;
			}
			this.m_VMPauseResumeButton.ToolTipText = VMISResources.ToolStripToolTip_Pause;
			this.m_VMPauseResumeButton.ImageIndex = 3;
		}

		// Token: 0x060001EF RID: 495 RVA: 0x00010D08 File Offset: 0x0000EF08
		public void InformBeginStateChangeOperation(VMStateChangeAction stateChange)
		{
			if (stateChange != VMStateChangeAction.Shutdown)
			{
				this.m_VMStartButton.Enabled = false;
				this.m_VMTurnOffButton.Enabled = false;
				this.m_VMShutdownButton.Enabled = false;
				this.m_VMRevertButton.Enabled = false;
				this.m_VMSaveButton.Enabled = false;
				this.m_VMResetButton.Enabled = false;
				this.m_VMPauseResumeButton.Enabled = false;
				this.m_VMEnhancedButton.Enabled = false;
				this.m_VMShareButton.Enabled = false;
				return;
			}
			this.m_ShutdownInProgress = true;
			this.m_VMShutdownButton.Enabled = false;
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x00010D99 File Offset: 0x0000EF99
		public void InformEndStateChangeOperation(VMStateChangeAction stateChange)
		{
			if (stateChange == VMStateChangeAction.Shutdown)
			{
				this.m_ShutdownInProgress = false;
			}
			this.UpdateEnabledState();
		}

		// Token: 0x0400013B RID: 315
		private VmisToolStripButton m_VMStartButton;

		// Token: 0x0400013C RID: 316
		private VmisToolStripButton m_VMTurnOffButton;

		// Token: 0x0400013D RID: 317
		private VmisToolStripButton m_VMPauseResumeButton;

		// Token: 0x0400013E RID: 318
		private VmisToolStripButton m_VMShutdownButton;

		// Token: 0x0400013F RID: 319
		private VmisToolStripButton m_VMSnapshotButton;

		// Token: 0x04000140 RID: 320
		private VmisToolStripButton m_VMRevertButton;

		// Token: 0x04000141 RID: 321
		private VmisToolStripButton m_VMResetButton;

		// Token: 0x04000142 RID: 322
		private VmisToolStripButton m_VMSaveButton;

		// Token: 0x04000143 RID: 323
		private VmisToolStripButton m_VMSendSasButton;

		// Token: 0x04000144 RID: 324
		private VmisToolStripButton m_VMEnhancedButton;

		// Token: 0x04000145 RID: 325
		private VmisToolStripButton m_VMShareButton;

		// Token: 0x04000146 RID: 326
		private IMenuActionTarget m_ActionTarget;

		// Token: 0x04000147 RID: 327
		private VMConnectUserActionPerformer m_Performer;

		// Token: 0x04000148 RID: 328
		private bool m_ShutdownInProgress;

		// Token: 0x0200004E RID: 78
		private enum ButtonImageIndex
		{
			// Token: 0x040001D5 RID: 469
			Start,
			// Token: 0x040001D6 RID: 470
			TurnOff,
			// Token: 0x040001D7 RID: 471
			Shutdown,
			// Token: 0x040001D8 RID: 472
			Pause,
			// Token: 0x040001D9 RID: 473
			Resume,
			// Token: 0x040001DA RID: 474
			Snapshot,
			// Token: 0x040001DB RID: 475
			Revert,
			// Token: 0x040001DC RID: 476
			Reset,
			// Token: 0x040001DD RID: 477
			Save,
			// Token: 0x040001DE RID: 478
			SendSas,
			// Token: 0x040001DF RID: 479
			EnableEnhanced,
			// Token: 0x040001E0 RID: 480
			DisableEnhanced,
			// Token: 0x040001E1 RID: 481
			Share
		}
	}
}
