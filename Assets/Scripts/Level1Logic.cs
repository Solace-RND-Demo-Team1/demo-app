using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class Level1Logic : MonoBehaviour {
    
    public bool ProductionMode { get { return production_mode; } }

    public GameObject Vehicle { get { return prefab; } }
    public GameObject Vehicle2 { get { return prefab2; } }
    public GameObject ContainerGroup { get { return containerGroup; } }
    public GameObject BillboardText { get { return billboardText; } }
    public GameObject IntroCarGroup { get { return introCarGroup; } }

    public GameObject SpawnPoints { get { return spawnPointsGroup; } }
    
    public Camera Camera1 { get { return camera1; } }
    public Camera Camera2 { get { return camera2; } }
    public Camera BillboardCamera { get { return camera3; } }

    public GameObject StartGameButtonText { get { return startGameButtonText; } }


    [SerializeField] private bool production_mode;

    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject prefab2;

    [SerializeField] private GameObject containerGroup;
    [SerializeField] private GameObject billboardText;

    [SerializeField] private GameObject introCarGroup;

    [SerializeField] private GameObject spawnPointsGroup;


    [SerializeField] private Camera camera1;
    [SerializeField] private Camera camera2;
    [SerializeField] private Camera camera3;

    [SerializeField]  private GameObject startGameButtonText;


    [DllImport("__Internal")]
    private static extern void ConfigureGameOptions(int maxPlayers);
    [DllImport("__Internal")]
    private static extern void NewGameLobby();
    [DllImport("__Internal")]
    private static extern void GameStart(int numStartingPlayers);
    [DllImport("__Internal")]
    private static extern void PlayerSubstitution(int playerId, string gamerTag, string colour);
    [DllImport("__Internal")]
    private static extern void KillPlayer(int playerId, string gamerTag);

    private const int MAX_VEHICLES = 8;

    private enum States { GameLobby, GameStarted, GameOver, Leaderboard };
    private States gameState = States.GameLobby;

    private GameObject[] vehicles = new GameObject[MAX_VEHICLES];
    private string[] playerTags = new string[MAX_VEHICLES];
    private int[] playerScores = new int[MAX_VEHICLES];

    private string[] carColours = { "Purple", "Dark Orange", "Blue", "Light Orange", "Yellow", "Green", "Grey", "White", "Pink", "Cyan" };

    private PlayerDamage[] playerHealth = new PlayerDamage[MAX_VEHICLES];

    private TextMeshPro mText = null;
    private TextMeshProUGUI startButtonText = null;


    private Vector3[] spawnPoints = new Vector3[MAX_VEHICLES];
    
    private int currentGameCameraIndex = 0;

    private int numStartingPlayers = 0;

    void Awake()
    {
        mText = billboardText.GetComponent<TextMeshPro>();
        startButtonText = startGameButtonText.GetComponent<TextMeshProUGUI>();
        startButtonText.text = "Start";

        for (int i=0; i < MAX_VEHICLES; i++)
        {
            spawnPoints[i] = spawnPointsGroup.transform.GetChild(i).transform.position;
        }

        NewGame();
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

                camera1.GetComponent<Animator>().Play("Camera1", -1, 0f);

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

                camera3.GetComponent<Animator>().Play("BillBoardCam", -1, 0f);
                break;
        }
    }

    public void SubstitutePlayer(string encodedParams)
    {
        string[] args = encodedParams.Split(',');

        int playerId = int.Parse(args[0]);
        string playerTag = args[1];
        int score = int.Parse(args[2]);

        PlayerSubstitution(playerId, playerTag, score);
    }
        
    private void PlayerSubstitution (int playerId, string playerTag, int score)
    {
        int i = playerId;
        if ((gameState == States.GameLobby || gameState == States.GameStarted) &&
            playerId >= 0 && playerId < MAX_VEHICLES)
        {
            // Initialize player vehicle
            //
            GameObject spawnVehicle = prefab;
        
            playerTags[i] = playerTag;
            playerScores[i] = score;

            // Create player vehicle
            if (vehicles[i]) Destroy(vehicles[i]);
            vehicles[i] = Instantiate(spawnVehicle, containerGroup.transform);
            vehicles[i].name = "Vehicle-" + i.ToString();

            playerHealth[i] = vehicles[i].GetComponent<PlayerDamage>();

            respawnPlayerAtPosition(i);

            MeshRenderer mr = vehicles[i].transform.GetChild(2).GetComponent<MeshRenderer>();
            Material[] materials = new Material[1];
            string resName = "MonsterTruck/Materials/car_03_m" + (i + 1).ToString();
            materials[0] = (Material)Resources.Load(resName, typeof(Material));
            mr.materials = materials;

            // Count number of players added so far
            //
            numStartingPlayers++;
            if (numStartingPlayers > MAX_VEHICLES) numStartingPlayers = MAX_VEHICLES ;

            // Notify HTML Page of Player Substitution Event
            //
            if (production_mode) PlayerSubstitution(i, playerTags[i], carColours[i]);
        }
    }

    private void respawnPlayerAtPosition(int i)
    {
        int currentSpawnPoint = i & 1;

        // Position to Spawn point and rotate to hill target
        Vector3 spawnPosition = spawnPoints[i];
        vehicles[i].transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
        vehicles[i].transform.LookAt(spawnPoints[1], Vector3.up);
    }

    public void StartGameLobby ()
    {
        if (production_mode) ConfigureGameOptions(MAX_VEHICLES);

        // TODO - Reset to Game Lobby State
        gameState = States.GameLobby;

        // Start GameLobby Camera
        switchCameraView(2);

        introCarGroup.SetActive(true);

        for (int i = 0; i < vehicles.Length; i++)
        {
            if (vehicles[i]) Destroy(vehicles[i]);
        }

        // Notify HTML Page of Player Substitution Event
        //
        if (production_mode) NewGameLobby();

        // Wait for Players

    }
    public void StartGame()
    {
        if (gameState == States.GameLobby)
        {
            // Game State is now 'Game Started'
            //
            gameState = States.GameStarted;

            switchCameraView(0);

            // Remove Game Intro Objects
            //
            introCarGroup.SetActive(false);
            for (int i = 0; i < vehicles.Length; i++)
            {
                if (vehicles[i]) Destroy(vehicles[i]);
            }

            // Notify HTML Page of Game Started event
            //
            if (production_mode) GameStart(numStartingPlayers);

            // Now that the game is started, The 'Start' button will
            //   take on the role of cycling players through the game (i.e. Kill off a player)
            startButtonText.text = "Next Plyr";

            // Stop Lobby Music
            //
            AudioSource aSrc = transform.gameObject.GetComponent<AudioSource>();
            aSrc.Stop();
            
        }
        else if (gameState == States.GameStarted)
        {
            // Game is Started / Kill Player Mode
            //
            for (int i = 0; i < vehicles.Length; i++)
            {
                if (vehicles[i] != null)
                {
                    // Kill off one player
                    //
                    killPlayer(i);
                    break;
                }
            }
        }
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
                    killPlayer(i);
                }
            }
        }
        
    }

    private void killPlayer(int playerId)
    {
        // Player health below 0 / Player Killed
        //
        if (vehicles[playerId] != null)
        {
            vehicles[playerId].GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            vehicles[playerId].GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f);


            playerHealth[playerId].PlayerHealth = 9000;
            playerHealth[playerId].InflictDamage(0);

            Destroy(vehicles[playerId]);
            vehicles[playerId] = null;

            //respawnPlayerAtPosition(playerId);

            // Notify HTML Page of Player Kill event
            //
            if (production_mode) KillPlayer(playerId, playerTags[playerId]);
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
