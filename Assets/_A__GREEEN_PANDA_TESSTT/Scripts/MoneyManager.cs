using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] Image happinessLevel;
    Color happinessStartColor;
    [HideInInspector] public int moneyOverall = 0;
    [HideInInspector] public int happiness = 0;
    float hapTimer;
    void Start()
    {
        moneyOverall = 0;
        happiness = 50;
        happinessStartColor = happinessLevel.color;
    }

    // Update is called once per frame
    void Update()
    {
        moneyText.text = "" + moneyOverall;
        happinessLevel.fillAmount = happiness / 100f;
        hapTimer += Time.deltaTime ;
        if (hapTimer > 1)
        {
            happiness += Mathf.RoundToInt(hapTimer);
            hapTimer = 0;
        }
        happiness = Mathf.Clamp(happiness, 0, 100);
        if (happiness < 10)
        {
            happinessLevel.color = Color.red;
        }
        else {
            happinessLevel.color = happinessStartColor;
        }
    }
}
