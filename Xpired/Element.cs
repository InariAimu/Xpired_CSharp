using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace Xpired
{
    public class Element
    {
        public int FGSpriteID;
        public int FGTypeID;

        public int BGSpriteID;
        public int BGTypeID;

        public int X;
        public int Y;
        public int Px;
        public int Py;
        public int F;

        public int TextID;

        public Element()
        {

        }

        public Element(Element SrcE)
        {
            FGSpriteID = SrcE.FGSpriteID;
            FGTypeID = SrcE.FGTypeID;

            BGSpriteID = SrcE.BGSpriteID;
            BGTypeID = SrcE.BGTypeID;

            X = SrcE.X;
            Y = SrcE.Y;
            Px = SrcE.Px;
            Py = SrcE.Py;
            F = SrcE.F;

            TextID = SrcE.TextID;
        }
    }
}
