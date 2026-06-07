using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace SlimUI.ModernMenu
{
    public class UISettingsManager : MonoBehaviour
    {
        public enum Platform { Desktop, Mobile };
        public Platform platform;

        // toggle buttons
        [Header("MOBILE SETTINGS")]
        public GameObject mobileSFXtext;
        public GameObject mobileMusictext;
        public GameObject mobileShadowofftextLINE;
        public GameObject mobileShadowlowtextLINE;
        public GameObject mobileShadowhightextLINE;

        [Header("VIDEO SETTINGS")]
        public GameObject fullscreentext;
        public GameObject ambientocclusiontext;
        public GameObject shadowofftextLINE;
        public GameObject shadowlowtextLINE;
        public GameObject shadowhightextLINE;
        public GameObject aaofftextLINE;
        public GameObject aa2xtextLINE;
        public GameObject aa4xtextLINE;
        public GameObject aa8xtextLINE;
        public GameObject vsynctext;
        public GameObject motionblurtext;
        public GameObject texturelowtextLINE;
        public GameObject texturemedtextLINE;
        public GameObject texturehightextLINE;
        public GameObject cameraeffectstext;

        [Header("GAME SETTINGS")]
        public GameObject showhudtext;
        public GameObject tooltipstext;

        [Header("AUDIO SETTINGS")]
        public GameObject sfxText;
        public GameObject soundTextObject;
        private bool soundEnabled = true;

        [Header("CONTROLS SETTINGS")]
        public GameObject invertmousetext;

        // sliders
        public GameObject musicSlider;
        public GameObject sensitivityXSlider;
        public GameObject sensitivityYSlider;
        public GameObject mouseSmoothSlider;

        private float sliderValueXSensitivity = 0.0f;
        private float sliderValueYSensitivity = 0.0f;
        private float sliderValueSmoothing = 0.0f;

        [System.Serializable]
        public struct KeyBindUI
        {
            public string actionName;
            [Header("Перетащи сюда объект notassigned")]
            public GameObject textObject;
        }

        [Header("СИСТЕМА КЛАВИШ")]
        public List<KeyBindUI> uiBinds;

        [HideInInspector]
        public Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

        private string currentActionToRebind = "";
        private GameObject currentTextObjectToUpdate = null;

        public void Awake()
        {
            InitKey("Forward", KeyCode.W);
            InitKey("Backward", KeyCode.S);
            InitKey("Left", KeyCode.A);
            InitKey("Right", KeyCode.D);
            InitKey("Jump", KeyCode.Space);
            InitKey("Sprint", KeyCode.LeftShift);
            InitKey("Interact", KeyCode.E);
        }

        public void Start()
        {
            // check slider values
            if (musicSlider) musicSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MusicVolume", 1f);
            if (sensitivityXSlider) sensitivityXSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("XSensitivity", 4f);
            if (sensitivityYSlider) sensitivityYSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("YSensitivity", 4f);
            if (mouseSmoothSlider) mouseSmoothSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MouseSmoothing", 2f);

            // check sfx mute status
            if (sfxText)
            {
                if (PlayerPrefs.GetInt("SFX_Mute", 0) == 0)
                {
                    sfxText.GetComponent<TMP_Text>().text = "on";
                }
                else
                {
                    sfxText.GetComponent<TMP_Text>().text = "off";
                }
            }

            soundEnabled = PlayerPrefs.GetInt("CustomSoundEnabled", 1) == 1;

            AudioListener.pause = !soundEnabled;

            if (soundTextObject != null)
            {
                var tmpText = soundTextObject.GetComponent<TMP_Text>();
                if (tmpText != null)
                {
                    tmpText.text = soundEnabled ? "on" : "off";
                }
            }

            // check full screen
            if (fullscreentext)
            {
                if (Screen.fullScreen == true)
                {
                    fullscreentext.GetComponent<TMP_Text>().text = "on";
                }
                else if (Screen.fullScreen == false)
                {
                    fullscreentext.GetComponent<TMP_Text>().text = "off";
                }
            }

            // check hud value
            if (showhudtext)
            {
                if (PlayerPrefs.GetInt("ShowHUD") == 0)
                {
                    showhudtext.GetComponent<TMP_Text>().text = "off";
                }
                else
                {
                    showhudtext.GetComponent<TMP_Text>().text = "on";
                }
            }

            // check tool tip value
            if (tooltipstext)
            {
                if (PlayerPrefs.GetInt("ToolTips") == 0)
                {
                    tooltipstext.GetComponent<TMP_Text>().text = "off";
                }
                else
                {
                    tooltipstext.GetComponent<TMP_Text>().text = "on";
                }
            }

            // check shadow distance/enabled
            if (platform == Platform.Desktop)
            {
                if (PlayerPrefs.GetInt("Shadows") == 0)
                {
                    QualitySettings.shadowCascades = 0;
                    QualitySettings.shadowDistance = 0;
                    if (shadowofftextLINE) shadowofftextLINE.gameObject.SetActive(true);
                    if (shadowlowtextLINE) shadowlowtextLINE.gameObject.SetActive(false);
                    if (shadowhightextLINE) shadowhightextLINE.gameObject.SetActive(false);
                }
                else if (PlayerPrefs.GetInt("Shadows") == 1)
                {
                    QualitySettings.shadowCascades = 2;
                    QualitySettings.shadowDistance = 75;
                    if (shadowofftextLINE) shadowofftextLINE.gameObject.SetActive(false);
                    if (shadowlowtextLINE) shadowlowtextLINE.gameObject.SetActive(true);
                    if (shadowhightextLINE) shadowhightextLINE.gameObject.SetActive(false);
                }
                else if (PlayerPrefs.GetInt("Shadows") == 2)
                {
                    QualitySettings.shadowCascades = 4;
                    QualitySettings.shadowDistance = 500;
                    if (shadowofftextLINE) shadowofftextLINE.gameObject.SetActive(false);
                    if (shadowlowtextLINE) shadowlowtextLINE.gameObject.SetActive(false);
                    if (shadowhightextLINE) shadowhightextLINE.gameObject.SetActive(true);
                }
            }
            else if (platform == Platform.Mobile)
            {
                if (PlayerPrefs.GetInt("MobileShadows") == 0)
                {
                    QualitySettings.shadowCascades = 0;
                    QualitySettings.shadowDistance = 0;
                    if (mobileShadowofftextLINE) mobileShadowofftextLINE.gameObject.SetActive(true);
                    if (mobileShadowlowtextLINE) mobileShadowlowtextLINE.gameObject.SetActive(false);
                    if (mobileShadowhightextLINE) mobileShadowhightextLINE.gameObject.SetActive(false);
                }
                else if (PlayerPrefs.GetInt("MobileShadows") == 1)
                {
                    QualitySettings.shadowCascades = 2;
                    QualitySettings.shadowDistance = 75;
                    if (mobileShadowofftextLINE) mobileShadowofftextLINE.gameObject.SetActive(false);
                    if (mobileShadowlowtextLINE) mobileShadowlowtextLINE.gameObject.SetActive(true);
                    if (mobileShadowhightextLINE) mobileShadowhightextLINE.gameObject.SetActive(false);
                }
                else if (PlayerPrefs.GetInt("MobileShadows") == 2)
                {
                    QualitySettings.shadowCascades = 4;
                    QualitySettings.shadowDistance = 100;
                    if (mobileShadowofftextLINE) mobileShadowofftextLINE.gameObject.SetActive(false);
                    if (mobileShadowlowtextLINE) mobileShadowlowtextLINE.gameObject.SetActive(false);
                    if (mobileShadowhightextLINE) mobileShadowhightextLINE.gameObject.SetActive(true);
                }
            }

            // check vsync
            if (vsynctext)
            {
                if (QualitySettings.vSyncCount == 0)
                {
                    vsynctext.GetComponent<TMP_Text>().text = "off";
                }
                else if (QualitySettings.vSyncCount == 1)
                {
                    vsynctext.GetComponent<TMP_Text>().text = "on";
                }
            }

            // check mouse inverse
            if (invertmousetext)
            {
                if (PlayerPrefs.GetInt("Inverted") == 0)
                {
                    invertmousetext.GetComponent<TMP_Text>().text = "off";
                }
                else if (PlayerPrefs.GetInt("Inverted") == 1)
                {
                    invertmousetext.GetComponent<TMP_Text>().text = "on";
                }
            }

            // check motion blur
            if (motionblurtext)
            {
                if (PlayerPrefs.GetInt("MotionBlur") == 0)
                {
                    motionblurtext.GetComponent<TMP_Text>().text = "off";
                }
                else if (PlayerPrefs.GetInt("MotionBlur") == 1)
                {
                    motionblurtext.GetComponent<TMP_Text>().text = "on";
                }
            }

            // check ambient occlusion
            if (ambientocclusiontext)
            {
                if (PlayerPrefs.GetInt("AmbientOcclusion") == 0)
                {
                    ambientocclusiontext.GetComponent<TMP_Text>().text = "off";
                }
                else if (PlayerPrefs.GetInt("AmbientOcclusion") == 1)
                {
                    ambientocclusiontext.GetComponent<TMP_Text>().text = "on";
                }
            }

            // check texture quality
            if (PlayerPrefs.GetInt("Textures") == 0)
            {
                QualitySettings.globalTextureMipmapLimit = 2;
                if (texturelowtextLINE) texturelowtextLINE.gameObject.SetActive(true);
                if (texturemedtextLINE) texturemedtextLINE.gameObject.SetActive(false);
                if (texturehightextLINE) texturehightextLINE.gameObject.SetActive(false);
            }
            else if (PlayerPrefs.GetInt("Textures") == 1)
            {
                QualitySettings.globalTextureMipmapLimit = 1;
                if (texturelowtextLINE) texturelowtextLINE.gameObject.SetActive(false);
                if (texturemedtextLINE) texturemedtextLINE.gameObject.SetActive(true);
                if (texturehightextLINE) texturehightextLINE.gameObject.SetActive(false);
            }
            else if (PlayerPrefs.GetInt("Textures") == 2)
            {
                QualitySettings.globalTextureMipmapLimit = 0;
                if (texturelowtextLINE) texturelowtextLINE.gameObject.SetActive(false);
                if (texturemedtextLINE) texturemedtextLINE.gameObject.SetActive(false);
                if (texturehightextLINE) texturehightextLINE.gameObject.SetActive(true);
            }

            UpdateAllKeyUI();
        }

        void OnEnable()
        {
            UpdateAllKeyUI();
        }

        void InitKey(string action, KeyCode defaultKey)
        {
            string savedKey = PlayerPrefs.GetString(action, defaultKey.ToString());
            KeyCode loadedKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), savedKey);

            if (keys.ContainsKey(action))
                keys[action] = loadedKey;
            else
                keys.Add(action, loadedKey);
        }

        public void UpdateAllKeyUI()
        {
            if (uiBinds == null || uiBinds.Count == 0) return;

            foreach (var bind in uiBinds)
            {
                if (string.IsNullOrEmpty(bind.actionName) || bind.textObject == null) continue;

                if (keys.ContainsKey(bind.actionName))
                {
                    SetTextOnObject(bind.textObject, keys[bind.actionName].ToString());
                }
            }
        }

        public void StartRebind(string actionName)
        {
            currentActionToRebind = actionName;

            foreach (var bind in uiBinds)
            {
                if (bind.actionName == actionName && bind.textObject != null)
                {
                    currentTextObjectToUpdate = bind.textObject;
                    SetTextOnObject(currentTextObjectToUpdate, "PRESS ANY KEY...");
                    break;
                }
            }
        }

        void OnGUI()
        {
            if (currentActionToRebind != "" && Event.current != null && Event.current.isKey && Event.current.keyCode != KeyCode.None)
            {
                KeyCode newKey = Event.current.keyCode;

                if (newKey == KeyCode.Escape) return;

                keys[currentActionToRebind] = newKey;
                PlayerPrefs.SetString(currentActionToRebind, newKey.ToString());
                PlayerPrefs.Save();

                if (currentTextObjectToUpdate != null)
                {
                    SetTextOnObject(currentTextObjectToUpdate, newKey.ToString());
                }

                currentActionToRebind = "";
                currentTextObjectToUpdate = null;
            }
        }

        private void SetTextOnObject(GameObject obj, string text)
        {
            TMP_Text tmproText = obj.GetComponent<TMP_Text>();
            if (tmproText != null)
            {
                tmproText.text = text.ToLower();
                return;
            }

            Text normalText = obj.GetComponent<Text>();
            if (normalText != null)
            {
                normalText.text = text.ToLower();
                return;
            }
        }

        public void ToggleSoundCustom()
        {
            soundEnabled = !soundEnabled;
            AudioListener.pause = !soundEnabled;

            if (soundTextObject != null)
            {
                var tmpText = soundTextObject.GetComponent<TMP_Text>();
                if (tmpText != null)
                {
                    tmpText.text = soundEnabled ? "on" : "off";
                }
            }

            PlayerPrefs.SetInt("CustomSoundEnabled", soundEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void Update()
        {
            if (sensitivityXSlider) sliderValueXSensitivity = sensitivityXSlider.GetComponent<Slider>().value;
            if (sensitivityYSlider) sliderValueYSensitivity = sensitivityYSlider.GetComponent<Slider>().value;
            if (mouseSmoothSlider) sliderValueSmoothing = mouseSmoothSlider.GetComponent<Slider>().value;
        }

        public void FullScreen()
        {
            Screen.fullScreen = !Screen.fullScreen;

            if (Screen.fullScreen == true)
            {
                fullscreentext.GetComponent<TMP_Text>().text = "on";
            }
            else if (Screen.fullScreen == false)
            {
                fullscreentext.GetComponent<TMP_Text>().text = "off";
            }
        }

        public void MusicSlider()
        {
            if (musicSlider) PlayerPrefs.SetFloat("MusicVolume", musicSlider.GetComponent<Slider>().value);
        }

        public void SensitivityXSlider()
        {
            PlayerPrefs.SetFloat("XSensitivity", sliderValueXSensitivity);
        }

        public void SensitivityYSlider()
        {
            PlayerPrefs.SetFloat("YSensitivity", sliderValueYSensitivity);
        }

        public void SensitivitySmoothing()
        {
            PlayerPrefs.SetFloat("MouseSmoothing", sliderValueSmoothing);
        }

        public void ToggleSFX()
        {
            if (PlayerPrefs.GetInt("SFX_Mute", 0) == 0)
            {
                PlayerPrefs.SetInt("SFX_Mute", 1);
                if (sfxText) sfxText.GetComponent<TMP_Text>().text = "off";
            }
            else
            {
                PlayerPrefs.SetInt("SFX_Mute", 0);
                if (sfxText) sfxText.GetComponent<TMP_Text>().text = "on";
            }
        }

        public void ShowHUD()
        {
            if (PlayerPrefs.GetInt("ShowHUD") == 0)
            {
                PlayerPrefs.SetInt("ShowHUD", 1);
                showhudtext.GetComponent<TMP_Text>().text = "on";
            }
            else if (PlayerPrefs.GetInt("ShowHUD") == 1)
            {
                PlayerPrefs.SetInt("ShowHUD", 0);
                showhudtext.GetComponent<TMP_Text>().text = "off";
            }
        }

        public void MobileSFXMute()
        {
            if (PlayerPrefs.GetInt("Mobile_MuteSfx") == 0)
            {
                PlayerPrefs.SetInt("Mobile_MuteSfx", 1);
                mobileSFXtext.GetComponent<TMP_Text>().text = "on";
            }
            else if (PlayerPrefs.GetInt("Mobile_MuteSfx") == 1)
            {
                PlayerPrefs.SetInt("Mobile_MuteSfx", 0);
                mobileSFXtext.GetComponent<TMP_Text>().text = "off";
            }
        }

        public void MobileMusicMute()
        {
            if (PlayerPrefs.GetInt("Mobile_MuteMusic") == 0)
            {
                PlayerPrefs.SetInt("Mobile_MuteMusic", 1);
                mobileMusictext.GetComponent<TMP_Text>().text = "on";
            }
            else if (PlayerPrefs.GetInt("Mobile_MuteMusic") == 1)
            {
                PlayerPrefs.SetInt("Mobile_MuteMusic", 0);
                mobileMusictext.GetComponent<TMP_Text>().text = "off";
            }
        }

        public void ToolTips()
        {
            if (PlayerPrefs.GetInt("ToolTips") == 0)
            {
                PlayerPrefs.SetInt("ToolTips", 1);
                tooltipstext.GetComponent<TMP_Text>().text = "on";
            }
            else if (PlayerPrefs.GetInt("ToolTips") == 1)
            {
                PlayerPrefs.SetInt("ToolTips", 0);
                tooltipstext.GetComponent<TMP_Text>().text = "off";
            }
        }

        public void ShadowsOff()
        {
            PlayerPrefs.SetInt("Shadows", 0);
            QualitySettings.shadowCascades = 0;
            QualitySettings.shadowDistance = 0;
            if (shadowofftextLINE) shadowofftextLINE.gameObject.SetActive(true);
            if (shadowlowtextLINE) shadowlowtextLINE.gameObject.SetActive(false);
            if (shadowhightextLINE) shadowhightextLINE.gameObject.SetActive(false);
        }

        public void ShadowsLow()
        {
            PlayerPrefs.SetInt("Shadows", 1);
            QualitySettings.shadowCascades = 2;
            QualitySettings.shadowDistance = 75;
            if (shadowofftextLINE) shadowofftextLINE.gameObject.SetActive(false);
            if (shadowlowtextLINE) shadowlowtextLINE.gameObject.SetActive(true);
            if (shadowhightextLINE) shadowhightextLINE.gameObject.SetActive(false);
        }

        public void ShadowsHigh()
        {
            PlayerPrefs.SetInt("Shadows", 2);
            QualitySettings.shadowCascades = 4;
            QualitySettings.shadowDistance = 500;
            if (shadowofftextLINE) shadowofftextLINE.gameObject.SetActive(false);
            if (shadowlowtextLINE) shadowlowtextLINE.gameObject.SetActive(false);
            if (shadowhightextLINE) shadowhightextLINE.gameObject.SetActive(true);
        }

        public void vsync()
        {
            if (QualitySettings.vSyncCount == 0)
            {
                QualitySettings.vSyncCount = 1;
                vsynctext.GetComponent<TMP_Text>().text = "on";
            }
            else if (QualitySettings.vSyncCount == 1)
            {
                QualitySettings.vSyncCount = 0;
                vsynctext.GetComponent<TMP_Text>().text = "off";
            }
        }

        public void InvertMouse()
        {
            if (PlayerPrefs.GetInt("Inverted") == 0)
            {
                PlayerPrefs.SetInt("Inverted", 1);
                invertmousetext.GetComponent<TMP_Text>().text = "on";
            }
            else if (PlayerPrefs.GetInt("Inverted") == 1)
            {
                PlayerPrefs.SetInt("Inverted", 0);
                invertmousetext.GetComponent<TMP_Text>().text = "off";
            }
        }

        public void MotionBlur()
        {
            if (PlayerPrefs.GetInt("MotionBlur") == 0)
            {
                PlayerPrefs.SetInt("MotionBlur", 1);
                motionblurtext.GetComponent<TMP_Text>().text = "on";
            }
            else if (PlayerPrefs.GetInt("MotionBlur") == 1)
            {
                PlayerPrefs.SetInt("MotionBlur", 0);
                motionblurtext.GetComponent<TMP_Text>().text = "off";
            }
        }

        public void AmbientOcclusion()
        {
            if (PlayerPrefs.GetInt("AmbientOcclusion") == 0)
            {
                PlayerPrefs.SetInt("AmbientOcclusion", 1);
                ambientocclusiontext.GetComponent<TMP_Text>().text = "on";
            }
            else if (PlayerPrefs.GetInt("AmbientOcclusion") == 1)
            {
                PlayerPrefs.SetInt("AmbientOcclusion", 0);
                ambientocclusiontext.GetComponent<TMP_Text>().text = "off";
            }
        }

        public void CameraEffects()
        {
            if (PlayerPrefs.GetInt("CameraEffects") == 0)
            {
                PlayerPrefs.SetInt("CameraEffects", 1);
                cameraeffectstext.GetComponent<TMP_Text>().text = "on";
            }
            else if (PlayerPrefs.GetInt("CameraEffects") == 1)
            {
                PlayerPrefs.SetInt("CameraEffects", 0);
                cameraeffectstext.GetComponent<TMP_Text>().text = "off";
            }
        }

        public void TexturesLow()
        {
            PlayerPrefs.SetInt("Textures", 0);
            QualitySettings.globalTextureMipmapLimit = 2;
            if (texturelowtextLINE) texturelowtextLINE.gameObject.SetActive(true);
            if (texturemedtextLINE) texturemedtextLINE.gameObject.SetActive(false);
            if (texturehightextLINE) texturehightextLINE.gameObject.SetActive(false);
        }

        public void TexturesMed()
        {
            PlayerPrefs.SetInt("Textures", 1);
            QualitySettings.globalTextureMipmapLimit = 1;
            if (texturelowtextLINE) texturelowtextLINE.gameObject.SetActive(false);
            if (texturemedtextLINE) texturemedtextLINE.gameObject.SetActive(true);
            if (texturehightextLINE) texturehightextLINE.gameObject.SetActive(false);
        }

        public void TexturesHigh()
        {
            PlayerPrefs.SetInt("Textures", 2);
            QualitySettings.globalTextureMipmapLimit = 0;
            if (texturelowtextLINE) texturelowtextLINE.gameObject.SetActive(false);
            if (texturemedtextLINE) texturemedtextLINE.gameObject.SetActive(false);
            if (texturehightextLINE) texturehightextLINE.gameObject.SetActive(true);
        }

        public void LoadGameScene()
        {
            PlayerPrefs.SetInt("CurrentLoop", 0);
            PlayerPrefs.Save();
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }

        public void ContinueGameScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
    }
}