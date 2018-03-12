using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;


#pragma warning disable 0169
#pragma warning disable 0168
#pragma warning disable 0414
#pragma warning disable 0649
namespace HexReaction
{
    public partial class Form1 : Form
    {
        // privates
        private static bool keyPressed;
        //private SmoothingMode smoothingMode;
        //private TextRenderingHint textSmoothing;
        private int milBreak = 10, clicks = 8, totalBonus=0, streak =0;
        private MainLogic mainLogic;
        private List<Link> backgroundLinks;
        private bool draw = false, restart = false;
        private int restartFlag = 0;
        private int gridSize = 10;
        private int bonusMultiplier = 6, breaker=1;
        private double difficulty = 0.15;
        private int help = 2;
        

        // Init stage        
        public Form1()
        {
            mainLogic = new MainLogic(0, gridSize, difficulty);
            InitializeComponent();
            //SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.Size = new Size(600, 410);
            InitializeBackgroundLinks();
            

        }
        private void InitializeBackgroundLinks()
        {
            backgroundLinks = new List<Link>();
            foreach (Link link in mainLogic.linkList)
            {
                backgroundLinks.Add(link);
            }
        }

        // Event listeners below 
        private void x25ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridSize = 25;
            Restart();
            int tmpMultiX = (gridSize - 10) * 50;
            int tmpMultiY = (gridSize - 10) * 30;
            this.Size = new Size(600+ tmpMultiX, 410+ tmpMultiY);
            this.x10ToolStripMenuItem.Checked = false;
            this.x15ToolStripMenuItem.Checked = false;
            this.x20ToolStripMenuItem.Checked = false;
            this.x25ToolStripMenuItem.Checked = true;
        }
        private void x20ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridSize = 20;
            Restart();
            int tmpMultiX = (gridSize - 10) * 50;
            int tmpMultiY = (gridSize - 10) * 30;
            this.Size = new Size(600 + tmpMultiX, 410 + tmpMultiY);
            this.x10ToolStripMenuItem.Checked = false;
            this.x15ToolStripMenuItem.Checked = false;
            this.x20ToolStripMenuItem.Checked = true;
            this.x25ToolStripMenuItem.Checked = false;
        }
        private void x15ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridSize = 15;
            Restart();
            int tmpMultiX = (gridSize - 10) * 50;
            int tmpMultiY = (gridSize - 10) * 30;
            this.Size = new Size(600 + tmpMultiX, 410 + tmpMultiY);
            this.x10ToolStripMenuItem.Checked = false;
            this.x15ToolStripMenuItem.Checked = true;
            this.x20ToolStripMenuItem.Checked = false;
            this.x25ToolStripMenuItem.Checked = false;
        }
        private void x10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridSize = 10;
            Restart();
            int tmpMultiX = (gridSize - 10) * 50;
            int tmpMultiY = (gridSize - 10) * 30;
            this.Size = new Size(600 + tmpMultiX, 410 + tmpMultiY);
            this.x10ToolStripMenuItem.Checked = true;
            this.x15ToolStripMenuItem.Checked = false;
            this.x20ToolStripMenuItem.Checked = false;
            this.x25ToolStripMenuItem.Checked = false;
        }
        private void easyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            difficulty = 0.2;
            Restart();
            this.easyToolStripMenuItem.Checked = true;
            this.mediumToolStripMenuItem.Checked = false;
            this.hardToolStripMenuItem.Checked = false;
        }
        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            difficulty = 0.15;
            Restart();
            this.easyToolStripMenuItem.Checked = false;
            this.mediumToolStripMenuItem.Checked = true;
            this.hardToolStripMenuItem.Checked = false;
        }
        private void hardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            difficulty = 0.1;
            Restart();
            this.easyToolStripMenuItem.Checked = false;
            this.mediumToolStripMenuItem.Checked = false;
            this.hardToolStripMenuItem.Checked = true;
        }
        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Restart();
        }
        private void mouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && clicks > 0 && !mainLogic.busy)
            {
                try
                {
                //this.Invoke(new Action(() =>
                //{
                    Point nd = mainLogic.Closest(e.Location);
                    int combo = 0;
                    if (!nd.IsEmpty)
                    {
                        clicks--;
                        //combo = mainLogic.DamageQueuedNodes(mainLogic.nodesList[nd].nodeID);
                        mainLogic.damageNodes.Add(mainLogic.nodesList[nd].nodeID);
                        mainLogic.busy = true;
                        clicks += (int)(combo / bonusMultiplier);
                    }
                //}));
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                }
            }
            if (e.Button == MouseButtons.Right && help > 0)
            {
                mainLogic.RandomizeBangs();
                help--;
            }
        }
        
        // Restarts logic
        private void Restart()
        {
            mainLogic = new MainLogic(0, gridSize, difficulty);
            this.Invalidate();
            clicks = 4 * (gridSize / 5);
            InitializeBackgroundLinks();
            help = 3;
        }

        
        private void Restart(int floorIncrement, int passClicks)
        {
            int tmpScore = mainLogic.score;
            int currentFloor = mainLogic.floor+floorIncrement;
            mainLogic = new MainLogic(currentFloor, gridSize, difficulty);
            mainLogic.score = tmpScore;
            this.Invalidate();
            clicks = 4 * (gridSize / 5) + passClicks;
            help++;
        }

        // Main OnPaint
        protected override void OnPaint(PaintEventArgs e)
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            Bitmap buffer;
            buffer = new Bitmap(this.Width, this.Height);
            try
            {
                if (true)
                {

                    using (Graphics gg = Graphics.FromImage(buffer))
                    {
                        gg.SmoothingMode = SmoothingMode.HighQuality;
                        gg.TextRenderingHint = TextRenderingHint.AntiAlias;
                        //gg.ScaleTransform(0.2f, 0.2f);
                        int xc = 15;
                        int yc = 15;
                        int lev = mainLogic.floor;
                        
                        // Draw permament backgroundLinks matrix
                        foreach (Link link in backgroundLinks)
                        {
                            gg.DrawLine(new Pen(Color.FromArgb(30, 190, 190, 190), 2), link.pointA, link.pointB);
                        }

                        // Draw links
                        foreach (Link link in mainLogic.linkList)
                        {
                            gg.DrawLine(new Pen(Color.FromArgb(50, 212, 232, 0), 2), link.pointA, link.pointB);
                        }

                        int lev1 = 0, lev2 = 0, lev3 = 0, lev4 = 0, lev5 = 0, lev6 = 0, lev7 = 0, lev8=0;
                        // Draw filled dots
                        int cnt = mainLogic.nodesList.Count;
                        for (int i = 0; i < cnt; i++)
                        {
                            Point tmpPoint = mainLogic.nodesList.ElementAt(i).Key;
                            Node tmpNode = mainLogic.nodesList.ElementAt(i).Value;
                            int es = tmpNode.size;
                            int esr = tmpNode.size / 2;
                            
                            Color tmpColor = Color.FromArgb(200, tmpNode.Colour().R, tmpNode.Colour().G, tmpNode.Colour().B);
                            gg.FillEllipse(new SolidBrush(tmpColor), new RectangleF(new Point(tmpPoint.X - (6 + esr), tmpPoint.Y - (6 + esr)), new Size(13 + es, 13 + es)));
                            //gg.FillEllipse(new SolidBrush(Color.FromArgb(100,255,255,255)), new RectangleF(new Point(tmpPoint.X - (5 + esr), tmpPoint.Y - (5 + esr)), new Size(10 + es, 10 + es)));

                            lev1 = tmpNode.bangs == 1 ? lev1 + 1 : lev1;
                            lev2 = tmpNode.bangs == 2 ? lev2 + 1 : lev2;
                            lev3 = tmpNode.bangs == 3 ? lev3 + 1 : lev3;
                            lev4 = tmpNode.bangs == 4 ? lev4 + 1 : lev4;
                            lev5 = tmpNode.bangs == 5 ? lev5 + 1 : lev5;
                            lev6 = tmpNode.bangs == 6 ? lev6 + 1 : lev6;
                            lev7 = tmpNode.bangs == 7 ? lev7 + 1 : lev7;
                            lev8 = tmpNode.bangs == 8 ? lev8 + 1 : lev8;
                        }
                        // Draw circles
                        for (int i = 0; i < cnt; i++)
                        {
                            Point tmpPoint = mainLogic.nodesList.ElementAt(i).Key;
                            Node tmpNode = mainLogic.nodesList.ElementAt(i).Value;
                            int nodeExtraSize = tmpNode.size;
                            int nodeExtraSizeRadius = tmpNode.size / 2;
                            Color tmpColor = Color.FromArgb(120, tmpNode.Colour().R, tmpNode.Colour().G, tmpNode.Colour().B);
                            gg.DrawEllipse(new Pen(tmpColor), new RectangleF(new Point(tmpPoint.X - (6 + nodeExtraSizeRadius), tmpPoint.Y - (6 + nodeExtraSizeRadius)), new Size(13 + nodeExtraSize, 13 + nodeExtraSize)));
                        }
                        
                        // Draw legend and scores
                        if (lev >= 0)
                        {
                            gg.FillEllipse(new SolidBrush(Color.Red), new RectangleF(new Point(5, 25), new Size(xc, yc)));
                            gg.DrawString("" + lev1, new Font(new FontFamily("Arial"), 10, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.White), 6, 27);

                            gg.FillEllipse(new SolidBrush(Color.Yellow), new RectangleF(new Point(5 + (xc + 2), 25), new Size(xc, yc)));
                            gg.DrawString("" + lev2, new Font(new FontFamily("Arial"), 10, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.Black), 8 + (xc), 27);

                            gg.FillEllipse(new SolidBrush(Color.Green), new RectangleF(new Point(5 + (xc + 2) * 2, 25), new Size(xc, yc)));
                            gg.DrawString("" + lev3, new Font(new FontFamily("Arial"), 10, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.White), 8 + (xc) * 2, 27);

                            gg.FillEllipse(new SolidBrush(Color.Cyan), new RectangleF(new Point(5 + (xc + 2) * 3, 25), new Size(xc, yc)));
                            gg.DrawString("" + lev4, new Font(new FontFamily("Arial"), 10, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.Black), 8 + (xc+1) * 3, 27);
                        }
                        if (lev >= 1)
                        {
                            gg.FillEllipse(new SolidBrush(Color.Blue), new RectangleF(new Point(5 + (xc + 2) * 4, 25), new Size(xc, yc)));
                            gg.DrawString("" + lev5, new Font(new FontFamily("Arial"), 10, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.White), 8 + (xc + 2) * 4, 27);
                        }
                        if (lev >= 2)
                        {
                            gg.FillEllipse(new SolidBrush(Color.Violet), new RectangleF(new Point(5 + (xc + 2) * 5, 25), new Size(xc, yc)));
                            gg.DrawString("" + lev6, new Font(new FontFamily("Arial"), 10, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.Black), 8 + (xc + 2) * 5, 27);
                        }
                        if (lev >= 3)
                        {
                            gg.FillEllipse(new SolidBrush(Color.Pink), new RectangleF(new Point(5 + (xc + 2) * 6, 25), new Size(xc, yc)));
                            gg.DrawString("" + lev7, new Font(new FontFamily("Arial"), 10, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.Black), 8 + (xc + 2) * 6, 27);
                        }
                        if (lev >= 4)
                        {
                            gg.FillEllipse(new SolidBrush(Color.White), new RectangleF(new Point(5 + (xc + 2) * 7, 25), new Size(xc, yc)));
                            gg.DrawString("" + lev8, new Font(new FontFamily("Arial"), 10, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.Black), 8 + (xc + 2) * 7, 27);
                        }
                        gg.DrawString("Nodes: " + mainLogic.nodesList.Count 
                            + "\t Score: " + mainLogic.score 
                            + "\t Clicks: " + clicks 
                            + "\t Streak: " + streak 
                            + "\t Lvl: " + mainLogic.floor
                            + "\t Shuffle: "+help, new Font(new FontFamily("Arial"), 10, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.White), 150, 27);

                        // grays out area once clicks are finished
                        if (clicks <= 0 && !mainLogic.busy)
                        {
                            gg.FillRectangle(new SolidBrush(Color.FromArgb(210, 50, 50, 50)), new RectangleF(new Point(0, 0), new Size(this.Width, this.Height)));
                        }

                    }
                    // will perform restart if no more clicks available
                    // or will play level up if qualifies
                    if ((restart && restartFlag > 0) || mainLogic.floor == 4)
                    {
                        int nodesCount = mainLogic.nodesList.Count;
                        // restart new game from start - Game over
                        if (clicks <= 0 && nodesCount > 0)
                        {
                            string message = "Game over"+"\n"+"Score : " + mainLogic.score;
                            string caption = "Game Over";
                            MessageBoxButtons buttons = MessageBoxButtons.OK;
                            DialogResult result;

                            // Displays the MessageBox.

                            result = MessageBox.Show(message, caption, buttons);
                            if (result == System.Windows.Forms.DialogResult.OK)
                            {
                                Restart();
                                return;
                            }
                        }
                        // Check if killed all nodesList
                        // Play next level
                        if (nodesCount <= 0 && clicks >= 0)
                        {
                            string message = "Score : " + mainLogic.score + "\nLevel up! \nLevel "+(mainLogic.floor+1);
                            string caption = "Congrats!";
                            MessageBoxButtons buttons = MessageBoxButtons.OK;
                            DialogResult result;

                            // Displays the MessageBox.

                            result = MessageBox.Show(message, caption, buttons);
                            if (result == System.Windows.Forms.DialogResult.OK)
                            {
                                if (mainLogic.floor == 4)
                                {
                                    Restart();
                                }
                                else
                                {
                                    Restart(1, clicks);
                                }
                            }
                        }
                        restart = false; restartFlag = 0;
                    }
                }
                // Call for graphics redraw and process damaged nodesList
                if (!this.IsDisposed)
                {
                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            this.BackgroundImage = buffer;
                            // If mainLogic logic computes nodesList, gui will draw status every 75 mils
                            if (mainLogic.busy)
                            {
                                totalBonus = mainLogic.DamageQueuedNodes();
                                streak = totalBonus;
                            } 
                            // Calculate extra clicks
                            else {                                
                                clicks += (int)(totalBonus / bonusMultiplier);
                                totalBonus = 0;
                                // Trigger restart flag (no clicks or no nodesList left)
                                // Wait 100 mils to let GUI finish drawing
                                if (clicks <= 0 || mainLogic.nodesList.Count <= 0)
                                {
                                    restart = true;
                                    restartFlag++;
                                    Thread.Sleep(100);
                                }
                            }

                        }));
                    } catch (Exception exc)
                    {

                    }
                }
            } catch (Exception z)
            {
            }
            if (mainLogic.busy)
            {
                Thread.Sleep(75);
            }
            
        }              
    }
}
