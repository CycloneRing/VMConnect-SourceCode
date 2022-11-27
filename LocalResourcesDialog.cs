using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Virtualization.Client.InteractiveSession.Resources;
using Microsoft.Virtualization.Client.Interop;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x0200000C RID: 12
	internal partial class LocalResourcesDialog : Form
	{
		// Token: 0x060000C3 RID: 195 RVA: 0x00008DF4 File Offset: 0x00006FF4
		public LocalResourcesDialog(RdpOptions options)
		{
			this.InitializeComponent();
			this.m_ResourceList.BeginUpdate();
			this.m_ResourceList.CheckBoxes = true;
			this.m_ResourceList.AfterCheck += this.AfterCheck;
			this.m_WindowInterceptors = new NativeWindowDoubleClickInterceptor(new List<Control>
			{
				this.m_ResourceList
			});
			this.m_ResourceList.Nodes.Add(ConnectionResources.Smartcard);
			this.m_ResourceList.Nodes.Add(ConnectionResources.Drives);
			this.m_ResourceList.Nodes.Add(ConnectionResources.PnpDevices);
			this.m_ResourceList.Nodes.Add(ConnectionResources.UsbDevices);
			this.m_UsbNodeExists = true;
			this.m_RdpOptions = options;
			using (RdpClient rdpClient = new RdpClient())
			{
				if (rdpClient != null)
				{
					rdpClient.BeginInit();
					base.Controls.Add(rdpClient);
					rdpClient.Hide();
					rdpClient.EndInit();
					IMsRdpClientNonScriptable3 msRdpClientNonScriptable = rdpClient.GetOcx() as IMsRdpClientNonScriptable3;
					if (msRdpClientNonScriptable != null)
					{
						try
						{
							RdpDriveInfo rdpDriveInfo = new RdpDriveInfo();
							IMsRdpDriveCollection driveCollection = msRdpClientNonScriptable.DriveCollection;
							for (uint num = 0U; num < driveCollection.DriveCount; num += 1U)
							{
								IMsRdpDrive msRdpDrive = driveCollection.get_DriveByIndex(num);
								TreeNode treeNode = new TreeNode();
								treeNode.Name = msRdpDrive.Name[0].ToString().ToLower(CultureInfo.InvariantCulture);
								treeNode.Text = rdpDriveInfo.GetDescription(msRdpDrive.Name);
								treeNode.Tag = (IMsRdpDriveV2)msRdpDrive;
								this.m_ResourceList.Nodes[1].Nodes.Add(treeNode);
							}
							this.m_ResourceList.Nodes[1].Nodes.Add(RdpOptions.DynamicDrives.ToLower(CultureInfo.InvariantCulture), ConnectionResources.AddedDrives);
							IMsRdpDeviceCollection deviceCollection = msRdpClientNonScriptable.DeviceCollection;
							if (deviceCollection.DeviceCount > 0U)
							{
								SortedDictionary<string, IMsRdpDeviceV2> sortedDictionary = new SortedDictionary<string, IMsRdpDeviceV2>(StringComparer.OrdinalIgnoreCase);
								SortedDictionary<string, IMsRdpDeviceV2> sortedDictionary2 = new SortedDictionary<string, IMsRdpDeviceV2>(StringComparer.OrdinalIgnoreCase);
								for (uint num2 = 0U; num2 < deviceCollection.DeviceCount; num2 += 1U)
								{
									IMsRdpDevice msRdpDevice = deviceCollection.get_DeviceByIndex(num2);
									IMsRdpDeviceV2 msRdpDeviceV = (IMsRdpDeviceV2)msRdpDevice;
									string deviceFriendlyName = this.GetDeviceFriendlyName(msRdpDeviceV);
									if (!string.IsNullOrEmpty(deviceFriendlyName) && !string.IsNullOrEmpty(msRdpDevice.DeviceInstanceId))
									{
										if (msRdpDeviceV.IsUSBDevice())
										{
											sortedDictionary.Add(deviceFriendlyName, msRdpDeviceV);
										}
										else
										{
											sortedDictionary2.Add(deviceFriendlyName, msRdpDeviceV);
										}
									}
								}
								foreach (KeyValuePair<string, IMsRdpDeviceV2> keyValuePair in sortedDictionary)
								{
									TreeNode treeNode = new TreeNode();
									treeNode.Text = keyValuePair.Key;
									treeNode.Tag = keyValuePair.Value;
									string text = keyValuePair.Value.DeviceInstanceId().TrimEnd(new char[1]);
									treeNode.Name = text.ToLower(CultureInfo.InvariantCulture);
									this.m_ResourceList.Nodes[3].Nodes.Add(treeNode);
								}
								foreach (KeyValuePair<string, IMsRdpDeviceV2> keyValuePair2 in sortedDictionary2)
								{
									TreeNode treeNode = new TreeNode();
									treeNode.Text = keyValuePair2.Key;
									treeNode.Tag = keyValuePair2.Value;
									string text2 = keyValuePair2.Value.DeviceInstanceId().TrimEnd(new char[1]);
									treeNode.Name = text2.ToLower(CultureInfo.InvariantCulture);
									this.m_ResourceList.Nodes[2].Nodes.Add(treeNode);
								}
							}
							this.m_ResourceList.Nodes[2].Nodes.Add(RdpOptions.DynamicDevices.ToLower(CultureInfo.InvariantCulture), ConnectionResources.AddedDevices);
						}
						catch (Exception ex)
						{
							VMTrace.TraceError("Exception occured while initializing device redirection settings UI.", ex);
						}
					}
					base.Controls.Remove(rdpClient);
				}
			}
			this.m_ResourceList.Nodes[0].Checked = this.m_RdpOptions.SmartCardsRedirection;
			this.InitializeDeviceSelection(RdpOptions.RedirectedDeviceType.Drive);
			this.InitializeDeviceSelection(RdpOptions.RedirectedDeviceType.Usb);
			this.InitializeDeviceSelection(RdpOptions.RedirectedDeviceType.Pnp);
			if (this.m_ResourceList.Nodes[3].Nodes.Count == 0)
			{
				this.m_ResourceList.Nodes.Remove(this.m_ResourceList.Nodes[3]);
				this.m_UsbNodeExists = false;
			}
			this.ExpandResourceTree();
			this.m_ResourceList.EndUpdate();
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x000092D4 File Offset: 0x000074D4
		private void InitializeDeviceSelection(RdpOptions.RedirectedDeviceType deviceType)
		{
			int index;
			switch (deviceType)
			{
			case RdpOptions.RedirectedDeviceType.Drive:
				index = 1;
				break;
			case RdpOptions.RedirectedDeviceType.Usb:
				index = 3;
				break;
			case RdpOptions.RedirectedDeviceType.Pnp:
				index = 2;
				break;
			default:
				return;
			}
			StringCollection redirectedDeviceCollection = this.m_RdpOptions.GetRedirectedDeviceCollection(deviceType);
			if (redirectedDeviceCollection != null && redirectedDeviceCollection.Count > 0)
			{
				if (redirectedDeviceCollection.Contains(RdpOptions.WildcardAllDevices))
				{
					this.m_ResourceList.Nodes[index].Checked = true;
					IEnumerator enumerator = this.m_ResourceList.Nodes[index].Nodes.GetEnumerator();
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							((TreeNode)obj).Checked = true;
						}
						return;
					}
				}
				foreach (object obj2 in this.m_ResourceList.Nodes[index].Nodes)
				{
					TreeNode treeNode = (TreeNode)obj2;
					if (redirectedDeviceCollection.Contains(treeNode.Name))
					{
						treeNode.Checked = true;
						this.ResolveConflict(treeNode);
					}
				}
			}
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00009410 File Offset: 0x00007610
		private void ExpandResourceTree()
		{
			foreach (TreeNode treeNode in this.m_ResourceList.Nodes.Cast<TreeNode>())
			{
				if (!treeNode.Checked)
				{
					if (treeNode.Nodes.Cast<TreeNode>().Any((TreeNode childNode) => childNode.Checked))
					{
						treeNode.Expand();
					}
				}
			}
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x000094A0 File Offset: 0x000076A0
		private void AfterCheck(object sender, TreeViewEventArgs e)
		{
			if (e.Action != TreeViewAction.Unknown)
			{
				e.Node.TreeView.SelectedNode = e.Node;
				if (e.Node.Nodes.Count > 0)
				{
					IEnumerator enumerator = e.Node.Nodes.GetEnumerator();
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							TreeNode treeNode = (TreeNode)obj;
							treeNode.Checked = e.Node.Checked;
							if (treeNode.Checked)
							{
								this.ResolveConflict(treeNode);
							}
						}
						return;
					}
				}
				if (e.Node.Checked)
				{
					this.ResolveConflict(e.Node);
				}
				this.UpdateParent(e.Node);
			}
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00009570 File Offset: 0x00007770
		private void UpdateParent(TreeNode changedNode)
		{
			if (changedNode.Parent != null)
			{
				if (!changedNode.Checked)
				{
					if (changedNode.Parent.Checked)
					{
						changedNode.Parent.Checked = false;
						return;
					}
				}
				else
				{
					bool flag = true;
					IEnumerator enumerator = changedNode.Parent.Nodes.GetEnumerator();
					{
						while (enumerator.MoveNext())
						{
							if (!((TreeNode)enumerator.Current).Checked)
							{
								flag = false;
								break;
							}
						}
					}
					if (flag)
					{
						changedNode.Parent.Checked = true;
					}
				}
			}
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x0000960C File Offset: 0x0000780C
		private void ResolveConflict(TreeNode selectedNode)
		{
			if (!this.m_UsbNodeExists)
			{
				return;
			}
			if (selectedNode.Parent == this.m_ResourceList.Nodes[1])
			{
				if (selectedNode.Tag == null)
				{
					return;
				}
				try
				{
					IMsRdpDriveV2 msRdpDriveV = selectedNode.Tag as IMsRdpDriveV2;
					int num = 1 << (int)msRdpDriveV.DriveLetterIndex();
					foreach (object obj in this.m_ResourceList.Nodes[3].Nodes)
					{
						TreeNode treeNode = (TreeNode)obj;
						if (treeNode.Checked)
						{
							IMsRdpDeviceV2 msRdpDeviceV = treeNode.Tag as IMsRdpDeviceV2;
							if (msRdpDeviceV.IsUSBDevice())
							{
								int num2 = (int)msRdpDeviceV.DriveLetterBitmap();
								if ((num & num2) != 0)
								{
									treeNode.Checked = false;
									this.UpdateParent(treeNode);
								}
							}
						}
					}
					return;
				}
				catch (Exception ex)
				{
					VMTrace.TraceError("Exception occured while trying to resolve conflict for the drive.", ex);
					return;
				}
			}
			if (selectedNode.Parent == this.m_ResourceList.Nodes[3])
			{
				try
				{
					int num2 = (int)(selectedNode.Tag as IMsRdpDeviceV2).DriveLetterBitmap();
					foreach (object obj2 in this.m_ResourceList.Nodes[1].Nodes)
					{
						TreeNode treeNode2 = (TreeNode)obj2;
						if (treeNode2.Checked && treeNode2.Tag != null)
						{
							IMsRdpDriveV2 msRdpDriveV2 = treeNode2.Tag as IMsRdpDriveV2;
							int num = 1 << (int)msRdpDriveV2.DriveLetterIndex();
							if ((num & num2) != 0)
							{
								treeNode2.Checked = false;
								this.UpdateParent(treeNode2);
							}
						}
					}
				}
				catch (Exception ex2)
				{
					VMTrace.TraceError("Exception occured while trying to resolve conflict for the USB device.", ex2);
				}
			}
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00009800 File Offset: 0x00007A00
		private void SaveAll()
		{
			this.m_RdpOptions.SmartCardsRedirection = this.m_ResourceList.Nodes[0].Checked;
			StringCollection stringCollection = new StringCollection();
			if (!this.m_ResourceList.Nodes[1].Checked)
			{
				IEnumerator enumerator = this.m_ResourceList.Nodes[1].Nodes.GetEnumerator();
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						TreeNode treeNode = (TreeNode)obj;
						if (treeNode.Checked && !stringCollection.Contains(treeNode.Name))
						{
							stringCollection.Add(treeNode.Name);
						}
					}
					goto IL_BA;
				}
			}
			stringCollection.Add(RdpOptions.WildcardAllDevices);
			IL_BA:
			this.m_RdpOptions.SetRedirectedDeviceCollection(RdpOptions.RedirectedDeviceType.Drive, stringCollection);
			StringCollection stringCollection2 = new StringCollection();
			StringCollection stringCollection3 = new StringCollection();
			if (this.m_UsbNodeExists)
			{
				if (!this.m_ResourceList.Nodes[3].Checked)
				{
					IEnumerator enumerator = this.m_ResourceList.Nodes[3].Nodes.GetEnumerator();
					{
						while (enumerator.MoveNext())
						{
							object obj2 = enumerator.Current;
							TreeNode treeNode2 = (TreeNode)obj2;
							if (treeNode2.Checked && !stringCollection2.Contains(treeNode2.Name))
							{
								stringCollection2.Add(treeNode2.Name);
							}
						}
						goto IL_171;
					}
				}
				stringCollection2.Add(RdpOptions.WildcardAllDevices);
			}
			IL_171:
			if (!this.m_ResourceList.Nodes[2].Checked)
			{
				IEnumerator enumerator = this.m_ResourceList.Nodes[2].Nodes.GetEnumerator();
				while (enumerator.MoveNext())
				{
					object obj3 = enumerator.Current;
					TreeNode treeNode3 = (TreeNode)obj3;
					if (treeNode3.Checked && !stringCollection3.Contains(treeNode3.Name))
					{
						stringCollection3.Add(treeNode3.Name);
					}
				}
				goto IL_204;
			}
			stringCollection3.Add(RdpOptions.WildcardAllDevices);
			IL_204:
			this.m_RdpOptions.SetRedirectedDeviceCollection(RdpOptions.RedirectedDeviceType.Usb, stringCollection2);
			this.m_RdpOptions.SetRedirectedDeviceCollection(RdpOptions.RedirectedDeviceType.Pnp, stringCollection3);
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00009A54 File Offset: 0x00007C54
		private void m_OkButton_Click(object sender, EventArgs e)
		{
			this.SaveAll();
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00009A5C File Offset: 0x00007C5C
		private string GetDeviceFriendlyName(IMsRdpDeviceV2 device)
		{
			try
			{
				string text = device.FriendlyName();
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			catch
			{
			}
			try
			{
				string text = device.DeviceText();
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			catch
			{
			}
			try
			{
				string text = device.DeviceDescription();
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x04000096 RID: 150
		private const int Smartcard = 0;

		// Token: 0x04000097 RID: 151
		private const int DriveNode = 1;

		// Token: 0x04000098 RID: 152
		private const int PnpNode = 2;

		// Token: 0x04000099 RID: 153
		private const int UsbNode = 3;

		// Token: 0x0400009A RID: 154
		private RdpOptions m_RdpOptions;

		// Token: 0x0400009B RID: 155
		private NativeWindowDoubleClickInterceptor m_WindowInterceptors;

		// Token: 0x0400009C RID: 156
		private bool m_UsbNodeExists;
	}
}
