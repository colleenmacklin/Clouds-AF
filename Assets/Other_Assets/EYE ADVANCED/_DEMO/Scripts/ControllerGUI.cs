using UnityEngine;
using System.Collections;

public class ControllerGUI : MonoBehaviour {

	//PUBLIC VARIABLES
	public Transform sceneLightObject;
	public float lightDirection = 0.25f;
	public float lightIntensity = 0.75f;

	public GameObject modeObjectE;
	public GameObject modeObjectF;
	public Transform targetObjectE;
	public Transform targetObjectF1;
	public Transform targetObjectF2;

	public bool autoDilate = false;
	public float lodLevel = 0.15f;
	public float parallaxAmt = 1.0f;
	public float pupilDilation = 0.5f;
	public float scleraSize = 0.0f;
	public float irisSize = 0.22f;
	public Color irisColor = new Color(1f,1f,1f,1f);
	public Color scleraColor = new Color(1f,1f,1f,1f);

	public int irisTexture = 0;
	public Texture[] irisTextures;

	public Texture2D texTitle;
	public Texture2D texTD;
	public Texture2D texDiv1;
	public Texture2D texSlideA;
	public Texture2D texSlideB;
	public Texture2D texSlideD;

	public Transform lodLevel0;
	public Transform lodLevel1;
	public Transform lodLevel2;


	//PRIVATE VARIABLES
	[HideInInspector]
	public string sceneMode = "figure";

	//private ControllerCamera camControl;
	private float currentLodLevel = 0.0f;
	private float doLodSwitch = -1.0f;
	private Vector3 lodRot;

	private Light sceneLight;
	private Renderer targetRenderer;
	private Renderer targetRenderer1;
	private Renderer targetRenderer2;
	private float lightAngle;
	private float ambientFac;
	//private float currTargetDilation = -2.0f;
	//private float targetDilation = -1.0f;
	//private float dilateTime = 0.0f;

	private float irisTextureF = 0.0f;
	private float irisTextureD = 0.0f;

	private Color colorGold = new Color(0.79f,0.55f,0.054f,1.0f);
	private Color colorGrey = new Color(0.333f,0.3f,0.278f,1.0f);
	private Color colorHighlight = new Color(0.99f,0.75f,0.074f,1.0f);

	private EyeAdv_AutoDilation autoDilateObject;
	//private EyeAdv_AutoDilation autoDilateObject1;
	//private EyeAdv_AutoDilation autoDilateObject2;




	void Start () {
		sceneMode = "eye";

		lodLevel0.gameObject.SetActive(true);
		lodLevel1.gameObject.SetActive(false);
		lodLevel2.gameObject.SetActive(false);

		if (sceneLightObject != null){
			sceneLight = sceneLightObject.GetComponent<Light>();
		}

		if (targetObjectE != null){
			targetRenderer = targetObjectE.transform.GetComponent<Renderer>();
			autoDilateObject = targetObjectE.gameObject.GetComponent<EyeAdv_AutoDilation>();
		}
		//if (targetObjectF1 != null){
		//	targetRenderer1 = targetObjectF1.transform.GetComponent<Renderer>();
		//	autoDilateObject1 = targetObjectF1.gameObject.GetComponent<EyeAdv_AutoDilation>();
		//}
		//if (targetObjectF2 != null){
		//	targetRenderer2 = targetObjectF2.transform.GetComponent<Renderer>();
		//	autoDilateObject2 = targetObjectF2.gameObject.GetComponent<EyeAdv_AutoDilation>();
		//}

		if (targetObjectE != null) targetRenderer = targetObjectE.transform.GetComponent<Renderer>();
		if (targetObjectF1 != null) targetRenderer1 = targetObjectF1.transform.GetComponent<Renderer>();
		if (targetObjectF2 != null) targetRenderer2 = targetObjectF2.transform.GetComponent<Renderer>();

		//camControl = gameObject.GetComponent<ControllerCamera>();
		
	}
	

	void Update () {

		//set scene light
		lightIntensity = Mathf.Clamp(lightIntensity,0.0f,1.0f);
		lightDirection = Mathf.Clamp(lightDirection,0.0f,1.0f);
		sceneLightObject.transform.eulerAngles = new Vector3(	sceneLightObject.transform.eulerAngles.x,
																Mathf.Lerp(0.0f,359.0f,lightDirection),
																sceneLightObject.transform.eulerAngles.z);
		sceneLight.intensity = lightIntensity;


		//handle auto dilation
		if (autoDilateObject != null){
			autoDilateObject.enableAutoDilation = autoDilate;
		}
		//if (autoDilateObject1 != null){
		//	autoDilateObject1.enableAutoDilation = autoDilate;
		//}
		//if (autoDilateObject2 != null){
		//	autoDilateObject2.enableAutoDilation = autoDilate;
		//}
		/*
		if (autoDilate && sceneLightObject != null){

			//calculate look angle
			lightAngle = Vector3.Angle(sceneLightObject.transform.forward,targetObject.transform.forward) / 180.0f;
			targetDilation = Mathf.Lerp(1.0f,0.0f,lightAngle * sceneLight.intensity);

			//handle dilation
			if (currTargetDilation != targetDilation){
				currTargetDilation = targetDilation;
				dilateTime -= Time.deltaTime*2.0f;
			}
			if (pupilDilation != targetDilation){
				dilateTime += Time.deltaTime;
				pupilDilation = Mathf.SmoothStep(pupilDilation,targetDilation,dilateTime*0.05f);
			} else {
				dilateTime = 0.0f;
			}
			
		}
		*/

		//clamp values
		irisSize = Mathf.Clamp(irisSize,0.0f,1.0f);
		parallaxAmt = Mathf.Clamp(parallaxAmt,0.0f,1.0f);
		//pupilDilation = Mathf.Clamp(pupilDilation,0.0f,1.0f);
		scleraSize = Mathf.Clamp(scleraSize,0.0f,1.0f);
		irisTextureF = Mathf.Clamp(Mathf.FloorToInt(irisTextureF),0,irisTextures.Length-1);
		irisTextureD = irisTextureF/(irisTextures.Length-1);
		irisTexture = Mathf.Clamp(Mathf.FloorToInt(irisTextureF),0,irisTextures.Length-1);




		//set shader values
		if (targetRenderer != null){
			targetRenderer.material.SetFloat("_irisSize",Mathf.Lerp(1.5f,5.0f,irisSize));
			targetRenderer.material.SetFloat("_parallax",Mathf.Lerp(0.0f,0.05f,parallaxAmt));
			if (!autoDilate){
				targetRenderer.material.SetFloat("_pupilSize",pupilDilation);
			}
			targetRenderer.material.SetFloat("_scleraSize",Mathf.Lerp(1.1f,2.2f,scleraSize));	
			targetRenderer.material.SetColor("_irisColor",irisColor);
			targetRenderer.material.SetColor("_scleraColor",scleraColor);	
			targetRenderer.material.SetTexture("_IrisColorTex",irisTextures[irisTexture]);	
		}

		if (targetRenderer1 != null){
			targetRenderer1.material.CopyPropertiesFromMaterial(targetRenderer.material);
		}
		if (targetRenderer2 != null){
			targetRenderer2.material.CopyPropertiesFromMaterial(targetRenderer.material);
		}


		//check and switch LOD level
		if (currentLodLevel != lodLevel){

			doLodSwitch = -1.0f;
			if (lodLevel < 0.31f && currentLodLevel > 0.31f) doLodSwitch = 0.0f;
			if (lodLevel > 0.70f && currentLodLevel > 0.70f) doLodSwitch = 2.0f;
			if (lodLevel > 0.31f && lodLevel < 0.70f){
				if (currentLodLevel < 0.31f || currentLodLevel > 0.70f){
					doLodSwitch = 1.0f;
				}
			}

			currentLodLevel = lodLevel;
			//lodRot = targetObjectF1.transform.eulerAngles;
			if (doLodSwitch >= 0.0f){
				if (doLodSwitch == 0.0f && lodLevel0 != null){
					lodLevel0.gameObject.SetActive(true);
					lodLevel1.gameObject.SetActive(false);
					lodLevel2.gameObject.SetActive(false);
					targetObjectF1 = lodLevel0;
				}
				if (doLodSwitch == 1.0f && lodLevel1 != null){
					lodLevel0.gameObject.SetActive(false);
					lodLevel1.gameObject.SetActive(true);
					lodLevel2.gameObject.SetActive(false);
					targetObjectF1 = lodLevel1;
				}
				if (doLodSwitch == 2.0f && lodLevel2 != null){
					lodLevel0.gameObject.SetActive(false);
					lodLevel1.gameObject.SetActive(false);
					lodLevel2.gameObject.SetActive(true);
					targetObjectF1 = lodLevel2;
				}
				//if (targetObjectF1 != null){

				//}
				//targetObjectF1.transform.eulerAngles = lodRot;
				//camControl.cameraTarget = targetObjectF1;
			}


		}

	/*
		//Switch Mode Object
		if (sceneMode == "figure"){
			if (modeObjectE != null) modeObjectE.SetActive(false);
			if (modeObjectF != null) modeObjectF.SetActive(true);
		}
		if (sceneMode == "eye"){
			if (modeObjectF != null) modeObjectF.SetActive(false);
			if (modeObjectE != null) modeObjectE.SetActive(true);
		}
	*/
	}




	void OnGUI(){

		//Main Title
		GUI.color = new Color(1.0f,1.0f,1.0f,1.0f);
		if (texTitle != null) GUI.Label(new Rect (25,25, texTitle.width,texTitle.height), texTitle);
		if (texTD != null) GUI.Label(new Rect (800,45, texTD.width*2,texTD.height*2), texTD);


		//VIEW MODE
		GUI.color = new Color(1.0f,1.0f,1.0f,1.0f);
		if (texDiv1 != null) GUI.Label(new Rect (150,130, texDiv1.width,texDiv1.height), texDiv1);

		GUI.color = colorGold;
		//if (sceneMode == "figure") GUI.color = colorGrey;
		//if (Rect(35,128,100,20).Contains(Event.current.mousePosition)) GUI.color = colorHighlight;
		GUI.Label (new Rect (35, 128, 180, 20), "EYEBALL VIEW");

		//GUI.color = colorGold;
		GUI.color = colorGrey;
		//if (Rect(160,128,100,20).Contains(Event.current.mousePosition)) GUI.color = colorHighlight;
		GUI.Label (new Rect (160, 128, 280, 20), "CHARACTER VIEW (not included)");

		//if (sceneMode == "figure"){
		//	GUI.color = colorHighlight;
		//	GUI.Label (Rect (825, 710, 280, 20), "note: figure is not included");
		//}

		if (Event.current.type == EventType.MouseUp && new Rect(35,128,100,20).Contains(Event.current.mousePosition)) sceneMode = "eye";
		if (Event.current.type == EventType.MouseUp && new Rect(160,128,100,20).Contains(Event.current.mousePosition)) sceneMode = "figure";

		//SETTINGS - LOD 
		GenerateSlider("EYE LOD LEVEL",35,185,false,"lodLevel",293);
		GUI.color = new Color(1.0f,1.0f,1.0f,1.0f);
		if (texDiv1 != null) GUI.Label(new Rect (130,217, texDiv1.width,texDiv1.height), texDiv1);
		if (texDiv1 != null) GUI.Label(new Rect (240,217, texDiv1.width,texDiv1.height), texDiv1);
		GUI.color = colorGold;
		if (lodLevel>0.32f) GUI.color = colorGrey;
		if (new Rect(60,215,40,20).Contains(Event.current.mousePosition)) GUI.color = colorHighlight;
		GUI.Label (new Rect(60, 215, 40, 20), "LOD 0");
		if (Event.current.type == EventType.MouseUp && new Rect(60,215,100,20).Contains(Event.current.mousePosition)) lodLevel = 0.0f;
		GUI.color = colorGold;
		if (lodLevel<0.32f || lodLevel>0.70f) GUI.color = colorGrey;
		if (new Rect(165,215,50,20).Contains(Event.current.mousePosition)) GUI.color = colorHighlight;
		GUI.Label (new Rect(165, 215, 50, 20), "LOD 1");
		if (Event.current.type == EventType.MouseUp && new Rect(165,215,100,20).Contains(Event.current.mousePosition)) lodLevel = 0.5f;
		GUI.color = colorGold;
		if (lodLevel<0.70f) GUI.color = colorGrey;
		if (new Rect(270,215,50,20).Contains(Event.current.mousePosition)) GUI.color = colorHighlight;
		GUI.Label (new Rect(270, 215, 50, 20), "LOD 2");
		if (Event.current.type == EventType.MouseUp && new Rect(270,215,100,20).Contains(Event.current.mousePosition)) lodLevel = 1.0f;

		//SETTINGS - Pupil Dilation
		GenerateSlider("PUPIL DILATION",35,248,true,"pupilDilation",293);
		GUI.color = new Color(1.0f,1.0f,1.0f,1.0f);
		if (texDiv1 != null) GUI.Label(new Rect (272,280, texDiv1.width,texDiv1.height), texDiv1);
		GUI.color = colorGold;
		if (!autoDilate) GUI.color = colorGrey;
		if (new Rect(240,278,40,20).Contains(Event.current.mousePosition)) GUI.color = colorHighlight;
		GUI.Label (new Rect(240, 278, 40, 20), "auto");
		GUI.color = colorGold;
		if (autoDilate) GUI.color = colorGrey;
		if (new Rect(280,278,40,20).Contains(Event.current.mousePosition)) GUI.color = colorHighlight;
		GUI.Label (new Rect(280, 278, 50, 20), "manual");
		if (Event.current.type == EventType.MouseUp && new Rect(240,278,40,20).Contains(Event.current.mousePosition)) autoDilate = true;
		if (Event.current.type == EventType.MouseUp && new Rect(280,278,50,20).Contains(Event.current.mousePosition)) autoDilate = false;


		//SETTINGS - Sclera Size
		GenerateSlider("SCLERA SIZE",35,310,true,"scleraSize",293);


		//SETTINGS - Iris Size
		GenerateSlider("IRIS SIZE",35,350,true,"irisSize",293);


		//SETTINGS - Iris Texture
		GenerateSlider("IRIS TEXTURE",35,390,false,"irisTexture",293);
		GUI.color = new Color(1.0f,1.0f,1.0f,1.0f);
		for (int t = 0; t < irisTextures.Length; t++){
			if (texDiv1 != null) GUI.Label(new Rect (36+(t*22),416, texDiv1.width,texDiv1.height), texDiv1);
		}

		//SETTINGS - Iris Parallax
		GenerateSlider("IRIS PARALLAX",35,440,true,"irisParallax",293);


		// SETTINGS - Iris Color
		GUI.color = colorGold;
		GUI.Label (new Rect (35,510, 180, 20), "IRIS COLOR");
		GUI.color = colorGrey;
		GUI.Label (new Rect (35,525, 20, 20), "r");
		GUI.Label (new Rect (35,538, 20, 20), "g");
		GUI.Label (new Rect (35,551, 20, 20), "b");
		GUI.Label (new Rect (35,564, 20, 20), "a");
		GenerateSlider("",50,512,false,"irisColorR",278);
		GenerateSlider("",50,525,false,"irisColorG",278);
		GenerateSlider("",50,538,false,"irisColorB",278);
		GenerateSlider("",50,550,false,"irisColorA",278);


		// SETTINGS - Sclera Color
		GUI.color = colorGold;
		GUI.Label (new Rect (35,590, 180, 20), "SCLERA COLOR");
		GUI.color = colorGrey;
		GUI.Label (new Rect (35,605, 20, 20), "r");
		GUI.Label (new Rect (35,618, 20, 20), "g");
		GUI.Label (new Rect (35,631, 20, 20), "b");
		GUI.Label (new Rect (35,644, 20, 20), "a");
		GenerateSlider("",50,592,false,"scleraColorR",278);
		GenerateSlider("",50,605,false,"scleraColorG",278);
		GenerateSlider("",50,618,false,"scleraColorB",278);
		GenerateSlider("",50,630,false,"scleraColorA",278);


		//LIGHT - Direction
		GUI.color = colorGold;
		GUI.Label (new Rect (35,730, 150, 20), "LIGHT DIRECTION");
		GenerateSlider("",160,716,false,"lightDir",820);

	}




	void GenerateSlider(string title, int sX, int sY, bool showPercent, string funcName, int sWidth){

		GUI.color = colorGold;
		if (title != "") GUI.Label (new Rect (sX,sY, 180, 20), title);

		if (funcName == "lightDir" && showPercent) GUI.Label (new Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt((lightDirection*100.0f)).ToString()+"%");
		if (funcName == "lodLevel" && showPercent) GUI.Label (new Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(100.0f-(lodLevel*100.0f)).ToString()+"%");
		if (funcName == "pupilDilation" && showPercent) GUI.Label (new Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(pupilDilation*100.0f).ToString()+"%");
		if (funcName == "scleraSize" && showPercent) GUI.Label (new Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(scleraSize*100.0f).ToString()+"%");
		if (funcName == "irisSize" && showPercent) GUI.Label (new Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(irisSize*100.0f).ToString()+"%");
		if (funcName == "irisTexture" && showPercent) GUI.Label (new Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(irisTextureD*100.0f).ToString()+"%");
		if (funcName == "irisParallax" && showPercent) GUI.Label (new Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(parallaxAmt*100.0f).ToString()+"%");
		if (funcName == "irisColorR" && showPercent) GUI.Label (new Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(irisColor.r*100.0f).ToString()+"%");
		if (funcName == "irisColorG" && showPercent) GUI.Label (new Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(irisColor.g*100.0f).ToString()+"%");
		if (funcName == "irisColorB" && showPercent) GUI.Label (new Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(irisColor.b*100.0f).ToString()+"%");
		if (funcName == "irisColorA" && showPercent) GUI.Label (new Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(irisColor.a*100.0f).ToString()+"%");
		if (funcName == "scleraColorR" && showPercent) GUI.Label (new Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(scleraColor.r*100.0f).ToString()+"%");
		if (funcName == "scleraColorG" && showPercent) GUI.Label (new Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(scleraColor.g*100.0f).ToString()+"%");
		if (funcName == "scleraColorB" && showPercent) GUI.Label (new Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(scleraColor.b*100.0f).ToString()+"%");
		if (funcName == "scleraColorA" && showPercent) GUI.Label (new Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(scleraColor.a*100.0f).ToString()+"%");


		GUI.color = new Color(1.0f,1.0f,1.0f,1.0f);
		if (texSlideB != null) GUI.DrawTextureWithTexCoords(new Rect (sX,sY+22, sWidth+2,7), texSlideB, new Rect (sX,sY+22, sWidth+2,7), true);
		
		if (funcName == "lightDir" && texSlideA != null) GUI.DrawTextureWithTexCoords(new Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,lightDirection),5), texSlideA, new Rect (sX+1,sY+23, sWidth,5), true);
		if (funcName == "lodLevel" && texSlideA != null) GUI.DrawTextureWithTexCoords(new Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,lodLevel),5), texSlideA, new Rect (sX+1,sY+23, sWidth,5), true);
		if (funcName == "pupilDilation" && texSlideA != null) GUI.DrawTextureWithTexCoords(new Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,pupilDilation),5), texSlideA, new Rect (sX+1,sY+23, sWidth,5), true);
		if (funcName == "scleraSize" && texSlideA != null) GUI.DrawTextureWithTexCoords(new Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,scleraSize),5), texSlideA, new Rect (sX+1,sY+23, sWidth,5), true);
		if (funcName == "irisSize" && texSlideA != null) GUI.DrawTextureWithTexCoords(new Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,irisSize),5), texSlideA, new Rect (sX+1,sY+23, sWidth,5), true);
		if (funcName == "irisTexture" && texSlideA != null) GUI.DrawTextureWithTexCoords(new Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,irisTextureD),5), texSlideA, new Rect (sX+1,sY+23, sWidth,5), true);
		if (funcName == "irisParallax" && texSlideA != null) GUI.DrawTextureWithTexCoords(new Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,parallaxAmt),5), texSlideA, new Rect (sX+1,sY+23, sWidth,5), true);
			
		GUI.color = new Color(irisColor.r,irisColor.g,irisColor.b,irisColor.a);
		if (funcName == "irisColorR" && texSlideD != null) GUI.DrawTextureWithTexCoords(new Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,irisColor.r),5), texSlideD, new Rect (sX+1,sY+23, sWidth,5), true);
		if (funcName == "irisColorG" && texSlideD != null) GUI.DrawTextureWithTexCoords(new Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,irisColor.g),5), texSlideD, new Rect (sX+1,sY+23, sWidth,5), true);
		if (funcName == "irisColorB" && texSlideD != null) GUI.DrawTextureWithTexCoords(new Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,irisColor.b),5), texSlideD, new Rect (sX+1,sY+23, sWidth,5), true);
		GUI.color = colorGrey*2f;
		if (funcName == "irisColorA" && texSlideD != null) GUI.DrawTextureWithTexCoords(new Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,irisColor.a),5), texSlideD, new Rect (sX+1,sY+23, sWidth,5), true);
		GUI.color = new Color(scleraColor.r,scleraColor.g,scleraColor.b,scleraColor.a);
		if (funcName == "scleraColorR" && texSlideD != null) GUI.DrawTextureWithTexCoords(new Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,scleraColor.r),5), texSlideD, new Rect (sX+1,sY+23, sWidth,5), true);
		if (funcName == "scleraColorG" && texSlideD != null) GUI.DrawTextureWithTexCoords(new Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,scleraColor.g),5), texSlideD, new Rect (sX+1,sY+23, sWidth,5), true);
		if (funcName == "scleraColorB" && texSlideD != null) GUI.DrawTextureWithTexCoords(new Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,scleraColor.b),5), texSlideD, new Rect (sX+1,sY+23, sWidth,5), true);
		GUI.color = colorGrey*2f;
		if (funcName == "scleraColorA" && texSlideD != null) GUI.DrawTextureWithTexCoords(new Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,scleraColor.a),5), texSlideD, new Rect (sX+1,sY+23, sWidth,5), true);
														

		GUI.color = new Color(1.0f,1.0f,1.0f,0.0f);
		if (funcName == "lightDir") lightDirection = GUI.HorizontalSlider (new Rect (sX-4, sY+19, sWidth+17, 10), lightDirection, 0.0f, 1.0f);
		if (funcName == "lodLevel") lodLevel = GUI.HorizontalSlider (new Rect (sX-4, sY+19, sWidth+17, 10), lodLevel, 0.0f, 1.0f);
		if (funcName == "pupilDilation") pupilDilation = GUI.HorizontalSlider (new Rect (sX-4, sY+19, sWidth+17, 10), pupilDilation, 0.0f, 1.0f);
		if (funcName == "scleraSize") scleraSize = GUI.HorizontalSlider (new Rect (sX-4, sY+19, sWidth+17, 10), scleraSize, 0.0f, 1.0f);
		if (funcName == "irisSize") irisSize = GUI.HorizontalSlider (new Rect (sX-4, sY+19, sWidth+17, 10), irisSize, 0.0f, 1.0f);
		if (funcName == "irisTexture") irisTextureF = GUI.HorizontalSlider (new Rect (sX-4, sY+19, sWidth+17, 10), irisTextureF, 0.0f, (float)irisTextures.Length-1f);
		if (funcName == "irisParallax") parallaxAmt = GUI.HorizontalSlider (new Rect (sX-4, sY+19, sWidth+17, 10), parallaxAmt, 0.0f, 1.0f);
		if (funcName == "irisColorR") irisColor.r = GUI.HorizontalSlider (new Rect (sX-4, sY+19, sWidth+17, 10), irisColor.r, 0.0f, 1.0f);
		if (funcName == "irisColorG") irisColor.g = GUI.HorizontalSlider (new Rect (sX-4, sY+19, sWidth+17, 10), irisColor.g, 0.0f, 1.0f);
		if (funcName == "irisColorB") irisColor.b = GUI.HorizontalSlider (new Rect (sX-4, sY+19, sWidth+17, 10), irisColor.b, 0.0f, 1.0f);
		if (funcName == "irisColorA") irisColor.a = GUI.HorizontalSlider (new Rect (sX-4, sY+19, sWidth+17, 10), irisColor.a, 0.0f, 1.0f);
		if (funcName == "scleraColorR") scleraColor.r = GUI.HorizontalSlider (new Rect (sX-4, sY+19, sWidth+17, 10), scleraColor.r, 0.0f, 1.0f);
		if (funcName == "scleraColorG") scleraColor.g = GUI.HorizontalSlider (new Rect (sX-4, sY+19, sWidth+17, 10), scleraColor.g, 0.0f, 1.0f);
		if (funcName == "scleraColorB") scleraColor.b = GUI.HorizontalSlider (new Rect (sX-4, sY+19, sWidth+17, 10), scleraColor.b, 0.0f, 1.0f);
		if (funcName == "scleraColorA") scleraColor.a = GUI.HorizontalSlider (new Rect (sX-4, sY+19, sWidth+17, 10), scleraColor.a, 0.0f, 1.0f);

	}
}
