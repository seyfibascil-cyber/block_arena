using System.Collections;
using UnityEngine;

public class ObstacleAnimation : MonoBehaviour
{
    [Header("Animasyon Ayarları")]
    [SerializeField] private float animationDuration = 0.25f;
    [SerializeField] private float startScaleY = 0.05f;

    private void Start()
    {
        StartCoroutine(PlayAppearAnimation());
    }

    private IEnumerator PlayAppearAnimation()
    {
        Vector3 finalScale = transform.localScale;

        Vector3 startScale = new Vector3(
            finalScale.x,
            startScaleY,
            finalScale.z
        );

        transform.localScale = startScale;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress =
                elapsedTime / animationDuration;

            progress = Mathf.SmoothStep(
                0f,
                1f,
                progress
            );

            transform.localScale = Vector3.Lerp(
                startScale,
                finalScale,
                progress
            );

            yield return null;
        }

        transform.localScale = finalScale;
    }
}