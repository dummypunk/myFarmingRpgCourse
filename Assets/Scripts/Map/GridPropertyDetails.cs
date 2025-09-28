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
    public int daySinceDug = -1;
    public int daySinceWatered = -1;
    public int seedItemcod;
    public int growthDays;
    public int daysSinceLastHarvest = -1;

    public GridPropertyDetails()
    {
    }
}