using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using PVZ_2_1.Properties;

namespace PVZ_2_1
{
    class Zombies
    {
        public Point location;
        public Image image;
        public int index;
        public int count;
        public int Lifezombie;
        public int row;//僵尸所在的行
        public bool alive;
        public bool down;
        public bool pz;//是否是普通僵尸
        public zombies name;
        public bool eat;
        public int power;
        public bool slow;
        public bool ice;
        public int coolStarttime;

        public Zombies()
        {
            image = Resources.Zombie;
            count = image.GetFrameCount(FrameDimension.Time);
            location = new Point(0, 0);
            Lifezombie = 200;
            alive = true;
            down = false;
            pz = true;
            name = zombies.普通僵尸;
            eat = false;
            power = 50;
            slow = false;
            ice = false;
            coolStarttime = 0;
        }
        public void Play()
        {
            if((!ice&&slow)||!slow)
                index++;
            if (index == count)
                index = 0;
            image.SelectActiveFrame(FrameDimension.Time, index);//将当前要显示的图片设置为动态图片的Index所对应的帧
        }
        public void move()
        {
            if (!eat && ((!ice&&slow)||!slow))
            {
                location.X -= 1;
                ice = !ice;
            }
            else ice = !ice;
            coolStarttime++;
        }
        public void Display(Graphics g,Point point)
        {
            g.DrawImage(image, point);
        }
        public Rectangle GetRange()
        {
            return new Rectangle(location.X + 80, location.Y + 60, 81, 97);
        }
    }
}
