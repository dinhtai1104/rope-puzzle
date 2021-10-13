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
public enum GameMode { Rescue, Normal }

public class GameController : MonoBehaviour
{
    public static PersonController lastSwing;
    public static int Direction = 0;
    public GameMode gameMode = GameMode.Normal;
    public int peopleOnAir = 0;
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
    public RingMovement ring;

    private int peopleOnSwing = 0;
    private void Start()
    {
        lastSwing = null;
        Direction = GetDirection();
        indexPerson = 0;
        listPerson = new List<PersonController>();
        rescuedText.text = rescuedPeople.ToString("0") + "/" + targetRescue.ToString("00");
        GameMaster.onNotifyRescuedPeople += this.HandleOnNotifyRescuedPeople;
        GameMaster.onNotifyRescuedPeopleOnAir += this.HandleOnNotifyRescuedPeopleOnAir;
        GameMaster.onNotifyRescuedPeopleDie += this.HandleOnNotifyRescuedPeopleDie;
        for (int i = 0; i < allPeople; i++)
        {
            Vector2 pos = new Vector3(Random.Range(houseLeft.position.x, houseRight.position.x), houseLeft.position.y, 0);
            PersonController person = PoolingSystem.Instance.GetPerson(pos);
            person.gameObject.SetActive(true);
            person.InitPerson(houseLeft.position, houseRight.position, carLeft.position, carRight.position);
            listPerson.Add(person);
        }
    }

    private void HandleOnNotifyRescuedPeopleDie()
    {
        peopleOnSwing--;

    }

    private void HandleOnNotifyRescuedPeopleOnAir()
    {
        peopleOnAir--;
        if (peopleOnSwing == 0)
        {
            if (gameMode == GameMode.Normal)
            {
                if (indexPerson >= allPeople)
                {
                    if (rescuedPeople < targetRescue)
                    {
                        // lose
                        Debug.Log("Lose");
                        return;
                    }
                    else
                    {
                        Debug.Log("WIn");
                        GameManager.Instance.State = STATE.WIN;
                        return;
                    }
                }
            }
            else
            {
                // allPeople + peopleOnAir
                if (indexPerson >= allPeople)
                {
                    if (peopleOnAir > 0)
                    {
                        Debug.Log("Lose");
                        GameManager.Instance.State = STATE.LOSE;
                    }
                    else
                    {
                        if (rescuedPeople < targetRescue)
                        {
                            Debug.Log("Lose");
                            GameManager.Instance.State = STATE.LOSE;
                        }
                        else
                        {
                            Debug.Log("WIN");
                            GameManager.Instance.State = STATE.WIN;
                        }
                    }
                }
            }
        }
    }

    private void HandleOnNotifyRescuedPeople()
    {
        peopleOnSwing--;
        rescuedPeople++;

        rescuedText.text = rescuedPeople.ToString("00") + "/" + targetRescue.ToString("00");
        rescuedText.rectTransform.DOScale(Vector3.one * 1.1f, 0.1f).OnComplete(() =>
        {
            rescuedText.rectTransform.DOScale(Vector3.one, 0.1f);
        });
        if (peopleOnSwing == 0) {
            if (gameMode == GameMode.Normal)
            {
                if (indexPerson >= allPeople)
                {
                    if (rescuedPeople < targetRescue)
                    {
                        // lose
                        Debug.Log("Lose");
                        GameManager.Instance.State = STATE.LOSE;
                        return;
                    }
                    else
                    {
                        Debug.Log("WIn");
                        GameManager.Instance.State = STATE.WIN;
                        return;
                    }
                }
            }
            else
            {
                // allPeople + peopleOnAir
                if (indexPerson >= allPeople)
                {
                    if (peopleOnAir > 0)
                    {
                        Debug.Log("Lose");
                        GameManager.Instance.State = STATE.LOSE;
                    }
                    else
                    {
                        if (rescuedPeople < targetRescue)
                        {
                            Debug.Log("Lose");
                            GameManager.Instance.State = STATE.LOSE;
                        }
                        else
                        {
                            Debug.Log("WIN");
                            GameManager.Instance.State = STATE.WIN;
                        }
                    }
                }
            }
        }
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
                if (ring.IsConnected())
                {
                    if (indexPerson < listPerson.Count)
                    {
                        lastSwing = listPerson[indexPerson];
                        listPerson[indexPerson].StartZipline(RopeSystem.Instance.getPath());
                        indexPerson++;
                        peopleOnSwing++;
                    }
                }
            }
        }
    }

    public void OnDestroy()
    {
        GameMaster.onNotifyRescuedPeople -= this.HandleOnNotifyRescuedPeople;
        GameMaster.onNotifyRescuedPeopleOnAir -= this.HandleOnNotifyRescuedPeopleOnAir;
        GameMaster.onNotifyRescuedPeopleDie -= this.HandleOnNotifyRescuedPeopleDie;
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
