namespace LudoAPI.Entities
{
    public class BoardView(Board board, Player[] players, Peg[] pegs)
    {
        private readonly Board board;
        private readonly Player[] players;
        private readonly Peg[] pegs;

        private static readonly int IndexPegStationA = 48 * 4 + 6;
        private static readonly int IndexPegStationB = IndexPegStationA + 27;
        private static readonly int IndexPegStationC = IndexPegStationA + 48*18;
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
            for (int i = 0; i < 4; i++)
            {
                boardBase[IndexPegStationA + ((int)(i / 2) * 4 * 48) + (i % 2) * 6] = 'a';
                boardBase[IndexPegStationB + ((int)(i / 2) * 4 * 48) + (i % 2) * 6] = 'b';
                boardBase[IndexPegStationC + ((int)(i / 2) * 4 * 48) + (i % 2) * 6] = 'c';
                boardBase[IndexPegStationD + ((int)(i / 2) * 4 * 48) + (i % 2) * 6] = 'd';
            }
            return new string(boardBase);
        }
    }
    public class XYCoord(int x, int y)
    {
        public int x;
        public int y;
    }
}
