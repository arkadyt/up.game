using UnityEngine;

// balloon behaviour
public class BalloonController : MonoBehaviour
{

    [Tooltip("Used for calculating bounds.")]
    public Camera mainCamera;
    private Bounds cameraBounds;
    private Bounds balloonBounds;

    [Tooltip("SlowDown gradient zone width starting from screen edge.")]
    [Range(2f, 6f)]
    public float gradientWidth = 4.0f;
    private float distInGradient = 0f;
    private bool hasBurst = false;
    private float horMoveAmount = 0f;

    // touchscreen vars
    private Vector2 startTap = new Vector2(5.2f, 15);
    private Vector2 currentTouchPos = Vector2.zero;
    private Touch myTouch;
    public float maxFollowDist = 1.1f;
    public GameObject anchorImg;
    public GameObject movedImg;

    // update animator thresholds if you change sideForce
    private float sideForce = 400f;

    private Rigidbody2D rb2d;
    private Animator anim;

    private readonly int xVelHash = Animator.StringToHash("xVelocity");

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        cameraBounds = CameraTools.OrthoBounds(mainCamera);
        balloonBounds = GetComponent<PolygonCollider2D>().bounds;
    }

    void Update()
    {
        if (!hasBurst)
        {
            // movement with keyboard
            horMoveAmount = Input.GetAxis("Horizontal");
            if (horMoveAmount > 0)
            {
                rb2d.AddForce(new Vector2(horMoveAmount * sideForce
                    * Time.deltaTime, 0.0f));
            }
            else if (horMoveAmount < 0)
            {
                rb2d.AddForce(new Vector2(horMoveAmount * sideForce
                    * Time.deltaTime, 0.0f));
            }
            // movement with touchscreen
            HandleTouchscreen();

            ApplySlowDown();

            // left/right tilt animation blending is controlled with vel.x
            anim.SetFloat(xVelHash, rb2d.velocity.x);
        }
    }

    void OnCollisionEnter2D(Collision2D colData)
    {
        SoundPlayer.Instance.balloonBurst.Play();

        hasBurst = true;
        gameObject.SetActive(false);
        // TODO: Spawn rubber flakes.
        GameController.Instance.OnGameOver();
    }

    /* Gradient movement constraints.
     * |----   O   ----|
     *     ^  and  ^ gradient constraints start.
     * 
     * As the object moves through the gradient, it's velocity is clamped to
     * the maximum allowed value which depends on how far the object is in.
     */
    private void ApplySlowDown()
    {
        // moves right
        if (horMoveAmount > 0 || rb2d.velocity.x > 0)
        {
            distInGradient = (rb2d.transform.position.x - gradientWidth);
            
            if (distInGradient > 0)
            {
                // distInGradient to a fraction number, calculated relative 
                // to the screen edge, considering balloon extents.
                distInGradient /= (cameraBounds.extents.x - gradientWidth - balloonBounds.extents.x);

                // inverse gradient
                distInGradient = Mathf.Clamp01(1 - distInGradient);

                // 4 is the top x velocity
                if (rb2d.velocity.x > 4 * distInGradient)
                {
                    float unclampedVel = rb2d.velocity.x;
                    rb2d.velocity = new Vector2(
                        4 * distInGradient,
                        0
                    );
                }
            }
        }
        // moves left
        else if (horMoveAmount < 0 || rb2d.velocity.x < 0)
        {
            distInGradient = -(rb2d.transform.position.x + gradientWidth);
            if (distInGradient > 0)
            {
                distInGradient /= (cameraBounds.extents.x - balloonBounds.extents.x - gradientWidth);
                distInGradient = Mathf.Clamp01(1 - distInGradient);
                if (rb2d.velocity.x < -4 * distInGradient)
                {
                    rb2d.velocity = new Vector2(
                        -4 * distInGradient,
                        0
                    );
                }
            }
        }

        // hard movement constraints
        transform.position = new Vector2(
            Mathf.Clamp(
                transform.position.x,
                -(cameraBounds.extents.x - balloonBounds.extents.x),
                cameraBounds.extents.x - balloonBounds.extents.x
            ),
            transform.position.y
        );
    }

    private void HandleTouchscreen()
    {
        if (Input.touchCount > 0)
        {
            myTouch = Input.GetTouch(0);

            if (myTouch.phase == TouchPhase.Began)
            {
                startTap = myTouch.position;
                anchorImg.transform.position = startTap;
            }

            if (myTouch.phase == TouchPhase.Ended)
            {
                // hide touch widgets
                anchorImg.transform.position = new Vector2(-500.0f, -500.0f);
                movedImg.transform.position = new Vector2(-500.0f, -500.0f);
            }

            currentTouchPos = myTouch.position;
            movedImg.transform.position = new Vector2(
                currentTouchPos.x,
                anchorImg.transform.position.y
            );

            if (currentTouchPos.x < startTap.x - maxFollowDist)
            {
                rb2d.AddForce(new Vector2(-sideForce
                    * Time.deltaTime, 0.0f));
            }
            else if (currentTouchPos.x > startTap.x + maxFollowDist)
            {
                rb2d.AddForce(new Vector2(sideForce
                    * Time.deltaTime, 0.0f));
            }
        }
    }
}
