using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class CapturePoint : MonoBehaviour
{
    [HideInInspector] public float spotCapture;
    [SerializeField] private float playerCaptureSpeed;
    [SerializeField] private float zombieCaptureSpeed;
    [SerializeField] GameObject[] zombiePrefab;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] int zombieSpawnQuantity;

    [HideInInspector] public List<GameObject> zombiesInGame;

    [SerializeField] TextMeshPro[] textMesh;
    [SerializeField] float rotateSpeed;
    Transform rotator;
    Color startCol;

    void Start()
    {
        spotCapture = 0f;
        SpawnEnemies();
        rotator = transform.GetChild(0);
        //textMesh = new TextMeshPro[1];
        //for (int i = 0; i < textMesh.Length; i++)
        //{
        //    textMesh[i] = rotator.GetChild(i).gameObject.GetComponent<TextMeshPro>();
        //}
        startCol = textMesh[0].color;
    }


    void Update()
    {
        rotator.Rotate(Vector3.up * rotateSpeed);
        spotCapture = Mathf.Clamp(spotCapture, -100, 100);

        if (zombiesInGame.Count < 3)
        {
            SpawnEnemies();
        }
        ColorText();

    }

    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "Player")
            {
            spotCapture += Time.deltaTime * playerCaptureSpeed;

            }

        if (other.gameObject.tag == "EnemyMelee")
        {
            spotCapture -= Time.deltaTime * zombieCaptureSpeed;
        }
    }


    private void SpawnEnemies()
    {
        List<int> ints = new List<int>();
        List<int> randonInts = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++) {
            ints.Add(i);
        }
        for (int i = 0; i < zombieSpawnQuantity; i++)
        {
            int index = Random.Range(0, ints.Count - 1);    //  Pick random element from the list
            int r = ints[index];    //  i = the number that was randomly picked
            ints.RemoveAt(index);
            randonInts.Add(r);
        }


        Transform[] spawnPointsused = new Transform[zombieSpawnQuantity];

        for (int i = 0; i < zombieSpawnQuantity; i++)
        {
            spawnPointsused[i] = spawnPoints[randonInts[i]];
        }

        foreach (Transform spawnPoint in spawnPointsused)
        {
            GameObject zombie = zombiePrefab[Random.Range(0, zombiePrefab.Length)];
            zombie.GetComponent<ZombieController>().captureSpot = this.gameObject;
            //zombiesInGame.Add(zombie);
            //Instantiate(zombie, spawnPoint.position, Quaternion.identity, this.transform);
            Instantiate(zombie, spawnPoint.position, Quaternion.identity);
        }
    }

    private void ColorText() {

        int texte = (int)spotCapture ;
        if (texte >= 0)
        {
            foreach (TextMeshPro text in textMesh)
            {
                text.text = "" + texte;
                text.color = Color.green * startCol;
            }
        }
        else {
            foreach (TextMeshPro text in textMesh)
            {
                text.text ="" + -texte;
                text.color = Color.red * startCol;
            }
        }
    }

}
   

