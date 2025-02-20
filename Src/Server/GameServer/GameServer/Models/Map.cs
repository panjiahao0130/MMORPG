using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;

using Common;
using Common.Data;

using Network;
using GameServer.Managers;
using GameServer.Entities;
using GameServer.Services;

namespace GameServer.Models
{
    class Map
    {
        internal class MapCharacter
        {
            public NetConnection<NetSession> connection;
            public Character character;

            public MapCharacter(NetConnection<NetSession> conn, Character cha)
            {
                this.connection = conn;
                this.character = cha;
            }
        }

        public int ID
        {
            get { return this.Define.ID; }
        }
        internal MapDefine Define;

        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();


        internal Map(MapDefine define)
        {
            this.Define = define;
        }

        internal void Update()
        {

        }

        /// <summary>
        /// 角色进入地图
        /// </summary>
        /// <param name="character"></param>
        internal void CharacterEnter(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("CharacterEnter: Map:{0} characterId:{1}", this.Define.ID, character.Id);

            character.Info.mapId = this.ID;
            /*if (!MapCharacters.ContainsKey(character.Id))
            {
                
            }*/
            this.MapCharacters[character.Id] = new MapCharacter(conn, character);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            message.Response.mapCharacterEnter.mapId = this.Define.ID;

            foreach (var kv in this.MapCharacters)
            {
                message.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);

                this.SendCharacterEnterMap(kv.Value.connection, character.Info);
            }

            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data, 0, data.Length);
            
        }

        void SendCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            Log.InfoFormat("SendCharacterEnterMap: Map{0} characterId:{1}", this.Define.ID, character.Id);
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            message.Response.mapCharacterEnter.mapId = this.Define.ID;
            message.Response.mapCharacterEnter.Characters.Add(character);

            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data, 0, data.Length);
        }
        internal void CharacterLeave(Character cha)
        {
            Log.InfoFormat("CharacterLeave: Map{0} characterId:{1}",this.Define.ID,cha.Id);
            
            foreach (var kv in this.MapCharacters)
            {
                this.SendCharacterLeave(kv.Value.connection,cha);
            }
            this.MapCharacters.Remove(cha.Id);
        }
        void SendCharacterLeave(NetConnection<NetSession> conn,Character cha)
        {
            Log.InfoFormat("SendCharacterLeaveMap To {0}:{1} : Map:{2} Character:{3}:{4}", conn.Session.Character.Id, conn.Session.Character.Info.Name, this.Define.ID, cha.Id, cha.Info.Name);
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            message.Response.mapCharacterLeave.entityId = cha.entityId;
            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data,0,data.Length);
            Log.InfoFormat("SendCharacterLeaveMapSuccess");
        }
        internal void UpdateEntity(NEntitySync entitySync)
        {
            foreach (var kv in this.MapCharacters)
            {
                //自己不更新
                if (kv.Value.character.entityId == entitySync.Id)
                {
                    kv.Value.character.Position = entitySync.Entity.Position;
                    kv.Value.character.Direction = entitySync.Entity.Direction;
                    kv.Value.character.Speed = entitySync.Entity.Speed;
                }
                //其他玩家发送响应
                else
                {
                    MapService.Instance.SendEntutyUpdate(kv.Value.connection,entitySync);
                }
            }
        }
    }
}
