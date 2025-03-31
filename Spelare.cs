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
        private int Poäng;
        private int KordinatX;
        private int KordinatY;
        private int PositionAvKortDuVäljer=0;
        private int PositionAvSpelareDuVäljer=0;


        List<List<string>> VadHarDeAndraSpelat = new List<List<string>>{ new List<string>(), new List<string>(),new List<string>(),new List<string>()};
        

        public Spelare(string namn,int a,int x,int y){
            VadHeterDu=namn;
            VemÄrDu = a;
            KordinatX=x;
            KordinatY=y;
        }

        public List<List<string>> vadhardeandraspelat{set=>VadHarDeAndraSpelat=value; get=> VadHarDeAndraSpelat;}
        public int vemärdu{get=>VemÄrDu;}
        public string namn{get=>VadHeterDu;}
        public int poäng{set=>Poäng=value;get=>Poäng;}
        public int posiSpelare{set=>PositionAvSpelareDuVäljer=value;get=>PositionAvSpelareDuVäljer;}
        public int PosiAvDittKort{set=>PositionAvKortDuVäljer=value;get=>PositionAvKortDuVäljer;}
        public int Kordinatx{get=>KordinatX;}
        public int Kordinaty{get=>KordinatY;}
        public List<Kortvisuel> hand{
            set{Hand = value;}
            get{return Hand;}
        }

        public List<int> FindPositionOfAll(string kort){
            List<int> a = new List<int>();
            for(int i=0;i<Hand.Count;i++){
                if(Hand[i].Kort==kort){
                    a.Add(i);
                }
            }
            return a;
        }
        public int FindAllCount(string kort){
            int antal = 0;
            foreach(Kortvisuel k in Hand){
                if(k.Kort==kort){
                    antal++;
                }
            }
            return antal;
        }
        public bool contains(string kort){
            foreach(Kortvisuel k in Hand){
                if(k.Kort==kort){
                    return true;
                }
            }
            return false;
        }
    }
}