// ===============================
// STEP 2: PLAYER CLASS
// ===============================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[System.Serializable]
public class Player
{
    public PlayerColor color;
    public int availableTroops;

    private Dictionary<string,float>_localTroopDistributionQuality = new Dictionary<string, float>();

    public Dictionary<string,float> LocalTroopDistributionQuality => _localTroopDistributionQuality;

    private Dictionary<string,float>_localTroopStrength = new Dictionary<string, float>();
    public Dictionary<string,float> LocalTroopStrength => _localTroopStrength;

    public Dictionary<string, float> _ownedTerritoryRatio = new Dictionary<string, float>();
    public Dictionary<string,float> OwnedTerritoryRatio => _ownedTerritoryRatio;

    [SerializeField]
    private Dictionary<string, bool> _conqueringImminent = new Dictionary<string, bool>();

    private float _globalTroopDistributionQuality = 0f;
    public float GlobalTroopDistributionQuality => _globalTroopDistributionQuality;
    private float _globalTroopStrength = 0f;
    public float GlobalTroopStrength => _globalTroopStrength;
    private float _playerStrength = 0f;
    public float PlayerStrength => _playerStrength;

    public Player(PlayerColor c)
    {
        color = c;
        availableTroops = 0;
    }

    public virtual void StartTurn()
    {
        //int owned = GameManager.Instance.Continents.Count(t => t.owner == color);
        
        //Calculate number of available troops
        var ownedTerritories = GameManager.Instance.Territories.FindAll(t => t.owner == color);
        availableTroops = Mathf.Max(3, ownedTerritories.Count / 3);
        Debug.Log(color + " turn. Troops: " + availableTroops);

        //calculate troop distribution quality per continent
        CalculateGlobalTroopDistributionQuality();
        CalculateGlobalTroopStrength();
        GetOwnedCountriesPerContinent();
        _playerStrength = _globalTroopStrength * _globalTroopDistributionQuality;
        //DisplayPlayerStrength();
    }

    public void GetOwnedCountriesPerContinent()
    {
        foreach(var continent in GameManager.Instance.Continents)
        {
            var ownedTerritories = 0;
            foreach(var territory in continent.Value)
            {
                if(territory.owner == color)
                {
                    ownedTerritories++;
                }
            }
            _ownedTerritoryRatio[continent.Key] = ownedTerritories/GameManager.Instance.Continents[continent.Key].Count;
            if(ownedTerritories == GameManager.Instance.Continents[continent.Key].Count-1)
            {
                _conqueringImminent[continent.Key] = true;
            }
            else
            {
                _conqueringImminent[continent.Key] = false;
            }
        }
    }

    public bool ConqueringImminent(Territory t)
    {
        return _conqueringImminent[t.Continent];
    }

    public void CalculateGlobalTroopDistributionQuality()
    {
        int occupiedContinents = 0;

        foreach(var continent in GameManager.Instance.Continents)
        {
            var localTroopDistributionQuality = 0f;
            var territoryCount = (continent.Value as List<Territory>).Count;
            var troopsInContinent = GetTroopCount(continent.Value);
            int idealTroopsPerTerritory = troopsInContinent/territoryCount;
            float referenceQuality = idealTroopsPerTerritory/(float)troopsInContinent;

            foreach(var territory in continent.Value)
            {
                if(territory.owner == color)
                    localTroopDistributionQuality += referenceQuality - (Mathf.Abs(territory.Troops-idealTroopsPerTerritory)/(float)idealTroopsPerTerritory*referenceQuality);
            }

            _localTroopDistributionQuality[continent.Key] = localTroopDistributionQuality;

            _globalTroopDistributionQuality += localTroopDistributionQuality;
            if(localTroopDistributionQuality > 0.0f)
            {
                occupiedContinents++;
            }
        }
        _globalTroopDistributionQuality *= occupiedContinents/(float)GameManager.Instance.Continents.Count;

    }

    public static int GetTroopCount(List<Territory> continent, PlayerColor color = PlayerColor.All)
    {
        var troopsInContinent = 0;

        foreach(var territory in continent)
        {
            if(color == PlayerColor.All || (territory.owner == color))
                troopsInContinent += territory.Troops;
        }
        return troopsInContinent;
    }

    public void CalculateGlobalTroopStrength()
    {
        int occupiedContinents = 0;
        int totalTroopCount = 0;
        foreach(var continent in GameManager.Instance.Continents)
        {
            var allTroopsInContinent = GetTroopCount(continent.Value);
            var myTroopsInContinent = GetTroopCount(continent.Value, color);
            totalTroopCount += myTroopsInContinent;

            _localTroopStrength[continent.Key] = myTroopsInContinent/(float)allTroopsInContinent;
            _globalTroopStrength += _localTroopStrength[continent.Key];
            if(_localTroopStrength[continent.Key] > 0.0f)
            {
                occupiedContinents++;
            }

        }

        _globalTroopStrength = totalTroopCount/(float)GameManager.Instance.GetTotalTroopCount();

    }


    public void DisplayPlayerStrength()
    {
        string result = "Local Troop Distribution Quality:\n";

        foreach(var val in _localTroopDistributionQuality)
        {
            result +=val.Key + ": " + val.Value + "\n";
        }

        result += "Global Troop Distribution Quality: " + _globalTroopDistributionQuality + "\n\n";
        
        result += "Local Troop Strength:\n";

        foreach(var val in _localTroopStrength)
        {
            result +=val.Key + ": " + val.Value + "\n";
        }
        result += "Global Troop Strength " + _globalTroopStrength + "\n\n";

        Debug.Log(result);

    }

    public void Reset()
    {

        _localTroopDistributionQuality.Clear();
        _localTroopStrength.Clear();
        _globalTroopDistributionQuality = 0f;
        _globalTroopStrength = 0f;
    }
}  