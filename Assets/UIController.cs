using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : Singleton<UIController>
{

    public WinComponent winUI;
    public LoseComponent loseUI;

    public Text levelName;
    public void OnShowWinUI()
    {
        winUI.OnShow();
    }
    public void OnShowLoseUI()
    {
        loseUI.OnShow();
    }

    public void SetLevelName(int level)
    {
        levelName.text = "LEVEL " + level;
    }


    public void OnReplayGame()
    {
        PoolingSystem.Instance.ResetPool();
        GameManager.Instance.State = STATE.NONE;
        SetOffAllUI();
        GameManager.Instance.ReplayGame();
    }

    public void OnPauseGame()
    {

    }

    public void OnNextGame()
    {
        PoolingSystem.Instance.ResetPool();
        Utils.LEVEL++;
        GameMaster.JoinGame?.Invoke();
    }

    internal void SetOffAllUI()
    {
        winUI.OnClose();
        loseUI.OnClose();
    }
}
