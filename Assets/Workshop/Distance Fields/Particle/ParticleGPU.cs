using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;


//Use this as the base object
public struct Agent
{
    public Vector2 pos;
    public Vector2 vel;
    public Vector2 acc;
    public Vector2 oPos;
}

public class ParticleGPU : MonoBehaviour
{

    [Header("Trail Agent Params")]
    [Range(1024, 2000000)]
    public int population = 1024;
    private ComputeBuffer agentsBuffer;
    private ComputeBuffer agentsPositionBuffer;

    [Range(0f, 5f)]
    public float velocityLimit = .9f;

    [Range(0, 5)]
    int sensorRange = 10; //physarum?

    //the size of the resultant texture
    public enum Size
    {
        SMALL = 256,
        MEDIUM = 512,
        LARGE = 1024
    }
    [Header("Setup")]

    private int rez;
    [SerializeField]
    public Size resolution;
    [SerializeField]
    int stepsPerFrame = 0;
    [SerializeField]
    int stepMod = 1;

    [Header("Particle Setups")]
    [SerializeField]
    [Range(1, 4)]
    private int blurRange = 2;
    [SerializeField]
    [Range(0f, 1f)]
    private float decayFactor = .95f;
    [SerializeField]
    [Range(0f, 5f)]
    private float steerForce = 1f;
    [SerializeField]
    [Range(0f, 5f)]
    private float gradientForce = 1.5f;
    [SerializeField]
    [Range(0f, 5f)]
    private float maxAcc = .2f;
    [SerializeField]
    [Range(0f, 2f)]
    private float velocityDecay = .95f;
    [SerializeField]
    [Range(0f, 1f)]
    private float allowedSpeed = 1f;

    public ComputeShader compute;
    public Material targetMaterial;
    public Material writeMaterial;
    public Material diffuseMaterial;

    [SerializeField]
    public JFAGPU floodFiller;
    [SerializeField]
    private RenderTexture refTex;

    private RenderTexture outTex;
    private RenderTexture diffuseTex;
    private RenderTexture gradientTex;

    private RenderTexture writeTex;
    private RenderTexture debugTex;
    private RenderTexture tmp;

    private List<ComputeBuffer> buffers;
    private List<RenderTexture> textures;

    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    // Update is called once per frame`
    void Update()
    {
        refTex = floodFiller.JFARenderTex;

        GPUStepAgents();
        GPUWriteAgents();
        GPUDiffuseAgents();
        GPUGradient();
        Render();

    }

    [Button]
    private void Reset()
    {
        Release();
        buffers = new List<ComputeBuffer>();
        textures = new List<RenderTexture>();
        rez = (int)resolution;

        //textures
        outTex = CreateTexture(rez, FilterMode.Point);
        diffuseTex = CreateTexture(rez, FilterMode.Point);
        gradientTex = CreateTexture(rez, FilterMode.Point);
        writeTex = CreateTexture(rez, FilterMode.Point);

        textures.Add(outTex);
        textures.Add(diffuseTex);
        textures.Add(gradientTex);
        textures.Add(writeTex);

        //buffer
        agentsBuffer = new ComputeBuffer(population, sizeof(float) * 8);
        buffers.Add(agentsBuffer);

        GPUReset();
        GPUResetAgents();

        writeMaterial.SetTexture("_UnlitColorMap", gradientTex);
        diffuseMaterial.SetTexture("_UnlitColorMap", diffuseTex);
    }

    private void GPUReset()
    {
        int kernel;

        kernel = compute.FindKernel("ResetKernel");
        compute.SetInt("resolution", rez);

        compute.SetTexture(kernel, "outTex", outTex);
        compute.Dispatch(kernel, rez / 32, rez / 32, 1);

        compute.SetTexture(kernel, "outTex", diffuseTex);
        compute.Dispatch(kernel, rez / 32, rez / 32, 1);

        compute.SetTexture(kernel, "outTex", gradientTex);
        compute.Dispatch(kernel, rez / 32, rez / 32, 1);

        compute.SetTexture(kernel, "outTex", writeTex);
        compute.Dispatch(kernel, rez / 32, rez / 32, 1);
    }

    [Button]
    private void GPUResetAgents()
    {
        int kernel;
        kernel = compute.FindKernel("ResetAgentsKernel");

        compute.SetInt("resolution", rez);
        compute.SetBuffer(kernel, "agentsBuffer", agentsBuffer);

        compute.Dispatch(kernel, population / 1024, 1, 1);

    }

    [Button]
    private void GPUStepAgents()
    {
        int kernel;

        kernel = compute.FindKernel("StepAgentsKernel");

        compute.SetInt("resolution", rez);
        compute.SetFloat("velocityLimit", velocityLimit);
        compute.SetFloat("gradientForce", gradientForce);
        compute.SetFloat("steerForce", steerForce);
        compute.SetFloat("velocityDecay", velocityDecay);
        compute.SetFloat("maxAcc", maxAcc);
        compute.SetFloat("allowedSpeed", allowedSpeed);
        compute.SetBuffer(kernel, "agentsBuffer", agentsBuffer);
        compute.SetTexture(kernel, "inTex", diffuseTex);
        compute.SetTexture(kernel, "refTex", refTex);

        compute.Dispatch(kernel, population / 1024, 1, 1);

    }
    [Button]
    private void GPUWriteAgents()
    {
        int kernel;

        //Note that in the case of the physarum and stuff we don't
        //actually draw the physarum, we just draw their trails.
        //so that trail texture can be ping ponged back and forth
        //in this one we will claer a texture and rewrite to it.
        // kernel = compute.FindKernel("ResetKernel");
        // compute.SetTexture(kernel, "outTex", writeTex);
        // compute.Dispatch(kernel, 32, 32, 1);

        kernel = compute.FindKernel("WriteAgentsKernel");


        compute.SetBuffer(kernel, "agentsBuffer", agentsBuffer);
        compute.SetTexture(kernel, "outTex", writeTex);

        compute.Dispatch(kernel, population / 1024, 1, 1);
    }

    [Button]
    private void GPUDiffuseAgents()
    {
        int kernel;

        // kernel = compute.FindKernel("ResetKernel");
        // compute.SetTexture(kernel, "outTex", write);
        // compute.Dispatch(kernel, 32, 32, 1);

        kernel = compute.FindKernel("DiffuseKernel");
        compute.SetFloat("decayFactor", decayFactor);
        compute.SetInt("blurRange", blurRange);
        compute.SetInt("resolution", rez);
        compute.SetTexture(kernel, "inTex", writeTex);
        compute.SetTexture(kernel, "outTex", diffuseTex);

        compute.Dispatch(kernel, rez / 32, rez / 32, 1);

    }

    [Button]
    private void GPUGradient()
    {
        int kernel;

        kernel = compute.FindKernel("ParticleGradientKernel");
        compute.SetInt("resolution", rez);
        compute.SetTexture(kernel, "inTex", diffuseTex);
        compute.SetTexture(kernel, "outTex", gradientTex);

        compute.Dispatch(kernel, rez / 32, rez / 32, 1);

    }
    [Button]
    private void GPUCopyTexture(ref RenderTexture TexA, ref RenderTexture TexB)
    {
        int kernel;

        kernel = compute.FindKernel("CopyTextureKernel");
        compute.SetTexture(kernel, "inTex", TexA);
        compute.SetTexture(kernel, "outTex", TexB);

        compute.Dispatch(kernel, rez / 32, rez / 32, 1);

    }

    [Button]
    private void Render()
    {//needs to be swapped and changed before render
        Swap(ref writeTex, ref diffuseTex);

        targetMaterial.SetTexture("_BaseColorMap", diffuseTex);
        targetMaterial.SetTexture("_NormalMap", gradientTex);
    }

    //////////////////////////////////////
    //
    // UTIL
    //
    ////////////////////////////////////////

    public void Swap(ref RenderTexture texA, ref RenderTexture texB)
    {
        tmp = texA;
        texA = texB;
        texB = tmp;
    }

    public void Swap()
    {
        tmp = writeTex;
        writeTex = diffuseTex;
        diffuseTex = tmp;
    }
    public void Release()
    {
        if (buffers != null)
        {
            foreach (ComputeBuffer buffer in buffers)
            {
                if (buffer != null)
                {
                    buffer.Release();
                    //We have to manually release buffers
                }
            }
        }

        buffers = new List<ComputeBuffer>();

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

    protected RenderTexture CreateTexture(int r, FilterMode filterMode)
    {
        RenderTexture texture = new RenderTexture(r, r, 1, RenderTextureFormat.ARGBFloat);

        texture.name = "out";
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
}
