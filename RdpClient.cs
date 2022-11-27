using System;
using System.Windows.Forms;
using AxMicrosoft.Virtualization.Client.Interop;
using Microsoft.Virtualization.Client.Interop;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000011 RID: 17
	internal class RdpClient : AxMsRdpClient9NotSafeForScripting
	{
		// Token: 0x14000003 RID: 3
		// (add) Token: 0x060000E6 RID: 230 RVA: 0x0000A070 File Offset: 0x00008270
		// (remove) Token: 0x060000E7 RID: 231 RVA: 0x0000A0A8 File Offset: 0x000082A8
		public event EventHandler MouseActivate;

		// Token: 0x060000E8 RID: 232 RVA: 0x0000A0DD File Offset: 0x000082DD
		public RdpClient()
		{
			base.SetStyle(ControlStyles.EnableNotifyMessage, true);
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x0000A0F1 File Offset: 0x000082F1
		protected override void OnNotifyMessage(Message m)
		{
			base.OnNotifyMessage(m);
			if (m.Msg == 33 && this.MouseActivate != null)
			{
				this.MouseActivate(this, EventArgs.Empty);
			}
		}

		// Token: 0x060000EA RID: 234 RVA: 0x0000A120 File Offset: 0x00008320
		public void OnNotifyRedirectDeviceChange(IntPtr wParam, IntPtr lParam)
		{
			IMsRdpClientNonScriptable msRdpClientNonScriptable = (IMsRdpClientNonScriptable)base.GetOcx();
			if (msRdpClientNonScriptable != null)
			{
				msRdpClientNonScriptable.NotifyRedirectDeviceChange((ulong)wParam.ToInt64(), lParam.ToInt64());
			}
		}
	}
}
