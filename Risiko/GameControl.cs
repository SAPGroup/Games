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
        /// Momentan eigentlich kein unterschied zwischen 0 und 1
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
        /// <summary>
        /// Array der die Anzahl der Einheiten die der Spieler setzen möchte speichert
        /// in die Länder in seinem Besitz
        /// Array.Length = field.countries.length -> unkomplizierter als spezifische ArrayLänge
        /// </summary>
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

        //ContinentTable
        /// <summary>
        /// Speichert die eigentlichen Farben der KontinentLänder
        /// </summary>
        internal Color[] tempMouseOverContinentLabelsColor;
        /// <summary>
        /// Speichert ob Labels bereits erzeugt worden sind
        /// </summary>
        internal bool ContinentLabelsCreated = false;
        public bool continentLabelsCreated
        {
            get { return ContinentLabelsCreated; }
            set { ContinentLabelsCreated = value; }
        }


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
            set { AutoLanderkennung = value; }
        }

        // Konstruktor
        public GameControl(RisikoMain MainIn)
        {
            Main = MainIn;
        }
        public GameControl() { }


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
            //Kontinent-Tabelle lesen
            LoadContinentsTableFromTxtSource();

            int[] WidthHeight = Main.GetMapData();
            CheckFactor(WidthHeight[0], WidthHeight[1]);

            // lässt Karte zeichnen
            Main.DrawMap();
            // Lässt KontinentenTabelle erzeugen und direkt anpassen
            ResizeContinentLabels();

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
            if (tempOldFactor != Factor)
            {
                Main.DrawMap();
                ResizeContinentLabels();
            }
        }
        /// <summary>
        /// Zeichnet Land vollständig, mit MiddleUnits
        /// diese Methode immer aufrufen, modularer
        /// </summary>
        /// <param name="Index"></param>
        internal void DrawCountryFull(int Index)
        {
            Main.DrawSingleCountry(Index);
            Main.DrawMiddleUnits(Index);
            //Main.DrawCorners(Index);
        }
        /// <summary>
        /// malt Land mit CountryIn Index markiert
        /// </summary>
        /// <param name="IndexIn"></param>
        internal void DrawCountryFullSelected(int IndexIn)
        {
            Main.DrawSingleCountryMarked(IndexIn);
            Main.DrawMiddleUnits(IndexIn);
        }
        /// <summary>
        /// zeichnet gesamte Karte mithilfe von DrawCountryFull
        /// </summary>
        internal void DrawFullMap()
        {
            for (int i = 0; i < Field.countries.Length; ++i)
            {
                DrawCountryFull(i);
            }
        }
        /// <summary>
        /// Markiert ganzen Kontinent, mit Index ParameterIn
        /// Speichert die eigentlichen Farben in tempMouseOverContinentLabelsColor
        /// </summary>
        /// <param name="ContinentIndexIn"></param>
        public void MarkContinent(int ContinentIndexIn)
        {
            tempMouseOverContinentLabelsColor = new Color[Field.continents[ContinentIndexIn].numberOfCountries];

            for (int i = 0; i < Field.countries.Length; ++i)
            {
                if (Field.countries[i].continent == ContinentIndexIn)
                {
                    DrawCountryFullSelected(i);
                }
            }
        }
        /// <summary>
        /// Lädt die eigentlichen Farben der Länder eines 
        /// Kontinents ContinentIndexIn und lässt Länder
        /// zeichnen
        /// </summary>
        /// <param name="ContinentIndexIn"></param>
        public void UnmarkContinent(int ContinentIndexIn)
        {
            for (int i = 0; i < Field.countries.Length; ++i)
            {
                if (Field.countries[i].continent == ContinentIndexIn)
                {
                    DrawCountryFull(i);
                }
            }
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
        /// <summary>
        /// Liest die StartPosition des KontinentenTabels aus TxtSource
        /// </summary>
        internal void LoadContinentsTableFromTxtSource()
        {
            if (TxtSource != null)
            {
                //StartIndex der Länder und Länge
                int StartIndex = 0, Length = 0;

                // Eckpunkte auslesen
                SearchForSpecialPartFromTxtSource(ref StartIndex, ref Length, TxtSource, "ContinentTable");
                Point tempStartPosition = new Point();
                string zeile = TxtSource[StartIndex];
                //Tabs und Leerzeichen entfernen
                zeile = zeile.Trim('\t', ' ');
                // Zeile zerlegen
                string[] Parts = zeile.Split('.');

                // Eckpunkte des Landes auslesen, -> String wieder zerlegen
                string[] Corners = Parts[0].Split(';');

                tempStartPosition.X = Convert.ToInt32(Corners[0]);
                tempStartPosition.Y = Convert.ToInt32(Corners[1]);

                Field.firstContLabelPosition = tempStartPosition;
            }
        }
        /// <summary>
        /// Erzeugt die KontinentLabels
        /// </summary>
        internal void ResizeContinentLabels()
        {
            if (Main.lblContinents == null)
                Main.CreateContinentLabels();
            for (int i = 0; i < Field.continents.Length; ++i)
            {
                Main.lblContinents[i].Font = new Font(Main.lblContinents[i].Font.FontFamily, Factor);
                Main.lblContinents[i].Location = new Point(Field.firstContLabelPosition.X * Factor,
                                                            Field.firstContLabelPosition.Y * Factor + i * Factor * 2);
                Main.lblContinents[i].Size = new Size(Factor * 13, Factor * 3);

                //Main.lblContinents[i].BringToFront();
            }
        }

        // Gamestate, Kontrolle
        /// <summary>
        /// Geht über zu nächstem GameState, falls Ende eines Zuges
        /// -> nächster Spieler
        /// </summary>
        public void nextGameState()
        {
            // "normale" Gamestates
            if (GameState == 1)
            {
                GameState = 2;
                Main.NegateVisibilityPB();
                StartUnitAdding = false;
                DrawFullMap();
            }
            else if (GameState == 2)
            {
                GameState = 3;
                DrawFullMap();
            }
            else if (GameState == 3)
            {
                GameState = 1;
                Main.NegateVisibilityPB();
                StartUnitAdding = true;
                DrawFullMap();
                
            }
            else if (GameState == 0)
            {
                int PlayerIndex = ActualPlayerIndex();
                // Letzter Spieler hat gesetzt
                if (PlayerIndex == Players.Length-1)
                {
                    GameState = 1;
                    ActualPlayer = Players[0];
                    ActualPlayer.unitsPT = GetAddUnitsAtBeginningForActualPlayer();
                }
                // Noch ein Spieler muss setzen
                else
                {
                    ActualPlayer = Players[++PlayerIndex];
                }
                Main.SetPBColor(ActualPlayer.playerColor);
                Main.ActualizePB();
                DrawFullMap();
            }


            // Old
            //if (++GameState > 3)
            //{
            //    GameState = 1;
            //    int actualPlayer = ActualPlayerIndex() + 1;
            //    if (actualPlayer >= Players.Length)
            //        actualPlayer = 0;
            //    ActualPlayer = Players[actualPlayer];
            //}
            //if (GameState == 1)
            //    startUnitAdding = true;
            //else
            //    startUnitAdding = false;
        }

        public void MoveAttackSetEnd()
        {
            // "normale" Gamestates
            if (GameState == 1 | GameState == 0)
            {
                // Keine Einheiten mehr zu setzen
                if (ActualPlayer.unitsPT == 0)
                {
                    // Einheiten in Länder "verschieben"
                    for (int i = 0; i < Field.countries.Length; ++i)
                    {
                        if (UnitsToAdd[i] != 0)
                        {
                            Field.countries[i].unitsStationed += UnitsToAdd[i];
                            // Direkt danach mit 0 belegen für nächsten Spieler
                            UnitsToAdd[i] = 0;
                        }    
                    }
                    // Nächster Spieler
                    nextGameState();
                }
                else
                {
                    // Fehlermeldung
                    Main.ShowMessage("Sie haben noch Einheiten zu setzen.");
                }
            }
            else if (GameState == 2)
            {

            }
            else if (GameState == 3)
            {

            }
        }

        // Neues Spiel
        /// <summary>
        /// Startet neues Spiel
        /// </summary>
        /// <param name="Names"></param>
        /// <param name="Colors"></param>
        /// <param name="AI"></param>
        public void StartNewGame(string[] Names, Color[] Colors, bool[] AI)
        {
            // Karte Zeichnen und dabei Laden
            DrawAndLoadMap();
            // prüfen ob alle arrays gleich lang sind
            if (Names.Length != Colors.Length)
                return;
            // Players-Array erzeugen
            Players = new Player[Names.Length];
            //einzelne Player erzeugen
            for (int i = 0; i < Names.Length; ++i)
            {
                Players[i] = new Player(Names[i], AI[i], Colors[i]);
            }
            // Länder verteilen
            SpreadCountriesToPlayers();
            // Karte mit neuen Länderfarben zeichnen
            DrawFullMap();
            // Gamestate festlegen
            GameState = 0;
            // UnitsToAdd-Array erstellen
            UnitsToAdd = new int[Field.countries.Length];
            // Zeigt an das das Einheiten-Setzen begonnen hat
            StartUnitAdding = true;
            // ActualPlayer festlegen
            ActualPlayer = Players[0];

            for (int i = 0;i <Players.Length;++i)
            {
                Players[i].unitsPT = 20;
            }

            Main.pBUnits.Maximum = actualPlayer.unitsPT;
            Main.pBUnits.Value = actualPlayer.unitsPT;
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
                if (temp == -1 & tempMovedIndex != -1)
                {
                    //kein Treffer
                    //Field.countries[tempMovedIndex].colorOfCountry = tempMovedCountryColor;

                    DrawCountryFull(tempMovedIndex);
                    tempMovedIndex = -1;
                }
                // Bei Treffer
                else if (temp != -1 & temp != tempMovedIndex)
                {
                    //zuvor Bereits Land ausgewählt                
                    if (tempMovedIndex != -1)
                    {
                        //Field.countries[tempMovedIndex].colorOfCountry = tempMovedCountryColor;
                        DrawCountryFull(tempMovedIndex);
                    }

                    //tempMovedCountryColor = Field.countries[temp].colorOfCountry;
                    //Field.countries[temp].colorOfCountry = ColorCountrySelected;

                    tempMovedIndex = temp;

                    //DrawCountryFull(temp);
                    DrawCountryFullSelected(temp);
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
            // Letzte Fehlermeldung löschen
            Main.DeleteMessage();
            if (GameState == 1 | GameState == 0)
                MouseClickedAddUnit(tempCountry, e);
            else if (GameState == 2)
                MouseClickedAttackMode(tempCountry, e);
            else if (GameState == 3)
                ;// TODO: methode fürs ziehen

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
        /// Abfrage nach Buttons usw wird auch hier getätigt
        /// </summary>
        /// <param name="tempCountryIn"></param>
        public void MouseClickedAttackMode(int tempCountryIn, MouseEventArgs e)
        {
            // Linksklick in Land
            if (tempCountryIn != -1 & e.Button == MouseButtons.Left)
            {
                if (tempClickedFstIndex == -1)
                {
                    //eigenes Land
                    if (actualPlayer == Field.countries[tempCountryIn].owner)
                    {
                        // Index und Farbe abspeichern
                        tempClickedFstCountryColor = Field.countries[tempCountryIn].colorOfCountry;
                        tempClickedFstIndex = tempCountryIn;

                        // Gelb machen
                        Field.countries[tempClickedFstIndex].colorOfCountry = ColorCountrySelected;
                        // zeichnen
                        DrawCountryFull(tempClickedFstIndex);
                    }
                    // Nicht eigenes Land
                    else
                    {
                        Main.ShowMessage("Wählen Sie eines Ihrer eigenen Länder.");
                    }
                }
                // Zuvor schon Land angeklickt
                else if (tempClickedFstIndex != -1)
                {
                    if (tempClickedScndIndex != -1 & tempClickedFstIndex != -1)
                    {
                        Main.ShowMessage("Sie können nicht mehr als 2 Länder auswählen.");
                        return;
                    }
                    // gegnerisches Land
                    if (actualPlayer != Field.countries[tempCountryIn].owner)
                    {
                        // gegnerisches Nachbarland
                        if (Field.CountriesAreNeighbours(tempClickedFstIndex, tempCountryIn))
                        {
                            //Index und Farbe abspeichern
                            tempClickedScndIndex = tempCountryIn;
                            tempClickedScndCountryColor = Field.countries[tempCountryIn].colorOfCountry;

                            // Gelb machen
                            Field.countries[tempClickedScndIndex].colorOfCountry = ColorCountrySelected;
                            // zeichnen
                            DrawCountryFull(tempClickedScndIndex);

                            // TODO: eigentlicher Attack
                        }
                        // zwar gegnerisches Land, jedoch kein Nachbar
                        else
                        {
                            Main.ShowMessage("Wählen Sie ein benachbartes gegnerisches Land.");
                        }
                    }
                    // eigenes Land
                    else
                    {
                        Main.ShowMessage("Wählen Sie ein gegnerisches Land.");
                    }
                }
            }
            // Rechtsklick
            else if (e.Button == MouseButtons.Right)
            {
                // Fst und Scnd werden resettet
                int scnd = tempClickedScndIndex;
                int fst = tempClickedFstIndex;
                ResetClickedCountries(3);
                DrawCountryFull(scnd);
                DrawCountryFull(fst);
            }
            // Linksklick außerhalb des Landes
            else if (tempCountryIn == -1 & e.Button == MouseButtons.Left)
            {
                if (tempClickedScndIndex != -1)
                {
                    int tempIndex = tempClickedScndIndex;
                    ResetClickedCountries(2);
                    DrawCountryFull(tempIndex);
                }
                else if (tempClickedScndIndex == -1 & tempClickedFstIndex != -1)
                {
                    int tempIndex = tempClickedFstIndex;
                    ResetClickedCountries(1);
                    DrawCountryFull(tempIndex);
                }
            }
        }

        /// <summary>
        /// Wird bei Klick auf pnlMap im Gamestate 1 oder 0 aufgerufen
        /// </summary>
        /// <param name="tempCountryIn"></param>
        /// <param name="e"></param>
        public void MouseClickedAddUnit(int tempCountryIn, MouseEventArgs e)
        {
            // Linksklick in Land
            if (tempCountryIn != -1 & e.Button == MouseButtons.Left)
            {
                // Spieler ist Besitzer des Landes
                if (Field.countries[tempCountryIn].owner == ActualPlayer)
                {
                    if (ActualPlayer.unitsPT > 0)
                    {
                        UnitsToAdd[tempCountryIn]++;
                        ActualPlayer.unitsPT--;
                        DrawCountryFullSelected(tempCountryIn);
                        Main.ActualizePB();
                    }
                    else
                    {
                        Main.ShowMessage("Sie haben all ihre Einheiten bereits gesetzt.");
                    }
                }
                // nicht eigenes Land
                else
                {
                    Main.ShowMessage("Wählen Sie eines ihrer eigenen Länder.");
                }
            }
            else if (tempCountryIn != -1 & e.Button == MouseButtons.Right)
            {
                // Spieler ist Besitzer des Landes
                if (Field.countries[tempCountryIn].owner == ActualPlayer)
                {
                    if (UnitsToAdd[tempCountryIn] > 0)
                    {
                        UnitsToAdd[tempCountryIn]--;
                        ActualPlayer.unitsPT++;
                        DrawCountryFullSelected(tempCountryIn);
                        Main.ActualizePB();
                    }
                    else
                    {
                        Main.ShowMessage("Sie haben in diesem Land keine Einheiten mehr hinzugefügt.");
                    }
                }
                // nicht eigenes Land
                else
                {
                    Main.ShowMessage("Wählen Sie eines ihrer eigenen Länder.");
                }
            }
        }




        // Sonstiges
        private int GetAddUnitsAtBeginningForActualPlayer()
        {
            return ((int)(ActualPlayer.ownedCountries.Length / 3) + GetAddUnitsFromContsForActualPlayer());
        }

        /// <summary>
        /// Liefert Index des aktuellen Spielers in Players-Array
        /// </summary>
        /// <returns></returns>
        internal int ActualPlayerIndex()
        {
            for (int i = 0; i < Players.Length; ++i)
            {
                if (Players[i].name == ActualPlayer.name)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Verteilt die Länder an die Spieler, zufallsgeneriert
        /// TODO: Option Länder selbst auswählen, abwechselnd Einheit in verfügbares Land setzen
        /// </summary>
        public void SpreadCountriesToPlayers()
        {
            Random rnd = new Random();
            // Speichert temporär die Anzahl der Länder die der Spieler
            // bereits besitzt
            int[] CounterOfCountries = new int[players.Length];

            // Die Anzahl der Länder die jeder Spieler mindestens bekommt
            int CountriesEachPlayer = Field.countries.Length / players.Length;

            // Anzahl der "mindest" länder setzen
            for (int i = 0; i < players.Length; ++i)
                CounterOfCountries[i] = CountriesEachPlayer;

            // Die Anzahl der Länder die "zu viel" sind, die also
            // Spielern zusätzlich zugeteilt werden
            // TODO: "Echte" Spieler vlt bevorzugen (nicht KI)
            int CountriesLeft = Field.countries.Length - (CountriesEachPlayer * players.Length);

            // Zufallsvariable
            int tempRnd;

            // Gibt zufällig manchen Spielern mehr Länder (die die zu viel waren)
            while (CountriesLeft > 0)
            {
                tempRnd = (int)rnd.NextDouble() * players.Length;
                if (CounterOfCountries[tempRnd] == CountriesEachPlayer)
                {
                    CounterOfCountries[tempRnd]++;
                    CountriesLeft--;
                }
            }

            // Gibt den Spielern die Länder
            for (int i = 0; i < Field.countries.Length; i++)
            {
                tempRnd = (int)(rnd.NextDouble() * players.Length);
                if (CounterOfCountries[tempRnd] > 0)
                {
                    // Country- Besitzer festlegen
                    Field.countries[i].owner = Players[tempRnd];
                    // Anzahl der Länder für neuen Besitzer die noch zu vergeben sind verringern
                    CounterOfCountries[tempRnd]--;
                    // 1 Einheit in Land setzen
                    Field.countries[i].unitsStationed = 1;
                    // 1 Einheit bei Spieler abziehen
                    Players[tempRnd].unitsPT--;
                }
                else
                    --i;
                // damit land sicher vergeben wird
            }

            // Farbe des Spielers in Land übernehmen
            for (int i = 0; i < Field.countries.Length; ++i)
            {
                Field.countries[i].colorOfCountry = Field.countries[i].owner.playerColor;
            }

            // Besitz der Länder in ownedCountries der Spieler speichern, (2seitige Beziehung) TODO: unnötig?
            for (int i = 0; i < Field.countries.Length; ++i)
            {
                players[GetPlayerIndex(Field.countries[i].owner.name)].AddOwnedCountry(Field.countries[i]);
            }
        }

        /// <summary>
        /// Liefert Index aus Players[] des Spielers mit NameIn zurück
        /// </summary>
        /// <param name="NameIn"></param>
        /// <returns></returns>
        private int GetPlayerIndex(string NameIn)
        {
            for (int i = 0; i < Players.Length; ++i)
            {
                if (Players[i].name == NameIn)
                    return i;
            }
            // error
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
            string[] OutBuf = new string[StringArray.Length + 1];
            for (int i = 0; i < StringArray.Length; ++i)
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
        public int GetAddUnitsFromContsForActualPlayer()
        {
            int OutBuff = 0;

            int[] CountriesOfContsOfPlayer = new int[Field.continents.Length];
            for (int i = 0; i < CountriesOfContsOfPlayer.Length; ++i)
            {
                CountriesOfContsOfPlayer[i] = 0;
            }

            for (int i = 0; i < ActualPlayer.ownedCountries.Length; ++i)
            {
                CountriesOfContsOfPlayer[ActualPlayer.ownedCountries[i].continent]++;
            }

            for (int i = 0; i < Field.continents.Length; ++i)
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
        ///// <summary>
        ///// Zählt Einträge zwischen Start und End, ab StartIndex
        ///// </summary>
        ///// <param name="Txt"></param>
        ///// <returns></returns>
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
        // Alter Teil von MouseClicked
        // OLD
        // Wenn Klick in Land und Linke Maustaste
        //if (tempCountry != -1 & e.Button == MouseButtons.Left)
        //{
        //    if (GameState == 2)
        //        MouseClickedAttackMode(tempCountry, e);
        //    else if (GameState == 3)
        //        ;// 
        //    else if (GameState == 0 | GameState == 1)
        //        MouseClickedAddUnit(tempCountry);
        //}
        //else if (e.Button == MouseButtons.Right)
        //{
        //    if (GameState == 2 | GameState == 3)
        //    {
        //        // Fst und Scnd werden resettet
        //        int scnd = tempClickedScndIndex;
        //        int fst = tempClickedFstIndex;
        //        ResetClickedCountries(3);
        //        DrawCountryFull(scnd);
        //        DrawCountryFull(fst);
        //    }
        //}
        //else if (tempCountry == -1 & e.Button == MouseButtons.Left)
        //{
        //    if (GameState == 2)
        //    {
        //        if (tempClickedScndIndex != -1)
        //        {
        //            int tempIndex = tempClickedScndIndex;
        //            ResetClickedCountries(2);
        //            DrawCountryFull(tempIndex);
        //        }
        //        else if (tempClickedScndIndex == -1 & tempClickedFstIndex != -1)
        //        {
        //            int tempIndex = tempClickedFstIndex;
        //            ResetClickedCountries(1);
        //            DrawCountryFull(tempIndex);
        //        }
        //    }
        //}
    }
}
