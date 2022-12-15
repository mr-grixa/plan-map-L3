using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Windows.Forms;

namespace L3
{
    public partial class Form1 : Form
    {
        string[] map;
        string[] space=new string[40] ;
        Point start, finish;
        public Form1()
        {
            InitializeComponent();
        }

        private void button_load_Click(object sender, EventArgs e)
        {

            OpenFileDialog OpenFileDialog1 = new OpenFileDialog();
            OpenFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            OpenFileDialog1.FilterIndex = 2;
            OpenFileDialog1.RestoreDirectory = true;
            if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (StreamReader sw = new StreamReader(OpenFileDialog1.FileName, false))
                {
                    map = sw.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                }
                reload_map();
                draw();
            }


        }

        private void button_save_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName, false))
                {
                    foreach (string s in map)
                    {
                        sw.WriteLine(s);
                    }
                }
            }
        }
        public void reload_map()
        {
            map.CopyTo(space, 0);
            for (int y = 1; y < space.Length - 1; y++)
            {
                for (int x = 1; x < space[y].Length - 1; x++)
                {
                    if (map[y][x] == '#')
                    {
                        space[y] = space[y].Remove(x - 1, 3).Insert(x - 1, "###");
                        space[y + 1] = space[y + 1].Remove(x - 1, 3).Insert(x - 1, "###");
                        space[y - 1] = space[y - 1].Remove(x - 1, 3).Insert(x - 1, "###");
                    }

                }
            }
            for (int y = 2; y < space.Length - 2; y++)
            {
                for (int x = 2; x < space[y].Length - 2; x++)
                {
                    if (space[y][x] != '#')
                    {
                        space[y] = space[y].Remove(x, 1).Insert(x, "@");
                    }
                }
            }
        }
        public void draw()
        {
            Graphics g = pictureBox1.CreateGraphics();
            for (int y = 0; y < map.Length; y++)
            {
                for (int x = 0; x < map[y].Length; x++)
                {
                    if (map[y][x] == '#')
                    {
                        g.FillRectangle(Brushes.Black, x * 10, y * 10, 10, 10);
                    }
                    else
                    {
                        g.FillRectangle(Brushes.White, x * 10, y * 10, 10, 10);
                    }
                    if (checkBox_conf.Checked)
                    {
                        if (space[y][x] == '@')
                        {
                            g.FillRectangle(Brushes.LightGreen, x * 10, y * 10, 10, 10);
                        }
                        else if (space[y][x] == 'X')
                        {
                            g.FillRectangle(Brushes.Yellow, x * 10, y * 10, 10, 10);
                        }
                    }
                }
            }

            g.FillEllipse(Brushes.Green, start.X * 10, start.Y * 10, 10, 10);
            g.FillEllipse(Brushes.Blue, finish.X * 10, finish.Y * 10, 10, 10);
        }
        int x, y;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (map != null)
            {
                x = e.X / 10;
                y = e.Y / 10;
                if (e.Button == MouseButtons.Left)
                {
                    Graphics g = pictureBox1.CreateGraphics();                  
                    if (map[y][x] == '#')
                    {
                        g.FillRectangle(Brushes.White, x * 10, y * 10, 10, 10);
                        map[y] = map[y].Remove(x, 1).Insert(x, ".");
                    }
                    else
                    {
                        g.FillRectangle(Brushes.Black, x * 10, y * 10, 10, 10);
                        map[y] = map[y].Remove(x, 1).Insert(x, "#");
                    }
                    reload_map();
                    draw();
                }
                else
                {
                    Point xy = Cursor.Position;
                    contextMenuStrip1.Show(xy);
                    
                }

            }

        }

        public void search_random()
        {
            Random r = new Random();
            Pen pen = new Pen(Color.Orange, 5);
            Point local = start;
            Graphics g = pictureBox1.CreateGraphics();

            space[local.Y] = space[local.Y].Remove(local.X, 1).Insert(local.X, "X");
            List<Point> buffer = new List<Point>();
            int[,] cena= new int[64,40];
            for (int X = 0; X < 64; X++)
            {
                for (int Y = 0; Y < 40; Y++)
                {
                    cena[X, Y] = 99999;
                }
            }
            int I = 0;
            cena[start.X, start.Y] = I;
            while (local != finish)
            {
                I++;
                for (int X = local.X-1; X <= local.X+1; X++)
                {
                    for (int Y = local.Y - 1; Y <= local.Y + 1; Y++)
                    {
                        if (space[Y][X] == '@')
                        {
                            if ((local.X != X || local.Y != Y)&& cena[X, Y] > I)
                            {
                                buffer.Add(new Point(X, Y));
                                cena[X, Y] = I;
                            }                            
                        }
                    }
                }
                
                if (buffer.Count == 0)
                {
                    MessageBox.Show("Нет пути!");
                    break;
                }

                draw();
                int buff = r.Next(0, buffer.Count);
                local = buffer[buff];
                space[local.Y] = space[local.Y].Remove(local.X, 1).Insert(local.X, "X");
                Point B = local;
                Point nB = local;
                while (cena[B.X, B.Y] != 0)
                {
                    int min = 99999999;
                    for (int X = B.X - 1; X <= B.X + 1; X++)
                    {
                        for (int Y = B.Y - 1; Y <= B.Y + 1; Y++)
                        {
                            if (min> cena[X, Y])
                            {
                                min = cena[X, Y];
                                nB = new Point(X, Y);
                            }
                        }
                    }
                    g.DrawLine(pen, B.X * 10, B.Y * 10, nB.X * 10, nB.Y * 10);
                    B = nB;
                }
                buffer.RemoveAt(buff);
            }
        }

        private void toolStripMenuItem_finish_Click(object sender, EventArgs e)
        {
            finish = new Point(x, y);
            draw();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            search_random();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            reload_map();
            draw();
        }

        private void toolStripMenuItem_start_Click(object sender, EventArgs e)
        {
            start =new Point(x, y);
            draw();
        }


    }
}
