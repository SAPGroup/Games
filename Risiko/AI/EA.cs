using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risiko
{
    class EA
    {
        //Klasse für Evolutionären Algorithmus
        internal const int NumberOfAIs = 20;
        /// <summary>
        /// Generationen von AI
        /// </summary>
        internal AI[] ThisGeneration;
        internal AI[] NextGeneration;
    }
}
