using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GravityControl))]
[RequireComponent(typeof(ClimbMaster))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController2p5D : HisaoMono
{
    public bool useClimp;
    
    
    public Rigidbody rb;
    
    public InteractiveCaster ic;
    [SerializeField] private Vector2 dir;
    [SerializeField] private Animator anim;
    [SerializeField] private float _VelocityX;
    [SerializeField] private float _Speed;
    [SerializeField] private float _JumpVelocity;
    [SerializeField] private float _DashVelocity;
    [SerializeField] private float _DashTime;
    public static PlayerController2p5D instance;
    public GroundCaster GCaster;
    public ClimbMaster cm;
    private bool enableInput;
    private bool canDoubleJump;
    private bool canDash;
    private bool impotantAnim;
    [SerializeField]private bool canWallJump;
    private float tempG;
    [SerializeField] private Vector3 TempDashVelocity;
    public GameObject BoomObj;
    private Vector3 normalCC;
    private Vector3 splashVelocity;
    public ShadowFX sfx;
 

    public KeyCode UpKey;
    public KeyCode LeftKey;
    public KeyCode RightKey;
    public KeyCode DownKey;
    public KeyCode JumpKey;
    public KeyCode InteractiveKey;
    public KeyCode BoomKey;

    public MyButton UpBtn = new MyButton();
    public MyButton LeftBtn= new MyButton();
    public MyButton RightBtn= new MyButton();
    public MyButton DownBtn= new MyButton();
    public MyButton JumpBtn= new MyButton();
    public MyButton InteractiveBtn= new MyButton();
    public MyButton BoomBtn = new MyButton();
    private void Awake()
    {
        tempG = GetComponent<GravityControl>().G;
        instance = this;
        rb = GetComponent<Rigidbody>();
        cm = GetComponent<ClimbMaster>();
 
        EnableInput();
    }
    private void FixedUpdate()
    {
        if (enableInput)
        {
            TickKey();
            
        }else{return;}
        
        var tempDir = new Vector2
            (
            (LeftBtn.IsPressing ? 0 : 1) - (RightBtn.IsPressing ? 0 : 1),
            (UpBtn.IsPressing ? 0 : 1) - (DownBtn.IsPressing ? 0 : 1)
            );
        dir = Vector2.Lerp(dir,tempDir,0.3f);
        _VelocityX = dir.x  * _Speed ;
        rb.velocity = new Vector3(_VelocityX, 
            rb.velocity.y,
            0) +TempDashVelocity ;
       // anim.SetFloat("Speed", Mathf.Abs(dir.x));
       // anim.SetBool("OnFall",!GCaster.GetOnGround());
       
       var targetDir = transform.forward;
       
       if (dir.x>0)
       {
           targetDir = Vector3.forward+ new Vector3(0.001f,0,0);
       }

       if (dir.x<0)
       {
           targetDir = Vector3.back + new Vector3(0.001f,0,0);
           ;
       }
       transform.forward = Vector3.Lerp(transform.forward,targetDir,0.5f);


       if (useClimp)
       {
                   TryClimb();
       }

        if (JumpBtn.OnPressed || UpBtn.OnPressed)
        {
            

                if (  canWallJump && !GCaster.GetOnGround())
                {
                    WallJump();
                }
  
            
            TryJump();
        }
        if (InteractiveBtn.OnPressed)
        {
            ic.tryInteractive();
        }

        transform.position += splashVelocity;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
    public void Jump(float jumpVelocity)
    {
        // anim.SetTrigger("Jump");
        impotantAnim = true;
        // anim.SetBool("impotantAnim",impotantAnim);
        GCaster.OutGround();
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.up * jumpVelocity);
    }

    public void WallJump()
    {
        canWallJump = false;
        // anim.SetTrigger("Jump");
        impotantAnim = true;
        // anim.SetBool("impotantAnim",impotantAnim);
        GCaster.OutGround();
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.up * _JumpVelocity);
    }
    public void Dash(float dashTime,float dashVelocity)
    {
        sfx.VFXGo();
        // anim.SetTrigger("Jump");
        impotantAnim = true;
        // anim.SetBool("impotantAnim",impotantAnim);
        rb.velocity = Vector3.zero;
        StartCoroutine(onDash(dashTime, dashVelocity));
    }

    public void Dead()
    {
        DisableInput();
        rb.velocity = Vector3.zero;
        splashVelocity = Vector3.zero;
        _DashVelocity = 0f;
        // anim.CrossFade("Die",0.2f);
  
        StartCoroutine(DelayDead());
    }

    private IEnumerator DelayDead()
    {
        var t = 0f;
        var cam = Camera.main;
        var camZ = cam.transform.position.z;
      
        while (t<5)
        {
            t += Time.deltaTime;
           
            cam.transform.position = Vector3.Lerp(cam.transform.position,
                new Vector3(transform.position.x, transform.position.y, -5), 0.1f);
            
            yield return new WaitForEndOfFrame();
        }
 
    }
    private void TryJump()
    {
  
        
        if (GCaster.GetOnGround())
            {
                canDoubleJump = true;
                Jump(_JumpVelocity);
                    
            }
            else
            {
               TryDoubleJump();
            }
            
        
    }

    private void TryDoubleJump()
    {
     
        
        if (canDoubleJump)
        {
            canDash = true;
            Jump(_JumpVelocity);
            canDoubleJump = false;
        }
        else
        {
            TryDash();
        }
    }

    private void TryDash()
    {
       
        if (canDash)
        {
            Dash(_DashTime,_DashVelocity);
            canDash = false;
        }
    }
    private void TryClimb()
    {
      
        if (cm.GetCanClimb())
        {
            
          
            float speedPerDir = 0f;
            if (Mathf.Abs(dir.x)>Mathf.Abs(dir.y))
            {
                speedPerDir = dir.x;
            }
            else
            {
                speedPerDir = dir.y;
            }
//            Debug.Log(speedPerDir);
            // anim.SetFloat("animSpeed", Mathf.Lerp(anim.GetFloat("animSpeed"), -speedPerDir, 0.2f));
            rb.velocity = new Vector3(_VelocityX/5, -dir.y * _Speed/5, 0) + splashVelocity;

            GetComponent<GravityControl>().G = Physics.gravity.y;

           
                canWallJump = true;
                canDoubleJump = false;
                canDash = false;
 
    
        }
        else
        {
          
            GetComponent<GravityControl>().G = tempG;
            // anim.speed = 1;

            
        }
        // anim.SetBool("OnClimb",cm.GetCanClimb());
    }

    public void ImportantAnimEnd()
    {
        impotantAnim = false;
        // anim.SetBool("impotantAnim",impotantAnim);
    }
 

    public void EnableInput()
    {
        enableInput = true;
    }

    public void DisableInput()
    {
        enableInput = false;
    }

    public void AddSplashVelocity(Vector3 pos,float intensity,float splashTime)
    {
        var dir = (transform.position - pos).normalized;
        StartCoroutine(splashGo(dir, intensity,splashTime));
    }

    private IEnumerator splashGo(Vector3 dir,float intensity,float splashTime)
    {
        var t = 0f;
        Debug.Log( new Vector3(dir.x,1,0) * intensity *(1- t / 1) );
        while (t<1f)
        {
            t += Time.deltaTime*splashTime;
            splashVelocity = new Vector3(0,1,0) * intensity *(1- t / 1);
            yield return new WaitForEndOfFrame();
        }
        splashVelocity = Vector3.zero;
    }
    private IEnumerator onDash(float dashTime,float dashVelocity)
    {
        var t = 0f;
        while (t<dashTime)
        {
            yield return new WaitForEndOfFrame();
            TempDashVelocity = new Vector3(dir.x>=0?1:-1,0,0) * dashVelocity * (1- t/dashTime);
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            t += Time.deltaTime;
            
        }
        TempDashVelocity = Vector3.zero;
    }
    private void TickKey()
    {
        UpBtn.Tick(Input.GetKey(UpKey));
        LeftBtn.Tick(Input.GetKey(LeftKey));
        RightBtn.Tick(Input.GetKey(RightKey));
        DownBtn.Tick(Input.GetKey(DownKey));
        JumpBtn.Tick(Input.GetKey(JumpKey));
        InteractiveBtn.Tick(Input.GetKey(InteractiveKey));
        BoomBtn.Tick(Input.GetKey(BoomKey));
    }
}
