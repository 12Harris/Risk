using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;
using DataStructures;
using Utilities;
using System.Reflection;
using UnityEngine.SceneManagement;
using System.Linq;
/*
Assert.AreEqual(expected, actual);
Assert.IsTrue(condition);
Assert.IsFalse(condition);
Assert.IsNull(value);
Assert.IsNotNull(value);
Assert.Throws<System.Exception>(() => method());
*/

public class PlayerTests
{
    [UnityTest]
    public IEnumerator BlueOccupiesNAmerica_Test()
    {

        SceneManager.LoadScene("SampleScene");

        yield return null; // wait for scene load
        yield return new WaitForSeconds(0.2f);

        Player playerRed = GameManager.Instance.players[0];
        Player playerBlue = GameManager.Instance.players[1];
        

        //Test 1
        ResetTerritories();
        Assert.IsTrue(AllTerritoriesBelongTo(PlayerColor.Red));
        
        //Test 2 player blue owns north america
        AssignContinent(GameManager.Instance.Continents["NorthAmerica"], PlayerColor.Blue);
        playerBlue.StartTurn();
        Assert.IsTrue(playerBlue.LocalTroopDistributionQuality["NorthAmerica"] == 1f);
        Assert.AreEqual( 1/6.0, playerBlue.GlobalTroopDistributionQuality,0.0001f);
        Assert.AreEqual( 1.0, playerBlue.LocalTroopStrength["NorthAmerica"],0.0001f);
        Assert.AreEqual( 4/20.0, playerBlue.GlobalTroopStrength,0.0001f);

        ResetTerritories();

        //Test 3 player blue only one troop
        AssignTerritory("Alaska", PlayerColor.Blue);
        playerBlue.StartTurn();
        Assert.IsTrue(playerBlue.LocalTroopDistributionQuality["NorthAmerica"] == 0.25f);
        Assert.AreEqual( 0.25/6.0, playerBlue.GlobalTroopDistributionQuality,0.0001f);
        Assert.AreEqual( 0.25, playerBlue.LocalTroopStrength["NorthAmerica"],0.0001f);
        Assert.AreEqual( 1/20.0, playerBlue.GlobalTroopStrength,0.0001f);
    }

    [UnityTest]
    public IEnumerator BlueEvenlyDistributed1_Test()
    {
        SceneManager.LoadScene("SampleScene");

        yield return null; // wait for scene load
        yield return new WaitForSeconds(0.2f);

        GameManager.Instance.Initialize();


        Player playerRed = GameManager.Instance.players[0];
        Player playerBlue = GameManager.Instance.players[1];
        

        ResetTerritories();

        AssignTerritory("Alaska", PlayerColor.Blue);
        AssignTerritory("Brazil", PlayerColor.Blue);
        AssignTerritory("UK", PlayerColor.Blue);
        AssignTerritory("C Africa", PlayerColor.Blue);
        AssignTerritory("SE Asia", PlayerColor.Blue);
        AssignTerritory("Indonesia", PlayerColor.Blue);
        playerBlue.StartTurn();

        Assert.IsTrue(playerBlue.LocalTroopDistributionQuality["NorthAmerica"] == 0.25f);
        Assert.AreEqual( 0.25, playerBlue.LocalTroopStrength["NorthAmerica"],0.0001f);

        Assert.AreEqual( 0.33333333333333f, playerBlue.LocalTroopDistributionQuality["SouthAmerica"]);
        Assert.AreEqual( 0.33333333333333f, playerBlue.LocalTroopStrength["SouthAmerica"],0.0001f);

        Assert.AreEqual( 0.33333333333f, playerBlue.LocalTroopDistributionQuality["Europe"],0.0001f);
        Assert.AreEqual( 0.33333333333f, playerBlue.LocalTroopStrength["Europe"],0.0001f);

        Assert.AreEqual( 0.3333333333f, playerBlue.LocalTroopDistributionQuality["Africa"],0.0001f);
        Assert.AreEqual( 0.3333333333f, playerBlue.LocalTroopStrength["Africa"],0.0001f);

        Assert.AreEqual( 0.25f, playerBlue.LocalTroopDistributionQuality["Asia"],0.0001f);
        Assert.AreEqual( 0.25f, playerBlue.LocalTroopStrength["Asia"],0.0001f);

        Assert.AreEqual( 0.333333333f, playerBlue.LocalTroopDistributionQuality["Oceania"],0.0001f);
        Assert.AreEqual( 0.333333333f, playerBlue.LocalTroopStrength["Oceania"],0.0001f);

        Assert.AreEqual( 1.833333f, playerBlue.GlobalTroopDistributionQuality,0.0001f);
        Assert.AreEqual( 6/20.0, playerBlue.GlobalTroopStrength,0.0001f);
    }


    [UnityTest]
    public IEnumerator AI_AnalyzeMap_Test()
    {
        SceneManager.LoadScene("SampleScene");

        yield return null; // wait for scene load
        yield return new WaitForSeconds(0.2f);

        GameManager.Instance.Initialize(true);

        Player playerRed = GameManager.Instance.players[0];
        Player playerBlue = GameManager.Instance.players[1];

        //Test 1
        ResetTerritories();
        AssignTerritory("Alaska", PlayerColor.Blue);
        GameManager.Instance.GetTerritory("Canada").troops = 5;
        playerRed.StartTurn();
        Assert.IsTrue((playerRed as AIPlayer).PossibleAttacks.Count == 1);

        //Test 2
        ResetTerritories();
        AssignTerritory("Brazil", PlayerColor.Blue);
        GameManager.Instance.GetTerritory("Mexico").troops = 5;
        GameManager.Instance.GetTerritory("Peru").troops = 5;
        GameManager.Instance.GetTerritory("Argentina").troops = 5;
        GameManager.Instance.GetTerritory("NorthAfrica").troops = 5;
        playerRed.StartTurn();
        Assert.IsTrue((playerRed as AIPlayer).PossibleAttacks.Count == 4);
    }

    [UnityTest]
    public IEnumerator ShortestTerritoryPath_Test()
    {
        SceneManager.LoadScene("SampleScene");

        yield return null; // wait for scene load
        yield return new WaitForSeconds(0.2f);

        //GameManager.Instance.Initialize(true);


        var expected = new List<string> {"Alaska", "Canada"};
        var actual = Pathfinding.BFS("Alaska", "Canada");

        CollectionAssert.AreEqual(expected, actual);

        expected = new List<string> {"Alaska", "Canada", "USA", "Mexico", "Brazil", "Argentina"};
        actual = Pathfinding.BFS("Alaska", "Argentina");

        CollectionAssert.AreEqual(expected, actual);
    }


    [UnityTest]
    public IEnumerator MinRequiredDistance_Test()
    {
        SceneManager.LoadScene("SampleScene");

        yield return null; // wait for scene load
        yield return new WaitForSeconds(0.2f);

        AIPlayer ai = new AIPlayer(PlayerColor.Red);
        Objective objective = new ConquerContinentObjective(new List<string>{"Europe", "Oceania"});
        ai.SetObjective(objective);

        var minRequired = (objective as ConquerContinentObjective).GetMinRequiredDistance();
        Assert.IsTrue(minRequired == 18,"min required distance " + minRequired);

        var maxRequired = (objective as ConquerContinentObjective).GetMaxRequiredDistance();
        Assert.IsTrue(maxRequired == 7,"max required distance " + maxRequired);

        Assert.IsTrue(objective.GetDistanceScore(GameManager.Instance.Territories.Find(x => x.territoryName == "Alaska")) < 0.01f, "required: " + 
                    objective.GetDistanceScore(GameManager.Instance.Territories.Find(x => x.territoryName == "Alaska")));

        Assert.IsTrue(objective.GetDistanceScore(GameManager.Instance.Territories.Find(x => x.territoryName == "MiddleEast")) > 0.99f);
        Assert.IsTrue(objective.GetDistanceScore(GameManager.Instance.Territories.Find(x => x.territoryName == "SoutheastAsia")) > 0.99f);


        Assert.AreEqual( 0.5454f,objective.GetDistanceScore(GameManager.Instance.Territories.Find(x => x.territoryName == "Argentina")) ,0.0001f);
        Assert.AreEqual( 0.90909f,objective.GetDistanceScore(GameManager.Instance.Territories.Find(x => x.territoryName == "NorthAfrica")) ,0.00001f);

    }

    private void AssignTerritory(string name, PlayerColor color)
    {
        GameManager.Instance.Territories.Find(x => x.territoryName == name).owner = color;
    }

    private void AssignContinent(List<Territory> continent, PlayerColor color)
    {
        foreach(var t in continent)
        {
            t.owner = color;
        }
    }

    private void ResetTerritories()
    {
        Player playerRed = GameManager.Instance.players[0];
        Player playerBlue = GameManager.Instance.players[1];

        foreach(var t in GameManager.Instance.Territories)
        {
            t.owner = PlayerColor.Red;
            t.troops = 1;
        }
        playerBlue.Reset();
    }

    private bool AllTerritoriesBelongTo(PlayerColor color)
    {
        bool result = true;
        for(int i = 0; i < GameManager.Instance.Territories.Count && result; i++)
        {
            if(GameManager.Instance.Territories[i].owner != color)
            {
                result = false;
            }
        }
        return result;
    }
}