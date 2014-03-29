using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risiko
{
    class GameField
    {
        /// <summary>
        /// Daten von Gamefield, Anzahl der Läbder, Höhe und Breite
        /// </summary>
        internal int NumberOfCountries;
        public int numberOfCountries
        {
            get { return NumberOfCountries; }
            set { NumberOfCountries = value; }
        }

        internal int Height;
        public int height
        {
            get { return Height; }
            set { Height = value; }
        }

        internal int Width;
        public int width
        {
            get { return Width; }
            set { Width = value; }
        }

        internal Continent[] Continents;
        public Continent[] continents
        {
            get { return Continents; }
            set { Continents = value; }
        }
        
        /// <summary>
        /// Länder des Spielfelds
        /// </summary>
        internal Country[] Countries;
        public Country[] countries
        {
            get { return Countries; }
            set { Countries = value; }
        }

        /// <summary>
        /// Start-Location des ersten Continent-Labels
        /// </summary>
        private Point FirstContLabelPosition;
        public Point firstContLabelPosition
        {
            get { return FirstContLabelPosition; }
            set { FirstContLabelPosition = value; }
        }
        


        // Sonstiges
        public Country GiveCountry(int i)
        {
            return Countries[i];
        }

        public int GiveContinentIndex(string NameIn)
        {
            for (int i = 0;i < Continents.Length;++i)
            {
                if (Continents[i].nameOfContinent == NameIn)
                    return i;
            }
            return -1;
        }

        public bool CountriesAreNeighbours(int C1, int C2)
        {
            string C2Name = Countries[C2].name;
            for (int i = 0;i < Countries[C1].neighbouringCountries.Length;++i)
            {
                if (Countries[C1].neighbouringCountries[i] == C2Name)
                    return true;
            }

            return false;
        }
    }
}
