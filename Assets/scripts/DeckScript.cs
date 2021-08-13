using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckScript : ObjectBase
{
    public bool IsCardsMoving()
    {
        for(int i=0; i<GetNbChildCards(); i++)
        {
            if(GetChild(i).IsMoving())
            {
                return true;
            }
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetGameMaster().IsPaused())
        {
            return;
        }
    }

    void OnGUI()
    {

    }
}
