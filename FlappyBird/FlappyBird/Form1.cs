using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlappyBird
{
    public partial class Form1 : Form
    {
        private readonly System.Windows.Forms.Timer _gameTimer = new System.Windows.Forms.Timer();
        private readonly GameState _gameState = new GameState(initialPipeSpeed: 6);
        private PlayerBird _bird = null!;
        private PipeObstacle[] _pipes = null!;
        private Button _restartButton = null!;

        private const int GroundHeight = 50;
        private const int PipeGap = 150;
        private const int PipeSpacing = 280;

        public Form1()
        {
            InitializeComponent();
            ConfigureForm();
            InitializeControls();
            InitializeGame();
        }

        private void ConfigureForm()
        {
            Text = "YMH219 - Flappy Bird";
            ClientSize = new Size(480, 640);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            DoubleBuffered = true;
            KeyPreview = true;

            _gameTimer.Interval = 16; // ~60 FPS
            _gameTimer.Tick += GameTimer_Tick;
            KeyDown += Form1_KeyDown;
        }

        private void InitializeControls()
        {
            _restartButton = new Button
            {
                Text = "Yeniden Başlat (R)",
                Size = new Size(160, 42),
                Location = new Point((ClientSize.Width - 160) / 2, (ClientSize.Height / 2) + 30),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Visible = false,
                TabStop = false
            };
            _restartButton.Click += (_, _) => RestartGame();
            Controls.Add(_restartButton);
        }

        private void InitializeGame()
        {
            int playableHeight = ClientSize.Height - GroundHeight;
            _bird = new PlayerBird(startX: 90, startY: playableHeight / 2f - 16, size: 32);

            int firstPipeX = ClientSize.Width + 60;
            _pipes = new[]
            {
                new PipeObstacle(firstPipeX, playableHeight, width: 65, gapHeight: PipeGap),
                new PipeObstacle(firstPipeX + PipeSpacing, playableHeight, width: 65, gapHeight: PipeGap)
            };

            _gameState.Reset(initialPipeSpeed: 6);
            _restartButton.Visible = false;
            _gameTimer.Start();
            Invalidate();
        }

        private void RestartGame()
        {
            InitializeGame();
            Focus();
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Up)
            {
                if (!_gameState.IsGameOver)
                {
                    _bird.Jump();
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.R && _gameState.IsGameOver)
            {
                RestartGame();
                e.Handled = true;
            }
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            if (_gameState.IsGameOver)
                return;

            int playableHeight = ClientSize.Height - GroundHeight;
            _bird.Update();

            // Tavan sınırını koru
            if (_bird.Y < 0)
            {
                _bird.Y = 0;
                _bird.VelocityY = 0;
            }

            // Zemin çarpışması
            if (_bird.Bounds.Bottom >= playableHeight)
            {
                _bird.Y = playableHeight - _bird.Size;
                TriggerGameOver();
                return;
            }

            foreach (var pipe in _pipes)
            {
                pipe.Move(_gameState.PipeSpeed);

                // Çarpışma kontrolü
                Rectangle topPipe = pipe.TopPipeBounds;
                Rectangle bottomPipe = pipe.BottomPipeBounds(playableHeight);

                if (_bird.Bounds.IntersectsWith(topPipe) || _bird.Bounds.IntersectsWith(bottomPipe))
                {
                    TriggerGameOver();
                    return;
                }

                // Boru geçildiğinde skor artır
                if (!pipe.Scored && pipe.X + pipe.Width < _bird.X)
                {
                    pipe.Scored = true;
                    _gameState.IncrementScore();
                }

                // Ekran solundan çıkan boruyu sağda yeniden konumlandır
                if (pipe.X + pipe.Width < 0)
                {
                    int maxPipeX = GetRightmostPipeX();
                    int nextX = Math.Max(ClientSize.Width + 40, maxPipeX + PipeSpacing);
                    pipe.Reset(nextX, playableHeight);
                }
            }

            Invalidate();
        }

        private int GetRightmostPipeX()
        {
            int maxX = int.MinValue;
            foreach (var pipe in _pipes)
            {
                if (pipe.X > maxX)
                    maxX = pipe.X;
            }
            return maxX;
        }

        private void TriggerGameOver()
        {
            _gameState.IsGameOver = true;
            _gameTimer.Stop();
            _restartButton.Visible = true;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            int playableHeight = ClientSize.Height - GroundHeight;
             
            // Gökyüzü
            g.Clear(Color.SkyBlue);

            // Borular
            using (Brush pipeBrush = new SolidBrush(Color.ForestGreen))
            using (Pen borderPen = new Pen(Color.DarkGreen, 2))
            {
                foreach (var pipe in _pipes)
                {
                    Rectangle top = pipe.TopPipeBounds;
                    Rectangle bottom = pipe.BottomPipeBounds(playableHeight);

                    g.FillRectangle(pipeBrush, top);
                    g.DrawRectangle(borderPen, top);

                    g.FillRectangle(pipeBrush, bottom);
                    g.DrawRectangle(borderPen, bottom);
                }
            }

            // Zemin
            Rectangle groundRect = new Rectangle(0, playableHeight, ClientSize.Width, GroundHeight);
            using (Brush groundBrush = new SolidBrush(Color.Sienna))
            using (Brush grassBrush = new SolidBrush(Color.LimeGreen))
            {
                g.FillRectangle(groundBrush, groundRect);
                g.FillRectangle(grassBrush, new Rectangle(0, playableHeight, ClientSize.Width, 10));
            }

            // Kuş
            using (Brush birdBrush = new SolidBrush(Color.Gold))
            using (Pen birdPen = new Pen(Color.OrangeRed, 2))
            {
                g.FillEllipse(birdBrush, _bird.Bounds);
                g.DrawEllipse(birdPen, _bird.Bounds);
            }

            // Skor göstergesi
            using (Font scoreFont = new Font("Segoe UI", 22, FontStyle.Bold))
            using (Brush scoreBrush = new SolidBrush(Color.White))
            {
                g.DrawString($"Skor: {_gameState.Score}", scoreFont, scoreBrush, new PointF(16, 14));
            }

            // Game Over ekranı
            if (_gameState.IsGameOver)
            {
                using (Brush overlayBrush = new SolidBrush(Color.FromArgb(140, Color.Black)))
                using (Font overFont = new Font("Segoe UI", 28, FontStyle.Bold))
                using (Font infoFont = new Font("Segoe UI", 12, FontStyle.Regular))
                using (Brush textBrush = new SolidBrush(Color.White))
                {
                    g.FillRectangle(overlayBrush, ClientRectangle);
                    g.DrawString("GAME OVER", overFont, textBrush, new PointF(118, 210));
                    g.DrawString($"Toplam Skor: {_gameState.Score}", infoFont, textBrush, new PointF(175, 265));
                }
            }
        }
    }
}