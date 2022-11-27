using System;
using System.Threading;
using Microsoft.Virtualization.Client.Management;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000019 RID: 25
	internal class MigrationTracker
	{
		// Token: 0x06000186 RID: 390 RVA: 0x0000E528 File Offset: 0x0000C728
		public void Setup(IVMComputerSystem virtualMachine)
		{
			this.m_SourceServer = virtualMachine.Server;
			this.m_VirtualMachine = virtualMachine;
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.RegisterForMigrationTaskCreation));
		}

		// Token: 0x06000187 RID: 391 RVA: 0x0000E550 File Offset: 0x0000C750
		private void RegisterForMigrationTaskCreation(object state)
		{
			Server sourceServer = this.m_SourceServer;
			try
			{
				this.m_SourceHostComputer = ObjectLocator.GetHostComputerSystem(sourceServer);
				this.m_SourceHostComputer.VMVirtualizationTaskCreated += this.OnTaskCreated;
			}
			catch (Exception ex)
			{
				VMTrace.TraceWarning("Unable to register for listening for migration tasks!", ex);
				return;
			}
			try
			{
				foreach (IVMMigrationTask ivmmigrationTask in this.m_VirtualMachine.GetMigrationTasks())
				{
					if (ivmmigrationTask.Status == VMTaskStatus.Running)
					{
						this.OnTaskCreated(ivmmigrationTask);
						break;
					}
				}
			}
			catch (Exception ex2)
			{
				VMTrace.TraceWarning("Unexpected failure while searching for existing migration tasks.!", ex2);
			}
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000E614 File Offset: 0x0000C814
		public void Cleanup()
		{
			if (this.m_SourceHostComputer != null)
			{
				Program.TasksToCompleteBeforeExitingTracker.RegisterBackgroundTask(new MigrationTracker.CleanupMethod(this.CleanupInternal), new object[]
				{
					this.m_SourceHostComputer
				});
			}
			this.m_SourceServer = null;
			this.m_VirtualMachine = null;
			this.m_SourceHostComputer = null;
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000E664 File Offset: 0x0000C864
		private void CleanupInternal(IHostComputerSystem sourceHostComputer)
		{
			try
			{
				if (sourceHostComputer != null)
				{
					sourceHostComputer.VMVirtualizationTaskCreated -= this.OnTaskCreated;
				}
			}
			catch (Exception ex)
			{
				VMTrace.TraceWarning("Unable to unregister from the event handler used for tracking the migration tasks.", ex);
			}
		}

		// Token: 0x0600018A RID: 394 RVA: 0x0000E6A8 File Offset: 0x0000C8A8
		private void OnTaskCreated(IVMTask newTask)
		{
			bool flag = false;
			if (newTask is IVMMigrationTask && this.m_SourceServer != null && newTask.Server == this.m_SourceServer)
			{
				IVMMigrationTask ivmmigrationTask = (IVMMigrationTask)newTask;
				if (flag = this.IsThisVmMigrating(ivmmigrationTask))
				{
					ivmmigrationTask.Completed += this.OnMigrationTaskCompleted;
					this.RaiseMigrationStarted();
				}
			}
			if (!flag)
			{
				newTask.Dispose();
			}
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0000E714 File Offset: 0x0000C914
		private bool IsThisVmMigrating(IVMMigrationTask migrationTask)
		{
			bool flag = migrationTask.JobType == 305 || migrationTask.JobType == 306;
			bool flag2 = string.Equals(migrationTask.VmComputerSystemInstanceId, this.m_VirtualMachine.InstanceId, StringComparison.OrdinalIgnoreCase);
			return flag && flag2;
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000E758 File Offset: 0x0000C958
		private void OnMigrationTaskCompleted(object sender, EventArgs ea)
		{
			IVMMigrationTask ivmmigrationTask = (IVMMigrationTask)sender;
			VMTaskStatus status = ivmmigrationTask.Status;
			string destinationHost = ivmmigrationTask.DestinationHost;
			ivmmigrationTask.Completed -= this.OnMigrationTaskCompleted;
			ivmmigrationTask.Dispose();
			this.RaiseMigrationCompleted(status, destinationHost);
		}

		// Token: 0x14000013 RID: 19
		// (add) Token: 0x0600018D RID: 397 RVA: 0x0000E798 File Offset: 0x0000C998
		// (remove) Token: 0x0600018E RID: 398 RVA: 0x0000E7D0 File Offset: 0x0000C9D0
		public event EventHandler MigrationStarted;

		// Token: 0x14000014 RID: 20
		// (add) Token: 0x0600018F RID: 399 RVA: 0x0000E808 File Offset: 0x0000CA08
		// (remove) Token: 0x06000190 RID: 400 RVA: 0x0000E840 File Offset: 0x0000CA40
		public event EventHandler<MigrationTracker.MigrationCompletedStatusEventArgs> MigrationCompleted;

		// Token: 0x06000191 RID: 401 RVA: 0x0000E875 File Offset: 0x0000CA75
		private void RaiseMigrationStarted()
		{
			if (this.MigrationStarted != null)
			{
				this.MigrationStarted(this, EventArgs.Empty);
			}
		}

		// Token: 0x06000192 RID: 402 RVA: 0x0000E890 File Offset: 0x0000CA90
		private void RaiseMigrationCompleted(VMTaskStatus completedStatus, string destinationHost)
		{
			if (completedStatus == VMTaskStatus.Running)
			{
				VMTrace.TraceWarning("Unexpected task completion status of running.", Array.Empty<string>());
				return;
			}
			if (this.MigrationCompleted != null)
			{
				MigrationTracker.MigrationCompletedStatusEventArgs e = new MigrationTracker.MigrationCompletedStatusEventArgs(completedStatus, destinationHost);
				this.MigrationCompleted(this, e);
			}
		}

		// Token: 0x040000FE RID: 254
		private Server m_SourceServer;

		// Token: 0x040000FF RID: 255
		private IVMComputerSystem m_VirtualMachine;

		// Token: 0x04000100 RID: 256
		private IHostComputerSystem m_SourceHostComputer;

		// Token: 0x02000048 RID: 72
		// (Invoke) Token: 0x060003E6 RID: 998
		private delegate void CleanupMethod(IHostComputerSystem sourceHostComputer);

		// Token: 0x02000049 RID: 73
		public class MigrationCompletedStatusEventArgs : EventArgs
		{
			// Token: 0x17000184 RID: 388
			// (get) Token: 0x060003E9 RID: 1001 RVA: 0x00015B62 File Offset: 0x00013D62
			// (set) Token: 0x060003EA RID: 1002 RVA: 0x00015B6A File Offset: 0x00013D6A
			public VMTaskStatus CompletionStatus { get; private set; }

			// Token: 0x17000185 RID: 389
			// (get) Token: 0x060003EB RID: 1003 RVA: 0x00015B73 File Offset: 0x00013D73
			// (set) Token: 0x060003EC RID: 1004 RVA: 0x00015B7B File Offset: 0x00013D7B
			public string DestinationHost { get; private set; }

			// Token: 0x060003ED RID: 1005 RVA: 0x00015B84 File Offset: 0x00013D84
			public MigrationCompletedStatusEventArgs(VMTaskStatus completitionStatus, string destinationHost)
			{
				this.CompletionStatus = completitionStatus;
				this.DestinationHost = destinationHost;
			}
		}
	}
}
