using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x0200002A RID: 42
	internal class VmisMenuItemFactory
	{
		// Token: 0x06000218 RID: 536 RVA: 0x00011914 File Offset: 0x0000FB14
		public VmisMenuItemFactory(EventHandler onMenuItem)
		{
			this.m_OnMenuItem = onMenuItem;
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000219 RID: 537 RVA: 0x00011923 File Offset: 0x0000FB23
		public EventHandler OnMenuItem
		{
			get
			{
				return this.m_OnMenuItem;
			}
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0001192B File Offset: 0x0000FB2B
		public VmisMenuItem CreateMenuItem(MenuItemResourceInfo menuInfo)
		{
			return new VmisMenuItem(menuInfo.GetText(), this.OnMenuItem, menuInfo.GetKey());
		}

		// Token: 0x0600021B RID: 539 RVA: 0x00011946 File Offset: 0x0000FB46
		public VmisMenuItem CreateParentMenuItem(MenuItemResourceInfo menuInfo, params ToolStripItem[] items)
		{
			return new VmisMenuItem(menuInfo.GetText(), items);
		}

		// Token: 0x0600021C RID: 540 RVA: 0x00011955 File Offset: 0x0000FB55
		[SuppressMessage("Microsoft.Globalization", "CA1303")]
		public ToolStripItem CreateMenuItemSeparator()
		{
			return new ToolStripSeparator();
		}

		// Token: 0x0400015D RID: 349
		private EventHandler m_OnMenuItem;
	}
}
