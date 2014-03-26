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
        internal int AdditionalUnits;
        public int additionalUnits
        {
            get { return AdditionalUnits; }
            set { AdditionalUnits = value; }
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

        // benötigt?
        ///// <summary>
        ///// Länder des Kontinents
        ///// </summary>
        //internal Country[] CountriesOfContinent;
        //public Country[] countriesOfContinent
        //{
        //    get { return CountriesOfContinent; }
        //    set { CountriesOfContinent = value; }
        //}


        /// <summary>
        /// Anzahl der beinhalteten Länder
        /// </summary>
        private int NumberOfCountries;
        public int numberOfCountries
        {
            get { return NumberOfCountries; }
            set { NumberOfCountries = value; }
        }
        

        /// <summary>
        /// "Standard"konstruktor
        /// </summary>
        public Continent()
        {
            AdditionalUnits = 0;
            NameOfContinent = "";
        }

        public Continent(int AddMenIn, string Name, int CountriesIn)
        {
            AdditionalUnits = AddMenIn;
            NameOfContinent = Name;
            NumberOfCountries = CountriesIn;
        }
    }
}
