﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Models;
using Network;
using UnityEngine;

using SkillBridge.Message;

namespace Services
{
    class UserService : Singleton<UserService>, IDisposable
    {
        
        public UnityEngine.Events.UnityAction<Result, string> OnRegister;
        public UnityEngine.Events.UnityAction<Result, string> OnLogin;
        public UnityEngine.Events.UnityAction<Result, string> OnCreateCharacter;

        //待处理的消息
        NetMessage pendingMessage = null;

        bool connected = false;

        public UserService()
        {
            NetClient.Instance.OnConnect += OnGameServerConnect;
            NetClient.Instance.OnDisconnect += OnGameServerDisconnect;
           
            MessageDistributer.Instance.Subscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Subscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
            MessageDistributer.Instance.Subscribe<UserGameEnterResponse>(OnGameEnter);
            MessageDistributer.Instance.Subscribe<UserGameLeaveResponse>(OnGameLeave);
            //MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(OnCharacterEnter);

        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Unsubscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
            MessageDistributer.Instance.Unsubscribe<UserGameEnterResponse>(OnGameEnter);
            MessageDistributer.Instance.Unsubscribe<UserGameLeaveResponse>(OnGameLeave);
            NetClient.Instance.OnConnect -= OnGameServerConnect;
            NetClient.Instance.OnDisconnect -= OnGameServerDisconnect;
        }

        public void Init()
        {

        }

        public void ConnectToServer()
        {
            Debug.Log("ConnectToServer() Start ");
            //NetClient.Instance.CryptKey = this.SessionId;
            NetClient.Instance.Init("127.0.0.1", 8000);
            NetClient.Instance.Connect();
        }


        void OnGameServerConnect(int result, string reason)
        {
            Log.InfoFormat("LoadingMesager::OnGameServerConnect :{0} reason:{1}", result, reason);
            if (NetClient.Instance.Connected)
            {
                this.connected = true;
                if(this.pendingMessage!=null)
                {
                    NetClient.Instance.SendMessage(this.pendingMessage);
                    this.pendingMessage = null;
                }
            }
            else
            {
                if (!this.DisconnectNotify(result, reason))
                {
                    MessageBox.Show(string.Format("网络错误，无法连接到服务器！\n RESULT:{0} ERROR:{1}", result, reason), "错误", MessageBoxType.Error);
                }
            }
        }

        public void OnGameServerDisconnect(int result, string reason)
        {
            this.DisconnectNotify(result, reason);
            return;
        }

        bool DisconnectNotify(int result,string reason)
        {
            if (this.pendingMessage != null)
            { 
                if(this.pendingMessage.Request.userRegister!=null)
                {
                    if (this.OnRegister != null)
                    {
                        this.OnRegister(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                else if (pendingMessage.Request.userLogin!= null)
                {
                    if (this.OnLogin!= null)
                    {
                        this.OnLogin(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                else
                {
                    if (this.OnCreateCharacter != null)
                    {
                        this.OnCreateCharacter(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                return true;
               
            }
            return false;
        }

        public void SendLogin(string user, string psw)
        {
            Debug.LogFormat("UserLoginRequest::user :{0} psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userLogin = new UserLoginRequest();
            message.Request.userLogin.User = user;
            message.Request.userLogin.Passward = psw;
            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }

        }

        public void OnUserLogin(object sender, UserLoginResponse response)
        {
            Debug.LogFormat("OnUserRegister:{0} [{1}]", response.Result, response.Errormsg);

            if (response.Result == Result.Success)
            {//登陆成功逻辑
                Models.User.Instance.SetupUserInfo(response.Userinfo);
            }
            if (this.OnLogin != null)
            {
                this.OnLogin(response.Result, response.Errormsg);
            }
        }
        
        public void SendRegister(string user, string psw)
        {
            Debug.LogFormat("UserRegisterRequest::user :{0} psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userRegister = new UserRegisterRequest();
            message.Request.userRegister.User = user;
            message.Request.userRegister.Passward = psw;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        void OnUserRegister(object sender, UserRegisterResponse response)
        {
            Debug.LogFormat("OnUserRegister:{0} [{1}]", response.Result, response.Errormsg);

            if (this.OnRegister != null)
            {
                this.OnRegister(response.Result, response.Errormsg);
            }
        }
        

        public void SendCreateCharacter(string charName, CharacterClass charClass)
        {
            Debug.LogFormat("UserCreateCharacterRequest::charName :{0} charClass:{1}", charName, charClass);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.createChar = new UserCreateCharacterRequest();
            message.Request.createChar.Name = charName;
            message.Request.createChar.Class = charClass;
            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }
        void OnUserCreateCharacter(object sender, UserCreateCharacterResponse response)
        {
            Debug.LogFormat("OnUserCreateCharacter:{0} [{1}]", response.Result, response.Errormsg);

            if(response.Result == Result.Success)
            {
                Models.User.Instance.Info.Player.Characters.Clear();
                Models.User.Instance.Info.Player.Characters.AddRange(response.Characters);
            }

            if (this.OnCreateCharacter != null)
            {
                this.OnCreateCharacter(response.Result, response.Errormsg);
            }
        }

        public void SendGameEnter(int selectCharacterIdx)
        {
            Debug.LogFormat("UserGameEnterRequest:charIdx: {0}", selectCharacterIdx);
            NetMessage message=new NetMessage();
            message.Request=new NetMessageRequest();
            message.Request.gameEnter=new UserGameEnterRequest();
            message.Request.gameEnter.characterIdx = selectCharacterIdx;
            NetClient.Instance.SendMessage(message);
        }
        private void OnGameEnter(object sender, UserGameEnterResponse response)
        {
            Debug.LogFormat("OnGameEnter:{0} [{1}]", response.Result, response.Errormsg);
            if (response.Result == Result.Success)
            {
                //todo:初始化各种数据
                User.Instance.CurrentCharacter = response.Character;
            }
        }
       
        public void SendGameLeave()
        {
            Debug.Log("UserGameLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.gameLeave = new UserGameLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }
        private void OnGameLeave(object sender, UserGameLeaveResponse response)
        {
            Debug.LogFormat("OnGameLeave:{0} [{1}]", response.Result, response.Errormsg);
            MapService.Instance.CurrentMapId = 0;
            User.Instance.CurrentCharacter = null;
            Debug.Log("当期地图:清除角色信息");
        }
    }
}
