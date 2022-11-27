using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Virtualization.Client.Management;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000026 RID: 38
	internal class ClipboardMenu : MenuItemProvider, IMenuItemProvider
	{
		// Token: 0x060001FE RID: 510 RVA: 0x000111C4 File Offset: 0x0000F3C4
		public ClipboardMenu(IMenuActionTarget menuTarget) : base(menuTarget)
		{
			VmisMenuItemFactory menuItemFactory = base.GetMenuItemFactory();
			this.m_TypeText = menuItemFactory.CreateMenuItem(ClipboardMenu.ResourceInfo.TypeText);
			this.m_CaptureScreen = menuItemFactory.CreateMenuItem(ClipboardMenu.ResourceInfo.CaptureScreen);
			this.m_Clipboard = menuItemFactory.CreateParentMenuItem(ClipboardMenu.ResourceInfo.Clipboard, new ToolStripItem[]
			{
				this.m_TypeText,
				this.m_CaptureScreen
			});
			base.MenuItems = new VmisMenuItem[]
			{
				this.m_TypeText,
				this.m_CaptureScreen
			};
		}

		// Token: 0x060001FF RID: 511 RVA: 0x00011248 File Offset: 0x0000F448
		public static IVMKeyboard GetKeyboardDeviceForVM(IVMComputerSystem virtualMachine)
		{
			if (virtualMachine == null)
			{
				throw new ArgumentNullException("virtualMachine");
			}
			IVMKeyboard keyboard = virtualMachine.Keyboard;
			if (keyboard == null)
			{
				virtualMachine.UpdateAssociationCache();
				keyboard = virtualMachine.Keyboard;
			}
			return keyboard;
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0001127C File Offset: 0x0000F47C
		protected override void OnMenuItem(object sender, EventArgs ea)
		{
			if (sender == this.m_TypeText)
			{
				string text = Clipboard.GetText();
				if (string.IsNullOrEmpty(text))
				{
					return;
				}
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.TypeText), text);
				return;
			}
			else
			{
				if (sender == this.m_CaptureScreen)
				{
					Thread thread = new Thread(new ThreadStart(this.CaptureScreen));
					thread.SetApartmentState(ApartmentState.STA);
					thread.Start();
					return;
				}
				base.OnMenuItem(sender, ea);
				return;
			}
		}

		// Token: 0x06000201 RID: 513 RVA: 0x000112E4 File Offset: 0x0000F4E4
		public override void UpdateEnabledState()
		{
			if (base.MenuTarget.VirtualMachine == null)
			{
				this.m_TypeText.Enabled = false;
				this.m_CaptureScreen.Enabled = false;
				return;
			}
			VMComputerSystemState state = base.MenuTarget.VirtualMachine.State;
			bool isRdpConnected = base.MenuTarget.IsRdpConnected;
			bool isRdpEnabled = base.MenuTarget.IsRdpEnabled;
			this.m_TypeText.Enabled = (state == VMComputerSystemState.Running && isRdpConnected && isRdpEnabled);
			this.m_CaptureScreen.Enabled = ((state == VMComputerSystemState.Running || state == VMComputerSystemState.Paused) && isRdpConnected && isRdpEnabled);
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0001136E File Offset: 0x0000F56E
		public override VmisMenuItem[] GetMenuItems()
		{
			return new VmisMenuItem[]
			{
				this.m_Clipboard
			};
		}

		// Token: 0x06000203 RID: 515 RVA: 0x00011380 File Offset: 0x0000F580
		private void CaptureScreen()
		{
			try
			{
				Image image = base.MenuTarget.CopyScreenImage();
				if (image != null)
				{
					try
					{
						Clipboard.SetImage(image);
					}
					finally
					{
						image.Dispose();
					}
				}
			}
			catch (Exception ex)
			{
				VMTrace.TraceError("Failed to capture the vm's screen image!", ex);
				base.MenuTarget.AsyncUIThreadMethodInvoker(new DisplayErrorLocalHandler(Program.Displayer.DisplayError), new object[]
				{
					VMISResources.Error_CaptureScreenFailed,
					ex
				});
			}
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0001140C File Offset: 0x0000F60C
		private void TypeText(object textObj)
		{
			try
			{
				string text = textObj as string;
				IVMKeyboard keyboardDeviceForVM = ClipboardMenu.GetKeyboardDeviceForVM(base.MenuTarget.VirtualMachine);
				if (keyboardDeviceForVM != null)
				{
					text = text.Replace("\r\n", "\r");
					keyboardDeviceForVM.UpdatePropertyCache(TimeSpan.FromSeconds(2.0));
					if (keyboardDeviceForVM.UnicodeSupported)
					{
						keyboardDeviceForVM.TypeText(text);
					}
					else
					{
						byte[] scancodes = ClipboardMenu.ScancodeConverter.ToScancodes(text);
						keyboardDeviceForVM.TypeScancodes(scancodes);
					}
				}
				else
				{
					base.MenuTarget.AsyncUIThreadMethodInvoker(new WaitCallback(this.TypeTextFailed), new object[]
					{
						VMISResources.Error_KeyboardDeviceNotFound
					});
				}
			}
			catch (Exception ex)
			{
				VMTrace.TraceError("Typing text in the vm failed!", ex);
				base.MenuTarget.AsyncUIThreadMethodInvoker(new WaitCallback(this.TypeTextFailed), new object[]
				{
					ex
				});
			}
		}

		// Token: 0x06000205 RID: 517 RVA: 0x000114EC File Offset: 0x0000F6EC
		private void TypeTextFailed(object errorObj)
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

		// Token: 0x04000151 RID: 337
		private const int gm_SendKeysWaitMilliseconds = 10;

		// Token: 0x04000152 RID: 338
		private VmisMenuItem m_TypeText;

		// Token: 0x04000153 RID: 339
		private VmisMenuItem m_CaptureScreen;

		// Token: 0x04000154 RID: 340
		private VmisMenuItem m_Clipboard;

		// Token: 0x02000050 RID: 80
		private static class ResourceInfo
		{
			// Token: 0x040001E7 RID: 487
			private const string Append = "ClipboardMenu_";

			// Token: 0x040001E8 RID: 488
			public static readonly MenuItemResourceInfo TypeText = new MenuItemResourceInfo("ClipboardMenu_TypeText", true);

			// Token: 0x040001E9 RID: 489
			public static readonly MenuItemResourceInfo CaptureScreen = new MenuItemResourceInfo("ClipboardMenu_CaptureScreen", true);

			// Token: 0x040001EA RID: 490
			public static readonly MenuItemResourceInfo Clipboard = new MenuItemResourceInfo("ClipboardMenu_Clipboard", false);
		}

		// Token: 0x02000051 RID: 81
		internal static class ScancodeConverter
		{
			// Token: 0x060003F1 RID: 1009 RVA: 0x00015C38 File Offset: 0x00013E38
			internal static byte[] ToScancodes(string text)
			{
				if (string.IsNullOrEmpty(text))
				{
					return new byte[0];
				}
				List<byte> list = new List<byte>(text.Length * 3);
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				list.AddRange(ClipboardMenu.ScancodeConverter.GetModifierScancode(Keys.ShiftKey, false));
				list.AddRange(ClipboardMenu.ScancodeConverter.GetModifierScancode(Keys.ControlKey, false));
				list.AddRange(ClipboardMenu.ScancodeConverter.GetModifierScancode(Keys.Menu, false));
				for (int i = 0; i < text.Length; i++)
				{
					short num = ClipboardMenu.ScancodeConverter.NativeMethods.VkKeyScan(text[i]);
					if (num == -1)
					{
						num = 447;
					}
					bool flag4 = (num & 256) != 0;
					if (flag4 != flag)
					{
						list.AddRange(ClipboardMenu.ScancodeConverter.GetModifierScancode(Keys.ShiftKey, flag4));
						flag = flag4;
					}
					bool flag5 = (num & 512) != 0;
					if (flag5 != flag2)
					{
						list.AddRange(ClipboardMenu.ScancodeConverter.GetModifierScancode(Keys.ControlKey, flag5));
						flag2 = flag5;
					}
					bool flag6 = (num & 1024) != 0;
					if (flag6 != flag3)
					{
						list.AddRange(ClipboardMenu.ScancodeConverter.GetModifierScancode(Keys.Menu, flag6));
						flag3 = flag6;
					}
					int num2 = ClipboardMenu.ScancodeConverter.NativeMethods.MapVirtualKey((int)(num & 255), ClipboardMenu.ScancodeConverter.NativeMethods.MapType.MAPVK_VK_TO_VSC);
					if (num2 != 0)
					{
						list.AddRange(ClipboardMenu.ScancodeConverter.GetBytes(num2));
						list.AddRange(ClipboardMenu.ScancodeConverter.GetBytes(ClipboardMenu.ScancodeConverter.ToBreakScancode(num2)));
					}
				}
				if (flag)
				{
					list.AddRange(ClipboardMenu.ScancodeConverter.GetModifierScancode(Keys.ShiftKey, false));
				}
				if (flag2)
				{
					list.AddRange(ClipboardMenu.ScancodeConverter.GetModifierScancode(Keys.ControlKey, false));
				}
				if (flag3)
				{
					list.AddRange(ClipboardMenu.ScancodeConverter.GetModifierScancode(Keys.Menu, false));
				}
				return list.ToArray();
			}

			// Token: 0x060003F2 RID: 1010 RVA: 0x00015DA5 File Offset: 0x00013FA5
			internal static int ToBreakScancode(int makeScanCode)
			{
				if (makeScanCode == 0)
				{
					throw new ArgumentOutOfRangeException("makeScanCode");
				}
				return makeScanCode | 128;
			}

			// Token: 0x060003F3 RID: 1011 RVA: 0x00015DBC File Offset: 0x00013FBC
			internal static byte[] GetBytes(int scancode)
			{
				if (scancode == 0)
				{
					throw new ArgumentOutOfRangeException("scancode");
				}
				byte[] bytes = BitConverter.GetBytes(scancode);
				List<byte> list = new List<byte>(bytes.Length);
				for (int i = bytes.Length - 1; i >= 0; i--)
				{
					if (bytes[i] != 0)
					{
						list.Add(bytes[i]);
					}
				}
				return list.ToArray();
			}

			// Token: 0x060003F4 RID: 1012 RVA: 0x00015E0B File Offset: 0x0001400B
			internal static byte[] GetModifierScancode(Keys key, bool isMake)
			{
				if (isMake)
				{
					return ClipboardMenu.ScancodeConverter.GetBytes(ClipboardMenu.ScancodeConverter.NativeMethods.MapVirtualKey((int)key, ClipboardMenu.ScancodeConverter.NativeMethods.MapType.MAPVK_VK_TO_VSC));
				}
				return ClipboardMenu.ScancodeConverter.GetBytes(ClipboardMenu.ScancodeConverter.ToBreakScancode(ClipboardMenu.ScancodeConverter.NativeMethods.MapVirtualKey((int)key, ClipboardMenu.ScancodeConverter.NativeMethods.MapType.MAPVK_VK_TO_VSC)));
			}

			// Token: 0x02000062 RID: 98
			internal static class NativeMethods
			{
				// Token: 0x06000414 RID: 1044
				[DllImport("user32.dll")]
				internal static extern short VkKeyScan(char character);

				// Token: 0x06000415 RID: 1045
				[DllImport("user32.dll")]
				internal static extern int MapVirtualKey(int code, ClipboardMenu.ScancodeConverter.NativeMethods.MapType mapType);

				// Token: 0x02000063 RID: 99
				internal enum MapType
				{
					// Token: 0x0400021C RID: 540
					MAPVK_VK_TO_VSC,
					// Token: 0x0400021D RID: 541
					MAPVK_VSC_TO_VK,
					// Token: 0x0400021E RID: 542
					MAPVK_VK_TO_CHAR,
					// Token: 0x0400021F RID: 543
					MAPVK_VSC_TO_VK_EX,
					// Token: 0x04000220 RID: 544
					MAPVK_VK_TO_VSC_EX
				}
			}
		}
	}
}
