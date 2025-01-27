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
        public Character AddCharacter(TCharacter chara)
        {
            Character character = new Character(CharacterType.Player,chara);
            Log.InfoFormat("AddCharacter: {0}", character.Id);
            this.Characters[chara.ID] = character;
            Log.InfoFormat("AddCharacter: {0}", character.Id);
            return character;
        }
        public void RemoveCharacter(int chara)
        {
            //EntityManager.Instance.RemoveEntity(Characters[chara].Data.MapID, Characters[chara]);
            this.Characters.Remove(chara);
        }
    }
}
