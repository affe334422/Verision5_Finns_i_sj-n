using System;
using System.Collections.Generic;
using System.Xml;
using Fan_igen;
using Finns_i_Monogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Verision5_Finns_i_sjon;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D texture;
    private SpriteFont font1;
    private SpriteFont font2;
    private KeyboardState kstate;
    private List<Spelare> sl = new List<Spelare>{new Spelare("DU",0,875,900),new Spelare("Spelare 1",1,110,450),new Spelare("Spelare 2",2,875,50),new Spelare("Spelare 3",3,1600,450)};
    private bool Start = true;
    private List<string> Kortlek = new List<string>{
        "Ess", "Ess", "Ess", "Ess", "2", "2", "2", "2", "3", "3", "3", "3",
        "4", "4", "4", "4", "5", "5", "5", "5", "6", "6", "6", "6",
        "7", "7", "7", "7", "8", "8", "8", "8", "9", "9", "9", "9",
        "10", "10", "10", "10", "Knäkt", "Knäkt", "Knäkt", "Knäkt",
        "Dam", "Dam", "Dam", "Dam", "Kung", "Kung", "Kung", "Kung"
    };
    List<Kortvisuel> Sjön = new List<Kortvisuel>();
    Random ran = new Random();
    bool Space=true;

    Spelfas fas;
    enum Spelfas{
        StartAnimation,
        DuVäljerKort,
        DuVäljerSpelare,
        TaKortFrånBot,
        annat,
        
    }

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferHeight = 1000;
        _graphics.PreferredBackBufferWidth = 1800;
    }
    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        font1 = Content.Load<SpriteFont>("myFont1");
        font2 = Content.Load<SpriteFont>("myFont2");
        // Skapa en 1x1 vit textur
        texture = new Texture2D(GraphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        kstate = Keyboard.GetState();
        if(Start){
            Start=false;
            for(int i=0;i<Kortlek.Count;i++){
                Sjön.Add(new Kortvisuel(ran.Next(500,1251),ran.Next(200,601),Kortlek[i]));
            }
            foreach(Kortvisuel k in Sjön){
                k.ÄndraLängd = 80;
                k.ÄndraTjock = 50;
            }
            foreach(Spelare s in sl){ // lägger till kort i deras händer.
                for(int i=0;i<7;i++){
                    int b = ran.Next(0,Sjön.Count);
                    s.hand.Add(Sjön[b]);
                    Sjön.RemoveAt(b);
                }
            }
            fas = Spelfas.StartAnimation;
        }
        if(fas==Spelfas.StartAnimation){
            int[] VilkaÄrFärdiga = {0,0,0,0};
            for(int i=0;i<4;i++){
                VilkaÄrFärdiga[i]=Animation(sl,i);
            }
            if(HarAllaKortPåRättPlats(sl,VilkaÄrFärdiga)){
                sl[0].hand=SåDetSerBraUt(sl,0);
                fas=Spelfas.DuVäljerKort;
            }
        }
        if(fas==Spelfas.DuVäljerKort){
            bool KanDuKöra = true;
            if(sl[0].hand.Count<1&&Sjön.Count<1){ // om både handen och sjön är tom så kan du inte göra något
                fas=Spelfas.annat;
                KanDuKöra=false;
            }else if(sl[0].hand.Count<1){ // om bara handen är tom så plockar du ett kort och körviare.
                int kort=ran.Next(0,Sjön.Count);
                sl[0].hand.Add(Sjön[kort]);
                Sjön.RemoveAt(kort);
            }
            if(KanDuKöra&&MovmentAvKort(sl[0],ref Space)){
                fas=Spelfas.DuVäljerSpelare;
            }
        }
        if(fas==Spelfas.DuVäljerSpelare&&MovmentAvSpelare(sl[0],ref Space)){
            fas=Spelfas.TaKortFrånBot;
        }
        if(fas==Spelfas.TaKortFrånBot){
            if(sl[sl[0].posiSpelare].contains(sl[0].hand[sl[0].PosiAvDittKort].Kort)){
                RemoveKort(sl[0],sl[sl[0].posiSpelare]);
                fas=Spelfas.DuVäljerKort;
            }else{
                fas=Spelfas.annat;
            }
        }
        if(fas==Spelfas.annat){Console.WriteLine("");}
        




        if(kstate.IsKeyUp(Keys.Left)&&kstate.IsKeyUp(Keys.Right)&&kstate.IsKeyUp(Keys.Space)){
            Space=true;
        }
        if (kstate.IsKeyDown(Keys.Escape)){
            Exit();
        }
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.SeaGreen);
        _spriteBatch.Begin();

            foreach(Kortvisuel k in Sjön){ // Ritar sjön.
                _spriteBatch.Draw(texture,k.rödrektangle,Color.Black);
                _spriteBatch.Draw(texture,k.vitrektangle,Color.Red);
            }
            if(fas==Spelfas.DuVäljerSpelare){
                _spriteBatch.Draw(texture,new Rectangle(sl[sl[0].posiSpelare].Kordinatx-5,sl[sl[0].posiSpelare].Kordinaty-5,160,100),Color.Red);
            }
            if(fas==Spelfas.StartAnimation){
                foreach(Spelare s in sl){
                    foreach(Kortvisuel k in s.hand){
                        _spriteBatch.Draw(texture,k.rödrektangle,Color.Black);
                        _spriteBatch.Draw(texture,k.vitrektangle,Color.Red);
                    }
                }
            }else{
                foreach(Kortvisuel Kort in sl[0].hand){ // ritar din hand.
                    _spriteBatch.Draw(texture,Kort.rödrektangle,Color.Red);
                    _spriteBatch.Draw(texture,Kort.vitrektangle,Color.White);
                    _spriteBatch.DrawString(font1,Kort.Kort,new Vector2(Kort.vitrektangle.X+10,Kort.vitrektangle.Y+10),Color.Black);
                }
            }
            for(int i=1;i<4;i++){
                _spriteBatch.DrawString(font1,sl[i].namn,new Vector2(sl[i].Kordinatx,sl[i].Kordinaty),Color.Black);
                _spriteBatch.DrawString(font1,sl[i].hand.Count+"",new Vector2(sl[i].Kordinatx+50,sl[i].Kordinaty+35),Color.Black);
            }
            

        _spriteBatch.End();
        

        base.Draw(gameTime);
    }

    void RemoveKort(Spelare s1, Spelare s2){
        List<int> varärkort = new List<int>();
        for(int i=0;i<s2.hand.Count;i++){
            if(s2.hand[i].Kort==s1.hand[s1.PosiAvDittKort].Kort){
                varärkort.Add(i);
            }
        }
        for(int i=varärkort.Count-1;i>-1;i--){
            s1.hand.Add(s2.hand[i]);
            s2.hand.RemoveAt(i);
        }
    }
    static bool MovmentAvSpelare(Spelare s, ref bool kör){
        KeyboardState kstate = Keyboard.GetState();
        if(s.posiSpelare<1){
            s.posiSpelare=3;
        }else if(s.posiSpelare>3){
            s.posiSpelare=1;
        }
        if(kstate.IsKeyDown(Keys.Left)&&kör){
            kör=false;
            s.posiSpelare--;
            if(s.posiSpelare<1){
                s.posiSpelare=3;
            }
        }
        if(kstate.IsKeyDown(Keys.Right)&&kör){
            kör=false;
            s.posiSpelare++;
            if(s.posiSpelare>3){
                s.posiSpelare=1;
            }
        }
        if(kstate.IsKeyDown(Keys.Space)&&kör){
            kör=false;
            return true;
        }
        return false;
    }
    static bool MovmentAvKort(Spelare s,ref bool kör){
        KeyboardState kstate = Keyboard.GetState();
        if(s.PosiAvDittKort<0){
            s.PosiAvDittKort=s.hand.Count-1;
        }else if(s.PosiAvDittKort>s.hand.Count-1){
            s.PosiAvDittKort=0;
        }
        if(kstate.IsKeyDown(Keys.Left)&&kör){
            kör=false;
            s.PosiAvDittKort--;
            if(s.PosiAvDittKort<0){
                s.PosiAvDittKort=s.hand.Count-1;
            }
        }
        if(kstate.IsKeyDown(Keys.Right)&&kör){
            kör=false;
            s.PosiAvDittKort++;
            if(s.PosiAvDittKort>s.hand.Count-1){
                s.PosiAvDittKort=0;
            }
        }
        s.hand[s.PosiAvDittKort].FlyttaY=770;
        for(int i=0;i<s.hand.Count;i++){
            if(i!=s.PosiAvDittKort){
                s.hand[i].FlyttaY=800;
            }
        }

        if(kstate.IsKeyDown(Keys.Space)&&kör){
            kör=false;
            return true;
        }
        return false;
    }
    static List<Kortvisuel> SåDetSerBraUt(List<Spelare> SL,int v){ // sorterar och plaserar korten så de ser bra ut.
        int Y = 800;
        int gräns = 1200;
        List<Kortvisuel> New = new List<Kortvisuel>();
        gräns = gräns/SL[v].hand.Count;
        for(int i = 0;i<SL[v].hand.Count;i++){
            New.Add(new Kortvisuel((gräns * i)+300,Y,SL[v].hand[i].Kort));
        }
        SL[v].hand.Clear();
        string[] ordning = {"2","3","4","5","6","7","8","9","10","Knäkt","Dam","Kung","Ess"};
        for(int i=0;i<New.Count;i++){
            for(int ii=0;ii<New.Count-1;ii++){
                if(Array.IndexOf(ordning,New[ii].Kort)>Array.IndexOf(ordning,New[ii+1].Kort)){
                    string a = New[ii].Kort;
                    New[ii].Kort = New[ii+1].Kort;
                    New[ii+1].Kort = a;
                }
            }
        }
        return New;
    }
    static bool HarAllaKortPåRättPlats(List<Spelare> sl,int[] v){
        for(int i=0;i<4;i++){
            if(v[i]!=sl[i].hand.Count){
                return false;
            }
        }
        return true;
    }
    static int AnimationB(List<Spelare> sl,int v){
        int VilkaÄrFärdiga = 0;
        if(v==0){
            foreach(Kortvisuel k in sl[v].hand){
                if(sl[v].Kordinaty>k.vitrektangle.Y){
                    if(k.aniy==0){k.aniy=3;}
                    k.FlyttaX=k.vitrektangle.X+k.anix;
                    k.FlyttaY=k.vitrektangle.Y+k.aniy;
                }else{
                    VilkaÄrFärdiga++;
                }
            }
        }
        if(v==1){
            foreach(Kortvisuel k in sl[v].hand){
                if(sl[v].Kordinatx<k.vitrektangle.X){
                    if(k.anix==0){k.anix=-3;}
                    k.FlyttaX=k.vitrektangle.X+k.anix;
                    k.FlyttaY=k.vitrektangle.Y+k.aniy;
                }else{
                    VilkaÄrFärdiga++;
                }
            }
        }
        if(v==2){
            foreach(Kortvisuel k in sl[v].hand){
                if(sl[v].Kordinaty<k.vitrektangle.Y){
                    if(k.aniy==0){k.aniy=-3;}
                    k.FlyttaX=k.vitrektangle.X+k.anix;
                    k.FlyttaY=k.vitrektangle.Y+k.aniy;
                }else{
                    VilkaÄrFärdiga++;
                }
            }
        }
        if(v==3){
            foreach(Kortvisuel k in sl[v].hand){
                if(sl[v].Kordinatx>k.vitrektangle.X){
                    if(k.anix==0){k.anix=3;}
                    k.FlyttaX=k.vitrektangle.X+k.anix;
                    k.FlyttaY=k.vitrektangle.Y+k.aniy;
                }else{
                    VilkaÄrFärdiga++;
                }
            }
        }
        return VilkaÄrFärdiga;
    }
    static int Animation(List<Spelare> sl,int v){
        foreach(Kortvisuel k in sl[v].hand){
            k.anix=(sl[v].Kordinatx-k.vitrektangle.X)/20;
            k.aniy=(sl[v].Kordinaty-k.vitrektangle.Y)/20;
        }
        return AnimationB(sl,v);
    }
}
