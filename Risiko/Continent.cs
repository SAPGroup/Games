using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risiko
{
    class Continent
    {
        /// <summary>
        /// Anzahl der zusätzklichen Einheiten die ein Spieler erhält
        /// wenn er den ganzen Kontinent besitzt
        /// </summary>
        private int AdditionalMen;
        public int additionalMen
        {
            get { return AdditionalMen; }
            set { AdditionalMen = value; }
        }

        /// <summary>
        /// Name des Kontinents
        /// </summary>
        private string NameOfContinent;
        public string nameOfContinent
        {
            get { return NameOfContinent; }
            set { NameOfContinent = value; }
        }

        /// <summary>
        /// Länder des Kontinents
        /// </summary>
        private Country[] CountriesOfContinent;
        public Country[] countriesOfContinent
        {
            get { return CountriesOfContinent; }
            set { CountriesOfContinent = value; }
        }

    }
}
