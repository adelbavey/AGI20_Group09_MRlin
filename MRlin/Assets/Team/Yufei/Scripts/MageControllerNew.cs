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
    //public Transform runeTransform;
    //public AudioSource leftRune;
    //public AudioSource midRune;
    //public AudioSource rightRune;

    public LayerMask mouseAimMask;
    public Transform gyroRayObject;

    public int currentHealth;
    public HealthBar healthBar;

    public event EventHandler<OnCastingStartsEventArgs> OnCastingStarts;
    public class OnCastingStartsEventArgs : EventArgs
    {
        public Transform wandTransform;
        public int spell;
    }

    //private Animator animator;
    private Rigidbody rBody;
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

    /* Void: No successful spell.
     * Ice Spell: Low damage, high speed.
     * Storm Spell: Medium damage, medium speed.
     * Fire Spell: High damage, low speed. 
     * Shield: To deflect opponent's spell. */
    private enum spellElement { Void, Ice, Storm, Fire, Shield };
    private spellElement currentSpell;
    private Camera mainCamera;
    private Vector3 targetVector;

    // Start is called before the first frame update
    void Start()
    {
        //animator = GetComponent<Animator>();
        rBody = GetComponent<Rigidbody>();
        currentSpell = spellElement.Void;

        wandMagicParticle = wandMagicTransform.GetComponent<ParticleSystem>();
        wandMagicSound = wandMagicTransform.GetComponent<AudioSource>();
        wandMagicSound.volume = 0.5f;
        wandMagicTrail = wandMagicTransform.GetComponent<TrailRenderer>();
        mainCamera = Camera.main;

        currentPhase = gamePhase.Idle;

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
                    spell = (int)currentSpell
                });
                currentPhase = gamePhase.Casting;
                Invoke("Damage", 1.0f);
            }
        }
        else // Keyboard and mouse interaction
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
                    spell = (int)currentSpell
                });
                currentPhase = gamePhase.Casting;
                Invoke("Damage", 1.0f);
            }

            if (currentPhase == gamePhase.Spelling)
            {
                SpellingPhase();
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {

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
            currentSpell = spellElement.Ice;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            headFireBig.GetComponent<VisualEffect>().visualEffectAsset = fireBigVFX2;
            headGlow.GetComponent<VisualEffect>().visualEffectAsset = headGlowVFX2;
            currentSpell = spellElement.Storm;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            headFireBig.GetComponent<VisualEffect>().visualEffectAsset = fireBigVFX3;
            headGlow.GetComponent<VisualEffect>().visualEffectAsset = headGlowVFX3;
            currentSpell = spellElement.Fire;
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

