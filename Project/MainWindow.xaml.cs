﻿using System;
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

            SetCell("Start", m_StartIndex);
            SetCell("Destination", m_DestIndex);
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
                else if (!m_CurrentlyFinding)
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

        private void FreeMapBuffer()
        {
            for (int i = 0; i < yCount; i++)
            {
                for (int j = 0; j < xCount; j++)
                {
                    MainGrid.Children.Remove(m_Rectangles[j + i * yCount]);
                    MainGrid.RowDefinitions.Clear();
                    MainGrid.ColumnDefinitions.Clear();
                }
            }
            m_Rectangles.Clear();
        }

        private void Slider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FreeMapBuffer();

            var slider = sender as Slider;
            xCount = (int)slider.Value;
            yCount = (int)slider.Value;

            MapSizeLbl.Content = "Map Size: " + xCount + " x " + yCount;
            GenerateTiles(xCount, yCount);
                
            m_IsStartSet = false;
            m_IsDestinationSet = false;
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

        private void SetCell(string cellType, int index)
        {
            m_Rectangles[index].Name = cellType;
            if (cellType == "Start")
            {
                m_Rectangles[index].Fill = new SolidColorBrush(Color.FromRgb(0, 200, 0));
                m_IsStartSet = true;
            }
            else if (cellType == "Destination")
            {
                m_Rectangles[index].Fill = new SolidColorBrush(Color.FromRgb(200, 0, 0));
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
            if (m_IsStartSet && m_IsDestinationSet && !m_CurrentlyFinding)
            {
                ClearCurrentPath();
                UpdateAdjacencyList();
                m_Graph.BFS(m_StartIndex, m_DestIndex, m_Rectangles);
            }
        }

        private void ClearBoard_Click(object sender, RoutedEventArgs e)
        {
            if (!m_CurrentlyFinding)
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
                m_AdjacencyList = new LinkedList<int>[xCount * yCount];
                V = m_AdjacencyList.Count();

                for (int i = 0; i < m_AdjacencyList.Length; i++)
                    m_AdjacencyList[i] = new LinkedList<int>();
            }

            public async void BFS(int start, int destination, List<Rectangle> rectArr)
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

                bool foundPath = false;

                while (queue.Any())
                {
                    int current = queue.First();
                    if (current == destination)
                    {
                        foundPath = true;
                        break;
                    }

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

                if (!foundPath)
                {
                    int delayIntensity = 20;
                    for (int i = 0; i < delayIntensity; i++)
                    {
                        for (int j = 0; j < xCount * yCount; j++)
                        {
                            if (rectArr[j].Name == "Free" || rectArr[j].Name == "Path")
                                rectArr[j].Fill = new SolidColorBrush(Color.FromRgb(255, (byte)(255 * i / (delayIntensity - 1)), (byte)(255 * i / (delayIntensity - 1))));
                        }
                        await Task.Delay(delayIntensity - i);
                    }
                }
                else
                {
                    List<int> path = new List<int>();
                    int index = destination;
                    while (index != -1)
                    {
                        path.Add(index);
                        index = parent[index];
                    }
                    path.Reverse();

                    m_CurrentlyFinding = true;
                    foreach (var val in path)
                    {
                        if (val != start && val != destination)
                        {
                            rectArr[val].Fill = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                            rectArr[val].Name = "Path";

                            if (!m_IsStartSet || !m_IsDestinationSet)
                                rectArr[val].Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        }
                    
                        await Task.Delay(20);
                    }
                    m_CurrentlyFinding = false;
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
        private static int xDestPos = 12, yDestPos = 7;

        private int m_ViewportWidth = 600;
        private int m_ViewportHeight = 600;
        private static bool m_IsStartSet = false;
        private static  bool m_IsDestinationSet = false;

        private List<Rectangle> m_Rectangles = new List<Rectangle>();
        private Action m_CurrentAction = (Action)3;

        private Graph m_Graph = new Graph(xCount * yCount);

        private int m_StartIndex = xStartPos + yStartPos * yCount;

        private int m_DestIndex = xDestPos + yDestPos * yCount;

        private static bool m_CurrentlyFinding = false;
    }
}