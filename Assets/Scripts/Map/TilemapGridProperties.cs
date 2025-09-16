using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class TilemapGridProperties : MonoBehaviour
{
    private Tilemap tilemap;
    [SerializeField] private SO_GridProperties so_gridProperties = null;
    [SerializeField] private GridBoolProperty gridBoolProperty;
    
    private void OnEnable()
    {
        //Only populate in the editor 
        if (!Application.IsPlaying(gameObject))
        {
            tilemap = GetComponent<Tilemap>();

            if (so_gridProperties != null)
            {
                so_gridProperties.gridPropertyList.Clear();
            }
        }
    }

    private void OnDisable()
    {
        if (!Application.IsPlaying(gameObject))
        {
            UpdateGirdProperties();
        }

        if (so_gridProperties != null)
        {
            EditorUtility.SetDirty(so_gridProperties);
        }
    }

    private void UpdateGirdProperties()
    {
        //重新整理tilemap的边界，使其符合当前tilemap的大小
        tilemap.CompressBounds();

        if (so_gridProperties != null)
        {
            Vector3Int startCell = tilemap.cellBounds.min;
            Vector3Int endCell = tilemap.cellBounds.max;

            for (int x = startCell.x ; x < endCell.x; x++)
            {
                for (int y = startCell.y; y < endCell.y; y++)
                {
                    TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));

                    if (tile != null)
                    {
                        GridProperty gridProperty = new GridProperty(new GridCoordinate(x, y), gridBoolProperty, true);
                        so_gridProperties.gridPropertyList.Add(gridProperty);
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (!Application.IsPlaying(gameObject))
        {
            Debug.Log("DISABLED PROPERTY TILEMAPS");
        }
    }
}