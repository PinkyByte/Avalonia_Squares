namespace Squares.Model
{
    public class TableEventArgs : EventArgs
    {
        private int x;
        private int y;
        private bool vertical;

        public int X { get { return x; } }
        public int Y { get { return y; } }
        public bool IsVertical { get { return vertical; } }

        public TableEventArgs(int x, int y, bool vertical)
        {
            this.x = x;
            this.y = y;
            this.vertical = vertical;
        }
    }
}
