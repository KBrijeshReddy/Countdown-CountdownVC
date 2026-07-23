using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 originalPosition;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        originalPosition = transform.localPosition;
    }

    public void Shake(float duration, float strength)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duration, strength));
    }

    private IEnumerator ShakeRoutine(float duration, float strength)
    {
        float timer = 0f;

        while (timer < duration)
        {
            Vector2 offset = Random.insideUnitCircle * strength;

            transform.localPosition =
                originalPosition +
                new Vector3(offset.x, offset.y, 0f);

            timer += Time.unscaledDeltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}