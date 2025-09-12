using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class GameObjectSave
{
    //string字符串为SceneName，通过SceneName来判别当前场景中的SceneData
    public Dictionary<string, SceneSave> sceneData;

    public GameObjectSave()
    {
        sceneData = new Dictionary<string, SceneSave>();
    }

    public GameObjectSave(Dictionary<string, SceneSave> sceneData)
    {
        this.sceneData = sceneData;
    }
}