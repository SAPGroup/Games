using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risiko
{
    class AIPlayer
    {
        internal Player ThisPlayer;
        //AIPlayer values
        internal double RelativeStrength = 0; 

        //use this constructor
        public AIPlayer(Player PlayerIn)
        {
            ThisPlayer = PlayerIn;
        }
        //Normal constructor
        public AIPlayer()
        {
            
        }
    }
}
