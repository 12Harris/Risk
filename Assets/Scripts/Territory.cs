// ===============================
// STEP 0: PROJECT SETUP (Unity 2D)
// ===============================
// Unity Hub → New Project → 2D Core → Name: RiskClone
// Scenes folder → MainScene
// Scripts folder → create the scripts below

// ===============================
// STEP 1: TERRITORY DATA MODEL
// ===============================
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public enum PlayerColor { Red, Blue, Green, Yellow, All }

[System.Serializable]
public class Territory : MonoBehaviour
{
    public string territoryName;
    public PlayerColor owner;
    public int troops = 1;
    public int Troops => troops;
    public List<Territory> neighbors = new List<Territory>();
    public List<Territory> Neighbors => neighbors;

    [SerializeField]
    private string _continent;
    public string Continent => _continent;

    [SerializeField] private bool _isBorderTerritory;
    public bool IsBorderTerritory => _isBorderTerritory;


    private void Start()
    {
        GameManager.Instance.Territories.Add(this);
        GameManager.Instance.AddToContinent(_continent,this);
    }

    public void AddNeighbors()
    {
        neighbors.Clear();
        var neighbourString = GameManager.Instance.WorldGraph.GetAllNeighboursOf(territoryName);

        var from = transform.position;

        foreach(var territory in GameManager.Instance.Territories)
        {
            if(neighbourString.Contains(territory.territoryName))
            {
                neighbors.Add(territory);
            }
        }

        foreach(var neighbour in neighbors)
        {
            var to = neighbour.transform.position;

            LineManager.Instance.AddLine(from, to, Color.green);
        }

        LineManager.Instance.GenerateMesh();
    }

    /*public void DrawConnections()
    {
        Debug.Log("draw conn");
        var from = transform.position;

        _lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);

        foreach(var neighbour in neighbors)
        {
            var to = neighbour.transform.position;

            LineManager.Instance.AddLine(from, to, Color.green);
        }

        GL.End();
            
    }*/


    /*void OnRenderObject()
    {
        if(neighbors.Count == 0)
            return;

        if (Camera.current != Camera.main)
            return;
        DrawConnections();
    }*/

    public bool ConnectsMultipleContinents()
    {
        List<string> _connectedContinents = new List<string>();
        foreach(var neighbour in Neighbors)
        {
            if(!_connectedContinents.Contains(neighbour.Continent))
            {
                _connectedContinents.Add(neighbour.Continent);
            }
        }
        return _connectedContinents.Count > 1;
    }

    public void AddTroops(int amount)
    {
        troops += amount;
    }

    public void RemoveTroops(int amount)
    {
        troops = Mathf.Max(0, troops - amount);
    }
}
