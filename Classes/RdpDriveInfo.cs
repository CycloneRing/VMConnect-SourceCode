using System;
using System.IO;
using Microsoft.Virtualization.Client.InteractiveSession.Resources;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000014 RID: 20
	internal class RdpDriveInfo
	{
		// Token: 0x060000FE RID: 254 RVA: 0x0000A1BE File Offset: 0x000083BE
		public RdpDriveInfo()
		{
			this.m_DriveInfo = DriveInfo.GetDrives();
		}

		// Token: 0x060000FF RID: 255 RVA: 0x0000A1D4 File Offset: 0x000083D4
		public string GetDescription(string driveLetter)
		{
			string result = driveLetter;
			string text = null;
			if (driveLetter.Length > 0)
			{
				foreach (DriveInfo driveInfo2 in this.m_DriveInfo)
				{
					if (driveInfo2.Name.Length > 0 && driveInfo2.Name[0] == driveLetter[0])
					{
						try
						{
							if (!string.IsNullOrWhiteSpace(driveInfo2.VolumeLabel))
							{
								text = driveInfo2.VolumeLabel;
							}
						}
						catch
						{
						}
						if (string.IsNullOrWhiteSpace(text))
						{
							text = this.GetDefaultVolumeName(driveInfo2.DriveType);
						}
						result = text + " (" + driveLetter[0].ToString() + ":)";
						break;
					}
				}
			}
			return result;
		}

		// Token: 0x06000100 RID: 256 RVA: 0x0000A29C File Offset: 0x0000849C
		private string GetDefaultVolumeName(DriveType driveType)
		{
			switch (driveType)
			{
			case DriveType.Removable:
				return ConnectionResources.Removable;
			case DriveType.Fixed:
				return ConnectionResources.Fixed;
			case DriveType.Network:
				return ConnectionResources.Network;
			case DriveType.CDRom:
				return ConnectionResources.CDRom;
			case DriveType.Ram:
				return ConnectionResources.Ram;
			default:
				return ConnectionResources.Unknown;
			}
		}

		// Token: 0x040000B2 RID: 178
		private DriveInfo[] m_DriveInfo;
	}
}
