using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageController : MonoBehaviour
{
    public Transform groundCheckerTransform;
    public float groundCheckerRadius = 0.2f;

    public Transform targetTransform;
    public Transform wandTransform;
    public Transform wandMagicTransform;
    public Transform runeTransform;

    public LayerMask mouseAimMask;

    private Animator animator;
    private Rigidbody rBody;
    private ParticleSystem wandMagicParticle;
    private AudioSource wandMagicSound;
    private TrailRenderer wandMagicTrail;
    private enum gamePhase { Idle, Spelling, Casting };
    private gamePhase currentPhase;
    private enum runeHolder { Left, Middle, Right };
    private runeHolder currentRune;

    private Camera mainCamera;
    private Vector3 targetVector;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rBody = GetComponent<Rigidbody>();
        wandMagicParticle = wandMagicTransform.GetComponent<ParticleSystem>();
        wandMagicSound = wandMagicTransform.GetComponent<AudioSource>();
        wandMagicTrail = wandMagicTransform.GetComponent<TrailRenderer>();
        mainCamera = Camera.main;

        currentPhase = gamePhase.Idle;
        currentRune = runeHolder.Left;
        Cursor.visible = false;
        wandMagicTrail.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Mouse aim hit to invisible mouse target (Mouse Target layer)
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseAimMask))
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
        }

        if (currentPhase == gamePhase.Spelling)
        {
            SpellingPhase();
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
        if (currentRune == runeHolder.Left) {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                runeTransform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                runeTransform.GetChild(0).GetChild(0).GetComponent<Light>().enabled = true;
                runeTransform.GetChild(0).GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Play();
                currentRune = runeHolder.Middle;
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                runeTransform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().enabled = true;
                runeTransform.GetChild(0).GetChild(1).GetComponent<Light>().enabled = true;
                runeTransform.GetChild(0).GetChild(1).GetChild(0).GetComponent<ParticleSystem>().Play();
                currentRune = runeHolder.Middle;
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                runeTransform.GetChild(0).GetChild(2).GetComponent<MeshRenderer>().enabled = true;
                runeTransform.GetChild(0).GetChild(2).GetComponent<Light>().enabled = true;
                runeTransform.GetChild(0).GetChild(2).GetChild(0).GetComponent<ParticleSystem>().Play();
                currentRune = runeHolder.Middle;
            }
        }
        else if (currentRune == runeHolder.Middle)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                runeTransform.GetChild(1).GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                runeTransform.GetChild(1).GetChild(0).GetComponent<Light>().enabled = true;
                runeTransform.GetChild(1).GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Play();
                currentRune = runeHolder.Right;
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                runeTransform.GetChild(1).GetChild(1).GetComponent<MeshRenderer>().enabled = true;
                runeTransform.GetChild(1).GetChild(1).GetComponent<Light>().enabled = true;
                runeTransform.GetChild(1).GetChild(1).GetChild(0).GetComponent<ParticleSystem>().Play();
                currentRune = runeHolder.Right;
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                runeTransform.GetChild(1).GetChild(2).GetComponent<MeshRenderer>().enabled = true;
                runeTransform.GetChild(1).GetChild(2).GetComponent<Light>().enabled = true;
                runeTransform.GetChild(1).GetChild(2).GetChild(0).GetComponent<ParticleSystem>().Play();
                currentRune = runeHolder.Right;
            }
        }
        else if (currentRune == runeHolder.Right)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                runeTransform.GetChild(2).GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                runeTransform.GetChild(2).GetChild(0).GetComponent<Light>().enabled = true;
                runeTransform.GetChild(2).GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Play();
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                runeTransform.GetChild(2).GetChild(1).GetComponent<MeshRenderer>().enabled = true;
                runeTransform.GetChild(2).GetChild(1).GetComponent<Light>().enabled = true;
                runeTransform.GetChild(2).GetChild(1).GetChild(0).GetComponent<ParticleSystem>().Play();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                runeTransform.GetChild(2).GetChild(2).GetComponent<MeshRenderer>().enabled = true;
                runeTransform.GetChild(2).GetChild(2).GetComponent<Light>().enabled = true;
                runeTransform.GetChild(2).GetChild(2).GetChild(0).GetComponent<ParticleSystem>().Play();
            }
        }

    }

    private void FixedUpdate()
    {
        // Body pointing vector
        targetVector = Vector3.Normalize(targetTransform.position - transform.position);

        // Rigid body facing rotation
        rBody.MoveRotation(Quaternion.Euler(new Vector3(0, (Mathf.Atan2(targetVector.x, targetVector.z) / Mathf.PI) * 180 , 0)));
    }

    private void OnAnimatorIK()
    {
        // Wand targeting IK
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKPosition(AvatarIKGoal.RightHand, targetTransform.position);
    }
}
