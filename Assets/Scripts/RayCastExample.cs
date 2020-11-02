using UnityEngine;
using System.Collections;

public class RayCastExample : MonoBehaviour
{

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("RAYCAST -- Name = " + hit.collider.name);
                Debug.Log("RAYCAST -- Tag = " + hit.collider.tag);
                //Debug.Log("RAYCAST -- Hit Point = " + hit.point);
                //Debug.Log("RAYCAST -- Object position = " + hit.collider.gameObject.transform.position);
                Debug.Log("--------------");
            }
        }
    }
}
