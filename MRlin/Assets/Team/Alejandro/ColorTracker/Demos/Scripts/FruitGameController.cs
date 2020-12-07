using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// The Vexpot.ColorTrackerDemo namespace contains all scripts used in demonstration scenes.
/// </summary>
namespace Vexpot.ColorTrackerDemo
{
    /// <summary>
    /// Some useful shared data for store player info in the fruit game.
    /// </summary>
    public static class FruitGameData
    {
        /// <summary>
        /// The amount of fruit sliced.
        /// </summary>
        public static int slicedCount = 0;
    }

    /// <summary>
    ///  Simple game controller to setup some game parameters.
    /// </summary>
    public class FruitGameController : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        public Text scoreLabel;
        /// <summary>
        /// 
        /// </summary>
        public Text getReadyLabel;
        /// <summary>
        /// 
        /// </summary>
        public float shotFrequency = 0.8f;
        /// <summary>
        /// 
        /// </summary>
        public AudioClip shotSound;
        /// <summary>
        /// 
        /// </summary>
        public AudioClip music;
        /// <summary>
        /// 
        /// </summary>
        public List<GameObject> fruitsTypes;

        private AudioSource _shotSound;
        private AudioSource _music;
        private int shots = 0;

        void Awake()
        {
            _music = gameObject.AddComponent<AudioSource>();

            _shotSound = gameObject.AddComponent<AudioSource>();
            if (_shotSound)
                _shotSound.clip = shotSound;

            Random.seed = 10002;

            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Fruits"), LayerMask.NameToLayer("FruitsSlice"));
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Fruits"), LayerMask.NameToLayer("Fruits"));
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("FruitsSlice"), LayerMask.NameToLayer("FruitsSlice"));

            RandomizeIndex(fruitsTypes);
            InvokeRepeating("ShotFruit", 2, shotFrequency);
            Invoke("HideGetReady", 1.5f);
        }

        void Start()
        {
            if (_music)
            {
                _music.PlayOneShot(music, 0.8f);
            }
        }

        void Update()
        {
            if (scoreLabel)
                scoreLabel.text = "Sliced fruits: " + FruitGameData.slicedCount.ToString();
        }

        private void HideGetReady()
        {
            if (getReadyLabel)
                getReadyLabel.enabled = false;
        }

        private void ShotFruit()
        {
            if (fruitsTypes.Count > 0)
            {
                GameObject fruit = fruitsTypes[shots];

                if (shots < fruitsTypes.Count - 1)
                {
                    shots++;
                }
                else
                {
                    RandomizeIndex(fruitsTypes);
                    shots = 0;
                }

                var obj = GameObject.Instantiate(fruit);

                float posX = Random.Range(-2, 2);
                float posY = Random.Range(-3, -1);
                float posZ = Random.Range(0, 1);

                obj.transform.position = new Vector3(posX, posY, posZ);
                obj.SetActive(true);

                if (_shotSound)
                    _shotSound.Play();
            }

        }

        private void RandomizeIndex(List<GameObject> list)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var r = Random.Range(0, i);
                var tmp = list[i];
                list[i] = list[r];
                list[r] = tmp;
            }
        }
    }
}
  