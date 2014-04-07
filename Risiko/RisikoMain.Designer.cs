namespace Risiko
{
    partial class RisikoMain
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.dateiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spielSpeichernToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bearbeitenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.einstellungenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.einstellungenToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDrawMap = new System.Windows.Forms.Button();
            this.btnEndMoveAttack = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnTest1 = new System.Windows.Forms.Button();
            this.timerDeleteMessage = new System.Windows.Forms.Timer(this.components);
            this.btnOptions = new System.Windows.Forms.Button();
            this.btnTest2 = new System.Windows.Forms.Button();
            this.pBUnits = new ExtendedDotNET.Controls.Progress.ProgressBar();
            this.timerAttack = new System.Windows.Forms.Timer(this.components);
            this.pnlMap = new Risiko.DoubleBufferedPanel();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.White;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dateiToolStripMenuItem,
            this.bearbeitenToolStripMenuItem,
            this.einstellungenToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(606, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // dateiToolStripMenuItem
            // 
            this.dateiToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.spielSpeichernToolStripMenuItem});
            this.dateiToolStripMenuItem.Name = "dateiToolStripMenuItem";
            this.dateiToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.dateiToolStripMenuItem.Text = "Datei";
            // 
            // spielSpeichernToolStripMenuItem
            // 
            this.spielSpeichernToolStripMenuItem.Name = "spielSpeichernToolStripMenuItem";
            this.spielSpeichernToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.spielSpeichernToolStripMenuItem.Text = "Spiel speichern";
            this.spielSpeichernToolStripMenuItem.Click += new System.EventHandler(this.spielSpeichernToolStripMenuItem_Click);
            // 
            // bearbeitenToolStripMenuItem
            // 
            this.bearbeitenToolStripMenuItem.Name = "bearbeitenToolStripMenuItem";
            this.bearbeitenToolStripMenuItem.Size = new System.Drawing.Size(75, 20);
            this.bearbeitenToolStripMenuItem.Text = "Bearbeiten";
            // 
            // einstellungenToolStripMenuItem
            // 
            this.einstellungenToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.einstellungenToolStripMenuItem1});
            this.einstellungenToolStripMenuItem.Name = "einstellungenToolStripMenuItem";
            this.einstellungenToolStripMenuItem.Size = new System.Drawing.Size(90, 20);
            this.einstellungenToolStripMenuItem.Text = "Einstellungen";
            // 
            // einstellungenToolStripMenuItem1
            // 
            this.einstellungenToolStripMenuItem1.Name = "einstellungenToolStripMenuItem1";
            this.einstellungenToolStripMenuItem1.Size = new System.Drawing.Size(234, 22);
            this.einstellungenToolStripMenuItem1.Text = "Automatische Landerkennung";
            this.einstellungenToolStripMenuItem1.Click += new System.EventHandler(this.einstellungenToolStripMenuItem1_Click);
            // 
            // btnDrawMap
            // 
            this.btnDrawMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDrawMap.BackColor = System.Drawing.Color.White;
            this.btnDrawMap.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.btnDrawMap.Location = new System.Drawing.Point(12, 469);
            this.btnDrawMap.Name = "btnDrawMap";
            this.btnDrawMap.Size = new System.Drawing.Size(99, 23);
            this.btnDrawMap.TabIndex = 2;
            this.btnDrawMap.Text = "Karte zeichnen";
            this.btnDrawMap.UseVisualStyleBackColor = false;
            this.btnDrawMap.Click += new System.EventHandler(this.btnDrawMap_Click);
            // 
            // btnEndMoveAttack
            // 
            this.btnEndMoveAttack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEndMoveAttack.BackColor = System.Drawing.Color.White;
            this.btnEndMoveAttack.Location = new System.Drawing.Point(519, 405);
            this.btnEndMoveAttack.Name = "btnEndMoveAttack";
            this.btnEndMoveAttack.Size = new System.Drawing.Size(75, 23);
            this.btnEndMoveAttack.TabIndex = 3;
            this.btnEndMoveAttack.Text = "Zug beenden";
            this.btnEndMoveAttack.UseVisualStyleBackColor = false;
            this.btnEndMoveAttack.Click += new System.EventHandler(this.btnEndMove_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(21, 410);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(30, 13);
            this.lblMessage.TabIndex = 4;
            this.lblMessage.Text = "(leer)";
            // 
            // btnTest1
            // 
            this.btnTest1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest1.BackColor = System.Drawing.Color.White;
            this.btnTest1.Location = new System.Drawing.Point(519, 468);
            this.btnTest1.Name = "btnTest1";
            this.btnTest1.Size = new System.Drawing.Size(75, 23);
            this.btnTest1.TabIndex = 5;
            this.btnTest1.Text = "Test";
            this.btnTest1.UseVisualStyleBackColor = false;
            this.btnTest1.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // timerDeleteMessage
            // 
            this.timerDeleteMessage.Interval = 10000;
            this.timerDeleteMessage.Tick += new System.EventHandler(this.timerDeleteMessage_Tick);
            // 
            // btnOptions
            // 
            this.btnOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOptions.BackColor = System.Drawing.Color.White;
            this.btnOptions.BackgroundImage = global::Risiko.Properties.Resources.DoppelpfeilGrossSchwarz;
            this.btnOptions.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnOptions.Location = new System.Drawing.Point(485, 405);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(28, 23);
            this.btnOptions.TabIndex = 7;
            this.btnOptions.UseVisualStyleBackColor = false;
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            this.btnOptions.MouseEnter += new System.EventHandler(this.btnOptions_MouseEnter);
            // 
            // btnTest2
            // 
            this.btnTest2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest2.BackColor = System.Drawing.Color.White;
            this.btnTest2.Location = new System.Drawing.Point(429, 469);
            this.btnTest2.Name = "btnTest2";
            this.btnTest2.Size = new System.Drawing.Size(75, 23);
            this.btnTest2.TabIndex = 8;
            this.btnTest2.Text = "Test2";
            this.btnTest2.UseVisualStyleBackColor = false;
            this.btnTest2.Click += new System.EventHandler(this.btnTest2_Click);
            // 
            // pBUnits
            // 
            this.pBUnits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pBUnits.BackColor = System.Drawing.Color.White;
            this.pBUnits.BarOffset = 1;
            this.pBUnits.Caption = "Progress";
            this.pBUnits.CaptionColor = System.Drawing.Color.Black;
            this.pBUnits.CaptionMode = ExtendedDotNET.Controls.Progress.ProgressCaptionMode.Value;
            this.pBUnits.CaptionShadowColor = System.Drawing.Color.White;
            this.pBUnits.ChangeByMouse = false;
            this.pBUnits.DashSpace = 0;
            this.pBUnits.DashWidth = 5;
            this.pBUnits.Edge = ExtendedDotNET.Controls.Progress.ProgressBarEdge.Rounded;
            this.pBUnits.EdgeColor = System.Drawing.Color.Gray;
            this.pBUnits.EdgeLightColor = System.Drawing.Color.LightGray;
            this.pBUnits.EdgeWidth = 1;
            this.pBUnits.FloodPercentage = 0F;
            this.pBUnits.FloodStyle = ExtendedDotNET.Controls.Progress.ProgressFloodStyle.Horizontal;
            this.pBUnits.ForeColor = System.Drawing.Color.Yellow;
            this.pBUnits.Invert = false;
            this.pBUnits.Location = new System.Drawing.Point(305, 404);
            this.pBUnits.MainColor = System.Drawing.Color.Red;
            this.pBUnits.Maximum = 100;
            this.pBUnits.Minimum = 0;
            this.pBUnits.Name = "pBUnits";
            this.pBUnits.Orientation = ExtendedDotNET.Controls.Progress.ProgressBarDirection.Horizontal;
            this.pBUnits.ProgressBackColor = System.Drawing.Color.White;
            this.pBUnits.ProgressBarStyle = ExtendedDotNET.Controls.Progress.ProgressStyle.Solid;
            this.pBUnits.SecondColor = System.Drawing.Color.White;
            this.pBUnits.Shadow = true;
            this.pBUnits.ShadowOffset = 1;
            this.pBUnits.Size = new System.Drawing.Size(174, 24);
            this.pBUnits.Step = 1;
            this.pBUnits.TabIndex = 9;
            this.pBUnits.TextAntialias = true;
            this.pBUnits.Value = 100;
            // 
            // timerAttack
            // 
            this.timerAttack.Interval = 1000;
            // 
            // pnlMap
            // 
            this.pnlMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMap.BackColor = System.Drawing.Color.White;
            this.pnlMap.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pnlMap.Location = new System.Drawing.Point(12, 27);
            this.pnlMap.Name = "pnlMap";
            this.pnlMap.Size = new System.Drawing.Size(582, 372);
            this.pnlMap.TabIndex = 1;
            this.pnlMap.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pnlMap_MouseClick);
            this.pnlMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlMap_MouseMove);
            this.pnlMap.Resize += new System.EventHandler(this.ResizeMap);
            // 
            // RisikoMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(606, 503);
            this.Controls.Add(this.pBUnits);
            this.Controls.Add(this.btnTest2);
            this.Controls.Add(this.btnOptions);
            this.Controls.Add(this.btnTest1);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnEndMoveAttack);
            this.Controls.Add(this.btnDrawMap);
            this.Controls.Add(this.pnlMap);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "RisikoMain";
            this.Text = "Risiko";
            this.Load += new System.EventHandler(this.RisikoMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.MenuStrip menuStrip1;
        internal System.Windows.Forms.ToolStripMenuItem dateiToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem bearbeitenToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem einstellungenToolStripMenuItem;
        //internal System.Windows.Forms.Panel pnlMap;
        internal DoubleBufferedPanel pnlMap;
        internal System.Windows.Forms.Button btnDrawMap;
        internal System.Windows.Forms.Button btnEndMoveAttack;
        internal System.Windows.Forms.Label lblMessage;
        internal System.Windows.Forms.Button btnTest1;
        internal System.Windows.Forms.ToolStripMenuItem einstellungenToolStripMenuItem1;
        internal System.Windows.Forms.Button btnOptions;
        internal System.Windows.Forms.Timer timerDeleteMessage;
        internal System.Windows.Forms.Button btnTest2;
        private System.ComponentModel.IContainer components;
        internal ExtendedDotNET.Controls.Progress.ProgressBar pBUnits;
        private System.Windows.Forms.Timer timerAttack;
        private System.Windows.Forms.ToolStripMenuItem spielSpeichernToolStripMenuItem;
    }
}

