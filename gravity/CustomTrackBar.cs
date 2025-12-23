using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace gravity
{
    public class CustomTrackBar : Control
    {
        private int minimum = 0;
        private int maximum = 100;
        private int value = 0;
        private int tickFrequency = 10;
        private bool isDragging = false;
        
        public int Minimum
        {
            get => minimum;
            set
            {
                minimum = value;
                if (this.value < minimum) this.value = minimum;
                Invalidate();
            }
        }

        public int Maximum
        {
            get => maximum;
            set
            {
                maximum = value;
                if (this.value > maximum) this.value = maximum;
                Invalidate();
            }
        }

        public int Value
        {
            get => value;
            set
            {
                int newValue = Math.Max(minimum, Math.Min(maximum, value));
                if (this.value != newValue)
                {
                    this.value = newValue;
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        public int TickFrequency
        {
            get => tickFrequency;
            set
            {
                tickFrequency = value;
                Invalidate();
            }
        }

        public event EventHandler ValueChanged;

        public CustomTrackBar()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true); 
            this.DoubleBuffered = true;
            this.Height = 45;
            this.BackColor = Color.FromArgb(1, 0, 0, 0); 
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int trackHeight = 6;
            int trackY = (Height - trackHeight) / 2;
            int sliderWidth = 16;
            int sliderHeight = 20;

            using (SolidBrush trackBrush = new SolidBrush(Color.FromArgb(100, 150, 150, 150)))
            {
                g.FillRectangle(trackBrush, 0, trackY, Width, trackHeight);
            }

            float valuePercent = (float)(value - minimum) / (maximum - minimum);
            int fillWidth = (int)(Width * valuePercent);
            
            // 只在有足夠寬度時才繪製填充部分
            if (fillWidth > 1)
            {
                using (LinearGradientBrush fillBrush = new LinearGradientBrush(
                    new Rectangle(0, trackY, fillWidth, trackHeight),
                    Color.FromArgb(180, 0, 200, 100),
                    Color.FromArgb(180, 0, 150, 200),
                    LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(fillBrush, 0, trackY, fillWidth, trackHeight);
                }
            }

            int sliderX = (int)(Width * valuePercent) - sliderWidth / 2;
            int sliderY = (Height - sliderHeight) / 2;
            
            // 確保滑塊不會超出邊界
            sliderX = Math.Max(0, Math.Min(Width - sliderWidth, sliderX));
            
            using (SolidBrush sliderBrush = new SolidBrush(Color.FromArgb(220, 255, 255, 255)))
            {
                g.FillEllipse(sliderBrush, sliderX, sliderY, sliderWidth, sliderHeight);
            }
            
            using (Pen sliderBorder = new Pen(Color.FromArgb(180, 100, 200, 150), 2))
            {
                g.DrawEllipse(sliderBorder, sliderX, sliderY, sliderWidth, sliderHeight);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                UpdateValueFromMouse(e.X);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isDragging)
            {
                UpdateValueFromMouse(e.X);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isDragging = false;
        }

        private void UpdateValueFromMouse(int mouseX)
        {
            float percent = Math.Max(0, Math.Min(1, (float)mouseX / Width));
            Value = minimum + (int)((maximum - minimum) * percent);
        }
    }
}
