namespace gravity
{
    public class Block
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int Size { get; set; }

        public Block(double x, double y, int size)
        {
            X = x;
            Y = y;
            Size = size;
        }

        public void Draw(Graphics g)
        {
            using (SolidBrush brush = new SolidBrush(Color.Blue))
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