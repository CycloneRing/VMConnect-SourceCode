using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Virtualization.Client.Management;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x0200001C RID: 28
	public partial class NoConnectionDialog : UserControl
	{
		// Token: 0x060001A9 RID: 425 RVA: 0x0000EB74 File Offset: 0x0000CD74
		internal NoConnectionDialog(IMenuActionTarget target)
		{
			this.InitializeComponent();
			this._logicalFontSize = DpiUtilities.SystemToLogicalUnits(base.FontSize);
			this._logicalTxtOneFontSize = DpiUtilities.SystemToLogicalUnits(this.txtOne.FontSize);
			base.Initialized += this.NoConnectionDialog_Initialized;
			this.btnTurnOn.Content = VMISResources.ToolStripToolTip_Start;
			this.btnResume.Content = VMISResources.ToolStripToolTip_Resume;
			this._actionTarget = target;
			this._performer = new VMConnectUserActionPerformer(this._actionTarget);
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000EBFE File Offset: 0x0000CDFE
		private void NoConnectionDialog_Initialized(object sender, EventArgs e)
		{
			this._oldDpi = VisualTreeHelper.GetDpi(sender as Visual);
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000EC14 File Offset: 0x0000CE14
		internal void UpdateDpi(IntPtr windowHandle)
		{
			this._windowHandle = windowHandle;
			DpiScale newDpi = new DpiScale((double)DpiUtilities.DeviceDpiScale(this._windowHandle), (double)DpiUtilities.DeviceDpiScale(this._windowHandle));
			this.OnDpiChanged(this._oldDpi, newDpi);
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000EC54 File Offset: 0x0000CE54
		protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
		{
			base.OnDpiChanged(oldDpi, newDpi);
			if (this._windowHandle != IntPtr.Zero)
			{
				base.FontSize = DpiUtilities.LogicalToDeviceUnits(this._windowHandle, this._logicalFontSize);
				this.txtOne.FontSize = DpiUtilities.LogicalToDeviceUnits(this._windowHandle, this._logicalTxtOneFontSize);
			}
			this._oldDpi = newDpi;
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000ECB5 File Offset: 0x0000CEB5
		internal void InformBeginStateChangeOperation(VMStateChangeAction stateChange)
		{
			this.btnTurnOn.Visibility = Visibility.Hidden;
		}

		// Token: 0x060001AE RID: 430 RVA: 0x0000ECC3 File Offset: 0x0000CEC3
		internal void InformEndStateChangeOperation(VMStateChangeAction stateChange)
		{
			this.UpdateEnabledState();
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000ECCB File Offset: 0x0000CECB
		internal void SetStrings(IReadOnlyList<string> displayStrings)
		{
			this.txtOne.Text = displayStrings[0];
			this.txtTwo.Text = displayStrings[1];
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000ECF4 File Offset: 0x0000CEF4
		internal void UpdateEnabledState()
		{
			if (this._actionTarget.VirtualMachine != null)
			{
				VMComputerSystemState state = this._actionTarget.VirtualMachine.State;
				bool flag = state == VMComputerSystemState.PowerOff || state == VMComputerSystemState.Saved;
				bool flag2 = state == VMComputerSystemState.Paused;
				this.btnTurnOn.Visibility = (flag ? Visibility.Visible : Visibility.Hidden);
				this.btnResume.Visibility = (flag2 ? Visibility.Visible : Visibility.Hidden);
				return;
			}
			this.btnTurnOn.Visibility = Visibility.Hidden;
			this.btnResume.Visibility = Visibility.Hidden;
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000ED6E File Offset: 0x0000CF6E
		private void btnTurnOn_Click(object sender, RoutedEventArgs e)
		{
			this._performer.OnStateChangeTaskTrigged(VMControlAction.StartTurnOff);
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000ED7C File Offset: 0x0000CF7C
		private void btnResume_Click(object sender, RoutedEventArgs e)
		{
			this._performer.OnStateChangeTaskTrigged(VMControlAction.PauseResume);
		}

		// Token: 0x0400010D RID: 269
		private IMenuActionTarget _actionTarget;

		// Token: 0x0400010E RID: 270
		private VMConnectUserActionPerformer _performer;

		// Token: 0x0400010F RID: 271
		private DpiScale _oldDpi;

		// Token: 0x04000110 RID: 272
		private IntPtr _windowHandle;

		// Token: 0x04000111 RID: 273
		private double _logicalFontSize;

		// Token: 0x04000112 RID: 274
		private double _logicalTxtOneFontSize;
	}
}
