using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour {

    public GameObject healthCube1 { get { return cube1; } }
    public GameObject healthCube2 { get { return cube2; } }
    public GameObject healthCube3 { get { return cube3; } }


    public int PlayerId { get { return playerId; } }
    public int PlayerHealth { get { return playerHealth; } set { playerHealth = value; } }

    [SerializeField] private int playerId;
    [SerializeField] private int playerHealth;

    [SerializeField] private GameObject cube1;
    [SerializeField] private GameObject cube2;
    [SerializeField] private GameObject cube3;
    
    private const int MAX_HEALTH = 9000;

    public void InflictDamage(int damage)
    {
        playerHealth -= damage;

        Debug.Log("Player health == " + playerHealth.ToString());

        if (playerHealth > 6000)
        {
            cube1.SetActive(true);
            cube2.SetActive(true);
            cube3.SetActive(true);

        }
        else if (playerHealth > 3000 && playerHealth <= 6000)
        {
            cube1.SetActive(false);
            cube2.SetActive(true);
            cube3.SetActive(true);
        }
        else if (playerHealth > 0 && playerHealth <= 3000)
        {
            cube1.SetActive(false);
            cube2.SetActive(true);
            cube3.SetActive(false); 
        }
        else
        {
            cube1.SetActive(false);
            cube2.SetActive(false);
            cube3.SetActive(false);
        }
    }

    // Use this for initialization
    void Start () {
        playerHealth = MAX_HEALTH;

    }
	
}
