using DataStructures.StringLists;
using System;

namespace DataStructures
{
    /// <summary>
    /// Class for a concrete MatrixGraph
    /// </summary>
    /// <authors>cei, Harris von Hassell, Sean Hartwig</authors>
    public class TerritoryGraph : Graph
    {
        /// <summary>
        /// The nodes of the graph
        /// </summary>
        private readonly StringList nodes;

        /// <summary>
        /// The adjacency matrix
        /// </summary>
        private bool[][] adjacencyMatrix;

        /// <summary>
        /// Constructor that creates an empty graph
        /// </summary>
        public TerritoryGraph()
        {
            nodes = new StringList();
            adjacencyMatrix = new bool[0][];
        }

        /// <summary>
        /// Constructor that accepts values for the two attributes (internal)
        /// </summary>
        internal TerritoryGraph(StringList nodes, bool[][] adjacencyMatrix)
        {
            this.nodes = nodes;
            this.adjacencyMatrix = adjacencyMatrix;
        }

        /// <summary>
        /// Method that returns the adjacency matrix (internal)
        /// </summary>
        internal bool[][] GetAdjacencyMatrix()
        {
            return this.adjacencyMatrix;
        }

        public override bool Contains(string node)
        {
            return nodes.Contains(node);
        }

        public override bool ContainsEdge(string start, string end)
        {
            if (Contains(start) && Contains(end))
            {
                int startIndex = nodes.GetIndexOf(start).Value;
                int endIndex = nodes.GetIndexOf(end).Value;
                // Check if there is a connection
                return adjacencyMatrix[startIndex][endIndex];
            }
            else
            {
                return false;
            }
        }

        public override void Add(string node)
        {
            // If the node is not yet present, add it to the end of the node list
            // and update the adjacency matrix
            if (!Contains(node))
            {
                nodes.Append(node);

                bool[][] newAdjacencyMatrix = new bool[adjacencyMatrix.Length + 1][];
                for (int i = 0; i < newAdjacencyMatrix.Length; i++)
                {
                    newAdjacencyMatrix[i] = new bool[adjacencyMatrix.Length + 1];
                }

                // Copy the previous values of the adjacency matrix
                for (int row = 0; row < adjacencyMatrix.Length; row++)
                {
                    for (int col = 0; col < adjacencyMatrix[row].Length; col++)
                    {
                        newAdjacencyMatrix[row][col] = adjacencyMatrix[row][col];
                    }
                }

                int maxIndex = newAdjacencyMatrix.Length - 1;
                // Fill the new column with false
                for (int col = 0; col < maxIndex; col++)
                {
                    newAdjacencyMatrix[maxIndex][col] = false;
                }
                // Fill the new row with false
                for (int row = 0; row < maxIndex; row++)
                {
                    newAdjacencyMatrix[row][maxIndex] = false;
                }

                this.adjacencyMatrix = newAdjacencyMatrix;
            }
        }

        public override void AddEdge(string start, string end)
        {
            if (!Contains(start) || !Contains(end))
            {
                throw new ArgumentException("The start node and/or the end node does not exist!");
            }
            int startIndex = nodes.GetIndexOf(start).Value;
            int endIndex = nodes.GetIndexOf(end).Value;

            if (!adjacencyMatrix[startIndex][endIndex])
            {
                // Set the two corresponding entries of the matrix to true
                adjacencyMatrix[startIndex][endIndex] = true;
                adjacencyMatrix[endIndex][startIndex] = true;
            }
        }

        public override void Remove(string node)
        {
            if (Contains(node))
            {
                int nodeIndex = nodes.GetIndexOf(node).Value;
                nodes.Remove(node);
                bool[][] newAdjacencyMatrix = new bool[adjacencyMatrix.Length - 1][];
                for (int i = 0; i < newAdjacencyMatrix.Length; i++)
                {
                    newAdjacencyMatrix[i] = new bool[adjacencyMatrix.Length - 1];
                }

                // Copy all rows and columns without the affected one
                int newRow = 0;
                int newCol = 0;
                for (int row = 0; row < adjacencyMatrix.Length; row++)
                {
                    if (row != nodeIndex)
                    {
                        for (int col = 0; col < adjacencyMatrix.Length; col++)
                        {
                            if (col != nodeIndex)
                            {
                                newAdjacencyMatrix[newRow][newCol] = adjacencyMatrix[row][col];
                                newCol++;
                            }
                        }
                        newCol = 0;
                        newRow++;
                    }
                }

                this.adjacencyMatrix = newAdjacencyMatrix;
            }
        }

        public override void RemoveEdge(string start, string end)
        {
            if (Contains(start) && Contains(end))
            {
                int startIndex = nodes.GetIndexOf(start).Value;
                int endIndex = nodes.GetIndexOf(end).Value;

                // Set the two corresponding entries of the matrix to false
                adjacencyMatrix[startIndex][endIndex] = false;
                adjacencyMatrix[endIndex][startIndex] = false;
            }
        }

        public override StringList GetAllNodes()
        {
            // Return copy of the node list
            return nodes.Copy();
        }

        public override StringList GetAllNeighboursOf(string node)
        {
            if (!Contains(node))
            {
                return null;
            }
            else
            {
                StringList list = new StringList();
                int nodeIndex = nodes.GetIndexOf(node).Value;
                for (int col = 0; col < adjacencyMatrix.Length; col++)
                {
                    if (adjacencyMatrix[nodeIndex][col])
                    {
                        list.Append(nodes.GetAt(col));
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// Example method to find a path between two territories (BFS)
        /// </summary>
        public bool AreTerritoriesConnected()
        {
            return IsConnected(); // Since it's one connected graph, this works
        }
    }
}