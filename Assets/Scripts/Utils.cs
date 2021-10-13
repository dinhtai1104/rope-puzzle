using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    const string GAME = "GAME";
    const string LEVEL_KEY = "LEVEL_KEY";
    const string COIN_KEY = "COIN_KEY";

    private static int levelMax = 1;
    public static int LEVEL_MAX
    {
        get
        {
            return levelMax;
        }
        set
        {
            levelMax = value;
            PlayerPrefs.SetInt(LEVEL_KEY, levelMax);
        }
    }
    private static int levelCurrent = 1;
    public static int LEVEL
    {
        get
        {
            return levelCurrent;
        } 
        set
        {
            levelCurrent = value;
            if (levelCurrent > levelMax)
            {
                LEVEL_MAX = levelCurrent;
            }
        }
    }

    public static void LoadData()
    {
        if (!PlayerPrefs.HasKey(GAME))
        {
            PlayerPrefs.SetInt(GAME, 1);
            PlayerPrefs.SetInt(LEVEL_KEY, 1);
        }
        LEVEL = PlayerPrefs.GetInt(LEVEL_KEY);
    }
}
