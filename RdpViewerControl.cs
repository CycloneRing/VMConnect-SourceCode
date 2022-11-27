using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using AxMicrosoft.Virtualization.Client.Interop;
using Microsoft.Virtualization.Client.Interop;
using Microsoft.Virtualization.Client.Management;
using Microsoft.Win32;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000017 RID: 23
	internal class RdpViewerControl : Control
	{
		// Token: 0x06000101 RID: 257 RVA: 0x0000A2EC File Offset: 0x000084EC
		[SuppressMessage("Microsoft.Design", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Called is not granted access to operations or resources that can be used in a destructive manner.")]
		public RdpViewerControl(Form form, RdpConnectionInfo rdpConnectionInfo)
		{
			this._form = form;
			this.m_BlockConnectionRequests = 0;
			this.m_RdpConnectionInfo = rdpConnectionInfo;
			this.ConnectionState = ((rdpConnectionInfo.VirtualMachine != null) ? RdpViewerConnectionState.NotConnected : RdpViewerConnectionState.NoVirtualMachine);
			this.m_VMConnectUseEnhancedModeRuntimePreference = ClientVirtualizationSettings.Instance.VMConnectUseEnhancedMode;
			base.SuspendLayout();
			this.BackColor = Color.Black;
			new ComponentResourceManager(typeof(RdpViewerControl)).ApplyResources(this, "$this");
			base.TabStop = true;
			base.SetStyle(ControlStyles.ContainerControl, true);
			this.SetDisplayStrings();
			this.CreateRdpClient(true);
			base.ResumeLayout();
			this.m_SnapshotTracker.HasSnapshotsChanged += this.HandleHasSnapshotsChanged;
			this.m_MigrationTracker.MigrationStarted += this.HandleMigrationStarted;
			this.m_MigrationTracker.MigrationCompleted += this.HandleMigrationCompleted;
			this.m_MouseActivateTimer = new System.Windows.Forms.Timer();
			this.m_MouseActivateTimer.Interval = 300;
			this.m_MouseActivateTimer.Tick += this.HandleMouseActivateTimer;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x0000A444 File Offset: 0x00008644
		private void CreateRdpClient(bool onInit)
		{
			VMTrace.TraceInformation("Recreating the RDP client ActiveX control.", Array.Empty<string>());
			RdpClient rdpClient = this.m_RdpClient;
			this.m_RdpClient = new RdpClient();
			this.m_RdpClient.BeginInit();
			this.m_RdpClient.Name = "RdpClient";
			if (!onInit)
			{
				this.m_RdpClient.Visible = false;
			}
			base.Controls.Add(this.m_RdpClient);
			this.m_RdpClient.EndInit();
			if (rdpClient != null)
			{
				base.Controls.Remove(rdpClient);
				rdpClient.Dispose();
			}
			this.m_RdpClient.Location = new Point(0, 0);
			this.m_RdpClient.Dock = DockStyle.None;
			this.SetupRdpEventHandlers();
			this.m_RdpClient.AdvancedSettings3.EnableAutoReconnect = false;
			this.m_RdpClient.AdvancedSettings.ContainerHandledFullScreen = 1;
			this.m_RdpClient.AdvancedSettings7.RelativeMouseMode = true;
			this.m_RdpClient.AdvancedSettings7.AuthenticationServiceClass = "Microsoft Virtual Console Service";
			if (this.VirtualMachine.Setting.EnhancedSessionTransportType == EnhancedSessionTransportType.HvSocket)
			{
				this.m_RdpClient.AdvancedSettings8.NetworkConnectionType = 6U;
			}
			if (this.m_RdpConnectionInfo.Credential != null)
			{
				this.m_RdpClient.UserName = this.m_RdpConnectionInfo.Credential.LogonName;
				IntPtr intPtr = IntPtr.Zero;
				try
				{
					intPtr = Marshal.SecureStringToBSTR(this.m_RdpConnectionInfo.Credential.Password);
					this.m_RdpClient.AdvancedSettings7.ClearTextPassword = Marshal.PtrToStringBSTR(intPtr);
				}
				finally
				{
					if (intPtr != IntPtr.Zero)
					{
						Marshal.ZeroFreeBSTR(intPtr);
					}
				}
			}
			IMsRdpClientNonScriptable3 msRdpClientNonScriptable = this.m_RdpClient.GetOcx() as IMsRdpClientNonScriptable3;
			if (msRdpClientNonScriptable != null)
			{
				msRdpClientNonScriptable.ConnectionBarText = string.Format(CultureInfo.CurrentCulture, VMISResources.RdpViewer_FullScreenConnectionBarText, this.VirtualMachine.Name, this.VirtualMachine.Server);
			}
			else
			{
				VMTrace.TraceError("Query for interface ImsRdpClientNonScriptable failed!", Array.Empty<string>());
			}
			this.SetSecureModeOn();
			this.EnableFrameBufferRedirection();
			if (!string.IsNullOrEmpty(Program.TestCmdLineValue))
			{
				this.m_RdpClient.Debugger.CLXCmdLine = Program.TestCmdLineValue;
			}
			this.m_RdpClient.AdvancedSettings7.GrabFocusOnConnect = false;
			ClientVirtualizationSettings instance = ClientVirtualizationSettings.Instance;
			VMConnectKeyboardOption vmconnectKeyboardOption = instance.VMConnectKeyboardOption;
			try
			{
				this.KeyboardHookMode = vmconnectKeyboardOption;
			}
			catch (ArgumentException)
			{
				VMTrace.TraceWarning("LocalStorage setting for VMConnectKeyboardOption is set to an invalid value: " + vmconnectKeyboardOption.ToString(), Array.Empty<string>());
			}
			this.ReleaseKey = instance.VMConnectReleaseKey;
		}

		// Token: 0x06000103 RID: 259 RVA: 0x0000A6CC File Offset: 0x000088CC
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				this.m_MouseActivateTimer.Dispose();
			}
		}

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000104 RID: 260 RVA: 0x0000A6E4 File Offset: 0x000088E4
		// (remove) Token: 0x06000105 RID: 261 RVA: 0x0000A71C File Offset: 0x0000891C
		public event EventHandler VMNameChanged;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000106 RID: 262 RVA: 0x0000A754 File Offset: 0x00008954
		// (remove) Token: 0x06000107 RID: 263 RVA: 0x0000A78C File Offset: 0x0000898C
		public event EventHandler VMStateChanged;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000108 RID: 264 RVA: 0x0000A7C4 File Offset: 0x000089C4
		// (remove) Token: 0x06000109 RID: 265 RVA: 0x0000A7FC File Offset: 0x000089FC
		public event EventHandler VMConfigurationChanged;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x0600010A RID: 266 RVA: 0x0000A834 File Offset: 0x00008A34
		// (remove) Token: 0x0600010B RID: 267 RVA: 0x0000A86C File Offset: 0x00008A6C
		public event EventHandler ConnectionStateChanged;

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x0600010C RID: 268 RVA: 0x0000A8A4 File Offset: 0x00008AA4
		// (remove) Token: 0x0600010D RID: 269 RVA: 0x0000A8DC File Offset: 0x00008ADC
		public event EventHandler RequestEnterFullScreen;

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x0600010E RID: 270 RVA: 0x0000A914 File Offset: 0x00008B14
		// (remove) Token: 0x0600010F RID: 271 RVA: 0x0000A94C File Offset: 0x00008B4C
		public event EventHandler RequestExitFullScreen;

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06000110 RID: 272 RVA: 0x0000A984 File Offset: 0x00008B84
		// (remove) Token: 0x06000111 RID: 273 RVA: 0x0000A9BC File Offset: 0x00008BBC
		public event EventHandler RequestFullScreenClose;

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000112 RID: 274 RVA: 0x0000A9F4 File Offset: 0x00008BF4
		// (remove) Token: 0x06000113 RID: 275 RVA: 0x0000AA2C File Offset: 0x00008C2C
		public event EventHandler RequestFullScreenMinimize;

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x06000114 RID: 276 RVA: 0x0000AA64 File Offset: 0x00008C64
		// (remove) Token: 0x06000115 RID: 277 RVA: 0x0000AA9C File Offset: 0x00008C9C
		public event EventHandler MouseCursorCapturedChanged;

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x06000116 RID: 278 RVA: 0x0000AAD4 File Offset: 0x00008CD4
		// (remove) Token: 0x06000117 RID: 279 RVA: 0x0000AB0C File Offset: 0x00008D0C
		public event EventHandler KeyboardInputCapturedChanged;

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x06000118 RID: 280 RVA: 0x0000AB44 File Offset: 0x00008D44
		// (remove) Token: 0x06000119 RID: 281 RVA: 0x0000AB7C File Offset: 0x00008D7C
		public event EventHandler MouseModeRelativeChanged;

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x0600011A RID: 282 RVA: 0x0000ABB4 File Offset: 0x00008DB4
		// (remove) Token: 0x0600011B RID: 283 RVA: 0x0000ABEC File Offset: 0x00008DEC
		public event EventHandler HasSnapshotsChanged;

		// Token: 0x14000010 RID: 16
		// (add) Token: 0x0600011C RID: 284 RVA: 0x0000AC24 File Offset: 0x00008E24
		// (remove) Token: 0x0600011D RID: 285 RVA: 0x0000AC5C File Offset: 0x00008E5C
		public event EventHandler OfflineDisplayStringsUpdated;

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x0600011E RID: 286 RVA: 0x0000AC94 File Offset: 0x00008E94
		// (remove) Token: 0x0600011F RID: 287 RVA: 0x0000ACCC File Offset: 0x00008ECC
		public event EventHandler GuestResolutionChanged;

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000120 RID: 288 RVA: 0x0000AD01 File Offset: 0x00008F01
		public IVMComputerSystem VirtualMachine
		{
			get
			{
				return this.m_RdpConnectionInfo.VirtualMachine;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000121 RID: 289 RVA: 0x0000AD0E File Offset: 0x00008F0E
		public bool IsVMMigrating
		{
			get
			{
				return this.m_MigrationData.IsMigrating;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000122 RID: 290 RVA: 0x0000AD1B File Offset: 0x00008F1B
		public bool IsVMMigratingOrMigrated
		{
			get
			{
				return this.m_MigrationData.IsMigrating || this.m_MigrationData.IsMigrated;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000123 RID: 291 RVA: 0x0000AD37 File Offset: 0x00008F37
		public int LaunchIndex
		{
			get
			{
				return this.m_RdpConnectionInfo.LaunchIndex;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000124 RID: 292 RVA: 0x0000AD44 File Offset: 0x00008F44
		public bool ShowOptions
		{
			get
			{
				return this.m_RdpConnectionInfo.ShowOptions;
			}
		}

		// Token: 0x06000125 RID: 293 RVA: 0x0000AD54 File Offset: 0x00008F54
		public void EnterFullScreen()
		{
			if (!this.FullScreen)
			{
				this.FullScreen = true;
				if (this.IsConnected)
				{
					if (this.ConnectionState != RdpViewerConnectionState.ConnectedEnhancedVideoSyncedSession)
					{
						if (this.ConnectionState != RdpViewerConnectionState.ConnectedEnhancedVideo)
						{
							this.m_PreFullScreenZoomLevel = new ZoomLevel?(this.m_CurrentZoomLevel);
							this.SetZoomLevel(ZoomLevel.P100);
						}
						this.m_RdpClient.Location = new Point(Math.Max((base.Width - this.m_DesktopResolution.Width) / 2, 0), Math.Max((base.Height - this.m_DesktopResolution.Height) / 2, 0));
					}
					else
					{
						this.m_RdpClient.SyncSessionDisplaySettings();
					}
				}
				this.m_RdpClient.FullScreen = true;
				this.Refresh();
			}
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0000AE0C File Offset: 0x0000900C
		public void ExitFullScreen()
		{
			if (this.FullScreen)
			{
				this.FullScreen = false;
				this.m_RdpClient.FullScreen = false;
				this.m_RdpClient.Location = new Point(0, 0);
				if (this.IsConnectedBasic)
				{
					this.SetZoomLevel(this.m_PreFullScreenZoomLevel.Value);
					this.m_PreFullScreenZoomLevel = null;
					this.KeyboardInputCaptured = true;
				}
				this.Refresh();
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000127 RID: 295 RVA: 0x0000AE78 File Offset: 0x00009078
		// (set) Token: 0x06000128 RID: 296 RVA: 0x0000AE80 File Offset: 0x00009080
		public bool FullScreen { get; private set; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000129 RID: 297 RVA: 0x0000AE89 File Offset: 0x00009089
		public bool FullScreenUseAllMonitors
		{
			get
			{
				RdpOptions rdpOptions = this.m_RdpOptions;
				return rdpOptions != null && rdpOptions.UseAllMonitors;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600012A RID: 298 RVA: 0x0000AE9C File Offset: 0x0000909C
		public Size DesktopResolution
		{
			get
			{
				return this.m_DesktopResolution;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x0600012B RID: 299 RVA: 0x0000AEA4 File Offset: 0x000090A4
		public Size ScaledDesktopResolution
		{
			get
			{
				return new Size((int)((double)(this.m_DesktopResolution.Width * (int)this.m_CurrentZoomLevel) / 100.0), (int)((double)(this.m_DesktopResolution.Height * (int)this.m_CurrentZoomLevel) / 100.0));
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x0600012C RID: 300 RVA: 0x0000AEF2 File Offset: 0x000090F2
		public bool IsConnected
		{
			get
			{
				return this.ConnectionState >= RdpViewerConnectionState.ConnectedNoVideo;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x0600012D RID: 301 RVA: 0x0000AF00 File Offset: 0x00009100
		public bool IsConnectedBasic
		{
			get
			{
				return this.ConnectionState >= RdpViewerConnectionState.ConnectedNoVideo && this.ConnectionState < RdpViewerConnectionState.ConnectedEnhancedVideo;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600012E RID: 302 RVA: 0x0000AF16 File Offset: 0x00009116
		public bool IsConnectedEnhanced
		{
			get
			{
				return this.ConnectionState >= RdpViewerConnectionState.ConnectedEnhancedVideo;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600012F RID: 303 RVA: 0x0000AF24 File Offset: 0x00009124
		// (set) Token: 0x06000130 RID: 304 RVA: 0x0000AF2C File Offset: 0x0000912C
		public RdpViewerConnectionState ConnectionState
		{
			get
			{
				return this.m_State;
			}
			private set
			{
				if (this.m_State != value)
				{
					this.m_State = value;
					this.SetDisplayStrings();
					EventHandler connectionStateChanged = this.ConnectionStateChanged;
					if (connectionStateChanged == null)
					{
						return;
					}
					connectionStateChanged(this, EventArgs.Empty);
				}
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000131 RID: 305 RVA: 0x0000AF5C File Offset: 0x0000915C
		public RdpAuthenticationType AuthenticationType
		{
			get
			{
				if (this.IsConnected)
				{
					IMsRdpClientAdvancedSettings6 msRdpClientAdvancedSettings = null;
					try
					{
						msRdpClientAdvancedSettings = this.m_RdpClient.AdvancedSettings7;
					}
					catch (ObjectDisposedException ex)
					{
						VMTrace.TraceWarning("Rdp client object is not valid.", ex);
						return RdpAuthenticationType.None;
					}
					return (RdpAuthenticationType)msRdpClientAdvancedSettings.AuthenticationType;
				}
				return RdpAuthenticationType.None;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000132 RID: 306 RVA: 0x0000AFAC File Offset: 0x000091AC
		// (set) Token: 0x06000133 RID: 307 RVA: 0x0000AFB4 File Offset: 0x000091B4
		public bool MouseCursorCaptured
		{
			get
			{
				return this.m_MouseCursorCaptured;
			}
			private set
			{
				if (this.m_MouseCursorCaptured != value)
				{
					this.m_MouseCursorCaptured = value;
					if (this.MouseCursorCapturedChanged != null)
					{
						this.MouseCursorCapturedChanged(this, EventArgs.Empty);
					}
				}
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000134 RID: 308 RVA: 0x0000AFDF File Offset: 0x000091DF
		// (set) Token: 0x06000135 RID: 309 RVA: 0x0000AFE7 File Offset: 0x000091E7
		public bool KeyboardInputCaptured
		{
			get
			{
				return this.m_KeyboardInputCaptured;
			}
			private set
			{
				if (this.m_KeyboardInputCaptured != value)
				{
					this.m_KeyboardInputCaptured = value;
					if (this.KeyboardInputCapturedChanged != null)
					{
						this.KeyboardInputCapturedChanged(this, EventArgs.Empty);
					}
				}
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000136 RID: 310 RVA: 0x0000B012 File Offset: 0x00009212
		// (set) Token: 0x06000137 RID: 311 RVA: 0x0000B01A File Offset: 0x0000921A
		public bool MouseModeRelative
		{
			get
			{
				return this.m_MouseModeRelative;
			}
			private set
			{
				if (this.m_MouseModeRelative != value)
				{
					this.m_MouseModeRelative = value;
					if (!value)
					{
						this.MouseCursorCaptured = false;
					}
					if (this.MouseModeRelativeChanged != null)
					{
						this.MouseModeRelativeChanged(this, EventArgs.Empty);
					}
				}
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000138 RID: 312 RVA: 0x0000B04F File Offset: 0x0000924F
		// (set) Token: 0x06000139 RID: 313 RVA: 0x0000B061 File Offset: 0x00009261
		public VMConnectKeyboardOption KeyboardHookMode
		{
			get
			{
				return (VMConnectKeyboardOption)this.m_RdpClient.SecuredSettings2.KeyboardHookMode;
			}
			set
			{
				if (value > VMConnectKeyboardOption.FullScreen)
				{
					throw new InvalidEnumArgumentException("value", (int)value, typeof(VMConnectKeyboardOption));
				}
				if (this.ConnectionState == RdpViewerConnectionState.NotConnected)
				{
					this.m_RdpClient.SecuredSettings2.KeyboardHookMode = (int)value;
					return;
				}
				throw new InvalidOperationException();
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600013A RID: 314 RVA: 0x0000B09D File Offset: 0x0000929D
		// (set) Token: 0x0600013B RID: 315 RVA: 0x0000B0B0 File Offset: 0x000092B0
		public VMConnectReleaseKey ReleaseKey
		{
			get
			{
				return (VMConnectReleaseKey)this.m_RdpClient.AdvancedSettings7.HotKeyFocusReleaseLeft;
			}
			set
			{
				if (this.ConnectionState == RdpViewerConnectionState.NotConnected)
				{
					int num = (int)VMConnectReleaseKeyHelper.ValidateValue(value);
					IMsRdpClientAdvancedSettings6 advancedSettings = this.m_RdpClient.AdvancedSettings7;
					advancedSettings.HotKeyFocusReleaseLeft = num;
					advancedSettings.HotKeyFocusReleaseRight = num;
					return;
				}
				throw new InvalidOperationException();
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600013C RID: 316 RVA: 0x0000B0EB File Offset: 0x000092EB
		public bool HasSnapshots
		{
			get
			{
				return this.m_SnapshotTracker.HasSnapshots;
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x0600013D RID: 317 RVA: 0x0000B0F8 File Offset: 0x000092F8
		// (set) Token: 0x0600013E RID: 318 RVA: 0x0000B13C File Offset: 0x0000933C
		public bool KeyboardHookLoaded
		{
			get
			{
				bool result;
				try
				{
					result = (this.m_RdpClient.SecuredSettings2.KeyboardHookMode == 1);
				}
				catch (ObjectDisposedException ex)
				{
					VMTrace.TraceWarning("Rdp client object is not valid.", ex);
					result = false;
				}
				return result;
			}
			set
			{
				if (this.ConnectionState == RdpViewerConnectionState.NotConnected)
				{
					try
					{
						if (value)
						{
							this.m_RdpClient.SecuredSettings2.KeyboardHookMode = 1;
						}
						else
						{
							this.m_RdpClient.SecuredSettings2.KeyboardHookMode = 2;
						}
						return;
					}
					catch (ObjectDisposedException ex)
					{
						VMTrace.TraceWarning("Rdp client object is not valid.", ex);
						return;
					}
				}
				throw new InvalidOperationException();
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x0600013F RID: 319 RVA: 0x0000B1A0 File Offset: 0x000093A0
		public bool RdpEnhancedModeAvailable
		{
			get
			{
				return this.m_OldRdpEnhancedModeAvailable && !RdpOptions.IsRestrictedAdminModePolicyEnabled();
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000140 RID: 320 RVA: 0x0000B1B4 File Offset: 0x000093B4
		// (set) Token: 0x06000141 RID: 321 RVA: 0x0000B1C6 File Offset: 0x000093C6
		public bool PreferEnhancedModeConnection
		{
			get
			{
				return this.IsShieldedVm() || this.m_VMConnectUseEnhancedModeRuntimePreference;
			}
			set
			{
				this.m_VMConnectUseEnhancedModeRuntimePreference = value;
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000142 RID: 322 RVA: 0x0000B1CF File Offset: 0x000093CF
		public bool IsDeviceRedirectionInitialized
		{
			get
			{
				return this.m_RdpClientDeviceRedirectionInitialized;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000143 RID: 323 RVA: 0x0000B1D7 File Offset: 0x000093D7
		public IReadOnlyList<string> OfflineDisplayStrings
		{
			get
			{
				return this.m_DisplayStrings;
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000144 RID: 324 RVA: 0x0000B1DF File Offset: 0x000093DF
		public ZoomLevel CurrentZoomLevel
		{
			get
			{
				return this.m_CurrentZoomLevel;
			}
		}

		// Token: 0x06000145 RID: 325 RVA: 0x0000B1E8 File Offset: 0x000093E8
		private void OnConnected(object sender, EventArgs ea)
		{
			VMTrace.TraceUserActionCompleted("RdpViewer connected to virtual machine video successfully.", Array.Empty<string>());
			object connectionLock = this.m_ConnectionLock;
			lock (connectionLock)
			{
				this.ConnectionState = (this.m_ConnectInEnhancedMode ? RdpViewerConnectionState.ConnectedEnhancedVideo : RdpViewerConnectionState.ConnectedBasicVideo);
				object obj = this.IsConnected && this.ConnectionState != RdpViewerConnectionState.ConnectedNoVideo;
				this.m_ReconnectionAttempts = 0;
				this.SetDisplayStrings();
				object obj2 = obj;
				if (obj2 != null)
				{
					this.m_RdpClient.Visible = true;
					this.KeyboardInputCaptured = true;
				}
				this.CaptureKeyboardFocus();
				if (obj2 != null && this.IsConnectedEnhanced && this.m_RdpOptions.FullScreen)
				{
					EventHandler requestEnterFullScreen = this.RequestEnterFullScreen;
					if (requestEnterFullScreen != null)
					{
						requestEnterFullScreen(this, EventArgs.Empty);
					}
				}
			}
		}

		// Token: 0x06000146 RID: 326 RVA: 0x0000B2B4 File Offset: 0x000094B4
		private void OnDisconnected(object sender, IMsTscAxEvents_OnDisconnectedEvent ea)
		{
			bool flag = false;
			bool enhancedMode = false;
			bool flag2 = false;
			bool flag3 = false;
			this.m_DisconnectInProgress = true;
			VMTrace.TraceInformation("RdpViewer disconnected from the virtual machine's video.", Array.Empty<string>());
			bool disconnectedWhileConnected = this.IsConnected;
			if (!this.IsConnected && this.ConnectionState != RdpViewerConnectionState.Connecting)
			{
				VMTrace.TraceInformation("OnDisconnected event is being sent when not in the Connected or Connecting state. This is an extra Disconnected event. Ignore.", Array.Empty<string>());
				return;
			}
			string errorMessage = this.GetDisconnectErrorMessage(ea.discReason, this.m_RdpClient.ExtendedDisconnectReason, disconnectedWhileConnected, out this.m_ConnectionErrorReason);
			if (this.m_ConnectionErrorReason == RdpViewerControl.ConnectionErrorReason.DisconnectedNormally)
			{
				if (this.IsVMVideoAvailable() && !this.IsVMMigratingOrMigrated)
				{
					VMStateChangeAction? informedVMStateChange = this.m_InformedVMStateChange;
					VMStateChangeAction vmstateChangeAction = VMStateChangeAction.Reset;
					if (!(informedVMStateChange.GetValueOrDefault() == vmstateChangeAction & informedVMStateChange != null))
					{
						informedVMStateChange = this.m_InformedVMStateChange;
						vmstateChangeAction = VMStateChangeAction.Turnoff;
						if (!(informedVMStateChange.GetValueOrDefault() == vmstateChangeAction & informedVMStateChange != null))
						{
							informedVMStateChange = this.m_InformedVMStateChange;
							vmstateChangeAction = VMStateChangeAction.SaveState;
							if (!(informedVMStateChange.GetValueOrDefault() == vmstateChangeAction & informedVMStateChange != null))
							{
								informedVMStateChange = this.m_InformedVMStateChange;
								vmstateChangeAction = VMStateChangeAction.Shutdown;
								if (!(informedVMStateChange.GetValueOrDefault() == vmstateChangeAction & informedVMStateChange != null))
								{
									goto IL_132;
								}
							}
						}
					}
				}
				this.m_ConnectionErrorReason = RdpViewerControl.ConnectionErrorReason.NoConnectionError;
				errorMessage = null;
			}
			IL_132:
			object connectionLock = this.m_ConnectionLock;
			lock (connectionLock)
			{
				flag = this.m_SilentDisconnect;
				this.m_SilentDisconnect = false;
				flag3 = this.IsConnectedEnhanced;
				if (!flag)
				{
					this.PrintDisconnectionErrorDebugMessage(ea.discReason, errorMessage);
				}
				this.Deactivate();
				if (this.FullScreen)
				{
					EventHandler requestExitFullScreen = this.RequestExitFullScreen;
					if (requestExitFullScreen != null)
					{
						requestExitFullScreen(this, EventArgs.Empty);
					}
				}
				this.SetDisplayStrings();
				this.ConnectionState = RdpViewerConnectionState.NotConnected;
				if (this.m_ConnectionModeSwitch != RdpViewerControl.RdpViewerModeSwitch.None)
				{
					flag2 = true;
					enhancedMode = (this.m_ConnectionModeSwitch == RdpViewerControl.RdpViewerModeSwitch.SwitchToEnhanced);
					this.m_ConnectionModeSwitch = RdpViewerControl.RdpViewerModeSwitch.None;
				}
				this.CreateRdpClient(false);
				Interlocked.Exchange(ref this.m_BlockConnectionRequests, 0);
				this.m_DisconnectInProgress = false;
			}
			if (this.m_ConnectionErrorReason == RdpViewerControl.ConnectionErrorReason.DisconnectedNormallyLogoffByUser)
			{
				this.Connect(false, true);
				return;
			}
			if (flag2)
			{
				this.Connect(enhancedMode, false);
				return;
			}
			if (this.m_ConnectionErrorReason == RdpViewerControl.ConnectionErrorReason.UnexpectedDisconnection && this.m_ReconnectionAttempts < 5)
			{
				Thread.Sleep(50);
				this.m_ReconnectionAttempts++;
				base.BeginInvoke(new Action<bool, bool>(this.Connect), new object[]
				{
					this.m_ConnectInEnhancedMode,
					false
				});
				return;
			}
			if (this.m_ConnectInEnhancedMode && this.m_ReconnectionAttempts >= 5)
			{
				this.m_ReconnectionAttempts = 0;
				this.m_ConnectInEnhancedMode = false;
				this.Connect(false, true);
				return;
			}
			if (!flag && !string.IsNullOrEmpty(errorMessage))
			{
				if (this.m_ConnectionErrorReason != RdpViewerControl.ConnectionErrorReason.DisconnectedNormally)
				{
					base.BeginInvoke(new RdpViewerControl.DisplayDisconnectionMessageMethod(this.DisplayDisconnectionMessage), new object[]
					{
						errorMessage,
						disconnectedWhileConnected
					});
					return;
				}
				if (flag3 && ea.discReason == 1)
				{
					this.CloseForm();
					return;
				}
				DelayedUIInvoker delayedUIInvoker = new DelayedUIInvoker();
				delayedUIInvoker.DelayTime = TimeSpan.FromSeconds(1.0);
				delayedUIInvoker.Invoked += delegate(object o, EventArgs e)
				{
					if (this.IsVMVideoAvailable())
					{
						this.DisplayDisconnectionMessage(errorMessage, disconnectedWhileConnected);
						return;
					}
					this.m_ConnectionErrorReason = RdpViewerControl.ConnectionErrorReason.NoConnectionError;
				};
				delayedUIInvoker.Invoke();
			}
		}

		// Token: 0x06000147 RID: 327 RVA: 0x0000B5E8 File Offset: 0x000097E8
		private void OnVirtualMachineResolutionChanged(object sender, IMsTscAxEvents_OnRemoteDesktopSizeChangeEvent ea)
		{
			VMTrace.TraceInformation(string.Format("Virtual machine desktop resolution changed. New resolution is: ({0}, {1})", ea.width, ea.height), Array.Empty<string>());
			this.m_DesktopResolution = new Size(ea.width, ea.height);
			EventHandler guestResolutionChanged = this.GuestResolutionChanged;
			if (guestResolutionChanged != null)
			{
				guestResolutionChanged(this, EventArgs.Empty);
			}
			bool flag = this.ConnectionState == RdpViewerConnectionState.ConnectedNoVideo;
			if (this.IsConnected || this.ConnectionState == RdpViewerConnectionState.Connecting)
			{
				this.m_RdpClient.Size = new Size(Math.Min(base.Width, this.ScaledDesktopResolution.Width), Math.Min(base.Height, this.ScaledDesktopResolution.Height));
				if (this.IsConnectedBasic && this.FullScreen)
				{
					this.m_RdpClient.Location = new Point(Math.Max((base.Width - this.m_DesktopResolution.Width) / 2, 0), Math.Max((base.Height - this.m_DesktopResolution.Height) / 2, 0));
				}
				if (ea.width == RdpViewerControl.DisabledVideoResolution.Width && ea.height == RdpViewerControl.DisabledVideoResolution.Height)
				{
					this.ConnectionState = RdpViewerConnectionState.ConnectedNoVideo;
				}
				else if (flag)
				{
					this.m_RdpClient.Visible = true;
				}
				this.SetDisplayStrings();
			}
		}

		// Token: 0x06000148 RID: 328 RVA: 0x0000B749 File Offset: 0x00009949
		private void OnMouseActivate(object sender, EventArgs ea)
		{
			if (this.IsConnected && this.MouseModeRelative)
			{
				if (!this.FullScreen)
				{
					this.MouseCursorCaptured = true;
					return;
				}
				this.m_MouseActivateTimer.Start();
			}
		}

		// Token: 0x06000149 RID: 329 RVA: 0x0000B778 File Offset: 0x00009978
		private void HandleMouseActivateTimer(object sender, EventArgs ea)
		{
			this.m_MouseActivateTimer.Stop();
			NativeMethods.CURSORINFO cursorinfo = default(NativeMethods.CURSORINFO);
			cursorinfo.cbSize = Marshal.SizeOf(typeof(NativeMethods.CURSORINFO));
			if (!NativeMethods.GetCursorInfo(ref cursorinfo))
			{
				VMTrace.TraceWarning("GetCursorInfo failed. Assume cursor is hidden.", Array.Empty<string>());
			}
			if (cursorinfo.flags == 0)
			{
				this.MouseCursorCaptured = true;
			}
		}

		// Token: 0x0600014A RID: 330 RVA: 0x0000B7D8 File Offset: 0x000099D8
		private void HandleVMCacheUpdated(object sender, EventArgs ea)
		{
			if (base.InvokeRequired)
			{
				try
				{
					base.BeginInvoke(new EventHandler(this.HandleVMStateChangeUIThread), new object[]
					{
						sender,
						ea
					});
					base.BeginInvoke(new EventHandler(this.HandleVMNameChangeUIThread), new object[]
					{
						sender,
						ea
					});
					return;
				}
				catch (InvalidOperationException)
				{
					return;
				}
			}
			this.HandleVMStateChangeUIThread(sender, ea);
			this.HandleVMNameChangeUIThread(sender, ea);
		}

		// Token: 0x0600014B RID: 331 RVA: 0x0000B854 File Offset: 0x00009A54
		private void HandleVMDeleted(object sender, EventArgs ea)
		{
			if (base.InvokeRequired)
			{
				base.BeginInvoke(new EventHandler(this.HandleVMDeletedWithWait), new object[]
				{
					sender,
					ea
				});
				return;
			}
			this.HandleVMDeletedWithWait(sender, ea);
		}

		// Token: 0x0600014C RID: 332 RVA: 0x0000B888 File Offset: 0x00009A88
		private void HandleVMDeletedWithWait(object sender, EventArgs ea)
		{
			if (!this.IsVMMigratingOrMigrated)
			{
				Thread.Sleep(1000);
			}
			try
			{
				this.HandleVMDeletedUIThread(sender, ea);
			}
			catch (InvalidOperationException)
			{
			}
		}

		// Token: 0x0600014D RID: 333 RVA: 0x0000B8C8 File Offset: 0x00009AC8
		private void HandleVMStateChangeUIThread(object sender, EventArgs ea)
		{
			if (sender == this.VirtualMachine)
			{
				VMComputerSystemState state = this.VirtualMachine.State;
				VMComputerSystemHealthState healthState = this.VirtualMachine.HealthState;
				bool flag = this.VirtualMachine.EnhancedSessionModeState == EnhancedSessionModeStateType.Available;
				if (state != this.m_OldVMState || healthState != this.m_OldVMHealthState || flag != this.m_OldRdpEnhancedModeAvailable)
				{
					this.m_OldLastConfigurationChange = this.VirtualMachine.TimeOfLastConfigurationChange;
					this.m_VMIsRestoring = (state == VMComputerSystemState.Starting && this.m_OldVMState == VMComputerSystemState.Saved);
					this.m_OldVMState = state;
					this.m_OldVMHealthState = healthState;
					this.m_OldRdpEnhancedModeAvailable = flag;
					VMTrace.TraceInformation(string.Format(CultureInfo.InvariantCulture, "VM State has changed. New state is {0} and new healthstate is {1} ", state, healthState), Array.Empty<string>());
					this.SetDisplayStrings();
					bool flag2 = flag && this.PrepareForEnhancedMode(false);
					bool flag3 = flag2 && (this.IsConnectedBasic || (this.IsShieldedVm() && this.ConnectionState == RdpViewerConnectionState.Connecting));
					bool flag4 = state == VMComputerSystemState.Paused && !flag && this.IsConnectedEnhanced;
					if (this.IsVMVideoAvailable() && (this.ConnectionState == RdpViewerConnectionState.NotConnected || flag3 || flag4))
					{
						if (Program.Displayer.NumberOfDialogsOpen < 1)
						{
							this.Connect(flag2, true);
						}
						else
						{
							Program.Displayer.LastDialogClosed += this.ConnectAfterDialogsClose;
						}
					}
					EventHandler vmstateChanged = this.VMStateChanged;
					if (vmstateChanged == null)
					{
						return;
					}
					vmstateChanged(this, EventArgs.Empty);
					return;
				}
				else
				{
					DateTime timeOfLastConfigurationChange = this.VirtualMachine.TimeOfLastConfigurationChange;
					if (timeOfLastConfigurationChange > this.m_OldLastConfigurationChange)
					{
						this.m_OldLastConfigurationChange = timeOfLastConfigurationChange;
						if (this.VMConfigurationChanged != null)
						{
							this.VMConfigurationChanged(this, EventArgs.Empty);
						}
					}
				}
			}
		}

		// Token: 0x0600014E RID: 334 RVA: 0x0000BA70 File Offset: 0x00009C70
		private void ConnectAfterDialogsClose(object sender, EventArgs ea)
		{
			Program.Displayer.LastDialogClosed -= this.ConnectAfterDialogsClose;
			bool flag = this.m_OldRdpEnhancedModeAvailable && this.PrepareForEnhancedMode(false);
			bool flag2 = flag && (this.IsConnectedBasic || (this.IsShieldedVm() && this.ConnectionState == RdpViewerConnectionState.Connecting));
			if ((this.IsVMVideoAvailable() && this.ConnectionState == RdpViewerConnectionState.NotConnected) || flag2)
			{
				this.Connect(flag, true);
			}
		}

		// Token: 0x0600014F RID: 335 RVA: 0x0000BAEC File Offset: 0x00009CEC
		private void HandleVMNameChangeUIThread(object sender, EventArgs ea)
		{
			if (sender == this.VirtualMachine)
			{
				string name = this.VirtualMachine.Name;
				if (name != this.m_OldVMName)
				{
					this.m_OldVMName = name;
					this.SetDisplayStrings();
					EventHandler vmnameChanged = this.VMNameChanged;
					if (vmnameChanged == null)
					{
						return;
					}
					vmnameChanged(this, EventArgs.Empty);
				}
			}
		}

		// Token: 0x06000150 RID: 336 RVA: 0x0000BB40 File Offset: 0x00009D40
		private void HandleVMDeletedUIThread(object sender, EventArgs ea)
		{
			if (sender == this.VirtualMachine && !this.IsVMMigratingOrMigrated)
			{
				this.m_Deleted = true;
				string name = this.VirtualMachine.Name;
				string mainInstruction;
				if (!this.m_IsVmClustered)
				{
					mainInstruction = string.Format(CultureInfo.CurrentCulture, VMISResources.VMDeleted, name);
				}
				else
				{
					mainInstruction = string.Format(CultureInfo.CurrentCulture, VMISResources.VMDeletedWhileClustered, name, this.VirtualMachine.Server);
				}
				bool flag;
				Program.Displayer.Display(Program.Displayer.ParentWindowHandle, Program.Displayer.DefaultTitle, mainInstruction, null, null, TaskDialogIcon.Information, TaskDialogCommonButtons.None, DialogResult.OK, new string[]
				{
					CommonResources.ButtonExit
				}, 0, false, null, out flag);
				this.CloseForm();
			}
		}

		// Token: 0x06000151 RID: 337 RVA: 0x0000BBF0 File Offset: 0x00009DF0
		private void HandleHasSnapshotsChanged(object sender, EventArgs ea)
		{
			if (base.InvokeRequired)
			{
				try
				{
					base.BeginInvoke(new EventHandler(this.HandleHasSnapshotsChanged), new object[]
					{
						sender,
						ea
					});
					return;
				}
				catch (InvalidOperationException)
				{
					return;
				}
			}
			if (this.HasSnapshotsChanged != null)
			{
				this.HasSnapshotsChanged(this, ea);
			}
		}

		// Token: 0x06000152 RID: 338 RVA: 0x0000BC50 File Offset: 0x00009E50
		private void HandleMigrationStarted(object sender, EventArgs ea)
		{
			if (base.InvokeRequired)
			{
				try
				{
					base.BeginInvoke(new EventHandler(this.HandleMigrationStarted), new object[]
					{
						sender,
						ea
					});
				}
				catch (InvalidOperationException)
				{
				}
				return;
			}
			VMTrace.TraceInformation("A migration of this virtual machine has started.", Array.Empty<string>());
			this.m_MigrationData.IsMigrating = true;
			EventHandler vmstateChanged = this.VMStateChanged;
			if (vmstateChanged == null)
			{
				return;
			}
			vmstateChanged(this, EventArgs.Empty);
		}

		// Token: 0x06000153 RID: 339 RVA: 0x0000BCD0 File Offset: 0x00009ED0
		private void HandleMigrationCompleted(object sender, MigrationTracker.MigrationCompletedStatusEventArgs ea)
		{
			if (base.InvokeRequired)
			{
				try
				{
					base.BeginInvoke(new EventHandler<MigrationTracker.MigrationCompletedStatusEventArgs>(this.HandleMigrationCompleted), new object[]
					{
						sender,
						ea
					});
				}
				catch (InvalidOperationException)
				{
				}
				return;
			}
			this.m_MigrationData.IsMigrating = false;
			this.m_MigrationData.IsMigrated = (ea.CompletionStatus == VMTaskStatus.CompletedSuccessfully);
			this.m_MigrationData.DestinationHostName = ea.DestinationHost;
			VMTrace.TraceInformation("A migration of this virtual machine has completed. Success: " + this.m_MigrationData.IsMigrated.ToString(CultureInfo.InvariantCulture), Array.Empty<string>());
			if (this.m_MigrationData.IsMigrated)
			{
				this.m_MigrationData.MigrationDataFetched += this.OnMigrationDataFetchCompleted;
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.m_MigrationData.FetchDestinationData));
				return;
			}
			this.m_MigrationData.Clear();
			EventHandler vmstateChanged = this.VMStateChanged;
			if (vmstateChanged == null)
			{
				return;
			}
			vmstateChanged(this, EventArgs.Empty);
		}

		// Token: 0x06000154 RID: 340 RVA: 0x0000BDD8 File Offset: 0x00009FD8
		private void OnMigrationDataFetchCompleted(object sender, EventArgs ea)
		{
			if (base.InvokeRequired)
			{
				try
				{
					base.BeginInvoke(new EventHandler(this.OnMigrationDataFetchCompleted), new object[]
					{
						sender,
						ea
					});
				}
				catch (InvalidOperationException)
				{
				}
				return;
			}
			MigrationData migrationData = (MigrationData)sender;
			migrationData.MigrationDataFetched -= this.OnMigrationDataFetchCompleted;
			bool flag = migrationData.FetchDestinationDataError == null;
			Exception exception = migrationData.FetchDestinationDataError;
			if ((migrationData.DestinationHost == null || migrationData.MigratedVM == null || migrationData.RdpPortOnDestination == 0) && migrationData.FetchDestinationDataError == null)
			{
				VMTrace.TraceWarning("OnMigrationDataFetchCompleted called with null or incomplete migration data.", Array.Empty<string>());
				return;
			}
			VMTrace.TraceInformation("Fetching data on the destination machine has completed. Success: " + flag.ToString(CultureInfo.InvariantCulture), Array.Empty<string>());
			if (flag)
			{
				try
				{
					this.CleanupVirtualMachine();
					this.m_RdpConnectionInfo = new RdpConnectionInfo
					{
						ServerConnectionName = this.m_MigrationData.DestinationHost.FullName,
						VirtualMachine = this.m_MigrationData.MigratedVM,
						RdpPort = this.m_MigrationData.RdpPortOnDestination
					};
					this.SetupVirtualMachine();
				}
				catch (Exception ex)
				{
					flag = false;
					exception = ex;
				}
			}
			if (flag)
			{
				this.m_MigrationData.Clear();
				EventHandler vmnameChanged = this.VMNameChanged;
				if (vmnameChanged != null)
				{
					vmnameChanged(this, EventArgs.Empty);
				}
				EventHandler vmstateChanged = this.VMStateChanged;
				if (vmstateChanged != null)
				{
					vmstateChanged(this, EventArgs.Empty);
				}
				this.SetDisplayStrings();
				bool enhancedMode = this.PrepareForEnhancedMode(false);
				this.Connect(enhancedMode, false);
				return;
			}
			string mainInstruction = string.Format(CultureInfo.CurrentCulture, VMISResources.VMMigrated, this.VirtualMachine.Name, migrationData.DestinationHostName);
			string summary;
			string details;
			InformationDisplayer.GetErrorInformationFromException(exception, out summary, out details, null);
			bool flag2;
			Program.Displayer.Display(Program.Displayer.ParentWindowHandle, Program.Displayer.DefaultTitle, mainInstruction, summary, details, TaskDialogIcon.Information, TaskDialogCommonButtons.None, DialogResult.OK, new string[]
			{
				CommonResources.ButtonExit
			}, 0, false, null, out flag2);
			this.CloseForm();
		}

		// Token: 0x06000155 RID: 341 RVA: 0x0000BFD0 File Offset: 0x0000A1D0
		internal bool SyncDisplaySettings()
		{
			if (!this.FullScreen)
			{
				return false;
			}
			bool result;
			try
			{
				this.m_RdpClient.SyncSessionDisplaySettings();
				this.m_RdpClient.Location = new Point(0, 0);
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x0000C020 File Offset: 0x0000A220
		internal bool UpdateDisplaySettings(Size resolution, bool force = false)
		{
			if (this.ConnectionState != RdpViewerConnectionState.ConnectedEnhancedVideoSyncedSession)
			{
				return false;
			}
			if (this.FullScreen)
			{
				return true;
			}
			if (this.m_DesktopResolution.Width == resolution.Width && this.m_DesktopResolution.Height == resolution.Height && !force)
			{
				return true;
			}
			bool result;
			try
			{
				Screen.FromControl(this);
				uint ulDesktopScaleFactor = (uint)((double)this.DeviceDpi() / 96.0 * 100.0);
				this.m_RdpClient.UpdateSessionDisplaySettings((uint)resolution.Width, (uint)resolution.Height, (uint)DpiUtilities.DevicePixelsToPhysicalSizeInMillimeters(base.Handle, resolution.Width), (uint)DpiUtilities.DevicePixelsToPhysicalSizeInMillimeters(base.Handle, resolution.Height), 0U, ulDesktopScaleFactor, 100U);
				this.m_DesktopResolution = resolution;
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000157 RID: 343 RVA: 0x0000C0F8 File Offset: 0x0000A2F8
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 262 && (int)m.WParam == 32)
			{
				base.BeginInvoke(new MethodInvoker(this.ReleaseAltKey));
				this.Deactivate();
			}
			base.WndProc(ref m);
		}

		// Token: 0x06000158 RID: 344 RVA: 0x0000C138 File Offset: 0x0000A338
		public bool IsVMVideoAvailable()
		{
			if (this.m_RdpConnectionInfo.VirtualMachine == null)
			{
				return false;
			}
			VMComputerSystemState state = this.m_RdpConnectionInfo.VirtualMachine.State;
			return state == VMComputerSystemState.Running || state == VMComputerSystemState.Paused || state == VMComputerSystemState.Pausing || state == VMComputerSystemState.Stopping || state == VMComputerSystemState.Saving;
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0000C184 File Offset: 0x0000A384
		public void Connect(bool enhancedMode, bool checkForExistingConnections = true)
		{
			CommonConfiguration instance = CommonConfiguration.Instance;
			object connectionLock = this.m_ConnectionLock;
			lock (connectionLock)
			{
				if (!this.m_DisconnectInProgress)
				{
					if (this.m_ConnectionModeSwitch == RdpViewerControl.RdpViewerModeSwitch.None && this.IsConnected && this.IsConnectedEnhanced != enhancedMode && (!enhancedMode || this.m_OldRdpEnhancedModeAvailable))
					{
						if (enhancedMode)
						{
							this.m_ConnectionModeSwitch = RdpViewerControl.RdpViewerModeSwitch.SwitchToEnhanced;
						}
						else
						{
							this.m_ConnectionModeSwitch = RdpViewerControl.RdpViewerModeSwitch.SwitchToBasic;
						}
						if (!enhancedMode && checkForExistingConnections && instance.NeedToConfirm(Confirmations.AskAboutKickingOffOtherUsers))
						{
							ThreadPool.QueueUserWorkItem(new WaitCallback(this.ConnectionBackgroundWork), this.VirtualMachine);
						}
						else
						{
							this.ConnectCallback(this.VirtualMachine, false);
						}
					}
					else if ((this.ConnectionState == RdpViewerConnectionState.NotConnected || this.IsShieldedVm()) && this.IsVMVideoAvailable() && this.m_RdpConnectionInfo.VirtualMachine.State != VMComputerSystemState.Stopping)
					{
						this.m_ConnectInEnhancedMode = enhancedMode;
						this.m_ConnectionErrorReason = RdpViewerControl.ConnectionErrorReason.NoConnectionError;
						this.ConnectionState = RdpViewerConnectionState.Connecting;
						this.SetDisplayStrings();
						if (!this.m_ConnectInEnhancedMode && checkForExistingConnections && instance.NeedToConfirm(Confirmations.AskAboutKickingOffOtherUsers))
						{
							ThreadPool.QueueUserWorkItem(new WaitCallback(this.ConnectionBackgroundWork), this.VirtualMachine);
						}
						else
						{
							this.ConnectCallback(this.VirtualMachine, false);
						}
					}
					else
					{
						Interlocked.Exchange(ref this.m_BlockConnectionRequests, 0);
					}
				}
			}
		}

		// Token: 0x0600015A RID: 346 RVA: 0x0000C2F0 File Offset: 0x0000A4F0
		public void Disconnect()
		{
			object connectionLock = this.m_ConnectionLock;
			lock (connectionLock)
			{
				this.m_SilentDisconnect = true;
				if ((this.IsConnected || this.ConnectionState == RdpViewerConnectionState.Connecting) && !this.m_RdpClient.Disposing && !this.m_RdpClient.IsDisposed && this.m_RdpClient.IsHandleCreated)
				{
					try
					{
						this.m_RdpClient.Disconnect();
					}
					catch (ObjectDisposedException ex)
					{
						VMTrace.TraceWarning("rdp client got disposed while disconnecting.", ex);
					}
					catch (Exception ex2)
					{
						COMException ex3 = ex2 as COMException;
						if (ex3 == null || ex3.ErrorCode != -2147467259)
						{
							if (ex3 != null)
							{
								VMTrace.TraceError("The mstscax control encountered a COM error while trying to disconnect! COM error code: " + ex3.ErrorCode.ToString(), Array.Empty<string>());
							}
						}
						else
						{
							VMTrace.TraceError("The mstscax control returned E_FAIL when calling its Disconnect method.", Array.Empty<string>());
						}
					}
				}
			}
		}

		// Token: 0x0600015B RID: 347 RVA: 0x0000C3F4 File Offset: 0x0000A5F4
		public Image CopyScreenImage()
		{
			Image result = null;
			if (this.IsConnected)
			{
				byte[] thumbnailImage = this.VirtualMachine.Setting.GetThumbnailImage(this.m_DesktopResolution.Width, this.m_DesktopResolution.Height);
				if (thumbnailImage != null)
				{
					result = CommonUtilities.CreateBitmap(this.m_DesktopResolution.Width, this.m_DesktopResolution.Height, thumbnailImage);
				}
			}
			return result;
		}

		// Token: 0x0600015C RID: 348 RVA: 0x0000C454 File Offset: 0x0000A654
		[SuppressMessage("Microsoft.Design", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Caller is not granted access to operations or resources that can be used in a destructive manner.")]
		public void Deactivate()
		{
			IOleInPlaceObject oleInPlaceObject = null;
			try
			{
				oleInPlaceObject = (this.m_RdpClient.GetOcx() as IOleInPlaceObject);
			}
			catch (ObjectDisposedException ex)
			{
				VMTrace.TraceWarning("Rdp client object is not valid.", ex);
				return;
			}
			if (oleInPlaceObject != null)
			{
				oleInPlaceObject.UIDeactivate();
				if (!this.FullScreen)
				{
					InteractiveSessionForm interactiveSessionForm = (InteractiveSessionForm)this._form;
					if (interactiveSessionForm != null)
					{
						interactiveSessionForm.StatusBar.Focus();
					}
				}
				else
				{
					base.Focus();
				}
				this.MouseCursorCaptured = false;
				this.KeyboardInputCaptured = false;
				return;
			}
			VMTrace.TraceError("QueryInterface for IOleInPlaceObject on the Rdp Control failed.", Array.Empty<string>());
		}

		// Token: 0x0600015D RID: 349 RVA: 0x0000C4E8 File Offset: 0x0000A6E8
		public void InformBeginStateChangeOperation(VMStateChangeAction stateChange)
		{
			this.m_InformedVMStateChange = new VMStateChangeAction?(stateChange);
			if (stateChange != VMStateChangeAction.Shutdown && (stateChange == VMStateChangeAction.TurnOn || stateChange == VMStateChangeAction.Restore))
			{
				this.SetDisplayStrings();
			}
		}

		// Token: 0x0600015E RID: 350 RVA: 0x0000C508 File Offset: 0x0000A708
		public void InformEndStateChangeOperation(VMStateChangeAction stateChange)
		{
			VMStateChangeAction? informedVMStateChange = this.m_InformedVMStateChange;
			this.m_InformedVMStateChange = null;
			if (stateChange != VMStateChangeAction.Shutdown && informedVMStateChange != null && (informedVMStateChange.Value == VMStateChangeAction.TurnOn || informedVMStateChange.Value == VMStateChangeAction.Restore))
			{
				this.SetDisplayStrings();
			}
		}

		// Token: 0x0600015F RID: 351 RVA: 0x0000C550 File Offset: 0x0000A750
		public void CaptureKeyboardFocus()
		{
			bool flag = false;
			try
			{
				flag = this.m_RdpClient.Visible;
			}
			catch (ObjectDisposedException ex)
			{
				VMTrace.TraceWarning("Rdp client object is not valid.", ex);
				return;
			}
			if (flag)
			{
				this.m_RdpClient.Focus();
				this.KeyboardInputCaptured = true;
			}
		}

		// Token: 0x06000160 RID: 352 RVA: 0x0000C5A4 File Offset: 0x0000A7A4
		public void CleanupVirtualMachine()
		{
			if (this.VirtualMachine != null && !this.m_Deleted)
			{
				this.VirtualMachine.CacheUpdated -= this.HandleVMCacheUpdated;
				Program.TasksToCompleteBeforeExitingTracker.RegisterBackgroundTask(new WaitCallback(this.CancelImeEvents), new object[]
				{
					this.VirtualMachine
				});
				this.m_SnapshotTracker.CleanupVirtualMachineSnapshots();
			}
			this.m_MigrationTracker.Cleanup();
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000C614 File Offset: 0x0000A814
		public void SetupVirtualMachine()
		{
			if (this.VirtualMachine != null)
			{
				this.m_OldVMState = this.VirtualMachine.State;
				this.m_OldVMHealthState = this.VirtualMachine.HealthState;
				this.m_OldVMName = this.VirtualMachine.Name;
				this.m_OldLastConfigurationChange = this.VirtualMachine.TimeOfLastConfigurationChange;
				this.m_OldRdpEnhancedModeAvailable = (this.VirtualMachine.EnhancedSessionModeState == EnhancedSessionModeStateType.Available);
				this.m_IsVmClustered = false;
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.LoadIsClustered), this.VirtualMachine);
				this.VirtualMachine.CacheUpdated += this.HandleVMCacheUpdated;
				this.m_SnapshotTracker.SetNewVirtualMachine(this.VirtualMachine);
				this.m_MigrationData.Setup(this.VirtualMachine);
				this.m_MigrationTracker.Setup(this.VirtualMachine);
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.RegisterForDeletedEvent), this.VirtualMachine);
			}
		}

		// Token: 0x06000162 RID: 354 RVA: 0x0000C708 File Offset: 0x0000A908
		public bool ConnectingInMultiMon()
		{
			IMsRdpClientNonScriptable5 msRdpClientNonScriptable = null;
			try
			{
				msRdpClientNonScriptable = (this.m_RdpClient.GetOcx() as IMsRdpClientNonScriptable5);
			}
			catch (ObjectDisposedException ex)
			{
				VMTrace.TraceWarning("Rdp client object is not valid.", ex);
				return false;
			}
			return msRdpClientNonScriptable.UseMultimon && msRdpClientNonScriptable.RemoteMonitorLayoutMatchesLocal;
		}

		// Token: 0x06000163 RID: 355 RVA: 0x0000C75C File Offset: 0x0000A95C
		public bool PrepareForEnhancedMode(bool vmConnectUseEnhancedModeForce)
		{
			bool flag;
			if (!this.m_InPrepareForEnhancedMode && this.RdpEnhancedModeAvailable && (this.PreferEnhancedModeConnection || vmConnectUseEnhancedModeForce))
			{
				this.m_InPrepareForEnhancedMode = true;
				flag = true;
				bool showOptions = this.ShowOptions;
				if (!this.IsDeviceRedirectionInitialized)
				{
					RdpOptions rdpOptions = new RdpOptions(this.VirtualMachine);
					if (!rdpOptions.InitializedFromSavedConfig || showOptions)
					{
						flag = this.DisplayRdpOptionDialog(rdpOptions);
						flag &= !base.IsDisposed;
						this.PreferEnhancedModeConnection = flag;
					}
					this.VirtualMachine.UpdatePropertyCache();
					if (flag && this.VirtualMachine.EnhancedSessionModeState != EnhancedSessionModeStateType.Available)
					{
						string mainInstruction = string.Format(CultureInfo.CurrentCulture, VMISResources.Enhanced_NotAvailable, this.VirtualMachine.Name);
						string summary = string.Format(CultureInfo.CurrentCulture, VMISResources.Enhanced_NotAvailableSummary, Array.Empty<object>());
						string details = string.Format(CultureInfo.CurrentCulture, VMISResources.Enhanced_NotAvailableDetails, Array.Empty<object>());
						Program.Displayer.DisplayInformation(mainInstruction, summary, details);
						flag = false;
					}
					if (flag)
					{
						this.m_RdpOptions = rdpOptions;
						this.m_RdpClientDeviceRedirectionInitialized = true;
					}
				}
				this.m_InPrepareForEnhancedMode = false;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000164 RID: 356 RVA: 0x0000C86C File Offset: 0x0000AA6C
		public void SetZoomLevel(ZoomLevel level)
		{
			if (level == ZoomLevel.Auto)
			{
				level = (ZoomLevel)((int)((double)this.DeviceDpi() / 96.0 * 100.0));
			}
			if (this.IsConnectedBasic && this.m_CurrentZoomLevel != level)
			{
				IMsRdpExtendedSettings msRdpExtendedSettings;
				if ((msRdpExtendedSettings = (this.m_RdpClient.GetOcx() as IMsRdpExtendedSettings)) != null)
				{
					object obj = (uint)level;
					msRdpExtendedSettings.set_Property("ZoomLevel", ref obj);
				}
				this.m_CurrentZoomLevel = level;
				if (!this.FullScreen)
				{
					EventHandler guestResolutionChanged = this.GuestResolutionChanged;
					if (guestResolutionChanged != null)
					{
						guestResolutionChanged(this, EventArgs.Empty);
					}
				}
				this.m_RdpClient.Size = new Size(Math.Min(base.Width, this.ScaledDesktopResolution.Width), Math.Min(base.Height, this.ScaledDesktopResolution.Height));
			}
		}

		// Token: 0x06000165 RID: 357 RVA: 0x0000C944 File Offset: 0x0000AB44
		private bool DisplayRdpOptionDialog(RdpOptions rdpOptions)
		{
			EnhancedConnectionDialog enhancedConnectionDialog = new EnhancedConnectionDialog(Screen.FromControl(this), rdpOptions, this.VirtualMachine.Name);
			enhancedConnectionDialog.StartPosition = FormStartPosition.CenterParent;
			DialogResult dialogResult = enhancedConnectionDialog.ShowDialog(this);
			enhancedConnectionDialog.Dispose();
			return dialogResult != DialogResult.Cancel;
		}

		// Token: 0x06000166 RID: 358 RVA: 0x0000C984 File Offset: 0x0000AB84
		public void OnNotifyRedirectDeviceChange(IntPtr wParam, IntPtr lParam)
		{
			try
			{
				this.m_RdpClient.OnNotifyRedirectDeviceChange(wParam, lParam);
			}
			catch (ObjectDisposedException ex)
			{
				VMTrace.TraceWarning("Rdp client object is not valid.", ex);
			}
		}

		// Token: 0x06000167 RID: 359 RVA: 0x0000C9C0 File Offset: 0x0000ABC0
		public bool IsShieldedVm()
		{
			IVMComputerSystem virtualMachine = this.VirtualMachine;
			bool? flag;
			if (virtualMachine == null)
			{
				flag = null;
			}
			else
			{
				IVMSecurityInformation securityInformation = virtualMachine.SecurityInformation;
				flag = ((securityInformation != null) ? new bool?(securityInformation.Shielded) : null);
			}
			bool? flag2 = flag;
			return flag2.GetValueOrDefault();
		}

		// Token: 0x06000168 RID: 360 RVA: 0x0000CA08 File Offset: 0x0000AC08
		private void SetupRdpEventHandlers()
		{
			this.m_RdpClient.OnConnected += this.OnConnected;
			this.m_RdpClient.OnLoginComplete += delegate(object sender, EventArgs args)
			{
				if (this.ConnectionState == RdpViewerConnectionState.ConnectedEnhancedVideo)
				{
					this.ConnectionState = RdpViewerConnectionState.ConnectedEnhancedVideoSyncedSession;
				}
			};
			this.m_RdpClient.OnDisconnected += this.OnDisconnected;
			this.m_RdpClient.OnRequestContainerMinimize += delegate(object sender, EventArgs ea)
			{
				EventHandler requestFullScreenMinimize = this.RequestFullScreenMinimize;
				if (requestFullScreenMinimize == null)
				{
					return;
				}
				requestFullScreenMinimize(this, EventArgs.Empty);
			};
			this.m_RdpClient.OnRequestLeaveFullScreen += delegate(object sender, EventArgs ea)
			{
				EventHandler requestExitFullScreen = this.RequestExitFullScreen;
				if (requestExitFullScreen == null)
				{
					return;
				}
				requestExitFullScreen(this, EventArgs.Empty);
			};
			this.m_RdpClient.OnRemoteDesktopSizeChange += this.OnVirtualMachineResolutionChanged;
			this.m_RdpClient.OnConfirmClose += delegate(object sender, IMsTscAxEvents_OnConfirmCloseEvent ea)
			{
				EventHandler requestFullScreenClose = this.RequestFullScreenClose;
				if (requestFullScreenClose == null)
				{
					return;
				}
				requestFullScreenClose(this, EventArgs.Empty);
			};
			this.m_RdpClient.OnRequestGoFullScreen += delegate(object sender, EventArgs ea)
			{
				EventHandler requestEnterFullScreen = this.RequestEnterFullScreen;
				if (requestEnterFullScreen == null)
				{
					return;
				}
				requestEnterFullScreen(this, EventArgs.Empty);
			};
			this.m_RdpClient.OnMouseInputModeChanged += delegate(object sender, IMsTscAxEvents_OnMouseInputModeChangedEvent ea)
			{
				this.MouseModeRelative = ea.fMouseModeRelative;
			};
			this.m_RdpClient.OnFocusReleased += delegate(object sender, IMsTscAxEvents_OnFocusReleasedEvent ea)
			{
				this.Deactivate();
			};
			this.m_RdpClient.MouseActivate += this.OnMouseActivate;
			this.m_RdpClient.Enter += delegate(object sender, EventArgs ea)
			{
				this.KeyboardInputCaptured = true;
			};
		}

		// Token: 0x06000169 RID: 361 RVA: 0x0000CB2C File Offset: 0x0000AD2C
		private void ReleaseAltKey()
		{
			try
			{
				if (this.VirtualMachine != null)
				{
					IVMKeyboard keyboardDeviceForVM = ClipboardMenu.GetKeyboardDeviceForVM(this.VirtualMachine);
					if (keyboardDeviceForVM != null)
					{
						keyboardDeviceForVM.ReleaseKey(18);
					}
				}
			}
			catch (VirtualizationManagementException ex)
			{
				VMTrace.TraceWarning("Unable to release Alt key.", ex);
			}
			catch (Exception ex2)
			{
				VMTrace.TraceWarning("Unable to release Alt key.", ex2);
				Program.UnhandledExceptionHandler.HandleBackgroundThreadException(ex2);
			}
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000CBA0 File Offset: 0x0000ADA0
		private void ConnectionBackgroundWork(object virtualMachineObj)
		{
			IVMComputerSystem ivmcomputerSystem = (IVMComputerSystem)virtualMachineObj;
			bool flag = false;
			try
			{
				IVMComputerSystem vmcomputerSystem = ObjectLocator.GetVMComputerSystem(ivmcomputerSystem.Server, ivmcomputerSystem.InstanceId);
				vmcomputerSystem.UpdateAssociationCache();
				flag = vmcomputerSystem.DoesTerminalConnectionExist();
			}
			catch (VirtualizationManagementException ex)
			{
				VMTrace.TraceWarning("Error determining if vm is connected to by another user!", ex);
			}
			catch (Exception ex2)
			{
				VMTrace.TraceError("Error determining if vm is connected to by another user!", ex2);
				Program.UnhandledExceptionHandler.HandleBackgroundThreadException(ex2);
			}
			try
			{
				base.BeginInvoke(new RdpViewerControl.ConnectCallbackMethod(this.ConnectCallback), new object[]
				{
					ivmcomputerSystem,
					flag
				});
			}
			catch (InvalidOperationException ex3)
			{
				this.ConnectionState = RdpViewerConnectionState.NotConnected;
				Interlocked.Exchange(ref this.m_BlockConnectionRequests, 0);
				VMTrace.TraceWarning("vmconnect closed. Ignore error.", ex3);
			}
		}

		// Token: 0x0600016B RID: 363 RVA: 0x0000CC74 File Offset: 0x0000AE74
		private void ConnectCallback(IVMComputerSystem virtualMachine, bool existingConnections)
		{
			bool flag = !existingConnections;
			bool flag2 = false;
			object connectionLock = this.m_ConnectionLock;
			lock (connectionLock)
			{
				virtualMachine.UpdateAssociationCache();
				if (!this.m_DisconnectInProgress)
				{
					if (this.IsConnected)
					{
						if (this.m_ConnectionModeSwitch != RdpViewerControl.RdpViewerModeSwitch.None)
						{
							this.Disconnect();
						}
					}
					else if (this.ConnectionState == RdpViewerConnectionState.Connecting)
					{
						if (Interlocked.Exchange(ref this.m_BlockConnectionRequests, 1) == 0)
						{
							if (this.IsShieldedVm())
							{
								this.VirtualMachine.UpdatePropertyCache();
								if (this.VirtualMachine.EnhancedSessionModeState == EnhancedSessionModeStateType.Disabled)
								{
									this.ConnectionState = RdpViewerConnectionState.NotConnected;
									this.m_ConnectionErrorReason = RdpViewerControl.ConnectionErrorReason.VirtualMachineShielded;
								}
								else
								{
									this.ConnectionState = RdpViewerConnectionState.Connecting;
									flag2 = true;
								}
								this.SetDisplayStrings();
							}
							else
							{
								flag2 = true;
							}
						}
					}
				}
			}
			if (flag2)
			{
				VMTrace.TraceInformation("Processing the connect request.", Array.Empty<string>());
				if (existingConnections)
				{
					CommonConfiguration instance = CommonConfiguration.Instance;
					if (instance.NeedToConfirm(Confirmations.AskAboutKickingOffOtherUsers))
					{
						string mainInstruction = string.Format(CultureInfo.CurrentCulture, VMISResources.ConnectWithExistingUser, this.VirtualMachine.Name);
						string[] customButtons = new string[]
						{
							VMISResources.Disconnected_WhileConnecting_ReconnectButton,
							CommonResources.ButtonExit
						};
						bool flag4;
						flag = (Program.Displayer.DisplayConfirmation(Program.Displayer.DefaultTitle, mainInstruction, customButtons, 1, TaskDialogIcon.Warning, CommonResources.ConfirmationVerifyText, out flag4) == 0);
						if (flag4)
						{
							instance.TurnoffNeedToConfirmFlag(Confirmations.AskAboutKickingOffOtherUsers);
							ThreadPool.QueueUserWorkItem(new WaitCallback(CommonUtilities.SaveConfiguration), instance);
						}
					}
					else
					{
						flag = true;
					}
				}
				if (flag)
				{
					try
					{
						this.m_CurrentZoomLevel = ZoomLevel.P100;
						this.m_RdpClient.Server = this.m_RdpConnectionInfo.ServerConnectionName;
						this.m_RdpClient.AdvancedSettings2.RDPPort = this.m_RdpConnectionInfo.RdpPort;
						VMTrace.TraceUserActionInitiated(string.Format(CultureInfo.InvariantCulture, "Connecting with server full name: {0} to RDP port {1}", this.m_RdpClient.Server, this.m_RdpClient.AdvancedSettings2.RDPPort), Array.Empty<string>());
						this.m_RdpClient.AdvancedSettings2.minInputSendInterval = 20;
						this.m_RdpClient.AdvancedSettings7.PCB = this.m_RdpConnectionInfo.VirtualMachine.InstanceId.ToString(CultureInfo.InvariantCulture);
						this.VirtualMachine.UpdatePropertyCache();
						if (this.m_ConnectInEnhancedMode && this.VirtualMachine.EnhancedSessionModeState == EnhancedSessionModeStateType.Available)
						{
							this.m_RdpClient.ColorDepth = 32;
							if (this.m_RdpOptions.FullScreen)
							{
								Screen screen;
								if (this.m_RdpOptions.EnhancedModeDialogScreen != null)
								{
									screen = this.m_RdpOptions.EnhancedModeDialogScreen;
									this.m_RdpOptions.EnhancedModeDialogScreen = null;
								}
								else
								{
									screen = Screen.FromControl(this);
								}
								this.m_RdpClient.DesktopWidth = screen.Bounds.Width;
								this.m_RdpClient.DesktopHeight = screen.Bounds.Height;
							}
							else
							{
								this.m_RdpClient.DesktopWidth = this.m_RdpOptions.DesktopSize.Width;
								this.m_RdpClient.DesktopHeight = this.m_RdpOptions.DesktopSize.Height;
							}
							this.m_RdpClient.AdvancedSettings8.RedirectSmartCards = this.m_RdpOptions.SmartCardsRedirection;
							if (this.m_RdpOptions.EnablePrinterRedirection)
							{
								this.m_RdpClient.AdvancedSettings8.RedirectPrinters = this.m_RdpOptions.PrinterRedirection;
							}
							this.m_RdpClient.AdvancedSettings8.RedirectClipboard = this.m_RdpOptions.ClipboardRedirection;
							this.m_RdpClient.AdvancedSettings8.AudioRedirectionMode = (uint)this.m_RdpOptions.AudioPlaybackRedirectionMode;
							this.m_RdpClient.AdvancedSettings8.AudioCaptureRedirectionMode = this.m_RdpOptions.AudioCaptureRedirectionMode;
							((IMsRdpClientNonScriptable5)this.m_RdpClient.GetOcx()).UseMultimon = this.m_RdpOptions.UseAllMonitors;
							this.EnableDriveRedirection();
							this.EnableDeviceRedirection(RdpOptions.RedirectedDeviceType.Usb);
							this.EnableDeviceRedirection(RdpOptions.RedirectedDeviceType.Pnp);
							this.m_RdpClient.AdvancedSettings7.PCB = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", this.m_RdpClient.AdvancedSettings7.PCB, ";", "EnhancedMode=1");
						}
						else
						{
							this.m_ConnectInEnhancedMode = false;
							if (this.IsShieldedVm())
							{
								Interlocked.Exchange(ref this.m_BlockConnectionRequests, 0);
								return;
							}
						}
						this.m_RdpClient.Connect();
						return;
					}
					catch (Exception ex)
					{
						COMException ex2 = ex as COMException;
						if (ex2 == null || ex2.ErrorCode != -2147467259)
						{
							if (ex2 != null)
							{
								VMTrace.TraceError("The mstscax control encountered a COM error while trying to connect! COM error code: " + ex2.ErrorCode.ToString(), Array.Empty<string>());
							}
						}
						else
						{
							VMTrace.TraceError("The mstscax control returned E_FAIL when calling its Connect method.", Array.Empty<string>());
						}
						VMTrace.TraceError("The mstscax control encountered an error while trying to connect!", ex);
						string connectMethodThrew = VMISResources.ConnectMethodThrew;
						this.m_ConnectionErrorReason = RdpViewerControl.ConnectionErrorReason.UnknownLoginFailure;
						this.ConnectionState = RdpViewerConnectionState.NotConnected;
						this.SetDisplayStrings();
						Interlocked.Exchange(ref this.m_BlockConnectionRequests, 0);
						if (!this.IsVMMigratingOrMigrated)
						{
							this.DisplayDisconnectionMessage(connectMethodThrew, false);
						}
						return;
					}
				}
				this.m_ConnectionErrorReason = RdpViewerControl.ConnectionErrorReason.UserCanceledConnect;
				this.ConnectionState = RdpViewerConnectionState.NotConnected;
				this.SetDisplayStrings();
				Form form = this._form;
				if (form == null)
				{
					return;
				}
				form.Close();
				return;
			}
			EventHandler vmstateChanged = this.VMStateChanged;
			if (vmstateChanged == null)
			{
				return;
			}
			vmstateChanged(this, EventArgs.Empty);
		}

		// Token: 0x0600016C RID: 364 RVA: 0x0000D1C0 File Offset: 0x0000B3C0
		private void EnableDriveRedirection()
		{
			StringCollection redirectedDeviceCollection = this.m_RdpOptions.GetRedirectedDeviceCollection(RdpOptions.RedirectedDeviceType.Drive);
			bool flag = false;
			if (redirectedDeviceCollection != null && redirectedDeviceCollection.Count > 0)
			{
				IMsRdpClientNonScriptable5 msRdpClientNonScriptable = (IMsRdpClientNonScriptable5)this.m_RdpClient.GetOcx();
				if (msRdpClientNonScriptable != null)
				{
					if (redirectedDeviceCollection.Contains(RdpOptions.WildcardAllDevices))
					{
						flag = true;
					}
					IMsRdpDriveCollection driveCollection = msRdpClientNonScriptable.DriveCollection;
					for (uint num = 0U; num < driveCollection.DriveCount; num += 1U)
					{
						IMsRdpDrive msRdpDrive = driveCollection.get_DriveByIndex(num);
						if (flag)
						{
							msRdpDrive.RedirectionState = true;
						}
						else
						{
							string value = msRdpDrive.Name[0].ToString().ToLower(CultureInfo.InvariantCulture);
							if (redirectedDeviceCollection.Contains(value))
							{
								msRdpDrive.RedirectionState = true;
							}
						}
					}
					if (flag || redirectedDeviceCollection.Contains(RdpOptions.DynamicDrives.ToLower(CultureInfo.InvariantCulture)))
					{
						msRdpClientNonScriptable.RedirectDynamicDrives = true;
					}
				}
			}
		}

		// Token: 0x0600016D RID: 365 RVA: 0x0000D29C File Offset: 0x0000B49C
		private void EnableDeviceRedirection(RdpOptions.RedirectedDeviceType deviceType)
		{
			if (deviceType != RdpOptions.RedirectedDeviceType.Usb && deviceType != RdpOptions.RedirectedDeviceType.Pnp)
			{
				return;
			}
			IMsRdpClientNonScriptable5 msRdpClientNonScriptable = (IMsRdpClientNonScriptable5)this.m_RdpClient.GetOcx();
			if (msRdpClientNonScriptable != null)
			{
				StringCollection redirectedDeviceCollection = this.m_RdpOptions.GetRedirectedDeviceCollection(deviceType);
				bool flag = false;
				if (redirectedDeviceCollection != null && redirectedDeviceCollection.Count > 0)
				{
					if (redirectedDeviceCollection.Contains(RdpOptions.WildcardAllDevices))
					{
						flag = true;
					}
					IMsRdpDeviceCollection deviceCollection = msRdpClientNonScriptable.DeviceCollection;
					for (uint num = 0U; num < deviceCollection.DeviceCount; num += 1U)
					{
						IMsRdpDevice msRdpDevice = deviceCollection.get_DeviceByIndex(num);
						if (flag)
						{
							IMsRdpDeviceV2 msRdpDeviceV = (IMsRdpDeviceV2)msRdpDevice;
							if (deviceType == RdpOptions.RedirectedDeviceType.Usb)
							{
								if (msRdpDeviceV.IsUSBDevice())
								{
									msRdpDevice.RedirectionState = true;
								}
							}
							else if (!msRdpDeviceV.IsUSBDevice())
							{
								msRdpDevice.RedirectionState = true;
							}
						}
						else
						{
							string value = msRdpDevice.DeviceInstanceId.TrimEnd(new char[1]).ToLower(CultureInfo.InvariantCulture);
							if (redirectedDeviceCollection.Contains(value))
							{
								msRdpDevice.RedirectionState = true;
							}
						}
					}
					if (deviceType == RdpOptions.RedirectedDeviceType.Pnp && (flag || redirectedDeviceCollection.Contains(RdpOptions.DynamicDevices.ToLower(CultureInfo.InvariantCulture))))
					{
						msRdpClientNonScriptable.RedirectDynamicDevices = true;
					}
				}
			}
		}

		// Token: 0x0600016E RID: 366 RVA: 0x0000D3AC File Offset: 0x0000B5AC
		private void LoadIsClustered(object state)
		{
			try
			{
				foreach (IVMDeviceSetting ivmdeviceSetting in ((IVMComputerSystem)state).Setting.GetDeviceSettings())
				{
					if (ivmdeviceSetting.VMDeviceSettingType == VMDeviceSettingType.DataExchange)
					{
						IVMDataExchangeComponentSetting ivmdataExchangeComponentSetting = (IVMDataExchangeComponentSetting)ivmdeviceSetting;
						ivmdataExchangeComponentSetting.UpdatePropertyCache(TimeSpan.FromSeconds(2.0));
						this.m_IsVmClustered = ivmdataExchangeComponentSetting.VMIsClustered;
						break;
					}
				}
			}
			catch (Exception ex)
			{
				VMTrace.TraceWarning("Failed to load if the vm is clustered. Will assume that it is not.", ex);
			}
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000D44C File Offset: 0x0000B64C
		private void RegisterForDeletedEvent(object vmObj)
		{
			IVMComputerSystem ivmcomputerSystem = vmObj as IVMComputerSystem;
			try
			{
				if (ivmcomputerSystem != null)
				{
					ivmcomputerSystem.Deleted += this.HandleVMDeleted;
				}
			}
			catch (Exception ex)
			{
				VMTrace.TraceError("Encountered an error registering for the vm's Deleted event!", ex);
			}
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000D498 File Offset: 0x0000B698
		private void CancelImeEvents(object vmObj)
		{
			IVMComputerSystem ivmcomputerSystem = vmObj as IVMComputerSystem;
			try
			{
				if (ivmcomputerSystem != null)
				{
					ivmcomputerSystem.UnregisterForInstanceModificationEvents();
					ivmcomputerSystem.Deleted -= this.HandleVMDeleted;
				}
			}
			catch (Exception ex)
			{
				VMTrace.TraceWarning("Unable to unregistering for the vm's IME and Deleted events.", ex);
			}
		}

		// Token: 0x06000171 RID: 369 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
		private void DisplayDisconnectionMessage(string errorMessage, bool disconnectedWhileConnected)
		{
			TaskDialogIcon taskDialogIcon = TaskDialogIcon.Warning;
			if (this.m_ConnectionErrorReason == RdpViewerControl.ConnectionErrorReason.AuthorizationFailure || this.m_ConnectionErrorReason == RdpViewerControl.ConnectionErrorReason.LoginTimedOut || this.m_ConnectionErrorReason == RdpViewerControl.ConnectionErrorReason.UnknownLoginFailure || this.m_ConnectionErrorReason == RdpViewerControl.ConnectionErrorReason.UnexpectedDisconnection)
			{
				taskDialogIcon = TaskDialogIcon.Error;
			}
			if (this.ConnectionState != RdpViewerConnectionState.NotConnected || !this.IsVMVideoAvailable())
			{
				if (this.ConnectionState == RdpViewerConnectionState.NotConnected)
				{
					Program.Displayer.Display(Program.Displayer.ParentWindowHandle, Program.Displayer.DefaultTitle, errorMessage, null, null, taskDialogIcon, true);
				}
				return;
			}
			string mainInstruction;
			string[] customerButtons;
			if (disconnectedWhileConnected)
			{
				mainInstruction = errorMessage + "\r\n\r\n" + VMISResources.Disconnected_WhileConnected_ReconnectQuestion;
				customerButtons = new string[]
				{
					VMISResources.Disconnected_WhileConnected_ReconnectButton,
					CommonResources.ButtonExit
				};
			}
			else
			{
				mainInstruction = errorMessage + "\r\n\r\n" + VMISResources.Disconnected_WhileConnecting_ReconnectQuestion;
				customerButtons = new string[]
				{
					VMISResources.Disconnected_WhileConnecting_ReconnectButton,
					CommonResources.ButtonExit
				};
			}
			if (Program.Displayer.DisplayChoices(Program.Displayer.DefaultTitle, mainInstruction, null, customerButtons, 0, taskDialogIcon) == 0)
			{
				bool enhancedMode = this.PrepareForEnhancedMode(false);
				this.Connect(enhancedMode, true);
				return;
			}
			Form form = this._form;
			if (form == null)
			{
				return;
			}
			form.Close();
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000D600 File Offset: 0x0000B800
		private void PrintDisconnectionErrorDebugMessage(int disconnectReason, string disconnectErrorMsg)
		{
			if (this.m_ConnectionErrorReason == RdpViewerControl.ConnectionErrorReason.DisconnectedNormally)
			{
				VMTrace.TraceInformation(string.Format(CultureInfo.InvariantCulture, "The server performed a normal disconnect of the client. Disconnect code: '{0}'", disconnectReason.ToString("X", CultureInfo.InvariantCulture)), Array.Empty<string>());
				return;
			}
			try
			{
				VMTrace.TraceWarning(string.Format(CultureInfo.InvariantCulture, "The server disconnected the client with the text '{0}'. Disconnect code: '{1}', Extended disconnect code: '{2}'.", this.m_RdpClient.DisconnectedText, disconnectReason.ToString("X", CultureInfo.InvariantCulture), this.m_RdpClient.ExtendedDisconnectReason.ToString()), Array.Empty<string>());
			}
			catch (ObjectDisposedException ex)
			{
				VMTrace.TraceWarning("Rdp client object is not valid.", ex);
				return;
			}
			if (!string.IsNullOrEmpty(disconnectErrorMsg))
			{
				VMTrace.TraceWarning("Disconnect error message: " + disconnectErrorMsg, Array.Empty<string>());
			}
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000D6D0 File Offset: 0x0000B8D0
		private string GetDisconnectErrorMessage(int disconnectReason, ExtendedDisconnectReasonCode extendedDisconnectReason, bool disconnectedWhileConnected, out RdpViewerControl.ConnectionErrorReason connectionErrorReason)
		{
			string text = null;
			if (disconnectedWhileConnected || this.m_ConnectInEnhancedMode || disconnectReason == 2308)
			{
				connectionErrorReason = RdpViewerControl.ConnectionErrorReason.UnexpectedDisconnection;
			}
			else
			{
				connectionErrorReason = RdpViewerControl.ConnectionErrorReason.UnknownLoginFailure;
			}
			switch (extendedDisconnectReason)
			{
			case ExtendedDisconnectReasonCode.exDiscReasonNoInfo:
			case ExtendedDisconnectReasonCode.exDiscReasonAPIInitiatedDisconnect:
			case ExtendedDisconnectReasonCode.exDiscReasonAPIInitiatedLogoff:
				goto IL_BD;
			case ExtendedDisconnectReasonCode.exDiscReasonServerIdleTimeout:
			case ExtendedDisconnectReasonCode.exDiscReasonServerLogonTimeout:
			case ExtendedDisconnectReasonCode.exDiscReasonServerInsufficientPrivileges:
			case ExtendedDisconnectReasonCode.exDiscReasonServerFreshCredsRequired:
			case ExtendedDisconnectReasonCode.exDiscReasonRpcInitiatedDisconnectByUser:
				break;
			case ExtendedDisconnectReasonCode.exDiscReasonReplacedByOtherConnection:
				connectionErrorReason = RdpViewerControl.ConnectionErrorReason.DisconnectedNormally;
				text = VMISResources.DisconnectReasonExtended_ReplacedByOtherConnection;
				goto IL_BD;
			case ExtendedDisconnectReasonCode.exDiscReasonOutOfMemory:
				text = VMISResources.DisconnectReasonExtended_ServerOutOfMempory;
				goto IL_BD;
			case ExtendedDisconnectReasonCode.exDiscReasonServerDeniedConnection:
			case ExtendedDisconnectReasonCode.exDiscReasonServerDeniedConnectionFips:
				text = VMISResources.DisconnectReasonExtended_ServerDeniedConnection;
				goto IL_BD;
			case ExtendedDisconnectReasonCode.exDiscReasonLogoffByUser:
				connectionErrorReason = RdpViewerControl.ConnectionErrorReason.DisconnectedNormallyLogoffByUser;
				text = VMISResources.DisconnectReason_NormallyWhileConnected;
				goto IL_BD;
			default:
				if (extendedDisconnectReason == ExtendedDisconnectReasonCode.exDiscReasonRdpEncInvalidCredentials)
				{
					connectionErrorReason = RdpViewerControl.ConnectionErrorReason.AuthorizationFailure;
					text = VMISResources.DisconnectReasonExtended_AccessToVideoDenied;
					goto IL_BD;
				}
				break;
			}
			VMTrace.TraceWarning(string.Format(CultureInfo.InvariantCulture, "Extended disconnect reason '{0}' is unexpected in a virtual machine scenario.", extendedDisconnectReason.ToString()), Array.Empty<string>());
			IL_BD:
			if (text == null)
			{
				if (disconnectReason <= 3848)
				{
					if (disconnectReason <= 2052)
					{
						if (disconnectReason <= 1032)
						{
							if (disconnectReason <= 264)
							{
								switch (disconnectReason)
								{
								case 0:
								case 2:
								case 3:
								case 4:
									connectionErrorReason = RdpViewerControl.ConnectionErrorReason.DisconnectedNormally;
									return disconnectedWhileConnected ? VMISResources.DisconnectReason_NormallyWhileConnected : VMISResources.DisconnectReason_NormallyWhileConnecting;
								case 1:
									if (!disconnectedWhileConnected)
									{
										connectionErrorReason = RdpViewerControl.ConnectionErrorReason.UserCanceledConnect;
										return VMISResources.DisconnectReason_ConnectionAttemptCanceled;
									}
									connectionErrorReason = RdpViewerControl.ConnectionErrorReason.DisconnectedNormally;
									return VMISResources.DisconnectReason_NormallyWhileConnected;
								default:
									switch (disconnectReason)
									{
									case 260:
										break;
									case 261:
									case 263:
										goto IL_51E;
									case 262:
										goto IL_46C;
									case 264:
										connectionErrorReason = RdpViewerControl.ConnectionErrorReason.LoginTimedOut;
										return VMISResources.DisconnectReason_LoginTimedOut;
									default:
										goto IL_51E;
									}
									break;
								}
							}
							else
							{
								switch (disconnectReason)
								{
								case 516:
									goto IL_435;
								case 517:
								case 519:
									goto IL_51E;
								case 518:
									goto IL_46C;
								case 520:
									break;
								default:
									switch (disconnectReason)
									{
									case 772:
										break;
									case 773:
										goto IL_51E;
									case 774:
										goto IL_46C;
									case 775:
										goto IL_482;
									case 776:
										goto IL_42A;
									default:
										switch (disconnectReason)
										{
										case 1028:
											break;
										case 1029:
											goto IL_51E;
										case 1030:
											goto IL_477;
										case 1031:
											goto IL_482;
										case 1032:
											goto IL_42A;
										default:
											goto IL_51E;
										}
										break;
									}
									return VMISResources.DisconnectReason_NetworkError;
								}
							}
						}
						else if (disconnectReason <= 1544)
						{
							if (disconnectReason == 1286)
							{
								goto IL_477;
							}
							switch (disconnectReason)
							{
							case 1540:
								break;
							case 1541:
							case 1543:
								goto IL_51E;
							case 1542:
								goto IL_477;
							case 1544:
								goto IL_46C;
							default:
								goto IL_51E;
							}
						}
						else
						{
							if (disconnectReason == 1796)
							{
								goto IL_435;
							}
							if (disconnectReason == 1798)
							{
								goto IL_477;
							}
							if (disconnectReason != 2052)
							{
								goto IL_51E;
							}
						}
						IL_42A:
						return VMISResources.DisconnectReason_BadServerName;
					}
					if (disconnectReason <= 2823)
					{
						if (disconnectReason <= 2308)
						{
							if (disconnectReason == 2054)
							{
								goto IL_477;
							}
							if (disconnectReason != 2308)
							{
								goto IL_51E;
							}
							return VMISResources.DisconnectReason_SocketClosed;
						}
						else
						{
							if (disconnectReason == 2310)
							{
								goto IL_477;
							}
							switch (disconnectReason)
							{
							case 2566:
								goto IL_477;
							case 2567:
								connectionErrorReason = RdpViewerControl.ConnectionErrorReason.AuthorizationFailure;
								return VMISResources.DisconnectReason_NoSuchUser;
							case 2568:
								break;
							default:
								switch (disconnectReason)
								{
								case 2820:
									return VMISResources.DisconnectReason_NetworkInitConnectError;
								case 2821:
									goto IL_51E;
								case 2822:
									goto IL_482;
								case 2823:
									connectionErrorReason = RdpViewerControl.ConnectionErrorReason.AuthorizationFailure;
									return VMISResources.DisconnectReason_AccountDisabled;
								default:
									goto IL_51E;
								}
								break;
							}
						}
					}
					else if (disconnectReason <= 3336)
					{
						switch (disconnectReason)
						{
						case 3078:
							goto IL_482;
						case 3079:
							connectionErrorReason = RdpViewerControl.ConnectionErrorReason.AuthorizationFailure;
							return VMISResources.DisconnectReason_AccountRestricted;
						case 3080:
							return VMISResources.DisconnectReason_DecompressionFailure;
						default:
							switch (disconnectReason)
							{
							case 3334:
								break;
							case 3335:
								connectionErrorReason = RdpViewerControl.ConnectionErrorReason.AuthorizationFailure;
								return VMISResources.DisconnectReason_AccountLockedOut;
							case 3336:
								goto IL_46C;
							default:
								goto IL_51E;
							}
							break;
						}
					}
					else
					{
						if (disconnectReason == 3591)
						{
							connectionErrorReason = RdpViewerControl.ConnectionErrorReason.AuthorizationFailure;
							return VMISResources.DisconnectReason_AccountExpired;
						}
						if (disconnectReason == 3847)
						{
							connectionErrorReason = RdpViewerControl.ConnectionErrorReason.AuthorizationFailure;
							return VMISResources.DisconnectReason_InvalidLogonHours;
						}
						if (disconnectReason != 3848)
						{
							goto IL_51E;
						}
						return VMISResources.DisconnectReason_PolicyProhibitsConnection;
					}
					return disconnectedWhileConnected ? VMISResources.DisconnectReason_InternalError_Connected : VMISResources.DisconnectReason_InternalError_Connecting;
					IL_477:
					return VMISResources.DisconnectReason_SecurityError;
					IL_482:
					return VMISResources.DisconnectReason_EncryptionError;
				}
				if (disconnectReason <= 9988)
				{
					if (disconnectReason <= 6919)
					{
						if (disconnectReason <= 4356)
						{
							if (disconnectReason == 4102)
							{
								goto IL_46C;
							}
							if (disconnectReason != 4356)
							{
								goto IL_51E;
							}
						}
						else if (disconnectReason != 4612 && disconnectReason != 4868)
						{
							if (disconnectReason != 6919)
							{
								goto IL_51E;
							}
							connectionErrorReason = RdpViewerControl.ConnectionErrorReason.AuthorizationFailure;
							return VMISResources.DisconnectReason_CertExpired;
						}
					}
					else if (disconnectReason <= 8708)
					{
						if (disconnectReason != 8452 && disconnectReason != 8708)
						{
							goto IL_51E;
						}
					}
					else if (disconnectReason != 8964 && disconnectReason != 9220)
					{
						if (disconnectReason != 9988)
						{
							goto IL_51E;
						}
						goto IL_435;
					}
				}
				else if (disconnectReason <= 13316)
				{
					if (disconnectReason <= 10500)
					{
						if (disconnectReason == 10244)
						{
							goto IL_435;
						}
						if (disconnectReason != 10500)
						{
							goto IL_51E;
						}
					}
					else
					{
						if (disconnectReason != 12548 && disconnectReason != 13060 && disconnectReason != 13316)
						{
							goto IL_51E;
						}
						goto IL_435;
					}
				}
				else if (disconnectReason <= 13828)
				{
					if (disconnectReason != 13572 && disconnectReason != 13828)
					{
						goto IL_51E;
					}
					goto IL_435;
				}
				else
				{
					if (disconnectReason != 14084 && disconnectReason != 14340 && disconnectReason != 14596)
					{
						goto IL_51E;
					}
					goto IL_435;
				}
				return VMISResources.DisconnectReason_ClientSideProtocolError;
				IL_435:
				return VMISResources.DisconnectReason_ConnectionFailed;
				IL_46C:
				return VMISResources.DisconnectReason_LowMemory;
				IL_51E:
				VMTrace.TraceWarning(string.Format(CultureInfo.InvariantCulture, "Unexpected error disconnect reason '{0}'.", disconnectReason.ToString("X", CultureInfo.InvariantCulture)), Array.Empty<string>());
				text = (disconnectedWhileConnected ? VMISResources.DisconnectReason_UnexpectedErrorCode_Connected : VMISResources.DisconnectReason_UnexpectedErrorCode_Connecting);
			}
			return text;
		}

		// Token: 0x06000174 RID: 372 RVA: 0x0000DC38 File Offset: 0x0000BE38
		private void SetDisplayStrings()
		{
			string a = this.m_DisplayStrings[0];
			string a2 = this.m_DisplayStrings[1];
			switch (this.ConnectionState)
			{
			case RdpViewerConnectionState.NoVirtualMachine:
				this.m_DisplayStrings[0] = VMISResources.RdpViewer_Information_NotConnected;
				this.m_DisplayStrings[1] = string.Empty;
				break;
			case RdpViewerConnectionState.NotConnected:
				if (this.IsVMVideoAvailable())
				{
					switch (this.m_ConnectionErrorReason)
					{
					case RdpViewerControl.ConnectionErrorReason.NoConnectionError:
						this.m_DisplayStrings[0] = string.Empty;
						this.m_DisplayStrings[1] = string.Empty;
						break;
					case RdpViewerControl.ConnectionErrorReason.DisconnectedNormally:
						this.m_DisplayStrings[0] = VMISResources.RdpViewer_Information_DisconnectedNormally;
						this.m_DisplayStrings[1] = string.Empty;
						break;
					case RdpViewerControl.ConnectionErrorReason.AuthorizationFailure:
						this.m_DisplayStrings[0] = VMISResources.RdpViewer_Information_LogonFailed_Authorization;
						this.m_DisplayStrings[1] = VMISResources.RdpViewer_Resolution_LogonFailed_Authorization;
						break;
					case RdpViewerControl.ConnectionErrorReason.LoginTimedOut:
					case RdpViewerControl.ConnectionErrorReason.UnknownLoginFailure:
						this.m_DisplayStrings[0] = string.Format(CultureInfo.CurrentCulture, VMISResources.RdpViewer_Information_CouldNotConnect, this.VirtualMachine.Name);
						this.m_DisplayStrings[1] = VMISResources.RdpViewer_Resolution_CouldNotConnect;
						break;
					case RdpViewerControl.ConnectionErrorReason.UnexpectedDisconnection:
						this.m_DisplayStrings[0] = VMISResources.RdpViewer_Information_DisconnectedUnexpectedly;
						this.m_DisplayStrings[1] = VMISResources.RdpViewer_Resolution_DisconnectedUnexpectedly;
						break;
					case RdpViewerControl.ConnectionErrorReason.UserCanceledConnect:
						this.m_DisplayStrings[0] = VMISResources.RdpViewer_Information_ConnectCanceled;
						this.m_DisplayStrings[1] = string.Empty;
						break;
					case RdpViewerControl.ConnectionErrorReason.DisconnectedNormallyLogoffByUser:
						this.m_DisplayStrings[0] = VMISResources.RdpViewer_Information_DisconnectedNormallyLogoffByUser;
						this.m_DisplayStrings[1] = string.Empty;
						break;
					case RdpViewerControl.ConnectionErrorReason.VirtualMachineShielded:
						this.m_DisplayStrings[0] = VMISResources.RdpViewer_Information_VirtualMachineShieldedEnhancedSessionDisabled;
						this.m_DisplayStrings[1] = VMISResources.RdpViewer_Resolution_VirtualMachineShieldedEnhancedSessionDisabled;
						break;
					}
				}
				else if (this.m_InformedVMStateChange != null && this.m_InformedVMStateChange.Value == VMStateChangeAction.TurnOn)
				{
					this.m_DisplayStrings[0] = string.Format(CultureInfo.CurrentCulture, VMISResources.RdpViewer_Informaton_InitiatedStarting, this.VirtualMachine.Name);
					this.m_DisplayStrings[1] = string.Empty;
				}
				else if (this.m_InformedVMStateChange != null && this.m_InformedVMStateChange.Value == VMStateChangeAction.Restore)
				{
					this.m_DisplayStrings[0] = string.Format(CultureInfo.CurrentCulture, VMISResources.RdpViewer_Information_InitiatedRestoring, this.VirtualMachine.Name);
					this.m_DisplayStrings[1] = string.Empty;
				}
				else if (this.VirtualMachine.State == VMComputerSystemState.Starting)
				{
					if (!this.m_VMIsRestoring)
					{
						this.m_DisplayStrings[0] = string.Format(CultureInfo.CurrentCulture, VMISResources.RdpViewer_Informaton_VMStarting, this.VirtualMachine.Name);
						this.m_DisplayStrings[1] = string.Empty;
					}
					else
					{
						this.m_DisplayStrings[0] = string.Format(CultureInfo.CurrentCulture, VMISResources.RdpViewer_Information_VMRestoring, this.VirtualMachine.Name);
						this.m_DisplayStrings[1] = string.Empty;
					}
				}
				else if (this.VirtualMachine.State == VMComputerSystemState.Pausing)
				{
					this.m_DisplayStrings[0] = string.Format(CultureInfo.CurrentCulture, VMISResources.RdpViewer_Information_Pausing, this.VirtualMachine.Name);
					this.m_DisplayStrings[1] = string.Empty;
				}
				else if (this.VirtualMachine.State == VMComputerSystemState.ComponentServicing)
				{
					this.m_DisplayStrings[0] = string.Format(CultureInfo.CurrentCulture, VMISResources.RdpViewer_Information_ComponentServicing, this.VirtualMachine.Name);
					this.m_DisplayStrings[1] = string.Empty;
				}
				else if (this.VirtualMachine.State == VMComputerSystemState.Resuming)
				{
					this.m_DisplayStrings[0] = string.Format(CultureInfo.CurrentCulture, VMISResources.RdpViewer_Information_Resuming, this.VirtualMachine.Name);
					this.m_DisplayStrings[1] = string.Empty;
				}
				else
				{
					this.m_DisplayStrings[0] = string.Format(CultureInfo.CurrentCulture, VMISResources.RdpViewer_Information_NotStarted, this.VirtualMachine.Name);
					this.m_DisplayStrings[1] = VMISResources.RdpViewer_Resolution_NotStarted;
				}
				break;
			case RdpViewerConnectionState.Connecting:
				if (this.IsShieldedVm())
				{
					this.VirtualMachine.UpdatePropertyCache();
					if (this.VirtualMachine.EnhancedSessionModeState == EnhancedSessionModeStateType.Disabled)
					{
						this.ConnectionState = RdpViewerConnectionState.NotConnected;
						this.m_ConnectionErrorReason = RdpViewerControl.ConnectionErrorReason.VirtualMachineShielded;
						this.m_DisplayStrings[0] = VMISResources.RdpViewer_Information_VirtualMachineShieldedEnhancedSessionDisabled;
						this.m_DisplayStrings[1] = VMISResources.RdpViewer_Resolution_VirtualMachineShieldedEnhancedSessionDisabled;
					}
					else if (this.VirtualMachine.EnhancedSessionModeState == EnhancedSessionModeStateType.Enabled)
					{
						this.m_DisplayStrings[0] = VMISResources.RdpViewer_Information_VirtualMachineShieldedEnhancedSessionEnabled;
						this.m_DisplayStrings[1] = VMISResources.RdpViewer_Resolution_VirtualMachineShieldedEnhancedSessionEnabled;
					}
					else
					{
						this.m_DisplayStrings[0] = string.Format(CultureInfo.CurrentCulture, VMISResources.RdpViewer_Information_VirtualMachineShieldedEnhancedSessionAvailable, this.VirtualMachine.Name);
						this.m_DisplayStrings[1] = VMISResources.RdpViewer_Resolution_VirtualMachineShieldedEnhancedSessionAvailable;
					}
				}
				else
				{
					this.m_DisplayStrings[0] = string.Format(CultureInfo.CurrentCulture, VMISResources.RdpViewer_Information_Connecting, this.VirtualMachine.Name);
					this.m_DisplayStrings[1] = string.Empty;
				}
				break;
			case RdpViewerConnectionState.ConnectedNoVideo:
				this.m_DisplayStrings[0] = VMISResources.RdpViewer_Information_DisconnectedNormally;
				this.m_DisplayStrings[1] = VMISResources.DisconnectReason_3DVideo;
				break;
			case RdpViewerConnectionState.ConnectedBasicVideo:
			case RdpViewerConnectionState.ConnectedEnhancedVideo:
			case RdpViewerConnectionState.ConnectedEnhancedVideoSyncedSession:
				this.m_DisplayStrings[0] = string.Empty;
				this.m_DisplayStrings[1] = string.Empty;
				break;
			}
			if (a != this.m_DisplayStrings[0] || a2 != this.m_DisplayStrings[1])
			{
				EventHandler offlineDisplayStringsUpdated = this.OfflineDisplayStringsUpdated;
				if (offlineDisplayStringsUpdated == null)
				{
					return;
				}
				offlineDisplayStringsUpdated(this, EventArgs.Empty);
			}
		}

		// Token: 0x06000175 RID: 373 RVA: 0x0000E160 File Offset: 0x0000C360
		[SuppressMessage("Microsoft.Design", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Called is not granted access to operations or resources that can be used in a destructive manner.")]
		private void SetSecureModeOn()
		{
			IMsRdpClientNonScriptable3 msRdpClientNonScriptable = (IMsRdpClientNonScriptable3)this.m_RdpClient.GetOcx();
			this.m_RdpClient.AdvancedSettings6.AuthenticationLevel = 0U;
			msRdpClientNonScriptable.EnableCredSspSupport = true;
			msRdpClientNonScriptable.NegotiateSecurityLayer = false;
			try
			{
				IMsRdpExtendedSettings msRdpExtendedSettings = (IMsRdpExtendedSettings)this.m_RdpClient.GetOcx();
				object obj = true;
				msRdpExtendedSettings.set_Property("DisableCredentialsDelegation", ref obj);
			}
			catch (Exception)
			{
				VMTrace.TraceWarning("Ignoring failure to set the DisableCredentialsDelegation property on the RDP control.", Array.Empty<string>());
			}
		}

		// Token: 0x06000176 RID: 374 RVA: 0x0000E1E4 File Offset: 0x0000C3E4
		private void EnableFrameBufferRedirection()
		{
			bool flag = true;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Virtualization"))
				{
					if (registryKey != null)
					{
						object value = registryKey.GetValue("DisableFrameBufferRedirection");
						if (value != null && value is int)
						{
							flag = ((int)value == 0);
						}
					}
				}
			}
			catch (Exception ex)
			{
				VMTrace.TraceWarning("Failed while attempting to read DisableFrameBufferRedirection reg key.", ex);
			}
			try
			{
				if (flag)
				{
					VMTrace.TraceInformation("Enabling frame buffer redirection for the RDP control. Frame buffer redirection is only used for local connections.", Array.Empty<string>());
					IMsRdpExtendedSettings msRdpExtendedSettings = (IMsRdpExtendedSettings)this.m_RdpClient.GetOcx();
					object obj = true;
					msRdpExtendedSettings.set_Property("EnableFrameBufferRedirection", ref obj);
				}
				else
				{
					VMTrace.TraceInformation("Not enabling frame buffer redirection due to presence of the DisableFrameBufferRedirection registry key.", Array.Empty<string>());
				}
			}
			catch (Exception ex2)
			{
				VMTrace.TraceWarning("Failed to enable frame buffer redirection on the RDP control. Frame buffer redirection is an unnecessary optimization which we can easily run without.", ex2);
			}
		}

		// Token: 0x06000177 RID: 375 RVA: 0x0000E2C8 File Offset: 0x0000C4C8
		private void CloseForm()
		{
			if (this.FullScreen)
			{
				EventHandler requestExitFullScreen = this.RequestExitFullScreen;
				if (requestExitFullScreen != null)
				{
					requestExitFullScreen(this, EventArgs.Empty);
				}
			}
			Form form = this._form;
			if (form == null)
			{
				return;
			}
			form.Close();
		}

		// Token: 0x040000C0 RID: 192
		public static readonly Size BiosResolution = new Size(640, 400);

		// Token: 0x040000C1 RID: 193
		public static readonly Size DisabledVideoResolution = new Size(240, 240);

		// Token: 0x040000C2 RID: 194
		private const int gm_MaxReconnectionAttempts = 5;

		// Token: 0x040000C3 RID: 195
		private const string gm_EnhancedModeBlob = "EnhancedMode=1";

		// Token: 0x040000C4 RID: 196
		private const string gm_RdpPreconnectBlobDelimiter = ";";

		// Token: 0x040000C5 RID: 197
		private Form _form;

		// Token: 0x040000C6 RID: 198
		private RdpClient m_RdpClient;

		// Token: 0x040000C7 RID: 199
		private bool m_RdpClientDeviceRedirectionInitialized;

		// Token: 0x040000C8 RID: 200
		private RdpConnectionInfo m_RdpConnectionInfo;

		// Token: 0x040000C9 RID: 201
		private RdpOptions m_RdpOptions;

		// Token: 0x040000CA RID: 202
		private bool m_ConnectInEnhancedMode;

		// Token: 0x040000CB RID: 203
		private readonly object m_ConnectionLock = new object();

		// Token: 0x040000CC RID: 204
		private int m_BlockConnectionRequests;

		// Token: 0x040000CD RID: 205
		private bool m_DisconnectInProgress;

		// Token: 0x040000CE RID: 206
		private Size m_DesktopResolution;

		// Token: 0x040000CF RID: 207
		private string[] m_DisplayStrings = new string[]
		{
			string.Empty,
			string.Empty
		};

		// Token: 0x040000D0 RID: 208
		private RdpViewerConnectionState m_State;

		// Token: 0x040000D1 RID: 209
		private VMComputerSystemState m_OldVMState;

		// Token: 0x040000D2 RID: 210
		private VMComputerSystemHealthState m_OldVMHealthState;

		// Token: 0x040000D3 RID: 211
		private string m_OldVMName;

		// Token: 0x040000D4 RID: 212
		private DateTime m_OldLastConfigurationChange;

		// Token: 0x040000D5 RID: 213
		private bool m_OldRdpEnhancedModeAvailable;

		// Token: 0x040000D6 RID: 214
		private RdpViewerControl.ConnectionErrorReason m_ConnectionErrorReason;

		// Token: 0x040000D7 RID: 215
		private int m_ReconnectionAttempts;

		// Token: 0x040000D8 RID: 216
		private bool m_MouseCursorCaptured;

		// Token: 0x040000D9 RID: 217
		private bool m_KeyboardInputCaptured;

		// Token: 0x040000DA RID: 218
		private bool m_MouseModeRelative;

		// Token: 0x040000DB RID: 219
		private VMStateChangeAction? m_InformedVMStateChange;

		// Token: 0x040000DC RID: 220
		private bool m_VMIsRestoring;

		// Token: 0x040000DD RID: 221
		private RdpViewerControl.SnapshotTracker m_SnapshotTracker = new RdpViewerControl.SnapshotTracker();

		// Token: 0x040000DE RID: 222
		private MigrationTracker m_MigrationTracker = new MigrationTracker();

		// Token: 0x040000DF RID: 223
		private MigrationData m_MigrationData = new MigrationData();

		// Token: 0x040000E0 RID: 224
		private bool m_Deleted;

		// Token: 0x040000E1 RID: 225
		private bool m_IsVmClustered;

		// Token: 0x040000E2 RID: 226
		private System.Windows.Forms.Timer m_MouseActivateTimer;

		// Token: 0x040000E3 RID: 227
		private bool m_SilentDisconnect;

		// Token: 0x040000E4 RID: 228
		private bool m_InPrepareForEnhancedMode;

		// Token: 0x040000E5 RID: 229
		private bool m_VMConnectUseEnhancedModeRuntimePreference;

		// Token: 0x040000E6 RID: 230
		private RdpViewerControl.RdpViewerModeSwitch m_ConnectionModeSwitch;

		// Token: 0x040000E7 RID: 231
		private ZoomLevel m_CurrentZoomLevel;

		// Token: 0x040000E8 RID: 232
		private ZoomLevel? m_PreFullScreenZoomLevel;

		// Token: 0x02000041 RID: 65
		private enum ConnectionErrorReason
		{
			// Token: 0x040001AD RID: 429
			NoConnectionError,
			// Token: 0x040001AE RID: 430
			DisconnectedNormally,
			// Token: 0x040001AF RID: 431
			AuthorizationFailure,
			// Token: 0x040001B0 RID: 432
			LoginTimedOut,
			// Token: 0x040001B1 RID: 433
			UnknownLoginFailure,
			// Token: 0x040001B2 RID: 434
			UnexpectedDisconnection,
			// Token: 0x040001B3 RID: 435
			UserCanceledConnect,
			// Token: 0x040001B4 RID: 436
			DisconnectedNormallyLogoffByUser,
			// Token: 0x040001B5 RID: 437
			VirtualMachineShielded
		}

		// Token: 0x02000042 RID: 66
		private enum RdpViewerModeSwitch
		{
			// Token: 0x040001B7 RID: 439
			None,
			// Token: 0x040001B8 RID: 440
			SwitchToBasic,
			// Token: 0x040001B9 RID: 441
			SwitchToEnhanced
		}

		// Token: 0x02000043 RID: 67
		private class SnapshotTracker
		{
			// Token: 0x14000017 RID: 23
			// (add) Token: 0x060003CD RID: 973 RVA: 0x0001560C File Offset: 0x0001380C
			// (remove) Token: 0x060003CE RID: 974 RVA: 0x00015644 File Offset: 0x00013844
			public event EventHandler HasSnapshotsChanged;

			// Token: 0x17000183 RID: 387
			// (get) Token: 0x060003CF RID: 975 RVA: 0x0001567C File Offset: 0x0001387C
			public bool HasSnapshots
			{
				get
				{
					bool result;
					lock (this)
					{
						result = (this.m_VirtualMachine != null && this.m_Snapshots.Count > 0);
					}
					return result;
				}
			}

			// Token: 0x060003D0 RID: 976 RVA: 0x000156CC File Offset: 0x000138CC
			private void HandleSnapshotCreated(IVMComputerSystem snapshottedVM, IVMComputerSystemSetting createdSnapshot)
			{
				bool flag = false;
				lock (this)
				{
					if (snapshottedVM.InstanceId == this.m_VirtualMachine.InstanceId)
					{
						try
						{
							createdSnapshot.Deleted += this.HandleSnapshotDeleted;
							bool hasSnapshots = this.HasSnapshots;
							this.m_Snapshots.Add(createdSnapshot);
							flag = (hasSnapshots != this.HasSnapshots);
						}
						catch (ServerObjectDeletedException)
						{
						}
					}
				}
				if (flag && this.HasSnapshotsChanged != null)
				{
					this.HasSnapshotsChanged(this, EventArgs.Empty);
				}
			}

			// Token: 0x060003D1 RID: 977 RVA: 0x00015778 File Offset: 0x00013978
			private void HandleSnapshotDeleted(object sender, EventArgs ea)
			{
				bool flag = false;
				lock (this)
				{
					IVMComputerSystemSetting ivmcomputerSystemSetting = (IVMComputerSystemSetting)sender;
					int num = -1;
					int num2 = 0;
					while (num2 < this.m_Snapshots.Count && num == -1)
					{
						if (ivmcomputerSystemSetting.InstanceId == this.m_Snapshots[num2].InstanceId)
						{
							num = num2;
						}
						num2++;
					}
					if (num != -1)
					{
						bool hasSnapshots = this.HasSnapshots;
						this.m_Snapshots.RemoveAt(num);
						flag = (hasSnapshots != this.HasSnapshots);
					}
				}
				if (flag && this.HasSnapshotsChanged != null)
				{
					this.HasSnapshotsChanged(this, EventArgs.Empty);
				}
			}

			// Token: 0x060003D2 RID: 978 RVA: 0x0001583C File Offset: 0x00013A3C
			public void CleanupVirtualMachineSnapshots()
			{
				lock (this)
				{
					if (this.m_VirtualMachine != null)
					{
						List<IVMComputerSystemSetting> list = new List<IVMComputerSystemSetting>(this.m_Snapshots);
						Program.TasksToCompleteBeforeExitingTracker.RegisterBackgroundTask(new RdpViewerControl.SnapshotTracker.CleanupMethod(this.Cleanup), new object[]
						{
							this.m_VirtualMachine,
							list
						});
					}
					this.m_Snapshots.Clear();
				}
			}

			// Token: 0x060003D3 RID: 979 RVA: 0x000158BC File Offset: 0x00013ABC
			public void SetNewVirtualMachine(IVMComputerSystem virtualMachine)
			{
				bool hasSnapshots;
				lock (this)
				{
					hasSnapshots = this.HasSnapshots;
					if (this.m_VirtualMachine != null)
					{
						Program.TasksToCompleteBeforeExitingTracker.RegisterBackgroundTask(new RdpViewerControl.SnapshotTracker.CleanupMethod(this.Cleanup), new object[]
						{
							this.m_VirtualMachine,
							this.m_Snapshots
						});
					}
					this.m_VirtualMachine = ObjectLocator.GetVMComputerSystem(virtualMachine.Server, virtualMachine.InstanceId);
					this.m_Snapshots.Clear();
				}
				if (hasSnapshots && this.HasSnapshotsChanged != null)
				{
					this.HasSnapshotsChanged(this, EventArgs.Empty);
				}
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.GetSnapshots), virtualMachine);
			}

			// Token: 0x060003D4 RID: 980 RVA: 0x00015980 File Offset: 0x00013B80
			private void GetSnapshots(object virtualMachineObj)
			{
				bool flag = false;
				lock (this)
				{
					if (((IVMComputerSystem)virtualMachineObj).InstanceId == this.m_VirtualMachine.InstanceId)
					{
						try
						{
							this.m_VirtualMachine.SnapshotCreated += this.HandleSnapshotCreated;
							foreach (IVMComputerSystemSetting ivmcomputerSystemSetting in this.m_VirtualMachine.Snapshots)
							{
								flag = true;
								ivmcomputerSystemSetting.Deleted += this.HandleSnapshotDeleted;
								this.m_Snapshots.Add(ivmcomputerSystemSetting);
							}
						}
						catch (VirtualizationManagementException ex)
						{
							VMTrace.TraceWarning("Unable to get snapshots for vm.", ex);
						}
					}
				}
				if (flag && this.HasSnapshotsChanged != null)
				{
					this.HasSnapshotsChanged(this, EventArgs.Empty);
				}
			}

			// Token: 0x060003D5 RID: 981 RVA: 0x00015A84 File Offset: 0x00013C84
			private void Cleanup(IVMComputerSystem virtualMachine, List<IVMComputerSystemSetting> snapshots)
			{
				try
				{
					if (virtualMachine != null)
					{
						virtualMachine.SnapshotCreated -= this.HandleSnapshotCreated;
					}
					if (snapshots != null)
					{
						foreach (IVMComputerSystemSetting ivmcomputerSystemSetting in snapshots)
						{
							if (ivmcomputerSystemSetting != null)
							{
								ivmcomputerSystemSetting.Deleted -= this.HandleSnapshotDeleted;
							}
						}
					}
				}
				catch (Exception ex)
				{
					VMTrace.TraceWarning("Unable to unregister from the event handlers used for tracking the snapshots of a VM.", ex);
				}
			}

			// Token: 0x040001BA RID: 442
			private IVMComputerSystem m_VirtualMachine;

			// Token: 0x040001BB RID: 443
			private readonly List<IVMComputerSystemSetting> m_Snapshots = new List<IVMComputerSystemSetting>();

			// Token: 0x02000061 RID: 97
			// (Invoke) Token: 0x06000411 RID: 1041
			private delegate void CleanupMethod(IVMComputerSystem virtualMachine, List<IVMComputerSystemSetting> snapshots);
		}

		// Token: 0x02000044 RID: 68
		// (Invoke) Token: 0x060003D8 RID: 984
		private delegate void ConnectCallbackMethod(IVMComputerSystem virtualMachine, bool existingConnections);

		// Token: 0x02000045 RID: 69
		// (Invoke) Token: 0x060003DC RID: 988
		private delegate void DisplayDisconnectionMessageMethod(string errorMessage, bool disconnectedWhileConnected);
	}
}
