
namespace SuperFileTransferClient
{
    partial class ComputerControl
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblAddr = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblAddr
            // 
            this.lblAddr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAddr.Location = new System.Drawing.Point(0, 0);
            this.lblAddr.Name = "lblAddr";
            this.lblAddr.Size = new System.Drawing.Size(148, 41);
            this.lblAddr.TabIndex = 0;
            this.lblAddr.Text = "label1";
            this.lblAddr.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPort
            // 
            this.lblPort.Location = new System.Drawing.Point(0, 67);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(147, 34);
            this.lblPort.TabIndex = 1;
            this.lblPort.Text = "label2";
            this.lblPort.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ComputerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.lblAddr);
            this.Name = "ComputerControl";
            this.Size = new System.Drawing.Size(148, 148);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblAddr;
        private System.Windows.Forms.Label lblPort;
    }
}
