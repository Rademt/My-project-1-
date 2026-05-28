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

        [Header("Налаштування коливання камери")]
        public float walkingBobAmount = 0.07f;
        public float walkingBobSpeed = 12f;
        public float runningBobAmount = 0.12f;
        public float runningBobSpeed = 16f;

        [Header("Звуки шагів")]
        public AudioSource stepsAudio;
        public AudioClip walkClip;
        public AudioClip runClip;

        Vector3 velocity;
        bool isGrounded;
        float defaultYPos;
        float timer;

        private float lastSinValue = 0f;

        private SlimUI.ModernMenu.UISettingsManager keyManager;

        void Start()
        {
            if (cameraTransform != null) defaultYPos = cameraTransform.localPosition.y;

            keyManager = FindObjectOfType<SlimUI.ModernMenu.UISettingsManager>();

            if (keyManager == null)
            {
                Debug.LogError("UISettingsManager не знайшли на сцені!");
            }
        }

        void Update()
        {
            isGrounded = controller.isGrounded;

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");

            if (keyManager != null && keyManager.keys.Count > 0)
            {
                isRunning = Input.GetKey(keyManager.keys["Sprint"]);

                x = 0f;
                z = 0f;

                if (Input.GetKey(keyManager.keys["Forward"])) z += 1f;
                if (Input.GetKey(keyManager.keys["Backward"])) z -= 1f;
                if (Input.GetKey(keyManager.keys["Left"])) x -= 1f;
                if (Input.GetKey(keyManager.keys["Right"])) x += 1f;
            }

            float currentSpeed = isRunning ? runSpeed : walkSpeed;

            Vector3 move = transform.right * x + transform.forward * z;
            if (move.magnitude > 1) move.Normalize();

            controller.Move(move * currentSpeed * Time.deltaTime);

            if (isGrounded && move.magnitude > 0.1f && cameraTransform != null)
            {
                float speedMultiplier = isRunning ? runningBobSpeed : walkingBobSpeed;
                timer += Time.deltaTime * speedMultiplier;
                float amount = isRunning ? runningBobAmount : walkingBobAmount;

                float sinValue = Mathf.Sin(timer);
                float newY = defaultYPos + sinValue * amount;
                cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, newY, cameraTransform.localPosition.z);

                if (sinValue < -0.95f && lastSinValue >= -0.95f)
                {
                    if (stepsAudio != null)
                    {
                        AudioClip clipToPlay = isRunning ? runClip : walkClip;

                        if (clipToPlay != null && (!stepsAudio.isPlaying || stepsAudio.clip != clipToPlay))
                        {
                            stepsAudio.clip = clipToPlay;

                            stepsAudio.pitch = Random.Range(0.9f, 1.1f);
                            stepsAudio.volume = isRunning ? 0.8f : 0.45f;

                            stepsAudio.Play();
                        }
                    }
                }
                lastSinValue = sinValue;
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