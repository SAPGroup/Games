using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    internal class LauncherControl
    {

        // Um auf aufrufende Klasse zugreifen zu können
        internal Launcher Caller;

        // Speichert den "Index" des Spiels ab, das gespielt werden soll
        // gestartet, fortgesetzt usw
        internal int GameToPlay = 0;
        public int gameToPlay
        {
            get { return GameToPlay; }
            set { GameToPlay = value; }
        }

        // Konstruktor
        public LauncherControl(Launcher CallerIn)
        {
            Caller = CallerIn;
        }


        public void newGame(int GameIndex)
        {
            GameToPlay = GameIndex;
            Caller.ShowNewGame();
        }
    }
}
