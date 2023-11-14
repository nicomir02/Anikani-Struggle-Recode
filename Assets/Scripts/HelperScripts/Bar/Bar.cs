using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class Bar : MonoBehaviour
{
    [SerializeField] protected float MaxValue; //Später durch Max Leben von Unit ersetzen 
    [SerializeField] protected float Value; //Momentane Wert (HealthManager)

    [SerializeField] protected RectTransform bottomBar;
    [SerializeField] protected RectTransform topBar;

    [SerializeField] protected float speed = 10f;

    protected float fullWidth;
    protected float targetWidth => Value * fullWidth / MaxValue;

    protected Coroutine adjustBarWidthCoroutine;

    public void Change(float amount) {
        newMaxValue();
        Debug.Log(amount);
        Value = Mathf.Clamp(Value+amount, 0, MaxValue); 
        Debug.Log(Value);
        //if(Value <= 0) gameObject.transform.parent.GetComponent<AudioUnit>().startAudio(2);
        if(adjustBarWidthCoroutine != null) {
            StopCoroutine(adjustBarWidthCoroutine);
        }

        adjustBarWidthCoroutine = StartCoroutine(AdjustBarWidth(amount));
    }

    public abstract void newMaxValue();

    private IEnumerator AdjustBarWidth(float amount) {
        RectTransform suddenChangeBar;
        RectTransform slowChangeBar;

        if(amount >= 0) {
            suddenChangeBar = bottomBar;
            slowChangeBar = topBar;
        }else {
            suddenChangeBar = topBar;
            slowChangeBar = bottomBar;
        }

        suddenChangeBar.SetWidth(targetWidth);
        while(Mathf.Abs(suddenChangeBar.rect.width - slowChangeBar.rect.width) > 0.1f) {
            slowChangeBar.SetWidth(Mathf.Lerp(slowChangeBar.rect.width, targetWidth, Time.deltaTime * speed));
            yield return null; //new WaitForSecondsRealtime(0.1f);
        }

        //UnitSprite us = gameObject.transform.parent.GetComponent<UnitSprite>();
        //if(Value <= 0) gameObject.transform.parent.GetComponent<SpriteRenderer>().color = GameObject.Find("GameManager").GetComponent<InputManager>().unsichtbar;
        //if(GameObject.Find("GameManager").GetComponent<RoundManager>().id == us.id) GameObject.Find("PlayerManager").GetComponent<UnitManager>().syncStillExists(us.vec);
        

        slowChangeBar.SetWidth(targetWidth);
    }
}