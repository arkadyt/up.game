using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Wait out layer specific time, clean Tag, scale object back and deactivate it.
 * Assigned to every cloud obj.
 * 
 * Note: 
 * Object deactivation pauses all attached scripts and makes rigidbody
 * forget it's velocity.
 */
public class Reverter : MonoBehaviour
{

    private float timeSinceStart = 0;

    private CloudLayer cloudLayer;

    public void setON(CloudLayer cLayer)
    {
        CancelInvoke();
        timeSinceStart = 0;
        this.cloudLayer = cLayer;
        InvokeRepeating("checkingRoutine", 1.0f, 1.0f);
    }

    private void checkingRoutine()
    {
        timeSinceStart += 1.0f;

        // wait time depends on cloud layer
        switch (this.cloudLayer)
        {
            case CloudLayer.THIN:
                if (timeSinceStart >= CloudSpawner.Instance.thinObjLifeTime)
                {
                    revertAction();
                }
                break;
            case CloudLayer.FAR:
                if (timeSinceStart >= CloudSpawner.Instance.farObjLifeTime)
                {
                    revertAction();
                }
                break;
            case CloudLayer.MID:
                if (timeSinceStart >= CloudSpawner.Instance.midObjLifeTime)
                {
                    revertAction();
                }
                break;
            case CloudLayer.CLOSEST:
                if (timeSinceStart >= CloudSpawner.Instance.closestObjLifeTime)
                {
                    revertAction();
                }
                break;
            case CloudLayer.OVERLAY:
                if (timeSinceStart >= CloudSpawner.Instance.overlayObjLifeTime)
                {
                    revertAction();
                }
                break;
        }
    }

    private void revertAction()
    {
        CancelInvoke();

        gameObject.tag = CloudSpawner.Tag_Untagged;

        switch (cloudLayer)
        {
            case CloudLayer.THIN:
                gameObject.transform.localScale /= CloudSpawner.Instance.thinScale;
                break;
            case CloudLayer.FAR:
                gameObject.transform.localScale /= CloudSpawner.Instance.farScale;
                break;
            case CloudLayer.MID:
                gameObject.transform.localScale /= CloudSpawner.Instance.midScale;
                break;
            case CloudLayer.CLOSEST:
                gameObject.transform.localScale /= CloudSpawner.Instance.closestScale;
                break;
            case CloudLayer.OVERLAY:
                gameObject.transform.localScale /= CloudSpawner.Instance.overlayScale;
                break;
        }

        timeSinceStart = 0;

        gameObject.SetActive(false);
    }
}
