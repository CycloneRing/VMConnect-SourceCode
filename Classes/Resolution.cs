using System;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000008 RID: 8
	internal class Resolution : IComparable<Resolution>, IEquatable<Resolution>
	{
		// Token: 0x06000052 RID: 82 RVA: 0x00005457 File Offset: 0x00003657
		public Resolution(int w, int h)
		{
			this.m_Width = w;
			this.m_Height = h;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00005470 File Offset: 0x00003670
		public int CompareTo(Resolution r)
		{
			if (this.m_Height == r.Height && this.m_Width == r.Width)
			{
				return 0;
			}
			if (this.m_Width < r.Width || (this.m_Width == r.Width && this.m_Height < r.Height))
			{
				return -1;
			}
			return 1;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000054C8 File Offset: 0x000036C8
		public bool Equals(Resolution r)
		{
			return r != null && this.CompareTo(r) == 0;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000054DC File Offset: 0x000036DC
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			Resolution r = obj as Resolution;
			return this.Equals(r);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000054FC File Offset: 0x000036FC
		public override int GetHashCode()
		{
			return this.m_Width * 13 + this.m_Height;
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000057 RID: 87 RVA: 0x0000550E File Offset: 0x0000370E
		public int Width
		{
			get
			{
				return this.m_Width;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000058 RID: 88 RVA: 0x00005516 File Offset: 0x00003716
		public int Height
		{
			get
			{
				return this.m_Height;
			}
		}

		// Token: 0x0400004E RID: 78
		private int m_Width;

		// Token: 0x0400004F RID: 79
		private int m_Height;
	}
}
