using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace Xpired
{
    public class Level
    {
        public Element[,] Map;
        public int Width;
        public int Height;

        public String Name;
        public int ID;
        public String Password;

        public int TimeLimit = -1;

        public String BackGroundFileName;
        public Image BackGroundImage;

        public String[] Text;

        public Level()
        {

        }

        public Level(Level SrcLv)
        {
            this.Width = SrcLv.Width;
            this.Height = SrcLv.Height;
            this.Map = new Element[this.Width, this.Height];
            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    this.Map[x, y] = new Element(SrcLv.Map[x, y]);
                }
            }
            this.Name = SrcLv.Name;
            this.ID = SrcLv.ID;
            this.Password = SrcLv.Password;
            this.TimeLimit = SrcLv.TimeLimit;
            this.BackGroundFileName = SrcLv.BackGroundFileName;
            this.BackGroundImage = null;
            this.Text = new String[SrcLv.Text.Length];
            for (int i = 0; i < SrcLv.Text.Length; i++)
            {
                this.Text[i] = SrcLv.Text[i];
            }
        }
    }
}
