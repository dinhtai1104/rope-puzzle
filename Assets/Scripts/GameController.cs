using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WrappingRope;
using Random = UnityEngine.Random;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameController : Singleton<GameController>
{

    [Space(10)]
    public Transform house, car;
    public LayerMask layerRing;
    private List<PersonController> listPerson;
    private int indexPerson = 0;



    [Header("For Data")]
    public int allPeople;
    public int targetRescue;


    [Space(5)]
    [Header("For UI")]
    public Text rescuedText;
    private int rescuedPeople;

    [Space(5)]
    [Header("In Game")]
    public Transform houseLeft;
    public Transform houseRight;
    public Transform carLeft;
    public Transform carRight;
    private void Start()
    {
        indexPerson = 0;
        listPerson = new List<PersonController>();
        rescuedText.text = rescuedPeople.ToString("0") + "/" + targetRescue.ToString("00");
        GameMaster.onNotifyRescuedPeople += this.HandleOnNotifyRescuedPeople;
        for (int i = 0; i < allPeople; i++)
        {
            PersonController person = PoolingSystem.Instance.GetPerson();
            person.transform.position = new Vector3(Random.Range(houseLeft.position.x, houseRight.position.x), houseLeft.position.y, 0);
            person.InitPerson(houseLeft.position, houseRight.position, carLeft.position, carRight.position);
            person.gameObject.SetActive(true);
            listPerson.Add(person);
        }
    }

    private void HandleOnNotifyRescuedPeople()
    {
        rescuedPeople++;
        rescuedText.text = rescuedPeople.ToString("00") + "/" + targetRescue.ToString("00");
        rescuedText.rectTransform.DOScale(Vector3.one * 1.1f, 0.1f).OnComplete(() =>
        {
            rescuedText.rectTransform.DOScale(Vector3.one, 0.1f);
        });
    }

    public int GetDirection()
    {
        if (car.localPosition.x > 0)
        {
            return -1;
        }
        return 1;
    }
    public bool IsFinish()
    {
        if (indexPerson - 1 >= 0)
        {
            return listPerson[indexPerson - 1].IsFinish();
        } else
        {
            return true;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hitRingRC = Physics2D.Raycast(mPos, Vector3.zero, Mathf.Infinity, layerRing);
            if (hitRingRC.transform == null) {
                if (RingMovement.Instance.IsConnected())
                {
                    if (indexPerson < listPerson.Count)
                    {
                        listPerson[indexPerson].StartZipline(RopeSystem.Instance.getPath());
                        indexPerson++;
                    }
                }
            }
        }
    }

    public override void OnDestroy()
    {
        GameMaster.onNotifyRescuedPeople -= this.HandleOnNotifyRescuedPeople;
        base.OnDestroy();
    }

#if UNITY_EDITOR
    [ButtonEditor]
    public void SaveLevel()
    {
        PrefabUtility.SaveAsPrefabAsset(gameObject, "Assets/Resources/Level/" + gameObject.name + ".prefab");
        Debug.Log("Save Successful: " + gameObject.name);
    }
#endif
}
