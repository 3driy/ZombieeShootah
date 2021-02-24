using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Lazy;

//public class SceneControll : MonoBehaviour
public class SceneControll : Singletone<SceneControll> 
{

    [SerializeField] GameObject playerPrefab;

    //private GameObject[] enemies;
    [HideInInspector]
    public GameObject player;
    [HideInInspector] public bool paused;
    public bool inMenu;


    [HideInInspector]
    public bool fromStart;
    [HideInInspector]
    public int currentSceneIndex;
    [SerializeField] private Vector3 startplayerPos;
    [HideInInspector]
    public Vector3 savedPlayerPos;
    [HideInInspector]
    public int savedShot;
    [SerializeField] private int startAmmo;
    [HideInInspector]
    public int savedAmmo;
    [HideInInspector]
    public float savedHealth;
    [HideInInspector]
    public int savedKilledInGame;

    [HideInInspector]
    public int killedInGame;
    [HideInInspector]
    public int highScore;
    [HideInInspector]
    public bool died;

    GameObject[] capturePoint;
    //float[] capturePoint;

    [HideInInspector]
    public List<GameObject> medkitsInGame;

    [HideInInspector]
    public List<GameObject> ammoInGame;
    //void Awake()
    void Start()
    {
            LoadData();
        if (!inMenu)
        {
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            paused = false;
            GameObject playerToSpawn = playerPrefab;
            PlayerControll controllP = playerToSpawn.GetComponent<PlayerControll>();
            PlayerHealthAmmo controllHA = playerToSpawn.GetComponent<PlayerHealthAmmo>();
            died = false;
            if (fromStart)
            {
                killedInGame = 0;
                controllP.shot = 0;
                controllHA.playerHealth = controllHA.maxPlayerHealth;
                controllHA.ammo = startAmmo;
                Instantiate(playerToSpawn, startplayerPos + Vector3.up * 0.1f, Quaternion.identity);
            }
            else
            {
                killedInGame = savedKilledInGame;
                controllP.shot = savedShot;
                controllHA.playerHealth = savedHealth;
                controllHA.ammo = savedAmmo;
                Instantiate(playerToSpawn, savedPlayerPos + Vector3.up * 0.1f, Quaternion.identity);
            }
            UnPause();
            player = GameObject.FindGameObjectWithTag("Player");
            capturePoint = GameObject.FindGameObjectsWithTag("Capturepoint");
           
        }
        else {
            savedAmmo = startAmmo;
            savedHealth = 100f;
            savedShot = 0;
            savedPlayerPos = Vector3.zero;
        }
    }

    public void Pause()
    {
        paused = true;
        Time.timeScale = 0;
    }
    public void UnPause()
    {
        paused = false;
        Time.timeScale = 1;
    }

    private void LoadData() {
        PlayerSaveData data = SaveSystem.LoadPLayer();
        if (data != null)
        {
            died = data.died;
            savedKilledInGame = data.savedScore;
            fromStart = data.fromStart;
            savedAmmo = data.savedAmmo;
            savedHealth = data.savedHealth;
            savedPlayerPos = new Vector3(data.savedPosition[0], data.savedPosition[1], data.savedPosition[2]);
            savedShot = data.savedShot;
            highScore = data.highScore;
        }
        else { fromStart = true; }
        if (inMenu) {
            highScore = data.highScore;
        }
    }

    public void GoToMainMenu() {
        Time.timeScale = 1f;
        SaveSystem.SavePlayer(this);
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        if (killedInGame >= highScore)
        {
            highScore = killedInGame;
        }
        savedKilledInGame = killedInGame;
        if (!inMenu)
        {
            CheckCapturePoints();
        }
    }

    private void CheckCapturePoints() {
        float[] levels = new float[capturePoint.Length];
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i] = capturePoint[i].GetComponent<CapturePoint>().spotCapture;
        }
        float average = 0;
        for (int i = 0; i < levels.Length; i++)
        {
            average += levels[i];
        }
        average = average / capturePoint.Length;
        //Debug.Log(average);
        if (average < -99f) {
            player.GetComponent<PlayerHealthAmmo>().Lose();
        }

    }
}
