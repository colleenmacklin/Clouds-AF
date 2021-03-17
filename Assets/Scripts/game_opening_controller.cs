using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class game_opening_controller : MonoBehaviour
{

    public GameObject Cloud;
    public Texture2D[] ShapeArray; //textures stay as an array because we are not generating run time textures
    public Texture2D chosenShape;
    private int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.TriggerEvent("UpdateMe"); //tell components to update, again, might be a more performant/efficient way to do this
    }

    void Update()
    {

        //call this on a timer, or set of co-routines
        if (Input.GetKeyDown("o"))
        {
            SetCloudToShape();

        }


    }

    
    private void SetCloudToShape()
    {
        //choose a random cloud to turn into a shape
        //This whole logic might need to be changed


        if (i <= (ShapeArray.Length-1))
        {
            Debug.Log("ShapeArray.Length: " + ShapeArray.Length);
            Debug.Log("shapenum: " + i);
            chosenShape = ShapeArray[i];

            i++;
        }
        else i = 0;

        EventManager.TriggerEvent("SpawnShape"); //tell a cloud to turn into a shape


        //int shapeNum = (Random.Range(0, ShapeArray.Length));

        Debug.Log("shape: " + chosenShape.name);
        //EventManager.TriggerEvent("UpdateMe"); //tell components to update, again, might be a more performant/efficient way to do this


    }


}
