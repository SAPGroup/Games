﻿using System;
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
        private RisikoMain Main;
        // Spielfeld
        private GameField Field = new GameField();
        public GameField field
        {
            get { return Field; }
        }
        // Spieler
        /// <summary>
        /// verschiedene Spieler
        /// </summary>
        private Player[] Players;
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
        private int GameState = -1;
        /// <summary>
        /// Aktueller Spieler
        /// </summary>
        private Player ActualPlayer;
        public Player actualPlayer
        {
            get { return ActualPlayer; }
            set { ActualPlayer = value; }
        }


        /// <summary>
        /// Zum Ein-Auslesen aus 
        /// </summary>
        private FileStream fs;
        private StreamReader sr;
        private StreamWriter sw;
        private string TxtDataSource = "Worldmap.txt";
        
        
        
        //ZufallszahlenGenerator
        Random rnd = new Random();


        //Einheiten
        // Array der die Anzahl der Einheiten die der Spieler setzen möchte speichert
        // in die Länder in seinem Besitz
        private int[] UnitsToAdd;
        public int[] unitsToAdd
        {
            get { return UnitsToAdd; }
            set { UnitsToAdd = value; }
        }

        private bool StartUnitAdding = false;
        public bool startUnitAdding
        {
            get { return StartUnitAdding; }
            set { StartUnitAdding = value; }
        }
        

        // ZUM ZEICHNEN, TODO: Private -> set,get
        // false wenn Karte noch gar nicht gezeichnet wurde
        public bool DrawnMap = false;
        // false echte SpielerFarben, sonst blasser
        public bool DrawPale = false;
        // temporärer Index des zuletzt überfahrenen Landes
        public int tempIndex = -1;
        // Index des zuletzt angeklickten Landes, bei Angreifen und Ziehen (game.gamestate 2 und 3) wichtig
        public int tempClickedIndex = -1;
        // Faktor zum zeichnen
        private int Factor;
        public int factor
        {
            get { return Factor; }
        }
        //
        private Color tempSelCountryColor;
        
        //Einstellungen
        private bool AutoLanderkennung = true;
        public bool autoLanderkennung
        {
            get { return AutoLanderkennung; }
        }

        // Konstruktor
        public GameControl(RisikoMain MainIn)
        {
            Main = MainIn;
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
        private void DrawAndLoadMap()
        {
            //Main.GiveMap().BackgroundImage = 


            //LoadCountriesFromDBSource();
            LoadCountriesFromTxtSource();

            int[] WidthHeight = Main.GetMapData();
            CheckFactor(WidthHeight[0], WidthHeight[1]);

            // lässt Karte zeichnen
            Main.DrawMap();

            DrawnMap = true;
        }
        /// <summary>
        /// Zeichnet Karte bei veränderung des Faktors erneut, ohne laden
        /// </summary>
        private void DrawMapWoLoad()
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
            // hier könnte man DrawnMap (bool (Map bereits gezeichnet)) auch abfragen, jedoch unnötig
            // da bereits in CheckClickOnPolygon
            if (autoLanderkennung)
            {
                Point clickedPosition = new Point(e.X, e.Y);
                //int temp = checkClickOnPolygon(clickedPosition);
                int temp = CheckClickInPolygon(clickedPosition);

                //if(temp != -1)
                //  tempOldIndex = temp;

                if (temp == -1 & tempIndex != -1)//& tempOldIndex != tempIndex)
                {
                    //kein Treffer

                    // auskommentiert da Abfrage zu oft (auch in einem Land) auftritt
                    // wieso liefert CheckClickOnPolygon direkt in einem Land -1? 

                    Field.countries[tempIndex].colorOfCountry = tempSelCountryColor;

                    Main.DrawCountry(tempIndex);
                    // da sonst Kreis in der Mitte verschwindet
                    Main.DrawMiddleCircle(tempIndex);
                    tempIndex = -1;
                }
                else if (temp != -1 & temp != tempIndex)
                {
                    //bei Treffer                
                    if (tempIndex != -1)
                    {
                        Field.countries[tempIndex].colorOfCountry = tempSelCountryColor;
                        Main.DrawCountry(tempIndex);
                        // da sonst der Kreis in der Mitte verschwindet
                        Main.DrawMiddleCircle(tempIndex);
                    }

                    tempSelCountryColor = Field.countries[temp].colorOfCountry;
                    Field.countries[temp].colorOfCountry = Color.Yellow;

                    tempIndex = temp;

                    Main.DrawCountry(temp);
                    // da sonst Kreis in der Mitte verschwindet
                    Main.DrawMiddleCircle(temp);
                }
            }
        }


        // Sonstiges
        /// <summary>
        /// Liefert Index des aktuellen Spielers in Players-Array
        /// </summary>
        /// <returns></returns>
        private int ActualPlayerIndex()
        {
            for (int i = 0;i < Players.Length;++i)
            {
                if (Players[i].name == ActualPlayer.name)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Lädt Karte aus Textdatei
        /// </summary>
        private void LoadCountriesFromTxtSource()
        {
            // initialisieren der Reader, um aus Txt-Datei zu lesen
            fs = new FileStream(TxtDataSource, FileMode.Open);
            sr = new StreamReader(fs);

            int NumberOfCountries = 0;
            string zeile;

            while (sr.Peek() != -1)
            {
                // nächste Zeile in zeile speichern
                zeile = sr.ReadLine();
                ++NumberOfCountries;
            }
            // erzeugt Länder Array und setzt Länge dessen in Field fest
            Field.numberOfCountries = NumberOfCountries;
            Field.countries = new Country[NumberOfCountries];

            // springt an Anfang der Datei
            fs.Position = 0;
            sr.DiscardBufferedData();
            //sr.BaseStream.Seek(0, SeekOrigin.Begin);
           



            // temp-Werte der Länder
            string tempName;
            Color tempColor;
            Point[] tempPoints;
            // Um die Maximalen Werte der Karte auszulesen
            int tempMaxX = 0, tempMaxY = 0;
            // Zähler für die erzeugten Länder
            int Counter = 0;

            while (sr.Peek() != -1)
            {
                // nächste Zeile in zeile speichern
                zeile = sr.ReadLine();
                // Zeile zerlegen
                string [] Parts = zeile.Split('.');

                // Name und Farbe des Landes auslesen
                tempColor = GetColorFromString(Parts[0]);
                tempName = Parts[1];

                // Eckpunkte des Landes auslesen, -> String wieder zerlegen
                string[] Corners = Parts[4].Split(';');
                // Länge von tempPoints festlegen
                tempPoints = new Point[Corners.Length/2];
                for (int i = 0;i < Corners.Length/2; i++)
                {
                    tempPoints[i].X = Convert.ToInt32(Corners[i * 2]);
                    tempPoints[i].Y = Convert.ToInt32(Corners[i * 2 + 1]);
                    if (Convert.ToInt32(Corners[i * 2]) > tempMaxX)
                        tempMaxX = Convert.ToInt32(Corners[i * 2]);
                    if (Convert.ToInt32(Corners[i * 2 + 1]) > tempMaxY)
                        tempMaxY = Convert.ToInt32(Corners[i * 2 + 1]);
                }
                Field.countries[Counter] = new Country(tempName, tempPoints, tempColor);
                Counter++;
            }
            Field.height = tempMaxY;
            Field.width = tempMaxX;

            // springt an Anfang der Datei
            fs.Position = 0;
            sr.DiscardBufferedData();
            //sr.BaseStream.Seek(0, SeekOrigin.Begin);





            // Zähler kann von oben verwendet werden, wird auf 0 zurück gesetzt
            Counter = 0;
            while (sr.Peek() != -1)
            {
                // nächste Zeile in zeile speichern
                zeile = sr.ReadLine();
                // Zeile zerlegen
                string[] Parts = zeile.Split('.');

                string[] Neighbours = Parts[5].Split(';');
                string[] tempNeighbouringCountries = new string[Neighbours.Length];
                for (int i = 0; i < Neighbours.Length; ++i)
                {
                    int tempCountryID = Convert.ToInt32(Neighbours[i]);
                    tempNeighbouringCountries[i] = Field.countries[tempCountryID - 1].name;
                }

                Field.countries[Counter].neighbouringCountries = tempNeighbouringCountries;
                Counter++;
            }



            // Ende,  streamreader schließen, kappt Verbindung zur Txt-Datei
            sr.Close();
        }

        

        /// <summary>
        /// Setzt den Faktor der Darstellung der Karte
        /// </summary>
        private void CheckFactor(int newWidth, int newHeight)
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
        public int CheckClickInPolygon(Point ClickedPosition)
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
        //private OleDbConnection con = new OleDbConnection();
        //private OleDbCommand cmd = new OleDbCommand();
        //private OleDbDataReader reader;
        ///// <summary>
        ///// Pfad der Quelldatei!!
        ///// syntax: con.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
        ///// "Data Source=C:\\Temp\\Risiko_Weltkarte.accdb";
        ///// 
        ///// TODO: Durch Datei öfnnen verändern (neues Spiel -> Quelldatei auswählen usw)
        ///// </summary>
        //private string DataSourceString = System.Environment.CurrentDirectory + "\\Risiko_Weltkarte2.accdb";
        ///// <summary>
        ///// Lädt Länder aus DB
        ///// </summary>
        //private void LoadCountriesFromDBSource()
        //{
        //    // Source einbinden
        //    //DataSourceString = System.Environment.CurrentDirectory + "\\Risiko_Weltkarte1.accdb";
        //    con.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
        //                           "Data Source=" + DataSourceString;

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
        //private void GetNumberOfCountriesDB()
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
    }
}
