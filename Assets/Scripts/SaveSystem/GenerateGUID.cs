using System;
using UnityEngine;

[ExecuteAlways]

public class GenerateGUID : MonoBehaviour
{
    [SerializeField] private string _gUID = "";
    
    public string GUID { get => _gUID;  set => _gUID = value;  }
    
    private void Awake()
    {
        //判断该gameObject是否在运行中，只在游戏非运行下的编辑器内运行，
        if (!Application.IsPlaying(gameObject))
        {
            if (_gUID == "")
            {
                //分配GUID
                _gUID = System.Guid.NewGuid().ToString();
            }
        }
    }
}