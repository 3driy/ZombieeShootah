using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesLookAt : MonoBehaviour
{
    [SerializeField] Material animMat;
    [SerializeField]float xRange = 0.01f;
    [SerializeField]float yRange = 0.01f;
    float timer;
    float fulltimer;
    [SerializeField]float minTimer = 0.7f;
    [SerializeField]float maxTimer = 1.7f;
    Color startEyesPos;
    Color newEyesPos;

    // Start is called before the first frame update
    void Start()
    {
        ChangeSight();
            //StartCoroutine (EyesLooking());
    }

    // Update is called once per frame
    void Update()
    {
       
        //if (timer <= 0) { ChangeSight();}


    }

   void ChangeSight() {

        float xfactor = Random.Range(-xRange, xRange);
        float yfactor = Random.Range(-yRange, yRange);

        //Vector2 randomEyesPos = new Vector2(xf  );
        startEyesPos = animMat.GetColor("_EyesOffset");
        newEyesPos = new Vector4(xfactor, yfactor, 0, 0);
        fulltimer = Random.Range(minTimer,maxTimer);
        timer = fulltimer;
        //return newEyesPos;
            StartCoroutine (EyesLooking());
    
    }

    IEnumerator EyesLooking() {
           
        while (timer > 0) {
            timer -= Time.deltaTime;
            Color eyeOffset = Color.Lerp(startEyesPos, newEyesPos, timer / fulltimer);
            animMat.SetColor("_EyesOffset",eyeOffset);
            yield return null;
        }
        if (timer <= 0) {
            ChangeSight();
        }
    
    
    }
}
