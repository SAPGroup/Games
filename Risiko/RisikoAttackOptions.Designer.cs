namespace Risiko
{
    partial class RisikoAttackOptions
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        internal System.ComponentModel.IContainer components = null;

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
            this.rBAllMen = new System.Windows.Forms.RadioButton();
            this.gBAttackersOptions = new System.Windows.Forms.GroupBox();
            this.rB3EveryTime = new System.Windows.Forms.RadioButton();
            this.rBCustomAttackers = new System.Windows.Forms.RadioButton();
            this.numUDCustomAttackers = new System.Windows.Forms.NumericUpDown();
            this.gBAttackersOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUDCustomAttackers)).BeginInit();
            this.SuspendLayout();
            // 
            // rBAllMen
            // 
            this.rBAllMen.AutoSize = true;
            this.rBAllMen.Location = new System.Drawing.Point(6, 19);
            this.rBAllMen.Name = "rBAllMen";
            this.rBAllMen.Size = new System.Drawing.Size(83, 17);
            this.rBAllMen.TabIndex = 0;
            this.rBAllMen.TabStop = true;
            this.rBAllMen.Text = "Bis zum Tod";
            this.rBAllMen.UseVisualStyleBackColor = true;
            // 
            // gBAttackersOptions
            // 
            this.gBAttackersOptions.Controls.Add(this.numUDCustomAttackers);
            this.gBAttackersOptions.Controls.Add(this.rBCustomAttackers);
            this.gBAttackersOptions.Controls.Add(this.rB3EveryTime);
            this.gBAttackersOptions.Controls.Add(this.rBAllMen);
            this.gBAttackersOptions.Location = new System.Drawing.Point(12, 12);
            this.gBAttackersOptions.Name = "gBAttackersOptions";
            this.gBAttackersOptions.Size = new System.Drawing.Size(218, 100);
            this.gBAttackersOptions.TabIndex = 1;
            this.gBAttackersOptions.TabStop = false;
            this.gBAttackersOptions.Text = "Männer die Angreifen";
            // 
            // rB3EveryTime
            // 
            this.rB3EveryTime.AutoSize = true;
            this.rB3EveryTime.Location = new System.Drawing.Point(6, 42);
            this.rB3EveryTime.Name = "rB3EveryTime";
            this.rB3EveryTime.Size = new System.Drawing.Size(62, 17);
            this.rB3EveryTime.TabIndex = 1;
            this.rB3EveryTime.TabStop = true;
            this.rB3EveryTime.Text = "Immer 3";
            this.rB3EveryTime.UseVisualStyleBackColor = true;
            // 
            // rBCustomAttackers
            // 
            this.rBCustomAttackers.AutoSize = true;
            this.rBCustomAttackers.Location = new System.Drawing.Point(6, 65);
            this.rBCustomAttackers.Name = "rBCustomAttackers";
            this.rBCustomAttackers.Size = new System.Drawing.Size(93, 17);
            this.rBCustomAttackers.TabIndex = 2;
            this.rBCustomAttackers.TabStop = true;
            this.rBCustomAttackers.Text = "Eigene Anzahl";
            this.rBCustomAttackers.UseVisualStyleBackColor = true;
            this.rBCustomAttackers.CheckedChanged += new System.EventHandler(this.rBCustomAttackers_CheckedChanged);
            // 
            // numUDCustomAttackers
            // 
            this.numUDCustomAttackers.Location = new System.Drawing.Point(140, 65);
            this.numUDCustomAttackers.Name = "numUDCustomAttackers";
            this.numUDCustomAttackers.Size = new System.Drawing.Size(72, 20);
            this.numUDCustomAttackers.TabIndex = 3;
            // 
            // RisikoAttackOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(251)))));
            this.ClientSize = new System.Drawing.Size(242, 227);
            this.Controls.Add(this.gBAttackersOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RisikoAttackOptions";
            this.Text = "RisikoAttackOptions";
            this.Deactivate += new System.EventHandler(this.RisikoAttackOptionsDeactivate);
            this.Load += new System.EventHandler(this.RisikoAttackOptions_Load);
            this.gBAttackersOptions.ResumeLayout(false);
            this.gBAttackersOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUDCustomAttackers)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.RadioButton rBAllMen;
        internal System.Windows.Forms.GroupBox gBAttackersOptions;
        internal System.Windows.Forms.NumericUpDown numUDCustomAttackers;
        internal System.Windows.Forms.RadioButton rBCustomAttackers;
        internal System.Windows.Forms.RadioButton rB3EveryTime;

    }
}