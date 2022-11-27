using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Virtualization.Client.Management;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000034 RID: 52
	internal class VMControlMenuItems : MenuItemProvider, IMenuItemProvider
	{
		// Token: 0x06000282 RID: 642 RVA: 0x000138D0 File Offset: 0x00011AD0
		[SuppressMessage("GoldMan", "#pw17908:ConcatenatedStringAsLocalizable", Justification = "We allow concatenating constant end ellipses")]
		public VMControlMenuItems(IMenuActionTarget target) : base(target)
		{
			VmisMenuItemFactory menuItemFactory = base.GetMenuItemFactory();
			this.m_Start = menuItemFactory.CreateMenuItem(VMControlMenuItems.ResourceInfo.Start);
			this.m_Revert = menuItemFactory.CreateMenuItem(VMControlMenuItems.ResourceInfo.Revert);
			this.m_Shutdown = menuItemFactory.CreateMenuItem(VMControlMenuItems.ResourceInfo.Shutdown);
			this.m_Save = menuItemFactory.CreateMenuItem(VMControlMenuItems.ResourceInfo.Save);
			this.m_Pause = menuItemFactory.CreateMenuItem(VMControlMenuItems.ResourceInfo.Pause);
			this.m_Reset = menuItemFactory.CreateMenuItem(VMControlMenuItems.ResourceInfo.Reset);
			this.m_Start.Action = VMControlAction.StartTurnOff;
			this.m_Shutdown.Action = VMControlAction.Shutdown;
			this.m_Save.Action = VMControlAction.Save;
			this.m_Pause.Action = VMControlAction.PauseResume;
			this.m_Reset.Action = VMControlAction.Reset;
			this.m_Revert.Action = VMControlAction.Revert;
			CommonConfiguration instance = CommonConfiguration.Instance;
			if (instance.NeedToConfirm(Confirmations.Shutdown))
			{
				VmisMenuItem shutdown = this.m_Shutdown;
				shutdown.Text += VmisMenuItem.EndEllipses;
			}
			if (instance.NeedToConfirm(Confirmations.Reset))
			{
				VmisMenuItem reset = this.m_Reset;
				reset.Text += VmisMenuItem.EndEllipses;
			}
			if (instance.NeedToConfirm(Confirmations.Revert))
			{
				VmisMenuItem revert = this.m_Revert;
				revert.Text += VmisMenuItem.EndEllipses;
			}
			base.MenuItems = new VmisMenuItem[]
			{
				this.m_Start,
				this.m_Shutdown,
				this.m_Save,
				this.m_Pause,
				this.m_Reset,
				this.m_Revert
			};
			this.m_Performer = new VMConnectUserActionPerformer(target);
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000283 RID: 643 RVA: 0x00013A58 File Offset: 0x00011C58
		public VmisMenuItem StartMenuItem
		{
			get
			{
				return this.m_Start;
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000284 RID: 644 RVA: 0x00013A60 File Offset: 0x00011C60
		public VmisMenuItem ShutdownMenuItem
		{
			get
			{
				return this.m_Shutdown;
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000285 RID: 645 RVA: 0x00013A68 File Offset: 0x00011C68
		public VmisMenuItem SaveMenuItem
		{
			get
			{
				return this.m_Save;
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000286 RID: 646 RVA: 0x00013A70 File Offset: 0x00011C70
		public VmisMenuItem PauseMenuItem
		{
			get
			{
				return this.m_Pause;
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000287 RID: 647 RVA: 0x00013A78 File Offset: 0x00011C78
		public VmisMenuItem ResetMenuItem
		{
			get
			{
				return this.m_Reset;
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000288 RID: 648 RVA: 0x00013A80 File Offset: 0x00011C80
		public VmisMenuItem RevertMenuItem
		{
			get
			{
				return this.m_Revert;
			}
		}

		// Token: 0x06000289 RID: 649 RVA: 0x00013A88 File Offset: 0x00011C88
		protected override void OnMenuItem(object sender, EventArgs ea)
		{
			VmisMenuItem vmisMenuItem = sender as VmisMenuItem;
			this.m_Performer.OnStateChangeTaskTrigged(vmisMenuItem.Action);
			if (vmisMenuItem.Text.EndsWith(VmisMenuItem.EndEllipses, StringComparison.CurrentCulture))
			{
				bool flag = false;
				CommonConfiguration instance = CommonConfiguration.Instance;
				if (vmisMenuItem == this.m_Start && vmisMenuItem.Text.StartsWith(VMISResources.Menu_StartCommand_TurnOff, StringComparison.CurrentCulture) && !instance.NeedToConfirm(Confirmations.Turnoff))
				{
					flag = true;
				}
				else if (vmisMenuItem == this.m_Shutdown && !instance.NeedToConfirm(Confirmations.Shutdown))
				{
					flag = true;
				}
				else if (vmisMenuItem == this.m_Reset && !instance.NeedToConfirm(Confirmations.Reset))
				{
					flag = true;
				}
				else if (vmisMenuItem == this.m_Revert && !instance.NeedToConfirm(Confirmations.Revert))
				{
					flag = true;
				}
				if (flag)
				{
					vmisMenuItem.Text = vmisMenuItem.Text.Remove(vmisMenuItem.Text.Length - VmisMenuItem.EndEllipses.Length);
				}
			}
		}

		// Token: 0x0600028A RID: 650 RVA: 0x00013B60 File Offset: 0x00011D60
		[SuppressMessage("GoldMan", "#pw17908:ConcatenatedStringAsLocalizable", Justification = "We allow concatenating constant end ellipses")]
		public override void UpdateEnabledState()
		{
			if (base.MenuTarget.VirtualMachine != null)
			{
				VMComputerSystemState state = base.MenuTarget.VirtualMachine.State;
				bool flag = state == VMComputerSystemState.Running || state == VMComputerSystemState.Paused;
				this.m_Start.Enabled = (state == VMComputerSystemState.PowerOff || state == VMComputerSystemState.Saved || flag);
				this.m_Revert.Enabled = ((state == VMComputerSystemState.PowerOff || state == VMComputerSystemState.Saved || flag) && base.MenuTarget.HasSnapshots);
				this.m_Shutdown.Enabled = (state == VMComputerSystemState.Running && !this.m_ShutdownInProgress);
				this.m_Save.Enabled = flag;
				this.m_Pause.Enabled = (state == VMComputerSystemState.Running || state == VMComputerSystemState.Paused);
				this.m_Reset.Enabled = (state == VMComputerSystemState.Running);
				if (state == VMComputerSystemState.Paused)
				{
					this.m_Pause.Text = VMISResources.Menu_PauseCommand_Resume;
				}
				else
				{
					this.m_Pause.Text = VMISResources.Menu_PauseCommand_Pause;
				}
				if (state == VMComputerSystemState.PowerOff || state == VMComputerSystemState.Saved)
				{
					this.m_Start.Text = VMISResources.Menu_StartCommand_Start;
					return;
				}
				this.m_Start.Text = VMISResources.Menu_StartCommand_TurnOff;
				if (CommonConfiguration.Instance.NeedToConfirm(Confirmations.Turnoff))
				{
					VmisMenuItem start = this.m_Start;
					start.Text += VmisMenuItem.EndEllipses;
					return;
				}
			}
			else
			{
				VmisMenuItem[] menuItems = base.MenuItems;
				for (int i = 0; i < menuItems.Length; i++)
				{
					menuItems[i].Enabled = false;
				}
				this.m_Pause.Text = VMISResources.Menu_PauseCommand_Pause;
				this.m_Start.Text = VMISResources.Menu_StartCommand_Start;
			}
		}

		// Token: 0x0600028B RID: 651 RVA: 0x00013CE0 File Offset: 0x00011EE0
		public override void InformBeginStateChangeOperation(VMStateChangeAction stateChange)
		{
			if (stateChange != VMStateChangeAction.Shutdown)
			{
				VmisMenuItem[] menuItems = base.MenuItems;
				for (int i = 0; i < menuItems.Length; i++)
				{
					menuItems[i].Enabled = false;
				}
				return;
			}
			this.m_ShutdownInProgress = true;
			this.m_Shutdown.Enabled = false;
		}

		// Token: 0x0600028C RID: 652 RVA: 0x00013D23 File Offset: 0x00011F23
		public override void InformEndStateChangeOperation(VMStateChangeAction stateChange)
		{
			if (stateChange == VMStateChangeAction.Shutdown)
			{
				this.m_ShutdownInProgress = false;
			}
			this.UpdateEnabledState();
		}

		// Token: 0x04000187 RID: 391
		private VmisMenuItem m_Start;

		// Token: 0x04000188 RID: 392
		private VmisMenuItem m_Revert;

		// Token: 0x04000189 RID: 393
		private VmisMenuItem m_Shutdown;

		// Token: 0x0400018A RID: 394
		private VmisMenuItem m_Save;

		// Token: 0x0400018B RID: 395
		private VmisMenuItem m_Pause;

		// Token: 0x0400018C RID: 396
		private VmisMenuItem m_Reset;

		// Token: 0x0400018D RID: 397
		private VMConnectUserActionPerformer m_Performer;

		// Token: 0x0400018E RID: 398
		private bool m_ShutdownInProgress;

		// Token: 0x0200005E RID: 94
		private static class ResourceInfo
		{
			// Token: 0x04000214 RID: 532
			private const string Append = "VMControl_";

			// Token: 0x04000215 RID: 533
			public static readonly MenuItemResourceInfo Start = new MenuItemResourceInfo("VMControl_Start", true);

			// Token: 0x04000216 RID: 534
			public static readonly MenuItemResourceInfo Revert = new MenuItemResourceInfo("VMControl_Revert", true);

			// Token: 0x04000217 RID: 535
			public static readonly MenuItemResourceInfo Shutdown = new MenuItemResourceInfo("VMControl_Shutdown", true);

			// Token: 0x04000218 RID: 536
			public static readonly MenuItemResourceInfo Save = new MenuItemResourceInfo("VMControl_Save", true);

			// Token: 0x04000219 RID: 537
			public static readonly MenuItemResourceInfo Pause = new MenuItemResourceInfo("VMControl_Pause", true);

			// Token: 0x0400021A RID: 538
			public static readonly MenuItemResourceInfo Reset = new MenuItemResourceInfo("VMControl_Reset", true);
		}
	}
}
