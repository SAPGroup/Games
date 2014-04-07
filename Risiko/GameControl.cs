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
        /// Speichert die OriginalAnzahl der Angreifer ab
        /// somit kann Abfrage erfolgen ob Prozentzahl tod ist
        /// </summary>
        internal int OrgAttackers = 0;
        /// <summary>
        /// Wert ob in dieser Runde bereits angegriffen wurde
        /// </summary>
        internal bool ActualPlayerHasDoneAnything = false;

        /// <summary>
        /// Speichert die Länder in den Einzelnen Nachbarschaften ab
        /// erste Dimension: Nachbarschaften
        /// zweit: Indizes der Länder (Index aus OwnedCountries des ActualPlayers)
        /// </summary>
        internal int[][] NeighbouringCountriesFromActualPlayer;
        internal int[] AddUnitsToNeighbourhood;


        /// <summary>
        /// Zum Ein-Auslesen aus 
        /// </summary>
        internal FileStream fs;
        internal StreamReader sr;
        internal StreamWriter sw;

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
        internal Color ColorCountrySelected = Color.Yellow;

        //Moved
        // temporärer Index des zuletzt überfahrenen Landes
        internal int tempMovedIndex = -1;

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

        // Gamestate, Kontrolle, Wichtige Methoden
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
                ActualPlayerHasDoneAnything = false;
                DrawFullMap();
            }
            else if (GameState == 2)
            {
                GameState = 3;
                OrgAttackers = 0;
                ActualPlayerHasDoneAnything = false;
                GetNeighboursArray();
                DrawFullMap();
            }
            else if (GameState == 3)
            {
                // Neuen ActualPlayer berechnen
                int PlayerIndex = ActualPlayerIndex();
                if (PlayerIndex == Players.Length - 1)
                    ActualPlayer = Players[0];
                else
                    ActualPlayer = Players[PlayerIndex + 1];

                // Einheiten berechnen
                ActualPlayer.unitsPT = GetAddUnitsAtBeginningForActualPlayer();
                // UnitAdding aktivieren
                StartUnitAdding = true;

                // neuer Gamestate
                GameState = 1;
                // PB sichtbar machen
                Main.NegateVisibilityPB();
                // PB wert aktualisieren
                Main.ActualizePBmax();
                Main.ActualizePB();
                // PB farbe auf Spielerfarbe setzen
                Main.SetPBColor(ActualPlayer.playerColor);


                //eigentlich nicht benötigt, Karte neu zeichnen
                //DrawFullMap();
            }
            else if (GameState == 0)
            {
                int PlayerIndex = ActualPlayerIndex();
                // Letzter Spieler hat gesetzt
                if (PlayerIndex == Players.Length - 1)
                {
                    GameState = 1;
                    ActualPlayer = Players[0];
                    ActualPlayer.unitsPT = GetAddUnitsAtBeginningForActualPlayer();
                    Main.ActualizePBmax();
                    Main.ActualizePB();
                    Main.SetPBColor(ActualPlayer.playerColor);
                }
                // Noch ein Spieler muss setzen
                else
                {
                    ActualPlayer = Players[++PlayerIndex];
                }
                Main.SetPBColor(ActualPlayer.playerColor);
                Main.ActualizePBmax();
                Main.ActualizePB();
                DrawFullMap();
            }
        }
        /// <summary>
        /// Methode die Aufgerufen wird, wenn der wichtigste Button
        /// MoveEndSet geklickt wird
        /// </summary>
        public void ButtonMoveAttackSetEnd()
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
            // Gamestate 2
            else if (GameState == 2)
            {
                if (tempClickedScndIndex == -1 && tempClickedFstIndex == -1)
                {
                    if (!ActualPlayerHasDoneAnything)
                        nextGameState();
                    else
                    {
                        Main.ShowMessage("Wollen Sie den Zug wirklich ohne Angriff beenden?");
                        ActualPlayerHasDoneAnything = true;
                    }
                }
                // nur ein ausgewähltes Land
                else if (tempClickedFstIndex == -1 & tempClickedScndIndex != -1 |
                            tempClickedFstIndex != -1 & tempClickedScndIndex == -1)
                {
                    Main.ShowMessage("Sie haben noch kein zweites Land ausgewählt.");
                }
                else if (tempClickedFstIndex != -1 & tempClickedScndIndex != -1)
                {
                    Attack();
                }
            }
            // GameState 3
            else if (GameState == 3)
            {
                if (CheckUnitsToSetLeft())
                {
                    Main.ShowMessage("Sie haben noch Einheiten zu setzen.");
                }
                else
                {
                    nextGameState();
                }
            }
        }
        /// <summary>
        /// Steuert Attacke
        /// Attackiert von FstIndex nach ScndIndex
        /// mit Anzahl der Einheiten die in Einstellungen
        /// von ActualPlayer angegeben sind
        /// </summary>
        public void Attack()
        {
            if (ActualPlayer.settingAttack == -1)
            {
                // OrgAttackers noch nicht gesetzt
                if (Field.countries[tempClickedFstIndex].unitsStationed >= 2)
                {
                    OrgAttackers = Field.countries[tempClickedFstIndex].unitsStationed - 1;
                }
                else
                {
                    Main.ShowMessage("Zu Wenig Einheiten um anzugreifen.");
                    return;
                }
                int Attackers = OrgAttackers;

                // eigentlicher Angriff
                if (ActualPlayer.settingEndAttackLossPercentage != 0)
                {
                    while (CheckLosses() & Attackers > 0)
                    {
                        int ActualUnits = Field.countries[tempClickedFstIndex].unitsStationed;
                        if (Attackers >= 3)
                            DoActualAttack(3);
                        else
                            DoActualAttack(Attackers);
                        Attackers -= (ActualUnits - Field.countries[tempClickedFstIndex].unitsStationed);
                        if (CheckOwnerShipNew(Attackers))
                            break;
                    }
                    Main.ShowMessage("Der Angriff wurde wegen Verlusten abgebrochen.");
                    ResetClickedCountries(3);
                }
                else
                {
                    while (Attackers > 0)
                    {
                        int ActualUnits = Field.countries[tempClickedFstIndex].unitsStationed;
                        if (Attackers >= 3)
                            DoActualAttack(3);
                        else
                            DoActualAttack(Attackers);
                        Attackers -= (ActualUnits - Field.countries[tempClickedFstIndex].unitsStationed);
                        if (CheckOwnerShipNew(Attackers))
                            break;
                    }
                }
            }
            // 3er-AttackModus
            else if (ActualPlayer.settingAttack == 0)
            {
                int Attackers;
                if (Field.countries[tempClickedFstIndex].unitsStationed - 1 > 3)
                    Attackers = 3;
                else
                    Attackers = Field.countries[tempClickedFstIndex].unitsStationed - 1;

                DoActualAttack(Attackers);
                if (!CheckOwnerShipNew(Attackers))
                {
                    DrawCountryFull(tempClickedFstIndex);
                    DrawCountryFull(tempClickedScndIndex);
                }

                //DrawCountryFull(tempClickedFstIndex);
                //DrawCountryFull(tempClickedScndIndex);
            }
            // Custom Unit-Mode
            else if (ActualPlayer.settingAttack > 0)
            {
                int Attackers = ActualPlayer.settingAttack;
                if (Field.countries[tempClickedFstIndex].unitsStationed - 1 < Attackers)
                    Attackers = Field.countries[tempClickedFstIndex].unitsStationed - 1;
                OrgAttackers = Attackers;

                while (CheckLosses() & Attackers > 0)
                {
                    int ActualUnits = Field.countries[tempClickedFstIndex].unitsStationed;
                    if (Attackers >= 3)
                        DoActualAttack(3);
                    else
                        DoActualAttack(Attackers);
                    Attackers -= (ActualUnits - Field.countries[tempClickedFstIndex].unitsStationed);
                    if (CheckOwnerShipNew(Attackers))
                        break;
                }
            }
        }

        // Anfang des Spiels
        /// <summary>
        /// Startet neues Spiel
        /// </summary>
        /// <param name="Names"></param>
        /// <param name="Colors"></param>
        /// <param name="AI"></param>
        public void StartNewGame(string[] Names, Color[] Colors, bool[] AI)
        {
            // Karte Laden
            //DrawAndLoadMap();
            LoadAllFromTxtSource();
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
            DrawMap();
            // Gamestate festlegen
            GameState = 0;
            // UnitsToAdd-Array erstellen
            UnitsToAdd = new int[Field.countries.Length];
            // Zeigt an dass das Einheiten-Setzen begonnen hat
            StartUnitAdding = true;
            // ActualPlayer festlegen
            ActualPlayer = Players[0];

            for (int i = 0; i < Players.Length; ++i)
            {
                Players[i].unitsPT = 3;
            }
            ActualPlayer.ownedCountries[0].unitsStationed = 1;

            Main.pBUnits.Maximum = actualPlayer.unitsPT;
            Main.pBUnits.Value = actualPlayer.unitsPT;
            DrawFullMap();
        }
        public void LoadGame()
        {
            // Lädt alles aus TxtSource
            LoadAllFromTxtSource();
            // Spieler Laden
            LoadPlayersFromSavegameTxtSource();
            // Länder
            LoadCountriesFromTxtSource();
            //Kontinente aus QuellStringArray lesen
            LoadContinentsFromTxtSource();
            //Kontinent-Tabelle lesen
            LoadContinentsTableFromTxtSource();

            // ein Gamestate weiter machen
            nextGameState();
            if (GameState == 3 | GameState == 2)
                Main.pBUnits.Visible = false;
            // Karte zeichnen
            DrawMap();
            DrawFullMap();
        }






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
            // bei veränderung neu zeichnen
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

        // Quell-TxtDatei
        /// <summary>
        /// Lädt "alles" aus Txt Source
        /// keine Weiterverarbeitung
        /// </summary>
        public void LoadAllFromTxtSource()
        {
            // Quelldatei auslesen
            TxtSource = LoadStringsFromTxtSource();
            //Länder aus QuellStringArray lesen
            LoadCountriesFromTxtSource();
            //Kontinente aus QuellStringArray lesen
            LoadContinentsFromTxtSource();
            //Kontinent-Tabelle lesen
            LoadContinentsTableFromTxtSource();
        }
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
                    tempName = Parts[1].Trim('\t');

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
                    if(Convert.ToInt32(Parts[2]) == -1 | Players == null)
                        Field.countries[j] = new Country(tempName, tempPoints, tempColor);
                    else
                    {
                        int tempUnits = Convert.ToInt32(Parts[3]);
                        Field.countries[j] = new Country(tempName, tempPoints, tempColor, Players[Convert.ToInt32(Parts[2])], tempUnits);
                        Players[Convert.ToInt32(Parts[2])].AddOwnedCountry(Field.countries[j]);
                    }
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
                    Country[] tempNeighbouringCountries = new Country[Neighbours.Length];
                    for (int i = 0; i < Neighbours.Length; ++i)
                    {
                        int tempCountryID = Convert.ToInt32(Neighbours[i]);
                        tempNeighbouringCountries[i] = Field.countries[tempCountryID - 1];
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
        /// Lädt Spieler, ActualPlayer und Gamestate aus TxtSource
        /// </summary>
        internal void LoadPlayersFromSavegameTxtSource()
        {
            if (TxtSource != null)
            {
                //StartIndex der Länder und Länge
                int StartIndex = 0, Length = 0;
                SearchForSpecialPartFromTxtSource(ref StartIndex, ref Length, TxtSource, "Players");

                Players = new Player[Length];

                string tempName;
                Color tempColor;
                bool tempIsAI;
                int ActualPlayerIndex = 0;
                int ActualGamestate = 0;

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

                    tempName = Parts[0];
                    tempColor = GetColorFromString(Parts[1]);
                    if (Parts[3] != "1")
                        tempIsAI = false;
                    else
                        tempIsAI = true;
                    if (Convert.ToInt32(Parts[2]) != 0)
                    {
                        ActualPlayerIndex = j;
                        ActualGamestate = Convert.ToInt32(Parts[2]) -1;
                    }
                    Players[j] = new Player(tempName, tempIsAI, tempColor);
                }
                ActualPlayer = Players[ActualPlayerIndex];
                if (ActualGamestate < 0)
                    ActualGamestate += 4;
                GameState = ActualGamestate;
            }
        }

        //Speichern
        /// <summary>
        /// Veranlasst Speichervorgang zu starten
        /// </summary>
        internal void SaveGame()
        {
            SaveFileDialog sfd = new SaveFileDialog();

            //sfd.InitialDirectory =;
            sfd.Filter = " Texte (*.txt)|*.txt|" + " Alle Dateien (*.*)|*.*";
            sfd.Title = "Datei zum Speichern auswählen";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                WriteSavegame(sfd.FileName);
            }
            else
            {
                MessageBox.Show("Es ist ein Fehler aufgetreten.");
            }
        }
        /// <summary>
        /// Schreibt "Spielstand" in SaveLocation 
        /// </summary>
        /// <param name="SaveLocation"></param>
        internal void WriteSavegame(string SaveLocation)
        {
            // Erzeigt FileStream
            fs = new FileStream(SaveLocation, FileMode.Create);
            sw = new StreamWriter(fs);

            // Alle Länder, Kontinente, Spieler + StartEnd jeweils und ContinentTable
            int LengthOfOutBuff = Field.countries.Length + Field.continents.Length + Players.Length + 9;
            string[] ToWriteBuff = new string[LengthOfOutBuff];
            // Speichert Countries
            WriteCountriesToSaveGame(ref ToWriteBuff);
            // Speichert Continente
            WriteContinentsToSaveGame(ref ToWriteBuff);
            // Speichert KontinentenTabelle
            WriteContinentTableToSaveGame(ref ToWriteBuff);
            // speichert Spieler ab
            WritePlayersToSaveGame(ref ToWriteBuff);

            // Letztliches wirkliches abspeichern
            for (int i = 0;i < ToWriteBuff.Length;++i)
            {
                sw.WriteLine(ToWriteBuff[i]);
            }
            sw.Close();
        }
        /// <summary>
        /// Schreibt alle Länder in lesbarem Format in OutBuff
        /// </summary>
        /// <param name="OutBuff"></param>
        internal void WriteCountriesToSaveGame(ref string[] OutBuff)
        {
            OutBuff[0] = "Start Countries";
            int i;
            for (i = 0;i < Field.countries.Length;++i)
            {
                string tempOut = "";
                //Farbe
                tempOut += GetStringFromColor(Field.countries[i].colorOfCountry) + ".";
                // Name
                tempOut += Field.countries[i].name + ".";
                // Spieler
                for (int j = 0;j < Players.Length;++j)
                    if (Field.countries[i].owner.name == Players[j].name)
                        tempOut += Convert.ToString(j) + ".";
                
                // Einheiten
                tempOut += Convert.ToString(Field.countries[i].unitsStationed) + ".";
                // Corners
                for (int j = 0; j < Field.countries[i].corners.Length; ++j )
                {
                    tempOut += Convert.ToString(Field.countries[i].corners[j].X) + ";";
                    tempOut += Convert.ToString(Field.countries[i].corners[j].Y) + ";";
                }
                tempOut = tempOut.TrimEnd(';');
                tempOut += ".";

                // Neighbours
                for (int j = 0;j < Field.countries[i].neighbouringCountries.Length;++j)
                    for (int k = 0; k < Field.countries.Length; ++k)
                        if (Field.countries[k] == Field.countries[i].neighbouringCountries[j])
                            tempOut += Convert.ToString(k+1) + ";";
                tempOut = tempOut.TrimEnd(';');
                tempOut += ".";
                
                //Continents
                tempOut += Convert.ToString(Field.countries[i].continent+1);

                // festlegen
                OutBuff[i+1] = tempOut;
            }
            OutBuff[i+1] = "End Countries";
        }
        /// <summary>
        /// Speichert die Kontinente lesbar in SaveGame ab
        /// </summary>
        /// <param name="OutBuff"></param>
        internal void WriteContinentsToSaveGame(ref string[] OutBuff)
        {
            int StartingIndex = Field.countries.Length + 3;
            OutBuff[StartingIndex - 1] = "Start Continents";
            int i;
            for (i = 0;i < Field.continents.Length;++i)
            {
                string tempOut = "";
                // Name
                tempOut += Field.continents[i].nameOfContinent + ".";
                // additionalUnits
                tempOut += Convert.ToString(Field.continents[i].additionalUnits) + ".";
                // Number of Countries
                tempOut += Convert.ToString(Field.continents[i].numberOfCountries);

                OutBuff[StartingIndex + i] = tempOut;
            }
            OutBuff[i + StartingIndex] = "End Continents";
        }
        /// <summary>
        /// Speichert die KontinentenTabelle lesbar in SaveGame
        /// </summary>
        /// <param name="OutBuff"></param>
        internal void WriteContinentTableToSaveGame(ref string[] OutBuff)
        {
            int StartingIndex = Field.countries.Length + Field.continents.Length + 4;
            OutBuff[StartingIndex] = "Start ContinentTable";
            OutBuff[StartingIndex + 1] = Convert.ToString(Field.firstContLabelPosition.X) + ";";
            OutBuff[StartingIndex + 1] += Convert.ToString(Field.firstContLabelPosition.Y);
            OutBuff[StartingIndex + 2] = "End ContinentTable";
        }
        /// <summary>
        /// Speichert Spieler lesbar in SaveGame
        /// </summary>
        /// <param name="OutBuff"></param>
        internal void WritePlayersToSaveGame(ref string[] OutBuff)
        {
            int StartingIndex = Field.countries.Length + Field.continents.Length + 8;
            OutBuff[StartingIndex - 1] = "Start Players";
            int i;
            for (i = 0;i < Players.Length;++i)
            {
                string tempOut = "";
                // Name
                tempOut += Players[i].name + ".";
                // Farbe
                tempOut += GetStringFromColor(Players[i].playerColor) + ".";
                // ActualPlayer/Gamestate
                if (ActualPlayer == Players[i])
                    tempOut += Convert.ToString(GameState) + ".";
                else
                    tempOut += "-1" + ".";
                // isAi
                if (Players[i].AIPlayer)
                    tempOut += "1";
                else
                    tempOut += "0";

                OutBuff[StartingIndex + i] = tempOut;
            }
            OutBuff[StartingIndex + i] = "End Players";
        }


        //Maus-Aktionen
        /// <summary>
        /// MouseBewegung über Map
        /// </summary>
        /// <param name="e"></param>
        public void MouseMoved(MouseEventArgs e)
        {

            // ist Einstellung
            // Speichert in temp den Index des Landes ab, über dem sich der Mauszeiger befindet
            int temp = CheckClickInCountry(new Point(e.X, e.Y));
            if (autoLanderkennung)
            {
                // Kein Treffer
                if (temp == -1 & tempMovedIndex != -1)
                {
                    //kein Treffer
                    DrawCountryFull(tempMovedIndex);
                    tempMovedIndex = -1;
                }
                // Bei Treffer
                else if (temp != -1 & temp != tempMovedIndex)
                {
                    //zuvor Bereits Land ausgewählt                
                    if (tempMovedIndex != -1)
                    {
                        DrawCountryFull(tempMovedIndex);
                    }

                    tempMovedIndex = temp;
                    
                    DrawCountryFullSelected(temp);
                }
            }
            //if (GameState == 3 & temp != -1)
            //{
            //    int tempIndex = GetNumberIndexOutOf2DArray(NeighbouringCountriesFromActualPlayer, temp)[0];
            //    int[] tempIndizes = NeighbouringCountriesFromActualPlayer[tempIndex];
            //    for (int i = 0;i < tempIndizes.Length;++i)
            //    {
            //        string tempname = ActualPlayer.ownedCountries[tempIndizes[i]].name;
            //        for (int j = 0;j < Field.countries.Length;++j)
            //        {
            //            if (Field.countries[j].name == tempname)
            //            {
            //                DrawCountryFullSelected(j);
            //            }
            //        }
            //    }
            //    Main.ShowMessage(Convert.ToString(AddUnitsToNeighbourhood[tempIndex]));
            //}
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
                MouseClickedMoveMode(tempCountry, e);

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
                if (tempClickedFstIndex != -1)
                {
                    int fst = tempClickedFstIndex;
                    Field.countries[tempClickedFstIndex].colorOfCountry = tempClickedFstCountryColor;
                    DrawCountryFull(tempClickedFstIndex);
                    tempClickedFstIndex = -1;
                    DrawCountryFull(fst);
                }
                //Second resetten
                if (tempClickedScndIndex != -1)
                {
                    int scnd = tempClickedScndIndex;
                    Field.countries[tempClickedScndIndex].colorOfCountry = tempClickedScndCountryColor;
                    DrawCountryFull(tempClickedScndIndex);
                    tempClickedScndIndex = -1;
                    DrawCountryFull(scnd);
                }
            }
            else if (Clicked == 2)
            {
                //nur Second resetten
                if (tempClickedScndIndex != -1)
                {
                    int scnd = tempClickedScndIndex;
                    Field.countries[tempClickedScndIndex].colorOfCountry = tempClickedScndCountryColor;
                    DrawCountryFull(tempClickedScndIndex);
                    tempClickedScndIndex = -1;
                    DrawCountryFull(scnd);
                }
            }
            else if (Clicked == 1)
            {
                //nur First resetten
                if (tempClickedFstIndex != -1)
                {
                    int fst = tempClickedFstIndex;
                    Field.countries[tempClickedFstIndex].colorOfCountry = tempClickedFstCountryColor;
                    DrawCountryFull(tempClickedFstIndex);
                    tempClickedFstIndex = -1;
                    DrawCountryFull(fst);
                }
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
                ResetClickedCountries(3);
            }
            // Linksklick außerhalb des Landes
            else if (tempCountryIn == -1 & e.Button == MouseButtons.Left)
            {
                if (tempClickedScndIndex != -1)
                {
                    ResetClickedCountries(2);
                }
                else if (tempClickedScndIndex == -1 & tempClickedFstIndex != -1)
                {
                    ResetClickedCountries(1);
                }
            }
        }
        /// <summary>
        /// bei Gamestate = 3, BewegungsModus
        /// Wie beim setzen nur mit abziehen und nur 
        /// innerhalb der Nachbarschaft setzbar
        /// Abfrage nach Buttons usw wird auch hier getätigt
        /// </summary>
        /// <param name="tempCountryIn"></param>
        /// <param name="e"></param>
        public void MouseClickedMoveMode(int tempCountryIn, MouseEventArgs e)
        {
            if (tempCountryIn != -1)
            {
                if (Field.countries[tempCountryIn].owner == actualPlayer)
                {
                    // Wird in allen Verzweigungn potentiell benötigt, da von TempCountryIn abhängig
                    // nur wenn dieses != -1 ist
                    int ownedindex = GetOwnedIndexFromName(Field.countries[tempCountryIn].name);
                    int tempIndex = GetNumberIndexOutOf2DArray(NeighbouringCountriesFromActualPlayer, ownedindex)[0];
                    // Linksklick in Land
                    if (e.Button == MouseButtons.Left)
                    {
                        if (AddUnitsToNeighbourhood[tempIndex] > 0)
                        {
                            AddUnitsToNeighbourhood[tempIndex]--;
                            Field.countries[tempCountryIn].unitsStationed++;
                            DrawCountryFullSelected(tempCountryIn);
                        }
                        else
                        {
                            Main.ShowMessage("Sie haben keine Einheit mehr, die Sie in diesem Bereich setzen können.");
                        }
                    }
                    // Rechtsklick
                    else if (e.Button == MouseButtons.Right)
                    {
                        if (Field.countries[tempCountryIn].unitsStationed > 1)
                        {
                            Field.countries[tempCountryIn].unitsStationed--;
                            AddUnitsToNeighbourhood[tempIndex]++;
                            DrawCountryFullSelected(tempCountryIn);
                        }
                        else
                        {
                            Main.ShowMessage("Sie können kein Land ohne wenigstens eine Einheit zurücklassen.");
                        }
                    }
                }
                else
                {
                    Main.ShowMessage("Sie können nur in ihren eigenen Ländern Einheiten setzen.");
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


        // Attack-Methoden
        /// <summary>
        /// Checkt Angegriffenes Land
        /// Wenn keine Einheiten mehr vorhanden
        /// werden UnitsToMoveIfDefeated Einheiten in
        /// das Land gezogen und der neue Besitzer festgelegt
        /// </summary>
        /// <param name="UnitsToMoveIfDefeated"></param>
        /// <returns></returns>
        private bool CheckOwnerShipNew(int UnitsToMoveIfDefeated)
        {
            if (Field.countries[tempClickedScndIndex].unitsStationed == 0)
            {
                Field.countries[tempClickedScndIndex].owner = ActualPlayer;
                Field.countries[tempClickedScndIndex].unitsStationed = UnitsToMoveIfDefeated;
                Field.countries[tempClickedFstIndex].unitsStationed -= UnitsToMoveIfDefeated;
                tempClickedScndCountryColor = actualPlayer.playerColor;
                ActualPlayer.AddOwnedCountry(Field.countries[tempClickedScndIndex]);
                OrgAttackers = 0;
                ResetClickedCountries(3);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Gibt zurück Ob trotz Verlusten weiterattackiert werden soll
        /// true: weiter machen
        /// false: aufhören
        /// </summary>
        /// <returns></returns>
        private bool CheckLosses()
        {
            if (ActualPlayer.settingEndAttackLossPercentage != 0)
            {
                int LossesPossible = OrgAttackers * (ActualPlayer.settingEndAttackLossPercentage / 100);
                if (OrgAttackers - Field.countries[tempClickedFstIndex].unitsStationed < LossesPossible)
                    return true;
                else
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Führt eigentlichen Angriff aus
        /// Greift mit Attackers-Einheiten gegen Land an
        /// zw 1 u 3
        /// </summary>
        /// <param name="Attackers"></param>
        private void DoActualAttack(int Attackers)
        {
            int Defender;
            if (Field.countries[tempClickedScndIndex].unitsStationed >=
                Field.countries[tempClickedScndIndex].owner.numberOfDefenders)
                Defender = Field.countries[tempClickedScndIndex].owner.numberOfDefenders;
            else
                Defender = 1;

            // Arrays erzeugen und Sortieren
            int[] AttackerThrows = new int[Attackers];
            for (int i = 0; i < AttackerThrows.Length; ++i)
                AttackerThrows[i] = (int)(rnd.NextDouble() * 5) + 1;
            AttackerThrows = SortArray(AttackerThrows);

            int[] DefenderThrows = new int[Defender];
            for (int i = 0; i < DefenderThrows.Length; ++i)
                DefenderThrows[i] = (int)(rnd.NextDouble() * 5) + 1;
            if (DefenderThrows.Length == 2)
                DefenderThrows = SortArray(DefenderThrows);

            if (AttackerThrows.Length >= DefenderThrows.Length)
            {
                for (int i = 0; i < DefenderThrows.Length; ++i)
                {
                    if (AttackerThrows[i] > DefenderThrows[i])
                        Field.countries[tempClickedScndIndex].unitsStationed--;
                    else
                        Field.countries[tempClickedFstIndex].unitsStationed--;
                }
            }
            else
            {
                for (int i = 0; i < AttackerThrows.Length; ++i)
                {
                    if (AttackerThrows[i] > DefenderThrows[i])
                        Field.countries[tempClickedScndIndex].unitsStationed--;
                    else
                        Field.countries[tempClickedFstIndex].unitsStationed--;
                }
            }
        }

        // Move-Methoden
        public void GetNeighboursArray()
        {
            int Neighbourhoods = 0;
            int[,] tempNeighbours = new int[ActualPlayer.ownedCountries.Length, ActualPlayer.ownedCountries.Length];
            for (int i = 0; i < ActualPlayer.ownedCountries.Length; ++i)
            {
                for (int j = 0; j < ActualPlayer.ownedCountries.Length; ++j)
                    tempNeighbours[i, j] = -1;
            }

            int[] OutBufAddUnits;
            bool[] CountryUsed = new bool[ActualPlayer.ownedCountries.Length];
            for (int i = 0; i < CountryUsed.Length; ++i)
                CountryUsed[i] = false;


            for (int i = 0; i < ActualPlayer.ownedCountries.Length; )
            {
                AddNeigbourHoodToOutBuff(ref tempNeighbours, i, ref CountryUsed, Neighbourhoods);
                Neighbourhoods++;
                i = GiveNextUnusedCountry(CountryUsed);
                if (i == -1)
                    break;
            }

            CreateRealOutBuffFromGetNeighboursArray(tempNeighbours);
        }
        /// <summary>
        /// Generiert die relevanten Rückgaben von der Analyse
        /// der "Nachbarschaften", verringert dabei Größe des Arrays
        /// auf das nötigste
        /// erzeugt einen Jagged-Array
        /// </summary>
        /// <param name="tempNeighbours"></param>
        internal void CreateRealOutBuffFromGetNeighboursArray(int[,] tempNeighbours)
        {
            int NeighbourHoods = 0;
            // Für OutBuff richtige Größe der ersten Dimension berechnen
            for (int i = 0; i < tempNeighbours.GetLength(0); ++i)
            {
                if (tempNeighbours[i, 0] != -1)
                    NeighbourHoods++;
                else
                    break;
            }

            int[][] OutBuff = new int[NeighbourHoods][];
            // Einheiten die in Nachbarschaft frei sind mit richtiger Größe erzeugen
            AddUnitsToNeighbourhood = new int[NeighbourHoods];
            // initialisiert mit richtigem wert 0
            for (int i = 0;i < AddUnitsToNeighbourhood.GetLength(0);++i)
            {
                AddUnitsToNeighbourhood[i] = 0;
            }
            // Für jede einzelne Nachbarschaft
            for (int i = 0; i < OutBuff.GetLength(0); ++i)
            {
                // Anzahl der Länder in NeighbourHood auslesen
                int CountriesInNeighbourHood = 0;
                for (int j = 0; j < tempNeighbours.GetLength(1); ++j)
                {
                    if (tempNeighbours[i, j] != -1)
                        CountriesInNeighbourHood++;
                    else
                        break;
                }
                // entsprechend der Anzahl der Länder Größe der zweiten Dimension von OutBuff erzeugen
                OutBuff[i] = new int[CountriesInNeighbourHood];
                for (int j = 0; j < CountriesInNeighbourHood; ++j)
                {
                    // OutBuff mit den Werten belegen
                    OutBuff[i][j] = tempNeighbours[i, j];
                }
            }
            // OutBuff abspeichern
            NeighbouringCountriesFromActualPlayer = OutBuff;
        }
        /// <summary>
        /// Liefert nächstes Land zurück, das noch
        /// ohne "Nachbarschaft" ist
        /// </summary>
        /// <param name="CountryUsed"></param>
        /// <returns></returns>
        internal int GiveNextUnusedCountry(bool[] CountryUsed)
        {
            for (int i = 0; i < CountryUsed.Length; ++i)
            {
                if (CountryUsed[i] == false)
                    return i;
            }
            return -1;
        }
        /// <summary>
        /// Fügt Land mit IndexIn aus OwnedCountries
        /// mit dessen gesamter "Nachbarschaft"
        /// zu OutBuff in NeighbourHood Dimension hinzu
        /// Rekursive Mehode um gesamte Nachbarschaft abzudecken
        /// </summary>
        /// <param name="OutBuff"></param>
        /// <param name="IndexIn"></param>
        /// <param name="CountryUsed"></param>
        /// <param name="NeighbourHood"></param>
        internal void AddNeigbourHoodToOutBuff(ref int[,] OutBuff, int IndexIn, ref bool[] CountryUsed, int NeighbourHood)
        {
            if (CountryUsed[IndexIn] == false)
            {
                CountryUsed[IndexIn] = true;
                AddCountryToOutBuff(ref OutBuff, NeighbourHood, IndexIn);
                for (int i = 0; i < ActualPlayer.ownedCountries[IndexIn].neighbouringCountries.Length; ++i)
                {
                    if (ActualPlayer.ownedCountries[IndexIn].neighbouringCountries[i].owner == ActualPlayer)
                    {
                        AddCountryToOutBuff(ref OutBuff, NeighbourHood, GetOwnedIndexFromName(ActualPlayer.ownedCountries[IndexIn].neighbouringCountries[i].name));
                        AddNeigbourHoodToOutBuff(ref OutBuff, GetOwnedIndexFromName(ActualPlayer.ownedCountries[IndexIn].neighbouringCountries[i].name), ref CountryUsed, NeighbourHood);
                    }
                }
            }
        }
        /// <summary>
        /// Fügt IndexOfCountry in der NeighbourHood
        /// Dimension in OutBuff ein
        /// </summary>
        /// <param name="OutBuff"></param>
        /// <param name="NeighbourHood"></param>
        /// <param name="IndexOfCountry"></param>
        internal void AddCountryToOutBuff(ref int[,] OutBuff, int NeighbourHood, int IndexOfCountry)
        {
            for (int i = 0; i < ActualPlayer.ownedCountries.Length; ++i)//OutBuf.GetLength(NeighbourHood);++i)
            {
                if (OutBuff[NeighbourHood, i] == IndexOfCountry)
                {
                    return;
                }
                else if (OutBuff[NeighbourHood, i] == -1)
                {
                    OutBuff[NeighbourHood, i] = IndexOfCountry;
                    return;
                }
            }
        }
        /// <summary>
        /// Liefert den Index des Landes mit Namen NameIn
        /// aus OwnedCountries von ActualPlayer zurück
        /// </summary>
        /// <param name="NameIn"></param>
        /// <returns></returns>
        internal int GetOwnedIndexFromName(string NameIn)
        {
            for (int i = 0; i < ActualPlayer.ownedCountries.Length; ++i)
            {
                if (ActualPlayer.ownedCountries[i].name == NameIn)
                    return i;
            }
            return -1;
        }
        /// <summary>
        /// Prüft ob in allen Nachbarschaften Einheiten gesetzt wurden
        /// </summary>
        /// <returns></returns>
        internal bool CheckUnitsToSetLeft()
        {
            for (int i = 0;i < AddUnitsToNeighbourhood.Length;++i)
            {
                if (AddUnitsToNeighbourhood[i] != 0)
                    return true;
            }
            return false;
        }

        // Sonstiges
        /// <summary>
        /// Berechnet die Anzahl der Einheiten die ein Spieler am Anfang
        /// eines Zuges zusätzlich erhält
        /// </summary>
        /// <returns></returns>
        private int GetAddUnitsAtBeginningForActualPlayer()
        {
            return ((int)(ActualPlayer.ownedCountries.Length / 3) + GetAddUnitsFromContsForActualPlayer());
        }
        /// <summary>
        /// Sortiert Array, höchste zuerst
        /// </summary>
        /// <param name="Array"></param>
        /// <returns></returns>
        public int[] SortArray(int[] Array)
        {
            int temp = 0;

            for (int write = 0; write < Array.Length; write++)
            {
                for (int sort = 0; sort < Array.Length - 1; sort++)
                {
                    if (Array[sort] < Array[sort + 1])
                    {
                        temp = Array[sort + 1];
                        Array[sort + 1] = Array[sort];
                        Array[sort] = temp;
                    }
                }
            }
            return Array;
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

            // Besitz der Länder in ownedCountries der Spieler speichern, (2seitige Beziehung)
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
                CountriesOfContsOfPlayer[i] = 0;

            for (int i = 0; i < ActualPlayer.ownedCountries.Length; ++i)
                CountriesOfContsOfPlayer[ActualPlayer.ownedCountries[i].continent]++;

            for (int i = 0; i < Field.continents.Length; ++i)
                if (Field.continents[i].numberOfCountries == CountriesOfContsOfPlayer[i])
                    OutBuff += Field.continents[i].AdditionalUnits;

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
        /// Wandelt Farbe in String um
        /// </summary>
        /// <param name="ColorIn"></param>
        /// <returns></returns>
        public string GetStringFromColor(Color ColorIn)
        {
            if (ColorIn == Color.Blue)
                return "black";
            else if (ColorIn == Color.Green)
                return "green";
            else if (ColorIn == Color.Yellow)
                return "yellow";
            else if (ColorIn == Color.Red)
                return "red";
            else if (ColorIn == Color.White)
                return "white";
            else if (ColorIn == Color.Black)
                return "black";
            else if (ColorIn == Color.Violet)
                return "violet";
            else if (ColorIn == Color.Orange)
                return "orange";
            else
                return "green";
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
        /// <summary>
        /// Liefert Mittel-Punkt eines Polygons zurück
        /// In Form1, da in Game.Countries.Corners nur die Eckpunkte des "kleinen",
        /// internen Polygons gespeichert sind
        /// 
        /// Rechnet die Punkte aus den Ländern automatisch mit Faktor um
        /// somit kein umrechnen in aufrufender Methode mehr nötig
        /// </summary>
        /// <param name="realPoints"></param>
        /// <returns></returns>
        public Point GetRealMiddleOfPolygon(Point[] Points)
        {
            double Area = 0.0;
            double MiddleX = 0.0;
            double MiddleY = 0.0;

            Point[] realPoints = GetRealPointsFromCorners(Points);


            for (int i = 0, j = realPoints.Length - 1; i < realPoints.Length; j = i++)
            {
                float temp = realPoints[i].X * realPoints[j].Y - realPoints[j].X * realPoints[i].Y;
                Area += temp;
                MiddleX += (realPoints[i].X + realPoints[j].X) * temp;
                MiddleY += (realPoints[i].Y + realPoints[j].Y) * temp;
            }

            Area *= 3;
            return new Point((int)(MiddleX / Area), (int)(MiddleY / Area));
        }
        /// <summary>
        /// Liefert Position von NumberIn in ArrayIn
        /// zurück, selbst als Array
        /// OutBuff[0] erste Dimension
        /// OutBuff[1] zweite Dimension
        /// </summary>
        /// <param name="ArrayIn"></param>
        /// <param name="NumberIn"></param>
        /// <returns></returns>
        internal int[] GetNumberIndexOutOf2DArray(int[][] ArrayIn, int NumberIn)
        {
            int[] OutBuff = new int[2];
            for (int i = 0; i < ArrayIn.Length; ++i)
            {
                int[] tempArray = ArrayIn[i];
                for (int j = 0;j < tempArray.Length;++j)
                {
                    if (ArrayIn[i][j] == NumberIn)
                    {
                        OutBuff[0] = i;
                        OutBuff[1] = j;
                        return OutBuff;
                    }
                }
            }
            return OutBuff;
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
        //internal int ChangeIndexFromFromOwnedCountriesToAllCountries(int indexIn)
        //{
        //    string name = ActualPlayer.ownedCountries[indexIn].name;
        //    for (int i = 0; i < Field.countries.Length; ++i)
        //    {
        //        if (Field.countries[i].name == name)
        //            return i;
        //    }
        //    return -1;
        //}
        //internal int ChangeIndexFromFromOwnedCountriesToAllCountries(int indexIn)
        //{
        //    string name = ActualPlayer.ownedCountries[indexIn].name;
        //    for (int i = 0; i < Field.countries.Length; ++i)
        //    {
        //        if (Field.countries[i].name == name)
        //            return i;
        //    }
        //    return -1;
        //}
        //internal int[] GetNumberIndexOutOf2DArray(int[,] arrayIn, int numberIn)
        //{
        //    int[] OutBuf = new int[2];
        //    for (int i = 0; i < arrayIn.Length; ++i)
        //    {
        //        for (int j = 0; j < arrayIn.GetLength(i); ++j)
        //            if (arrayIn[i, j] == numberIn)
        //            {
        //                OutBuf[0] = i;
        //                OutBuf[1] = j;
        //                return OutBuf;
        //            }
        //    }
        //    return OutBuf;
        //}
    }
}
