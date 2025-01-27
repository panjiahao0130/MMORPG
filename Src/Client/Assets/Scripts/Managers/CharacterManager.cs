using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;
using SkillBridge.Message;
using UnityEngine;

namespace Managers
{
    class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

        public UnityAction<Character> OnCharacterEnter;
        public UnityAction<Character> OnCharacterLeave;
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
            int[] keys = Characters.Keys.ToArray();
            foreach (var key in keys)
            {
                this.RemoveCharacter(key);
            }
            this.Characters.Clear();
        }

        public void AddCharacter(NCharacterInfo cha)
        {
            Debug.LogFormat("AddCharacter :{0}:{1},Map:{2},Entity{3}",cha.Id,cha.Name,cha.mapId,cha.Entity.String());
            Character character =new Character(cha);
            Characters[cha.Id]= character;
            if (OnCharacterEnter!= null)
            {
                OnCharacterEnter(character);
            }
        }
        public void RemoveCharacter(int characterId)
        {
            Debug.LogFormat("RemoveCharacter :{0}",characterId);
            if (Characters.ContainsKey(characterId))
            {
                if (OnCharacterLeave!= null)
                {
                    OnCharacterLeave(Characters[characterId]);
                }
                Characters.Remove(characterId);
            }
            
        }
    }
}
