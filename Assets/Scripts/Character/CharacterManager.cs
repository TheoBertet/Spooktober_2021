using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public float invincibilityDuration;
    public int maxHP;

    public GameObject pentaclePrefab;
    public CinemachineVirtualCamera cinemachineVirtualCamera;

    private CharacterMovementManager cmm;
    private SpriteRenderer sr;
    private GameUIManager gameUI;
    private Animator animator;

    private GameObject lastPentacle;

    private bool canBeHit = true;
    private int hp;
    private int souls;
    private bool alive = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        cmm = transform.GetComponent<CharacterMovementManager>();
        cmm.SetCharacterManager(GetComponent<CharacterManager>());
        sr = transform.GetComponent<SpriteRenderer>();
        gameUI = GameObject.Find("UI").GetComponent<GameUIManager>();
        gameUI.SetMaxHP(maxHP);
        hp = maxHP;
        souls = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T) && cmm.IsGrounded())
        {
            Sacrifice();
        }
    }

    private void AddSoul()
    {
        souls++;
        gameUI.AddSoul(1);
    }

    private void Sacrifice()
    {
        if(souls > 0)
        {
            cmm.StartSacrifice();
            souls--;
            gameUI.ConsumeSoul();
        }
    }

    public void SummonPentacle()
    {
        if (lastPentacle != null)
            Destroy(lastPentacle);

        lastPentacle = Instantiate(pentaclePrefab,transform.position, transform.rotation);
    }

    private bool ResurrectIfPossible()
    {
        if(lastPentacle != null)
        {
            hp = maxHP;
            gameUI.Heal(maxHP);
            transform.position = lastPentacle.transform.position;
            Destroy(lastPentacle);
            StartCoroutine(CropCamera());
            return true;
        }
        return false;
    }

    IEnumerator InvicibilityAnim()
    {
        for (float timeStart = Time.time; invincibilityDuration >= Time.time - timeStart;)
        {
            sr.color = new Color(1, 0.7f, 0.7f, 0.9f);
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(.1f);
        }
        sr.enabled = true;
        canBeHit = true;
        sr.color = new Color(1, 1, 1, 1);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (alive && collision.tag == "Monster")
        {
            if (cmm.IsDashing())
            {
                collision.transform.parent.GetComponent<SkelettonBehaviour>().Die();
            }
            else
            {
                if (canBeHit)
                {
                    StartCoroutine(InvicibilityAnim());
                    canBeHit = false;
                    gameUI.Damage(1);
                    hp--;
                    CheckForDeath();
                }
            }
        }

        if(alive && collision.tag == "DieTrigger")
        {
            Die();
        }

        if (collision.tag == "Rabbit")
        {
            AddSoul();
            Destroy(collision.gameObject);
        }

        if(collision.tag == "TutoTrigger")
        {
            collision.gameObject.GetComponent<TextMeshPro>().enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "TutoTrigger")
        {
            collision.gameObject.GetComponent<TextMeshPro>().enabled = false;
        }
    }

    private void CheckForDeath()
    {
        if(hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if(!ResurrectIfPossible())
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            gameUI.Die();
            cmm.enabled = false;
            enabled = false;
            alive = false;
        }
    }

    IEnumerator CropCamera()
    {
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_DeadZoneHeight = 0f;

        yield return new WaitForSeconds(.2f);

        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_DeadZoneHeight = .7f;

        yield return null;
    }
}
