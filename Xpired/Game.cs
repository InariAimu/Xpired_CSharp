using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading;

namespace Xpired
{
    public class Game
    {
        public Form GameForm;
        public PictureBox GameWnd;
        public ToolStripLabel FpsLabel;

        public Resource GameRes = new Resource();

        public List<Level> LevelList = new List<Level>();
        public String LevelFileName;
        public int CurrLevelID;
        public Level CurrLevel;

        public Player Player;

        public bool HasBem = false;
        public int CurrTime = 0;
        public bool IsTimedLevel = false;

        public KeyState KeyState = new KeyState();

        public int GCycle = 33;
        public int DeadDelay = 30;

        public bool IsGameOver;

        public Game(PictureBox Wnd)
        {
            this.GameWnd = Wnd;
            InitGame();
        }

        public void InitGame()
        {
            Player = new Player();
        }

        public void GameMain()
        {
            int Ticks;
            int OldTick;
            int Frame;
            int WT = 0;
            int Sec = 0;
            bool Sw = false;

            IsGameOver = false;
            CurrLevelID = 0;

            do
            {
                InitLevel(CurrLevelID);
                Sec = 1000 / GCycle;
                Player.Alive = (int)(1.5 * DeadDelay);
                Frame = 0;
                OldTick = ExactTimer.GetDotNetTick();
                WT = OldTick;

                do
                {
                    this.GameWnd.Invoke(new Action(() =>
                    {
                        if (this.GameWnd.Image != null)
                        {
                            this.GameWnd.Image.Dispose();
                        }
                        this.GameWnd.Image = RenderGame();
                    }));
                    Frame++;

                    Ticks = ExactTimer.GetDotNetTick();
                    if (Ticks - OldTick >= 1000)
                    {
                        this.GameForm.Invoke(new Action(() => { this.FpsLabel.Text = Frame.ToString() + " fps"; }));
                        Frame = 0;
                        OldTick = Ticks;
                    }

                    if ((WT + GCycle) > Ticks)
                    {
                        Thread.Sleep((WT + GCycle) - Ticks);
                    }

                    Ticks = ExactTimer.GetDotNetTick();

                    while ((WT + GCycle) <= Ticks)
                    {

                        if ((Math.Abs(Player.Px) == 19) || (Math.Abs(Player.Py) == 19))
                        {
                            Player.X += Math.Sign(Player.Px);
                            Player.Y += Math.Sign(Player.Py);
                            //Player.F=1;

                            if ((CurrLevel.Map[Player.X, Player.Y].BGTypeID == Convert.ToInt32('T')) &&
                                (Player.Px != 0 || Player.Py != 0))
                            {
                                Player.X2 = Player.X;
                                Player.Y2 = Player.Y;
                                if (GetTeleport(Player.X2, Player.Y2, out Player.X2, out Player.Y2))
                                {
                                    TeleportEffect(Player.X, Player.Y);
                                    Player.X = Player.X2;
                                    Player.Y = Player.Y2;
                                }
                                Player.Px = Math.Sign(Player.Px);
                                Player.Py = Math.Sign(Player.Py);
                                if (!IsFree(Player.Px, Player.Py))
                                {
                                    Player.Px = 0;
                                    Player.Py = 0;
                                }
                            }
                            else
                            {
                                Player.Px = 0;
                                Player.Py = 0;
                            }
                        }
                        else if (Player.Px != 0 || Player.Py != 0)
                        {
                            Player.Px += Player.F * Math.Sign(Player.Px);
                            Player.Py += Player.F * Math.Sign(Player.Py);
                            Player.LM = (Player.Py > 0 ? 1 : 0) + (Player.Px > 0 ? 1 : 0) * 2 + (Player.Px < 0 ? 1 : 0) * 3;
                        }

                        MoveObjects();

                        if (Player.Px == 0 && Player.Py == 0 && Player.Alive == DeadDelay)
                        {
                            if (KeyState.Ctrl)
                            {
                                Player.F = 2;
                            }
                            else
                            {
                                Player.F = 1;
                            }

                            if (CurrLevel.Map[Player.X, Player.Y].BGTypeID == Convert.ToInt32('#'))
                            {
                                Player.F = 2;
                                if ((!KeyState.Left) && (!KeyState.Right) && (!KeyState.Up) && (!KeyState.Down))
                                {
                                    Player.Px = -1 * (Player.LM == 3 ? 1 : 0) + (Player.LM == 2 ? 1 : 0);
                                    Player.Py = -1 * (Player.LM == 0 ? 1 : 0) + (Player.LM == 1 ? 1 : 0);
                                }
                                if (!IsFree(Player.Px, Player.Py))
                                {
                                    Player.Px = 0;
                                    Player.Py = 0;
                                }
                            }

                            if ((KeyState.Left && !KeyState.Right && !KeyState.Up && !KeyState.Down) || (KeyState.Left && KeyState.Down && Sw) || (KeyState.Left && KeyState.Up && !Sw))
                            {
                                if (IsFree(-1, 0))
                                {
                                    Sw = !Sw; Player.Px = -1; Player.Py = 0;
                                }
                                else Sw = !Sw;
                            }
                            else if ((KeyState.Right && !KeyState.Left && !KeyState.Up && !KeyState.Down) || (KeyState.Right && KeyState.Down && !Sw) || (KeyState.Right && KeyState.Up && Sw))
                            {
                                if (IsFree(+1, 0))
                                {
                                    Sw = !Sw; Player.Px = +1; Player.Py = 0;
                                }
                                else Sw = !Sw;
                            }
                            else if ((KeyState.Up && !KeyState.Down && !KeyState.Left && !KeyState.Right) || (KeyState.Up && KeyState.Left && Sw) || (KeyState.Up && KeyState.Right && !Sw))
                            {
                                if (IsFree(0, -1))
                                {
                                    Sw = !Sw; Player.Py = -1; Player.Px = 0;
                                }
                                else Sw = !Sw;
                            }
                            else if ((KeyState.Down && !KeyState.Up && !KeyState.Left && !KeyState.Right) || (KeyState.Down && KeyState.Left && !Sw) || (KeyState.Down && KeyState.Right && Sw))
                            {
                                if (IsFree(0, +1))
                                {
                                    Sw = !Sw; Player.Py = +1; Player.Px = 0;
                                }
                                else Sw = !Sw;
                            }
                        }

                        if (Player.Alive == DeadDelay)
                        {
                            if (Math.Abs(Player.Px) > 9 || Math.Abs(Player.Py) > 9)
                            {
                                if ((CurrLevel.Map[Player.X + Math.Sign(Player.Px), Player.Y + Math.Sign(Player.Py)].F == 4) ||
                                    (CurrLevel.Map[Player.X + Math.Sign(Player.Px), Player.Y + Math.Sign(Player.Py)].F == 6) ||
                                    (CurrLevel.Map[Player.X + Math.Sign(Player.Px), Player.Y + Math.Sign(Player.Py)].F == 8))
                                {
                                    Player.Alive = DeadDelay - 1;
                                    //TextAdd(Player.X * 20 + 10, Player.Y * 20 + 10, "AAAAaaaaaeghr!");
                                }
                            }
                            else if ((CurrLevel.Map[Player.X, Player.Y].F == 4) ||
                                (CurrLevel.Map[Player.X, Player.Y].F == 6) ||
                                (CurrLevel.Map[Player.X, Player.Y].F == 8))
                            {
                                Player.Alive = DeadDelay - 1;
                                //TextAdd(Player.X * 20 + 10, Player.Y * 20 + 10, "AAAAaaaaaeghr!");
                            }
                            else
                            if (CurrLevel.Map[Player.X, Player.Y].BGTypeID == Convert.ToInt32('@'))
                            {
                                Player.Alive = DeadDelay - 1;
                                //TextAdd(Player.X * 20 + 10, Player.Y * 20 + 10, "AAAAaaaaaeghr!");
                            }
                            else
                            if (IsBemAround(Player.X, Player.Y))
                            {
                                Player.Alive = DeadDelay - 1;
                                //TextAdd(Player.X * 20 + 10, Player.Y * 20 + 10, "AAAAaaaaaeghr!");
                            }
                        }

                        if ((Player.Alive != DeadDelay) && (Player.Alive != 0))
                        {
                            Player.Alive--;
                        }

                        if (IsTimedLevel)
                        {
                            Sec--;
                            if (Sec <= 0)
                            {
                                Sec = 1000 / GCycle;
                                this.CurrTime--;
                                if (this.CurrTime <= 0)
                                {
                                    this.CurrTime = 0;
                                    Player.Alive--;
                                    IsTimedLevel = false;
                                    //ExplodeLevel();
                                }
                            }
                        }

                        //TextBehave();

                        WT += GCycle;
                    }

                } while (Player.Alive > 0);

            } while (IsGameOver == false);
        }

        public void InitLevel(int LevelID)
        {
            this.CurrLevel = new Level(LevelList[LevelID]);
            this.CurrLevel.BackGroundImage = GameRes.LoadTiledBackground(
                CurrLevel.BackGroundFileName, CurrLevel.Width * 20, CurrLevel.Height * 20);

            this.HasBem = false;
            for (int x = 0; x < CurrLevel.Width; x++)
            {
                for (int y = 0; y < CurrLevel.Height; y++)
                {
                    if (CurrLevel.Map[x, y].FGTypeID == Convert.ToInt32('B'))
                    {
                        this.HasBem = true;
                        break;
                    }
                }
                if (this.HasBem) break;
            }

            using (Graphics g = Graphics.FromImage(this.CurrLevel.BackGroundImage))
            {
                for (int x = 0; x < CurrLevel.Width; x++)
                {
                    for (int y = 0; y < CurrLevel.Height; y++)
                    {
                        if (CurrLevel.Map[x, y].BGTypeID == Convert.ToInt32('*'))
                        {
                            Player.X = x;
                            Player.Y = y;
                        }
                        if (CurrLevel.Map[x, y].FGTypeID == Convert.ToInt32('B'))
                        {
                            CurrLevel.Map[x, y].F = 10;
                        }

                        if (!this.HasBem || CurrLevel.Map[x, y].BGTypeID != Convert.ToInt32('+'))
                        {
                            if (CurrLevel.Map[x, y].BGSpriteID != 32)
                            {
                                g.DrawImage(GameRes.SpriteList[CurrLevel.Map[x, y].BGSpriteID].Image, x * 20, y * 20);
                            }
                        }
                        else
                        {
                            g.DrawImage(GameRes.SpriteList[Convert.ToInt32('+')].Image, x * 20, y * 20);
                        }
                    }
                }
            }
        }

        public bool IsBem(int X, int Y)
        {
            if (X < 0 || X >= CurrLevel.Width || Y < 0 || Y >= CurrLevel.Height)
            {
                return false;
            }
            if (CurrLevel.Map[X, Y].FGTypeID == Convert.ToInt32('B'))
            {
                return true;
            }
            return false;
        }

        public bool IsBemAround(int X, int Y)
        {
            if (IsBem(X - 1, Y - 1) || IsBem(X, Y - 1) || IsBem(X + 1, Y - 1) ||
                IsBem(X - 1, Y) || IsBem(X + 1, Y) || IsBem(X - 1, Y + 1) ||
                IsBem(X, Y + 1) || IsBem(X + 1, Y + 1))
            {
                return true;
            }
            return false;
        }

        public bool IsHard(int X, int Y)
        {
            if (X == Player.X && Y == Player.Y)
            {
                return true;
            }
            if (X < 0 || X >= CurrLevel.Width || Y < 0 || Y >= CurrLevel.Height ||
                CurrLevel.Map[X, Y].FGTypeID == Convert.ToInt32('X') ||
                CurrLevel.Map[X, Y].FGTypeID == Convert.ToInt32('x') ||
                CurrLevel.Map[X, Y].FGTypeID == Convert.ToInt32('B'))
            {
                return true;
            }
            return false;
        }

        public bool IsShift(int X, int Y)
        {
            if (X < 0 || X >= CurrLevel.Width || Y < 0 || Y >= CurrLevel.Height)
            {
                return false;
            }
            if (CurrLevel.Map[X, Y].FGTypeID == Convert.ToInt32('c') ||
                CurrLevel.Map[X, Y].FGTypeID == Convert.ToInt32('o') ||
                CurrLevel.Map[X, Y].FGTypeID == Convert.ToInt32('O'))
            {
                return true;
            }
            return false;
        }

        public bool IsObject(int X, int Y)
        {
            if (X < 0 || X >= CurrLevel.Width || Y < 0 || Y >= CurrLevel.Height)
            {
                return false;
            }
            if (IsBem(X, Y) || IsHard(X, Y) || IsShift(X, Y))
            {
                return true;
            }
            return false;
        }

        public bool IsFree(int PlayerPx, int PlayerPy)
        {
            if (IsHard(Player.X + PlayerPx, Player.Y + PlayerPy)) return false;
            if (IsShift(Player.X + PlayerPx, Player.Y + PlayerPy) &&
                Math.Sign(CurrLevel.Map[Player.X + PlayerPx, Player.Y + PlayerPy].Px) == Math.Sign(PlayerPx) &&
                Math.Sign(CurrLevel.Map[Player.X + PlayerPx, Player.Y + PlayerPy].Py) == Math.Sign(PlayerPy))
            {
                return true;
            }
            if (IsShift(Player.X + PlayerPx, Player.Y + PlayerPy))
            {
                return CheckAndDoPush(PlayerPx, PlayerPy, Player.F);
            }
            return true;
        }

        /// <summary>
        /// 检查推动方向上的碰撞
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Px"></param>
        /// <param name="Py"></param>
        /// <returns></returns>
        public bool CheckCollision(int X, int Y, int Px, int Py)
        {
            if (X + Px < 0 || Y + Py < 0 || X + Px > 19 || Y + Py > 19)
            {
                return true;
            }
            if (IsObject(X + Px, Y + Py) &&
                (Math.Sign(CurrLevel.Map[X + Px, Y + Py].Px) != Px || Math.Sign(CurrLevel.Map[X + Px, Y + Py].Py) != Py))
            {
                return true;
            }
            if (Px <= 0 && IsObject(X + Px - 1, Y + Py) && CurrLevel.Map[X + Px - 1, Y + Py].Px > 0)
            {
                return true;
            }
            if (Px >= 0 && IsObject(X + Px + 1, Y + Py) && CurrLevel.Map[X + Px + 1, Y + Py].Px < 0)
            {
                return true;
            }
            if (Py <= 0 && IsObject(X + Px, Y + Py - 1) && CurrLevel.Map[X + Px, Y + Py - 1].Py > 0)
            {
                return true;
            }
            if (Py >= 0 && IsObject(X + Px, Y + Py + 1) && CurrLevel.Map[X + Px, Y + Py + 1].Py < 0)
            {
                return true;
            }
            return false;
        }

        public bool CheckAndDoPush(int Px, int Py, int F)
        {
            if (CurrLevel.Map[Px + Player.X, Py + Player.Y].X != 0 ||
                CurrLevel.Map[Px + Player.X, Py + Player.Y].Y != 0 ||
                !IsShift(Px + Player.X, Py + Player.Y) ||
                IsShift(Px * 2 + Player.X, Py * 2 + Player.Y) ||
                IsHard(Px * 2 + Player.X, Py * 2 + Player.Y))
            {
                return false;
            }

            if (CurrLevel.Map[Px + Player.X, Py + Player.Y].FGTypeID == Convert.ToInt32('c'))
            {
                Player.F = 1;
                F = 1;
            }

            CurrLevel.Map[Px + Player.X, Py + Player.Y].Px = Px;
            CurrLevel.Map[Px + Player.X, Py + Player.Y].Py = Py;
            CurrLevel.Map[Px + Player.X, Py + Player.Y].X = Px * F;
            CurrLevel.Map[Px + Player.X, Py + Player.Y].Y = Py * F;
            CurrLevel.Map[Px + Player.X, Py + Player.Y].F = F;
            return true;
        }

        void TeleportEffect(int X, int Y)
        {
            //PlaySample(Snd_Teleport);
            CurrLevel.Map[X, Y].FGSpriteID = 11;
            CurrLevel.Map[X, Y].FGTypeID = Convert.ToInt32(' ');
            CurrLevel.Map[X, Y].F = 9;
            CurrLevel.Map[X, Y].X = 0;
            CurrLevel.Map[X, Y].Y = 0;
            CurrLevel.Map[X, Y].Px = 3;
            CurrLevel.Map[X, Y].Py = 3;
        }

        public bool GetTeleport(int x, int y, out int X, out int Y)
        {
            int i, j;
            X = x;
            Y = y;
            for (i = 0; i <= 20; i++)
            {
                for (j = (Y + 1) * (i == 0 ? 1 : 0); j < 20; j++)
                {
                    if (CurrLevel.Map[(X + i) % 20, j].BGTypeID == Convert.ToInt32('T') &&
                        (CurrLevel.Map[(X + i) % 20, j].FGTypeID == Convert.ToInt32(' ') ||
                        ((X + i) % 20 == X) && j == Y) &&
                        ((X + i) % 20 != Player.X || j != Player.Y))
                    {
                        if (((X + i) % 20 == X) && (j == Y))
                        {
                            return false;
                        }
                        X = (X + i) % 20;
                        Y = j;
                        return true;
                    }
                }
            }
            return false;
        }

        public void MoveObjects()
        {
            bool BEM2 = false;
            Random rand = new Random();

            using (Graphics g = Graphics.FromImage(CurrLevel.BackGroundImage))
            {
                Brush b = new SolidBrush(Color.Black);

                for (int x = 0; x < CurrLevel.Width; x++)
                {
                    for (int y = 0; y < CurrLevel.Height; y++)
                    {
                        if (CurrLevel.Map[x, y].F != 0)
                        {
                            if (CurrLevel.Map[x, y].F < 3)
                            {// SHIFT/SWIFT
                                if ((CurrLevel.Map[x, y].X == 0) && (CurrLevel.Map[x, y].Y == 0))
                                {
                                    if ((CurrLevel.Map[x, y].F == 1) &&
                                        (CurrLevel.Map[x, y].BGTypeID != Convert.ToInt32('T')))
                                        CurrLevel.Map[x, y].F = 0;

                                    if (CurrLevel.Map[x, y].BGTypeID == Convert.ToInt32('@'))
                                    {
                                        //PlaySample(Snd_Burn);
                                        g.DrawImageUnscaled(GameRes.SpriteList[4].Image, x * 20, y * 20);
                                        CurrLevel.Map[x, y].BGTypeID = Convert.ToInt32(' ');
                                        CurrLevel.Map[x, y].FGTypeID = Convert.ToInt32(' ');
                                        CurrLevel.Map[x, y].F = 6;
                                        CurrLevel.Map[x, y].Px = 0;
                                        CurrLevel.Map[x, y].Py = 0;
                                    }
                                    else if (CurrLevel.Map[x, y].BGTypeID == Convert.ToInt32('%'))
                                    {
                                        CurrLevel.Map[x, y].F = 0;
                                        CurrLevel.Map[x, y].Px = 0;
                                        CurrLevel.Map[x, y].Py = 0;
                                    }
                                    else if ((CurrLevel.Map[x, y].Px != 0 || CurrLevel.Map[x, y].Py != 0) &&
                                        (CheckCollision(x, y, Math.Sign(CurrLevel.Map[x, y].Px), Math.Sign(CurrLevel.Map[x, y].Py))))
                                    {
                                        if ((CurrLevel.Map[x, y].FGTypeID != 'c') && (CurrLevel.Map[x, y].F > 1))
                                        {
                                            CurrLevel.Map[x, y].F = 4 + (CurrLevel.Map[x, y].FGTypeID == Convert.ToInt32('O') ? 1 : 0);
                                        }
                                        CurrLevel.Map[x, y].Px = 0;
                                        CurrLevel.Map[x, y].Py = 0;
                                    }
                                }

                                if ((CurrLevel.Map[x, y].Px != 0 || CurrLevel.Map[x, y].Py != 0) &&
                                    ((Math.Abs(CurrLevel.Map[x, y].X += CurrLevel.Map[x, y].Px * CurrLevel.Map[x, y].F) >= 20) ||
                                    (Math.Abs(CurrLevel.Map[x, y].Y += CurrLevel.Map[x, y].Py * CurrLevel.Map[x, y].F) >= 20)))
                                {
                                    if (CurrLevel.Map[x + CurrLevel.Map[x, y].Px, y + CurrLevel.Map[x, y].Py].BGTypeID == Convert.ToInt32('T'))
                                    {// T e l e p o r t
                                        int k = x + CurrLevel.Map[x, y].Px;
                                        int l = y + CurrLevel.Map[x, y].Py;
                                        bool p = GetTeleport(k, l, out k, out l);

                                        CurrLevel.Map[k, l].FGSpriteID = CurrLevel.Map[x, y].FGSpriteID;
                                        CurrLevel.Map[k, l].FGTypeID = CurrLevel.Map[x, y].FGTypeID;
                                        CurrLevel.Map[k, l].X = 0;
                                        CurrLevel.Map[k, l].Y = 0;

                                        CurrLevel.Map[k, l].Px = CurrLevel.Map[x, y].Px;
                                        CurrLevel.Map[k, l].Py = CurrLevel.Map[x, y].Py;
                                        CurrLevel.Map[k, l].F = CurrLevel.Map[x, y].F;

                                        if (p) TeleportEffect(x + CurrLevel.Map[x, y].Px, y + CurrLevel.Map[x, y].Py);
                                    }
                                    else
                                    {
                                        CurrLevel.Map[x + CurrLevel.Map[x, y].Px, y + CurrLevel.Map[x, y].Py].FGSpriteID = CurrLevel.Map[x, y].FGSpriteID;
                                        CurrLevel.Map[x + CurrLevel.Map[x, y].Px, y + CurrLevel.Map[x, y].Py].FGTypeID = CurrLevel.Map[x, y].FGTypeID;
                                        CurrLevel.Map[x + CurrLevel.Map[x, y].Px, y + CurrLevel.Map[x, y].Py].X = 0;
                                        CurrLevel.Map[x + CurrLevel.Map[x, y].Px, y + CurrLevel.Map[x, y].Py].Y = 0;
                                        if (CurrLevel.Map[x, y].F < 2)
                                        {
                                            CurrLevel.Map[x + CurrLevel.Map[x, y].Px, y + CurrLevel.Map[x, y].Py].Px = 0;
                                            CurrLevel.Map[x + CurrLevel.Map[x, y].Px, y + CurrLevel.Map[x, y].Py].Py = 0;
                                            CurrLevel.Map[x + CurrLevel.Map[x, y].Px, y + CurrLevel.Map[x, y].Py].F = 1;
                                        }
                                        else
                                        {
                                            CurrLevel.Map[x + CurrLevel.Map[x, y].Px, y + CurrLevel.Map[x, y].Py].Px = CurrLevel.Map[x, y].Px;
                                            CurrLevel.Map[x + CurrLevel.Map[x, y].Px, y + CurrLevel.Map[x, y].Py].Py = CurrLevel.Map[x, y].Py;
                                            CurrLevel.Map[x + CurrLevel.Map[x, y].Px, y + CurrLevel.Map[x, y].Py].F = 2;
                                        }
                                    }
                                    CurrLevel.Map[x, y].FGSpriteID = Convert.ToInt32(' ');
                                    CurrLevel.Map[x, y].FGTypeID = Convert.ToInt32(' ');
                                    CurrLevel.Map[x, y].F = 0;
                                    CurrLevel.Map[x, y].X = 0;
                                    CurrLevel.Map[x, y].Y = 0;
                                    CurrLevel.Map[x, y].Px = 0;
                                    CurrLevel.Map[x, y].Py = 0;
                                }
                                //DR_SPR(x * 20 + CurrLevel.Map[x, y].X, y * 20 + CurrLevel.Map[x, y].Y, CurrLevel.Map[x, y].FGSpriteID);
                            }
                            else// EXPLODE/IMPLODE
                            if ((CurrLevel.Map[x, y].F >= 4) && (CurrLevel.Map[x, y].F <= 6))
                            {
                                //DR_SPR(x * 20 + CurrLevel.Map[x, y].X, y * 20 + CurrLevel.Map[x, y].Y, CurrLevel.Map[x, y].FGSpriteID);
                                if ((CurrLevel.Map[x, y].Px == 0) && (CurrLevel.Map[x, y].Py == 0))
                                {
                                    CurrLevel.Map[x, y].Px = 4;
                                    CurrLevel.Map[x, y].Py = 6;
                                }
                                else if (!(--CurrLevel.Map[x, y].Py != 0))
                                {
                                    CurrLevel.Map[x, y].Py = 6;
                                    if ((CurrLevel.Map[x, y].Px == 3) && (CurrLevel.Map[x, y].F < 6))
                                    {
                                        //PlaySample(Snd_Expl);
                                        for (int k = -1; k <= 1; k++)
                                        {
                                            for (int l = -1; l <= 1; l++)
                                            {
                                                if ((k != 0 || l != 0) && (x + k >= 0) &&
                                                    (x + k < CurrLevel.Width) &&
                                                    (y + l >= 0) && (y + l < CurrLevel.Height))
                                                {
                                                    if (((CurrLevel.Map[x + k, y + l].FGTypeID == Convert.ToInt32('o')) ||
                                                        (CurrLevel.Map[x + k, y + l].FGTypeID == Convert.ToInt32('O'))) &&
                                                        ((CurrLevel.Map[x + k, y + l].F < 4) || (CurrLevel.Map[x + k, y + l].F > 6)))
                                                    {
                                                        CurrLevel.Map[x + k, y + l].F = 4 +
                                                            (CurrLevel.Map[x + k, y + l].FGTypeID == Convert.ToInt32('O') ? 1 : 0);
                                                        CurrLevel.Map[x + k, y + l].X += k;
                                                        CurrLevel.Map[x + k, y + l].Y += l;
                                                        CurrLevel.Map[x + k, y + l].Px = 0;
                                                        CurrLevel.Map[x + k, y + l].Py = 0;
                                                    }
                                                    else
                                                    if ((CurrLevel.Map[x + k, y + l].FGTypeID == Convert.ToInt32('x')) ||
                                                        (CurrLevel.Map[x + k, y + l].FGTypeID == Convert.ToInt32('c')))
                                                    {
                                                        //PlaySample(Snd_Fall);
                                                        CurrLevel.Map[x + k, y + l].F = 6;
                                                        CurrLevel.Map[x + k, y + l].X += k;
                                                        CurrLevel.Map[x + k, y + l].Y += l;
                                                        CurrLevel.Map[x + k, y + l].Px = 0;
                                                        CurrLevel.Map[x + k, y + l].Py = 0;
                                                    }
                                                    else
                                                    if ((CurrLevel.Map[x + k, y + l].FGTypeID == Convert.ToInt32(' ')) ||
                                                        (CurrLevel.Map[x + k, y + l].FGTypeID == Convert.ToInt32('B')))
                                                    {
                                                        if (CurrLevel.Map[x + k, y + l].F == 4)
                                                        { }
                                                        else if (CurrLevel.Map[x + k, y + l].F == 5)
                                                        { }
                                                        else
                                                        //if (ALvl.M[I+K][J+L].F==6)
                                                        {
                                                            CurrLevel.Map[x + k, y + l].F = 6 -
                                                                2 * (CurrLevel.Map[x, y].F == 5 ? 1 : 0);
                                                            CurrLevel.Map[x + k, y + l].Px = 0;
                                                            CurrLevel.Map[x + k, y + l].Py = 0;
                                                            CurrLevel.Map[x + k, y + l].X = 0;
                                                            CurrLevel.Map[x + k, y + l].Y = 0;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (CurrLevel.Map[x, y].Px == 2)
                                    {
                                        for (int k = 10; k <= 20; k += CurrLevel.Map[x, y].F)
                                        {
                                            int cx = x * 20 + 7 + rand.Next() % 6;
                                            int cy = y * 20 + 7 + rand.Next() % 6;
                                            g.FillEllipse(new SolidBrush(Color.FromArgb(20 - (k * (rand.Next() % 2)) / 2, 0, 0, 0)),
                                                Rectangle.FromLTRB(cx - k / 2, cy - k / 2, cx + k / 2, cy + k / 2));
                                        }
                                        //DR_RCT(x * 20 - 5, y * 20 - 5, x * 20 + 19 + 5, y * 20 + 19 + 5);
                                    }

                                    CurrLevel.Map[x, y].FGSpriteID = --CurrLevel.Map[x, y].Px;
                                    if (CurrLevel.Map[x, y].FGSpriteID == 0)
                                    {
                                        CurrLevel.Map[x, y].F = 0;
                                        CurrLevel.Map[x, y].FGTypeID = Convert.ToInt32(' ');
                                        CurrLevel.Map[x, y].FGSpriteID = Convert.ToInt32(' ');
                                        CurrLevel.Map[x, y].Px = 0;
                                        CurrLevel.Map[x, y].Py = 0;
                                        CurrLevel.Map[x, y].X = 0;
                                        CurrLevel.Map[x, y].Y = 0;
                                        if (CurrLevel.Map[x, y].BGTypeID == Convert.ToInt32('&'))
                                            CurrLevel.Map[x, y].F = 8;
                                    }
                                }
                                //DR_SPR(x * 20 + CurrLevel.Map[x, y].X, y * 20 + CurrLevel.Map[x, y].Y, CurrLevel.Map[x, y].FGSpriteID);
                            }
                            else// FLAMABLE
                            if (CurrLevel.Map[x, y].F == 8)
                            {
                                //DR_SPR(x * 20, y * 20, CurrLevel.Map[x, y].FGSpriteID);
                                if (CurrLevel.Map[x, y].Px == 0)
                                {
                                    if ((CurrLevel.Map[x, y].FGTypeID == Convert.ToInt32('O')))
                                    {
                                        CurrLevel.Map[x, y].F = 5;
                                        CurrLevel.Map[x, y].Px = 0;
                                        CurrLevel.Map[x, y].Py = 0;
                                    }
                                    else if ((CurrLevel.Map[x, y].FGTypeID == Convert.ToInt32('o')))
                                    {
                                        CurrLevel.Map[x, y].F = 4;
                                        CurrLevel.Map[x, y].Px = 0;
                                        CurrLevel.Map[x, y].Py = 0;
                                    }
                                    else
                                    {
                                        CurrLevel.Map[x, y].FGSpriteID = 6;
                                        CurrLevel.Map[x, y].Px = 40;
                                    }
                                    //PlaySample(Snd_Burn);
                                }
                                else if (--CurrLevel.Map[x, y].Px != 0)
                                {
                                    CurrLevel.Map[x, y].FGSpriteID = 5 + CurrLevel.Map[x, y].Px % 4;
                                    if (CurrLevel.Map[x, y].Px == 20)
                                        for (int k = -1; k <= 1; k++)
                                            for (int l = -1; l <= 1; l++)
                                                if ((x + k >= 0) &&
                                                    (x + k < CurrLevel.Width) &&
                                                    (y + l >= 0) &&
                                                    (y + l < CurrLevel.Height))
                                                    if ((CurrLevel.Map[x + k, y + l].BGTypeID == Convert.ToInt32('&')) &&
                                                        (CurrLevel.Map[x + k, y + l].F != 8) &&
                                                        ((CurrLevel.Map[x + k, y + l].F < 4) ||
                                                        (CurrLevel.Map[x + k, y + l].F > 6)))
                                                    {
                                                        CurrLevel.Map[x + k, y + l].F = 8;
                                                        CurrLevel.Map[x + k, y + l].Px = 0;
                                                    }
                                }
                                else
                                {
                                    CurrLevel.Map[x, y].FGTypeID = Convert.ToInt32(' ');
                                    CurrLevel.Map[x, y].FGSpriteID = Convert.ToInt32(' ');
                                    CurrLevel.Map[x, y].BGTypeID = Convert.ToInt32(' ');
                                    CurrLevel.Map[x, y].BGSpriteID = Convert.ToInt32(' ');
                                    CurrLevel.Map[x, y].F = 0;
                                    CurrLevel.Map[x, y].Px = 0;
                                    CurrLevel.Map[x, y].Py = 0;
                                    //DrawSpr(Spr[4].img, ALvl.Bg, x * 20, y * 20);
                                    g.DrawImageUnscaled(GameRes.SpriteList[4].Image, x * 20, y * 20);
                                    //filledCircleRGBA(ALvl.Bg, x * 20 + 7 + rand.Next() % 6, y * 20 + 7 + rand.Next() % 6, 13, 0, 0, 0, 64);
                                    int cx = x * 20 + 7 + rand.Next() % 6;
                                    int cy = y * 20 + 7 + rand.Next() % 6;
                                    g.FillEllipse(new SolidBrush(Color.FromArgb(64, 0, 0, 0)),
                                        Rectangle.FromLTRB(cx - 13 / 2, cy - 13 / 2, cx + 13 / 2, cy + 13 / 2));
                                    //DR_RCT(x * 20 - 3, y * 20 - 3, x * 20 + 19 + 3, y * 20 + 19 + 3);
                                }
                                //DR_SPR(x * 20, y * 20, CurrLevel.Map[x, y].FGSpriteID);
                                //					fprintf(stderr,"at(%d,%d)-t%d-pxy(%d,%d)-xy(%d,%d)\n",I,J,ALvl.M[I][J].F,ALvl.M[I][J].Px,ALvl.M[I][J].Py,ALvl.M[I][J].X,ALvl.M[I][J].Y);
                            }
                            else// PARTICLES
                            if (CurrLevel.Map[x, y].F == 9)
                            {
                                //DR_SPR(x * 20, y * 20, CurrLevel.Map[x, y].FGSpriteID);
                                if ((CurrLevel.Map[x, y].Py -= 1) == 0)
                                {
                                    if ((CurrLevel.Map[x, y].Px -= 1) == 0)
                                    {
                                        CurrLevel.Map[x, y].FGSpriteID = Convert.ToInt32(' ');
                                        CurrLevel.Map[x, y].F = 0;
                                    }
                                    else
                                    {
                                        CurrLevel.Map[x, y].FGSpriteID--;
                                        CurrLevel.Map[x, y].Py = 3;
                                    }
                                }
                                //DR_SPR(x * 20, y * 20, CurrLevel.Map[x, y].FGSpriteID);
                            }
                            else// BEM
                            if (CurrLevel.Map[x, y].F == 10)
                            {
                                BEM2 = true;
                                if ((rand.Next() % 50) == 0)
                                {
                                    //DR_SPR(x * 20, y * 20, CurrLevel.Map[x, y].FGSpriteID);
                                    CurrLevel.Map[x, y].FGSpriteID = Convert.ToInt32('B') + rand.Next() % 4;
                                    //DR_SPR(x * 20, y * 20, CurrLevel.Map[x, y].FGSpriteID);
                                }
                            }
                            else
                            {// WEIRD!!!

                            }
                        }
                    }
                }

                if (this.HasBem && (!BEM2))
                {
                    for (int x = 0; x < CurrLevel.Width; x++)
                    {
                        for (int y = 0; y < CurrLevel.Height; y++)
                        {
                            if (CurrLevel.Map[x, y].BGTypeID == Convert.ToInt32('+'))
                            {
                                g.DrawImageUnscaled(GameRes.SpriteList[Convert.ToInt32('+')].Image, x * 20 + CurrLevel.Map[x, y].X, y * 20 + CurrLevel.Map[x, y].Y);
                            }
                        }
                    }
                    this.HasBem = BEM2;
                }
            }
        }

        public Image RenderGame()
        {
            Image frame = new Bitmap(CurrLevel.Width * 20, CurrLevel.Height * 20);

            using (Graphics g = Graphics.FromImage(frame))
            {
                g.Clear(Color.FromArgb(255, 128, 128, 128));
                if (CurrLevel.BackGroundImage != null)
                {
                    g.DrawImageUnscaled(CurrLevel.BackGroundImage, 0, 0);
                }

                Image ShadowLayer = new Bitmap(CurrLevel.Width * 20, CurrLevel.Height * 20);
                using (Graphics gs = Graphics.FromImage(ShadowLayer))
                {
                    Brush bb = new SolidBrush(Color.Black);
                    gs.FillRectangle(bb, 0, 0, CurrLevel.Width * 20, 15);
                    gs.FillRectangle(bb, 0, 0, 15, CurrLevel.Height * 20);
                    for (int x = 0; x < 20; x++)
                    {
                        for (int y = 0; y < 20; y++)
                        {
                            int fid = CurrLevel.Map[x, y].FGSpriteID;
                            if (fid != 32 && GameRes.SpriteList[fid].Shade != null)
                            {
                                gs.DrawImageUnscaled(GameRes.SpriteList[fid].Shade, x * 20 + CurrLevel.Map[x, y].X, y * 20 + CurrLevel.Map[x, y].Y);
                            }
                        }
                    }
                    gs.DrawImageUnscaled(GameRes.PlayerShade, Player.X * 20 + Player.Px, Player.Y * 20 + Player.Py);
                }

                ImageAttributes attr = new ImageAttributes();
                Point[] pointArray = {
                    new Point(0,0),
                    new Point(CurrLevel.Width * 20,0),
                    new Point(0,CurrLevel.Height * 20)};
                float[][] ptsArray = {
                    new float[] {1, 0, 0, 0, 0},
                    new float[] {0, 1, 0, 0, 0},
                    new float[] {0, 0, 1, 0, 0},
                    new float[] {0, 0, 0, 0.5f, 0},
                    new float[] {0, 0, 0, 0, 1}};
                ColorMatrix clrMatrix = new ColorMatrix(ptsArray);
                attr.SetColorMatrix(clrMatrix);
                g.DrawImage(ShadowLayer, pointArray, Rectangle.FromLTRB(0, 0, CurrLevel.Width * 20, CurrLevel.Height * 20), GraphicsUnit.Pixel, attr);

                for (int x = 0; x < 20; x++)
                {
                    for (int y = 0; y < 20; y++)
                    {
                        int fid = CurrLevel.Map[x, y].FGSpriteID;
                        if (fid != 32 && GameRes.SpriteList[fid].Image != null)
                        {
                            g.DrawImageUnscaled(GameRes.SpriteList[fid].Image, x * 20 + CurrLevel.Map[x, y].X, y * 20 + CurrLevel.Map[x, y].Y);
                        }
                    }
                }

                if ((Player.Alive == DeadDelay) || ((Player.Alive > DeadDelay) && (Player.Alive % 2 > 0)))
                {
                    int LocX = 0;
                    int LocY = 0;
                    GetPlayerImageLocation(out LocX, out LocY);

                    if (Player.Py == 0)
                    {
                        g.DrawImage(GameRes.PlayerHLeg,
                            Rectangle.FromLTRB(Player.X * 20 + Player.Px, Player.Y * 20 + Player.Py, Player.X * 20 + Player.Px + 20, Player.Y * 20 + Player.Py + 20),
                            Rectangle.FromLTRB(LocX, LocY, LocX + 20, LocY + 20),
                            GraphicsUnit.Pixel);
                    }
                    else if (Player.Px == 0)
                    {
                        g.DrawImage(GameRes.PlayerVLeg,
                            Rectangle.FromLTRB(Player.X * 20 + Player.Px, Player.Y * 20 + Player.Py, Player.X * 20 + Player.Px + 20, Player.Y * 20 + Player.Py + 20),
                            Rectangle.FromLTRB(LocX, LocY, LocX + 20, LocY + 20),
                            GraphicsUnit.Pixel);
                    }

                    if (IsShift(Player.X + Math.Sign(Player.Px), Player.Y + Math.Sign(Player.Py)))
                    {
                        if (Player.Py == 0)
                        {
                            g.DrawImage(GameRes.PlayerHPush,
                                Rectangle.FromLTRB(Player.X * 20 + Player.Px, Player.Y * 20 + Player.Py, Player.X * 20 + Player.Px + 20, Player.Y * 20 + Player.Py + 20),
                                Rectangle.FromLTRB(LocX, LocY, LocX + 20, LocY + 20),
                                GraphicsUnit.Pixel);
                        }
                        else if (Player.Px == 0)
                        {
                            g.DrawImage(GameRes.PlayerVPush,
                                Rectangle.FromLTRB(Player.X * 20 + Player.Px, Player.Y * 20 + Player.Py, Player.X * 20 + Player.Px + 20, Player.Y * 20 + Player.Py + 20),
                                Rectangle.FromLTRB(LocX, LocY, LocX + 20, LocY + 20),
                                GraphicsUnit.Pixel);
                        }
                    }
                    else
                    {
                        if (Player.Py == 0)
                        {
                            g.DrawImage(GameRes.PlayerHBody,
                                Rectangle.FromLTRB(Player.X * 20 + Player.Px, Player.Y * 20 + Player.Py, Player.X * 20 + Player.Px + 20, Player.Y * 20 + Player.Py + 20),
                                Rectangle.FromLTRB(LocX, LocY, LocX + 20, LocY + 20),
                                GraphicsUnit.Pixel);
                        }
                        else if (Player.Px == 0)
                        {
                            g.DrawImage(GameRes.PlayerVBody,
                                Rectangle.FromLTRB(Player.X * 20 + Player.Px, Player.Y * 20 + Player.Py, Player.X * 20 + Player.Px + 20, Player.Y * 20 + Player.Py + 20),
                                Rectangle.FromLTRB(LocX, LocY, LocX + 20, LocY + 20),
                                GraphicsUnit.Pixel);
                        }
                    }
                }
                else if (Player.Alive < DeadDelay)
                {
                    g.DrawImageUnscaled(GameRes.SpriteList[15 - Player.Alive / (DeadDelay / 4)].Image, Player.X * 20 + Player.Px, Player.Y * 20 + Player.Py);
                    //DrawSpr(Spr[15 - PAlive / (DeadDelay / 4)].img, screen, PX * 20 + PPX, PY * 20 + PPY);
                }
            }
            return frame;
        }

        public void GetPlayerImageLocation(out int ox, out int oy)
        {
            ox = 0;
            oy = 0;
            int px = Player.Px;
            int py = Player.Py;
            if (py == 0)
            {
                if (px < 0)
                {
                    oy = 1;
                    px = -px;
                }

                if (px <= 5)
                {
                    ox = px + 5;
                }
                else if (px <= 15)
                {
                    ox = 15 - px;
                }
                else
                {
                    ox = px - 15;
                }
            }
            else if (px == 0)
            {
                if (py < 0)
                {
                    ox = 0;
                    py = -py;
                }
                else
                {
                    ox = 1;
                }

                if (py <= 5)
                {
                    oy = py + 5;
                }
                else if (py <= 15)
                {
                    oy = 15 - py;
                }
                else
                {
                    oy = py - 15;
                }
            }
            ox = ox * 40;
            oy = oy * 40;
        }

        public bool LoadLevel()
        {
            try
            {
                this.LevelList.Clear();
                FileInfo fi = new FileInfo(this.LevelFileName);

                TextReader tr = File.OpenText(this.LevelFileName);
                String Line = "";

                if (fi.Extension == ".lvl")
                {
                    Level currLevel = null;
                    int currLevelID = 1;
                    int i = 0;
                    while ((Line = tr.ReadLine()) != null)
                    {
                        if (Line == String.Empty)
                        {

                        }
                        else if (Line[0] == '#')
                        {

                        }
                        else if (Line[0] == '>' && Line[Line.Length - 1] == '-')
                        {
                            if (currLevel != null)
                            {
                                this.LevelList.Add(currLevel);
                            }
                            currLevel = new Level();
                            currLevelID++;
                            currLevel.ID = currLevelID;
                            currLevel.Map = new Element[20, 20];
                            currLevel.Width = 20;
                            currLevel.Height = 20;
                            currLevel.Text = new String[10];
                            i = 0;
                        }
                        else if (Line[1] == '=')
                        {
                            currLevel.Text[Convert.ToInt32(Line[0]) - 48] = Line.Substring(2);
                        }
                        else if (i < 20 && Line.Length >= 20 * 7 && Line[0] == ',')
                        {
                            for (int j = 0; j < 20 * 7; j += 7)
                            {
                                currLevel.Map[j / 7, i] = new Element();
                                currLevel.Map[j / 7, i].FGSpriteID = Convert.ToInt32(Line[j + 1]);
                                currLevel.Map[j / 7, i].FGTypeID = Convert.ToInt32(Line[j + 2]);
                                currLevel.Map[j / 7, i].BGSpriteID = Convert.ToInt32(Line[j + 3]);
                                currLevel.Map[j / 7, i].BGTypeID = Convert.ToInt32(Line[j + 4]);
                                currLevel.Map[j / 7, i].F = Convert.ToInt32(Line[j + 5]);
                                currLevel.Map[j / 7, i].TextID = Convert.ToInt32(Line[j + 6]);
                            }
                            i++;
                        }
                        else if (Line.Substring(0, 5) == "name=")
                        {
                            currLevel.Name = Line.Substring(5);
                        }
                        else if (Line.Substring(0, 9) == "password=")
                        {
                            currLevel.Password = Line.Substring(9);
                        }
                        else if (Line.Substring(0, 9) == "deadline=")
                        {
                            currLevel.TimeLimit = Convert.ToInt32(Line.Substring(9));
                        }
                        else if (Line.Substring(0, 11) == "background=")
                        {
                            currLevel.BackGroundFileName = Line.Substring(11);
                        }
                        else
                        {

                        }
                    }
                    this.LevelList.Add(currLevel);
                }

                tr.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
