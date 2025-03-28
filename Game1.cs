using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
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

    // för animation
    int VilkenSpelare;
    int VilketKortSjön;
    List<int> VilkaKort = new List<int>();
    bool FörstaAnimation = true;

    Spelfas fas;
    enum Spelfas{
        StartAnimation,
        DuVäljerKort,
        DuVäljerSpelare,
        TaKortFrånBot,
        annat,
        taKortFrånSpelareAnimation,
        taKortFrånSjönAnimation,
        Robot1,
        Robot2,
        Robot3,
        Slut,
        
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
        if(sl[0].hand.Count>0||sl[1].hand.Count>0||sl[2].hand.Count>0||sl[3].hand.Count>0||Sjön.Count>0){
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
                VilkenSpelare=0;
                HarDe4(sl[VilkenSpelare]);
                bool KanDuKöra = true;
                if(sl[VilkenSpelare].hand.Count<1&&Sjön.Count<1){ // om både handen och sjön är tom så kan du inte göra något
                    fas=Spelfas.Robot1;
                    KanDuKöra=false;
                }else if(sl[VilkenSpelare].hand.Count<1){ // om bara handen är tom så plockar du ett kort och körviare.
                    int kort=ran.Next(0,Sjön.Count);
                    sl[VilkenSpelare].hand.Add(Sjön[kort]);
                    Sjön.RemoveAt(kort);
                }
                if(sl[VilkenSpelare].hand.Count>0){
                    sl[VilkenSpelare].hand=SåDetSerBraUt(sl,VilkenSpelare);
                }
                if(KanDuKöra&&MovmentAvKort(sl[VilkenSpelare],ref Space)){
                    fas=Spelfas.DuVäljerSpelare;
                }
            }
            if(fas==Spelfas.DuVäljerSpelare&&MovmentAvSpelare(sl[VilkenSpelare],ref Space)){
                fas=Spelfas.TaKortFrånBot;
            }
            if(fas==Spelfas.TaKortFrånBot){
                UpdateraMinnen(sl);
                if(sl[sl[VilkenSpelare].posiSpelare].contains(sl[VilkenSpelare].hand[sl[VilkenSpelare].PosiAvDittKort].Kort)){
                    VilkaKort=VarÄrKort(sl[VilkenSpelare],sl[sl[VilkenSpelare].posiSpelare]);
                    foreach(List<string> st in sl[VilkenSpelare].vadhardeandraspelat){
                        st.Remove(sl[VilkenSpelare].hand[sl[VilkenSpelare].PosiAvDittKort].Kort);
                    }
                    fas=Spelfas.taKortFrånSpelareAnimation;
                }else{
                    if(Sjön.Count>0){
                        VilketKortSjön=ran.Next(0,Sjön.Count);
                        fas=Spelfas.taKortFrånSjönAnimation;
                    }else{
                        fas=Spelfas.Robot1;
                    }
                }
            }
            if(fas==Spelfas.Robot1){
                VilkenSpelare=1;
                HarDe4(sl[VilkenSpelare]);
                bool KanDuKöra = true;
                if(sl[VilkenSpelare].hand.Count<1&&Sjön.Count<1){ // om både handen och sjön är tom så kan du inte göra något
                    fas=Spelfas.Robot2;
                    KanDuKöra=false;
                }else if(sl[VilkenSpelare].hand.Count<1){ // om bara handen är tom så plockar du ett kort och körviare.
                    int kort=ran.Next(0,Sjön.Count);
                    sl[VilkenSpelare].hand.Add(Sjön[kort]);
                    Sjön.RemoveAt(kort);
                }
                if(KanDuKöra){
                    Robot(VilkenSpelare);
                    UpdateraMinnen(sl);
                    if(sl[sl[VilkenSpelare].posiSpelare].contains(sl[VilkenSpelare].hand[sl[VilkenSpelare].PosiAvDittKort].Kort)){
                        VilkaKort=VarÄrKort(sl[VilkenSpelare],sl[sl[VilkenSpelare].posiSpelare]);
                        fas=Spelfas.taKortFrånSpelareAnimation;
                    }else{
                        if(Sjön.Count>0){
                            VilketKortSjön=ran.Next(0,Sjön.Count);
                            fas=Spelfas.taKortFrånSjönAnimation;
                        }else{
                            fas=Spelfas.DuVäljerKort;
                        }
                    }
                }
            }
            if(fas==Spelfas.Robot2){
                VilkenSpelare=2;
                HarDe4(sl[VilkenSpelare]);
                bool KanDuKöra = true;
                if(sl[VilkenSpelare].hand.Count<1&&Sjön.Count<1){ // om både handen och sjön är tom så kan du inte göra något
                    fas=Spelfas.Robot3;
                    KanDuKöra=false;
                }else if(sl[VilkenSpelare].hand.Count<1){ // om bara handen är tom så plockar du ett kort och körviare.
                    int kort=ran.Next(0,Sjön.Count);
                    sl[VilkenSpelare].hand.Add(Sjön[kort]);
                    Sjön.RemoveAt(kort);
                }
                if(KanDuKöra){
                    Robot(VilkenSpelare);
                    UpdateraMinnen(sl);
                    if(sl[sl[VilkenSpelare].posiSpelare].contains(sl[VilkenSpelare].hand[sl[VilkenSpelare].PosiAvDittKort].Kort)){
                        VilkaKort=VarÄrKort(sl[VilkenSpelare],sl[sl[VilkenSpelare].posiSpelare]);
                        fas=Spelfas.taKortFrånSpelareAnimation;
                    }else{
                        if(Sjön.Count>0){
                            VilketKortSjön=ran.Next(0,Sjön.Count);
                            fas=Spelfas.taKortFrånSjönAnimation;
                        }else{
                            fas=Spelfas.DuVäljerKort;
                        }
                    }
                }
            }
            if(fas==Spelfas.Robot3){
                VilkenSpelare=3;
                HarDe4(sl[VilkenSpelare]);
                bool KanDuKöra = true;
                if(sl[VilkenSpelare].hand.Count<1&&Sjön.Count<1){ // om både handen och sjön är tom så kan du inte göra något
                    fas=Spelfas.DuVäljerKort;
                    KanDuKöra=false;
                }else if(sl[VilkenSpelare].hand.Count<1){ // om bara handen är tom så plockar du ett kort och körviare.
                    int kort=ran.Next(0,Sjön.Count);
                    sl[VilkenSpelare].hand.Add(Sjön[kort]);
                    Sjön.RemoveAt(kort);
                }
                if(KanDuKöra){
                    Robot(VilkenSpelare);
                    UpdateraMinnen(sl);
                    if(sl[sl[VilkenSpelare].posiSpelare].contains(sl[VilkenSpelare].hand[sl[VilkenSpelare].PosiAvDittKort].Kort)){
                        VilkaKort=VarÄrKort(sl[VilkenSpelare],sl[sl[VilkenSpelare].posiSpelare]);
                        fas=Spelfas.taKortFrånSpelareAnimation;
                    }else{
                        if(Sjön.Count>0){
                            VilketKortSjön=ran.Next(0,Sjön.Count);
                            fas=Spelfas.taKortFrånSjönAnimation;
                        }else{
                            fas=Spelfas.DuVäljerKort;
                        }
                    }
                }
            }
            if(fas==Spelfas.taKortFrånSjönAnimation){
                if(AnimationTarFrånSjön(Sjön,sl[VilkenSpelare],VilketKortSjön)){
                    sl[VilkenSpelare].hand.Add(Sjön[VilketKortSjön]);
                    Sjön.RemoveAt(VilketKortSjön);
                    if(VilkenSpelare==0){
                        fas=Spelfas.Robot1;
                    }else if(VilkenSpelare==1){
                        fas=Spelfas.Robot2;
                    }else if(VilkenSpelare==2){
                        fas=Spelfas.Robot3;
                    }else if(VilkenSpelare==3){
                        fas=Spelfas.DuVäljerKort;
                    }
                }
            }
            if(fas==Spelfas.taKortFrånSpelareAnimation){
                if(AnimationTarFrånSpelare(sl[VilkenSpelare],sl[sl[VilkenSpelare].posiSpelare],VilkaKort)){
                    VilkaKort.Clear();
                    RemoveKort(sl[VilkenSpelare],sl[sl[VilkenSpelare].posiSpelare]);
                    if(VilkenSpelare==0){
                        fas=Spelfas.DuVäljerKort;
                    }else if(VilkenSpelare==1){
                        fas=Spelfas.Robot1;
                    }else if(VilkenSpelare==2){
                        fas=Spelfas.Robot2;
                    }else if(VilkenSpelare==3){
                        fas=Spelfas.Robot3;
                    }
                }
            }
        }else{
            fas=Spelfas.Slut;
        }
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
            if(fas==Spelfas.taKortFrånSpelareAnimation){
                foreach(int i in VilkaKort){
                    if(sl[VilkenSpelare].posiSpelare!=0){
                        _spriteBatch.Draw(texture,sl[sl[VilkenSpelare].posiSpelare].hand[i].rödrektangle,Color.Black);
                        _spriteBatch.Draw(texture,sl[sl[VilkenSpelare].posiSpelare].hand[i].vitrektangle,Color.Red);
                    }
                }
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
            if(fas!=Spelfas.Slut){
                for(int i=1;i<4;i++){
                    _spriteBatch.DrawString(font1,sl[i].namn,new Vector2(sl[i].Kordinatx,sl[i].Kordinaty),Color.Black);
                    _spriteBatch.DrawString(font1,sl[i].hand.Count+"",new Vector2(sl[i].Kordinatx+50,sl[i].Kordinaty+35),Color.Black);
                }
            }
            for(int i=1;i<4;i++){
                _spriteBatch.Draw(texture,new Rectangle(sl[i].Kordinatx,sl[i].Kordinaty+70,100,30*sl[0].vadhardeandraspelat[i].Count),Color.DarkGreen);
                for(int a=0;a<sl[0].vadhardeandraspelat[i].Count;a++){
                    _spriteBatch.DrawString(font1,sl[0].vadhardeandraspelat[i][a],new Vector2(sl[i].Kordinatx+2,69+sl[i].Kordinaty+(30*a)),Color.Black);
                }
            }
            if(fas==Spelfas.Slut){
                for(int i=0;i<4;i++){
                    _spriteBatch.DrawString(font1,sl[i].namn,new Vector2(350+(300*i),400),Color.Black);
                    _spriteBatch.DrawString(font1,"Poäng "+sl[i].poäng,new Vector2(350+(300*i),450),Color.Black);
                }
            }
            

        _spriteBatch.End();
        

        base.Draw(gameTime);
    }
    
    void HarDe4(Spelare ss){
        string[] Kort = {"2","3","4","5","6","7","8","9","10","Knäkt","Dam","Kung","Ess"};
        foreach(string k in Kort){
            if(ss.FindAll(k)==4){
                ss.poäng++;
                for(int i=ss.FindPositionOfAll(k).Count-1;i>-1;i--){
                    ss.hand.RemoveAt(ss.FindPositionOfAll(k)[i]);
                    foreach(Spelare s1 in sl){
                        foreach(List<string> str in s1.vadhardeandraspelat){
                            if(str.Contains(k)){
                                str.Remove(k);
                            }
                        }

                    }
                }
            }
        }
    }
    void UpdateraMinnen(List<Spelare> sl){
        for(int i=0;i<sl.Count;i++){
            if(i!=VilkenSpelare){
                if(!sl[i].vadhardeandraspelat[VilkenSpelare].Contains(sl[VilkenSpelare].hand[sl[VilkenSpelare].PosiAvDittKort].Kort)){
                    sl[i].vadhardeandraspelat[VilkenSpelare].Add(sl[VilkenSpelare].hand[sl[VilkenSpelare].PosiAvDittKort].Kort);
                }
            }
        }
    }
    static bool HarNågonLikaKortSomDu(Spelare ss){
        Random ran = new Random();
        List<int> spelare = new List<int>();
        List<int> sp = new List<int>{0,1,2,3};
        int valet;
        for(int i=0;i<4;i++){
            valet = ran.Next(0,sp.Count);
            spelare.Add(sp[valet]);
            sp.RemoveAt(valet);
        }
        spelare.Remove(ss.vemärdu);
        
        
        foreach(int i in spelare){
            for(int a=0;a<ss.hand.Count;a++){
                if(ss.vadhardeandraspelat[i].Contains(ss.hand[a].Kort)){
                    ss.posiSpelare=i;
                    ss.PosiAvDittKort=a;
                    ss.vadhardeandraspelat[i].Remove(ss.hand[a].Kort);
                    return false;
                }
            }
        }
        /*
            med en lista i spelare kan vi göra så den gemför mellan listorna i sig för att kolla om de innehåler något av det.
            4 listor en som minst en som är tom för det är dens egen.

            gör så den kollar slump mässigt en av listorna o sen om den inte inehåller det så kollar 
            den nästa till alla är kollade och om ingen hade det den själv hade i handen så väljer den bara slump mässigt ett av sina
            kort.

            den ska även byta vilken spelare den väljer om den hittar ett lika kort i listorna.
        */

        return true;
    }
    void Robot(int v){
        if(HarNågonLikaKortSomDu(sl[v])){
            int kanskedem;
            do{
                kanskedem = ran.Next(0,4);
            }while(kanskedem==sl[v].vemärdu);
            sl[v].posiSpelare=kanskedem;
            sl[v].PosiAvDittKort=ran.Next(0,sl[v].hand.Count); // method för det.
        }
    }
    static Kortvisuel anixy(Kortvisuel kort, int KordinatX, int KordinatY){
        if(KordinatX-kort.vitrektangle.X>0){
            if(kort.anix<3){
                kort.anix=3;
            }
        }else{
            if(kort.anix>-3){
                kort.anix=-3;
            }
        }
        if(KordinatY-kort.vitrektangle.Y>0){
            if(kort.aniy<3){
                kort.aniy=3;
            }
        }else{
            if(kort.aniy>-3){
                kort.aniy=-3;
            }
        }
        return kort;
    }
    bool AnimationTarFrånSjön(List<Kortvisuel> Sjön, Spelare s1, int VilketKortSjön){
        if(FörstaAnimation){    
            Sjön[VilketKortSjön].anix=(s1.Kordinatx-Sjön[VilketKortSjön].vitrektangle.X)/20;
            Sjön[VilketKortSjön].aniy=(s1.Kordinaty-Sjön[VilketKortSjön].vitrektangle.Y)/20;
            FörstaAnimation=false;
        }
        if(ÄrDetÖverUnder(s1,Sjön[VilketKortSjön].vitrektangle.X,Sjön[VilketKortSjön].vitrektangle.Y)){
            Sjön[VilketKortSjön].FlyttaX=Sjön[VilketKortSjön].vitrektangle.X+Sjön[VilketKortSjön].anix;
            Sjön[VilketKortSjön].FlyttaY=Sjön[VilketKortSjön].vitrektangle.Y+Sjön[VilketKortSjön].aniy;
        }else{
            FörstaAnimation=true;
            ;
            return true;
        }
        return false;
    }
    bool AnimationTarFrånSpelare(Spelare s1, Spelare s2,List<int> vilkakort){
        List<bool> ÄrDeFärdiga = new List<bool>();
        int a=-1;
        foreach(int i in vilkakort){
            a++;
            ÄrDeFärdiga.Add(false);
            if(FörstaAnimation){
                s2.hand[i].anix=(s1.Kordinatx-s2.hand[i].vitrektangle.X)/40;
                s2.hand[i].aniy=(s1.Kordinaty-s2.hand[i].vitrektangle.Y)/40;
            }
            if(ÄrDetÖverUnder(s1,s2.hand[i].vitrektangle.X,s2.hand[i].vitrektangle.Y)){
                s2.hand[i].FlyttaX=s2.hand[i].vitrektangle.X+s2.hand[i].anix;
                s2.hand[i].FlyttaY=s2.hand[i].vitrektangle.Y+s2.hand[i].aniy;
            }else{
                ÄrDeFärdiga[a]=true;
            }
        }
        FörstaAnimation=false;
        
        if(ÄrDeFärdiga.FindAll(k=>k==true).Count==vilkakort.Count){
            FörstaAnimation=true;
            return true;
        }
        return false;
    }
    static bool ÄrDetÖverUnder(Spelare ss,int Kordinatx,int Kordinaty){
        if(ss.vemärdu==0){
            if(ss.Kordinaty>Kordinaty){
                return true;
            }
        }
        if(ss.vemärdu==1){
            if(ss.Kordinatx<Kordinatx){
                return true;
            }
        }
        if(ss.vemärdu==2){
            if(ss.Kordinaty<Kordinaty){
                return true;
            }
        }
        if(ss.vemärdu==3){
            if(ss.Kordinatx>Kordinatx){
                return true;
            }
        }
        return false;
    }
    static List<int> VarÄrKort(Spelare s1, Spelare s2){
        List<int> varärkort = new List<int>();
        for(int i=0;i<s2.hand.Count;i++){
            if(s2.hand[i].Kort==s1.hand[s1.PosiAvDittKort].Kort){
                varärkort.Add(i);
            }
        }
        return varärkort;
    }
    static void RemoveKort(Spelare s1, Spelare s2){
        List<int> varärkort = new List<int>();
        for(int i=0;i<s2.hand.Count;i++){
            if(s2.hand[i].Kort==s1.hand[s1.PosiAvDittKort].Kort){
                varärkort.Add(i);
            }
        }
        for(int i=varärkort.Count-1;i>-1;i--){
            s1.hand.Add(s2.hand[varärkort[i]]);
            s2.hand.RemoveAt(varärkort[i]);
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
                s.hand[i].FlyttaY=780;
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
                    if(k.aniy<3){k.aniy=3;}
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

    void test(Spelare ss){
        foreach(List<string> li in ss.vadhardeandraspelat){
            foreach(string s in li){
                Console.Write("s ");
            }
            Console.WriteLine();
        }
    }

}