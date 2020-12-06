using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// The Vexpot.ColorTrackerDemo namespace contains all scripts used in demonstration scenes.
/// </summary>
namespace Vexpot.ColorTrackerDemo
{
    /// <summary>
    /// Encapsulates the fruits behavior in the game. 
    /// </summary>
    public class SplitableFruit : MonoBehaviour, IPointerEnterHandler
    {
        /// <summary>
        /// The first fruit slice.
        /// </summary>
        public string slice1;
        /// <summary>
        /// The second fruit slice.
        /// </summary>
        public string slice2;
        /// <summary>
        /// A common physic material to all fruits.
        /// </summary>
        public PhysicMaterial physicMat;
        /// <summary>
        /// Time in seconds to destroy the fruit.
        /// </summary>
        public float timeToDestroy = 2.0f;
        /// <summary>
        ///  The sound to play when the fruit is slashed.
        /// </summary>
        public AudioClip splatSound;


        private AudioSource _splatSound;
        private GameObject _slice1;
        private GameObject _slice2;
        private bool canSplit = true;
        private Renderer fruitRenderer;

        /// <summary>
        /// Initialize all fruit parameters.
        /// </summary>
        void Awake()
        {
            _splatSound = gameObject.AddComponent<AudioSource>();
            if (_splatSound)
            {
                _splatSound.clip = splatSound;
                _splatSound.volume = 0.5f;
            }

            _slice1 = GetChildGameObject(slice1);
            _slice2 = GetChildGameObject(slice2);

            fruitRenderer = gameObject.GetComponent<MeshRenderer>();

            var collider = gameObject.AddComponent<BoxCollider>();

            if (physicMat && collider)
                collider.material = physicMat;

            var body = gameObject.AddComponent<Rigidbody>();

            float impulseX = Random.Range(-5, 5);
            float impulseY = Random.Range(10, 13);
            float impulseZ = Random.Range(0, 1);

            body.AddForce(new Vector3(impulseX, impulseY, impulseZ), ForceMode.Impulse);
            body.AddTorque(new Vector3(impulseX * 1.5f, impulseY, impulseZ), ForceMode.Impulse);

            if (_slice1)
            {
                _slice1.SetActive(false);
            }

            if (_slice2)
            {
                _slice2.SetActive(false);
            }

            Invoke("DestroyFruit", timeToDestroy);
        }

        /// <summary>
        /// Destroys the fruit when is no longer needed.
        /// </summary>
        void DestroyFruit()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Performs the fruit explosion effect when pointer reach the fruit.
        /// </summary>
        void SplitFruit()
        {
            if (canSplit)
            {
                if (fruitRenderer)
                    fruitRenderer.enabled = false;

                if (_slice1)
                {
                    var collider = _slice1.AddComponent<BoxCollider>();

                    if (physicMat)
                        collider.material = physicMat;

                    _slice1.AddComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(5, 0, 0));

                    _slice1.SetActive(true);
                }
                if (_slice2)
                {
                    var collider = _slice2.AddComponent<BoxCollider>();

                    if (physicMat)
                        collider.material = physicMat;

                    _slice2.AddComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(-5, 0, 0)); ;
                    _slice2.SetActive(true);
                }

                FruitGameData.slicedCount++;
                if (_splatSound)
                {
                    _splatSound.Play();
                }
                canSplit = false;
            }
        }

        /// <summary>
        /// Finds all <see cref="Transform"/> components on children objects.
        /// </summary>
        /// <param name="withName"></param>
        /// <returns></returns>
        GameObject GetChildGameObject(string withName)
        {
            Transform[] ts = GetComponentsInChildren<Transform>();
            foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
            return null;
        }

        /// <summary>
        /// When virtual pointer touch this object then performs the fruit explosion.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            SplitFruit();
        }
    }
}