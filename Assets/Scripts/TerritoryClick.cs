// ===============================
// STEP 4: CLICK SELECTION
// ===============================
using UnityEngine.EventSystems;
using UnityEngine;

public class TerritoryClick : MonoBehaviour
{
    Territory territory;


    void Start()
    {
        territory = GetComponent<Territory>();
    }


    void OnMouseDown()
    {
        GameManager gm = GameManager.Instance;
        Debug.Log("on mouse down territory");


        if (gm.selectedFrom == null)
        {
            if (territory.owner == gm.players[gm.currentPlayerIndex].color)
            gm.selectedFrom = territory;
            Debug.Log("Selected territory: " + gm.selectedFrom);
        }
        else
        {
            gm.selectedTo = territory;
            CombatSystem.ResolveAttack(gm.selectedFrom, gm.selectedTo);
            gm.selectedFrom = null;
            gm.selectedTo = null;
        }   
    }
}