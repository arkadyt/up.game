using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameController : MonoBehaviour
{

    public static GameController Instance;

    public GameObject menuUI, inGameUI;

    private int score = 0;
    public Text scoreText;

    public bool gameOver = false;
    public GameObject gameOverText;

    // double-score prevention. unity 2d collider bug.
    private float timeSinceLastScored = 0f;

    private bool windBool = true;

    [Range(1f, 100f)]
    public float timeScale = 1f;

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

    void Update()
    {
        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void FixedUpdate()
    {
        // controlling time with inspector slider (debugging purposes)
        if (this.timeScale != Time.timeScale)
        {
            Time.timeScale = this.timeScale;
        }

        timeSinceLastScored += Time.fixedDeltaTime;

        if (score >= 3 && windBool)
        {
            WindEffect.Instance.setWindXForce(2.0f, 6.0f);
            windBool = false;
        }
    }

    public void EnterTheGame()
    {
        menuUI.SetActive(false);
        inGameUI.SetActive(true);
        GetComponent<DubinaSpawner>().enabled = true;
    }

    public void OnGameOver()
    {
        gameOver = true;
        gameOverText.SetActive(true);
    }

    public void OnPlayerScore()
    {
        if (gameOver)
        {
            return;
        }

        if (timeSinceLastScored < 1.0f)
        {
            return;
        }
        timeSinceLastScored = 0.0f;

        score++;
        scoreText.text = score.ToString();

        AudioSource mA = SoundPlayer.Instance.scored;
        mA.PlayOneShot(mA.clip);
    }
}