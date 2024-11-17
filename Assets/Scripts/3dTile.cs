using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class CustomTile : TileBase
{
    public GameObject prefab;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        // tileData.flags = TileFlags.LockAll;
        tileData.colliderType = Tile.ColliderType.Grid; 
        tileData.gameObject = prefab;


    }
}
