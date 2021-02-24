using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCoreControll : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject canvasText;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject inGameUI;
    Text text;
    [SerializeField] Transform[] spawnPoints;
    GameObject[] enemies;
    bool paused;
    // Start is called before the first frame update
    void Start()
    {
        paused = false;
        text = canvasText.GetComponent<Text>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {

            Instantiate(enemy, spawnPoints[i].position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        text.text = "Enemies left: " + enemies.Length;
        if (Input.GetButtonDown("Escape")) {
            paused = !paused;
        }
        if (paused) { Pause(); }
        if (!paused) { UnPause(); }

    }

    void MoreEnemies() {

        for (int i = 3 ; i > 3; i--)
        {
            Instantiate(enemy, spawnPoints[i].position, Quaternion.identity);
        }

    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        inGameUI.SetActive(false);
        Time.timeScale = 0;
    }
    void UnPause()
    {
        pauseMenu.SetActive(false);
        inGameUI.SetActive(true);
        Time.timeScale = 1;
    }
}

