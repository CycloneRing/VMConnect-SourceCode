using System;
using System.Windows.Forms;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x0200002E RID: 46
	internal abstract class MenuItemProvider : IMenuItemProvider
	{
		// Token: 0x06000239 RID: 569 RVA: 0x0001195C File Offset: 0x0000FB5C
		protected MenuItemProvider(IMenuActionTarget menuTarget)
		{
			if (menuTarget == null)
			{
				throw new ArgumentNullException("menuTarget");
			}
			this.m_MenuTarget = menuTarget;
		}

		// Token: 0x0600023A RID: 570 RVA: 0x00011979 File Offset: 0x0000FB79
		protected VmisMenuItemFactory GetMenuItemFactory()
		{
			return new VmisMenuItemFactory(new EventHandler(this.OnMenuItem));
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x0600023B RID: 571 RVA: 0x0001198D File Offset: 0x0000FB8D
		// (set) Token: 0x0600023C RID: 572 RVA: 0x00011995 File Offset: 0x0000FB95
		protected VmisMenuItem[] MenuItems
		{
			get
			{
				return this.m_MenuItems;
			}
			set
			{
				this.m_MenuItems = value;
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x0600023D RID: 573 RVA: 0x0001199E File Offset: 0x0000FB9E
		protected IMenuActionTarget MenuTarget
		{
			get
			{
				return this.m_MenuTarget;
			}
		}

		// Token: 0x0600023E RID: 574 RVA: 0x000119A6 File Offset: 0x0000FBA6
		protected virtual void OnMenuItem(object sender, EventArgs ea)
		{
			Program.Displayer.DisplayInformation("Not yet implemeted.", "");
		}

		// Token: 0x0600023F RID: 575 RVA: 0x000119B8 File Offset: 0x0000FBB8
		public virtual bool InformKeyUp(Keys key)
		{
			if (this.m_MenuItems != null)
			{
				foreach (VmisMenuItem vmisMenuItem in this.m_MenuItems)
				{
					if (vmisMenuItem.Enabled && vmisMenuItem.ShortcutKeys == key)
					{
						this.OnMenuItem(vmisMenuItem, EventArgs.Empty);
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000240 RID: 576 RVA: 0x00011A06 File Offset: 0x0000FC06
		public virtual void UpdateEnabledState()
		{
		}

		// Token: 0x06000241 RID: 577 RVA: 0x00011A08 File Offset: 0x0000FC08
		public virtual void InformBeginStateChangeOperation(VMStateChangeAction stateChange)
		{
		}

		// Token: 0x06000242 RID: 578 RVA: 0x00011A0A File Offset: 0x0000FC0A
		public virtual void InformEndStateChangeOperation(VMStateChangeAction stateChange)
		{
		}

		// Token: 0x06000243 RID: 579 RVA: 0x00011A0C File Offset: 0x0000FC0C
		public virtual void InformVMConfigurationChanged()
		{
		}

		// Token: 0x06000244 RID: 580 RVA: 0x00011A0E File Offset: 0x0000FC0E
		public virtual void CloseMenu()
		{
		}

		// Token: 0x06000245 RID: 581 RVA: 0x00011A10 File Offset: 0x0000FC10
		public virtual VmisMenuItem[] GetMenuItems()
		{
			return this.m_MenuItems;
		}

		// Token: 0x04000163 RID: 355
		private VmisMenuItem[] m_MenuItems;

		// Token: 0x04000164 RID: 356
		private IMenuActionTarget m_MenuTarget;
	}
}
