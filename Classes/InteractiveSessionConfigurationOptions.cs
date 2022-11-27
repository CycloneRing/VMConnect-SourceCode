using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000004 RID: 4
	[SettingsProvider(typeof(LocalStorageProvider))]
	internal partial class InteractiveSessionConfigurationOptions : ApplicationSettingsBase
	{
		// Token: 0x0600000D RID: 13 RVA: 0x00002705 File Offset: 0x00000905
		private InteractiveSessionConfigurationOptions()
		{
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000E RID: 14 RVA: 0x0000270D File Offset: 0x0000090D
		public static InteractiveSessionConfigurationOptions Instance
		{
			get
			{
				return InteractiveSessionConfigurationOptions.gm_Instance;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000F RID: 15 RVA: 0x00002714 File Offset: 0x00000914
		public static string FileName
		{
			get
			{
				return "vmconnect.config";
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000010 RID: 16 RVA: 0x0000271C File Offset: 0x0000091C
		// (set) Token: 0x06000011 RID: 17 RVA: 0x0000276F File Offset: 0x0000096F
		[UserScopedSetting]
		public Point? StartingPosition
		{
			get
			{
				object obj = this["StartingPosition"];
				if (obj == null)
				{
					return null;
				}
				Point point = (Point)obj;
				if (!Screen.FromPoint(point).WorkingArea.Contains(point))
				{
					return null;
				}
				return new Point?(point);
			}
			set
			{
				this["StartingPosition"] = value;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000016 RID: 22 RVA: 0x000027C8 File Offset: 0x000009C8
		// (set) Token: 0x06000017 RID: 23 RVA: 0x000027EC File Offset: 0x000009EC
		[UserScopedSetting]
		public uint ZoomLevel
		{
			get
			{
				object obj = this["ZoomLevel"];
				if (obj == null)
				{
					return 0U;
				}
				return (uint)obj;
			}
			set
			{
				this["ZoomLevel"] = value;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000018 RID: 24 RVA: 0x000027FF File Offset: 0x000009FF
		public override SettingsContext Context
		{
			get
			{
				SettingsContext context = base.Context;
				context["FileName"] = InteractiveSessionConfigurationOptions.FileName;
				return context;
			}
		}

		// Token: 0x0400000A RID: 10
		private static InteractiveSessionConfigurationOptions gm_Instance = new InteractiveSessionConfigurationOptions();
	}
}
