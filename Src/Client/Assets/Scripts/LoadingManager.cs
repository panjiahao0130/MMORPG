﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Managers;
using SkillBridge.Message;
using ProtoBuf;
using Services;

public class LoadingManager : MonoBehaviour {

    public GameObject UITips;
    public GameObject UILoading;
    public GameObject UILogin;
    public Image bg1;
    public Image bg2;

    public Slider progressBar;
    public Text progressText;
    public Text progressNumber;

    // Use this for initialization
    IEnumerator Start()
    {
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
        UnityLogger.Init();
        Common.Log.Init("Unity");
        Common.Log.Info("LoadingManager start");

        UITips.SetActive(true);
        UILoading.SetActive(false);
        UILogin.SetActive(false);
        yield return new WaitForSeconds(2f);
        bg2.gameObject.SetActive(true);
        UILoading.SetActive(true);
        yield return new WaitForSeconds(1f);
        UITips.SetActive(false);

        yield return DataManager.Instance.LoadData();

        //Init basic services
        MapService.Instance.Init();
        UserService.Instance.Init();



        TestManager.Instance.Init();
        // Fake Loading Simulate
        for (float i = 50; i < 100;)
        {
            i += Random.Range(0.1f, 1.5f);
            progressBar.value = i;
            yield return new WaitForEndOfFrame();
        }

        UILoading.SetActive(false);
        bg2.gameObject.SetActive(false);
        bg1.gameObject.SetActive(true);
        UILogin.SetActive(true);
        yield return null;
    }


    // Update is called once per frame
    void Update () {
        progressNumber.text = (int)progressBar.value + "%";
    }
}
