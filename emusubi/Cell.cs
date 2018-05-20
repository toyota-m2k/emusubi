using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emusubi
{
    public class Cell : NotificationBase
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        private int mOcupied;
        public int Ocupied
        {
            get => mOcupied;
            set => UpdateAndNotify("Ocupied", ref mOcupied, value);
        }

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
            if(mOcupied!=0||Type!=CellType.ROUTE)
            {
                throw new InvalidOperationException("Cell is not empty.");
            }
            mOcupied = id;
        }
        public void ResetRoute()
        {
            if (Type == CellType.ROUTE)
            {
                mOcupied = 0;
            }
        }
    }
}
