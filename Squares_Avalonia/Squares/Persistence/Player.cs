namespace Squares.Persistence
{
    public class Player
    {
        private int score;
        private string color;
        public int Score { get { return score; } set { score = value; } }
        public string Color { get { return color; } }

        public Player(string color)
        {
            score = 0;
            this.color = color;
        }
        public Player(int score, string color)
        {
            this.score = score;
            this.color = color;
        }
    }
}
