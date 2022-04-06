using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public GameObject player;
    
 
    private float spawnLimitXLeft;
    public Transform limit;
    private int levelStage;

    private float startDelay;
    private float spawnInterval;

    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().name=="Level1"){
            levelStage=1;
            spawnInterval=15.0f;
            startDelay=3.0f;
        }else if(SceneManager.GetActiveScene().name=="Level2"){
            levelStage=2;
            spawnInterval=7.5f;
            startDelay=2.5f;
        }else if(SceneManager.GetActiveScene().name=="Level3"){
            levelStage=3;
            spawnInterval=3.25f;
            startDelay=1.5f;
        }
        InvokeRepeating("Spawn", startDelay, spawnInterval);
    }
    void Update(){
        if (player.transform.position.x >= 94.0f && levelStage == 1){
            SceneManager.LoadScene("Level2");
        }else if(player.transform.position.x >= 94.0f && levelStage == 2){
            SceneManager.LoadScene("Level3");
        }
    }

    // Spawn random ball at random x position at top of play area
    void Spawn ()
    {
    
        // instantiate ball at random spawn location
        if(gameObject.transform.position.x <=108.0f){
             Instantiate(EnemyPrefab, new Vector3(gameObject.transform.position.x, -2.64f, -7.9f), EnemyPrefab.transform.rotation);
        }
       
        
    }

}
