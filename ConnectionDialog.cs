using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Virtualization.Client.Common;
using Microsoft.Virtualization.Client.InteractiveSession.Resources;
using Microsoft.Virtualization.Client.Management;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000006 RID: 6
	internal partial class ConnectionDialog : Form
	{
		// Token: 0x06000022 RID: 34 RVA: 0x00002F20 File Offset: 0x00001120
		public ConnectionDialog()
		{
			this.InitializeComponent();
			this.gm_DefaultHeight = DpiUtilities.LogicalToSystemUnits(this.gm_DefaultHeight);
			base.Height = this.gm_DefaultHeight;
			this.m_ProgressAnimationTimer.Interval = 100;
			this.m_ProgressAnimationTimer.Tick += delegate(object o, EventArgs e)
			{
				this.UpdateProgressAnimation();
			};
			this.m_ConnectAsCheckboxFormat = this.m_ConnectAsCheckbox.Text;
			this.m_ConnectAsCheckbox.Text = string.Format(CultureInfo.CurrentCulture, this.m_ConnectAsCheckboxFormat, VMISResources.ConnectionDialog_ConnectAsNone);
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000024 RID: 36 RVA: 0x0000301A File Offset: 0x0000121A
		public RdpConnectionInfo RdpConnectionInfo
		{
			get
			{
				if (base.DialogResult != DialogResult.OK)
				{
					throw new InvalidOperationException();
				}
				return this.m_RdpConnectionInfo;
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00003034 File Offset: 0x00001234
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.m_Displayer = new InformationDisplayer(base.Handle, Program.Displayer.DefaultTitle);
			this.SetServerNameAutoCompletionList();
			if (this.m_ServerComboBox.Text.Trim() != string.Empty)
			{
				this.LoadSavedCredential(this.m_ServerComboBox.Text);
			}
			this.UpdateConnectAsCheckboxState();
			this.UpdateOKButtonState();
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000030A2 File Offset: 0x000012A2
		private void OnServerComboBoxTextChanged(object sender, EventArgs e)
		{
			this.UpdateConnectAsCheckboxState();
			this.UpdateOKButtonState();
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000030B0 File Offset: 0x000012B0
		private void OnMachineComboBoxTextChanged(object sender, EventArgs e)
		{
			this.UpdateOKButtonState();
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000030B8 File Offset: 0x000012B8
		private void OnOkClicked(object sender, EventArgs ea)
		{
			string serverName = this.m_ServerComboBox.Text.Trim();
			string machineName = this.m_MachineComboBox.Text.Trim();
			foreach (object obj in base.Controls)
			{
				Control control = (Control)obj;
				if (!(control is Panel) && control != this.m_CancelButton)
				{
					control.Enabled = false;
				}
			}
			this.EnableAnimateProgress(true);
			this.m_PendingConnectionState = new ConnectionDialog.EstablishVMConnectionState
			{
				ServerName = serverName,
				MachineName = machineName,
				Credential = (this.m_ConnectAsCheckbox.Checked ? this.m_ConnectAsCredential : null),
				SaveCredential = this.m_SaveCredential
			};
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.EstablishVMConnection), this.m_PendingConnectionState);
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000031A8 File Offset: 0x000013A8
		private void OnCancelClicked(object sender, EventArgs ea)
		{
			if (this.m_PendingConnectionState == null)
			{
				base.DialogResult = DialogResult.Cancel;
				base.Close();
				return;
			}
			this.m_PendingConnectionState.CancelConnection();
			this.EstablishVMConnectionCompleted(null);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000031D4 File Offset: 0x000013D4
		private void OnMachineComboBoxEnter(object sender, EventArgs e)
		{
			string b = this.m_ServerComboBox.Text.Trim();
			if (this.m_MachineComboBox.Items.Count > 0 && !string.Equals(this.m_CurrentServerRetrieved, b, StringComparison.OrdinalIgnoreCase))
			{
				this.m_MachineComboBox.Items.Clear();
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00003224 File Offset: 0x00001424
		private void OnMachineComboBoxDropDown(object sender, EventArgs e)
		{
			string value = this.m_ServerComboBox.Text.Trim();
			if (this.m_MachineComboBox.Items.Count == 0 && !string.IsNullOrEmpty(value))
			{
				this.m_MachineComboBox.Items.Add(ConnectionResources.LoadingVMs);
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00003274 File Offset: 0x00001474
		private void OnMachineComboBoxDropDownClosed(object sender, EventArgs e)
		{
			this.m_MachineComboBoxTextInfo.m_Text = this.m_MachineComboBox.Text;
			this.m_MachineComboBoxTextInfo.m_SelectionStart = this.m_MachineComboBox.SelectionStart;
			this.m_MachineComboBoxTextInfo.m_SelectionLength = this.m_MachineComboBox.SelectionLength;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x000032C4 File Offset: 0x000014C4
		private void OnMachineComboBoxSelectedIndexChanged(object sender, EventArgs ea)
		{
			string a = (string)this.m_MachineComboBox.SelectedItem;
			if (a == ConnectionResources.LoadingVMs || a == ConnectionResources.LoadingVMsFailed || a == ConnectionResources.NoVmsFound)
			{
				this.m_MachineComboBox.SelectedIndex = -1;
				base.BeginInvoke(new ConnectionDialog.ResetTextBoxCallback(this.ResetTextBox), new object[]
				{
					this.m_MachineComboBox,
					this.m_MachineComboBoxTextInfo
				});
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00003340 File Offset: 0x00001540
		private void OnServerNameSelectedIndexChanged(object sender, EventArgs ea)
		{
			if (this.m_ServerComboBox.SelectedIndex == this.m_ServerComboBox.Items.Count - 1)
			{
				this.m_ServerComboBox.SelectedIndex = -1;
				base.BeginInvoke(new ConnectionDialog.ResetTextBoxCallback(this.ResetTextBox), new object[]
				{
					this.m_ServerComboBox,
					this.m_ServerComboBoxTextInfo
				});
				base.BeginInvoke(new MethodInvoker(this.BrowseForServerName));
				return;
			}
			string serverName = (string)this.m_ServerComboBox.SelectedItem;
			this.LoadSavedCredential(serverName);
			this.RetrieveVMNameListAsync(serverName);
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000033D8 File Offset: 0x000015D8
		private void OnServerComboBoxDropDownClosed(object sender, EventArgs e)
		{
			this.m_ServerComboBoxTextInfo.m_Text = this.m_ServerComboBox.Text;
			this.m_ServerComboBoxTextInfo.m_SelectionStart = this.m_ServerComboBox.SelectionStart;
			this.m_ServerComboBoxTextInfo.m_SelectionLength = this.m_ServerComboBox.SelectionLength;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00003427 File Offset: 0x00001627
		private void OnServerComboBoxLeave(object sender, EventArgs e)
		{
			if (this.m_ServerComboBox.Text != ConnectionResources.BrowseForMore)
			{
				this.LoadSavedCredential(this.m_ServerComboBox.Text);
				this.RetrieveVMNameListAsync(this.m_ServerComboBox.Text.Trim());
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003468 File Offset: 0x00001668
		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			if (base.DialogResult == DialogResult.OK)
			{
				string text = this.m_ServerComboBox.Text.Trim();
				int num = -1;
				for (int i = 0; i < this.m_ServerComboBox.Items.Count; i++)
				{
					if (string.Equals(text, (string)this.m_ServerComboBox.Items[i], StringComparison.OrdinalIgnoreCase))
					{
						num = i;
						break;
					}
				}
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append(text);
				int num2 = 0;
				while (num2 < 10 && num2 < this.m_ServerComboBox.Items.Count - 1)
				{
					if (num2 != num)
					{
						stringBuilder.Append(";");
						stringBuilder.Append(this.m_ServerComboBox.Items[num2]);
					}
					num2++;
				}
				InteractiveSessionConfigurationOptions.Instance.ConnectionDialogServers = stringBuilder.ToString();
			}
			this.EnableAnimateProgress(false);
			base.OnFormClosed(e);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003554 File Offset: 0x00001754
		private void OnConnectAsCheckboxCheckedChanged(object sender, EventArgs e)
		{
			this.m_ConnectAs = this.m_ConnectAsCheckbox.Checked;
			this.m_SetUserButton.Enabled = this.m_ConnectAs;
			string text = this.m_ServerComboBox.Text.Trim();
			if (text != string.Empty)
			{
				this.RetrieveVMNameListAsync(text);
			}
			this.UpdateOKButtonState();
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000035B0 File Offset: 0x000017B0
		private void OnSetUserButtonClick(object sender, EventArgs e)
		{
			bool flag = false;
			WindowsCredential connectAsCredential = null;
			bool saveCredential = false;
			int num = 0;
			do
			{
				try
				{
					flag = WindowsCredential.PromptForWindowsCredentials(base.Handle, num, VMISResources.ConnectionDialog_PromptForCredentialsCaption, VMISResources.ConnectionDialog_PromptForCredentialsMessage, true, out connectAsCredential, out saveCredential);
					num = 0;
				}
				catch (InvalidCredentialException ex)
				{
					this.m_Displayer.DisplayError(VMISResources.ConnectionDialog_InvalidUserName_Title, ex.Message, VMISResources.ConnectionDialog_InvalidUserName_Detail);
					num = ex.HResult;
				}
			}
			while (num != 0);
			if (flag)
			{
				this.m_ConnectAsCredential = connectAsCredential;
				this.m_SaveCredential = saveCredential;
				this.m_ConnectAsCheckbox.Text = string.Format(CultureInfo.CurrentCulture, this.m_ConnectAsCheckboxFormat, this.m_ConnectAsCredential.LogonName);
				if (this.m_ServerComboBox.Text != string.Empty)
				{
					this.RetrieveVMNameListAsync(this.m_ServerComboBox.Text);
				}
				this.UpdateOKButtonState();
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00003688 File Offset: 0x00001888
		private void UpdateOKButtonState()
		{
			bool flag = this.m_ServerComboBox.Text.Trim() == string.Empty;
			bool flag2 = this.m_MachineComboBox.Text.Trim() == string.Empty;
			bool flag3 = !this.m_ConnectAsCheckbox.Checked || this.m_ConnectAsCredential != null;
			this.m_OkButton.Enabled = (!flag && !flag2 && flag3);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003700 File Offset: 0x00001900
		private void UpdateConnectAsCheckboxState()
		{
			string text = this.m_ServerComboBox.Text.Trim();
			bool flag = !string.IsNullOrEmpty(text) && !Server.IsLocalhostName(text);
			this.m_ConnectAsCheckbox.Enabled = flag;
			if (!flag)
			{
				this.m_ConnectAsCheckbox.Checked = false;
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003750 File Offset: 0x00001950
		private void SetServerNameAutoCompletionList()
		{
			string[] array = InteractiveSessionConfigurationOptions.Instance.ConnectionDialogServers.Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			int num = Math.Min(10, array.Length);
			string[] array2 = new string[num + 2];
			Array.Copy(array, 0, array2, 0, num);
			array2[array2.Length - 2] = "localhost";
			array2[array2.Length - 1] = Environment.MachineName;
			List<string> list = new List<string>(array.Length);
			AutoCompleteStringCollection autoCompleteStringCollection = new AutoCompleteStringCollection();
			foreach (string text in array2)
			{
				bool flag = true;
				using (List<string>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (string.Equals(enumerator.Current, text, StringComparison.OrdinalIgnoreCase))
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					list.Add(text);
				}
			}
			foreach (string text2 in list)
			{
				this.m_ServerComboBox.Items.Add(text2);
				autoCompleteStringCollection.Add(text2);
			}
			this.m_ServerComboBox.Items.Add(ConnectionResources.BrowseForMore);
			this.m_ServerComboBox.AutoCompleteCustomSource = autoCompleteStringCollection;
			if (list.Count > 0)
			{
				this.m_ServerComboBox.SelectedIndex = 0;
			}
			if (this.m_ServerComboBox.SelectedIndex != -1)
			{
				this.RetrieveVMNameListAsync(this.m_ServerComboBox.Items[this.m_ServerComboBox.SelectedIndex].ToString());
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000038F8 File Offset: 0x00001AF8
		private void BrowseForServerName()
		{
			string text = ADComputerBrowse.BrowseForComputer(this);
			if (!string.IsNullOrEmpty(text))
			{
				this.m_ServerComboBox.Text = text;
				this.RetrieveVMNameListAsync(text);
				return;
			}
			this.ResetTextBox(this.m_ServerComboBox, this.m_ServerComboBoxTextInfo);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x0000393C File Offset: 0x00001B3C
		private void EstablishVMConnection(object obj)
		{
			try
			{
				ConnectionDialog.EstablishVMConnectionState establishVMConnectionState = (ConnectionDialog.EstablishVMConnectionState)obj;
				IVMComputerSystem ivmcomputerSystem;
				Exception error;
				bool flag = ConnectionHelper.TryGetVirtualMachine(establishVMConnectionState.ServerName, establishVMConnectionState.Credential, establishVMConnectionState.MachineName, out ivmcomputerSystem, out error);
				if (flag)
				{
					establishVMConnectionState.ServerConnectionName = ivmcomputerSystem.Server.FullName;
				}
				if (flag)
				{
					try
					{
						ITerminalServiceSetting terminalServiceSetting = ObjectLocator.GetTerminalServiceSetting(ivmcomputerSystem.Server);
						establishVMConnectionState.RdpPort = terminalServiceSetting.ListenerPort;
					}
					catch (VirtualizationManagementException ex)
					{
						VMTrace.TraceError("Error finding the WMI terminal service object!", ex);
						ivmcomputerSystem = null;
						flag = false;
						error = new VirtualizationManagementException(VMISResources.Error_CouldNotDetermineRdpPort, ex);
					}
				}
				if (flag)
				{
					try
					{
						ivmcomputerSystem.RegisterForInstanceModificationEvents(InstanceModificationEventStrategy.InstanceModificationEvent);
						ivmcomputerSystem.UpdatePropertyCache();
					}
					catch (VirtualizationManagementException ex2)
					{
						Program.TasksToCompleteBeforeExitingTracker.RegisterBackgroundTask(new WaitCallback(this.CancelImeEvents), new object[]
						{
							ivmcomputerSystem
						});
						ivmcomputerSystem = null;
						flag = false;
						error = ex2;
					}
				}
				establishVMConnectionState.Succeeded = flag;
				establishVMConnectionState.Error = error;
				establishVMConnectionState.VMConnection = ivmcomputerSystem;
				try
				{
					if (!establishVMConnectionState.Canceled)
					{
						base.BeginInvoke(new WaitCallback(this.EstablishVMConnectionCompleted), new object[]
						{
							establishVMConnectionState
						});
					}
					else if (flag)
					{
						Program.TasksToCompleteBeforeExitingTracker.RegisterBackgroundTask(new WaitCallback(this.CancelImeEvents), new object[]
						{
							ivmcomputerSystem
						});
					}
					if (establishVMConnectionState.Succeeded && establishVMConnectionState.SaveCredential && establishVMConnectionState.Credential != null)
					{
						string vmiscredentialName = CommonUtilities.GetVMISCredentialName(establishVMConnectionState.ServerConnectionName ?? establishVMConnectionState.ServerName);
						establishVMConnectionState.Credential.Save(vmiscredentialName);
					}
				}
				catch (InvalidOperationException)
				{
				}
			}
			catch (Exception ex3)
			{
				Program.UnhandledExceptionHandler.HandleBackgroundThreadException(ex3);
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003B1C File Offset: 0x00001D1C
		private void EstablishVMConnectionCompleted(object ignored)
		{
			ConnectionDialog.EstablishVMConnectionState pendingConnectionState = this.m_PendingConnectionState;
			this.m_PendingConnectionState = null;
			if (pendingConnectionState != null)
			{
				this.EnableAnimateProgress(false);
				if (!pendingConnectionState.Canceled)
				{
					if (pendingConnectionState.Succeeded)
					{
						this.m_RdpConnectionInfo = new RdpConnectionInfo
						{
							ServerConnectionName = pendingConnectionState.ServerConnectionName,
							VirtualMachine = pendingConnectionState.VMConnection,
							RdpPort = pendingConnectionState.RdpPort,
							Credential = this.m_ConnectAsCredential
						};
						base.DialogResult = DialogResult.OK;
						base.Close();
						return;
					}
					Exception exception = null;
					string mainInstruction;
					if (pendingConnectionState.Error is ObjectNotFoundException)
					{
						mainInstruction = pendingConnectionState.Error.Message;
					}
					else
					{
						mainInstruction = string.Format(CultureInfo.CurrentCulture, CommonResources.ErrorFindVm, pendingConnectionState.MachineName, pendingConnectionState.ServerName);
						exception = pendingConnectionState.Error;
					}
					this.m_Displayer.DisplayError(mainInstruction, exception);
					IEnumerator enumerator = base.Controls.GetEnumerator();
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							((Control)obj).Enabled = true;
						}
						return;
					}
				}
				foreach (object obj2 in base.Controls)
				{
					((Control)obj2).Enabled = true;
				}
				if (pendingConnectionState.Succeeded)
				{
					Program.TasksToCompleteBeforeExitingTracker.RegisterBackgroundTask(new WaitCallback(this.CancelImeEvents), new object[]
					{
						pendingConnectionState.VMConnection
					});
				}
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003CB0 File Offset: 0x00001EB0
		private void CancelImeEvents(object vmObj)
		{
			IVMComputerSystem ivmcomputerSystem = vmObj as IVMComputerSystem;
			try
			{
				if (ivmcomputerSystem != null)
				{
					ivmcomputerSystem.UnregisterForInstanceModificationEvents();
				}
			}
			catch (VirtualizationManagementException ex)
			{
				VMTrace.TraceWarning("Unable to unregistering for the vm's IME events.", ex);
			}
			catch (Exception ex2)
			{
				Program.UnhandledExceptionHandler.HandleBackgroundThreadException(ex2);
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003D08 File Offset: 0x00001F08
		private void ResetTextBox(ComboBox textBox, ConnectionDialog.TextBoxInformation textBoxInfo)
		{
			textBox.Text = textBoxInfo.m_Text;
			textBox.SelectionStart = textBoxInfo.m_SelectionStart;
			textBox.SelectionLength = textBoxInfo.m_SelectionLength;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003D30 File Offset: 0x00001F30
		private void RetrieveVMNameListAsync(string serverName)
		{
			if (string.IsNullOrEmpty(serverName))
			{
				return;
			}
			bool flag = false;
			List<string> serverAsyncList = this.m_ServerAsyncList;
			lock (serverAsyncList)
			{
				using (List<string>.Enumerator enumerator = this.m_ServerAsyncList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (string.Equals(enumerator.Current, serverName, StringComparison.OrdinalIgnoreCase))
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					this.m_ServerAsyncList.Add(serverName);
					this.EnableAnimateProgress(true);
					ConnectionDialog.VMNameListInfo state = new ConnectionDialog.VMNameListInfo
					{
						ServerName = serverName,
						ConnectAsUser = this.m_ConnectAs,
						Credential = this.m_ConnectAsCredential
					};
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.RetrieveVMNameList), state);
				}
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003E0C File Offset: 0x0000200C
		private void RetrieveVMNameList(object param)
		{
			ConnectionDialog.VMNameListInfo vmnameListInfo = (ConnectionDialog.VMNameListInfo)param;
			try
			{
				ConnectionDialog.VMNameAutoCompletionInfo vmnameAutoCompletionInfo = new ConnectionDialog.VMNameAutoCompletionInfo();
				vmnameAutoCompletionInfo.m_ServerName = vmnameListInfo.ServerName;
				List<string> list;
				Exception error;
				if (ConnectionHelper.TryGetVirtualMachines(vmnameListInfo.ServerName, vmnameListInfo.ConnectAsUser ? vmnameListInfo.Credential : null, out list, out error))
				{
					list.Sort(new NameWithTrailingNumberSorter(true));
					using (List<string>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							string value = enumerator.Current;
							vmnameAutoCompletionInfo.m_VirtualMachineNames.Add(value);
						}
						goto IL_88;
					}
				}
				vmnameAutoCompletionInfo.m_Error = error;
				IL_88:
				try
				{
					base.BeginInvoke(new ConnectionDialog.SetVMNameComboBoxInvoker(this.SetVMNameComboBox), new object[]
					{
						vmnameAutoCompletionInfo
					});
				}
				catch (InvalidOperationException)
				{
				}
			}
			catch (Exception ex)
			{
				Program.UnhandledExceptionHandler.HandleBackgroundThreadException(ex);
			}
			finally
			{
				List<string> serverAsyncList = this.m_ServerAsyncList;
				lock (serverAsyncList)
				{
					this.m_ServerAsyncList.Remove(vmnameListInfo.ServerName);
				}
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003F4C File Offset: 0x0000214C
		private void SetVMNameComboBox(ConnectionDialog.VMNameAutoCompletionInfo vmCompletionInfo)
		{
			string text = this.m_ServerComboBox.Text.Trim();
			if (!base.IsDisposed && string.Equals(vmCompletionInfo.m_ServerName, text, StringComparison.OrdinalIgnoreCase))
			{
				this.m_CurrentServerRetrieved = text;
				this.EnableAnimateProgress(false);
				this.m_MachineComboBox.BeginUpdate();
				if (vmCompletionInfo.m_Error == null)
				{
					if (vmCompletionInfo.m_VirtualMachineNames.Count > 0)
					{
						int num = 0;
						foreach (object obj in vmCompletionInfo.m_VirtualMachineNames)
						{
							string text2 = (string)obj;
							if (this.m_MachineComboBox.Items.Count > num)
							{
								this.m_MachineComboBox.Items[num] = text2;
							}
							else
							{
								this.m_MachineComboBox.Items.Add(text2);
							}
							if (string.Equals(this.m_MachineComboBox.Text, text2, StringComparison.OrdinalIgnoreCase))
							{
								this.m_MachineComboBox.SelectedIndex = num;
							}
							num++;
						}
						for (int i = this.m_MachineComboBox.Items.Count - 1; i >= num; i--)
						{
							this.m_MachineComboBox.Items.RemoveAt(i);
						}
						this.m_MessageTableLayoutPanel.Visible = false;
						AnimatedFormResizer.Resize(this, new Size(base.Width, this.gm_DefaultHeight), TimeSpan.FromMilliseconds(75.0), null, AnimationMode.Linear);
					}
					else
					{
						if (this.m_MachineComboBox.Items.Count == 0)
						{
							this.m_MachineComboBox.Items.Add(ConnectionResources.NoVmsFound);
						}
						else
						{
							this.m_MachineComboBox.Items[0] = ConnectionResources.NoVmsFound;
							for (int j = this.m_MachineComboBox.Items.Count - 1; j > 0; j--)
							{
								this.m_MachineComboBox.Items.RemoveAt(j);
							}
						}
						this.DisplayInlineMessage(null);
					}
				}
				else
				{
					if (this.m_MachineComboBox.Items.Count == 0)
					{
						this.m_MachineComboBox.Items.Add(ConnectionResources.LoadingVMsFailed);
					}
					else
					{
						this.m_MachineComboBox.Items[0] = ConnectionResources.LoadingVMsFailed;
						for (int k = this.m_MachineComboBox.Items.Count - 1; k > 0; k--)
						{
							this.m_MachineComboBox.Items.RemoveAt(k);
						}
					}
					this.DisplayInlineMessage(vmCompletionInfo.m_Error);
				}
				this.m_MachineComboBox.EndUpdate();
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000041D4 File Offset: 0x000023D4
		private void DisplayInlineMessage(Exception exception)
		{
			this.m_MessageTableLayoutPanel.Visible = false;
			if (exception != null)
			{
				this.m_WarningPictureBox.Image = ImageResources.WarningSmallImage;
				this.m_MessageLabel.Text = exception.Message;
			}
			else
			{
				this.m_WarningPictureBox.Image = ImageResources.InformationSmallImage;
				this.m_MessageLabel.Text = CommonResources.InformationCouldNotEnumerateVms_CheckPermissions;
			}
			Size endSize = new Size(base.Width, this.gm_DefaultHeight + this.m_MessageTableLayoutPanel.Height + this.m_MessageTableLayoutPanel.Margin.Vertical);
			AnimatedFormResizer.Resize(this, endSize, TimeSpan.FromMilliseconds(75.0), new Action(this.OnFormResizeCompleted), AnimationMode.Linear);
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00004288 File Offset: 0x00002488
		private void OnFormResizeCompleted()
		{
			if (base.Height != this.gm_DefaultHeight)
			{
				this.m_MessageTableLayoutPanel.Visible = true;
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x000042A4 File Offset: 0x000024A4
		private void UpdateProgressAnimation()
		{
			this.m_ProgressAnimationStage -= 15;
			if (this.m_ProgressAnimationStage < 0)
			{
				this.m_ProgressAnimationStage += this.m_ProgressPanel.Width;
			}
			Point location = new Point(-this.m_ProgressAnimationStage, this.m_ProgressPanelLeft.Location.Y);
			this.m_ProgressPanelLeft.Location = location;
			location = new Point(this.m_ProgressPanelRight.Width - this.m_ProgressAnimationStage, this.m_ProgressPanelRight.Location.Y);
			this.m_ProgressPanelRight.Location = location;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00004346 File Offset: 0x00002546
		private void EnableAnimateProgress(bool enableAnimation)
		{
			if (enableAnimation)
			{
				this.m_ProgressAnimationTimer.Start();
				return;
			}
			this.m_ProgressAnimationTimer.Stop();
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00004364 File Offset: 0x00002564
		private void LoadSavedCredential(string serverName)
		{
			ServerNames serverNames = ServerNames.Resolve(serverName, null);
			WindowsCredential connectAsCredential;
			if (WindowsCredential.Load(CommonUtilities.GetVMISCredentialName(serverNames.HasFullName ? serverNames.FullName : serverNames.NetBiosName), out connectAsCredential))
			{
				this.m_ConnectAsCredential = connectAsCredential;
				this.m_ConnectAsCheckbox.Text = string.Format(CultureInfo.CurrentCulture, this.m_ConnectAsCheckboxFormat, this.m_ConnectAsCredential.LogonName);
				this.m_ConnectAsCheckbox.Checked = true;
				this.m_ConnectAs = true;
				return;
			}
			if (this.m_ConnectAsCredential == null)
			{
				this.m_ConnectAsCheckbox.Text = string.Format(CultureInfo.CurrentCulture, this.m_ConnectAsCheckboxFormat, VMISResources.ConnectionDialog_ConnectAsNone);
			}
			this.m_ConnectAsCheckbox.Checked = false;
			this.m_ConnectAs = false;
		}

		// Token: 0x04000019 RID: 25
		private const int gm_StoredServerCountCap = 10;

		// Token: 0x0400001A RID: 26
		private readonly int gm_DefaultHeight = 250;

		// Token: 0x0400001B RID: 27
		private ConnectionDialog.TextBoxInformation m_MachineComboBoxTextInfo = new ConnectionDialog.TextBoxInformation();

		// Token: 0x0400001C RID: 28
		private ConnectionDialog.TextBoxInformation m_ServerComboBoxTextInfo = new ConnectionDialog.TextBoxInformation();

		// Token: 0x0400001D RID: 29
		private string m_CurrentServerRetrieved;

		// Token: 0x0400001E RID: 30
		private RdpConnectionInfo m_RdpConnectionInfo;

		// Token: 0x0400001F RID: 31
		private ConnectionDialog.EstablishVMConnectionState m_PendingConnectionState;

		// Token: 0x04000021 RID: 33
		private int m_ProgressAnimationStage;

		// Token: 0x04000022 RID: 34
		private List<string> m_ServerAsyncList = new List<string>();

		// Token: 0x04000023 RID: 35
		private InformationDisplayer m_Displayer;

		// Token: 0x04000024 RID: 36
		private string m_ConnectAsCheckboxFormat;

		// Token: 0x04000025 RID: 37
		private bool m_ConnectAs;

		// Token: 0x04000026 RID: 38
		private WindowsCredential m_ConnectAsCredential;

		// Token: 0x04000027 RID: 39
		private bool m_SaveCredential;

		// Token: 0x02000039 RID: 57
		private class EstablishVMConnectionState
		{
			// Token: 0x17000176 RID: 374
			// (get) Token: 0x0600039F RID: 927 RVA: 0x000154DD File Offset: 0x000136DD
			// (set) Token: 0x060003A0 RID: 928 RVA: 0x000154E5 File Offset: 0x000136E5
			public string ServerName { get; set; }

			// Token: 0x17000177 RID: 375
			// (get) Token: 0x060003A1 RID: 929 RVA: 0x000154EE File Offset: 0x000136EE
			// (set) Token: 0x060003A2 RID: 930 RVA: 0x000154F6 File Offset: 0x000136F6
			public string MachineName { get; set; }

			// Token: 0x17000178 RID: 376
			// (get) Token: 0x060003A3 RID: 931 RVA: 0x000154FF File Offset: 0x000136FF
			// (set) Token: 0x060003A4 RID: 932 RVA: 0x00015507 File Offset: 0x00013707
			public bool Canceled { get; set; }

			// Token: 0x17000179 RID: 377
			// (get) Token: 0x060003A5 RID: 933 RVA: 0x00015510 File Offset: 0x00013710
			// (set) Token: 0x060003A6 RID: 934 RVA: 0x00015518 File Offset: 0x00013718
			public bool Succeeded { get; set; }

			// Token: 0x1700017A RID: 378
			// (get) Token: 0x060003A7 RID: 935 RVA: 0x00015521 File Offset: 0x00013721
			// (set) Token: 0x060003A8 RID: 936 RVA: 0x00015529 File Offset: 0x00013729
			public Exception Error { get; set; }

			// Token: 0x1700017B RID: 379
			// (get) Token: 0x060003A9 RID: 937 RVA: 0x00015532 File Offset: 0x00013732
			// (set) Token: 0x060003AA RID: 938 RVA: 0x0001553A File Offset: 0x0001373A
			public IVMComputerSystem VMConnection { get; set; }

			// Token: 0x1700017C RID: 380
			// (get) Token: 0x060003AB RID: 939 RVA: 0x00015543 File Offset: 0x00013743
			// (set) Token: 0x060003AC RID: 940 RVA: 0x0001554B File Offset: 0x0001374B
			public int RdpPort { get; set; }

			// Token: 0x1700017D RID: 381
			// (get) Token: 0x060003AD RID: 941 RVA: 0x00015554 File Offset: 0x00013754
			// (set) Token: 0x060003AE RID: 942 RVA: 0x0001555C File Offset: 0x0001375C
			public string ServerConnectionName { get; set; }

			// Token: 0x1700017E RID: 382
			// (get) Token: 0x060003AF RID: 943 RVA: 0x00015565 File Offset: 0x00013765
			// (set) Token: 0x060003B0 RID: 944 RVA: 0x0001556D File Offset: 0x0001376D
			public WindowsCredential Credential { get; set; }

			// Token: 0x1700017F RID: 383
			// (get) Token: 0x060003B1 RID: 945 RVA: 0x00015576 File Offset: 0x00013776
			// (set) Token: 0x060003B2 RID: 946 RVA: 0x0001557E File Offset: 0x0001377E
			public bool SaveCredential { get; set; }

			// Token: 0x060003B3 RID: 947 RVA: 0x00015587 File Offset: 0x00013787
			public void CancelConnection()
			{
				this.Canceled = true;
			}
		}

		// Token: 0x0200003A RID: 58
		private class TextBoxInformation
		{
			// Token: 0x040001A1 RID: 417
			public string m_Text;

			// Token: 0x040001A2 RID: 418
			public int m_SelectionStart;

			// Token: 0x040001A3 RID: 419
			public int m_SelectionLength;
		}

		// Token: 0x0200003B RID: 59
		private class VMNameAutoCompletionInfo
		{
			// Token: 0x040001A4 RID: 420
			public string m_ServerName;

			// Token: 0x040001A5 RID: 421
			public AutoCompleteStringCollection m_VirtualMachineNames = new AutoCompleteStringCollection();

			// Token: 0x040001A6 RID: 422
			public Exception m_Error;
		}

		// Token: 0x0200003C RID: 60
		private class VMNameListInfo
		{
			// Token: 0x17000180 RID: 384
			// (get) Token: 0x060003B7 RID: 951 RVA: 0x000155B3 File Offset: 0x000137B3
			// (set) Token: 0x060003B8 RID: 952 RVA: 0x000155BB File Offset: 0x000137BB
			public string ServerName { get; set; }

			// Token: 0x17000181 RID: 385
			// (get) Token: 0x060003B9 RID: 953 RVA: 0x000155C4 File Offset: 0x000137C4
			// (set) Token: 0x060003BA RID: 954 RVA: 0x000155CC File Offset: 0x000137CC
			public bool ConnectAsUser { get; set; }

			// Token: 0x17000182 RID: 386
			// (get) Token: 0x060003BB RID: 955 RVA: 0x000155D5 File Offset: 0x000137D5
			// (set) Token: 0x060003BC RID: 956 RVA: 0x000155DD File Offset: 0x000137DD
			public WindowsCredential Credential { get; set; }
		}

		// Token: 0x0200003D RID: 61
		// (Invoke) Token: 0x060003BF RID: 959
		private delegate void ResetTextBoxCallback(ComboBox textBox, ConnectionDialog.TextBoxInformation textBoxInfo);

		// Token: 0x0200003E RID: 62
		// (Invoke) Token: 0x060003C3 RID: 963
		private delegate void SetVMNameComboBoxInvoker(ConnectionDialog.VMNameAutoCompletionInfo vmCompletionInfo);
	}
}
