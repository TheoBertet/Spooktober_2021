using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject target;

    private float camLeftBorder;
    private float camRightBorder;
    private float camLength;

    // Start is called before the first frame update
    void Start()
    {
        float dist = (transform.position - Camera.main.transform.position).z;
        camLeftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).x;
        camRightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;

        camLength = camRightBorder - camLeftBorder;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(target.transform.position.x - transform.position.x) >= (camLength / 2) * 0.6)
        {
            Debug.Log("----> Camera to move : ");
            Debug.Log("Target : " + target.transform.position.x + " || Border : " + (camLength / 2) * 0.6);
            Debug.Log("Target - Camera : " + Mathf.Abs(target.transform.position.x - transform.position.x));
            transform.position += new Vector3(Mathf.Abs(target.transform.position.x - transform.position.x) + (target.transform.position.x < 0 ? 1:-1) * (camLength / 2) * 0.6f,
                0,
                0);
        }
    }
}
