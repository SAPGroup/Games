using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risiko
{
    class AIContinent
    {
        internal Continent ThisContinent;

        //AIContinent values
        internal double Effectiveness;
        
        //use this
        public AIContinent(Continent ContIn)
        {
            ThisContinent = ContIn;
        }

        //normal constructor
        public AIContinent()
        {
            
        }

    }
}
