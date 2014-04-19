using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using MySql.Data.MySqlClient;


namespace Risiko
{
    public partial class RisikoMain : Form
    {
        // zum zeichnen
        internal Bitmap z_asBitmap;                                              //Bilddatei der Graphic z
        internal Graphics z;
        internal Pen stift;

        /// <summary>
        /// Modus für verschiedene Spielarten
        /// 0 = Neues Spiel, Offline
        /// 1 = Spiel Laden, Offline
        /// </summary>
        internal int Mode;
        /// <summary>
        ///  Speichert zusätzliche Information, wie Datastring
        /// </summary>
        internal string tempInfo;

        //Während Laufzeit erstellte Labels
        internal Label[] lblContinents;

        // Control, Steuerung
        internal GameControl Control;
        internal GameControl control
        {
            get { return Control; }
        }

        // um Locations zu speichern
        Point[] ControlLocations = new Point[7];

        //Konstruktoren
        public RisikoMain()
        {
            InitializeComponent();
        }
        public RisikoMain(int ModeIn, string Info)
        {
            Mode = ModeIn;
            tempInfo = Info;
            InitializeComponent();
        }
        //Load-Methode
        internal void RisikoMain_Load(object sender, EventArgs e)
        {
            // Rücksetzen des Nachrichtenlabels
            lblMessage.Text = "";
            // Control initialisieren
            Control = new GameControl(this);
            // für Ländergrenzen
            stift = new Pen(Color.Black);
            stift.Width = 1;
            // BackColor der Map
            pnlMap.BackColor = Color.White;


            // Design
            // Buttons
            btnDrawMap.FlatStyle = FlatStyle.Flat;
            btnDrawMap.FlatAppearance.BorderColor = Color.White;
            btnDrawMap.FlatAppearance.BorderSize = 0;

            btnEndMoveAttack.FlatStyle = FlatStyle.Flat;
            btnEndMoveAttack.FlatAppearance.BorderSize = 0;

            btnTest1.FlatStyle = FlatStyle.Flat;
            btnTest1.FlatAppearance.BorderSize = 0;

            btnOptions.FlatStyle = FlatStyle.Flat;
            btnOptions.FlatAppearance.BorderSize = 0;

            // ProgressBar
            pBUnits.Minimum = 0;
            pBUnits.Step = 1;
            pBUnits.BorderStyle = BorderStyle.None;

            // Rote schrift im Message label -> bessere Sichtbarkeit
            lblMessage.ForeColor = Color.Red;

            // Control soll hier direkt laden, spieler erzeugen usw
            if (Mode == 1)
            {
                Control.TxtDataSource = tempInfo;
                Control.LoadGame();
            }
        }

        // Menüband
        /// <summary>
        /// Menüband, Klick auf Speichern
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void spielSpeichernToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control.SaveGame();
        }
        internal void einstellungenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Control.autoLanderkennung)
                Control.autoLanderkennung = false;
            else
                Control.autoLanderkennung = true;
        }

        // KlickEreignisse
        internal void btnDrawMap_Click(object sender, EventArgs e)
        {
            Control.DrawMap();
        }
        private void btnEndMove_Click(object sender, EventArgs e)
        {
            SaveControlLocations();
            Control.ButtonMoveAttackSetEnd();
            LoadControlLocations();
        }
        internal void btnOptions_Click(object sender, EventArgs e)
        {
            RisikoAttackOptions Opt = new RisikoAttackOptions(this);
            Opt.Show();
        }
        
        //          !!!         Maussteuerung       !!!
        /// <summary>
        /// Bei Mausbewegung
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void pnlMap_MouseMove(object sender, MouseEventArgs e)
        {
            Control.MouseMoved(e);
        }
        internal void pnlMap_MouseClick(object sender, MouseEventArgs e)
        {
            SaveControlLocations();
            Control.MouseClicked(e);
            LoadControlLocations();
        }


        // Für Einstellungs-Popup
        /// <summary>
        /// Liefert Position des Popups in Relation zum Button
        /// </summary>
        /// <returns></returns>
        public Point GivePopUpPos()
        {
            return new Point(Location.X + btnOptions.Location.X + 8, Location.Y + btnOptions.Location.Y + 29);
        }
        private void btnOptions_MouseEnter(object sender, EventArgs e)
        {
            RisikoAttackOptions Opt = new RisikoAttackOptions(this);
            Opt.Show();
        }

        // zeichnen
        /// <summary>
        /// Legt pnlMap background fest
        /// </summary>
        /// <param name="Map"></param>
        public void DrawMap()
        {
            z_asBitmap = new Bitmap(pnlMap.Width, pnlMap.Height);
            z = Graphics.FromImage(z_asBitmap);

            // Länder zeichnen
            for (int i = 0; i < Control.field.numberOfCountries; ++i)
            {
                Point[] tempPoints = Control.field.GiveCountry(i).corners;
                Point[] realPoints = new Point[Control.field.GiveCountry(i).corners.Length];

                for (int j = 0; j < realPoints.Length; ++j)
                {
                    realPoints[j].X = (tempPoints[j].X * Control.factor);
                    realPoints[j].Y = (tempPoints[j].Y * Control.factor);
                }

                SolidBrush tempObjectbrush = new SolidBrush(Control.field.GiveCountry(i).colorOfCountry);
                z.FillPolygon(tempObjectbrush, realPoints);
                z.DrawPolygon(stift, realPoints);
            }


            // Karte festlegen
            pnlMap.BackgroundImage = z_asBitmap;
        }
        /// <summary>
        /// zeichnet auf pnl mithilfe der Standard GDI+
        /// allerdings nur ein Land, womit zu viel zeichnen vermieden wird
        /// -> kein Flackern
        /// </summary>
        /// <param name="IndexIn">Index des Landes</param>
        public void DrawSingleCountry(int IndexIn)
        {
            // Zeichnet Land einfarbig, in Farbe des besitzenden Spielers
            Graphics temp;
            temp = pnlMap.CreateGraphics();

            Point[] tempPoints = Control.field.GiveCountry(IndexIn).corners;
            Point[] realPoints = new Point[Control.field.GiveCountry(IndexIn).corners.Length];

            for (int i = 0; i < realPoints.Length; ++i)
            {
                realPoints[i].X = (tempPoints[i].X * Control.factor);
                realPoints[i].Y = (tempPoints[i].Y * Control.factor);
            }

            SolidBrush tempObjectbrush = new SolidBrush(Control.field.GiveCountry(IndexIn).colorOfCountry);
            temp.FillPolygon(tempObjectbrush, realPoints);
            temp.DrawPolygon(stift, realPoints);
        }
        /// <summary>
        /// Zeichnet Land mit Index IndexIn markiert
        /// </summary>
        /// <param name="IndexIn"></param>
        public void DrawSingleCountryMarked(int IndexIn)
        {
            // Zeichnet Land einfarbig, in Farbe des besitzenden Spielers
            Graphics temp;
            temp = pnlMap.CreateGraphics();

            Point[] tempPoints = Control.field.GiveCountry(IndexIn).corners;
            Point[] realPoints = new Point[Control.field.GiveCountry(IndexIn).corners.Length];

            for (int i = 0; i < realPoints.Length; ++i)
            {
                realPoints[i].X = (tempPoints[i].X * Control.factor);
                realPoints[i].Y = (tempPoints[i].Y * Control.factor);
            }

            SolidBrush tempObjectbrush = new SolidBrush(Control.ColorCountrySelected);
            temp.FillPolygon(tempObjectbrush, realPoints);
            temp.DrawPolygon(stift, realPoints);
        }
        /// <summary>
        /// Kreis in der Mitte eine Landes (CountryIn)
        /// wird gezeichnet, mit der Anzahl der Einheiten
        /// </summary>
        /// <param name="Country"></param>
        public void DrawMiddleCircle(int Country)
        {
            Point[] realPoints = Control.GetRealPointsFromCorners(Control.field.countries[Country].corners);
            Point Middle = Control.GetMiddleOfPolygon(realPoints);

            //graphics initialisieren
            Graphics temp = pnlMap.CreateGraphics();

            //MittelKreis in Schwarz zeichnen
            SolidBrush tempObjectbrush = new SolidBrush(Color.Black);
            temp.FillEllipse(tempObjectbrush, Middle.X - 10, Middle.Y - 10, 20, 20);

            //zum schreiben
            Font f = new Font("Arial", 10);
            tempObjectbrush = new SolidBrush(Color.White);

            // -5 magic, TODO: Verbessern, passt nicht immer (nicht immer in der Mitte -> zweistellig usw.)
            temp.DrawString(
                Convert.ToString(Control.field.countries[Country].unitsStationed), f, tempObjectbrush, Middle.X - 5, Middle.Y - 5);
        }
        /// <summary>
        /// Zeichnet nur Anzahl der Einheiten, in Weiß in der Mitte des Landes
        /// </summary>
        /// <param name="Country"></param>
        public void DrawMiddleUnits(int Country)
        {
            Point[] realPoints = Control.GetRealPointsFromCorners(Control.field.countries[Country].corners);
            Point Middle = Control.GetMiddleOfPolygon(realPoints);

            //graphics initialisieren
            Graphics temp = pnlMap.CreateGraphics();

            //zum schreiben
            Font f = new Font("Arial", 10);
            SolidBrush tempObjectbrush = new SolidBrush(Color.Black);
          
            // -5 magic, TODO: Verbessern, passt nicht immer (nicht immer in der Mitte -> zweistellig usw.)
            temp.DrawString(
                Convert.ToString(Control.field.countries[Country].unitsStationed), f, tempObjectbrush, Middle.X - 5, Middle.Y - 5);

            if (Control.startUnitAdding && Control.unitsToAdd != null && Control.unitsToAdd[Country] != 0)
            {
                tempObjectbrush = new SolidBrush(Color.Green);
                temp.DrawString(
                "+" + Convert.ToString(Control.unitsToAdd[Country]), f, tempObjectbrush, Middle.X + 10, Middle.Y - 5);
            }
        }
        /// <summary>
        /// Zeichnet Land mit Farbigem äußeren, gründ atm
        /// </summary>
        /// <param name="IndexIn"></param>
        public void DrawCountryWithInner(int IndexIn)
        {
            //Faktor
            double Factor = 0.9;

            // Zeichnet Land einfarbig, in Farbe des besitzenden Spielers
            Graphics temp;
            temp = pnlMap.CreateGraphics();

            //Äußere Eckpunkte
            Point[] tempPoints = Control.field.GiveCountry(IndexIn).corners;
            Point[] realPoints = new Point[Control.field.GiveCountry(IndexIn).corners.Length];

            for (int i = 0; i < realPoints.Length; ++i)
            {
                realPoints[i].X = (tempPoints[i].X * Control.factor);
                realPoints[i].Y = (tempPoints[i].Y * Control.factor);
            }

            // Innere Eckpunkte
            Point Middle = Control.GetMiddleOfPolygon(realPoints);
            Point[] InnerPoly = new Point[realPoints.Length];
            for (int i = 0; i < realPoints.Length; ++i)
            {
                int innerVX = realPoints[i].X - Middle.X;
                int innerVY = realPoints[i].Y - Middle.Y;

                InnerPoly[i].X = Middle.X + (int)(innerVX * Factor);
                InnerPoly[i].Y = Middle.Y + (int)(innerVY * Factor);
            }
            // Außen zeichnen
            SolidBrush tempObjectbrush = new SolidBrush(Color.Green);
            temp.FillPolygon(tempObjectbrush, realPoints);
            temp.DrawPolygon(stift, realPoints);
            // Innen Zeichnen
            tempObjectbrush = new SolidBrush(Control.field.GiveCountry(IndexIn).colorOfCountry);
            temp.FillPolygon(tempObjectbrush, InnerPoly);

        }
        /// <summary>
        /// Schreibt die Koordinaten der Eckpunkte der Länder an die Eckpunkte der Länder
        /// </summary>
        /// <param name="CountryIn"></param>
        public void DrawCorners(int CountryIn)
        {
            Point[] realPoints = Control.GetRealPointsFromCorners(Control.field.countries[CountryIn].corners);
            Point[] Corners = Control.field.countries[CountryIn].corners;

            //graphics initialisieren
            Graphics temp = pnlMap.CreateGraphics();

            //zum schreiben
            Font f = new Font("Arial", 10);
            SolidBrush tempObjectbrush = new SolidBrush(Color.Black);

            for (int i = 0; i < Corners.Length; ++i)
            {
                String Corner = Convert.ToString(Corners[i].X) + ";" + Convert.ToString(Corners[i].Y);
                // -5 magic, TODO: Verbessern, passt nicht immer (nicht immer in der Mitte -> zweistellig usw.)
                temp.DrawString(Corner, f, tempObjectbrush, realPoints[i].X, realPoints[i].Y);
            }
        }
        /// <summary>
        /// TEMP:
        /// Zeichnet Nachbarländer (zeichnet das Netzwerk an Nachbarländern) 
        /// um zu überprüfen ob die Nachbarläner richtig ausgelesen wurden
        /// </summary>
        public void DrawAllNeighbours()
        {
            Graphics temp = pnlMap.CreateGraphics();

            for (int i = 0; i < Control.field.numberOfCountries; ++i)
            {
                Country[] Neighbours = Control.field.countries[i].neighbouringCountries;
                for (int j = 0; j < Neighbours.Length; ++j)
                {
                    string tempName = Neighbours[j].name;
                    int tempK = 0;
                    for (int k = 0; k < Control.field.numberOfCountries; ++k)
                        if (Control.field.countries[k].name == tempName)
                            tempK = k;
                    temp.DrawLine(stift, Control.GetRealMiddleOfPolygon(Control.field.countries[tempK].corners).X,
                        Control.GetRealMiddleOfPolygon(Control.field.countries[tempK].corners).Y, Control.GetRealMiddleOfPolygon(Control.field.countries[i].corners).X,
                        Control.GetRealMiddleOfPolygon(Control.field.countries[i].corners).Y);
                }
                Point[] realPoints = Control.GetRealPointsFromCorners(Control.field.countries[i].corners);
                Point Middle = Control.GetMiddleOfPolygon(realPoints);
                //zum schreiben
                Font f = new Font("Arial", 10);
                SolidBrush tempObjectbrush = new SolidBrush(Color.Black);

                // -5 magic, TODO: Verbessern, passt nicht immer (nicht immer in der Mitte -> zweistellig usw.)
                temp.DrawString(
                    Convert.ToString(Control.GetOwnedIndexFromName(Control.field.countries[i].name)) + Control.field.countries[i].name, f, tempObjectbrush, Middle.X + 10, Middle.Y + 5);
            }
        }
        /// <summary>
        /// Liefert Höhe und Breite von pnlMap,
        /// </summary>
        /// <returns></returns>
        public int[] GetMapData()
        {
            int[] data = new int[2];
            data[0] = pnlMap.Width;
            data[1] = pnlMap.Height;
            return data;
        }
        /// <summary>
        /// Zeichnet Karte Neu bei veränderung der Größe der Map
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void ResizeMap(object sender, EventArgs e)
        {
            Control.DrawMap();
        }


        // KontinenteLabel + EreignisMethoden
        /// <summary>
        /// Erzeugt die Label der Kontinente
        /// </summary>
        public void CreateContinentLabels()
        {
            lblContinents = new Label[Control.field.continents.Length];
            for (int i = 0; i < Control.field.continents.Length; ++i)
            {
                lblContinents[i] = new Label();
                lblContinents[i].Location = new Point(Control.field.firstContLabelPosition.X * Control.factor,
                                                Control.field.firstContLabelPosition.Y * Control.factor + i * 15);
                lblContinents[i].Size = new Size(100, 13);
                lblContinents[i].BackColor = Color.Transparent;
                lblContinents[i].Text = Control.field.continents[i].nameOfContinent + ":  " +
                                 Control.field.continents[i].additionalUnits;

                lblContinents[i].MouseEnter += new EventHandler(lblContinent_MouseEnter);
                lblContinents[i].MouseLeave += new EventHandler(lblContinent_MouseLeave);

                lblContinents[i].AutoSize = true;

                Controls.Add(lblContinents[i]);
                lblContinents[i].BringToFront();
            }
        }

        // EreignisMethoden der lblContinents, Enter und Leave
        internal void lblContinent_MouseEnter(object sender, EventArgs e)
        {
            Label senderLbl = sender as Label;
            string[] Text = senderLbl.Text.Split(':');

            int ContinentIndex = Control.field.GiveContinentIndex(Text[0]);

            Control.MarkContinent(ContinentIndex);
        }
        internal void lblContinent_MouseLeave(object sender, EventArgs e)
        {
            Label senderLbl = sender as Label;
            string[] Text = senderLbl.Text.Split(':');

            int ContinentIndex = Control.field.GiveContinentIndex(Text[0]);

            Control.UnmarkContinent(ContinentIndex);
        }


        // Sonstiges
        /// <summary>
        /// Setzt Texteigenschaft von lblMessage auf Message
        /// </summary>
        /// <param name="Message"></param>
        public void ShowMessage(string Message)
        {
            lblMessage.Text = Message;
            timerDeleteMessage.Start();
        }
        /// <summary>
        /// Löscht Meldung aus lblMessage
        /// </summary>
        public void DeleteMessage()
        {
            lblMessage.Text = "";
        }
        /// <summary>
        /// Tick des Timers, Tickt jeweils nur einmal
        /// deaktiviert sich selbst und löscht lblMessage.Text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void timerDeleteMessage_Tick(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            timerDeleteMessage.Stop();
        }
        /// <summary>
        /// Löscht die KontinentLabels
        /// Eigentlich nicht benötigt
        /// </summary>
        public void RemoveContinentsLabels()
        {
            for (int i = 0; i < Control.field.continents.Length; ++i)
            {
                Controls.Remove(lblContinents[i]);
            }
        }
        /// <summary>
        /// Aktualisiert Progressbar
        /// </summary>
        public void ActualizePB()
        {
            pBUnits.Value = Control.actualPlayer.unitsPT;
        }
        /// <summary>
        /// Speichert die Position alles Controls
        /// </summary>
        public void SaveControlLocations()
        {
            ControlLocations[0] = lblMessage.Location;
            ControlLocations[1] = pBUnits.Location;
            ControlLocations[2] = btnOptions.Location;
            ControlLocations[3] = btnEndMoveAttack.Location;
            ControlLocations[4] = btnDrawMap.Location;
            ControlLocations[5] = btnTest1.Location;
            ControlLocations[6] = btnTest2.Location;
        }
        /// <summary>
        /// Stellt die gespeicherte Position aller Controls wieder her
        /// </summary>
        public void LoadControlLocations()
        {
            lblMessage.Location = ControlLocations[0];
            pBUnits.Location = ControlLocations[1];
            btnOptions.Location = ControlLocations[2];
            btnEndMoveAttack.Location = ControlLocations[3];
            btnDrawMap.Location = ControlLocations[4];
            btnTest1.Location = ControlLocations[5];
            btnTest2.Location = ControlLocations[6];
        }
        /// <summary>
        /// Kehr Sichtbarkeit der Progressbar um
        /// </summary>
        public void NegateVisibilityPB()
        {
            pBUnits.Visible = !pBUnits.Visible;
        }
        /// <summary>
        /// Setzt die Progressbar.Color auf ColorIn
        /// </summary>
        /// <param name="ColorIn"></param>
        public void SetPBColor(Color ColorIn)
        {
            pBUnits.MainColor = ColorIn;
        }
        /// <summary>
        /// Aktualisiert den Maximalen Wert der Progressbar
        /// </summary>
        public void ActualizePBmax()
        {
            pBUnits.Maximum = Control.actualPlayer.unitsPT;
        }

        // TestMethoden für TestButtons
        private void btnTest2_Click(object sender, EventArgs e)
        {
            DrawAllNeighbours();
        }
        internal void btnTest_Click(object sender, EventArgs e)
        {
            // neues Spiel
            string[] name = { "Peter", "Hans" };
            Color[] color = { Color.FromArgb(0xEE, 0x2C, 0x2C), Color.FromArgb(0x54, 0x8B, 0x54) };
            Color[] color2 = { Color.Red, Color.Green };
            bool[] ai = { false, false };
            Control.StartNewGame(name, color2, ai);
        }

        private void pnlMap_Paint(object sender, PaintEventArgs e)
        {

        }

        //Old
        //// KontinentTabelle zeichnen
        //    Point[] Corners = Control.field.tableCorners;
        //    for (int i = 0;i < Corners.Length;++i)
        //    {
        //        Corners[i].X *= Control.factor;
        //        Corners[i].Y *= Control.factor;
        //    }
        //    stift.Width = 2;
        //    z.DrawPolygon(stift, Corners);
        //    stift.Width = 1;

        //    // KontinentTabelle ausfüllen
        //    //zum schreiben
        //    Font f = new Font("Arial", 10);
        //    SolidBrush tempWritingBrush = new SolidBrush(Color.Black);

        //    Point StartPoint = new Point(Corners[0].X + 10, Corners[0].Y + 10);
        //    for (int i = 0; i < Control.field.continents.Length; ++i )
        //    {
        //        z.DrawString(
        //        Control.field.continents[i].nameOfContinent + ":\t" + Control.field.continents[i].additionalUnits, f, tempWritingBrush, StartPoint);
        //        StartPoint.Y += 20;
        //    }
    }
}
