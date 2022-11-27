using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000030 RID: 48
	internal class MenuManager
	{
		// Token: 0x0600024E RID: 590 RVA: 0x00011B54 File Offset: 0x0000FD54
		public MenuManager(IMenuActionTarget target)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			this.m_MenuTarget = target;
			this.m_MenuItemProviders = new List<IMenuItemProvider>(7);
			this.m_MenuItemProviders.Add(new FileMenu(target));
			this.m_MenuItemProviders.Add(new ActionMenu(target));
			this.m_MenuItemProviders.Add(new MediaMenu(target));
			if (this.m_MenuTarget.RdpMode == RdpConnectionMode.Basic || this.m_MenuTarget.RdpMode == RdpConnectionMode.EnhancedAvailable)
			{
				this.m_MenuItemProviders.Add(new ClipboardMenu(target));
			}
			this.m_MenuItemProviders.Add(new ViewMenu(target));
			this.m_MenuItemProviders.Add(new HelpMenu(target));
			this.m_MainMenu = new MenuStrip();
			this.m_MainMenu.Renderer = new ToolStripScaleAwareProfessionalRenderer();
			foreach (IMenuItemProvider provider in this.m_MenuItemProviders)
			{
				this.AddTopLevelMenuItem(provider);
			}
			(target as IDpiForm).DpiChanged += this.OnDpiChanged;
			(target as Control).VisibleChanged += this.OnVisibleChanged;
		}

		// Token: 0x0600024F RID: 591 RVA: 0x00011C9C File Offset: 0x0000FE9C
		private void OnVisibleChanged(object sender, EventArgs e)
		{
			this.UpdateDropDownSizes();
		}

		// Token: 0x06000250 RID: 592 RVA: 0x00011CA4 File Offset: 0x0000FEA4
		private void OnDpiChanged(object sender, System.Windows.Forms.DpiChangedEventArgs e)
		{
			if (e.DeviceDpiOld == e.DeviceDpiNew)
			{
				return;
			}
			if (this.m_OriginalMainMenuFontSizeInPoints == 0f)
			{
				this.m_OriginalMainMenuFontSizeInPoints = (sender as Control).DeviceToLogicalUnits(this.m_MainMenu.Font.SizeInPoints, e.DeviceDpiOld);
			}
			this.m_MainMenu.Font = new Font(this.m_MainMenu.Font.FontFamily, (sender as Control).LogicalToDeviceUnits(this.m_OriginalMainMenuFontSizeInPoints));
			this.UpdateDropDownSizes();
		}

		// Token: 0x06000251 RID: 593 RVA: 0x00011D2C File Offset: 0x0000FF2C
		private void UpdateDropDownSizes()
		{
			foreach (object obj in this.m_MainMenu.Items)
			{
				((VmisMenuItem)obj).UpdateDropDownSizes();
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000252 RID: 594 RVA: 0x00011D88 File Offset: 0x0000FF88
		public MenuStrip MainMenu
		{
			get
			{
				return this.m_MainMenu;
			}
		}

		// Token: 0x06000253 RID: 595 RVA: 0x00011D90 File Offset: 0x0000FF90
		public void UpdateEnabledState()
		{
			if (this.m_MenuTarget.RdpMode == RdpConnectionMode.Basic || this.m_MenuTarget.RdpMode == RdpConnectionMode.EnhancedAvailable)
			{
				if (!(this.m_MenuItemProviders[3] is ClipboardMenu))
				{
					this.m_MenuItemProviders.Insert(3, new ClipboardMenu(this.m_MenuTarget));
					this.AddTopLevelMenuItem(3, this.m_MenuItemProviders[3]);
				}
			}
			else if (this.m_MenuItemProviders[3] is ClipboardMenu)
			{
				this.RemoveTopLevelMenuItem(this.m_MenuItemProviders[3]);
				this.m_MenuItemProviders.RemoveAt(3);
			}
			foreach (IMenuItemProvider menuItemProvider in this.m_MenuItemProviders)
			{
				menuItemProvider.UpdateEnabledState();
			}
			this.UpdateDropDownSizes();
		}

		// Token: 0x06000254 RID: 596 RVA: 0x00011E74 File Offset: 0x00010074
		public bool InformKeyUp(Keys key)
		{
			bool result = false;
			using (List<IMenuItemProvider>.Enumerator enumerator = this.m_MenuItemProviders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.InformKeyUp(key))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		// Token: 0x06000255 RID: 597 RVA: 0x00011ED0 File Offset: 0x000100D0
		public virtual void InformBeginStateChangeOperation(VMStateChangeAction stateChange)
		{
			foreach (IMenuItemProvider menuItemProvider in this.m_MenuItemProviders)
			{
				menuItemProvider.InformBeginStateChangeOperation(stateChange);
			}
		}

		// Token: 0x06000256 RID: 598 RVA: 0x00011F24 File Offset: 0x00010124
		public virtual void InformEndStateChangeOperation(VMStateChangeAction stateChange)
		{
			foreach (IMenuItemProvider menuItemProvider in this.m_MenuItemProviders)
			{
				menuItemProvider.InformEndStateChangeOperation(stateChange);
			}
		}

		// Token: 0x06000257 RID: 599 RVA: 0x00011F78 File Offset: 0x00010178
		public void InformVMConfigurationChange()
		{
			foreach (IMenuItemProvider menuItemProvider in this.m_MenuItemProviders)
			{
				menuItemProvider.InformVMConfigurationChanged();
			}
		}

		// Token: 0x06000258 RID: 600 RVA: 0x00011FC8 File Offset: 0x000101C8
		public void CloseMenu()
		{
			foreach (IMenuItemProvider menuItemProvider in this.m_MenuItemProviders)
			{
				menuItemProvider.CloseMenu();
			}
		}

		// Token: 0x06000259 RID: 601 RVA: 0x00012018 File Offset: 0x00010218
		private void AddTopLevelMenuItem(IMenuItemProvider provider)
		{
			this.m_MainMenu.Items.Add(provider.GetMenuItems()[0]);
		}

		// Token: 0x0600025A RID: 602 RVA: 0x00012033 File Offset: 0x00010233
		private void AddTopLevelMenuItem(int index, IMenuItemProvider provider)
		{
			this.m_MainMenu.Items.Insert(index, provider.GetMenuItems()[0]);
		}

		// Token: 0x0600025B RID: 603 RVA: 0x0001204E File Offset: 0x0001024E
		private void RemoveTopLevelMenuItem(IMenuItemProvider provider)
		{
			this.m_MainMenu.Items.Remove(provider.GetMenuItems()[0]);
		}

		// Token: 0x04000169 RID: 361
		private const int NUM_MENUS = 7;

		// Token: 0x0400016A RID: 362
		private const int CLIPBOARD_INDEX = 3;

		// Token: 0x0400016B RID: 363
		private List<IMenuItemProvider> m_MenuItemProviders;

		// Token: 0x0400016C RID: 364
		private MenuStrip m_MainMenu;

		// Token: 0x0400016D RID: 365
		private IMenuActionTarget m_MenuTarget;

		// Token: 0x0400016E RID: 366
		private float m_OriginalMainMenuFontSizeInPoints;
	}
}
