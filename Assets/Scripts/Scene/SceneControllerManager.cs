using System;
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

    private IEnumerator Start()
    {
        //设置初始化黑色背景的Alpha值
        faderImage.color = new Color(0f, 0f, 0f, 1f);
        faderCanvanGroup.alpha = 1f;
        
        //游戏初始化场景启动时启动携程，并等待携程执行完毕
        yield return StartCoroutine(LoadSceneSetActive(startingSceneName.ToString()));
        
        EventHandler.CallAfterSceneLoadEvent();
        
        //当场景加载完毕，黑色背景渐变到透明
        StartCoroutine(Fade(0f));
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
    
    //用于整合创建fading过程中的输入阻挡的携程
    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {
        //调用before scene unload fade out event
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();
        
        //开始场景切换渐变，并阻止输入，持续一秒钟时间后继续
        yield return StartCoroutine(Fade(1f));
        
        //设置player position
        Player.Instance.gameObject.transform.position = spawnPosition;
        
        EventHandler.CallBeforeSceneUnloadEvent();
        
        //UnLoad the current active Scene
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        
        //Start loading the given scene and wait for it to finish
        yield return StartCoroutine(LoadSceneSetActive(sceneName));
        
        EventHandler.CallAfterSceneLoadEvent();

        yield return StartCoroutine(Fade(0f));

        EventHandler.CallAfterSceneLoadFadeInEvent();
    }

    private IEnumerator Fade(float finalAlpha)
    {
        //Set the Fading Flag to true so the FadeAndSwitchScenes coroutine won't be called again
        isFading = true;
        
        //设置canvas group阻挡射线为ture，可以避免屏幕上的输入
        faderCanvanGroup.blocksRaycasts = true;
        
        //计算canvas group的渐变速度，基于当前的canvas group的alpha的数值
        float fadeSpeed = Mathf.Abs(faderCanvanGroup.alpha - finalAlpha) / fadeDuration;

        //如果faderCanvasGroup的alpha和finalAlpha不相等，进行循环
        while (!Mathf.Approximately(faderCanvanGroup.alpha, finalAlpha))
        {
            //将current Alpha向目标Alpha移动
            faderCanvanGroup.alpha = Mathf.MoveTowards(faderCanvanGroup.alpha, finalAlpha, 
                fadeSpeed * Time.deltaTime);
            
            //Wait for a time then continue
            yield return null;
        }
        
        //Set the flag to false since the fade has finished
        isFading = false;
        
        //停止canvasGroup的射线阻挡
        faderCanvanGroup.blocksRaycasts = false;
    }
    
    private IEnumerator LoadSceneSetActive(string sceneName)
    {
        //allow the given scene to load over several frames and add it to the already loaded scenes 
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        
        //Find the scene that was most recently loaded            scnenCount - 1 为scene中最新创建的
        Scene newLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        
        //设置当前scene为切换场景
        SceneManager.SetActiveScene(newLoadedScene);
    }
}
