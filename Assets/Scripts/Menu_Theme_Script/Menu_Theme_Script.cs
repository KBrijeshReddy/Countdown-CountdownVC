using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Theme_Script : MonoBehaviour
{
    public CanvasGroup goodTheme;
    public CanvasGroup evilTheme;
    public Image blackOverlay;
    public Image redBeam;   // good -> evil
    public Image goldBeam;  // evil -> good

    public float holdDuration = 4f;
    public float dimDuration = 1f;
    public int gutterFlickers = 4;
    public float blackHoldDuration = 0.4f;
    public float igniteSweepDuration = 0.5f;

    void Start()
    {
        SetAlpha(blackOverlay, 0f);
        SetAlpha(redBeam, 0f);
        SetAlpha(goldBeam, 0f);
        redBeam.fillAmount = 0f;
        goldBeam.fillAmount = 0f;
        StartCoroutine(RunLoop());
    }

    IEnumerator RunLoop()
    {
        bool showingGood = true;
        while (true)
        {
            yield return new WaitForSeconds(holdDuration);

            yield return StartCoroutine(Extinguish(showingGood ? goodTheme : evilTheme));
            yield return new WaitForSeconds(blackHoldDuration);

            CanvasGroup incoming = showingGood ? evilTheme : goodTheme;
            CanvasGroup outgoing = showingGood ? goodTheme : evilTheme;
            outgoing.alpha = 0f;
            incoming.alpha = 1f;

            // pick beam based on direction: good->evil = red, evil->good = gold
            Image beam = showingGood ? redBeam : goldBeam;
            yield return StartCoroutine(Ignite(beam));

            showingGood = !showingGood;
        }
    }

    IEnumerator Extinguish(CanvasGroup theme)
    {
        for (int i = 0; i < gutterFlickers; i++)
        {
            theme.alpha = Random.Range(0.3f, 1f);
            yield return new WaitForSeconds(dimDuration / (gutterFlickers * 2f));
        }
        float t = 0f;
        float startAlpha = theme.alpha;
        while (t < dimDuration)
        {
            t += Time.deltaTime;
            float p = t / dimDuration;
            theme.alpha = Mathf.Lerp(startAlpha, 0f, p);
            SetAlpha(blackOverlay, Mathf.Lerp(0f, 1f, p));
            yield return null;
        }
        theme.alpha = 0f;
        SetAlpha(blackOverlay, 1f);
    }

    IEnumerator Ignite(Image beam)
    {
        beam.fillAmount = 0f;
        SetAlpha(beam, 1f);

        float t = 0f;
        while (t < igniteSweepDuration)
        {
            t += Time.deltaTime;
            float p = t / igniteSweepDuration;
            beam.fillAmount = p;
            SetAlpha(blackOverlay, Mathf.Lerp(1f, 0f, p));
            yield return null;
        }
        beam.fillAmount = 1f;
        SetAlpha(blackOverlay, 0f);

        t = 0f;
        float fadeOutDuration = 0.4f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            SetAlpha(beam, Mathf.Lerp(1f, 0f, t / fadeOutDuration));
            yield return null;
        }
        SetAlpha(beam, 0f);
        beam.fillAmount = 0f;
    }

    void SetAlpha(Image img, float a)
    {
        Color c = img.color;
        c.a = a;
        img.color = c;
    }
}