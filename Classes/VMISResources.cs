using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000035 RID: 53
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class VMISResources
	{
		// Token: 0x0600028D RID: 653 RVA: 0x00013D36 File Offset: 0x00011F36
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal VMISResources()
		{
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x0600028E RID: 654 RVA: 0x00013D3E File Offset: 0x00011F3E
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (VMISResources.resourceMan == null)
				{
					VMISResources.resourceMan = new ResourceManager("Microsoft.Virtualization.Client.InteractiveSession.Resources.VMISResources", typeof(VMISResources).Assembly);
				}
				return VMISResources.resourceMan;
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600028F RID: 655 RVA: 0x00013D6A File Offset: 0x00011F6A
		// (set) Token: 0x06000290 RID: 656 RVA: 0x00013D71 File Offset: 0x00011F71
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return VMISResources.resourceCulture;
			}
			set
			{
				VMISResources.resourceCulture = value;
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000291 RID: 657 RVA: 0x00013D79 File Offset: 0x00011F79
		internal static string AboutDialogTitle
		{
			get
			{
				return VMISResources.ResourceManager.GetString("AboutDialogTitle", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000292 RID: 658 RVA: 0x00013D8F File Offset: 0x00011F8F
		internal static string CommandLine_InvalidCount
		{
			get
			{
				return VMISResources.ResourceManager.GetString("CommandLine_InvalidCount", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000293 RID: 659 RVA: 0x00013DA5 File Offset: 0x00011FA5
		internal static string CommandLine_InvalidGuid
		{
			get
			{
				return VMISResources.ResourceManager.GetString("CommandLine_InvalidGuid", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000294 RID: 660 RVA: 0x00013DBB File Offset: 0x00011FBB
		internal static string CommandLine_InvalidUserName
		{
			get
			{
				return VMISResources.ResourceManager.GetString("CommandLine_InvalidUserName", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000295 RID: 661 RVA: 0x00013DD1 File Offset: 0x00011FD1
		internal static string CommandLine_MissingCredential
		{
			get
			{
				return VMISResources.ResourceManager.GetString("CommandLine_MissingCredential", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000296 RID: 662 RVA: 0x00013DE7 File Offset: 0x00011FE7
		internal static string CommandLine_MissingOptionArgument
		{
			get
			{
				return VMISResources.ResourceManager.GetString("CommandLine_MissingOptionArgument", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000297 RID: 663 RVA: 0x00013DFD File Offset: 0x00011FFD
		internal static string CommandLine_MutuallyExclusiveOptions
		{
			get
			{
				return VMISResources.ResourceManager.GetString("CommandLine_MutuallyExclusiveOptions", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000298 RID: 664 RVA: 0x00013E13 File Offset: 0x00012013
		internal static string CommandLine_UnknownOption
		{
			get
			{
				return VMISResources.ResourceManager.GetString("CommandLine_UnknownOption", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000299 RID: 665 RVA: 0x00013E29 File Offset: 0x00012029
		internal static string CommandLine_Usage
		{
			get
			{
				return VMISResources.ResourceManager.GetString("CommandLine_Usage", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600029A RID: 666 RVA: 0x00013E3F File Offset: 0x0001203F
		internal static string CommandLine_UsageTitle
		{
			get
			{
				return VMISResources.ResourceManager.GetString("CommandLine_UsageTitle", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x0600029B RID: 667 RVA: 0x00013E55 File Offset: 0x00012055
		internal static string Connecting
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Connecting", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x0600029C RID: 668 RVA: 0x00013E6B File Offset: 0x0001206B
		internal static string ConnectionDialog_ConnectAsNone
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ConnectionDialog_ConnectAsNone", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600029D RID: 669 RVA: 0x00013E81 File Offset: 0x00012081
		internal static string ConnectionDialog_InvalidUserName_Detail
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ConnectionDialog_InvalidUserName_Detail", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600029E RID: 670 RVA: 0x00013E97 File Offset: 0x00012097
		internal static string ConnectionDialog_InvalidUserName_Title
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ConnectionDialog_InvalidUserName_Title", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x0600029F RID: 671 RVA: 0x00013EAD File Offset: 0x000120AD
		internal static string ConnectionDialog_PromptForCredentialsCaption
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ConnectionDialog_PromptForCredentialsCaption", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060002A0 RID: 672 RVA: 0x00013EC3 File Offset: 0x000120C3
		internal static string ConnectionDialog_PromptForCredentialsMessage
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ConnectionDialog_PromptForCredentialsMessage", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x060002A1 RID: 673 RVA: 0x00013ED9 File Offset: 0x000120D9
		internal static string ConnectMethodThrew
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ConnectMethodThrew", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x060002A2 RID: 674 RVA: 0x00013EEF File Offset: 0x000120EF
		internal static string ConnectWithExistingUser
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ConnectWithExistingUser", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x060002A3 RID: 675 RVA: 0x00013F05 File Offset: 0x00012105
		internal static string Disconnected_WhileConnected_ReconnectButton
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Disconnected_WhileConnected_ReconnectButton", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x060002A4 RID: 676 RVA: 0x00013F1B File Offset: 0x0001211B
		internal static string Disconnected_WhileConnected_ReconnectQuestion
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Disconnected_WhileConnected_ReconnectQuestion", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x060002A5 RID: 677 RVA: 0x00013F31 File Offset: 0x00012131
		internal static string Disconnected_WhileConnecting_ReconnectButton
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Disconnected_WhileConnecting_ReconnectButton", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x060002A6 RID: 678 RVA: 0x00013F47 File Offset: 0x00012147
		internal static string Disconnected_WhileConnecting_ReconnectQuestion
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Disconnected_WhileConnecting_ReconnectQuestion", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x060002A7 RID: 679 RVA: 0x00013F5D File Offset: 0x0001215D
		internal static string DisconnectReason_3DVideo
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_3DVideo", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060002A8 RID: 680 RVA: 0x00013F73 File Offset: 0x00012173
		internal static string DisconnectReason_AccountDisabled
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_AccountDisabled", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060002A9 RID: 681 RVA: 0x00013F89 File Offset: 0x00012189
		internal static string DisconnectReason_AccountExpired
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_AccountExpired", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x060002AA RID: 682 RVA: 0x00013F9F File Offset: 0x0001219F
		internal static string DisconnectReason_AccountLockedOut
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_AccountLockedOut", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060002AB RID: 683 RVA: 0x00013FB5 File Offset: 0x000121B5
		internal static string DisconnectReason_AccountRestricted
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_AccountRestricted", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060002AC RID: 684 RVA: 0x00013FCB File Offset: 0x000121CB
		internal static string DisconnectReason_BadServerName
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_BadServerName", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060002AD RID: 685 RVA: 0x00013FE1 File Offset: 0x000121E1
		internal static string DisconnectReason_CertExpired
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_CertExpired", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060002AE RID: 686 RVA: 0x00013FF7 File Offset: 0x000121F7
		internal static string DisconnectReason_ClientSideProtocolError
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_ClientSideProtocolError", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060002AF RID: 687 RVA: 0x0001400D File Offset: 0x0001220D
		internal static string DisconnectReason_ConnectionAttemptCanceled
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_ConnectionAttemptCanceled", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060002B0 RID: 688 RVA: 0x00014023 File Offset: 0x00012223
		internal static string DisconnectReason_ConnectionFailed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_ConnectionFailed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060002B1 RID: 689 RVA: 0x00014039 File Offset: 0x00012239
		internal static string DisconnectReason_DecompressionFailure
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_DecompressionFailure", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060002B2 RID: 690 RVA: 0x0001404F File Offset: 0x0001224F
		internal static string DisconnectReason_EncryptionError
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_EncryptionError", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060002B3 RID: 691 RVA: 0x00014065 File Offset: 0x00012265
		internal static string DisconnectReason_InternalError_Connected
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_InternalError_Connected", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060002B4 RID: 692 RVA: 0x0001407B File Offset: 0x0001227B
		internal static string DisconnectReason_InternalError_Connecting
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_InternalError_Connecting", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060002B5 RID: 693 RVA: 0x00014091 File Offset: 0x00012291
		internal static string DisconnectReason_InvalidLogonHours
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_InvalidLogonHours", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060002B6 RID: 694 RVA: 0x000140A7 File Offset: 0x000122A7
		internal static string DisconnectReason_LoginTimedOut
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_LoginTimedOut", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060002B7 RID: 695 RVA: 0x000140BD File Offset: 0x000122BD
		internal static string DisconnectReason_LowMemory
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_LowMemory", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060002B8 RID: 696 RVA: 0x000140D3 File Offset: 0x000122D3
		internal static string DisconnectReason_NetworkError
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_NetworkError", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060002B9 RID: 697 RVA: 0x000140E9 File Offset: 0x000122E9
		internal static string DisconnectReason_NetworkInitConnectError
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_NetworkInitConnectError", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060002BA RID: 698 RVA: 0x000140FF File Offset: 0x000122FF
		internal static string DisconnectReason_NormallyWhileConnected
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_NormallyWhileConnected", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060002BB RID: 699 RVA: 0x00014115 File Offset: 0x00012315
		internal static string DisconnectReason_NormallyWhileConnecting
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_NormallyWhileConnecting", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060002BC RID: 700 RVA: 0x0001412B File Offset: 0x0001232B
		internal static string DisconnectReason_NoSuchUser
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_NoSuchUser", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060002BD RID: 701 RVA: 0x00014141 File Offset: 0x00012341
		internal static string DisconnectReason_PasswordExpired
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_PasswordExpired", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060002BE RID: 702 RVA: 0x00014157 File Offset: 0x00012357
		internal static string DisconnectReason_PolicyProhibitsConnection
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_PolicyProhibitsConnection", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060002BF RID: 703 RVA: 0x0001416D File Offset: 0x0001236D
		internal static string DisconnectReason_SecurityError
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_SecurityError", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060002C0 RID: 704 RVA: 0x00014183 File Offset: 0x00012383
		internal static string DisconnectReason_SocketClosed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_SocketClosed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060002C1 RID: 705 RVA: 0x00014199 File Offset: 0x00012399
		internal static string DisconnectReason_UnexpectedErrorCode_Connected
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_UnexpectedErrorCode_Connected", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060002C2 RID: 706 RVA: 0x000141AF File Offset: 0x000123AF
		internal static string DisconnectReason_UnexpectedErrorCode_Connecting
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReason_UnexpectedErrorCode_Connecting", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060002C3 RID: 707 RVA: 0x000141C5 File Offset: 0x000123C5
		internal static string DisconnectReasonExtended_AccessToVideoDenied
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReasonExtended_AccessToVideoDenied", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x060002C4 RID: 708 RVA: 0x000141DB File Offset: 0x000123DB
		internal static string DisconnectReasonExtended_ReplacedByOtherConnection
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReasonExtended_ReplacedByOtherConnection", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060002C5 RID: 709 RVA: 0x000141F1 File Offset: 0x000123F1
		internal static string DisconnectReasonExtended_ServerDeniedConnection
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReasonExtended_ServerDeniedConnection", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060002C6 RID: 710 RVA: 0x00014207 File Offset: 0x00012407
		internal static string DisconnectReasonExtended_ServerOutOfMempory
		{
			get
			{
				return VMISResources.ResourceManager.GetString("DisconnectReasonExtended_ServerOutOfMempory", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060002C7 RID: 711 RVA: 0x0001421D File Offset: 0x0001241D
		internal static string Enhanced_NotAvailable
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Enhanced_NotAvailable", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060002C8 RID: 712 RVA: 0x00014233 File Offset: 0x00012433
		internal static string Enhanced_NotAvailableDetails
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Enhanced_NotAvailableDetails", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060002C9 RID: 713 RVA: 0x00014249 File Offset: 0x00012449
		internal static string Enhanced_NotAvailableSummary
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Enhanced_NotAvailableSummary", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060002CA RID: 714 RVA: 0x0001425F File Offset: 0x0001245F
		internal static string Error_AxControlNotRegistered
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Error_AxControlNotRegistered", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060002CB RID: 715 RVA: 0x00014275 File Offset: 0x00012475
		internal static string Error_CaptureScreenFailed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Error_CaptureScreenFailed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x060002CC RID: 716 RVA: 0x0001428B File Offset: 0x0001248B
		internal static string Error_ChangeVMStateFailed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Error_ChangeVMStateFailed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x060002CD RID: 717 RVA: 0x000142A1 File Offset: 0x000124A1
		internal static string Error_CouldNotDetermineRdpPort
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Error_CouldNotDetermineRdpPort", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x060002CE RID: 718 RVA: 0x000142B7 File Offset: 0x000124B7
		internal static string Error_CouldNotDetermineServerFqdn
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Error_CouldNotDetermineServerFqdn", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x060002CF RID: 719 RVA: 0x000142CD File Offset: 0x000124CD
		internal static string Error_DebugMenu_CacheRefresh
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Error_DebugMenu_CacheRefresh", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x000142E3 File Offset: 0x000124E3
		internal static string Error_KeyboardDeviceNotFound
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Error_KeyboardDeviceNotFound", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x060002D1 RID: 721 RVA: 0x000142F9 File Offset: 0x000124F9
		internal static string Error_KeyboardSendKeysFailed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Error_KeyboardSendKeysFailed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x060002D2 RID: 722 RVA: 0x0001430F File Offset: 0x0001250F
		internal static string Error_LoginAuthorizationFailed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Error_LoginAuthorizationFailed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x060002D3 RID: 723 RVA: 0x00014325 File Offset: 0x00012525
		internal static string Error_LoginFailed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Error_LoginFailed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x060002D4 RID: 724 RVA: 0x0001433B File Offset: 0x0001253B
		internal static string Error_MustCloseSettingsDialog
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Error_MustCloseSettingsDialog", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060002D5 RID: 725 RVA: 0x00014351 File Offset: 0x00012551
		internal static string Error_OpeningSettings
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Error_OpeningSettings", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x060002D6 RID: 726 RVA: 0x00014367 File Offset: 0x00012567
		internal static string Error_RenameSnapshotFailed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Error_RenameSnapshotFailed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x060002D7 RID: 727 RVA: 0x0001437D File Offset: 0x0001257D
		internal static string Error_TakeSnapshotFailed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Error_TakeSnapshotFailed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x060002D8 RID: 728 RVA: 0x00014393 File Offset: 0x00012593
		internal static string Error_VmImeEventRegistrationFailed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Error_VmImeEventRegistrationFailed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x060002D9 RID: 729 RVA: 0x000143A9 File Offset: 0x000125A9
		internal static string Error_VMShareFailed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Error_VMShareFailed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x060002DA RID: 730 RVA: 0x000143BF File Offset: 0x000125BF
		internal static string ErrorDialogTitle
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ErrorDialogTitle", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x060002DB RID: 731 RVA: 0x000143D5 File Offset: 0x000125D5
		internal static string KeyInputNotCaptured
		{
			get
			{
				return VMISResources.ResourceManager.GetString("KeyInputNotCaptured", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x060002DC RID: 732 RVA: 0x000143EB File Offset: 0x000125EB
		internal static string MainWindow_TitleBar
		{
			get
			{
				return VMISResources.ResourceManager.GetString("MainWindow_TitleBar", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x060002DD RID: 733 RVA: 0x00014401 File Offset: 0x00012601
		internal static string MainWindow_TitleNoVM
		{
			get
			{
				return VMISResources.ResourceManager.GetString("MainWindow_TitleNoVM", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x060002DE RID: 734 RVA: 0x00014417 File Offset: 0x00012617
		internal static string MediaMenu_CaptureDriveFailed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("MediaMenu_CaptureDriveFailed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x060002DF RID: 735 RVA: 0x0001442D File Offset: 0x0001262D
		internal static string MediaMenu_EjectDisk
		{
			get
			{
				return VMISResources.ResourceManager.GetString("MediaMenu_EjectDisk", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x060002E0 RID: 736 RVA: 0x00014443 File Offset: 0x00012643
		internal static string MediaMenu_EjectDiskFailed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("MediaMenu_EjectDiskFailed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x060002E1 RID: 737 RVA: 0x00014459 File Offset: 0x00012659
		internal static string MediaMenu_InsertDiskFailed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("MediaMenu_InsertDiskFailed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x060002E2 RID: 738 RVA: 0x0001446F File Offset: 0x0001266F
		internal static string MediaMenu_UncaptureDrive
		{
			get
			{
				return VMISResources.ResourceManager.GetString("MediaMenu_UncaptureDrive", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x060002E3 RID: 739 RVA: 0x00014485 File Offset: 0x00012685
		internal static string MediaMenu_UncaptureDriveFailed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("MediaMenu_UncaptureDriveFailed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x060002E4 RID: 740 RVA: 0x0001449B File Offset: 0x0001269B
		internal static string Menu_BreakKey
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Menu_BreakKey", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x060002E5 RID: 741 RVA: 0x000144B1 File Offset: 0x000126B1
		internal static string Menu_PauseCommand_Pause
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Menu_PauseCommand_Pause", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x060002E6 RID: 742 RVA: 0x000144C7 File Offset: 0x000126C7
		internal static string Menu_PauseCommand_Resume
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Menu_PauseCommand_Resume", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x060002E7 RID: 743 RVA: 0x000144DD File Offset: 0x000126DD
		internal static string Menu_PauseKey
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Menu_PauseKey", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x060002E8 RID: 744 RVA: 0x000144F3 File Offset: 0x000126F3
		internal static string Menu_StartCommand_Start
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Menu_StartCommand_Start", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x060002E9 RID: 745 RVA: 0x00014509 File Offset: 0x00012709
		internal static string Menu_StartCommand_TurnOff
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Menu_StartCommand_TurnOff", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x060002EA RID: 746 RVA: 0x0001451F File Offset: 0x0001271F
		internal static string MouseInTerminalServicesSessionMainInstruction
		{
			get
			{
				return VMISResources.ResourceManager.GetString("MouseInTerminalServicesSessionMainInstruction", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x060002EB RID: 747 RVA: 0x00014535 File Offset: 0x00012735
		internal static string NoVirtualMachineOnServer
		{
			get
			{
				return VMISResources.ResourceManager.GetString("NoVirtualMachineOnServer", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x060002EC RID: 748 RVA: 0x0001454B File Offset: 0x0001274B
		internal static string RdpViewer_FullScreenConnectionBarText
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_FullScreenConnectionBarText", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x060002ED RID: 749 RVA: 0x00014561 File Offset: 0x00012761
		internal static string RdpViewer_Information_ConnectCanceled
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Information_ConnectCanceled", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x060002EE RID: 750 RVA: 0x00014577 File Offset: 0x00012777
		internal static string RdpViewer_Information_Connecting
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Information_Connecting", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x060002EF RID: 751 RVA: 0x0001458D File Offset: 0x0001278D
		internal static string RdpViewer_Information_CouldNotConnect
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Information_CouldNotConnect", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x060002F0 RID: 752 RVA: 0x000145A3 File Offset: 0x000127A3
		internal static string RdpViewer_Information_DisconnectedNormally
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Information_DisconnectedNormally", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x060002F1 RID: 753 RVA: 0x000145B9 File Offset: 0x000127B9
		internal static string RdpViewer_Information_DisconnectedNormallyLogoffByUser
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Information_DisconnectedNormallyLogoffByUser", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x060002F2 RID: 754 RVA: 0x000145CF File Offset: 0x000127CF
		internal static string RdpViewer_Information_DisconnectedUnexpectedly
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Information_DisconnectedUnexpectedly", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x060002F3 RID: 755 RVA: 0x000145E5 File Offset: 0x000127E5
		internal static string RdpViewer_Information_InitiatedRestoring
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Information_InitiatedRestoring", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x060002F4 RID: 756 RVA: 0x000145FB File Offset: 0x000127FB
		internal static string RdpViewer_Information_LogonFailed_Authorization
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Information_LogonFailed_Authorization", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x060002F5 RID: 757 RVA: 0x00014611 File Offset: 0x00012811
		internal static string RdpViewer_Information_LogonFailed_Cancel
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Information_LogonFailed_Cancel", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x060002F6 RID: 758 RVA: 0x00014627 File Offset: 0x00012827
		internal static string RdpViewer_Information_NotConnected
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Information_NotConnected", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x060002F7 RID: 759 RVA: 0x0001463D File Offset: 0x0001283D
		internal static string RdpViewer_Information_NotStarted
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Information_NotStarted", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x060002F8 RID: 760 RVA: 0x00014653 File Offset: 0x00012853
		internal static string RdpViewer_Information_Pausing
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Information_Pausing", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x060002F9 RID: 761 RVA: 0x00014669 File Offset: 0x00012869
		internal static string RdpViewer_Information_Resuming
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Information_Resuming", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x060002FA RID: 762 RVA: 0x0001467F File Offset: 0x0001287F
		internal static string RdpViewer_Information_ComponentServicing
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Information_ComponentServicing", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x060002FB RID: 763 RVA: 0x00014695 File Offset: 0x00012895
		internal static string RdpViewer_Information_VirtualMachineShieldedEnhancedSessionAvailable
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Information_VirtualMachineShieldedEnhancedSessionAvailable", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x060002FC RID: 764 RVA: 0x000146AB File Offset: 0x000128AB
		internal static string RdpViewer_Information_VirtualMachineShieldedEnhancedSessionDisabled
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Information_VirtualMachineShieldedEnhancedSessionDisabled", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x060002FD RID: 765 RVA: 0x000146C1 File Offset: 0x000128C1
		internal static string RdpViewer_Information_VirtualMachineShieldedEnhancedSessionEnabled
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Information_VirtualMachineShieldedEnhancedSessionEnabled", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x060002FE RID: 766 RVA: 0x000146D7 File Offset: 0x000128D7
		internal static string RdpViewer_Information_VMRestoring
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Information_VMRestoring", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x060002FF RID: 767 RVA: 0x000146ED File Offset: 0x000128ED
		internal static string RdpViewer_Informaton_InitiatedStarting
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Informaton_InitiatedStarting", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000300 RID: 768 RVA: 0x00014703 File Offset: 0x00012903
		internal static string RdpViewer_Informaton_VMStarting
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Informaton_VMStarting", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000301 RID: 769 RVA: 0x00014719 File Offset: 0x00012919
		internal static string RdpViewer_Resolution_CouldNotConnect
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Resolution_CouldNotConnect", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000302 RID: 770 RVA: 0x0001472F File Offset: 0x0001292F
		internal static string RdpViewer_Resolution_DisconnectedUnexpectedly
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Resolution_DisconnectedUnexpectedly", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000303 RID: 771 RVA: 0x00014745 File Offset: 0x00012945
		internal static string RdpViewer_Resolution_LogonFailed_Authorization
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Resolution_LogonFailed_Authorization", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000304 RID: 772 RVA: 0x0001475B File Offset: 0x0001295B
		internal static string RdpViewer_Resolution_NotStarted
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Resolution_NotStarted", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000305 RID: 773 RVA: 0x00014771 File Offset: 0x00012971
		internal static string RdpViewer_Resolution_VirtualMachineShieldedEnhancedSessionAvailable
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Resolution_VirtualMachineShieldedEnhancedSessionAvailable", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06000306 RID: 774 RVA: 0x00014787 File Offset: 0x00012987
		internal static string RdpViewer_Resolution_VirtualMachineShieldedEnhancedSessionDisabled
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Resolution_VirtualMachineShieldedEnhancedSessionDisabled", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000307 RID: 775 RVA: 0x0001479D File Offset: 0x0001299D
		internal static string RdpViewer_Resolution_VirtualMachineShieldedEnhancedSessionEnabled
		{
			get
			{
				return VMISResources.ResourceManager.GetString("RdpViewer_Resolution_VirtualMachineShieldedEnhancedSessionEnabled", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000308 RID: 776 RVA: 0x000147B3 File Offset: 0x000129B3
		internal static string StatusBar_Migrating
		{
			get
			{
				return VMISResources.ResourceManager.GetString("StatusBar_Migrating", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000309 RID: 777 RVA: 0x000147C9 File Offset: 0x000129C9
		internal static string StatusBar_MouseCaptured
		{
			get
			{
				return VMISResources.ResourceManager.GetString("StatusBar_MouseCaptured", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x0600030A RID: 778 RVA: 0x000147DF File Offset: 0x000129DF
		internal static string StatusBar_MouseNotCapturedInTerminalServicesSession
		{
			get
			{
				return VMISResources.ResourceManager.GetString("StatusBar_MouseNotCapturedInTerminalServicesSession", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x0600030B RID: 779 RVA: 0x000147F5 File Offset: 0x000129F5
		internal static string StatusBar_VMStateDisplay
		{
			get
			{
				return VMISResources.ResourceManager.GetString("StatusBar_VMStateDisplay", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x0600030C RID: 780 RVA: 0x0001480B File Offset: 0x00012A0B
		internal static string Task_Restore_Canceled
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_Restore_Canceled", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x0600030D RID: 781 RVA: 0x00014821 File Offset: 0x00012A21
		internal static string Task_Restore_Failed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_Restore_Failed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x0600030E RID: 782 RVA: 0x00014837 File Offset: 0x00012A37
		internal static string Task_Restore_Name
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_Restore_Name", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x0600030F RID: 783 RVA: 0x0001484D File Offset: 0x00012A4D
		internal static string Task_Restore_Succeeded
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_Restore_Succeeded", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000310 RID: 784 RVA: 0x00014863 File Offset: 0x00012A63
		internal static string Task_Revert_Canceled
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_Revert_Canceled", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000311 RID: 785 RVA: 0x00014879 File Offset: 0x00012A79
		internal static string Task_Revert_Failed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_Revert_Failed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000312 RID: 786 RVA: 0x0001488F File Offset: 0x00012A8F
		internal static string Task_Revert_Name
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_Revert_Name", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x06000313 RID: 787 RVA: 0x000148A5 File Offset: 0x00012AA5
		internal static string Task_Revert_Succeeded
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_Revert_Succeeded", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000314 RID: 788 RVA: 0x000148BB File Offset: 0x00012ABB
		internal static string Task_SaveState_Canceled
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_SaveState_Canceled", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000315 RID: 789 RVA: 0x000148D1 File Offset: 0x00012AD1
		internal static string Task_SaveState_Failed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_SaveState_Failed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000316 RID: 790 RVA: 0x000148E7 File Offset: 0x00012AE7
		internal static string Task_SaveState_Name
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_SaveState_Name", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000317 RID: 791 RVA: 0x000148FD File Offset: 0x00012AFD
		internal static string Task_SaveState_Succeeded
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_SaveState_Succeeded", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000318 RID: 792 RVA: 0x00014913 File Offset: 0x00012B13
		internal static string Task_ShareVM_Canceled
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_ShareVM_Canceled", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06000319 RID: 793 RVA: 0x00014929 File Offset: 0x00012B29
		internal static string Task_ShareVM_Failed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_ShareVM_Failed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x0600031A RID: 794 RVA: 0x0001493F File Offset: 0x00012B3F
		internal static string Task_ShareVM_Name
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_ShareVM_Name", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x0600031B RID: 795 RVA: 0x00014955 File Offset: 0x00012B55
		internal static string Task_ShareVM_Succeeded
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_ShareVM_Succeeded", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x0600031C RID: 796 RVA: 0x0001496B File Offset: 0x00012B6B
		internal static string Task_Shutdown_Canceled
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_Shutdown_Canceled", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x0600031D RID: 797 RVA: 0x00014981 File Offset: 0x00012B81
		internal static string Task_Shutdown_Failed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_Shutdown_Failed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x0600031E RID: 798 RVA: 0x00014997 File Offset: 0x00012B97
		internal static string Task_Shutdown_Name
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_Shutdown_Name", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x0600031F RID: 799 RVA: 0x000149AD File Offset: 0x00012BAD
		internal static string Task_Shutdown_Succeeded
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_Shutdown_Succeeded", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x06000320 RID: 800 RVA: 0x000149C3 File Offset: 0x00012BC3
		internal static string Task_TakeSnapshot_Canceled
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_TakeSnapshot_Canceled", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x06000321 RID: 801 RVA: 0x000149D9 File Offset: 0x00012BD9
		internal static string Task_TakeSnapshot_Failed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_TakeSnapshot_Failed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06000322 RID: 802 RVA: 0x000149EF File Offset: 0x00012BEF
		internal static string Task_TakeSnapshot_Name
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_TakeSnapshot_Name", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000323 RID: 803 RVA: 0x00014A05 File Offset: 0x00012C05
		internal static string Task_TakeSnapshot_Succeeded
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_TakeSnapshot_Succeeded", VMISResources.resourceCulture);
			}
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000324 RID: 804 RVA: 0x00014A1B File Offset: 0x00012C1B
		internal static string Task_TurnOff_Canceled
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_TurnOff_Canceled", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x06000325 RID: 805 RVA: 0x00014A31 File Offset: 0x00012C31
		internal static string Task_TurnOff_Failed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_TurnOff_Failed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000326 RID: 806 RVA: 0x00014A47 File Offset: 0x00012C47
		internal static string Task_TurnOff_Name
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_TurnOff_Name", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x06000327 RID: 807 RVA: 0x00014A5D File Offset: 0x00012C5D
		internal static string Task_TurnOff_Succeeded
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_TurnOff_Succeeded", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x06000328 RID: 808 RVA: 0x00014A73 File Offset: 0x00012C73
		internal static string Task_TurnOn_Canceled
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_TurnOn_Canceled", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x06000329 RID: 809 RVA: 0x00014A89 File Offset: 0x00012C89
		internal static string Task_TurnOn_Failed
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_TurnOn_Failed", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x0600032A RID: 810 RVA: 0x00014A9F File Offset: 0x00012C9F
		internal static string Task_TurnOn_Name
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_TurnOn_Name", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x0600032B RID: 811 RVA: 0x00014AB5 File Offset: 0x00012CB5
		internal static string Task_TurnOn_Succeeded
		{
			get
			{
				return VMISResources.ResourceManager.GetString("Task_TurnOn_Succeeded", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x0600032C RID: 812 RVA: 0x00014ACB File Offset: 0x00012CCB
		internal static string ToolStripToolTip_DisableEnhanced
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ToolStripToolTip_DisableEnhanced", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x0600032D RID: 813 RVA: 0x00014AE1 File Offset: 0x00012CE1
		internal static string ToolStripToolTip_EnableEnhanced
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ToolStripToolTip_EnableEnhanced", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x0600032E RID: 814 RVA: 0x00014AF7 File Offset: 0x00012CF7
		internal static string ToolStripToolTip_Pause
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ToolStripToolTip_Pause", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x0600032F RID: 815 RVA: 0x00014B0D File Offset: 0x00012D0D
		internal static string ToolStripToolTip_Reset
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ToolStripToolTip_Reset", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000330 RID: 816 RVA: 0x00014B23 File Offset: 0x00012D23
		internal static string ToolStripToolTip_Resume
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ToolStripToolTip_Resume", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000331 RID: 817 RVA: 0x00014B39 File Offset: 0x00012D39
		internal static string ToolStripToolTip_Revert
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ToolStripToolTip_Revert", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000332 RID: 818 RVA: 0x00014B4F File Offset: 0x00012D4F
		internal static string ToolStripToolTip_Sas
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ToolStripToolTip_Sas", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06000333 RID: 819 RVA: 0x00014B65 File Offset: 0x00012D65
		internal static string ToolStripToolTip_Save
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ToolStripToolTip_Save", VMISResources.resourceCulture);
			}
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000334 RID: 820 RVA: 0x00014B7B File Offset: 0x00012D7B
		internal static string ToolStripToolTip_Share
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ToolStripToolTip_Share", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000335 RID: 821 RVA: 0x00014B91 File Offset: 0x00012D91
		internal static string ToolStripToolTip_Shutdown
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ToolStripToolTip_Shutdown", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000336 RID: 822 RVA: 0x00014BA7 File Offset: 0x00012DA7
		internal static string ToolStripToolTip_Snapshot
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ToolStripToolTip_Snapshot", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000337 RID: 823 RVA: 0x00014BBD File Offset: 0x00012DBD
		internal static string ToolStripToolTip_Start
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ToolStripToolTip_Start", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06000338 RID: 824 RVA: 0x00014BD3 File Offset: 0x00012DD3
		internal static string ToolStripToolTip_TurnOff
		{
			get
			{
				return VMISResources.ResourceManager.GetString("ToolStripToolTip_TurnOff", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x06000339 RID: 825 RVA: 0x00014BE9 File Offset: 0x00012DE9
		internal static string VMDeleted
		{
			get
			{
				return VMISResources.ResourceManager.GetString("VMDeleted", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x0600033A RID: 826 RVA: 0x00014BFF File Offset: 0x00012DFF
		internal static string VMDeletedWhileClustered
		{
			get
			{
				return VMISResources.ResourceManager.GetString("VMDeletedWhileClustered", VMISResources.resourceCulture);
			}
		}

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x0600033B RID: 827 RVA: 0x00014C15 File Offset: 0x00012E15
		internal static string VMMigrated
		{
			get
			{
				return VMISResources.ResourceManager.GetString("VMMigrated", VMISResources.resourceCulture);
			}
		}

		// Token: 0x0400018F RID: 399
		private static ResourceManager resourceMan;

		// Token: 0x04000190 RID: 400
		private static CultureInfo resourceCulture;
	}
}
