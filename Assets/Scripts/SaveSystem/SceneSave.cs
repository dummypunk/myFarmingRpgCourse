using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class SceneSave
{
    //通过string字符串来识别SceneItem的List
    public Dictionary<string, List<SceneItem>> listSceneItemDictionary;
}