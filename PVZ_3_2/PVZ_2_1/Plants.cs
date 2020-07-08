using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using PVZ_2_1.Properties;

namespace PVZ_2_1
{
    class Plants
    {
        public Point location;
        public Image image;
        public int index;
        public int count;
        public int startTime;//控制产生武器的时间（阳光或子弹）
        public plants _plants;        
        public int BulletNum;
        public int life;
        public bool bomb;
        public Plants()
        {
            image = Resources.豌豆射手;
            //表示帧总数
            location = new Point(160, 26);
            count = image.GetFrameCount(FrameDimension.Time);//返回指定维度的帧数。 
            startTime = 0;
            BulletNum = 1;
            life = 300;
            bomb = false;
        }

        public void Play()
        {
            index++;
            if (index == count)
                index = 0;
            image.SelectActiveFrame(FrameDimension.Time, index);//将当前要显示的图片设置为动态图片的Index所对应的帧
        }
        public void attack()
        {
            startTime+=1;
        }

        public void Diaplay(Graphics g)
        {
            g.DrawImage(image, location);
        }
        public Rectangle GetRange()
        {
            return new Rectangle(location.X-20 , location.Y , 81, 97);
        }
    }
}
