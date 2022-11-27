﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using Microsoft.Virtualization.Client.Management;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000025 RID: 37
	internal class ActionMenu : IMenuItemProvider
	{
		// Token: 0x060001F4 RID: 500 RVA: 0x00010DC8 File Offset: 0x0000EFC8
		[SuppressMessage("GoldMan", "#pw17908:ConcatenatedStringAsLocalizable", Justification = "We allow concatenating constant end ellipses")]
		public ActionMenu(IMenuActionTarget menuTarget)
		{
			if (menuTarget == null)
			{
				throw new ArgumentNullException("menuTarget");
			}
			this.m_Target = menuTarget;
			this.m_ControlMenuItems = new VMControlMenuItems(menuTarget);
			VmisMenuItemFactory vmisMenuItemFactory = new VmisMenuItemFactory(new EventHandler(this.OnActionMenuItem));
			this.m_SendSas = vmisMenuItemFactory.CreateMenuItem(ActionMenu.ResourceInfo.SecureAttentionSequence);
			this.m_Snapshot = vmisMenuItemFactory.CreateMenuItem(ActionMenu.ResourceInfo.Snapshot);
			this.m_Snapshot.Action = VMControlAction.Snapshot;
			this.m_Share = vmisMenuItemFactory.CreateMenuItem(ActionMenu.ResourceInfo.Share);
			this.m_Share.Action = VMControlAction.Share;
			this.m_SendSas.ShortcutKeys = (Keys.LButton | Keys.RButton | Keys.Space | Keys.Control | Keys.Alt);
			this.m_SendSas.SetShortcutKeyDisplayString();
			if (!SnapshotRenameDialog.SnapshotRenameConfig.Instance.AcceptSnapshotAutoGeneratedName)
			{
				VmisMenuItem snapshot = this.m_Snapshot;
				snapshot.Text += VmisMenuItem.EndEllipses;
			}
			ToolStripItem[] items = new ToolStripItem[]
			{
				this.m_SendSas,
				vmisMenuItemFactory.CreateMenuItemSeparator(),
				this.m_ControlMenuItems.StartMenuItem,
				this.m_ControlMenuItems.ShutdownMenuItem,
				this.m_ControlMenuItems.SaveMenuItem,
				vmisMenuItemFactory.CreateMenuItemSeparator(),
				this.m_ControlMenuItems.PauseMenuItem,
				this.m_ControlMenuItems.ResetMenuItem,
				vmisMenuItemFactory.CreateMenuItemSeparator(),
				this.m_Snapshot,
				this.m_ControlMenuItems.RevertMenuItem,
				this.m_Share
			};
			this.m_Action = vmisMenuItemFactory.CreateParentMenuItem(ActionMenu.ResourceInfo.Action, items);
			this.m_Performer = new VMConnectUserActionPerformer(this.m_Target);
			this.m_Performer.SnapshotOperationComplete += this.HandleSnapshotOperationComplete;
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x00010F6B File Offset: 0x0000F16B
		private void OnActionMenuItem(object sender, EventArgs ea)
		{
			if (sender == this.m_SendSas)
			{
				this.m_Performer.OnSendSasTaskTriggered();
				return;
			}
			if (sender == this.m_Snapshot)
			{
				this.m_Performer.OnSnapshotTaskTrigged();
				return;
			}
			if (sender == this.m_Share)
			{
				this.m_Performer.OnShareTaskTriggered();
			}
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00010FAC File Offset: 0x0000F1AC
		private void HandleSnapshotOperationComplete(object sender, EventArgs ea)
		{
			if (this.m_Snapshot.Text.EndsWith(VmisMenuItem.EndEllipses, StringComparison.CurrentCulture) && SnapshotRenameDialog.SnapshotRenameConfig.Instance.AcceptSnapshotAutoGeneratedName)
			{
				this.m_Snapshot.Text = this.m_Snapshot.Text.Remove(this.m_Snapshot.Text.Length - VmisMenuItem.EndEllipses.Length);
			}
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x00011013 File Offset: 0x0000F213
		public VmisMenuItem[] GetMenuItems()
		{
			return new VmisMenuItem[]
			{
				this.m_Action
			};
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x00011024 File Offset: 0x0000F224
		public bool InformKeyUp(Keys key)
		{
			if (this.m_ControlMenuItems.InformKeyUp(key))
			{
				return true;
			}
			if (this.m_SendSas.Enabled && this.m_SendSas.ShortcutKeys == key)
			{
				this.OnActionMenuItem(this.m_SendSas, EventArgs.Empty);
				return true;
			}
			if (this.m_Snapshot.Enabled && this.m_Snapshot.ShortcutKeys == key)
			{
				this.OnActionMenuItem(this.m_Snapshot, EventArgs.Empty);
				return true;
			}
			return false;
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x000110A0 File Offset: 0x0000F2A0
		public void UpdateEnabledState()
		{
			bool flag = false;
			if (this.m_Target.RdpMode == RdpConnectionMode.Basic || this.m_Target.RdpMode == RdpConnectionMode.EnhancedAvailable)
			{
				flag = true;
			}
			this.m_ControlMenuItems.UpdateEnabledState();
			if (this.m_Target.VirtualMachine == null)
			{
				this.m_SendSas.Enabled = false;
				this.m_Snapshot.Enabled = false;
				this.m_Share.Enabled = false;
				return;
			}
			VMComputerSystemState state = this.m_Target.VirtualMachine.State;
			this.m_SendSas.Enabled = (state == VMComputerSystemState.Running && this.m_Target.IsRdpConnected && this.m_Target.IsRdpEnabled && flag);
			this.m_Snapshot.Enabled = this.m_Target.VirtualMachine.IsSnapshotAvailable();
			bool flag2 = ExportInvoker.ShouldShowShareAction(this.m_Target.VirtualMachine);
			this.m_Share.Visible = flag2;
			this.m_Share.Enabled = flag2;
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0001118B File Offset: 0x0000F38B
		void IMenuItemProvider.InformBeginStateChangeOperation(VMStateChangeAction stateChange)
		{
			this.m_ControlMenuItems.InformBeginStateChangeOperation(stateChange);
		}

		// Token: 0x060001FB RID: 507 RVA: 0x00011199 File Offset: 0x0000F399
		void IMenuItemProvider.InformEndStateChangeOperation(VMStateChangeAction stateChange)
		{
			this.m_ControlMenuItems.InformEndStateChangeOperation(stateChange);
		}

		// Token: 0x060001FC RID: 508 RVA: 0x000111A7 File Offset: 0x0000F3A7
		void IMenuItemProvider.InformVMConfigurationChanged()
		{
			this.m_ControlMenuItems.InformVMConfigurationChanged();
		}

		// Token: 0x060001FD RID: 509 RVA: 0x000111B4 File Offset: 0x0000F3B4
		void IMenuItemProvider.CloseMenu()
		{
			this.m_ControlMenuItems.CloseMenu();
		}

		// Token: 0x0400014A RID: 330
		private VMControlMenuItems m_ControlMenuItems;

		// Token: 0x0400014B RID: 331
		private VmisMenuItem m_SendSas;

		// Token: 0x0400014C RID: 332
		private VmisMenuItem m_Snapshot;

		// Token: 0x0400014D RID: 333
		private VmisMenuItem m_Share;

		// Token: 0x0400014E RID: 334
		private VmisMenuItem m_Action;

		// Token: 0x0400014F RID: 335
		private IMenuActionTarget m_Target;

		// Token: 0x04000150 RID: 336
		private VMConnectUserActionPerformer m_Performer;

		// Token: 0x0200004F RID: 79
		private static class ResourceInfo
		{
			// Token: 0x040001E2 RID: 482
			private const string Append = "ActionMenu_";

			// Token: 0x040001E3 RID: 483
			public static readonly MenuItemResourceInfo SecureAttentionSequence = new MenuItemResourceInfo("ActionMenu_SecureAttentionSequence", false);

			// Token: 0x040001E4 RID: 484
			public static readonly MenuItemResourceInfo Snapshot = new MenuItemResourceInfo("ActionMenu_Snapshot", true);

			// Token: 0x040001E5 RID: 485
			public static readonly MenuItemResourceInfo Action = new MenuItemResourceInfo("ActionMenu_Action", false);

			// Token: 0x040001E6 RID: 486
			public static readonly MenuItemResourceInfo Share = new MenuItemResourceInfo("ActionMenu_Share", true);
		}
	}
}
