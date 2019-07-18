using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Player : MonoBehaviour {

    
    public Animator animations;

    private Rigidbody rb;

    public float jumpLength;
    public float speed = 25f;
    public float laneSpeed;
    public float jumpHeight;
    public float slideLength;
    public int maxLife = 3;
    public float minSpeed = 25f;
    public float maxSpeed = 30f;
    public float invincibleTime;
    public int coins;
    public int totalcoins=0;
    public int gems;
    public int totalgems=0;
    public int currentLife;


    public AudioSource playerHit;
    public AudioSource playerJump;
    public AudioSource musictuto;
    //public AudioSource winEnding;

    private int currentLane = 0;
    private Vector3 verticalTargetPosition;
    private BoxCollider boxCollider;
    private Vector3 boxColliderSize;
    private bool jumping = false;
    private float jumpStart;
    private bool sliding = false;
    private bool IsSwipping = false;

    private bool invincible = false;
    private float slideStart;
    static int blinkingValue;
    public GameObject model;
    private UIManager uiManager;

    public GameObject endingPanel;

    public float timeCounterMovement;
    public float timeCounterJump;
    public Light[] llumverda;
    public AudioSource bipsr;
    public bool isGrounded = false;
    public Vector3 distToGround;
    public LayerMask groundLayer;
    public GameObject planemovement;


    public GameObject raycastCapsule;

    // Use this for initialization
    void Start () {
        animations = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        boxColliderSize = boxCollider.size;
        currentLife = maxLife;
        //speed = minSpeed;
        blinkingValue = Shader.PropertyToID("_BlinkingValue");
        uiManager = FindObjectOfType<UIManager>();
        //winEnding = GetComponent<AudioSource>();
        totalcoins = 0;
        totalgems = 0;
}
	
	// Update is called once per frame
	void Update () {
        GodMode();//
        Vector3 down = raycastCapsule.transform.TransformDirection(Vector3.down) * 1.0f;
        Debug.DrawRay(raycastCapsule.transform.position, down, Color.red);
        Debug.Log(Physics.Raycast(raycastCapsule.transform.position, Vector3.down, 1.0f));

        PlayerPrefs.SetInt("coinscore", totalcoins);
        PlayerPrefs.SetInt("gemscore", gems);


        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            if(timeCounterMovement <= 0.015f) {
                animations.SetBool("isRunning", false);
                animations.SetBool("isJumping", false);
                animations.SetBool("isChangeLeft", true);
                animations.SetBool("isChangeRight", false);
                animations.SetBool("isSliding", false);
                timeCounterMovement += Time.deltaTime;
                
            } else {
                timeCounterMovement = 0;
                animations.SetBool("isRunning", true);
                animations.SetBool("isJumping", false);
                animations.SetBool("isChangeLeft", false);
                animations.SetBool("isChangeRight", false);
                animations.SetBool("isSliding", false);

                Debug.Log("2");
            }

            ChangeLane(-8);
            //animations.SetBool("isChangeLeft", false);
            //  animations.Play("ChangeLeft");

        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            if (timeCounterMovement <= 0.015f) {
                animations.SetBool("isRunning", false);
                animations.SetBool("isJumping", false);
                animations.SetBool("isChangeRight", true);
                animations.SetBool("isChangeLeft", false);
                animations.SetBool("isSliding", false);

                timeCounterMovement += Time.deltaTime;

            } else {
                timeCounterMovement = 0;
                animations.SetBool("isRunning", true);
                animations.SetBool("isJumping", false);
                animations.SetBool("isChangeRight", false);
                animations.SetBool("isChangeLeft", false);
                animations.SetBool("isSliding", false);

                Debug.Log("2");
            }

            ChangeLane(+8);
            //animations.SetBool("isChangeLeft", false);
            //  animations.Play("ChangeLeft");

        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (Physics.Raycast(raycastCapsule.transform.position, Vector3.down, 1.0f)) {
                Jump();
                if (jumping) {
                    if (timeCounterJump <= 0.3f) {

                        playerJump.Play();
                        animations.SetBool("isChangeLeft", false);
                        animations.SetBool("isJumping", true);
                        animations.SetBool("isRunning", false);
                        animations.SetBool("isChangeRight", false);
                        animations.SetBool("isSliding", false);

                        animations.Play("Jump");
                        timeCounterJump += Time.deltaTime;
                        Debug.Log("1");


                    } else {
                        timeCounterJump = 0;
                        animations.SetBool("isRunning", true);
                        animations.SetBool("isJumping", false);
                        animations.SetBool("isChangeLeft", false);
                        animations.SetBool("isChangeRight", false);
                        animations.SetBool("isSliding", false);

                        Debug.Log("2");

                    }
                }
            }
            
            


        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            animations.SetBool("isJumping",false);
            animations.SetBool("isRunning", false);
            animations.SetBool("isChangeRight", false);
            animations.SetBool("isChangeLeft", false);
            animations.SetBool("isSliding", true);

            Debug.Log("slideeeeeeee");
            Slide();
        } else {
            animations.SetBool("isJumping", false);
            animations.SetBool("isChangeLeft", false);
            animations.SetBool("isRunning", true);
            animations.SetBool("isChangeRight", false);
            animations.SetBool("isSliding", false);

            timeCounterMovement = 0;
            timeCounterJump = 0;
        }

        
          if (jumping)
        {
            //Debug.Log("JUUUUUMP:" + jumping); aqui es true
            // CREC QUE AQUI HAURIEM DE FER EL RAYCAST.

            float ratio = (transform.position.z - jumpStart) / jumpLength;
            //Debug.Log("z:" + transform.position.z);
            //Debug.Log("jumpstart:" + jumpStart);
            //Debug.Log("jumplenght:" + jumpLength);

            //Debug.Log("ratio:"+ratio);
            if(ratio >= 1f)
            {
                jumping = false;
                //anim.SetBool("Jumping", false);
            }
            else
            {
                verticalTargetPosition.y = Mathf.Sin(ratio * Mathf.PI) * jumpHeight;
            }
            
        }
        else
        {
            verticalTargetPosition.y = Mathf.MoveTowards(verticalTargetPosition.y, 0, 5* Time.deltaTime);
        }

        if (sliding)
        {

            float ratio = (transform.position.z -slideStart) /slideLength;
            if(ratio >= 1f)
            {
                sliding = false;
                //anim.SetBool("Sliding", false);
            }
        }
        Vector3 targetPosition = new Vector3(verticalTargetPosition.x, verticalTargetPosition.y, transform.position.z);

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, laneSpeed * Time.deltaTime);
        //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + speed * Time.deltaTime);
        
	}

    private void FixedUpdate()
    {
        rb.velocity = Vector3.forward * speed;
    }

    void ChangeLane(int direction)
    {
        int targetLane = currentLane + direction;
        if (targetLane < -8 || targetLane > 8) return;

        currentLane = targetLane;
        verticalTargetPosition = new Vector3((currentLane), 0 ,0);
    }
    void Jump()
    {

        
        if(!jumping)
        {

            //Debug.Log("JUUUUUMP:" + jumping); aqui es false.
            jumpStart = transform.position.z;
            //anim.SetFloat("JumpSpeed", speed / jumpLength);
            //anim.SetBool("Jumping", true);
            jumping = true;
        }
    }
    void Slide()
    {
        if(!jumping&&!sliding)
        {
            
            slideStart = transform.position.z;
           // anim.SetFloat("JumpSpeed", speed / slideLength);
            //anim.SetBool("Sliding", true);
            Vector3 newSize = boxCollider.size;
            newSize.y = newSize.y/2;
            boxCollider.size = newSize;
            sliding = true;

        }

        
    }




    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("hostess")) {

        }

        if (other.CompareTag("llum")) {
            bipsr.Play();
            llumverda[0].enabled = true;
            llumverda[1].enabled = false;
            llumverda[2].enabled = true;
            llumverda[3].enabled = false;
            Debug.Log("llum");
        }

        if (other.CompareTag("Coin"))
        {
            coins++;
            uiManager.UpdateCoins(coins);
            
        }

        if (other.CompareTag("Gem"))
        {
            speed = 0;
            animations.Play("Dance");

            gems++;
            uiManager.UpdateGems(gems);

            // tag == "EndLevel"
            endingPanel.SetActive(true);
            
            //HE PUESTO A UNO AQUI
            Time.timeScale = 1;
            musictuto.Stop();
            
        }

        if (other.CompareTag("Gem2"))
        {

            gems++;
            uiManager.UpdateGems(gems);

        }

        if (other.CompareTag("Obstacle"))
        {
            if (invincible)
                return;

            currentLife--;
            playerHit.Play();
            uiManager.UpdateLives(currentLife);
            //anim.SetTrigger("Hit");
            //speed = 0;
            if(currentLife <= 0)
            {
                //SceneManager.LoadScene(0);
                //game over
                endingPanel.SetActive(true);
                musictuto.Stop();
                
                // HE AÑADIDO SPEED = 0 PARA QUE PARE DE CORRER SI MUERE
                speed = 0;
                Time.timeScale = 0;
                animations.Play("Stop");

            }
            else
            {
                StartCoroutine(Blinking(invincibleTime));
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Dead")) {
            endingPanel.SetActive(true);
            speed = 0;
            animations.Play("Dance");
            Time.timeScale = 0;
            musictuto.Stop();
            
        }

        if (other.CompareTag("llum")) {
            llumverda[0].enabled = false;
            llumverda[1].enabled = true;
            llumverda[2].enabled = false;
            llumverda[3].enabled = true;
            Debug.Log("llum");
        }
        if (other.CompareTag("plane")) {
            Destroy(planemovement);
        }
    }
    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("plane")) {
            planemovement.transform.position = new Vector3(planemovement.transform.position.x, planemovement.transform.position.y, planemovement.transform.position.z - 1.2f);
            //Debug.Log("plane");
        }
    }
    IEnumerator Blinking (float time)
    {
        invincible = true;
        float timer = 0;
        float currentBlink = 3f;
        float lastBlink = 0;
        float blinkPeriod = 0.1f;
        bool enabled = false;
        yield return new WaitForSeconds(0.1f);
        speed = minSpeed;
        while(timer < time && invincible)
        {
            model.SetActive(enabled);
            //Shader.SetGlobalFloat(blinkingValue, currentBlink);
            yield return null;
            timer += Time.deltaTime;
            lastBlink += Time.deltaTime;
            if(blinkPeriod < lastBlink)
            {
                lastBlink = 0;
                currentBlink = 1f - currentBlink;
                enabled = !enabled;
            }
        }
        model.SetActive(true);
        //Shader.SetGlobalFloat(blinkingValue, 0);
        invincible = false;

    }
    void GodMode() {
        if (Input.GetKeyDown(KeyCode.H)) {
            currentLife = 999;
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            coins = 99;
        }
    }


    
}
