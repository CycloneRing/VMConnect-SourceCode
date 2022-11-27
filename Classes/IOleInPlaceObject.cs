using System;
using System.Runtime.InteropServices;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000012 RID: 18
	[Guid("00000113-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IOleInPlaceObject
	{
		// Token: 0x060000EB RID: 235
		void GetWindow(out IntPtr phwnd);

		// Token: 0x060000EC RID: 236
		void ContextSensitiveHelp([MarshalAs(UnmanagedType.Bool)] [In] bool fEnterMode);

		// Token: 0x060000ED RID: 237
		void InPlaceDeactivate();

		// Token: 0x060000EE RID: 238
		void UIDeactivate();

		// Token: 0x060000EF RID: 239
		void SetObjectRects(IntPtr lprcPosRect, IntPtr lprcClipRect);

		// Token: 0x060000F0 RID: 240
		void ReactivateAndUndo();
	}
}
