using System;
using System.Windows.Forms;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000033 RID: 51
	internal class ViewMenu : MenuItemProvider, IMenuItemProvider
	{
		// Token: 0x0600027B RID: 635 RVA: 0x000133D8 File Offset: 0x000115D8
		public ViewMenu(IMenuActionTarget menuTarget) : base(menuTarget)
		{
			VmisMenuItemFactory menuItemFactory = base.GetMenuItemFactory();
			this.m_Performer = new VMConnectUserActionPerformer(base.MenuTarget);
			this.m_Enhanced = menuItemFactory.CreateMenuItem(ViewMenu.ResourceInfo.Enhanced);
			this.m_FullScreen = menuItemFactory.CreateMenuItem(ViewMenu.ResourceInfo.FullScreen);
			this.m_Toolbar = menuItemFactory.CreateMenuItem(ViewMenu.ResourceInfo.Toolbar);
			this.m_CustomZoom = menuItemFactory.CreateMenuItem(ViewMenu.ResourceInfo.PCustom);
			this.m_ZoomChildren = new VmisMenuItem[]
			{
				menuItemFactory.CreateMenuItem(ViewMenu.ResourceInfo.PAuto),
				menuItemFactory.CreateMenuItem(ViewMenu.ResourceInfo.P25),
				menuItemFactory.CreateMenuItem(ViewMenu.ResourceInfo.P50),
				menuItemFactory.CreateMenuItem(ViewMenu.ResourceInfo.P75),
				menuItemFactory.CreateMenuItem(ViewMenu.ResourceInfo.P100),
				menuItemFactory.CreateMenuItem(ViewMenu.ResourceInfo.P125),
				menuItemFactory.CreateMenuItem(ViewMenu.ResourceInfo.P150),
				menuItemFactory.CreateMenuItem(ViewMenu.ResourceInfo.P200),
				this.m_CustomZoom
			};
			VmisMenuItemFactory vmisMenuItemFactory = menuItemFactory;
			MenuItemResourceInfo zoom = ViewMenu.ResourceInfo.Zoom;
			ToolStripItem[] zoomChildren = this.m_ZoomChildren;
			this.m_Zoom = vmisMenuItemFactory.CreateParentMenuItem(zoom, zoomChildren);
			this.m_View = menuItemFactory.CreateParentMenuItem(ViewMenu.ResourceInfo.View, new ToolStripItem[]
			{
				this.m_FullScreen,
				this.m_Toolbar,
				this.m_Enhanced,
				this.m_Zoom
			});
			this.m_FullScreen.ShortcutKeys = (Keys.LButton | Keys.RButton | Keys.ShiftKey | Keys.Control | Keys.Alt);
			this.m_FullScreen.SetShortcutKeyDisplayString();
			InteractiveSessionConfigurationOptions instance = InteractiveSessionConfigurationOptions.Instance;
			this.m_Toolbar.Checked = instance.ShowToolbar;
			this.m_Enhanced.Checked = false;
			this.m_Enhanced.Enabled = false;
			uint zoomLevel = instance.ZoomLevel;
			int num = this.m_ZoomLevels.Length;
			for (int i = 0; i < this.m_ZoomLevels.Length; i++)
			{
				if (zoomLevel == (uint)this.m_ZoomLevels[i])
				{
					num = i;
					break;
				}
			}
			this.m_ZoomChildren[num].Checked = true;
			base.MenuItems = new VmisMenuItem[]
			{
				this.m_FullScreen,
				this.m_Toolbar,
				this.m_Enhanced,
				this.m_Zoom
			};
		}

		// Token: 0x0600027C RID: 636 RVA: 0x000135FC File Offset: 0x000117FC
		protected override void OnMenuItem(object sender, EventArgs ea)
		{
			if (sender == this.m_FullScreen)
			{
				base.MenuTarget.GoFullScreen();
				return;
			}
			if (sender == this.m_Enhanced)
			{
				this.m_Performer.OnEnhancedTriggered();
				return;
			}
			if (sender == this.m_Toolbar)
			{
				this.m_Toolbar.Checked = !this.m_Toolbar.Checked;
				base.MenuTarget.ShowToolbar(this.m_Toolbar.Checked);
				return;
			}
			if (sender == this.m_CustomZoom)
			{
				CustomZoomDialog customZoomDialog = new CustomZoomDialog();
				customZoomDialog.DisplayZoom = (uint)base.MenuTarget.CurrentZoomLevel;
				if (customZoomDialog.ShowDialog(base.MenuTarget.DialogOwner) == DialogResult.OK)
				{
					this.m_Performer.OnZoomLevelChanged((ZoomLevel)customZoomDialog.DisplayZoom);
					this.UpdateZoomChecks();
					return;
				}
			}
			else
			{
				for (int i = 0; i < this.m_ZoomChildren.Length; i++)
				{
					VmisMenuItem vmisMenuItem = this.m_ZoomChildren[i];
					if (sender == vmisMenuItem)
					{
						vmisMenuItem.Checked = true;
						this.m_Performer.OnZoomLevelChanged(this.m_ZoomLevels[i]);
					}
					else
					{
						vmisMenuItem.Checked = false;
					}
				}
			}
		}

		// Token: 0x0600027D RID: 637 RVA: 0x000136FB File Offset: 0x000118FB
		public override VmisMenuItem[] GetMenuItems()
		{
			return new VmisMenuItem[]
			{
				this.m_View
			};
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0001370C File Offset: 0x0001190C
		public override void UpdateEnabledState()
		{
			int state = (int)base.MenuTarget.VirtualMachine.State;
			this.m_FullScreen.Enabled = (base.MenuTarget.IsRdpConnected && base.MenuTarget.IsRdpEnabled);
			if (state != 2)
			{
				this.m_Enhanced.Checked = false;
				this.m_Enhanced.Enabled = false;
				this.m_Zoom.Enabled = false;
				return;
			}
			switch (base.MenuTarget.RdpMode)
			{
			case RdpConnectionMode.InTransition:
			case RdpConnectionMode.Basic:
				this.m_Enhanced.Checked = false;
				this.m_Enhanced.Enabled = false;
				this.m_Zoom.Enabled = true;
				return;
			case RdpConnectionMode.EnhancedAvailable:
				this.m_Enhanced.Checked = false;
				this.m_Enhanced.Enabled = true;
				this.m_Zoom.Enabled = true;
				return;
			case RdpConnectionMode.Enhanced:
				this.m_Enhanced.Checked = true;
				this.m_Enhanced.Enabled = !base.MenuTarget.IsShieldedVm();
				this.m_Zoom.Enabled = false;
				return;
			default:
				return;
			}
		}

		// Token: 0x0600027F RID: 639 RVA: 0x00013818 File Offset: 0x00011A18
		public override bool InformKeyUp(Keys key)
		{
			bool result = false;
			if (this.m_FullScreen.Enabled && (key == this.m_FullScreen.ShortcutKeys || key == ViewMenu.gm_FullScreenKeysAlternate))
			{
				this.OnMenuItem(this.m_FullScreen, EventArgs.Empty);
				result = true;
			}
			return result;
		}

		// Token: 0x06000280 RID: 640 RVA: 0x00013860 File Offset: 0x00011A60
		private void UpdateZoomChecks()
		{
			ZoomLevel currentZoomLevel = base.MenuTarget.CurrentZoomLevel;
			bool flag = false;
			for (int i = 0; i < this.m_ZoomChildren.Length - 1; i++)
			{
				VmisMenuItem vmisMenuItem = this.m_ZoomChildren[i];
				vmisMenuItem.Checked = (currentZoomLevel == this.m_ZoomLevels[i]);
				if (vmisMenuItem.Checked)
				{
					flag = true;
				}
			}
			this.m_CustomZoom.Checked = !flag;
		}

		// Token: 0x0400017D RID: 381
		private static Keys gm_FullScreenKeysAlternate = Keys.LButton | Keys.RButton | Keys.Control | Keys.Alt;

		// Token: 0x0400017E RID: 382
		private VmisMenuItem m_FullScreen;

		// Token: 0x0400017F RID: 383
		private VmisMenuItem m_View;

		// Token: 0x04000180 RID: 384
		private VmisMenuItem m_Toolbar;

		// Token: 0x04000181 RID: 385
		private VmisMenuItem m_Enhanced;

		// Token: 0x04000182 RID: 386
		private ZoomLevel[] m_ZoomLevels = new ZoomLevel[]
		{
			ZoomLevel.Auto,
			ZoomLevel.P25,
			ZoomLevel.P50,
			ZoomLevel.P75,
			ZoomLevel.P100,
			ZoomLevel.P125,
			ZoomLevel.P150,
			ZoomLevel.P200
		};

		// Token: 0x04000183 RID: 387
		private VmisMenuItem m_CustomZoom;

		// Token: 0x04000184 RID: 388
		private VmisMenuItem[] m_ZoomChildren;

		// Token: 0x04000185 RID: 389
		private VmisMenuItem m_Zoom;

		// Token: 0x04000186 RID: 390
		private VMConnectUserActionPerformer m_Performer;

		// Token: 0x0200005D RID: 93
		private static class ResourceInfo
		{
			// Token: 0x04000204 RID: 516
			private const string Append = "ViewMenu_";

			// Token: 0x04000205 RID: 517
			public static readonly MenuItemResourceInfo FullScreen = new MenuItemResourceInfo("ViewMenu_FullScreen", false);

			// Token: 0x04000206 RID: 518
			public static readonly MenuItemResourceInfo View = new MenuItemResourceInfo("ViewMenu_View", false);

			// Token: 0x04000207 RID: 519
			public static readonly MenuItemResourceInfo Toolbar = new MenuItemResourceInfo("ViewMenu_Toolbar", false);

			// Token: 0x04000208 RID: 520
			public static readonly MenuItemResourceInfo Enhanced = new MenuItemResourceInfo("ViewMenu_Enhanced", false);

			// Token: 0x04000209 RID: 521
			public static readonly MenuItemResourceInfo Zoom = new MenuItemResourceInfo("ViewMenu_Zoom", false);

			// Token: 0x0400020A RID: 522
			private const string ZoomLevel = "Zoom_";

			// Token: 0x0400020B RID: 523
			public static readonly MenuItemResourceInfo PAuto = new MenuItemResourceInfo("ViewMenu_Zoom_Auto", false);

			// Token: 0x0400020C RID: 524
			public static readonly MenuItemResourceInfo PCustom = new MenuItemResourceInfo("ViewMenu_Zoom_Custom", false);

			// Token: 0x0400020D RID: 525
			public static readonly MenuItemResourceInfo P25 = new MenuItemResourceInfo("ViewMenu_Zoom_25", false);

			// Token: 0x0400020E RID: 526
			public static readonly MenuItemResourceInfo P50 = new MenuItemResourceInfo("ViewMenu_Zoom_50", false);

			// Token: 0x0400020F RID: 527
			public static readonly MenuItemResourceInfo P75 = new MenuItemResourceInfo("ViewMenu_Zoom_75", false);

			// Token: 0x04000210 RID: 528
			public static readonly MenuItemResourceInfo P100 = new MenuItemResourceInfo("ViewMenu_Zoom_100", false);

			// Token: 0x04000211 RID: 529
			public static readonly MenuItemResourceInfo P125 = new MenuItemResourceInfo("ViewMenu_Zoom_125", false);

			// Token: 0x04000212 RID: 530
			public static readonly MenuItemResourceInfo P150 = new MenuItemResourceInfo("ViewMenu_Zoom_150", false);

			// Token: 0x04000213 RID: 531
			public static readonly MenuItemResourceInfo P200 = new MenuItemResourceInfo("ViewMenu_Zoom_200", false);
		}
	}
}
