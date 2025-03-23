using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finns_i_Monogame;

namespace Fan_igen
{
    public class Spelare
    {
        private List<Kortvisuel> Hand = new List<Kortvisuel>();
        private string VadHeterDu;
        private int VemÄrDu;
        private int KordinatX;
        private int KordinatY;
        private int PositionAvKortDuVäljer=0;
        private int PositionAvSpelareDuVäljer=0;
        

        public Spelare(string namn,int a,int x,int y){
            VadHeterDu=namn;
            VemÄrDu = a;
            KordinatX=x;
            KordinatY=y;
        }
        public string namn{get=>VadHeterDu;}
        public int posiSpelare{set=>PositionAvSpelareDuVäljer=value;get=>PositionAvSpelareDuVäljer;}
        public int PosiAvDittKort{set=>PositionAvKortDuVäljer=value;get=>PositionAvKortDuVäljer;}
        public int Kordinatx{get=>KordinatX;}
        public int Kordinaty{get=>KordinatY;}
        public List<Kortvisuel> hand{
            set{Hand = value;}
            get{return Hand;}
        }
    }
}