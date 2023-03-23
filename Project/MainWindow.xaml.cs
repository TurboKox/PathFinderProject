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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            GenerateTiles(25, 25);
        }

        private void GenerateTiles(int xCount, int yCount)
        {
            int cellWidth = (int)Width / xCount;
            int cellHeight = (int)Height / yCount;

            for (int i = 0; i < xCount; i++)
            {
                var rowDef = new RowDefinition();
                rowDef.Height = new GridLength(cellHeight);
                MainGrid.RowDefinitions.Add(rowDef);

                for (int j = 0; j < yCount; j++)
                {
                    var columnDef = new ColumnDefinition();
                    columnDef.Width = new GridLength(cellWidth);

                    Rectangle rectangle = new Rectangle();
                    //rectangle.Fill = new SolidColorBrush(Color.FromRgb((byte)(i*j), 0, (byte)(i * j)));
                    rectangle.Stroke = new  SolidColorBrush(Color.FromRgb(150, 150, 150));
                    Grid.SetRow(rectangle, i);
                    Grid.SetColumn(rectangle, j);

                    MainGrid.ColumnDefinitions.Add(columnDef);
                    MainGrid.Children.Add(rectangle);
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(Mouse.GetPosition(this).ToString());
        }

        private void MainGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var mouseWasDownOn = e.Source as FrameworkElement;
            if (mouseWasDownOn != null)
            {
                //MessageBox.Show(mouseWasDownOn.ToString());
            }
        }
    }
}