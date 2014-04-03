using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risiko
{
    class AI
    {
        //Properties, dont change from EA
        internal AICountry[] Countries;
        internal AIContinent[] Continents;
        internal Player[] Players;
        // Die KI selbst
        internal Player ActualPlayer;
        /// <summary>
        /// Die "echten" countries, eigene AICountries werden mit diesen aktualisiert
        /// für Kontinente nicht benötigt, da diese sich nicht verändern
        /// </summary>
        private Country[] OfficialCountries;
        public Country[] officialCountries
        {
            get { return OfficialCountries; }
            set { OfficialCountries = value; }
        }
        
        

        // Set all Properties in Constructor
        public AI(Country[] CountriesIn, Continent[] ContinentsIn, Player[] PlayersIn, Player ActualPlayerIn)
        {
            // Set all Properties
            Countries = ConvertCountryToAICountry(CountriesIn);
            Continents = ConvertContinentToAIContinent(ContinentsIn);
            Players = PlayersIn;
            ActualPlayer = ActualPlayerIn;
        }

        /// <summary>
        /// Normal Constructor, dont use
        /// vlt hiermit immer die beste KI erzeugen
        /// </summary>
        public AI()
        {
            
        }

        // Setzen von Einheiten

        // Angreifen

        // Verschieben

        // Sonstiges

        /// <summary>
        /// Wandelt normale Countries in AICountries um
        /// nutzt dabei den speziellen Konstruktor der AICountry
        /// </summary>
        /// <param name="CountriesIn"></param>
        /// <returns></returns>
        internal AICountry[] ConvertCountryToAICountry(Country[] CountriesIn)
        {
            AICountry[] OutBuff = new AICountry[CountriesIn.Length];
            for (int i = 0;i < CountriesIn.Length;++i)
                OutBuff[i] = new AICountry(CountriesIn[i]);
            return OutBuff;
        }
        /// <summary>
        /// Wandelt normalen Continent in AIContinent um
        /// verwendet dabei speziellen Konstruktor von AICountry
        /// </summary>
        /// <param name="ContsIn"></param>
        /// <returns></returns>
        internal AIContinent[] ConvertContinentToAIContinent(Continent[] ContsIn)
        {
            AIContinent[] OutBuff = new AIContinent[ContsIn.Length];
            for (int i = 0;i <ContsIn.Length;++i)
                OutBuff[i] = new AIContinent(ContsIn[i]);
            return OutBuff;
        }
        /// <summary>
        /// Aktualisiert die AICountries mit den offiziellen, veränderten
        /// normalen Countries
        /// </summary>
        public void ActualizeAICountries()
        {
            for (int i = 0;i < Countries.Length;++i)
                Countries[i].ActualizeWithNormalCountry(OfficialCountries[i]);
        }
        /// <summary>
        /// Berrechnet die Effektivität aller Kontinente
        /// Effektivität = Angrenzende Länder an Kontinent / Zusätzliche Einheiten durch Kontinent
        /// </summary>
        public void CalculateContinentEffectiveness()
        {
            for (int k = 0;k < Continents.Length;++k)
            {
                int AttackCountries = 0;
                for (int i = 0; i < Countries.Length; ++i)
                    for (int j = 0; j < Countries[i].neighbouringCountries.Length; ++j)
                        if (Countries[i].continent != Countries[i].neighbouringCountries[j].continent)
                            AttackCountries++;
                Continents[k].Effectiveness = AttackCountries/(double)Continents[k].additionalUnits;
            }
        }
        public void CalculateDefendValue()
        {
            for (int i = 0;i < ActualPlayer.ownedCountries.Length;++i)
            {

            }
        }
    }
}
