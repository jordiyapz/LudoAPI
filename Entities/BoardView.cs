using System.Drawing;
using System.Xml.Linq;

namespace LudoAPI.Entities
{
    public class BoardView(Board board, Player[] players, Peg[] pegs)
    {
        private readonly Board board = board;
        private readonly Player[] players = players;
        private readonly Peg[] pegs = pegs;

        private static readonly int IndexPegStationA = 48 * 4 + 6;
        private static readonly int IndexPegStationB = IndexPegStationA + 27;
        private static readonly int IndexPegStationC = IndexPegStationA + 48 * 18;
        private static readonly int IndexPegStationD = IndexPegStationC + 27;

        public string render()
        {
            char[] boardBase = @"+-----------------+--+--+--+-----------------+
|                 |  |  |  |                 |
|  +-----+-----+  +--+--+--+  +-----+-----+  |
|  |     |     |  |  |     |  |     |     |  |
|  |     |     |  +--+  +--+  |     |     |  |
|  |     |     |  |  |  |  |  |     |     |  |
|  +-----+-----+  +--+  +--+  +-----+-----+  |
|  |     |     |  |  |  |  |  |     |     |  |
|  |     |     |  +--+  +--+  |     |     |  |
|  |     |     |  |  |  |  |  |     |     |  |
|  +-----+-----+  +--+  +--+  +-----+-----+  |
|                 |  |  |  |                 |
+--+--+--+--+--+--+--+  +--+--+--+--+--+--+--+
|  |  |  |  |  |  |        |  |  |  |  |  |  |
+--+  +--+--+--+--+        +--+--+--+--+--+--+
|  |                                      |  |
+--+--+--+--+--+--+        +--+--+--+--+  +--+
|  |  |  |  |  |  |        |  |  |  |  |  |  |
+--+--+--+--+--+--+--+  +--+--+--+--+--+--+--+
|                 |  |  |  |                 |
|  +-----+-----+  +--+  +--+  +-----+-----+  |
|  |     |     |  |  |  |  |  |     |     |  |
|  |     |     |  +--+  +--+  |     |     |  |
|  |     |     |  |  |  |  |  |     |     |  |
|  +-----+-----+  +--+  +--+  +-----+-----+  |
|  |     |     |  |  |  |  |  |     |     |  |
|  |     |     |  +--+  +--+  |     |     |  |
|  |     |     |  |     |  |  |     |     |  |
|  +-----+-----+  +--+--+--+  +-----+-----+  |
|                 |  |  |  |                 |
+-----------------+--+--+--+-----------------+".ToCharArray();
            //for (int i = 0; i < 4; i++)
            //{
            //    boardBase[IndexPegStationA + ((int)(i / 2) * 4 * 48) + (i % 2) * 6] = 'a';
            //    boardBase[IndexPegStationB + ((int)(i / 2) * 4 * 48) + (i % 2) * 6] = 'b';
            //    boardBase[IndexPegStationC + ((int)(i / 2) * 4 * 48) + (i % 2) * 6] = 'c';
            //    boardBase[IndexPegStationD + ((int)(i / 2) * 4 * 48) + (i % 2) * 6] = 'd';
            //}

            foreach (Player player in players)
            {
                int numOfIdlePegs = 4;
                foreach (Peg peg in pegs.Where(p => p.Owner == player.Id).ToArray())
                {
                    XYCoord c = CalcCoord(peg.Position);
                    var qdr = (int)player.CharSymbol - 97;
                    c = RotateCoord(c, qdr);
                    c = MapCoordToBoard(c);
                    boardBase[c.x + c.y * 48] = player.CharSymbol;
                    boardBase[c.x + 1 + c.y * 48] = (char)(peg.Order + 48);
                    numOfIdlePegs--;
                }

                int pegStationIndex = player.Symbol switch
                {
                    PegSymbol.A => IndexPegStationA,
                    PegSymbol.B => IndexPegStationB,
                    PegSymbol.C => IndexPegStationC,
                    PegSymbol.D => IndexPegStationD,
                    _ => throw new ArgumentException("Invalid peg symbol"),
                };

                for (int i = 0; i < numOfIdlePegs; i++)
                {
                    boardBase[pegStationIndex + ((int)(i / 2) * 4 * 48) + (i % 2) * 6] = player.CharSymbol;
                }
            }

            return new string(boardBase);
        }

        public static XYCoord CalcCoord(int pos)
        {
            if (pos < 5)
                return new XYCoord(1 + pos, 6);
            if (pos < 11)
                return new XYCoord(6, 10 - pos);
            if (pos < 12)
                return new XYCoord(pos - 4, 0);
            if (pos < 18)
                return new XYCoord(8, pos - 12);
            if (pos < 24)
                return new XYCoord(pos - 9, 6);
            if (pos < 26)
                return new XYCoord(14, pos - 17);
            if (pos < 31)
                return new XYCoord(39 - pos, 8);
            if (pos < 37)
                return new XYCoord(8, pos - 22);
            if (pos < 39)
                return new XYCoord(44 - pos, 14);
            if (pos < 44)
                return new XYCoord(6, 52 - pos);
            if (pos < 50)
                return new XYCoord(49 - pos, 8);
            return new XYCoord(pos - 50, 7);
        }

        public static XYCoord RotateCoord(XYCoord coord, int quadrant)
        {
            // x' = x cos(θ) - y sin(θ)
            // y' = x sin(θ) + y cos(θ)
            if (quadrant == 0) return coord;
            XYCoord c = (quadrant == 1) ? coord : RotateCoord(coord, quadrant - 1);
            XYCoord transform = c + (-7, -7);
            XYCoord rotated = (-transform.y, transform.x);
            return rotated + (7, 7);
        }

        public static XYCoord MapCoordToBoard(XYCoord coord)
        {
            return new XYCoord(coord.x * 3, coord.y * 2) + (1, 1);
        }
    }
    public class XYCoord(int x, int y)
    {
        public int x { get; } = x;
        public int y { get; } = y;

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            XYCoord other = (XYCoord)obj;
            return x == other.x && y == other.y;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public static XYCoord operator +(XYCoord a, XYCoord b)
        {
            return new XYCoord(a.x + b.x, a.y + b.y);
        }
        public static XYCoord operator *(XYCoord a, int x)
        {
            return new XYCoord(a.x * x, a.y * x);
        }
        public static implicit operator (int, int)(XYCoord c) =>
                                (c.x, c.y);
        public static implicit operator XYCoord((int X, int Y) c) =>
                                new(c.X, c.Y);
    }
}
