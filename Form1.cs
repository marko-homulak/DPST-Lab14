using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Lab14
{
    public partial class Form1 : Form
    {
        private FloydWarshall floyd;
        private Visualization viz;

        const int INF = int.MaxValue / 2;

        int[,] adjacencyMatrix;
        List<int> path;

        List<Vertex> vertices = new List<Vertex>();

        public Form1()
        {
            InitializeComponent();

            // Початкова матриця суміжності
            adjacencyMatrix = new int[,]{
                { 0, 6, 10, INF, INF, INF, INF, INF, INF, INF },
                { 6, 0, 12, 11, 14, INF, INF, INF, INF, INF },
                { 10, 12, 0, 12, INF, INF, 8, 16, INF, INF },
                { INF, 11, 12, 0, INF, 6, 3, INF, INF, INF },
                { INF, 14, INF, INF, 0, 4, INF, INF, 6, INF },
                { INF, INF, INF, 6, 4, 0, INF, INF, 12, INF },
                { INF, INF, 8, 3, INF, INF, 0, INF, 16, 6 },
                { INF, INF, 16, INF, INF, INF, INF, 0, INF, 8 },
                { INF, INF, INF, INF, 6, 12, 16, INF, 0, 13 },
                { INF, INF, INF, INF, INF, INF, 6, 8, 13, 0 }
            };

            floyd = new FloydWarshall(adjacencyMatrix);
            path = new List<int>();
            viz = new Visualization(vertices, pictureBoxGraph.Width, pictureBoxGraph.Height, this.Font);

            LoadCoordinatesFromXml("coordinates.xml");

            labelXY.Visible = false;

            pictureBoxGraph.Image = viz.DrawVertices();
        }

        private void btnAlghoritm_Click(object sender, EventArgs e)
        {
            int start, end;

            if (!int.TryParse(numUpDownStart.Text, out start) ||
                !int.TryParse(numUpDownEnd.Text, out end))
            {
                MessageBox.Show("Please enter valid numeric values.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (floyd.GetDistance(start, end) >= INF || floyd.GetDistance(start, end) == 0)
            {
                labelResult.Text = $"There is no path between {start} and {end}.";
                labelPath.Visible = false;
                pictureBoxGraph.Image = viz.DrawVertices();
            }
            else
            {
                labelResult.Text = $"S({start}->{end}) = {floyd.GetDistance(start, end)};";
                path = floyd.ReconstructPath(start, end);
                labelPath.Visible = true;
                labelPath.Text = "S: " + string.Join("->", path);

                pictureBoxGraph.Image = viz.DrawVertices();
                pictureBoxGraph.Image = viz.DrawPath(path);
            }
        }

        private void pictureBoxGraph_Click(object sender, EventArgs e)
        {
            //getCoordinate(sender, e);
        }

        private void getCoordinate(object sender, EventArgs e)
        {
            labelXY.Visible = true;
            MouseEventArgs me = (MouseEventArgs)e;
            Point coordinates = me.Location;
            labelXY.Text = $"X: {coordinates.X}, Y: {coordinates.Y}";
        }

        private void LoadCoordinatesFromXml(string path)
        {
            var xml = System.Xml.Linq.XDocument.Load(path);
            foreach (var v in xml.Descendants("vertex"))
            {
                string name = v.Attribute("name").Value;
                int x = int.Parse(v.Attribute("x").Value);
                int y = int.Parse(v.Attribute("y").Value);

                vertices.Add(new Vertex
                {
                    Name = name,
                    Position = new Point(x, y)
                });
            }
        }

    }
}
