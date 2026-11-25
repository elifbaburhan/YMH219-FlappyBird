using System;
using System.Drawing;

namespace FlappyBird
{
    public class PipeObstacle
    {
        private static readonly Random Rng = new Random();

        public int X { get; set; }
        public int Width { get; }
        public int GapHeight { get; }
        public int TopPipeHeight { get; private set; }
        public int BottomPipeY => TopPipeHeight + GapHeight;
        public bool Scored { get; set; }

        public Rectangle TopPipeBounds => new Rectangle(X, 0, Width, Math.Max(0, TopPipeHeight));

        public Rectangle BottomPipeBounds(int playableHeight)
        {
            int y = Math.Min(BottomPipeY, playableHeight);
            int h = Math.Max(0, playableHeight - y);
            return new Rectangle(X, y, Width, h);
        }

        public PipeObstacle(int startX, int playableHeight, int width = 65, int gapHeight = 150)
        {
            Width = width;
            GapHeight = gapHeight;
            Reset(startX, playableHeight);
        }

        public void Move(int speed)
        {
            X -= speed;
        }

        public void Reset(int newX, int playableHeight)
        {
            X = newX;
            Scored = false;

            int margin = 50;
            int maxTop = Math.Max(margin + 20, playableHeight - GapHeight - margin);
            TopPipeHeight = Rng.Next(margin, maxTop);
        }
    }
}
