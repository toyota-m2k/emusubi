using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace emusubi
{
    public class Item
    {
        public int ID { get; private set; }
        public Color Color { get; private set; }

        public Cordinate Start;
        public Cordinate Goal;

        public Item(int id, Cordinate a, Cordinate b, Color color)
        {
            ID = id;
            Start = a;
            Goal = b;
            Color = color;
        }

        public Route Route { get; set; } = null;
    }
}
