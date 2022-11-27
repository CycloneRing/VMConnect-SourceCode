using System;
using System.Windows.Forms;
using Microsoft.Virtualization.Client.Management;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000027 RID: 39
	internal class FileMenu : MenuItemProvider
	{
		// Token: 0x06000206 RID: 518 RVA: 0x0001152C File Offset: 0x0000F72C
		public FileMenu(IMenuActionTarget menuTarget) : base(menuTarget)
		{
			VmisMenuItemFactory menuItemFactory = base.GetMenuItemFactory();
			this.m_Settings = menuItemFactory.CreateMenuItem(FileMenu.ResourceInfo.Settings);
			this.m_Exit = menuItemFactory.CreateMenuItem(FileMenu.ResourceInfo.Exit);
			this.m_File = menuItemFactory.CreateParentMenuItem(FileMenu.ResourceInfo.File, new ToolStripItem[]
			{
				this.m_Settings,
				menuItemFactory.CreateMenuItemSeparator(),
				this.m_Exit
			});
			base.MenuItems = new VmisMenuItem[]
			{
				this.m_Settings,
				this.m_Exit
			};
		}

		// Token: 0x06000207 RID: 519 RVA: 0x000115BC File Offset: 0x0000F7BC
		protected override void OnMenuItem(object sender, EventArgs ea)
		{
			if (sender == this.m_Settings)
			{
				try
				{
					base.MenuTarget.OpenVMSettingsDialog();
					return;
				}
				catch (VirtualizationManagementException exception)
				{
					Program.Displayer.DisplayError(VMISResources.Error_OpeningSettings, exception);
					return;
				}
			}
			if (sender == this.m_Exit)
			{
				base.MenuTarget.Exit();
				return;
			}
			base.OnMenuItem(sender, ea);
		}

		// Token: 0x06000208 RID: 520 RVA: 0x00011620 File Offset: 0x0000F820
		public override void UpdateEnabledState()
		{
			this.m_Settings.Enabled = (base.MenuTarget.VirtualMachine != null);
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0001163B File Offset: 0x0000F83B
		public override VmisMenuItem[] GetMenuItems()
		{
			return new VmisMenuItem[]
			{
				this.m_File
			};
		}

		// Token: 0x04000155 RID: 341
		private VmisMenuItem m_Settings;

		// Token: 0x04000156 RID: 342
		private VmisMenuItem m_Exit;

		// Token: 0x04000157 RID: 343
		private VmisMenuItem m_File;

		// Token: 0x02000052 RID: 82
		private static class ResourceInfo
		{
			// Token: 0x040001EB RID: 491
			private const string Append = "FileMenu_";

			// Token: 0x040001EC RID: 492
			public static readonly MenuItemResourceInfo Settings = new MenuItemResourceInfo("FileMenu_Settings", true);

			// Token: 0x040001ED RID: 493
			public static readonly MenuItemResourceInfo Exit = new MenuItemResourceInfo("FileMenu_Exit", false);

			// Token: 0x040001EE RID: 494
			public static readonly MenuItemResourceInfo File = new MenuItemResourceInfo("FileMenu_File", false);
		}
	}
}
