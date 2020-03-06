using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OTTER
{
    /// <summary>
    /// -
    /// </summary>
    public partial class BGL : Form
    {
        /* ------------------- */
        #region Environment Variables

        List<Func<int>> GreenFlagScripts = new List<Func<int>>();

        /// <summary>
        /// Uvjet izvršavanja igre. Ako je <c>START == true</c> igra će se izvršavati.
        /// </summary>
        /// <example><c>START</c> se često koristi za beskonačnu petlju. Primjer metode/skripte:
        /// <code>
        /// private int MojaMetoda()
        /// {
        ///     while(START)
        ///     {
        ///       //ovdje ide kod
        ///     }
        ///     return 0;
        /// }</code>
        /// </example>
        public static bool START = true;

        //sprites
        /// <summary>
        /// Broj likova.
        /// </summary>
        public static int spriteCount = 0, soundCount = 0;

        /// <summary>
        /// Lista svih likova.
        /// </summary>
        //public static List<Sprite> allSprites = new List<Sprite>();
        public static SpriteList<Sprite> allSprites = new SpriteList<Sprite>();

        //sensing
        int mouseX, mouseY;
        Sensing sensing = new Sensing();

        //background
        List<string> backgroundImages = new List<string>();
        int backgroundImageIndex = 0;
        string ISPIS = "";

        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        double lastTime, thisTime, diff;

        #endregion
        /* ------------------- */
        #region Events

        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try
            {                
                foreach (Sprite sprite in allSprites)
                {                    
                    if (sprite != null)
                        if (sprite.Show == true)
                        {
                            g.DrawImage(sprite.CurrentCostume, new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Heigth));
                        }
                    if (allSprites.Change)
                        break;
                }
                if (allSprites.Change)
                    allSprites.Change = false;
            }
            catch
            {
                //ako se doda sprite dok crta onda se mijenja allSprites
                MessageBox.Show("Greška!");
            }
        }

        private void startTimer(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
           
            Init();
            
        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        /// <summary>
        /// Crta tekst po pozornici.
        /// </summary>
        /// <param name="sender">-</param>
        /// <param name="e">-</param>
        public void DrawTextOnScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var brush = new SolidBrush(Color.WhiteSmoke);
            string text = ISPIS;

            SizeF stringSize = new SizeF();
            Font stringFont = new Font("Arial", 14);
            stringSize = e.Graphics.MeasureString(text, stringFont);

            using (Font font1 = stringFont)
            {
                RectangleF rectF1 = new RectangleF(0, 0, stringSize.Width, stringSize.Height);
                e.Graphics.FillRectangle(brush, Rectangle.Round(rectF1));
                e.Graphics.DrawString(text, font1, Brushes.Black, rectF1);
            }
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;            
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = false;
            sensing.MouseDown = false;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            //sensing.MouseX = e.X;
            //sensing.MouseY = e.Y;
            //Sensing.Mouse.x = e.X;
            //Sensing.Mouse.y = e.Y;
            sensing.Mouse.X = e.X;
            sensing.Mouse.Y = e.Y;

        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            sensing.Key = e.KeyCode.ToString();
            sensing.KeyPressedTest = true;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            sensing.Key = "";
            sensing.KeyPressedTest = false;
        }

        private void Update(object sender, EventArgs e)
        {
            if (sensing.KeyPressed(Keys.Escape))
            {
                START = false;
            }

            if (START)
            {
                this.Refresh();
            }
        }

        #endregion
        /* ------------------- */
        #region Start of Game Methods

        //my
        #region my

        //private void StartScriptAndWait(Func<int> scriptName)
        //{
        //    Task t = Task.Factory.StartNew(scriptName);
        //    t.Wait();
        //}

        //private void StartScript(Func<int> scriptName)
        //{
        //    Task t;
        //    t = Task.Factory.StartNew(scriptName);
        //}

        private int AnimateBackground(int intervalMS)
        {
            while (START)
            {
                setBackgroundPicture(backgroundImages[backgroundImageIndex]);
                Game.WaitMS(intervalMS);
                backgroundImageIndex++;
                if (backgroundImageIndex == 3)
                    backgroundImageIndex = 0;
            }
            return 0;
        }

        private void KlikNaZastavicu()
        {
            foreach (Func<int> f in GreenFlagScripts)
            {
                Task.Factory.StartNew(f);
            }
        }

        #endregion

        /// <summary>
        /// BGL
        /// </summary>
        
        public BGL()
        {
            InitializeComponent();
           
        }

        /// <summary>
        /// Pričekaj (pauza) u sekundama.
        /// </summary>
        /// <example>Pričekaj pola sekunde: <code>Wait(0.5);</code></example>
        /// <param name="sekunde">Realan broj.</param>
        public void Wait(double sekunde)
        {
            int ms = (int)(sekunde * 1000);
            Thread.Sleep(ms);
        }

        //private int SlucajanBroj(int min, int max)
        //{
        //    Random r = new Random();
        //    int br = r.Next(min, max + 1);
        //    return br;
        //}

        /// <summary>
        /// -
        /// </summary>
        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;
            //Load resources and level here
            this.Paint += new PaintEventHandler(DrawTextOnScreen);
            SetupGame();
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="val">-</param>
        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

        /// <summary>
        /// -
        /// </summary>
        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        //stage
        #region Stage

        /// <summary>
        /// Postavi naslov pozornice.
        /// </summary>
        /// <param name="title">tekst koji će se ispisati na vrhu (naslovnoj traci).</param>
        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Postavi boju pozadine.
        /// </summary>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        public void setBackgroundColor(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Postavi boju pozornice. <c>Color</c> je ugrađeni tip.
        /// </summary>
        /// <param name="color"></param>
        public void setBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        /// <summary>
        /// Postavi sliku pozornice.
        /// </summary>
        /// <param name="backgroundImage">Naziv (putanja) slike.</param>
        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        /// <summary>
        /// Izgled slike.
        /// </summary>
        /// <param name="layout">none, tile, stretch, center, zoom</param>
        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        #endregion

        //sound
        #region sound methods

        /// <summary>
        /// Učitaj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        /// <param name="file">-</param>
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        /// <summary>
        /// Sviraj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        /// <summary>
        /// loopSound
        /// </summary>
        /// <param name="soundNum">-</param>
        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        /// <summary>
        /// Zaustavi zvuk.
        /// </summary>
        /// <param name="soundNum">broj</param>
        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        #endregion

        //file
        #region file methods

        /// <summary>
        /// Otvori datoteku za čitanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        /// <summary>
        /// Otvori datoteku za pisanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        /// <summary>
        /// Zapiši liniju u datoteku.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <param name="line">linija</param>
        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        /// <summary>
        /// Pročitaj liniju iz datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća pročitanu liniju</returns>
        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        /// <summary>
        /// Čita sadržaj datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća sadržaj</returns>
        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        #endregion

        //mouse & keys
        #region mouse methods

        /// <summary>
        /// Sakrij strelicu miša.
        /// </summary>
        public void hideMouse()
        {
            Cursor.Hide();
        }

        /// <summary>
        /// Pokaži strelicu miša.
        /// </summary>
        public void showMouse()
        {
            Cursor.Show();
        }

        /// <summary>
        /// Provjerava je li miš pritisnut.
        /// </summary>
        /// <returns>true/false</returns>
        public bool isMousePressed()
        {
            //return sensing.MouseDown;
            return sensing.MouseDown;
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">naziv tipke</param>
        /// <returns></returns>
        public bool isKeyPressed(string key)
        {
            if (sensing.Key == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">tipka</param>
        /// <returns>true/false</returns>
        public bool isKeyPressed(Keys key)
        {
            if (sensing.Key == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
        /* ------------------- */

        /* ------------ GAME CODE START ------------ */

        /* Game variables */


        /* Initialization */
        Tenk tenk;
        Enemy enemy,enemy2;
        Mobitel huawei;
        Prepreka pila;
        Metak metak;
        Boss helicopter;
        Bomba bomba;

        bool enemypadanje;
        bool enemya2padanje;
        bool huaweipadanje;
        bool novoKretanje=false;
        bool kretanjeGlavnog = false;

        public delegate void PorazDel();
        public static event PorazDel _poraz;

        public delegate void PobjedaDel();
        public static event PobjedaDel _pobjeda;

        
        
        private void SetupGame()
        {
            
            //1. setup stage
            SetStageTitle("PMF");
            //setBackgroundColor(Color.WhiteSmoke);            
            setBackgroundPicture("backgrounds\\pozadina.png");
            //none, tile, stretch, center, zoom
            setPictureLayout("stretch");

            //2. add sprites
            enemy = new Enemy("sprites\\enemy.png", -200, -200);
            enemy.SetSize(10);
            Game.AddSprite(enemy);

            enemy2 = new Enemy("sprites\\enemy.png", -250, -200);
            enemy2.SetSize(10);
            Game.AddSprite(enemy2);

            huawei = new Mobitel("sprites\\mobitel.png", -100, -100);
            huawei.SetSize(15);
            Game.AddSprite(huawei);

            helicopter = new Boss("sprites\\helicopter.png", -100, -300);
            helicopter.SetSize(35);            
            Game.AddSprite(helicopter);

            pila = new Prepreka(-200, -200);
            pila.SetSize(5);
            pila.Show = true;
            Game.AddSprite(pila);

            metak = new Metak("sprites\\bullet1.png", -150, -150,1);
            metak.SetSize(35);
            metak.RotationStyle = "All Around";
            metak.SetVisible(false);
            Game.AddSprite(metak);

            bomba = new Bomba("sprites\\bomba.png", -100, -400);
            bomba.SetSize(20);
            Game.AddSprite(bomba);

            tenk = new Tenk("sprites\\tank.png", 0, 0);
            tenk.SetSize(35);
            tenk.RotationStyle = "All Around";            
            int dno = GameOptions.DownEdge - tenk.Heigth - (pila.Heigth / 2);
            tenk.SetX((GameOptions.RightEdge - tenk.Width) / 2); //sredina
            tenk.SetY(dno);
            Game.AddSprite(tenk);

            _poraz += Kraj;
            _pobjeda += Pobjeda;

            //3. scripts that start
            Game.StartScript(TenkKretanje);
            Game.StartScript(Provjera);
            
            
        }

        /* Scripts */
        private int Provjera()
        {
            while (START)
            {
                if (tenk.Bodovi == 5 && levelCount == 0)
                {                   
                    levelCount++;
                    tenk.Zivot++;                    
                    MessageBox.Show("Čestitam prošli se prvi level\n\tLEVEL 2");
                }
                if(tenk.Bodovi==10 && levelCount == 1)
                {
                    tenk.Zivot++;
                    levelCount++;                    
                    MessageBox.Show("Čestitam prošli ste drugi level\n\tBOSS LEVEL");
                }
                if (levelCount==0)
                {
                    if (novoKretanje == false)
                    {
                        Game.StartScript(KretanjePrepreke);
                    }
                    if (tenk.Bodovi % 2 != 0 && enemypadanje == false)
                    {                        
                        Game.StartScript(EnemyPadanje);
                    }
                    if (tenk.Bodovi % 2 == 0 && huaweipadanje == false)
                    {
                        Game.StartScript(HuaweiPadanje);
                    }
                }
                if(levelCount==1)
                {
                    if (novoKretanje == false)
                    {
                        Game.StartScript(KretanjePrepreke);
                    }
                    if (huaweipadanje == false)
                    {
                        Game.StartScript(HuaweiPadanje);
                    }
                    if (enemypadanje == false)
                    {
                        Game.StartScript(EnemyPadanje);
                    }
                    if (enemya2padanje == false)
                    {
                        Game.StartScript(EnemyPadanje2_2);
                    }
                }
                if(levelCount==2)
                {
                    if (novoKretanje == false)
                    {
                        Game.StartScript(KretanjePrepreke);
                    }
                    if (kretanjeGlavnog == false)
                    {
                        Game.StartScript(BossKretanje);
                    }                    
                }                
                if (helicopter.Zivot == 0)
                {
                    _pobjeda.Invoke();                             
                }                
                Wait(1);
            }
            return 0;
        }
        
        public int levelCount = 0;
        private int TenkKretanje()
        {
            while (START) 
            {
                ISPIS = "Bodovi: " + tenk.Bodovi.ToString()+ " Životi: "+tenk.Zivot.ToString();
                tenk.PointToMouse(sensing.Mouse);

                Wait(0.01);
                if (sensing.KeyPressed("A"))
                {
                    tenk.X -= tenk.Brzina;
                    Wait(0.01);
                }
                if (sensing.KeyPressed("D"))
                {
                    tenk.X += tenk.Brzina;
                    Wait(0.01);
                }
                if (sensing.MouseDown)
                {
                    if (tenk.bulletReady)
                    {
                        tenk.bulletReady = false;
                        Game.StartScript(Pucanje);
                        tenk.bulletReady = false;
                    }
                }
                if(tenk.Y== GameOptions.DownEdge - tenk.Heigth - (pila.Heigth / 2))
                {
                    if (sensing.KeyPressed(Keys.Space)) 
                    {
                        sensing.KeyUp();
                        Game.StartScript(Skok);
                    }
                }     
                if (tenk.TouchingSprite(enemy))
                {
                    tenk.Bodovi++;
                    enemypadanje = false;
                    enemy.Show = false;
                    enemy.GotoXY(-100, -100);
                }
                if (tenk.TouchingSprite(huawei))
                {
                    tenk.Bodovi++;
                    huaweipadanje= false;
                    huawei.Show = false;
                    huawei.GotoXY(-100, -100);
                }
                if (tenk.TouchingSprite(enemy2))
                {
                    tenk.Bodovi++;
                    enemya2padanje = false;
                    enemy2.Show = false;
                    enemy2.GotoXY(-100, -100);
                }
                if (tenk.TouchingSprite(pila))
                {
                    tenk.Zivot--;
                    int dno = GameOptions.DownEdge - tenk.Heigth - (pila.Heigth / 2);
                    tenk.SetX((GameOptions.RightEdge - tenk.Width) / 2); //sredina
                    tenk.SetY(dno);
                    pila.GotoXY(-500, -500);
                    novoKretanje = false;

                }
                if (tenk.TouchingSprite(bomba))
                {
                    tenk.Zivot--;
                    bomba.SetVisible(false);
                    bomba.GotoXY(-100, -400);
                    helicopter.bombaReady = true;

                }
                if (tenk.Zivot == 0)
                {
                    _poraz.Invoke();
                }
            }
            return 0;
        }

        private int Skok()
        {
            sensing.KeyUp();
            for (int i = 0; i < 10; i++)
            {
                tenk.Y -= 10;
                if (tenk.TouchingSprite(pila))
                {
                    break;
                }
                Wait(0.03);
            }
            for (int i = 0; i < 10; i++)
            {
                tenk.Y += 10;
                if (tenk.TouchingSprite(pila))
                {
                    break;
                }
                Wait(0.05);
            }
            return 0;
        }

        private int KretanjePrepreke()
        {
            
            novoKretanje = true;
            Random r = new Random();
            int broj = r.Next(0, 2);
            if (broj == 0)
            {
                pila.GotoXY(0 + pila.Width, GameOptions.DownEdge - pila.Heigth);                
            }
            else
            {
                pila.GotoXY(GameOptions.RightEdge - pila.Width, GameOptions.DownEdge - pila.Heigth);                
            }
            while (novoKretanje)
            {
                if (broj == 0)
                {
                    pila.X += pila.Brzina;
                    Wait(0.1);
                }
                else
                {
                    pila.X -= pila.Brzina;
                    Wait(0.1);
                }
                if (pila.X<=GameOptions.LeftEdge || pila.X>=GameOptions.RightEdge)
                {
                    pila.GotoXY(-500, -500);
                    novoKretanje = false;
                }

                if ((tenk.Bodovi == 5 && levelCount==0) || (tenk.Bodovi==10 && levelCount==1) || helicopter.Zivot==0)
                {
                    pila.GotoXY(-100, -100);
                    novoKretanje = false;
                }
            }
            return 0;
        }
        private int Pucanje()
        {
            metak.Goto_Sprite(tenk);
            metak.SetVisible(true);
            metak.SetDirection(tenk.GetHeading());
            while (tenk.bulletReady == false)
            {
                metak.MoveSteps(metak.Brzina);
                Wait(0.1);
                if (metak.TouchingEdge())
                {
                    metak.Y -= metak.Brzina;
                    metak.SetVisible(false);
                    tenk.bulletReady = true;

                }
                if (metak.TouchingSprite(bomba))
                {
                    metak.GotoXY(-100, -400);
                    bomba.GotoXY(-150, -200);
                    tenk.bulletReady = true;
                    helicopter.bombaReady = true;
                }

            }
            return 0;
        }

        private int EnemyPadanje()
        {
            enemypadanje = true;
            Random r = new Random();
            int pocetniX = r.Next(0, GameOptions.RightEdge - enemy.Width);

            enemy.GotoXY(pocetniX, 0);
            enemy.Show = true;

            while (enemypadanje)
            {
                enemy.Y += enemy.Brzina;
                Wait(0.05);
                string rub;
                if (enemy.TouchingEdge(out rub))
                {
                    if (rub == "bottom")
                    {
                        tenk.Zivot--;
                        enemypadanje = false;

                    }
                }
                if (enemy.TouchingSprite(metak))
                {
                    tenk.bulletReady = true;
                    tenk.Bodovi++;
                    metak.GotoXY(-600, -600);
                    enemypadanje = false;
                    enemy.Show = false;                    
                    enemy.GotoXY(-100, -100);
                   
                    
                }
                if (tenk.Bodovi == 10)
                {
                    enemy.GotoXY(-200, -250);
                    break;
                }
            }
            return 0;
        } 


        private int HuaweiPadanje()
        {
            huaweipadanje = true;
            Random r = new Random();
            int pocetniX = r.Next(0, GameOptions.RightEdge - huawei.Width);

            huawei.GotoXY(pocetniX, 0);
            huawei.Show = true;
            huawei.Zivot = 2;
            while (huaweipadanje)
            {
                huawei.Y += huawei.Brzina;
                Wait(0.05);
                
                string rub;
                if (huawei.TouchingEdge(out rub))
                {
                    if (rub == "bottom")
                    {
                        tenk.Zivot--;
                        huaweipadanje = false;

                    }
                }
                if (huawei.TouchingSprite(metak))
                {
                    tenk.bulletReady = true;
                    
                    huawei.Zivot-=metak.Snaga;
                    try
                    {
                        metak.Snaga = metak.Snaga;
                    }
                    catch (ArgumentException)
                    {
                        MessageBox.Show("Negativna snaga!");
                    }
                    metak.GotoXY(-600, -600);

                }
                if (huawei.Zivot==0)
                {
                    tenk.Bodovi++;
                    huaweipadanje = false;
                    huawei.Show = false;
                    huawei.GotoXY(-100, -100);
                    
                }
                if (tenk.Bodovi == 10)
                {
                    huawei.GotoXY(-200, -350);
                    break;
                }


            }
            return 0;
        }

        

        private int EnemyPadanje2_2()
        {
            enemya2padanje = true;
            Random t = new Random();
            int pocetniX2 = t.Next(0, GameOptions.RightEdge - enemy2.Width);

            enemy2.GotoXY(pocetniX2, 0);
            enemy2.Show = true;

            while (enemya2padanje)
            {
                enemy2.Y += enemy2.Brzina;
                Wait(0.05);
                string rub;
                if (enemy2.TouchingEdge(out rub))
                {
                    if (rub == "bottom")
                    {
                        tenk.Zivot--;
                        enemya2padanje = false;
                    }
                }
                if (enemy2.TouchingSprite(metak))
                {
                    tenk.bulletReady = true;
                    tenk.Bodovi++;
                    metak.GotoXY(-600, -600);
                    enemya2padanje = false;
                    enemy2.Show = false;
                    enemy2.GotoXY(-500, -500);
                }
                if (tenk.Bodovi == 10)
                {
                    enemy2.GotoXY(-200, -300);
                    break;
                }
            }
            return 0;
        }

       
        private int BossKretanje()
        {
            kretanjeGlavnog = true;
            helicopter.GotoXY(GameOptions.RightEdge - helicopter.Width, 10);
            helicopter.SetDirection(270);
            helicopter.Show = true;

            while (kretanjeGlavnog)
            {
                helicopter.MoveSteps(helicopter.Brzina);
                Wait(0.05);
                string rub;
                if(helicopter.TouchingEdge(out rub))
                {
                    if (rub == "left")
                    {
                        helicopter.SetDirection(90);
                    }
                    if (rub == "right")
                    {
                        helicopter.SetDirection(270);
                    }
                }
                if (helicopter.bombaReady)
                {
                    helicopter.bombaReady = false;
                    Game.StartScript(PadanjeBombe);
                    helicopter.bombaReady = false;                    
                }
                if (helicopter.TouchingSprite(metak))
                {
                    tenk.bulletReady = true;
                    helicopter.Zivot--;                    
                    metak.GotoXY(-600, -600);
                }
                if (helicopter.Zivot == 0)
                {
                    tenk.Bodovi++;
                    kretanjeGlavnog = false;
                    helicopter.GotoXY(-150, -200);
                    helicopter.bombaReady = true;
                    helicopter.Show = false;                    
                }

            }            
            return 0;
        }

        private int PadanjeBombe()
        {
            Wait(1);
            bomba.Goto_Sprite(helicopter);
            bomba.SetVisible(true);
            bomba.SetDirection(180);

            while (helicopter.bombaReady == false)
            {
                bomba.MoveSteps(bomba.Brzina);
                Wait(0.1);
                if (bomba.TouchingEdge())
                {
                    bomba.Y -= bomba.Brzina;
                    bomba.SetVisible(false);
                    bomba.GotoXY(-100, -400);
                    helicopter.bombaReady = true;
                }
                
            }
            return 0;
        }

        public void Kraj()
        {
            ISPIS = "Bodovi: " + tenk.Bodovi.ToString() + " Životi: " + tenk.Zivot.ToString();
            tenk.SetVisible(false);
            pila.SetVisible(false);
            enemy.SetVisible(false);
            enemy2.SetVisible(false);
            huawei.SetVisible(false);
            helicopter.SetVisible(false);
            bomba.SetVisible(false);
            metak.SetVisible(false);
            START = false;

            MessageBoxButtons buttons = MessageBoxButtons.RetryCancel;
            DialogResult dr = MessageBox.Show("You have failed our great nation!", "Restart", buttons);


            if (dr == DialogResult.Retry)
            {
                Application.Restart();
            }
            else
            {
                START = false;
                Application.Exit();
            }
            Wait(2);
        }

        public void Pobjeda()
        {
            helicopter.SetVisible(false);
            bomba.SetVisible(false);
            pila.SetVisible(false);
            tenk.SetVisible(false);
            START = false;
            allSprites.Clear();

            Form2 fm = new Form2();
            fm.ShowDialog();
        }
       
        /* ------------ GAME CODE END ------------ */


    }
}
