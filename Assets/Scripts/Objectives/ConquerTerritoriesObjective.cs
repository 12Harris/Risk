using UnityEngine;
public class ConquerTerritoriesObjective : Objective
{

    public override bool IsRequired(Territory t)
    {
        return false;
    }

    public override float GetDistanceScore(Territory t)
    {
        return 0;
    }


}