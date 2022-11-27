using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Virtualization.Client.Management;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000020 RID: 32
	internal class VMConnectUserActionPerformer
	{
		// Token: 0x060001BC RID: 444 RVA: 0x0000EF5C File Offset: 0x0000D15C
		internal VMConnectUserActionPerformer(IMenuActionTarget actionTarget)
		{
			this.m_ActionTarget = actionTarget;
		}

		// Token: 0x14000016 RID: 22
		// (add) Token: 0x060001BD RID: 445 RVA: 0x0000EF6C File Offset: 0x0000D16C
		// (remove) Token: 0x060001BE RID: 446 RVA: 0x0000EFA4 File Offset: 0x0000D1A4
		public event EventHandler SnapshotOperationComplete;

		// Token: 0x060001BF RID: 447 RVA: 0x0000EFDC File Offset: 0x0000D1DC
		internal void OnStateChangeTaskTrigged(VMControlAction action)
		{
			if (action == VMControlAction.None || action == VMControlAction.Snapshot)
			{
				return;
			}
			if (this.m_StateChangeTaskRunning)
			{
				return;
			}
			if (this.m_ActionTarget.VirtualMachine != null)
			{
				VMStateChangeAction vmstateChangeAction = this.MapVMControlActionToVMStateChangeAction(action);
				if (this.AskForUserConfirmation(vmstateChangeAction))
				{
					ClientCreatedTask clientCreatedTask = new ClientCreatedTask(this.m_ActionTarget.AsyncUIThreadMethodInvoker);
					this.m_ActionTarget.InformBeginStateChangeOperation(vmstateChangeAction);
					this.m_StateChangeTaskRunning = true;
					this.DisplayStateChangeProgress(vmstateChangeAction, clientCreatedTask);
					VMStateChangeInvoker vmstateChangeInvoker = new VMStateChangeInvoker(this.m_ActionTarget.AsyncUIThreadMethodInvoker, Program.Displayer);
					vmstateChangeInvoker.Completed += this.HandleStateChangeCompleted;
					vmstateChangeInvoker.PerformAsyncStateChange(vmstateChangeAction, false, this.m_ActionTarget.VirtualMachine, clientCreatedTask);
				}
			}
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000F080 File Offset: 0x0000D280
		internal void OnSnapshotTaskTrigged()
		{
			if (this.m_ActionTarget.VirtualMachine != null)
			{
				ClientCreatedTask task = new ClientCreatedTask(this.m_ActionTarget.AsyncUIThreadMethodInvoker);
				string task_TakeSnapshot_Name = VMISResources.Task_TakeSnapshot_Name;
				string task_TakeSnapshot_Succeeded = VMISResources.Task_TakeSnapshot_Succeeded;
				string task_TakeSnapshot_Failed = VMISResources.Task_TakeSnapshot_Failed;
				string task_TakeSnapshot_Canceled = VMISResources.Task_TakeSnapshot_Canceled;
				this.m_ActionTarget.StatusBar.DisplayTaskProgress(task, task_TakeSnapshot_Name, task_TakeSnapshot_Succeeded, task_TakeSnapshot_Failed, task_TakeSnapshot_Canceled);
				TakeSnapshotInvoker takeSnapshotInvoker = new TakeSnapshotInvoker(this.m_ActionTarget.AsyncUIThreadMethodInvoker);
				takeSnapshotInvoker.Completed += this.HandleSnapshotCompleted;
				takeSnapshotInvoker.BeginAsyncTakeSnapshot(this.m_ActionTarget.VirtualMachine, task, this.m_ActionTarget.DialogOwner, new MethodInvoker(this.m_ActionTarget.DeactivateOnDialogClosed));
			}
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000F12D File Offset: 0x0000D32D
		internal void OnSendSasTaskTriggered()
		{
			if (this.m_ActionTarget.VirtualMachine != null)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.SendSas), this.m_ActionTarget.VirtualMachine);
			}
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000F15C File Offset: 0x0000D35C
		internal void OnShareTaskTriggered()
		{
			if (this.m_ActionTarget.VirtualMachine != null)
			{
				ClientCreatedTask task = new ClientCreatedTask(this.m_ActionTarget.AsyncUIThreadMethodInvoker);
				string task_ShareVM_Name = VMISResources.Task_ShareVM_Name;
				string task_ShareVM_Succeeded = VMISResources.Task_ShareVM_Succeeded;
				string task_ShareVM_Failed = VMISResources.Task_ShareVM_Failed;
				string task_ShareVM_Canceled = VMISResources.Task_ShareVM_Canceled;
				this.m_ActionTarget.StatusBar.DisplayTaskProgress(task, task_ShareVM_Name, task_ShareVM_Succeeded, task_ShareVM_Failed, task_ShareVM_Canceled);
				ExportInvoker exportInvoker = new ExportInvoker(this.m_ActionTarget.AsyncUIThreadMethodInvoker);
				exportInvoker.Completed += this.HandleShareCompleted;
				exportInvoker.BeginAsyncExport(this.m_ActionTarget.VirtualMachine, task, this.m_ActionTarget.DialogOwner);
			}
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000F1F4 File Offset: 0x0000D3F4
		internal void OnEnhancedTriggered()
		{
			this.m_ActionTarget.ToggleEnhanced();
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x0000F201 File Offset: 0x0000D401
		internal void OnZoomLevelChanged(ZoomLevel level)
		{
			this.m_ActionTarget.SetZoomLevel(level);
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000F210 File Offset: 0x0000D410
		private void HandleSnapshotCompleted(object sender, VirtualizationOperationCompleteEventArgs ea)
		{
			if (ea.Status != VMTaskStatus.CompletedSuccessfully)
			{
				string mainInstruction = VMISResources.Error_TakeSnapshotFailed;
				if (ea.Exception is VirtualizationOperationFailedException && ((VirtualizationOperationFailedException)ea.Exception).Operation == VirtualizationOperation.Put)
				{
					mainInstruction = VMISResources.Error_RenameSnapshotFailed;
				}
				Program.Displayer.DisplayError(mainInstruction, ea.Exception);
			}
			else
			{
				TakeSnapshotInvoker takeSnapshotInvoker = sender as TakeSnapshotInvoker;
				if (takeSnapshotInvoker != null)
				{
					if (takeSnapshotInvoker.VirtualMachine.WasOnlineProductionSnapshot())
					{
						Program.Displayer.DisplayInformation(CommonResources.ProductionSnapshotInformation_Title, CommonResources.ProductionSnapshotInformation_Summary, CommonResources.ProductionSnapshotInformation_Details, Confirmations.InformProductionSnapshot);
					}
					else
					{
						int userSnapshotType = (int)this.m_ActionTarget.VirtualMachine.Setting.UserSnapshotType;
						VMComputerSystemState state = this.m_ActionTarget.VirtualMachine.State;
						if (userSnapshotType == 3 && state == VMComputerSystemState.Running)
						{
							Program.Displayer.DisplayInformation(CommonResources.StandardSnapshotInformation_Title, CommonResources.StandardSnapshotInformation_Summary, CommonResources.StandardSnapshotInformation_Details, Confirmations.InformStandardSnapshot);
						}
					}
				}
			}
			if (this.SnapshotOperationComplete != null)
			{
				this.SnapshotOperationComplete(this, EventArgs.Empty);
			}
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000F304 File Offset: 0x0000D504
		private void HandleShareCompleted(object sender, VirtualizationOperationCompleteEventArgs ea)
		{
			ExportInvoker exportInvoker = (ExportInvoker)sender;
			if (ea.Status != VMTaskStatus.CompletedSuccessfully)
			{
				string mainInstruction = string.Format(CultureInfo.CurrentCulture, VMISResources.Error_VMShareFailed, exportInvoker.VirtualMachineName);
				Program.Displayer.DisplayError(mainInstruction, ea.Exception);
			}
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000F348 File Offset: 0x0000D548
		private void HandleStateChangeCompleted(object sender, VirtualizationOperationCompleteEventArgs ea)
		{
			VMStateChangeInvoker vmstateChangeInvoker = (VMStateChangeInvoker)sender;
			this.m_ActionTarget.InformEndStateChangeOperation(vmstateChangeInvoker.StateChange);
			this.m_StateChangeTaskRunning = false;
			if (ea.Status != VMTaskStatus.CompletedSuccessfully)
			{
				string mainInstruction = string.Format(CultureInfo.CurrentCulture, VMISResources.Error_ChangeVMStateFailed, vmstateChangeInvoker.VMName);
				Program.Displayer.DisplayError(mainInstruction, ea.Exception);
			}
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000F3A4 File Offset: 0x0000D5A4
		private void SendSas(object virtualMachineObj)
		{
			IVMComputerSystem virtualMachine = (IVMComputerSystem)virtualMachineObj;
			try
			{
				IVMKeyboard keyboardDeviceForVM = ClipboardMenu.GetKeyboardDeviceForVM(virtualMachine);
				if (keyboardDeviceForVM != null)
				{
					keyboardDeviceForVM.TypeCtrlAltDel();
				}
				else
				{
					this.m_ActionTarget.AsyncUIThreadMethodInvoker(new WaitCallback(this.SendSasFailed), new object[]
					{
						VMISResources.Error_KeyboardDeviceNotFound
					});
				}
			}
			catch (Exception ex)
			{
				VMTrace.TraceError("Error typing Ctrl-Alt-Delete in vm!", ex);
				this.m_ActionTarget.AsyncUIThreadMethodInvoker(new WaitCallback(this.SendSasFailed), new object[]
				{
					ex
				});
			}
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000F43C File Offset: 0x0000D63C
		private void SendSasFailed(object errorObj)
		{
			string error_KeyboardSendKeysFailed = VMISResources.Error_KeyboardSendKeysFailed;
			string text = errorObj as string;
			if (text != null)
			{
				Program.Displayer.DisplayError(error_KeyboardSendKeysFailed, text);
				return;
			}
			Exception exception = errorObj as Exception;
			Program.Displayer.DisplayError(error_KeyboardSendKeysFailed, exception);
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000F47C File Offset: 0x0000D67C
		private void DisplayStateChangeProgress(VMStateChangeAction stateChangeAction, ClientCreatedTask stateChangeTask)
		{
			if (stateChangeAction != VMStateChangeAction.Pause && stateChangeAction != VMStateChangeAction.Resume && stateChangeAction != VMStateChangeAction.Reset)
			{
				string taskName;
				string taskCompleteMessage;
				string taskErrorMessage;
				string taskCanceledMessage;
				if (stateChangeAction == VMStateChangeAction.TurnOn)
				{
					taskName = VMISResources.Task_TurnOn_Name;
					taskCompleteMessage = VMISResources.Task_TurnOn_Succeeded;
					taskErrorMessage = VMISResources.Task_TurnOn_Failed;
					taskCanceledMessage = VMISResources.Task_TurnOn_Canceled;
				}
				else if (stateChangeAction == VMStateChangeAction.Restore)
				{
					taskName = VMISResources.Task_Restore_Name;
					taskCompleteMessage = VMISResources.Task_Restore_Succeeded;
					taskErrorMessage = VMISResources.Task_Restore_Failed;
					taskCanceledMessage = VMISResources.Task_Restore_Canceled;
				}
				else if (stateChangeAction == VMStateChangeAction.Turnoff)
				{
					taskName = VMISResources.Task_TurnOff_Name;
					taskCompleteMessage = VMISResources.Task_TurnOff_Succeeded;
					taskErrorMessage = VMISResources.Task_TurnOff_Failed;
					taskCanceledMessage = VMISResources.Task_TurnOff_Canceled;
				}
				else if (stateChangeAction == VMStateChangeAction.Revert)
				{
					taskName = VMISResources.Task_Revert_Name;
					taskCompleteMessage = VMISResources.Task_Revert_Succeeded;
					taskErrorMessage = VMISResources.Task_Revert_Failed;
					taskCanceledMessage = VMISResources.Task_Revert_Canceled;
				}
				else if (stateChangeAction == VMStateChangeAction.Shutdown)
				{
					taskName = VMISResources.Task_Shutdown_Name;
					taskCompleteMessage = VMISResources.Task_Shutdown_Succeeded;
					taskErrorMessage = VMISResources.Task_Shutdown_Failed;
					taskCanceledMessage = VMISResources.Task_Shutdown_Canceled;
				}
				else
				{
					taskName = VMISResources.Task_SaveState_Name;
					taskCompleteMessage = VMISResources.Task_SaveState_Succeeded;
					taskErrorMessage = VMISResources.Task_SaveState_Failed;
					taskCanceledMessage = VMISResources.Task_SaveState_Canceled;
				}
				this.m_ActionTarget.StatusBar.DisplayTaskProgress(stateChangeTask, taskName, taskCompleteMessage, taskErrorMessage, taskCanceledMessage);
			}
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000F564 File Offset: 0x0000D764
		private bool AskForUserConfirmation(VMStateChangeAction action)
		{
			bool result = true;
			string title = null;
			string text = null;
			string[] array = null;
			Confirmations flag = Confirmations.Reset;
			CommonConfiguration instance = CommonConfiguration.Instance;
			if (action == VMStateChangeAction.Reset)
			{
				flag = Confirmations.Reset;
				if (instance.NeedToConfirm(flag))
				{
					text = CommonResources.ConfirmationReset;
					title = CommonResources.ConfirmationResetTitle;
					array = new string[]
					{
						CommonResources.ConfirmationResetButton1,
						CommonResources.ConfirmationResetButton2
					};
				}
			}
			else if (action == VMStateChangeAction.Turnoff)
			{
				flag = Confirmations.Turnoff;
				if (instance.NeedToConfirm(flag))
				{
					text = CommonResources.ConfirmationTurnoff;
					title = CommonResources.ConfirmationTurnoffTitle;
					array = new string[]
					{
						CommonResources.ConfirmationTurnoffButton1,
						CommonResources.ConfirmationTurnoffButton2
					};
				}
			}
			else if (action == VMStateChangeAction.Revert)
			{
				flag = Confirmations.Revert;
				if (instance.NeedToConfirm(flag))
				{
					text = CommonResources.ConfirmationRevert;
					title = CommonResources.ConfirmationRevertTitle;
					array = new string[]
					{
						CommonResources.ConfirmationRevertButton1,
						CommonResources.ConfirmationRevertButton2
					};
				}
			}
			else if (action == VMStateChangeAction.Shutdown)
			{
				flag = Confirmations.Shutdown;
				if (instance.NeedToConfirm(flag))
				{
					text = CommonResources.ConfirmationShutdown;
					title = CommonResources.ConfirmationShutdownTitle;
					array = new string[]
					{
						CommonResources.ConfirmationShutdownButton1,
						CommonResources.ConfirmationShutdownButton2
					};
				}
			}
			if (text != null && array != null)
			{
				bool flag2;
				int num = Program.Displayer.DisplayConfirmation(title, text, array, 1, TaskDialogIcon.Warning, CommonResources.ConfirmationVerifyText, out flag2);
				if (flag2)
				{
					instance.TurnoffNeedToConfirmFlag(flag);
					ThreadPool.QueueUserWorkItem(new WaitCallback(CommonUtilities.SaveConfiguration), instance);
				}
				result = (num == 0);
			}
			return result;
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000F6B0 File Offset: 0x0000D8B0
		private VMStateChangeAction MapVMControlActionToVMStateChangeAction(VMControlAction action)
		{
			VMComputerSystemState state = this.m_ActionTarget.VirtualMachine.State;
			VMStateChangeAction result;
			if (action == VMControlAction.StartTurnOff)
			{
				if (state == VMComputerSystemState.PowerOff || state == VMComputerSystemState.Hibernated)
				{
					result = VMStateChangeAction.TurnOn;
				}
				else if (state == VMComputerSystemState.Saved)
				{
					result = VMStateChangeAction.Restore;
				}
				else
				{
					result = VMStateChangeAction.Turnoff;
				}
			}
			else if (action == VMControlAction.Revert)
			{
				result = VMStateChangeAction.Revert;
			}
			else if (action == VMControlAction.Shutdown)
			{
				result = VMStateChangeAction.Shutdown;
			}
			else if (action == VMControlAction.Save)
			{
				result = VMStateChangeAction.SaveState;
			}
			else if (action == VMControlAction.PauseResume)
			{
				if (state == VMComputerSystemState.Running)
				{
					result = VMStateChangeAction.Pause;
				}
				else
				{
					result = VMStateChangeAction.Resume;
				}
			}
			else
			{
				if (action != VMControlAction.Reset)
				{
					throw new ArgumentOutOfRangeException("action");
				}
				result = VMStateChangeAction.Reset;
			}
			return result;
		}

		// Token: 0x04000125 RID: 293
		private readonly IMenuActionTarget m_ActionTarget;

		// Token: 0x04000126 RID: 294
		private bool m_StateChangeTaskRunning;
	}
}
