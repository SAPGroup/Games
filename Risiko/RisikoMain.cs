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
    }
}
