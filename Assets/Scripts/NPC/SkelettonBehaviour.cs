using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkelettonBehaviour : MonoBehaviour
{
    public LayerMask platformsLayer;
    public float movingSpeed;
    public bool startReturned;

    private BoxCollider2D bc;
    private Rigidbody2D rb;
    private Animator animator;

    private bool canMove = true;
    private bool isTurningRight = false;

    // Start is called before the first frame update
    void Start()
    {
        bc = transform.GetComponent<BoxCollider2D>();
        rb = transform.GetComponent<Rigidbody2D>();
        animator = transform.GetComponent<Animator>();

        if (startReturned)
            Flip();
    }

    // Update is called once per frame
    void Update()
    {
        Animate();
        CheckDirection();
    }

    private void CheckDirection()
    {
        if (Physics2D.OverlapCircle(transform.position
                            + new Vector3((isTurningRight? 1 : -1) * (bc.bounds.size.x / 2)*2.5f, -(bc.bounds.size.y/2)*1.4f, 0f), .005f, platformsLayer)
            && !Physics2D.OverlapCircle(transform.position
                            + new Vector3((isTurningRight ? 1 : -1) * (bc.bounds.size.x / 2) * 2.5f, 0f, 0f), .005f, platformsLayer))
        {
            if(canMove)
            {
                Move();
            }
        }
        else
        {
            Flip();
        }
    }

    private void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, 0);
        isTurningRight = !isTurningRight;
    }

    private void Move()
    {
        rb.velocity = new Vector3(movingSpeed * (isTurningRight ? 1 : -1), rb.velocity.y);
    }

    private void Animate()
    {
        if(Mathf.Abs(rb.velocity.x) >= 0.03f)
        {
            animator.SetBool("isRunning", true);
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
