using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    /* things we need to do here:
     * - Keep track of shapes, remove when used
     * - Change a random cloud's shape triggered by either a click or timer
     *  - maintain a timer, overall number of changes
     * 
     */

	public static GameEvents current;

	public GameObject[] CloudArray;
	public Sprite[] ShapeArray;
	public bool timeToChange; //amount of time it takes to change the cloud
	public int numChanges; //this number is the total number of cloud changes/events before we go to the ending scene. could also trigger state changes like sunset, etc.

	private myShape shape;

	private Cloud myCloud;

	private void Awake()
	{
		current = this;
		CloudArray = GameObject.FindGameObjectsWithTag("Cloud");
	}

    private void Start()
	{
		shape = CloudArray[0].GetComponent<myShape>();

		print("clouds: " + shape.myName);
		ChangeClouds(CloudArray[0]);

	}

    public void ChangeClouds(GameObject g)
	{

		//triggered by either a timer or a click event
		//tells cloud to change
		//g.SpawnObject(ShapeArray[0]);
		g.GetComponent<myShape>();
		//myCloud = g.GetComponent.<Cloud>().SpawnObject(ShapeArray[0]); //UnityScript
		//myCloud = g.FindObjectOfType(typeof(Cloud)) as Cloud;
//		myCloud = g.Cloud;
		//myCloud.SpawnObject(ShapeArray[0]);

		//g.GetComponent < Spawnobject(ShapeArray[0]) >;

	}

	//public event Action onChangeClouds;
	/*
	 public void ChangeClouds()
		{
			if (onChangeClouds != null)
			{
				onChangeClouds();
			}
		}
		*/
}
