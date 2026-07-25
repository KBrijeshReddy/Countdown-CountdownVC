using UnityEngine;
using System.Collections;

public class TimeFreeze : MonoBehaviour
{
    public static TimeFreeze Instance;

    private Coroutine freezeRoutine;

    private void Awake()
    {
        Instance = this;
    }

    public void Freeze(float duration)
    {
        if (freezeRoutine != null)
            StopCoroutine(freezeRoutine);

        freezeRoutine =
            StartCoroutine(
                FreezeRoutine(duration));
    }

    private IEnumerator FreezeRoutine(float duration)
    {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;

        freezeRoutine = null;
    }

    public void FreezeThen(float duration, MonoBehaviour caller, IEnumerator action)
    {
        if (freezeRoutine != null)
            StopCoroutine(freezeRoutine);

        freezeRoutine =
            StartCoroutine(
                FreezeThenRoutine(duration, caller, action));
    }

    private IEnumerator FreezeThenRoutine(
        float duration,
        MonoBehaviour caller,
        IEnumerator action)
    {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;

        freezeRoutine = null;

        caller.StartCoroutine(action);
    }
}