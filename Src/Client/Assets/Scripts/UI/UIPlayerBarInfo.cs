using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerBarInfo : MonoBehaviour
{
    public Text playerInfo;
    public Character character; 
    public Transform owner;

    float height = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdataPlayerName();
        if (owner != null)
            this.transform.position = owner.position + Vector3.up * height;
        this.transform.forward = Camera.main.transform.forward;
    }
    void UpdataPlayerName()
    {
        if (this.character != null)
        {
            string name = this.character.Name + " [" +this.character.Info.Level + "]";

            if (this.playerInfo.text != name)
            {
                this.playerInfo.text = name;
            }
        }
    }
}
