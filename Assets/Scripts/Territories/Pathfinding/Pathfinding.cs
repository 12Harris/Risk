using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;
using DataStructures.StringLists;
public class Pathfinding
{
    public static List<string> BFS(string start, string target)
    {
        /*var queue = new Queue<int>();
        var visited = new HashSet<int>();
        var parent = new Dictionary<int, int?>();*/
        var queue = new StringList();
        var visited = new HashSet<string>();
        var parent = new Dictionary<string, string?>();

        queue.Append(start);
        visited.Add(start);
        parent[start] = null;

        while (!queue.IsEmpty())
        {
            var current = queue.GetAt(0);
            queue.Remove(current);

            if (current == target)
                break;


            StringList neighbours = GameManager.Instance.WorldGraph.GetAllNeighboursOf(current);
            for (int i = 0; i < neighbours.Size(); i++)
            {
                var neighbour = neighbours.GetAt(i);
                if(visited.Contains(neighbour))
                    continue;
                
                visited.Add(neighbour);
                parent[neighbour] = current;
                queue.Append(neighbour);
                
            }
        }

        return ReconstructPath(parent, start, target);
    }

    static List<string> ReconstructPath(
        Dictionary<string, string?> parent,
        string start,
        string target)
    {
        var path = new List<string>();
        string? current = target;

        while (current != null)
        {
            path.Add(current);
            current = parent[current];
        }

        path.Reverse();

        return path.Count > 0 && path[0] == start ? path : null;
    }

}