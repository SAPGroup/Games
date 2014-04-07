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
        internal AIPlayer[] Players;
        // Die KI selbst
        internal AIPlayer ActualPlayer;
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
            Players = ConvertPlayersToAIPlayers(PlayersIn);
            ActualPlayer = ConvertSinglePlayerToAIPlayer(ActualPlayerIn);
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
        /// Aktualisiert die AICountries mit den offiziellen, veränderten
        /// normalen Countries
        /// </summary>
        public void ActualizeAICountries()
        {
            for (int i = 0; i < Countries.Length; ++i)
                Countries[i].ActualizeWithNormalCountry(OfficialCountries[i]);
        }
        /// <summary>
        /// Setzt alle Werte neu, bei neuem Zug
        /// </summary>
        /// <param name="CountriesIn"></param>
        /// <param name="PlayersIn"></param>
        /// <param name="ActualPlayerIn"></param>
        public void SetNewValues(Country[] CountriesIn, Player[] PlayersIn, Player ActualPlayerIn)
        {
            Countries = ConvertCountryToAICountry(CountriesIn);
            Players = ConvertPlayersToAIPlayers(PlayersIn);
            ActualPlayer = ConvertSinglePlayerToAIPlayer(ActualPlayerIn);
        }

        //Calculation
        /// <summary>
        /// Berrechnet die Effektivität aller Kontinente
        /// Effektivität = Angrenzende Länder an Kontinent / Zusätzliche Einheiten durch Kontinent
        /// </summary>
        public void CalculateContinentEffectiveness()
        {
            for (int k = 0; k < Continents.Length; ++k)
            {
                int AttackCountries = 0;
                for (int i = 0; i < Countries.Length; ++i)
                    for (int j = 0; j < Countries[i].ThisCountry.neighbouringCountries.Length; ++j)
                        if (Countries[i].ThisCountry.continent != Countries[i].ThisCountry.neighbouringCountries[j].continent)
                            AttackCountries++;
                Continents[k].Effectiveness = AttackCountries / (double)Continents[k].ThisContinent.additionalUnits;
            }
        }
        /// <summary>
        /// Berechnet Verteidiungswert
        /// Relation zwischen potentiellen Angreifern gegen Land und pot Angreifern insgesamtgungswert
        /// DefendValue = PotentialAttackers/ OverAllAtackers
        /// </summary>
        public void CalculateDefendValue()
        {
            int[] PotentialAttackersAround = new int[ActualPlayer.ThisPlayer.ownedCountries.Length];
            int OverAllAttackers = 0;

            for (int i = 0; i < ActualPlayer.ThisPlayer.ownedCountries.Length; ++i)
            {
                for (int j = 0; j < ActualPlayer.ThisPlayer.ownedCountries[i].neighbouringCountries.Length; ++j)
                {
                    if (ActualPlayer.ThisPlayer.ownedCountries[i].neighbouringCountries[j].owner != ActualPlayer.ThisPlayer)
                    {
                        PotentialAttackersAround[i] +=
                            ActualPlayer.ThisPlayer.ownedCountries[i].neighbouringCountries[j].unitsStationed - 1;
                        OverAllAttackers += ActualPlayer.ThisPlayer.ownedCountries[i].neighbouringCountries[j].unitsStationed - 1;
                    }
                }
            }
            double[] DefendValue = new double[PotentialAttackersAround.Length];
            for (int i = 0; i < DefendValue.Length; ++i)
            {
                DefendValue[i] = PotentialAttackersAround[i] / (double)OverAllAttackers;
                //ActualPlayer.ownedCountries[i].
            }
        }
        /// <summary>
        /// Analysiert andere Spieler aufgrund
        /// </summary>
        public void AnalyzeOtherPlayers()
        {
            for (int i = 0; i < Players.Length; ++i)
                if (Players[i] != ActualPlayer)
                {

                }
        }
        /// <summary>
        /// Berechnet die Anzahl der Einheiten die die Spieler in der nächsten Runde erhalten würden
        /// </summary>
        public void CalculateUnitsPerTurnForPlayers()
        {
            for (int j = 0; j < Players.Length; ++j)
            {
                int OutBuff = 0;

                int[] CountriesOfContsOfPlayer = new int[Continents.Length];
                for (int i = 0; i < CountriesOfContsOfPlayer.Length; ++i)
                    CountriesOfContsOfPlayer[i] = 0;

                for (int i = 0; i < Players[j].ThisPlayer.ownedCountries.Length; ++i)
                    CountriesOfContsOfPlayer[Players[j].ThisPlayer.ownedCountries[i].continent]++;

                for (int i = 0; i < Continents.Length; ++i)
                    if (Continents[i].ThisContinent.numberOfCountries == CountriesOfContsOfPlayer[i])
                        OutBuff += Continents[i].ThisContinent.AdditionalUnits;

                Players[j].ThisPlayer.UnitsPT = OutBuff + Players[j].ThisPlayer.ownedCountries.Length / 3;
            }
        }


        //Convertations
        /// <summary>
        /// Wandelt normale Countries in AICountries um
        /// nutzt dabei den speziellen Konstruktor der AICountry
        /// </summary>
        /// <param name="CountriesIn"></param>
        /// <returns></returns>
        internal AICountry[] ConvertCountryToAICountry(Country[] CountriesIn)
        {
            AICountry[] OutBuff = new AICountry[CountriesIn.Length];
            for (int i = 0; i < CountriesIn.Length; ++i)
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
            for (int i = 0; i < ContsIn.Length; ++i)
                OutBuff[i] = new AIContinent(ContsIn[i]);
            return OutBuff;
        }
        /// <summary>
        /// Wandelt normale Player in AIPlayer um
        /// verwendet speziellen Konstruktor von AIPlayer
        /// </summary>
        /// <param name="PlayersIn"></param>
        /// <returns></returns>
        internal AIPlayer[] ConvertPlayersToAIPlayers(Player[] PlayersIn)
        {
            AIPlayer[] OutBuff = new AIPlayer[PlayersIn.Length];
            for (int i = 0; i < PlayersIn.Length; ++i)
                OutBuff[i] = new AIPlayer(PlayersIn[i]);
            return OutBuff;

        }
        /// <summary>
        /// Wander normalen einzelnen Player in AIPlayer um
        /// verwendet speziellen Konstruktor von AIPlayer
        /// </summary>
        /// <param name="PlayerIn"></param>
        /// <returns></returns>
        internal AIPlayer ConvertSinglePlayerToAIPlayer(Player PlayerIn)
        {
            return new AIPlayer(PlayerIn);
        }
    }
}
