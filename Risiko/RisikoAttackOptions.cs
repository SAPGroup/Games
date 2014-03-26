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
    public partial class RisikoAttackOptions : Form
    {
        internal RisikoMain Caller;

        public RisikoAttackOptions(RisikoMain CallerIn)
        {
            InitializeComponent();

            Caller = CallerIn;
        }

        internal void RisikoAttackOptions_Load(object sender, EventArgs e)
        {
            Location = Caller.GivePopUpPos();
            Location = new Point(Location.X, Location.Y-Height);

            numUDCustomAttackers.Minimum = 1;
            if(Caller.control.actualPlayer != null)
                numUDCustomAttackers.Maximum = Caller.control.actualPlayer.unitsInCountry[Caller.control.tempMovedIndex]; // TODO: Index
            numUDCustomAttackers.Visible = false;
            //MessageBox.Show("Peter");
        }

        //Schließt wenn Klick außerhalb
        internal void RisikoAttackOptionsDeactivate(object sender, EventArgs e)
        {
            //Close();
        }

        internal void rBCustomAttackers_CheckedChanged(object sender, EventArgs e)
        {
            numUDCustomAttackers.Visible = rBCustomAttackers.Checked;
        }

        private void RisikoAttackOptions_MouseLeave(object sender, EventArgs e)
        {
            Point Mouse = new Point(Cursor.Position.X, Cursor.Position.Y);
            Point[] Form = new Point[4];
            Form[0] = new Point(Location.X, Location.Y);
            Form[1] = new Point(Location.X + Width, Location.Y);
            Form[2] = new Point(Location.X + Width, Location.Y + Height);
            Form[3] = new Point(Location.X, Location.Y + Height);
            if (!Caller.control.PointInPolygon(Mouse, Form))
            {
                Close();
            }
        }
    }
}
