using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace Xpired
{
    public class Sprite
    {
        public Image Image;
        public Image Shade;

        public Sprite(Image image,Image shade)
        {
            Image = image;
            Shade = shade;
        }
    }
}
