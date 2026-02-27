using DataStructures.StringLists;
using System.Text;

namespace DataStructures
{
    /// <summary>
    /// Class for an undirected, unweighted graph with nodes of type String.
    /// </summary>
    /// <authors>cei, Harris von Hassell, Sean Hartwig</authors>
    public abstract class Graph
    {
        /// <summary>
        /// Checks if the given node is part of the graph.
        /// </summary>
        /// <param name="node">Searched node</param>
        /// <returns>true if the node is part of the graph.</returns>
        public abstract bool Contains(string node);

        /// <summary>
        /// Checks if the graph contains a given edge. It can be assumed that the graph is consistent,
        /// so no additional checks are necessary whether there is an edge from start to end AND from end to start.
        /// If one of the two nodes does not exist, the edge does not exist either.
        /// </summary>
        /// <param name="start">Node where the edge begins</param>
        /// <param name="end">Node where the edge ends</param>
        /// <returns>true if the graph contains an edge from start to end</returns>
        public abstract bool ContainsEdge(string start, string end);

        /// <summary>
        /// Adds a node to the graph. A node can only be added once. If a node is already
        /// present, the graph will remain unchanged.
        /// In a node list, the new node is added at the end.
        /// If the graph is represented by an adjacency matrix, both a column
        /// and a row are added to this matrix in this method, but no edges (-> the new row/column is filled with false).
        /// </summary>
        /// <param name="node">The node to add</param>
        public abstract void Add(string node);

        /// <summary>
        /// Adds an edge from the start node to the end node.
        /// Since the graph is undirected, an edge is also added from the end node to the start node.
        /// If the edge already exists, nothing happens.
        /// If the start node or end node does not exist, an IllegalArgumentException is thrown.
        /// </summary>
        /// <param name="start">Start node of the edge</param>
        /// <param name="end">End node of the edge</param>
        public abstract void AddEdge(string start, string end);

        /// <summary>
        /// Removes the given node from the graph. All connections (edges) of this node are also removed.
        /// When working with an adjacency matrix, it will have one less column and one less row afterwards.
        /// If the node is not part of the graph, it remains unchanged.
        /// </summary>
        /// <param name="node">Node to be removed</param>
        public abstract void Remove(string node);

        /// <summary>
        /// Removes the edge between two nodes. For undirected graphs, after this method call there must be neither
        /// an edge from start to end nor an edge from end to start. It can be assumed that the
        /// graph is consistent.
        /// If the start node or end node does not exist, the graph remains unchanged.
        /// </summary>
        /// <param name="start">Start node of the edge</param>
        /// <param name="end">End node of the edge</param>
        public abstract void RemoveEdge(string start, string end);

        /// <summary>
        /// Returns all nodes that are part of the graph in a list. There is no information here about the
        /// edges between the nodes. No reference to the node list is given out, but a copy.
        /// </summary>
        /// <returns>All nodes that are part of the graph</returns>
        public abstract StringList GetAllNodes();

        /// <summary>
        /// Returns all neighbors of a given node. If there are no neighbors, the list is empty.
        /// If the node is not part of the graph, null is returned.
        /// Two nodes are neighbors if an edge exists between them.
        /// </summary>
        /// <param name="node">The node whose neighbors should be determined</param>
        /// <returns>List with the neighbors of the node or null if the node is not part of the graph</returns>
        public abstract StringList GetAllNeighboursOf(string node);

        /// <summary>
        /// Checks if the graph is connected. The graph is connected if there is a path from any node to
        /// any other node. See also https://en.wikipedia.org/wiki/Connectivity_(graph_theory).
        /// </summary>
        /// <returns>true if the graph is connected</returns>
        public bool IsConnected()
        {
            if (this.GetAllNodes().IsEmpty())
                return true;

            StringList allNodes = GetAllNodes();
            bool[] visited = new bool[GetAllNodes().Size()];
            StringList queue = new StringList();

            int startNodeIndex = 0;
            queue.Append(allNodes.GetAt(startNodeIndex));
            visited[startNodeIndex] = true;
            while (!queue.IsEmpty())
            {
                string node = queue.GetAt(0);
                queue.Remove(node);
                StringList neighbours = GetAllNeighboursOf(node);
                for (int i = 0; i < neighbours.Size(); i++)
                {
                    int neighbourIndex = allNodes.GetIndexOf(neighbours.GetAt(i)).Value;
                    if (!visited[neighbourIndex])
                    {
                        visited[neighbourIndex] = true;
                        queue.Append(neighbours.GetAt(i));
                    }
                }
            }
            return GetAllTrue(visited);
        }

        /// <summary>
        /// Checks if two graphs are equal. They are equal if they have the same nodes and the same edges.
        /// The internal order of the nodes is irrelevant! A graph with nodes {A, B, C} can therefore be equal to a
        /// graph {B, C, A} if the same edges exist.
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>true if the object is an equal graph</returns>
        public override bool Equals(object obj)
        {
            bool hasSameNodes = false;
            bool hasSameEdges = false;

            if (obj is TerritoryGraph otherGraph)
            {
                // If the node lists are different lengths, return false
                StringList nodes = GetAllNodes();
                StringList otherNodes = otherGraph.GetAllNodes();
                int size = nodes.Size();
                int otherNodesSize = otherNodes.Size();
                if (size == otherNodesSize)
                {
                    hasSameNodes = true;
                    hasSameEdges = true;

                    // Check if the current node is present in the other graph
                    for (int i = 0; i < size && hasSameNodes && hasSameEdges; i++)
                    {
                        string node = nodes.GetAt(i);
                        hasSameNodes = otherGraph.Contains(node);

                        if (hasSameNodes)
                        {
                            StringList theseNeighbours = GetAllNeighboursOf(node);
                            StringList otherNeighbours = otherGraph.GetAllNeighboursOf(node);
                            int theseNeighboursSize = theseNeighbours.Size();
                            int otherNeighboursSize = otherNeighbours.Size();

                            if (theseNeighboursSize == otherNeighboursSize)
                            {
                                for (int k = 0; k < theseNeighboursSize && hasSameEdges; k++)
                                {
                                    hasSameEdges = otherNeighbours.Contains(theseNeighbours.GetAt(k));
                                }
                            }
                            else
                            {
                                hasSameEdges = false;
                            }
                        }
                    }
                }
            }

            return hasSameNodes && hasSameEdges;
        }

        /// <summary>
        /// Creates a string representation of the graph.
        /// For each node, the node followed by "->" and a list of its neighbors
        /// is output on a separate line. The neighbors are separated by commas.
        /// </summary>
        /// <returns>String representation of the graph</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            StringList nodes = this.GetAllNodes();
            int nodesSize = nodes.Size();
            for (int i = 0; i < nodesSize; ++i)
            {
                string node = nodes.GetAt(i);
                StringList neighbours = this.GetAllNeighboursOf(node);
                if (neighbours != null)
                    sb.Append(node).Append(" -> ").Append(neighbours.ToString()).Append("\n");
            }
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            return GetAllNodes().GetHashCode();
        }

        /// <summary>
        /// Helper method to determine if all values in a boolean array are true
        /// </summary>
        /// <param name="array">The array to examine</param>
        /// <returns>true if all values in the array are true</returns>
        private bool GetAllTrue(bool[] array)
        {
            bool result = true;
            for (int i = 0; i < array.Length && result; i++)
            {
                result = array[i];
            }
            return result;
        }
    }
}