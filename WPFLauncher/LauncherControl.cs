using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Risiko;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace WPFLauncher
{
    class LauncherControl
    {
        internal RisikoMain RisikoGame;
        internal MainWindow Caller;
        internal int ActualGame;

        public LauncherControl(MainWindow CallerIn)
        {
            Caller = CallerIn;
        }

        public void LoadGame()
        {
            if (ActualGame == 0)
            {
                OpenFileDialog ofd = new OpenFileDialog();

                ofd.Multiselect = false;
                //ofd.InitialDirectory =;
                ofd.Filter = " Texte (*.txt)|*.txt|" + " Alle Dateien (*.*)|*.*";
                ofd.Title = "Spielstand zum Laden auswählen";

                Nullable<bool> result = ofd.ShowDialog();

                if (result == true)
                {
                    RisikoGame = new RisikoMain(1, ofd.FileNames[0]);
                    RisikoGame.Show();
                }
                else
                {
                    MessageBox.Show("Es ist ein Fehler aufgetreten.");
                }
            }
        }
    }
}
