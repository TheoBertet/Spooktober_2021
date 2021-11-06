using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarManager : MonoBehaviour
{
    public GameObject KeyFrame;

    private bool isPlayerHere = false;

    // Start is called before the first frame update
    void Start()
    {
        KeyFrame.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E) && isPlayerHere)
        {
            GameManager.Instance.NextLevel();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            KeyFrame.SetActive(true);
            isPlayerHere = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            KeyFrame.SetActive(false);
            isPlayerHere = false;
        }
    }
}
