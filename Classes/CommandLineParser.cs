using System;
using System.Globalization;
using System.Security.Authentication;
using System.Windows.Forms;
using Microsoft.Virtualization.Client.Common;
using Microsoft.Virtualization.Client.Management;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000002 RID: 2
	internal static class CommandLineParser
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		public static bool TryParse(string[] args, out RdpConnectionInfo connectionInfo)
		{
			bool flag = true;
			RdpConnectionInfo rdpConnectionInfo = new RdpConnectionInfo();
			int num = 0;
			int num2 = 0;
			string text = null;
			bool flag2 = false;
			bool showOptions = false;
			WindowsCredential windowsCredential = null;
			string text2 = null;
			string text3 = null;
			Guid empty = Guid.Empty;
			string text4 = null;
			string text5 = null;
			int launchIndex = 0;
			for (int i = 0; i < args.Length; i++)
			{
				string text6 = args[i];
				if (text6.Equals("/edit", StringComparison.OrdinalIgnoreCase) || text6.Equals("-edit", StringComparison.OrdinalIgnoreCase))
				{
					showOptions = true;
					num2++;
				}
				else if (text6.Equals("/credential", StringComparison.OrdinalIgnoreCase) || text6.Equals("-credential", StringComparison.OrdinalIgnoreCase))
				{
					if (++i >= args.Length)
					{
						CommandLineParser.DisplayMissingOptionArgument(text6);
						flag = false;
						break;
					}
					if (text2 != null)
					{
						CommandLineParser.DisplayMutuallyExclusiveOptions("credential", "user");
						flag = false;
						break;
					}
					if (text3 != null)
					{
						CommandLineParser.DisplayMutuallyExclusiveOptions("credential", "password");
						flag = false;
						break;
					}
					string text7 = args[i];
					if (!WindowsCredential.Load(text7, out windowsCredential))
					{
						CommandLineParser.DisplayMissingCredential(text7);
						flag = false;
						break;
					}
					num2++;
				}
				else if (text6.Equals("/user", StringComparison.OrdinalIgnoreCase) || text6.Equals("-user", StringComparison.OrdinalIgnoreCase))
				{
					if (windowsCredential != null)
					{
						CommandLineParser.DisplayMutuallyExclusiveOptions("credential", "user");
						flag = false;
						break;
					}
					if (++i >= args.Length)
					{
						CommandLineParser.DisplayMissingOptionArgument(text6);
						flag = false;
						break;
					}
					text2 = args[i];
					num2++;
				}
				else if (text6.Equals("/password", StringComparison.OrdinalIgnoreCase) || text6.Equals("-password", StringComparison.OrdinalIgnoreCase))
				{
					if (++i >= args.Length)
					{
						CommandLineParser.DisplayMissingOptionArgument(text6);
						flag = false;
						break;
					}
					if (windowsCredential != null)
					{
						CommandLineParser.DisplayMutuallyExclusiveOptions("credential", "password");
						flag = false;
						break;
					}
					text3 = args[i];
					num2++;
				}
				else if (text6.Equals("/g", StringComparison.OrdinalIgnoreCase) || text6.Equals("-g", StringComparison.OrdinalIgnoreCase))
				{
					if (++i >= args.Length)
					{
						CommandLineParser.DisplayMissingOptionArgument(text6);
						flag = false;
						break;
					}
					text5 = args[i];
					if (!Guid.TryParse(text5, out empty))
					{
						CommandLineParser.DisplayInvalidGuid(text5);
						flag = false;
						break;
					}
					flag2 = true;
					num2++;
				}
				else if (text6.Equals("/c", StringComparison.OrdinalIgnoreCase) || text6.Equals("-c", StringComparison.OrdinalIgnoreCase))
				{
					if (++i >= args.Length)
					{
						CommandLineParser.DisplayMissingOptionArgument(text6);
						flag = false;
						break;
					}
					string text8 = args[i];
					if (!int.TryParse(text8, out launchIndex))
					{
						CommandLineParser.DisplayInvalidCount(text8);
						flag = false;
						break;
					}
					flag2 = true;
					num2++;
				}
				else
				{
					if (text6.Equals("/?", StringComparison.OrdinalIgnoreCase) || text6.Equals("-?", StringComparison.OrdinalIgnoreCase))
					{
						CommandLineParser.DisplayUsage();
						flag = false;
						break;
					}
					num++;
					if (num == 1)
					{
						text = text6;
					}
					else
					{
						if (num != 2)
						{
							if (text6.StartsWith("/", StringComparison.OrdinalIgnoreCase) || text6.StartsWith("-", StringComparison.OrdinalIgnoreCase))
							{
								CommandLineParser.DisplayUnknownOption(text6);
							}
							else
							{
								CommandLineParser.DisplayUsage();
							}
							flag = false;
							break;
						}
						text4 = text6;
					}
				}
			}
			if (flag)
			{
				flag = ((num == 0 && num2 == 0) || (num == 1 && flag2) || num == 2);
				if (!flag)
				{
					CommandLineParser.DisplayUsage();
				}
			}
			if (flag)
			{
				if (text2 != null && text3 != null)
				{
					try
					{
						windowsCredential = WindowsCredential.CreateFromLogonNameAndPassword(text2, text3);
					}
					catch (InvalidCredentialException)
					{
						CommandLineParser.DisplayInvalidUserName(text2);
						flag = false;
					}
				}
				if (flag && text != null)
				{
					rdpConnectionInfo.ServerConnectionName = text;
					bool flag3 = false;
					IVMComputerSystem virtualMachine = null;
					Exception ex = null;
					if (flag2)
					{
						flag3 = ConnectionHelper.TryGetVirtualMachine(text, windowsCredential, empty, (text4 != null) ? text4 : text5, out virtualMachine, out ex);
						if (!flag3)
						{
							VMTrace.TraceError("Failed to find virtual machine by GUID.", ex);
						}
					}
					else if (text4 != null)
					{
						flag3 = ConnectionHelper.TryGetVirtualMachine(text, windowsCredential, text4, out virtualMachine, out ex);
						if (!flag3)
						{
							VMTrace.TraceError("Failed to find virtual machine by name.", ex);
						}
					}
					if (flag3)
					{
						int rdpPort = 0;
						if (CommandLineParser.TryGetRdpPort(virtualMachine, out rdpPort) && CommandLineParser.TrySetupImeEvents(virtualMachine))
						{
							rdpConnectionInfo.VirtualMachine = virtualMachine;
							rdpConnectionInfo.RdpPort = rdpPort;
							rdpConnectionInfo.LaunchIndex = launchIndex;
							rdpConnectionInfo.ShowOptions = showOptions;
							rdpConnectionInfo.Credential = windowsCredential;
							connectionInfo = rdpConnectionInfo;
						}
					}
					else if (ex != null)
					{
						string errorMsg;
						if (ex is ObjectNotFoundException)
						{
							errorMsg = ex.Message;
							ex = null;
						}
						else
						{
							errorMsg = string.Format(CultureInfo.CurrentCulture, CommonResources.ErrorFindVm, (text4 != null) ? text4 : text5, text);
						}
						CommandLineParser.DisplayMessageBoxError(errorMsg, ex);
						flag = false;
					}
				}
			}
			connectionInfo = rdpConnectionInfo;
			return flag;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000024C0 File Offset: 0x000006C0
		private static bool TrySetupImeEvents(IVMComputerSystem virtualMachine)
		{
			bool result = false;
			try
			{
				if (virtualMachine != null)
				{
					virtualMachine.RegisterForInstanceModificationEvents(InstanceModificationEventStrategy.InstanceModificationEvent);
					virtualMachine.UpdatePropertyCache();
				}
				result = true;
			}
			catch (VirtualizationManagementException ex)
			{
				VMTrace.TraceError("Failed to register for IME events.", ex);
				CommandLineParser.DisplayMessageBoxError(string.Format(CultureInfo.CurrentCulture, VMISResources.Error_VmImeEventRegistrationFailed, virtualMachine.Name), ex);
			}
			return result;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002520 File Offset: 0x00000720
		private static bool TryGetRdpPort(IVMComputerSystem virtualMachine, out int rdpPort)
		{
			rdpPort = 0;
			bool result = false;
			try
			{
				ITerminalServiceSetting terminalServiceSetting = ObjectLocator.GetTerminalServiceSetting(virtualMachine.Server);
				rdpPort = terminalServiceSetting.ListenerPort;
				result = true;
			}
			catch (VirtualizationManagementException ex)
			{
				VMTrace.TraceError("Could not determine RDP port.", ex);
				CommandLineParser.DisplayMessageBoxError(VMISResources.Error_CouldNotDetermineRdpPort, ex);
			}
			return result;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002574 File Offset: 0x00000774
		private static void DisplayUsage()
		{
			string commandLine_Usage = VMISResources.CommandLine_Usage;
			string commandLine_UsageTitle = VMISResources.CommandLine_UsageTitle;
			MessageBoxOptions defaultMessageBoxOptions = CommonUtilities.GetDefaultMessageBoxOptions();
			MessageBox.Show(commandLine_Usage, commandLine_UsageTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, defaultMessageBoxOptions);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000259E File Offset: 0x0000079E
		private static void DisplayMissingOptionArgument(string option)
		{
			if (option == null)
			{
				throw new ArgumentNullException("option");
			}
			CommandLineParser.DisplayMessageBoxError(string.Format(CultureInfo.CurrentCulture, VMISResources.CommandLine_MissingOptionArgument, option), null);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000025C4 File Offset: 0x000007C4
		private static void DisplayMutuallyExclusiveOptions(string option1, string option2)
		{
			if (option1 == null)
			{
				throw new ArgumentNullException("option1");
			}
			if (option2 == null)
			{
				throw new ArgumentNullException("option2");
			}
			CommandLineParser.DisplayMessageBoxError(string.Format(CultureInfo.CurrentCulture, VMISResources.CommandLine_MutuallyExclusiveOptions, option1, option2), null);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000025F9 File Offset: 0x000007F9
		private static void DisplayUnknownOption(string option)
		{
			if (option == null)
			{
				throw new ArgumentNullException("option");
			}
			CommandLineParser.DisplayMessageBoxError(string.Format(CultureInfo.CurrentCulture, VMISResources.CommandLine_UnknownOption, option), null);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x0000261F File Offset: 0x0000081F
		private static void DisplayMissingCredential(string credentialName)
		{
			if (credentialName == null)
			{
				throw new ArgumentNullException("credentialName");
			}
			CommandLineParser.DisplayMessageBoxError(string.Format(CultureInfo.CurrentCulture, VMISResources.CommandLine_MissingCredential, credentialName), null);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002645 File Offset: 0x00000845
		private static void DisplayInvalidUserName(string user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			CommandLineParser.DisplayMessageBoxError(string.Format(CultureInfo.CurrentCulture, VMISResources.CommandLine_InvalidUserName, user), null);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000266B File Offset: 0x0000086B
		private static void DisplayInvalidGuid(string invalidGuid)
		{
			if (invalidGuid == null)
			{
				throw new ArgumentNullException("invalidGuid");
			}
			CommandLineParser.DisplayMessageBoxError(string.Format(CultureInfo.CurrentCulture, VMISResources.CommandLine_InvalidGuid, invalidGuid), null);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002691 File Offset: 0x00000891
		private static void DisplayInvalidCount(string invalidCount)
		{
			if (invalidCount == null)
			{
				throw new ArgumentNullException("invalidCount");
			}
			CommandLineParser.DisplayMessageBoxError(string.Format(CultureInfo.CurrentCulture, VMISResources.CommandLine_InvalidCount, invalidCount), null);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000026B8 File Offset: 0x000008B8
		private static void DisplayMessageBoxError(string errorMsg, Exception ex = null)
		{
			MessageBoxOptions defaultMessageBoxOptions = CommonUtilities.GetDefaultMessageBoxOptions();
			string text2;
			if (ex != null)
			{
				string arg;
				string text;
				InformationDisplayer.GetErrorInformationFromException(ex, out arg, out text, null);
				text2 = string.Format(CultureInfo.InvariantCulture, "{0}\n\n{1}", errorMsg, arg);
			}
			else
			{
				text2 = errorMsg;
			}
			MessageBox.Show(text2, Program.Displayer.DefaultTitle, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, defaultMessageBoxOptions);
		}
	}
}
