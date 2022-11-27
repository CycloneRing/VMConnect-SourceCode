using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000010 RID: 16
	internal static class Program
	{
		// Token: 0x060000DF RID: 223 RVA: 0x00009EB8 File Offset: 0x000080B8
		static Program()
		{
			Program.gm_UnhandledExceptionHandler.CloseApp = false;
			Program.gm_UnhandledExceptionHandler.DisplayMessage = false;
			Program.gm_UnhandledExceptionHandler.LogExceptionInfo = false;
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x00009F0E File Offset: 0x0000810E
		public static InformationDisplayer Displayer
		{
			get
			{
				return Program.gm_ErrorDisplayer;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000E1 RID: 225 RVA: 0x00009F15 File Offset: 0x00008115
		public static UnhandledExceptionHandler UnhandledExceptionHandler
		{
			get
			{
				return Program.gm_UnhandledExceptionHandler;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x00009F1C File Offset: 0x0000811C
		public static BackgroundTaskTracker TasksToCompleteBeforeExitingTracker
		{
			get
			{
				return Program.gm_BackgroundTaskTracker;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000E3 RID: 227 RVA: 0x00009F23 File Offset: 0x00008123
		public static string TestCmdLineValue
		{
			get
			{
				return Program.gm_TestCmdLineValue;
			}
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00009F2C File Offset: 0x0000812C
		[STAThread]
		private static void Main(string[] args)
		{
			try
			{
				TraceConfigurationOptions.Instance.Reload();
				VMTrace.Initialize(TraceConfigurationOptions.Instance.Level, (VMTraceTagFormatLevels)TraceConfigurationOptions.Instance.TraceTagFormat);
				args = Program.ExtractTestCookie(args);
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				VmisApplicationContext vmisApplicationContext = new VmisApplicationContext();
				if (vmisApplicationContext.TryParseCommandLine(args))
				{
					Program.gm_UnhandledExceptionHandler.RegisterApplicationThreadExceptionHandler();
					Application.Run(vmisApplicationContext);
				}
			}
			catch (COMException ex)
			{
				if (ex.ErrorCode != -2147221164 && ex.ErrorCode != -2147221231)
				{
					throw;
				}
				Program.Displayer.DisplayError(VMISResources.Error_AxControlNotRegistered, string.Empty);
			}
			finally
			{
				VMTrace.CloseLogFile();
			}
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00009FE8 File Offset: 0x000081E8
		private static string[] ExtractTestCookie(string[] args)
		{
			string[] array = args;
			if (args != null && args.Length != 0)
			{
				int num = -1;
				for (int i = 0; i < args.Length; i++)
				{
					if (args[i].StartsWith("-clxcmdline=", StringComparison.OrdinalIgnoreCase))
					{
						num = i;
						break;
					}
				}
				if (num != -1)
				{
					Program.gm_TestCmdLineValue = args[num].Substring("-clxcmdline=".Length);
					array = new string[args.Length - 1];
					int num2 = 0;
					for (int j = 0; j < args.Length; j++)
					{
						if (j != num)
						{
							array[num2++] = args[j];
						}
					}
				}
			}
			return array;
		}

		// Token: 0x040000A6 RID: 166
		private const string gm_TestCmdLine = "-clxcmdline=";

		// Token: 0x040000A7 RID: 167
		private static string gm_TestCmdLineValue;

		// Token: 0x040000A8 RID: 168
		private static InformationDisplayer gm_ErrorDisplayer = new InformationDisplayer(IntPtr.Zero, VMISResources.ErrorDialogTitle);

		// Token: 0x040000A9 RID: 169
		private static UnhandledExceptionHandler gm_UnhandledExceptionHandler = new UnhandledExceptionHandler();

		// Token: 0x040000AA RID: 170
		private static BackgroundTaskTracker gm_BackgroundTaskTracker = new BackgroundTaskTracker();
	}
}
