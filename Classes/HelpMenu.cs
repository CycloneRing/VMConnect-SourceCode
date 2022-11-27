using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000028 RID: 40
	internal class HelpMenu : MenuItemProvider, IMenuItemProvider
	{
		// Token: 0x0600020A RID: 522 RVA: 0x0001164C File Offset: 0x0000F84C
		public HelpMenu(IMenuActionTarget menuTarget) : base(menuTarget)
		{
			VmisMenuItemFactory menuItemFactory = base.GetMenuItemFactory();
			this.m_LaunchHelp = menuItemFactory.CreateMenuItem(HelpMenu.ResourceInfo.LaunchHelp);
			this.m_About = menuItemFactory.CreateMenuItem(HelpMenu.ResourceInfo.About);
			this.m_HelpMenu = menuItemFactory.CreateParentMenuItem(HelpMenu.ResourceInfo.Help, new ToolStripItem[]
			{
				this.m_LaunchHelp,
				menuItemFactory.CreateMenuItemSeparator(),
				this.m_About
			});
			base.MenuItems = new VmisMenuItem[]
			{
				this.m_LaunchHelp,
				this.m_About
			};
		}

		// Token: 0x0600020B RID: 523 RVA: 0x000116DC File Offset: 0x0000F8DC
		[SuppressMessage("Microsoft.Performance", "CA1806:DoNotIgnoreMethodResults")]
		protected override void OnMenuItem(object sender, EventArgs ea)
		{
			if (sender == this.m_About)
			{
				HelpMenu.ShellAbout(base.MenuTarget.DialogOwner.Handle, VMISResources.AboutDialogTitle, null, IntPtr.Zero);
				return;
			}
			if (sender == this.m_LaunchHelp)
			{
				CommonUtilities.LaunchHelpUrl(HelpConstants.VMConnectOverviewUrl, Program.Displayer.DisplayErrorCallback, base.MenuTarget.AsyncUIThreadMethodInvoker);
			}
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0001173C File Offset: 0x0000F93C
		public override VmisMenuItem[] GetMenuItems()
		{
			return new VmisMenuItem[]
			{
				this.m_HelpMenu
			};
		}

		// Token: 0x0600020D RID: 525
		[DllImport("shell32.dll", CharSet = CharSet.Unicode)]
		private static extern int ShellAbout(IntPtr hWnd, string szApp, string szOtherStuff, IntPtr hIcon);

		// Token: 0x04000158 RID: 344
		private VmisMenuItem m_HelpMenu;

		// Token: 0x04000159 RID: 345
		private VmisMenuItem m_LaunchHelp;

		// Token: 0x0400015A RID: 346
		private VmisMenuItem m_About;

		// Token: 0x02000053 RID: 83
		private static class ResourceInfo
		{
			// Token: 0x040001EF RID: 495
			private const string Append = "HelpMenu_";

			// Token: 0x040001F0 RID: 496
			public static readonly MenuItemResourceInfo LaunchHelp = new MenuItemResourceInfo("HelpMenu_LaunchHelp", true);

			// Token: 0x040001F1 RID: 497
			public static readonly MenuItemResourceInfo About = new MenuItemResourceInfo("HelpMenu_About", false);

			// Token: 0x040001F2 RID: 498
			public static readonly MenuItemResourceInfo Help = new MenuItemResourceInfo("HelpMenu_Help", false);
		}
	}
}
