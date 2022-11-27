using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x0200000F RID: 15
	public class NativeWindowDoubleClickInterceptor : NativeWindow
	{
		// Token: 0x060000DB RID: 219 RVA: 0x00009DC0 File Offset: 0x00007FC0
		public NativeWindowDoubleClickInterceptor(IEnumerable<Control> parents)
		{
			if (parents != null)
			{
				foreach (Control control in parents)
				{
					control.HandleCreated += this.HandleCreated;
					control.HandleDestroyed += this.HandleDestroyed;
				}
			}
			this.m_Parents = parents;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00009E34 File Offset: 0x00008034
		private void HandleCreated(object sender, EventArgs e)
		{
			try
			{
				base.AssignHandle(((Control)sender).Handle);
			}
			catch (Win32Exception ex)
			{
				VMTrace.TraceError("Exception occured while user control window handle.", ex);
			}
			catch (Exception ex2)
			{
				VMTrace.TraceError("Unexpected exception. Custom intercept control already has a handle assigned to it.", ex2);
			}
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00009E8C File Offset: 0x0000808C
		private void HandleDestroyed(object sender, EventArgs e)
		{
			this.ReleaseHandle();
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00009E94 File Offset: 0x00008094
		protected override void WndProc(ref Message m)
		{
			if (m.Msg != 515)
			{
				base.WndProc(ref m);
				return;
			}
			m.Result = IntPtr.Zero;
		}

		// Token: 0x040000A4 RID: 164
		private const int WM_LBUTTONDBLCLK = 515;

		// Token: 0x040000A5 RID: 165
		private IEnumerable<Control> m_Parents;
	}
}
