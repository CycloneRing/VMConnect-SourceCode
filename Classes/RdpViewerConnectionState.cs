using System;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000015 RID: 21
	internal enum RdpViewerConnectionState
	{
		// Token: 0x040000B4 RID: 180
		NoVirtualMachine,
		// Token: 0x040000B5 RID: 181
		NotConnected,
		// Token: 0x040000B6 RID: 182
		Connecting,
		// Token: 0x040000B7 RID: 183
		ConnectedNoVideo,
		// Token: 0x040000B8 RID: 184
		ConnectedBasicVideo,
		// Token: 0x040000B9 RID: 185
		ConnectedEnhancedVideo,
		// Token: 0x040000BA RID: 186
		ConnectedEnhancedVideoSyncedSession
	}
}
