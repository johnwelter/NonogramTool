using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Markup;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public List<Button> butts = new List<Button>();

        public int glyphSize = 5;
        public String[] savDlg = { "「{0}」　ヲ　ホゾン　シタ。"
                                  };
        
        public MainWindow()
        {
            
            InitializeComponent();
            System.Console.WriteLine("populating grid...");
            populateGrid();
            System.Console.WriteLine("populatied!");
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ResizeClick(object sender, RoutedEventArgs e)
        {
            updateGridSize();
        }

        void updateGridSize()
        {

            clearGrid();
            glyphSize = Math.Min(Convert.ToInt32(glyphSizeBox.Text) * 5, 20);
            populateGrid();



        }

        void populateGrid()
        {
            System.Console.WriteLine("set up grid");
            for (int i = 0; i < glyphSize; i++)
            {

                RowDefinition r = new RowDefinition();
                ColumnDefinition c = new ColumnDefinition();


                GlyphGrid.RowDefinitions.Add(r);
                GlyphGrid.ColumnDefinitions.Add(c);

            }
            System.Console.WriteLine("grid set!");

            System.Console.WriteLine("making buttons...");
            for (int i = 0; i < glyphSize; i++)
            {
                for (int j = 0; j < glyphSize; j++)
                {

                    Button b = new Button();
                    Grid.SetColumn(b, j);
                    Grid.SetRow(b, i);

                    //if (j > 0 && j < glyphSize - 1 && i > 0 && i < glyphSize - 1)
                    {
                        b.Background = System.Windows.Media.Brushes.Black;
                        b.Click += new RoutedEventHandler(GridClick);
                        butts.Add(b);

                    }
                    //else
                    //{
                    //    b.IsEnabled = false;
                    //    b.Style = Resources.MergedDictionaries[0]["RimButton"] as Style;


                    //}
                    
                    GlyphGrid.Children.Add(b);
                    System.Console.WriteLine("added button at {0}, {1}", i, j);

                }

            }



        }

        void clearGrid()
        {
            GlyphGrid.Children.Clear();
            GlyphGrid.RowDefinitions.Clear();
            GlyphGrid.ColumnDefinitions.Clear();
            butts.Clear();
        }
        private void GridClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Media.Brush bg = ((Button)sender).Background;
            BrushConverter bgc = new BrushConverter();
            String test = bgc.ConvertToString(bg);
            test = test.Remove(0, 1);
            UInt32 currentVal = (UInt32)Convert.ToInt32(test, 16);

            UInt32 newVal = currentVal ^ 0x00FFFFFF;
            
            ((Button)sender).Background = (System.Windows.Media.Brush)bgc.ConvertFromString("#"+newVal.ToString("X8"));
            

        }

        private void FileValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9a-zA-Z]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            //maybe use bg worker here instead
            //saveDialog.Text = "ホゾン　チュウ．．．";
            //createImage();

            //not making an image anymore, making a picross map instead
            //we'll make the col/row strings first
            
            createMapData();
        }

        void createImage()
        {

            int[,] colors = new int[glyphSize,glyphSize];

            for(int i = 0; i < glyphSize; i++)
            {

                for(int j = 0; j < glyphSize; j++)
                {

                    colors[i, j] = 0;

                    //System.Console.Write(colors[i, j]);
                }
                //System.Console.WriteLine();

            }

            foreach(Button B in butts)
            {
         
                System.Windows.Media.Brush bg = B.Background;
                BrushConverter bgc = new BrushConverter();
                String test = bgc.ConvertToString(bg);
                test = test.Remove(0, 1);
                UInt32 currentVal = (UInt32)Convert.ToInt32(test, 16);

                int idx = Grid.GetColumn(B);
                int idy = Grid.GetRow(B);

                if ((currentVal & 0x00FFFFFF) > 0)
                {
                    colors[idy, idx] = 1;
                }
               
            }
            //System.Console.WriteLine();


            for(int i = 0; i < glyphSize; i++)
            {
                for(int j = 0; j < glyphSize; j++)
                {
                    //System.Console.Write(colors[i, j]);
                }
                //System.Console.WriteLine();
            }


            Bitmap image = new Bitmap(1080, 1080);

            for(int k = 0; k < 1080; k++)
            {
                for(int l = 0; l < 1080; l++)
                {
                    int fx = (((glyphSize) * l) / 1080);
                    int fy = (((glyphSize) * k) / 1080);
                    System.Drawing.Color pixCol = System.Drawing.Color.FromArgb(colors[fy, fx] * 255, colors[fy, fx] * 255, colors[fy, fx] * 255);
                    image.SetPixel(l, k, pixCol);
                }
            }


            String fileName = nameBox.Text + ".png";
            String path = fileName;
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            Random rnd = new Random();
            int r = rnd.Next(0, savDlg.Count());
            saveDialog.Text = String.Format(savDlg[r], fileName);
            image.Dispose();



        }


        void createMapData()
        {
            //firstm make a helper map

            int[,] map = createGrid();

            int mapSize = glyphSize;
            String header = ".db";
            String rowString = ".db";
            String colString = ".db";

            int bytesPerRow = (int)Math.Ceiling(glyphSize / 8.0f);
            int totalMapBytes = bytesPerRow * glyphSize;
            byte[] mapBytes = new byte[totalMapBytes];

            int correctMarks = 0;
            int mapByteIndex = 0;
            int mapBitIndex = 0;

            for (int i = 0; i < mapSize; i++)
            {
                List<int> rowNumbers = new List<int>();
                List<int> colNumbers = new List<int>();
                int rowVal;
                int rowRunVal = 0;
                int colVal;
                int colRunVal = 0;

                for (int j = 0; j < mapSize; j++)
                {
                    //doing rows first
                    rowVal = map[i, j];
                   
                    if (rowVal == 1)
                    {
                        correctMarks++;
                        int byteIdx = i * bytesPerRow + (j / 8);
                        byte[] bitMasks = BitConverter.GetBytes(0b10000000 / (int)Math.Pow(2, j % 8));
                        byte bitMask = bitMasks[0];
                        mapBytes[byteIdx] = (byte)(mapBytes[byteIdx] | bitMask);

                        if (rowRunVal == -1)
                        {
                            rowRunVal = 1;
                        }
                        else
                        {
                            rowRunVal++;
                        }
                    }
                    else if (rowVal == 0 && rowRunVal >= 1)
                    {
                        rowNumbers.Add(rowRunVal);
                        rowRunVal = -1;
                    }




                    colVal = map[j, i];

                    if (colVal == 1)
                    {
                        if (colRunVal == -1)
                        {
                            colRunVal = 1;
                        }
                        else
                        {
                            colRunVal++;
                        }
                    }
                    else if (colVal == 0 && colRunVal >= 1)
                    {
                        colNumbers.Add(colRunVal);
                        colRunVal = -1;
                    }

                }

                if(colRunVal > 0)
                {
                    colNumbers.Add(colRunVal);
                }
                if(rowRunVal > 0)
                {
                    rowNumbers.Add(rowRunVal);
                }

                for (int k = rowNumbers.Count() - 1; k >= 0; k--)
                {
                    //convert number to hex val, somehow- check the text tool 

                    rowString += " $" + rowNumbers[k].ToString("X2") + ",";
                }
                for (int k = colNumbers.Count() - 1; k >= 0; k--)
                {
                    //convert number to hex val, somehow- check the text tool 

                    colString += " $" + colNumbers[k].ToString("X2") + ",";
                }

                if (rowRunVal == 0)
                {
                    //we wouldn't have added anything if runVal was 0, so we need to add a zero instead
                    rowString += " $00, $FF";
                }
                else
                {
                    rowString += " $FF";
                }

                if (colRunVal == 0)
                {
                    //we wouldn't have added anything if runVal was 0, so we need to add a zero instead
                    colString += " $00, $FF";
                }
                else
                {
                    colString += " $FF";
                }

                if(i < mapSize-1)
                {
                    rowString += ",";
                    colString += ",";
                }
            }
            int headerSize = (glyphSize / 5) - 1;
            int correctMarksLo = correctMarks & 0x000000FF;
            int correctMarksHi = correctMarks & 0x0000FF00;
            header += String.Format(" ${0}, ${1}, ${2}", headerSize.ToString("X2"), correctMarksLo.ToString("X2"), correctMarksHi.ToString("X2"));

            String Map = ".db";
            for(int i = 0; i < mapBytes.Length; i++)
            {
                Map += " $" + ((int)mapBytes[i]).ToString("X2");
                if(i < mapBytes.Length - 1)
                {
                    Map += ",";
                }
            }
            String outputText = nameBox.Text + ":\n  " + header + "\n  " + Map + "\n  " + colString + "\n  " + rowString; 
            textBlock.Text = outputText;
            //System.Console.WriteLine(";; header, stores the map size index and the correct marker count in little endian");
            //System.Console.WriteLine(header);
            //System.Console.WriteLine(";; binary map data");
            //System.Console.WriteLine(Map);
            //System.Console.WriteLine(";; column numbers are written backwards, since we want to write them from the bottom up");
            //System.Console.WriteLine(colString);
            //System.Console.WriteLine(";; row numbers are written backwards, since we want to write them right to left");
            //System.Console.WriteLine(rowString);
        }

        int[,] createGrid()
        {

            int[,] map = new int[glyphSize, glyphSize];

            for (int i = 0; i < glyphSize; i++)
            {

                for (int j = 0; j < glyphSize; j++)
                {

                    map[i, j] = 1;

                    //System.Console.Write(colors[i, j]);
                }
                //System.Console.WriteLine();

            }

            foreach (Button B in butts)
            {

                System.Windows.Media.Brush bg = B.Background;
                BrushConverter bgc = new BrushConverter();
                String test = bgc.ConvertToString(bg);
                test = test.Remove(0, 1);
                UInt32 currentVal = (UInt32)Convert.ToInt32(test, 16);

                int idx = Grid.GetColumn(B);
                int idy = Grid.GetRow(B);

                if ((currentVal & 0x00FFFFFF) > 0)
                {
                    map[idy, idx] = 0;
                }

            }

            return map;

        }
        private void glyphSizeBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void nameBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
