using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Timers;
using PVZ_2_1.Properties;

namespace PVZ_2_1
{
    class Sun
    {
        public Point location;
        public Image sun;
        public int index;
        public int count;
        public int length;
        public int lifeTime;//超过一定时间阳光会消失
        public bool click;
        public int k;
        public Sun()
        {
            sun = Resources.Sun;
            count = sun.GetFrameCount(FrameDimension.Time);
            location = new Point(location.X , 0);
            length = 0;
            lifeTime = 120;
            click = false;
            k = 10;
        }
        public void Display(Graphics g)
        {
            g.DrawImage(sun, location.X,location.Y);
        }
        public void Play()
        {
            index++;
            lifeTime--;
            if (index == count)
                index = 0;
            sun.SelectActiveFrame(FrameDimension.Time, index);
        }
        public void Move()
        {
            if (!click)
                location.Y += 3;
            else
            {
                location.X = location.X / k * (k - 1);
                location.Y = location.Y / k * (k - 1);
                k--;
            }
        }
        public Rectangle GetRange()
        {
            return new Rectangle(location, sun.Size);
        }
    }
}
