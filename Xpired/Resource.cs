using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Xpired
{
    public class Resource
    {
        public Dictionary<int, Sprite> SpriteList = new Dictionary<int, Sprite>();
        public Sprite Level;
        public Sprite Lives;
        public Sprite Time;
        public Sprite Numbers;
        
        public Image PlayerVLeg;
        public Image PlayerVBody;
        public Image PlayerVPush;

        public Image PlayerHLeg;
        public Image PlayerHBody;
        public Image PlayerHPush;

        public Image PlayerShade;

        public int ShadeAlpha = 0;

        public void Init()
        {
            Level = new Sprite(LoadSprite(@".\img\level.bmp", 0), null);
            Lives = new Sprite(LoadSprite(@".\img\lives.bmp", 0), null);
            Time = new Sprite(LoadSprite(@".\img\time.bmp", 0), null);
            Numbers = new Sprite(LoadSprite(@".\img\numbers.bmp", 0), null);

            SpriteList.Add(Convert.ToInt32('*'),new Sprite(
                LoadSprite(@".\img\start.bmp", 128),
                LoadSprite(@".\img\shade-player.bmp", ShadeAlpha)));
            SpriteList.Add(Convert.ToInt32('+'),new Sprite(
                LoadSprite(@".\img\exit.bmp", 128),
                LoadSprite(@".\img\exit-denied.bmp", 128)));

            SpriteList.Add(Convert.ToInt32('%'),new Sprite(
                LoadSprite(@".\img\retard.bmp", 140), null));//128
            SpriteList.Add(Convert.ToInt32('#'),new Sprite(
                LoadSprite(@".\img\ice.bmp", 140), null));
            SpriteList.Add(Convert.ToInt32('&'),new Sprite(
                LoadSprite(@".\img\flamable.bmp", 255 - 150), null));
            SpriteList.Add(Convert.ToInt32('@'),new Sprite(
                LoadSprite(@".\img\hot.bmp", 255 - 200), null));
            SpriteList.Add(Convert.ToInt32('T'),new Sprite(
                LoadSprite(@".\img\teleport.bmp", 255 - 200), null));

            SpriteList.Add(Convert.ToInt32('X'),new Sprite(
                LoadSprite(@".\img\wall.bmp", 0),
                LoadSprite(@".\img\shade-square.bmp", ShadeAlpha)));
            SpriteList.Add(Convert.ToInt32('x'),new Sprite(
                LoadSprite(@".\img\tinywall.bmp", 0),
                LoadSprite(@".\img\shade-square.bmp", ShadeAlpha)));
            SpriteList.Add(Convert.ToInt32('c'),new Sprite(
                LoadSprite(@".\img\crate.bmp", 0),
                LoadSprite(@".\img\shade-crate.bmp", ShadeAlpha)));
            SpriteList.Add(Convert.ToInt32('o'),new Sprite(
                LoadSprite(@".\img\barel.bmp", 0),
                LoadSprite(@".\img\shade-round.bmp", ShadeAlpha)));
            SpriteList.Add(Convert.ToInt32('O'),new Sprite(
                LoadSprite(@".\img\explosive.bmp", 0),
                LoadSprite(@".\img\shade-round.bmp", ShadeAlpha)));

            SpriteList.Add(1, new Sprite(
                LoadSprite(@".\img\expl3.bmp", 0), null));
            SpriteList.Add(2, new Sprite(
                LoadSprite(@".\img\expl2.bmp", 0), null));
            SpriteList.Add(3, new Sprite(
                LoadSprite(@".\img\expl1.bmp", 0),
                LoadSprite(@".\img\shade-expl1.bmp", ShadeAlpha)));

            SpriteList.Add(4, new Sprite(
                LoadSprite(@".\img\bo-flamable.bmp", 0), null));

            SpriteList.Add(5, new Sprite(
                LoadSprite(@".\img\fire1.bmp", 0), null));
            SpriteList.Add(6, new Sprite(
                LoadSprite(@".\img\fire2.bmp", 0), null));
            SpriteList.Add(7, new Sprite(
                LoadSprite(@".\img\fire3.bmp", 0), null));
            SpriteList.Add(8, new Sprite(
                LoadSprite(@".\img\fire2.bmp", 0), null));
            
            SpriteList.Add(9, new Sprite(
                LoadSprite(@".\img\telee3.bmp", 0),
                LoadSprite(@".\img\shade-t3.bmp", ShadeAlpha)));
            SpriteList.Add(10, new Sprite(
                LoadSprite(@".\img\telee2.bmp", 0),
                LoadSprite(@".\img\shade-t2.bmp", ShadeAlpha)));
            SpriteList.Add(11, new Sprite(
                LoadSprite(@".\img\telee1.bmp", 0),
                LoadSprite(@".\img\shade-t1.bmp", ShadeAlpha)));

            SpriteList.Add(12, new Sprite(
                LoadSprite(@".\img\player-death1.bmp", 0), null));
            SpriteList.Add(13, new Sprite(
                LoadSprite(@".\img\player-death2.bmp", 0), null));
            SpriteList.Add(14, new Sprite(
                LoadSprite(@".\img\player-death3.bmp", 0), null));
            SpriteList.Add(15, new Sprite(
                LoadSprite(@".\img\player-death4.bmp", 0), null));
            
            SpriteList.Add(Convert.ToInt32('B'), new Sprite(
                LoadSprite(@".\img\bem1.bmp", 0),
                LoadSprite(@".\img\shade-bem1.bmp", ShadeAlpha)));
            SpriteList.Add(Convert.ToInt32('C'), new Sprite(
                LoadSprite(@".\img\bem2.bmp", 0),
                LoadSprite(@".\img\shade-bem2.bmp", ShadeAlpha)));
            SpriteList.Add(Convert.ToInt32('D'), new Sprite(
                LoadSprite(@".\img\bem3.bmp", 0),
                LoadSprite(@".\img\shade-bem3.bmp", ShadeAlpha)));
            SpriteList.Add(Convert.ToInt32('E'), new Sprite(
                LoadSprite(@".\img\bem4.bmp", 0),
                LoadSprite(@".\img\shade-bem4.bmp", ShadeAlpha)));

            PlayerVLeg = LoadSprite(@".\img\player-v-legs.bmp", 0);
            PlayerVBody = LoadSprite(@".\img\player-v-body.bmp", 0);
            PlayerVPush = LoadSprite(@".\img\player-v-push.bmp", 0);

            PlayerHLeg = LoadSprite(@".\img\player-h-legs.bmp", 0);
            PlayerHBody = LoadSprite(@".\img\player-h-body.bmp", 0);
            PlayerHPush = LoadSprite(@".\img\player-h-push.bmp", 0);

            PlayerShade = LoadSprite(@".\img\shade-player.bmp", ShadeAlpha);
        }

        public Image LoadTiledBackground(String FileName, int DestWidth, int DestHeight)
        {
            Image output = new Bitmap(DestWidth, DestHeight);

            if (FileName != null)
            {
                Image original = Image.FromFile(FileName);

                using (TextureBrush tbr = new TextureBrush(original))
                {
                    tbr.WrapMode = WrapMode.Tile;
                    using (Graphics g = Graphics.FromImage(output))
                    {
                        g.FillRectangle(tbr, Rectangle.FromLTRB(0, 0, DestWidth, DestHeight));
                    }
                }
            }
            else
            {
                using (Graphics g = Graphics.FromImage(output))
                {
                    g.Clear(Color.FromArgb(255, 96, 96, 96));
                }
            }
            return output;
        }

        public Image LoadSprite(String FileName)
        {
            Image output = Image.FromFile(FileName);
            return output;
        }

        public Image LoadSprite(String FileName, int Alpha)
        {
            return LoadSprite(FileName, Color.FromArgb(0, 255, 0, 255), (255.0f - Alpha) / 255.0f);
        }

        public Image LoadSprite(String FileName, Color ColorKey, float Alpha)
        {
            Image original = Image.FromFile(FileName);
            Image output = new Bitmap(original.Width, original.Height);

            using (Graphics g = Graphics.FromImage(output))
            {
                ImageAttributes attr = new ImageAttributes();
                Point[] pointArray = {
                    new Point(0,0),
                    new Point(original.Width,0),
                    new Point(0,original.Height)};
                float[][] ptsArray = {
                    new float[] {1, 0, 0, 0, 0},
                    new float[] {0, 1, 0, 0, 0},
                    new float[] {0, 0, 1, 0, 0},
                    new float[] {0, 0, 0, Alpha, 0}, //注意：此处为0.5f，图像为半透明
                    new float[] {0, 0, 0, 0, 1}};
                ColorMatrix clrMatrix = new ColorMatrix(ptsArray);
                attr.SetColorKey(ColorKey, ColorKey);
                attr.SetColorMatrix(clrMatrix);
                g.DrawImage(original, pointArray, Rectangle.FromLTRB(0, 0, original.Width, original.Height), GraphicsUnit.Pixel, attr);
            }

            return output;
        }
    }
}
