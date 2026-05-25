using UnityEngine;
using System.Collections.Generic;

public class AnomalyLogic : MonoBehaviour
{
    public enum AnomalyType { SwapObjects, JustSpawn, ScaleObject, TriggerScreamer }

    [System.Serializable]
    public class AdvancedAnomaly
    {
        public string name;
        public AnomalyType type;

        [Header("For Swapping / Spawning")]
        public GameObject normalObject;
        public GameObject anomalyObject;

        [Header("For Scaling")]
        public GameObject objectToScale;
        public Vector3 targetScale = new Vector3(2f, 2f, 2f);

        [Header("For Custom Scripts (like Screamers)")]
        public GameObject screamerTriggerObject;
    }

    [Tooltip("Шанс появления аномалии (0-100)")]
    public int AnomalyChance = 50;

    [Header("🛠️ РЕЖИМ ТЕСТИРОВАНИЯ (ДЛЯ РАЗРАБОТЧИКА)")]
    [Tooltip("Если включено, рандом отключается, и всегда спавнится аномалия, выбранная ниже")]
    public bool debugMode = false;
    [Tooltip("Индекс аномалии из списка ниже, которую нужно принудительно включить (0, 1, 2...)")]
    public int debugAnomalyIndex = 0;

    [Header("Список всех аномалий")]
    public List<AdvancedAnomaly> allAnomalies;

    [HideInInspector]
    public bool wasAnomalySpawned = false;

    void Start()
    {
        Debug.Log($"[DEBUG] Главный триггер '{gameObject.name}' успешно активирован на сцене и готов ловить игрока!");

#if UNITY_EDITOR
        if (Time.realtimeSinceStartup < 2f)
        {
            PlayerPrefs.SetInt("CurrentLoop", debugMode ? 1 : 0);
            PlayerPrefs.Save();
            Debug.Log("<color=orange>[РЕДАКТОР]: Настройка этажей оптимизирована под тест!</color>");
        }
#endif

        wasAnomalySpawned = false;
        if (allAnomalies == null || allAnomalies.Count == 0) return;

        // --- ИНИЦИАЛИЗАЦИЯ СЦЕНЫ (СБРОС ВСЕХ АНОМАЛИЙ) ---
        foreach (var anomaly in allAnomalies)
        {
            if (anomaly.anomalyObject != null) anomaly.anomalyObject.SetActive(false);
            if (anomaly.normalObject != null) anomaly.normalObject.SetActive(true);

            // УМНОЕ ОТКЛЮЧЕНИЕ ТРИГГЕРОВ
            if (anomaly.screamerTriggerObject != null)
            {
                // Проверяем, колесница ли это
                var wheelScript = anomaly.screamerTriggerObject.GetComponentInChildren<TriggerScreamerObject>();

                if (wheelScript != null)
                {
                    // Если это колесница — ЖЕСТКО ВЫКЛЮЧАЕМ её триггер, чтобы она не ехала на чистом круге!
                    anomaly.screamerTriggerObject.SetActive(false);
                }
                else
                {
                    // Если это старая дверь туалета, НЕ ВЫКЛЮЧАЕМ её объект целиком, 
                    // чтобы дверь не исчезала, а просто глушим сам скрипт скримера
                    var toiletScript = anomaly.screamerTriggerObject.GetComponentInChildren<ToiletDoorScreamer>();
                    if (toiletScript != null)
                    {
                        // Тут дверь остается видимой, но скример спать ложится (если у тебя там есть метод выключения)
                        // Если метода нет, то оставляем как есть, главное — дверь не исчезнет!
                    }
                }
            }
        }

        Random.InitState(System.DateTime.Now.Millisecond + System.DateTime.Now.Second);
        int currentLoop = PlayerPrefs.GetInt("CurrentLoop", 0);

        if (debugMode)
        {
            if (debugAnomalyIndex >= 0 && debugAnomalyIndex < allAnomalies.Count)
            {
                wasAnomalySpawned = true;
                ActivateAnomaly(allAnomalies[debugAnomalyIndex]);
                Debug.Log($"<color=magenta>[DEBUG РЕЖИМ]: Принудительно запущена аномалия №{debugAnomalyIndex} — {allAnomalies[debugAnomalyIndex].name}!</color>");
            }
            else
            {
                Debug.LogError($"[DEBUG ОШИБКА]: Индекс {debugAnomalyIndex} не существует в списке аномалий!");
            }
            return;
        }

        if (currentLoop == 0)
        {
            Debug.Log("<color=yellow>[ЭТАЖ 0]:</color> Ознакомительный круг. Полная чистота.");
            return;
        }

        int globalRoll = Random.Range(0, 101);

        if (globalRoll <= AnomalyChance)
        {
            wasAnomalySpawned = true;
            int randomIndex = Random.Range(0, allAnomalies.Count);
            ActivateAnomaly(allAnomalies[randomIndex]);
        }
        else
        {
            Debug.Log($"<color=green>[ЧИСТЫЙ КРУГ]:</color> Ролл не прошел. Колесница спит. Этаж: {currentLoop}");
        }
    }

    private void ActivateAnomaly(AdvancedAnomaly anomaly)
    {
        switch (anomaly.type)
        {
            case AnomalyType.SwapObjects:
                if (anomaly.normalObject != null) anomaly.normalObject.SetActive(false);
                if (anomaly.anomalyObject != null) anomaly.anomalyObject.SetActive(true);
                break;

            case AnomalyType.JustSpawn:
                if (anomaly.anomalyObject != null) anomaly.anomalyObject.SetActive(true);
                break;

            case AnomalyType.ScaleObject:
                if (anomaly.objectToScale != null) anomaly.objectToScale.transform.localScale = anomaly.targetScale;
                break;

            case AnomalyType.TriggerScreamer:
                if (anomaly.screamerTriggerObject != null)
                {
                    // Включаем триггер колесницы/скримера (теперь он сработает только если выпала аномалия!)
                    anomaly.screamerTriggerObject.SetActive(true);

                    // Логика для двери туалета
                    ToiletDoorScreamer toiletScript = anomaly.screamerTriggerObject.GetComponentInChildren<ToiletDoorScreamer>();
                    if (toiletScript != null)
                    {
                        toiletScript.EnableScreamerAnomaly();
                    }

                    // Логика для колесницы
                    TriggerScreamerObject wheelScript = anomaly.screamerTriggerObject.GetComponentInChildren<TriggerScreamerObject>();
                    if (wheelScript != null)
                    {
                        Debug.Log($"[AnomalyLogic] Колесница '{anomaly.screamerTriggerObject.name}' активирована как АНОМАЛИЯ!");
                    }
                }
                break;
        }

        Debug.Log($"<color=cyan>[ВЫБРАНА АНОМАЛИЯ]:</color> {anomaly.name}");
    }
}