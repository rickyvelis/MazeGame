using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITween : MonoBehaviour
{
    public LeanTweenType EaseType;
    private bool _closed;

    public void OnEnable()
    {
        _closed = false;
        transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(gameObject, new Vector3(1, 1, 1), 0.3f).setEase(EaseType);
    }

    public void OnClose()
    {
        _closed = true;
        LeanTween.scale(gameObject, new Vector3(0, 0, 0), 0.5f).setEase(EaseType);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_closed) OnClose();
            else OnEnable();
        }
    }
}
