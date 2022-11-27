using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000021 RID: 33
	internal class VmisApplicationContext : ApplicationContext
	{
		// Token: 0x060001CD RID: 461 RVA: 0x0000F72C File Offset: 0x0000D92C
		private void HandleConnectionDialogClosing(object sender, FormClosingEventArgs ea)
		{
			ConnectionDialog connectionDialog = (ConnectionDialog)sender;
			if (connectionDialog.DialogResult == DialogResult.OK)
			{
				InteractiveSessionConfigurationOptions.Instance.StartingPosition = new Point?(connectionDialog.Location);
				this.CreateInteractiveSessionForm(connectionDialog.RdpConnectionInfo).Show();
			}
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000F76F File Offset: 0x0000D96F
		protected override void OnMainFormClosed(object sender, EventArgs e)
		{
			InteractiveSessionConfigurationOptions.Instance.Save();
			CommonConfiguration.Instance.Save();
			Program.TasksToCompleteBeforeExitingTracker.WaitForBackgroundTasksToComplete();
			Program.TasksToCompleteBeforeExitingTracker.Dispose();
			base.OnMainFormClosed(sender, e);
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000F7A4 File Offset: 0x0000D9A4
		public bool TryParseCommandLine(string[] args)
		{
			if (args == null || args.Length == 0)
			{
				ConnectionDialog connectionDialog = new ConnectionDialog();
				connectionDialog.FormClosing += this.HandleConnectionDialogClosing;
				Point? startingPosition = InteractiveSessionConfigurationOptions.Instance.StartingPosition;
				if (startingPosition != null)
				{
					connectionDialog.StartPosition = FormStartPosition.Manual;
					connectionDialog.Location = startingPosition.Value;
				}
				connectionDialog.ShowInTaskbar = true;
				base.MainForm = connectionDialog;
				return true;
			}
			RdpConnectionInfo connectionInfo;
			if (CommandLineParser.TryParse(args, out connectionInfo))
			{
				this.CreateInteractiveSessionForm(connectionInfo);
				return true;
			}
			return false;
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000F820 File Offset: 0x0000DA20
		private InteractiveSessionForm CreateInteractiveSessionForm(RdpConnectionInfo connectionInfo)
		{
			InteractiveSessionForm interactiveSessionForm = new InteractiveSessionForm(connectionInfo);
			base.MainForm = interactiveSessionForm;
			return interactiveSessionForm;
		}
	}
}
