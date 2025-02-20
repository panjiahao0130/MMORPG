using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;
using GameServer.Entities;
using Common;
using Managers;

namespace GameServer.Managers
{
    class CharacterManager : Singleton<CharacterManager>
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

        public CharacterManager()
        {

        }
        public void Dispose()
        {

        }
        public void Init()
        {

        }
        public void Clear()
        {
            this.Characters.Clear();
        }
        public Character AddCharacter(TCharacter cha)
        {
            Character character = new Character(CharacterType.Player,cha);
            Log.InfoFormat("AddCharacter: {0}({1}) Map:{2}", cha.Name, cha.ID, cha.MapID);
            EntityManager.Instance.AddEntity(cha.MapID, character);
            character.Info.EntityId = character.entityId;
            this.Characters[character.entityId] = character;
            /*for(int i = 0; i < Characters.Count; i++)
            {
                Log.InfoFormat("角色列表中有：{0}{1}", Characters[cha.ID].Info.Name, Characters[cha.ID].Info.Id);
            }*/
            return character;
        }
        public void RemoveCharacter(int chaId)
        {
           
            if (this.Characters.ContainsKey(chaId))
            {
                Log.InfoFormat("RemoveCharacter: {0}", chaId);
                var cha = Characters[chaId];
                EntityManager.Instance.RemoveEntity(cha.Data.MapID, cha);
                this.Characters.Remove(chaId);
            }
            else
            {
                Log.ErrorFormat("当前角色id不存在");
            }
            

        }
    }
}
