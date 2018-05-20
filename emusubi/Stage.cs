using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace emusubi
{
    public struct Cordinate
    {
        public int X;
        public int Y;
        public Cordinate(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Route : List<Cordinate>
    {
        public Route()
        {

        }
        public Route(Route src)
            : base(src)
        {

        }
    }


    public class Stage : NotificationBase
    {
        public int DIMX = 9;
        public int DIMY = 9;

        private List<Cell> mCells;
        private List<Item> mInitialItems;

        public Cell this[int x, int y]
        {
            get => mCells[y * DIMX + x];
        }
        public Cell this[Cordinate c]
        {
            get => this[c.X, c.Y];
        }

        public Item ItemOf(int id)
        {
            return mInitialItems[id - 1];
        }

        public int MaxID => mInitialItems.Count;

        public Stage()
        {
            mCells = new List<Cell>(DIMX * DIMY);
            for(int i=0;i<DIMX*DIMY; i++)
            {
                mCells.Add(new Cell(i%DIMX, i/DIMX));
            }

            mInitialItems = new List<Item>()
            {
                new Item(1, new Cordinate(1,7), new Cordinate(4,4), Colors.Red),
                new Item(2, new Cordinate(8,0), new Cordinate(1,5), Colors.Blue),
                new Item(3, new Cordinate(7,2), new Cordinate(5,6), Colors.Yellow),
                new Item(4, new Cordinate(1,1), new Cordinate(6,5), Colors.Green),
                new Item(5, new Cordinate(2,1), new Cordinate(2,4), Colors.Orange),
                new Item(6, new Cordinate(1,4), new Cordinate(7,7), Colors.Purple),
            };

            foreach (var item in mInitialItems)
            {
                this[item.Start].InitAsStart(item.ID);
                this[item.Goal].InitAsGoal(item.ID);
            }

        }

        public void Reset()
        {
            foreach(var c in mCells)
            {
                c.ResetRoute();
            }
        }

        //public void Reset(int id, Route route)
        //{
        //    foreach (var c in mCells)
        //    {
        //        if (c.Ocupied == id)
        //        {
        //            c.ResetRoute();
        //        }
        //    }
        //    if (null != route)
        //    {
        //        foreach (var c in route)
        //        {
        //            this[c].SetRoute(id);
        //        }
        //    }
        //}

        public void Search()
        {
            var item = ItemOf(1);
            var result = SearchRoute(item, new Route() { item.Start });
            Debug.WriteLine("Result={0}", result.ToString());
        }

        //public void SearchRoute(Item prevItem)
        //{
        //    //int id = 1;
        //    //var item = ItemOf(id);
        //    //var route1 = new Route()
        //    //    {
        //    //        item.Start
        //    //    };
        //    //SearchRouteForItem(item, route1);

        //    var currentItem = ItemOf(prevItem.ID + 1);
        //    foreach(var route in prevItem.RootList)
        //    {
        //        Reset(prevItem.ID, route);

        //        var route2 = new Route() { currentItem.Start };
        //        SearchRouteForItem(currentItem, route2);

        //        if(currentItem.ID==MaxID)
        //        {
        //            if(item)
        //        }
        //    }
        //}

        public Cordinate? Upper(Cordinate current)
        {
            if(current.Y<=0)
            {
                return null;
            }
            return new Cordinate(current.X, current.Y - 1);
        }
        public Cordinate? Lower(Cordinate current)
        {
            if (current.Y+1 >= DIMY)
            {
                return null;
            }
            return new Cordinate(current.X, current.Y + 1);
        }
        public Cordinate? Left(Cordinate current)
        {
            if (current.X <= 0)
            {
                return null;
            }
            return new Cordinate(current.X-1, current.Y);
        }
        public Cordinate? Right(Cordinate current)
        {
            if (current.X+1 >= DIMY)
            {
                return null;
            }
            return new Cordinate(current.X + 1, current.Y);
        }

        enum Direction
        {
            INIT = -1,
            UP = 0,
            RIGHT = 1,
            DOWN = 2,
            LEFT = 3,
            LAST = 4,
        }
        private Cordinate? NextCordinate(Cordinate current, ref Direction direction)
        {
            Cordinate? next = null;
            while (null == next)
            {
                direction++;
                switch (direction)
                {
                    case Direction.UP:
                        next = Upper(current);
                        break;
                    case Direction.DOWN:
                        next = Lower(current);
                        break;
                    case Direction.LEFT:
                        next = Left(current);
                        break;
                    case Direction.RIGHT:
                        next = Right(current);
                        break;
                    default:
                        return null;
                }
            }
            return next;
        }

        public bool SearchRoute(Item item, Route route)
        {
            var current = route[route.Count - 1];
            Debug.WriteLine($"Item({item.ID})-at({current.X}, {current.Y})");
            var direction = Direction.INIT;
            Cordinate? next;
            while((next=NextCordinate(current, ref direction))!=null)
            {
                var cell = this[next.Value];
                if (cell.Ocupied == 0)
                {
                    Debug.WriteLine($"Item({item.ID})-at({current.X}, {current.Y}) -- Move {direction.ToString()}");
                    // Movable
                    this[next.Value].SetRoute(item.ID);
                    var routeNext = new Route(route);
                    routeNext.Add(next.Value);
                    if(SearchRoute(item, routeNext))
                    {
                        return true;
                    }
                    this[next.Value].ResetRoute();
                }
                else if(cell.Ocupied==item.ID && cell.Type == Cell.CellType.GOAL)
                {
                    Debug.WriteLine($"Item({item.ID})-at({current.X}, {current.Y})-Reaced Goal");
                    // Reached to goal
                    item.Route = route;
                    if(item.ID == MaxID)
                    {
                        return true;
                    }
                    var nextItem = ItemOf(item.ID + 1);
                    if (SearchRoute(nextItem, new Route() { nextItem.Start }))
                    {
                        return true;
                    }
                }
            }
            Debug.WriteLine($"Item({item.ID})-at({current.X}, {current.Y})-No Way");
            return false;
        }

    }
}
