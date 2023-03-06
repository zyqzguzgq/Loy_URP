﻿using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Serialization;

[ExecuteAlways]
public class Ocean_Manager : MonoBehaviour
{
    private Material oceanMat;
    
    public Gradient AbsorptionCradient;
    public Gradient ScatterCradient;
    
    private Texture2D _sssTexture;
    private static readonly int AbsorptionScatteringLUT = Shader.PropertyToID("_AbsorptionScatteringLUT");

    private void OnEnable()
    {
        oceanMat = GetComponent<MeshRenderer>().sharedMaterial;
        
        GenerateSSSColorLUT();
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