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

            GenerateTiles(xCount, yCount);
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
                    rectangle.Name = "Free";
                    m_Rectangles.Add(rectangle);
                    Grid.SetRow(rectangle, i);
                    Grid.SetColumn(rectangle, j);
                    MainGrid.Children.Add(rectangle);
                }
            }
        }

        private void MainGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var cell = e.Source as Rectangle;

            if (cell != null)
            {
                if (m_CurrentAction == Action.SET_START && !m_IsStartSet)
                {
                    if (cell.Name == "Free")
                    {
                        cell.Fill = new SolidColorBrush(Color.FromRgb(0, 200, 0));
                        cell.Name = "Start";
                        m_IsStartSet = true;
                    }
                }
                else if (m_CurrentAction == Action.SET_DESTINATION && !m_IsDestinationSet)
                {
                    if (cell.Name == "Free")
                    {
                        cell.Fill = new SolidColorBrush(Color.FromRgb(200, 0, 0));
                        cell.Name = "Destination";
                        m_IsDestinationSet = true;
                    }
                }
                else
                {
                    if (cell.Name == "Free")
                    {
                        cell.Fill = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                        cell.Name = "Wall";
                    }
                    else if (cell.Name == "Wall")
                    {
                        cell.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        cell.Name = "Free";
                    }
                }
            }
        }

        private void ClearCell(string cellType)
        {
            for (int i = 0; i < xCount * yCount; i++)
            {
                if (m_Rectangles[i].Name == cellType)
                {
                    m_Rectangles[i].Name = "Free";
                    m_Rectangles[i].Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }
            }
        }

        private void SetStartPositionBtn_Click(object sender, RoutedEventArgs e)
        {
            m_CurrentAction = Action.SET_START;
            if (m_IsStartSet)
                ClearCell("Start");
            m_IsStartSet = false;
        }

        private void SetDestinationBtn_Click(object sender, RoutedEventArgs e)
        {
            m_CurrentAction = Action.SET_DESTINATION;
            if (m_IsDestinationSet)
                ClearCell("Destination");
            m_IsDestinationSet = false;
        }

        private void FindPathBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private int xCount = 10, yCount = 10;
        private int m_ViewportWidth = 600;
        private int m_ViewportHeight = 600;
        private bool m_IsStartSet = false;
        private bool m_IsDestinationSet = false;
        private List<Rectangle> m_Rectangles = new List<Rectangle>();
        private enum Action
        {
            SET_START, SET_DESTINATION
        }
        private Action m_CurrentAction = (Action)3;
    }
}