using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace emusubi
{
    public class Cell : NotificationBase
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool Check { get; set; }

        private int mOcupied;
        public int Ocupied
        {
            get => mOcupied;
            set => UpdateAndNotify(new string[] {"Ocupied", "Color"}, ref mOcupied, value);
        }

        private readonly Brush[] mCellColors = new Brush[] { new SolidColorBrush(Colors.White), new SolidColorBrush(Colors.Green), new SolidColorBrush(Colors.Blue), new SolidColorBrush(Colors.Yellow), new SolidColorBrush(Colors.Red), new SolidColorBrush(Colors.Purple), new SolidColorBrush(Colors.Pink) };
        public Brush Color => mCellColors[Ocupied];

        public enum CellType
        {
            ROUTE,
            START,
            GOAL,
        }
        public CellType Type
        {
            get; private set;
        }

        public Cell(int x, int y)
        {
            mOcupied = 0;
            X = x;
            Y = y;
            Type = CellType.ROUTE;
        }
        public void InitAsStart(int id)
        {
            mOcupied = id;
            Type = CellType.START;
        }
        public void InitAsGoal(int id)
        {
            mOcupied = id;
            Type = CellType.GOAL;
        }

        public void SetRoute(int id)
        {
            if(Ocupied!=0||Type!=CellType.ROUTE)
            {
                throw new InvalidOperationException("Cell is not empty.");
            }
            Ocupied = id;
        }
        public void ResetRoute()
        {
            if (Type == CellType.ROUTE)
            {
                Ocupied = 0;
            }
        }
    }
}
