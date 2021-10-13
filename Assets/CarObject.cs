using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarObject : MonoBehaviour
{
    public Animator myAnimator;
    // Start is called before the first frame update
    void Start()
    {
        GameMaster.onNotifyRescuedPeople += this.HandleOnNotifyRescuedPeople;
    }

    private void HandleOnNotifyRescuedPeople()
    {
        myAnimator.SetTrigger("rescue");
    }

    private void OnDestroy()
    {
        GameMaster.onNotifyRescuedPeople -= this.HandleOnNotifyRescuedPeople;
    }
}
