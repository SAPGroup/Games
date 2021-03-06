﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risiko
{
    class Country
    {
        /// <summary>
        /// Eckpunkte des Landes
        /// </summary>
        internal Point[] Corners;

        /// <summary>
        /// Name des Landes
        /// </summary>
        internal string Name;

        /// <summary>
        /// Farbe des Landes
        /// </summary>
        internal Color ColorOfCountry;

        /// <summary>
        /// Besitzender Spieler, -1 kein spieler
        /// </summary>
        internal Player Owner;

        /// <summary>
        /// Anzahl der Einheiten im Land
        /// </summary>
        internal int UnitsStationed;

        /// <summary>
        /// Array in dem die Nachbarländer gespeichert sind
        /// </summary>
        internal Country[] NeighbouringCountries;

        /// <summary>
        /// Speichert den Kontinent, zu dem das Land gehört
        /// 0-x , Kontinente, -1 unbelegt (noch nicht initialisiert)
        /// </summary>
        internal int Continent = -1;


        // Konstruktoren
        /// <summary>
        /// Basiskonstruktor, Corners werden nicht gesetzt, Name = leer
        /// </summary>
        public Country()
        {
            Name = "";
            ColorOfCountry = Color.Black;
            UnitsStationed = 0;
            Owner = null;
        }

        /// <summary>
        /// veränderterKonstruktor
        /// </summary>
        /// <param name="NameIn"></param>
        /// <param name="CornersIn"></param>
        public Country(string NameIn, Point[] CornersIn, Color ColorIn)
        {
            Corners = CornersIn;
            Name = NameIn;
            ColorOfCountry = ColorIn;
        }

        /// <summary>
        /// veränderterKonstruktor
        /// </summary>
        /// <param name="NameIn"></param>
        /// <param name="CornersIn"></param>
        public Country(string NameIn, Point[] CornersIn, Color ColorIn, Player OwnerIn, int UnitsIn)
        {
            Corners = CornersIn;
            Name = NameIn;
            ColorOfCountry = ColorIn;
            Owner = OwnerIn;
            UnitsStationed = UnitsIn;
        }

        /// <summary>
        /// veränderterKonstruktor
        /// </summary>
        /// <param name="NameIn"></param>
        /// <param name="CornersIn"></param>
        public Country(string NameIn, Point[] CornersIn, Color ColorIn, Country[] NeighbouringCountriesIn, int ContinentIn)
        {
            Corners = CornersIn;
            Name = NameIn;
            ColorOfCountry = ColorIn;
            NeighbouringCountries = NeighbouringCountriesIn;
            Continent = ContinentIn;
        }

        /// <summary>
        /// veränderterKonstruktor
        /// </summary>
        /// <param name="NameIn"></param>
        /// <param name="CornersIn"></param>
        public Country(string NameIn, Point[] CornersIn, Color ColorIn, Country[] NeighbouringCountriesIn)
        {
            Corners = CornersIn;
            Name = NameIn;
            ColorOfCountry = ColorIn;
            NeighbouringCountries = NeighbouringCountriesIn;
        }


        //
        //Set für alle Variablen
        /// <summary>
        /// Setzt die Eigenschaften eines Objekts
        /// </summary>
        /// <param name="CornersIn"></param>
        /// <param name="NameIn"></param>
        public void SetValues(Point[] CornersIn, string NameIn, Color ColorIn, Player OwnerIn, int UnitsStationedIn)
        {
            Corners = CornersIn;
            Name = NameIn;
            ColorOfCountry = ColorIn;
            UnitsStationed = UnitsStationedIn;
            Owner = OwnerIn;
        }

        //
        // Set- und Get-Methoden
        /// <summary>
        /// Set- und Get- Methoden für Name
        /// </summary>
        public string name
        {
            get { return Name; }
            internal set { Name = value; }
        }

        /// <summary>
        /// Set und Get der Ecken des Vielecks
        /// </summary>
        public Point[] corners
        {
            get { return Corners; }
            internal set { Corners = value; }
        }

        /// <summary>
        /// Set und Get der Farbe des Landes
        /// </summary>
        public Color colorOfCountry
        {
            get { return ColorOfCountry; }
            set { ColorOfCountry = value; }
        }

        /// <summary>
        /// Set und Get des Besitzers
        /// </summary>
        public Player owner
        {
            get { return Owner; }
            set { Owner = value; }
        }

        /// <summary>
        /// Set und Get der Stationierten Einheiten
        /// </summary>
        public int unitsStationed
        {
            get { return UnitsStationed; }
            set
            {
                if (value >= 0)
                    UnitsStationed = value;
            }
        }

        /// <summary>
        /// Set und Get der benachbarten Länder
        /// </summary>
        public Country[] neighbouringCountries
        {
            get { return NeighbouringCountries; }
            set { NeighbouringCountries = value; }
        }

        /// <summary>
        /// Set- und Get- des Kontinent, zu dem das Land gehört
        /// </summary>
        public int continent
        {
            get { return Continent; }
            set { Continent = value; }
        }
    }
}
