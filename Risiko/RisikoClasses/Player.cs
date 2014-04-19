using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Risiko
{
    internal class Player
    {
        /// <summary>
        /// Array aller besetzten Länder
        /// </summary>
        internal Country[] OwnedCountries;
        public Country[] ownedCountries
        {
            get { return OwnedCountries; }
            set { OwnedCountries = value; }
        }

        /// <summary>
        /// Name des Spielers
        /// </summary>
        internal string Name;
        public string name
        {
            get { return Name; }
            set
            {
                if (value != "")
                    Name = value;
            }
        }

        /// <summary>
        /// Die Anzahl der Männer in den besetzten Länder (Index passend zu OwnedCountries)
        /// </summary>
        internal int[] UnitsInCountries;
        public int[] unitsInCountry
        {
            get { return UnitsInCountries; }
            set { UnitsInCountries = value; }
        }

        /// <summary>
        /// Die Anzahl der Männer die der Spieler am Anfang des Zuges setzten kann
        /// wird berechnet
        /// </summary>
        internal int UnitsPT;
        public int unitsPT
        {
            get { return UnitsPT; }
            set
            {
                if (value >= 0)
                    UnitsPT = value;
            }
        }

        /// <summary>
        /// Legt fest ob Computergegner oder "richtiger" Spieler
        /// </summary>
        internal bool AIPlayer;

        /// <summary>
        /// Farbe des Spielers
        /// </summary>
        internal Color PlayerColor;
        public Color playerColor
        {
            get { return PlayerColor; }
            set { PlayerColor = value; }
        }

        /// <summary>
        /// Speichert die Anzahl der Männer die der Spieler bei Angriff pro
        /// Würfelrunde stellt
        /// 1 oder 2
        /// </summary>
        internal int NumberOfDefenders;
        public int numberOfDefenders
        {
            get { return NumberOfDefenders; }
            set
            {
                if (value == 1 | value == 2)
                    NumberOfDefenders = value;
            }
        }

        //Einstellungen
        /// <summary>
        /// Legt Angriffseinstellungen des Spielers fest
        /// -1: alle
        ///  0: immer 3
        /// >0: Anzahl
        /// </summary>
        internal int SettingAttack;
        public int settingAttack
        {
            get { return SettingAttack; }
            set { SettingAttack = value; }
        }
        /// <summary>
        /// Beendigung des Angriffs bei Verlust eines Prozentanteils
        /// 0 : keine warnung
        /// >0: Prozentzahl
        /// </summary>
        internal int SettingEndAttackLossPercentage;
        public int settingEndAttackLossPercentage
        {
            get { return SettingEndAttackLossPercentage; }
            set { SettingEndAttackLossPercentage = value; }
        }

        // Konstruktoren
        /// <summary>
        /// Basiskonstruktor
        /// </summary>
        public Player()
        {
            Name = "";
            UnitsPT = 0;
        }

        /// <summary>
        /// Konstruktor mit Name und besetzten Ländern
        /// </summary>
        /// <param name="NameIn"></param>
        /// <param name="OwnedCountriesIn"></param>
        public Player(string NameIn, Country[] OwnedCountriesIn)
        {
            Name = NameIn;
            OwnedCountries = OwnedCountriesIn;
        }

        /// <summary>
        /// Konstruktor mit Name und besetzten Ländern und PlayerTyp
        /// </summary>
        /// <param name="NameIn"></param>
        /// <param name="OwnedCountriesIn"></param>
        public Player(string NameIn, Country[] OwnedCountriesIn, bool IsAIPlayer)
        {
            Name = NameIn;
            OwnedCountries = OwnedCountriesIn;
            AIPlayer = IsAIPlayer;
        }

        public Player(string NameIn, bool IsAIPlayer, Color PlayerColorIn)
        {
            Name = NameIn;
            AIPlayer = IsAIPlayer;
            PlayerColor = PlayerColorIn;
            NumberOfDefenders = 1;
        }

        

        



        public void SetAllValues(string NameIn, Country[] OwnedCountriesIn, int[] UnitsInCountriesIn)
        {
            Name = NameIn;
            OwnedCountries = OwnedCountriesIn;
            UnitsInCountries = UnitsInCountriesIn;
        }

        /// <summary>
        /// Fügt ein Land dem StringArray der besitzenden Länder hinzu
        /// </summary>
        /// <param name="CountryName"></param>
        public void AddOwnedCountry(Country CountryIn)
        {
            if (ownedCountries != null)
            {
                Country[] newOwnedCountries = new Country[ownedCountries.Length + 1];
                for (int i = 0; i < ownedCountries.Length; ++i)
                {
                    newOwnedCountries[i] = ownedCountries[i];
                }
                newOwnedCountries[ownedCountries.Length] = CountryIn;

                ownedCountries = newOwnedCountries;
            }
            else
            {
                ownedCountries = new Country[1];
                ownedCountries[0] = CountryIn;
            }
        }

        public void TakeOwnedCountry(Country CountryIn)
        {
            Country[] OutBuff = new Country[OwnedCountries.Length-1];
            for (int i = 0, Counter = 0;i < OwnedCountries.Length;++i)
                if (OwnedCountries[i] != CountryIn) 
                    OutBuff[Counter++] = OwnedCountries[i];

            OwnedCountries = OutBuff;
        }

        /// <summary>
        /// Liefert Index des Landes mit Name CountryNameIn
        /// </summary>
        /// <param name="CountryNameIn"></param>
        /// <returns></returns>
        public int GetCountryIndexFromOwnedCountries(string CountryNameIn)
        {
            for (int i = 0;ownedCountries.Length > i;++i)
            {
                if (ownedCountries[i].name == CountryNameIn)
                    return i;
            }
            // error
            return -1;
        }
    }
}
