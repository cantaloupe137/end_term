namespace gravity
{
    public class Block
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int Size { get; set; }
        public bool IsRepulsive { get; set; } = false;

        public Block(double x, double y, int size)
        {
            X = x;
            Y = y;
            Size = size;
        }

        public void Draw(Graphics g)
        {
            Color blockColor = IsRepulsive ? Color.Red : Color.Blue;
            
            using (SolidBrush brush = new SolidBrush(blockColor))
            {
                g.FillRectangle(brush, (float)X, (float)Y, Size, Size);
            }

            using (Pen pen = new Pen(Color.White, 2))
            {
                g.DrawRectangle(pen, (float)X, (float)Y, Size, Size);
            }
        }
    }
}