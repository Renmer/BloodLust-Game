using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.Text;

public class PlayerController : MonoBehaviour
{
    public Transform firePoint;//nos ayuda a establecer un punto de partida del projectil

    public int lifePoints;
    public Transform limit;
    public GameObject bulletPrefab; //La bala
    public Animator anim;
    public float battery=100;
    public AudioClip[] audioC;
    public float jumpForce = 10.0f;
    public Rigidbody2D rb;
    public bool derecha =true;
    public bool isOnGround=true;
    public int moveInput=1;
    public GameObject controlAudio;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI bateriaText;
    public GameObject pila;
    private int countLife = 0;
    private float lim=0;
    private float speed = 3.5f;
    private int score = 0;
    System.Random rnd = new System.Random();
    Transform CamTransform;

    // Start is called before the first frame update
    void Start()
    {
        int n = rnd.Next(-19, 108);
        Instantiate(pila, new Vector3(n, -2.64f, -7.9f), pila.transform.rotation);
        anim.SetBool("Idle", true);
        CamTransform = Camera.main.transform;
        limit = GameObject.FindGameObjectWithTag("Limit").GetComponent<Transform> ();
        controlAudio = GameObject.FindGameObjectWithTag("Fondo");
        lifePoints=5;
        GameObject tmpObj = Instantiate(pila);
        tmpObj.transform.position = Camera.main.ScreenToWorldPoint(new Vector2(0,0));
        bulletPrefab.GetComponent<Bullet>().isLampOn = false;
    }

    void awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        bateriaText.text = ": " + Mathf.Round(battery) + "%";
        lifeText.text = ": " + lifePoints;
        if(gameObject.transform.position.x > CamTransform.position.x && gameObject.transform.position.x < 86.80){
             CamTransform.position = new Vector3(gameObject.transform.position.x, CamTransform.position.y,  CamTransform.position.z);
        }

        OutOfBounds();
        anim.SetBool("Idle", true);
        if (Input.GetKeyDown(KeyCode.Space) && Time.time>lim && anim.GetBool("Caminar")==false)
        {   
            //anim.SetBool("Idle", false);
            anim.SetBool("Disparar", true);
            lim=Time.time+0.4f;
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            
        }

        if(Input.GetKey(KeyCode.S)){
            anim.SetBool("Idle", false);
            anim.SetBool("Agacharse", true);
        }

        if(Input.GetKeyUp(KeyCode.W) && anim.GetBool("Agacharse") && isOnGround==true){
            
            anim.SetBool("Agacharse", false);
        }

        if(Input.GetKeyUp(KeyCode.Space)){
            anim.SetBool("Idle", false);
            anim.SetBool("Disparar", false);
        }

        if(Input.GetKeyDown(KeyCode.A) && derecha == true){
             anim.SetBool("Idle", false);
            moveInput = 0;
            Flip(); 
            derecha = false;
            /*esto es por si el personaje se voltea
        si el personaje se voltea, el projectil tambien debe de hacerlo
        */
        }

        if(Input.GetKey(KeyCode.A) && anim.GetBool("Agacharse") == false && anim.GetBool("Disparar") == false){
             anim.SetBool("Idle", false);
            moveInput = 0;
            anim.SetBool("Caminar", true);
            transform.position += Vector3.left * speed * Time.deltaTime;
             
        }

        if(Input.GetKey(KeyCode.W) && anim.GetBool("Agacharse")==false && isOnGround==true){
             anim.SetBool("Idle", false);
            anim.SetBool("Saltar", true);
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            transform.position+=Vector3.left*speed*Time.deltaTime;
            isOnGround=false;
        }

        if(Input.GetKey(KeyCode.D) && anim.GetBool("Agacharse") == false && anim.GetBool("Disparar") == false){
             anim.SetBool("Idle", false);
            moveInput = 1;
            anim.SetBool("Caminar", true);
            transform.position += Vector3.right * speed * Time.deltaTime;
             
        }

        if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)){
             
            anim.SetBool("Caminar", false);
        }

        if(Input.GetKeyDown(KeyCode.D) && derecha == false){
             anim.SetBool("Idle", false);
            moveInput = 0;
            Flip();
            derecha = true;
             /*esto es por si el personaje se voltea
        si el personaje se voltea, el projectil tambien debe de hacerlo
        */
        }

        if(Input.GetKeyDown(KeyCode.P) && anim.GetBool("Lamp")==false){
             anim.SetBool("Idle", false);
            StartCoroutine("Lamp");
        }

        if(anim.GetBool("Lamp")==true && Input.GetKeyDown(KeyCode.U)){
             anim.SetBool("Idle", false);
            anim.SetBool("Lamp", false);
            bulletPrefab.GetComponent<Bullet>().isLampOn = false;
            StopCoroutine("Lamp");
        }

        if(anim.GetBool("Lamp")==true){
             anim.SetBool("Idle", false);
            battery=battery-Time.deltaTime;
            Debug.Log("Batería de la lampara: "+ Mathf.Round(battery));
            bateriaText.text = ": " + Mathf.Round(battery) + "%";
        }

        if(lifePoints == 0){
             anim.SetBool("Idle", false);
             anim.SetBool("Caminar", false);
             anim.SetBool("Dash", false);
             anim.SetBool("Saltar", false);
             anim.SetBool("Lamp", false);
             anim.SetBool("Agacharse", false);
             anim.SetBool("Disparar", false);
             anim.SetBool("Hurt", false);
             anim.SetBool("Morirse", true);
             StartCoroutine("Terminar");
        }
    }

    IEnumerator Terminar(){
        yield return new WaitForSeconds(1.0f);
        Time.timeScale = 0;
        controlAudio.GetComponent<AudioSource>().Stop();
        int posicion = 5 - SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + posicion);
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        countLife++;
        if(countLife == 8)
        {
            lifePoints++;
            countLife = 0;
        }   
        scoreText.text = ": "+ score;
    }

    private void Flip(){
        transform.Rotate(0f, 180f, 0f);
    }

    IEnumerator Lamp(){
        bulletPrefab.GetComponent<Bullet>().isLampOn = true;
        anim.SetBool("Lamp", true);
        while (anim.GetBool("Lamp")== true)
        {
            yield return new WaitForSeconds(1);
            battery -= 2;
        }
        bulletPrefab.GetComponent<Bullet>().isLampOn = false;
        anim.SetBool("Lamp", false);
    }

    private void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Floor")){
            anim.SetBool("Saltar", false);
            isOnGround=true;                  
        }
    }

    void OutOfBounds(){
        if(gameObject.transform.position.x>=108.0f){
            transform.position = new Vector3(108.0f, transform.position.y, transform.position.z);
        }else if( gameObject.transform.position.x<=limit.position.x){
            transform.position = new Vector3(limit.position.x, transform.position.y, transform.position.z);
        }
    }
}