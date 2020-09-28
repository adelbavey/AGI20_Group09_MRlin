using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    public AudioSource casting;
    public AudioSource explosion;

    private Vector3 castDir;
    private int[] runeElements;
    private bool explosionAudio = false;
    public void SetUp(Transform wandTransform, int[] runeElements)
    {
        this.castDir = wandTransform.right;
        this.runeElements = runeElements;
        transform.GetChild(0).GetChild(this.runeElements[0]).GetComponent<ParticleSystem>().Play();
        transform.GetChild(1).GetChild(this.runeElements[1]).GetComponent<ParticleSystem>().Play();
        transform.GetChild(2).GetChild(this.runeElements[2]).GetComponent<ParticleSystem>().Play();
        casting.Play();
        Destroy(gameObject, 5f);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float castSpeed = 30f;
        transform.position += castDir * castSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the collision is not caused by mage
        if (other.GetComponent<MageController>() == null)
        {
            //Debug.Log("Hit");
            transform.GetChild(0).GetChild(this.runeElements[0]).GetComponent<ParticleSystem>().Stop();
            transform.GetChild(1).GetChild(this.runeElements[1]).GetComponent<ParticleSystem>().Stop();
            transform.GetChild(2).GetChild(this.runeElements[2]).GetComponent<ParticleSystem>().Stop();
            transform.GetChild(3).GetComponent<ParticleSystem>().Play();

            if (explosionAudio == false)
            {
                explosion.Play();
                explosionAudio = true;
            }
        }
    }
}
