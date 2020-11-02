using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //this allows the dialogue to show up in the inspector
public class Dialogue
{

	public string name;

	[TextArea(3, 10)]
	public string[] sentences; //an array of sentences


}