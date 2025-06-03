namespace Squares.Persistence
{
    public class Table
    {
        private bool[,] rows;
        private bool[,] columns;

        public bool[,] Rows { get { return rows; } }
        public bool[,] Columns { get { return columns; } }

        public Table(int n)
        {
            rows = new bool[n + 1, n];
            columns = new bool[n, n + 1];
        }
    }
}
