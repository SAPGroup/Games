using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Risiko;

namespace Launcher
{
    class LauncherControl
    {
        internal Launcher Main;
        internal RisikoMain Risiko;
        public LauncherControl(Launcher MainIn)
        {
            Main = MainIn;
        }

        // Welches Spiel ausgewählt worden ist
        internal int SelectedGame;
        public int selectedGame 
        {
            get { return SelectedGame; }
            set { SelectedGame = value; }
        }
        
        public void LoadGame()
        {
            if (SelectedGame == 0)
            {
                OpenFileDialog ofd = new OpenFileDialog();

                ofd.Multiselect = false;
                //ofd.InitialDirectory =;
                ofd.Filter = " Texte (*.txt)|*.txt|" + " Alle Dateien (*.*)|*.*";
                ofd.Title = "Spielstand zum Laden auswählen";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Risiko = new RisikoMain(1, ofd.FileNames[0]);
                    Risiko.Show();
                }
                else
                {
                    MessageBox.Show("Es ist ein Fehler aufgetreten.");
                }
            }
        }
    }
}
