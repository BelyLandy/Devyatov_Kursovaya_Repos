using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Arrow_Pointer : MonoBehaviour
{
    [Tooltip("Пауза перед первым показом после убийства врага")]
    public float delayBeforeFirstShow = 2.5f;

    [Tooltip("Время, в течение которого указатель полностью видим")]
    public float visibleDuration = 2f;

    [Tooltip("Пауза между повторными показами")]
    public float repeatDelay = 5f;

    [Tooltip("Длительность плавного появления/исчезновения")]
    public float fadeDuration = 0.5f;

    private bool routineInProgress;
    private Image hand;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        hand = GetComponent<Image>();
        hand.enabled = false;
    }

    private void OnEnable()
    {
        HealthController.OnUnitDeath += HandPointerCheck;
    }

    private void OnDisable()
    {
        HealthController.OnUnitDeath -= HandPointerCheck;
    }

    private void HandPointerCheck(GameObject unit)
    {
        if (unit.CompareTag("Enemy") && !routineInProgress)
            StartCoroutine(ShowHandRoutine());
    }

    private IEnumerator ShowHandRoutine()
    {
        routineInProgress = true;

        yield return new WaitForSeconds(delayBeforeFirstShow);

        while (true)
        {
            if (EnemyManager.GetTotalEnemyCount() == 0 || EnemiesDetectedPlayer())
                break;

            // Появление с fade-ин
            yield return StartCoroutine(Fade(0f, 1f));

            yield return new WaitForSeconds(visibleDuration);

            // Исчезновение с fade-аут
            yield return StartCoroutine(Fade(1f, 0f));

            yield return new WaitForSeconds(repeatDelay);
        }

        hand.enabled = false;
        routineInProgress = false;
    }

    /// <summary>
    /// Плавно меняет альфу изображения от startAlpha к endAlpha за fadeDuration секунд.
    /// </summary>
    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (!hand.enabled) hand.enabled = true;

        Color c = hand.color;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            c.a = Mathf.Lerp(startAlpha, endAlpha, t);
            hand.color = c;
            yield return null;
        }

        c.a = endAlpha;
        hand.color = c;

        // Полностью скрыли — можно отключить компонент, чтобы не ловить клики
        if (Mathf.Approximately(endAlpha, 0f))
            hand.enabled = false;
    }

    private bool EnemiesDetectedPlayer()
    {
        foreach (GameObject enemy in EnemyManager.enemyList)
        {
            if (enemy && enemy.GetComponent<UnitActions>().targetSpotted)
                return true;
        }

        return false;
    }
}