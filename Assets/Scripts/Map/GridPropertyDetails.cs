[System.Serializable]

public class GridPropertyDetails
{
    public int gridX;
    public int gridY;
    public bool isDiggable;
    public bool canDropItems;
    public bool canPlaceFurniture;
    public bool isPath;
    public bool isNPCObstacle;
    public int daySinceDug;
    public int daySinceWatered;
    public int seedItemcod;
    public int growthDays;
    public int daysSinceLastHarvest;

    public GridPropertyDetails()
    {
    }
}