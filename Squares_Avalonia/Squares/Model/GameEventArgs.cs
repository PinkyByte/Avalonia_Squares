using Squares.Persistence;

namespace Squares.Model
{
    public class GameEventArgs : EventArgs
    {
        private Player winner;

        public Player Winner { get { return winner; } }

        public GameEventArgs(Player winner)
        {
            this.winner = winner;
        }
    }
}
