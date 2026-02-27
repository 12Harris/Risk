using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ConquerContinentObjective : Objective
{


    private List<string> _requiredContinents = new List<string>();
    private List<Territory> _borderCountriesContinentA;
    private List<Territory> _borderCountriesContinentB;

    public ConquerContinentObjective(List<string> requiredContinents)
    {
        _requiredContinents = requiredContinents;
        _borderCountriesContinentA = GameManager.Instance.Continents[requiredContinents[0]].FindAll(x => x.IsBorderTerritory == true);
        _borderCountriesContinentB = GameManager.Instance.Continents[requiredContinents[1]].FindAll(x => x.IsBorderTerritory == true);

    }

    public override bool IsRequired(Territory t)
    {
        return false;
    }

    public float GetMinRequiredDistance()
    {
        if(_requiredContinents.Contains("Europe") && _requiredContinents.Contains("Oceania"))
        {
            return GetRequiredDistance(GameManager.Instance.Territories.Find(x => x.territoryName == "Alaska"));
        }
        return 0;
    }


    public float GetMaxRequiredDistance()
    {
        List<float> weights = new List<float>();
        if(_requiredContinents.Contains("Europe") && _requiredContinents.Contains("Oceania"))
        {
            foreach(var t in GameManager.Instance.Continents["Asia"])
            {
                weights.Add(GetRequiredDistance(t));
            }

            foreach(var w in weights)
            {
                Debug.Log("w: " + w);
            }
        }   
        return weights.Min();
    }

    public float GetRequiredDistance(Territory t)
    {
        var minPath = new List<string>();
        var paths = new List<List<string>>();

        if(_requiredContinents[0] != "" && 
                !GameManager.Instance.Continents[_requiredContinents[0]].Contains(t))
        {
            foreach(var b in  _borderCountriesContinentA)
            {
                var path = Pathfinding.BFS(t.territoryName,b.territoryName);
                if(minPath.Count == 0 || path.Count < minPath.Count)
                {
                    minPath = path;
                }
            }
            paths.Add(minPath);
        }

        if(_requiredContinents[1] != "" && 
                !GameManager.Instance.Continents[_requiredContinents[1]].Contains(t))
        {
            minPath = new List<string>();
            foreach(var b in  _borderCountriesContinentB)
            {
                var path = Pathfinding.BFS(t.territoryName,b.territoryName);
                if(minPath.Count == 0 || path.Count < minPath.Count)
                {
                    minPath = path;
                }
            }
            paths.Add(minPath);
        }

        //Debug.Log("Path 0: " + paths[0] + ", Path1: " + paths[1]);
        int c = 0;
        foreach(var path in paths)
        {
            c += path.Count;
        }
        return c;
    }

    public override float GetDistanceScore(Territory t)
    {
        var requiredDistance = GetRequiredDistance(t);

        var distSubtracted = requiredDistance - GetMaxRequiredDistance();

        float distanceScore = 1-(distSubtracted/(GetMinRequiredDistance()-GetMaxRequiredDistance()));

        return distanceScore;
    }

}