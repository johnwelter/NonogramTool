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
using System.IO;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Runtime.Remoting.Messaging;

namespace WpfApplication2
{


    static class Consts
    {
        public const int PalTiles_Loc = 0x24;
        public const int PalTile_Offset_x = 12;
        public const int PalTile_Offset_y = 12;
    }

    class Palettes : ObservableCollection<string>
    {
        public Palettes()
        {
            Add("Gray");
            Add("Blue");
            Add("Lavender");
            Add("Violet");
            Add("Magenta");
            Add("Pink");
            Add("Red");
            Add("Orange");
            Add("Gold");
            Add("Green");
            Add("Gameboy");
            Add("Teal");
            Add("Skyblue");
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        //public List<Button> butts = new List<Button>();
        public List<Label> butts = new List<Label>();
        public List<Label> NTbutts = new List<Label>();
        //for 5x5 -> ceil(5/2) -> 3 -> 9 tiles
        //for 10x10 -> ceil(10/2) -> 5 -> 25 tiles
        //for 15x15 -> ceil(15/2) -> 8 -> 64 tiles

        public int glyphSize = 1;
        public UInt32 heldCol = 0;
        public UInt32 NTheldCol = 0;
        public String[] savDlg = { "「{0}」　ヲ　ホゾン　シタ。" };

        public bool paintOn = false;
        public bool NTpaintOn = false;

        public MainWindow()
        {

            InitializeComponent();
            comboBox.ItemsSource = new Palettes();
            comboBox.SelectedIndex = 0;
            this.MouseUp += new MouseButtonEventHandler(ClearPaintCtrl);
            System.Console.WriteLine("populating grid...");
            updateGridSize();
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
            clearNTGrid();
            glyphSize = Math.Min(Convert.ToInt32(glyphSizeBox.Text), 4);
            populateGrid(glyphSize * 5);
            populateNTGrid(glyphSize * 5);
        }

        void populateGrid(int size)
        {
            System.Console.WriteLine("set up grid");
            for (int i = 0; i < size; i++)
            {

                RowDefinition r = new RowDefinition();
                ColumnDefinition c = new ColumnDefinition();


                GlyphGrid.RowDefinitions.Add(r);
                GlyphGrid.ColumnDefinitions.Add(c);

            }
            System.Console.WriteLine("grid set!");

            System.Console.WriteLine("making buttons...");
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Label l = new Label();
                    //Button b = new Button();
                    //Grid.SetColumn(b, j);
                    Grid.SetColumn(l, j);
                    //Grid.SetRow(b, i);
                    Grid.SetRow(l, i);

                    //if (j > 0 && j < glyphSize - 1 && i > 0 && i < glyphSize - 1)
                    {
                        l.Background = System.Windows.Media.Brushes.Black;
                        //l.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 128, 128));
                        l.MouseMove += new MouseEventHandler(MouseOverColor);
                        l.MouseDown += new MouseButtonEventHandler(StartPaintCtrl);
                        l.Style = Resources.MergedDictionaries[0]["GridLabel"] as Style;
                        //b.Click += new RoutedEventHandler(GridClick);
                        //b.MouseEnter += new MouseEventHandler(MouseOverColor);
                        butts.Add(l);

                    }
                    //else
                    //{
                    //    b.IsEnabled = false;
                    //    b.Style = Resources.MergedDictionaries[0]["RimButton"] as Style;


                    //}

                    GlyphGrid.Children.Add(l);
                    System.Console.WriteLine("added button at {0}, {1}", i, j);

                }

            }

        }
        void populateNTGrid(int size)
        {
            System.Console.WriteLine("set up grid");
            for (int i = 0; i < size; i++)
            {

                RowDefinition r = new RowDefinition();
                ColumnDefinition c = new ColumnDefinition();


                NameTableGrid.RowDefinitions.Add(r);
                NameTableGrid.ColumnDefinitions.Add(c);

            }
            System.Console.WriteLine("grid set!");

            System.Console.WriteLine("making buttons...");
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Label l = new Label();
                    //Button b = new Button();
                    //Grid.SetColumn(b, j);
                    Grid.SetColumn(l, j);
                    //Grid.SetRow(b, i);
                    Grid.SetRow(l, i);

                    //if (j > 0 && j < glyphSize - 1 && i > 0 && i < glyphSize - 1)
                    {
                        l.Background = System.Windows.Media.Brushes.Black;
                        //l.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 128, 128));
                        l.MouseMove += new MouseEventHandler(NTMouseOverColor);
                        l.MouseDown += new MouseButtonEventHandler(NTStartPaintCtrl);
                        l.Style = Resources.MergedDictionaries[0]["GridLabel"] as Style;
                        //b.Click += new RoutedEventHandler(GridClick);
                        //b.MouseEnter += new MouseEventHandler(MouseOverColor);
                        NTbutts.Add(l);

                    }
                    //else
                    //{
                    //    b.IsEnabled = false;
                    //    b.Style = Resources.MergedDictionaries[0]["RimButton"] as Style;


                    //}

                    NameTableGrid.Children.Add(l);
                    //System.Console.WriteLine("added button at {0}, {1}", i, j);

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
        void clearNTGrid()
        {
            NameTableGrid.Children.Clear();
            NameTableGrid.RowDefinitions.Clear();
            NameTableGrid.ColumnDefinitions.Clear();
            NTbutts.Clear();
        }
        private void GridClick(object sender, bool setHold)
        {

            System.Windows.Media.Brush bg = ((Label)sender).Background;
            BrushConverter bgc = new BrushConverter();
            String test = bgc.ConvertToString(bg);
            test = test.Remove(0, 1);
            UInt32 currentVal = (UInt32)Convert.ToInt32(test, 16);


            UInt32 newVal = currentVal ^ 0x00FFFFFF;
            if (setHold)
            {
                heldCol = newVal;
            }
            else if (heldCol == currentVal)
            {
                return;
            }

            ((Label)sender).Background = (System.Windows.Media.Brush)bgc.ConvertFromString("#" + newVal.ToString("X8"));

        }

        private void NTGridClick(object sender)
        {
            System.Windows.Media.Brush bg = ((Label)sender).Background;
            BrushConverter bgc = new BrushConverter();
            ((Label)sender).Background = (System.Windows.Media.Brush)bgc.ConvertFromString("#" + NTheldCol.ToString("X8"));
        }


        private void StartPaintCtrl(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GridClick(sender, true);
            paintOn = true;
        }

        private void NTStartPaintCtrl(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NTGridClick(sender);
            NTpaintOn = true;
        }
        private void MouseOverColor(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (paintOn)
            {
                GridClick(sender, false);
            }
        }
        private void NTMouseOverColor(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (NTpaintOn)
            {
                NTGridClick(sender);
            }
        }

        private void ClearPaintCtrl(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            paintOn = false;
            NTpaintOn = false;
        }

        private void FileValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9a-zA-Z]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PrintClick(object sender, RoutedEventArgs e)
        {
            textBlock.Text = createMapData();
        }

        private void NTPrintClick(object sender, RoutedEventArgs e)
        {

            textBlock.Text = createNTData();
            textBlock.Text += "\n";
            textBlock.Text += createNameData();
        }
        private String createNTData()
        {
            String title = nameBox.Text + "Image:\n ";
            String metaString = ".db";

            int byteIdx = 0;
            UInt32 byteVal = 0;
            for (int i = 0; i < NTbutts.Count; i++)
            {
                //check the last channel, convert to a 0-3 index with x*button color/byte size
                System.Windows.Media.Brush bg = NTbutts[i].Background;
                BrushConverter bgc = new BrushConverter();
                String test = bgc.ConvertToString(bg);
                test = test.Remove(0, 1);
                UInt32 currentVal = (UInt32)Convert.ToInt32(test, 16);
                currentVal &= 0x000000FF;
                currentVal = (4 * currentVal) / 256;

                byteVal |= currentVal;
                byteIdx++;
                if (byteIdx < 4)
                {
                    byteVal = byteVal << 2;
                }
                else
                {
                    byteIdx = 0;
                    metaString += " $" + byteVal.ToString("X2") + ",";
                    byteVal = 0;

                }
            }

            if (byteIdx != 0)
            {
                for(int i = byteIdx; i < 3; i++)
                {
                    byteVal = byteVal << 2;
                }
                metaString += " $" + byteVal.ToString("X2") + ",";
            }

            metaString += " $FF";

            String outputText = title + metaString;
            return outputText;


        }

        private String createMapData()
        {
            //firstm make a helper map

            int[,] map = createGrid();

            int mapSize = glyphSize * 5;
            String header = ".db";
            String rowString = ".db";
            String colString = ".db";

            int bytesPerRow = (int)Math.Ceiling(mapSize / 8.0f);
            int totalMapBytes = bytesPerRow * mapSize;
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


                    //then the columns

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

                if (colRunVal > 0)
                {
                    colNumbers.Add(colRunVal);
                }
                if (rowRunVal > 0)
                {
                    rowNumbers.Add(rowRunVal);
                }

                bool rowParity = true;
                for (int k = rowNumbers.Count() - 1; k >= 0; k--)
                {

                    //convert number to hex val
                    if (mapSize > 15)
                    {
                        rowString += " $" + rowNumbers[k].ToString("X2") + ",";
                    }
                    else
                    {
                        rowString += rowParity ? " $" + rowNumbers[k].ToString("X1") : rowNumbers[k].ToString("X1") + ",";
                        rowParity = !rowParity;
                    }
                }

                bool colParity = true;
                for (int k = colNumbers.Count() - 1; k >= 0; k--)
                {
                    //convert number to hex val
                    if (mapSize > 15)
                    {
                        colString += " $" + colNumbers[k].ToString("X2") + ",";
                    }
                    else
                    {
                        colString += colParity ? " $" + colNumbers[k].ToString("X1") : colNumbers[k].ToString("X1") + ",";
                        colParity = !colParity;
                    }
                }

                if (rowRunVal == 0)
                {
                    //we wouldn't have added anything if runVal was 0, so we need to add a zero instead
                    rowString += " $00, $FF";
                }
                else
                {
                    if (!rowParity)
                    {
                        //clean up the last number
                        rowString += "0,";
                    }
                    rowString += " $FF";
                }

                if (colRunVal == 0)
                {
                    //we wouldn't have added anything if runVal was 0, so we need to add a zero instead
                    colString += " $00, $FF";
                }
                else
                {
                    if (!colParity)
                    {
                        //clean up the last number
                        colString += "0,";
                    }
                    colString += " $FF";
                }

                if (i < mapSize - 1)
                {
                    rowString += ",";
                    colString += ",";
                }
            }
            int headerSize = glyphSize;
            int correctMarksLo = correctMarks & 0x000000FF;
            int correctMarksHi = correctMarks & 0x0000FF00;
            int palOffset = comboBox.SelectedIndex;
            palOffset |= (bool)checkBox.IsChecked ? 0x10 : 0;
            palOffset |= (bool)keepWhite.IsChecked ? 0x20 : 0;

            header += String.Format(" ${0}, ${1}, ${2}, ${3}", headerSize.ToString("X2"), correctMarksLo.ToString("X2"), correctMarksHi.ToString("X2"), palOffset.ToString("X2"));

            String Map = ".db";
            for (int i = 0; i < mapBytes.Length; i++)
            {
                Map += " $" + ((int)mapBytes[i]).ToString("X2");
                if (i < mapBytes.Length - 1)
                {
                    Map += ",";
                }
            }

            String outputText = nameBox.Text + ":\n  " + header + "\n  " + Map + "\n  " + colString + "\n  " + rowString;
            return outputText;
            //textBlock.Text = outputText;
            //System.Console.WriteLine(";; header, stores the map size index and the correct marker count in little endian");
            //System.Console.WriteLine(header);
            //System.Console.WriteLine(";; binary map data");
            //System.Console.WriteLine(Map);
            //System.Console.WriteLine(";; column numbers are written backwards, since we want to write them from the bottom up");
            //System.Console.WriteLine(colString);
            //System.Console.WriteLine(";; row numbers are written backwards, since we want to write them right to left");
            //System.Console.WriteLine(rowString);
        }

        private String createNameData()
        {
            String name = nameBox.Text.ToLower();
            String nameData = ".db";
            int count = name.Length;
            nameData += " $" + count.ToString("X2") + ",";
            foreach (char c in name)
            {
                int val = (int)(c - 'a');
                val += 10;
                nameData += " $" + val.ToString("X2") + ",";
            }

            nameData += " $FF";

            String outputText = nameBox.Text + "Name:\n  " + nameData;
            return outputText;
        }

        int[,] createGrid()
        {

            int[,] map = new int[glyphSize * 5, glyphSize * 5];

            for (int i = 0; i < glyphSize * 5; i++)
            {

                for (int j = 0; j < glyphSize * 5; j++)
                {

                    map[i, j] = 1;

                    //System.Console.Write(colors[i, j]);
                }
                //System.Console.WriteLine();

            }

            //foreach (Button B in butts)
            foreach (Label B in butts)
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

        private void NTColorClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Media.Brush bg = ((Button)sender).Background;
            BrushConverter bgc = new BrushConverter();
            String test = bgc.ConvertToString(bg);
            test = test.Remove(0, 1);
            UInt32 currentVal = (UInt32)Convert.ToInt32(test, 16);
            NTheldCol = currentVal;
        }

        private void CopyToNTClick(object sender, RoutedEventArgs e)
        {
            //go through both lists of buttons, and copy the background color over
            if (butts.Count != NTbutts.Count)
            {
                return;
            }

            for (int i = 0; i < butts.Count; i++)
            {
                NTbutts[i].Background = butts[i].Background;
            }
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ExportASM(object sender, RoutedEventArgs e)
        {
            String output = "";
            output += createMapData();
            output += "\n";
            output += createNTData();
            output += "\n";
            output += createNameData();

            File.WriteAllText(nameBox.Text + ".asm", output);

        }

    }
}
