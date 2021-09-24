using System;
using System.Collections.Generic;

namespace MijnGame14
{
    interface Drawable {
        void Draw();
    }
    struct Coordinate {
        public int X { get; set; }
        public int Y { get; set; }
        public Coordinate(int X, int Y) {
            this.X = X;
            this.Y = Y;
        }
        public static Coordinate operator +(Coordinate c1, Coordinate c2) {
            return new Coordinate(c1.X + c2.X, c1.Y + c2.Y);
        }
    }
    class NegatiefDrawenException : Exception { }
    class OutOfFieldException : Exception { }
    class SelfBiteException : Exception { }
    static class Drawer {
        public static void WriteDown(Coordinate Position, string Text) {
            if (Position.X < 0 || Position.Y < 0) throw new NegatiefDrawenException();
            Console.SetCursorPosition(Position.X, Position.Y);
            Console.WriteLine(Text);
        }
    }
    abstract class Placeable : Drawable {
        public Coordinate Position;
        
        public Placeable(char symbol = ' ') {
            this.Symbol = symbol;
        }
        public void ResetPosition() {
            Position = new Coordinate(0, 0);
        }
        public virtual void Draw() {
            Drawer.WriteDown(Position, "" + Symbol);
        }
        public void Erase() {
            Drawer.WriteDown(Position," ");
        }
        public virtual char Symbol { get; }
    }
    class Coin : Placeable {
        private bool flash;
        private static Random random = new Random();
        public Coordinate position;
        private int spaceX;
        private int spaceY;
        public Coin(int fieldWidth, int fieldHeiget){
            this.flash = true;
            this.spaceX = fieldWidth;
            this.spaceY = fieldHeiget;
            position.X = random.Next(1,spaceX);
            position.Y = random.Next(1,spaceY);
        }
        public override char Symbol
        {
             get {
                if (flash)
                    return 'O';
                else
                    return ' ';
             }
        }
        public override void Draw() {
            Drawer.WriteDown(this.position,"O");
            flash = !flash;
        }
        public void Reposition(){
            position.X = random.Next(1,spaceX);
            position.Y = random.Next(1,spaceY);
        }
    }
    class Player : Placeable {
        public string Name { get; set; }
        public Queue<Coordinate> Body { get; set; }
        public int BodyLength { get; set; }
        public int Points { get; set; }
        public char Direction { get; set; } // {u, r, d, l}
        public Player() : base('*') {
            this.Body = new Queue<Coordinate>();
         }
        public Boolean Move() {
            switch(Direction){
                case 'u': this.Position.Y--; break;
                case 'r': this.Position.X++; break;
                case 'd': this.Position.Y++; break;
                case 'l': this.Position.X--; break;
            }
            if (Body.Contains(Position)) return true;
            Body.Enqueue(Position);
            Body.Dequeue();
            return false;
        }
        public override void Draw() {
            foreach(Coordinate c in Body){
                Drawer.WriteDown(c, "*");
            }
            Drawer.WriteDown(Position,"H");
        }

        public void kapoof(){
            var pos = this.Position;
            if(pos.X<7) pos.X = 0;
            else pos.X = pos.X - 7;
            Console.SetCursorPosition(pos.X,pos.Y);
            Console.WriteLine("KAPOOOOOOOF!!!");
            Console.ReadLine();
        }
        public static bool operator >(Player sp1, Player sp2) {
            return sp1.Points > sp2.Points;
        }
        public static bool operator <(Player sp1, Player sp2) {
            return sp1.Points < sp2.Points;
        }
    }
    // class Vijandje : Placeable {
    //     public Vijandje() : base('E') { }
    // }
    class Field : Drawable
    {
        public int Width { get; set; } = 60;
        public int Height { get; set; } = 18;
        

        public void Draw()
        {
            for (int i=0; i<Width+1; i++) Drawer.WriteDown(new Coordinate(i,0),"_");
            for (int i=1; i<Height+2; i++) {
                Drawer.WriteDown(new Coordinate(0,i),"|");
                for (int j=1; j<Width; j++) Drawer.WriteDown(new Coordinate(j,i)," ");
                Drawer.WriteDown(new Coordinate(Width+1,i),"|");
            }
            for (int i=1; i<Width; i++) Drawer.WriteDown(new Coordinate(i,Height+1),"_");
        }
        // public void Clear(){
            
        // }
    }
    static class AantalExtensie
    {
        public static String AantalString(this int num) {
            if (num >= 1000000000) { return (num / 1000000000).ToString() + "B"; }
            if (num >= 1000000) { return (num / 1000000).ToString() + "M"; }
            if (num >= 1000) { return (num / 1000).ToString() + "k"; }
            return num.ToString();  
        }
    }
    class Level : Drawable
    {
        public Field veld = new Field();
        // public List<Vijandje> Vijandjes { get; set; }
        public string Name { get; set; }
        public int? Moeilijkheid { get; set; }
        public void Draw()
        {
            veld.Draw();
        }
    }

    class Game{
        public static Player player;
        public static Coin coin;
        public static Level level;
        public static int delay = 200;


        public static void Init(){
            Console.CursorVisible = false;
            Console.WriteLine("Welcome to Snakescii! \n Enter your name.");
            player = new Player() { Points = 10 };
            player.Name = Console.ReadLine();
            player.Body.Enqueue(new Coordinate(7,5));
            player.Body.Enqueue(new Coordinate(6,5));
            player.Position = new Coordinate(5,5);
            player.Body.Enqueue(player.Position);
            player.Direction='r';
            level = new Level() { 
                // Vijandjes = new List<Vijandje>() { 
                // new Vijandje() { Position = new Coordinate(1, 3) }, 
                // new Vijandje() { Position = new Coordinate(2, 2) } 
                // }
            };
            
            coin = new Coin(level.veld.Width, level.veld.Height);
            level.Draw();
            player.Draw();
            coin.Draw();
        }

        public static Boolean checkOutside(){
            Coordinate pos = player.Position;
            if (pos.X < 1 || pos.X > level.veld.Width || pos.Y < 1 || pos.Y > level.veld.Height){
                return true;
            } 
            return false;
        }

        public static Boolean checkEaten(Player p, Coin c){
            if (p.Position.X == c.position.X && p.Position.Y == c.position.Y) return true;
            return false;
        }

        public static void Run(){
            try {
                var key = Console.ReadKey();
                while (key.KeyChar != 'q') {
                    switch (key.KeyChar) {
                        case 'a': { player.Direction = 'l'; break; }
                        case 'w': { player.Direction = 'u'; break; }
                        case 's': { player.Direction = 'd'; break; }
                        case 'd': { player.Direction = 'r'; break; }
                        default : { break; }
                    }
                    while (!Console.KeyAvailable) {
                        level.Draw();
                        if (player.Move()) throw new SelfBiteException();
                        if (checkOutside()) throw new OutOfFieldException();
                        if (checkEaten(player, coin)) {
                            player.Body.Enqueue(player.Position);
                            coin.Reposition();
                            delay -= 10;
                        }
                        player.Draw();
                        coin.Draw();
                        System.Threading.Thread.Sleep(delay);
                    }
                    key = Console.ReadKey();
                }
            } catch (NegatiefDrawenException){
                Console.WriteLine("Ergens is geprobeerd te Drawen op het negatieve vlak!");
            } catch (OutOfFieldException){
                player.kapoof();
            } catch (SelfBiteException){
                player.kapoof();
            }
        }
    }


    class Program
    {
        static void Main(string[] args) {
            Game.Init();
            Game.Run();
        }
    }
}
        //    \  |  /
        // - kapooooof -
        //    /  |  \