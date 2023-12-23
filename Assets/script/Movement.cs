using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float speedSonic;
    [SerializeField] float addSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float mixMaxSpeed;
    [SerializeField] float heightJump;
    [SerializeField] float ForcePushGravity;
    [SerializeField] bool inGround;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform cam;
    [SerializeField] Animator anim;

    Vector3 dirToWallSonic; 
    Vector3 dirNormal;
    float horizontal;
    float vertical;

    // Start is called before the first frame update
    void Start()
    {
        dirNormal = Vector3.up;
    }

    // Update is called once per frame
    void Update()
    {
        //inputs
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        //rotate relative to wall
        RotationSonic();

        //calc decent
        float angle = Vector3.Dot(Vector3.up, transform.forward);
        if(angle > 0.4f || angle < -0.4f)
        {
            maxSpeed += angle * -6 * Time.deltaTime;        
        }else{
            maxSpeed -= 2 * Time.deltaTime;
        }
        maxSpeed = Mathf.Clamp(maxSpeed, mixMaxSpeed, 15);
        //cal vel not ground
        if(!inGround)
        {
            maxSpeed += 2 * Time.deltaTime;  
            dirNormal = Vector3.up;
        }

        //anim
        AnimatorSonic();
    }
    void FixedUpdate() 
    {
        raycasCalc();
        if(dirToWallSonic != Vector3.zero)
        {   
            speedSonic += addSpeed * Time.deltaTime;
            
        }else{
            dirToWallSonic = transform.forward;
            speedSonic -= 6 * Time.deltaTime;
        }
        speedSonic = Mathf.Clamp(speedSonic, 0, maxSpeed); 

        //move player
        rb.AddForce(dirToWallSonic * (vertical * speedSonic), ForceMode.Force);

        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if(flatVel.magnitude > speedSonic)
        {
            rb.velocity = new Vector3(flatVel.normalized.x * speedSonic, rb.velocity.y, flatVel.normalized.z * speedSonic);
        }
    
        //in Ground
        Vector3 posRay = transform.position + transform.up * 0.2f;
        inGround = Physics.Raycast(posRay, -transform.up, 0.5f);
        Debug.DrawRay(posRay, -transform.up, Color.red);
        
        //gravity
        rb.velocity -= dirNormal * ForcePushGravity * Time.deltaTime;

        //jump Sonic
        if(inGround)
        {
            if(Input.GetButtonDown("Jump"))
            {
                float jumpSpeed = Mathf.Sqrt(2 * ForcePushGravity * heightJump);
                rb.velocity = dirNormal * jumpSpeed;
                anim.Play("Ball");
            }
        }else{
            dirToWallSonic.y = 0;
        }

        //rotate sonic to dir
        Vector3 dirRotate = Vector3.Lerp(transform.forward, dirToWallSonic, 10 * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(dirRotate, transform.up);
    }
    void raycasCalc()
    {
        RaycastHit hit;
        Vector3 posPoint = transform.position + transform.up * 0.2f;
        if(Physics.Raycast(posPoint, -transform.up, out hit, 0.5f))
        {
            if(speedSonic >= maxSpeed / 2 && inGround)
            {
                dirNormal = hit.normal;
            }else{
                dirNormal = Vector3.up;
            }

            dirSonicWalk(dirNormal);
        }
    }
    void dirSonicWalk(Vector3 normal)
    {
        Vector3 relativeCam = cam.forward * vertical + cam.right * horizontal;
        dirToWallSonic = Vector3.ProjectOnPlane(relativeCam, normal).normalized;
    }
    void RotationSonic()
    {
        Vector3 newForwardRotate = dirToWallSonic - dirNormal * Vector3.Dot(dirToWallSonic, dirNormal);
        Quaternion newRot = Quaternion.LookRotation(newForwardRotate, dirNormal);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, 5 * Time.deltaTime);
    }
    void AnimatorSonic()
    {
        anim.SetBool("inGround", inGround);
        anim.SetFloat("velocity", speedSonic);
    }
}
