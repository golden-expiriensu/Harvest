using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PooledFlyingNumber : PooledObject
{
    public Animation disappearAnimation;
    private float disappearTime;
    private GameObject thisGameObject;
    public TextMeshProUGUI textTMPro;

    protected override void Awake()
    {
        base.Awake();
        thisGameObject = gameObject;
        textTMPro = GetComponentInChildren<TextMeshProUGUI>();
    }

    public override void OnPooled()
    {
        disappearTime = disappearAnimation.clip.length;
        StartCoroutine(Disappear());
    }

    private IEnumerator Disappear()
    {
        disappearAnimation.Play();
        yield return new WaitForSeconds(disappearTime);
        thisGameObject.SetActive(false);
    }
}
