using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            pnlMap.BackColor = Color.Beige;
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
            //pnlMap bekommt Bilddatei zugewiesen
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
    }
}
