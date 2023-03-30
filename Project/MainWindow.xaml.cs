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
            int cellWidth = m_ViewportWidth / xCount;
            int cellHeight = m_ViewportHeight / yCount;

            for (int i = 0; i < xCount; i++)
            {
                var rowDef = new RowDefinition();
                rowDef.Height = new GridLength(cellHeight);
                MainGrid.RowDefinitions.Add(rowDef);

                
            }
            for (int j = 0; j < yCount; j++)
            {
                var columnDef = new ColumnDefinition();
                columnDef.Width = new GridLength(cellWidth);
                MainGrid.ColumnDefinitions.Add(columnDef);
            }

            for (int i = 0; i < xCount; i++)
            {
                for (int j = 0; j < yCount; j++)
                {
                    Rectangle rectangle = new Rectangle();
                    rectangle.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    rectangle.Stroke = new SolidColorBrush(Color.FromRgb(150, 150, 150));
                    Grid.SetRow(rectangle, i);
                    Grid.SetColumn(rectangle, j);
                    MainGrid.Children.Add(rectangle);
                }
            }

            var rowOptions = new RowDefinition();
            rowOptions.Height = new GridLength(m_ViewportHeight);
            MainGrid.RowDefinitions.Add(rowOptions);

            var columnOptions = new ColumnDefinition();
            columnOptions.Width = new GridLength(Width - m_ViewportWidth);
            MainGrid.ColumnDefinitions.Add(columnOptions);

            Grid.SetRow(Options, xCount);
            Grid.SetColumn(Options, yCount);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show(Mouse.GetPosition(this).ToString());
        }

        private void MainGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var cell = e.Source as Rectangle;
            if (cell.Fill != new SolidColorBrush(Color.FromRgb(255, 255, 255)))
                cell.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        }

        private int m_ViewportWidth = 600;
        private int m_ViewportHeight = 600;
    }
}