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
        internal Player ActualPlayer;

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
            numUDCustomAttackers.Maximum = Caller.control.actualPlayer.unitsInCountry[Caller.control.tempIndex]; // TODO: Index
            numUDCustomAttackers.Visible = false;
        }

        //Schließt wenn Klick außerhalb
        internal void RisikoAttackOptionsDeactivate(object sender, EventArgs e)
        {
            Close();
        }

        internal void rBCustomAttackers_CheckedChanged(object sender, EventArgs e)
        {
            numUDCustomAttackers.Visible = rBCustomAttackers.Checked;
        }
    }
}
