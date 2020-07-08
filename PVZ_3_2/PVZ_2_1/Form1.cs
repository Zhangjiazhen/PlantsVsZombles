using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Media;
using System.Drawing.Imaging;
using System.Threading;
using PVZ_2_1.Properties;

namespace PVZ_2_1
{
    public partial class Form1 : Form
    {
        Random ra = new Random();

        private plan _plan = plan.开始界面;
        //记录上一个游戏模式
        private plan lastplan = new plan();
        //exe文件所在目录
        private string _exePath;
        //控制最后一波的显示
        int final = 20;
        Thread ChoosePlants = null;
        Thread ModeIn = null;
        Thread BackBG = null;
        Thread CountDown = null;
        Point getPoint = MousePosition;
        //设置菜单向上滑动的效果
        int height = 600;
        Image _countdown = Resources.准备;
        //int height = 300;
        //用户名
        string username = "张佳振";
        //左边框框下落高度
        int downHeight1;
        int downHeight2;
        int downHeight3;
        //判断模式选择界面界面是否改变过
        bool chan = false;
        //设置植物选择时背景偏离位置
        int leaveX = 0;
        //设置植物选择时上升位置
        int leaveY = 513;
        //阳光数
        int sunnum = 1000;
        //已选择卡片的数量
        int chooseCard = 0;
        //确定植物
        int Splants = 0;
        //判断是否已经点击铲子或植物
        bool pick = false;
        bool shovel = false;
        bool countdown = false;
        //定义植物的单链表
        List<Plants> L_plants = new List<Plants>();
        //定义植物选择时的僵尸
        List<Zombies> L_zombies = new List<Zombies>();
        //正式游戏时的僵尸
        List<Zombies> L_zombiesF = new List<Zombies>();
        //定义产生的阳光
        List<Sun> L_sun = new List<Sun>();
        //定义子弹
        List<Bullet> L_bullet = new List<Bullet>();

        //储存拾起的植物
        plants pickPlant;
        //储存土地上已种的植物
        plants[,] land = new plants[6, 9];
        //设置非向日葵产生阳光的时间
        int SunTime = 0;
        //统计已出现僵尸的数量
        int numZombie = 0;
        //设置出现僵尸的时间间隔
        int MaxZTime = 10;
        //设置已出现僵尸的时间
        int ZombieTime = 0;
        //数组，储存卡片槽
        public cardthing[] card = new cardthing[9];
        public model _model = model.白天模式;
        Font fontSun = new Font("隶书", 15, FontStyle.Bold, GraphicsUnit.Pixel);
        SolidBrush fontBrushSun = new SolidBrush(Color.Black);
        public struct cardthing
        {
            public plants _plants;
            public int startTime;
            public bool CanChoose;
        }
        public struct plantthing
        {
            public int sum;
            public bool choose;
            public plants _plants;
            public int life;
            public int coolingTime;
            public int AttackTime;
            public int Attack;
        };
        plantthing[] _plantthing = new plantthing[12];

        //开启线程1
        public void FirstThread()
        {
            ModeIn = new Thread(new ThreadStart(oneLoad));
            ModeIn.Start();
            while (!ModeIn.IsAlive) ;
        }
        //设置进入模式选择的动画
        public void oneLoad()
        {
            int speed = 3;
            downHeight1 = -60;
            downHeight2 = -70;
            downHeight3 = -80;
            int i = -10;
            while (height > 0 || downHeight1 < 150 || downHeight2 < 150 || downHeight3 < 150)
            {
                if (height > 0) height += i;
                if (downHeight1 < 150)
                    downHeight1 += speed;
                if (downHeight2 < 150)
                    downHeight2 += speed;
                if (downHeight3 < 150)
                    downHeight3 += speed;
                pictureBox1.Invalidate();
                Thread.Sleep(10);
            }
            downHeight1 = downHeight2 = downHeight3 = 150;
            height = 0;
            pictureBox1.Invalidate();
            while (downHeight1 >= 120 || downHeight2 >= 90 || downHeight3 >= 90)
            {
                if (downHeight1 >= 120)
                    downHeight1 -= speed;
                if (downHeight2 >= 90)
                    downHeight2 -= speed;
                if (downHeight3 >= 90)
                    downHeight3 -= speed;
                pictureBox1.Invalidate();
                Thread.Sleep(10);
            }
            while (downHeight1 < 150 || downHeight2 < 150 || downHeight3 < 150)
            {
                if (downHeight1 < 150)
                    downHeight1 += speed;
                if (downHeight2 < 150)
                    downHeight2 += speed;
                if (downHeight3 < 150)
                    downHeight3 += speed;
                pictureBox1.Invalidate();
                Thread.Sleep(10);
            }
            downHeight1 = downHeight2 = downHeight3 = 150;
            height = 0;
            pictureBox1.Invalidate();
            ModeIn.Abort();
        }
        //线程二
        public void SecondThread()
        {
            ChoosePlants = new Thread(new ThreadStart(TwoLoad));
            ChoosePlants.Start();
            while (!ChoosePlants.IsAlive) ;
        }
        //设置植物选择时的动画
        public void TwoLoad()
        {
            //timer1.Enabled = true;
            //Graphics g = pictureBox1.CreateGraphics();
            while (leaveX < 600)
            {
                pictureBox1.Invalidate();
                leaveX += 10;
                Thread.Sleep(40);
            }
            while (leaveY > 0)
            {
                leaveY -= 10;
                //for (int i = 0; i < L_zombies.Count; i++)
                //    L_zombies[i].Play();
                pictureBox1.Invalidate();
                Thread.Sleep(20);
            }
            leaveY = 0;
            pictureBox1.Invalidate();
            ChoosePlants.Abort();
        }
        //线程三
        public void ThirdThread()
        {
            BackBG = new Thread(new ThreadStart(ThreeLoad));
            BackBG.Start();
            while (!BackBG.IsAlive) ;
        }
        //设置背景的回移
        public void ThreeLoad()
        {
            //timer1.Enabled = true;
            while (leaveX > 220)
            {
                leaveX -= 5;
                //for (int i = 0; i < L_zombies.Count; i++)
                //    L_zombies[i].Play();
                pictureBox1.Invalidate();
                Thread.Sleep(40);

            }
            countdown = true;
            //开启线程4
            FourthThread();
            BackBG.Abort();

        }
        //开启线程4
        public void FourthThread()
        {
            CountDown = new Thread(new ThreadStart(FourLoad));
            CountDown.Start();
            while (!CountDown.IsAlive) ;
        }
        //设置游戏开始倒计时
        public void FourLoad()
        {
            int count = _countdown.GetFrameCount(FrameDimension.Time);
            int i = 0;
            while (i < count)
            {
                _countdown.SelectActiveFrame(FrameDimension.Time, i);
                i++;
                pictureBox1.Invalidate();
                Thread.Sleep(1000);
            }
            countdown = false;
            pictureBox1.Invalidate();
            CountDown.Abort();
        }
        //类成员方法：播放声音
        public void SoundPlay(string wavFile)
        {
            //装载声音文件
            SoundPlayer soundPlay = new SoundPlayer(wavFile);
            //以同步播放方式，播放声音
            soundPlay.Play();
        }
        //定义卡片选择时的僵尸
        public void C_zombie()
        {
            Zombies b0 = new Zombies();
            Zombies b1 = new Zombies();
            Zombies b2 = new Zombies();
            Zombies b3 = new Zombies();
            Zombies b4 = new Zombies();
            Zombies b5 = new Zombies();
            Zombies b6 = new Zombies();
            Zombies b7 = new Zombies();
            Zombies b8 = new Zombies();

            b0.image = Resources.Zombie;
            b0.count = b0.image.GetFrameCount(FrameDimension.Time);
            b0.location = new Point(438, 92);

            b1.image = Resources.Zombie;
            b1.count = b1.image.GetFrameCount(FrameDimension.Time);
            b1.location = new Point(505, 108);

            b2.image = Resources.Zombie;
            b2.count = b2.image.GetFrameCount(FrameDimension.Time);
            b2.location = new Point(550, 178);

            b3.image = Resources.Zombie;
            b3.count = b3.image.GetFrameCount(FrameDimension.Time);
            b3.location = new Point(495, 211);

            b4.image = Resources.ConeheadZombie;
            b4.count = b4.image.GetFrameCount(FrameDimension.Time);
            b4.location = new Point(451, 261);

            b5.image = Resources.ConeheadZombie;
            b5.count = b5.image.GetFrameCount(FrameDimension.Time);
            b5.location = new Point(393, 192);

            b6.image = Resources.BucketheadZombie;
            b6.count = b6.image.GetFrameCount(FrameDimension.Time);
            b6.location = new Point(552, 264);

            b7.image = Resources.BucketheadZombie;
            b7.count = b7.image.GetFrameCount(FrameDimension.Time);
            b7.location = new Point(564, 348);

            b8.image = Resources.PoleVaultingZombie;
            b8.count = b8.image.GetFrameCount(FrameDimension.Time);
            b8.location = new Point(385, 390);


            L_zombies.Add(b0);
            L_zombies.Add(b1);
            L_zombies.Add(b2);
            L_zombies.Add(b3);
            L_zombies.Add(b5);
            L_zombies.Add(b4);

            L_zombies.Add(b6);
            L_zombies.Add(b7);
            L_zombies.Add(b8);


        }
        //初始化植物各类属性
        public void initializeP()
        {
            //豌豆射手
            _plantthing[0].sum = 100;
            _plantthing[0].life = 300;
            _plantthing[0].coolingTime = 75;
            _plantthing[0].AttackTime = 14;
            _plantthing[0].Attack = 20;
            //向日葵
            _plantthing[1].sum = 50;
            _plantthing[1].life = 300;
            _plantthing[1].coolingTime = 75;
            _plantthing[1].AttackTime = 240;
            _plantthing[1].Attack = 0;
            //樱桃炸弹
            _plantthing[2].sum = 150;
            _plantthing[2].life = 300;
            _plantthing[2].coolingTime = 300;
            _plantthing[2].AttackTime = 0;
            _plantthing[2].Attack = 0;
            //坚果墙
            _plantthing[3].sum = 50;
            _plantthing[3].life = 300;
            _plantthing[3].coolingTime = 300;
            _plantthing[3].AttackTime = 240;
            _plantthing[3].Attack = 0;
            //寒冰射手
            _plantthing[4].sum = 175;
            _plantthing[4].life = 300;
            _plantthing[4].coolingTime = 75;
            _plantthing[4].AttackTime = 14;
            _plantthing[4].Attack = 20;
            //双发射手
            _plantthing[5].sum = 200;
            _plantthing[5].life = 300;
            _plantthing[5].coolingTime = 75;
            _plantthing[5].AttackTime = 140;
            _plantthing[5].Attack = 20;
            //火爆辣椒
            _plantthing[6].sum = 125;
            _plantthing[6].life = 300;
            _plantthing[6].coolingTime = 300;
            _plantthing[6].AttackTime = 240;
            _plantthing[0].Attack = 20;
            //火炬树桩
            _plantthing[7].sum = 175;
            _plantthing[7].life = 300;
            _plantthing[7].coolingTime = 75;
            _plantthing[7].AttackTime = 0;
            _plantthing[7].Attack = 0;
            //机枪射手
            _plantthing[8].sum = 250;
            _plantthing[8].life = 300;
            _plantthing[8].coolingTime = 500;
            _plantthing[8].AttackTime = 140;
            _plantthing[8].Attack = 20;
            //小喷菇
            _plantthing[9].sum = 0;
            _plantthing[9].life = 300;
            _plantthing[9].coolingTime = 75;
            _plantthing[9].AttackTime = 14;
            _plantthing[9].Attack = 20;
            //_plantthing[9].choose = true;
            //阳光菇
            _plantthing[10].sum = 25;
            _plantthing[10].life = 300;
            _plantthing[10].coolingTime = 75;
            _plantthing[10].AttackTime = 240;
            _plantthing[10].Attack = 0;
            //_plantthing[10].choose = true;
            //大喷菇
            _plantthing[11].sum = 75;
            _plantthing[11].life = 300;
            _plantthing[11].coolingTime = 75;
            _plantthing[11].AttackTime = 240;
            _plantthing[11].Attack = 20;
            //_plantthing[11].choose = true;


        }
        /// <summary>
        /// 随机添加僵尸
        /// </summary>
        public void AddZombie(int x, int y)
        {
            Random na = new Random();
            Zombies myZ = new Zombies();
            int a = na.Next(x, y);
            if (a == 1) myZ.image = Resources.Zombie;
            else if (a == 2) myZ.image = Resources.Zombie2;
            else if (a == 3 || a == 4) myZ.image = Resources.Zombie3;
            else if (a == 5 || a == 6)
            {
                myZ.image = Resources.ConeheadZombie;
                myZ.Lifezombie = 560;
                myZ.pz = false;
                myZ.name = zombies.路障僵尸;
            }
            else if (a == 7 || a == 8)
            {
                myZ.image = Resources.BucketheadZombie;
                myZ.Lifezombie = 1300;
                myZ.pz = false;
                myZ.name = zombies.铁桶僵尸;
            }
            else if (a == 9)
            {
                myZ.image = Resources.PoleVaultingZombie;
                myZ.name = zombies.撑杆僵尸;
            }
            else
            {
                myZ.image = Resources.FlagZombie;
                myZ.name = zombies.摇旗僵尸;
            }
            int c = na.Next(100);
            myZ.location.X = 750 + c;
            int b = na.Next(5);
            myZ.location.Y = b * 97 + 30;
            myZ.row = b;
            if (a == 9)
            {
                myZ.location.Y -= 60;
                myZ.location.X -= 230;
            }

            myZ.count = myZ.image.GetFrameCount(FrameDimension.Time);
            L_zombiesF.Add(myZ);
            numZombie++;
        }
        /// <summary>
        /// 僵尸头落地
        /// </summary>
        /// <param name="point"></param>
        public void addZombieHead(Point point)
        {
            Zombies myZ = new Zombies();
            myZ.location = new Point(point.X + 60, point.Y);
            myZ.image = Resources.ZombieHead;
            myZ.count = myZ.image.GetFrameCount(FrameDimension.Time);
            myZ.alive = false;
            myZ.Lifezombie = 0;
            myZ.down = true;
            L_zombiesF.Add(myZ);
        }
        /// <summary>
        /// 添加子弹
        /// </summary>
        /// <param name="image"></param>
        /// <param name="location"></param>
        public void addBullet(Image image, Point location, int row, bullet _bullet)
        {
            Bullet myB = new Bullet();
            myB.name = _bullet;
            myB.image = image;
            myB.location = location;
            myB.row = row;
            L_bullet.Add(myB);

        }
        /// <summary>
        /// 僵尸吃东西
        /// </summary>
        /// <param name="myz"></param>
        private void eatZombie(Zombies myz)
        {
            myz.eat = true;
            if (myz.name == zombies.普通僵尸) myz.image = Resources.ZombieAttack;
            else if (myz.name == zombies.路障僵尸) myz.image = Resources.ConeheadZombieAttack;
            else if (myz.name == zombies.铁桶僵尸) myz.image = Resources.BucketheadZombieAttack;
            else if (myz.name == zombies.摇旗僵尸) myz.image = Resources.FlagZombieAttack;
            else myz.image = Resources.PoleVaultingZombieAttack;
            myz.index = 0;
            myz.count = myz.image.GetFrameCount(FrameDimension.Time);
        }
        /// <summary>
        /// 僵尸由攻击变为行走
        /// </summary>
        /// <param name="myz"></param>
        private void noeatZombie(Zombies myz)
        {
            myz.eat = false;
            if (myz.name == zombies.普通僵尸) myz.image = Resources.Zombie;
            else if (myz.name == zombies.路障僵尸) myz.image = Resources.ConeheadZombie;
            else if (myz.name == zombies.铁桶僵尸) myz.image = Resources.BucketheadZombie;
            else if (myz.name == zombies.摇旗僵尸) myz.image = Resources.FlagZombie;
            else myz.image = Resources.PoleVaultingZombieWalk;
            myz.index = 0;
            myz.count = myz.image.GetFrameCount(FrameDimension.Time);
        }
        /// <summary>
        /// 把坐标转换为小块土地
        /// </summary>
        /// <param name="point"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool ConvertPointToRowcol(Point point, out int row, out int col)
        {
            if (point.X >= 35 && point.Y >= 80 && point.X <= 764 && point.Y <= 566)
            {
                col = (point.X - 35) / 81;
                row = (point.Y - 80) / 97;
                return true;
            }
            else
            {
                row = col = -1;
                return false;
            }
        }
        public Form1()
        {
            //获取exe文件的所在目录和文件名
            _exePath = Process.GetCurrentProcess().MainModule.FileName;
            //获取_exePath最后一个"\"的位置
            int Location = _exePath.LastIndexOf('\\');
            //截取_exePath的目录部分
            _exePath = _exePath.Substring(0, Location + 1);
            InitializeComponent();
            //this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }
        public void initializeAll()
        {
            for (int i = 0; i < 12; i++)
                _plantthing[i].choose = false;
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 9; j++)
                    land[i, j] = plants.无;
            height = 600;
            leaveY = 513;
            sunnum = 1000;
            leaveX = 0;
            chooseCard = 0;
            pick = false;
            shovel = false;
            countdown = false;
            //链表全清空
            L_bullet.Clear();
            L_plants.Clear();
            L_sun.Clear();
            L_zombiesF.Clear();
            //lang数组全清空
            SunTime = 0;
            numZombie = 0;
            MaxZTime = 10;
        }
        //public static class AniWindowClass 
        //{
        //    [System.Runtime.InteropServices.DllImport("user32")]
        //    private static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);
        //    private const int AW_HOR_POSITIVE = 0x0001;
        //    private const int AW_HOR_NEGATIVE = 0x0002;
        //    private const int AW_VER_POSITIVE = 0x0004;
        //    private const int AW_VER_NEGATIVE = 0x0008;
        //    private const int AW_CENTER = 0x0010;
        //    private const int AW_HIDE = 0x10000;
        //    private const int AW_ACTIVATE = 0x20000;
        //    private const int AW_SLIDE = 0x40000;
        //    private const int AW_BLEND = 0x80000;
        //    private static int CloseOpen = 0x20000;
        //    public static void AniWindow(IntPtr hwnd, int dwFlags, int CloseOrOpen, System.Windows.Forms.Form myform)
        //    {
        //        try
        //        {
        //            if (CloseOrOpen == 1)
        //            {
        //                foreach (System.Windows.Forms.Control mycontrol in myform.Controls)
        //                {
        //                    string m = mycontrol.GetType().ToString();
        //                    m = m.Substring(m.Length - 5);
        //                    if (m == "Label")
        //                    {
        //                        mycontrol.Visible = false;
        //                    }
        //                }
        //            }
        //            //打开or关闭 0是关闭  1是打开
        //            if (CloseOrOpen == 0) { CloseOpen = 0x10000; }
        //            if (dwFlags == 100)
        //            {
        //                int zz = 10;
        //                Random a = new Random();
        //                dwFlags = (int)a.Next(zz);
        //            }
        //            switch (dwFlags)
        //            {
        //                case 0://普通显示
        //                    AnimateWindow(hwnd, 200, AW_ACTIVATE);
        //                    break;
        //                case 1://从左向右显示
        //                    AnimateWindow(hwnd, 200, AW_HOR_POSITIVE | CloseOpen);
        //                    break;
        //                case 2://从右向左显示
        //                    AnimateWindow(hwnd, 200, AW_HOR_NEGATIVE | CloseOpen);
        //                    break;
        //                case 3://从上到下显示
        //                    AnimateWindow(hwnd, 200, AW_VER_POSITIVE | CloseOpen);
        //                    break;
        //                case 4://从下到上显示
        //                    AnimateWindow(hwnd, 200, AW_VER_NEGATIVE | CloseOpen);
        //                    break;
        //                case 5://透明渐变显示
        //                    AnimateWindow(hwnd, 200, AW_BLEND | CloseOpen);
        //                    break;
        //                case 6://从中间向四周
        //                    AnimateWindow(hwnd, 200, AW_CENTER | CloseOpen);
        //                    break;
        //                case 7://左上角伸展
        //                    AnimateWindow(hwnd, 200, AW_SLIDE | AW_HOR_POSITIVE | AW_VER_POSITIVE | CloseOpen);
        //                    break;
        //                case 8://左下角伸展
        //                    AnimateWindow(hwnd, 200, AW_SLIDE | AW_HOR_POSITIVE | AW_VER_NEGATIVE | CloseOpen);
        //                    break;
        //                case 9://右上角伸展
        //                    AnimateWindow(hwnd, 200, AW_SLIDE | AW_HOR_NEGATIVE | AW_VER_POSITIVE | CloseOpen);
        //                    break;
        //                case 10://右下角伸展
        //                    AnimateWindow(hwnd, 200, AW_SLIDE | AW_HOR_NEGATIVE | AW_VER_NEGATIVE | CloseOpen);
        //                    break;
        //            }
        //            if (CloseOrOpen == 1)
        //            {
        //                foreach (System.Windows.Forms.Control mycontrol in myform.Controls)
        //                {
        //                    string m = mycontrol.GetType().ToString();
        //                    m = m.Substring(m.Length - 5);
        //                    if (m == "Label")
        //                    {
        //                        mycontrol.Visible = true;
        //                    }
        //                }
        //            }
        //        }
        //        catch { }
        //    }
        //}
        //Load事件
        private void Form1_Load(object sender, EventArgs e)
        {
            //初始化植物的各种属性
            initializeP();
            for (int i = 0; i < 12; i++)
                _plantthing[i].choose = false;
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 9; j++)
                    land[i, j] = plants.无;
            C_zombie();
            //AniWindowClass.AniWindow(this.Handle, 100, 1, this);
            //AniWindowClass.AniWindow(this.Handle, 100, 1, this);
            

        }
        //Paint事件
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (_plan == plan.开始界面)
            {
                e.Graphics.DrawImage(Resources.点击进入, 0, 0);
                //书写“点击开始”
                Font fontl = new Font("隶书", 20, FontStyle.Regular, GraphicsUnit.Pixel);
                SolidBrush fontBrush1 = new SolidBrush(Color.Yellow);
                SolidBrush fontBrush2 = new SolidBrush(Color.Orange);
                if (getPoint.X > 249 && getPoint.X < 542 && getPoint.Y > 542 && getPoint.Y < 577)
                    e.Graphics.DrawString("点击开始", fontl, fontBrush2, new Point(365, 547));
                else e.Graphics.DrawString("点击开始", fontl, fontBrush1, new Point(365, 547));
            }
            else if (_plan == plan.帮助界面)
            {
                e.Graphics.DrawImage(Resources.help, 0, 0);
            }
            else if (_plan == plan.胜利界面)
            {
                e.Graphics.DrawImage(Resources.win, 0, 0);
            }
            else if (_plan == plan.模式选择 || _plan == plan.确定退出)
            {
                e.Graphics.DrawImage(Resources.模式选择背景, 0, 0);
                e.Graphics.DrawImage(Resources.菜单墓碑, 0, height);
                //右边四个模式
                e.Graphics.DrawImage(Resources.冒险模式1, 412, height + 85, 330, 120);
                e.Graphics.DrawImage(Resources.玩玩小游戏1, 412, height + 175, 313, 133);
                e.Graphics.DrawImage(Resources.解谜模式1, 415, height + 255, 286, 122);
                e.Graphics.DrawImage(Resources.生存模式1, 415, height + 325, 266, 123);
                //书写“选项”，“帮助”，“退出”                
                e.Graphics.DrawImage(Resources.选项1, 565, 495 + height, 81, 31);
                e.Graphics.DrawImage(Resources.帮助1, 650, 525 + height, 48, 22);
                e.Graphics.DrawImage(Resources.退出1, 720, 517 + height, 47, 27);
                //左边三个框new Bitmap(_exePath + "Image\\SelectorScreen_WoodSign1.png")
                e.Graphics.DrawImage(Resources.欢迎回来, 30, -150 + downHeight1, 293, 150);
                e.Graphics.DrawImage(Resources.点击我1, 30, -150 + downHeight2 + 140, 291, 71);
                e.Graphics.DrawImage(Resources.木头, 30, -150 + downHeight3 + 140 + 51, 92, 40);
                //写用户名 
                Font font = new Font("方正舒体", 20, FontStyle.Regular, GraphicsUnit.Pixel);
                SolidBrush fontBrush = new SolidBrush(Color.White);
                e.Graphics.DrawString(username, font, fontBrush, new Point(120, downHeight1 - 55));
                if (_plan == plan.确定退出)
                    e.Graphics.DrawImage(Resources.确定退出, 180, 140);
            }
            else if (_plan == plan.植物选择界面||lastplan==plan.植物选择界面)
            {
                int i;
                if (_model == model.白天模式) e.Graphics.DrawImage(Resources.白天背景, -leaveX, 0);
                else e.Graphics.DrawImage(Resources.黑夜背景, -leaveX, 0);
                e.Graphics.DrawImage(Resources.卡片选择栏, 0, 87 + leaveY);
                //书写“一起摇滚吧”
                if (chooseCard == 6)
                {
                    e.Graphics.DrawImage(Resources.摇滚背景2, 157, 547 + leaveY);
                    e.Graphics.DrawString("一起摇滚吧！", new Font("宋体", 20, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.Yellow), 177, 558 + leaveY);
                }
                else
                {
                    e.Graphics.DrawImage(Resources.摇滚背景1, 157, 547 + leaveY);
                    e.Graphics.DrawString("一起摇滚吧！", new Font("宋体", 20, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.YellowGreen), 177, 558 + leaveY);
                }

                //卡片选择栏显示卡片
                for (i = 0; i < 8; i++)
                {
                    if (_plantthing[i].choose == false)
                        e.Graphics.DrawImage(new Bitmap(_exePath + "card\\p" + (i + 1).ToString() + ".jpg"), 24 + 52 * i, 128 + leaveY);
                    else e.Graphics.DrawImage(new Bitmap(_exePath + "card\\p" + (i + 1).ToString() + "_1.jpg"), 24 + 52 * i, 128 + leaveY);
                }
                if (_plantthing[8].choose == false)
                    e.Graphics.DrawImage(new Bitmap(_exePath + "card\\p9.jpg"), 24, 199 + leaveY);
                else e.Graphics.DrawImage(new Bitmap(_exePath + "card\\p9_1.jpg"), 24, 199 + leaveY);
                //白天模式下对夜间植物
                if (_model == model.白天模式)
                    for (i = 1; i < 4; i++)
                    {
                        if (_plantthing[i + 8].choose == false)
                            e.Graphics.DrawImage(new Bitmap(_exePath + "card\\p" + (i + 9).ToString() + "_2.jpg"), 24 + 52 * i, 199 + leaveY);
                        else e.Graphics.DrawImage(new Bitmap(_exePath + "card\\p" + (i + 9).ToString() + "_1.jpg"), 24 + 52 * i, 199 + leaveY);
                    }
                //黑夜模式下对夜间植物
                else
                    for (i = 1; i < 4; i++)
                    {
                        if (_plantthing[i + 8].choose == false)
                            e.Graphics.DrawImage(new Bitmap(_exePath + "card\\p" + (i + 9).ToString() + ".jpg"), 24 + 52 * i, 199 + leaveY);
                        else e.Graphics.DrawImage(new Bitmap(_exePath + "card\\p" + (i + 9).ToString() + "_1.jpg"), 24 + 52 * i, 199 + leaveY);
                    }

                if (leaveX >= 600)
                {
                    //画卡片槽
                    e.Graphics.DrawImage(Resources.卡片槽, 0, 0);
                    e.Graphics.DrawString(sunnum.ToString(), fontSun, fontBrushSun, 26, 62);
                    for (i = 0; i < 6; i++)
                    {
                        e.Graphics.DrawImage(Resources.卡片背景, 80 + 55 * i, 9);
                    }
                    for (i = 0; i < chooseCard; i++)
                    {
                        e.Graphics.DrawImage(new Bitmap(_exePath + "card\\p" + (int)card[i]._plants + ".jpg"), 80 + 55 * i, 9);
                    }
                }
                foreach (Zombies myZombie in L_zombies)
                {
                    myZombie.Display(e.Graphics, new Point(myZombie.location.X + 600 - leaveX, myZombie.location.Y));
                }
                //菜单栏
                e.Graphics.DrawImage(Resources.Button, 680, 0);
                e.Graphics.DrawString("菜单", new Font("宋体", 23, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.LimeGreen), 705, 10);
            }

            //游戏界面
            else if (_plan == plan.游戏界面 || lastplan==plan.游戏界面 || _plan == plan.失败界面)
            {
                int row, col;
                bool _convert = ConvertPointToRowcol(getPoint, out row, out col);
                if (_model == model.白天模式) e.Graphics.DrawImage(Resources.白天背景, -leaveX, 0);
                else e.Graphics.DrawImage(Resources.黑夜背景, -leaveX, 0);
                if (countdown)
                {
                    e.Graphics.DrawImage(_countdown, 285, 280);
                }                
                //背景不动后，画卡片槽
                if (leaveX <= 220)
                {
                    //菜单栏
                    e.Graphics.DrawImage(Resources.Button, 680, 0);
                    e.Graphics.DrawString("菜单", new Font("宋体", 23, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.LimeGreen), 705, 10);
                    //画卡片槽
                    e.Graphics.DrawImage(Resources.卡片槽, 0, 0);
                    e.Graphics.DrawString(sunnum.ToString(), fontSun, fontBrushSun, 26, 62);
                    for (int i = 0; i < 6; i++)
                    {
                        e.Graphics.DrawImage(Resources.卡片背景, 80 + 55 * i, 9);
                    }
                    for (int i = 0; i < chooseCard; i++)
                    {
                        if (sunnum >= _plantthing[(int)card[i]._plants - 1].sum && card[i].CanChoose)
                            e.Graphics.DrawImage(new Bitmap(_exePath + "card\\p" + (int)card[i]._plants + ".jpg"), 80 + 55 * i, 9);
                        else if (card[i].CanChoose)
                        {
                            e.Graphics.DrawImage(new Bitmap(_exePath + "card\\p" + (int)card[i]._plants + "_2.jpg"), 80 + 55 * i, 9);
                        }
                        else
                        {
                            e.Graphics.DrawImage(new Bitmap(_exePath + "card\\p" + (int)card[i]._plants + "_2.jpg"), 80 + 55 * i, 9);
                            e.Graphics.DrawImageUnscaledAndClipped(new Bitmap(_exePath + "card\\p" + (int)card[i]._plants + "_1.jpg"),
                              new Rectangle(80 + 55 * i, 9, 50, 70 - 70 * card[i].startTime / _plantthing[(int)card[i]._plants - 1].coolingTime));
                            //e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Gray)), new Rectangle(80 + 55 * i, 9, 50, 70 - 70 * card[i].startTime / _plantthing[(int)card[i]._plants - 1].coolingTime));  
                        }
                    }
                    if (pick && shovel)
                        e.Graphics.DrawImage(Resources.铲子2, 447, 0);
                    else e.Graphics.DrawImage(Resources.铲子1, 447, 0);
                }
                else
                {
                    foreach (Zombies myZombie in L_zombies)
                    {
                        myZombie.Display(e.Graphics, new Point(myZombie.location.X + 600 - leaveX, myZombie.location.Y));
                    }
                }
                //画植物
                foreach (Plants myBullet in L_plants)
                {
                    myBullet.Diaplay(e.Graphics);
                }
                //画僵尸
                //foreach (Zombies myZ in L_zombiesF)
                //    myZ.Display(e.Graphics, myZ.location);
                for (int i = 0; i < 6; i++)
                {

                    for (int j = 0; j < L_zombiesF.Count; j++)
                    {
                        if (L_zombiesF[j].row == i)
                            L_zombiesF[j].Display(e.Graphics, L_zombiesF[j].location);
                    }
                }
                //画子弹
                foreach (Bullet myB in L_bullet)
                    myB.display(e.Graphics);
                //画阳光
                foreach (Sun Mysun in L_sun)
                {
                    Mysun.Display(e.Graphics);
                }
                //如果拾起了铲子
                if (pick && shovel)
                {
                    e.Graphics.DrawImage(Resources.铲子3, getPoint.X - 12, getPoint.Y - 60);
                }
                //如果拾起了植物
                else if (pick && !shovel)
                {
                    //使图半透明化
                    float[][] nArray = { new float[] { 1, 0, 0, 0, 0 }, 
                                           new float[] { 0, 1, 0, 0, 0 }, 
                                           new float[] { 0, 0, 1, 0, 0 }, 
                                           new float[] { 0, 0, 0, 0.5f, 0 },
                                           new float[] { 0, 0, 0, 0, 1 } }; 
                    ColorMatrix matrix = new ColorMatrix(nArray); 
                    ImageAttributes attributes = new ImageAttributes(); 
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    Bitmap kBit = new Bitmap(_exePath + "image\\Plants\\" + (int)pickPlant + ".gif");
                    if (_convert && land[row, col] == plants.无 && pickPlant != plants.机枪射手 || _convert && land[row, col] == plants.双发射手 && pickPlant == plants.机枪射手)
                        e.Graphics.DrawImage(kBit, new Rectangle(35 + col * 81, 90 + row * 97,kBit.Width,kBit.Height), 0,0,kBit.Width,kBit.Height, GraphicsUnit.Pixel, attributes);
                    e.Graphics.DrawImage(new Bitmap(_exePath + "image\\Plants\\" + (int)pickPlant + ".gif"), getPoint.X - 12, getPoint.Y - 60);
                }
                if (numZombie >= 5 && final >= 0)
                {
                    e.Graphics.DrawImage(Resources.最后一波, 185, 280,492,134);
                }
                //失败界面
                else if (_plan == plan.失败界面)
                {
                    e.Graphics.DrawImage(Resources.ZombiesWon, 120, 70);
                }
            }
            //暂停界面
            if (_plan == plan.暂停界面)
                e.Graphics.DrawImage(Resources.options_menuback, 214, 40); 
        }
        //鼠标移动响应
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Font font1 = new Font("方正舒体", 35, FontStyle.Bold, GraphicsUnit.Pixel);
            Font font2 = new Font("方正舒体", 25, FontStyle.Bold, GraphicsUnit.Pixel);

            SolidBrush fontBrush1 = new SolidBrush(Color.LimeGreen);
            SolidBrush fontBrush4 = new SolidBrush(Color.Black);
            Font fontl = new Font("隶书", 20, FontStyle.Regular, GraphicsUnit.Pixel);
            SolidBrush fontBrush3 = new SolidBrush(Color.Yellow);
            SolidBrush fontBrush2 = new SolidBrush(Color.Orange);
            Graphics g = pictureBox1.CreateGraphics();

            //开始界面时
            if (_plan == plan.开始界面)
            {
                //鼠标移动到方框内时
                if (e.X > 249 && e.X < 542 && e.Y > 542 && e.Y < 577)
                {
                    g.DrawString("点击开始", fontl, fontBrush2, new Point(365, 547));
                    this.Cursor = Cursors.Hand;
                }
                else
                {
                    g.DrawString("点击开始", fontl, fontBrush3, new Point(365, 547));
                    this.Cursor = Cursors.Default;
                }
            }
            //帮助界面
            else if (_plan == plan.帮助界面)
            {
                if (e.X >= 327 && e.X <= 478 && e.Y >= 522 && e.Y <= 558)
                    this.Cursor = Cursors.Hand;
                else this.Cursor = Cursors.Default;
            }
            //胜利界面
            else if (_plan == plan.胜利界面)
            {
                if (e.X >= 327 && e.X <= 478 && e.Y >= 522 && e.Y <= 558)
                    this.Cursor = Cursors.Hand;
                else this.Cursor = Cursors.Default;
            }
            //失败界面
            else if (_plan == plan.失败界面)
            {
                if (new Rectangle(new Point(680, 0), Resources.Button.Size).IntersectsWith(new Rectangle(e.Location, new Size(1, 1))))
                    this.Cursor = Cursors.Hand;
                else this.Cursor = Cursors.Default;
            }
            //确定退出界面
            else if (_plan == plan.确定退出)
            {
                //退出游戏
                if (e.X >= 211 && e.X <= 365 && e.Y >= 316 && e.Y <= 351)
                    this.Cursor = Cursors.Hand;
                //取消
                else if (e.X >= 390 && e.X <= 543 && e.Y >= 315 && e.Y <= 349)
                    this.Cursor = Cursors.Hand;
                else this.Cursor = Cursors.Default;
            }
            //模式选择界面时
            else if (_plan == plan.模式选择)
            {
                //如果移动到请点击我的区域时
                if (e.X > 40 && e.X < 306 && e.Y > 154 && e.Y < 184)
                {
                    chan = true;
                    g.DrawImage(Resources.点击我2, 30, -150 + downHeight1 + 140, 291, 71);
                    g.DrawImage(Resources.木头, 30, -150 + downHeight1 + 140 + 51, 92, 40);
                    this.Cursor = Cursors.Hand;

                }
                //移动到冒险模式    
                else if (e.X > 423 && e.X < 734 && e.Y > 92 && e.Y < 193)
                {
                    chan = true;
                    g.DrawImage(Resources.冒险模式2, 412, height + 85, 330, 120);
                    this.Cursor = Cursors.Hand;
                    g.DrawImage(Resources.玩玩小游戏1, 412, height + 175, 313, 133);
                }
                //移动到玩玩小游戏  
                else if (e.X > 424 && e.X < 711 && e.Y > 193 && e.Y < 288)
                {
                    chan = true;
                    g.DrawImage(Resources.玩玩小游戏2, 412, height + 175, 313, 133);
                    this.Cursor = Cursors.Hand;
                    g.DrawImage(Resources.冒险模式1, 412, height + 85, 330, 120);
                    g.DrawImage(Resources.解谜模式1, 415, height + 255, 286, 122);
                }
                //移动到解迷模式  
                else if (e.X > 423 && e.X < 694 && e.Y > 288 && e.Y < 363)
                {
                    chan = true;
                    g.DrawImage(Resources.解谜模式2, 415, height + 255, 286, 122);
                    this.Cursor = Cursors.Hand;
                    g.DrawImage(Resources.玩玩小游戏1, 412, height + 175, 313, 133);
                    g.DrawImage(Resources.生存模式1, 415, height + 325, 266, 123);

                }
                //移动到生存模式  
                else if (e.X > 426 && e.X < 672 && e.Y > 363 && e.Y < 455)
                {
                    chan = true;
                    g.DrawImage(Resources.生存模式2, 415, height + 325, 266, 123);
                    this.Cursor = Cursors.Hand;
                    g.DrawImage(Resources.解谜模式1, 415, height + 255, 286, 122);

                }
                //移动到“选项”  
                else if (e.X > 565 && e.X < 646 && e.Y > 495 && e.Y < 526)
                {
                    chan = true;
                    this.Cursor = Cursors.Hand;
                    g.DrawImage(Resources.选项2, 565, 495 + height, 81, 31);
                }
                //移动到“帮助”  
                else if (e.X > 650 && e.X < 698 && e.Y > 525 && e.Y < 547)
                {
                    chan = true;
                    this.Cursor = Cursors.Hand;
                    g.DrawImage(Resources.帮助2, 650, 525 + height, 48, 22);
                }
                //移动到“退出”  
                else if (e.X > 720 && e.X < 767 && e.Y > 517 && e.Y < 544)
                {
                    chan = true;
                    this.Cursor = Cursors.Hand;
                    g.DrawImage(Resources.退出2, 720, 517 + height, 47, 27);
                }
                else
                {
                    if (chan == true)
                    {
                        g.DrawImage(Resources.冒险模式1, 412, height + 85, 330, 120);
                        g.DrawImage(Resources.玩玩小游戏1, 412, height + 175, 313, 133);
                        g.DrawImage(Resources.解谜模式1, 415, height + 255, 286, 122);
                        g.DrawImage(Resources.生存模式1, 415, height + 325, 266, 123);
                        g.DrawImage(Resources.点击我1, 30, -150 + downHeight1 + 140, 291, 71);
                        g.DrawImage(Resources.木头, 30, -150 + downHeight1 + 140 + 51, 92, 40);
                        g.DrawImage(Resources.选项1, 565, 495 + height, 81, 31);
                        g.DrawImage(Resources.帮助1, 650, 525 + height, 48, 22);
                        g.DrawImage(Resources.退出1, 720, 517 + height, 47, 27);
                        this.Cursor = Cursors.Default;
                        chan = false;
                    }
                }
            }
            //游戏界面
            else if (_plan == plan.游戏界面)
            {
                getPoint.X = e.X;
                getPoint.Y = e.Y;
                //如果没拾起
                if (!pick && !shovel)
                {
                    if (e.X >= 447 && e.X <= 517 && e.Y >= 0 && e.Y <= 72)
                        this.Cursor = Cursors.Hand;
                    else this.Cursor = Cursors.Default;
                }
            }
        }
        //鼠标点击响应
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Graphics g = pictureBox1.CreateGraphics();
            //开始界面时的点击响应
            if (_plan == plan.开始界面 && e.X > 249 && e.X < 542 && e.Y > 542 && e.Y < 577 && e.Button == MouseButtons.Left)
            {
                _plan = plan.模式选择;
                lastplan = plan.开始界面;
                this.Cursor = Cursors.Default;
                //启动线程一
                FirstThread();
            }
            else if (_plan == plan.模式选择)
            {
                //点到冒险模式时
                if (e.X > 423 && e.X < 734 && e.Y > 92 && e.Y < 193 && e.Button == MouseButtons.Left)
                {
                    _plan = plan.植物选择界面;
                    lastplan = plan.模式选择;
                    this.Cursor = Cursors.Default;
                    timer1.Enabled = true;
                    //启动线程二
                    SecondThread();
                }
                //点到退出时
                else if (e.X > 720 && e.X < 767 && e.Y > 517 && e.Y < 544 && e.Button == MouseButtons.Left)
                {
                    _plan = plan.确定退出;
                    pictureBox1.Invalidate();
                    this.Cursor = Cursors.Default;
                }
                //点到帮助时
                else if (e.X > 650 && e.X < 698 && e.Y > 525 && e.Y < 547)
                {
                    _plan = plan.帮助界面;
                    lastplan = plan.模式选择;
                    pictureBox1.Invalidate();
                }
            }

            else if (_plan == plan.植物选择界面)
            {
                //如果点到菜单栏
                if (new Rectangle(new Point(680, 0), Resources.Button.Size).IntersectsWith(new Rectangle(e.Location, new Size(1, 1))))
                {
                    timer1.Enabled = false;
                    lastplan = plan.植物选择界面;
                    _plan = plan.暂停界面;

                }
                //确定点击的是哪种植物
                //点击第一排的植物
                if (e.X >= 24 && e.X < 440 && e.Y >= 128 + leaveY && e.Y <= 198 + leaveY && chooseCard <= 5 && e.Button == MouseButtons.Left)
                {
                    Splants = (e.X - 24) / 52 + 1;
                    //检查是否已被选择过
                    for (int i = 0; i <= chooseCard; i++)
                    {
                        if ((int)card[i]._plants == Splants) return;
                    }
                    card[chooseCard]._plants = (plants)Splants;
                    _plantthing[Splants - 1].choose = true;
                    chooseCard++;
                }
                //点击第二排的植物
                else if (e.X >= 24 && e.X < 232 && e.Y >= 199 + leaveY && e.Y <= 269 + leaveY && chooseCard <= 5 && e.Button == MouseButtons.Left)
                {
                    Splants = (e.X - 24) / 52 + 9;
                    for (int i = 0; i <= chooseCard; i++)
                    {
                        if ((int)card[i]._plants == Splants) return;
                    }
                    card[chooseCard]._plants = (plants)Splants;
                    _plantthing[Splants - 1].choose = true;
                    chooseCard++;
                }

                //点击卡片槽时，把卡片下架
                for (int i = 0; i < chooseCard; i++)
                {
                    if (e.X >= 80 + 55 * i && e.X <= 130 + 55 * i && e.Y >= 9 && e.Y <= 79 && e.Button == MouseButtons.Left)
                    {
                        _plantthing[(int)card[i]._plants - 1].choose = false;
                        //卡片槽里卡片下架
                        for (int j = i; j < chooseCard; j++)
                        {
                            card[j] = card[j + 1];
                        }
                        card[chooseCard]._plants = plants.无;
                        chooseCard--;

                    }
                }
                if (chooseCard == 6)
                {
                    //如果点击“一起摇滚吧！”
                    if (e.X >= 157 && e.X <= 313 && e.Y >= 547 && e.Y <= 589 && e.Button == MouseButtons.Left)
                    {
                        _plan = plan.游戏界面;
                        lastplan = plan.暂停界面;
                        //初始化卡片槽的属性
                        for (int j = 0; j < 9; j++)
                        {
                            card[j].startTime = 0;
                            card[j].CanChoose = true;
                        }
                        //启动线程三
                        ThirdThread();
                    }
                }
                pictureBox1.Invalidate();
            }
            //游戏界面
            else if (_plan == plan.游戏界面)
            {
                //如果没有拾起铲子或植物
                if (!shovel && !pick)
                {
                    //如果点到阳光
                    for (int i = 0; i < L_sun.Count; i++)
                    {
                        if (L_sun[i].GetRange().IntersectsWith(new Rectangle(e.Location, new Size(1, 1))))
                        {
                            L_sun[i].click = true;
                            return;
                        }
                    }
                    //如果点到铲子区域
                    if (e.X >= 447 && e.X <= 517 && e.Y >= 0 && e.Y <= 72 && e.Button == MouseButtons.Left && !countdown)
                    {
                        shovel = true;
                        pick = true;
                        //this.Cursor = Cursors.Hand;
                    }
                    else
                    {
                        for (int i = 0; i < chooseCard; i++)
                        {
                            //如果点到卡片槽的其中卡片
                            if (e.X >= 80 + 55 * i && e.X <= 130 + 55 * i && e.Y >= 9 && e.Y <= 79 && e.Button == MouseButtons.Left
                                && sunnum >= _plantthing[(int)card[i]._plants - 1].sum && card[i].CanChoose && !countdown)
                            {
                                shovel = false;
                                pick = true;
                                pickPlant = card[i]._plants;
                            }
                        }

                    }
                    //如果点到菜单栏
                    if (new Rectangle(new Point(680, 0), Resources.Button.Size).IntersectsWith(new Rectangle(e.Location, new Size(1, 1))))
                    {
                        timer1.Enabled = false;
                        lastplan = plan.游戏界面;
                        _plan = plan.暂停界面;

                    }
                }
                //如果已经拾起铲子或植物
                else
                {
                    int row, col;
                    bool _convert = ConvertPointToRowcol(new Point(e.X, e.Y), out row, out col);
                    //点鼠标右键，取消拾起
                    if (e.Button == MouseButtons.Right)
                    { shovel = false; pick = false; }
                    //如果拾起了铲子
                    if (shovel && e.Button == MouseButtons.Left && pick)
                    {
                        shovel = false;
                        pick = false;
                        this.Cursor = Cursors.Default;
                        land[row, col] = plants.无;
                        for (int k = 0; k < L_plants.Count; k++)
                        {
                            if ((L_plants[k].location.X - 35) / 81 == col && (L_plants[k].location.Y - 90) / 97 == row)
                            {
                                L_plants.RemoveAt(k);
                                k--;
                            }
                        }
                    }
                    //如果拾起了植物
                    else if (!shovel && pick && e.Button == MouseButtons.Left)
                    {
                        if (_convert && land[row, col] == plants.无 && pickPlant != plants.机枪射手)
                        {
                            pick = false;
                            shovel = false;
                            this.Cursor = Cursors.Default;
                            land[row, col] = pickPlant;
                            sunnum -= _plantthing[(int)pickPlant - 1].sum;
                            for (int i = 0; i < 6; i++)
                            {
                                if (card[i]._plants == pickPlant)
                                {
                                    card[i].CanChoose = false;
                                }
                            }
                            //添加植物链表
                            Plants myPlants = new Plants();
                            myPlants.location = new Point(35 + col * 81, 90 + row * 97);
                            myPlants.image = Image.FromFile(_exePath + "image\\plants\\" + (int)pickPlant + ".gif");
                            myPlants.count = myPlants.image.GetFrameCount(FrameDimension.Time);
                            myPlants._plants = pickPlant;
                            if (pickPlant == plants.双发射手) myPlants.BulletNum = 2;
                            else if (pickPlant == plants.坚果墙) myPlants.life = 4000;
                            L_plants.Add(myPlants);

                            pickPlant = plants.无;
                        }
                        //对机枪射手单独处理
                        else if (_convert && land[row, col] == plants.双发射手 && pickPlant == plants.机枪射手)
                        {
                            pick = false;
                            shovel = false;
                            this.Cursor = Cursors.Default;
                            land[row, col] = pickPlant;
                            sunnum -= _plantthing[(int)pickPlant - 1].sum;
                            for (int i = 0; i < 6; i++)
                            {
                                if (card[i]._plants == pickPlant)
                                {
                                    card[i].CanChoose = false;
                                }
                            }
                            for (int j = 0; j < L_plants.Count; j++)
                            {
                                if ((L_plants[j].location.X - 35) / 81 == col && (L_plants[j].location.Y - 90) / 97 == row)
                                    L_plants.RemoveAt(j);
                            }
                            //添加植物链表
                            Plants myPlants = new Plants();
                            myPlants.location = new Point(35 + col * 81, 90 + row * 97);
                            myPlants.image = Image.FromFile(_exePath + "image\\plants\\" + (int)pickPlant + ".gif");
                            myPlants.count = myPlants.image.GetFrameCount(FrameDimension.Time);
                            myPlants._plants = pickPlant;
                            myPlants.BulletNum = 4;
                            L_plants.Add(myPlants);
                            pickPlant = plants.无;
                        }
                    }
                }

                pictureBox1.Invalidate();
            }
            else if (_plan == plan.暂停界面)
            {
                //返回游戏
                if (e.X >= 301 && e.X <= 522 & e.Y >= 437 && e.Y <= 491)
                {
                    if (lastplan == plan.游戏界面)
                        _plan = plan.游戏界面;
                    else if (lastplan == plan.植物选择界面)
                        _plan = plan.植物选择界面;
                    timer1.Enabled = true;
                }
                //主菜单
                else if (e.X >= 346 && e.X <= 485 && e.Y >= 332 && e.Y <= 377)
                {
                    _plan = plan.模式选择;
                    //初始化所有东西
                    initializeAll();
                    //启动线程一
                    FirstThread();
                }
            }
            else if (_plan == plan.确定退出)
            {
                //退出游戏
                if (e.X >= 211 && e.X <= 365 && e.Y >= 316 && e.Y <= 351)
                {
                    this.Close();
                }
                //取消
                else if (e.X >= 390 && e.X <= 543 && e.Y >= 315 && e.Y <= 349)
                {
                    _plan = plan.模式选择;
                    //启动线程一
                    FirstThread();
                }
            }
            else if (_plan == plan.帮助界面 || _plan == plan.胜利界面)
            {
                //点到主菜单
                if (e.X >= 327 && e.X <= 478 && e.Y >= 522 && e.Y <= 558)
                {
                    _plan = plan.模式选择;
                    //初始化所有东西
                    initializeAll();
                    //启动线程一
                    FirstThread();
                }

            }
            else if (_plan == plan.失败界面)
            {
                if (new Rectangle(new Point(680, 0), Resources.Button.Size).IntersectsWith(new Rectangle(e.Location, new Size(1, 1))))
                {
                    _plan = plan.模式选择;
                    //初始化所有东西
                    initializeAll();
                    //启动线程一
                    FirstThread();
                }
            }
        }
        /// <summary>
        ///定时器，用来控制gif动画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_plan == plan.游戏界面)
            {
                //更新已种植物的帧动画
                for (int i = 0; i < L_plants.Count; i++)
                {
                    L_plants[i].Play();
                    L_plants[i].attack();
                    //植物产生阳光
                    if (L_plants[i]._plants == plants.向日葵)
                    {
                        L_plants[i].attack();
                        if (L_plants[i].startTime >= 400)
                        {
                            L_plants[i].startTime = 0;
                            Sun mysun = new Sun();
                            mysun.location = new Point(L_plants[i].location.X + 10, L_plants[i].location.Y);
                            mysun.length = L_plants[i].location.Y + 20;
                            L_sun.Add(mysun);
                        }
                    }
                    //炸弹爆炸
                    else if (L_plants[i]._plants == plants.樱桃炸弹)
                    {
                        if (L_plants[i].index == L_plants[i].count - 1 && !L_plants[i].bomb)
                        {
                            L_plants[i].image = Resources.炸;
                            L_plants[i].index = 0;
                            L_plants[i].count = L_plants[i].image.GetFrameCount(FrameDimension.Time);
                            L_plants[i].startTime = 0;
                            L_plants[i].bomb = true;
                            land[(L_plants[i].location.Y - 90) / 97, (L_plants[i].location.X - 35) / 81] = plants.无;
                            L_plants[i].location.X -= 50;
                            L_plants[i].location.Y -= 40;
                            for (int j = 0; j < L_zombiesF.Count; j++)
                            {
                                if (new Rectangle(L_plants[i].location.X, L_plants[i].location.Y, 244, 200).IntersectsWith(L_zombiesF[j].GetRange()))
                                {
                                    L_zombiesF[j].image = Resources.BoomDie;
                                    L_zombiesF[j].location.X -= 130;
                                    L_zombiesF[j].location.Y -= 60;
                                    L_zombiesF[j].index = 0;
                                    L_zombiesF[j].Lifezombie = 0;
                                    L_zombiesF[j].alive = false;
                                    L_zombiesF[j].down = true;
                                    L_zombiesF[j].count = L_zombiesF[j].image.GetFrameCount(FrameDimension.Time);
                                    L_zombiesF[j].eat = true;
                                }
                            }
                        }
                        else if (L_plants[i].startTime >= 5 && L_plants[i].bomb)
                        {
                            L_plants.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                    //火爆辣椒
                    else if (L_plants[i]._plants == plants.火爆辣椒)
                    {
                        if (L_plants[i].index == L_plants[i].count - 1 && !L_plants[i].bomb)
                        {
                            L_plants[i].image = Resources.火爆辣椒攻击;
                            L_plants[i].index = 0;
                            L_plants[i].count = L_plants[i].image.GetFrameCount(FrameDimension.Time);
                            L_plants[i].startTime = 0;
                            L_plants[i].bomb = true;
                            land[(L_plants[i].location.Y - 90) / 97, (L_plants[i].location.X - 35) / 81] = plants.无;
                            L_plants[i].location.X = 20;
                            L_plants[i].location.Y -= 40;
                            for (int j = 0; j < L_zombiesF.Count; j++)
                            {
                                if (L_zombiesF[j].row == (L_plants[i].location.Y - 50) / 97)
                                {
                                    L_zombiesF[j].image = Resources.BoomDie;
                                    L_zombiesF[j].index = 0;
                                    L_zombiesF[j].Lifezombie = 0;
                                    L_zombiesF[j].location.X -= 130;
                                    L_zombiesF[j].location.Y -= 60;
                                    L_zombiesF[j].alive = false;
                                    L_zombiesF[j].down = true;
                                    L_zombiesF[j].count = L_zombiesF[j].image.GetFrameCount(FrameDimension.Time);
                                    L_zombiesF[j].eat = true;
                                }
                            }
                        }
                        else if (L_plants[i].startTime >= 5 && L_plants[i].bomb)
                        {

                            L_plants.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                    //控制子弹的发射间隔
                    if ((L_plants[i]._plants == plants.豌豆射手 || L_plants[i]._plants == plants.双发射手 || L_plants[i]._plants == plants.寒冰射手 || L_plants[i]._plants == plants.机枪射手)
                        && L_plants[i].startTime > 30)
                    {
                        L_plants[i].startTime = 0;
                    }
                }
                //更新阳光
                for (int i = 0; i < L_sun.Count; i++)
                {
                    L_sun[i].Play();
                    if (L_sun[i].location.Y < L_sun[i].length || L_sun[i].click) L_sun[i].Move();
                    if (L_sun[i].k <= 0)
                    {
                        L_sun.RemoveAt(i);
                        i = 0;
                        sunnum += 25;
                    }
                    if (L_sun.Count > 0)
                        if (L_sun[i].lifeTime <= 0 && !L_sun[i].click)
                            L_sun.RemoveAt(i);
                }
                //更新僵尸属性
                for (int i = 0; i < L_zombiesF.Count; i++)
                {
                    L_zombiesF[i].Play();
                    L_zombiesF[i].move();
                    if (L_zombiesF[i].location.X < -90)
                    {
                        _plan = plan.失败界面;
                        timer1.Enabled = false;
                        pictureBox1.Invalidate();
                    }
                    if (L_zombiesF[i].coolStarttime >= 200)
                        L_zombiesF[i].slow = false;
                    if (!L_zombiesF[i].alive && L_zombiesF[i].index == L_zombiesF[i].count - 1)
                    {
                        if (L_zombiesF[i].down)
                        {
                            L_zombiesF.RemoveAt(i);
                            i--;
                            //判断是否胜利
                            if (L_zombiesF.Count == 0 && numZombie >= 10)
                            {
                                _plan = plan.胜利界面;
                                timer1.Enabled = false;
                                pictureBox1.Invalidate();
                            }
                        }
                        else
                        {
                            L_zombiesF[i].image = Resources.ZombieDie;
                            L_zombiesF[i].count = L_zombiesF[i].image.GetFrameCount(FrameDimension.Time);
                            L_zombiesF[i].index = 0;
                            L_zombiesF[i].down = true;
                        }
                    }
                }
                if (leaveX <= 220 && !countdown)
                {
                    //控制非植物产生的阳光
                    SunTime++;
                    //控制僵尸的出现时间间隔
                    ZombieTime++;
                    //出现僵尸
                    if (ZombieTime >= MaxZTime)
                    {
                        ZombieTime = 0;
                        if (numZombie == 0)
                        {
                            AddZombie(1, 4);
                        }
                        else if (numZombie >= 1 && numZombie <= 3)
                        {
                            MaxZTime = 400;
                            AddZombie(1, 9);
                        }
                        else if (numZombie == 4)
                        {
                            AddZombie(10, 11);
                            for (int i = 0; i < 6; i++)
                                AddZombie(1, 9);
                        }

                    }
                }
                if (numZombie >= 5 && final >= 0) final--;
                //出现阳光
                if (SunTime >= 85)
                {
                    Sun a = new Sun();
                    a.location.X = ra.Next(10, 700);
                    a.length = ra.Next(100, 500);
                    L_sun.Add(a);
                    SunTime = 0;
                }
                //更新卡片显示（冷却）
                for (int i = 0; i < 6; i++)
                {
                    if (!card[i].CanChoose)
                        card[i].startTime++;
                    if (card[i].startTime >= _plantthing[(int)card[i]._plants - 1].coolingTime)
                    {
                        card[i].CanChoose = true;
                        card[i].startTime = 0;
                    }
                }
                for (int i = 0; i < L_bullet.Count; i++)
                {
                    if (L_bullet[i].arrive)
                    {
                        L_bullet.RemoveAt(i);
                    }
                }
                //更新子弹类
                for (int i = 0; i < L_bullet.Count; i++)
                {
                    for (int j = 0; j < L_zombiesF.Count; j++)
                    {
                        if (L_bullet[i].GetRange().IntersectsWith(L_zombiesF[j].GetRange()) && L_zombiesF[j].alive)
                        {
                            L_bullet[i].arrive = true;
                            L_bullet[i].image = Resources.豌豆子弹碰撞;
                            //僵尸减生命
                            L_zombiesF[j].Lifezombie -= L_bullet[i].power;
                            if (L_bullet[i].name == bullet.冰子弹)
                            {
                                L_zombiesF[j].slow = true;
                                L_zombiesF[j].coolStarttime = 0;
                            }
                            if (L_zombiesF[j].Lifezombie <= 0)
                            {
                                L_zombiesF[j].alive = false;
                                if (!L_zombiesF[j].eat)
                                    if (L_zombiesF[j].name != zombies.摇旗僵尸)
                                        L_zombiesF[j].image = Resources.ZombieLostHead;
                                    else L_zombiesF[j].image = Resources.FlagZombieLostHead;
                                else
                                    if (L_zombiesF[j].name != zombies.摇旗僵尸)
                                        L_zombiesF[j].image = Resources.ZombieLostHeadAttack;
                                    else L_zombiesF[j].image = Resources.FlagZombieLostHeadAttack;
                                L_zombiesF[j].count = L_zombiesF[j].image.GetFrameCount(FrameDimension.Time);
                                L_zombiesF[j].index = 0;
                                addZombieHead(new Point(L_zombiesF[j].location.X, L_zombiesF[j].location.Y));
                            }//L_zombiesF.RemoveAt(j);
                            else if (L_zombiesF[j].Lifezombie <= 200 && !L_zombiesF[j].pz)
                            {
                                if (!L_zombiesF[j].eat)
                                {
                                    if (L_zombiesF[j].name == zombies.路障僵尸)
                                    {
                                        L_zombiesF[j].image = Resources.Zombie;                                        
                                    }
                                    else if (L_zombiesF[j].name == zombies.铁桶僵尸)
                                    {
                                        L_zombiesF[j].image = Resources.Zombie2;                                        
                                    }
                                }
                                else
                                {
                                    L_zombiesF[j].image = Resources.ZombieAttack;
                                }
                                L_zombiesF[j].name = zombies.普通僵尸;
                                L_zombiesF[j].count = L_zombiesF[j].image.GetFrameCount(FrameDimension.Time);
                                L_zombiesF[j].index = 0;
                                L_zombiesF[j].pz = true;
                            }
                            //L_bullet[i].location.X += 15;
                            L_bullet[i].Move();
                            L_bullet[i].Move();

                            //跳出循环
                            break;
                        }

                    }
                    if (L_bullet[i].location.X <= 773)
                        if (land[L_bullet[i].row, (L_bullet[i].location.X - 45) / 81] == plants.火炬树桩
                            && L_bullet[i].name == bullet.普通子弹
                            && (L_bullet[i].location.X - 45) % 81 < 15)//这段代码有问题
                        {
                            L_bullet[i].image = Resources.火子弹1;
                            L_bullet[i].power = 40;
                            L_bullet[i].name = bullet.火子弹;
                        }
                        else if (land[L_bullet[i].row, (L_bullet[i].location.X - 45) / 81] == plants.火炬树桩
                            && L_bullet[i].name == bullet.冰子弹
                            && (L_bullet[i].location.X - 45) % 81 < 15)
                        {
                            L_bullet[i].image = Resources.豌豆子弹1;
                            L_bullet[i].power = 20;
                            L_bullet[i].name = bullet.普通子弹;
                        }
                    //if (L_bullet.Count != 0)
                    L_bullet[i].Move();
                    if (L_bullet[i].location.X >= 810)
                    {
                        L_bullet.RemoveAt(i);
                        i--;
                    }

                }

                //更新子弹
                for (int j = 0; j < L_plants.Count; j++)
                {
                    for (int i = 0; i < L_zombiesF.Count; i++)
                    {
                        if ((L_plants[j].location.Y - 90) / 97 == L_zombiesF[i].row && L_zombiesF[i].location.X > L_plants[j].location.X - 40
                            && (L_plants[j]._plants == plants.寒冰射手 || L_plants[j]._plants == plants.机枪射手 || L_plants[j]._plants == plants.双发射手 || L_plants[j]._plants == plants.豌豆射手)
                             )
                        {
                            if (L_plants[j].startTime == 5)
                            {
                                if (L_plants[j]._plants == plants.寒冰射手)
                                {
                                    addBullet(Resources.寒冰子弹1, new Point(L_plants[j].location.X + 30, L_plants[j].location.Y), L_zombiesF[i].row, bullet.冰子弹);
                                }
                                else
                                {
                                    addBullet(Resources.豌豆子弹1, new Point(L_plants[j].location.X + 30, L_plants[j].location.Y), L_zombiesF[i].row, bullet.普通子弹);
                                }
                                break;

                            }
                            //如果是双发射手或机枪射手(多发子弹）
                            else if (((L_plants[j].startTime - 5) / 3) < L_plants[j].BulletNum && ((L_plants[j].startTime - 5) / 3) > 0
                                && (L_plants[j].startTime - 5) % 3 == 0)
                            {
                                addBullet(Resources.豌豆子弹1, new Point(L_plants[j].location.X + 30, L_plants[j].location.Y), L_zombiesF[i].row, bullet.普通子弹);
                                break;
                            }
                        }
                    }

                }
                //植物是否被僵尸攻击
                for (int i = 0; i < L_plants.Count; i++)
                {
                    for (int j = 0; j < L_zombiesF.Count; j++)
                    {
                        if (L_plants[i].GetRange().IntersectsWith(L_zombiesF[j].GetRange()) && !L_zombiesF[j].eat&&L_zombiesF[j].alive)
                            eatZombie(L_zombiesF[j]);
                        else if (L_plants[i].GetRange().IntersectsWith(L_zombiesF[j].GetRange()) && L_zombiesF[j].eat)
                        {
                            if (L_zombiesF[j].index == 0 || L_zombiesF[j].index == L_zombiesF[j].count / 2)
                            {
                                L_plants[i].life -= L_zombiesF[j].power;
                                if (L_plants[i]._plants == plants.坚果墙)
                                {
                                    if (L_plants[i].life == 2700)
                                    {
                                        L_plants[i].image = Resources.坚果墙2;
                                        L_plants[i].index = 0;
                                        L_plants[i].count = L_plants[i].image.GetFrameCount(FrameDimension.Time);
                                    }
                                    else if (L_plants[i].life == 1300)
                                    {
                                        L_plants[i].image = Resources.坚果墙3;
                                        L_plants[i].index = 0;
                                        L_plants[i].count = L_plants[i].image.GetFrameCount(FrameDimension.Time);
                                    }
                                }
                            }
                        }
                    }

                }
                for (int i = 0; i < L_plants.Count; i++)
                {
                    for (int j = 0; j < L_zombiesF.Count; j++)
                        if (L_plants[i].life <= 0)
                            noeatZombie(L_zombiesF[j]);
                    if (L_plants[i].life <= 0)
                    {
                        land[(L_plants[i].location.Y - 90) / 97, (L_plants[i].location.X - 35) / 81] = plants.无;
                        L_plants.RemoveAt(i);
                        i--;
                    }
                }
            }
            else if (_plan == plan.植物选择界面)
            {
                for (int i = 0; i < L_zombies.Count; i++)
                    L_zombies[i].Play();
            }
            pictureBox1.Invalidate();
        }
        //窗体关闭时
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ChoosePlants != null)
                ChoosePlants.Abort();
            if (ModeIn != null)
                ModeIn.Abort();
            if (BackBG != null)
                BackBG.Abort();
            if (CountDown != null)
                CountDown.Abort();
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //AniWindowClass.AniWindow(this.Handle, 100, 0, this); 
        }
    }
}
