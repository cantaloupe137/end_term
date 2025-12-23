namespace gravity
{
    public class PhysicsEngine
    {
        private readonly List<Ball> balls = new List<Ball>();
        private readonly List<Block> blocks = new List<Block>();
        private static readonly Random random = new Random();
        
        public double GravityStrength { get; set; } = 30.0;
        public double MaxVelocity { get; set; } = 100.0;
        public double TimeScale { get; set; } = 1.0;
        public bool EnableRepulsion { get; set; } = true;
        public bool ShowGravityField { get; set; } = true;
        public double RepulsionStrength { get; set; } = 50.0;

        public void AddBallWithVelocity(double x, double y, int size, double velocityX, double velocityY)
        {
            Color randomColor = GetRandomColor();
            var ball = new Ball(x, y, size, randomColor)
            {
                VelocityX = velocityX,
                VelocityY = velocityY
            };
            balls.Add(ball);
        }

        private static Color GetRandomColor()
        {
            int r = random.Next(100, 256);
            int g = random.Next(100, 256);
            int b = random.Next(100, 256);
            return Color.FromArgb(r, g, b);
        }

        public void AddBlock(double x, double y, int size)
        {
            blocks.Add(new Block(x, y, size));
        }

        public void Clear()
        {
            balls.Clear();
            blocks.Clear();
        }

        public void Update(int screenWidth, int screenHeight)
        {
            if (blocks.Count > 0)
            {
                foreach (var ball in balls)
                {
                    ball.ApplyGravity(blocks, GravityStrength * TimeScale, MaxVelocity);
                    
                    if (EnableRepulsion)
                    {
                        ball.ApplyRepulsion(balls, RepulsionStrength * TimeScale);
                    }
                    
                    ball.UpdatePosition(TimeScale);
                }

                balls.RemoveAll(ball => ball.IsOutOfBounds(screenWidth, screenHeight));
            }
        }

        public void Draw(Graphics g)
        {
            // 第一層：重力場（最底層）
            if (ShowGravityField && blocks.Count > 0)
            {
                DrawGravityField(g);
            }

            // 第二層：球的軌跡
            foreach (var ball in balls)
            {
                ball.DrawTrail(g);
            }

            // 第三層：球本體
            foreach (var ball in balls)
            {
                ball.DrawBall(g);
            }

            // 第四層：方塊（最上層）
            foreach (var block in blocks)
            {
                block.Draw(g);
            }
        }

        private void DrawGravityField(Graphics g)
        {
            int gridSize = 120;
            
            var oldSmoothingMode = g.SmoothingMode;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            
            for (int x = gridSize / 2; x < 1920; x += gridSize)
            {
                for (int y = gridSize / 2; y < 1080; y += gridSize)
                {
                    double totalForceX = 0;
                    double totalForceY = 0;

                    foreach (var block in blocks)
                    {
                        double dx = (block.X + block.Size / 2.0) - x;
                        double dy = (block.Y + block.Size / 2.0) - y;
                        double distance = Math.Sqrt(dx * dx + dy * dy);

                        if (distance > 1)
                        {
                            double force = GravityStrength / distance;
                            totalForceX += (dx / distance) * force;
                            totalForceY += (dy / distance) * force;
                        }
                    }

                    double magnitude = Math.Sqrt(totalForceX * totalForceX + totalForceY * totalForceY);
                    
                    if (magnitude > 0.1)
                    {
                        int alpha = Math.Min(255, (int)(magnitude * 40) + 150);
                        int colorIntensity = Math.Min(255, (int)(magnitude * 60) + 180);
                        Color arrowColor = Color.FromArgb(alpha, 0, colorIntensity, 80);
                        
                        double arrowLength = Math.Min(gridSize * 0.75, magnitude * 8);
                        double arrowX = totalForceX / magnitude * arrowLength;
                        double arrowY = totalForceY / magnitude * arrowLength;

                        using (Pen fieldPen = new Pen(arrowColor, 3))
                        {
                            g.DrawLine(fieldPen, x, y, (float)(x + arrowX), (float)(y + arrowY));
                            
                            double arrowHeadSize = 12;
                            double angle = Math.Atan2(arrowY, arrowX);
                            
                            float tipX = (float)(x + arrowX);
                            float tipY = (float)(y + arrowY);
                            float leftX = (float)(tipX - arrowHeadSize * Math.Cos(angle - Math.PI / 6));
                            float leftY = (float)(tipY - arrowHeadSize * Math.Sin(angle - Math.PI / 6));
                            float rightX = (float)(tipX - arrowHeadSize * Math.Cos(angle + Math.PI / 6));
                            float rightY = (float)(tipY - arrowHeadSize * Math.Sin(angle + Math.PI / 6));
                            
                            g.DrawLine(fieldPen, tipX, tipY, leftX, leftY);
                            g.DrawLine(fieldPen, tipX, tipY, rightX, rightY);
                            
                            using (SolidBrush arrowBrush = new SolidBrush(arrowColor))
                            {
                                PointF[] triangle = { 
                                    new PointF(tipX, tipY), 
                                    new PointF(leftX, leftY), 
                                    new PointF(rightX, rightY) 
                                };
                                g.FillPolygon(arrowBrush, triangle);
                            }
                        }
                    }
                }
            }
            
            g.SmoothingMode = oldSmoothingMode;
        }

        public int BallCount => balls.Count;
        public int BlockCount => blocks.Count;
    }
}