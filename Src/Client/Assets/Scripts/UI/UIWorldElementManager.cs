using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIWorldElementManager : MonoSingleton<UIWorldElementManager>
{
    public GameObject nameBarPrefab;
    Dictionary<Transform, GameObject> charecters = new Dictionary<Transform, GameObject>();
    // Start is called before the first frame update
    protected override void OnStart()
    {
        nameBarPrefab.SetActive(false);
    }
    
    public void AddCharacterNameBar(Transform owner, Character character)
    {
        GameObject goNameBar = Instantiate(nameBarPrefab,this.transform);
        goNameBar.name = "NameBar:" + character.entityId;
        goNameBar.GetComponent<UIPlayerBarInfo>().character = character;
        goNameBar.GetComponent<UIPlayerBarInfo>().owner = owner;
        goNameBar.SetActive(true);
        this.charecters[owner] = goNameBar;
    }
    public void RemoveCharacterNameBar(Transform owner)
    {
        if (this.charecters.ContainsKey(owner))
        {
            Destroy(charecters[owner]);
            this.charecters.Remove(owner);
        }

    }
}
