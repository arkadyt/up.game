using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DubinaSpawner : MonoBehaviour
{

    public static DubinaSpawner Instance;

    [Header("Parameters")]
    public int poolSize = 5;
    public float spawnFreq = 7f;
    public float scrollSpeed = 3f;

    public float spawnXOffsetRange = 3f;

    [HideInInspector]
    public GameObject[] clubs; // pool
    public GameObject clubsPrefab;

    // offscreen initial spawn position.
    public Vector2 poolPrePosition = new Vector2(-70f, 50f);

    [Range(16f, 26f)]
    public float spawnYPos = 20f;
    private float timeSinceLastClubsSpawned;
    private int currentClub = 0;

    private float timeSinceRunStart;
    private int difficultyIncrements;

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
        clubs = new GameObject[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            clubs[i] = (GameObject)
                Instantiate(clubsPrefab, poolPrePosition, Quaternion.identity);
            clubs[i].SetActive(false);
        }

        // start obstacle flow now
        timeSinceLastClubsSpawned = spawnFreq - 0.1f;
    }

    void FixedUpdate()
    {
        timeSinceLastClubsSpawned += Time.fixedDeltaTime;
        timeSinceRunStart += Time.fixedDeltaTime;

        if (GameController.Instance.gameOver == false
            && timeSinceLastClubsSpawned > spawnFreq)
        {

            timeSinceLastClubsSpawned = 0;

            if (!clubs[currentClub].activeInHierarchy)
                clubs[currentClub].SetActive(true);

            float spawnXPos = Random.Range(-spawnXOffsetRange, spawnXOffsetRange);

            clubs[currentClub].GetComponent<Scrollable>()
                .scrollSpeed = this.scrollSpeed;

            clubs[currentClub].transform.position = new Vector2(spawnXPos, spawnYPos);

            currentClub++;

            if (currentClub > poolSize - 1)
            {
                currentClub = 0;
            }
        }

        // increase difficulty every 5 seconds
        if (timeSinceRunStart > difficultyIncrements * 5)
        {
            spawnFreq *= 0.95f;
            scrollSpeed *= 1.05f;
            difficultyIncrements++;
        }
    }
}
