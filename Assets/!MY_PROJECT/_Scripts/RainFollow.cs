using UnityEngine;

public class RainFollow : MonoBehaviour
{
    [Tooltip("—сылка на трансформ игрока или главной камеры")]
    public Transform PlayerTransform;

    [Tooltip("¬ысота, на которой туча будет висеть над игроком")]
    public float HeightOffset = 15f;

    void Start()
    {
        // ≈сли забыл прив€зать игрока в инспекторе, скрипт попытаетс€ найти главную камеру сам
        if (PlayerTransform == null && Camera.main != null)
        {
            PlayerTransform = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (PlayerTransform == null) return;

        //  опируем X и Z координаты игрока, но держим тучу строго на фиксированной высоте
        Vector3 newPosition = new Vector3(PlayerTransform.position.x, HeightOffset, PlayerTransform.position.z);

        transform.position = newPosition;
    }
}