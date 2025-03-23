using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SharpDX.MediaFoundation;

namespace Finns_i_Monogame
{
    public class Kortvisuel
    {
        private Rectangle Vitrektangle;
        private Rectangle Rödrektangle;
        private string Kortet = "";
        private int tjock = 100;
        private int längd = 180;
        private int aniX;
        private int aniY;

        public Kortvisuel(int X, int Y,string Kort){ // anväds när man skapar kort.
            Vitrektangle = new Rectangle(X,Y,tjock,längd);
            Rödrektangle = new Rectangle(X-5,Y-5,tjock+10,längd+10);
            Kortet = Kort;
        }
        public Rectangle vitrektangle{ // används för att rita den vita. alla saker man gör med rec funkar också.
            set{Vitrektangle = value;}
            get{return Vitrektangle;}
        }
        public Rectangle rödrektangle{ // samma med denna.
            set{Rödrektangle = value;}
            get{return Rödrektangle;}
        }
        public int FlyttaX{
            set{
                Vitrektangle.X = value;
                Rödrektangle.X = value-5;
            }
        }
        public int FlyttaY{
            set{
                Vitrektangle.Y = value;
                Rödrektangle.Y = value-5;
            }
        }
        public int ÄndraLängd{
            set{
                Vitrektangle.Height = value;
                Rödrektangle.Height = value+10;
                längd=value;
            }
            get{return längd;}
        }
        public int ÄndraTjock{
            set{
                Vitrektangle.Width = value;
                Rödrektangle.Width = value+10;
                tjock=value;
            }
            get{return tjock;}
        }
        public string Kort{
            set{Kortet = value;}
            get{return Kortet;}
        }
        public int anix{set=>aniX=value;get=>aniX;}
        public int aniy{set=>aniY=value;get=>aniY;}

    }
}