using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackerPoint : MonoBehaviour
{
    [HideInInspector]
    public float captureSpotPoints;
    [SerializeField]  GameObject[] fillImageObj;
    Image[] fillImages;
    Image[] backImages;
    Color startBackColor;

    [HideInInspector]
    public  bool inFront;
    [HideInInspector]
    public bool onRight;

    // Start is called before the first frame update
    void Start()
    {
        fillImages = new Image[fillImageObj.Length];
        backImages = new Image[fillImageObj.Length];
        for (int i = 0; i < fillImageObj.Length; i++)
        {
            fillImages[i] = fillImageObj[i].GetComponent<Image>();
            backImages[i] = fillImageObj[i].transform.GetChild(0).GetComponent<Image>();
            startBackColor = backImages[i].color;
        }

    }

    // Update is called once per frame
    void Update()
    {
        ColorFill();
        CircleArrow();

    }

    void ColorFill()
    {
        float proportion = Mathf.Abs(captureSpotPoints) / 100f;
        if (captureSpotPoints >= 0)
        {
            fillImages[0].fillClockwise = true;
            for (int i = 0; i < fillImages.Length; i++)
            {
                backImages[i].color = Color.Lerp(Color.gray, Color.green, proportion) * startBackColor;
                fillImages[i].color = Color.Lerp(Color.gray, Color.green, proportion);
            }

        }
        else
        {

            fillImages[0].fillClockwise = false;
            for (int i = 0; i < fillImages.Length; i++)
            {
                backImages[i].color = Color.Lerp(Color.gray, Color.red, proportion) * startBackColor;
                fillImages[i].color = Color.Lerp(Color.gray, Color.red, proportion);
            }

        }

        foreach (Image fill in fillImages)
        {
            fill.fillAmount = proportion;
        }
    }


    void CircleArrow() {

        if (inFront)
        {
            fillImageObj[0].SetActive(true);
            fillImageObj[1].SetActive(false);
            fillImageObj[2].SetActive(false);
        }
        else {
            if (onRight)
            {
                fillImageObj[2].SetActive(true);
                fillImageObj[1].SetActive(false);
                fillImageObj[0].SetActive(false);

            }
            else {
                fillImageObj[1].SetActive(true);
                fillImageObj[2].SetActive(false);
                fillImageObj[0].SetActive(false);


            }


        }

    }

}
