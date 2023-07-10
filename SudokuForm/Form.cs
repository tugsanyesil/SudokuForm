using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SudokuForm
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
        }

        readonly int Gap = 10;
        Point SudokuLocation;
        Sudoku Sudoku = null;
        private void Form_Load(object sender, EventArgs e)
        {
            SudokuLocation = new Point(Gap, Gap);
            GenerateButton_Click(null, EventArgs.Empty);
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            if (Sudoku != null) { Controls.Remove(Sudoku); }

            Sudoku = new Sudoku { Location = SudokuLocation };
            LogLabel.Text = Sudoku.Log;
            GenerateButton.Location = new Point(Sudoku.Right + Gap, Sudoku.Top);
            CopyMapButton.Location = new Point(GenerateButton.Left, GenerateButton.Bottom + Gap);
            LogLabel.Location = new Point(GenerateButton.Left, CopyMapButton.Bottom + Gap);
            ClientSize = new Size(GenerateButton.Left + Math.Max(TextRenderer.MeasureText(LogLabel.Text, LogLabel.Font).Width, GenerateButton.Width) + Gap, Sudoku.Bottom + Gap);
            Controls.Add(Sudoku);
        }

        private void CopyMapButton_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText(Sudoku.GetMapAsText());
        }
    }

    public class Sudoku : Control
    {
        private static readonly int MiniLength = 3;
        private static readonly int Length = 9;
        private static readonly int LabelSize = 30;

        private int[,] Map;
        private Label[,] Labels;
        public string Log;

        public Sudoku()
        {
            Map = new int[Length, Length];
            List<int>[,] Cube = new List<int>[Length, Length];
            List<Point> PointList = new List<Point>();
            Labels = new Label[Length, Length];

            var random = new Random();
            var NumberSet = Enumerable.Range(1, Length);

            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    Cube[i, j] = NumberSet.ToList();
                    PointList.Add(new Point(i, j));
                }
            }

            while (PointList.Count != 0)
            {
                PointList = PointList.OrderBy(point => Cube[point.X, point.Y].Count).ToList();
                int i = PointList[0].X;
                int j = PointList[0].Y;

                int choise = 0;
                if (Cube[i, j].Count != 0)
                {
                    choise = Cube[i, j][random.Next(Cube[i, j].Count)];
                }
                Cube[i, j].Remove(choise);

                for (int k = 0; k < Length; k++)
                {
                    if (k != i) { Cube[k, j].Remove(choise); } // delete row
                    if (k != j) { Cube[i, k].Remove(choise); } // delete col
                }

                int mi = 0;
                int mj = 0;
                int ri = i / MiniLength;
                int rj = j / MiniLength;
                for (int ii = 0; ii < MiniLength; ii++) // delete region 3x3
                {
                    for (int jj = 0; jj < MiniLength; jj++)
                    {
                        mi = ri * MiniLength + ii;
                        mj = rj * MiniLength + jj;
                        if (mi != i && mj != j)
                        {
                            Cube[mi, mj].Remove(choise);
                        }
                    }
                }
                Map[i, j] = choise;

                Labels[i, j] = new Label() { Size = new Size(LabelSize, LabelSize), BorderStyle = BorderStyle.FixedSingle, TextAlign = ContentAlignment.MiddleCenter, Location = new Point(LabelSize * i, LabelSize * j), Text = choise.ToString() };
                Size = new Size(LabelSize * Length, LabelSize * Length);
                Controls.Add(Labels[i, j]);

                PointList.RemoveAt(0);
            }

            BackColor = Color.White;
            Log = "Error\n";
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    if (Cube[i, j].Count > 0)
                    {
                        var str = "";
                        Cube[i, j].ForEach(c => str += c + " ");
                        Log += "(" + i + ", " + j + ")   Count : " + Cube[i, j].Count + "   Numbers : " + str + "\n";
                        Labels[i, j].BackColor = Color.Red;
                    }
                    else if (Map[i, j] == 0)
                    {
                        Labels[i, j].BackColor = Color.Green;
                    }
                }
            }
            if (Log == "Error\n")
            {
                Log = "No Error";
            }
        }

        public string GetMapAsText()
        {
            string text = "";
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    text += Map[i, j].ToString() + "\t";
                }
                if (i != Length - 1) { text += "\n"; }
            }
            return text;
        }
    }
}
