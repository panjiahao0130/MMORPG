using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain>
{
    public Text characterName;
    public Text characterLevel;
    // Start is called before the first frame update
    protected override void OnStart()
    {
        this.characterName.text = User.Instance.CurrentCharacter.Name;
        this.characterLevel.text = User.Instance.CurrentCharacter.Level.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnBack()
    {
        SceneManager.Instance.LoadScene("CharSelect");
        //Services.UserService.Instance.SendGameLeave();
    }
}
