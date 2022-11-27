using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000009 RID: 9
	internal class DisplayResolution
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000059 RID: 89 RVA: 0x0000551E File Offset: 0x0000371E
		public List<Resolution> AllResolutions
		{
			get
			{
				return this.m_AllResolutions;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00005526 File Offset: 0x00003726
		public int DefaultResolutionIndex
		{
			get
			{
				return this.m_DefaultResolutionIdx;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600005B RID: 91 RVA: 0x0000552E File Offset: 0x0000372E
		public int FullScreenResolutionIndex
		{
			get
			{
				return this.m_FullScreenResolutionIdx;
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00005536 File Offset: 0x00003736
		public DisplayResolution(Resolution defaultResolution)
		{
			this.m_DefaultResolution = defaultResolution;
			this.m_AllResolutions = new List<Resolution>();
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00005550 File Offset: 0x00003750
		public void CreateResolutionList(Screen targetScreen)
		{
			Resolution resolution = new Resolution(targetScreen.Bounds.Width, targetScreen.Bounds.Height);
			foreach (Resolution resolution2 in DisplayResolution.gm_StandardResolution)
			{
				Resolution resolution3 = resolution2;
				if (resolution.Width < resolution.Height)
				{
					resolution3 = new Resolution(resolution2.Height, resolution2.Width);
				}
				if (resolution3.CompareTo(resolution) <= 0)
				{
					this.m_AllResolutions.Add(resolution3);
				}
			}
			if (this.m_AllResolutions.BinarySearch(resolution) < 0)
			{
				this.m_AllResolutions.Insert(this.m_AllResolutions.Count, resolution);
			}
			this.m_FullScreenResolutionIdx = this.m_AllResolutions.Count - 1;
			Resolution item = this.m_DefaultResolution;
			if (resolution.Width < resolution.Height)
			{
				item = new Resolution(this.m_DefaultResolution.Height, this.m_DefaultResolution.Width);
			}
			int num = this.m_AllResolutions.BinarySearch(item);
			if (num < 0)
			{
				num = ~num;
			}
			if (num == this.m_AllResolutions.Count)
			{
				this.m_DefaultResolutionIdx = this.m_AllResolutions.Count - 1;
				return;
			}
			this.m_DefaultResolutionIdx = num;
		}

		// Token: 0x04000050 RID: 80
		private static readonly IReadOnlyCollection<Resolution> gm_StandardResolution = new Resolution[]
		{
			new Resolution(640, 480),
			new Resolution(800, 600),
			new Resolution(1024, 768),
			new Resolution(1280, 720),
			new Resolution(1280, 768),
			new Resolution(1280, 800),
			new Resolution(1280, 1024),
			new Resolution(1366, 768),
			new Resolution(1440, 900),
			new Resolution(1400, 1050),
			new Resolution(1600, 1200),
			new Resolution(1680, 1050),
			new Resolution(1920, 1080),
			new Resolution(1920, 1200),
			new Resolution(2048, 1536),
			new Resolution(2560, 1440),
			new Resolution(2560, 1600)
		};

		// Token: 0x04000051 RID: 81
		private List<Resolution> m_AllResolutions;

		// Token: 0x04000052 RID: 82
		private Resolution m_DefaultResolution;

		// Token: 0x04000053 RID: 83
		private int m_DefaultResolutionIdx;

		// Token: 0x04000054 RID: 84
		private int m_FullScreenResolutionIdx;
	}
}
