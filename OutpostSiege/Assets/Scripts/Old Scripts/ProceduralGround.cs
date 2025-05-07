using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    [Header("Tile Prefabs")]
    [SerializeField] private GameObject dirtTile;
    [SerializeField] private GameObject grassTile;
    [SerializeField] private GameObject transitionLeftTile;
    [SerializeField] private GameObject transitionRightTile;

    [Header("Generation Settings")]
    [SerializeField] private int xLimit = 50;      // From -xLimit to +xLimit
    [SerializeField] private int grassWidth = 10;  // Number of grass tiles in the center
    [SerializeField] private float tileHeight = 2;

    private float tileWidth;

    private void Start()
    {
        tileWidth = GetTileWidth();
        GenerateGround();
    }

    private float GetTileWidth()
    {
        if (dirtTile.TryGetComponent(out SpriteRenderer sr))
        {
            return sr.bounds.size.x;
        }
        else
        {
            Debug.LogWarning("Dirt tile missing SpriteRenderer. Defaulting tileWidth to 1.");
            return 1f;
        }
    }

    private void GenerateGround()
    {
        int startGrass = -grassWidth / 2;
        int endGrass = grassWidth / 2;

        for (int x = -xLimit; x <= xLimit; x++)
        {
            Vector3 position = new Vector3(x * tileWidth, tileHeight, 0f);
            GameObject tileToPlace;

            if (x < startGrass - 1)
                tileToPlace = dirtTile;
            else if (x == startGrass - 1)
                tileToPlace = transitionRightTile;
            else if (x >= startGrass && x <= endGrass)
                tileToPlace = grassTile;
            else if (x == endGrass + 1)
                tileToPlace = transitionLeftTile;
            else
                tileToPlace = dirtTile;

            Instantiate(tileToPlace, position, Quaternion.identity, transform);
        }
    }
}
