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
        internal int AdditionalMen;
        public int additionalMen
        {
            get { return AdditionalMen; }
            set { AdditionalMen = value; }
        }

        /// <summary>
        /// Name des Kontinents
        /// </summary>
        internal string NameOfContinent;
        public string nameOfContinent
        {
            get { return NameOfContinent; }
            set { NameOfContinent = value; }
        }

        /// <summary>
        /// Länder des Kontinents
        /// </summary>
        internal Country[] CountriesOfContinent;
        public Country[] countriesOfContinent
        {
            get { return CountriesOfContinent; }
            set { CountriesOfContinent = value; }
        }

        /// <summary>
        /// "Standard"konstruktor
        /// </summary>
        public Continent()
        {
            AdditionalMen = 0;
            NameOfContinent = "";
        }

        public Continent(int AddMenIn, string Name)
        {
            AdditionalMen = AddMenIn;
            NameOfContinent = Name;
        }
    }
}
