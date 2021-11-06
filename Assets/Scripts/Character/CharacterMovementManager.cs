using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterMovementManager : MonoBehaviour
{
    public float movingSpeed;
    public float jumpingForce;
    public float dashPower;
    public float dashReload;
    public float dashDuration;
    public LayerMask platformsLayer;
    public LayerMask grabCornersLayer;
    public LayerMask monstersLayer;
    public GameObject keyFramePrefab;

    private Animator animator;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private CharacterManager cm;

    private bool isRunning = false;
    private bool canRun = true;
    private bool isTurningRight = false;
    private bool canJump = true;
    private bool isGrounded = false;
    private bool isGrabbing = false;
    private bool isDashing = false;
    private bool canDash = true;
    private bool isSacrificing = false;
    private float lastDash;

    private GameObject keyFrameGob;

    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetComponent<Animator>();
        sr = transform.GetComponent<SpriteRenderer>();
        rb = transform.GetComponent<Rigidbody2D>();
        bc = transform.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInputs();
        Animate();
        MoveCharacter();
        CheckReloads();
    }

    private void CheckInputs()
    {
        // On actualise isGrounded pour savoir si le joueur est au sol ou dans les airs,
        // Utile pour les sauts
        CheckIfGrounded();

        if (isGrabbing)
        {
            canRun = false;
        }
        else
        {
            canRun = true;
            // On regarde si le joueur s'accroche à un bord de plateforme
            CheckForCornerGrab();
        }



        // Gestion du saut
        if (canJump && isGrounded && !isSacrificing) {
            //if (Input.GetAxisRaw("Jump") > 0)
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }



        // Gestion de l'arrêt des déplacements
        if(!isRunning && canRun)
        {
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                isRunning = true;
            }
        }
        
        // Gestion des déplacements
        if(isRunning)
        {
            // Dash activé en déplacement
            if(canDash && Input.GetAxisRaw("Dash") > 0)
            {
                Dash();
            }

            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                isTurningRight = true;
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
                isTurningRight = false;
            }
            else
            {
                isRunning = false;
            }
        }
    }

    private void CheckReloads()
    {
        // Check for Dash's reload
        if(!canDash)
        {
            if (Time.time - lastDash >= dashReload)
            {
                canDash = true;
            }
        }
    }

    private void Animate()
    {
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isGrabbing", isGrabbing);
        animator.SetBool("isDashing", isDashing);
        transform.localScale = new Vector3(isTurningRight ? -1 : 1,transform.localScale.y);
        animator.SetBool("isSacrificing", isSacrificing);
    }

    private void MoveCharacter()
    {
        // Pour éviter que la vélocité fasse stick le player aux murs
        if (!isGrabbing && !isSacrificing && isRunning && !Physics2D.OverlapBox(transform.position
                            + new Vector3((Input.GetAxis("Horizontal") > 0 ? 1 : -1) * bc.bounds.size.x / 2, 0f, 0f), new Vector2(.1f, bc.bounds.size.y), 0f, platformsLayer))
        {
            int direction = Input.GetAxis("Horizontal") > 0 ? 1 : -1;
            rb.velocity = new Vector3(movingSpeed * direction, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y);
        }
    }

    private void Jump()
    {
        // Un jump après un grab pour se libérer et sauter plus haut
        if (isGrabbing)
            Ungrab();

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0, 100 * jumpingForce));
    }

    private void Dash()
    {
        StartCoroutine(DashAnim());
        lastDash = Time.time;
        canDash = false;
        isDashing = true;
    }

    IEnumerator DashAnim()
    {
        for(float timeStart = Time.time; dashDuration >= Time.time - timeStart;)
        {
            rb.AddForce(new Vector2(dashPower * 100 * (isTurningRight ? 1 : -1), 0));
            yield return null;
        }
        isDashing = false;
    }


    private void CheckIfGrounded()
    {
        // On crée une collision temporaire aux pieds du joueur pour voir s'il est en contact
        // avec une plateforme
        // Le joueur est considéré comme grounded si il est accroché à un rebord
        if (Physics2D.OverlapCircle(transform.position
            + new Vector3(0f, -bc.bounds.size.y / 2, 0f), bc.bounds.size.x/2, platformsLayer) || isGrabbing)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void CheckForCornerGrab()
    {
        // On crée une collision temporaire à gauche/droite du joueur pour voir s'il est en contact
        // avec une zone de grab
        Collider2D cornerToGrab;
        if (cornerToGrab = Physics2D.OverlapCircle(transform.position
            + new Vector3((isTurningRight?1:-1) * bc.bounds.size.x / 2, 0f, 0f), .1f, grabCornersLayer))
        {
            if (Input.GetAxis("Action") > 0 && !isSacrificing)
                Grab(cornerToGrab);
        }
        else
        {

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "GrabCorner")
        {
            ShowKeyFrame("E", collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "GrabCorner")
        {
            HideKeyFrame();
        }
    }

    private void Grab(Collider2D cornerToGrab)
    {
        // On considère le joueur en phase de grab, l'empêchant de se déplacer normalement
        isGrabbing = true;

        // On désactive la gravité et les mouvements ; le joueur est immobile pendant le grab
        rb.velocity = new Vector2(0f, 0f);
        rb.gravityScale = 0;

        // On déplace le joueur à l'emplacement du grab
        transform.position = cornerToGrab.transform.position;

        HideKeyFrame();
    }
    private void Ungrab()
    {
        // On rétablit la gravité
        rb.gravityScale = 1;

        // On ne considère plus le joueur en phase de grab
        isGrabbing = false;
    }

    public bool IsDashing()
    {
        return isDashing;
    }

    private void ShowKeyFrame(string key, Transform parent)
    {
        if(keyFrameGob == null)
        {
            keyFrameGob = Instantiate<GameObject>(keyFramePrefab, parent);
            keyFrameGob.transform.Find("Key").GetComponent<TextMeshPro>().SetText(key);
        }
    }

    private void HideKeyFrame()
    {
        if(keyFrameGob != null)
        {
            Destroy(keyFrameGob);
        }
    }

    public void StartSacrifice()
    {
        isSacrificing = true;
    }

    public void EndSacrifice()
    {
        isSacrificing = false;
        cm.SummonPentacle();
    }

    public void SetCharacterManager(CharacterManager cm)
    {
        this.cm = cm;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}
