using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster
{
    public delegate void GameEventNoParam();
    public static GameEventNoParam onNotifyRescuedPeople;
    public static GameEventNoParam onNotifyRescuedPeopleOnAir;
    public static GameEventNoParam onNotifyRescuedPeopleDie;
    public static GameEventNoParam JoinGame;

}
