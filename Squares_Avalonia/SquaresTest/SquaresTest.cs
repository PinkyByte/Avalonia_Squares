using Squares.Model;
using Squares.Persistence;
using Moq;

namespace SquaresTest
{
    [TestClass]
    public class SquaresTest
    {
        private SquaresModel model = null!;
        private Table table = null!;
        private Player player01 = null!;
        private Player player02 = null!;
        private Mock<ISquaresDataAccess> mock = null!;
        [TestInitialize]
        public void Initialize()
        {
            table = new Table(SquaresModel.GameSizeMedium);
            player01 = new Player("kék");
            player02 = new Player("narancs");
            for (int i = 0; i < table.Rows.GetLength(0); i++)
            {
                for (int j = 0; j < table.Rows.GetLength(1); j++)
                {
                    table.Rows[i, j] = false;
                }
            }
            for (int i = 0; i < table.Columns.GetLength(0); i++)
            {
                for (int j = 0; j < table.Columns.GetLength(1); j++)
                {
                    table.Columns[i, j] = false;
                }
            }
            mock = new Mock<ISquaresDataAccess>();
            mock.Setup(m => m.LoadData(It.IsAny<Stream>())).Returns(() => Task.FromResult((table, player01, player02)));
            model = new SquaresModel(mock.Object);
            model.TableChange += new EventHandler<TableEventArgs>(Model_TableChange);
            model.ScoreIncrease += new EventHandler<PlayerEventArgs>(Model_ScoreIncrease);
            model.GameOver += new EventHandler<GameEventArgs>(Model_GameOver);
        }
        [TestMethod]
        public void GameSizeSmallTest()
        {
            model.GameSize = SquaresModel.GameSizeSmall;
            model.NewGame();
            int lines = 0;
            for (int i = 0; i < model.Table.Rows.GetLength(0); i++)
            {
                for (int j = 0; j < model.Table.Rows.GetLength(1); j++)
                {
                    lines++;
                }
            }
            for (int i = 0; i < model.Table.Columns.GetLength(0); i++)
            {
                for (int j = 0; j < model.Table.Columns.GetLength(1); j++)
                {
                    lines++;
                }
            }
            Assert.AreEqual(24, lines);
            Assert.AreEqual(0, model.PlayerOneScore());
            Assert.AreEqual(0, model.PlayerTwoScore());
        }
        [TestMethod]
        public void GameSizeMediumTest()
        {
            model.GameSize = SquaresModel.GameSizeMedium;
            model.NewGame();
            int lines = 0;
            for (int i = 0; i < model.Table.Rows.GetLength(0); i++)
            {
                for (int j = 0; j < model.Table.Rows.GetLength(1); j++)
                {
                    lines++;
                }
            }
            for (int i = 0; i < model.Table.Columns.GetLength(0); i++)
            {
                for (int j = 0; j < model.Table.Columns.GetLength(1); j++)
                {
                    lines++;
                }
            }
            Assert.AreEqual(60, lines);
            Assert.AreEqual(0, model.PlayerOneScore());
            Assert.AreEqual(0, model.PlayerTwoScore());
        }
        [TestMethod]
        public void GameSizeLargeTest()
        {
            model.GameSize = SquaresModel.GameSizeLarge;
            model.NewGame();
            int lines = 0;
            for (int i = 0; i < model.Table.Rows.GetLength(0); i++)
            {
                for (int j = 0; j < model.Table.Rows.GetLength(1); j++)
                {
                    lines++;
                }
            }
            for (int i = 0; i < model.Table.Columns.GetLength(0); i++)
            {
                for (int j = 0; j < model.Table.Columns.GetLength(1); j++)
                {
                    lines++;
                }
            }
            Assert.AreEqual(180, lines);
            Assert.AreEqual(0, model.PlayerOneScore());
            Assert.AreEqual(0, model.PlayerTwoScore());
        }
        [TestMethod]
        public void IsGameOverTest()
        {
            model.GameSize = SquaresModel.GameSizeMedium;
            model.NewGame();
            for (int i = 0; i < model.Table.Rows.GetLength(0); i++)
            {
                for (int j = 0; j < model.Table.Rows.GetLength(1); j++)
                {
                    model.Table.Rows[i, j] = true;
                }
            }
            for (int i = 0; i < model.Table.Columns.GetLength(0); i++)
            {
                for (int j = 0; j < model.Table.Columns.GetLength(1); j++)
                {
                    model.Table.Columns[i, j] = true;
                }
            }
            model.Table.Columns[0, 0] = false;
            model.DrawLine(0, 0, true);
            Assert.IsTrue(model.IsGameOver());
        }
        [TestMethod]
        public void DrawLineAndScoreTwoVerticalTest()
        {
            model.GameSize = SquaresModel.GameSizeMedium;
            model.NewGame();
            model.Table.Rows[0, 0] = true;
            model.Table.Rows[0, 1] = true;
            model.Table.Rows[1, 0] = true;
            model.Table.Rows[1, 1] = true;
            model.Table.Columns[0, 0] = true;
            model.Table.Columns[0, 2] = true;
            model.DrawLine(0, 1, true);
            Assert.AreEqual(2, model.PlayerOneScore());
            Assert.AreEqual(0, model.PlayerTwoScore());
        }
        [TestMethod]
        public void DrawLineAndScoreTwoHorizontalTest()
        {
            model.GameSize = SquaresModel.GameSizeMedium;
            model.NewGame();
            model.Table.Rows[0, 0] = true;
            model.Table.Rows[2, 0] = true;
            model.Table.Columns[0, 0] = true;
            model.Table.Columns[0, 1] = true;
            model.Table.Columns[1, 0] = true;
            model.Table.Columns[1, 1] = true;
            model.DrawLine(1, 0, false);
            Assert.AreEqual(2, model.PlayerOneScore());
            Assert.AreEqual(0, model.PlayerTwoScore());
        }
        [TestMethod]
        public void DrawLineAndScoreOneVerticalTest()
        {
            model.GameSize = SquaresModel.GameSizeMedium;
            model.NewGame();
            model.Table.Rows[0, 0] = true;
            model.Table.Rows[1, 0] = true;
            model.Table.Columns[0, 1] = true;
            model.DrawLine(0, 0, true);
            Assert.AreEqual(1, model.PlayerOneScore());
            Assert.AreEqual(0, model.PlayerTwoScore());
        }
        [TestMethod]
        public void DrawLineAndScoreOneHorizontalTest()
        {
            model.GameSize = SquaresModel.GameSizeMedium;
            model.NewGame();
            model.Table.Rows[1, 0] = true;
            model.Table.Columns[0, 0] = true;
            model.Table.Columns[0, 1] = true;
            model.DrawLine(0, 0, false);
            Assert.AreEqual(1, model.PlayerOneScore());
            Assert.AreEqual(0, model.PlayerTwoScore());
        }
        [TestMethod]
        public async Task LoadGameTest()
        {
            model.NewGame();
            MemoryStream stream = new MemoryStream();
            await model.LoadGame(stream);
            for (int i = 0; i < table.Rows.GetLength(0); i++)
            {
                for (int j = 0; j < table.Rows.GetLength(1); j++)
                {
                    Assert.AreEqual(table.Rows[i, j], model.Table.Rows[i, j]);
                }
            }
            for (int i = 0; i < table.Columns.GetLength(0); i++)
            {
                for (int j = 0; j < table.Columns.GetLength(1); j++)
                {
                    Assert.AreEqual(table.Columns[i, j], model.Table.Columns[i, j]);
                }
            }
            Assert.AreEqual(player01.Score, model.PlayerOneScore());
            Assert.AreEqual(player02.Score, model.PlayerTwoScore());
            mock.Verify(dataAccess => dataAccess.LoadData(stream), Times.Once());
        }
        private void Model_GameOver(object? sender, GameEventArgs e)
        {
            Assert.AreEqual(e.Winner.Score, model.PlayerOneScore());
        }
        private void Model_ScoreIncrease(object? sender, PlayerEventArgs e)
        {
            Assert.AreEqual(e.Player.Score, model.PlayerOneScore());
        }
        private void Model_TableChange(object? sender, TableEventArgs e)
        {
            Assert.IsTrue(model.Table.Columns[0, 1]);
        }
    }
}
