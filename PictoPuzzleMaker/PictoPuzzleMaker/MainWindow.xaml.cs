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
using Newtonsoft.Json;
using PictoPuzzleMaker.Data;
using PictoPuzzleMaker.WPFUtils;
using System.Reflection;
using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Xml.Linq;
using Microsoft.Win32;
using System.Runtime.InteropServices.ComTypes;


namespace PictoPuzzleMaker
{
    
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

        public PuzzleBank bank;
        public BankObserver bankObserver = new BankObserver();

        public Puzzle defaultPuzzle;
        public Puzzle currentPuzzle;

        public int glyphSize = 1;
        public int heldCol = 0;
        public int NTheldCol = 0;
        public String[] savDlg = { "「{0}」　ヲ　ホゾン　シタ。" };

        public bool paintOn = false;
        public bool NTpaintOn = false;

        public int currentColorButton = 0;
        public int returnColorButtonColor = 0;

        string currentFileName = "puzBank";
        string currentPath = "";
        string exportPath = "";

        public MainWindow()
        {
            FullPalette.LoadFullpalette();
            defaultPuzzle = new Puzzle(0);
            InitializeComponent();
            bank = new PuzzleBank();
            currentPuzzle = defaultPuzzle;

            bankObserver.LoadBank(bank);
            listView.ItemsSource = bankObserver.observablePuzzles;
            listView.SelectedIndex = 0;
            LoadPuzzleFromListView();


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
            glyphSize = Math.Min(Convert.ToInt32(glyphSizeBox.Text), 2);
            if (currentPuzzle != null)
            {
                currentPuzzle.SetSize(glyphSize);
            }
            updateGridSize();
        }

        void updateGridSize()
        {
            clearGrid();
            clearNTGrid();
            populateGrid(Puzzle.s_SideLengths[currentPuzzle.puzzleSize]);
            populateNTGrid(Puzzle.s_SideLengths[currentPuzzle.puzzleSize]);
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
                        
                        //label has attached 


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
            if((Label)sender == null || currentPuzzle == null)
            {
                return;
            }

            //get the col/row of button
            int x = Grid.GetColumn((Label)sender);
            int y = Grid.GetRow((Label)sender);
            int current = currentPuzzle.GetSolution(x, y);

            int newVal = current ^ 0x01;
            if (setHold)
            {
                heldCol = newVal;
            }
            else if (heldCol == current)
            {
                return;
            }

            currentPuzzle.SetSolution(x, y, newVal);
            BrushConverter bgc = new BrushConverter();
            ((Label)sender).Background = (System.Windows.Media.Brush)bgc.ConvertFromString("#" + Consts.solutionColors[newVal].ToString("X8"));

        }

        private void NTGridClick(object sender)
        {

            if ((Label)sender == null || currentPuzzle == null)
                return;

            int x = Grid.GetColumn((Label)sender);
            int y = Grid.GetRow((Label)(sender));
            currentPuzzle.SetImage(x, y, NTheldCol);

            BrushConverter bgc = new BrushConverter();
            int color = FullPalette.GetColor(FullPalette.desiredBlack);
            if (NTheldCol != 0)
                color = FullPalette.GetColor(currentPuzzle.GetPaletteAt(NTheldCol - 1));
            ((Label)sender).Background = (System.Windows.Media.Brush)bgc.ConvertFromString("#" + color.ToString("X8"));
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
            //textBlock.Text = createMapData();
        }

        private void NTPrintClick(object sender, RoutedEventArgs e)
        {

            //textBlock.Text = createNTData();
            //textBlock.Text += "\n";
            //textBlock.Text += createNameData();
        }
        private String createNTData(Puzzle puzzle)
        {
            String correctedName = currentFileName + "_" + puzzle.name.Replace(" ", "_");
            String title = correctedName + "Image:\n ";
            String metaString = ".db";

            int byteIdx = 0;
            int byteVal = 0;
            for (int i = 0; i < Puzzle.s_TotalLengths[puzzle.puzzleSize]; i++)
            {
                //check the last channel, convert to a 0-3 index with x*button color/byte size


                byteVal |= puzzle.GetImage(i);
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

        private String createMapData(Puzzle puzzle)
        {
            //firstm make a helper map
            int mapSize = Puzzle.s_SideLengths[puzzle.puzzleSize];
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

                    rowVal = puzzle.GetSolution(j, i);

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

                    colVal = puzzle.GetSolution(i, j);

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
            int headerSize = puzzle.puzzleSize;
            int correctMarksLo = correctMarks & 0x000000FF;
            int correctMarksHi = correctMarks & 0x0000FF00;
            int pal1 = puzzle.GetPaletteAt(0);
            int pal2 = puzzle.GetPaletteAt(1);
            int pal3 = puzzle.GetPaletteAt(2);

            header += String.Format(" ${0}, ${1}, ${2}, ${3}, ${4}, ${5}", headerSize.ToString("X2"), correctMarksLo.ToString("X2"), correctMarksHi.ToString("X2"), pal1.ToString("X2"), pal2.ToString("X2"), pal3.ToString("X2"));

            String Map = ".db";
            for (int i = 0; i < mapBytes.Length; i++)
            {
                Map += " $" + ((int)mapBytes[i]).ToString("X2");
                if (i < mapBytes.Length - 1)
                {
                    Map += ",";
                }
            }

            string correctedName = currentFileName+"_"+puzzle.name.Replace(' ', '_');

            String outputText = correctedName + ":\n  " + header + "\n  " + Map + "\n  " + colString + "\n  " + rowString;
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

        private String createNameData(Puzzle puzzle)
        {
            String name = puzzle.name.ToLower();
            String nameData = ".db";
            int count = name.Length;
            nameData += " $" + count.ToString("X2") + ",";
            foreach (char c in name)
            {

                int val = (int)(c - 'a');
                val += 10;
                if (char.IsDigit(c))
                {
                    val = (int)(c - '0');
                }
                else if (c == ' ')
                {
                    val = 0x24;
                }

                nameData += " $" + val.ToString("X2") + ",";
            }

            nameData += " $FF";

            string correctedName = currentFileName + "_" + puzzle.name.Replace(' ', '_');
            String outputText = correctedName + "Name:\n  " + nameData;
            return outputText;
        }

        private void glyphSizeBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void nameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(currentPuzzle != null && currentPuzzle.name != nameBox.Text)
            {
                currentPuzzle.name = nameBox.Text;
                listView.Items.Refresh();
            }
        }

        private void NTColorClick(object sender, RoutedEventArgs e)
        {
            if((Button)sender == null)
                { return; }

            

            int x = Grid.GetColumn((Button)sender);
            NTheldCol = x;
        }

        private void NTColorChange(object sender, MouseButtonEventArgs e)
        {
            if (currentPuzzle.IsEasyPalEnabled())
                return;

            int x = (int)Grid.GetColumn((Button) sender);
            if (x == 0)
                return;
            currentColorButton = x;
            int currentColorButtonColor = currentPuzzle.GetPaletteAt(x - 1);
            returnColorButtonColor = currentColorButtonColor;
            PaletteSelection palWin = new PaletteSelection();
            palWin.ShowDialog();

            if (returnColorButtonColor != currentPuzzle.GetPaletteAt(x - 1))
            {
                currentPuzzle.SetPaletteEntry(x - 1, returnColorButtonColor);
                UpdateNTColors();
            }

        }

        private void CopyToNTClick(object sender, RoutedEventArgs e)
        {
            //go through both lists of buttons, and copy the background color over
            if (butts.Count != NTbutts.Count)
            {
                return;
            }

            for (int i = 0; i < Puzzle.s_TotalLengths[currentPuzzle.puzzleSize]; i++)
            {
                currentPuzzle.SetImage(i, 3 * currentPuzzle.GetSolution(i));
            }
            UpdateNTColors();
        }

        private void UseEasyPalChecked(object sender, RoutedEventArgs e)
        {
            currentPuzzle.SetUseEasyPalette((bool)useEzPal.IsChecked);
            UpdateNTColors();
        }

        private void UseBrightsChecked(object sender, RoutedEventArgs e)
        {
            currentPuzzle.SetUseBrighterColors((bool)checkBox.IsChecked);
            UpdateNTColors();

        }

        private void UseWhiteChecked(object sender, RoutedEventArgs e)
        {
            currentPuzzle.SetKeepWhite((bool)keepWhite.IsChecked);
            UpdateNTColors();
        }

        private void EasyPalChanged(object sender, SelectionChangedEventArgs e)
        {
            currentPuzzle.SetEasyPaletteSelection((Palettes.eEasyPalette)comboBox.SelectedIndex);
            UpdateNTColors();
        }

        private void ExportASM(object sender, RoutedEventArgs e)
        {
            //foreach(Puzzle p in bankObserver.observablePuzzles)
            //{
            //    String output = "";
            //    output += createMapData(p);
            //    output += "\n";
            //    output += createNTData(p);
            //    output += "\n";
            //    output += createNameData(p);
            //    output += "\n";
            //}

            //File.WriteAllText(nameBox.Text + ".asm", output);

        }


        private void Toolbar_New(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmOpen = MessageBox.Show("Starting a new file will close the current file. Any unsaved changes will be lost.", "Confirmation", MessageBoxButton.OKCancel);

            if (confirmOpen == MessageBoxResult.OK)
            {
                bank = new PuzzleBank();
                bankObserver.LoadBank(bank);
                listView.ItemsSource = bankObserver.observablePuzzles;
                listView.SelectedIndex = 0;
                LoadPuzzleFromListView();

                currentFileName = "puzzleBank";
                currentPath = "";
                exportPath = "";
            }

        }

        private void Toolbar_Open(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmOpen = MessageBox.Show("Opening another file will close the current file. Any unsaved changes will be lost.", "Confirmation", MessageBoxButton.OKCancel);

            if (confirmOpen == MessageBoxResult.OK)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "PUZ (*.puz)|*.puz";
                if (openFileDialog.ShowDialog() == true)
                {

                    string json = System.IO.File.ReadAllText(openFileDialog.FileName);
                    bank = JsonConvert.DeserializeObject<PuzzleBank>(json);
                    bankObserver.LoadBank(bank);
                    listView.ItemsSource = bankObserver.observablePuzzles;
                    listView.SelectedIndex = 0;
                    LoadPuzzleFromListView();
                    currentPath = openFileDialog.FileName;
                    currentFileName = openFileDialog.SafeFileName;
                    currentFileName = currentFileName.Remove(currentFileName.Length - 4, 4);
                }


            }

        }
        private void Toolbar_Save(object sender, RoutedEventArgs e)
        {
            DoSave(true);
        }

        private void Toolbar_SaveAs(object sender, RoutedEventArgs e)
        {
            DoSave(false);
        }

        private void DoSave(bool useCurrent)
        {
            bool? doSave = true;
            if (!useCurrent || currentPath == "")
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = currentFileName;
                saveFileDialog.Filter = "PUZ (*.puz)|*.puz";
                doSave = saveFileDialog.ShowDialog();
                if (doSave == true)
                {

                    currentPath = saveFileDialog.FileName;
                    exportPath = "";
                    currentFileName = saveFileDialog.SafeFileName;
                    currentFileName = currentFileName.Remove(currentFileName.Length - 4, 4);

                }
                
            }
            
            if(doSave == true)
            {
                string json = JsonConvert.SerializeObject(bank, Formatting.Indented);
                System.IO.File.WriteAllText(currentPath, json);
            }

        }


        private void Toolbar_Export(object sender, RoutedEventArgs e)
        {
            string expPath = "";
            string expTables = "";

            expPath = currentFileName;
            expTables = currentFileName + "Tables";



            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = expPath;
            saveFileDialog.Filter = "ASM (*.asm)|*.asm";
            if (saveFileDialog.ShowDialog() == true)
            {
                exportPath = saveFileDialog.FileName;
                //do ASM export
                String output = "";
                foreach (Puzzle p in bankObserver.observablePuzzles)
                {
                    
                    output += createMapData(p);
                    output += "\n";
                    output += createNTData(p);
                    output += "\n";
                    output += createNameData(p);
                    output += "\n\n";
                }

                File.WriteAllText(exportPath, output);
            }

            saveFileDialog.FileName = expTables;
            saveFileDialog.Filter = "ASM (*.asm)|*.asm";
            if (saveFileDialog.ShowDialog() == true)
            {
                exportPath = saveFileDialog.FileName;
                //do ASM export
                String puzzleTable = "";
                String puzzleNames = "";
                for(int i = 0; i < bankObserver.observablePuzzles.Count; i++)
                {
                    Puzzle p = bankObserver.observablePuzzles[i];
                    String name = currentFileName + "_" + p.name.Replace(' ', '_');
                    if(i % 9 == 0)
                    {
                        puzzleTable += "  .word " + name;
                        puzzleNames += "  .word " + name + "Name";
                    }
                    else
                    {
                        puzzleTable += ", " + name;
                        puzzleNames += ", " + name + "Name";
                        if(i % 9 == 8)
                        {
                            puzzleTable += "\n";
                            puzzleNames += "\n";
                        }

                    }
                }

                String output = puzzleTable + "\n" + puzzleNames;

                File.WriteAllText(exportPath, output);
            }
        }
        private void Toolbar_About(object sender, RoutedEventArgs e)
        {

        }

        private void LoadSelectedClick(object sender, RoutedEventArgs e)
        {
            //LoadPuzzleFromListView();   
        }

        private void ResetClick(object sender, RoutedEventArgs e)
        {
            //clear the puzzle
        }

        private void LoadPuzzleFromListView()
        {
            //load from listView selected index
            currentPuzzle = bankObserver.observablePuzzles[listView.SelectedIndex];
            nameBox.Text = currentPuzzle.name;
            glyphSizeBox.Text = currentPuzzle.puzzleSize.ToString();

            updateGridSize();
            BrushConverter bgc = new BrushConverter();
            foreach (Label button in butts)
            {
                //set the color of the solution buttons to match the puzzle
                int x = Grid.GetColumn(button);
                int y = Grid.GetRow(button);
                int val = currentPuzzle.GetSolution(x, y);
                button.Background = (System.Windows.Media.Brush)bgc.ConvertFromString("#" + Consts.solutionColors[val].ToString("X8"));
            }
            int index = 0;
            bool enabled = false;
            bool bright = false;
            bool white = false;

            currentPuzzle.GetEasyPaletteData(out enabled, out bright, out white, out index);
            comboBox.SelectedIndex = index;
            checkBox.IsChecked = bright;
            keepWhite.IsChecked = white;
            useEzPal.IsChecked = enabled;
            UpdateNTColors();


        }

        public void UpdateNTColors()
        {
            BrushConverter bgc = new BrushConverter();
            int color;
            foreach (Label button in NTbutts)
            {
                int x = Grid.GetColumn(button);
                int y = Grid.GetRow(button);
                int val = currentPuzzle.GetImage(x, y);
                color = FullPalette.GetColor(FullPalette.desiredBlack);
                if(val != 0) 
                    color = FullPalette.GetColor(currentPuzzle.GetPaletteAt(val-1));
                button.Background = (System.Windows.Media.Brush)bgc.ConvertFromString("#" + color.ToString("X8"));
            }
            color = FullPalette.GetColor(currentPuzzle.GetPaletteAt(0));
            color1.Background = (System.Windows.Media.Brush)bgc.ConvertFromString("#" + color.ToString("X8"));
            color = FullPalette.GetColor(currentPuzzle.GetPaletteAt(1));
            color2.Background = (System.Windows.Media.Brush)bgc.ConvertFromString("#" + color.ToString("X8"));
            color = FullPalette.GetColor(currentPuzzle.GetPaletteAt(2));
            color3.Background = (System.Windows.Media.Brush)bgc.ConvertFromString("#" + color.ToString("X8"));



        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(bankObserver.observablePuzzles.Count > 0 && listView.SelectedIndex >= 0)
            {
                LoadPuzzleFromListView();

            }
        }
    }
}
