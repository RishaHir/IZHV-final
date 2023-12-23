using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameControl : MonoBehaviour
{
    private GameObject p1;
    private GameObject p2;
    private GameObject lano;
    // Start is called before the first frame update
    void Start()
    {
        p1 = GameObject.Find("Player1");
        p2 = GameObject.Find("Player2");
        lano = GameObject.Find("Lano");
        
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics2D.Raycast(p1.transform.position, Vector2.down, 1f, LayerMask.GetMask("Winzone"))
            ||
            Physics2D.Raycast(p2.transform.position, Vector2.down, 1f, LayerMask.GetMask("Winzone"))
           )
        {
            Win();
        }
        
        if (Physics2D.Raycast(p1.transform.position, Vector2.down, 1f, LayerMask.GetMask("Deadzone"))
            ||
            Physics2D.Raycast(p2.transform.position, Vector2.down, 1f, LayerMask.GetMask("Deadzone"))
           )
        {
            Reset();
        }
    }

    private void Win()
    {
        gameObject.GetComponentInChildren<Canvas>().enabled = true;
    }
    
    private void Reset()
    {
        p1.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        p1.GetComponent<Rigidbody2D>().position = new Vector2(-0.83f, -7.8f);
        p2.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        p2.GetComponent<Rigidbody2D>().position = new Vector2(3.38f, -7.8f);

        var rbodies = lano.GetComponentsInChildren<Rigidbody2D>();
        foreach (var rbody in rbodies)
        {
            rbody.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            rbody.gameObject.GetComponent<Rigidbody2D>().position = lano.transform.position;
        }
        
        gameObject.GetComponentInChildren<Canvas>().enabled = false;
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Reset();
        }
    }
}
