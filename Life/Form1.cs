using System;
using System.Windows;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Prism.Services.Dialogs;

namespace Life
{

    public partial class Form1 : Form
    {
        private int currentGeneration = 0;
        private Graphics graphics;
        private int resolutoin;
        private bool[,] field;
        private int cols;
        private int rows;
        private bool[,] newField;

        


        public Form1()
        {
            InitializeComponent();
        }

        private void StartGame()
        {
            if (timer1.Enabled)
                return;

            currentGeneration = 0;
            Text = $"Поколение: {currentGeneration}";


            nudResolution.Enabled = false;
            nudDensity.Enabled = false;
            resolutoin = (int)nudResolution.Value;
            cols = pictureBox1.Width / resolutoin;
            rows = pictureBox1.Height / resolutoin;
            field = new bool[cols, rows];

            if (handPlacement.Checked == true)
            {
                
                pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                graphics = Graphics.FromImage(pictureBox1.Image);
                graphics.Clear(Color.Black);

            }
            else 
            {
                Random random = new Random();
                for (int x = 0; x < cols; x++)
                {
                    for (int y = 0; y < rows; y++)
                    {
                        field[x, y] = random.Next((int)nudDensity.Value) == 0;
                    }
                }

                pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                graphics = Graphics.FromImage(pictureBox1.Image);
                timer1.Start();
            }
            
            
            
        }

        private void NextGeneration()
        {
  
            newField = new bool[cols, rows];

            graphics.Clear(Color.Black);

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    var neighboursCount = CountNeighbours(x, y);

                    if (mapLimit.Checked == true)
                    {
                        neighboursCount = CountNeighboursMapLimit(x, y);
                    }
                    else 
                    {
                        neighboursCount = CountNeighbours(x, y);
                    }

                    var hasLife = field[x, y];

                    if (!hasLife && neighboursCount == 3)
                        newField[x, y] = true;
                    else if (hasLife && (neighboursCount < 2 || neighboursCount > 3))
                        newField[x, y] = false;
                    else
                        newField[x, y] = field[x, y];

                    if (hasLife)
                        graphics.FillRectangle(Brushes.MediumAquamarine, x * resolutoin, y * resolutoin, resolutoin -1, resolutoin -1);

                }
 
            }

            bool notHasLifeField = field[0, 0];
            for (int x = 0; x < field.GetLength(0); x++)
            {
                for (int y = 0; y < field.GetLength(1); y++)
                {
                    notHasLifeField = notHasLifeField || field[x, y];
                }
            }

            bool isEqual = true;
            for (int i = 0; i < newField.GetLength(0); i++)
            {
                for (int j = 0; j < newField.GetLength(1); j++)
                {
                    if (newField[i, j] != field[i, j])
                    {
                        isEqual = false;
                        break;
                    }
                }
            }

            if (isEqual == true && notHasLifeField != false)
            {
                
                pictureBox1.Refresh();
                StopGame();
                Text = $"Поколение: {currentGeneration} имеет устойчивую форму. Игра окончена!";
                System.Windows.MessageBox.Show($"Поколение: {currentGeneration} имеет устойчивую форму!", 
                    "Игра окончена!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else 
            {
                field = newField;
                pictureBox1.Refresh();
                Text = $"Поколение: {++currentGeneration}";
            }
            

            if (notHasLifeField == false)
            {
                Text = $"Поколение: {++currentGeneration} было последним. Игра окончена!";
                pictureBox1.Refresh();
                StopGame();
                System.Windows.MessageBox.Show($"Поколение: {currentGeneration} было последним!", "Игра окончена!",
         MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }




        private int CountNeighbours(int x, int y)
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    var col = (x + i + cols) % cols;
                    var row = (y + j + rows) % rows;

                    var isSelfChecking = col == x && row == y;
                    var hasLife = field[col, row];

                    if (hasLife && !isSelfChecking)
                        count++;

                }
            }

            return count;
        }

        private int CountNeighboursMapLimit(int x, int y)
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                
                    if (x == 0 && y >= 0 && i < 0 || x >= ((x + i + cols) % cols) && i > 0)
                        continue;
  
                   
                    if (y == 0 && x >= 0 && j < 0 || y >= ((y + j + rows) % rows) && j > 0)
                        continue;

                    var col = (x + i + cols) % cols;

                    var row = (y + j + rows) % rows;

                    var isSelfChecking = col == x && row == y;
                    var hasLife = field[col, row];

                    if (hasLife && !isSelfChecking)
                        count++;

                }
            }

            return count;
        }

        private void StopGame()
        {
            if (!timer1.Enabled)
                return;
            timer1.Stop();
            nudResolution.Enabled = true;
            nudDensity.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            StartGame();
            
        }

        private void bStop_Click(object sender, EventArgs e)
        {
            StopGame();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

            if (handPlacement.Checked == true)
            {
                
            }
            else
            {
                if (!timer1.Enabled)
                    return;
            }

            if (e.Button == MouseButtons.Left)
            {
                var x = e.Location.X / resolutoin;
                var y = e.Location.Y / resolutoin;
                var validationPassed = ValidateMousePosition(x, y);
                if (validationPassed)
                    field[x, y] = true;
                graphics.FillRectangle(Brushes.MediumAquamarine, x * resolutoin, y * resolutoin, resolutoin - 1, resolutoin - 1);
            }
            if (e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / resolutoin;
                var y = e.Location.Y / resolutoin;
                var validationPassed = ValidateMousePosition(x, y);
                if (validationPassed)
                    field[x, y] = false;
                graphics.FillRectangle(Brushes.Black, x * resolutoin, y * resolutoin, resolutoin - 1, resolutoin - 1);
            }
            if (handPlacement.Checked == true && !timer1.Enabled)
            {
                pictureBox1.Refresh();
            }

        }

        private bool ValidateMousePosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < cols && y < rows;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            resolutoin = (int)nudResolution.Value;
            cols = pictureBox1.Width / resolutoin;
            rows = pictureBox1.Height / resolutoin;
            field = new bool[cols, rows];
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
            graphics.Clear(Color.Black);
            Text = $"Игра Жизнь. Поколение: {currentGeneration}";
            
        }

        private void mapLimit_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void bContinue_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
                return;
            nudResolution.Enabled = false;
            nudDensity.Enabled = false;
            timer1.Start();
        }
    }
}
