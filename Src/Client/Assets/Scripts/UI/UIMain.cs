using Models;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain>
{
    public Text characterName;
    public Text characterLevel;
 
    protected override void OnStart()
    {
        this.characterName.text = User.Instance.CurrentCharacter.Name;
        this.characterLevel.text = User.Instance.CurrentCharacter.Level.ToString();
    }

   
    public void OnBackBtnClick()
    {
        UserService.Instance.SendGameLeave();
        SceneManager.Instance.LoadScene("CharSelect");
    }
}
