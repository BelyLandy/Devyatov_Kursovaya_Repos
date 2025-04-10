using UnityEngine;
using System.Collections;

namespace CW_Devyatov_238
{
    /// <summary>
    /// Управляет здоровьем, уроном и визуальными/аудио эффектами игровых объектов.
    /// </summary>
    public class HealthController : MonoBehaviour
    {
        public int MaxHealth = 1;
        public int CurrentHealth = 1;
        public bool Invulnerable;
        public bool IsDead => (CurrentHealth == 0);
        public float HealthPercent => (float)CurrentHealth / MaxHealth;

        [Header("Настройки индикатора здоровья")]
        public bool DisplayMiniHealthBar;
        public Vector2 MiniHealthBarOffset = Vector2.zero;
        public bool DisplayLargeHealthBar;
        private GameObject healthBarInstance;

        [Header("Звуковые эффекты")]
        public string SFXOnHit = "";
        public string SFXOnDestroy = "";

        [Header("Эффекты")]
        public bool EnableHitFlash = true;
        public float HitFlashDuration = 0.15f;
        private bool hitFlashActive;

        [Space(10)]
        public bool EnableShakeEffect;
        public float ShakeIntensity = 0.08f;
        public float ShakeDuration = 0.5f;
        public float ShakeSpeed = 50f;

        [Space(10)]
        public GameObject HitEffectPrefab;
        public GameObject DestroyEffectPrefab;

        public bool IsPlayer => gameObject.CompareTag("Player");
        public bool IsEnemy => gameObject.CompareTag("Enemy");

        public delegate void HealthChangeEvent(HealthController hc);
        public static event HealthChangeEvent OnHealthChanged;
        public delegate void UnitDeathEvent(GameObject unit);
        public static event UnitDeathEvent OnUnitDeath;

        /// <summary>
        /// Регистрирует объекты врагов при активации.
        /// </summary>
        void OnEnable()
        {
            if (IsEnemy)
                EnemyManager.AddEnemyToList(gameObject);
        }

        /// <summary>
        /// Убирает объекты врагов из списка при деактивации.
        /// </summary>
        void OnDisable()
        {
            if (IsEnemy)
                EnemyManager.RemoveEnemyFromList(gameObject);
        }

        /// <summary>
        /// Инициализирует систему здоровья.
        /// </summary>
        void Start()
        {
            if (DisplayMiniHealthBar)
                InitializeMiniHealthBar();

            if (IsPlayer && OnHealthChanged != null)
                OnHealthChanged(this);
        }

        /// <summary>
        /// Создает и позиционирует мини-индикатор здоровья.
        /// </summary>
        void InitializeMiniHealthBar()
        {
            if (healthBarInstance == null)
            {
                healthBarInstance = GameObject.Instantiate(Resources.Load("HealthBar")) as GameObject;
                if (healthBarInstance == null)
                    return;
                healthBarInstance.transform.parent = transform;
                healthBarInstance.transform.position = transform.position + (Vector3)MiniHealthBarOffset;
                healthBarInstance.transform.GetChild(0).transform.localScale = new Vector3((float)CurrentHealth / MaxHealth, 1, 1);
            }
        }

        /// <summary>
        /// Уменьшает здоровье на указанное значение и применяет эффекты.
        /// </summary>
        public void SubtractHealth(int damage)
        {
            if (!Invulnerable)
                CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);

            BroadcastHealthChange();

            if (!Invulnerable && healthBarInstance != null)
                healthBarInstance.transform.GetChild(0).transform.localScale = new Vector3((float)CurrentHealth / MaxHealth, 1, 1);

            if (CurrentHealth > 0)
                CW_Devyatov_238.AudioController.PlaySFX(SFXOnHit, transform.position);
            else
                CW_Devyatov_238.AudioController.PlaySFX(SFXOnDestroy, transform.position);

            if (EnableHitFlash)
                StartCoroutine(ExecuteHitFlash());

            if (EnableShakeEffect && !IsDead)
            {
                StopCoroutine(ExecuteShake());
                StartCoroutine(ExecuteShake());
            }

            if (IsDead)
            {
                if (DestroyEffectPrefab != null)
                    TriggerEffect(DestroyEffectPrefab);

                if (IsEnemy || IsPlayer)
                    OnUnitDeath?.Invoke(gameObject);
                else
                    Destroy(gameObject);
            }
            else
            {
                if (HitEffectPrefab != null)
                    TriggerEffect(HitEffectPrefab);
            }
        }

        /// <summary>
        /// Увеличивает здоровье на заданное значение.
        /// </summary>
        public void AddHealth(int amount)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
            BroadcastHealthChange();
        }

        /// <summary>
        /// Оповещает слушателей об изменении здоровья.
        /// </summary>
        private void BroadcastHealthChange()
        {
            float currentPercentage = (1f / MaxHealth) * CurrentHealth;
            if (OnHealthChanged != null)
                OnHealthChanged(this);
        }

        /// <summary>
        /// Выполняет временный эффект вспышки при получении удара.
        /// </summary>
        private IEnumerator ExecuteHitFlash()
        {
            if (hitFlashActive)
                yield break;

            hitFlashActive = true;
            SpriteRenderer spriteRend = GetComponent<SpriteRenderer>();
            if (spriteRend == null)
                yield break;

            Material originalMat = spriteRend.material;
            spriteRend.material = Resources.Load("HitFlashMat") as Material;
            yield return new WaitForSeconds(HitFlashDuration);
            spriteRend.material = originalMat;
            hitFlashActive = false;
        }

        /// <summary>
        /// Применяет эффект горизонтального тряски к объекту.
        /// </summary>
        private IEnumerator ExecuteShake()
        {
            Vector3 startPos = transform.position;
            float t = 0;
            while (t < 1)
            {
                transform.position = Vector3.Lerp(startPos + (Vector3.left * ShakeIntensity / 2),
                    startPos + (Vector3.right * ShakeIntensity / 2), Mathf.Sin(t * ShakeSpeed));
                t += Time.deltaTime / ShakeDuration;
                yield return null;
            }
            transform.position = startPos;
        }

        /// <summary>
        /// Обновляет позицию и наличие индикатора здоровья во время проверки в редакторе.
        /// </summary>
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                if (DisplayMiniHealthBar && healthBarInstance == null)
                    InitializeMiniHealthBar();
                if (healthBarInstance != null)
                    healthBarInstance.transform.position = transform.position + (Vector3)MiniHealthBarOffset;
                if (healthBarInstance != null && !DisplayMiniHealthBar)
                    Destroy(healthBarInstance);
            }
        }

        /// <summary>
        /// Создает визуальный эффект в позиции объекта.
        /// </summary>
        public void TriggerEffect(GameObject effectPrefab)
        {
            if (effectPrefab == null)
                return;

            GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity) as GameObject;
            if (effect != null)
            {
                SpriteRenderer effectSR = effect.GetComponent<SpriteRenderer>();
                SpriteRenderer unitSR = GetComponent<SpriteRenderer>();
                if (effectSR != null && unitSR != null)
                    effectSR.sortingOrder = unitSR.sortingOrder + 1;
            }
        }
    }
}
