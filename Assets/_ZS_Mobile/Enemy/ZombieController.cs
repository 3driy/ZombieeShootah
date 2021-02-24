using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    NavMeshAgent agent;
    GameObject player;
    GameObject zombie;
    Animator animator;
    [HideInInspector] public GameObject captureSpot;
    [SerializeField] ZombieMeleeAttack meleeAttackHand;
    [SerializeField] float reactionDistance;
    private Material mat;
    Color startCol;
    //[SerializeField] private GameObject explosion;
    [SerializeField] private GameObject ammoLoot;
    [SerializeField] private GameObject medkitLoot;
    [SerializeField] private Material dissolveMat;
    //[SerializeField] private SpriteRenderer shadow;
    [SerializeField] private float health = 100f;
    private float startHealth;
    float distance;
    [SerializeField] float attackDistance;
    [HideInInspector] public bool attacking;
    PlayerHealthAmmo playerScript;
    bool dead;
    SceneControll localSceneController;

    AudioSource zombieSound;
    [SerializeField] AudioClip[] zombieRoarSound;
    float soundTimer;


    //void Awake()
    void Start()
    {
        soundTimer = Random.Range(1f, 8f);
        zombieSound = GetComponent<AudioSource>();
        dead = false;
        agent = GetComponent<NavMeshAgent>();
        zombie = transform.GetChild(0).transform.GetChild(1).gameObject;
        mat = zombie.GetComponent<Renderer>().material;
        animator = transform.GetChild(0).GetComponent<Animator>();
        startCol = mat.color;
        startHealth = health;
        animator.SetFloat("cycleOffset", Random.Range(0f, 1f));
        localSceneController = GameObject.FindGameObjectWithTag("SceneController").GetComponent<SceneControll>();
        //player = GameObject.FindGameObjectWithTag("Player");
        player = localSceneController.player;
        playerScript = player.GetComponent<PlayerHealthAmmo>();
        captureSpot.GetComponent<CapturePoint>().zombiesInGame.Add(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
       
        if (health <= 0 & !dead)
        {
            localSceneController.killedInGame++;
            Death();
        }
        CheckDistance();
        soundTimer -= Time.deltaTime;
        if (soundTimer <= 0) {
            PlaySound(zombieRoarSound[Random.Range(0,zombieRoarSound.Length)]);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Color col = Color.Lerp(Color.red, Color.white, health / startHealth);
        mat.SetColor("_Color", startCol * col);
    }
    public void TakeMeleeDamage(float damage)
    {
        health -= damage;
        Color col = Color.Lerp(Color.red, Color.white, health / startHealth);
        mat.SetColor("_Color", startCol * col);
    }
    void Death()
    {

        CapturePoint point = captureSpot.GetComponent<CapturePoint>();
        //point.zombiesInGame.Remove(this.gameObject);
        captureSpot.GetComponent<CapturePoint>().zombiesInGame.Remove(this.gameObject);
        dead = true;
        zombie.GetComponent<Renderer>().material = dissolveMat;

        StartCoroutine(DeathTimer());
        ChanceToSpawn();
    }

    void CheckDistance() {
        //if (agent.enabled)
           distance = (transform.position - player.transform.position).magnitude;
        if (distance > reactionDistance)
        {
            //Debug.Log("Oh! Too far!");
            agent.SetDestination(captureSpot.transform.position);
        }
        else if (distance <= attackDistance & !attacking)
        {
            attacking = true;
            StartCoroutine("ZombieAttack");
        }
        else
        {

            //Debug.Log("Player is close");
            if (agent.enabled)
            {
                agent.SetDestination(player.transform.position);
            }
        }
        //}
        //Debug.Log(agent.destination);
    }

    IEnumerator ZombieAttack() {
        agent.enabled = false;
        float timer = 1.3f;
        while (timer > 0f) {

            animator.SetBool("attack", true);
            timer -= Time.deltaTime;
            yield return null;
        }
        if (timer < 0.05f || animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) {
            meleeAttackHand.damaged = false;
            attacking = false;
            animator.SetBool("attack", false);
            agent.enabled = true;
            yield break;
        }
    }

    IEnumerator DeathTimer() {

        animator.SetBool("dead", true);
        //Color shadowColor = shadow.color;
        float timer = 1.7f;
        while (timer > 0f) {

            dissolveMat.color = Color.Lerp(Color.white, Color.black,  timer / 1.7f);
            //shadow.color = shadowColor * (timer / 1.7f);
            timer -= Time.deltaTime;
            yield return null;
        }
        if (timer <= 0f) { 
        
        Destroy(this.gameObject);
            yield break;
        }
    
    
    }

    private void ChanceToSpawn() {
        if (playerScript.playerAmmoZeroOne < 0.5f && Random.Range(0, 1) < 1 - playerScript.playerHealthZeroOne)
        {
            if (localSceneController.medkitsInGame.Count > 0)
            {
                float dist = 1.2f;
                for (int i = 0; i < localSceneController.medkitsInGame.Count - 1; i++)
                {
                    float distance = (localSceneController.medkitsInGame[i].transform.position - transform.position).magnitude;
                    dist = Mathf.Min(dist, distance);
                }


                if (dist < 1)
                {
                    SpawnLoot(medkitLoot);
                }
            }
            else
            {

                SpawnLoot(medkitLoot);
            }
        }
        if (playerScript.playerAmmoZeroOne < 0.5f && Random.Range(0, 1) < 1 - playerScript.playerAmmoZeroOne)
        {
            if (localSceneController.ammoInGame.Count > 0)
            {
                float dist = 1.2f;
                for (int i = 0; i < localSceneController.ammoInGame.Count - 1; i++)
                {
                    float distance = (localSceneController.ammoInGame[i].transform.position - transform.position).magnitude;
                    dist = Mathf.Min(dist, distance);
                }


                if (dist < 1)
                {
                    SpawnLoot(ammoLoot);
                }
            }
            else
            {

                SpawnLoot(ammoLoot);
            }
        }

    }

    private void SpawnLoot(GameObject lootObject) {

        Vector3 spawnPos = transform.position;
        Vector3 randomPos = new Vector3(Random.Range(-1, 1), 2f, Random.Range(-1, 1));
        //Vector3 randomPos = new Vector3(Random.Range(-1, 1),0f, Random.Range(-1, 1));

        RaycastHit hit;
        if (Physics.Raycast(spawnPos + randomPos, transform.TransformDirection(Vector3.down), out hit, 5f, LayerMask.GetMask("Environment")))
        {
            Instantiate(lootObject, hit.point, Quaternion.identity);
        }

        //Instantiate(lootObject, spawnPos + randomPos, Quaternion.identity);
    }

    private void PlaySound(AudioClip clip) {

        soundTimer = Random.Range(3, 8);
        zombieSound.clip = clip;
        zombieSound.Play();
    
    }
}
