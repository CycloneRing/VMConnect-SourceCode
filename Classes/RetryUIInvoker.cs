using System;
using System.Windows.Forms;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000018 RID: 24
	internal class RetryUIInvoker
	{
		// Token: 0x14000012 RID: 18
		// (add) Token: 0x06000181 RID: 385 RVA: 0x0000E3B4 File Offset: 0x0000C5B4
		// (remove) Token: 0x06000182 RID: 386 RVA: 0x0000E3EC File Offset: 0x0000C5EC
		private event RetryUIInvoker.TickHandler m_TickEvent;

		// Token: 0x06000183 RID: 387 RVA: 0x0000E424 File Offset: 0x0000C624
		public RetryUIInvoker(RetryUIInvoker.TickHandler functionToRetry, TimeSpan interval, TimeSpan timeout)
		{
			this.m_Timer = new Timer();
			this.m_Timer.Interval = (int)interval.TotalMilliseconds;
			this.m_Timer.Tick += this.HandleTimerTick;
			this.m_Timeout = timeout;
			this.m_TickEvent = functionToRetry;
		}

		// Token: 0x06000184 RID: 388 RVA: 0x0000E485 File Offset: 0x0000C685
		public void Invoke()
		{
			this.m_StartTime = DateTime.Now;
			this.m_Timer.Start();
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000E4A0 File Offset: 0x0000C6A0
		private void HandleTimerTick(object sender, EventArgs args)
		{
			object timerLock = this.m_TimerLock;
			lock (timerLock)
			{
				if (DateTime.Now - this.m_StartTime > this.m_Timeout || this.m_Success)
				{
					this.m_Timer.Stop();
					this.m_Timer.Dispose();
				}
				else
				{
					this.m_Success = this.m_TickEvent();
				}
			}
		}

		// Token: 0x040000F8 RID: 248
		private Timer m_Timer;

		// Token: 0x040000F9 RID: 249
		private readonly object m_TimerLock = new object();

		// Token: 0x040000FA RID: 250
		private bool m_Success;

		// Token: 0x040000FB RID: 251
		private DateTime m_StartTime;

		// Token: 0x040000FD RID: 253
		private TimeSpan m_Timeout;

		// Token: 0x02000047 RID: 71
		// (Invoke) Token: 0x060003E2 RID: 994
		public delegate bool TickHandler();
	}
}
