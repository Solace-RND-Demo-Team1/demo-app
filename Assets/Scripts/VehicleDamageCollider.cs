using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleDamageCollider : MonoBehaviour {

    public GameObject Vehicle { get { return prefab; } }
    public AudioClip CarCrash1 { get { return crashClip1; } }

    [SerializeField] private GameObject prefab;
    [SerializeField] private AudioClip crashClip1;

    private PlayerDamage playerHealthScript;

    private bool damageBeingInflicted = false;

    // Use this for initialization
    void Start () {
        playerHealthScript = prefab.GetComponent<PlayerDamage>();
    }

    private void OnTriggerExit(Collider collision)
    {
        damageBeingInflicted = false;
    }

        private void OnTriggerEnter(Collider collision)
    {
        if (damageBeingInflicted == false)
        {
            damageBeingInflicted = true;

            int damage = 0;
            switch (collision.gameObject.name)
            {
                case "BumperCollider":
                    damage = 1500;
                    break;
                case "ColliderBodyDamage":
                    damage = 250;
                    break;
                case "ColliderBottom":
                    damage = 500;
                    break;
                case "ColliderFront":
                    damage = 600;
                    break;
                
                default:
                    damage = 0;
                    break;
            }
            if (damage > 0)
            {                
                if (playerHealthScript != null)
                {
                    Debug.Log("Collider Event: " + collision.gameObject.name + ", Damage: " + damage.ToString());

                    playerHealthScript.InflictDamage(damage);

                    AudioSource audSrc = prefab.AddComponent<AudioSource>();
                    audSrc.PlayOneShot(crashClip1);

                }
            }
        }
    }
}
