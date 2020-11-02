//Attach this script to a GameObject to have it output messages when your mouse hovers over it.
using UnityEngine;
using TMPro;

public class OnMouseOverCloud : MonoBehaviour
{

    public GameObject cloudName;

    //When the mouse hovers over the GameObject, it turns to this color (red)
    Color m_MouseOverColor = Color.red;

    //This stores the GameObject’s original color
    Color m_OriginalColor;

    //Get the GameObject’s mesh renderer to access the GameObject’s material and color
    MeshRenderer m_Renderer;
    public string theShape;

    void Start()
	{
		//Fetch the mesh renderer component from the GameObject
		m_Renderer = GetComponent<MeshRenderer>();
		//Fetch the original color of the GameObject
		m_OriginalColor = m_Renderer.material.color;
	}

	void OnMouseOver()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        //var myShape = this.GetComponent<Cloud>.name;
        //If your mouse hovers over the GameObject with the script attached, output this message
        //Debug.Log("Mouse is over GameObject.");
        EventManager.TriggerEvent("mouseoverCloud");


        //Debug.Log("Mouse is over: " + myShape);
        //theShape = myShape;

        //if (cloudName.GetComponent<TextMeshPro>().text == theShape){
        //    cloudName.GetComponent<TextMeshPro>().text = "yes! it is a " + theShape + "!";
        //}
        // Change the color of the GameObject to red when the mouse is over GameObject
        //m_Renderer.material.color = m_MouseOverColor;


    }

    void OnMouseExit()
    {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        Debug.Log("Mouse is no longer on GameObject.");
		// Reset the color of the GameObject back to normal
		//m_Renderer.material.color = m_OriginalColor;

	}
}