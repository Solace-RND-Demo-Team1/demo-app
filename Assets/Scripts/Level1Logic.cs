using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1Logic : MonoBehaviour {

    public GameObject Vehicle { get { return prefab; } }
    public GameObject Vehicle2 { get { return prefab2; } }
    public GameObject ContainerGroup { get { return containerGroup; } }
    public GameObject BillboardText { get { return billboardText; } }
    public GameObject IntroCarGroup { get { return introCarGroup; } }

    public Camera Camera1 { get { return camera1; } }
    public Camera Camera2 { get { return camera2; } }
    public Camera BillboardCamera { get { return camera3; } }

    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject prefab2;

    [SerializeField] private GameObject containerGroup;
    [SerializeField] private GameObject billboardText;

    [SerializeField] private GameObject introCarGroup;


    [SerializeField] private Camera camera1;
    [SerializeField] private Camera camera2;
    [SerializeField] private Camera camera3;



    private const int MAX_VEHICLES = 8;

    private enum States { GameLobby, GameStarted, GameOver, Leaderboard };
    private States gameState = States.GameLobby;

    private GameObject[] vehicles = new GameObject[MAX_VEHICLES];
    private string[] playerTags = new string[MAX_VEHICLES];
    private int[] playerScores = new int[MAX_VEHICLES];
    
    private PlayerDamage[] playerHealth = new PlayerDamage[MAX_VEHICLES];

    private TextMeshPro mText;

    private Vector3 spawnPoint1 = new Vector3(-111.62f, 15f, -75.16f);
    private Vector3 spawnPoint2 = new Vector3(48.64f, 15f, -51.8f);
    
    private int currentGameCameraIndex = 0;

    // Init
    void Awake()
    {
        mText = billboardText.GetComponent<TextMeshPro>();

        StartGameLobby();
      
    }

    public void DisplayBillboardText(string text)
    {
        mText.text = text;
    }

    public void ToggleGameCamera()
    {
        currentGameCameraIndex++;
        currentGameCameraIndex = currentGameCameraIndex > 2 ? 0 : currentGameCameraIndex;
        switchCameraView(currentGameCameraIndex);
    }

    private void switchCameraView(int viewId)
    {
        switch (viewId)
        {
            case 0:
                camera1.enabled = true;
                camera1.tag = "MainCamera";

                camera2.enabled = false;
                camera2.tag = "Untagged";

                camera3.enabled = false;
                camera3.tag = "Untagged";
                break;
            case 1:
                camera1.enabled = false;
                camera1.tag = "Untagged";

                camera2.enabled = true;
                camera2.tag = "MainCamera";

                camera3.enabled = true;
                camera3.tag = "Untagged";
                break;
            case 2:
            default:
                camera1.enabled = false;
                camera1.tag = "Untagged";

                camera2.enabled = false;
                camera2.tag = "Untagged";

                camera3.enabled = true;
                camera3.tag = "MainCamera";
                break;
        }
    }
    public void PlayerSubstitution (int playerId, string playerTag, int score, int lives)
    {
        int i = playerId;
        if ((gameState == States.GameLobby || gameState == States.GameStarted) &&
            playerId >= 0 && playerId < MAX_VEHICLES)
        {
            // Initialize player vehicle
            //
            GameObject spawnVehicle = prefab;
         
            //if (currentSpawnPoint == 1)
            //{
            //    spawnVehicle = prefab2;
            //}

            playerTags[i] = playerTag;
            playerScores[i] = score;
            
            // Create player vehicle
            vehicles[i] = Instantiate(spawnVehicle, containerGroup.transform);
            vehicles[i].name = "Vehicle-" + i.ToString();

            playerHealth[i] = vehicles[i].GetComponent<PlayerDamage>();

            respawnPlayer(i);

            MeshRenderer mr = vehicles[i].transform.GetChild(2).GetComponent<MeshRenderer>();
            Material[] materials = new Material[1];
            string resName = "MonsterTruck/Materials/car_03_m" + (i + 1).ToString();
            materials[0] = (Material)Resources.Load(resName, typeof(Material));
            mr.materials = materials;
        }
    }

    private void respawnPlayer(int i)
    {
        int currentSpawnPoint = i & 1;

        Vector3 spawnPosition = spawnPoint1;
        if (currentSpawnPoint == 1)
        {
            spawnPosition = spawnPoint2;
        }

        vehicles[i].transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);

        if (currentSpawnPoint == 0)
        {
            vehicles[i].transform.Rotate(Vector3.up * 90f);
        }
        else
        {
            vehicles[i].transform.Rotate(Vector3.up * -90f);
        }
    }

    public void StartGameLobby ()
    {
        // TODO - Reset to Game Lobby State
        gameState = States.GameLobby;

        // Start GameLobby Camera
        switchCameraView(2);

        introCarGroup.SetActive(true);

        for (int i = 0; i < vehicles.Length; i++)
        {
            if (vehicles[i]) Destroy(vehicles[i]);
        }

        // Wait for Players
    }
    public void StartGame ()
    {
        // TODO - Reset to Game Start state
        gameState = States.GameStarted;

        switchCameraView(0);

        // Remove Game Intro Objects
        //
        introCarGroup.SetActive(false);
        for (int i = 0; i < vehicles.Length; i++)
        {
            if (vehicles[i]) Destroy(vehicles[i]);
        }

        // Create new Vehicles
        PlayerSubstitution(0, "Player1", 0, 1);
        PlayerSubstitution(1, "Player2", 0, 1);
    }

    // Use this for initialization
    private void NewGame () {

        // Start New Game
        //
        StartGameLobby();
        
    }

    private void Update()
    {
        for (int i=0; i < MAX_VEHICLES; i++)
        {
            if (playerHealth[i] != null)
            {
                if (playerHealth[i].PlayerHealth < 0)
                {
                    // Respawn
                    vehicles[i].GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
                    vehicles[i].GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f);


                    playerHealth[i].PlayerHealth = 9000;
                    playerHealth[i].InflictDamage(0);
                    respawnPlayer(i);
                }
            }
        }
        
    }

    private States GameState
    {
        get
        {
            return gameState;
        }

        set
        {
            gameState = value;
        }
    }
}
