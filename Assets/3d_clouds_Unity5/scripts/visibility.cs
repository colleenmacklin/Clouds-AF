using UnityEngine;
using System.Collections;

public class visibility : MonoBehaviour {
	public static string act1="1";
	public static string act2="2";
	public static string act3="3";
	public static string act4="4";
	public static string act5="x";
	public static string act6="6";
	public static float rate;
	
	
	private float drk =0f;
	private float tr=1f;
	
	
	// Use this for initialization
	void Start () { //remembering all emissionrates
		rate=this.GetComponent<ParticleSystem>().emissionRate;
		
		
	}
	
	void OnGUI(){//activating clouds when corresponding btn was pressed 
		
		if (this.name=="cloud1"){
		if (GUI.Button(new Rect(30,30,20,20),act1)){
	this.GetComponent<Renderer>().enabled=true;
			act1="x";
			act2="2";
			act3="3";
			act4="4";
			act5="5";
			act6="6";
				
		}
			
		}
		
			if (this.name=="cloud2"){
		if (GUI.Button(new Rect(60,30,20,20),act2)){
	this.GetComponent<Renderer>().enabled=true;
				act1="1";
			act2="x";
			act3="3";
			act4="4";
			act5="5";
			act6="6";
		}
			
		}
		
		
			if (this.name=="cloud3"){
			
		if (GUI.Button(new Rect(90,30,20,20),act3)){
	this.GetComponent<Renderer>().enabled=true;
				act1="1";
			act2="2";
			act3="x";
			act4="4";
			act5="5";
			act6="6";
		}
			
		}
		
		
			if (this.name=="cloud4"){
		if (GUI.Button(new Rect(120,30,20,20),act4)){
	this.GetComponent<Renderer>().enabled=true;
				act1="1";
			act2="2";
			act3="3";
			act4="x";
			act5="5";
			act6="6";
		}
			
		}
		
		
			if (this.name=="cloud5"){
		if (GUI.Button(new Rect(150,30,20,20),act5)){
		this.GetComponent<Renderer>().enabled=true;
				act1="1";
			act2="2";
			act3="3";
			act4="4";
			act5="x";
			act6="6";
		}
			
		}
		
		
			if (this.name=="cloud6"){
		if (GUI.Button(new Rect(180,30,20,20),act6)){
				   
				this.GetComponent<Renderer>().enabled=true;
			
				act1="1";
			act2="2";
			act3="3";
			act4="4";
			act5="5";
			act6="x";
				
		
		}
			
		}

		
		
	}
	

	
	// Update is called once per frame
	void Update () {
		
		if (tr!=transparency.density){
		
	tr=transparency.density;
	this.GetComponent<ParticleSystem>().emissionRate = rate*tr;
			print(tr);
		}
		
			if (drk!=transparency.darkness){
		
	drk=transparency.darkness;
	this.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1f-drk,1f-drk,1f-drk,this.GetComponent<Renderer>().material.GetColor("_TintColor").a));
			print(tr);
		}
		
	if (this.name=="cloud1" && this.GetComponent<Renderer>().enabled==true && act1=="1"){
  this.GetComponent<Renderer>().enabled=false;
		}
		
		
			if (this.name=="cloud2" && this.GetComponent<Renderer>().enabled==true && act2=="2"){
  this.GetComponent<Renderer>().enabled=false;
		}
		
		
			if (this.name=="cloud3" && this.GetComponent<Renderer>().enabled==true && act3=="3"){
  this.GetComponent<Renderer>().enabled=false;
		}
		
		
			if (this.name=="cloud4" && this.GetComponent<Renderer>().enabled==true && act4=="4"){
  this.GetComponent<Renderer>().enabled=false;
		}
		
		
			if (this.name=="cloud5" && this.GetComponent<Renderer>().enabled==true && act5=="5"){
  this.GetComponent<Renderer>().enabled=false;
		}
		
		
				if (this.name=="cloud6" && this.GetComponent<Renderer>().enabled==true && act6=="6"){
  this.GetComponent<Renderer>().enabled=false;
		}
		
	}
}
