using System.Numerics;

namespace TG_Program
{
    public partial class Form1 : Form
    {
        List<Vrchol> vrcholy = new List<Vrchol>();
        List<Hrana> hrany = new List<Hrana>();

        Vrchol vrchol1;
        Vrchol vrchol2;

        bool moving = false;
        bool creatingEdge = false;
        bool djisktra = false;
        bool appLocked = false;


        public Form1()
        {
            InitializeComponent();

        }

        private void OnNewHranaInitiated(Vrchol iniciator)
        {
            if (appLocked)
                return;
            if (!moving)
            {
                creatingEdge = true;
                this.vrchol1 = iniciator;
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            Vrchol vrchol = new Vrchol(AssignID());
            vrcholy.Add(vrchol);
            panel1.Controls.Add(vrchol);
            vrchol.Location = GenerateRandomPosition();
            vrchol.clicked += OnVrcholClicked;
            vrchol.newHranaIniciated += OnNewHranaInitiated;

        }
        public void OnVrcholClicked(Vrchol vrchol)
        {
            if (appLocked)
                return;
            if (creatingEdge)
            {
                if (vrchol != vrchol1)
                {
                    int vaha = 0;
                    if (checkBox1.Checked == true)
                    {
                        Random random = new Random();
                        vaha = random.Next(1, 10);
                    }
                    else
                    {
                        vaha = Prompt.ShowDialog();
                    }

                    Hrana hrana = new Hrana(vrchol1, vrchol, vaha);
                    hrany.Add(hrana);
                    DrawHrany();
                    creatingEdge = false;

                    this.vrchol1 = null;

                }
            }
            else if (djisktra)
            {
                if (vrchol1 == null)
                {
                    vrchol1 = vrchol;

                }
                else if (vrchol2 == null)
                {
                    vrchol2 = vrchol;
                }
                if (vrchol1 != null && vrchol2 != null)
                {
                    RunDjisktra();
                    djisktra = false;
                    vrchol1 = null;
                    vrchol2 = null;
                }
            }
            else
            {

                if (vrchol != vrchol2)
                {
                    vrchol2 = vrchol;
                    moving = true;
                    MoveVrchol();
                }
                else
                {
                    moving = false;
                    vrchol2 = null;
                    panel1.MouseMove -= Panel1_MouseMove;
                    panel1.MouseUp -= Panel1_MouseUp;
                    DrawHrany();
                }
            }
        }
        private void MoveVrchol()
        {

            panel1.MouseMove += Panel1_MouseMove;
            panel1.MouseUp += Panel1_MouseUp;
        }

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!moving || vrchol2 == null)
                return;

            vrchol2.Location = GetRelativePositionOfCursorToPanel();
        }

        private void Panel1_MouseUp(object sender, MouseEventArgs e)
        {

            moving = false;
            vrchol2 = null;
            panel1.MouseMove -= Panel1_MouseMove;
            panel1.MouseUp -= Panel1_MouseUp;
        }

        private int AssignID()
        {
            int id = 0;
            for (int i = 0; i < vrcholy.Count; i++)
            {
                if (vrcholy[i].id == id)
                {
                    id++;
                    i = 0;
                }
            }
            return id;

        }
        private Point GenerateRandomPosition()
        {
            Random random = new Random();
            Point position = new Point();
            position.X = random.Next(200, panel1.Width - 200);
            position.Y = random.Next(200, panel1.Height - 200);
            return position;
        }
        private Point GetRelativePositionOfCursorToPanel()
        {
            Point position = panel1.PointToClient(Cursor.Position);
            position.X -= vrchol2.Width / 2;
            position.Y -= vrchol2.Height / 2;
            return position;
        }

        void DrawHrany()
        {
            Pen pen = new Pen(Color.Black, 2);
            Graphics g = panel1.CreateGraphics();
            EraseEdges(g);
            foreach (Hrana hrana in hrany)
            {
                g.DrawLine(pen, hrana.vrchol1.Location.X + hrana.vrchol1.Width / 2, hrana.vrchol1.Location.Y + hrana.vrchol1.Height / 2, hrana.vrchol2.Location.X + hrana.vrchol2.Width / 2, hrana.vrchol2.Location.Y + hrana.vrchol2.Height / 2);
                Point middleOfLine = new Point((hrana.vrchol1.Location.X + hrana.vrchol2.Location.X) / 2, (hrana.vrchol1.Location.Y + hrana.vrchol2.Location.Y) / 2);
                Label label = new Label();
                label.Text = hrana.vaha.ToString();
                label.Location = middleOfLine;
                label.Size = new Size(30, 20);
                panel1.Controls.Add(label);
            }
        }
        void EraseEdges(Graphics g)
        {
            SolidBrush brush = new SolidBrush(panel1.BackColor);
            g.FillRectangle(brush, 0, 0, panel1.Width, panel1.Height);

            List<Control> controlsToRemove = new List<Control>();
            foreach (Control control in panel1.Controls)
            {
                if (control is Label)
                {
                    controlsToRemove.Add(control);
                }
            }

            foreach (Control control in controlsToRemove)
            {
                panel1.Controls.Remove(control);
                control.Dispose(); // uvolní i zdroje labelu
            }
        }

        class Hrana
        {
            public Vrchol vrchol1;
            public Vrchol vrchol2;
            public int vaha;

            public Hrana(Vrchol vrchol1, Vrchol vrchol2, int vaha)
            {
                this.vrchol1 = vrchol1;
                this.vrchol2 = vrchol2;
                this.vaha = vaha;
            }
        }

        public static class Prompt
        {
            public static int ShowDialog()
            {
                Form prompt = new Form();
                prompt.Width = 300;
                prompt.Height = 200;
                Label textLabel = new Label() { Left = 50, Top = 20, Text = "Zadejte váhu hrany:" };
                NumericUpDown inputBox = new NumericUpDown() { Left = 50, Top = 50, Width = 200 };
                Button confirm = new Button() { Text = "OK", Left = 50, Width = 100, Top = 100 };
                confirm.Click += (sender, e) => { prompt.Close(); };

                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(inputBox);
                prompt.Controls.Add(confirm);

                prompt.ShowDialog();
                return (int)inputBox.Value;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            djisktra = true;
        }
        void RunDjisktra()
        {
            if (appLocked)
                return;
            appLocked = true;
            if (hrany.Any(h => h.vaha < 0))
            {
                MessageBox.Show("Dijkstra nefunguje se zápornými vahami.");
                return;
            }

            Dictionary<Vrchol, int> vzdalenost = new Dictionary<Vrchol, int>();
            Dictionary<Vrchol, Vrchol> predchozi = new Dictionary<Vrchol, Vrchol>();
            HashSet<Vrchol> nezpracovane = new HashSet<Vrchol>(vrcholy);

            foreach (var v in vrcholy)
            {
                vzdalenost[v] = int.MaxValue;
                predchozi[v] = null;
                v.BackColor = Color.LightGray; // nezpracovaný
            }

            vzdalenost[vrchol1] = 0;

            while (nezpracovane.Count > 0)
            {
                Vrchol aktualni = null;
                int minVzdalenost = int.MaxValue;

                foreach (var v in nezpracovane)
                {
                    if (vzdalenost[v] < minVzdalenost)
                    {
                        minVzdalenost = vzdalenost[v];
                        aktualni = v;
                    }
                }

                if (aktualni == null || minVzdalenost == int.MaxValue)
                    break;

                aktualni.BackColor = Color.Gold; // právě vybraný
                Cekej(500);

                nezpracovane.Remove(aktualni);

                if (aktualni == vrchol2)
                {
                    aktualni.BackColor = Color.LimeGreen;
                    break;
                }

                foreach (var hrana in hrany)
                {
                    Vrchol soused = null;

                    if (hrana.vrchol1 == aktualni)
                        soused = hrana.vrchol2;
                    else if (hrana.vrchol2 == aktualni)
                        soused = hrana.vrchol1;

                    if (soused == null)
                        continue;

                    if (!nezpracovane.Contains(soused))
                        continue;

                    Color puvodniBarva = soused.BackColor;
                    soused.BackColor = Color.LightSkyBlue;
                    Cekej(200);

                    long novaVzdalenostLong = (long)vzdalenost[aktualni] + hrana.vaha;
                    if (novaVzdalenostLong < vzdalenost[soused])
                    {
                        vzdalenost[soused] = (int)novaVzdalenostLong;
                        predchozi[soused] = aktualni;
                    }

                    soused.BackColor = puvodniBarva;
                }

                aktualni.BackColor = Color.SeaGreen; // zpracováno
                Cekej(300);
            }

            if (vzdalenost[vrchol2] == int.MaxValue)
            {
                MessageBox.Show("Cesta neexistuje.");
                return;
            }

            // Sestavení cesty
            List<Vrchol> cesta = new List<Vrchol>();
            Vrchol krok = vrchol2;

            while (krok != null)
            {
                cesta.Add(krok);
                krok = predchozi[krok];
            }

            cesta.Reverse();

            foreach (var v in cesta)
                v.BackColor = Color.Red;

            string textCesty = string.Join(" -> ", cesta.Select(v => "V" + v.id));
            MessageBox.Show($"Nejkratší cesta: {textCesty}\nDélka: {vzdalenost[vrchol2]}");
            appLocked = false;
        }


        void Cekej(int ms)
        {
            DateTime konec = DateTime.Now.AddMilliseconds(ms);
            while (DateTime.Now < konec)
            {
                Application.DoEvents();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (appLocked)
                return;

            foreach(var vrchol in vrcholy)
            {
                vrchol.BackColor = Color.SkyBlue;
            }
        }
    }
}
