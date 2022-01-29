using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tetrisProject
{
    public partial class Form1 : Form
    {
        int[,] map = new int[16, 8];
        int size;
        int linesRemoved;
        int score;
        int interval;
        Shape currentShape;

        public Form1()
        {
            InitializeComponent();
            this.KeyUp += new KeyEventHandler(keyFunc);
            Initialize();
        }

        public void Initialize()
        {
            size = 25;
            score = 0;
            linesRemoved = 0;
            interval = 300;

            currentShape = new Shape(3, 0);

            label1.Text = "Score: " + score;
            label2.Text = "Lines: " + linesRemoved; 

            timer1.Interval = interval;
            timer1.Tick += new EventHandler(update);
            timer1.Start();

            Invalidate();
        }

        private void keyFunc(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (!IsIntersects())
                    {
                        ResetArea();
                        currentShape.RotateShape();
                        Merge();
                        Invalidate();
                    }
                    break;
                case Keys.Space:
                    timer1.Interval = 50;
                    break;
                case Keys.Right:
                    if (!CollideHor(1))
                    {
                        ResetArea();
                        currentShape.MoveRight();
                        Merge();
                        Invalidate();
                    }
                    break;
                case Keys.Left:
                    if (!CollideHor(-1))
                    {
                        ResetArea();
                        currentShape.MoveLeft();
                        Merge();
                        Invalidate();
                    }
                    break;
            }
        }

        public void ShowNextShape(Graphics g)
        {
            for (int i = 0; i < currentShape.sizeNextMatrix; i++)
            {
                for (int j = 0; j < currentShape.sizeNextMatrix; j++)
                {
                    if (currentShape.nextMatrix[i, j] == 1)
                    {
                        g.FillRectangle(Brushes.Red, new Rectangle(275 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                    }
                    if (currentShape.nextMatrix[i, j] == 2)
                    {
                        g.FillRectangle(Brushes.Yellow, new Rectangle(275 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                    }
                    if (currentShape.nextMatrix[i, j] == 3)
                    {
                        g.FillRectangle(Brushes.Green, new Rectangle(275 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                    }
                    if (currentShape.nextMatrix[i, j] == 4)
                    {
                        g.FillRectangle(Brushes.Blue, new Rectangle(275 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                    }
                    if (currentShape.nextMatrix[i, j] == 5)
                    {
                        g.FillRectangle(Brushes.Purple, new Rectangle(275 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                    }
                }
            }
        }

        private void update(object sender, EventArgs e)
        {
            ResetArea();
            if (!Collide())
            {
                currentShape.MoveDown();
            }
            else
            {
                Merge();
                SliceMap();
                timer1.Interval = interval;
                currentShape.ResetShape(3, 0);
                if (Collide())
                {
                    ClearMap();
                    timer1.Tick -= new EventHandler(update);
                    timer1.Stop();
                    MessageBox.Show("Your score: " + score);
                    Initialize();
                }
            }
            Merge();
            Invalidate();
        }

        public void SliceMap()
        {
            int count = 0;
            int curRemovedLines = 0;
            for (int i = 0; i < 16; i++)
            {
                count = 0;
                for (int j = 0; j < 8; j++)
                {
                    if (map[i, j] != 0)
                        count++;
                }
                if (count == 8)
                {
                    curRemovedLines++;
                    for (int k = i; k >= 1; k--)
                    {
                        for (int l = 0; l < 8; l++)
                        {
                            map[k, l] = map[k - 1, l];
                        }
                    }
                }
            }
            for (int i = 0; i < curRemovedLines; i++)
            {
                score += 10 * (i + 1);
            }
            linesRemoved += curRemovedLines;

            if (linesRemoved % 2 == 0)
            {
                if (interval > 100)
                    interval -= 10;
            }

            label1.Text = "Score: " + score;
            label2.Text = "Lines: " + linesRemoved;
        }

        public bool IsIntersects()
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (j >= 0 && j <= 7)
                    {
                        if (map[i, j] != 0 && currentShape.matrix[i - currentShape.y, j - currentShape.x] == 0)
                            return true;
                    }
                }
            }
            return false;
        }

        public void Merge()
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                        map[i, j] = currentShape.matrix[i - currentShape.y, j - currentShape.x];
                }
            }
        }

        public bool Collide()
        {
            for (int i = currentShape.y + currentShape.sizeMatrix - 1; i >= currentShape.y; i--)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                    {
                        if (i + 1 == 16)
                            return true;
                        if (map[i + 1, j] != 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool CollideHor(int dir)
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                    {
                        if (j + 1 * dir > 7 || j + 1 * dir < 0)
                            return true;

                        if (map[i, j + 1 * dir] != 0)
                        {
                            if (j - currentShape.x + 1 * dir >= currentShape.sizeMatrix || j - currentShape.x + 1 * dir < 0)
                            {
                                return true;
                            }
                            if (currentShape.matrix[i - currentShape.y, j - currentShape.x + 1 * dir] == 0)
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        public void ResetArea()
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (i >= 0 && j >= 0 && i < 16 && j < 8)
                    {
                        if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                        {
                            map[i, j] = 0;
                        }
                    }
                }
            }
        }

        public void DrawMap(Graphics g)
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (map[i, j] == 1)
                    {
                        g.FillRectangle(Brushes.Red, new Rectangle(25 + j * (size) + 1, 25 + i * (size) + 1, size - 1, size - 1));
                    }
                    if (map[i, j] == 2)
                    {
                        g.FillRectangle(Brushes.Yellow, new Rectangle(25 + j * (size) + 1, 25 + i * (size) + 1, size - 1, size - 1));
                    }
                    if (map[i, j] == 3)
                    {
                        g.FillRectangle(Brushes.Green, new Rectangle(25 + j * (size) + 1, 25 + i * (size) + 1, size - 1, size - 1));
                    }
                    if (map[i, j] == 4)
                    {
                        g.FillRectangle(Brushes.Blue, new Rectangle(25 + j * (size) + 1, 25 + i * (size) + 1, size - 1, size - 1));
                    }
                    if (map[i, j] == 5)
                    {
                        g.FillRectangle(Brushes.Purple, new Rectangle(25 + j * (size) + 1, 25 + i * (size) + 1, size - 1, size - 1));
                    }
                }
            }
        }

        public void ClearMap()
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    map[i, j] = 0;
                }
            }
        }

        public void DrawGrid(Graphics g)
        {
            for (int i = 0; i <= 16; i++)
            {
                g.DrawLine(Pens.Black, new Point(25, 25 + i * size), new Point(25 + 8 * size, 25 + i * size));
            }
            for (int i = 0; i <= 8; i++)
            {
                g.DrawLine(Pens.Black, new Point(25 + i * size, 25), new Point(25 + i * size, 25 + 16 * size));
            }
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            DrawGrid(e.Graphics);
            DrawMap(e.Graphics);
            ShowNextShape(e.Graphics);
        }
    }
}