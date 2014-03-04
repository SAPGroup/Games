using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Risiko
{
    class GameControl
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
        /// Reader, zum Lesen aus der Datenbank
        /// </summary>
        OleDbConnection con = new OleDbConnection();
        OleDbCommand cmd = new OleDbCommand();
        OleDbDataReader reader;

        /// <summary>
        /// Pfad der Quelldatei!!
        /// syntax: con.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
        /// "Data Source=C:\\Temp\\Risiko_Weltkarte.accdb";
        /// 
        /// TODO: Durch Datei öfnnen verändern (neues Spiel -> Quelldatei auswählen usw)
        /// </summary>
        private string DataSourceString = System.Environment.CurrentDirectory + "\\Risiko_Weltkarte2.accdb";
        
        
        
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
        
        //Einstellungen
        // Landerkennung usw

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

            LoadCountriesFromDBSource();

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

        private void LoadCountriesFromDBSource()
        {
            // Source einbinden
            //DataSourceString = System.Environment.CurrentDirectory + "\\Risiko_Weltkarte1.accdb";
            con.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                                   "Data Source=" + DataSourceString;

            // Anzahl der Länder auslesen
            GetNumberOfCountriesDB();

            cmd.Connection = con;
            // Aus table Weltkarte (alles)
            cmd.CommandText = "select * from Worldmap;";

            // Länder erzeugen
            Field.countries = new Country[Field.numberOfCountries];

            try
            {
                //öffnen
                con.Open();
                reader = cmd.ExecuteReader();

                // Fortlaufender Zähler, zählt welche Country aktuell erzeugt werden muss
                int counter = 0;

                // temp Werte, die später dem Konstruktor der Country zugeführt werden
                Color tempColorOfCountry;
                string tempName;
                Point[] tempPoints;
                // Max X und Y Werte, um Höhe und Breite der internen "kleinen" Karte herauszufinden
                int tempMaxX = 0;
                int tempMaxY = 0;

                while (reader.Read())
                {
                    // Name
                    tempName = Convert.ToString(reader["Name"]);

                    // Color (Farbe)
                    tempColorOfCountry = GetColorFromString(Convert.ToString(reader["Color"]));

                    // Corners (Ecken)
                    string tempCorners = Convert.ToString(reader["Corners"]);
                    string[] Corners = tempCorners.Split(';');
                    tempPoints = new Point[Corners.Length / 2];
                    for (int i = 0; i < Corners.Length / 2; ++i)
                    {
                        tempPoints[i].X = Convert.ToInt32(Corners[i * 2]);
                        tempPoints[i].Y = Convert.ToInt32(Corners[i * 2 + 1]);
                        if (Convert.ToInt32(Corners[i * 2]) > tempMaxX)
                            tempMaxX = Convert.ToInt32(Corners[i * 2]);
                        if (Convert.ToInt32(Corners[i * 2 + 1]) > tempMaxY)
                            tempMaxY = Convert.ToInt32(Corners[i * 2 + 1]);
                    }

                    // Konstruktor der Country
                    Field.countries[counter] = new Country(tempName, tempPoints, tempColorOfCountry);
                    counter++;
                }
                Field.width = tempMaxX;
                Field.height = tempMaxY;
                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                // temp String, falls Fehlermeldung
                string temp = ex.Message;
            }


            // Laden der NachbarLänder, Countries muss schon festlegen, da daraus Namen
            // der Länder gelesen werden
            cmd.CommandText = "select * from Worldmap;";
            try
            {
                //öffnen
                con.Open();
                reader = cmd.ExecuteReader();

                // Fortlaufender Zähler, zählt welche Country aktuell erzeugt werden muss
                int counter = 0;

                while (reader.Read())
                {

                    string tempNeighbours = Convert.ToString(reader["Neighbours"]);
                    string[] Neighbours = tempNeighbours.Split(';');
                    string[] tempNeighbouringCountries = new string[Neighbours.Length];
                    for (int i = 0; i < Neighbours.Length; ++i)
                    {
                        int tempCountryID = Convert.ToInt32(Neighbours[i]);
                        tempNeighbouringCountries[i] = Field.countries[tempCountryID - 1].name;
                    }

                    Field.countries[counter].neighbouringCountries = tempNeighbouringCountries;
                    counter++;
                }
                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                // temp String, falls Fehlermeldung
                string temp = ex.Message;
                MessageBox.Show(temp);
            }
        }


        /// <summary>
        /// Speichert Anzahl der Länder in Field.numberOfCountries ab
        /// </summary>
        private void GetNumberOfCountriesDB()
        {
            cmd.Connection = con;
            cmd.CommandText = "select * from Worldmap;";
            try
            {
                //öffnen
                con.Open();
                reader = cmd.ExecuteReader();
                // Anzahl der Länder
                int tempNumberOfCountries = 0;
                while (reader.Read())
                {
                    ++tempNumberOfCountries;
                }
                Field.numberOfCountries = tempNumberOfCountries;
                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                string temp = ex.Message;
            }
        }

        /// <summary>
        /// Setzt den Faktor der Darstellung der Karte
        /// </summary>
        private void CheckFactor(int newHeight, int newWidth)
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

    }
}
