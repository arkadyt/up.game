using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// splash screen behaviour
public class SplashController : MonoBehaviour
{

    public Animator splPanelAnimator;
    public Animator splImageAnimator;

    private float splashCounter = 0f;
    private bool conterStarted = false;

    private readonly int fadeOutHash = Animator.StringToHash("FadeOut");
    private readonly int fadeInHash = Animator.StringToHash("FadeIn");
    private readonly int exitHash = Animator.StringToHash("Exit");

    void Awake()
    {
        splashCounter = 0f;
        // conterStarted = true;
    }

    void FixedUpdate()
    {
        //// fading splash screen elements
        //if (splashCounter > 2.0f)
        //{
        //    // fade logo in
        //    splImageAnimator.SetTrigger(fadeInHash);

        //    if (splashCounter > 6.0f)
        //    {
        //        // fade logo out
        //        splImageAnimator.SetTrigger(fadeOutHash);

        //        if (splashCounter > 8.0f)
        //        {
        //            // fade out whole splash
        //            splPanelAnimator.SetTrigger(fadeOutHash);

        //            if (splashCounter > 12.0f)
        //            {
        //                conterStarted = false;
        //                splImageAnimator.SetTrigger(exitHash);
        //            }
        //        }
        //    }
        //}

        if (conterStarted)
            splashCounter += Time.deltaTime;
    }

}
