using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;

namespace Xpired
{
    public partial class MainFrm : Form
    {
        Game Game;
        Thread GameThread;

        public MainFrm()
        {
            InitializeComponent();
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            this.Game = new Game(this.pictureBox1);
            Game.GameForm = this;
            Game.FpsLabel = this.toolStripStatusLabel1;
            Game.LevelFileName = "xpired.lvl";
            Game.LoadLevel();

            this.GameThread = new Thread(Game.GameMain);

            Game.GameRes.Init();
        }

        private void MainFrm_Shown(object sender, EventArgs e)
        {
            this.GameThread.Start();
        }

        private void MainFrm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control) Game.KeyState.Ctrl = true;
            if (e.KeyCode == Keys.Up) Game.KeyState.Up = true;
            if (e.KeyCode == Keys.Down) Game.KeyState.Down = true;
            if (e.KeyCode == Keys.Left) Game.KeyState.Left = true;
            if (e.KeyCode == Keys.Right) Game.KeyState.Right = true;
            if (e.KeyCode == Keys.Escape) Game.KeyState.Escape = true;
        }

        private void MainFrm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control) Game.KeyState.Ctrl = false;
            if (e.KeyCode == Keys.Up) Game.KeyState.Up = false;
            if (e.KeyCode == Keys.Down) Game.KeyState.Down = false;
            if (e.KeyCode == Keys.Left) Game.KeyState.Left = false;
            if (e.KeyCode == Keys.Right) Game.KeyState.Right = false;
            if (e.KeyCode == Keys.Escape) Game.KeyState.Escape = false;
        }

        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.GameThread.Abort();
        }
    }
}
