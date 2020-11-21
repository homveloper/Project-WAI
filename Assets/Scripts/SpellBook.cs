using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellBook : MonoBehaviour
{
    [SerializeField]
    public Image castingBar;
    public Text castTime;

    public CanvasGroup canvasGroup;
    public Coroutine spellRoutine;
    public Coroutine fadeRoutine;

    public float castingTime = 3;

    void Start()
    {
        CastSpell();
    }
    public void CastSpell()
    {
        castingBar.fillAmount = 0;
        spellRoutine = StartCoroutine(Progress());
        fadeRoutine = StartCoroutine(FadeBar());
    }
    private IEnumerator Progress()
    {
        float timePassed = Time.deltaTime;
        float rate = 1.0f/castingTime;
        float progress = 0.0f;

        while(progress <= 1.0f)
        {
            castingBar.fillAmount = Mathf.Lerp(0,1,progress);
            progress += rate * Time.deltaTime;
            timePassed += Time.deltaTime;

            castTime.text = (castingTime - timePassed).ToString("F2");

            if(castingTime - timePassed < 0)
            {
                castTime.text = "0.00";
            } 

            yield return null;
        }

        StopCasting();
    }

    private IEnumerator FadeBar()
    {  
        float rate = 1.0f /  0.50f;
        float progress = 0.0f;

        while(progress <= 1.0)
        {
            canvasGroup.alpha = Mathf.Lerp(0,1,progress);
            progress += rate * Time.deltaTime;

            yield return null;
        }
    }

    public void StopCasting()
    {
        if(fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            canvasGroup.alpha = 0;
            fadeRoutine = null;
        }

        if(spellRoutine != null)
        {
            StopCoroutine(spellRoutine);
            spellRoutine = null;
        }
    }
}
