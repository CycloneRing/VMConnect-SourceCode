using System;
using System.Windows.Forms;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x0200002D RID: 45
	internal interface IMenuItemProvider
	{
		// Token: 0x06000232 RID: 562
		bool InformKeyUp(Keys key);

		// Token: 0x06000233 RID: 563
		void UpdateEnabledState();

		// Token: 0x06000234 RID: 564
		void InformBeginStateChangeOperation(VMStateChangeAction stateChange);

		// Token: 0x06000235 RID: 565
		void InformEndStateChangeOperation(VMStateChangeAction stateChange);

		// Token: 0x06000236 RID: 566
		void InformVMConfigurationChanged();

		// Token: 0x06000237 RID: 567
		void CloseMenu();

		// Token: 0x06000238 RID: 568
		VmisMenuItem[] GetMenuItems();
	}
}
