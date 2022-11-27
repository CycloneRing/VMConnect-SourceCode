using System;
using Microsoft.Virtualization.Client.Common;
using Microsoft.Virtualization.Client.Management;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000013 RID: 19
	public class RdpConnectionInfo
	{
		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x0000A158 File Offset: 0x00008358
		// (set) Token: 0x060000F3 RID: 243 RVA: 0x0000A160 File Offset: 0x00008360
		public string ServerConnectionName { get; set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x0000A169 File Offset: 0x00008369
		// (set) Token: 0x060000F5 RID: 245 RVA: 0x0000A171 File Offset: 0x00008371
		public IVMComputerSystem VirtualMachine { get; set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x0000A17A File Offset: 0x0000837A
		// (set) Token: 0x060000F7 RID: 247 RVA: 0x0000A182 File Offset: 0x00008382
		public int RdpPort { get; set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x0000A18B File Offset: 0x0000838B
		// (set) Token: 0x060000F9 RID: 249 RVA: 0x0000A193 File Offset: 0x00008393
		public int LaunchIndex { get; set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000FA RID: 250 RVA: 0x0000A19C File Offset: 0x0000839C
		// (set) Token: 0x060000FB RID: 251 RVA: 0x0000A1A4 File Offset: 0x000083A4
		public bool ShowOptions { get; set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000FC RID: 252 RVA: 0x0000A1AD File Offset: 0x000083AD
		// (set) Token: 0x060000FD RID: 253 RVA: 0x0000A1B5 File Offset: 0x000083B5
		public WindowsCredential Credential { get; set; }
	}
}
