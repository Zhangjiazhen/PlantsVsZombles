using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using PVZ_2_1.Properties;

namespace PVZ_2_1
{    
    class Bullet
    {
        public Image image;
        public Point location;
        public int row;
        public bool arrive;
        public int power;
        public bullet name;
        public Bullet()
        {
            image = Resources.豌豆子弹1;
            arrive = false;
            power = 20;
            name = bullet.普通子弹;
        }
        public void Move()
        {
            location.X += 15;
        }
        public void display(Graphics g)
        {
            g.DrawImage(image, location);
        }
        public Rectangle GetRange()
        {
            return new Rectangle(location, image.Size);
        }
    }
}
