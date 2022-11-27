using System;
using System.Resources;
using System.Windows.Forms;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x0200002F RID: 47
	internal struct MenuItemResourceInfo
	{
		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000246 RID: 582 RVA: 0x00011A18 File Offset: 0x0000FC18
		private static ResourceManager Resources
		{
			get
			{
				if (MenuItemResourceInfo.gm_Resources == null)
				{
					MenuItemResourceInfo.gm_Resources = new ResourceManager("Microsoft.Virtualization.Client.InteractiveSession.Resources.MenuResources", typeof(MenuItemResourceInfo).Assembly);
				}
				return MenuItemResourceInfo.gm_Resources;
			}
		}

		// Token: 0x06000247 RID: 583 RVA: 0x00011A44 File Offset: 0x0000FC44
		public MenuItemResourceInfo(string resourcePath, bool hasKey)
		{
			this.m_ResourcePath = resourcePath;
			this.m_HasKey = hasKey;
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000248 RID: 584 RVA: 0x00011A54 File Offset: 0x0000FC54
		public bool HasKey
		{
			get
			{
				return this.m_HasKey;
			}
		}

		// Token: 0x06000249 RID: 585 RVA: 0x00011A5C File Offset: 0x0000FC5C
		public override string ToString()
		{
			if (this.m_ResourcePath != null)
			{
				return this.m_ResourcePath;
			}
			return string.Empty;
		}

		// Token: 0x0600024A RID: 586 RVA: 0x00011A72 File Offset: 0x0000FC72
		public string GetText()
		{
			return this.GetResourceString(this.m_ResourcePath);
		}

		// Token: 0x0600024B RID: 587 RVA: 0x00011A80 File Offset: 0x0000FC80
		public Keys GetKey()
		{
			Keys result = Keys.None;
			if (this.HasKey)
			{
				string resourceString = this.GetResourceString(this.m_ResourcePath + MenuItemResourceInfo.gm_Key);
				try
				{
					result = (Keys)Enum.Parse(typeof(Keys), resourceString, true);
				}
				catch (ArgumentException ex)
				{
					VMTrace.TraceError("Menu item shortcut key is not formatted correctly!", ex);
				}
			}
			return result;
		}

		// Token: 0x0600024C RID: 588 RVA: 0x00011AE8 File Offset: 0x0000FCE8
		private string GetResourceString(string resourcePath)
		{
			string result = null;
			if (!string.IsNullOrEmpty(resourcePath))
			{
				try
				{
					result = MenuItemResourceInfo.Resources.GetString(resourcePath);
				}
				catch (InvalidOperationException ex)
				{
					VMTrace.TraceError("Menu resource is not a string!", ex);
				}
				catch (MissingManifestResourceException ex2)
				{
					VMTrace.TraceError("No usable set of menu resources found in the manifest!", ex2);
				}
			}
			return result;
		}

		// Token: 0x04000165 RID: 357
		private static ResourceManager gm_Resources;

		// Token: 0x04000166 RID: 358
		private static string gm_Key = "Key";

		// Token: 0x04000167 RID: 359
		private readonly string m_ResourcePath;

		// Token: 0x04000168 RID: 360
		private readonly bool m_HasKey;
	}
}
