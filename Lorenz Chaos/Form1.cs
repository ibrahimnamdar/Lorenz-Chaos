using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lorenz_Chaos
{
    public partial class Form1 : Form
    {
        Bitmap offScrBuff;
        Mutex mut;
        int index = 0;
        double sigma=10.0, beta=2.3;
        int p = 28;
        
        public Form1()
        {
            
            InitializeComponent();
            offScrBuff = new Bitmap(1000, 1000);
            mut = new Mutex();
            pictureBox1.Paint += new PaintEventHandler(pictureBox1_Paint);
            button1.Click += new System.EventHandler(this.button1_Click);
            //textBox1.Text = p.ToString();
            //textBox1.Text = sigma.ToString();
            //textBox1.Text = beta.ToString();
        }

        void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);
            mut.WaitOne();
            e.Graphics.DrawImage(offScrBuff, 0, 0);
            mut.ReleaseMutex();
        }

        void DrawLorenzChaos(double a, double b, double r)
        {
            //double a = 10, b = (8.0 / 3.0), r = 28;     //standard values for lorenz model

            /*m defines the number of iterations of the for loop so the number of lines drawn
            good idea to keep m inversely proportional to dt (the time interval). A smaller dt will
            mean smaller lines so smoother overall drawing m=50000 and dt=0.0005 is a good starting point
            that demonstrates chaos well*/
            double m = 500000, dt = 0.00055;

            //EVOLUTION VALUE FOR RUNGE_KUTTA METHOD
            //values for first particle
            double y11, y12, y13;
            double y21, y22, y23;
            double y31, y32, y33;
            double y41, y42, y43;
            double y51, y52, y53;
            double xi, yi, xf, yf;           //coordinates for drawing particle 1 trajectory

            double f10, f11, f12, f13;      //function values to be calculated, 
            double f20, f21, f22, f23;      //for fxy (x>1) these are intermediate fn calculations at different
            double f30, f31, f32, f33;      //times in Runga Kutta

            //values for second particle
            double z11, z12, z13;
            double z21, z22, z23;
            double z31, z32, z33;
            double z41, z42, z43;
            double z51, z52, z53;
            double ai, bi, af, bf;          //coordinates for drawing particle 2 trajectory (these are badly named...)

            double g10, g11, g12, g13;      //equivalent of f values for particle 2 
            double g20, g21, g22, g23;
            double g30, g31, g32, g33;

            //OTHER NEEDED QUANTITIES
            int i;          //for loop iteration integer
            int k1 = 20;    //scaling factors to make drawing fill form
            int k2 = 9;
            int y0 = 280;   //offset values to centre drawing on form
            int x0 = 400;
            int start = 10;   //starting position for calculations
            double diff = 0.01;//initial displacement between two particles

            //starting positions for particles            
            y11 = start;//particle 1
            y12 = start;
            y13 = start;

            z11 = start + diff;//particle 2
            z12 = start + diff;
            z13 = start + diff;

            //initial coords for particles at t=0
            xi = (y11) * k1 + x0;
            yi = (y12) * k2 + y0;
            ai = (z11) * k1 + x0;
            bi = (z12) * k2 + y0;
            for (i = 1; i <= m; i++)
            {
                f10 = a * (y12 - y11);
                f20 = r * y11 - y12 - y11 * y13;
                f30 = y11 * y12 - b * y13;

                y21 = y11 + f10 * dt / 2;               //finding y1 y2 y3 at the first
                y22 = y12 + f20 * dt / 2;               //fraction of dt
                y23 = y13 + f30 * dt / 2;

                f11 = a * (y22 - y21);
                f21 = r * y21 - y22 - y21 * y23;
                f31 = y21 * y22 - b * y23;

                y31 = y11 + f11 * dt / 2;               //finding y1 y2 y3 at the second
                y32 = y12 + f21 * dt / 2;               //fraction of dt
                y33 = y13 + f31 * dt / 2;

                f12 = a * (y32 - y31);
                f22 = r * y31 - y32 - y31 * y33;
                f32 = y31 * y32 - b * y33;

                y41 = y11 + f12 * dt;               //finding y1 y2 y3 at the third
                y42 = y12 + f22 * dt;               //fraction of dt
                y43 = y13 + f32 * dt;

                f13 = a * (y42 - y41);
                f23 = r * y41 - y42 - y41 * y43;
                f33 = y41 * y42 - b * y43;

                y51 = y11 + (f10 + 2 * f11 + 2 * f12 + f13) * dt / 6; //final y values at y(t+dt)
                y52 = y12 + (f20 + 2 * f21 + 2 * f22 + f23) * dt / 6; //then to be repesated in for loop for all steps
                y53 = y13 + (f30 + 2 * f31 + 2 * f32 + f33) * dt / 6;

                xf = (y51) * k1 + x0;
                yf = (y52) * k2 + y0;

                //second particle calculation
                g10 = a * (z12 - z11);
                g20 = r * z11 - z12 - z11 * z13;
                g30 = z11 * z12 - b * z13;

                z21 = z11 + g10 * dt / 2;               //finding y1 y2 y3 at the first
                z22 = z12 + g20 * dt / 2;               //fraction of dt
                z23 = z13 + g30 * dt / 2;

                g11 = a * (z22 - z21);
                g21 = r * z21 - z22 - z21 * z23;
                g31 = z21 * z22 - b * z23;

                z31 = z11 + g11 * dt / 2;               //finding y1 y2 y3 at the second
                z32 = z12 + g21 * dt / 2;               //fraction of dt
                z33 = z13 + g31 * dt / 2;

                g12 = a * (z32 - z31);
                g22 = r * z31 - z32 - z31 * z33;
                g32 = z31 * z32 - b * z33;

                z41 = z11 + g12 * dt;               //finding y1 y2 y3 at the third
                z42 = z12 + g22 * dt;               //fraction of dt
                z43 = z13 + g32 * dt;

                g13 = a * (z42 - z41);
                g23 = r * z41 - z42 - z41 * z43;
                g33 = z41 * z42 - b * z43;

                z51 = z11 + (g10 + 2 * g11 + 2 * g12 + g13) * dt / 6; //final y values at y(t+dt)
                z52 = z12 + (g20 + 2 * g21 + 2 * g22 + g23) * dt / 6; //then to be repesated in for loop for all steps
                z53 = z13 + (g30 + 2 * g31 + 2 * g32 + g33) * dt / 6;

                af = (z51) * k1 + x0;
                bf = (z52) * k2 + y0;


                //DRAWING LINE JUST CALCULATED

                mut.WaitOne();
                System.Drawing.Graphics graphicsObj = Graphics.FromImage(offScrBuff);

                graphicsObj.DrawLine(Pens.Red, (int)xi, (int)yi, (int)xf, (int)yf);

                graphicsObj.DrawLine(Pens.RoyalBlue, (int)ai, (int)bi, (int)af, (int)bf);

                graphicsObj.Dispose();
                mut.ReleaseMutex();

                pictureBox1.Invalidate();


                //REDEFINING COORDS AND VALUES FOR NEXT LOOP
                //first particle
                xi = (y51) * k1 + x0;
                yi = (y52) * k2 + y0;
                y11 = y51;
                y12 = y52;
                y13 = y53;

                //second particle
                ai = (z51) * k1 + x0;
                bi = (z52) * k2 + y0;
                z11 = z51;
                z12 = z52;
                z13 = z53;
                /*even at 1 the below makes the program far too slow, need an alternative
                 intention was for it to allow user to see the particle trajectories better*/
                //System.Threading.Thread.Sleep(1);
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            sigma= double.Parse(textBox2.Text);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            beta= double.Parse(textBox3.Text);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            p = int.Parse( textBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
               
            }
            Task.Factory.StartNew(() => { DrawLorenzChaos(sigma, beta, p); });
            
        }

        
    }
}
