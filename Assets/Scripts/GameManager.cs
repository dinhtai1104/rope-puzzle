using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public enum STATE { WIN, LOSE, PLAYING, NONE }
public class GameManager : Singleton<GameManager>
{
    public static int TotalLevel = 1;
    public int totalLevel = 100;
    private GameController levelCurrent, gameController;

    private STATE state = STATE.PLAYING;
    public STATE State
    {
        get
        {
            return state;
        }
        set
        {
            STATE last = state;
            state = value;
            if (state == STATE.NONE)
            {
                transform.DOKill();
            }
            if (last == STATE.PLAYING && state != STATE.PLAYING)
            {
                if (state == STATE.WIN && last == STATE.PLAYING)
                {
                    transform.DOScale(Vector3.one, 1.8f).OnComplete(() =>
                    {
                        UIController.Instance.OnShowWinUI();
                    });
                }
                else
                {
                    if (state == STATE.LOSE && last == STATE.PLAYING)
                    {
                        transform.DOScale(Vector3.one, 1.8f).OnComplete(() =>
                        {
                            //UIController.Instance.loseComponent.OnShow();
                            UIController.Instance.OnShowLoseUI();

                        });
                    }
                }
            }
        }
    }

    //Caching

    private void Start()
    {
        TotalLevel = totalLevel;
        LoadLevel();
        GameMaster.JoinGame += this.JoinGameHandleEvent;
    }

    private void JoinGameHandleEvent()
    {
        LoadLevel();
        UIController.Instance.SetOffAllUI();
    }

    public void QuitLevel()
    {
        if (gameController)
        {
            Destroy(gameController.gameObject);
        }
    }

    public void LoadLevel()
    {
        StopAllCoroutines();
        StartCoroutine(loadLevel());
    }

    private IEnumerator loadLevel()
    {
        State = STATE.PLAYING;
        UIController.Instance.SetLevelName(Utils.LEVEL);

        if (gameController)
        {
            Destroy(gameController.gameObject);
        }
        yield return null;


        levelCurrent = null;

        levelCurrent = Resources.Load<GameController>("Level/Level " + Utils.LEVEL);


        gameController = Instantiate(levelCurrent);

        
    }

    public void ReplayGame()
    {
        StopAllCoroutines();
        State = STATE.PLAYING;
        if (gameController)
        {
            Destroy(gameController.gameObject);
        }
        
        gameController = Instantiate(levelCurrent);
        UIController.Instance.SetLevelName(Utils.LEVEL);

    }

}

