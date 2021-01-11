using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Animations.Rigging;
using Vexpot.ColorTrackerDemo;

public class MageControllerNew : MonoBehaviour
{
    [Tooltip("User testing")]
    public bool testingMode;
    public Transform patternDisplay;
    [Tooltip("If this is controlled by a phone")]
    public bool gyroInteraction = false;
    [Tooltip("If this is an AI")]
    public bool isBot = false;
    [Tooltip("1: Player 1; 2: Player 2")]
    public int playerIndex = 1;
    public Transform opponentTransform;
    public Transform targetTransform;
    public Transform wandTransform;
    [SerializeField]
    private Transform wandMagicTransform;

    public List<Transform> ourIslandTransforms;
    public List<Transform> oppoIslandTransforms;
    public List<Transform> islandHighlightTransforms;

    public LayerMask mouseAimMask;
    public LayerMask highlightLayer1; // Our layer
    public LayerMask highlightLayer2; // Opponent's layer

    public int currentHealth; // Our health value
    public int oppoHealth; // Opponent's health value
    public HealthBar healthBar1; // Our bar
    public HealthBar healthBar2; // Opponent's bar

    public event EventHandler<OnCastingStartsEventArgs> OnCastingStarts;
    public class OnCastingStartsEventArgs : EventArgs
    {
        public Transform wandTransform;
        public Transform islandTargetTransform;
        public int spell;
    }

    public int gestureCode;

    //Private fields;
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
    private Transform headFireSmall;
    [SerializeField]
    private VisualEffectAsset fireSmallVFX1;
    [SerializeField]
    private VisualEffectAsset fireSmallVFX2;
    [SerializeField]
    private VisualEffectAsset fireSmallVFX3;

    [SerializeField]
    private Transform headGlow;
    [SerializeField]
    private VisualEffectAsset headGlowVFX1;
    [SerializeField]
    private VisualEffectAsset headGlowVFX2;

    [SerializeField]
    private Transform headSparks;
    [SerializeField]
    private VisualEffectAsset headSparksVFX1;
    [SerializeField]
    private VisualEffectAsset headSparksVFX2;

    [SerializeField]
    private Transform shieldTransform;

    public AudioSource hitAudio;
    [SerializeField]
    private AudioSource shieldAudio;

    private ParticleSystem wandMagicParticle;
    private AudioSource wandMagicSound;
    private TrailRenderer wandMagicTrail;
    private enum gamePhase { Idle, Spelling, Selecting, Shooting };
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

    public enum magePos { Left, Middle, Right };

    public magePos currentPos; // Our position
    public magePos oppoPos; // Opponent's position
    public bool shieldProtected;

    private Vector3 oldMagePosition;

    private Transform currentTarget;
    private magePos targetPos;

    private int shieldLeft;
    private int oppoShieldLeft;

    // Bot auto control
    private float oldMoveFactor;
    private float moveFactor;
    private float oldSpellFactor;
    private float spellFactor;

    private bool gameOver;
    private bool justMoved;

    // Camera movement
    FruitTrailRenderer ftr;

    // Start is called before the first frame update
    void Start()
    {
        gameOver = false;

        headFireBig.GetComponent<VisualEffect>().Stop();
        headFireSmall.GetComponent<VisualEffect>().Stop();

        if (playerIndex == 1)
        {
            headGlow.GetComponent<VisualEffect>().visualEffectAsset = headGlowVFX1;
            headSparks.GetComponent<VisualEffect>().visualEffectAsset = headSparksVFX1;
            highlightLayer1 = LayerMask.GetMask("Highlight1");
            highlightLayer2 = LayerMask.GetMask("Highlight2");

            patternDisplay.GetComponent<MRlinTesting>().testingMode = testingMode == true ? true : false;
        }
        else if (playerIndex == 2)
        {
            headGlow.GetComponent<VisualEffect>().visualEffectAsset = headGlowVFX2;
            headSparks.GetComponent<VisualEffect>().visualEffectAsset = headSparksVFX2;
            highlightLayer1 = LayerMask.GetMask("Highlight2");
            highlightLayer2 = LayerMask.GetMask("Highlight1");
        }
        else
        {
            Debug.LogError("Player index out of bounds.");
        }

        rBody = GetComponent<Rigidbody>();
        currentSpell = spellElement.Void;
        gestureCode = 0;

        wandMagicParticle = wandMagicTransform.GetComponent<ParticleSystem>();
        wandMagicSound = wandMagicTransform.GetComponent<AudioSource>();
        wandMagicSound.volume = 0.8f;
        wandMagicTrail = wandMagicTransform.GetComponent<TrailRenderer>();
        mainCamera = Camera.main;

        currentPhase = gamePhase.Idle;

        Cursor.visible = gyroInteraction == true ? true : false;

        wandMagicTrail.enabled = false;

        currentHealth = 100;
        oppoHealth = 100;
        healthBar1.setHealth(currentHealth);
        healthBar2.setHealth(oppoHealth);
        currentPos = magePos.Left;
        oppoPos = magePos.Left;

        shieldLeft = 3;
        oppoShieldLeft = 3;
        shieldProtected = false;

        for (int i = 0; i < 3; i++)
        {
            islandHighlightTransforms[i].GetComponent<MeshRenderer>().enabled = false;
        }

        if (isBot)
        {
            oldMoveFactor = 0.5f;
            moveFactor = 0.5f;
            StartCoroutine(moveFactorGen());
            StartCoroutine(spellFactorGen());
        }

        ftr = GameObject.FindGameObjectWithTag("TrackCamera").GetComponent<FruitTrailRenderer>();
        justMoved = false;

        if (testingMode) moveRight();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth > 0 && oppoHealth > 0)
        {
            healthBar1.setHealth(currentHealth);
            healthBar2.setHealth(oppoHealth);
        }
        else if (currentHealth > 0 && oppoHealth <= 0)
        {
            healthBar1.setHealth(currentHealth);
            healthBar2.setHealth(0);
            endGame(true);
        }
        else if (currentHealth <= 0 && oppoHealth > 0)
        {
            healthBar1.setHealth(0);
            healthBar2.setHealth(oppoHealth);
            endGame(false);
        }
        else
        {
            Debug.LogError("Unlikely situation: both lost");
        }

        if (!gameOver)
        {
            if (!isBot)
            {
                mainCamera.transform.rotation = Quaternion.LookRotation(
                    Vector3.Normalize(oppoIslandTransforms[1].position - mainCamera.transform.position));

                if (gyroInteraction)
                {
                    Ray newRay = new Ray(mouseRayObject.position, mouseRayObject.forward);

                    RaycastHit hit;
                    if (Physics.Raycast(newRay, out hit, Mathf.Infinity, mouseAimMask))
                    {
                        targetTransform.position = hit.point;
                    }

                    // Both mouse and phone screen tap
                    StateMachineTransition();

                    //if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                    //{
                    //    moveLeft();
                    //}
                    //if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                    //{
                    //    moveRight();
                    //}

                    //Camera-controlled movement
                    if (!testingMode)
                    {
                        Vector3 positionTarget = ftr.getTargetPos();
                        float x = positionTarget.x;

                        //Debug.Log(x);

                        if (x <= -1.5)
                        {
                            if (currentPos == magePos.Middle)
                            {
                                moveLeft();
                            }
                            else if (currentPos == magePos.Right)
                            {
                                moveLeft();
                                moveLeft();
                            }
                        }
                        else if (x >= 1.5)
                        {
                            if (currentPos == magePos.Middle)
                            {
                                moveRight();
                            }
                            else if (currentPos == magePos.Left)
                            {
                                moveRight();
                                moveRight();
                            }
                        }
                        else
                        {
                            if (currentPos == magePos.Left)
                            {
                                moveRight();
                            }
                            else if (currentPos == magePos.Right)
                            {
                                moveLeft();
                            }
                        }
                    }
                }

                // Keyboard and mouse interaction
                else if (!gyroInteraction)
                {
                    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    // Mouse aim hit to invisible mouse target (Mouse Target layer)
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseAimMask))
                    {
                        targetTransform.position = hit.point;
                        mouseRayObject.rotation = Quaternion.LookRotation(ray.direction);
                    }

                    // Both mouse and phone screen tap
                    StateMachineTransition();

                    if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        moveLeft();
                    }
                    if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        moveRight();
                    }
                }
            }
            else // Bot behaviors
            {
                RaycastHit hit;
                if (Physics.Raycast(mouseRayObject.position, targetVector, out hit, Mathf.Infinity, highlightLayer1))
                {
                    currentTarget = hit.transform;
                }

                targetTransform.position = islandHighlightTransforms[2 - (int)oppoPos].position;

                mouseRayObject.rotation = Quaternion.LookRotation(targetTransform.position - mouseRayObject.position);

                if (oldMoveFactor != moveFactor)
                {
                    if (moveFactor <= 0.5f)
                    {
                        oldMoveFactor = moveFactor;
                        if (currentPos == magePos.Left)
                        {
                            moveRight();
                        }
                        else
                        {
                            moveLeft();
                        }
                    }
                    else
                    {
                        oldMoveFactor = moveFactor;
                        if (currentPos == magePos.Right)
                        {
                            moveLeft();
                        }
                        else
                        {
                            moveRight();
                        }
                    }
                }
                botShooting();
            }
        }
    }

    private void moveRight()
    {
        if (currentPhase != gamePhase.Spelling)
        {
            if (currentPos == magePos.Middle)
            {
                if (!justMoved)
                {
                    justMoved = true;
                    StartCoroutine(justMovedChange());

                    oldMagePosition = transform.position;
                    transform.position = ourIslandTransforms[2].position + (oldMagePosition - ourIslandTransforms[1].position);
                    if (!isBot) mainCamera.transform.position += transform.position - oldMagePosition;
                    currentPos = magePos.Right;
                    opponentTransform.GetComponent<MageControllerNew>().oppoPos = magePos.Right;
                }
            }
            else if (currentPos == magePos.Left)
            {
                if (!justMoved)
                {
                    justMoved = true;
                    StartCoroutine(justMovedChange());

                    oldMagePosition = transform.position;
                    transform.position = ourIslandTransforms[1].position + (oldMagePosition - ourIslandTransforms[0].position);
                    if (!isBot) mainCamera.transform.position += transform.position - oldMagePosition;
                    currentPos = magePos.Middle;
                    opponentTransform.GetComponent<MageControllerNew>().oppoPos = magePos.Middle;
                }
            }
        }
    }

    private void moveLeft()
    {
        if (currentPhase != gamePhase.Spelling)
        {
            if (currentPos == magePos.Middle)
            {
                if (!justMoved)
                {
                    justMoved = true;
                    StartCoroutine(justMovedChange());

                    oldMagePosition = transform.position;
                    transform.position = ourIslandTransforms[0].position + (oldMagePosition - ourIslandTransforms[1].position);
                    if (!isBot) mainCamera.transform.position += transform.position - oldMagePosition;
                    currentPos = magePos.Left;
                    opponentTransform.GetComponent<MageControllerNew>().oppoPos = magePos.Left;
                }
            }
            else if (currentPos == magePos.Right)
            {
                if (!justMoved)
                {
                    justMoved = true;
                    StartCoroutine(justMovedChange());

                    oldMagePosition = transform.position;
                    transform.position = ourIslandTransforms[1].position + (oldMagePosition - ourIslandTransforms[2].position);
                    if (!isBot) mainCamera.transform.position += transform.position - oldMagePosition;
                    currentPos = magePos.Middle;
                    opponentTransform.GetComponent<MageControllerNew>().oppoPos = magePos.Middle;
                }
            }
        }
    }

    private void botShooting()
    {
        if (oldSpellFactor != spellFactor)
        {
            if (spellFactor <= 1.5f)
            {
                oldSpellFactor = spellFactor;
                currentSpell = spellElement.Ice;
                headFireBig.GetComponent<VisualEffect>().visualEffectAsset = fireBigVFX1;
                headFireSmall.GetComponent<VisualEffect>().visualEffectAsset = fireSmallVFX1;

            }
            else if (spellFactor > 1.5f && spellFactor <= 2.4f)
            {
                oldSpellFactor = spellFactor;
                currentSpell = spellElement.Storm;
                headFireBig.GetComponent<VisualEffect>().visualEffectAsset = fireBigVFX2;
                headFireSmall.GetComponent<VisualEffect>().visualEffectAsset = fireSmallVFX2;

            }
            else if (spellFactor > 2.4f && spellFactor <= 3.2f)
            {
                oldSpellFactor = spellFactor;
                currentSpell = spellElement.Fire;
                headFireBig.GetComponent<VisualEffect>().visualEffectAsset = fireBigVFX3;
                headFireSmall.GetComponent<VisualEffect>().visualEffectAsset = fireSmallVFX3;
            }
            else
            {
                oldSpellFactor = spellFactor;
                currentSpell = spellElement.Shield;

                if (shieldLeft > 0)
                {
                    currentSpell = spellElement.Shield;
                    shieldProtected = true;
                    shieldAudio.Play();

                    Transform spellTransform = Instantiate(
                        shieldTransform,
                        transform.position + new Vector3(0, 0.5f, 0),
                        transform.rotation
                        );

                    shieldLeft -= 1;
                    opponentTransform.GetComponent<MageControllerNew>().oppoShieldLeft -= 1;

                    opponentTransform.GetComponent<MageControllerNew>().healthBar2.setShield(shieldLeft);
                    healthBar1.setShield(shieldLeft);
                    healthBar2.setShield(oppoShieldLeft);
                    StartCoroutine(DestroyShield(spellTransform, 3.0f));
                }
                else
                {
                    spellFactor = UnityEngine.Random.Range(0.0f, 3.2f);
                    Debug.Log("Auto change to attack spell!");
                }
            }


            // Start shooting
            OnCastingStarts?.Invoke(this, new OnCastingStartsEventArgs
            {
                wandTransform = this.wandTransform,
                islandTargetTransform = this.currentTarget,
                spell = (int)currentSpell
            });

            if (currentTarget.name == "Select_Highlight_2")
            {
                targetPos = magePos.Left;
            }
            else if (currentTarget.name == "Select_Highlight_1")
            {
                targetPos = magePos.Middle;
            }
            else if (currentTarget.name == "Select_Highlight_0")
            {
                targetPos = magePos.Right;
            }

            if (currentSpell == spellElement.Ice)
            {
                StartCoroutine(Damage(1, 0.707f));
            }
            else if (currentSpell == spellElement.Storm)
            {
                StartCoroutine(Damage(2, 1.0f));
            }
            else if (currentSpell == spellElement.Fire)
            {
                StartCoroutine(Damage(3, 1.414f));
            }
        }
    }

    private void StateMachineTransition()
    {
        if (currentPhase == gamePhase.Idle)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartSpellingPhase(true);
            }
        }
        else if (currentPhase == gamePhase.Spelling)
        {
            if (Input.GetMouseButtonUp(0))
            {
                StartSpellingPhase(false);
                StartSelectingPhase(true);
            }
        }
        else if (currentPhase == gamePhase.Selecting)
        {
            if (currentSpell != spellElement.Void)
            {
                SelectingPhase();
                if (Input.GetMouseButtonDown(0))
                {
                    StartSelectingPhase(false);
                    StartShootingPhase(true);
                }
            }
            else
            {
                currentPhase = gamePhase.Idle;
            }
        }
        else if (currentPhase == gamePhase.Shooting)
        {
            if (Input.GetMouseButtonUp(0))
            {
                StartShootingPhase(false);
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

            transform.GetComponent<PlayerInputMRlin>().gestureRecognition = true;
        }
        else
        {
            currentPhase = gamePhase.Idle;
            wandMagicParticle.Stop();
            wandMagicSound.Stop();
            wandMagicTrail.enabled = false;
            wandMagicTrail.Clear();

            transform.GetComponent<PlayerInputMRlin>().gestureRecognition = false;
            transform.GetComponent<PlayerInputMRlin>().patternRecognition();
        }
    }

    private void StartSelectingPhase(bool b)
    {
        if (b)
        {
            if (gestureCode == 4)
            {
                if (shieldLeft > 0)
                {
                    currentPhase = gamePhase.Idle; // Skip selection and shooting

                    currentSpell = spellElement.Shield;
                    shieldProtected = true;
                    shieldAudio.Play();

                    Transform spellTransform = Instantiate(
                        shieldTransform,
                        transform.position + new Vector3(0, 0.5f, 0),
                        transform.rotation
                        );

                    shieldLeft -= 1;
                    opponentTransform.GetComponent<MageControllerNew>().oppoShieldLeft -= 1;

                    opponentTransform.GetComponent<MageControllerNew>().healthBar2.setShield(shieldLeft);
                    healthBar1.setShield(shieldLeft);
                    healthBar2.setShield(oppoShieldLeft);
                    StartCoroutine(DestroyShield(spellTransform, 3.0f));
                }
                else
                {
                    healthBar1.setShield(shieldLeft);
                    healthBar2.setShield(oppoShieldLeft);
                    Debug.Log("No Shield!");
                    currentPhase = gamePhase.Idle; // Skip selection and shooting
                }
            }
            else
            {
                currentPhase = gamePhase.Selecting;

                // Handle the result of the spelling phase
                switch (gestureCode)
                {
                    case 0:
                        currentSpell = spellElement.Void;
                        break;
                    case 1:
                        currentSpell = spellElement.Ice;
                        headFireBig.GetComponent<VisualEffect>().visualEffectAsset = fireBigVFX1;
                        headFireSmall.GetComponent<VisualEffect>().visualEffectAsset = fireSmallVFX1;
                        break;
                    case 2:
                        currentSpell = spellElement.Storm;
                        headFireBig.GetComponent<VisualEffect>().visualEffectAsset = fireBigVFX2;
                        headFireSmall.GetComponent<VisualEffect>().visualEffectAsset = fireSmallVFX2;
                        break;
                    case 3:
                        currentSpell = spellElement.Fire;
                        headFireBig.GetComponent<VisualEffect>().visualEffectAsset = fireBigVFX3;
                        headFireSmall.GetComponent<VisualEffect>().visualEffectAsset = fireSmallVFX3;
                        break;
                    default:
                        currentSpell = spellElement.Void;
                        break;
                }
            }
        }
        else
        {
            currentPhase = gamePhase.Idle;
            for (int i = 0; i < 3; i++)
            {
                islandHighlightTransforms[i].GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    private void StartShootingPhase(bool b)
    {
        if (b)
        {
            currentPhase = gamePhase.Shooting;

            // Check if the subscriber is null and pass in spell element info
            OnCastingStarts?.Invoke(this, new OnCastingStartsEventArgs
            {
                wandTransform = this.wandTransform,
                islandTargetTransform = this.currentTarget,
                spell = (int)currentSpell
            });

            if (currentTarget.name == "Select_Highlight_2")
            {
                targetPos = magePos.Left;
            }
            else if (currentTarget.name == "Select_Highlight_1")
            {
                targetPos = magePos.Middle;
            }
            else if (currentTarget.name == "Select_Highlight_0")
            {
                targetPos = magePos.Right;
            }

            if (currentSpell == spellElement.Ice)
            {
                StartCoroutine(Damage(1, 0.707f));
            }
            else if (currentSpell == spellElement.Storm)
            {
                StartCoroutine(Damage(2, 1.0f));
            }
            else if (currentSpell == spellElement.Fire)
            {
                StartCoroutine(Damage(3, 1.414f));
            }
        }
        else
        {
            currentPhase = gamePhase.Idle;
            currentSpell = spellElement.Void;
            headFireBig.GetComponent<VisualEffect>().Stop();
            headFireSmall.GetComponent<VisualEffect>().Stop();
        }
    }

    private void SelectingPhase()
    {
        RaycastHit hit;
        if (Physics.Raycast(mouseRayObject.position, targetVector, out hit, Mathf.Infinity, highlightLayer1))
        {
            for (int i = 0; i < 3; i++)
            {
                islandHighlightTransforms[i].GetComponent<MeshRenderer>().enabled = false;
            }
            hit.transform.GetComponent<MeshRenderer>().enabled = true;
            currentTarget = hit.transform;
        }
    }

    void FixedUpdate()
    {
        // Body pointing vector
        targetVector = Vector3.Normalize(targetTransform.position - mouseRayObject.position);

        // Rigid body facing rotation
        if (!isBot)
        {
            rBody.MoveRotation(Quaternion.Euler(new Vector3(0, 0.0f * Mathf.Atan2(targetVector.x, targetVector.z) * Mathf.Rad2Deg, 0)));
        }

        spineController.rotation = Quaternion.Euler(new Vector3(0, 1.0f * Mathf.Atan2(targetVector.x, targetVector.z) * Mathf.Rad2Deg, 0));

        rightHandController.position = rightHandTarget.position;
        rightHandController.rotation = Quaternion.LookRotation(mouseRayObject.right, mouseRayObject.forward);
    }

    private IEnumerator Damage(int spell, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (!opponentTransform.GetComponent<MageControllerNew>().shieldProtected && !gameOver)
        {
            if (oppoPos == targetPos)
            {
                opponentTransform.GetComponent<MageControllerNew>().hitAudio.volume = 0.4f;
                opponentTransform.GetComponent<MageControllerNew>().hitAudio.Play();
                if (opponentTransform.GetComponent<MageControllerNew>().gyroInteraction)
                {
                    opponentTransform.GetComponent<MageControllerNew>().vibratePhone();
                }

                if (spell == 1)
                {
                    oppoHealth -= 7;
                    opponentTransform.GetComponent<MageControllerNew>().currentHealth -= 7;
                    opponentTransform.GetComponent<Animator>().SetTrigger("Hit");
                }
                else if (spell == 2)
                {
                    oppoHealth -= 10;
                    opponentTransform.GetComponent<MageControllerNew>().currentHealth -= 10;
                    opponentTransform.GetComponent<Animator>().SetTrigger("Hit");
                }
                else if (spell == 3)
                {
                    oppoHealth -= 14;
                    opponentTransform.GetComponent<MageControllerNew>().currentHealth -= 14;
                    opponentTransform.GetComponent<Animator>().SetTrigger("Hit");
                }
            }
        }
    }

    private IEnumerator DestroyShield(Transform shieldInstance, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        shieldProtected = false;
        foreach (Transform child in shieldInstance)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject.Destroy(shieldInstance.gameObject);
    }

    private IEnumerator moveFactorGen()
    {
        yield return new WaitForSeconds(5);
        while (true)
        {
            moveFactor = UnityEngine.Random.Range(0.0f, 1.0f);
            yield return new WaitForSeconds(5);
        }
    }

    private IEnumerator spellFactorGen()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {
            spellFactor = UnityEngine.Random.Range(0.0f, 4.0f);
            yield return new WaitForSeconds(5);
        }
    }

    private IEnumerator justMovedChange()
    {
        yield return new WaitForSeconds(1.0f);
        justMoved = false;
    }

    private void endGame(bool win)
    {
        gameOver = true;
        if (win)
        {
            Debug.Log("Player " + playerIndex + " won!");
            //opponentTransform.GetChild(4).GetComponent<Rig>().weight = 0;
            //opponentTransform.GetComponent<Animator>().SetTrigger("Dead");
        }
        else
        {
            Debug.Log("Player " + opponentTransform.GetComponent<MageControllerNew>().playerIndex + " won!");
            transform.GetChild(3).GetComponent<Rig>().weight = 0;
            transform.GetComponent<Animator>().SetTrigger("Dead");
        }
    }

    public Vector3 getTargetPos()
    {
        return targetTransform.position;
    }

    public void vibratePhone()
    {
#if UNITY_IOS
      Handheld.Vibrate();
#elif UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }
}


// Called by the base layer of Mage Animator Controller
//private void OnAnimatorIK()
//{
//    // Wand targeting IK
//    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.4f);
//    animator.SetIKPosition(AvatarIKGoal.RightHand, targetTransform.position);
//}