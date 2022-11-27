using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Microsoft.Virtualization.Client.Management;
using Microsoft.Virtualization.Client.Settings;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x0200000B RID: 11
	internal sealed partial class InteractiveSessionForm : Form, IMenuActionTarget, IVMConnectCommunicatorHelper, IDpiForm
	{
		// Token: 0x0600007A RID: 122 RVA: 0x00007008 File Offset: 0x00005208
		public InteractiveSessionForm(RdpConnectionInfo connectionInfo)
		{
			if (connectionInfo == null)
			{
				throw new ArgumentException("connectionInfo is null", "connectionInfo");
			}
			if (connectionInfo.VirtualMachine == null)
			{
				throw new ArgumentException("connectionInfo does not include a virtual machine");
			}
			this._oldDpi = this.DeviceDpi();
			this.m_TitleFormat = VMISResources.MainWindow_TitleBar;
			this.m_TitleNoVM = VMISResources.MainWindow_TitleNoVM;
			this.m_RdpViewer = new RdpViewerControl(this, connectionInfo);
			this.m_RdpMode = RdpConnectionMode.InTransition;
			base.SuspendLayout();
			new ComponentResourceManager(typeof(InteractiveSessionForm)).ApplyResources(this, "$this");
			base.Size = new Size(656, 508);
			base.KeyPreview = true;
			base.Icon = CommonResources.RdpIcon;
			this.m_ClearKeysNotCapturedTimer = new System.Windows.Forms.Timer();
			this.m_ClearKeysNotCapturedTimer.Interval = 30000;
			this.m_ClearKeysNotCapturedTimer.Tick += this.ClearKeysNotCaptured;
			this.m_MenuManager = new MenuManager(this);
			base.MainMenuStrip = this.m_MenuManager.MainMenu;
			base.Controls.Add(this.m_MenuManager.MainMenu);
			this.m_ToolBar = new VmisToolStrip(this, this);
			this.m_ToolBar.Name = "ToolBar";
			this.m_ToolBar.ShowItemToolTips = true;
			this.m_ToolBar.AutoSize = true;
			this.m_ToolBar.Dock = DockStyle.None;
			if (InteractiveSessionConfigurationOptions.Instance.ShowToolbar)
			{
				base.Controls.Add(this.m_ToolBar);
			}
			this.m_RdpViewer.Name = "RdpViewer";
			this.m_RdpViewer.Dock = DockStyle.None;
			this.m_RdpViewer.Visible = false;
			base.Controls.Add(this.m_RdpViewer);
			this.m_NoConnectionDialog = new NoConnectionDialog(this);
			this.m_NoConnectionDialog.SetStrings(this.m_RdpViewer.OfflineDisplayStrings);
			this.m_NoConnectionElementHost = new ElementHost();
			this.m_NoConnectionElementHost.Name = "NoConnectionElementHost";
			this.m_NoConnectionElementHost.Dock = DockStyle.None;
			this.m_NoConnectionElementHost.Child = this.m_NoConnectionDialog;
			this.m_NoConnectionElementHost.Size = this.LogicalToDeviceUnits(RdpViewerControl.BiosResolution);
			base.Controls.Add(this.m_NoConnectionElementHost);
			this.m_StatusBar = new VmisStatusStrip(this);
			this.m_StatusBar.Name = "StatusBar";
			this.m_StatusBar.AutoSize = true;
			this.m_StatusBar.SizingGrip = true;
			this.m_StatusBar.Dock = DockStyle.Bottom;
			base.Controls.Add(this.m_StatusBar);
			base.ResumeLayout();
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x0600007B RID: 123 RVA: 0x000072A8 File Offset: 0x000054A8
		// (remove) Token: 0x0600007C RID: 124 RVA: 0x000072E0 File Offset: 0x000054E0
		public new event System.Windows.Forms.DpiChangedEventHandler DpiChanged;

		// Token: 0x0600007D RID: 125 RVA: 0x00007315 File Offset: 0x00005515
		public void OnDpiChanged(System.Windows.Forms.DpiChangedEventArgs e)
		{
			DpiUtilities.SetWindowPosition(base.Handle, e.SuggestedRectangle);
			if (this.m_PreviousClientSize.IsEmpty)
			{
				this.SetFormSizeForDpiChange(e.DeviceDpiOld);
			}
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00007344 File Offset: 0x00005544
		private void SetFormSizeForDpiChange(int oldDpi)
		{
			base.SuspendLayout();
			this.m_NoConnectionDialog.UpdateDpi(base.Handle);
			this.m_ToolBar.ImageScalingSize = this.LogicalToDeviceUnits(new Size(16, 16));
			this.SetText();
			this.m_NoConnectionElementHost.Size = this.LogicalToDeviceUnits(RdpViewerControl.BiosResolution);
			if (this.m_RdpViewer.IsConnectedBasic && InteractiveSessionConfigurationOptions.Instance.ZoomLevel == 0U)
			{
				this.m_RdpViewer.SetZoomLevel(ZoomLevel.Auto);
			}
			else if (this.m_RdpViewer.ConnectionState == RdpViewerConnectionState.ConnectedEnhancedVideo)
			{
				this.SetFormSizes();
			}
			else if (this.m_RdpViewer.ConnectionState == RdpViewerConnectionState.ConnectedEnhancedVideoSyncedSession)
			{
				if (!this.m_PreviousClientSize.IsEmpty)
				{
					Size sz = this.DeviceToLogicalUnits(base.ClientSize);
					Size sz2 = this.DeviceToLogicalUnits(this.m_PreviousClientSize, oldDpi);
					Size sz3 = sz - sz2;
					if (Math.Abs(sz3.Width) > 10 || Math.Abs(sz3.Height) > 10)
					{
						this.m_TrueSyncedEnhancedModeDesktopResolution += sz3;
						this.SetFormSizes(this.LogicalToDeviceUnits(this.m_TrueSyncedEnhancedModeDesktopResolution), false);
						this.m_RdpViewer.Size = this.m_TrueSyncedEnhancedModeDesktopResolution;
					}
				}
				this.UpdateRdpClientDisplaySettings();
			}
			base.ResumeLayout(true);
			base.SuspendLayout();
			this.SetFormSizes();
			base.ResumeLayout();
			this.SetFormSizes();
		}

		// Token: 0x06000080 RID: 128 RVA: 0x000074C0 File Offset: 0x000056C0
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Point? startingPosition = InteractiveSessionConfigurationOptions.Instance.StartingPosition;
			if (startingPosition != null)
			{
				Point location = base.Location;
				base.StartPosition = FormStartPosition.Manual;
				base.Location = startingPosition.Value;
				if (this.m_RdpViewer.LaunchIndex > 0)
				{
					int num = 20 * this.m_RdpViewer.LaunchIndex;
					Point point = new Point(base.Left + num, base.Top + num);
					if (Screen.FromPoint(point).WorkingArea.Contains(new Point(point.X + 40, point.Y + 40)))
					{
						base.Location = point;
					}
					else
					{
						base.Location = location;
					}
				}
			}
			this.m_RdpViewer.VMStateChanged += this.HandleVMStateChanged;
			this.m_RdpViewer.RequestEnterFullScreen += this.HandleEnterFullScreen;
			this.m_RdpViewer.RequestExitFullScreen += this.HandleExitFullScreen;
			this.m_RdpViewer.RequestFullScreenClose += delegate(object sender, EventArgs ea)
			{
				base.Close();
			};
			this.m_RdpViewer.RequestFullScreenMinimize += this.HandleFullScreenMinimize;
			this.m_RdpViewer.ConnectionStateChanged += this.HandleViewerConnectedStateChanged;
			this.m_RdpViewer.MouseCursorCapturedChanged += this.HandleMouseCursorCapturedChanged;
			this.m_RdpViewer.VMNameChanged += delegate(object sender, EventArgs ea)
			{
				this.SetText();
			};
			this.m_RdpViewer.HasSnapshotsChanged += delegate(object sender, EventArgs ea)
			{
				this.m_MenuManager.UpdateEnabledState();
				this.m_ToolBar.UpdateEnabledState();
				this.m_NoConnectionDialog.UpdateEnabledState();
			};
			this.m_RdpViewer.KeyboardInputCapturedChanged += this.HandleKeyboardInputCapturedChanged;
			this.m_RdpViewer.VMConfigurationChanged += delegate(object sender, EventArgs ea)
			{
				this.m_MenuManager.InformVMConfigurationChange();
			};
			this.m_RdpViewer.OfflineDisplayStringsUpdated += delegate(object sender, EventArgs args)
			{
				this.m_NoConnectionDialog.SetStrings(this.m_RdpViewer.OfflineDisplayStrings);
			};
			this.m_RdpViewer.GuestResolutionChanged += delegate(object sender, EventArgs args)
			{
				this.SetFormSizes();
			};
			this.m_MenuManager.UpdateEnabledState();
			this.m_ToolBar.UpdateEnabledState();
			this.m_NoConnectionDialog.UpdateEnabledState();
			this.SetFormSizes();
		}

		// Token: 0x06000081 RID: 129 RVA: 0x000076CC File Offset: 0x000058CC
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			this.SetText();
			this.SetFormSizes();
			base.Update();
			this.m_RdpViewer.SetupVirtualMachine();
			bool enhancedMode = this.m_RdpViewer.PrepareForEnhancedMode(false);
			this.m_RdpViewer.Connect(enhancedMode, true);
			Program.Displayer.ParentWindowHandle = base.Handle;
			Program.UnhandledExceptionHandler.DisplayDialogParentWindowHandle = base.Handle;
			VMConnectCommunicator.Register(this);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00007740 File Offset: 0x00005940
		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			if (base.WindowState == FormWindowState.Normal && !this.m_RdpViewer.FullScreen)
			{
				InteractiveSessionConfigurationOptions.Instance.StartingPosition = new Point?(base.Location);
			}
			this.m_RdpViewer.CleanupVirtualMachine();
			this.m_MenuManager.CloseMenu();
			base.OnFormClosed(e);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00007794 File Offset: 0x00005994
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (this.GetVMSettingsDialog(this.m_RdpViewer.VirtualMachine) != null)
			{
				string summary = string.Format(CultureInfo.CurrentCulture, VMISResources.Error_MustCloseSettingsDialog, VMISResources.MainWindow_TitleNoVM);
				Program.Displayer.DisplayError(string.Empty, summary);
				e.Cancel = true;
				return;
			}
			VMConnectCommunicator.Unregister();
			this._closing = true;
			base.OnFormClosing(e);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x000077F4 File Offset: 0x000059F4
		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Tab)
			{
				if (this.m_RdpViewer.IsConnected)
				{
					this.m_RdpViewer.CaptureKeyboardFocus();
				}
			}
			else
			{
				this.m_MenuManager.InformKeyUp(e.KeyData);
			}
			base.OnKeyUp(e);
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00007834 File Offset: 0x00005A34
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (this.m_RdpViewer.IsConnected && !e.Control && !e.Alt && e.KeyCode != Keys.None && e.KeyCode != Keys.Tab && e.KeyCode != Keys.F1 && this.m_StatusBar.DisplayedInformation != VMISResources.KeyInputNotCaptured)
			{
				int scaledSize = this.LogicalToDeviceUnits(16);
				Image imageFromIcon = CommonResources.ResourceManager.GetImageFromIcon("WarningIcon", scaledSize);
				this.m_StatusBar.DisplayInformation(VMISResources.KeyInputNotCaptured, imageFromIcon);
				this.m_ClearKeysNotCapturedTimer.Start();
			}
			base.OnKeyDown(e);
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000078D0 File Offset: 0x00005AD0
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override void WndProc(ref Message m)
		{
			FormWindowState windowState = base.WindowState;
			int msg = m.Msg;
			if (msg <= 274)
			{
				if (msg != 126)
				{
					if (msg != 274)
					{
						goto IL_1C4;
					}
					int num = (int)m.WParam & 65520;
					if (num == 61472)
					{
						goto IL_1C4;
					}
					if (num != 61488)
					{
						if (num != 61728)
						{
							goto IL_1C4;
						}
						if (!this._onRestoreEnterFullScreen)
						{
							goto IL_1C4;
						}
						this._onRestoreEnterFullScreen = false;
						try
						{
							this._isRestoring = true;
							if (this.m_RdpViewer.IsConnected)
							{
								this.HandleEnterFullScreen(this, EventArgs.Empty);
							}
							goto IL_1C4;
						}
						finally
						{
							this._isRestoring = false;
						}
					}
					else
					{
						if (this.m_RdpViewer.IsConnected && !this.m_RdpViewer.FullScreen)
						{
							this.HandleEnterFullScreen(this, EventArgs.Empty);
							m.Result = IntPtr.Zero;
							return;
						}
						if (!this.m_RdpViewer.IsConnected)
						{
							m.Result = IntPtr.Zero;
							return;
						}
						goto IL_1C4;
					}
				}
				if (this.m_RdpViewer.FullScreen && Screen.FromHandle(base.Handle).Bounds.Size != this.m_RdpViewer.Size)
				{
					DelayedUIInvoker delayedUIInvoker = new DelayedUIInvoker();
					delayedUIInvoker.Invoked += delegate (object o, EventArgs e)
					{
						this.HandleExitFullScreen(this, EventArgs.Empty);
					};
					delayedUIInvoker.Invoke();
				}
			}
			else if (msg != 537)
			{
				if (msg == 736)
				{
					Rectangle suggestedRectangle = DpiUtilities.GetSuggestedRectangle(m.LParam);
					int num2 = this.NewDpi(ref m);
					System.Windows.Forms.DpiChangedEventArgs e = new System.Windows.Forms.DpiChangedEventArgs(this._oldDpi, num2, suggestedRectangle);
					System.Windows.Forms.DpiChangedEventHandler dpiChanged = this.DpiChanged;
					if (dpiChanged != null)
					{
						dpiChanged(this, e);
					}
					this.OnDpiChanged(e);
					this._oldDpi = num2;
				}
			}
			else
			{
				this.m_RdpViewer.OnNotifyRedirectDeviceChange(m.WParam, m.LParam);
			}
			IL_1C4:
			base.WndProc(ref m);
			if (m.Msg == 134)
			{
				if (m.WParam != IntPtr.Zero)
				{
					this.m_RdpViewer.CaptureKeyboardFocus();
				}
				else
				{
					this.m_RdpViewer.Deactivate();
				}
			}
			if (windowState != base.WindowState)
			{
				if (base.WindowState != FormWindowState.Minimized)
				{
					this.SetFormSizes();
					return;
				}
				this.m_RestoreState = windowState;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000087 RID: 135 RVA: 0x00007B10 File Offset: 0x00005D10
		internal MenuManager MenuManager
		{
			get
			{
				return this.m_MenuManager;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000088 RID: 136 RVA: 0x00007B18 File Offset: 0x00005D18
		internal VmisStatusStrip StatusBar
		{
			get
			{
				return this.m_StatusBar;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000089 RID: 137 RVA: 0x00007B20 File Offset: 0x00005D20
		public ThreadMethodInvoker AsyncUIThreadMethodInvoker
		{
			get
			{
				return new ThreadMethodInvoker(this.BeginInvokeIgnorningReturnValue);
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00007B30 File Offset: 0x00005D30
		private void HandleVMStateChanged(object sender, EventArgs ea)
		{
			this.UpdateRdpMode();
			this.SetText();
			this.m_NoConnectionDialog.SetStrings(this.m_RdpViewer.OfflineDisplayStrings);
			this.MenuManager.UpdateEnabledState();
			this.m_ToolBar.UpdateEnabledState();
			this.m_NoConnectionDialog.UpdateEnabledState();
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00007B80 File Offset: 0x00005D80
		private void HandleEnterFullScreen(object sender, EventArgs ea)
		{
			if (!this.m_RdpViewer.FullScreen)
			{
				try
				{
					this._enteringFullScreen = true;
					if (!this._isRestoring)
					{
						this.m_PreFullScreenBounds = base.Bounds;
						this.m_PreFullScreenRdpSize = this.m_RdpViewer.Size;
					}
					base.WindowState = FormWindowState.Normal;
					base.Controls.Remove(this.m_MenuManager.MainMenu);
					if (base.Controls.Contains(this.m_ToolBar))
					{
						base.Controls.Remove(this.m_ToolBar);
					}
					base.Controls.Remove(this.m_StatusBar);
					base.FormBorderStyle = FormBorderStyle.None;
					this.MaximumSize = Size.Empty;
					if (this.m_RdpViewer.ConnectingInMultiMon())
					{
						base.Bounds = new Rectangle(SystemInformation.VirtualScreen.Left, SystemInformation.VirtualScreen.Top, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
					}
					else
					{
						base.Bounds = Screen.FromHandle(base.Handle).Bounds;
					}
					this.m_RdpViewer.Location = new Point(0, 0);
					this.m_RdpViewer.Size = base.Size;
					this.m_RdpViewer.EnterFullScreen();
				}
				finally
				{
					this._enteringFullScreen = false;
				}
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00007CE8 File Offset: 0x00005EE8
		private void HandleExitFullScreen(object sender, EventArgs ea)
		{
			if (this.m_RdpViewer.FullScreen)
			{
				this.m_RdpViewer.ExitFullScreen();
				base.SuspendLayout();
				this.m_RdpViewer.Size = this.m_PreFullScreenRdpSize;
				base.Bounds = this.m_PreFullScreenBounds;
				base.Controls.Add(this.m_MenuManager.MainMenu);
				if (InteractiveSessionConfigurationOptions.Instance.ShowToolbar)
				{
					base.Controls.Add(this.m_ToolBar);
				}
				base.Controls.Add(this.m_StatusBar);
				if (this.m_RdpViewer.ConnectionState == RdpViewerConnectionState.ConnectedEnhancedVideoSyncedSession)
				{
					this.UpdateRdpClientDisplaySettings();
				}
				this.SetFormSizes();
				base.ResumeLayout(true);
			}
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00007D99 File Offset: 0x00005F99
		private void HandleFullScreenMinimize(object sender, EventArgs e)
		{
			if (this.m_RdpViewer.IsConnected && this.m_RdpViewer.FullScreen)
			{
				this._onRestoreEnterFullScreen = true;
				this.HandleExitFullScreen(this, EventArgs.Empty);
				base.WindowState = FormWindowState.Minimized;
			}
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00007DD0 File Offset: 0x00005FD0
		private void SetFormSizes()
		{
			if (this._closing || this._enteringFullScreen || this.m_RdpViewer.FullScreen)
			{
				return;
			}
			if (this.m_RdpViewer.IsConnected)
			{
				if (this.m_RdpViewer.IsConnectedBasic)
				{
					this.SetFormSizes(this.m_RdpViewer.ScaledDesktopResolution, true);
					this.m_RdpViewer.Size = this.m_RdpViewer.ScaledDesktopResolution;
					return;
				}
				if (this.m_RdpViewer.ConnectionState == RdpViewerConnectionState.ConnectedEnhancedVideo)
				{
					this.SetFormSizes(this.m_RdpViewer.DesktopResolution, true);
					this.m_RdpViewer.Size = this.m_RdpViewer.DesktopResolution;
					return;
				}
				this.SetFormSizes(this.m_RdpViewer.DesktopResolution, false);
				return;
			}
			else
			{
				if (this.m_RdpViewer.ConnectionState == RdpViewerConnectionState.Connecting)
				{
					this.SetFormSizes(this.LogicalToDeviceUnits(RdpViewerControl.BiosResolution), true);
					this.m_RdpViewer.Size = this.m_RdpViewer.DesktopResolution;
					return;
				}
				this.SetFormSizes(this.LogicalToDeviceUnits(RdpViewerControl.BiosResolution), true);
				return;
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00007ED4 File Offset: 0x000060D4
		private void SetFormSizes(Size viewerSize, bool sizeFixed)
		{
			bool flag = base.Controls.Contains(this.m_ToolBar);
			Size sz = new Size(0, base.MainMenuStrip.Height) + (flag ? new Size(0, this.m_ToolBar.Height) : Size.Empty) + viewerSize + new Size(0, this.m_StatusBar.Height) + this.GetWindowChromeDpiSizeAdjustment();
			Size sz2 = new Size(1, 0);
			base.ClientSize = sz - sz2;
			if (flag)
			{
				this.m_ToolBar.Location = new Point(0, base.MainMenuStrip.Height);
			}
			Point location = flag ? new Point(0, this.m_ToolBar.Location.Y + this.m_ToolBar.Height) : new Point(0, base.MainMenuStrip.Height);
			this.m_RdpViewer.Location = location;
			this.m_NoConnectionElementHost.Location = location;
			base.FormBorderStyle = (sizeFixed ? FormBorderStyle.Fixed3D : FormBorderStyle.Sizable);
			this.m_StatusBar.SizingGrip = !sizeFixed;
			base.Size += sz2;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00008004 File Offset: 0x00006204
		protected override void OnResizeBegin(EventArgs e)
		{
			base.OnResizeBegin(e);
			this.m_PreviousClientSize = base.ClientSize;
			this.m_PreviousClientSizeDpi = DpiUtilities.DeviceDpi(base.Handle);
		}

		// Token: 0x06000091 RID: 145 RVA: 0x0000802C File Offset: 0x0000622C
		protected override void OnResizeEnd(EventArgs e)
		{
			base.OnResizeEnd(e);
			if (this.m_PreviousClientSizeDpi != this.DeviceDpi())
			{
				this.SetFormSizeForDpiChange(this.m_PreviousClientSizeDpi);
				return;
			}
			if (this.m_RdpViewer.ConnectionState == RdpViewerConnectionState.ConnectedEnhancedVideoSyncedSession && !this.m_PreviousClientSize.IsEmpty && base.ClientSize != this.m_PreviousClientSize)
			{
				Size sz = this.DeviceToLogicalUnits(base.ClientSize - this.m_PreviousClientSize);
				this.m_TrueSyncedEnhancedModeDesktopResolution += sz;
				this.SetFormSizes(this.LogicalToDeviceUnits(this.m_TrueSyncedEnhancedModeDesktopResolution), false);
				this.m_RdpViewer.Size = this.m_TrueSyncedEnhancedModeDesktopResolution;
				this.UpdateRdpClientDisplaySettings();
			}
			this.m_PreviousClientSize = Size.Empty;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000080E9 File Offset: 0x000062E9
		private void CancelResizeAsync()
		{
			this.m_PreviousClientSize = Size.Empty;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000080F6 File Offset: 0x000062F6
		private void InitializeTrueSyncedEnhancedModeDesktopResolution()
		{
			this.m_TrueSyncedEnhancedModeDesktopResolution = this.m_RdpViewer.DesktopResolution;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x0000810C File Offset: 0x0000630C
		private bool UpdateRdpClientDisplaySettings()
		{
			Size size = this.m_RdpViewer.Size;
			Size size2 = this.LogicalToDeviceUnits(this.m_TrueSyncedEnhancedModeDesktopResolution);
			if (this.m_RdpViewer.UpdateDisplaySettings(size2, false))
			{
				this.m_RdpViewer.Size = size2;
				return true;
			}
			this.m_RdpViewer.Size = size;
			this.SetFormSizes();
			return false;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00008164 File Offset: 0x00006364
		private void HandleViewerConnectedStateChanged(object sender, EventArgs ea)
		{
			this.UpdateRdpMode();
			this.m_MenuManager.UpdateEnabledState();
			this.m_ToolBar.UpdateEnabledState();
			this.m_NoConnectionDialog.UpdateEnabledState();
			this.m_StatusBar.InformConnectedState(this.m_RdpViewer.IsConnected && this.m_RdpViewer.ConnectionState != RdpViewerConnectionState.ConnectedNoVideo, this.m_RdpViewer.AuthenticationType);
			if (this.m_RdpViewer.IsConnected)
			{
				base.Controls.Remove(this.m_NoConnectionElementHost);
				this.m_RdpViewer.Visible = true;
			}
			else
			{
				this.m_RdpViewer.Visible = false;
				base.Controls.Add(this.m_NoConnectionElementHost);
			}
			((IMenuActionTarget)this).SetZoomLevel((ZoomLevel)InteractiveSessionConfigurationOptions.Instance.ZoomLevel);
			this.SetFormSizes();
			base.Update();
			if (this.m_RdpViewer.ConnectionState == RdpViewerConnectionState.ConnectedEnhancedVideoSyncedSession)
			{
				this.InitializeTrueSyncedEnhancedModeDesktopResolution();
				if (this.m_RdpViewer.FullScreen)
				{
					this.m_RdpViewer.SyncDisplaySettings();
					return;
				}
				new RetryUIInvoker(() => this.UpdateRdpClientDisplaySettings(), TimeSpan.FromMilliseconds(500.0), TimeSpan.FromSeconds(10.0)).Invoke();
			}
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00008294 File Offset: 0x00006494
		private void HandleMouseCursorCapturedChanged(object sender, EventArgs ea)
		{
			bool flag = true;
			if (this.m_RdpViewer.MouseCursorCaptured)
			{
				if (!SystemInformation.TerminalServerSession)
				{
					string mouseReleaseString = this.GetMouseReleaseString();
					int scaledSize = this.LogicalToDeviceUnits(16);
					Image imageFromIcon = CommonResources.ResourceManager.GetImageFromIcon("WarningIcon", scaledSize);
					this.m_StatusBar.DisplayInformation(mouseReleaseString, imageFromIcon);
				}
				else
				{
					int scaledSize2 = this.LogicalToDeviceUnits(16);
					Image imageFromIcon2 = CommonResources.ResourceManager.GetImageFromIcon("InformationIcon", scaledSize2);
					this.m_StatusBar.DisplayInformation(VMISResources.StatusBar_MouseNotCapturedInTerminalServicesSession, imageFromIcon2);
					flag = false;
					DelayedUIInvoker delayedUIInvoker = new DelayedUIInvoker();
					delayedUIInvoker.Invoked += delegate(object o, EventArgs e)
					{
						this.m_RdpViewer.Deactivate();
						this.m_RdpViewer.CaptureKeyboardFocus();
						this.ExplainMouseNotCapturedInTerminalServicesSession();
					};
					delayedUIInvoker.Invoke();
					DelayedUIInvoker delayedUIInvoker2 = new DelayedUIInvoker();
					delayedUIInvoker2.DelayTime = TimeSpan.FromSeconds(10.0);
					delayedUIInvoker2.Invoked += delegate(object o, EventArgs e)
					{
						if (this.m_StatusBar.DisplayedInformation == VMISResources.StatusBar_MouseNotCapturedInTerminalServicesSession)
						{
							this.SetStatusDescription();
						}
					};
					delayedUIInvoker2.Invoke();
				}
			}
			else
			{
				string mouseReleaseString2 = this.GetMouseReleaseString();
				if (this.m_StatusBar.DisplayedInformation == mouseReleaseString2)
				{
					this.SetStatusDescription();
				}
			}
			if (flag)
			{
				this.m_StatusBar.InformMouseCapturedState(this.m_RdpViewer.MouseCursorCaptured);
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000083A8 File Offset: 0x000065A8
		private void HandleKeyboardInputCapturedChanged(object sender, EventArgs ea)
		{
			this.m_StatusBar.InformKeyboardInputCapturedState(this.m_RdpViewer.KeyboardInputCaptured);
			if (this.m_RdpViewer.KeyboardInputCaptured && this.m_StatusBar.DisplayedInformation == VMISResources.KeyInputNotCaptured)
			{
				this.m_ClearKeysNotCapturedTimer.Stop();
				this.SetStatusDescription();
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00008400 File Offset: 0x00006600
		private string GetMouseReleaseString()
		{
			VMConnectReleaseKey vmconnectReleaseKey = ClientVirtualizationSettings.Instance.VMConnectReleaseKey;
			vmconnectReleaseKey = VMConnectReleaseKeyHelper.ValidateValue(vmconnectReleaseKey);
			VMConnectReleaseKeyConverter vmconnectReleaseKeyConverter = new VMConnectReleaseKeyConverter();
			return string.Format(CultureInfo.CurrentCulture, VMISResources.StatusBar_MouseCaptured, vmconnectReleaseKeyConverter.ConvertToString(null, CultureInfo.CurrentUICulture, vmconnectReleaseKey));
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00008448 File Offset: 0x00006648
		private void ExplainMouseNotCapturedInTerminalServicesSession()
		{
			if (CommonConfiguration.Instance.NeedToConfirm(Confirmations.ExplainMouseInTerminalServicesSession))
			{
				string mouseInTerminalServicesSessionMainInstruction = VMISResources.MouseInTerminalServicesSessionMainInstruction;
				bool flag;
				Program.Displayer.DisplayInformation(mouseInTerminalServicesSessionMainInstruction, string.Empty, out flag);
				if (flag)
				{
					CommonConfiguration.Instance.TurnoffNeedToConfirmFlag(Confirmations.ExplainMouseInTerminalServicesSession);
					ThreadPool.QueueUserWorkItem(new WaitCallback(CommonUtilities.SaveConfiguration), CommonConfiguration.Instance);
				}
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600009A RID: 154 RVA: 0x000084A7 File Offset: 0x000066A7
		IVMComputerSystem IMenuActionTarget.VirtualMachine
		{
			get
			{
				return this.m_RdpViewer.VirtualMachine;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600009B RID: 155 RVA: 0x000084B4 File Offset: 0x000066B4
		IWin32Window IMenuActionTarget.DialogOwner
		{
			get
			{
				return this;
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x000084B7 File Offset: 0x000066B7
		void IMenuActionTarget.GoFullScreen()
		{
			this.HandleEnterFullScreen(this, EventArgs.Empty);
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000084C5 File Offset: 0x000066C5
		void IMenuActionTarget.Exit()
		{
			base.Close();
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600009E RID: 158 RVA: 0x000084CD File Offset: 0x000066CD
		bool IMenuActionTarget.IsRdpConnected
		{
			get
			{
				return this.m_RdpViewer.IsConnected;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600009F RID: 159 RVA: 0x000084DA File Offset: 0x000066DA
		bool IMenuActionTarget.IsRdpConnecting
		{
			get
			{
				return this.m_RdpViewer.ConnectionState == RdpViewerConnectionState.Connecting;
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x000084EA File Offset: 0x000066EA
		bool IMenuActionTarget.IsShieldedVm()
		{
			return this.m_RdpViewer.IsShieldedVm();
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x000084F7 File Offset: 0x000066F7
		RdpConnectionMode IMenuActionTarget.RdpMode
		{
			get
			{
				return this.m_RdpMode;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x000084FF File Offset: 0x000066FF
		ZoomLevel IMenuActionTarget.CurrentZoomLevel
		{
			get
			{
				return this.m_RdpViewer.CurrentZoomLevel;
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x0000850C File Offset: 0x0000670C
		void IMenuActionTarget.ToggleEnhanced()
		{
			if (this.m_RdpMode == RdpConnectionMode.InTransition)
			{
				return;
			}
			bool flag = true;
			if (!this.m_RdpViewer.IsConnectedEnhanced)
			{
				flag = this.m_RdpViewer.PrepareForEnhancedMode(true);
			}
			if (flag)
			{
				this.m_RdpMode = RdpConnectionMode.InTransition;
				this.m_MenuManager.UpdateEnabledState();
				this.m_ToolBar.UpdateEnabledState();
				this.m_NoConnectionDialog.UpdateEnabledState();
				bool flag2 = !this.m_RdpViewer.IsConnectedEnhanced;
				this.m_RdpViewer.PreferEnhancedModeConnection = flag2;
				this.m_RdpViewer.Connect(flag2, true);
			}
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00008591 File Offset: 0x00006791
		void IMenuActionTarget.SetZoomLevel(ZoomLevel level)
		{
			InteractiveSessionConfigurationOptions.Instance.ZoomLevel = (uint)level;
			this.m_RdpViewer.SetZoomLevel(level);
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x000085AA File Offset: 0x000067AA
		Image IMenuActionTarget.CopyScreenImage()
		{
			return this.m_RdpViewer.CopyScreenImage();
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x000085B7 File Offset: 0x000067B7
		VmisStatusStrip IMenuActionTarget.StatusBar
		{
			get
			{
				return this.m_StatusBar;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x000085BF File Offset: 0x000067BF
		bool IMenuActionTarget.HasSnapshots
		{
			get
			{
				return this.m_RdpViewer.HasSnapshots;
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x000085CC File Offset: 0x000067CC
		VMSettingsDialog IMenuActionTarget.OpenVMSettingsDialog()
		{
			IVMComputerSystem virtualMachine = this.m_RdpViewer.VirtualMachine;
			if (virtualMachine == null)
			{
				throw new InvalidOperationException();
			}
			VMSettingsDialog vmsettingsDialog = this.GetVMSettingsDialog(virtualMachine);
			if (vmsettingsDialog != null)
			{
				vmsettingsDialog.ActivateSelf();
			}
			else
			{
				virtualMachine.UpdateAssociationCache();
				string instanceId = virtualMachine.Setting.InstanceId;
				VMComputerSystemState state = virtualMachine.State;
				VMComputerSystemOperationalStatus[] operationalStatus = virtualMachine.GetOperationalStatus();
				vmsettingsDialog = new VMSettingsDialog(virtualMachine.Server, instanceId, false, true, state, operationalStatus, false);
				vmsettingsDialog.Tag = virtualMachine.InstanceId;
				vmsettingsDialog.StartPosition = FormStartPosition.Manual;
				Rectangle bounds = Screen.FromControl(this).Bounds;
				int num = (bounds.Size.Width - vmsettingsDialog.Size.Width) / 2;
				int num2 = (bounds.Size.Height - vmsettingsDialog.Size.Height) / 2;
				vmsettingsDialog.Location = new Point(bounds.Left + num, bounds.Top + num2);
				this.m_VMSettingsDialogs.Add(vmsettingsDialog);
				vmsettingsDialog.FormClosed += delegate(object sender, FormClosedEventArgs ea)
				{
					VMSettingsDialog item = (VMSettingsDialog)sender;
					this.m_VMSettingsDialogs.Remove(item);
				};
				vmsettingsDialog.Show(this);
			}
			return vmsettingsDialog;
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x000086E5 File Offset: 0x000068E5
		bool IMenuActionTarget.IsRdpEnabled
		{
			get
			{
				return this.m_RdpViewer.IsConnected && this.m_RdpViewer.ConnectionState != RdpViewerConnectionState.ConnectedNoVideo;
			}
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00008707 File Offset: 0x00006907
		void IMenuActionTarget.DeactivateOnDialogClosed()
		{
			if (this.m_RdpViewer.IsConnected)
			{
				this.m_RdpViewer.Deactivate();
				this.m_RdpViewer.CaptureKeyboardFocus();
			}
		}

		// Token: 0x060000AB RID: 171 RVA: 0x0000872C File Offset: 0x0000692C
		void IMenuActionTarget.InformBeginStateChangeOperation(VMStateChangeAction stateChange)
		{
			this.m_RdpViewer.InformBeginStateChangeOperation(stateChange);
			this.m_MenuManager.InformBeginStateChangeOperation(stateChange);
			this.m_ToolBar.InformBeginStateChangeOperation(stateChange);
			this.m_NoConnectionDialog.InformBeginStateChangeOperation(stateChange);
		}

		// Token: 0x060000AC RID: 172 RVA: 0x0000875E File Offset: 0x0000695E
		void IMenuActionTarget.InformEndStateChangeOperation(VMStateChangeAction stateChange)
		{
			this.m_RdpViewer.InformEndStateChangeOperation(stateChange);
			this.m_MenuManager.InformEndStateChangeOperation(stateChange);
			this.m_ToolBar.InformEndStateChangeOperation(stateChange);
			this.m_NoConnectionDialog.InformEndStateChangeOperation(stateChange);
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00008790 File Offset: 0x00006990
		void IMenuActionTarget.ShowToolbar(bool show)
		{
			InteractiveSessionConfigurationOptions.Instance.ShowToolbar = show;
			if (show)
			{
				base.Controls.Add(this.m_ToolBar);
			}
			else
			{
				base.Controls.Remove(this.m_ToolBar);
			}
			this.SetFormSizes();
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000AE RID: 174 RVA: 0x000087CA File Offset: 0x000069CA
		string IVMConnectCommunicatorHelper.CurrentVMServerName
		{
			get
			{
				IVMComputerSystem virtualMachine = this.m_RdpViewer.VirtualMachine;
				if (virtualMachine == null)
				{
					return null;
				}
				return virtualMachine.Server.FullName;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000AF RID: 175 RVA: 0x000087E7 File Offset: 0x000069E7
		string IVMConnectCommunicatorHelper.CurrentVMInstanceId
		{
			get
			{
				IVMComputerSystem virtualMachine = this.m_RdpViewer.VirtualMachine;
				if (virtualMachine == null)
				{
					return null;
				}
				return virtualMachine.InstanceId;
			}
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x000087FF File Offset: 0x000069FF
		void IVMConnectCommunicatorHelper.ActivateSelf()
		{
			if (base.InvokeRequired)
			{
				base.BeginInvoke(new MethodInvoker(this.Activate));
				return;
			}
			if (base.WindowState == FormWindowState.Minimized)
			{
				base.WindowState = this.m_RestoreState;
			}
			base.Activate();
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00008839 File Offset: 0x00006A39
		private void ClearKeysNotCaptured(object sender, EventArgs ea)
		{
			this.m_ClearKeysNotCapturedTimer.Stop();
			if (this.m_StatusBar.DisplayedInformation == VMISResources.KeyInputNotCaptured)
			{
				this.SetStatusDescription();
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00008864 File Offset: 0x00006A64
		private void SetText()
		{
			if (this.m_RdpViewer.VirtualMachine != null)
			{
				this.Text = string.Format(CultureInfo.CurrentCulture, this.m_TitleFormat, this.m_RdpViewer.VirtualMachine.Name, this.m_RdpViewer.VirtualMachine.Server);
				this.m_StatusBar.InformVMState(this.m_RdpViewer.VirtualMachine.State, this.m_RdpViewer.IsVMMigrating);
				this.SetStatusDescription();
				return;
			}
			this.Text = this.m_TitleNoVM;
			this.m_StatusBar.InformNoVM();
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x000088F8 File Offset: 0x00006AF8
		private void SetStatusDescription()
		{
			if (this.m_RdpViewer.VirtualMachine == null)
			{
				this.m_StatusBar.ClearInformationDisplay();
				return;
			}
			string failureStatusDescription = VMComputerSystemStateUtilities.GetFailureStatusDescription(this.m_RdpViewer.VirtualMachine.HealthState, this.m_RdpViewer.VirtualMachine.GetOperationalStatus(), this.m_RdpViewer.VirtualMachine.GetStatusDescriptions());
			if (!string.IsNullOrEmpty(failureStatusDescription))
			{
				int scaledSize = this.LogicalToDeviceUnits(16);
				Image imageFromIcon = CommonResources.ResourceManager.GetImageFromIcon("WarningIcon", scaledSize);
				this.m_StatusBar.DisplayInformation(failureStatusDescription, imageFromIcon);
				return;
			}
			this.m_StatusBar.ClearInformationDisplay();
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00008990 File Offset: 0x00006B90
		private VMSettingsDialog GetVMSettingsDialog(IVMComputerSystem virtualMachine)
		{
			if (virtualMachine == null)
			{
				return null;
			}
			VMSettingsDialog result = null;
			foreach (VMSettingsDialog vmsettingsDialog in this.m_VMSettingsDialogs)
			{
				if (object.Equals(virtualMachine.InstanceId, vmsettingsDialog.Tag))
				{
					result = vmsettingsDialog;
					break;
				}
			}
			return result;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x000089FC File Offset: 0x00006BFC
		private void BeginInvokeIgnorningReturnValue(Delegate method, params object[] args)
		{
			base.BeginInvoke(method, args);
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00008A08 File Offset: 0x00006C08
		private void UpdateRdpMode()
		{
			if (!this.m_RdpViewer.IsConnected)
			{
				if (this.m_RdpViewer.IsShieldedVm())
				{
					this.m_RdpViewer.VirtualMachine.UpdatePropertyCache();
					if (this.m_RdpViewer.VirtualMachine.EnhancedSessionModeState == EnhancedSessionModeStateType.Available)
					{
						this.m_RdpMode = RdpConnectionMode.EnhancedAvailable;
						return;
					}
				}
				this.m_RdpMode = RdpConnectionMode.Basic;
				return;
			}
			if (!this.m_RdpViewer.RdpEnhancedModeAvailable)
			{
				this.m_RdpMode = RdpConnectionMode.Basic;
				return;
			}
			if (this.m_RdpViewer.IsConnectedEnhanced)
			{
				this.m_RdpMode = RdpConnectionMode.Enhanced;
				return;
			}
			this.m_RdpMode = RdpConnectionMode.EnhancedAvailable;
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00008A94 File Offset: 0x00006C94
		private Size GetWindowChromeDpiSizeAdjustment()
		{
			int num = DpiUtilities.DeviceDpi(base.Handle);
			int systemDpi = DpiUtilities.SystemDpi;
			if (num == systemDpi)
			{
				return Size.Empty;
			}
			int num2 = (num > systemDpi) ? num : systemDpi;
			int num3 = (num < systemDpi) ? num : systemDpi;
			Size empty = Size.Empty;
			if (num2 == 120)
			{
				if (num3 == 96)
				{
					empty = new Size(2, 8);
				}
			}
			else if (num2 == 144)
			{
				if (num3 == 96)
				{
					empty = new Size(6, 17);
				}
				else if (num3 == 120)
				{
					empty = new Size(4, 9);
				}
			}
			else if (num2 == 168)
			{
				if (num3 == 96)
				{
					empty = new Size(8, 25);
				}
				else if (num3 == 120)
				{
					empty = new Size(6, 17);
				}
				else if (num3 == 144)
				{
					empty = new Size(2, 8);
				}
			}
			else if (num2 == 192)
			{
				if (num3 == 96)
				{
					empty = new Size(10, 32);
				}
				else if (num3 == 120)
				{
					empty = new Size(8, 24);
				}
				else if (num3 == 144)
				{
					empty = new Size(4, 15);
				}
				else if (num3 == 168)
				{
					empty = new Size(2, 7);
				}
			}
			else if (num2 == 216)
			{
				if (num3 == 96)
				{
					empty = new Size(12, 40);
				}
				else if (num3 == 120)
				{
					empty = new Size(10, 32);
				}
				else if (num3 == 144)
				{
					empty = new Size(6, 23);
				}
				else if (num3 == 168)
				{
					empty = new Size(4, 15);
				}
				else if (num3 == 192)
				{
					empty = new Size(2, 8);
				}
			}
			else if (num2 == 240)
			{
				if (num3 == 96)
				{
					empty = new Size(16, 49);
				}
				else if (num3 == 120)
				{
					empty = new Size(14, 41);
				}
				else if (num3 == 144)
				{
					empty = new Size(10, 32);
				}
				else if (num3 == 168)
				{
					empty = new Size(8, 24);
				}
				else if (num3 == 192)
				{
					empty = new Size(6, 17);
				}
				else if (num3 == 216)
				{
					empty = new Size(2, 8);
				}
			}
			else
			{
				double num4 = (double)((num2 - num3) / num3);
				empty = new Size((int)Math.Ceiling(8.0 * num4), (int)Math.Ceiling(32.0 * num4));
			}
			if (num2 == num)
			{
				return empty;
			}
			return new Size(empty.Width * -1, empty.Height * -1);
		}

		// Token: 0x0400007F RID: 127
		private string m_TitleFormat;

		// Token: 0x04000080 RID: 128
		private string m_TitleNoVM;

		// Token: 0x04000081 RID: 129
		private RdpViewerControl m_RdpViewer;

		// Token: 0x04000082 RID: 130
		private MenuManager m_MenuManager;

		// Token: 0x04000083 RID: 131
		private List<VMSettingsDialog> m_VMSettingsDialogs = new List<VMSettingsDialog>(1);

		// Token: 0x04000084 RID: 132
		private Rectangle m_PreFullScreenBounds;

		// Token: 0x04000085 RID: 133
		private Size m_PreFullScreenRdpSize;

		// Token: 0x04000086 RID: 134
		private Size m_PreviousClientSize;

		// Token: 0x04000087 RID: 135
		private int m_PreviousClientSizeDpi;

		// Token: 0x04000088 RID: 136
		private VmisStatusStrip m_StatusBar;

		// Token: 0x04000089 RID: 137
		private VmisToolStrip m_ToolBar;

		// Token: 0x0400008B RID: 139
		private FormWindowState m_RestoreState;

		// Token: 0x0400008C RID: 140
		private RdpConnectionMode m_RdpMode;

		// Token: 0x0400008D RID: 141
		private NoConnectionDialog m_NoConnectionDialog;

		// Token: 0x0400008E RID: 142
		private ElementHost m_NoConnectionElementHost;

		// Token: 0x0400008F RID: 143
		private Size m_TrueSyncedEnhancedModeDesktopResolution;

		// Token: 0x04000090 RID: 144
		private bool _enteringFullScreen;

		// Token: 0x04000091 RID: 145
		private bool _closing;

		// Token: 0x04000092 RID: 146
		private bool _onRestoreEnterFullScreen;

		// Token: 0x04000093 RID: 147
		private bool _isRestoring;

		// Token: 0x04000094 RID: 148
		private int _oldDpi = 96;

		// Token: 0x0200003F RID: 63
		// (Invoke) Token: 0x060003C7 RID: 967
		private delegate void OpenVirtualMachineRdpConnectionMethod(RdpConnectionInfo connectionInfo);
	}
}
