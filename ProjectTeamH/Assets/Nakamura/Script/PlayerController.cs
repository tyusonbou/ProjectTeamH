﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float jumpForce;
    public float jumpThereshold;
    //public float runForce;
    public float runSpeed;
    public float runThereshold;
    //public float AttackSpeed;
    public float EnemyAttack;

    public static float umbrellaHP;
    public float UmHpLimit;
    public float UmHpMaxLimit;
    public float invisibleInterval;

    public float FallSpped1;
    public float FallSpped2;
    public float FallSpped3;

    public int Hp;
    static int NeutralizerCount;
    public int BaketuPos;

    public GameObject Umbrella;
    public GameObject Neutralizer;
    public GameObject BaketuUse;
    public GameObject baketu;


    [SerializeField]
    bool isGround;
    [SerializeField]
    bool isJump = true;
    [SerializeField]
    bool isFall;
    [SerializeField]
    bool isDash;

    [SerializeField]
    AudioClip runSE;
    [SerializeField]
    AudioClip jumpSE;
    [SerializeField]
    AudioClip landingSE;
    [SerializeField]
    AudioClip openUmSE;
    [SerializeField]
    AudioClip closeUmSE;
    [SerializeField]
    AudioClip useNeuSE;
    [SerializeField]
    AudioClip getNeuSE;

    public static bool isCoolTime;
    public static bool isDead;
    public static bool isGetKey;
    
    static bool isUmbrella;


    //public bool isKnockBack;

    int LR = 0;
    
    float invisibleTimer = 0;

    string state;
    string preveState;
    static string LRState;
    float stateEffect = 1;

    private Rigidbody2D rb2d;

    private Animator animator;

    private new SpriteRenderer renderer;

    private AudioSource audioSource;

    private GameObject Key;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        umbrellaHP = 10;
        NeutralizerCount = 0;

        Umbrella.SetActive(false);
        LRState = "RIGHT";

        isGetKey = false;
        isDead = false;
        //isGoal = false;

        Key = GameObject.Find("Key");

        if (Key == null)
        {
            isGetKey = true;
        }
        else isGetKey = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (Mathf.Approximately(Time.timeScale, 0f)) { return; }
        //if (baketu.activeInHierarchy == true) { return; }

        GetInputKey(); //キー入力
        ChangeState(); //状態変化
        UseNeutralizer(); //中和剤使用
        LRBaketu();
        ChangeAnimation(); //アニメーション

        Move(); //移動

        //KnockBack(); //ノックバック

        //左右移動
        transform.position += new Vector3(runSpeed * Time.deltaTime * LR * stateEffect, 0, 0);

        UseUmbrella();//傘をさす

        
    }

    private void FixedUpdate()
    {
        
    }

    void GetInputKey()
    {
        if (isDead) { return; }

        LR = 0;
        if (!isDash)
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                LR = 1;
                LRState = "RIGHT";
            }

            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                LR = -1;
                LRState = "LEFT";
            }
        }
        
    }

    void ChangeState()
    {
        if (isDead) { return; }

        //落下、上昇してなければ接地
        if (Mathf.Abs(rb2d.velocity.y) > jumpThereshold)
        {
            isGround = false;
        }
        //接地かつ左右移動
        if ((Mathf.Abs(rb2d.velocity.x) > runThereshold) && (isGround)) 
        {
            isDash = true;
        }
        else isDash = false;


        if (isGround == true)
        {
            if (LR != 0)
            {
                state = "RUN";
                //audioSource.Play();
            }
            else
            {
                state = "IDLE";
                //audioSource.Stop();
            }
        }
        else
        {
            if (rb2d.velocity.y > 0)
            {
                state = "JUMP";
                isFall = false;
            }
            else if (rb2d.velocity.y < 0)
            {
                state = "FALL";
                isFall = true;
            }
        }

        if (baketu.activeInHierarchy == true)
        {
            state = "BAKETSU";
        }

        //if (isUmbrella)
        //{
        //    state = "UMBRELLA";
        //}
        //if (isKnockBack)
        //{
        //    state = "KNOCKBACK";
        //}
    }

    void Move()
    {
        if (isDead) { return; }

        if (isGround)
        {
            //ジャンプ
            if (Input.GetButtonDown("A"))
            {
                rb2d.velocity = Vector2.zero;
                rb2d.AddForce(Vector2.up * jumpForce);
                isGround = false;
                audioSource.PlayOneShot(jumpSE);
            }
            
        }
        

        if (!isFall)
        {
            //ジャンプ強さ
            if (Input.GetButtonUp("A"))
            {
                rb2d.velocity = Vector2.zero;
            }
        }
        //傘時の落下速度
        if (isFall && isUmbrella)
        {
            if (umbrellaHP >= 7) { rb2d.velocity = rb2d.velocity = new Vector2(0, -FallSpped1); }
            if (umbrellaHP >= 3 && umbrellaHP < 7) { rb2d.velocity = new Vector2(0, -FallSpped2); }
            if (umbrellaHP < 3) { rb2d.velocity = new Vector2(0, -FallSpped3); }
        }
        
       
    }

    //傘をさす
    void UseUmbrella()
    {
        if (Input.GetButtonDown("RB") && !isCoolTime)
        {
            audioSource.PlayOneShot(openUmSE);
        }
        if (Input.GetButtonUp("RB"))
        {
            audioSource.PlayOneShot(closeUmSE);
        }

        if (Input.GetButton("RB")  && !isCoolTime) 
        {
            Umbrella.SetActive(true);
            isUmbrella = true;
            
        }
        else if (!Input.GetButton("RB"))
        {
            Umbrella.SetActive(false);
            isUmbrella = false;
            
        }

        //傘の耐久時間
        if (umbrellaHP <= UmHpLimit)
        {
            isUmbrella = false;
            isCoolTime = true;
        }
        

        if (!isUmbrella)
        {
            Umbrella.SetActive(false);
            umbrellaHP += Time.deltaTime;
            if (umbrellaHP > UmHpMaxLimit)
            {
                umbrellaHP = UmHpMaxLimit;
                isCoolTime = false;
            }
        }
    }

    //void KnockBack()
    //{
    //    if (isKnockBack)
    //    {
    //        invisibleTimer += Time.deltaTime;
    //        if (invisibleTimer > invisibleInterval)
    //        {
    //            invisibleTimer = 0;
    //            rb2d.AddForce(Vector2.zero);
    //            isKnockBack = false;
    //        }
    //    }
    //}

    //中和剤使用
    void UseNeutralizer()
    {
        if (isDead) { return; }

        if ((Input.GetButtonDown("X")) && (NeutralizerCount>=1))
        {
            animator.SetTrigger("useNeutralizer");
            audioSource.PlayOneShot(useNeuSE);

            if(LRState=="RIGHT")
            {
                Instantiate(Neutralizer, transform.position + new Vector3(1, 0.5f, 0), Quaternion.identity);
            }
            if (LRState == "LEFT")
            {
                Instantiate(Neutralizer, transform.position + new Vector3(-1, 0.5f, 0), Quaternion.identity);
            }
            NeutralizerCount -= 1;
        }
    }

    //バケツの向き
    void LRBaketu()
    {
        if(LRState == "RIGHT")
        {
            BaketuUse.transform.position = new Vector3(transform.position.x + BaketuPos, transform.position.y - 0.25f, transform.position.z);
        }
        if(LRState == "LEFT")
        {
            BaketuUse.transform.position = new Vector3(transform.position.x - BaketuPos, transform.position.y - 0.25f, transform.position.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if ((col.gameObject.tag == "Neutralizer"))
        {
            NeutralizerCount += 1;
            audioSource.PlayOneShot(getNeuSE);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            if (!isGround) { isGround = true; }
            if (!isJump) { isJump = true; }
            if (isFall) { isFall = false; }

            audioSource.PlayOneShot(landingSE);
        }
       
        

        //if ((col.gameObject.tag == "Enemy") && (!isKnockBack))
        //{
        //    isKnockBack = true;
        //    Hp -= 1;

        //    Vector3 knockBackDirection = (col.gameObject.transform.position - transform.position).normalized;

        //    knockBackDirection.x *= -1;
        //    knockBackDirection.y = 1;
        //    knockBackDirection.z += -1;

        //    rb2d.AddForce(Vector2.zero);
        //    rb2d.AddForce(knockBackDirection* EnemyAttack);
        //}

       

    }
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            if (!isGround) { isGround = true; }
            if (!isJump) { isJump = true; }
            if (isFall) { isFall = false; }
        }
    }

   

    private void ChangeAnimation()
    {
        
        if (LRState == "RIGHT")
        {
            transform.localRotation = new Quaternion(0, 0, 0, 0);
        }
        else if (LRState == "LEFT") 
        {
            transform.localRotation = new Quaternion(0, 180, 0, 0);
        }

        if (preveState != state)
        {
            switch (state)
            {
                case "RUN":
                    animator.SetBool("isRun", true);
                    animator.SetBool("isJump", false);
                    animator.SetBool("isIdle", false);
                    animator.SetBool("isBaketsu", false);
                    stateEffect = 1f;
                    break;
                case "JUMP":
                    animator.SetBool("isRun", false);
                    animator.SetBool("isJump", true);
                    animator.SetBool("isIdle", false);
                    animator.SetBool("isBaketsu", false);
                    stateEffect = 1f;
                    break;
                case "BAKETSU":
                    animator.SetBool("isRun", false);
                    animator.SetBool("isJump", false);
                    animator.SetBool("isIdle", false);
                    animator.SetBool("isBaketsu", true);
                    stateEffect = 1f;
                    break;
                default:
                    animator.SetBool("isRun", false);
                    animator.SetBool("isJump", false);
                    animator.SetBool("isIdle", true);
                    animator.SetBool("isBaketsu", false);
                    stateEffect = 1f;
                    break;
            }

            preveState = state;
        }

        if (isDead)
        {
            animator.SetTrigger("Dead");
        }

    }
    public static bool GetKey()
    {
        return isGetKey;
    }
    public static bool GetUmbrella()
    {
        return isUmbrella;
    }
    public static int GetNeutralizer()
    {
        return NeutralizerCount;
    }
    public static string GetLRState()
    {
        return LRState;
    }
    public static bool GetCoolTime()
    {
        return isCoolTime;
    }
    public static bool GetDead()
    {
        return isDead;
    }
}
