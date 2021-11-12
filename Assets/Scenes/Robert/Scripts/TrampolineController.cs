using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineController : MonoBehaviour
{
    // Trampoline Control
    CharacterController pController;

    float xMovement;
    [SerializeField] float speed = 5f;


    // Explosion/Bounch Related
    float force = 10f;
    float radius = 10f;



    void Start()
    {
        pController = GetComponent<CharacterController>();
    }

    void Update()
    {

        xMovement = Input.GetAxis("Horizontal");


        pController.Move(new Vector3(xMovement, 0f, 0f) * speed * Time.deltaTime);

    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log("Hello");

        if (other.gameObject.CompareTag("Player"))
        {
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();

            rb.AddExplosionForce(force, transform.position, radius);
        }
    }
}