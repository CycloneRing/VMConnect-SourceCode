namespace Microsoft.Virtualization.Client.InteractiveSession
{
	// Token: 0x02000007 RID: 7
	public partial class CustomZoomDialog : global::System.Windows.Forms.Form
	{
		// Token: 0x0600004F RID: 79 RVA: 0x00004E8C File Offset: 0x0000308C
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00004EAC File Offset: 0x000030AC
		private void InitializeComponent()
		{
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::Microsoft.Virtualization.Client.InteractiveSession.CustomZoomDialog));
			this.radioButton1 = new global::System.Windows.Forms.RadioButton();
			this.radioButton2 = new global::System.Windows.Forms.RadioButton();
			this.radioButton3 = new global::System.Windows.Forms.RadioButton();
			this.radioButton4 = new global::System.Windows.Forms.RadioButton();
			this.radioButton5 = new global::System.Windows.Forms.RadioButton();
			this.radioButton6 = new global::System.Windows.Forms.RadioButton();
			this.radioButton7 = new global::System.Windows.Forms.RadioButton();
			this.customUpDown = new global::System.Windows.Forms.NumericUpDown();
			this.buttonOK = new global::System.Windows.Forms.Button();
			this.buttonCancel = new global::System.Windows.Forms.Button();
			this.label1 = new global::System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new global::System.Windows.Forms.TableLayoutPanel();
			((global::System.ComponentModel.ISupportInitialize)this.customUpDown).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.radioButton1, "radioButton1");
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.TabStop = true;
			this.radioButton1.UseVisualStyleBackColor = true;
			this.radioButton1.CheckedChanged += new global::System.EventHandler(this.RadioButtonCheckedChanged);
			componentResourceManager.ApplyResources(this.radioButton2, "radioButton2");
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.TabStop = true;
			this.radioButton2.UseVisualStyleBackColor = true;
			this.radioButton2.CheckedChanged += new global::System.EventHandler(this.RadioButtonCheckedChanged);
			componentResourceManager.ApplyResources(this.radioButton3, "radioButton3");
			this.radioButton3.Name = "radioButton3";
			this.radioButton3.TabStop = true;
			this.radioButton3.UseVisualStyleBackColor = true;
			this.radioButton3.CheckedChanged += new global::System.EventHandler(this.RadioButtonCheckedChanged);
			componentResourceManager.ApplyResources(this.radioButton4, "radioButton4");
			this.radioButton4.Name = "radioButton4";
			this.radioButton4.TabStop = true;
			this.radioButton4.UseVisualStyleBackColor = true;
			this.radioButton4.CheckedChanged += new global::System.EventHandler(this.RadioButtonCheckedChanged);
			componentResourceManager.ApplyResources(this.customUpDown, "customUpDown");
			global::System.Windows.Forms.NumericUpDown numericUpDown = this.customUpDown;
			int[] array = new int[4];
			array[0] = 200;
			numericUpDown.Maximum = new decimal(array);
			global::System.Windows.Forms.NumericUpDown numericUpDown2 = this.customUpDown;
			int[] array2 = new int[4];
			array2[0] = 10;
			numericUpDown2.Minimum = new decimal(array2);
			this.customUpDown.Name = "customUpDown";
			global::System.Windows.Forms.NumericUpDown numericUpDown3 = this.customUpDown;
			int[] array3 = new int[4];
			array3[0] = 10;
			numericUpDown3.Value = new decimal(array3);
			this.customUpDown.ValueChanged += new global::System.EventHandler(this.CustomUpDown_ValueChanged);
			componentResourceManager.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new global::System.EventHandler(this.ButtonOK_Click);
			this.buttonCancel.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			componentResourceManager.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			componentResourceManager.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.customUpDown, 1, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			componentResourceManager.ApplyResources(this.radioButton5, "radioButton5");
			this.radioButton5.Name = "radioButton5";
			this.radioButton5.TabStop = true;
			this.radioButton5.UseVisualStyleBackColor = true;
			this.radioButton5.CheckedChanged += new global::System.EventHandler(this.RadioButtonCheckedChanged);
			componentResourceManager.ApplyResources(this.radioButton6, "radioButton6");
			this.radioButton6.Name = "radioButton6";
			this.radioButton6.TabStop = true;
			this.radioButton6.UseVisualStyleBackColor = true;
			this.radioButton6.CheckedChanged += new global::System.EventHandler(this.RadioButtonCheckedChanged);
			componentResourceManager.ApplyResources(this.radioButton7, "radioButton7");
			this.radioButton7.Name = "radioButton7";
			this.radioButton7.TabStop = true;
			this.radioButton7.UseVisualStyleBackColor = true;
			this.radioButton7.CheckedChanged += new global::System.EventHandler(this.RadioButtonCheckedChanged);
			base.AcceptButton = this.buttonOK;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.buttonCancel;
			base.Controls.Add(this.tableLayoutPanel1);
			base.Controls.Add(this.buttonCancel);
			base.Controls.Add(this.buttonOK);
			base.Controls.Add(this.radioButton7);
			base.Controls.Add(this.radioButton6);
			base.Controls.Add(this.radioButton5);
			base.Controls.Add(this.radioButton4);
			base.Controls.Add(this.radioButton3);
			base.Controls.Add(this.radioButton2);
			base.Controls.Add(this.radioButton1);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "CustomZoomDialog";
			base.ShowInTaskbar = false;
			((global::System.ComponentModel.ISupportInitialize)this.customUpDown).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x04000041 RID: 65
		private global::System.ComponentModel.IContainer components;

		// Token: 0x04000042 RID: 66
		private global::System.Windows.Forms.RadioButton radioButton1;

		// Token: 0x04000043 RID: 67
		private global::System.Windows.Forms.RadioButton radioButton2;

		// Token: 0x04000044 RID: 68
		private global::System.Windows.Forms.RadioButton radioButton3;

		// Token: 0x04000045 RID: 69
		private global::System.Windows.Forms.RadioButton radioButton4;

		// Token: 0x04000046 RID: 70
		private global::System.Windows.Forms.RadioButton radioButton5;

		// Token: 0x04000047 RID: 71
		private global::System.Windows.Forms.RadioButton radioButton6;

		// Token: 0x04000048 RID: 72
		private global::System.Windows.Forms.RadioButton radioButton7;

		// Token: 0x04000049 RID: 73
		private global::System.Windows.Forms.NumericUpDown customUpDown;

		// Token: 0x0400004A RID: 74
		private global::System.Windows.Forms.Button buttonOK;

		// Token: 0x0400004B RID: 75
		private global::System.Windows.Forms.Button buttonCancel;

		// Token: 0x0400004C RID: 76
		private global::System.Windows.Forms.Label label1;

		// Token: 0x0400004D RID: 77
		private global::System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}
