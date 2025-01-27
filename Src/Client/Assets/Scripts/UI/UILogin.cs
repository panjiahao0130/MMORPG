using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Services;
using SkillBridge.Message;

public class UILogin : MonoBehaviour {

    /// <summary>
    /// 上次账号
    /// </summary>
    const string LAST_USER_NAME = "LAST_USER_NAME";
    /// <summary>
    /// 上次密码
    /// </summary>
    const string LASR_PASSWORD = "LASR_PASSWORD";
    public InputField username;
    public InputField password;
    public Button buttonLogin;
    public Button buttonRegister;

    // Use this for initialization
    void Start () {
        UserService.Instance.OnLogin = OnLogin;
        string lastUserName = PlayerPrefs.GetString(LAST_USER_NAME, "");
        string lastPassword = PlayerPrefs.GetString(LASR_PASSWORD, "");
        if (!string.IsNullOrEmpty(lastUserName) &&!string.IsNullOrEmpty(lastPassword))
        {
            this.username.text = lastUserName;
            this.password.text = lastPassword;
        }
    }


    // Update is called once per frame
    void Update () {
		
	}

    public void OnClickLogin()
    {
        if (string.IsNullOrEmpty(this.username.text))
        {
            MessageBox.Show("请输入账号");
            return;
        }
        if (string.IsNullOrEmpty(this.password.text))
        {
            MessageBox.Show("请输入密码");
            return;
        }
        UserService.Instance.SendLogin(this.username.text,this.password.text);
       

    }

    void OnLogin(Result result, string message)
    {
        if (result == Result.Success)
        {
            //登录成功，进入角色选择
            MessageBox.Show("登录成功,进入角色选择界面", "提示", MessageBoxType.Information).OnYes = this.CloceLogin;
        }
        else
            MessageBox.Show(message, "错误", MessageBoxType.Error);
    }

    void CloceLogin()
    {
        transform.gameObject.SetActive(false);
        PlayerPrefs.SetString(LAST_USER_NAME, this.username.text);
        PlayerPrefs.SetString(LASR_PASSWORD, this.password.text);
        PlayerPrefs.Save();
        SceneManager.Instance.LoadScene("CharSelect");
    }
}
