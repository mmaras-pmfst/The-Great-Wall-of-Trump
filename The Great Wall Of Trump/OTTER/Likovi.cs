using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    public abstract class Likovi:Sprite
    {
        protected int zivot;

        public int Zivot
        {
            get { return zivot; }
            set
            {
                
                zivot = value;
            }

        }

        protected int brzina;

        public int Brzina
        {
            get { return brzina; }
            set
            {
                if (brzina <= 0)
                {
                    brzina = 1;
                }
                else
                {
                    brzina = value;
                }
            }
        }

        public Likovi(string slika, int xcor, int ycor, int zivot, int brzina)
            : base(slika, xcor, ycor)
        {
            this.zivot = zivot;
            this.brzina = brzina;
        }
    }

    public abstract class Oruzje : Sprite
    {
        protected int snaga;

        public int Snaga
        {
            get { return snaga; }
            set
            {
                if (value <= 0)
                {
                    throw (new ArgumentException());
                }
                else
                {
                    snaga = value;
                }
            }
        }

        protected int brzina;

        public int Brzina
        {
            get { return brzina; }
            set
            {
                if (value <= 0)
                {
                    brzina = 1;
                }
                else
                {
                    brzina = value;
                }
            }
        }

        public Oruzje(string slika, int xcor, int ycor, int snaga, int brzina)
            : base(slika, xcor, ycor)
        {
            this.snaga = snaga;
            this.brzina = brzina;
        }

    }

    public class Tenk : Likovi
    {
        protected int bodovi;

        public int Bodovi
        {
            get { return bodovi; }
            set { bodovi = value; }
        }
        public override int X
        {
            get { return this.x; }
            set
            {
                if (value < GameOptions.LeftEdge)
                {
                    x = GameOptions.LeftEdge;
                }
                else if (value > GameOptions.RightEdge - this.Heigth)
                {
                    x = GameOptions.RightEdge - this.Heigth;
                }
                else
                {
                    x = value;
                }
            }
        }
        public bool bulletReady;

        public Tenk(string slika, int xcor, int ycor)
            : base(slika, xcor, ycor, 3, 5)
        {
            this.bodovi = 0;
            this.bulletReady = true;
        }

    }

    public class Enemy : Likovi
    {
        
        public Enemy(string slika, int xcor, int ycor)
            : base(slika, xcor, ycor, 1, 2)
        {
            
        }
    }
    public class Mobitel : Likovi
    {
        
        public Mobitel(string slika, int xcor, int ycor)
            : base(slika, xcor, ycor, 2, 1)
        {
            
        }
    }
    public class Boss : Likovi
    {
        public bool bombaReady;

        public Boss(string slika,int xcor,int ycor)
            : base(slika, xcor, ycor, 5, 5)
        {
            this.bombaReady=true;
        }
    }
    public class Bomba : Oruzje
    {
        public Bomba(string slika, int xcor, int ycor)
            : base(slika, xcor, ycor, 2, 28)
        {

        }
    }
    public class Prepreka : Oruzje
    {
        public Prepreka(int xcor, int ycor)
            : base("sprites\\saw.png", xcor, ycor, 1, 24)
        {

        }
    }
    public class Metak : Oruzje
    {
        
        public Metak(string slika, int xcor, int ycor,int snaga)
            : base(slika, xcor, ycor, snaga, 30)
        {
            
        }
    }

}
