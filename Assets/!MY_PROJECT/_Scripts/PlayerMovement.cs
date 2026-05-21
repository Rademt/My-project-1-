using UnityEngine;

namespace SojaExiles
{
    public class PlayerMovement : MonoBehaviour
    {
        public CharacterController controller;
        public Transform cameraTransform;

        public float walkSpeed = 3f;
        public float runSpeed = 6f;
        public float gravity = -25f;

        [Header("Настройки покачивания камеры")]
        public float walkingBobAmount = 0.07f;
        public float walkingBobSpeed = 12f;
        public float runningBobAmount = 0.12f;
        public float runningBobSpeed = 16f;

        [Header("Звуки шагов")]
        public AudioSource stepsAudio; // Сам источник звука (AudioSource)
        public AudioClip walkClip;     // Сюда перетащим аудиофайл обычной ходьбы (.mp3/.wav)
        public AudioClip runClip;      // Сюда перетащим аудиофайл бега (.mp3/.wav)

        Vector3 velocity;
        bool isGrounded;
        float defaultYPos;
        float timer;

        private float lastSinValue = 0f;

        void Start()
        {
            if (cameraTransform != null) defaultYPos = cameraTransform.localPosition.y;
        }

        void Update()
        {
            isGrounded = controller.isGrounded;

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            // Проверяем, зажат ли Shift
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float currentSpeed = isRunning ? runSpeed : walkSpeed;

            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;
            if (move.magnitude > 1) move.Normalize();

            controller.Move(move * currentSpeed * Time.deltaTime);

            // Если мы на земле и куда-то идем
            if (isGrounded && move.magnitude > 0.1f && cameraTransform != null)
            {
                float speedMultiplier = isRunning ? runningBobSpeed : walkingBobSpeed;
                timer += Time.deltaTime * speedMultiplier;
                float amount = isRunning ? runningBobAmount : walkingBobAmount;

                float sinValue = Mathf.Sin(timer);
                float newY = defaultYPos + sinValue * amount;
                cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, newY, cameraTransform.localPosition.z);

                // --- ЛОГИКА ШАГОВ И БЕГА ---
                if (sinValue < -0.95f && lastSinValue >= -0.95f)
                {
                    if (stepsAudio != null)
                    {
                        // 1. Выбираем нужный клип в зависимости от того, бежим мы или идем
                        AudioClip clipToPlay = isRunning ? runClip : walkClip;

                        // Если клип назначен и сейчас не играет ТОЧНО ТАКОЙ ЖЕ звук
                        if (clipToPlay != null && (!stepsAudio.isPlaying || stepsAudio.clip != clipToPlay))
                        {
                            stepsAudio.clip = clipToPlay; // Меняем звук в источнике

                            // Изменяем громкость и питч для реализма
                            stepsAudio.pitch = Random.Range(0.9f, 1.1f);
                            stepsAudio.volume = isRunning ? 0.8f : 0.45f; // Бег громче, ходьба тише

                            stepsAudio.Play();
                        }
                    }
                }
                lastSinValue = sinValue;
                // ------------------------------
            }
            else if (cameraTransform != null)
            {
                timer = 0;
                lastSinValue = 0f;
                cameraTransform.localPosition = new Vector3(
                    cameraTransform.localPosition.x,
                    Mathf.Lerp(cameraTransform.localPosition.y, defaultYPos, Time.deltaTime * 5f),
                    cameraTransform.localPosition.z
                );
            }

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }
}