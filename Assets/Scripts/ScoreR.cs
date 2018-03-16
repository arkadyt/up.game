using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// applied to the gap in between spiky clubs.
public class ScoreR : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BalloonController>() != null)
        {
            GameController.Instance.OnPlayerScore();
        }
    }
}
