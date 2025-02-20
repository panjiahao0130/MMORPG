using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Entities;
using Services;
using SkillBridge.Message;
using Models;
using Managers;
using Object = UnityEngine.Object;


public class GameObjectManager : MonoSingleton<GameObjectManager>
{

    Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();

    /*private void Awake()
    {
        
    }*/

    protected override void OnStart()
    {
        StartCoroutine(InitGameObjects());
        CharacterManager.Instance.OnCharacterEnter += OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave += OnCharacterLeave;
    }

    private void OnDestroy()
    {
        CharacterManager.Instance.OnCharacterEnter -= OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave -= OnCharacterLeave;
    }

    void OnCharacterEnter(Character cha)
    {
        CreateCharacterObject(cha);
    }
    void OnCharacterLeave(Character character)
    {
        if (!Characters.ContainsKey(character.entityId))
            return;
        if (Characters [character.entityId] != null)
        {
            Destroy(Characters[character.entityId]);
            this.Characters.Remove(character.entityId);
        }
    }
    IEnumerator InitGameObjects()
    {
        if (CharacterManager.Instance == null)
        {
            Debug.LogError("CharacterManager.Instance is null in InitGameObjects");
            yield break;
        }
        foreach (var cha in CharacterManager.Instance.Characters.Values)
        {
            CreateCharacterObject(cha);
            yield return null;
        }
    }

    private void CreateCharacterObject(Character character)
    {
        /*if (character.Info.Id == User.Instance.CurrentCharacterInfo.Id)
        {
            int gameObjectManagerCount = GameObject.FindObjectsOfType<GameObjectManager>().Length;
            int uiWorldElementManagerCount = GameObject.FindObjectsOfType<UIWorldElementManager>().Length;

            if (gameObjectManagerCount > 1)
            {
                Debug.LogError("Multiple instances of GameObjectManager detected for CurrentCharacter's Id: " + character.Info.Id);
            }

            if (uiWorldElementManagerCount > 1)
            {
                Debug.LogError("Multiple instances of UIWorldElementManager detected for CurrentCharacter's Id: " + character.Info.Id);
            }
        }*/
        if (!Characters.ContainsKey(character.entityId) || Characters[character.entityId] == null)
        {
            Debug.LogFormat("Create Character Object for Character[{0}] Name[{1}]", character.entityId, character.Info.Name);
            Object obj = Resloader.Load<Object>(character.Define.Resource);
            if (obj == null)
            {
                Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.", character.Define.TID, character.Define.Resource);
                return;
            }
            GameObject go = (GameObject)Instantiate(obj,this.transform);
            go.name = "Character_" + character.Info.Id + "_" + character.Info.Name;
            Characters[character.entityId] = go;
            UIWorldElementManager.Instance.AddCharacterNameBar(go.transform,character);
            
        }
        this.InitGameObject(Characters[character.entityId], character);
        
    }
    private void InitGameObject(GameObject go, Character character)
    {
        go.transform.position = GameObjectTool.LogicToWorld(character.position);
        go.transform.forward = GameObjectTool.LogicToWorld(character.direction);

        EntityController ec = go.GetComponent<EntityController>();
        if (ec != null)
        {
            ec.entity = character;
            ec.isPlayer = character.IsCurrentPlayer;
        }
        PlayerInputController pc = go.GetComponent<PlayerInputController>();
        if (pc != null)
        {
            if (character.IsCurrentPlayer)
            {
                User.Instance.CurrentCharacterObject = go;
                Debug.Log("Current Character Object:" + User.Instance.CurrentCharacterObject);
                MainPlayerCamera.Instance.player = go;
                pc.enabled = true;
                pc.character = character;
                pc.entityController = ec;
            }
            else
            {
                pc.enabled = false;
            }
        }
    }
}

