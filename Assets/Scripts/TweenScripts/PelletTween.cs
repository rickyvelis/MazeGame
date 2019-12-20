using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PelletTween : MonoBehaviour
{
    public LeanTweenType EaseType;
    public float Delay;
    public float Duration;

    /// <summary>
    /// Loops animation on Pellet when it gets instantiated.
    /// </summary>
    private void OnEnable()
    {
        LeanTween.scale(gameObject, new Vector3(0.55f, 0.55f, 0.55f), Duration).setDelay(Delay).setLoopPingPong().setEase(EaseType);
    }
}
