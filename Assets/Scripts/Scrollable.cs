using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Direction
{
    Upward, Downward
}

// sets object velocity considering chosen direction
public class Scrollable : MonoBehaviour
{
    private Rigidbody2D rb2d;

    [HideInInspector]
    public float scrollSpeed = 1.5f;
    [HideInInspector]
    public Direction scrollingDirection = Direction.Downward;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        reapplyVelocity();
    }

    // rigidbody forgets velocity on gameObject 'active' state change
    public void reapplyVelocity()
    {
        if (scrollingDirection.Equals(Direction.Upward))
        {
            rb2d.velocity = new Vector2(0, scrollSpeed);
        }
        else if (scrollingDirection.Equals(Direction.Downward))
        {
            rb2d.velocity = new Vector2(0, -scrollSpeed);
        }
    }

    void Update()
    {
        if (GameController.Instance.gameOver)
        {
            rb2d.velocity = Vector2.zero;
        }
    }
}
