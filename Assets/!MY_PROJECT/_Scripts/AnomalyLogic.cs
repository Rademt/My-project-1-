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

    [Tooltip("Шанс появи аномалії (0-100)")]
    public int AnomalyChance = 50;

    [Header("🛠️ РЕЖИМ ТЕСТУВАННЯ (ДЛЯ РОЗРОБНИКА)")]
    [Tooltip("Якщо включено, рандом відключається, і завжди спавниться аномалія, вибрана нижче")]
    public bool debugMode = false;
    [Tooltip("Індекс аномалії зі списку нижче, котру требя примусово увімкнути (0, 1, 2...)")]
    public int debugAnomalyIndex = 0;

    [Header("Список усіх аномалій")]
    public List<AdvancedAnomaly> allAnomalies;

    [HideInInspector]
    public bool wasAnomalySpawned = false;

    void Start()
    {
        Debug.Log($"[DEBUG] Головний трігер '{gameObject.name}' успішно активовано на сцені й готовий ловити гравця!");

#if UNITY_EDITOR
        if (Time.realtimeSinceStartup < 2f)
        {
            PlayerPrefs.SetInt("CurrentLoop", debugMode ? 1 : 0);
            PlayerPrefs.Save();
            Debug.Log("<color=orange>[РЕДАКТОР]: Налаштування поверхів оптимізовано для тестування!</color>");
        }
#endif

        wasAnomalySpawned = false;
        if (allAnomalies == null || allAnomalies.Count == 0) return;

        foreach(var anomaly in allAnomalies)
        {
            if (anomaly.anomalyObject != null) anomaly.anomalyObject.SetActive(false);
            if (anomaly.normalObject != null) anomaly.normalObject.SetActive(true);

            if (anomaly.screamerTriggerObject != null)
            {
                var wheelScript = anomaly.screamerTriggerObject.GetComponentInChildren<TriggerScreamerObject>();

                if (wheelScript != null)
                {
                    anomaly.screamerTriggerObject.SetActive(false);
                }
                else
                {
                    // Проверяем нашу НОВУЮ ловушку (которая на пустышке)
                    var trapScript = anomaly.screamerTriggerObject.GetComponentInChildren<ToiletTrapAnomaly>();

                    if (trapScript != null)
                    {
                        // Пустышку выключаем целиком, она не связана с физической дверью и ничего не сломает
                        anomaly.screamerTriggerObject.SetActive(false);
                    }

                    // СТАРЫЙ скример (ToiletDoorScreamer) мы тут ВООБЩЕ НЕ ТРОГАЕМ! 
                    // Пусть дверь живет своей жизнью, пока не выпадет аномалия.
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
                Debug.Log($"<color=magenta>[DEBUG РЕЖИМ]: Аномалія була запущена примусово №{debugAnomalyIndex} — {allAnomalies[debugAnomalyIndex].name}!</color>");
            }
            else
            {
                Debug.LogError($"[DEBUG ПОМИЛКА]: Індекс {debugAnomalyIndex} немає у списку аномалій!");
            }
            return;
        }

        if (currentLoop == 0)
        {
            Debug.Log("<color=yellow>[ЭТАЖ 0]:</color> Ознайомлювальне коло. Абсолютна чистота.");
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
            Debug.Log($"<color=green>[ЧИСТЕ КОЛО]:</color> Рол не пройшов. Колісниця спить. Поверх: {currentLoop}");
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
                    // Включаем родительский объект (для пустышки это важно, для старой двери она и так активна)
                    anomaly.screamerTriggerObject.SetActive(true);

                    // 1. Включаем СТАРЫЙ скример туалета (и активируем сам компонент скрипта)
                    ToiletDoorScreamer toiletScript = anomaly.screamerTriggerObject.GetComponentInChildren<ToiletDoorScreamer>();
                    if (toiletScript != null)
                    {
                        toiletScript.enabled = true;
                        toiletScript.EnableScreamerAnomaly();
                    }

                    // 2. Включаем НАШУ НОВУЮ ловушку с сердцем
                    ToiletTrapAnomaly trapScript = anomaly.screamerTriggerObject.GetComponentInChildren<ToiletTrapAnomaly>();
                    if (trapScript != null)
                    {
                        trapScript.EnableScreamerAnomaly();
                    }

                    // 3. Проверяем колесницу
                    TriggerScreamerObject wheelScript = anomaly.screamerTriggerObject.GetComponentInChildren<TriggerScreamerObject>();
                    if (wheelScript != null)
                    {
                        Debug.Log($"[AnomalyLogic] Колісниця '{anomaly.screamerTriggerObject.name}' активована як АНОМАЛІЯ!");
                    }
                }
                break;
        }

        Debug.Log($"<color=cyan>[ВИБРАНО АНОМАЛІЮ]:</color> {anomaly.name}");
    }
}