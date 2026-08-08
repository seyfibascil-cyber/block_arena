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
        Vector3 finalPosition = transform.position;

        Vector3 startScale = new Vector3(
            finalScale.x,
            startScaleY,
            finalScale.z
        );
        Vector3 startPosition = finalPosition - Vector3.up * 0.35f;

        transform.localScale = startScale;
        transform.position = startPosition;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress =
                elapsedTime / animationDuration;

            float smoothProgress = Mathf.SmoothStep(
                0f,
                1f,
                progress
            );

            float overshoot = Mathf.Sin(progress * Mathf.PI) * 0.08f;
            Vector3 animatedScale = Vector3.Lerp(
                startScale,
                finalScale,
                smoothProgress
            );
            animatedScale.y += overshoot;

            transform.localScale = animatedScale;
            transform.position = Vector3.Lerp(
                startPosition,
                finalPosition,
                smoothProgress
            );

            yield return null;
        }

        transform.localScale = finalScale;
        transform.position = finalPosition;
    }
}
