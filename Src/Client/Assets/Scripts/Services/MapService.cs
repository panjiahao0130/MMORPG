using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Network;
using Common.Data;
using SkillBridge.Message;
using Models;
using System;
using Managers;

namespace Services
{
    class MapService : Singleton<MapService> , IDisposable
    {
        public int CurrentMapId = 0;

        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
        }

        

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Unsubscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
        }
        public void Init()
        {

        }
        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            Debug.LogFormat("OnMapCharacterLeave: CharID{0}",response.characterId);
            if (response.characterId != User.Instance.CurrentCharacter.Id)
                CharacterManager.Instance.RemoveCharacter(response.characterId);
            else
                CharacterManager.Instance.Clear();
        }

        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("OnMapCharacterEnter : Map:{0}  Count:{1}", response.mapId, response.Characters.Count);
            foreach (var cha in response.Characters)
            {
                if (User.Instance.CurrentCharacter.Id == cha.Id)
                {
                    User.Instance.CurrentCharacter = cha;
                }
                CharacterManager.Instance.AddCharacter(cha);
            }
            if (CurrentMapId != response.mapId)
            {
                this.CurrentMapId = response.mapId;
                this.EnterMap(response.mapId);
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
                MapDefine map = DataManager.Instance.Maps[mapId];
                User.Instance.CurrentMapData = map;
                SceneManager.Instance.LoadScene(map.Resource);
            }
                //Debug.LogErrorFormat("EnterMap: Map:{0} not existed",mapId);
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
            //Debug.Log(sb.ToString());
        }
    }
}
