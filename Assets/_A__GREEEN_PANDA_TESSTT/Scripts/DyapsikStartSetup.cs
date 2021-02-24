using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DyapsikStartSetup : MonoBehaviour
{

    MoneyManager manager;
    [SerializeField] GameObject[] characterSkin;
    [SerializeField] GameObject[] hats;
    [SerializeField] Color[] skinColor;
    [SerializeField] GameObject moneyIcon;
    [SerializeField] GameObject whipJoke;
    [SerializeField] Transform iconPoint;
    float workTimer = 3f;
    // Start is called before the first frame update
    void Start()
    {
        Animator animator = transform.GetChild(0).GetComponent<Animator>();
        animator.SetFloat("workCycleOffset", Random.Range(0,1));
        animator.SetFloat("workMul", Random.Range(0.9f,1.2f));
        workTimer = 3f + Random.Range(1f, 2f);
        manager = GameObject.FindGameObjectWithTag("SceneController").GetComponent<MoneyManager>();
        foreach (GameObject skin in characterSkin)
        {
            skin.SetActive(false);
        }
        int randomChar = Random.Range(0, characterSkin.Length);
        characterSkin[randomChar].SetActive(true);
        Material skinMat = characterSkin[randomChar].GetComponent<Renderer>().materials[1];
           skinMat.color = skinColor[Random.Range(0, skinColor.Length)];
        foreach (GameObject hat in hats)
        {
            hat.SetActive(false);
        }
        hats[Random.Range(0, hats.Length)].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        workTimer -= Time.deltaTime;
        if (workTimer <= 0) { GiveMoney(10, 0); }
        
    }

    void OnMouseDown()
    {
        if (manager.happiness >= 10)
        {
            GiveMoney(30, 10);
          Instantiate(whipJoke, iconPoint.position + Vector3.up * 0.2f, iconPoint.rotation);
        }
    }

    void GiveMoney(int quantity, int sadness) {

        manager.moneyOverall += quantity;
        manager.happiness -= sadness;
        Instantiate(moneyIcon, iconPoint.position, iconPoint.rotation);
        workTimer = 3f;

    }

}
