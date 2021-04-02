using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace курсач
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SolidBrush brash = new SolidBrush(Color.Red);
        SolidBrush back = new SolidBrush(Color.Black);

        int head, tail;
        const int len = 1000;
        int[] x = new int[len];
        int[] y = new int[len];
        int n;
        int m;
        int prevx, prevy;
        int a = 15;
        bool start = true;
        byte w;
        Graphics g;
        bool changed;
        Pen pen = new Pen(Color.White, 2);
        Point apple;
        Random rand = new Random();
        SolidBrush app = new SolidBrush(Color.Yellow);
        int maxVal = 0;
        int val, score, lav;
        SolidBrush wall = new SolidBrush(Color.Gray);
        bool win1, win2, win3, win4, win5;
        bool lost, stop, exc;
        string path = @"..\Debug\save.txt";
        string best = @"..\Debug\best.txt";

        private void Stopped()
        {
            if (stop == false) timer1.Enabled = false;
            else
            {
                timer1.Enabled = true;
                stop = false;
                return;
            }
            stop = true;
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Flush();
                //sw.WriteLine("Была ли игра закончена?");
                sw.WriteLine("false");
                //sw.WriteLine("Координаты змейки:");
                for (int i = tail; i < head; i = (i + 1) % len)
                {
                    sw.Write(x[i].ToString() + " ");
                }
                sw.WriteLine();
                for (int i = tail; i < head; i = (i + 1) % len)
                {
                    sw.Write(y[i].ToString() + " ");
                }
                sw.WriteLine();
                //sw.WriteLine("Координаты яблока:");
                sw.WriteLine(apple.X.ToString() + " " + apple.Y.ToString());
                //sw.WriteLine("Уровень остановки :");
                if (win5 == true) lav = 5;
                else if (win4 == true) lav = 4;
                else if (win3 == true) lav = 3;
                else if (win2 == true) lav = 2;
                else if (win1 == true) lav = 1;
                else lav = 0;
                sw.WriteLine(lav.ToString());
                //Значение timer1.Interval
                sw.WriteLine(timer1.Interval.ToString());
                //Направление движения
                sw.WriteLine(w.ToString());
                //счёт на момент закрытия
                sw.WriteLine(score.ToString());
                sw.Close();
            }
         }

        private void Continue()
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string s = sr.ReadLine();
                if (s == "true") return;
                s = sr.ReadLine();
                if (s == null) return;
                string[] s1 = s.Split(' ');
                for (int i = 0; i < s1.Length-1; i++) x[i] = Convert.ToInt32(s1[i]);
                s = sr.ReadLine();
                string[] s2 = s.Split(' ');
                for (int i = 0; i < s2.Length-1; i++) y[i] = Convert.ToInt32(s2[i]);
                s = sr.ReadLine();
                head = s1.Length - 2;
                tail = 0;
                string[] s3 = s.Split(' ');
                apple.X = Convert.ToInt32(s3[0]);
                apple.Y = Convert.ToInt32(s3[1]);
                lav = Convert.ToInt32(sr.ReadLine());
                timer1.Interval = Convert.ToInt32(sr.ReadLine());
                w = Convert.ToByte(sr.ReadLine());
                score = Convert.ToInt32(sr.ReadLine());
                sr.Close();
            }
            label3.ForeColor = Color.White;
            label3.Visible = false;
            label4.Visible = true;            
            label4.ForeColor = Color.White;
            label4.Text = "Через 3 секунд игра продолжится";
            button1.Enabled = false;
            button1.Visible = false;
            start = false;
            exc = true;
            timer3.Enabled = true;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            g = CreateGraphics();
            exc = false;
            using (StreamReader sr = new StreamReader(best))
            {
                string s = sr.ReadLine();
                if (s != null ) maxVal = Convert.ToInt32(s);
                sr.Close();
            }
            n = Size.Width;
            m = Size.Height;
            BackColor = Color.Black;
            //label1.ForeColor = Color.White;
            //label1.Text = n.ToString() + " " + m.ToString();
            button1.Visible = true;
            label3.Visible = false;
            label4.Visible = false;
            button1.Text = "Начать игру";
            button1.BackColor = Color.Black;
            button1.ForeColor = Color.White;
            timer1.Interval = 100;
            changed = true;
            win1 = false;
            win2 = false;
            win3 = false;
            win4 = true;
            win5 = false;
            button1.Visible = false;
            button1.Enabled = false;
            label5.Visible = true;
            label5.Enabled = true;
            label5.ForeColor = Color.White;
            label5.Text = "Если вы хотите выйти из игры,не потеряв прогресс, то перед закрытием игры нажмите'Пробел' ";
            label3.ForeColor = Color.White;
            label3.Visible = false;
            label3.Enabled = false;
            timer2.Enabled = false;
            timer2.Interval = 2000;
            Continue();
            if (exc == true) timer4.Enabled = false;
        }

        private void BuildNewWall(int idx)
        {
            g.FillRectangle(back, 0, 0, n, m);
            g.FillRectangle(wall, 0, 0, a, m);
            g.FillRectangle(wall, n - 2 * a, 0, a, m);
            g.FillRectangle(wall, 0, 0, n, a);
            g.FillRectangle(wall, 0, m - 3 * a, n, a);

            label3.ForeColor = Color.White;
            label3.Visible = true;
            label3.Enabled = true;
            label4.Visible = true; 
            label4.Text = "Через 5 секунд начнётся следующий уровень";
            label4.ForeColor = Color.White;
            timer2.Enabled = true;
            timer1.Enabled = false;
            if (idx == 1)
            {
                if (exc != true) label3.Text = "Поздравляю! Первый уровень пройден!";
                g.FillRectangle(wall, 8 * a, 9 * a, 20 * a, a);
                g.FillRectangle(wall, 8 * a, 9 * a, a, 20 * a);
                win1 = true;
            }
            if (idx == 2)
            {
                if (exc != true) label3.Text = "Поздравляю! Второй уровень пройден!";
                g.FillRectangle(wall, 8 * a, 9 * a, 20 * a, a);
                g.FillRectangle(wall, 8 * a, 9 * a, a, 20 * a);
                g.FillRectangle(wall, 33 * a, 30 * a, a, 15 * a);
                win2 = true;
            }
            if (idx == 3)
            {
                if (exc != true) label3.Text = "Поздравляю! Третий уровень пройден!";
                g.FillRectangle(wall, 8 * a, 9 * a, 20 * a, a);
                g.FillRectangle(wall, 8 * a, 9 * a, a, 20 * a);
                g.FillRectangle(wall, 33 * a, 30 * a, a, 15 * a);
                g.FillRectangle(wall, 28 * a, 0, a, 10 * a);
                win3 = true;
            }
            if (idx == 4)
            {
                if (exc != true) label3.Text = "Поздравляю! Четвёртый уровень пройден!";
                g.FillRectangle(wall, 8 * a, 9 * a, 20 * a, a);
                g.FillRectangle(wall, 8 * a, 9 * a, a, 20 * a);
                g.FillRectangle(wall, 33 * a, 30 * a, a, 15 * a);
                g.FillRectangle(wall, 28 * a, 0, a, 10 * a);
                g.FillRectangle(wall, 20 * a, 30 * a, 13 * a, a);
                win4 = true;
            }
            if (idx == 5)
            {
                if (exc != true) label3.Text = "Поздравляю! Пятый уровень пройден!";
                g.FillRectangle(wall, 8 * a, 9 * a, 20 * a, a);
                g.FillRectangle(wall, 8 * a, 9 * a, a, 20 * a);
                g.FillRectangle(wall, 33 * a, 30 * a, a, 15 * a);
                g.FillRectangle(wall, 28 * a, 0, a, 10 * a);
                g.FillRectangle(wall, 20 * a, 30 * a, 13 * a, a);
                timer1.Interval = 45;
                win5 = true;
            }
        }

        private void Congratulations()
        {
            label1.ForeColor = Color.White;
            label2.ForeColor = Color.White;
            label1.Text = "Поздравляю!!! Не представляю как, но игра пройдена!!!";
            timer1.Enabled = false;
            label1.Enabled = true;
            label1.Visible = true;
            label2.Enabled = true;
            label2.Visible = true;
            label2.Text = "Длина змейки = 500!!!!!!";
            button1.Enabled = true;
            button1.Visible = true;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            label3.Visible = false;
            label4.Visible = false;
            label3.Enabled = false;
            label4.Enabled = false;
            timer1.Enabled = true;
            label5.Enabled = false;
            label5.Visible = false;
            label1.Visible = false;
            label2.Visible = false;
            start = false;
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            label3.Visible = false;
            label3.Enabled = false;
            button1.Visible = true;
            button1.Enabled = true;
            timer4.Enabled = false;
            label5.Visible = false;
            label5.Enabled = false;
        }

        private void NewLavel()
        {
            if (val < 20) return;
            else if (val == 20 && win1 == false) BuildNewWall(1);
            else if (val == 40 && win2 == false) BuildNewWall(2);
            else if (val == 60 && win3 == false) BuildNewWall(3);
            else if (val == 80 && win4 == false) BuildNewWall(4);
            else if (val == 100 && win5 == false) BuildNewWall(5);
            else if (val == 200) Congratulations();
        }

        private bool Eat(Graphics g)
        {
            if (x[head] == apple.X && y[head] == apple.Y)
            {
                apple.X = (rand.Next(a, n - 3 * a) / a) * a;
                apple.Y = (rand.Next(a, m - 3 * a) / a) * a;
                if (timer1.Interval > 70) timer1.Interval -= 2;
                while ( (Run(apple.X, apple.Y) && NotWall(apple.X, apple.Y) && Heap(apple.X, apple.Y)) == !true)
                {
                    apple.X = (rand.Next(a, n - 2 * a) / a) * a;
                    apple.Y = (rand.Next(a, m - 3 * a) / a) * a;
                }
                g.FillEllipse(app, apple.X, apple.Y, a, a);
                val++;
                score++;
                NewLavel();
                return true;
            }
            
            else return false;
        }

        private bool NotWall(int xx, int yy)
        {
            if ((win4 == true || win5 == true) && ((xx >= 8 * a && xx <= 28 * a && yy == 9 * a) || (xx == 8 * a && yy >= 9 * a && yy <= 28 * a) || (xx == 33 * a && yy >= 30 * a && yy <= 45 * a) || (xx == 28 * a && yy >= 0 && yy <= 8 * a) || (xx >= 20 * a && xx <= 32 * a && yy == 30 * a))) return false;
            else if ((win3 == true) && ((xx >= 8 * a && xx <= 27 * a && yy == 9 * a) || (xx == 8 * a && yy >= 9 * a && yy <= 28 * a) || (xx == 33 * a && yy >= 30 * a && yy <= 45 * a) || (xx == 28 * a && yy >= 0 && yy <= 8 * a))) return false;
            else if ((win2 == true) && ((xx >= 8 * a && xx <= 27 * a && yy == 9 * a) || (xx == 8 * a && yy >= 9 * a && yy <= 28 * a) || (xx == 33 * a && yy >= 30 * a && yy <= 45 * a))) return false;
            else if ((win1 == true) && ((xx >= 8 * a && xx <= 27 * a && yy == 9 * a) || (xx == 8 * a && yy >= 9 * a && yy <= 28 * a))) return false;
            else return true;
        }

        private bool Run(int xx, int yy)
        {
            int i = tail;
            while ( i != head )
            {
                if (xx == x[i] && yy == y[i]) return false;
                i = (i + 1) % len;
            }            
            return true;
        }

        private void Lose()
        {
            timer1.Enabled = false;
            lost = true;
            using (StreamReader sr = new StreamReader(path)) { sr.ReadLine(); sr.Close(); }
            using (StreamWriter sw = new StreamWriter(path)) { sw.WriteLine("true"); sw.Close(); }
                if (score > maxVal) maxVal = score;
            timer1.Interval = 100;
            button1.Enabled = true;
            button1.Visible = true;
            label1.Enabled = true;
            label1.Visible = true;
            label2.Enabled = true;
            label2.Visible = true;
            label1.ForeColor = Color.White;
            label2.ForeColor = Color.White;
            label1.Text = "Счёт игры - " + score.ToString();
            label2.Text = "Рекорд - " + maxVal.ToString();
            if (score == maxVal) using (StreamWriter sw = new StreamWriter(best)) { sw.WriteLine(score.ToString());sw.Close(); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            g.FillRectangle(back, 0, 0, n, m);
            g.FillRectangle(wall, 0, 0, a, m);
            g.FillRectangle(wall, n - 2 * a,0, a, m);
            g.FillRectangle(wall, 0, 0, n, a);
            g.FillRectangle(wall, 0, m - 3 * a, n, a);
            button1.Enabled = false;
            button1.Visible = false;
            timer1.Enabled = true;
            label1.Visible = false;
            label2.Visible = false;
            lost = false;

            win1 = false;
            win2 = false;
            win3 = false;
            win4 = false;
            win5 = false;

            /*win1 = true;
            win2 = true;
            win3 = true;
            win4 = true;
            win5 = true;

            BuildNewWall(5);E*/
            score = 0;
            start = true;
            w = 0;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left && w != 0 && changed == true) w = 2;
            else if (e.KeyCode == Keys.Right && w != 2 && changed == true) w = 0;
            else if (e.KeyCode == Keys.Up && w != 1 && changed == true) w = 3;
            else if (e.KeyCode == Keys.Down && w != 3 && changed == true) w = 1;
            changed = false;
            if (e.KeyCode == Keys.Space) Stopped();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            start = true;
            label3.Visible = false;
            label4.Visible = false;
            label3.Enabled = false;
            label4.Enabled = false;
            timer1.Enabled = true;
            timer2.Enabled = false;
        }

        private bool Heap(int xx, int yy)
        {
            if (xx == x[head] && yy == y[head]) return false;
            else return true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (exc == true)
            {
                g.FillRectangle(back, 0, 0, n, m);
                g.FillRectangle(wall, 0, 0, a, m);
                g.FillRectangle(wall, n - 2 * a, 0, a, m);
                g.FillRectangle(wall, 0, 0, n, a);
                g.FillRectangle(wall, 0, m - 3 * a, n, a);

                if (lav == 1)
                {
                    g.FillRectangle(wall, 8 * a, 9 * a, 20 * a, a);
                    g.FillRectangle(wall, 8 * a, 9 * a, a, 20 * a);
                    win1 = true;
                }
                if (lav == 2)
                {
                    g.FillRectangle(wall, 8 * a, 9 * a, 20 * a, a);
                    g.FillRectangle(wall, 8 * a, 9 * a, a, 20 * a);
                    g.FillRectangle(wall, 33 * a, 30 * a, a, 15 * a);
                    win2 = true;
                    win1 = true;
                }
                if (lav == 3)
                {
                    g.FillRectangle(wall, 8 * a, 9 * a, 20 * a, a);
                    g.FillRectangle(wall, 8 * a, 9 * a, a, 20 * a);
                    g.FillRectangle(wall, 33 * a, 30 * a, a, 15 * a);
                    g.FillRectangle(wall, 28 * a, 0, a, 10 * a);
                    win2 = true;
                    win1 = true;
                    win3 = true;
                }
                if (lav == 4)
                {
                    g.FillRectangle(wall, 8 * a, 9 * a, 20 * a, a);
                    g.FillRectangle(wall, 8 * a, 9 * a, a, 20 * a);
                    g.FillRectangle(wall, 33 * a, 30 * a, a, 15 * a);
                    g.FillRectangle(wall, 28 * a, 0, a, 10 * a);
                    g.FillRectangle(wall, 20 * a, 30 * a, 13 * a, a);
                    win4 = true;
                    win2 = true;
                    win1 = true;
                    win3 = true;
                }
                if (lav == 5)
                {
                    g.FillRectangle(wall, 8 * a, 9 * a, 20 * a, a);
                    g.FillRectangle(wall, 8 * a, 9 * a, a, 20 * a);
                    g.FillRectangle(wall, 33 * a, 30 * a, a, 15 * a);
                    g.FillRectangle(wall, 28 * a, 0, a, 10 * a);
                    g.FillRectangle(wall, 20 * a, 30 * a, 13 * a, a);
                    timer1.Interval = 45;
                    win5 = true;
                    win4 = true;
                    win2 = true;
                    win1 = true;
                    win3 = true;
                }
                g.FillEllipse(app, apple.X, apple.Y, a, a);
                int i = tail;
                while ( i != (head + 1))
                {
                    g.FillRectangle(brash, x[i], y[i], a, a);
                    i = (i + 1) % len;
                }
                exc = false;
            } else
            if (start == true )
            {
                head = 2;
                tail = 0;
                x[0] = a;
                y[0] = y[1] = y[2] = a;
                x[1] = 2 * a;
                x[2] = 3 * a;
                w = 0;
                val = 3;
                g.FillRectangle(brash, x[0], y[0], a, a);
                g.FillRectangle(brash, x[1], y[1], a, a);
                g.FillRectangle(brash, x[2], y[2], a, a);
                apple.X = (rand.Next(a, n - 3 * a) / a) * a;
                apple.Y = (rand.Next(a, m - 3 * a) / a) * a;
                while ((Run(apple.X, apple.Y) && NotWall(apple.X, apple.Y) && Heap(apple.X, apple.Y)) == !true)
                {
                    apple.X = (rand.Next(a,n-3*a)/a) * a;
                    apple.Y = (rand.Next(a,m-3*a)/a) * a;
                }
                g.FillEllipse(app, apple.X, apple.Y, a, a);
                start = false;
                changed = true;
                lost = false;
                stop = false;
            }

            prevx = x[head];
            prevy = y[head];
            head = (head + 1) % len;
            
            if (w == 0)
            {
                x[head] = prevx + a;
                y[head] = prevy;
                if (x[head] > n - 3*a) Lose();
            }
            if (w == 1)
            {
                x[head] = prevx;
                y[head] = prevy + a;
                if (y[head] > m - 4*a) Lose();
            }
            if (w == 2)
            {
                x[head] = prevx - a;
                y[head] = prevy;
                if (x[head] < a) Lose();
            }
            if (w == 3)
            {
                x[head] = prevx;
                y[head] = prevy - a;
                if (y[head] < a) Lose();
            }
            if (Run(x[head], y[head]) == false || NotWall(x[head], y[head]) == false) Lose();
            if (lost == false) g.FillRectangle(brash, x[head], y[head], a, a);

            if (Eat(g) == false && lost == false)
            {
                g.FillRectangle(back, x[tail], y[tail], a, a);
                tail = (tail + 1) % len;
            }
            changed = true;
        }
    }
}