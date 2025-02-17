using PictoPuzzleMaker.Data;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PictoPuzzleMaker
{
    /// <summary>
    /// Interaction logic for PaletteSelection.xaml
    /// </summary>
    public partial class PaletteSelection : Window
    {

        private MainWindow Main;
        public PaletteSelection()
        {
            InitializeComponent();

            Main = (MainWindow)Application.Current.MainWindow;
            BrushConverter bgc = new BrushConverter();
            Previous.Background = (System.Windows.Media.Brush)bgc.ConvertFromString("#" + FullPalette.GetColor(Main.returnColorButtonColor).ToString("X8"));
            Current.Background = (System.Windows.Media.Brush)bgc.ConvertFromString("#" + FullPalette.GetColor(Main.returnColorButtonColor).ToString("X8"));
            PopulateButtons();
        }

        public void PopulateButtons()
        {
            for(int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    Button button = new Button();
                    BrushConverter bgc = new BrushConverter();
                    int palIdx = (i * 16) + j;
                    button.Background = (System.Windows.Media.Brush)bgc.ConvertFromString("#" + FullPalette.GetColor(palIdx).ToString("X8"));
                    button.Click += OnColorClick;
                    Grid.SetColumn(button, j);
                    Grid.SetRow(button, i);
                    colorGrid.Children.Add(button);
                }
                
            }

        }

        public void OnColorClick(object sender, RoutedEventArgs e)
        {
            int x = Grid.GetColumn((Button)sender);
            int y = Grid.GetRow((Button)sender);
            //make 1D
            int palIdx = (y * 16) + x;
            Main.returnColorButtonColor = palIdx;
            BrushConverter bgc = new BrushConverter();

            Current.Background = (System.Windows.Media.Brush)bgc.ConvertFromString("#" + FullPalette.GetColor(Main.returnColorButtonColor).ToString("X8"));

        }

    }
}
