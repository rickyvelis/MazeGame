using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITween : MonoBehaviour
{
    public LeanTweenType EaseType;
    public float OnCloseDelay = 0.5f;
    public float OnEnableDelay = 0.3f;
    private bool _closed;

    /// <summary>
    /// What to do when UI Panel opens.
    /// </summary>
    public void OnEnable()
    {
        // Get the centeer of the screen.
        Vector3 center = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);

        // UI element isn't closed anymore, so set _closed to false.
        _closed = false;

        transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(gameObject, new Vector3(1, 1, 1), OnEnableDelay).setEase(EaseType);
        LeanTween.move(gameObject, center, OnEnableDelay).setEase(EaseType);
    }

    /// <summary>
    /// What to do when UI Panel closes.
    /// </summary>
    public void OnClose()
    {
        // UI element gets closed, so set _closed to true;
        _closed = true;

        LeanTween.scale(gameObject, new Vector3(0, 0, 0), OnCloseDelay).setEase(EaseType);
        LeanTween.move(gameObject, new Vector3(0, 0, 0), OnCloseDelay).setEase(EaseType);
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        // Close or open the UI Panel when 'Escape' is pressed.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_closed) OnClose();
            else OnEnable();
        }
    }
}
