using System;
using System.Windows.Forms;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000024 RID: 36
	internal class VmisToolStripButton : ToolStripButton
	{
		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060001F1 RID: 497 RVA: 0x00010DAC File Offset: 0x0000EFAC
		// (set) Token: 0x060001F2 RID: 498 RVA: 0x00010DB4 File Offset: 0x0000EFB4
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

		// Token: 0x04000149 RID: 329
		private VMControlAction m_Action;
	}
}
