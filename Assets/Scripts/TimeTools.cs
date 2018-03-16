using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTools : MonoBehaviour
{

    public static TimeTools Instance;

    private float previousTimeScale = 1f;
    private float newTimeScale = 1f;
    private float timePeriod = 1f;

    private float revertTimeScaleCounter = 0f;
    private bool counterStarted = false;

    void Awake()
    {
        // singleton
        // have to inherit from MonoBehaviour to play with time
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (counterStarted)
            revertTimeScaleCounter += Time.fixedDeltaTime;
    }

    public void changeGameSpeed(float newTimeScale, float timePeriod)
    {
        if (newTimeScale > 100 || newTimeScale < 0.01f)
        {
            Debug.LogError("Wrong timeScale specified. Accepted range is: 0.01f to 100f.");
            return;
        }

        this.newTimeScale = newTimeScale;
        this.timePeriod = timePeriod;

        counterStarted = true;
        revertTimeScaleCounter = 0f;
        Time.timeScale = newTimeScale;

        // time will go X times faster, multiply it by X
        InvokeRepeating("launchTimeReverter", 0.5f * newTimeScale
            , 0.5f * newTimeScale);
    }

    public void changeGameSpeed(float newTimeScale)
    {
        if (newTimeScale > 100 || newTimeScale < 0.01f)
        {
            Debug.LogError("Wrong timeScale specified. Accepted range is: 0.01f to 100f.");
            return;
        }

        Time.timeScale = newTimeScale;
    }

    private void launchTimeReverter()
    {
        // time will go X times faster multiply it by X!
        if (revertTimeScaleCounter >= this.timePeriod * this.newTimeScale)
        {
            // break the loop
            CancelInvoke();
            counterStarted = false;
            Time.timeScale = previousTimeScale;
        }
    }
}
