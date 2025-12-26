using System.Runtime.CompilerServices;

namespace gravity
{
    public partial class Form1 : Form
    {
        private readonly PhysicsEngine physicsEngine;
        private readonly System.Windows.Forms.Timer gameTimer;
        private const int DEFAULT_SIZE = 35;
        private const int FRAME_INTERVAL = 16; // 約 60 FPS

        // 控制面板元件
        private Panel controlPanel;
        private CustomTrackBar gravityStrengthTrackBar;
        private Label gravityStrengthLabel;
        private CustomTrackBar maxVelocityTrackBar;
        private Label maxVelocityLabel;
        private CustomTrackBar particleSizeTrackBar;
        private Label particleSizeLabel;
        private CustomTrackBar timeScaleTrackBar;
        private Label timeScaleLabel;
        private CustomTrackBar repulsionStrengthTrackBar;
        private Label repulsionStrengthLabel;
        private Button pauseButton;
        private Button clearButton;
        private Label statsLabel;
        private CheckBox repulsionCheckBox;
        private CheckBox gravityFieldCheckBox;

        private bool isPaused = false;
        private int currentParticleSize = DEFAULT_SIZE;

        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;

            this.ClientSize = new Size(1920, 1080);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "重力模擬系統 - 1920x1780";

            physicsEngine = new PhysicsEngine();

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = FRAME_INTERVAL;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            InitializeControlPanel();
        }

        private void InitializeControlPanel()
        {
            controlPanel = new Panel
            {
                BackColor = Color.FromArgb(1, 0, 0, 0), // 幾乎透明的黑色 (alpha=1)
                Width = 280,
                Height = 580,
                Location = new Point(10, 10),
                Padding = new Padding(10),
                AutoScroll = false,
                BorderStyle = BorderStyle.None
            };
            
            this.Controls.Add(controlPanel);

            int yPosition = 10;
            int labelHeight = 20;
            int trackBarHeight = 45;
            int labelToControlSpacing = 5;
            int sectionSpacing = 15;

            // 重力的強度
            gravityStrengthLabel = new Label
            {
                Text = "重力強度: 30.0",
                ForeColor = Color.White,
                AutoSize = false,
                Width = 260,
                Height = labelHeight,
                Location = new Point(10, yPosition)
            };
            controlPanel.Controls.Add(gravityStrengthLabel);
            yPosition += labelHeight + labelToControlSpacing;

            gravityStrengthTrackBar = new CustomTrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 30,
                Width = 260,
                Location = new Point(10, yPosition)
            };
            gravityStrengthTrackBar.ValueChanged += GravityStrengthTrackBar_ValueChanged;
            controlPanel.Controls.Add(gravityStrengthTrackBar);
            yPosition += trackBarHeight + sectionSpacing;

            // 球的最大速度
            maxVelocityLabel = new Label
            {
                Text = "最大速度: 100.0",
                ForeColor = Color.White,
                AutoSize = false,
                Width = 260,
                Height = labelHeight,
                Location = new Point(10, yPosition)
            };
            controlPanel.Controls.Add(maxVelocityLabel);
            yPosition += labelHeight + labelToControlSpacing;

            maxVelocityTrackBar = new CustomTrackBar
            {
                Minimum = 10,
                Maximum = 300,
                Value = 100,
                Width = 260,
                Location = new Point(10, yPosition)
            };
            maxVelocityTrackBar.ValueChanged += MaxVelocityTrackBar_ValueChanged;
            controlPanel.Controls.Add(maxVelocityTrackBar);
            yPosition += trackBarHeight + sectionSpacing;

            // 粒子的大小
            particleSizeLabel = new Label
            {
                Text = "粒子大小: 35",
                ForeColor = Color.White,
                AutoSize = false,
                Width = 260,
                Height = labelHeight,
                Location = new Point(10, yPosition)
            };
            controlPanel.Controls.Add(particleSizeLabel);
            yPosition += labelHeight + labelToControlSpacing;

            particleSizeTrackBar = new CustomTrackBar
            {
                Minimum = 10,
                Maximum = 100,
                Value = DEFAULT_SIZE,
                Width = 260,
                Location = new Point(10, yPosition)
            };
            particleSizeTrackBar.ValueChanged += ParticleSizeTrackBar_ValueChanged;
            controlPanel.Controls.Add(particleSizeTrackBar);
            yPosition += trackBarHeight + sectionSpacing;

            // 加速或減速時間流逝
            timeScaleLabel = new Label
            {
                Text = "時間縮放: 1.0x",
                ForeColor = Color.White,
                AutoSize = false,
                Width = 260,
                Height = labelHeight,
                Location = new Point(10, yPosition)
            };
            controlPanel.Controls.Add(timeScaleLabel);
            yPosition += labelHeight + labelToControlSpacing;

            timeScaleTrackBar = new CustomTrackBar
            {
                Minimum = 1,
                Maximum = 50,
                Value = 10,
                Width = 260,
                Location = new Point(10, yPosition)
            };
            timeScaleTrackBar.ValueChanged += TimeScaleTrackBar_ValueChanged;
            controlPanel.Controls.Add(timeScaleTrackBar);
            yPosition += trackBarHeight + sectionSpacing;

            // 斥力強度
            repulsionStrengthLabel = new Label
            {
                Text = "排斥力強度: 50.0",
                ForeColor = Color.White,
                AutoSize = false,
                Width = 260,
                Height = labelHeight,
                Location = new Point(10, yPosition)
            };
            controlPanel.Controls.Add(repulsionStrengthLabel);
            yPosition += labelHeight + labelToControlSpacing;

            repulsionStrengthTrackBar = new CustomTrackBar
            {
                Minimum = 0,
                Maximum = 20,
                Value = 50,
                Width = 260,
                Location = new Point(10, yPosition)
            };
            repulsionStrengthTrackBar.ValueChanged += RepulsionStrengthTrackBar_ValueChanged;
            controlPanel.Controls.Add(repulsionStrengthTrackBar);
            yPosition += trackBarHeight + sectionSpacing;

            // 是否啟用球間排斥力
            repulsionCheckBox = new CheckBox
            {
                Text = "啟用球間排斥力",
                ForeColor = Color.White,
                AutoSize = true,
                Checked = true,
                Location = new Point(10, yPosition)
            };
            repulsionCheckBox.CheckedChanged += (s, e) => physicsEngine.EnableRepulsion = repulsionCheckBox.Checked;
            controlPanel.Controls.Add(repulsionCheckBox);
            yPosition += 25 + sectionSpacing;

            // 顯示重力場
            gravityFieldCheckBox = new CheckBox
            {
                Text = "顯示重力場 (綠色箭頭)",
                ForeColor = Color.White,
                AutoSize = true,
                Checked = true, // 預設開啟
                Location = new Point(10, yPosition)
            };
            gravityFieldCheckBox.CheckedChanged += (s, e) => physicsEngine.ShowGravityField = gravityFieldCheckBox.Checked;
            controlPanel.Controls.Add(gravityFieldCheckBox);
            yPosition += 25 + sectionSpacing;

            // 按鈕
            pauseButton = new Button
            {
                Text = "暫停",
                Width = 125,
                Height = 35,
                Location = new Point(10, yPosition),
                BackColor = Color.FromArgb(70, 70, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            pauseButton.Click += PauseButton_Click;
            controlPanel.Controls.Add(pauseButton);

            clearButton = new Button
            {
                Text = "清除全部",
                Width = 125,
                Height = 35,
                Location = new Point(145, yPosition),
                BackColor = Color.FromArgb(70, 70, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            clearButton.Click += ClearButton_Click;
            controlPanel.Controls.Add(clearButton);
            yPosition += 45;

            // 資料統計
            statsLabel = new Label
            {
                Text = "粒子: 0 | 重力源: 0",
                ForeColor = Color.Lime,
                AutoSize = false,
                Width = 260,
                Height = 30,
                Location = new Point(10, yPosition),
                Font = new Font("Consolas", 9, FontStyle.Bold)
            };
            controlPanel.Controls.Add(statsLabel);
            yPosition += 35;

            controlPanel.BringToFront();
        }

        private void GravityStrengthTrackBar_ValueChanged(object sender, EventArgs e)
        {
            double value = gravityStrengthTrackBar.Value;
            physicsEngine.GravityStrength = value;
            gravityStrengthLabel.Text = $"重力強度: {value:F1}";
        }

        private void MaxVelocityTrackBar_ValueChanged(object sender, EventArgs e)
        {
            double value = maxVelocityTrackBar.Value;
            physicsEngine.MaxVelocity = value;
            maxVelocityLabel.Text = $"最大速度: {value:F1}";
        }

        private void ParticleSizeTrackBar_ValueChanged(object sender, EventArgs e)
        {
            currentParticleSize = particleSizeTrackBar.Value;
            particleSizeLabel.Text = $"粒子大小: {currentParticleSize}";
        }

        private void TimeScaleTrackBar_ValueChanged(object sender, EventArgs e)
        {
            double timeScale = timeScaleTrackBar.Value / 10.0;
            physicsEngine.TimeScale = timeScale;
            timeScaleLabel.Text = $"時間縮放: {timeScale:F1}x";
        }

        private void RepulsionStrengthTrackBar_ValueChanged(object sender, EventArgs e)
        {
            double value = repulsionStrengthTrackBar.Value;
            physicsEngine.RepulsionStrength = value;
            repulsionStrengthLabel.Text = $"排斥力強度: {value:F1}";
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            isPaused = !isPaused;
            pauseButton.Text = isPaused ? "繼續" : "暫停";
            pauseButton.BackColor = isPaused ? Color.FromArgb(100, 50, 50) : Color.FromArgb(70, 70, 70);
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            physicsEngine.Clear();
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            if (controlPanel.Bounds.Contains(e.Location))
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                physicsEngine.AddBallWithVelocity(e.X, e.Y, currentParticleSize, 0, 0);
                this.Invalidate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                physicsEngine.AddBlock(e.X, e.Y, currentParticleSize);
                this.Invalidate();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            if (e.KeyCode == Keys.Space)
            {
                PauseButton_Click(null, null);
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                physicsEngine.Update(this.ClientSize.Width, this.ClientSize.Height);
            }
            
            statsLabel.Text = $"粒子: {physicsEngine.BallCount} | 重力源: {physicsEngine.BlockCount}";
            
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            physicsEngine.Draw(e.Graphics);
        }
    }
}
