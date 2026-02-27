using DataStructures.StringLists;
using DataStructures;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Helper classes for deserializing adjacency list JSON with Unity's JsonUtility
    /// </summary>
    [Serializable]
    public class AdjacencyNode
    {
        public string nodeName;
        public string[] neighbours;
    }

    [Serializable]
    public class AdjacencyList
    {
        public AdjacencyNode[] nodes;
    }

    /// <summary>
    /// Utility class for loading graph data from JSON
    /// </summary>
    public static class GraphJsonLoader
    {
        /// <summary>
        /// Loads a MatrixGraph from a JSON file containing an adjacency list.
        /// The JSON should be in the format: {"NodeName": ["Neighbor1", "Neighbor2", ...], ...}
        /// </summary>
        /// <param name="jsonText">The JSON text content</param>
        /// <returns>A MatrixGraph with all nodes and edges from the JSON</returns>
        public static TerritoryGraph LoadFromJson(string jsonText)
        {
            // Convert the dictionary-style JSON to an array format that Unity can handle
            string convertedJson = ConvertDictionaryJsonToArray(jsonText);
            
            // Deserialize using Unity's JsonUtility
            AdjacencyList adjacencyList = JsonUtility.FromJson<AdjacencyList>(convertedJson);
            
            // Create the graph
            TerritoryGraph graph = new TerritoryGraph();
            
            // First pass: Add all nodes
            foreach (var node in adjacencyList.nodes)
            {
                graph.Add(node.nodeName);
            }
            
            // Second pass: Add all edges
            foreach (var node in adjacencyList.nodes)
            {
                if (node.neighbours != null)
                {
                    foreach (var neighbour in node.neighbours)
                    {
                        // Only add edge if it doesn't exist yet (to avoid duplicates in undirected graph)
                        if (!graph.ContainsEdge(node.nodeName, neighbour))
                        {
                            graph.AddEdge(node.nodeName, neighbour);
                        }
                    }
                }
            }
            
            return graph;
        }
        
        /// <summary>
        /// Converts dictionary-style JSON {"key": ["val1", "val2"]} 
        /// to array-style JSON {"nodes": [{"nodeName": "key", "neighbours": ["val1", "val2"]}, ...]}
        /// </summary>
        private static string ConvertDictionaryJsonToArray(string jsonText)
        {
            // Remove outer braces and whitespace
            jsonText = jsonText.Trim();
            if (jsonText.StartsWith("{"))
                jsonText = jsonText.Substring(1);
            if (jsonText.EndsWith("}"))
                jsonText = jsonText.Substring(0, jsonText.Length - 1);
            
            List<string> nodeJsons = new List<string>();
            
            // Simple parser to split the dictionary into key-value pairs
            int braceDepth = 0;
            int bracketDepth = 0;
            int currentStart = 0;
            bool inString = false;
            
            for (int i = 0; i < jsonText.Length; i++)
            {
                char c = jsonText[i];
                
                if (c == '"' && (i == 0 || jsonText[i - 1] != '\\'))
                {
                    inString = !inString;
                }
                else if (!inString)
                {
                    if (c == '{') braceDepth++;
                    else if (c == '}') braceDepth--;
                    else if (c == '[') bracketDepth++;
                    else if (c == ']') bracketDepth--;
                    else if (c == ',' && braceDepth == 0 && bracketDepth == 0)
                    {
                        string pair = jsonText.Substring(currentStart, i - currentStart).Trim();
                        if (!string.IsNullOrEmpty(pair))
                        {
                            nodeJsons.Add(ConvertPairToNodeJson(pair));
                        }
                        currentStart = i + 1;
                    }
                }
            }
            
            // Add the last pair
            string lastPair = jsonText.Substring(currentStart).Trim();
            if (!string.IsNullOrEmpty(lastPair))
            {
                nodeJsons.Add(ConvertPairToNodeJson(lastPair));
            }
            
            return "{\"nodes\":[" + string.Join(",", nodeJsons) + "]}";
        }
        
        /// <summary>
        /// Converts a single key-value pair like "NodeName": ["Neighbor1", "Neighbor2"]
        /// to {"nodeName": "NodeName", "neighbours": ["Neighbor1", "Neighbor2"]}
        /// </summary>
        private static string ConvertPairToNodeJson(string pair)
        {
            int colonIndex = pair.IndexOf(':');
            if (colonIndex == -1)
                return "";
            
            string key = pair.Substring(0, colonIndex).Trim();
            string value = pair.Substring(colonIndex + 1).Trim();
            
            return "{\"nodeName\":" + key + ",\"neighbours\":" + value + "}";
        }
        
        /// <summary>
        /// Loads a MatrixGraph from a JSON file using Unity's Resources folder
        /// </summary>
        /// <param name="resourcePath">Path to the JSON file in Resources folder (without extension)</param>
        /// <returns>A MatrixGraph with all nodes and edges from the JSON file</returns>
        public static TerritoryGraph LoadFromResourcesJson(string resourcePath)
        {
            TextAsset jsonFile = Resources.Load<TextAsset>(resourcePath);
            if (jsonFile == null)
            {
                Debug.LogError($"Could not load JSON file from Resources: {resourcePath}");
                return new TerritoryGraph();
            }
            
            return LoadFromJson(jsonFile.text);
        }
    }
}
