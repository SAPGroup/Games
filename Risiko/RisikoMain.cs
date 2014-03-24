using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;

namespace Risiko
{
    public partial class RisikoMain : Form
    {
        //
        private Bitmap z_asBitmap;                                              //Bilddatei der Graphic z
        private Graphics z;
        private Pen stift;  

        // Control
        private GameControl Control;
        internal GameControl control
        {
            get { return Control; }
        }


        public RisikoMain()
        {
            InitializeComponent();
        }

        private void RisikoMain_Load(object sender, EventArgs e)
        {
            // Rücksetzen des Nachrichtenlabels
            lblMessage.Text = "";
            // Control initialisieren
            Control = new GameControl(this);
            // für Ländergrenzen
            stift = new Pen(Color.Black);
            // BackColor der Map
            pnlMap.BackColor = Color.White;


            // Design
            btnDrawMap.FlatStyle = FlatStyle.Flat;
            btnDrawMap.FlatAppearance.BorderColor = Color.White;
            btnDrawMap.FlatAppearance.BorderSize = 1;

            btnEndMove.FlatStyle = FlatStyle.Flat;
            btnEndMove.FlatAppearance.BorderColor = Color.White;
            btnEndMove.FlatAppearance.BorderSize = 1;

            btnTest.FlatStyle = FlatStyle.Flat;
            btnTest.FlatAppearance.BorderColor = Color.White;
            btnTest.FlatAppearance.BorderSize = 1;

            btnOptions.FlatStyle = FlatStyle.Flat;
            btnOptions.FlatAppearance.BorderColor = Color.White;
            btnOptions.FlatAppearance.BorderSize = 1;

            
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            
        }

        private void btnDrawMap_Click(object sender, EventArgs e)
        {
            Control.DrawMap();
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

        private void ResizeMap(object sender, EventArgs e)
        {
            Control.DrawMap();
        }

        private void einstellungenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //TODO: Open PropertiesForm -> machen
        }

        private void pnlMap_MouseMove(object sender, MouseEventArgs e)
        {
            Control.MouseMoved(e);
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

            pnlMap.BackgroundImage = z_asBitmap;
        }

        /// <summary>
        /// zeichnet auf pnl mithilfe der Standard GDI+
        /// allerdings nur ein Land, womit zu viel zeichnen vermieden wird
        /// -> kein Flackern
        /// </summary>
        /// <param name="IndexIn">Index des Landes</param>
        public void DrawCountry(int IndexIn)
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
            SolidBrush tempObjectbrush = new SolidBrush(Color.White);

            // -5 magic, TODO: Verbessern, passt nicht immer (nicht immer in der Mitte -> zweistellig usw.)
            temp.DrawString(
                Convert.ToString(Control.field.countries[Country].unitsStationed), f, tempObjectbrush, Middle.X - 5, Middle.Y - 5);
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
            for (int i = 0;i < realPoints.Length;++i)
            {
                int innerVX = realPoints[i].X - Middle.X;
                int innerVY = realPoints[i].Y - Middle.Y;

                InnerPoly[i].X = Middle.X + (int)(innerVX*Factor);
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

        public void DrawCorners(int CountryIn)
        {
            Point[] realPoints = Control.GetRealPointsFromCorners(Control.field.countries[CountryIn].corners);
            Point[] Corners = Control.field.countries[CountryIn].corners;

            //graphics initialisieren
            Graphics temp = pnlMap.CreateGraphics();

            //zum schreiben
            Font f = new Font("Arial", 10);
            SolidBrush tempObjectbrush = new SolidBrush(Color.Black);

            for (int i = 0;i < Corners.Length;++i)
            {
                String Corner = Convert.ToString(Corners[i].X) + ";" + Convert.ToString(Corners[i].Y);
                // -5 magic, TODO: Verbessern, passt nicht immer (nicht immer in der Mitte -> zweistellig usw.)
                temp.DrawString(Corner, f, tempObjectbrush, realPoints[i].X, realPoints[i].Y);
            }
        }


        private void pnlMap_Paint(object sender, PaintEventArgs e)
        {

        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {

        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            RisikoAttackOptions Opt = new RisikoAttackOptions(this);
            Opt.Show();
        }


        // Sonstiges
        public Point GivePopUpPos()
        {
            return new Point(Location.X + btnOptions.Location.X + 8, Location.Y + btnOptions.Location.Y + 29);
        }

        internal Player GiveActualPlayer()
        {
            return Control.actualPlayer;
        }
    }
}
