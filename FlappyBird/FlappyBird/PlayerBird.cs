using System.Drawing;

namespace FlappyBird
{
    public class PlayerBird
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Size { get; }
        public float VelocityY { get; set; }
        public float Gravity { get; set; }
        public float JumpStrength { get; set; }

        public Rectangle Bounds => new Rectangle((int)X, (int)Y, Size, Size);

        public PlayerBird(float startX, float startY, int size = 32)
        {
            X = startX;
            Y = startY;
            Size = size;
            Gravity = 0.65f;
            JumpStrength = -9.5f;
            VelocityY = 0f;
        }

        public void Jump()
        {
            VelocityY = JumpStrength;
        }

        public void Update()
        {
            VelocityY += Gravity;
            Y += VelocityY;
        }

        public void Reset(float startX, float startY)
        {
            X = startX;
            Y = startY;
            VelocityY = 0f;
        }
    }
}