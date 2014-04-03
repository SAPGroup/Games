using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risiko
{
    class AIContinent:Continent
    {
        //Properties
        //succeeded
        //nameofcontinent, numberofcountries, additionalunits
        internal double Effectiveness;

        public AIContinent(Continent ContIn)
        {
            NameOfContinent = ContIn.nameOfContinent;
            NumberOfCountries = ContIn.numberOfCountries;
            AdditionalUnits = ContIn.additionalUnits;
        }

        //normal constructor
        public AIContinent()
        {
            
        }

    }
}
