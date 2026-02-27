using UnityEngine;

public abstract class Objective
{
    
    public abstract bool IsRequired(Territory t);

    public abstract float GetDistanceScore(Territory t);

}
