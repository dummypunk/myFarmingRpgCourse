using UnityEngine;
using Cinemachine;

public class SwitchConfineBoundingShape : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SwitchBoundingShape();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 激活cinemachine专用的碰撞盒来限制相机超出地图边缘
    /// </summary>


    private void SwitchBoundingShape()
    {
        //获取多边形碰撞盒的gameobject“boundsConfiner”，避免摄像机超出地图边缘
        PolygonCollider2D polygonCollider2D = GameObject.FindWithTag(Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();

        CinemachineConfiner cinemachineConfiner = GetComponent<CinemachineConfiner>();

        cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;

        //以上方法改变了摄像机边界，需要通过以下方法清楚缓存

        cinemachineConfiner.InvalidatePathCache();
    }
}
