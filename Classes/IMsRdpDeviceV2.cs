using System;
using System.Runtime.InteropServices;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x0200000D RID: 13
	[Guid("5fb94466-7661-42a8-98b7-01904c11668f")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IMsRdpDeviceV2
	{
		// Token: 0x060000CE RID: 206
		[return: MarshalAs(UnmanagedType.BStr)]
		string DeviceInstanceId();

		// Token: 0x060000CF RID: 207
		[return: MarshalAs(UnmanagedType.BStr)]
		string FriendlyName();

		// Token: 0x060000D0 RID: 208
		[return: MarshalAs(UnmanagedType.BStr)]
		string DeviceDescription();

		// Token: 0x060000D1 RID: 209
		void RedirectionState([MarshalAs(UnmanagedType.Bool)] [In] bool RedirState);

		// Token: 0x060000D2 RID: 210
		[return: MarshalAs(UnmanagedType.Bool)]
		bool RedirectionState();

		// Token: 0x060000D3 RID: 211
		[return: MarshalAs(UnmanagedType.BStr)]
		string DeviceText();

		// Token: 0x060000D4 RID: 212
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsUSBDevice();

		// Token: 0x060000D5 RID: 213
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsCompositeDevice();

		// Token: 0x060000D6 RID: 214
		[return: MarshalAs(UnmanagedType.U4)]
		uint DriveLetterBitmap();
	}
}
