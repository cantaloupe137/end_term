# 期末專案 -  gravity simulator
一個模擬重力的小程式

## 專案截圖
<img width="1905" height="974" alt="圖片" src="https://github.com/user-attachments/assets/4c0f4c68-da8f-43c1-bc8e-aacc91fc6380" />

## 功能
基於原本重力模擬的內容，把畫線發射的功能移了，取而代之增加的是新的Control Panel，能夠選擇球的大小，球的大小會和他們受到的重力有關，球與球之間有排斥力，以及能夠畫出重力場的功能，更直觀的顯示為何離重力源越近的位置，所受到的重力場強度越強。

模擬球和球之間的排斥力
```csharp
public void ApplyGravity(List<Block> attractors, double gravityStrength, double maxVelocity)
{
    double totalAccelerationX = 0;  
    double totalAccelerationY = 0;

    foreach (var block in attractors)
    {
        double dx = block.X - X;
        double dy = block.Y - Y;
        double distance = Math.Sqrt(dx * dx + dy * dy);

        if (distance > 1)
        {
            // 加速度
            double acceleration = gravityStrength / distance;
            
            totalAccelerationX += (dx / distance) * acceleration;
            totalAccelerationY += (dy / distance) * acceleration;
        }
    }

    VelocityX += totalAccelerationX;
    VelocityY += totalAccelerationY;

    LimitVelocity(maxVelocity);
}
```
速度的限制
```csharp
 private void LimitVelocity(double maxVelocity)
        {
            double velocity = Math.Sqrt(VelocityX * VelocityX + VelocityY * VelocityY);
            if (velocity > maxVelocity)
            {
                VelocityX = (VelocityX / velocity) * maxVelocity;
                VelocityY = (VelocityY / velocity) * maxVelocity;
            }
        }
```

重力的實作
```csharp
public void ApplyGravity(List<Block> attractors, double gravityStrength, double maxVelocity)
        {
            if (attractors.Count == 0) return;

            double totalAccelX = 0;
            double totalAccelY = 0;

            foreach (var block in attractors)
            {
                double dx = block.X - X;
                double dy = block.Y - Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);

                if (distance > 1)
                {
                    double Accel = gravityStrength * Mass / distance;
                    totalAccelX += (dx / distance) * Accel;
                    totalAccelY += (dy / distance) * Accel;
                }
            }

            VelocityX += totalAccelX;
            VelocityY += totalAccelY;

            LimitVelocity(maxVelocity);
        }
```

物體的實作
```csharp
public Ball(double x, double y, int size, Color color)
        {
            X = x;
            Y = y;
            Size = size;
            BallColor = color;
            VelocityX = 0;
            VelocityY = 0;
            Trail = new List<PointF>();

            Mass = (size / 2.0) * (size / 2.0) * Math.PI / 100.0;
        }
```

繪製重力場的箭頭
```csharp
sing (Pen fieldPen = new Pen(arrowColor, 3))
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
```
