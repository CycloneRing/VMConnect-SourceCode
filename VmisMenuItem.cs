using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000029 RID: 41
	internal class VmisMenuItem : ToolStripMenuItem
	{
		// Token: 0x0600020E RID: 526 RVA: 0x0001174D File Offset: 0x0000F94D
		public VmisMenuItem(string text) : base(text)
		{
		}

		// Token: 0x0600020F RID: 527 RVA: 0x00011756 File Offset: 0x0000F956
		public VmisMenuItem(string text, EventHandler onClick) : base(text, null, onClick)
		{
		}

		// Token: 0x06000210 RID: 528 RVA: 0x00011761 File Offset: 0x0000F961
		public VmisMenuItem(string text, EventHandler onClick, Keys shortcutKey) : base(text, null, onClick, shortcutKey)
		{
			this.SetShortcutKeyDisplayString();
		}

		// Token: 0x06000211 RID: 529 RVA: 0x00011773 File Offset: 0x0000F973
		public VmisMenuItem(string text, params ToolStripItem[] items) : base(text, null, items)
		{
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000212 RID: 530 RVA: 0x0001177E File Offset: 0x0000F97E
		// (set) Token: 0x06000213 RID: 531 RVA: 0x00011786 File Offset: 0x0000F986
		public VMControlAction Action
		{
			get
			{
				return this.m_Action;
			}
			set
			{
				this.m_Action = value;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000214 RID: 532 RVA: 0x0001178F File Offset: 0x0000F98F
		public static string EndEllipses
		{
			get
			{
				return "...";
			}
		}

		// Token: 0x06000215 RID: 533 RVA: 0x00011798 File Offset: 0x0000F998
		internal void SetShortcutKeyDisplayString()
		{
			if (base.ShortcutKeys == Keys.None)
			{
				return;
			}
			base.ShortcutKeyDisplayString = VmisMenuItem.gm_Converter.ConvertToString(null, CultureInfo.CurrentUICulture, base.ShortcutKeys);
			base.ShortcutKeyDisplayString = base.ShortcutKeyDisplayString.Replace(VMISResources.Menu_PauseKey, VMISResources.Menu_BreakKey);
		}

		// Token: 0x06000216 RID: 534 RVA: 0x000117EC File Offset: 0x0000F9EC
		internal void UpdateDropDownSizes()
		{
			if (base.HasDropDown)
			{
				base.DropDown.AutoSize = true;
				foreach (object obj in base.DropDownItems)
				{
					ToolStripItem toolStripItem = (ToolStripItem)obj;
					toolStripItem.AutoSize = true;
					toolStripItem.AutoSize = false;
				}
				base.DropDown.AutoSize = false;
				base.DropDown.Size = new Size(base.DropDown.Size.Width + ToolStripScaleAwareProfessionalRenderer.ToolStripItemWidthScaleDelta, base.DropDown.Size.Height);
				foreach (object obj2 in base.DropDownItems)
				{
					ToolStripItem toolStripItem2 = (ToolStripItem)obj2;
					toolStripItem2.Size = new Size(toolStripItem2.Width + ToolStripScaleAwareProfessionalRenderer.ToolStripItemWidthScaleDelta, toolStripItem2.Height);
				}
			}
		}

		// Token: 0x0400015B RID: 347
		private static KeysConverter gm_Converter = new KeysConverter();

		// Token: 0x0400015C RID: 348
		private VMControlAction m_Action;
	}
}
