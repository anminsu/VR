using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

    public Vector3 myVelocity;

	void Update()
    {
        myVelocity = GetComponent<Rigidbody>().velocity;
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log("Owww Collision");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Owww Trigger");
    }
}
