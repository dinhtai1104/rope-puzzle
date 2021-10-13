using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingSystem : Singleton<PoolingSystem>
{
    public PersonController personPrefab;

    private List<PersonController> listPerson = new List<PersonController>();
    private void Start()
    {
        listPerson = new List<PersonController>();
    }


    private int personIndex = 0;
    public PersonController GetPerson(Vector2 pos)
    {
        if (personIndex >= listPerson.Count)
        {
            PersonController per = Instantiate(personPrefab, transform);
            per.transform.position = Vector2.one * 100;
            per.gameObject.SetActive(false);
            listPerson.Add(per);
        }
        listPerson[personIndex].transform.position = pos;
        return listPerson[personIndex++];
    }

    public void ResetPool()
    {
        personIndex = 0;
        foreach (PersonController go in listPerson)
        {
            go.gameObject.SetActive(false);
        }
    }
}
