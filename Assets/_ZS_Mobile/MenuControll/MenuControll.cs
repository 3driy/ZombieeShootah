using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuControll : MonoBehaviour
{

    [SerializeField] Button ContinueButon;
    [SerializeField] GameObject loadingScrene;
    [SerializeField] TextMeshProUGUI highScore;
    
    SceneControll controller;
    public void Qiut()
    {
        Application.Quit();
    }

    public void StartGame() {
        SceneControll sceneController = GameObject.FindGameObjectWithTag("SceneController").GetComponent<SceneControll>();
        SaveSystem.FromStart(sceneController);
        //StartCoroutine(LoadAsync(1));
        StartCoroutine(LoadAsync(Random.Range(1,SceneManager.sceneCountInBuildSettings)));
    }
    public void ContinueGame() {

          PlayerSaveData data = SaveSystem.LoadPLayer();
          StartCoroutine(LoadAsync(data.currentSceneIndex));
    }

    IEnumerator LoadAsync(int sceneIndex) {
        loadingScrene.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!operation.isDone) {
            yield return null;
        }
    
    }

    private void Start()
    {

        controller = GameObject.FindGameObjectWithTag("SceneController").GetComponent<SceneControll>();
        highScore.text += controller.highScore;
        if (controller.died) {
            ContinueButon.interactable = false;
        }
    }
   
}
