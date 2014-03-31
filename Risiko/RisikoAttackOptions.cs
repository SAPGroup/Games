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
        internal bool Entered = false;

        public RisikoAttackOptions(RisikoMain CallerIn)
        {
            InitializeComponent();

            Caller = CallerIn;
        }

        internal void RisikoAttackOptions_Load(object sender, EventArgs e)
        {
            Location = Caller.GivePopUpPos();
            Location = new Point(Location.X - Width + Caller.btnOptions.Width, Location.Y - Height);

            numUDCustomAttackers.Minimum = 1;
            if (Caller.control.actualPlayer != null && Caller.control.actualPlayer.unitsInCountry != null && Caller.control.tempClickedFstIndex != -1)
                numUDCustomAttackers.Maximum = Caller.control.actualPlayer.unitsInCountry[Caller.control.tempClickedFstIndex];
            numUDCustomAttackers.Visible = false;
            // Timer anschalten
            timerClose.Enabled = true;

            // Vor laden alles auf disabled stellen
            lblTextStopAttacking1.Enabled = false;
            lblTextStopAttacking0.Enabled = false;

            cBStopAttacking.Enabled = false;

            numUDStopPercentage.Enabled = false;
            numUDCustomAttackers.Visible = false;

            rBAllMen.Checked = false;
            rBCustomAttackers.Checked = false;
            rB3EveryTime.Checked = false;

            // Einstellungen des Spielers laden
            LoadPlayerSettings();
        }

        //Schließt wenn Klick außerhalb
        internal void RisikoAttackOptionsDeactivate(object sender, EventArgs e)
        {
            SaveChanges();
            Close();
        }

        internal void rBCustomAttackers_CheckedChanged(object sender, EventArgs e)
        {
            numUDCustomAttackers.Visible = rBCustomAttackers.Checked;
            if (rBCustomAttackers.Checked)
            {
                cBStopAttacking.Enabled = true;
                if (cBStopAttacking.Checked)
                {
                    lblTextStopAttacking1.Enabled = true;
                    lblTextStopAttacking0.Enabled = true;
                    numUDStopPercentage.Enabled = true;
                }
            }
            else
            {
                lblTextStopAttacking0.Enabled = false;
                lblTextStopAttacking1.Enabled = false;
                cBStopAttacking.Enabled = false;
                numUDStopPercentage.Enabled = false;
            }
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
                SaveChanges();
                Close();
            }
        }

        private void RisikoAttackOptions_MouseEnter(object sender, EventArgs e)
        {
            Entered = true;
        }

        private void timerClose_Tick(object sender, EventArgs e)
        {
            Point Mouse = new Point(Cursor.Position.X, Cursor.Position.Y);
            Point[] Button = new Point[4];
            Button[0] = new Point(Caller.Location.X + Caller.btnOptions.Location.X + 8,
                                    Caller.Location.Y + Caller.btnOptions.Location.Y + 29);
            Button[1] = new Point(Caller.Location.X + Caller.btnOptions.Location.X + 8 + Caller.btnOptions.Width,
                                    Caller.Location.Y + Caller.btnOptions.Location.Y + 29);
            Button[2] = new Point(Caller.Location.X + Caller.btnOptions.Location.X + 8 + Caller.btnOptions.Width,
                                    Caller.Location.Y + Caller.btnOptions.Location.Y + 29 + Caller.btnOptions.Height);
            Button[3] = new Point(Caller.Location.X + Caller.btnOptions.Location.X + 8,
                                    Caller.Location.Y + Caller.btnOptions.Location.Y + 29 + Caller.btnOptions.Height);

            if (Entered)
                timerClose.Enabled = false;
            else if (!Entered & !Caller.control.PointInPolygon(Mouse, Button))
            {
                SaveChanges();
                Close();
            }
        }

        private void rBAllMen_CheckedChanged(object sender, EventArgs e)
        {
            if (rBAllMen.Checked)
            {
                cBStopAttacking.Enabled = true;
                if (cBStopAttacking.Checked)
                {
                    lblTextStopAttacking1.Enabled = true;
                    lblTextStopAttacking0.Enabled = true;
                    numUDStopPercentage.Enabled = true;
                }
            }
            else
            {
                lblTextStopAttacking0.Enabled = false;
                lblTextStopAttacking1.Enabled = false;
                cBStopAttacking.Enabled = false;
                numUDStopPercentage.Enabled = false;
            }
        }

        /// <summary>
        /// Speichert die aktuellen Einstellungn in Actual Player
        /// </summary>
        private void SaveChanges()
        {
            if (Caller.control.actualPlayer != null)
            {
                if (rBAllMen.Checked)
                {
                    // Alle Männer
                    Caller.control.actualPlayer.SettingAttack = -1;
                    if (cBStopAttacking.Checked && numUDStopPercentage.Value != 0)
                    {
                        Caller.control.actualPlayer.settingEndAttackLossPercentage = (int)numUDStopPercentage.Value;
                    }
                    else
                    {
                        Caller.control.actualPlayer.settingEndAttackLossPercentage = 0;
                    }
                }
                // Immer 3
                else if (rB3EveryTime.Checked)
                {
                    Caller.control.actualPlayer.SettingAttack = 0;
                }
                // Eigene Anzahl
                else if (rBCustomAttackers.Checked)
                {
                    if (numUDCustomAttackers.Value != 0)
                    {
                        Caller.control.actualPlayer.SettingAttack = (int)numUDCustomAttackers.Value;
                        if (cBStopAttacking.Checked && numUDStopPercentage.Value != 0)
                        {
                            Caller.control.actualPlayer.settingEndAttackLossPercentage = (int)numUDStopPercentage.Value;
                        }
                        else
                        {
                            Caller.control.actualPlayer.settingEndAttackLossPercentage = 0;
                        }
                    }
                    else
                        Caller.ShowMessage("Die Einstellungen konnten nich gespeichert werden.");
                }
            }
        }

        /// <summary>
        /// Lädt Einstellungen des Spielers
        /// </summary>
        private void LoadPlayerSettings()
        {
            if (Caller.control.actualPlayer != null)
            {
                // Einstellungen des Spielers
                // Alle Einheiten
                if (Caller.control.actualPlayer.SettingAttack == -1)
                {
                    rBAllMen.Checked = true;
                    if (Caller.control.actualPlayer.settingEndAttackLossPercentage != 0)
                    {
                        cBStopAttacking.Enabled = true;
                        cBStopAttacking.Checked = true;
                        lblTextStopAttacking0.Enabled = true;
                        lblTextStopAttacking1.Enabled = true;
                        numUDStopPercentage.Enabled = true;
                        numUDStopPercentage.Value = Caller.control.actualPlayer.settingEndAttackLossPercentage;
                    }
                }
                // Immer 3
                else if (Caller.control.actualPlayer.settingAttack == 0)
                {
                    rB3EveryTime.Checked = true;
                }
                // Eigene Anzahl
                else if (Caller.control.actualPlayer.settingAttack > 0)
                {
                    rBCustomAttackers.Checked = true;
                    numUDCustomAttackers.Enabled = true;
                    numUDCustomAttackers.Visible = true;
                    numUDCustomAttackers.Value = Caller.control.actualPlayer.settingAttack;
                    if (Caller.control.actualPlayer.settingEndAttackLossPercentage != 0)
                    {
                        cBStopAttacking.Enabled = true;
                        cBStopAttacking.Checked = true;
                        lblTextStopAttacking0.Enabled = true;
                        lblTextStopAttacking1.Enabled = true;
                        numUDStopPercentage.Enabled = true;
                        numUDStopPercentage.Value = Caller.control.actualPlayer.settingEndAttackLossPercentage;
                    }
                }
            }
        }

        private void cBStopAttacking_CheckedChanged(object sender, EventArgs e)
        {
            if (cBStopAttacking.Checked)
            {
                lblTextStopAttacking1.Enabled = true;
                lblTextStopAttacking0.Enabled = true;
                numUDStopPercentage.Enabled = true;
            }
            else
            {
                lblTextStopAttacking0.Enabled = false;
                lblTextStopAttacking1.Enabled = false;
                numUDStopPercentage.Enabled = false;
            }
        }
    }
}
