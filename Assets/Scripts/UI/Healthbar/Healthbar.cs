using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField]
    private Image BottomBar;

    [SerializeField]
    private Image TopBar;

    [SerializeField]
    private float AnimationSpeed = 1f;


    private float _maxvalue;
    private float _value;

    private Coroutine _coroutine;

    public void Initialize(float maxvalue, float value)
    {
        _maxvalue = maxvalue;
        _value = value;
    }

    public void Change(float delta)
    {
        _value = Mathf.Clamp(_value + delta, 0, _maxvalue);

        if(_coroutine != null ) { StopCoroutine(_coroutine); }

        _coroutine = StartCoroutine(ChangeBars(delta));
    }

    private IEnumerator ChangeBars(float delta)
    {
        var directChangebar = delta >= 0 ? BottomBar : TopBar;
        var animateChangebar = delta >= 0 ? TopBar : BottomBar;

        var targetValue = _value / _maxvalue;

        directChangebar.fillAmount = targetValue;

        while(Mathf.Abs(animateChangebar.fillAmount - targetValue) > 0.01f)
        {
            animateChangebar.fillAmount = Mathf.MoveTowards(animateChangebar.fillAmount, targetValue, Time.deltaTime * AnimationSpeed);
            yield return null;
        }

        animateChangebar.fillAmount = targetValue;
    }
}
