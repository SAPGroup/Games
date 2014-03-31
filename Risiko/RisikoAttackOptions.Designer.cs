namespace Risiko
{
    partial class RisikoAttackOptions
    {

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        internal void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rBAllMen = new System.Windows.Forms.RadioButton();
            this.gBAttackersOptions = new System.Windows.Forms.GroupBox();
            this.numUDCustomAttackers = new System.Windows.Forms.NumericUpDown();
            this.rBCustomAttackers = new System.Windows.Forms.RadioButton();
            this.rB3EveryTime = new System.Windows.Forms.RadioButton();
            this.timerClose = new System.Windows.Forms.Timer(this.components);
            this.cBStopAttacking = new System.Windows.Forms.CheckBox();
            this.lblTextStopAttacking1 = new System.Windows.Forms.Label();
            this.numUDStopPercentage = new System.Windows.Forms.NumericUpDown();
            this.lblTextStopAttacking0 = new System.Windows.Forms.Label();
            this.gBAttackersOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUDCustomAttackers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUDStopPercentage)).BeginInit();
            this.SuspendLayout();
            // 
            // rBAllMen
            // 
            this.rBAllMen.AutoSize = true;
            this.rBAllMen.Location = new System.Drawing.Point(6, 19);
            this.rBAllMen.Name = "rBAllMen";
            this.rBAllMen.Size = new System.Drawing.Size(91, 21);
            this.rBAllMen.TabIndex = 0;
            this.rBAllMen.TabStop = true;
            this.rBAllMen.Text = "Bis zum Tod";
            this.rBAllMen.UseVisualStyleBackColor = true;
            this.rBAllMen.CheckedChanged += new System.EventHandler(this.rBAllMen_CheckedChanged);
            // 
            // gBAttackersOptions
            // 
            this.gBAttackersOptions.Controls.Add(this.numUDCustomAttackers);
            this.gBAttackersOptions.Controls.Add(this.rBCustomAttackers);
            this.gBAttackersOptions.Controls.Add(this.rB3EveryTime);
            this.gBAttackersOptions.Controls.Add(this.rBAllMen);
            this.gBAttackersOptions.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gBAttackersOptions.ForeColor = System.Drawing.Color.White;
            this.gBAttackersOptions.Location = new System.Drawing.Point(12, 12);
            this.gBAttackersOptions.Name = "gBAttackersOptions";
            this.gBAttackersOptions.Size = new System.Drawing.Size(218, 95);
            this.gBAttackersOptions.TabIndex = 1;
            this.gBAttackersOptions.TabStop = false;
            this.gBAttackersOptions.Text = "Einheiten die Angreifen";
            // 
            // numUDCustomAttackers
            // 
            this.numUDCustomAttackers.Location = new System.Drawing.Point(140, 65);
            this.numUDCustomAttackers.Name = "numUDCustomAttackers";
            this.numUDCustomAttackers.Size = new System.Drawing.Size(72, 22);
            this.numUDCustomAttackers.TabIndex = 3;
            // 
            // rBCustomAttackers
            // 
            this.rBCustomAttackers.AutoSize = true;
            this.rBCustomAttackers.Location = new System.Drawing.Point(6, 65);
            this.rBCustomAttackers.Name = "rBCustomAttackers";
            this.rBCustomAttackers.Size = new System.Drawing.Size(108, 21);
            this.rBCustomAttackers.TabIndex = 2;
            this.rBCustomAttackers.TabStop = true;
            this.rBCustomAttackers.Text = "Eigene Anzahl";
            this.rBCustomAttackers.UseVisualStyleBackColor = true;
            this.rBCustomAttackers.CheckedChanged += new System.EventHandler(this.rBCustomAttackers_CheckedChanged);
            // 
            // rB3EveryTime
            // 
            this.rB3EveryTime.AutoSize = true;
            this.rB3EveryTime.Location = new System.Drawing.Point(6, 42);
            this.rB3EveryTime.Name = "rB3EveryTime";
            this.rB3EveryTime.Size = new System.Drawing.Size(73, 21);
            this.rB3EveryTime.TabIndex = 1;
            this.rB3EveryTime.TabStop = true;
            this.rB3EveryTime.Text = "Immer 3";
            this.rB3EveryTime.UseVisualStyleBackColor = true;
            // 
            // timerClose
            // 
            this.timerClose.Interval = 2000;
            this.timerClose.Tick += new System.EventHandler(this.timerClose_Tick);
            // 
            // cBStopAttacking
            // 
            this.cBStopAttacking.AutoSize = true;
            this.cBStopAttacking.Location = new System.Drawing.Point(12, 113);
            this.cBStopAttacking.Name = "cBStopAttacking";
            this.cBStopAttacking.Size = new System.Drawing.Size(15, 14);
            this.cBStopAttacking.TabIndex = 2;
            this.cBStopAttacking.UseVisualStyleBackColor = true;
            this.cBStopAttacking.CheckedChanged += new System.EventHandler(this.cBStopAttacking_CheckedChanged);
            // 
            // lblTextStopAttacking1
            // 
            this.lblTextStopAttacking1.AutoSize = true;
            this.lblTextStopAttacking1.Location = new System.Drawing.Point(81, 138);
            this.lblTextStopAttacking1.Name = "lblTextStopAttacking1";
            this.lblTextStopAttacking1.Size = new System.Drawing.Size(198, 13);
            this.lblTextStopAttacking1.TabIndex = 3;
            this.lblTextStopAttacking1.Text = "% der Einheiten vom Anfang des Angriffs";
            // 
            // numUDStopPercentage
            // 
            this.numUDStopPercentage.Location = new System.Drawing.Point(32, 136);
            this.numUDStopPercentage.Name = "numUDStopPercentage";
            this.numUDStopPercentage.Size = new System.Drawing.Size(43, 20);
            this.numUDStopPercentage.TabIndex = 4;
            // 
            // lblTextStopAttacking0
            // 
            this.lblTextStopAttacking0.AutoSize = true;
            this.lblTextStopAttacking0.Location = new System.Drawing.Point(34, 114);
            this.lblTextStopAttacking0.Name = "lblTextStopAttacking0";
            this.lblTextStopAttacking0.Size = new System.Drawing.Size(176, 13);
            this.lblTextStopAttacking0.TabIndex = 5;
            this.lblTextStopAttacking0.Text = "Angriff unterbrechen bei weniger als";
            // 
            // RisikoAttackOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.ClientSize = new System.Drawing.Size(285, 194);
            this.Controls.Add(this.lblTextStopAttacking0);
            this.Controls.Add(this.numUDStopPercentage);
            this.Controls.Add(this.lblTextStopAttacking1);
            this.Controls.Add(this.cBStopAttacking);
            this.Controls.Add(this.gBAttackersOptions);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RisikoAttackOptions";
            this.Opacity = 0.85D;
            this.Text = "RisikoAttackOptions";
            this.Deactivate += new System.EventHandler(this.RisikoAttackOptionsDeactivate);
            this.Load += new System.EventHandler(this.RisikoAttackOptions_Load);
            this.MouseEnter += new System.EventHandler(this.RisikoAttackOptions_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.RisikoAttackOptions_MouseLeave);
            this.gBAttackersOptions.ResumeLayout(false);
            this.gBAttackersOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUDCustomAttackers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUDStopPercentage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.RadioButton rBAllMen;
        internal System.Windows.Forms.GroupBox gBAttackersOptions;
        internal System.Windows.Forms.NumericUpDown numUDCustomAttackers;
        internal System.Windows.Forms.RadioButton rBCustomAttackers;
        internal System.Windows.Forms.RadioButton rB3EveryTime;
        private System.Windows.Forms.Timer timerClose;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.CheckBox cBStopAttacking;
        private System.Windows.Forms.Label lblTextStopAttacking1;
        private System.Windows.Forms.NumericUpDown numUDStopPercentage;
        private System.Windows.Forms.Label lblTextStopAttacking0;

    }
}