using UnityEngine;

public class WindEffect : MonoBehaviour
{

    public static WindEffect Instance;

    private Vector2 windForce;
    private Rigidbody2D[] rBodies;

    private float windTimer = 0.0f;
    private float stopTime = 0.0f;
    private bool countTime = false;

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
        refreshObjects();
    }

    void FixedUpdate()
    {
        Debug.Log("wF = " + windForce.ToString());
        if (rBodies.Length > 0)
        {
            foreach (var item in rBodies)
            {
                item.AddForce(windForce);
            }
        }

        if (countTime)
        {
            if (windTimer > stopTime)
            {
                countTime = false;
                unsetWind();
            }
            windTimer += Time.fixedDeltaTime;
        }
    }

    public void setWindXForce(float wForce, float time)
    {
        Mathf.Clamp(wForce, -4, 4); // adequate limits
        this.windForce.x = wForce;

        if (refreshObjects())
        {
            stopTime = time;
            windTimer = 0.0f;

            countTime = true;
        }
    }

    public void setWindXForce(float wForce)
    {
        Mathf.Clamp(wForce, -4, 4);
        this.windForce.x = wForce;

        refreshObjects();
    }

    public void unsetWind()
    {
        this.windForce = Vector2.zero;
    }

    private bool refreshObjects()
    {
        GameObject[] gObjects = GameObject.FindGameObjectsWithTag("Player");

        if (gObjects.Length > 0)
        {
            rBodies = new Rigidbody2D[gObjects.Length];
            for (int i = 0; i < gObjects.Length; i++)
            {
                rBodies[i] = gObjects[i].GetComponent<Rigidbody2D>();
            }
            return true;
        }

        return false;
    }
}
