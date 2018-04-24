using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLandingDust : MonoBehaviour {

    [SerializeField] private GameObject dustParticleEffect;
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "ColliderBottom")
        {
            dustParticleEffect.SetActive(true);
            Debug.Log("Particles Activated");
        }
    }
}
