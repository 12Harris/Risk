// ===============================
// STEP 3: GAME MANAGER (TURNS)
// ===============================
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using DataStructures;
using Utilities;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    public List<Player> players = new List<Player>();
    public int currentPlayerIndex = 0;


    public Territory selectedFrom;
    public Territory selectedTo;

    [SerializeField]
    private TextAsset _worldRiskJsonFile;
    
    private TerritoryGraph _worldGraph;

    public TerritoryGraph WorldGraph => _worldGraph;
    
    private Dictionary<string,List<Territory>> _continents = new Dictionary<string, List<Territory>>();
    public Dictionary<string, List<Territory>> Continents => _continents;
    
    private List<Territory> _territories = new List<Territory>();
    public List<Territory> Territories => _territories;

    [SerializeField]
    private Dictionary<string, float> _continentBonus = new Dictionary<string, float>();
    public Dictionary<string, float> ContinentBonus => _continentBonus;


    void Awake()
    {
        Instance = this;
    }

    public void Initialize(bool withAI=false)
    {
        if (withAI)
        {
            players.Add(new AIPlayer(PlayerColor.Red));
        }
        else
        {
            players.Add(new Player(PlayerColor.Red));
        }
        players.Add(new Player(PlayerColor.Blue));

        AssignInitialTerritories();
        LoadGraphFromJson();

        foreach(var t in _territories)
        {
            t.AddNeighbors();
        }

        _continentBonus["NorthAmerica"] = 3;
        _continentBonus["SouthAmerica"] = 2;
        _continentBonus["Africa"] = 2;
        _continentBonus["Europe"] = 2;
        _continentBonus["Asia"] = 3;
        _continentBonus["Australia"] = 2;

        PrintGraphInfo();
        StartTurn();  
    }

    void Start()
    {
        Initialize();
    }

    public void AddToContinent(string continent, Territory territory)
    {
        if(!_continents.ContainsKey(continent))
        {
            _continents[continent] = new List<Territory>();
        }
        _continents[continent].Add(territory);
        //foreach(var t in _continents[continent])
            //Debug.Log("TERRITORIES: " +t);

    }

    void AssignInitialTerritories()
    {
        Territory[] all = FindObjectsOfType<Territory>();
        int i = 0;
        foreach (var t in all)
        {
            t.owner = players[i % players.Count].color;
            t.troops = 1;
            i++;
        }
    }

    public Territory GetTerritory(string name)
    {
        return Territories.Find(x => x.territoryName == name);
    }

    void StartTurn()
    {
        Player p = players[currentPlayerIndex];
        p.StartTurn();
    }


    public void EndTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        StartTurn();
    }

    /// <summary>
    /// Loads the graph from the JSON file
    /// </summary>
    private void LoadGraphFromJson()
    {
        if (_worldRiskJsonFile != null)
        {
            // Load from assigned TextAsset
            _worldGraph = GraphJsonLoader.LoadFromJson(_worldRiskJsonFile.text);
            Debug.Log("Graph loaded successfully from TextAsset!");
        }
        else
        {
            // Alternative: Load from Resources folder
            // Place your JSON file in Assets/Resources/
            // worldGraph = GraphJsonLoader.LoadFromResourcesJson("world_risk_20_adjacency");
            
            Debug.LogWarning("No JSON file assigned! Please assign the JSON file in the Inspector.");
        }
    }

    /// <summary>
    /// Prints information about the loaded graph
    /// </summary>
    private void PrintGraphInfo()
    {
        if (_worldGraph == null)
        {
            Debug.LogError("Graph is null!");
            return;
        }
        
        Debug.Log("=== World Risk Graph ===");
        Debug.Log($"Total territories: {_worldGraph.GetAllNodes().Size()}");
        Debug.Log($"Is connected: {_worldGraph.IsConnected()}");
        Debug.Log("\n" + _worldGraph.ToString());
        
        // Example: Get neighbors of a specific territory
        string territory = "USA";
        var neighbors = _worldGraph.GetAllNeighboursOf(territory);
        if (neighbors != null)
        {
            Debug.Log($"\n{territory} borders: {neighbors.ToString()}");
        }
        
        // Example: Check if two territories are connected
        string territory1 = "Brazil";
        string territory2 = "Mexico";
        bool areConnected = _worldGraph.ContainsEdge(territory1, territory2);
        Debug.Log($"\n{territory1} and {territory2} are connected: {areConnected}");
    }
    
    /// <summary>
    /// Example method to find a path between two territories (BFS)
    /// </summary>
    public bool AreTerritoriesConnected(string from, string to)
    {
        if (_worldGraph == null || !_worldGraph.Contains(from) || !_worldGraph.Contains(to))
            return false;
        
        return _worldGraph.IsConnected(); // Since it's one connected graph, this works
    }

    public int GetTotalTroopCount()
    {
        int sum = 0;
        foreach(var t in Territories)
        {
            sum += t.troops;
        }
        return sum;
    }
}