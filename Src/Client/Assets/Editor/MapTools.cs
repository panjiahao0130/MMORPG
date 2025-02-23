using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MapTools {

  [MenuItem("Map Tools/Export Teleports")]
  public static void ExportTeleporters()
  {
        DataManager.Instance.Load();
        Scene current = EditorSceneManager.GetActiveScene();
        string currentScene = current.name;
        if(current.isDirty)
        {
            EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确定");
            return;
        }
        TeleporterObject[] teleporters = null; 
        string sceneFile;
        foreach (var map in DataManager.Instance.Maps)
        {
             sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            if(!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("Scene{0} not existed!", sceneFile); 
                continue;
            }
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);
            teleporters = GameObject.FindObjectsOfType<TeleporterObject>();
            foreach (var teleporter in teleporters)
            {
                if(!DataManager.Instance.Teleporters.ContainsKey(teleporter.ID))
                {
                    EditorUtility.DisplayDialog("错误",string.Format("地图:{0}中配置的 Teleporter:[1]中不存在",map.Value.Resource,teleporter.ID), "确定");
                    return;
                }
                TeleporterDefine def = DataManager.Instance.Teleporters[teleporter.ID];
                if(def.MapID !=map.Value.ID)
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图:{0}中配置的 Teleporter:[1] MapID:{2} 错误", map.Value.Resource, teleporter.ID,def.MapID), "确定");
                }
                def.Position = GameObjectTool.WorldToLogicN(teleporter.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(teleporter.transform.forward);
            }
        }
        DataManager.Instance.SaveTeleporters();
        sceneFile = "Assets/Levels/" +currentScene + ".unity";
        EditorSceneManager.OpenScene(sceneFile);

        EditorUtility.DisplayDialog("提示", "传送点导出完成", "确定");
    }
}
