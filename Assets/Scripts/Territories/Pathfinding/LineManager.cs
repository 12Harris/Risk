using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public struct Line
{
    public Vector3 start;
    public Vector3 end;
    public Color color;
    public Line(Vector3 s, Vector3 e, Color c)
    {
        start = s;
        end = e;
        color = c;
    }
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LineManager : MonoBehaviour
{
    public static LineManager Instance;

    private List<Line> lines = new List<Line>();

    private List<int> _indices = new List<int>();

    private List<Vector3> _visited = new List<Vector3>();

    [SerializeField]
    private Material _lineMaterial;
    private Mesh _mesh;


    void Awake()
    {
        Instance = this;
    }

    public void AddLine(Vector3 start, Vector3 end, Color color)
    {
        lines.Add(new Line(start, end, color));
        //_indices.Add(lines.Count -2);
        //_indices.Add(lines.Count -1);

        if(!_visited.Contains(start))
            _visited.Add(start);
        
        if(!_visited.Contains(end))
            _visited.Add(end);

        _indices.Add(GetIndex(start));
        _indices.Add(GetIndex(end));

    }

    public int GetIndex(Vector3 v)
    {
        int foundIndex = -1;
        foreach(var vec in _visited)
        {
            foundIndex++;
            if(vec == v)
            {
                break;
            }
        }
        return foundIndex;
    }

    public void GenerateMesh()
    {
        _mesh = new Mesh();

        _mesh.vertices = GetLineVertices();
        _mesh.SetIndices(_indices.ToArray(), MeshTopology.Lines, 0);

        GetComponent<MeshFilter>().mesh = _mesh;
        GetComponent<MeshRenderer>().material = _lineMaterial;
    }

    public List<Line> GetLines()
    {
        return lines;
    }

    public Vector3[] GetLineVertices()
    {
        int count = _visited.Count;
        List<Vector3> result = new List<Vector3>();
        for(int i = 0; i < count; i++)
        {
           result.Add(_visited[i]);
        }
        return result.ToArray();
    }

    public void ClearLines()
    {
        lines.Clear();
    }
}