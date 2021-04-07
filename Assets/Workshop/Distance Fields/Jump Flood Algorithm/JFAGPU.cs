using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;


public class JFAGPU : MonoBehaviour
{
    //public Texture2D imageTarget;
    public Texture imageTarget;
    public int rez = 512;
    public bool gradient;
    public Material outputMaterial;
    public RenderTexture inputImageTex;
    public Material imageMaterial;
    public Vector2Int imageDisplaceVector;
    public float textureRotation;
    public float textureScale;


    public RenderTexture laplaceTexture;

    public ComputeShader compute;

    public int stepPosition = 0;
    public RenderTexture JFATexA;
    public RenderTexture JFATexB;
    public RenderTexture JFATexC;
    public RenderTexture JFARenderTex;

    public Material displayMaterial;

    private List<RenderTexture> textures;
    void Start()
    {
        Reset();
        // WebCamTexture tex = new WebCamTexture();
        // imageTarget = tex;
        // tex.requestedHeight = 512;
        // tex.requestedWidth = 512;
        // tex.requestedFPS = 30;
        // tex.Play();
        //displayMaterial.SetTexture("_DistanceTexture", JFARenderTex);
    }

    void Update()
    {
        Clear();
        stepPosition = 0;
        ImageToTexture();
        Edge();
        JFAInit();
        for (int i = 0; i < Mathf.Log((float)rez, 2) + 1; i++)
        {
            JFAStep();
        }
        Sign();
        if (gradient) JFAGradient();
        else JFARender();

        JFATexC = JFARenderTex;
    }

    private void SetDisplayMaterial()
    {

    }

    [Button]
    public void Reset()
    {
        Release();
        laplaceTexture = CreateTexture(rez, FilterMode.Point);
        JFATexA = CreateTexture(rez, FilterMode.Point);
        JFATexB = CreateTexture(rez, FilterMode.Point);
        JFATexC = CreateTexture(rez, FilterMode.Point);
        JFARenderTex = CreateTexture(rez, FilterMode.Point);
        inputImageTex = CreateTexture(rez, FilterMode.Point);
        stepPosition = 0;
        GPUResetKernel();
        outputMaterial.SetTexture("_MainTex", laplaceTexture);
    }

    //Clears all current textures.
    private void Clear()
    {
        int kernel = compute.FindKernel("ClearTextureKernel");

        compute.SetTexture(kernel, "outTex", JFATexA);
        compute.Dispatch(kernel, rez / 32, rez / 32, 1);

        compute.SetTexture(kernel, "outTex", JFATexB);
        compute.Dispatch(kernel, rez / 32, rez / 32, 1);

        compute.SetTexture(kernel, "outTex", JFARenderTex);
        compute.Dispatch(kernel, rez / 32, rez / 32, 1);

        compute.SetTexture(kernel, "outTex", laplaceTexture);
        compute.Dispatch(kernel, rez / 32, rez / 32, 1);

        compute.SetTexture(kernel, "outTex", inputImageTex);
        compute.Dispatch(kernel, rez / 32, rez / 32, 1);
    }

    private int CalculateJump(int step)
    {
        return (int)Mathf.Pow(2f, Mathf.Log((float)rez, 2f) - (float)step - 1f);
    }

    [Button]
    public void ImageToTexture()
    {
        int kernel = compute.FindKernel("WriteImageKernel");
        compute.SetTexture(kernel, "outTex", inputImageTex);
        compute.SetTexture(kernel, "inTex", imageTarget);
        compute.SetInt("rez", rez);
        compute.SetFloat("textureScale", textureScale);
        compute.SetFloat("textureRotation", textureRotation / 180 * Mathf.PI);
        compute.SetVector("imageDisplacement", new Vector4(imageDisplaceVector.x, imageDisplaceVector.y, 0f, 0f));
        compute.Dispatch(kernel, rez / 16, rez / 16, 1);

    }

    [Button]
    public void Edge() //Laplacian
    {
        int kernel = compute.FindKernel("LaplaceKernel");
        compute.SetTexture(kernel, "outTex", laplaceTexture);
        compute.SetTexture(kernel, "inTex", inputImageTex);
        compute.SetInt("rez", rez);
        compute.Dispatch(kernel, rez / 16, rez / 16, 1);
    }

    [Button]
    public void Sign()
    {

        int kernel = compute.FindKernel("SignKernel");
        compute.SetTexture(kernel, "inTex", inputImageTex);
        compute.SetTexture(kernel, "outTex", JFATexB);//runs after the texture is created and adds sign
        compute.Dispatch(kernel, rez / 16, rez / 16, 1);
    }

    [Button]
    public void JFAInit()
    {
        int kernel = compute.FindKernel("JFAInitKernel");
        compute.SetTexture(kernel, "outTex", JFATexA);
        compute.SetTexture(kernel, "inTex", laplaceTexture);
        compute.SetInt("rez", rez);
        compute.Dispatch(kernel, rez / 16, rez / 16, 1);
        outputMaterial.SetTexture("_MainTex", JFATexA);
    }

    [Button]
    public void JFAStep()
    {
        //now to ping pong two textures.
        int kernel = compute.FindKernel("JFAStepKernel");
        compute.SetTexture(kernel, "outTex", JFATexB);
        compute.SetTexture(kernel, "inTex", JFATexA);
        compute.SetInt("rez", rez);
        int jumpSize = CalculateJump(stepPosition);
        compute.SetInt("jumpSize", jumpSize);
        //Debug.Log(jumpSize);
        compute.Dispatch(kernel, rez / 16, rez / 16, 1);
        Swap();
        stepPosition++;

        outputMaterial.SetTexture("_MainTex", JFATexB);
    }

    [Button]
    public void JFARender()
    {
        int kernel = compute.FindKernel("JFARenderKernel");
        compute.SetTexture(kernel, "outTex", JFARenderTex);
        compute.SetTexture(kernel, "inTex", JFATexB);
        compute.SetInt("rez", rez);
        compute.Dispatch(kernel, rez / 16, rez / 16, 1);
        outputMaterial.SetTexture("_MainTex", JFARenderTex);
    }

    [Button]
    public void JFAGradient()
    {
        int kernel = compute.FindKernel("JFAGradientKernel");
        compute.SetTexture(kernel, "inTex", JFATexB);
        compute.SetTexture(kernel, "outTex", JFARenderTex);
        compute.SetInt("rez", rez);
        compute.Dispatch(kernel, rez / 16, rez / 16, 1);
        outputMaterial.SetTexture("_MainTex", JFARenderTex);
    }
    private void GPUResetKernel()
    {
        int kernel = compute.FindKernel("ResetKernel");
        compute.SetTexture(kernel, "outTex", laplaceTexture);
        compute.Dispatch(kernel, rez / 16, rez / 16, 1);
    }

    private RenderTexture CreateTexture(int r, FilterMode filterMode)
    {
        RenderTexture texture = new RenderTexture(r, r, 1, RenderTextureFormat.ARGBFloat);

        texture.name = "Output";
        texture.enableRandomWrite = true;
        texture.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
        texture.volumeDepth = 1;
        texture.filterMode = filterMode;
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.autoGenerateMips = false;
        texture.useMipMap = false;

        texture.Create();
        textures.Add(texture);

        return texture;
    }

    private void Swap()
    {
        var tmp = JFATexA;
        JFATexA = JFATexB;
        JFATexB = tmp;
    }


    public void Release()
    {
        // if (buffers != null)
        // {
        //     foreach (ComputeBuffer buffer in buffers)
        //     {
        //         if (buffer != null)
        //         {
        //             buffer.Release();
        //         }
        //     }
        // }

        // buffers = new List<ComputeBuffer>();

        if (textures != null)
        {
            foreach (RenderTexture tex in textures)
            {
                if (tex != null)
                {
                    tex.Release();
                }
            }
        }

        textures = new List<RenderTexture>();
    }

    void OnDestroy()
    {
        Release();
    }

    void OnEnable()
    {
        Release();
    }

    void OnDisable()
    {
        Release();
    }
}
