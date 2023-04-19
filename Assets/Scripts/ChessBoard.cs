using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    [SerializeField] private int TILES_X, TILES_Y;

    [SerializeField] private Tile TilePrefab;

    [SerializeField] private Transform Camera;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for(int x = 0; x < TILES_X; x++)
        {
            for(int y = 0; y < TILES_Y; y++)
            {
                var spawnedTile = Instantiate(TilePrefab, new Vector3(x,y), Quaternion.identity);

                spawnedTile.name = $"Space x{x}, y{y}";

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0); 
                spawnedTile.init(isOffset);
            }
        }

        Camera.transform.position = new Vector3(4, 3, -1);

    }

}
