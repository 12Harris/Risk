using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIPlayer : Player
{
    private List<(Territory, Territory)> _possibleAttacks = new List<(Territory,Territory)>();
    public List<(Territory,Territory)> PossibleAttacks => _possibleAttacks;

    //0 = continental influence
    //1 = objective importance
    //2 = required for objecive
    //3 = troop ratio 
    private float[] _deploymentInfluences = new float[4];

    public float[] DeploymentInfluences => _deploymentInfluences;

    private Objective _objective;
    public Objective Objective => _objective;   
    private int MAX_TERRITORY_NEIGHBOURS = 4;


    public AIPlayer(PlayerColor c) :base(c)
    {

    }

    public void SetObjective(Objective objective)
    {
        _objective = objective;
    }

    private void AnalyzeMap()
    {
        _possibleAttacks.Clear();

        foreach(var territory in GameManager.Instance.Territories)
        {
            if(territory.owner == color && territory.troops > 3)
            {
                var neighbors = territory.Neighbors;
                foreach(var neighbour in neighbors)
                {
                    if(neighbour.owner != color)
                    {
                        _possibleAttacks.Add((territory,neighbour));
                        float[] deploymen = CalculateDeploymentInfluence(territory, neighbour);
                    }
                }
            }
        }
    }

    public void CalculateDeploymentInfluences()
    {
        
    }

    private float[] CalculateDeploymentInfluence(Territory from, Territory to)
    {
        float[] deploymentInfluence = new float[3];
        var ownedContinentRatio = LocalTroopStrength[from.Continent] * OwnedTerritoryRatio[from.Continent];
        var continentBonus = 1.0f;
        var borderCountryMultiplier = to.IsBorderTerritory ? 1.5f : 1.0f;
        if(ConqueringImminent(to))
            continentBonus = 1.5f;
        float connectivity_score = to.Neighbors.Count / MAX_TERRITORY_NEIGHBOURS;
        float bonus_connectivity_multiplier = to.ConnectsMultipleContinents() ? 1.0f : 1.5f;
        connectivity_score *= bonus_connectivity_multiplier;
        float troopRatio = from.troops/from.troops+to.troops;
        float troopRatioWeight = 1 - troopRatio;
        float defensive_score = 1 / to.Neighbors.Count;
     
        float base_importance = _objective.GetDistanceScore(to) * ownedContinentRatio * borderCountryMultiplier * continentBonus * troopRatioWeight * 10;
        
        deploymentInfluence[0] = base_importance;
        deploymentInfluence[1] = connectivity_score;
        deploymentInfluence[2] = defensive_score;

        return deploymentInfluence;
    }


    public override void StartTurn()
    {
        base.StartTurn();
        AnalyzeMap();
    }
}
