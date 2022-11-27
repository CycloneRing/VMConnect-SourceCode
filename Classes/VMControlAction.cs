using System;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x0200001F RID: 31
	internal enum VMControlAction
	{
		// Token: 0x0400011B RID: 283
		None,
		// Token: 0x0400011C RID: 284
		StartTurnOff,
		// Token: 0x0400011D RID: 285
		Shutdown,
		// Token: 0x0400011E RID: 286
		PauseResume,
		// Token: 0x0400011F RID: 287
		Revert,
		// Token: 0x04000120 RID: 288
		Snapshot,
		// Token: 0x04000121 RID: 289
		Save,
		// Token: 0x04000122 RID: 290
		Reset,
		// Token: 0x04000123 RID: 291
		Enhanced,
		// Token: 0x04000124 RID: 292
		Share
	}
}
