﻿using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Serialization;



[ExecuteAlways]
public class Ocean_Manager : MonoBehaviour
{

    public enum ReflectMode
    {
        PlanarReflect,
        SSR,
        None
    };
    
    private Material oceanMat;

    public ReflectMode rfmode;
    
    public Gradient AbsorptionCradient;
    public Gradient ScatterCradient;
    
    private Texture2D _sssTexture;
    private static readonly int AbsorptionScatteringLUT = Shader.PropertyToID("_AbsorptionScatteringLUT");

    //private CommandBuffer cmdBuffer;

    private void OnEnable()
    {
        oceanMat = GetComponent<MeshRenderer>().sharedMaterial;
        
        GenerateSSSColorLUT();

       // cmdBuffer = new CommandBuffer();
    }

    //private void OnDestroy()
    //{
    //    cmdBuffer.Release();
    //}

    private void Update()
    {
        if (rfmode == ReflectMode.SSR)
        {
            CoreUtils.SetKeyword(oceanMat, "_SSRREFLECT", true);
            CoreUtils.SetKeyword(oceanMat, "_REFLECTION_PLANARREFLECTION", false);
            //oceanMat.SetFloat("_ReflectMode", 1);
        }
        else if(rfmode == ReflectMode.PlanarReflect)
        {
            CoreUtils.SetKeyword(oceanMat, "_SSRREFLECT", false);
            CoreUtils.SetKeyword(oceanMat, "_REFLECTION_PLANARREFLECTION", true);
            //oceanMat.SetFloat("_ReflectMode", 0);
        }
        else
        {
            //oceanMat.SetFloat("_ReflectMode", 2);
        }
        //cmdBuffer.
    }

    void GenerateSSSColorLUT()
    {
        if(_sssTexture == null)
            _sssTexture = new Texture2D(128, 4, GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None);
        _sssTexture.wrapMode = TextureWrapMode.Clamp;

        var cols = new Color[512];
        for (var i = 0; i < 128; i++)
        {
            cols[i] = AbsorptionCradient.Evaluate(i / 128f);
        }
        for (var i = 0; i < 128; i++)
        {
            cols[i + 128] = ScatterCradient.Evaluate(i / 128f);
        }
        for (var i = 0; i < 128; i++)
        {
            cols[i + 256] = Color.black;
        }

        _sssTexture.SetPixels(cols);
        _sssTexture.Apply();
        Shader.SetGlobalTexture(AbsorptionScatteringLUT, _sssTexture);
    }
    
}
