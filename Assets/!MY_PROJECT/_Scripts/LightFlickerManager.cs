using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LightFlickerManager : MonoBehaviour
{
    public List<Light> corridorLights;
    public float minTimeBetweenFlickers = 2f;
    public float maxTimeBetweenFlickers = 5f;

    void Start()
    {
        Debug.Log("<color=yellow>[[СВІТЛО]: Сценарій успішно запущено на сцені!</color>");
        StartCoroutine(HardFlickerRoutine());
    }

    private IEnumerator HardFlickerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBetweenFlickers, maxTimeBetweenFlickers));

            Debug.Log("<color=red>[СВІТЛО]: Моргаю! Вимикаю лампи...</color>");
            SwitchAllLights(false);

            yield return new WaitForSeconds(0.2f);

            Debug.Log("<color=green>[СВІТЛО]: Вмикаю лампи знову!</color>");
            SwitchAllLights(true);
        }
    }

    private void SwitchAllLights(bool state)
    {
        for (int i = 0; i < corridorLights.Count; i++)
        {
            if (corridorLights[i] != null)
            {
                corridorLights[i].gameObject.SetActive(state);
            }
        }
    }
}