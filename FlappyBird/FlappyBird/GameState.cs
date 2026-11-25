namespace FlappyBird
{
    public class GameState
    {
        public int Score { get; set; }
        public int PipeSpeed { get; set; }
        public bool IsGameOver { get; set; }

        public GameState(int initialPipeSpeed = 6)
        {
            Reset(initialPipeSpeed);
        }

        public void Reset(int initialPipeSpeed = 6)
        {
            Score = 0;
            PipeSpeed = initialPipeSpeed;
            IsGameOver = false;
        }

        public void IncrementScore()
        {
            Score++;
        }
    }
}