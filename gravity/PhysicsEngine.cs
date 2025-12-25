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
            int gridSize = 120; // 每隔120px，像素一個採樣的點
            
            var oldSmoothingMode = g.SmoothingMode;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            
            // 建立網格
            for (int x = gridSize / 2; x < 1920; x += gridSize)
            {
                for (int y = gridSize / 2; y < 1080; y += gridSize)
                {
                    double totalForceX = 0;
                    double totalForceY = 0;

                    // 計算所有點受到重力來源的合力
                    foreach (var block in blocks)
                    {
                        // 計算重力來源中心到網格的向量
                        double dx = (block.X + block.Size / 2.0) - x;
                        double dy = (block.Y + block.Size / 2.0) - y;
                        double distance = Math.Sqrt(dx * dx + dy * dy);

                        if (distance > 1)
                        {
                            // 重力 : F = G / r
                            double force = GravityStrength / distance;

                            // 分解成水平分量和垂直分量
                            totalForceX += (dx / distance) * force;
                            totalForceY += (dy / distance) * force;
                        }
                    }

                    // 計算合力大小
                    double magnitude = Math.Sqrt(totalForceX * totalForceX + totalForceY * totalForceY);
                    
                    if (magnitude > 0.1)
                    {
                        // 顏色的強度會隨著強度變化
                        int alpha = Math.Min(255, (int)(magnitude * 40) + 150);
                        int colorIntensity = Math.Min(255, (int)(magnitude * 60) + 180);
                        Color arrowColor = Color.FromArgb(alpha, 0, colorIntensity, 80);
                        
                        // 箭頭長度也隨著重力強度變化，不過有上限
                        double arrowLength = Math.Min(gridSize * 0.75, magnitude * 8);

                        // 計算箭頭的終端座標
                        double arrowX = totalForceX / magnitude * arrowLength;
                        double arrowY = totalForceY / magnitude * arrowLength;

                        // 畫箭頭
                        using (Pen fieldPen = new Pen(arrowColor, 3))
                        {
                            g.DrawLine(fieldPen, x, y, (float)(x + arrowX), (float)(y + arrowY));
                            
                            // 箭頭的方向和角度
                            double arrowHeadSize = 12;
                            double angle = Math.Atan2(arrowY, arrowX);
                            
                            // 箭頭尖點座標
                            float tipX = (float)(x + arrowX);
                            float tipY = (float)(y + arrowY);

                            // 箭頭左邊(rotate -30 degrees)
                            float leftX = (float)(tipX - arrowHeadSize * Math.Cos(angle - Math.PI / 6));
                            float leftY = (float)(tipY - arrowHeadSize * Math.Sin(angle - Math.PI / 6));

                            // 箭頭右邊(rotate +30 degrees)
                            float rightX = (float)(tipX - arrowHeadSize * Math.Cos(angle + Math.PI / 6));
                            float rightY = (float)(tipY - arrowHeadSize * Math.Sin(angle + Math.PI / 6));
                            
                            // 把箭頭左右畫好
                            g.DrawLine(fieldPen, tipX, tipY, leftX, leftY);
                            g.DrawLine(fieldPen, tipX, tipY, rightX, rightY);
                            
                            // 填滿三角形
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