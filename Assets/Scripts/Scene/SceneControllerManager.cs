using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneControllerManager : SingletonMonobehaviour<SceneControllerManager>
{
    private bool isFading;
    [SerializeField] private float fadeDuration;
    [SerializeField] private CanvasGroup faderCanvanGroup = null;
    [SerializeField] private Image faderImage = null;
    public SceneName startingSceneName;

    //用于整合创建fading过程中的输入阻挡的携程
    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {
        //调用before 
    }
    
    //当玩家切换场景时，会调用该方法
    public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition)
    {
        //如果没有进行fade 择开始fading然后切换场景
        if (!isFading)
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
        }
    }
}
