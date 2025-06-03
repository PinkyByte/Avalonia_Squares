using Squares.Persistence;

namespace Squares.Model
{
    public class SquaresModel
    {

        #region Constants

        public const int GameSizeSmall = 3;
        public const int GameSizeMedium = 5;
        public const int GameSizeLarge = 9;

        #endregion

        #region Fields

        private int gameSize;
        private Player player01;
        private Player player02;
        private Player currentPlayer;
        private Table table;
        private ISquaresDataAccess dataAccess;

        #endregion

        #region Events

        public event EventHandler<TableEventArgs>? TableChange;
        public event EventHandler<PlayerEventArgs>? ScoreIncrease;
        public event EventHandler<GameEventArgs>? GameOver;
        #endregion

        #region Constructor

        public SquaresModel(ISquaresDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
            gameSize = GameSizeSmall;
            table = new Table(gameSize);
            player01 = new Player("kék");
            player02 = new Player("narancs");
            currentPlayer = player01;
        }

        #endregion

        #region Properties

        public int GameSize { get { return gameSize; } set { gameSize = value; } }
        public Player CurrentPlayer { get { return currentPlayer; } }
        public int PlayerOneScore() { return player01.Score; }
        public int PlayerTwoScore() { return player02.Score; }
        public Table Table { get { return table; } }
        public bool IsGameOver()
        {
            for (int i = 0; i < table.Rows.GetLength(0); i++)
            {
                for (int j = 0; j < table.Rows.GetLength(1); j++)
                {
                    if (!table.Rows[i, j])
                    {
                        return false;
                    }
                }
            }
            for (int i = 0; i < table.Columns.GetLength(0); i++)
            {
                for (int j = 0; j < table.Columns.GetLength(1); j++)
                {
                    if (!table.Columns[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion

        #region Public Methods

        public void NewGame()
        {
            table = new Table(gameSize);
            player01.Score = 0;
            player02.Score = 0;
            currentPlayer = player01;
        }
        public void DrawLine(int x, int y, bool vertical)
        {
            bool scored = false;
            if (vertical && !table.Columns[x, y])
            {
                table.Columns[x, y] = true;
                OnTableChange(x, y, vertical);
                if (HasScoredTwo(x, y, vertical))
                {
                    currentPlayer.Score += 2;
                    OnScoreIncrease(currentPlayer, 2);
                    scored = true;
                }
                else if (HasScoredOne(x, y, vertical))
                {
                    currentPlayer.Score += 1;
                    OnScoreIncrease(currentPlayer, 1);
                    scored = true;
                }
            }
            else if (!vertical && !table.Rows[x, y])
            {
                table.Rows[x, y] = true;
                OnTableChange(x, y, vertical);
                if (HasScoredTwo(x, y, vertical))
                {
                    currentPlayer.Score += 2;
                    OnScoreIncrease(currentPlayer, 2);
                    scored = true;
                }
                else if (HasScoredOne(x, y, vertical))
                {
                    currentPlayer.Score += 1;
                    OnScoreIncrease(currentPlayer, 1);
                    scored = true;
                }
            }
            if (!scored)
            {
                if (currentPlayer == player01)
                {
                    currentPlayer = player02;
                }
                else
                {
                    currentPlayer = player01;
                }
            }
            if (IsGameOver())
            {
                if (player01.Score > player02.Score)
                {
                    OnGameOver(player01);
                }
                else
                {
                    OnGameOver(player02);
                }
            }
        }
        public async Task LoadGame(string path)
        {
            await LoadGame(File.OpenRead(path));
        }
        public async Task LoadGame(Stream path)
        {
            (table, player01, player02) = await dataAccess.LoadData(path);
            gameSize = table.Rows.GetLength(1);
        }
        public async Task SaveGame(string path)
        {
            await SaveGame(File.OpenWrite(path));
        }
        public async Task SaveGame(Stream path)
        {
            await dataAccess.SaveData(path, table, player01, player02);
        }

        #endregion

        #region Private Methods

        private void OnTableChange(int x, int y, bool vertical)
        {
            TableChange?.Invoke(this, new TableEventArgs(x, y, vertical));
        }
        private void OnScoreIncrease(Player player, int score)
        {
            ScoreIncrease?.Invoke(this, new PlayerEventArgs(player, score));
        }
        private void OnGameOver(Player winner)
        {
            GameOver?.Invoke(this, new GameEventArgs(winner));
        }
        private bool HasScoredOne(int x, int y, bool vertical)
        {
            if (vertical)
            {
                if (y == 0)
                {
                    return table.Rows[x, y] && table.Rows[x + 1, y] && table.Columns[x, y + 1];
                }
                else if (y == table.Columns.GetLength(1) - 1)
                {
                    return table.Rows[x, y - 1] && table.Rows[x + 1, y - 1] && table.Columns[x, y - 1];
                }
                else
                {
                    return (table.Rows[x, y - 1] && table.Rows[x + 1, y - 1] && table.Columns[x, y - 1]) || (table.Rows[x, y] && table.Rows[x + 1, y] && table.Columns[x, y + 1]);
                }
            }
            else
            {
                if (x == 0)
                {
                    return table.Columns[x, y] && table.Columns[x, y + 1] && table.Rows[x + 1, y];
                }
                else if (x == table.Rows.GetLength(0) - 1)
                {
                    return table.Columns[x - 1, y] && table.Columns[x - 1, y + 1] && table.Rows[x - 1, y];
                }
                else
                {
                    return (table.Columns[x - 1, y] && table.Columns[x - 1, y + 1] && table.Rows[x - 1, y]) || (table.Columns[x, y] && table.Columns[x, y + 1] && table.Rows[x + 1, y]);
                }
            }
        }
        private bool HasScoredTwo(int x, int y, bool vertical)
        {
            if (vertical && y > 0 && y < table.Columns.GetLength(1) - 1)
            {
                return table.Rows[x, y - 1] && table.Rows[x + 1, y - 1] && table.Rows[x, y] && table.Rows[x + 1, y] && table.Columns[x, y - 1] && table.Columns[x, y + 1];
            }
            else if (!vertical && x > 0 && x < table.Rows.GetLength(0) - 1)
            {
                return table.Columns[x, y] && table.Columns[x, y + 1] && table.Columns[x - 1, y] && table.Columns[x - 1, y + 1] && table.Rows[x - 1, y] && table.Rows[x + 1, y];
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
