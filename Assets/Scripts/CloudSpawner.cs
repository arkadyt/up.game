using UnityEngine;

public enum CloudLayer
{
    OVERLAY, THIN, FAR, MID, CLOSEST
}

public enum CloudType
{
    OVERLAY, THIN, NORMAL, LONG
}

// script regulates the flow of 14 different cloud types through
// different cloud layers.
public class CloudSpawner : MonoBehaviour
{

    public static CloudSpawner Instance;

    [Tooltip("Used for background start prefilling.")]
    public Camera mainCamera;
    private Bounds camBounds;

    public const string Tag_InUse = "Cloud_InUse";
    public const string Tag_Untagged = "Untagged";

    // SpriteRenderer requirement.
    public const string SL_NAME_THN = "BG_Clouds_Thin";
    public const string SL_NAME_FAR = "BG_Clouds_Far";
    public const string SL_NAME_MID = "BG_Clouds_Mid";
    public const string SL_NAME_CLS = "BG_Clouds_Closest";
    public const string SL_NAME_OVL = "FG_Clouds_Overlay";

    [Header("Clouds Pools")]
    public GameObject[] cloudsThinPrefabs;
    public GameObject[] cloudsNormalPrefabs;
    public GameObject[] cloudsLongPrefabs;
    public GameObject cloudOverlayPrefab;

    // prefab instances. 
    // working directly with passed prefabs modifies them on fs level.
    private GameObject[] cloudsThin;
    private GameObject[] cloudsNormal;
    private GameObject[] cloudsLong;
    private GameObject cloudOverlay;

    public Vector2 cloudsPoolsPrePosition = new Vector2(150f, 0f);

    [Header("Scroll speed by Layer")]
    public float thinScrollSpeed = 0.1f;
    public float overlayScrollSpeed = 60f;
    public float farScrollSpeed = 0.25f;
    public float midScrollSpeed = 0.6f;
    public float closestScrollSpeed = 1.7f;

    [Header("Spawn rate by Layer")]
    public float thinSpawnRate = 40f;
    public float overlaySpawnRate = 290f;
    public float farSpawnRate = 50f;
    public float midSpawnRate = 20f;
    public float closestSpawnRate = 112f;

    [Header("Life time by Layer")]
    public float thinObjLifeTime = 270f;
    public float overlayObjLifeTime = 14f;
    public float farObjLifeTime = 120f;
    public float midObjLifeTime = 58f;
    public float closestObjLifeTime = 34f;

    [Header("X spawn range by Layer")]
    public float thinXSpawnRange = -6f;
    public float overlayXSpawnRange = 100f;
    public float farXSpawnRange = -6.9f;
    public float midXSpawnRange = 8f;
    public float closestXSpawnRange = -21f;

    [Header("Y spawn position by Layer")]
    public float thinYSpawnPos = 14f;
    public float overlayYSpawnPos = 330f;
    public float farYSpawnPos = 15f;
    public float midYSpawnPos = 18f;
    public float closestYSpawnPos = 30f;

    [Header("Scale by Layer")]
    public float thinScale = 0.5f;
    public float overlayScale = 1f;
    public float farScale = 0.4f;
    public float midScale = 1f;
    public float closestScale = 5f;

    private float thinLayerCounter = 0;
    private float overlayLayerCounter = 0;
    private float farLayerCounter = 0;
    private float midLayerCounter = 0;
    private float closestLayerCounter = 0;

    private GameObject currentCloud;
    private bool foundNew = false;

    void Awake()
    {
        // singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        camBounds = CameraTools.OrthoBounds(mainCamera);

        cloudsLong = new GameObject[cloudsLongPrefabs.Length];
        cloudsNormal = new GameObject[cloudsNormalPrefabs.Length];
        cloudsThin = new GameObject[cloudsThinPrefabs.Length];

        // fill object pools
        for (int i = 0; i < cloudsLong.Length; i++)
        {
            cloudsLong[i] = Instantiate(cloudsLongPrefabs[i], cloudsPoolsPrePosition, Quaternion.identity);
        }
        for (int i = 0; i < cloudsNormal.Length; i++)
        {
            cloudsNormal[i] = Instantiate(cloudsNormalPrefabs[i], cloudsPoolsPrePosition, Quaternion.identity);
        }
        for (int i = 0; i < cloudsThin.Length; i++)
        {
            cloudsThin[i] = Instantiate(cloudsThinPrefabs[i], cloudsPoolsPrePosition, Quaternion.identity);
        }
        cloudOverlay = Instantiate(cloudOverlayPrefab, cloudsPoolsPrePosition, Quaternion.identity);

        // start cloud flow faster
        thinLayerCounter = thinSpawnRate - 0.1f;
        farLayerCounter = farSpawnRate - 0.1f;
        closestLayerCounter = closestSpawnRate - 0.1f;

        prefillScreenWithClouds();
    }

    void FixedUpdate()
    {
        thinLayerCounter += Time.fixedDeltaTime;
        farLayerCounter += Time.fixedDeltaTime;
        midLayerCounter += Time.fixedDeltaTime;
        closestLayerCounter += Time.fixedDeltaTime;
        overlayLayerCounter += Time.fixedDeltaTime;

        if (thinLayerCounter > thinSpawnRate)
        {
            thinLayerCounter = 0f;
            spawnCloud(CloudLayer.THIN, CloudType.THIN);
        }

        if (overlayLayerCounter > overlaySpawnRate)
        {
            overlayLayerCounter = 0f;
            spawnCloud(CloudLayer.OVERLAY, CloudType.OVERLAY);
        }

        if (closestLayerCounter > closestSpawnRate)
        {
            closestLayerCounter = 0f;
            spawnCloud(CloudLayer.CLOSEST, CloudType.NORMAL);
        }

        if (midLayerCounter > midSpawnRate)
        {
            midLayerCounter = 0f;
            spawnCloud(CloudLayer.MID, CloudType.NORMAL);
        }

        if (farLayerCounter > farSpawnRate)
        {
            farLayerCounter = 0f;

            if (Random.Range(0, 3) == 2)
            {
                // 33% chance hit
                spawnCloud(CloudLayer.FAR, CloudType.LONG);
            }
            else
            {
                spawnCloud(CloudLayer.FAR, CloudType.NORMAL);
            }

        }
    }

    private void spawnCloud(CloudLayer cloudLayer, CloudType cloudType,
        float yPosition)
    {

        float tempY = 0.0f;

        switch (cloudLayer)
        {
            case CloudLayer.THIN:
                tempY = thinYSpawnPos;
                thinYSpawnPos = yPosition;
                spawnCloud(cloudLayer, cloudType);
                thinYSpawnPos = tempY;
                break;
            case CloudLayer.FAR:
                tempY = farYSpawnPos;
                farYSpawnPos = yPosition;
                spawnCloud(cloudLayer, cloudType);
                farYSpawnPos = tempY;
                break;
        }
    }

    private void spawnCloud(CloudLayer cloudLayer, CloudType cloudType)
    {
        // pick a cloud from the respective pool
        switch (cloudType)
        {
            case CloudType.THIN:
                if (Random.Range(0, 10) == 6)
                    break;

                for (int i = 0; i < cloudsThin.Length; i++)
                {
                    // actually try to pick
                    if (!cloudsThin[i].CompareTag(CloudSpawner.Tag_InUse))
                    {
                        currentCloud = cloudsThin[i];
                        foundNew = true;

                        // make it non-linear
                        if (Random.Range(0, 3) == 2)
                        {
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                break;
            case CloudType.LONG:
                for (int i = 0; i < cloudsLong.Length; i++)
                {
                    if (!cloudsLong[i].CompareTag(CloudSpawner.Tag_InUse))
                    {
                        currentCloud = cloudsLong[i];
                        foundNew = true;
                        if (Random.Range(0, 3) == 2)
                        {
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                break;
            case CloudType.NORMAL:
                for (int i = 0; i < cloudsNormal.Length; i++)
                {
                    if (!cloudsNormal[i].CompareTag(CloudSpawner.Tag_InUse))
                    {
                        currentCloud = cloudsNormal[i];
                        foundNew = true;
                        if (Random.Range(0, 3) == 2)
                        {
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                break;
            case CloudType.OVERLAY:
                if (!cloudOverlay.CompareTag(CloudSpawner.Tag_InUse))
                {
                    currentCloud = cloudOverlay;
                    foundNew = true;
                }
                break;
        }

        // then spawn it
        if (foundNew)
        {
            foundNew = false;
            currentCloud.SetActive(true);
            currentCloud.tag = CloudSpawner.Tag_InUse;

            switch (cloudLayer)
            {
                case CloudLayer.THIN:
                    currentCloud.transform.localScale *= thinScale;
                    currentCloud.transform.position = new Vector2(
                        Random.Range(-thinXSpawnRange, thinXSpawnRange),
                        thinYSpawnPos);
                    currentCloud.GetComponent<SpriteRenderer>()
                        .sortingLayerName = CloudSpawner.SL_NAME_THN;
                    currentCloud.GetComponent<Scrollable>()
                        .scrollSpeed = CloudSpawner.Instance.thinScrollSpeed;
                    currentCloud.GetComponent<Scrollable>().reapplyVelocity();
                    currentCloud.GetComponent<Reverter>().setON(CloudLayer.THIN);
                    break;
                case CloudLayer.FAR:
                    currentCloud.transform.localScale *= farScale;
                    currentCloud.transform.position = new Vector2(
                            Random.Range(-farXSpawnRange, farXSpawnRange),
                            farYSpawnPos);
                    currentCloud.GetComponent<SpriteRenderer>()
                        .sortingLayerName = CloudSpawner.SL_NAME_FAR;
                    currentCloud.GetComponent<Scrollable>()
                        .scrollSpeed = CloudSpawner.Instance.farScrollSpeed;
                    currentCloud.GetComponent<Scrollable>().reapplyVelocity();
                    currentCloud.GetComponent<Reverter>().setON(CloudLayer.FAR);
                    break;
                case CloudLayer.MID:
                    currentCloud.transform.localScale *= midScale;
                    currentCloud.transform.position = new Vector2(
                        Random.Range(-midXSpawnRange, midXSpawnRange),
                        midYSpawnPos);
                    currentCloud.GetComponent<SpriteRenderer>()
                        .sortingLayerName = CloudSpawner.SL_NAME_MID;
                    currentCloud.GetComponent<Scrollable>()
                        .scrollSpeed = CloudSpawner.Instance.midScrollSpeed;
                    currentCloud.GetComponent<Scrollable>().reapplyVelocity();
                    currentCloud.GetComponent<Reverter>().setON(CloudLayer.MID);
                    break;
                case CloudLayer.CLOSEST:
                    currentCloud.transform.localScale *= closestScale;
                    currentCloud.transform.position = new Vector2(
                        Random.Range(-closestXSpawnRange, closestXSpawnRange),
                        closestYSpawnPos);
                    currentCloud.GetComponent<SpriteRenderer>()
                        .sortingLayerName = CloudSpawner.SL_NAME_CLS;
                    currentCloud.GetComponent<Scrollable>()
                        .scrollSpeed = CloudSpawner.Instance.closestScrollSpeed;
                    currentCloud.GetComponent<Scrollable>().reapplyVelocity();
                    currentCloud.GetComponent<Reverter>().setON(CloudLayer.CLOSEST);
                    break;
                case CloudLayer.OVERLAY:
                    currentCloud.transform.localScale *= overlayScale;
                    currentCloud.transform.position = new Vector2(
                        Random.Range(-overlayXSpawnRange, overlayXSpawnRange),
                        overlayYSpawnPos);
                    foreach (SpriteRenderer SR in currentCloud.GetComponentsInChildren<SpriteRenderer>())
                    {
                        SR.sortingLayerName = CloudSpawner.SL_NAME_OVL;
                    }
                    currentCloud.GetComponent<Scrollable>()
                        .scrollSpeed = CloudSpawner.Instance.overlayScrollSpeed;
                    currentCloud.GetComponent<Scrollable>().reapplyVelocity();
                    currentCloud.GetComponent<Reverter>().setON(CloudLayer.OVERLAY);
                    break;

            }

            // flip horizontally for greater diversity
            if (currentCloud.GetComponent<SpriteRenderer>() != null)
            {
                if (Random.Range(0, 2) == 1)
                {
                    currentCloud.GetComponent<SpriteRenderer>().flipX =
                        !currentCloud.GetComponent<SpriteRenderer>().flipX;
                }
            }
            else
            {
                // it's OverlayCloudPrefab
                if (Random.Range(0, 2) == 1)
                {
                    foreach (SpriteRenderer SR in currentCloud.GetComponentsInChildren<SpriteRenderer>())
                    {
                        SR.flipX = !SR.flipX;
                    }
                }
            }
        }
    }

    private void prefillScreenWithClouds()
    {
        float step = thinSpawnRate * thinScrollSpeed;
        float currentY = thinYSpawnPos;

        while (currentY - step > camBounds.min.y)
        {
            currentY -= step;
            spawnCloud(CloudLayer.THIN, CloudType.THIN, currentY);
        }

        step = farSpawnRate * farScrollSpeed;
        currentY = farYSpawnPos;

        while (currentY - step > camBounds.min.y)
        {
            currentY -= step;
            spawnCloud(CloudLayer.FAR, CloudType.NORMAL, currentY);
        }
    }
}