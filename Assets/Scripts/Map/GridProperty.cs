[System.Serializable]
public class GridProperty
{
    public GridCoordinate gridCoordinate;
    public GridBoolProperty gridBoolProperty;
    public bool girdBoolValue = false;

    public GridProperty(GridCoordinate gridCoordinate, GridBoolProperty gridBoolProperty,bool girdBoolValue)
    {
        this.gridCoordinate = gridCoordinate;
        this.gridBoolProperty = gridBoolProperty;
        this.girdBoolValue = girdBoolValue;
    }
}