using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using Microsoft.Virtualization.Client.Common;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x0200001E RID: 30
	internal class VMConnectCommunicator : MarshalByRefObject, IVMConnectCommunicator
	{
		// Token: 0x060001B8 RID: 440 RVA: 0x0000EE4C File Offset: 0x0000D04C
		public bool ActivateIfConnectedToVM(string serverName, string vmInstanceId)
		{
			bool result = false;
			if (!string.IsNullOrEmpty(serverName) && !string.IsNullOrEmpty(vmInstanceId) && string.Equals(VMConnectCommunicator.m_VMConnectHelper.CurrentVMServerName, serverName, StringComparison.OrdinalIgnoreCase) && string.Equals(VMConnectCommunicator.m_VMConnectHelper.CurrentVMInstanceId, vmInstanceId, StringComparison.OrdinalIgnoreCase))
			{
				result = true;
				VMConnectCommunicator.m_VMConnectHelper.ActivateSelf();
			}
			return result;
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000EEA0 File Offset: 0x0000D0A0
		[SuppressMessage("Microsoft.Design", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Called is not granted access to operations or resources that can be used in a destructive manner.")]
		public static void Register(IVMConnectCommunicatorHelper helper)
		{
			if (helper == null)
			{
				throw new ArgumentNullException("helper");
			}
			if (VMConnectCommunicator.m_VMConnectHelper != null)
			{
				throw new InvalidOperationException();
			}
			VMConnectCommunicator.m_VMConnectHelper = helper;
			VMConnectCommunicator.m_Channel = new IpcChannel("localhost:" + Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture));
			ChannelServices.RegisterChannel(VMConnectCommunicator.m_Channel, false);
			RemotingConfiguration.RegisterWellKnownServiceType(typeof(VMConnectCommunicator), "VMConnectCommunicator.rem", WellKnownObjectMode.Singleton);
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000EF1C File Offset: 0x0000D11C
		public static void Unregister()
		{
			try
			{
				ChannelServices.UnregisterChannel(VMConnectCommunicator.m_Channel);
			}
			catch (RemotingException ex)
			{
				VMTrace.TraceError("Failed to unregister VMConnect remoting channel", ex);
			}
		}

		// Token: 0x04000118 RID: 280
		private static IVMConnectCommunicatorHelper m_VMConnectHelper;

		// Token: 0x04000119 RID: 281
		private static IpcChannel m_Channel;
	}
}
