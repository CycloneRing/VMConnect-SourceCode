using System;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x0200001D RID: 29
	internal interface IVMConnectCommunicatorHelper
	{
		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060001B5 RID: 437
		string CurrentVMServerName { get; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060001B6 RID: 438
		string CurrentVMInstanceId { get; }

		// Token: 0x060001B7 RID: 439
		void ActivateSelf();
	}
}
