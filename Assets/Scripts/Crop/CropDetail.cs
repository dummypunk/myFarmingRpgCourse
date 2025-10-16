using UnityEngine;

[System.Serializable]
public class CropDetail
{
    [ItemCodeDescription]
    public int seedItemCode; // 对应种子的物品代码
    public int[] growthDays; // 每个生长阶段的生长天数
    public int totalGrowthDays; // 总生长天数
    public GameObject[] growthPrefab; // 实例化生长阶段时使用的预制体
    public Sprite[] growthSprite; // 生长阶段的精灵图
    public Season[] seasons; // 生长季节
    public Sprite harvestedSprite; // 收割后使用的精灵图
    
    [ItemCodeDescription]
    public int harvestedTransformItemCode; // 如果物品在收割时转变为另一个物品，则填入该物品代码
    public bool hideCropBeforeHarvestedAnimation; // 是否在收割动画前禁用作物
    public bool disableCropCollidersBeforeHarvestedAnimation; // 是否禁用作物的碰撞体，以避免收割动画影响其他游戏对象
    public bool isHarvestedAnimation; // 是否在最终生长阶段预制体上播放收割动画
    public bool isHarvestActionEffect = false; // 是否有收割动作效果的标志
    public bool spawnCropProducedAtPlayerPosition;
    public HarvestActionEffect harvestActionEffect; // 作物的收割动作效果
    
    [ItemCodeDescription]
    public int[] harvestToolItemCode; // 可用于收割的工具的物品代码数组，若无工具需求则为0
    public int[] requiredHarvestActions; // 对应收割工具所需的收割动作次数
    
    [ItemCodeDescription]
    public int[] cropProducedItemCode; // 收割后产出的物品代码数组
    public int[] cropProducedMinQuantity; // 产出作物的最小数量数组
    public int[] cropProducedMaxQuantity; // 产出作物的最大数量数组，若大于最小数量则随机生成最小到最大之间的数量
    public int daysToRegrow; // 下次再生的天数，若为单次作物则为-1
    
    /// <summary>
    /// 如果该工具物品代码可用于收割此作物则返回 true，否则返回 false
    /// </summary>
    public bool CanUseToolToHarvestCrop(int toolItemCode)
    {
        if (RequiredHarvestActionsForTool(toolItemCode) == -1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// 如果该工具不能用于收割此作物则返回 -1，否则返回此工具所需的收割动作次数
    /// </summary>
    public int RequiredHarvestActionsForTool(int toolItemCode)
    {
        for (int i = 0; i < harvestToolItemCode.Length; i++)
        {
            if (harvestToolItemCode[i] == toolItemCode)
            {
                return requiredHarvestActions[i];
            }
        }
        return -1;
    }
}