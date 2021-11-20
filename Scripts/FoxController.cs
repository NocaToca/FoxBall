using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class FoxController : MonoBehaviour
{
    [Header("Movement")]
    public float speed;
    public float jumpHeight;
    [Range(0.0f,5.0f)]
    public float walkJumpHeight;
    public float rollingExtraSpeed = 2.0f;
    private float jumpStrength = 0.0f;
    public GameObject feet;
    
    [Header("Animation")]
    [Range(1.0f, 100.0f)]
    public float sleepTime;
    public float animeSpeedWeight;

    [Header("Sleep")]
    public GameObject thoughts;
    public float sleepThoughtTime = 5.0f;
    
    GameObject thoughtBubble;
    bool sleep;
    bool space = false;
    bool grounded;
    bool movementLocked = false;
    bool jumping;
    bool rolling;
    bool walking;

    SpriteRenderer sr;
    Rigidbody2D rb;
    Animator anime;
    BoxCollider2D bc;
    CircleCollider2D cc;

    float timeIdle;
    float timeSleeping;
    float timeGrounded;
    float rollweight;

    bool isGrounded{get{return feet.GetComponent<GroundCheck>().grounded;}}

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anime = GetComponent<Animator>();
        bc = GetComponent<BoxCollider2D>();
        cc = GetComponent<CircleCollider2D>();

        feet.GetComponent<GroundCheck>().AddIgnoredObject(this.gameObject);
        rollweight = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();

        sleep = timeIdle > sleepTime;
        SetAnimParams();
        timeIdle += Time.deltaTime;
        HandleSleep();

    }

    void HandleSleep(){
        if(sleep && !movementLocked){
            movementLocked = true;
        }

        if(thoughtBubble != null){
            if(!sleep){
                thoughtBubble.GetComponent<DreamController>().Pop();
            }
        }

        if(sleepThoughtTime < timeSleeping){
            if(thoughtBubble == null){
                thoughtBubble = Instantiate(thoughts);
                timeSleeping = 0.0f;
                //Debug.Log("created");
            } else {
                thoughtBubble.GetComponent<DreamController>().Play();
            }
        }
        
        if(sleep){
            timeSleeping += Time.deltaTime;
        } else {
            timeSleeping = 0.0f;
        }
        
    }

    void SetAnimParams(){
        anime.SetBool("Sleep", sleep);
        anime.SetBool("SpacePress",space);
        anime.SetFloat("YVel", rb.velocity.y);
        anime.SetBool("Grounded", grounded);
        anime.SetBool("Rolling", rolling);
        anime.SetBool("Walking", walking);
    }

    void HandleMovement(){

        if(movementLocked){
            if(Input.anyKey){
                timeIdle = 0.0f;
            }
            if(!sleep && anime.GetCurrentAnimatorStateInfo(0).IsName("FoxIdle")){
                movementLocked = false;
                timeIdle = 0.0f;
            }
            return;
        }

        Vector2 vel = rb.velocity;

        Roll();

        float dir = Input.GetAxisRaw("Horizontal");
        if(dir > 0.0f){
            sr.flipX = false;
            timeIdle = 0.0f;
            walking = !rolling;
        } else 
        if(dir < 0.0f){
            sr.flipX = true; 
            timeIdle = 0.0f;
            walking = !rolling;
        } else {
            walking = false;
        }
        if(walking){
            if(grounded){   
                vel.y += walkJumpHeight;
            }   
        }

        if(!rolling){
            vel.x = dir * speed;
        }

        grounded = IsGrounded();
        if(grounded){
            timeGrounded += Time.deltaTime;
        } else {
            timeGrounded = 0.0f;
        }
        if(jumping){
            jumping = !grounded;
        }
        if(grounded){
            
            if(Input.GetKey(KeyCode.Space)){
                space = true;
                jumpStrength += Time.deltaTime;
                jumpStrength = (jumpStrength > jumpHeight + jumpHeight/2.0f) ? jumpHeight + jumpHeight/2.0f : jumpStrength;
                timeIdle = 0.0f;
            } else {
                if(space){
                    jumping = true;
                    vel.y += jumpHeight + jumpStrength;
                    space = false;
                    jumpStrength = 0.0f;
                }
            }
            
        }
        if(!grounded){
            timeIdle = 0.0f;
        }

        if(rolling){
            float extraXMovement = dir;
            //vel.x *= rollingExtraSpeed;
            if(Mathf.Abs(extraXMovement) > 0){
                rollweight += Time.deltaTime;
                rollweight = (rollweight > 1.0f) ? 1.0f : rollweight;
                extraXMovement = speed * rollingExtraSpeed * rollweight;
                vel.x = rb.velocity.x + extraXMovement * dir;
            } else {
                vel.x = rb.velocity.x + extraXMovement;
            }
            if(!grounded){
                vel.x = rb.velocity.x;
            }
            
        }

        rb.velocity = vel;

    }

    void Roll(){
        if(!Input.GetKey(KeyCode.LeftShift)){
            if(!rolling){
                return;
            }
            rolling = Mathf.Abs(rb.velocity.x) > .01f;
            if(!rolling){
                bc.enabled = (true);
                cc.enabled = (false);
                anime.speed = 1.0f;
                return;
            } 
        }

        bc.enabled = false;
        cc.enabled = (true);
        rolling = true;
        anime.speed = Mathf.Abs(rb.velocity.x) * animeSpeedWeight;
        transform.rotation = Quaternion.identity;
    }

    public bool IsGrounded(){
        return isGrounded;
    }
}
