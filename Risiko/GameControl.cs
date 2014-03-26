using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Risiko
{
    internal class GameControl
    {    
        // Verbindung zu Main
        internal RisikoMain Main;
        // Spielfeld
        internal GameField Field = new GameField();
        public GameField field
        {
            get { return Field; }
        }
        // Spieler
        /// <summary>
        /// verschiedene Spieler
        /// </summary>
        internal Player[] Players;
        public Player[] players
        {
            get { return Players; }
            set { Players = value; }
        }
        /// <summary>
        /// Aktueller Status, 0 Setzen der Spieler am Anfang des Spiels
        /// 1 setzen vor jeder Runde der Spieler
        /// 2 angreifen, 3 ziehen
        /// </summary>
        internal int GameState = -1;
        /// <summary>
        /// Aktueller Spieler
        /// </summary>
        internal Player ActualPlayer;
        public Player actualPlayer
        {
            get { return ActualPlayer; }
            set { ActualPlayer = value; }
        }


        /// <summary>
        /// Zum Ein-Auslesen aus 
        /// </summary>
        internal FileStream fs;
        internal StreamReader sr;
        internal StreamWriter sw;// TODO: abspeichern 

        /// <summary>
        /// Quelle aus der die Karte gelesen werden soll TODO: veränderbare Quelldatei
        /// </summary>
        internal string TxtDataSource = "WorldmapWithTabel.txt";
        public string txtDataSource
        {
            get { return TxtDataSource; }
            set { TxtDataSource = value; }
        }
        /// <summary>
        /// String Array aus Quelldatei
        /// </summary>
        internal string[] TxtSource;
        
        
        //ZufallszahlenGenerator
        Random rnd = new Random();


        //Einheiten
        // Array der die Anzahl der Einheiten die der Spieler setzen möchte speichert
        // in die Länder in seinem Besitz
        internal int[] UnitsToAdd;
        public int[] unitsToAdd
        {
            get { return UnitsToAdd; }
            set { UnitsToAdd = value; }
        }

        internal bool StartUnitAdding = false;
        public bool startUnitAdding
        {
            get { return StartUnitAdding; }
            set { StartUnitAdding = value; }
        }
        

        // ZUM ZEICHNEN
        // false wenn Karte noch gar nicht gezeichnet wurde
        public bool DrawnMap = false;
        // false echte SpielerFarben, sonst blasser
        public bool DrawPale = false;
        internal Color ColorCountrySelected = Color.Yellow;

        //Moved
        // temporärer Index des zuletzt überfahrenen Landes
        internal int tempMovedIndex = -1;
        // temporärer Speicher eines überfahrenen Landes
        internal Color tempMovedCountryColor;

        //Clicked
        // Index des zuletzt angeklickten Landes, bei Angreifen und Ziehen (game.gamestate 2 und 3) wichtig
        /// <summary>
        /// bei Gamestate 2: Fst eigenes Land, Scnd Gegnerisches Nachbarland
        /// bei Gamestate 3: Fst eigenes Land von dem Einheiten abgezogen werden, Scnd neues eigenes Land der Einheiten
        /// </summary>
        internal int tempClickedFstIndex = -1;
        internal Color tempClickedFstCountryColor;
        internal int tempClickedScndIndex = -1;
        internal Color tempClickedScndCountryColor;


        // Faktor zum zeichnen
        internal int Factor;
        public int factor
        {
            get { return Factor; }
        }
        
        
        //Einstellungen
        internal bool AutoLanderkennung = true;
        public bool autoLanderkennung
        {
            get { return AutoLanderkennung; }
        }

        // Konstruktor
        public GameControl(RisikoMain MainIn)
        {
            Main = MainIn;
        }
        public GameControl(){}


        //Karte zeichnen
        /// <summary>
        /// Allgemeines Karte zeichnen
        /// verzweigung mit und ohne Laden aus DB
        /// </summary>
        public void DrawMap()
        {
            if (DrawnMap)
                DrawMapWoLoad();
            else
                DrawAndLoadMap();
        }
        /// <summary>
        /// Lädt Daten der Karte aus DB und lässt diese anschließend von Main zeichnen
        /// </summary>
        internal void DrawAndLoadMap()
        {
            // Quelldatei auslesen
            TxtSource = LoadStringsFromTxtSource();
            //Länder aus QuellStringArray lesen
            LoadCountriesFromTxtSource();
            //Kontinente aus QuellStringArray lesen
            LoadContinentsFromTxtSource();

            int[] WidthHeight = Main.GetMapData();
            CheckFactor(WidthHeight[0], WidthHeight[1]);

            // lässt Karte zeichnen
            Main.DrawMap();

            DrawnMap = true;
        }
        /// <summary>
        /// Zeichnet Karte bei veränderung des Faktors erneut, ohne laden
        /// </summary>
        internal void DrawMapWoLoad()
        {
            // alten Faktor zwischenspeichern
            int tempOldFactor = Factor;
            // neuen Faktor auslesen
            int[] WidthHeight = Main.GetMapData();
            CheckFactor(WidthHeight[0], WidthHeight[1]);
            // bei veränderung neu zeichnen TODO: DrawFlag ?? 
            if(tempOldFactor != Factor)
                Main.DrawMap();
        }
        /// <summary>
        /// Zeichnet Land vollständig, mit MiddleUnits
        /// diese Methode immer aufrufen, modularer
        /// </summary>
        /// <param name="Index"></param>
        internal void DrawCountryFull(int Index)
        {
            Main.DrawCountry(Index);
            Main.DrawMiddleUnits(Index);
        }

        // "Datenbank", inzwischen TxtDatei
        /// <summary>
        /// Lädt den gesamten Inhalt der Txt-Source-Datei in String-Array
        /// </summary>
        /// <returns></returns>
        internal string[] LoadStringsFromTxtSource()
        {
            string[] OutBuf = new string[0];

            // initialisieren der Reader, um aus Txt-Datei zu lesen
            fs = new FileStream(TxtDataSource, FileMode.Open);
            sr = new StreamReader(fs);
            string zeile;

            while (sr.Peek() != -1)
            {
                // nächste Zeile in zeile speichern
                zeile = sr.ReadLine();
                OutBuf = AddStringToStringArray(zeile, OutBuf);
            }
            sr.DiscardBufferedData();
            sr.Close();

            return OutBuf;
        }
        /// <summary>
        /// Liest Länder aus TxtSource aus
        /// </summary>
        internal void LoadCountriesFromTxtSource()
        {
            // Prüfen ob Auslesen erfolgreich war, sonst nichts tun
            if (TxtSource != null)
            {
                //StartIndex der Länder und Länge
                int StartIndex = 0, Length = 0;
                SearchForSpecialPartFromTxtSource(ref StartIndex, ref Length, TxtSource, "Countries");

                // erzeugt Länder Array und setzt Länge dessen in Field fest
                Field.numberOfCountries = Length;
                Field.countries = new Country[Length];



                // !!!             Länder auslesen          !!!


                // temp-Werte der Länder
                string tempName;
                Color tempColor;
                Point[] tempPoints;
                // Um die Maximalen Werte der Karte auszulesen
                int tempMaxX = 0, tempMaxY = 0;

                for (int j = 0; j < Length; ++j)
                {
                    string zeile = TxtSource[StartIndex + j];
                    //Kommentarzeilen überspringen
                    if (zeile.StartsWith("//"))
                        continue;
                    //Tabs und Leerzeichen entfernen
                    zeile = zeile.Trim('\t', ' ');
                    // Zeile zerlegen
                    string[] Parts = zeile.Split('.');

                    // Name und Farbe des Landes auslesen
                    tempColor = GetColorFromString(Parts[0]);
                    tempName = Parts[1];

                    // Eckpunkte des Landes auslesen, -> String wieder zerlegen
                    string[] Corners = Parts[4].Split(';');
                    // Länge von tempPoints festlegen
                    tempPoints = new Point[Corners.Length / 2];
                    for (int i = 0; i < Corners.Length / 2; i++)
                    {
                        tempPoints[i].X = Convert.ToInt32(Corners[i * 2]);
                        tempPoints[i].Y = Convert.ToInt32(Corners[i * 2 + 1]);
                        if (Convert.ToInt32(Corners[i * 2]) > tempMaxX)
                            tempMaxX = Convert.ToInt32(Corners[i * 2]);
                        if (Convert.ToInt32(Corners[i * 2 + 1]) > tempMaxY)
                            tempMaxY = Convert.ToInt32(Corners[i * 2 + 1]);
                    }
                    Field.countries[j] = new Country(tempName, tempPoints, tempColor);
                }
                Field.height = tempMaxY;
                Field.width = tempMaxX;


                for (int j = 0; j < Length; ++j)
                {
                    string zeile = TxtSource[StartIndex + j];
                    //Kommentarzeilen überspringen
                    if (zeile.StartsWith("//"))
                        continue;
                    // Leerzeichen und Tabs entfernen
                    zeile = zeile.Trim('\t', ' ');
                    // Zeile zerlegen
                    string[] Parts = zeile.Split('.');

                    string[] Neighbours = Parts[5].Split(';');
                    string[] tempNeighbouringCountries = new string[Neighbours.Length];
                    for (int i = 0; i < Neighbours.Length; ++i)
                    {
                        int tempCountryID = Convert.ToInt32(Neighbours[i]);
                        tempNeighbouringCountries[i] = Field.countries[tempCountryID - 1].name;
                    }

                    Field.countries[j].neighbouringCountries = tempNeighbouringCountries;
                }
            }
        }
        /// <summary>
        /// Liest Kontinente aus TxtSource aus
        /// </summary>
        internal void LoadContinentsFromTxtSource()
        {
            //Fehler verhindern
            if (TxtSource != null)
            {
                //StartIndex der Kontinente und Länge
                int StartIndex = 0, Length = 0;
                SearchForSpecialPartFromTxtSource(ref StartIndex, ref Length, TxtSource, "Continents");

                Field.continents = new Continent[Length];

                // Kontinente selbst erzeugen
                // Vars
                string tempName;
                int tempAddUnits;
                int tempNumCountries;

                for (int i = 0; i < Length; ++i)
                {
                    string zeile = TxtSource[StartIndex + i];
                    //Kommentarzeilen überspringen
                    if (zeile.StartsWith("//"))
                        continue;
                    //Tabs und Leerzeichen entfernen
                    zeile = zeile.Trim('\t', ' ');
                    // Zeile zerlegen
                    string[] Parts = zeile.Split('.');

                    tempName = Parts[0];
                    tempAddUnits = Convert.ToInt32(Parts[1]);
                    tempNumCountries = Convert.ToInt32(Parts[2]);

                    Field.continents[i] = new Continent(tempAddUnits, tempName, tempNumCountries);
                }

                // In Countries Verweis erstellen
                SearchForSpecialPartFromTxtSource(ref StartIndex, ref Length, TxtSource, "Countries");
                int tempContinentIndex;
                for (int i = 0; i < Length; ++i)
                {
                    string zeile = TxtSource[StartIndex + i];
                    //Kommentarzeilen überspringen
                    if (zeile.StartsWith("//"))
                        continue;
                    //Tabs und Leerzeichen entfernen
                    zeile = zeile.Trim('\t', ' ');
                    // Zeile zerlegen
                    string[] Parts = zeile.Split('.');

                    tempContinentIndex = Convert.ToInt32(Parts[6]) - 1;
                    Field.countries[i].continent = tempContinentIndex;
                }
            }
        }
        /// <summary>
        /// Durchsucht String-Array nach "Start "+ToSearch und "End "+ToSearch
        /// und gibt Index von Start+1 und Differenz von Start und Ende aus
        /// indirekt da mehr als ein Rückgabewert
        /// 
        /// !!! Kommentare werden auch berücktsichtigt
        /// </summary>
        /// <param name="StartIndex"></param>
        /// <param name="Length"></param>
        /// <param name="ToSearchThrough"></param>
        /// <param name="ToSearch"></param>
        internal void SearchForSpecialPartFromTxtSource(ref int StartIndex, ref int Length, string[] ToSearchThrough, string ToSearch)
        {
            // Nicht gefunden, wenn diese Werte zurückgegeben werden
            StartIndex = -1;
            Length = -1;
            // Eigentliche Methode
            bool Started = false;
            for (int i = 0; ToSearchThrough.Length > i; ++i)
            {
                if (!Started & ToSearchThrough[i].StartsWith("Start " + ToSearch))
                {
                    Started = true;
                    StartIndex = i + 1;
                }
                else if (Started & ToSearchThrough[i].StartsWith("End " + ToSearch))
                {
                    Length = i - StartIndex;
                    return;
                }
            }
        }

        // Gamestate
        /// <summary>
        /// Geht über zu nächstem GameState, falls Ende eines Zuges
        /// -> nächster Spieler
        /// </summary>
        public void nextGameState()
        {
            if (++GameState > 3)
            {
                GameState = 1;
                int actualPlayer = ActualPlayerIndex() + 1;
                if (actualPlayer >= Players.Length)
                    actualPlayer = 0;
                ActualPlayer = Players[actualPlayer];
            } 
        }


        //Maus-Aktionen
        /// <summary>
        /// MouseBewegung über Map
        /// </summary>
        /// <param name="e"></param>
        public void MouseMoved(MouseEventArgs e)
        {
            // ist Einstellung
            if (autoLanderkennung)
            {
                // Speichert in temp den Index des Landes ab, über dem sich der Mauszeiger befindet
                int temp = CheckClickInCountry(new Point(e.X, e.Y));

                // Kein Treffer
                if (temp == -1 & tempMovedIndex != -1)//& tempOldIndex != tempIndex)
                {
                    //kein Treffer
                    Field.countries[tempMovedIndex].colorOfCountry = tempMovedCountryColor;

                    DrawCountryFull(tempMovedIndex);
                    tempMovedIndex = -1;
                }
                // Bei Treffer
                else if (temp != -1 & temp != tempMovedIndex)
                {
                    //zuvor Bereits Land ausgewählt                
                    if (tempMovedIndex != -1)
                    {
                        Field.countries[tempMovedIndex].colorOfCountry = tempMovedCountryColor;
                        DrawCountryFull(tempMovedIndex);
                    }

                    tempMovedCountryColor = Field.countries[temp].colorOfCountry;
                    Field.countries[temp].colorOfCountry = ColorCountrySelected;

                    tempMovedIndex = temp;

                    DrawCountryFull(temp);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public void MouseClicked(MouseEventArgs e)
        {
            int tempCountry = CheckClickInCountry(new Point(e.X, e.Y));

            // Wenn Klick in Land und Linke Maustaste
            if (tempCountry != -1 & e.Button == MouseButtons.Left)
            {
                if (GameState == 2)
                    MouseClickedAttackMode(tempCountry);
                else if (GameState == 3)
                    ;// TODO: Methode fürs ziehen
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Fst und Scnd werden resettet
                ResetClickedCountries(3);
            }
            else if (tempCountry == -1)
            {
                // TODO: nur 1 clicked Land zurücksetzen, bei 2 scnd, bei 1 fst
            }
        }

        /// <summary>
        /// resettet tempClickedFst und/oder Scnd
        /// 3: Fst und Scnd werden resettet
        /// 2: nur Scnd
        /// 1: nur Fst 
        /// </summary>
        /// <param name="Clicked"></param>
        private void ResetClickedCountries(int Clicked)
        {
            if (Clicked == 3)
            {
                // First resetten
                Field.countries[tempClickedFstIndex].colorOfCountry = tempClickedFstCountryColor;
                DrawCountryFull(tempClickedFstIndex);
                tempClickedFstIndex = -1;
                //Second resetten
                Field.countries[tempClickedScndIndex].colorOfCountry = tempClickedScndCountryColor;
                DrawCountryFull(tempClickedScndIndex);
                tempClickedScndIndex = -1;
            }
            else if (Clicked == 2)
            {
                //nur Second resetten
                Field.countries[tempClickedScndIndex].colorOfCountry = tempClickedScndCountryColor;
                DrawCountryFull(tempClickedScndIndex);
                tempClickedScndIndex = -1;
            }
            else if (Clicked == 1)
            {
                //nur First resetten
                Field.countries[tempClickedFstIndex].colorOfCountry = tempClickedFstCountryColor;
                DrawCountryFull(tempClickedFstIndex);
                tempClickedFstIndex = -1;
            }
        }

        /// <summary>
        /// bei Gamestate = 2, AngriffsModus, 2 Länder auswählbar
        /// 1 eigenes, 1 gegnerisches
        /// </summary>
        /// <param name="tempCountry"></param>
        public void MouseClickedAttackMode(int tempCountry)
        {
            if (tempClickedFstIndex == -1)
            {
                //eigenes Land
                if (actualPlayer == Field.countries[tempCountry].owner)
                {
                    // Index und Farbe abspeichern
                    tempClickedFstCountryColor = Field.countries[tempCountry].colorOfCountry;
                    tempClickedFstIndex = tempCountry;

                    // Gelb machen
                    Field.countries[tempClickedFstIndex].colorOfCountry = ColorCountrySelected;
                    // zeichnen
                    DrawCountryFull(tempClickedFstIndex);

                    // TODO: TrackBar setzen
                }
                // Nicht eigenes Land
                else
                {
                    Main.ShowMessage("Wählen Sie eines Ihrer eigenen Länder.");
                    Main.timerDeleteMessage.Enabled = true;
                }
            }
            // Zuvor schon Land angeklickt
            else if (tempClickedFstIndex != -1)
            {
                // gegnerisches Land
                if (actualPlayer != Field.countries[tempCountry].owner)
                {
                    // gegnerisches Nachbarland
                    if (Field.CountriesAreNeighbours(tempClickedFstIndex, tempCountry))
                    {
                        //Index und Farbe abspeichern
                        tempClickedScndIndex = tempCountry;
                        tempClickedScndCountryColor = Field.countries[tempCountry].colorOfCountry;

                        // Gelb machen
                        Field.countries[tempClickedScndIndex].colorOfCountry = ColorCountrySelected;
                        // zeichnen
                        DrawCountryFull(tempClickedScndIndex);
                    }
                    // zwar gegnerisches Land, jedoch kein Nachbar
                    else
                    {
                        Main.ShowMessage("Wählen Sie ein benachbartes gegnerisches Land.");
                        Main.timerDeleteMessage.Enabled = true;
                    }
                }
                // eigenes Land
                else
                {
                    Main.ShowMessage("Wählen Sie ein gegnerisches Land.");
                    Main.timerDeleteMessage.Enabled = true;
                }
            }
        }




        // Sonstiges
        /// <summary>
        /// Liefert Index des aktuellen Spielers in Players-Array
        /// </summary>
        /// <returns></returns>
        internal int ActualPlayerIndex()
        {
            for (int i = 0;i < Players.Length;++i)
            {
                if (Players[i].name == ActualPlayer.name)
                    return i;
            }
            return -1;
        }
        
        /// <summary>
        /// Fügt String, an Stringarray an
        /// und gibt erweiterten Array zurück
        /// </summary>
        /// <param name="ToAdd"></param>
        /// <param name="StringArray"></param>
        /// <returns></returns>
        internal string[] AddStringToStringArray(string ToAdd, string[] StringArray)
        {
            string[] OutBuf = new string[StringArray.Length+1];
            for (int i = 0;i < StringArray.Length;++i)
            {
                OutBuf[i] = StringArray[i];
            }
            OutBuf[StringArray.Length] = ToAdd;
            return OutBuf;
        }
        
        /// <summary>
        /// Liefert die zusätzlichen Einheiten eines Spielers von Kontinenten zurück
        /// </summary>
        /// <returns></returns>
        public int GiveAddUnitsFromContsForActualPlayer()
        {
            int OutBuff = 0;

            int[] CountriesOfContsOfPlayer = new int[Field.continents.Length];
            for (int i = 0;i < CountriesOfContsOfPlayer.Length;++i)
            {
                CountriesOfContsOfPlayer[i] = 0;
            }
                
            for (int i = 0; i < ActualPlayer.ownedCountries.Length; ++i)
            {
                CountriesOfContsOfPlayer[ActualPlayer.ownedCountries[i].continent]++;
            }

            for (int i = 0;i < Field.continents.Length;++i)
            {
                if (Field.continents[i].numberOfCountries == CountriesOfContsOfPlayer[i])
                    OutBuff += Field.continents[i].AdditionalUnits;
            }

            return OutBuff;
        }

        /// <summary>
        /// Setzt den Faktor der Darstellung der Karte
        /// </summary>
        internal void CheckFactor(int newWidth, int newHeight)
        {
            int temp1 = newWidth / Field.width;
            int temp2 = newHeight / Field.height;
            if (temp1 > temp2)
                Factor = temp2;
            else
                Factor = temp1;
        }

        /// <summary>
        /// wandelt tempString in Farbe um
        /// Muss leider alles vorgegeben werden (die möglichen farben, da sonst nur weiß zurückgegeben wird)
        /// </summary>
        /// <param name="tempColor"></param>
        /// <returns></returns>
        public Color GetColorFromString(string tempColor)
        {
            tempColor = tempColor.Trim('\t', ' ', '\'');
            if (tempColor == "blue")
                return Color.Blue;
            else if (tempColor == "green")
                return Color.Green;
            else if (tempColor == "yellow")
                return Color.Yellow;
            else if (tempColor == "red")
                return Color.Red;
            else if (tempColor == "white")
                return Color.White;
            else if (tempColor == "black")
                return Color.Black;
            else if (tempColor == "violet")
                return Color.Violet;
            else if (tempColor == "orange")
                return Color.Orange;
            else
                return Color.White;
        }

        /// <summary>
        /// Liefert den Index des Landes zurück
        /// über dem die Maus ist oder auf das geklickt wurde
        /// -1 -> kein Land
        /// ansonsten Index des Landes
        /// </summary>
        /// <param name="ClickedPosition"></param>
        /// <returns></returns>
        public int CheckClickInCountry(Point ClickedPosition)
        {
            if (DrawnMap)
            {
                //Länder, die überprüft werden sollen, werden in Array checkCountries[] geladen
                Country[] checkCountries = Field.countries;

                for (int i = 0; i < checkCountries.Length; ++i)
                {
                    Point[] tempPoints = Field.GiveCountry(i).corners;
                    Point[] realPoints = new Point[Field.GiveCountry(i).corners.Length];

                    for (int j = 0; j < realPoints.Length; ++j)
                    {
                        realPoints[j].X = (tempPoints[j].X * Factor);
                        realPoints[j].Y = (tempPoints[j].Y * Factor);
                    }

                    if (PointInPolygon(ClickedPosition, realPoints))
                        return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Checkt ob Punkt P ind Polygon Polygon
        /// true = innherhalb des Polygons
        /// false = außerhalb
        /// </summary>
        /// <param name="P"></param>
        /// <param name="Polygon"></param>
        /// <returns></returns>
        public bool PointInPolygon(Point P, Point[] Polygon)
        {
            Point P1, P2;

            bool Inside = false;

            if (Polygon.Length < 3)
                return Inside;

            Point oldPoint = new Point(Polygon[Polygon.Length - 1].X, Polygon[Polygon.Length - 1].Y);

            for (int i = 0; i < Polygon.Length; ++i)
            {
                Point newPoint = new Point(Polygon[i].X, Polygon[i].Y);

                if (newPoint.X > oldPoint.X)
                {
                    P1 = oldPoint;
                    P2 = newPoint;
                }
                else
                {
                    P1 = newPoint;
                    P2 = oldPoint;
                }

                if ((newPoint.X < P.X) == (P.X <= oldPoint.X) &&
                    ((long)P.Y - (long)P1.Y) * (long)(P2.X - P1.X) < ((long)P2.Y - (long)P1.Y) * (long)(P.X - P1.X))
                    Inside = !Inside;
                oldPoint = newPoint;
            }
            return Inside;
        }

        /// <summary>
        /// Liefert Mittel-Punkt eines Polygons zurück
        /// In Form1, da in Game.Countries.Corners nur die Eckpunkte des "kleinen",
        /// internen Polygons gespeichert sind
        /// </summary>
        /// <param name="realPoints"></param>
        /// <returns></returns>
        public Point GetMiddleOfPolygon(Point[] Points)
        {
            double Area = 0.0;
            double MiddleX = 0.0;
            double MiddleY = 0.0;

            for (int i = 0, j = Points.Length - 1; i < Points.Length; j = i++)
            {
                float temp = Points[i].X * Points[j].Y - Points[j].X * Points[i].Y;
                Area += temp;
                MiddleX += (Points[i].X + Points[j].X) * temp;
                MiddleY += (Points[i].Y + Points[j].Y) * temp;
            }

            Area *= 3;
            return new Point((int)(MiddleX / Area), (int)(MiddleY / Area));
        }

        /// <summary>
        /// Liefert die echten Bildpunkte des Landes zurück, nicht die des "kleinen"
        /// Koordinatenfeldes, die in Corners gespeichert ist
        /// </summary>
        /// <param name="Corners"></param>
        /// <returns></returns>
        public Point[] GetRealPointsFromCorners(Point[] Corners)
        {
            Point[] realPoints = new Point[Corners.Length];

            for (int i = 0; i < realPoints.Length; ++i)
            {
                realPoints[i].X = (Corners[i].X * Factor);
                realPoints[i].Y = (Corners[i].Y * Factor);
            }
            return realPoints;
        }




        // OLD
        ///// <summary>
        ///// Reader, zum Lesen aus der Datenbank
        ///// </summary>
        //internal OleDbConnection con = new OleDbConnection();
        //internal OleDbCommand cmd = new OleDbCommand();
        //internal OleDbDataReader reader;
        ///// <summary>
        ///// Pfad der Quelldatei!!
        ///// syntax: con.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
        ///// "Data Source=C:\\Temp\\Risiko_Weltkarte.accdb";
        ///// 
        /////
        ///// </summary>
        //internal string DataSourceString = System.Environment.CurrentDirectory + "\\Risiko_Weltkarte2.accdb";
        ///// <summary>
        ///// Lädt Länder aus DB
        ///// </summary>
        //internal void LoadCountriesFromDBSource()
        //{
        //    // Source einbinden
        //    //DataSourceString = System.Environment.CurrentDirectory + "\\Risiko_Weltkarte1.accdb";
        //    con.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
        //                          "Data Source=" + DataSourceString;
        //    // Anzahl der Länder auslesen
        //    GetNumberOfCountriesDB();
        //    cmd.Connection = con;
        //    // Aus table Weltkarte (alles)
        //    cmd.CommandText = "select * from Worldmap;";
        //    // Länder erzeugen
        //    Field.countries = new Country[Field.numberOfCountries];
        //    try
        //    {
        //        //öffnen
        //        con.Open();
        //        reader = cmd.ExecuteReader();
        //        // Fortlaufender Zähler, zählt welche Country aktuell erzeugt werden muss
        //        int counter = 0;
        //        // temp Werte, die später dem Konstruktor der Country zugeführt werden
        //        Color tempColorOfCountry;
        //        string tempName;
        //        Point[] tempPoints;
        //        // Max X und Y Werte, um Höhe und Breite der internen "kleinen" Karte herauszufinden
        //        int tempMaxX = 0;
        //        int tempMaxY = 0;
        //        while (reader.Read())
        //        {
        //            // Name
        //            tempName = Convert.ToString(reader["Name"]);
        //            // Color (Farbe)
        //            tempColorOfCountry = GetColorFromString(Convert.ToString(reader["Color"]));
        //            // Corners (Ecken)
        //            string tempCorners = Convert.ToString(reader["Corners"]);
        //            string[] Corners = tempCorners.Split(';');
        //            tempPoints = new Point[Corners.Length / 2];
        //            for (int i = 0; i < Corners.Length / 2; ++i)
        //            {
        //                tempPoints[i].X = Convert.ToInt32(Corners[i * 2]);
        //                tempPoints[i].Y = Convert.ToInt32(Corners[i * 2 + 1]);
        //                if (Convert.ToInt32(Corners[i * 2]) > tempMaxX)
        //                    tempMaxX = Convert.ToInt32(Corners[i * 2]);
        //                if (Convert.ToInt32(Corners[i * 2 + 1]) > tempMaxY)
        //                    tempMaxY = Convert.ToInt32(Corners[i * 2 + 1]);
        //            }
        //            // Konstruktor der Country
        //            Field.countries[counter] = new Country(tempName, tempPoints, tempColorOfCountry);
        //            counter++;
        //        }
        //        Field.width = tempMaxX;
        //        Field.height = tempMaxY;
        //        reader.Close();
        //        con.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        // temp String, falls Fehlermeldung
        //        string temp = ex.Message;
        //    }
        //    // Laden der NachbarLänder, Countries muss schon festlegen, da daraus Namen
        //    // der Länder gelesen werden
        //    cmd.CommandText = "select * from Worldmap;";
        //    try
        //    {
        //        //öffnen
        //        con.Open();
        //        reader = cmd.ExecuteReader();
        //        // Fortlaufender Zähler, zählt welche Country aktuell erzeugt werden muss
        //        int counter = 0;
        //        while (reader.Read())
        //        {
        //            string tempNeighbours = Convert.ToString(reader["Neighbours"]);
        //            string[] Neighbours = tempNeighbours.Split(';');
        //            string[] tempNeighbouringCountries = new string[Neighbours.Length];
        //            for (int i = 0; i < Neighbours.Length; ++i)
        //            {
        //                int tempCountryID = Convert.ToInt32(Neighbours[i]);
        //                tempNeighbouringCountries[i] = Field.countries[tempCountryID - 1].name;
        //            }
        //            Field.countries[counter].neighbouringCountries = tempNeighbouringCountries;
        //            counter++;
        //        }
        //        reader.Close();
        //        con.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        // temp String, falls Fehlermeldung
        //        string temp = ex.Message;
        //        MessageBox.Show(temp);
        //    }
        //}
        ///// <summary>
        ///// Speichert Anzahl der Länder in Field.numberOfCountries ab
        ///// </summary>
        //internal void GetNumberOfCountriesDB()
        //{
        //    cmd.Connection = con;
        //    cmd.CommandText = "select * from Worldmap;";
        //    try
        //    {
        //        //öffnen
        //        con.Open();
        //        reader = cmd.ExecuteReader();
        //        // Anzahl der Länder
        //        int tempNumberOfCountries = 0;
        //        while (reader.Read())
        //        {
        //            ++tempNumberOfCountries;
        //        }
        //        Field.numberOfCountries = tempNumberOfCountries;
        //        reader.Close();
        //        con.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        string temp = ex.Message;
        //    }
        //}
        ///// <summary>
        ///// Lädt Karte aus Textdatei
        ///// </summary>
        //internal void LoadCountriesFromTxtSource()
        //{
        //    // initialisieren der Reader, um aus Txt-Datei zu lesen
        //    fs = new FileStream(TxtDataSource, FileMode.Open);
        //    sr = new StreamReader(fs);
        //    int NumberOfCountries = 0;
        //    string zeile;
        //    while (sr.Peek() != -1)
        //    {
        //        // nächste Zeile in zeile speichern
        //        zeile = sr.ReadLine();
        //        ++NumberOfCountries;
        //    }
        //    // erzeugt Länder Array und setzt Länge dessen in Field fest
        //    Field.numberOfCountries = NumberOfCountries;
        //    Field.countries = new Country[NumberOfCountries];
        //    // springt an Anfang der Datei
        //    fs.Position = 0;
        //    sr.DiscardBufferedData();
        //    // temp-Werte der Länder
        //    string tempName;
        //    Color tempColor;
        //    Point[] tempPoints;
        //    // Um die Maximalen Werte der Karte auszulesen
        //    int tempMaxX = 0, tempMaxY = 0;
        //    // Zähler für die erzeugten Länder
        //    int Counter = 0;
        //    while (sr.Peek() != -1)
        //    {
        //        // nächste Zeile in zeile speichern
        //        zeile = sr.ReadLine();
        //        //Kommentarzeilen überspringen
        //        if (zeile[0] == '/' & zeile[1] == '/')
        //            continue;
        //        //Tabs und Leerzeichen entfernen
        //        zeile = zeile.Trim('\t', ' ');
        //        // Zeile zerlegen
        //        string[] Parts = zeile.Split('.');
        //        // Name und Farbe des Landes auslesen
        //        tempColor = GetColorFromString(Parts[0]);
        //        tempName = Parts[1];
        //        // Eckpunkte des Landes auslesen, -> String wieder zerlegen
        //        string[] Corners = Parts[4].Split(';');
        //        // Länge von tempPoints festlegen
        //        tempPoints = new Point[Corners.Length / 2];
        //        for (int i = 0; i < Corners.Length / 2; i++)
        //        {
        //            tempPoints[i].X = Convert.ToInt32(Corners[i * 2]);
        //            tempPoints[i].Y = Convert.ToInt32(Corners[i * 2 + 1]);
        //            if (Convert.ToInt32(Corners[i * 2]) > tempMaxX)
        //                tempMaxX = Convert.ToInt32(Corners[i * 2]);
        //            if (Convert.ToInt32(Corners[i * 2 + 1]) > tempMaxY)
        //                tempMaxY = Convert.ToInt32(Corners[i * 2 + 1]);
        //        }
        //        Field.countries[Counter] = new Country(tempName, tempPoints, tempColor);
        //        Counter++;
        //    }
        //    Field.height = tempMaxY;
        //    Field.width = tempMaxX;
        //    // springt an Anfang der Datei
        //    fs.Position = 0;
        //    sr.DiscardBufferedData();
        //    //sr.BaseStream.Seek(0, SeekOrigin.Begin);
        //    // Zähler kann von oben verwendet werden, wird auf 0 zurück gesetzt
        //    Counter = 0;
        //    while (sr.Peek() != -1)
        //    {
        //        // nächste Zeile in zeile speichern
        //        zeile = sr.ReadLine();
        //        // Leerzeichen und Tabs entfernen
        //        zeile = zeile.Trim('\t', ' ');
        //        // Zeile zerlegen
        //        string[] Parts = zeile.Split('.');
        //        string[] Neighbours = Parts[5].Split(';');
        //        string[] tempNeighbouringCountries = new string[Neighbours.Length];
        //        for (int i = 0; i < Neighbours.Length; ++i)
        //        {
        //            int tempCountryID = Convert.ToInt32(Neighbours[i]);
        //            tempNeighbouringCountries[i] = Field.countries[tempCountryID - 1].name;
        //        }
        //        Field.countries[Counter].neighbouringCountries = tempNeighbouringCountries;
        //        Counter++;
        //    }
        /// <summary>
        /// Zählt Einträge zwischen Start und End, ab StartIndex
        /// </summary>
        /// <param name="Txt"></param>
        /// <returns></returns>
        //internal int LoadNumberOfEntriesFromStrings(string[] Txt, int StartingIndex)
        //{
        //    int EntryCounter = 0;
        //    bool StartetCounting = false;
        //    for (int i = StartingIndex; i < Txt.Length; ++i)
        //    {
        //        if (Txt[i][0] == '/' & Txt[i][0] == '/')
        //            continue;
        //        else if (Txt[i].StartsWith("Start"))
        //            StartetCounting = true;
        //        else if (Txt[i].StartsWith("End"))
        //            StartetCounting = false;
        //        if (StartetCounting)
        //            EntryCounter++;
        //    }

        //    return EntryCounter;
        //}
        //    // Ende,  streamreader schließen, kappt Verbindung zur Txt-Datei
        //    sr.Close();
        //}


    }
}
