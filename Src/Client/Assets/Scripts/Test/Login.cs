using System.Collections;
using System.Collections.Generic;
using SkillBridge.Message;
using UnityEngine;

public class Login : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Network.NetClient.Instance.Init("127.0.0.1",8000);
        Network.NetClient.Instance.Connect();
        SkillBridge.Message.NetMessage msg = new SkillBridge.Message.NetMessage();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
