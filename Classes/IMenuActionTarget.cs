using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Virtualization.Client.Management;
using Microsoft.Virtualization.Client.Settings;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x0200002C RID: 44
	internal interface IMenuActionTarget
	{
		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600021D RID: 541
		IVMComputerSystem VirtualMachine { get; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x0600021E RID: 542
		IWin32Window DialogOwner { get; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600021F RID: 543
		bool IsRdpConnected { get; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000220 RID: 544
		bool IsRdpConnecting { get; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000221 RID: 545
		RdpConnectionMode RdpMode { get; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000222 RID: 546
		VmisStatusStrip StatusBar { get; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000223 RID: 547
		ThreadMethodInvoker AsyncUIThreadMethodInvoker { get; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000224 RID: 548
		bool HasSnapshots { get; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000225 RID: 549
		ZoomLevel CurrentZoomLevel { get; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000226 RID: 550
		bool IsRdpEnabled { get; }

		// Token: 0x06000227 RID: 551
		void ToggleEnhanced();

		// Token: 0x06000228 RID: 552
		void GoFullScreen();

		// Token: 0x06000229 RID: 553
		void Exit();

		// Token: 0x0600022A RID: 554
		void DeactivateOnDialogClosed();

		// Token: 0x0600022B RID: 555
		Image CopyScreenImage();

		// Token: 0x0600022C RID: 556
		VMSettingsDialog OpenVMSettingsDialog();

		// Token: 0x0600022D RID: 557
		void InformBeginStateChangeOperation(VMStateChangeAction stateChange);

		// Token: 0x0600022E RID: 558
		void InformEndStateChangeOperation(VMStateChangeAction stateChange);

		// Token: 0x0600022F RID: 559
		bool IsShieldedVm();

		// Token: 0x06000230 RID: 560
		void ShowToolbar(bool show);

		// Token: 0x06000231 RID: 561
		void SetZoomLevel(ZoomLevel level);
	}
}
