using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x0200001B RID: 27
	[SuppressUnmanagedCodeSecurity]
	internal static class NativeMethods
	{
		// Token: 0x060001A8 RID: 424
		[DllImport("user32.dll")]
		internal static extern bool GetCursorInfo(ref NativeMethods.CURSORINFO pci);

		// Token: 0x0200004A RID: 74
		internal struct CURSORINFO
		{
			// Token: 0x040001C2 RID: 450
			public int cbSize;

			// Token: 0x040001C3 RID: 451
			public int flags;

			// Token: 0x040001C4 RID: 452
			public IntPtr hCursor;

			// Token: 0x040001C5 RID: 453
			public Point ptScreenPos;
		}
	}
}
