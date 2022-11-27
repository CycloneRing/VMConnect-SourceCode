using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000032 RID: 50
	internal class ToolStripScaleAwareProfessionalRenderer : ToolStripProfessionalRenderer
	{
		// Token: 0x06000272 RID: 626 RVA: 0x00012ED4 File Offset: 0x000110D4
		protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
		{
			Rectangle rectangle = new Rectangle(e.ArrowRectangle.X + ToolStripScaleAwareProfessionalRenderer.ImageWidthScaleDelta, e.ArrowRectangle.Y, e.ArrowRectangle.Width + ToolStripScaleAwareProfessionalRenderer.ArrowWidthScaleDelta, e.ArrowRectangle.Height);
			using (Brush brush = new SolidBrush((e.ArrowColor == SystemColors.HighlightText) ? SystemColors.MenuText : e.ArrowColor))
			{
				Point point = new Point(rectangle.Left + rectangle.Width / 2, rectangle.Top + rectangle.Height / 2);
				int num = (int)Math.Ceiling((double)rectangle.Width / 2.5);
				int num2 = (int)Math.Ceiling((double)rectangle.Height / 2.5);
				int num3 = num / 2;
				int num4 = num2 / 2;
				ArrowDirection direction = e.Direction;
				Point[] points;
				if (direction != ArrowDirection.Left)
				{
					if (direction != ArrowDirection.Right)
					{
						throw new ArgumentOutOfRangeException("e", string.Format("'{0}' is an unsupported Direction value.", e.Direction));
					}
					points = new Point[]
					{
						new Point(point.X - num3, point.Y - num4),
						new Point(point.X - num3, point.Y + num4),
						new Point(point.X + num3, point.Y)
					};
				}
				else
				{
					points = new Point[]
					{
						new Point(point.X + num3, point.Y - num4),
						new Point(point.X + num3, point.Y + num4),
						new Point(point.X - num3, point.Y)
					};
				}
				e.Graphics.FillPolygon(brush, points);
			}
		}

		// Token: 0x06000273 RID: 627 RVA: 0x000130F4 File Offset: 0x000112F4
		protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
		{
			using (Pen pen = new Pen(SystemColors.ControlDark))
			{
				e.Graphics.DrawLine(pen, e.ToolStrip.Padding.Left + ToolStripScaleAwareProfessionalRenderer.ImageWidthScaleDelta, e.Item.Size.Height / 2, e.Item.Size.Width - e.ToolStrip.Padding.Right, e.Item.Size.Height / 2);
			}
		}

		// Token: 0x06000274 RID: 628 RVA: 0x000131A0 File Offset: 0x000113A0
		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			if (e.Item.IsOnDropDown)
			{
				ToolStripItemTextRenderEventArgs e2 = new ToolStripItemTextRenderEventArgs(e.Graphics, e.Item, e.Text, new Rectangle(e.TextRectangle.X + ToolStripScaleAwareProfessionalRenderer.ImageWidthScaleDelta, e.TextRectangle.Y, e.TextRectangle.Width, e.TextRectangle.Height), e.TextColor, e.TextFont, e.TextFormat);
				base.OnRenderItemText(e2);
				return;
			}
			base.OnRenderItemText(e);
		}

		// Token: 0x06000275 RID: 629 RVA: 0x00013238 File Offset: 0x00011438
		protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
		{
			ToolStripRenderEventArgs e2 = new ToolStripRenderEventArgs(e.Graphics, e.ToolStrip, new Rectangle(e.AffectedBounds.X + ToolStripScaleAwareProfessionalRenderer.ImageWidthScaleDelta, e.AffectedBounds.Y, e.AffectedBounds.Width, e.AffectedBounds.Height), e.BackColor);
			base.OnRenderImageMargin(e2);
		}

		// Token: 0x06000276 RID: 630 RVA: 0x000132A8 File Offset: 0x000114A8
		protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
		{
			if (e.ImageRectangle != Rectangle.Empty && e.Image != null)
			{
				base.OnRenderItemCheck(new ToolStripItemImageRenderEventArgs(e.Graphics, e.Item, null, new Rectangle(e.ImageRectangle.X, e.ImageRectangle.X, DpiUtilities.LogicalToDeviceUnits(e.ImageRectangle.Width), DpiUtilities.LogicalToDeviceUnits(e.ImageRectangle.Height))));
				e.Graphics.DrawImage(e.Image, new Rectangle(e.ImageRectangle.X, e.ImageRectangle.X, DpiUtilities.LogicalToDeviceUnits(e.ImageRectangle.Width), DpiUtilities.LogicalToDeviceUnits(e.ImageRectangle.Height)), new Rectangle(Point.Empty, e.Image.Size), GraphicsUnit.Pixel);
				return;
			}
			base.OnRenderItemCheck(e);
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000277 RID: 631 RVA: 0x000133AA File Offset: 0x000115AA
		public static int ToolStripItemWidthScaleDelta
		{
			get
			{
				return ToolStripScaleAwareProfessionalRenderer.ImageWidthScaleDelta + ToolStripScaleAwareProfessionalRenderer.ArrowWidthScaleDelta;
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000278 RID: 632 RVA: 0x000133B7 File Offset: 0x000115B7
		private static int ImageWidthScaleDelta
		{
			get
			{
				return DpiUtilities.LogicalToDeviceUnits(16) - 16;
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000279 RID: 633 RVA: 0x000133C3 File Offset: 0x000115C3
		private static int ArrowWidthScaleDelta
		{
			get
			{
				return (DpiUtilities.LogicalToDeviceUnits(4) - 4) * 2;
			}
		}

		// Token: 0x0400017B RID: 379
		private const int DefaultToolStripImageWidth = 16;

		// Token: 0x0400017C RID: 380
		private const int DefaultArrowWidth = 4;
	}
}
