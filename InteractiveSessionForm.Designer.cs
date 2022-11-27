namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x0200000B RID: 11
	internal sealed partial class InteractiveSessionForm : global::System.Windows.Forms.Form, global::Microsoft.Virtualization.Client.InteractiveSession.IMenuActionTarget, global::Microsoft.Virtualization.Client.InteractiveSession.IVMConnectCommunicatorHelper, global::System.Windows.Forms.IDpiForm
	{
		// Token: 0x0600007F RID: 127 RVA: 0x0000749C File Offset: 0x0000569C
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.m_ClearKeysNotCapturedTimer.Stop();
				this.m_ClearKeysNotCapturedTimer.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x0400008A RID: 138
		private global::System.Windows.Forms.Timer m_ClearKeysNotCapturedTimer;
	}
}
