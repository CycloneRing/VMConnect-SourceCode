using System;
using System.Globalization;
using System.Threading;
using Microsoft.Virtualization.Client.Management;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x0200001A RID: 26
	internal class MigrationData
	{
		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000194 RID: 404 RVA: 0x0000E8D5 File Offset: 0x0000CAD5
		// (set) Token: 0x06000195 RID: 405 RVA: 0x0000E8DD File Offset: 0x0000CADD
		public bool IsMigrating { get; set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000196 RID: 406 RVA: 0x0000E8E6 File Offset: 0x0000CAE6
		// (set) Token: 0x06000197 RID: 407 RVA: 0x0000E8EE File Offset: 0x0000CAEE
		public bool IsMigrated { get; set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000198 RID: 408 RVA: 0x0000E8F7 File Offset: 0x0000CAF7
		// (set) Token: 0x06000199 RID: 409 RVA: 0x0000E8FF File Offset: 0x0000CAFF
		public string DestinationHostName { get; set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x0600019A RID: 410 RVA: 0x0000E908 File Offset: 0x0000CB08
		// (set) Token: 0x0600019B RID: 411 RVA: 0x0000E910 File Offset: 0x0000CB10
		public Server DestinationHost { get; private set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600019C RID: 412 RVA: 0x0000E919 File Offset: 0x0000CB19
		// (set) Token: 0x0600019D RID: 413 RVA: 0x0000E921 File Offset: 0x0000CB21
		public IVMComputerSystem MigratedVM { get; private set; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600019E RID: 414 RVA: 0x0000E92A File Offset: 0x0000CB2A
		// (set) Token: 0x0600019F RID: 415 RVA: 0x0000E932 File Offset: 0x0000CB32
		public int RdpPortOnDestination { get; private set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060001A0 RID: 416 RVA: 0x0000E93B File Offset: 0x0000CB3B
		// (set) Token: 0x060001A1 RID: 417 RVA: 0x0000E943 File Offset: 0x0000CB43
		public Exception FetchDestinationDataError { get; private set; }

		// Token: 0x14000015 RID: 21
		// (add) Token: 0x060001A2 RID: 418 RVA: 0x0000E94C File Offset: 0x0000CB4C
		// (remove) Token: 0x060001A3 RID: 419 RVA: 0x0000E984 File Offset: 0x0000CB84
		public event EventHandler MigrationDataFetched;

		// Token: 0x060001A4 RID: 420 RVA: 0x0000E9B9 File Offset: 0x0000CBB9
		public void Setup(IVMComputerSystem virtualMachine)
		{
			this.m_SourceServer = virtualMachine.Server;
			this.m_VMInstanceId = virtualMachine.InstanceId;
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000E9D4 File Offset: 0x0000CBD4
		public void Clear()
		{
			VMTrace.TraceInformation("Clearing migration data.", Array.Empty<string>());
			this.IsMigrating = false;
			this.IsMigrated = false;
			this.DestinationHostName = null;
			this.DestinationHost = null;
			this.MigratedVM = null;
			this.RdpPortOnDestination = 0;
			this.FetchDestinationDataError = null;
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000EA24 File Offset: 0x0000CC24
		public void FetchDestinationData(object state)
		{
			if (string.IsNullOrEmpty(this.m_VMInstanceId))
			{
				throw new InvalidOperationException();
			}
			if (string.IsNullOrEmpty(this.DestinationHostName))
			{
				throw new InvalidOperationException();
			}
			try
			{
				int num = 0;
				this.DestinationHost = Server.GetServer(this.DestinationHostName, this.m_SourceServer.Credential);
				ITerminalServiceSetting terminalServiceSetting = ObjectLocator.GetTerminalServiceSetting(this.DestinationHost);
				terminalServiceSetting.UpdatePropertyCache(TimeSpan.FromSeconds(2.0));
				this.RdpPortOnDestination = terminalServiceSetting.ListenerPort;
				Exception ex = null;
				do
				{
					try
					{
						this.MigratedVM = ObjectLocator.GetVMComputerSystem(this.DestinationHost, this.m_VMInstanceId);
						this.MigratedVM.RegisterForInstanceModificationEvents(InstanceModificationEventStrategy.InstanceModificationEvent);
						this.MigratedVM.UpdatePropertyCache();
						ex = null;
					}
					catch (ObjectNotFoundException ex2)
					{
						VMTrace.TraceError("Could not find the virtual machine on the destination! Retry count: " + num.ToString(CultureInfo.InvariantCulture), ex2);
						ex = ex2;
						Thread.Sleep(TimeSpan.FromSeconds(1.0));
					}
				}
				while (ex != null && ++num < 5);
				this.FetchDestinationDataError = ex;
			}
			catch (Exception ex3)
			{
				VMTrace.TraceError("Unable to retrieve information for the migration destination machine!", ex3);
				this.FetchDestinationDataError = ex3;
			}
			if (this.MigrationDataFetched != null)
			{
				this.MigrationDataFetched(this, EventArgs.Empty);
			}
		}

		// Token: 0x04000103 RID: 259
		private Server m_SourceServer;

		// Token: 0x04000104 RID: 260
		private string m_VMInstanceId;
	}
}
