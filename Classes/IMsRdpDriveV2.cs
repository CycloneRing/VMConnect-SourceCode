using System;
using System.Runtime.InteropServices;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x0200000E RID: 14
	[Guid("3e05417c-2721-4008-9d80-4edf1539c817")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IMsRdpDriveV2
	{
		// Token: 0x060000D7 RID: 215
		[return: MarshalAs(UnmanagedType.BStr)]
		string Name();

		// Token: 0x060000D8 RID: 216
		void RedirectionState([MarshalAs(UnmanagedType.Bool)] [In] bool RedirState);

		// Token: 0x060000D9 RID: 217
		[return: MarshalAs(UnmanagedType.Bool)]
		bool RedirectionState();

		// Token: 0x060000DA RID: 218
		[return: MarshalAs(UnmanagedType.U4)]
		uint DriveLetterIndex();
	}
}
