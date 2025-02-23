﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using Common.Data;

namespace GameServer.Services
{
    class MapService : Singleton<MapService>
    {
        public MapService()
        {

            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnMapEntitySync);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapTeleportRequest>(this.OnMapTeleport);
        }

        public void Init()
        {
            MapManager.Instance.Init();
        }

        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest request)
        {
            Character character = sender.Session.Character;
            /*Log.InfoFormat("OnMapEntitySync: characterID:{0}:{1} Entity.Id{2} Evt:{3} Entity:{4}",sender.Session.Character.Id,
                sender.Session.Character.Info.Name,
                request.entitySync.Id,
                request.entitySync.Entity,
                request.entitySync.Entity.String());*/
            MapManager.Instance[character.Info.mapId].UpdateEntity(request.entitySync);
        }
        internal void SendEntutyUpdate(NetConnection<NetSession> sender, NEntitySync entitySync)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapEntitySync = new MapEntitySyncResponse();
            message.Response.mapEntitySync.entitySyncs.Add(entitySync);

            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }

        void OnMapTeleport(NetConnection<NetSession> sender, MapTeleportRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnMapTeleport: characterID:{0}:{1} TeleporterId:{2}", character.Id, character.Data, request.teleporterId);

            if(!DataManager.Instance.Teleporters.ContainsKey(request.teleporterId))
            {
                Log.WarningFormat("Source TeleporterID [{0}] not existed", request.teleporterId);
                return;
            }
            TeleporterDefine source = DataManager.Instance.Teleporters[request.teleporterId];
            if(source.LinkTo==0 || !DataManager.Instance.Teleporters.ContainsKey(source.LinkTo))
            {
                Log.WarningFormat("Source TeleporterID [{0}] LinkTo ID [{1}] not existed", request.teleporterId, source.LinkTo);
            }

            TeleporterDefine target = DataManager.Instance.Teleporters[source.LinkTo];

            MapManager.Instance[source.MapID].CharacterLeave(character);
            character.Position = target.Position;
            character.Direction = target.Direction;
            MapManager.Instance[target.MapID].CharacterEnter(sender, character);
        }
    }
}
