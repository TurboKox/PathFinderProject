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

            // Init

            GenerateTiles(xCount, yCount);

            SetCell("Start", xStartPos, yStartPos);
            SetCell("Destination", xDestPos, yDestPos);
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

            for (int i = 0; i < yCount; i++)
            {
                for (int j = 0; j < xCount; j++)
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
                    if (cell.Name == "Free" || cell.Name == "Path")
                    {
                        cell.Fill = new SolidColorBrush(Color.FromRgb(0, 200, 0));
                        cell.Name = "Start";
                        m_IsStartSet = true;

                        m_StartIndex = GetRectangleIndex(cell);
                    }
                }
                else if (m_CurrentAction == Action.SET_DESTINATION && !m_IsDestinationSet)
                {
                    if (cell.Name == "Free" || cell.Name == "Path")
                    {
                        cell.Fill = new SolidColorBrush(Color.FromRgb(200, 0, 0));
                        cell.Name = "Destination";
                        m_IsDestinationSet = true;

                        m_DestIndex = GetRectangleIndex(cell);
                    }
                }
                else
                {
                    if (cell.Name == "Free" || cell.Name == "Path")
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

        private int GetRectangleIndex(Rectangle rect)
        {
            for (int i = 0; i < m_Rectangles.Count; i++)
            {
                if (rect == m_Rectangles[i])
                    return i;
            }

            return -1;
        }

        private void UpdateAdjacencyList()
        {
            m_Graph.ClearAdjacencyList();
            for (int i = 0; i < yCount; i++)
            {
                for (int j = 0; j < xCount; j++)
                {
                    // check left
                    if (j - 1 >= 0 && m_Rectangles[(j - 1) + i * yCount].Name != "Wall")
                        m_Graph.AddEdge(j + i * yCount, (j - 1) + i * yCount);

                    // check right
                    if (j + 1 < xCount && m_Rectangles[(j + 1) + i * yCount].Name != "Wall")
                        m_Graph.AddEdge(j + i * yCount, (j + 1) + i * yCount);

                    // check up
                    if (i - 1 >= 0 && m_Rectangles[j + (i - 1) * yCount].Name != "Wall")
                        m_Graph.AddEdge(j + i * yCount, j + (i - 1) * yCount);

                    // check down
                    if (i + 1 < yCount && m_Rectangles[j + (i + 1) * yCount].Name != "Wall")
                        m_Graph.AddEdge(j + i * yCount, j + (i + 1) * yCount);
                }
            }
        }

        private void SetCell(string cellType, int xCellIndex, int yCellIndex)
        {
            m_Rectangles[xCellIndex + yCellIndex * yCount].Name = cellType;
            if (cellType == "Start")
            {
                m_Rectangles[xCellIndex + yCellIndex * yCount].Fill = new SolidColorBrush(Color.FromRgb(0, 200, 0));
                m_IsStartSet = true;
            }
            else if (cellType == "Destination")
            {
                m_Rectangles[xCellIndex + yCellIndex * yCount].Fill = new SolidColorBrush(Color.FromRgb(200, 0, 0));
                m_IsDestinationSet = true;
            }
        }

        private void ClearGivenCell(string cellType)
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
            ClearCurrentPath();
            m_CurrentAction = Action.SET_START;
            if (m_IsStartSet)
                ClearGivenCell("Start");

            m_IsStartSet = false;
        }

        private void SetDestinationBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearCurrentPath();
            m_CurrentAction = Action.SET_DESTINATION;
            if (m_IsDestinationSet)
                ClearGivenCell("Destination");

            m_IsDestinationSet = false;
        }

        private void FindPathBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearCurrentPath();
            UpdateAdjacencyList();
            m_Graph.BFS(m_StartIndex, m_DestIndex, ref m_Rectangles);
        }

        private void ClearBoard_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < xCount * yCount; i++)
            {
                if (m_Rectangles[i].Name == "Wall" || m_Rectangles[i].Name == "Path")
                {
                    m_Rectangles[i].Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    m_Rectangles[i].Name = "Free";
                }
            }
        }

        private void ClearCurrentPath()
        {
            for (int i = 0; i < xCount * yCount; i++)
            {
                if (m_Rectangles[i].Name == "Path")
                    m_Rectangles[i].Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }

        class Graph
        {
            public Graph(int verticiesCount)
            {
                m_AdjacencyList = new LinkedList<int>[verticiesCount];
                V = verticiesCount;

                for (int i = 0; i < m_AdjacencyList.Length; i++)
                    m_AdjacencyList[i] = new LinkedList<int>();
            }

            public void AddEdge(int v, int w)
            {
                m_AdjacencyList[v].AddLast(w);
            }

            public void ClearAdjacencyList()
            {
                m_AdjacencyList = new LinkedList<int>[V];

                for (int i = 0; i < m_AdjacencyList.Length; i++)
                    m_AdjacencyList[i] = new LinkedList<int>();
            }

            public void BFS(int start, int destination, ref List<Rectangle> rectArr)
            {
                int[] parent = new int[V];
                for (int i = 0; i < V; i++)
                    parent[i] = -1;
                bool[] visited = new bool[V];
                for (int i = 0; i < V; i++)
                    visited[i] = false;

                LinkedList<int> queue = new LinkedList<int>();

                visited[start] = true;
                queue.AddLast(start);

                while (queue.Any())
                {
                    int current = queue.First();
                    if (current == destination)
                        break;

                    queue.RemoveFirst();

                    LinkedList<int> list = m_AdjacencyList[current];

                    foreach (var val in list)
                    {
                        if (!visited[val])
                        {
                            visited[val] = true;
                            parent[val] = current;
                            queue.AddLast(val);
                        }
                    }
                }

                List<int> path = new List<int>();
                int curr = destination;
                while (curr != -1)
                {
                    path.Add(curr);
                    curr = parent[curr];
                }

                foreach (var val in path)
                {
                    if (val != start && val != destination)
                    {
                        rectArr[val].Fill = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        rectArr[val].Name = "Path";
                    }
                }
            }

            private int V;
            private LinkedList<int>[] m_AdjacencyList;
        }
        private enum Action
        {
            SET_START, SET_DESTINATION
        }

        // Config

        private static int xCount = 15, yCount = 15;
        
        private static int xStartPos = 2, yStartPos = 3;
        private static int xDestPos = 7, yDestPos = 4;

        private int m_ViewportWidth = 600;
        private int m_ViewportHeight = 600;
        private bool m_IsStartSet = false;
        private bool m_IsDestinationSet = false;

        private List<Rectangle> m_Rectangles = new List<Rectangle>();
        private Action m_CurrentAction = (Action)3;

        private Graph m_Graph = new Graph(xCount * yCount);
        private int m_StartIndex = xStartPos + yStartPos * yCount;
        private int m_DestIndex = xDestPos + yDestPos * yCount;
    }
}