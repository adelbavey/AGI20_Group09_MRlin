using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace QuickStart
{
    public class SceneScript : NetworkBehaviour
    {
        public Text canvasStatusText;
        public PlayerScript playerScript;
        public GameObject forest;
        public GameObject spotLight;

        public float lightningRate;
        public float lightningDurationTime;
        public float lightChangingSpeed;
        private Light _light;
        private float intensity1;
        private float intensity2;
        

        [SyncVar(hook = nameof(OnStatusTextChanged))]
        public string statusText;

        private void Awake()
        {
            forest.SetActive(true);
            _light = spotLight.GetComponent<Light>();
        }

        void OnStatusTextChanged(string _Old, string _New)
        {
            //called from sync var hook, to update info on screen for all players
            canvasStatusText.text = statusText;
        }

        public void ButtonSendMessage()
        {
            if (playerScript != null)
            {
                playerScript.CmdSendPlayerMessage();
            }
        }

        private void Update()
        {
            //InvokeRepeating("LightChange", lightningDurationTime, lightningRate);
        }

        private void LightChange()
        {

            //PingPong between 0 and 1
            intensity2 = Random.Range(0.0f, 1.0f);
            float time = Mathf.PingPong(lightChangingSpeed, 1);
            _light.intensity = Mathf.Lerp(intensity1, intensity2, time);
            intensity1 = intensity2;
        }

        public void disableMainCamera()
        {
            AudioListener.pause = true;
        }
    }
}