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
        private RisikoMain Caller;

        public RisikoAttackOptions(RisikoMain CallerIn)
        {
            InitializeComponent();

            Caller = CallerIn;
        }

        private void RisikoAttackOptions_Load(object sender, EventArgs e)
        {
            Location = Caller.GivePopUpPos();
            Location = new Point(Location.X, Location.Y-Height);

            numUDCustomAttackers.Minimum = 1;
            //numUDCustomAttackers.Maximum = Caller.control.        TODO: Men in country -1
            numUDCustomAttackers.Visible = false;
            
        }

        //Schließt wenn Klick außerhalb
        private void RisikoAttackOptionsDeactivate(object sender, EventArgs e)
        {
            Close();
        }

        private void rBCustomAttackers_CheckedChanged(object sender, EventArgs e)
        {
            numUDCustomAttackers.Visible = rBCustomAttackers.Checked;
        }
    }
}
