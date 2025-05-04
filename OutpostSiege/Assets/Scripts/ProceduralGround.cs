using UnityEngine;

public class GroundGenerator : MonoBehaviour
{
    [SerializeField] private GameObject Ground_Dirt;
    [SerializeField] private GameObject Ground_Grass_Left;
    [SerializeField] private GameObject Ground_Grass_Right;
    [SerializeField] private GameObject Ground_Grass_Between;

    [SerializeField] private int groundLength = 5;
    [SerializeField] private float tileSize = 2.0f;
    [SerializeField] private float groundHeight = -5f;
    public void SetGroundLength(int value)
    {
        groundLength += value;
    }


    void Start()
    {
        SetGroundLength(6);

        GenerateGroundRight();
        GenerateGroundLeft();
        //adauga coliziunea

        float endRight = groundLength*tileSize - (6 * tileSize);
        float endLeft = -groundLength* tileSize + (6 * tileSize);

        CreateEdgeBarrier2D(endRight);  // Barieră dreapta
        CreateEdgeBarrier2D(endLeft);   // Barieră stânga
    }
    

    //genereaza groundul in dreapta
    void GenerateGroundRight()
    {
        
        for (int i = 0; i < groundLength;)
        {
            float xPos = i * tileSize;

            if (i <= groundLength - 3 && Random.value > 0.5f)
            {
                bool ok = false;

                Instantiate(Ground_Grass_Left, new Vector3(xPos, groundHeight, 0), Quaternion.identity, transform);
                xPos += tileSize;

                int grassBetweenCount = Random.Range(2, 7);
                for (int j = 0; j < grassBetweenCount; j++)
                {
                    Instantiate(Ground_Grass_Between, new Vector3(xPos, groundHeight, 0), Quaternion.identity, transform);
                    xPos += tileSize;
                    ok = true;
                }

                if (ok)
                {
                    Instantiate(Ground_Grass_Right, new Vector3(xPos, groundHeight, 0), Quaternion.identity, transform);
                    xPos += tileSize;
                }

                i += (2 + grassBetweenCount);

                int dirtCount = Random.Range(1, 3);
                for (int j = 0; j < dirtCount && i < groundLength; j++)
                {
                    Instantiate(Ground_Dirt, new Vector3(xPos, groundHeight, 0), Quaternion.identity, transform);
                    xPos += tileSize;
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

    //genereaza groundul in stanga
    //se inverseaza stanga cu dreapta
    void GenerateGroundLeft()
    {
        for (int i = 0; i < groundLength;)
        {
            float xPos = i * tileSize;

            if (i <= groundLength - 3 && Random.value > 0.5f)
            {
                bool ok = false;

                Instantiate(Ground_Grass_Right, new Vector3(-xPos, groundHeight, 0), Quaternion.identity, transform);
                xPos += tileSize;

                int grassBetweenCount = Random.Range(2, 7);
                for (int j = 0; j < grassBetweenCount; j++)
                {
                    Instantiate(Ground_Grass_Between, new Vector3(-xPos, groundHeight, 0), Quaternion.identity, transform);
                    xPos += tileSize;
                    ok = true;
                }

                if (ok)
                {
                    Instantiate(Ground_Grass_Left, new Vector3(-xPos, groundHeight, 0), Quaternion.identity, transform);
                    xPos += tileSize;
                }

                i += (2 + grassBetweenCount);

                int dirtCount = Random.Range(1, 3);
                for (int j = 0; j < dirtCount && i < groundLength; j++)
                {
                    Instantiate(Ground_Dirt, new Vector3(-xPos, groundHeight, 0), Quaternion.identity, transform);
                    xPos += tileSize;
                    i++;
                }
            }
            else
            {
                Instantiate(Ground_Dirt, new Vector3(-xPos, groundHeight, 0), Quaternion.identity, transform);
                i++;
            }
        }
    }
    //funcie car pune coliziuni sa nu cada de pe mapa playerul
    void CreateEdgeBarrier2D(float xPosition)
    {
        GameObject barrier = new GameObject("EdgeBarrier2D");
        BoxCollider2D collider = barrier.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1f, 20f); // 1 unitate lățime, 20 unități înălțime
        barrier.transform.position = new Vector2(xPosition, groundHeight + 10f); // Centrat vertical
        barrier.transform.parent = transform;
    }


}
