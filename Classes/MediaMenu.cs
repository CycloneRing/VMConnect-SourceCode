using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Management.Infrastructure;
using Microsoft.Virtualization.Client.Management;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000031 RID: 49
	internal class MediaMenu : IMenuItemProvider, IObserver<CimSubscriptionResult>
	{
		// Token: 0x0600025C RID: 604 RVA: 0x00012068 File Offset: 0x00010268
		public MediaMenu(IMenuActionTarget menuTarget)
		{
			if (menuTarget == null)
			{
				throw new ArgumentNullException("menuTarget");
			}
			this.m_MenuTarget = menuTarget;
			VmisMenuItemFactory vmisMenuItemFactory = new VmisMenuItemFactory(new EventHandler(this.OnMenuItem));
			VmisMenuItem vmisMenuItem = vmisMenuItemFactory.CreateMenuItem(MediaMenu.ResourceInfo.DrivesNotLoaded);
			vmisMenuItem.Enabled = false;
			this.m_Media = vmisMenuItemFactory.CreateParentMenuItem(MediaMenu.ResourceInfo.Media, new ToolStripItem[]
			{
				vmisMenuItem
			});
			this.m_Media.DropDownOpening += this.BuildMenu;
			if (this.m_MenuTarget.VirtualMachine != null)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.InitializeDriveInfo), this.m_MenuTarget.VirtualMachine.Server);
			}
		}

		// Token: 0x0600025D RID: 605 RVA: 0x00012138 File Offset: 0x00010338
		private void BuildMenu(object sender, EventArgs ea)
		{
			if (this.m_MenuTarget.VirtualMachine == null)
			{
				if (this.m_Media.DropDownItems.Count != 1 || this.m_Media.DropDownItems[0].Text != MediaMenu.ResourceInfo.DrivesNotLoaded.GetText())
				{
					this.m_Media.DropDownItems.Clear();
					VmisMenuItem vmisMenuItem = new VmisMenuItemFactory(new EventHandler(this.OnMenuItem)).CreateMenuItem(MediaMenu.ResourceInfo.DrivesNotLoaded);
					vmisMenuItem.Enabled = false;
					this.m_Media.DropDownItems.Add(vmisMenuItem);
				}
				return;
			}
			List<IVMDriveSetting> list = null;
			List<IVirtualDiskSetting> list2 = null;
			List<string> list3 = null;
			List<IPhysicalCDRomDrive> physicalCDDrives = null;
			object objectSync = this.m_ObjectSync;
			lock (objectSync)
			{
				if (this.m_RebuildMenu)
				{
					this.m_RebuildMenu = false;
					list = this.m_Drives;
					list2 = this.m_Disks;
					list3 = this.m_DiskPaths;
					physicalCDDrives = this.m_PhysicalDrives;
				}
			}
			if (list != null && list2 != null && list3 != null)
			{
				this.CreateDriveMenuItems(list, list2, list3, physicalCDDrives);
			}
		}

		// Token: 0x0600025E RID: 606 RVA: 0x00012254 File Offset: 0x00010454
		private void CreateDriveMenuItems(List<IVMDriveSetting> drives, List<IVirtualDiskSetting> disks, List<string> diskPaths, List<IPhysicalCDRomDrive> physicalCDDrives)
		{
			this.m_Media.DropDownItems.Clear();
			VmisMenuItemFactory vmisMenuItemFactory = new VmisMenuItemFactory(new EventHandler(this.OnMenuItem));
			List<MediaMenu.DriveMenuItem> list = new List<MediaMenu.DriveMenuItem>();
			MediaMenu.DriveMenuItem driveMenuItem = null;
			MediaMenu.ShortcutKeyPicker shortcutPicker = new MediaMenu.ShortcutKeyPicker();
			for (int i = 0; i < drives.Count; i++)
			{
				MediaMenu.DriveMenuItem driveMenuItem2 = this.CreateDriveMenuItem(drives[i], disks[i], diskPaths[i], physicalCDDrives, vmisMenuItemFactory, shortcutPicker);
				if (drives[i].VMDeviceSettingType == VMDeviceSettingType.DvdSyntheticDrive)
				{
					list.Add(driveMenuItem2);
				}
				else
				{
					driveMenuItem = driveMenuItem2;
				}
			}
			foreach (MediaMenu.DriveMenuItem value in list)
			{
				this.m_Media.DropDownItems.Add(value);
			}
			if (list.Count > 0 && driveMenuItem != null)
			{
				this.m_Media.DropDownItems.Add(vmisMenuItemFactory.CreateMenuItemSeparator());
			}
			if (driveMenuItem != null)
			{
				this.m_Media.DropDownItems.Add(driveMenuItem);
			}
			if (this.m_Media.DropDownItems.Count > 0)
			{
				this.UpdateEnabledState();
			}
			else
			{
				VmisMenuItem vmisMenuItem = vmisMenuItemFactory.CreateMenuItem(MediaMenu.ResourceInfo.DrivesNotLoaded);
				vmisMenuItem.Enabled = false;
				this.m_Media.DropDownItems.Add(vmisMenuItem);
			}
			this.m_Media.UpdateDropDownSizes();
		}

		// Token: 0x0600025F RID: 607 RVA: 0x000123C0 File Offset: 0x000105C0
		private MediaMenu.DriveMenuItem CreateDriveMenuItem(IVMDriveSetting drive, IVirtualDiskSetting disk, string diskPath, List<IPhysicalCDRomDrive> physicalCDDrives, VmisMenuItemFactory factory, MediaMenu.ShortcutKeyPicker shortcutPicker)
		{
			MediaMenu.DriveMenuItem driveMenuItem = new MediaMenu.DriveMenuItem(this.m_MenuTarget.VirtualMachine.InstanceId, drive, disk, diskPath, shortcutPicker);
			MediaMenu.EjectDriveSubMenuItem ejectDriveSubMenuItem = new MediaMenu.EjectDriveSubMenuItem(MediaMenu.ResourceInfo.Eject, new EventHandler(this.OnMenuItem));
			driveMenuItem.DropDownItems.Add(ejectDriveSubMenuItem);
			driveMenuItem.DropDownItems.Add(factory.CreateMenuItemSeparator());
			MediaMenu.InsertDiskDriveSubMenuItem value = new MediaMenu.InsertDiskDriveSubMenuItem(MediaMenu.ResourceInfo.InsertDisk, new EventHandler(this.OnMenuItem));
			driveMenuItem.DropDownItems.Add(value);
			if (drive.VMDeviceSettingType == VMDeviceSettingType.DvdSyntheticDrive && drive.ControllerSetting.VMDeviceSettingType != VMDeviceSettingType.ScsiSyntheticController && physicalCDDrives != null && physicalCDDrives.Count > 0)
			{
				driveMenuItem.DropDownItems.Add(factory.CreateMenuItemSeparator());
				foreach (IPhysicalCDRomDrive physicalCDRomDrive in physicalCDDrives)
				{
					MediaMenu.CaptureDriveSubMenuItem captureDriveSubMenuItem = new MediaMenu.CaptureDriveSubMenuItem(MediaMenu.ResourceInfo.Capture, new EventHandler(this.OnMenuItem), physicalCDRomDrive);
					if (diskPath == physicalCDRomDrive.PnpDeviceId)
					{
						captureDriveSubMenuItem.Checked = true;
					}
					driveMenuItem.DropDownItems.Add(captureDriveSubMenuItem);
				}
			}
			ejectDriveSubMenuItem.UpdateText();
			return driveMenuItem;
		}

		// Token: 0x06000260 RID: 608 RVA: 0x00012508 File Offset: 0x00010708
		private void InitializeDriveInfo(object serverObj)
		{
			Server server = (Server)serverObj;
			this.GetDrivesForMenuItems(null);
			try
			{
				this.m_HardwareConfigChangeSubscription = server.SubscribeAsync(Server.CimV2Namespace, MediaMenu.gm_HardwareConfigChangeQuery, this);
			}
			catch (Exception ex)
			{
				VMTrace.TraceError("Error initializing hardware configuration watcher!", ex);
			}
			this.HandleHardwareConfigChangeInternal(serverObj);
		}

		// Token: 0x06000261 RID: 609 RVA: 0x00012564 File Offset: 0x00010764
		private void CleanupHardwareConfigWatcher()
		{
			if (this.m_HardwareConfigChangeSubscription != null)
			{
				this.m_HardwareConfigChangeSubscription.Dispose();
				this.m_HardwareConfigChangeSubscription = null;
			}
		}

		// Token: 0x06000262 RID: 610 RVA: 0x00012580 File Offset: 0x00010780
		private void GetDrivesForMenuItems(object ignored)
		{
			try
			{
				object getDrivesLock = this.m_GetDrivesLock;
				lock (getDrivesLock)
				{
					IVMComputerSystem virtualMachine = this.m_MenuTarget.VirtualMachine;
					if (virtualMachine != null)
					{
						List<IVMDriveSetting> list = new List<IVMDriveSetting>(5);
						List<IVirtualDiskSetting> list2 = new List<IVirtualDiskSetting>(5);
						List<string> list3 = new List<string>(5);
						foreach (IVMDeviceSetting ivmdeviceSetting in virtualMachine.Setting.GetDeviceSettingsLimited(true, TimeSpan.Zero))
						{
							if (ivmdeviceSetting.VMDeviceSettingType == VMDeviceSettingType.DvdSyntheticDrive || ivmdeviceSetting.VMDeviceSettingType == VMDeviceSettingType.DisketteSyntheticDrive)
							{
								try
								{
									IVMDriveSetting ivmdriveSetting = (IVMDriveSetting)ivmdeviceSetting;
									ivmdriveSetting.UpdateAssociationCache();
									IVirtualDiskSetting insertedDisk = ivmdriveSetting.GetInsertedDisk();
									string item = null;
									if (insertedDisk != null)
									{
										insertedDisk.UpdatePropertyCache();
										item = insertedDisk.Path;
									}
									list.Add(ivmdriveSetting);
									list2.Add(insertedDisk);
									list3.Add(item);
								}
								catch (VirtualizationManagementException ex)
								{
									VMTrace.TraceError("Error getting media menu information for drive! Continue with other drives.", ex);
								}
							}
						}
						object objectSync = this.m_ObjectSync;
						lock (objectSync)
						{
							bool flag3 = true;
							if (this.m_Drives != null && this.m_DiskPaths != null)
							{
								flag3 = (this.m_Drives.Count != list.Count || this.m_DiskPaths.Count != list3.Count);
								int num = 0;
								while (num < this.m_Drives.Count && !flag3)
								{
									flag3 = (this.m_Drives[num].DeviceId != list[num].DeviceId);
									num++;
								}
								int num2 = 0;
								while (num2 < this.m_DiskPaths.Count && !flag3)
								{
									flag3 = !string.Equals(this.m_DiskPaths[num2], list3[num2]);
									num2++;
								}
							}
							if (flag3)
							{
								this.m_Drives = list;
								this.m_Disks = list2;
								this.m_DiskPaths = list3;
								this.m_RebuildMenu = true;
							}
						}
					}
				}
			}
			catch (Exception ex2)
			{
				VMTrace.TraceError("There was an error retrieving the data to build the media menu!", ex2);
			}
		}

		// Token: 0x06000263 RID: 611 RVA: 0x00012824 File Offset: 0x00010A24
		public void OnCompleted()
		{
			VMTrace.TraceInformation(string.Format(CultureInfo.InvariantCulture, "{0}::OnCompleted called for event notification query '{1}'.", base.GetType().Name, MediaMenu.gm_HardwareConfigChangeQuery), Array.Empty<string>());
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0001284F File Offset: 0x00010A4F
		public void OnError(Exception error)
		{
			VMTrace.TraceError(string.Format(CultureInfo.InvariantCulture, "{0}::OnError called for event notification '{1}' with exception {2}.", base.GetType().Name, MediaMenu.gm_HardwareConfigChangeQuery, error.GetType().Name), error);
		}

		// Token: 0x06000265 RID: 613 RVA: 0x00012884 File Offset: 0x00010A84
		public void OnNext(CimSubscriptionResult value)
		{
			try
			{
				IVMComputerSystem virtualMachine = this.m_MenuTarget.VirtualMachine;
				if (virtualMachine != null)
				{
					this.HandleHardwareConfigChangeInternal(virtualMachine.Server);
				}
			}
			catch (Exception ex)
			{
				VMTrace.TraceError("Error handling WMI event arrived!", ex);
			}
		}

		// Token: 0x06000266 RID: 614 RVA: 0x000128CC File Offset: 0x00010ACC
		private void HandleHardwareConfigChangeInternal(object serverObj)
		{
			try
			{
				object hardwareConfigChangeLock = this.m_HardwareConfigChangeLock;
				lock (hardwareConfigChangeLock)
				{
					Server server = (Server)serverObj;
					List<IPhysicalCDRomDrive> list = new List<IPhysicalCDRomDrive>(3);
					foreach (IPhysicalCDRomDrive physicalCDRomDrive in ObjectLocator.GetHostComputerSystem(server).GetPhysicalCDDrives(true, TimeSpan.Zero))
					{
						if (!string.IsNullOrEmpty(physicalCDRomDrive.Drive))
						{
							list.Add(physicalCDRomDrive);
						}
					}
					object objectSync = this.m_ObjectSync;
					lock (objectSync)
					{
						bool flag3 = true;
						if (this.m_PhysicalDrives != null)
						{
							flag3 = (this.m_PhysicalDrives.Count != list.Count);
							int num = 0;
							while (num < this.m_PhysicalDrives.Count && !flag3)
							{
								flag3 = (this.m_PhysicalDrives[num].PnpDeviceId != list[num].PnpDeviceId);
								num++;
							}
						}
						if (flag3)
						{
							this.m_PhysicalDrives = list;
							this.m_RebuildMenu = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				VMTrace.TraceWarning("Error updating physical CD drive information. Note that if the user does not have admin access then they will not have permission to see the drives so in that case this is expected.", ex);
			}
		}

		// Token: 0x06000267 RID: 615 RVA: 0x00012A60 File Offset: 0x00010C60
		private void OnMenuItem(object sender, EventArgs ea)
		{
			MediaMenu.DriveSubMenuItem driveSubMenuItem = sender as MediaMenu.DriveSubMenuItem;
			if (driveSubMenuItem != null)
			{
				foreach (ToolStripMenuItem toolStripMenuItem in driveSubMenuItem.Parent.DropDownItems.OfType<ToolStripMenuItem>())
				{
					toolStripMenuItem.Enabled = false;
				}
				switch (driveSubMenuItem.Type)
				{
				case MediaMenu.DriveSubMenuItemType.Eject:
				case MediaMenu.DriveSubMenuItemType.Capture:
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.OnMenuItemBackground), driveSubMenuItem);
					return;
				case MediaMenu.DriveSubMenuItemType.InsertDisk:
					if (this.m_MenuTarget.VirtualMachine != null)
					{
						Server server = this.m_MenuTarget.VirtualMachine.Server;
						RemoteFileBrowser remoteFileBrowser = new RemoteFileBrowser(server);
						string initialPath;
						string fileExtensionFilter;
						if (driveSubMenuItem.Parent.Drive.VMDeviceSettingType == VMDeviceSettingType.DvdSyntheticDrive)
						{
							initialPath = CommonConfiguration.Instance.GetIsoImagePathForServer(server);
							fileExtensionFilter = CommonUtilities.IsoFileFilterDescription;
						}
						else
						{
							initialPath = CommonConfiguration.Instance.GetVirtualFloppyDiskPathForServer(server);
							fileExtensionFilter = CommonUtilities.VirtualFloppyDiskFilterDescription;
						}
						remoteFileBrowser.OwnerWindow = this.m_MenuTarget.DialogOwner.Handle;
						remoteFileBrowser.InitialPath = initialPath;
						remoteFileBrowser.FileExtensionFilter = fileExtensionFilter;
						string text = remoteFileBrowser.BrowseForFile();
						if (!string.IsNullOrEmpty(text))
						{
							try
							{
								if (driveSubMenuItem.Parent.Drive.VMDeviceSettingType == VMDeviceSettingType.DvdSyntheticDrive)
								{
									CommonConfiguration.Instance.SetIsoImagePathForServer(server, Path.GetDirectoryName(text));
								}
								else
								{
									CommonConfiguration.Instance.SetVirtualFloppyDiskPathForServer(server, Path.GetDirectoryName(text));
								}
							}
							catch (Exception ex)
							{
								VMTrace.TraceError("Error saving media path to local storage!", ex);
							}
							driveSubMenuItem.Tag = text;
							ThreadPool.QueueUserWorkItem(new WaitCallback(this.OnMenuItemBackground), driveSubMenuItem);
						}
						else
						{
							this.UpdateEnabledState();
						}
						this.m_MenuTarget.DeactivateOnDialogClosed();
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06000268 RID: 616 RVA: 0x00012C14 File Offset: 0x00010E14
		private void OnMenuItemBackground(object menuItemObj)
		{
			MediaMenu.DriveSubMenuItem driveSubMenuItem = (MediaMenu.DriveSubMenuItem)menuItemObj;
			MediaMenu.DriveMenuItem parent = driveSubMenuItem.Parent;
			string text = string.Empty;
			try
			{
				switch (driveSubMenuItem.Type)
				{
				case MediaMenu.DriveSubMenuItemType.Eject:
					if (driveSubMenuItem.Text.StartsWith(MediaMenu.ResourceInfo.Eject.GetText(), StringComparison.CurrentCulture))
					{
						text = VMISResources.MediaMenu_EjectDiskFailed;
					}
					else
					{
						text = VMISResources.MediaMenu_UncaptureDriveFailed;
					}
					parent.EjectDisk();
					break;
				case MediaMenu.DriveSubMenuItemType.InsertDisk:
				{
					text = VMISResources.MediaMenu_InsertDiskFailed;
					string diskPath = driveSubMenuItem.Tag as string;
					driveSubMenuItem.Tag = null;
					parent.InsertDisk(diskPath);
					break;
				}
				case MediaMenu.DriveSubMenuItemType.Capture:
					text = VMISResources.MediaMenu_CaptureDriveFailed;
					parent.CaptureDrive(((MediaMenu.CaptureDriveSubMenuItem)driveSubMenuItem).PhysicalDrive);
					break;
				}
				this.m_MenuTarget.AsyncUIThreadMethodInvoker(new MethodInvoker(this.UpdateEnabledState), null);
			}
			catch (Exception ex)
			{
				this.m_MenuTarget.AsyncUIThreadMethodInvoker(new MediaMenu.MenuItemFailureMethod(this.MenuItemFailure), new object[]
				{
					text,
					ex
				});
			}
		}

		// Token: 0x06000269 RID: 617 RVA: 0x00012D1C File Offset: 0x00010F1C
		private void MenuItemFailure(string mainInstruction, Exception ex)
		{
			Program.Displayer.DisplayError(mainInstruction, ex);
			this.UpdateEnabledState();
		}

		// Token: 0x0600026A RID: 618 RVA: 0x00012D30 File Offset: 0x00010F30
		public bool InformKeyUp(Keys key)
		{
			return false;
		}

		// Token: 0x0600026B RID: 619 RVA: 0x00012D34 File Offset: 0x00010F34
		public void UpdateEnabledState()
		{
			if (this.m_MenuTarget.VirtualMachine != null)
			{
				VMComputerSystemState state = this.m_MenuTarget.VirtualMachine.State;
				bool flag = state == VMComputerSystemState.Running || state == VMComputerSystemState.PowerOff;
				foreach (object obj in this.m_Media.DropDownItems)
				{
					MediaMenu.DriveMenuItem driveMenuItem = obj as MediaMenu.DriveMenuItem;
					if (driveMenuItem != null)
					{
						driveMenuItem.Enabled = flag;
						foreach (object obj2 in driveMenuItem.DropDownItems)
						{
							MediaMenu.DriveSubMenuItem driveSubMenuItem = obj2 as MediaMenu.DriveSubMenuItem;
							if (driveSubMenuItem != null)
							{
								switch (driveSubMenuItem.Type)
								{
								case MediaMenu.DriveSubMenuItemType.Eject:
									driveSubMenuItem.Enabled = (flag && !string.IsNullOrEmpty(driveSubMenuItem.Parent.InsertedDiskPath));
									break;
								case MediaMenu.DriveSubMenuItemType.InsertDisk:
									driveSubMenuItem.Enabled = flag;
									break;
								case MediaMenu.DriveSubMenuItemType.Capture:
									driveSubMenuItem.Enabled = (flag && !driveSubMenuItem.Checked);
									break;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600026C RID: 620 RVA: 0x00012E80 File Offset: 0x00011080
		public void InformVMConfigurationChanged()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.GetDrivesForMenuItems));
		}

		// Token: 0x0600026D RID: 621 RVA: 0x00012E94 File Offset: 0x00011094
		public void CloseMenu()
		{
			Program.TasksToCompleteBeforeExitingTracker.RegisterBackgroundTask(new MethodInvoker(this.CleanupHardwareConfigWatcher), Array.Empty<object>());
		}

		// Token: 0x0600026E RID: 622 RVA: 0x00012EB1 File Offset: 0x000110B1
		void IMenuItemProvider.InformBeginStateChangeOperation(VMStateChangeAction stateChange)
		{
		}

		// Token: 0x0600026F RID: 623 RVA: 0x00012EB3 File Offset: 0x000110B3
		void IMenuItemProvider.InformEndStateChangeOperation(VMStateChangeAction stateChange)
		{
		}

		// Token: 0x06000270 RID: 624 RVA: 0x00012EB5 File Offset: 0x000110B5
		public VmisMenuItem[] GetMenuItems()
		{
			return new VmisMenuItem[]
			{
				this.m_Media
			};
		}

		// Token: 0x0400016F RID: 367
		private static string gm_HardwareConfigChangeQuery = "select * from Win32_SystemConfigurationChangeEvent where EventType=\"1\"";

		// Token: 0x04000170 RID: 368
		private IMenuActionTarget m_MenuTarget;

		// Token: 0x04000171 RID: 369
		private VmisMenuItem m_Media;

		// Token: 0x04000172 RID: 370
		private object m_ObjectSync = new object();

		// Token: 0x04000173 RID: 371
		private bool m_RebuildMenu;

		// Token: 0x04000174 RID: 372
		private List<IVMDriveSetting> m_Drives;

		// Token: 0x04000175 RID: 373
		private List<IVirtualDiskSetting> m_Disks;

		// Token: 0x04000176 RID: 374
		private List<string> m_DiskPaths;

		// Token: 0x04000177 RID: 375
		private List<IPhysicalCDRomDrive> m_PhysicalDrives;

		// Token: 0x04000178 RID: 376
		private object m_GetDrivesLock = new object();

		// Token: 0x04000179 RID: 377
		private object m_HardwareConfigChangeLock = new object();

		// Token: 0x0400017A RID: 378
		private IDisposable m_HardwareConfigChangeSubscription;

		// Token: 0x02000054 RID: 84
		private class DriveMenuItem : VmisMenuItem
		{
			// Token: 0x060003F7 RID: 1015 RVA: 0x00015E92 File Offset: 0x00014092
			public DriveMenuItem(string vmInstanceId, IVMDriveSetting drive, IVirtualDiskSetting disk, string diskPath, MediaMenu.ShortcutKeyPicker shortcutPicker) : base(shortcutPicker.PickShortcut(drive.FriendlyName, drive.VMDeviceSettingType))
			{
				this.m_VMInstanceId = vmInstanceId;
				this.m_Drive = drive;
				this.m_InsertedDisk = disk;
				this.m_InsertedDiskPath = diskPath;
			}

			// Token: 0x17000186 RID: 390
			// (get) Token: 0x060003F8 RID: 1016 RVA: 0x00015ECA File Offset: 0x000140CA
			public IVMDriveSetting Drive
			{
				get
				{
					return this.m_Drive;
				}
			}

			// Token: 0x17000187 RID: 391
			// (get) Token: 0x060003F9 RID: 1017 RVA: 0x00015ED2 File Offset: 0x000140D2
			public string InsertedDiskPath
			{
				get
				{
					return this.m_InsertedDiskPath;
				}
			}

			// Token: 0x060003FA RID: 1018 RVA: 0x00015EDC File Offset: 0x000140DC
			public void EjectDisk()
			{
				try
				{
					this.m_InsertedDisk.Delete();
					this.m_InsertedDisk = null;
					this.m_InsertedDiskPath = null;
				}
				catch (VirtualizationOperationFailedException)
				{
					if (!this.IsDriveEmpty())
					{
						throw;
					}
					this.m_InsertedDisk = null;
					this.m_InsertedDiskPath = null;
				}
				catch (ServerObjectDeletedException)
				{
					if (!this.IsDriveEmpty())
					{
						throw;
					}
					this.m_InsertedDisk = null;
					this.m_InsertedDiskPath = null;
				}
			}

			// Token: 0x060003FB RID: 1019 RVA: 0x00015F5C File Offset: 0x0001415C
			public void InsertDisk(string diskPath)
			{
				bool flag = true;
				if (this.m_InsertedDisk != null)
				{
					try
					{
						this.m_InsertedDisk.Path = diskPath;
						this.m_InsertedDisk.Put();
						this.m_InsertedDiskPath = diskPath;
						flag = false;
					}
					catch (VirtualizationOperationFailedException)
					{
						if (!this.IsDriveEmpty())
						{
							throw;
						}
					}
					catch (ServerObjectDeletedException)
					{
						if (!this.IsDriveEmpty())
						{
							throw;
						}
					}
				}
				if (flag)
				{
					IHostComputerSystem hostComputerSystem = ObjectLocator.GetHostComputerSystem(this.m_Drive.Server);
					VMDeviceSettingType deviceType = (this.m_Drive.VMDeviceSettingType == VMDeviceSettingType.DvdSyntheticDrive) ? VMDeviceSettingType.IsoDisk : VMDeviceSettingType.FloppyDisk;
					IVirtualDiskSetting virtualDiskSetting = (IVirtualDiskSetting)hostComputerSystem.GetSettingCapabilities(deviceType, Capabilities.DefaultCapability);
					virtualDiskSetting.Path = diskPath;
					virtualDiskSetting.DriveSetting = this.m_Drive;
					IVMComputerSystem vmcomputerSystem = ObjectLocator.GetVMComputerSystem(this.m_Drive.Server, this.m_VMInstanceId);
					IVirtualDiskSetting virtualDiskSetting2 = null;
					try
					{
						using (IVMTask ivmtask = vmcomputerSystem.BeginAddDevice(virtualDiskSetting))
						{
							ivmtask.WaitForCompletion();
							virtualDiskSetting2 = (IVirtualDiskSetting)vmcomputerSystem.EndAddDevice(ivmtask);
						}
					}
					catch (VirtualizationOperationFailedException)
					{
						this.m_Drive.UpdateAssociationCache();
						virtualDiskSetting2 = this.m_Drive.GetInsertedDisk();
						if (virtualDiskSetting2 == null)
						{
							throw;
						}
						virtualDiskSetting2.Path = diskPath;
						virtualDiskSetting2.Put();
					}
					this.m_InsertedDisk = virtualDiskSetting2;
					this.m_InsertedDiskPath = diskPath;
				}
			}

			// Token: 0x060003FC RID: 1020 RVA: 0x000160C0 File Offset: 0x000142C0
			public void CaptureDrive(IPhysicalCDRomDrive physicalDrive)
			{
				this.InsertDisk(physicalDrive.PnpDeviceId);
			}

			// Token: 0x060003FD RID: 1021 RVA: 0x000160D0 File Offset: 0x000142D0
			private bool IsDriveEmpty()
			{
				bool result = false;
				IVirtualDiskSetting virtualDiskSetting;
				if (this.TryGetUpdatedInsertedDisk(out virtualDiskSetting))
				{
					result = (virtualDiskSetting == null);
				}
				return result;
			}

			// Token: 0x060003FE RID: 1022 RVA: 0x000160F0 File Offset: 0x000142F0
			private bool TryGetUpdatedInsertedDisk(out IVirtualDiskSetting insertedDisk)
			{
				bool result = false;
				insertedDisk = null;
				try
				{
					this.m_Drive.UpdateAssociationCache();
					insertedDisk = this.m_Drive.GetInsertedDisk();
					result = true;
				}
				catch (Exception ex)
				{
					VMTrace.TraceError("Error getting the updated disk object in the drive!", ex);
				}
				return result;
			}

			// Token: 0x040001F3 RID: 499
			private string m_VMInstanceId;

			// Token: 0x040001F4 RID: 500
			private IVMDriveSetting m_Drive;

			// Token: 0x040001F5 RID: 501
			private IVirtualDiskSetting m_InsertedDisk;

			// Token: 0x040001F6 RID: 502
			private string m_InsertedDiskPath;
		}

		// Token: 0x02000055 RID: 85
		private enum DriveSubMenuItemType
		{
			// Token: 0x040001F8 RID: 504
			Eject,
			// Token: 0x040001F9 RID: 505
			InsertDisk,
			// Token: 0x040001FA RID: 506
			Capture
		}

		// Token: 0x02000056 RID: 86
		private abstract class DriveSubMenuItem : VmisMenuItem
		{
			// Token: 0x060003FF RID: 1023 RVA: 0x00016140 File Offset: 0x00014340
			protected DriveSubMenuItem(MenuItemResourceInfo menuInfo, EventHandler onClick, MediaMenu.DriveSubMenuItemType type) : base(menuInfo.GetText(), onClick)
			{
				this.m_Type = type;
			}

			// Token: 0x17000188 RID: 392
			// (get) Token: 0x06000400 RID: 1024 RVA: 0x00016157 File Offset: 0x00014357
			public MediaMenu.DriveSubMenuItemType Type
			{
				get
				{
					return this.m_Type;
				}
			}

			// Token: 0x17000189 RID: 393
			// (get) Token: 0x06000401 RID: 1025 RVA: 0x0001615F File Offset: 0x0001435F
			public new MediaMenu.DriveMenuItem Parent
			{
				get
				{
					return base.OwnerItem as MediaMenu.DriveMenuItem;
				}
			}

			// Token: 0x040001FB RID: 507
			private MediaMenu.DriveSubMenuItemType m_Type;
		}

		// Token: 0x02000057 RID: 87
		private class EjectDriveSubMenuItem : MediaMenu.DriveSubMenuItem
		{
			// Token: 0x06000402 RID: 1026 RVA: 0x0001616C File Offset: 0x0001436C
			public EjectDriveSubMenuItem(MenuItemResourceInfo menuInfo, EventHandler onClick) : base(menuInfo, onClick, MediaMenu.DriveSubMenuItemType.Eject)
			{
			}

			// Token: 0x06000403 RID: 1027 RVA: 0x00016178 File Offset: 0x00014378
			public void UpdateText()
			{
				string text = null;
				MediaMenu.DriveMenuItem parent = base.Parent;
				if (parent != null)
				{
					text = parent.InsertedDiskPath;
				}
				if (text == null)
				{
					this.Text = MediaMenu.ResourceInfo.Eject.GetText();
					return;
				}
				if (CommonUtilities.IsIsoImage(text) || CommonUtilities.IsVirtualFloppyDisk(text))
				{
					string text2 = text;
					try
					{
						text2 = Path.GetFileName(text2);
					}
					catch (ArgumentException)
					{
					}
					if (text2.Length > 30)
					{
						StringBuilder stringBuilder = new StringBuilder(text2, 0, 13, 30);
						stringBuilder.Append("...");
						stringBuilder.Append(text2, text2.Length - 14, 14);
						text2 = stringBuilder.ToString();
					}
					text2 = text2.Replace("&", "&&");
					this.Text = string.Format(CultureInfo.CurrentCulture, VMISResources.MediaMenu_EjectDisk, text2);
					return;
				}
				IPhysicalCDRomDrive physicalCDRomDrive = null;
				if (parent != null)
				{
					foreach (ToolStripMenuItem toolStripMenuItem in base.Parent.DropDownItems.OfType<ToolStripMenuItem>())
					{
						if (toolStripMenuItem.Checked)
						{
							MediaMenu.CaptureDriveSubMenuItem captureDriveSubMenuItem = toolStripMenuItem as MediaMenu.CaptureDriveSubMenuItem;
							if (captureDriveSubMenuItem != null)
							{
								physicalCDRomDrive = captureDriveSubMenuItem.PhysicalDrive;
								break;
							}
							break;
						}
					}
				}
				this.Text = string.Format(CultureInfo.CurrentCulture, VMISResources.MediaMenu_UncaptureDrive, (physicalCDRomDrive != null) ? physicalCDRomDrive.Drive : string.Empty);
			}
		}

		// Token: 0x02000058 RID: 88
		private class InsertDiskDriveSubMenuItem : MediaMenu.DriveSubMenuItem
		{
			// Token: 0x06000404 RID: 1028 RVA: 0x000162D4 File Offset: 0x000144D4
			public InsertDiskDriveSubMenuItem(MenuItemResourceInfo menuInfo, EventHandler onClick) : base(menuInfo, onClick, MediaMenu.DriveSubMenuItemType.InsertDisk)
			{
			}
		}

		// Token: 0x02000059 RID: 89
		private sealed class CaptureDriveSubMenuItem : MediaMenu.DriveSubMenuItem
		{
			// Token: 0x06000405 RID: 1029 RVA: 0x000162DF File Offset: 0x000144DF
			public CaptureDriveSubMenuItem(MenuItemResourceInfo menuInfo, EventHandler onClick, IPhysicalCDRomDrive physicalDrive) : base(menuInfo, onClick, MediaMenu.DriveSubMenuItemType.Capture)
			{
				this.m_PhysicalDrive = physicalDrive;
				this.Text = string.Format(CultureInfo.CurrentCulture, this.Text, physicalDrive.Drive);
			}

			// Token: 0x1700018A RID: 394
			// (get) Token: 0x06000406 RID: 1030 RVA: 0x0001630D File Offset: 0x0001450D
			public IPhysicalCDRomDrive PhysicalDrive
			{
				get
				{
					return this.m_PhysicalDrive;
				}
			}

			// Token: 0x040001FC RID: 508
			private IPhysicalCDRomDrive m_PhysicalDrive;
		}

		// Token: 0x0200005A RID: 90
		private class ShortcutKeyPicker
		{
			// Token: 0x06000407 RID: 1031 RVA: 0x00016318 File Offset: 0x00014518
			public string PickShortcut(string menuName, VMDeviceSettingType driveType)
			{
				if (string.IsNullOrEmpty(menuName))
				{
					if (driveType == VMDeviceSettingType.DvdSyntheticDrive)
					{
						menuName = MenuResources.DefaultDvdDriveMenuName;
					}
					else
					{
						menuName = MenuResources.DefaultDisketteDriveMenuName;
					}
				}
				int num = menuName.IndexOf('&');
				while (num != -1)
				{
					if (num == menuName.Length - 1)
					{
						num = -1;
					}
					else
					{
						char c = char.ToLower(menuName[num + 1], CultureInfo.CurrentCulture);
						if (char.IsLetter(c) && !this.m_ExistingDriveShortcuts.Contains(c))
						{
							this.m_ExistingDriveShortcuts.Add(c);
							break;
						}
						menuName = menuName.Substring(0, num) + menuName.Substring(num + 1);
						num = menuName.IndexOf('&');
					}
				}
				if (num == -1)
				{
					char[] array = menuName.ToCharArray();
					for (int i = 0; i < array.Length; i++)
					{
						if (char.IsLetter(array[i]))
						{
							char item = char.ToLower(array[i], CultureInfo.CurrentCulture);
							if (!this.m_ExistingDriveShortcuts.Contains(item))
							{
								menuName = menuName.Substring(0, i) + "&" + menuName.Substring(i);
								this.m_ExistingDriveShortcuts.Add(item);
								break;
							}
						}
					}
				}
				return menuName;
			}

			// Token: 0x040001FD RID: 509
			private List<char> m_ExistingDriveShortcuts = new List<char>(5);
		}

		// Token: 0x0200005B RID: 91
		private static class ResourceInfo
		{
			// Token: 0x040001FE RID: 510
			private const string Append = "MediaMenu_";

			// Token: 0x040001FF RID: 511
			public static readonly MenuItemResourceInfo Media = new MenuItemResourceInfo("MediaMenu_Media", false);

			// Token: 0x04000200 RID: 512
			public static readonly MenuItemResourceInfo Eject = new MenuItemResourceInfo("MediaMenu_Eject", false);

			// Token: 0x04000201 RID: 513
			public static readonly MenuItemResourceInfo InsertDisk = new MenuItemResourceInfo("MediaMenu_InsertDisk", false);

			// Token: 0x04000202 RID: 514
			public static readonly MenuItemResourceInfo Capture = new MenuItemResourceInfo("MediaMenu_Capture", false);

			// Token: 0x04000203 RID: 515
			public static readonly MenuItemResourceInfo DrivesNotLoaded = new MenuItemResourceInfo("MediaMenu_DrivesNotLoaded", false);
		}

		// Token: 0x0200005C RID: 92
		// (Invoke) Token: 0x0600040B RID: 1035
		private delegate void MenuItemFailureMethod(string mainInstruction, Exception ex);
	}
}
