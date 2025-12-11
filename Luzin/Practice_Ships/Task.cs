using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luzin.Practice_Ships
{
    public class int2
    {
        public int x;
        public int y;

        public int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    internal class Task
    {
        private int2[] offsets = new int2[4]
        {
            new int2(0, 1),
            new int2(0, -1),
            new int2(1, 0),
            new int2(-1, 0),
        };

        private bool isExists(ref int[,] pool, int x, int y)
        {
            if (x < 0 || x >= pool.Length / 10 || y < 0 || y > pool.Length / 10) return false;
            return true;
        }

        private int GetShipSize(ref int[,] pool, int2 pos, bool? isVertical = null)
        {
            if (!isExists(ref pool, pos.x, pos.y)) return 0;
            if (pool[pos.x, pos.y] == 0) return 0;

            int res = 1;

            if (isVertical != null)
            {
                bool val = (bool)isVertical;

                if (isExists(ref pool, pos.x + 1, pos.y) && pool[pos.x + 1, pos.y] == 1 && val) return -100;
                if (isExists(ref pool, pos.x - 1, pos.y) && pool[pos.x - 1, pos.y] == 1 && val) return -100;
                if (isExists(ref pool, pos.x, pos.y + 1) && pool[pos.x, pos.y + 1] == 1 && !val) return -100;
                if (isExists(ref pool, pos.x, pos.y - 1) && pool[pos.x, pos.y - 1] == 1 && !val) return -100;
            }

            pool[pos.x, pos.y] = 0;

            if (isVertical == null)
            {
                bool f = false;

                if (isExists(ref pool, pos.x + 1, pos.y))
                {
                    res += GetShipSize(ref pool, new int2(pos.x + 1, pos.y), false);
                    if (pool[pos.x + 1, pos.y] == 1) f = true;
                }
                if (isExists(ref pool, pos.x - 1, pos.y))
                {
                    res += GetShipSize(ref pool, new int2(pos.x - 1, pos.y), false);
                    if (pool[pos.x - 1, pos.y] == 1) f = true;
                }

                if (!f)
                {
                    if (isExists(ref pool, pos.x, pos.y + 1))
                    {
                        res += GetShipSize(ref pool, new int2(pos.x, pos.y + 1), true);
                    }
                    if (isExists(ref pool, pos.x, pos.y - 1))
                    {
                        res += GetShipSize(ref pool, new int2(pos.x, pos.y - 1), true);
                    }
                }
                return res;
            }
            if (!(bool)isVertical)
            {
                if (isExists(ref pool, pos.x + 1, pos.y))
                {
                    res += GetShipSize(ref pool, new int2(pos.x + 1, pos.y), false);
                }
                if (isExists(ref pool, pos.x - 1, pos.y))
                {
                    res += GetShipSize(ref pool, new int2(pos.x - 1, pos.y), false);
                }
                return res;
            }
            else
            {
                if (isExists(ref pool, pos.x, pos.y + 1))
                {
                    res += GetShipSize(ref pool, new int2(pos.x, pos.y + 1), true);
                }
                if (isExists(ref pool, pos.x, pos.y - 1))
                {
                    res += GetShipSize(ref pool, new int2(pos.x, pos.y - 1), true);
                }
                return res;
            }
        }

        public bool isValid(ref int[,] pool)
        {
            int[] ships = { 4, 3, 2, 1 };

            for (int i = 0; i < pool.Length / 10; i++)
            {
                for (int j = 0; j < pool.Length / 10; j++)
                {
                    int size = GetShipSize(ref pool, new int2(i, j));

                    Console.Write(size);

                    if (size == 0)
                    {
                        continue;
                    }

                    if (size < 0 || size > 4)
                    {
                        return false;
                    }

                    ships[size - 1]--;
                    if (ships[size - 1] < 0)
                    {
                        return false;
                    }
                }
            }

            for (int i = 0; i < ships.Length; i++)
            {
                if (ships[i] != 0)
                {
                    return false;
                }
            }

            return true;
        }

        public void Main(string[] args)
        {

            int[,] pool = new int[10, 10]
            {
                {1, 1, 1, 1, 0, 0, 0, 0, 1, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 1, 1, 1, 0, 0, 1, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 1, 0, 0, 0, 0, 1, 0},
                {0, 0, 0, 1, 0, 0, 1, 0, 1, 0},
                {0, 0, 0, 0, 0, 0, 1, 0, 1, 0},
                {0, 1, 1, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 1, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 1, 0}
            };

            Console.WriteLine(isValid(ref pool));
        }
    }
}
