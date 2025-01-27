using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Network;
using Common.Data;
using SkillBridge.Message;
using Models;
using System;
using Entities;
using Managers;

namespace Services
{
    class MapService : Singleton<MapService> , IDisposable
    {
       

        public int CurrentMapId { get; set; }
        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
        }

        

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
        }
        public void Init()
        {

        }
        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("OnMapCharacterEnter:Map:{0} Count:{1}", response.mapId, response.Characters.Count);
            foreach (var cha in response.Characters)
            {
                //当前地图没有角色或是进入的角色是当前角色
                if (User.Instance.CurrentCharacterInfo == null || (cha.Type == CharacterType.Player && User.Instance.CurrentCharacterInfo.Id == cha.Id))
                {//当前角色切换地图
                    User.Instance.CurrentCharacterInfo = cha;
                    if(User.Instance.CurrentCharacter  == null)
                    {
                        User.Instance.CurrentCharacter = new Character(cha);
                    }
                    CharacterManager.Instance.AddCharacter(User.Instance.CurrentCharacterInfo);
                    if (CurrentMapId != response.mapId)
                    {
                        this.EnterMap(response.mapId);
                        this.CurrentMapId = response.mapId;
                    }
                    continue;
                }
                CharacterManager.Instance.AddCharacter(cha);
                
            }
            
            
           
        }

        private void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                MapDefine map = DataManager.Instance.Maps[mapId];
                User.Instance.CurrentMapData = map;
                SceneManager.Instance.LoadScene(map.Resource);
            }
            else
            {
                Debug.LogErrorFormat("EnterMap: Map:{0} not existed",mapId);
            }
        }


        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
           
        }
        
        public void SendMapEntitySync(EntityEvent entity,NEntity nEntity)
        {
            Debug.LogFormat("MapEntityUpdateRequest : ID:{0} POS:{1} DIR:{2} SPD:{3}",nEntity.Id,nEntity.Position.String(),
                nEntity.Direction.String(),nEntity.Speed);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapEntitySync = new MapEntitySyncRequest();
            message.Request.mapEntitySync.entitySync = new NEntitySync()
            {
                Id = nEntity.Id,
                Event = entity,
                Entity = nEntity
            };
            NetClient.Instance.SendMessage(message);
        }
        private void OnMapEntitySync(object sender, MapEntitySyncResponse message)
        {
            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //sb.AppendFormat("MapEntityYpdateResponse: Entitys: {0}",message.entitySyncs.Count);
            //sb.AppendLine();
            foreach (var entity in message.entitySyncs)
            {
                EntityManager.Instance.OnEntitySync(entity);
                //sb.AppendFormat("   [{0}]evt:{1} entity:{2}",entity.Id,entity.Event,entity.Entity.String());
                //sb.AppendLine();
            }
        }
            //Debug.Log(sb.ToString());
        
    }
}
