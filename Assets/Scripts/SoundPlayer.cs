using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// easy programmatical access to sound clips
public class SoundPlayer : MonoBehaviour
{

    public static SoundPlayer Instance;

    public AudioSource balloonBurst;
    public AudioSource scored;

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
}
