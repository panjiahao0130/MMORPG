using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Services;
using SkillBridge.Message;
public class UICharacterSelect : MonoBehaviour
{

    public GameObject panelCreate;
    public GameObject panelSelect;
    

    public InputField charName;
    CharacterClass charClass;

    public Transform uiCharList;
    public GameObject uiCharInfo;

    public List<GameObject> uiChars = new List<GameObject>();

    public Image[] titles;

    public Text descs;


    public Text[] names;

    private int selectCharacterIdx = -1; 

    public UICharacterView characterView;

    // Use this for initialization
    void Start()
    {
        InitCharacterSelect(true);
        UserService.Instance.OnCreateCharacter = OnCharacterCreate;
    }
    /// <summary>
    /// 点击角色选择界面的创建角色按钮
    /// </summary>
    public void OnClickCreateCharacterBtn()
    {
        panelSelect.SetActive(false);
        panelCreate.SetActive(true);
        //初始化角色类别按钮 战士
        OnClickCharacterClassBtn(1);
    }
    void Update()
    {

    }
    
    /// <summary>
    /// 点击角色创建界面的角色类别按钮
    /// </summary>
    /// <param name="charClass">角色类别</param>
    public void OnClickCharacterClassBtn(int charClass)
    {
        this.charClass = (CharacterClass)charClass;
        characterView.CurrectCharacter = charClass-1;
        for (int i = 0; i < 3; i++)
        {
            titles[i].gameObject.SetActive(i == charClass - 1);
            names[i].text=DataManager.Instance.Characters[i+1].Name;
        }
        descs.text=DataManager.Instance.Characters[charClass].Description;
    }


    /// <summary>
    /// 点击角色创建界面的进入游戏按钮
    /// </summary>
    public void OnClickCreate()
    {
        //todo 目前设计是点击创建后创建角色，然后进入角色选择界面，之后优化成直接进入主城
        if (string.IsNullOrEmpty(charName.text))
        {
            MessageBox.Show("请输入角色名称！");
            return;
        }
        UserService.Instance.SendCreateCharacter(charName.text, charClass);
    }
    void OnCharacterCreate(Result result, string message)
    {
        if (result == Result.Success)
        {
            InitCharacterSelect(true);
        }
        else
        {
            MessageBox.Show(message, "错误", MessageBoxType.Error);
        }
    }
    public void InitCharacterSelect(bool init)
    {
        panelSelect.SetActive(true);
        panelCreate.SetActive(false);
        if (init)
        {
            foreach (var oldChar in uiChars)
            {
                Destroy(oldChar);
            }
            uiChars.Clear();
            int count = User.Instance.Info.Player.Characters.Count;
            for (int i = 0; i < count; i++)
            {
                GameObject go = Instantiate(uiCharInfo, uiCharList);
                UICharInfo uiChar = go.GetComponent<UICharInfo>();
                uiChar.info = User.Instance.Info.Player.Characters[i];
                Button btn = go.GetComponentInChildren<Button>();
                int index = i;
                btn.onClick.AddListener(() => { OnSelectCharacter(index); });
                uiChars.Add(go);
                go.SetActive(true);
            }
        }
    
    }
    public void OnSelectCharacter(int index)
    {
        selectCharacterIdx = index;
        var character = User.Instance.Info.Player.Characters[index];
        Debug.LogFormat("Select Character：[{0}{1}{2}]", character.Id,character.Name,character.Class);
        characterView.CurrectCharacter = ((int)character.Class - 1);
        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            UICharInfo uiChar = uiChars[i].GetComponent<UICharInfo>();
            uiChar.Selected = i == index;
        }
    }
    public void OnClickPlay()
    {
        if (selectCharacterIdx>=0)
        {
            UserService.Instance.SendGameEnter(selectCharacterIdx);
        }
    }
    //todo 创建角色界面点击退出，显示角色模型错误
}