using Squares.Persistence;

namespace Squares.Model
{
    public class PlayerEventArgs : EventArgs
    {
        private Player player;
        private int score;

        public Player Player { get { return player; } }
        public int Score { get { return score; } }

        public PlayerEventArgs(Player player, int score)
        {
            this.player = player;
            this.score = score;
        }
    }
}
