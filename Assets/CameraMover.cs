using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    private Rigidbody2D rb1;

    private Rigidbody2D rb2;
    // Start is called before the first frame update
    void Start()
    {
        rb1 = GameObject.Find("Player1").GetComponent<Rigidbody2D>();
        rb2 = GameObject.Find("Player2").GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = (rb1.position - (rb1.position - rb2.position)/2);
        transform.position += new Vector3(0, 5, -10);
    }
}
