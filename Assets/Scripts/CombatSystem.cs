// ===============================
// STEP 5: COMBAT SYSTEM (DICE)
// ===============================
using System;
using UnityEngine;

public static class CombatSystem
{
    public static void ResolveAttack(Territory from, Territory to)
    {
        if (!from.neighbors.Contains(to)) return;
        if (from.troops < 2) return;
        if (from.owner == to.owner) return;


        int attackDice = Mathf.Min(3, from.troops - 1);
        int defendDice = Mathf.Min(2, to.troops);


        int[] attackRolls = RollDice(attackDice);
        int[] defendRolls = RollDice(defendDice);


        Array.Sort(attackRolls);
        Array.Sort(defendRolls);
        Array.Reverse(attackRolls);
        Array.Reverse(defendRolls);


        int comparisons = Mathf.Min(attackDice, defendDice);
        for (int i = 0; i < comparisons; i++)
        {
            if (attackRolls[i] > defendRolls[i])
                to.RemoveTroops(1);
            else
                from.RemoveTroops(1);
        }


        if (to.troops <= 0)
        {
            to.owner = from.owner;
            to.troops = 1;
            from.RemoveTroops(1);
        }
    }


    static int[] RollDice(int count)
    {
        int[] rolls = new int[count];
        for (int i = 0; i < count; i++)
        rolls[i] = UnityEngine.Random.Range(1, 7);
        return rolls;
    }
}


// ===============================
// STEP 6: WHAT TO ADD NEXT
// ===============================
// - UI (TextMeshPro) for troop counts
// - Reinforcement phase UI
// - Fortify phase
// - AI player
// - Win condition
// - Continent bonuses