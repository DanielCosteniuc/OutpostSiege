using UnityEngine;

public class GroundGenerator: MonoBehaviour
{
    [Header("Tile Prefabs")]
    [SerializeField] private GameObject Ground_Dirt;
    [SerializeField] private GameObject Ground_Grass_Left;
    [SerializeField] private GameObject Ground_Grass_Right;
    [SerializeField] private GameObject Ground_Grass_Between;

    [Header("Generation Settings")]
    [SerializeField] private int groundLength = 50;
    [SerializeField] private float groundHeight = -5f;

    private float tileWidth;

    private void Start()
    {
        tileWidth = GetTileWidth();
        GenerateGroundSide(true);  // Right
        GenerateGroundSide(false); // Left
    }

    private float GetTileWidth()
    {
        if (Ground_Dirt.TryGetComponent(out SpriteRenderer sr))
            return sr.bounds.size.x;
        else
        {
            Debug.LogWarning("Ground_Dirt is missing a SpriteRenderer. Defaulting to 1.");
            return 1f;
        }
    }

    private void GenerateGroundSide(bool isRight)
    {
        for (int i = 0; i < groundLength;)
        {
            float direction = isRight ? 1 : -1;
            float xPos = i * tileWidth * direction;

            if (i <= groundLength - 3 && Random.value > 0.5f)
            {
                bool hasMiddle = false;

                // Left or Right cap
                Instantiate(
                    isRight ? Ground_Grass_Left : Ground_Grass_Right,
                    new Vector3(xPos, groundHeight, 0),
                    Quaternion.identity,
                    transform
                );
                xPos += tileWidth * direction;

                int grassBetweenCount = Random.Range(2, 7);
                for (int j = 0; j < grassBetweenCount; j++)
                {
                    Instantiate(Ground_Grass_Between, new Vector3(xPos, groundHeight, 0), Quaternion.identity, transform);
                    xPos += tileWidth * direction;
                    hasMiddle = true;
                }

                if (hasMiddle)
                {
                    Instantiate(
                        isRight ? Ground_Grass_Right : Ground_Grass_Left,
                        new Vector3(xPos, groundHeight, 0),
                        Quaternion.identity,
                        transform
                    );
                    xPos += tileWidth * direction;
                }

                i += (2 + grassBetweenCount);

                int dirtCount = Random.Range(1, 3);
                for (int j = 0; j < dirtCount && i < groundLength; j++)
                {
                    Instantiate(Ground_Dirt, new Vector3(xPos, groundHeight, 0), Quaternion.identity, transform);
                    xPos += tileWidth * direction;
                    i++;
                }
            }
            else
            {
                Instantiate(Ground_Dirt, new Vector3(xPos, groundHeight, 0), Quaternion.identity, transform);
                i++;
            }
        }
    }
}
