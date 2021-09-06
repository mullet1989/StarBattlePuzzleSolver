using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConsoleApp3
{
    class Cell
    {
        public bool Star;
        public int RowIdx;
        public int ColIdx;
        public char RegionChar;
        public List<Cell> Region;
        public int RegionStars;
        public Cell[] Row;
        public int RowStars;
        public Cell[] Col;
        public int ColStars;
        public List<Cell> Neighbors;
        public int NeighborStars;

        public void MakeStar()
        {
            this.Star = true;

            for (int p = 0; p < this.Region.Count; p++)
            {
                this.Region.ElementAt(p).RegionStars += 1;
            }

            for (int p = 0; p < this.Row.Length; p++)
            {
                this.Row.ElementAt(p).RowStars += 1;
            }

            for (int p = 0; p < this.Col.Length; p++)
            {
                this.Col.ElementAt(p).ColStars += 1;
            }

            for (int p = 0; p < this.Neighbors.Count; p++)
            {
                this.Neighbors.ElementAt(p).NeighborStars += 1;
            }
        }

        public void MakeNotStar()
        {
            this.Star = false;

            for (int p = 0; p < this.Region.Count; p++)
            {
                this.Region.ElementAt(p).RegionStars -= 1;
            }

            for (int p = 0; p < this.Row.Length; p++)
            {
                this.Row.ElementAt(p).RowStars -= 1;
            }

            for (int p = 0; p < this.Col.Length; p++)
            {
                this.Col.ElementAt(p).ColStars -= 1;
            }

            // neighbor stars
            for (int p = 0; p < this.Neighbors.Count; p++)
            {
                this.Neighbors.ElementAt(p).NeighborStars -= 1;
            }
        }
    }

    class Program
    {
        private static Dictionary<char, List<Cell>> region = new Dictionary<char, List<Cell>>();
        private static int _searched = 0;
        private static int _stars = 0;

        private static int n = 3;
        private static int s = 15;

        private static int _total = n * s;

        static void Main(string[] args)
        {
            // char[][] input = new char[][]
            // {
            //     new[] { 'A', 'A', 'B', 'B', 'C', 'C' },
            //     new[] { 'A', 'A', 'B', 'C', 'C', 'C' },
            //     new[] { 'A', 'A', 'B', 'C', 'C', 'C' },
            //     new[] { 'D', 'D', 'B', 'B', 'E', 'E' },
            //     new[] { 'D', 'D', 'B', 'B', 'E', 'F' },
            //     new[] { 'D', 'D', 'B', 'B', 'F', 'F' },
            // };

            // char[][] input = new char[][]
            // {
            //     new[] { 'A', 'A', 'A', 'A', 'B', 'B', 'C', 'C', 'C', 'C' },
            //     new[] { 'A', 'D', 'A', 'A', 'B', 'B', 'B', 'C', 'B', 'B' },
            //     new[] { 'A', 'D', 'D', 'B', 'B', 'B', 'B', 'B', 'B', 'B' },
            //     new[] { 'D', 'D', 'D', 'D', 'B', 'E', 'E', 'E', 'E', 'B' },
            //     new[] { 'D', 'D', 'B', 'B', 'B', 'B', 'B', 'B', 'E', 'B' },
            //     new[] { 'F', 'F', 'F', 'F', 'G', 'G', 'H', 'H', 'H', 'H' },
            //     new[] { 'F', 'I', 'F', 'F', 'G', 'G', 'G', 'H', 'G', 'G' },
            //     new[] { 'F', 'I', 'I', 'G', 'G', 'G', 'G', 'G', 'G', 'G' },
            //     new[] { 'I', 'I', 'I', 'I', 'G', 'J', 'J', 'J', 'J', 'G' },
            //     new[] { 'I', 'I', 'G', 'G', 'G', 'G', 'G', 'G', 'J', 'G' },
            // };

            char[][] input = new char[][]
            {
                new[] { 'A', 'A', 'A', 'A', 'A', 'B', 'B', 'B', 'B', 'B', 'C', 'C', 'C', 'C', 'C' },
                new[] { 'A', 'D', 'D', 'D', 'D', 'B', 'B', 'B', 'B', 'B', 'E', 'E', 'E', 'C', 'C' },
                new[] { 'A', 'D', 'D', 'D', 'D', 'D', 'D', 'B', 'E', 'E', 'E', 'E', 'E', 'E', 'C' },
                new[] { 'A', 'D', 'D', 'D', 'F', 'D', 'D', 'B', 'E', 'E', 'G', 'E', 'E', 'E', 'C' },
                new[] { 'A', 'D', 'D', 'F', 'F', 'F', 'H', 'H', 'H', 'G', 'G', 'G', 'E', 'E', 'C' },
                new[] { 'A', 'A', 'A', 'F', 'F', 'F', 'H', 'H', 'H', 'G', 'G', 'G', 'C', 'C', 'C' },
                new[] { 'A', 'A', 'H', 'H', 'F', 'H', 'H', 'I', 'H', 'H', 'G', 'H', 'C', 'C', 'C' },
                new[] { 'A', 'A', 'A', 'H', 'H', 'H', 'I', 'I', 'I', 'H', 'H', 'H', 'J', 'J', 'J' },
                new[] { 'A', 'A', 'A', 'K', 'K', 'K', 'I', 'I', 'I', 'K', 'K', 'K', 'K', 'L', 'J' },
                new[] { 'A', 'A', 'A', 'M', 'K', 'K', 'K', 'I', 'K', 'K', 'K', 'M', 'L', 'L', 'J' },
                new[] { 'N', 'N', 'N', 'M', 'M', 'K', 'K', 'K', 'K', 'K', 'M', 'M', 'L', 'L', 'J' },
                new[] { 'N', 'N', 'N', 'O', 'M', 'M', 'M', 'M', 'M', 'M', 'M', 'L', 'L', 'L', 'J' },
                new[] { 'N', 'O', 'O', 'O', 'O', 'M', 'M', 'M', 'M', 'M', 'O', 'O', 'L', 'L', 'L' },
                new[] { 'N', 'O', 'O', 'O', 'O', 'O', 'M', 'M', 'M', 'O', 'O', 'O', 'L', 'L', 'L' },
                new[] { 'N', 'N', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'L', 'L', 'L' },
            };

            Cell[][] cells = new Cell[s][];

            for (int i = 0; i < input.Length; i++)
            {
                Cell[] row = new Cell[s];
                for (int l = 0; l < s; l++)
                {
                    row[l] = new Cell();
                }

                cells[i] = row;
                for (int j = 0; j < input[i].Length; j++)
                {
                    Cell c = row[j];
                    cells[i][j] = c;
                    row[j] = c;
                    c.Row = row;
                    c.RowIdx = i;
                    c.ColIdx = j;

                    if (region.ContainsKey(input[i][j]) == false)
                    {
                        region.Add(input[i][j], new List<Cell>());
                    }

                    region[input[i][j]].Add(c);

                    c.Region = region[input[i][j]];
                    c.RegionChar = input[i][j];
                }
            }

            // add neighbors
            for (int p = 0; p < s; p++)
            {
                for (int q = 0; q < s; q++)
                {
                    if (cells[p][q].Neighbors == null)
                    {
                        cells[p][q].Neighbors = new List<Cell>();
                    }

                    for (int i = Math.Max(0, cells[p][q].RowIdx - 1); i <= Math.Min(s - 1, cells[p][q].RowIdx + 1); i++)
                    {
                        for (int j = Math.Max(0, cells[p][q].ColIdx - 1); j <= Math.Min(s - 1, cells[p][q].ColIdx + 1); j++)
                        {
                            if (i == p && j == q)
                            {
                                continue;
                            }

                            cells[p][q].Neighbors.Add(cells[i][j]);
                        }
                    }
                }
            }

            // build cols
            for (int i = 0; i < input.Length; i++)
            {
                Cell[] col = new Cell[s];
                for (int j = 0; j < input[i].Length; j++)
                {
                    cells[j][i].Col = col;
                    col[j] = cells[j][i];
                }
            }

            Cell[] flattened = cells.SelectMany(x => x).ToArray();

            PrintEmpty(cells);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            StarBattle(flattened, 0);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
            Console.WriteLine(_searched);
            Print(flattened);
        }

        static void StarBattle(Cell[] cells, int depth)
        {
            // Print(cells);

            _searched += 1;

            if (_searched % 1000000 == 0)
            {
                Console.WriteLine(_searched);
            }

            List<Cell> options = new List<Cell>();
            int[] row = new int[s];
            int[] col = new int[s];
            int[] reg = new int[s];


            for (int i = 0; i < cells.Length; i++)
            {
                if (IsCandidate(cells[i]) == false)
                {
                    continue;
                }

                options.Add(cells[i]);

                row[cells[i].RowIdx] += 1;
                col[cells[i].ColIdx] += 1;
                reg[cells[i].RegionChar - 65] += 1;
            }

            if (options.Count + _stars < _total
                || options.Count == 0)
            {
                return;
            }

            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i].RowStars + row[cells[i].RowIdx] < n)
                {
                    return;
                }

                if (cells[i].ColStars + col[cells[i].ColIdx] < n)
                {
                    return;
                }

                if (cells[i].RegionStars + reg[cells[i].RegionChar - 65] < n)
                {
                    return;
                }
            }

            Cell cell = PickBestOption(options, row, col, reg);
            
            while (options.Any())
            {
                cell.MakeStar();
                _stars += 1;

                StarBattle(cells, depth + 1);

                if (_stars == _total)
                {
                    // Console.WriteLine("complete!!!");
                    return;
                }

                // need to reverse
                cell.MakeNotStar();
                _stars -= 1;

                row[cell.RowIdx] -= 1;
                col[cell.ColIdx] -= 1;
                reg[cell.RegionChar - 65] -= 1;

                if (cell.RowStars + row[cell.RowIdx] < n)
                {
                    return;
                }

                if (cell.ColStars + col[cell.ColIdx] < n)
                {
                    return;
                }

                if (cell.RegionStars + reg[cell.RegionChar - 65] < n)
                {
                    return;
                }

                // intelligently choose next option index
                options.Remove(cell);

                cell = PickBestOption(options, row, col, reg);
            }
        }

        private static Cell PickBestOption(List<Cell> options, int[] row, int[] col, int[] reg)
        {
            Cell cell = options[0];
            int min = int.MaxValue;
            foreach (var option in options)
            {
                if (option.RowStars + row[option.RowIdx] == n
                    || option.ColStars + col[option.ColIdx] == n
                    || option.RegionStars + reg[option.RegionChar - 65] == n)
                {
                    cell = option;
                    break;
                }

                int rowOpt = option.RowStars + row[option.RowIdx];
                int colOpt = option.ColStars + col[option.ColIdx];
                int regOpt = option.RegionStars + reg[option.RegionChar - 65];
                int minOpt = Math.Min(rowOpt, Math.Min(colOpt, regOpt));
                if (minOpt < min)
                {
                    min = minOpt;
                    cell = option;
                }
            }

            return cell;
        }

        public static bool IsCandidate(Cell cell)
        {
            if (cell.Star)
            {
                return false;
            }

            // row complete
            if (cell.RowStars == n)
            {
                return false;
            }


            // col complete
            if (cell.ColStars == n)
            {
                return false;
            }


            // region is complete
            if (cell.RegionStars == n)
            {
                return false;
            }

            // no stars adjacent
            if (cell.NeighborStars > 0)
            {
                return false;
            }

            return true;
        }

        static void Print(Cell[] cells)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                if (i % s == 0)
                {
                    Console.WriteLine();
                }

                if (cells[i].Star)
                {
                    Console.Write('*');
                }
                else
                {
                    Console.Write('.');
                }
            }

            Console.WriteLine();
        }

        static void PrintEmpty(Cell[][] cells)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                for (int j = 0; j < cells[i].Length; j++)
                {
                    Console.Write(cells[i][j].RegionChar);
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }

    internal class OptionsComparer : IComparer<Cell>
    {
        public int Compare(Cell? x, Cell? y)
        {
            int o1 = 0;
            int o2 = 0;

            return x.RowIdx.CompareTo(y.RowIdx);

            for (int i = 0; i < x.Neighbors.Count; i++)
            {
                if (Program.IsCandidate(x.Neighbors[i]))
                {
                    o1 += 1;
                }
            }

            for (int i = 0; i < y.Neighbors.Count; i++)
            {
                if (Program.IsCandidate(y.Neighbors[i]))
                {
                    o2 += 1;
                }
            }

            return o1.CompareTo(o2);
        }
    }
}