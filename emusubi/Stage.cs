using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

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
        public bool Equals(Cordinate s)
        {
            return X == s.X && Y == s.Y;
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
        public static int DIMX = 9;
        public static int DIMY = 9;

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
        private bool mRunning = false;
        public bool IsRunning
        {
            get => mRunning;
            set => UpdateAndNotify("IsRunning", ref mRunning, value);
        }

        private Dispatcher mDispatcher = null;
        public Task<bool> Search(Dispatcher dispatcher)
        {
            mDispatcher = dispatcher;
            return Task<bool>.Run(async () =>
            {
                IsRunning = true;
                var item = ItemOf(1);
                var result = await SearchRoute(item, new Route() { item.Start });
                Debug.WriteLine("Result={0}", result.ToString());
                IsRunning = false;
                return result;
            });
        }

        public void Stop()
        {
            IsRunning = false;
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

        private List<Cordinate> NextCordinates(Cordinate current)
        {
            var r = new List<Cordinate>(4);
            Cordinate? next;
            Direction d = Direction.INIT;
            while( (next=NextCordinate(current, ref d))!=null)
            {
                r.Add(next.Value);
            }
            return r;
        }

        private Cordinate? ExistsGoalInCordinates(Item currentItem, List<Cordinate>cordinates)
        {
            foreach(var c in cordinates)
            {
                if(c.Equals(currentItem.Goal))
                {
                    return c;
                }
            }
            return null;
        }

        private bool HasPath(Cordinate c)
        {
            var d = Direction.INIT;
            Cordinate? next;
            while( (next=NextCordinate(c,ref d))!=null )
            {
                if(0==this[next.Value].Ocupied)
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * この一手によって他のアイテムのパスがふさがれてしまう場合に除外する。
         */
        private bool PostCheck_OtherItemsPath(int currentId)
        {
            for(int id = currentId+1; id<=MaxID; id++)
            {
                if(!HasPath(ItemOf(id).Start))
                {
                    return false;
                }
                if (!HasPath(ItemOf(id).Goal))
                {
                    return false;
                }
            }
            return true;
        }

        bool ReachableCheck(Item target, Cordinate c)
        {
            Cordinate? next;
            Direction d = Direction.INIT;
            while((next=NextCordinate(c, ref d))!=null)
            {
                var cell = this[next.Value];
                if (cell.Ocupied == target.ID && cell.Type == Cell.CellType.GOAL)
                {
                    return true;
                }
                else if (!cell.Check && cell.Ocupied == 0)
                {
                    cell.Check = true;
                    if (ReachableCheck(target, next.Value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool PostCheck_OtherItemsReachable(int currentId)
        {
            for (int id = currentId + 1; id <= MaxID; id++)
            {
                foreach(var cell in mCells)
                {
                    cell.Check = false;
                }
                var item = ItemOf(id);
                if (!ReachableCheck(item, item.Start))
                {
                    return false;
                }
            }
            return true;
        }

        /**
            */
        private bool PreCheck_Loop(Item currentItem, Cordinate currentCordinate, Cordinate nextCordinate)
        {
            var cordinates = NextCordinates(nextCordinate);
            foreach(var c in cordinates)
            {
                if(this[c].Ocupied==currentItem.ID && !c.Equals(currentCordinate) && !c.Equals(currentItem.Goal))
                {
                    // 元来た方向ではない方向に、自分のセルが存在する ---> ループする
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> SearchRoute(Item item, Route route)
        {
            if (!IsRunning)
            {
                return false;
            }

            var current = route[route.Count - 1];
            Debug.WriteLine($"Item({item.ID})-at({current.X}, {current.Y})");
            //var direction = Direction.INIT;
            //Cordinate? next;
            var nextCordinates = NextCordinates(current);
            var goal = ExistsGoalInCordinates(item, nextCordinates);
            if(null!=goal)
            {
                Debug.WriteLine($"Item({item.ID})-at({current.X}, {current.Y})-Reaced Goal");
                // Reached to goal
                item.Route = route;
                if (item.ID == MaxID)
                {
                    return true;
                }
                var nextItem = ItemOf(item.ID + 1);
                if (await SearchRoute(nextItem, new Route() { nextItem.Start }))
                {
                    return true;
                }
            }
            else
            {
                foreach(var next in nextCordinates)
                {
                    var cell = this[next];
                    if (cell.Ocupied == 0 && PreCheck_Loop(item, current, next))
                    {
                        // Movable
                        // Debug.WriteLine($"Item({item.ID})-at({current.X}, {current.Y}) -- Move {direction.ToString()}");
                        mDispatcher.Invoke(() =>
                        {
                            this[next].SetRoute(item.ID);
                        });
                        if (PostCheck_OtherItemsPath(item.ID) && PostCheck_OtherItemsReachable(item.ID))
                        {
                            var routeNext = new Route(route);
                            routeNext.Add(next);
                            if (await SearchRoute(item, routeNext))
                            {
                                return true;
                            }
                        }
                        mDispatcher.Invoke(() =>
                        {
                            this[next].ResetRoute();
                        });
                    }

                }
            }
            Debug.WriteLine($"Item({item.ID})-at({current.X}, {current.Y})-No Way");
            return false;
        }

    }
}
