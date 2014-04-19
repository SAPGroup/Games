using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher
{
    public partial class Launcher : Form
    {
        private LauncherControl Control; 

        internal Color CustomBackColor = Color.FromArgb(0x35, 0x18, 0x3E);
        internal Color CustomHomeColor = Color.FromArgb(232, 44, 138);
        internal Color CustomGameColor = Color.FromArgb(89, 176, 49);
        internal Color CustomProfileColor = Color.FromArgb(0x67, 0xC3, 0xCF);
        internal Color CustomForeColor = Color.White;

        internal const int SmallDistance = 2;
        internal const int BigDistance = 7;
        internal Size MainSize = new Size(64, 84);
        internal Size BigButtonSize = new Size(85, 85);
        internal Size SmallButtonSize = new Size(60, 60);
        internal Point BigMainLocation = new Point(10, 12);
        internal Point SmallPnlLocation = new Point(0, 100);

        public Launcher()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control = new LauncherControl(this);
            //Form
            this.Font = new Font("Century Gothic", Font.Size);
            Size = new Size(300, 370);
            BackColor = CustomBackColor;

            //setzt Buttons und Panels mit gewissen Werten
            SetMainPnl();
            SetBigGamePnl();

            SetHomePnl();
            SetSmallGamePnl();

            MakeBigPnlVisible(pnlMain);
        }

        //Button Clicks
        private void btnMainHome_Click(object sender, EventArgs e)
        {
            MakeSmallPnlVisible(pnlHome);
        }
        private void btnMainProfile_Click(object sender, EventArgs e)
        {
            MakeSmallPnlVisible(pnlProfile);
        }
        private void btnMainGames_Click(object sender, EventArgs e)
        {
            MakeSmallPnlVisible(pnlSmallGames);
        }

        //Visibility
        internal void MakeSmallPnlVisible(Panel SmallPnl)
        {
            pnlSmallGames.Visible = false;
            pnlHome.Visible = false;
            pnlProfile.Visible = false;
            SmallPnl.Visible = true;
        }
        internal void MakeBigPnlVisible(Panel BigPanel)
        {
            pnlBigGame.Visible = false;
            pnlMain.Visible = false;
            BigPanel.Visible = true;

            if (BigPanel == pnlBigGame)
            {
                pnlBigGame.Visible = true;
                pnlMain.Visible = false;
                Size = new Size(400, 500);
            }
            else
            {
                pnlBigGame.Visible = false;
                pnlMain.Visible = true;
                Size = new Size(300, 370);
            }
        }

        //Set Big Pnls
        /// <summary>
        /// Setzt Werte für MainPanel
        /// </summary>
        internal void SetMainPnl()
        {
            pnlMain.Size = Size;
            // Design for Buttons
            btnMainHome.FlatStyle = FlatStyle.Flat;
            btnMainHome.FlatAppearance.BorderColor = CustomBackColor;
            btnMainHome.FlatAppearance.BorderSize = 1;

            btnMainGames.FlatStyle = FlatStyle.Flat;
            btnMainGames.FlatAppearance.BorderColor = CustomBackColor;
            btnMainGames.FlatAppearance.BorderSize = 1;

            btnMainProfile.FlatStyle = FlatStyle.Flat;
            btnMainProfile.FlatAppearance.BorderColor = CustomBackColor;
            btnMainProfile.FlatAppearance.BorderSize = 1;

            //Position
            pnlMain.Location = new Point(0, 0);
            pnlSmallGames.Location = SmallPnlLocation;
            pnlHome.Location = SmallPnlLocation;
            pnlProfile.Location = SmallPnlLocation;

            btnMainHome.Size = MainSize;
            btnMainGames.Size = MainSize;
            btnMainProfile.Size = MainSize;

            btnMainHome.Location = BigMainLocation;
            btnMainGames.Location = new Point(btnMainHome.Location.X + btnMainHome.Width + BigDistance, btnMainHome.Location.Y);
            btnMainProfile.Location = new Point(btnMainGames.Location.X + btnMainGames.Width + BigDistance, btnMainGames.Location.Y);

            //btnMainHome.Text = "Home";
            //btnMainHome.TextAlign = ContentAlignment.BottomCenter;
            //btnMainHome.ForeColor = CustomForeColor;

            //Sichtbarkeit
            pnlSmallGames.Visible = false;
            pnlHome.Visible = false;
            pnlProfile.Visible = false;
        }
        internal void SetBigGamePnl()
        {
            // Pnl allgemein
            pnlBigGame.Location = new Point(0, 0);
            pnlBigGame.Size = Size;

            // lblGameName
            lblTxtGameName.ForeColor = CustomForeColor;
            lblTxtGameName.Font = new Font("Century Gothic", 15);
            lblTxtGameName.Location = BigMainLocation;

            //BUTTONS
            btnNewGame.Size = MainSize;
            btnOnline.Size = MainSize;
            btnLoadGame.Size = MainSize;
            
            SetBtnProperties(btnNewGame, CustomBackColor);
            SetBtnProperties(btnOnline, CustomBackColor);
            SetBtnProperties(btnLoadGame, CustomBackColor);
            SetBtnProperties(btnBackToHome, CustomBackColor);

            //Locations
            btnNewGame.Location = new Point(lblTxtGameName.Location.X, lblTxtGameName.Location.Y + BigDistance + lblTxtGameName.Height);
            btnOnline.Location = new Point(btnNewGame.Location.X + BigDistance + btnNewGame.Width, btnNewGame.Location.Y);
            btnLoadGame.Location = new Point(btnOnline.Location.X + BigDistance + btnNewGame.Width, btnNewGame.Location.Y);
        }



        //Set Small Pnls
        /// <summary>
        /// Setzt Werte für Home-Panel
        /// </summary>
        internal void SetHomePnl()
        {
            //          !!!         LastPlayed-Games           !!!
            //txtLabel anpassen
            lblTxtLastPlayed.ForeColor = CustomForeColor;
            lblTxtLastPlayed.BackColor = CustomBackColor;
            lblTxtLastPlayed.Location = new Point(btnMainHome.Location.X - pnlHome.Location.X, lblTxtLastPlayed.Location.Y);

            //Buttons in pnlHome anpassen, Farbe und Style
            SetBtnProperties(btnLastPlayed1, CustomHomeColor);
            SetBtnProperties(btnLastPlayed2, CustomHomeColor);
            SetBtnProperties(btnLastPlayed3, CustomHomeColor);
            SetBtnProperties(btnLastPlayed4, CustomHomeColor);
            SetBtnProperties(btnLastPlayed5, CustomHomeColor);
            SetBtnProperties(btnLastPlayed6, CustomHomeColor);

            //Buttons in pnlHome anpassen, Position
            // Size
            btnLastPlayed1.Size = SmallButtonSize;
            btnLastPlayed2.Size = SmallButtonSize;
            btnLastPlayed3.Size = SmallButtonSize;
            btnLastPlayed4.Size = SmallButtonSize;
            btnLastPlayed5.Size = SmallButtonSize;
            btnLastPlayed6.Size = SmallButtonSize;

            //Pos
            btnLastPlayed1.Location = new Point(lblTxtLastPlayed.Location.X + 3, lblTxtLastPlayed.Location.Y + 20);
            int LeftXPos = btnLastPlayed1.Location.X;
            int RightXPos = LeftXPos + btnLastPlayed1.Width + SmallDistance;
            int FstYPos = btnLastPlayed1.Location.Y,
                ScndYPos = FstYPos + SmallDistance + btnLastPlayed1.Height,
                ThrdYPos = FstYPos + SmallDistance * 2 + btnLastPlayed1.Height * 2;

            btnLastPlayed1.Location = new Point(LeftXPos, FstYPos);
            btnLastPlayed2.Location = new Point(RightXPos, FstYPos);
            btnLastPlayed3.Location = new Point(LeftXPos, ScndYPos);
            btnLastPlayed4.Location = new Point(RightXPos, ScndYPos);
            btnLastPlayed5.Location = new Point(LeftXPos, ThrdYPos);
            btnLastPlayed6.Location = new Point(RightXPos, ThrdYPos);

            //          !!!         Friends         !!!
            lblTxtFriends.ForeColor = CustomForeColor;
            lblTxtFriends.BackColor = CustomBackColor;
            lblTxtFriends.Location = new Point(btnLastPlayed2.Location.X + btnLastPlayed2.Width + BigDistance, 9);

            //Size
            btnFriend1.Size = btnLastPlayed1.Size;
            btnFriend2.Size = btnLastPlayed2.Size;
            btnFriend3.Size = btnLastPlayed3.Size;
            btnFriend4.Size = btnLastPlayed4.Size;
            btnFriend5.Size = btnLastPlayed5.Size;
            btnFriend6.Size = btnLastPlayed6.Size;

            //Properties
            SetBtnProperties(btnFriend1, CustomHomeColor);
            SetBtnProperties(btnFriend2, CustomHomeColor);
            SetBtnProperties(btnFriend3, CustomHomeColor);
            SetBtnProperties(btnFriend4, CustomHomeColor);
            SetBtnProperties(btnFriend5, CustomHomeColor);
            SetBtnProperties(btnFriend6, CustomHomeColor);

            //Location
            LeftXPos = lblTxtFriends.Location.X + 3;
            RightXPos = LeftXPos + btnLastPlayed1.Width + SmallDistance;
            btnFriend1.Location = new Point(LeftXPos, FstYPos);
            btnFriend2.Location = new Point(RightXPos, FstYPos);
            btnFriend3.Location = new Point(LeftXPos, ScndYPos);
            btnFriend4.Location = new Point(RightXPos, ScndYPos);
            btnFriend5.Location = new Point(LeftXPos, ThrdYPos);
            btnFriend6.Location = new Point(RightXPos, ThrdYPos);
        }
        /// <summary>
        /// Setzt GamePanel
        /// </summary>
        internal void SetSmallGamePnl()
        {
            // Properties
            SetBtnProperties(btnGame1, CustomGameColor);
            SetBtnProperties(btnGame2, CustomGameColor);
            SetBtnProperties(btnGame3, CustomGameColor);
            SetBtnProperties(btnGame4, CustomGameColor);
            SetBtnProperties(btnGame5, CustomGameColor);
            SetBtnProperties(btnGame6, CustomGameColor);

            //Size
            btnGame1.Size = BigButtonSize;
            btnGame2.Size = BigButtonSize;
            btnGame3.Size = BigButtonSize;
            btnGame4.Size = BigButtonSize;
            btnGame5.Size = BigButtonSize;
            btnGame6.Size = BigButtonSize;

            //Location
            int LeftXPos = btnMainHome.Location.X,//(int) ((Width - (3*SmallDistance + 3*btnGame1.Width))/2)-pnlGames.Location.X,
                MidXPos = LeftXPos + SmallDistance + btnGame1.Width,
                RightXPos = MidXPos + SmallDistance + btnGame1.Width;
            int TopYPos = 10,
                BotYPos = TopYPos + SmallDistance + btnGame1.Height;

            btnGame1.Location = new Point(LeftXPos, TopYPos);
            btnGame2.Location = new Point(MidXPos, TopYPos);
            btnGame3.Location = new Point(RightXPos, TopYPos);
            btnGame4.Location = new Point(LeftXPos, BotYPos);
            btnGame5.Location = new Point(MidXPos, BotYPos);
            btnGame6.Location = new Point(RightXPos, BotYPos);
        }
        /// <summary>
        /// Setzt in ButtonIn die entsprechenden Style Werte wie in Projekt üblich
        /// </summary>
        /// <param name="ButtonIn"></param>
        internal void SetBtnProperties(Button ButtonIn, Color ColorIn)
        {
            ButtonIn.BackColor = ColorIn;
            ButtonIn.FlatStyle = FlatStyle.Flat;
            ButtonIn.FlatAppearance.BorderColor = ColorIn;
            ButtonIn.FlatAppearance.BorderSize = 1;
            ButtonIn.ForeColor = CustomForeColor;
        }
        internal void SetNewRisikoGamePnl()
        {
            //pnlNewRisikoGame.Location
        }


        private void btnGame1_Click(object sender, EventArgs e)
        {
            MakeBigPnlVisible(pnlBigGame);
            Control.selectedGame = 0;
        }

        private void btnBackToHome_Click(object sender, EventArgs e)
        {
            MakeBigPnlVisible(pnlMain);
        }

        private void btnLoadGame_Click(object sender, EventArgs e)
        {
            Control.LoadGame();
        }
    }
}
