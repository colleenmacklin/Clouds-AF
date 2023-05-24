using CursorManagerExtention.Lib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CursorManager : MonoBehaviour
{

    // Public Variables
    public bool iHaveCanvas;
    [ConditionalField("iHaveCanvas")] public GameObject CanvasObject;
    [ConditionalField("iHaveCanvas")] public bool iHaveImage = false;
    [ConditionalField("iHaveImage")] public Image UserImage;
    public Sprite[] CursorSprites;
    [ConditionalField("iHaveImage")] public bool ModifyImage = false;
    [ConditionalField("ModifyImage")] public Material CursorMaterial;
    public float CursorSize = 1;
    public float MarginX = 0;
    public float MarginY = 0;
    public bool IsAnimated;
    [ConditionalField("IsAnimated")] public float AnimationSpeed = 1;


    //Private Variables
    private readonly string ASSET_NAME = "Easy Animated Cursors";
    private Image cursorImage;
    private int SpriteLoc = 0;
    private int spriteLength = 0;
    private bool HaveError = false;
    private bool getSize = true;

    void OnEnable()
    {
       // EventManager.StartListening("openEye", openEyes);
       // EventManager.StartListening("closeEye", closeEyes);
       // EventManager.StartListening("glowEye", glowEyes);
       // EventManager.StartListening("shapeEye", shapeEyes);
    }

    void OnDisable()
    {
       // EventManager.StopListening("openEye", openEyes);
       // EventManager.StopListening("closeEye", closeEyes);
       // EventManager.StopListening("glowEye", glowEyes);
       // EventManager.StopListening("shapeEye", shapeEyes);

    }



    void Start()
    {
        Cursor.visible = false;
        GameObject tmp;

        //call on event manager

        if (!iHaveCanvas)
        {
            iHaveImage = false;
            ModifyImage = false;
            try
            {
                tmp = new GameObject();
                Object prefab = Resources.Load<GameObject>("AnimatedCursor/CanvasTemplate"); // UnityEditor.AssetDatabase.LoadAssetAtPath(@"Assets/" + ASSET_NAME + "/Helpers/CanvasTemplate.prefab", typeof(GameObject));
                GameObject obj = Instantiate(prefab) as GameObject;
                CanvasObject = obj.transform.GetChild(0).gameObject;
            }
            catch (System.Exception e)
            {
                HaveError = true;
                throw new System.Exception(e.Message + "Please check asset path! It should be under Assets. Example Assets/" + ASSET_NAME + "/ \nPlease reimport package or fix it manually");
            }
        }
        else
        {
            tmp = new GameObject();
        }

        if (!iHaveImage)
        {
            cursorImage = tmp.AddComponent<Image>();
            cursorImage.raycastTarget = false;
        }
        else
        {
            if (UserImage == null)
            {
                throw new System.Exception("You should set an User Image!");
            }
            cursorImage = UserImage;
        }

        tmp.transform.SetParent(CanvasObject.transform);
        cursorImage.sprite = CursorSprites[0];
        cursorImage.overrideSprite = CursorSprites[0];
        if (ModifyImage)
        {
            if (CursorMaterial == null)
            {
                throw new System.Exception("You should set an material!");
            }
            cursorImage.material = CursorMaterial;
            cursorImage.preserveAspect = true;
            cursorImage.rectTransform.sizeDelta = new Vector2(cursorImage.sprite.rect.width * CursorSize, cursorImage.sprite.rect.height * CursorSize);
        }

        if (!iHaveImage)
        {
            cursorImage.preserveAspect = true;
            cursorImage.rectTransform.sizeDelta = new Vector2(cursorImage.sprite.rect.width * CursorSize, cursorImage.sprite.rect.height * CursorSize);
        }

        if (IsAnimated)
        {
            spriteLength = CursorSprites.Length;
            //InvokeRepeating("ImageSwapper", 0f, 1 / AnimationSpeed*10);
            //InvokeRepeating("ImageSwapper", 0f, 1);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (HaveError) return;
#if UNITY_ANDROID || UNITY_IOS
        Vector3 MousePos = Input.GetTouch(0).position;
#else
        Vector3 MousePos = Input.mousePosition;
#endif
        cursorImage.transform.position = new Vector3(MousePos.x + MarginX, MousePos.y + MarginY);
        //Press the space bar to apply no locking to the Cursor
        if (Input.GetKey(KeyCode.Space))
            Cursor.lockState = CursorLockMode.None;

    }



    // Swap images in reverse.

    private void ImageSwapperbwds()
    {
        while (SpriteLoc > 0)
        {
            // if (SpriteLoc - 1 >= 0) //used to work - doesn't now! the mystery!
            //{
            SpriteLoc--;
            //}
        }

        cursorImage.overrideSprite = CursorSprites[SpriteLoc];

    }
    // Public Internal Calls.

    /// <summary>
    /// Change cursor sprite on runtime
    /// </summary>
    /// <param name="newSpriteArr"></param>
    /// <returns> true or false </returns>
    public bool SetCursor(Sprite[] newSpriteArr)
    {
        if (newSpriteArr == null || newSpriteArr.Length == 0)
        {
            throw new System.Exception("Sprite array cannot be null or empty! Please double check. ");
        }
        CursorSprites = newSpriteArr;
        SpriteLoc = 0;
        cursorImage.preserveAspect = true;
        spriteLength = newSpriteArr.Length;
        return true;
    }

    // Overloading with single.
    public bool SetCursor(Sprite newSpriteArr)
    {
        if (newSpriteArr == null)
        {
            throw new System.Exception("Sprite array cannot be null! Please double check. ");
        }
        CursorSprites = new Sprite[] { newSpriteArr };
        IsAnimated = false;
        spriteLength = 1;
        StopAnimation();
        cursorImage.overrideSprite = newSpriteArr;
        cursorImage.preserveAspect = true;
        return true;
    }

    /// <summary>
    /// Change cursor size on runtime
    /// </summary>
    /// <param name="newCursorSize"></param>
    public void SetCursorSize(float newCursorSize)
    {
        CursorSize = newCursorSize;
        cursorImage.rectTransform.sizeDelta = new Vector2(cursorImage.sprite.rect.width * CursorSize, cursorImage.sprite.rect.height * CursorSize);
        Debug.Log("Cursor size: " + CursorSize);
    }

    /// <summary>
    /// Set Animation speed on runtime.
    /// </summary>
    /// <param name="newAnimSpeed"></param>
    public void SetAnimationSpeed(float newAnimSpeed)
    {
        AnimationSpeed = newAnimSpeed;
        //openEyes();
        //StartAnimation();
    }

    /// <summary>
    /// Changes cursor material on runtime.
    /// </summary>
    /// <param name="newMaterial"></param>
    public void SetCursorMaterial(Material newMaterial)
    {
        if (newMaterial == null)
        {
            throw new System.Exception("New Material cannot be null! Please double check");
        }
        cursorImage.material = CursorMaterial;
    }

    /// <summary>
    /// Start Animation. This will modify IsAnimated property automatically. 
    /// </summary>
    ///
    void openEyes()
    {
        IsAnimated = true;
        StopAnimation();
        spriteLength = CursorSprites.Length;
        //InvokeRepeating("ImageSwapper", 0f, 1 / AnimationSpeed * 10);
        //InvokeRepeating("ImageSwapper", 0f, 1);
        StartCoroutine(animateSprite(1));
    }

    void closeEyes()
    {
        IsAnimated = true;
        StopAnimation();
        spriteLength = CursorSprites.Length;
        // InvokeRepeating("ImageSwapper", 0f, 1 / AnimationSpeed * 10);
        //InvokeRepeating("ImageSwapperbwds", 0f, 1 / AnimationSpeed * 10);
        StartCoroutine(animateSprite(-1));
    }

    IEnumerator animateSprite(int direction)
    {
        for (int increment = 0; increment < CursorSprites.Length - 1; increment++)
        {
            SpriteLoc += direction;
            if (SpriteLoc <= -1 || SpriteLoc >= CursorSprites.Length) break;
            cursorImage.overrideSprite = CursorSprites[SpriteLoc];
            yield return null;
        }

    }

    // Swap images.
    private void ImageSwapper()
    {
        if (spriteLength - 1 >= SpriteLoc + 1)
        {
            SpriteLoc++;
            //} else
            //{
            //  SpriteLoc = 0;
        }
        cursorImage.overrideSprite = CursorSprites[SpriteLoc];
    }
    void glowEyes()
    {
        //do something cool here when you mousedown - change it depending? show emanating shape?
    }

    void shapeEyes()
    {
        //show the sshape?
    }
    /*
    public void StartAnimation()
    {
        IsAnimated = true;
        StopAnimation();
        spriteLength = CursorSprites.Length;
        InvokeRepeating("ImageSwapper", 0f, 1 / AnimationSpeed * 10);
    }
    */
    /// <summary>
    /// Stop Animation. This will stop animation
    /// </summary>
    ///

    public void StopAnimation()
    {
        //IsAnimated = false;
        CancelInvoke("ImageSwapper");
        CancelInvoke("ImageSwapperbwds");

    }

}
