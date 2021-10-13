using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster
{
    public delegate void GameEventNoParam();
    public static GameEventNoParam onNotifyRescuedPeople;
}
