using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloudCollider : MonoBehaviour
{

    //public float cloudForce = 12f;

    /*
    void OnCollisionEnter(Collision col)
    {
        Debug.Log("CCCCCOOOOLLLLLIIIIISIIIIONNNN");
        if (col.gameObject.tag == "Cloud")
        {
            Destroy(col.gameObject);
            //instead of destroying, we could move the clouds away from each other
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("CCCCCOOOOLLLLLIIIIISIIIIONNNN STAYYYY");

    }
    */
    private void OnTriggerEnter(Collider other)
    {
        //turning off for now, try a different method that re-instantiates the cloud in a different location
        //other.attachedRigidbody.AddForce(Vector3.right * cloudForce, ForceMode.Acceleration);
    }
}
