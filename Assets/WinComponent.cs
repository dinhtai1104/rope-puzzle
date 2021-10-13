using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WinComponent : MonoBehaviour
{
    public RectTransform[] rects;
    public void OnShow()
    {
        PoolingSystem.Instance.ResetPool();
        gameObject.SetActive(true);
        Sequence sq = DOTween.Sequence();
        for (int i = 0; i < rects.Length; i++)
        {
            Vector3 pos = rects[i].anchoredPosition;
            rects[i].anchoredPosition = new Vector3(-Screen.width, pos.y);
            sq.Append(rects[i].DOAnchorPos(pos, 0.25f).From(new Vector3(-Screen.width, pos.y)));
        }
    }   
    public void OnClose()
    {
        gameObject.SetActive(false);
    }

    public void OnReplayClicked()
    {
        UIController.Instance.OnReplayGame();
    }

    public void OnNextLevelClicked()
    {
        UIController.Instance.OnNextGame();
    }
}
