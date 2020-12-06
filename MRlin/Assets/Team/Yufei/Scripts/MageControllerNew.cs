using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MageControllerNew : MonoBehaviour
{
    public bool phoneInteraction = true;
    public Transform targetTransform;
    public Transform wandTransform;
    public Transform wandMagicTransform;
    public Transform runeTransform;
    public AudioSource leftRune;
    public AudioSource midRune;
    public AudioSource rightRune;

    public LayerMask mouseAimMask;
    public Transform gyroRayObject;

    public int currentHealth;
    public HealthBar healthBar;

    public event EventHandler<OnCastingStartsEventArgs> OnCastingStarts;
    public class OnCastingStartsEventArgs : EventArgs
    {
        public Transform wandTransform;
        public int[] runeElements;
        public int spellElement;
    }

    //private Animator animator;
    private Rigidbody rBody;
    private int spellElement;
    [SerializeField]
    private Transform spineController;
    [SerializeField]
    private Transform rightHandController;
    [SerializeField]
    private Transform rightHandTarget;
    [SerializeField]
    private Transform mouseRayObject;

    [SerializeField]
    private Transform headFireBig;
    [SerializeField]
    private VisualEffectAsset fireBigVFX1;
    [SerializeField]
    private VisualEffectAsset fireBigVFX2;
    [SerializeField]
    private VisualEffectAsset fireBigVFX3;
    [SerializeField]
    private Transform headGlow;
    [SerializeField]
    private VisualEffectAsset headGlowVFX1;
    [SerializeField]
    private VisualEffectAsset headGlowVFX2;
    [SerializeField]
    private VisualEffectAsset headGlowVFX3;

    private ParticleSystem wandMagicParticle;
    private AudioSource wandMagicSound;
    private TrailRenderer wandMagicTrail;
    private enum gamePhase { Idle, Spelling, Casting };
    private gamePhase currentPhase;
    private enum runeHolder { Left, Middle, Right, Finished };
    private enum runeElement { Ice, Storm, Fire };
    private runeElement[] elementCombi = { runeElement.Ice, runeElement.Ice, runeElement.Ice };
    private runeHolder currentRune;

    private Camera mainCamera;
    private Vector3 targetVector;

    // Start is called before the first frame update
    void Start()
    {
        //animator = GetComponent<Animator>();
        rBody = GetComponent<Rigidbody>();
        spellElement = 1;

        wandMagicParticle = wandMagicTransform.GetComponent<ParticleSystem>();
        wandMagicSound = wandMagicTransform.GetComponent<AudioSource>();
        wandMagicSound.volume = 0.5f;
        wandMagicTrail = wandMagicTransform.GetComponent<TrailRenderer>();
        mainCamera = Camera.main;

        currentPhase = gamePhase.Idle;
        currentRune = runeHolder.Left;
        elementCombi = new runeElement[3];

        Cursor.visible = false;
        wandMagicTrail.enabled = false;

        currentHealth = 100;
        healthBar.setHealth(currentHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (phoneInteraction)
        {
            Ray newRay = new Ray(mouseRayObject.position, mouseRayObject.forward);

            RaycastHit hit;
            if (Physics.Raycast(newRay, out hit, Mathf.Infinity, mouseAimMask))
            {
                targetTransform.position = hit.point;
            }

            if (Input.GetMouseButtonDown(0))
            {
                StartSpellingPhase(true);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                StartSpellingPhase(false);
                // Check if the subscriber is null and pass in spell element info
                OnCastingStarts?.Invoke(this, new OnCastingStartsEventArgs
                {
                    wandTransform = this.wandTransform,
                    runeElements = new int[3] { (int)elementCombi[0], (int)elementCombi[1], (int)elementCombi[2] },
                    spellElement = spellElement
                });
                Invoke("Damage", 1.0f);
            }
        }
        else // Keyboard and mouse
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Mouse aim hit to invisible mouse target (Mouse Target layer)
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseAimMask))
            {
                targetTransform.position = hit.point;
                mouseRayObject.rotation = Quaternion.LookRotation(ray.direction);
            }

            if (Input.GetMouseButtonDown(0))
            {
                StartSpellingPhase(true);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                StartSpellingPhase(false);
                // Check if the subscriber is null and pass in spell element info
                OnCastingStarts?.Invoke(this, new OnCastingStartsEventArgs
                {
                    wandTransform = this.wandTransform,
                    runeElements = new int[3] { (int)elementCombi[0], (int)elementCombi[1], (int)elementCombi[2] },
                    spellElement = spellElement
                });
                Invoke("Damage", 1.0f);
            }

            if (currentPhase == gamePhase.Spelling)
            {
                SpellingPhase();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (currentPhase == gamePhase.Idle && currentRune == runeHolder.Finished)
                {
                    RuneStop(3, 3);
                    currentPhase = gamePhase.Casting;
                }
            }
            else if (Input.GetKeyUp(KeyCode.R))
            {
                if (currentPhase == gamePhase.Casting)
                {
                    // Check if the subscriber is null and pass in spell element info
                    OnCastingStarts?.Invoke(this, new OnCastingStartsEventArgs
                    {
                        wandTransform = this.wandTransform,
                        runeElements = new int[3] { (int)elementCombi[0], (int)elementCombi[1], (int)elementCombi[2] }
                    });
                }
            }
        }
    }

    private void StartSpellingPhase(bool b)
    {
        if (b)
        {
            currentPhase = gamePhase.Spelling;
            wandMagicParticle.Play();
            wandMagicSound.Play();
            wandMagicTrail.enabled = true;
        }
        else
        {
            currentPhase = gamePhase.Idle;
            wandMagicParticle.Stop();
            wandMagicSound.Stop();
            wandMagicTrail.enabled = false;
            wandMagicTrail.Clear();
        }
    }

    private void SpellingPhase()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            headFireBig.GetComponent<VisualEffect>().visualEffectAsset = fireBigVFX1;
            headGlow.GetComponent<VisualEffect>().visualEffectAsset = headGlowVFX1;
            spellElement = 1;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            headFireBig.GetComponent<VisualEffect>().visualEffectAsset = fireBigVFX2;
            headGlow.GetComponent<VisualEffect>().visualEffectAsset = headGlowVFX2;
            spellElement = 2;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            headFireBig.GetComponent<VisualEffect>().visualEffectAsset = fireBigVFX3;
            headGlow.GetComponent<VisualEffect>().visualEffectAsset = headGlowVFX3;
            spellElement = 3;
        }

        //if (currentRune == runeHolder.Left)
        //{
        //    if (Input.GetKeyDown(KeyCode.Q))
        //    {
        //        leftRune.Play();
        //        RuneStart(0, 0);
        //        elementCombi[0] = runeElement.Ice;
        //        currentRune = runeHolder.Middle;
        //    }
        //    else if (Input.GetKeyDown(KeyCode.W))
        //    {
        //        leftRune.Play();
        //        RuneStart(0, 1);
        //        elementCombi[0] = runeElement.Storm;
        //        currentRune = runeHolder.Middle;
        //    }
        //    else if (Input.GetKeyDown(KeyCode.E))
        //    {
        //        leftRune.Play();
        //        RuneStart(0, 2);
        //        elementCombi[0] = runeElement.Fire;
        //        currentRune = runeHolder.Middle;
        //    }
        //}
        //else if (currentRune == runeHolder.Middle)
        //{
        //    if (Input.GetKeyDown(KeyCode.Q))
        //    {
        //        midRune.Play();
        //        RuneStart(1, 0);
        //        elementCombi[1] = runeElement.Ice;
        //        currentRune = runeHolder.Right;
        //    }
        //    else if (Input.GetKeyDown(KeyCode.W))
        //    {
        //        midRune.Play();
        //        RuneStart(1, 1);
        //        elementCombi[1] = runeElement.Storm;
        //        currentRune = runeHolder.Right;
        //    }
        //    else if (Input.GetKeyDown(KeyCode.E))
        //    {
        //        midRune.Play();
        //        RuneStart(1, 2);
        //        elementCombi[1] = runeElement.Fire;
        //        currentRune = runeHolder.Right;
        //    }
        //}
        //else if (currentRune == runeHolder.Right)
        //{
        //    if (Input.GetKeyDown(KeyCode.Q))
        //    {
        //        rightRune.Play();
        //        RuneStart(2, 0);
        //        elementCombi[2] = runeElement.Ice;
        //        currentRune = runeHolder.Finished;
        //    }
        //    else if (Input.GetKeyDown(KeyCode.W))
        //    {
        //        rightRune.Play();
        //        RuneStart(2, 1);
        //        elementCombi[2] = runeElement.Storm;
        //        currentRune = runeHolder.Finished;
        //    }
        //    else if (Input.GetKeyDown(KeyCode.E))
        //    {
        //        rightRune.Play();
        //        RuneStart(2, 2);
        //        elementCombi[2] = runeElement.Fire;
        //        currentRune = runeHolder.Finished;
        //    }
        //}
    }

    private void RuneStart(int position, int element)
    {
        runeTransform.GetChild(position).GetChild(element).GetComponent<MeshRenderer>().enabled = true;
        runeTransform.GetChild(position).GetChild(element).GetComponent<Light>().enabled = true;
        runeTransform.GetChild(position).GetChild(element).GetChild(0).GetComponent<ParticleSystem>().Play();
    }

    private void RuneStop(int position, int element)
    {
        for (int i = 0; i < position; i++)
        {
            for (int j = 0; j < element; j++)
            {
                runeTransform.GetChild(i).GetChild(j).GetComponent<MeshRenderer>().enabled = false;
                runeTransform.GetChild(i).GetChild(j).GetComponent<Light>().enabled = false;
                runeTransform.GetChild(i).GetChild(j).GetChild(0).GetComponent<ParticleSystem>().Stop();
            }
        }
    }

    private void FixedUpdate()
    {
        // Body pointing vector
        targetVector = Vector3.Normalize(targetTransform.position - mouseRayObject.position);

        // Rigid body facing rotation
        rBody.MoveRotation(Quaternion.Euler(new Vector3(0, 0.0f * Mathf.Atan2(targetVector.x, targetVector.z) * Mathf.Rad2Deg, 0)));
        spineController.rotation = Quaternion.Euler(new Vector3(0, 1.0f * Mathf.Atan2(targetVector.x, targetVector.z) * Mathf.Rad2Deg, 0));

        rightHandController.position = rightHandTarget.position;
        rightHandController.rotation = Quaternion.LookRotation(mouseRayObject.right, mouseRayObject.forward);
    }

    // Called by the base layer of Mage Animator Controller
    //private void OnAnimatorIK()
    //{
    //    // Wand targeting IK
    //    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.4f);
    //    animator.SetIKPosition(AvatarIKGoal.RightHand, targetTransform.position);
    //}

    private void Damage()
    {
        if (currentHealth > 0)
        {
            currentHealth -= 10;
        }
        healthBar.setHealth(currentHealth);
    }
}

