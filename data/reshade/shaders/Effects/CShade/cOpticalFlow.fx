
#include "shared/cGraphics.fxh"
#include "shared/cImageProcessing.fxh"
#include "shared/cVideoProcessing.fxh"

/*
    [Shader parameters]
*/

CREATE_OPTION(float, _MipBias, "Optical flow", "Optical flow mipmap bias", "slider", 7.0, 0.0)
CREATE_OPTION(float, _BlendFactor, "Optical flow", "Temporal blending factor", "slider", 0.9, 0.0)

CREATE_TEXTURE(Tex1, BUFFER_SIZE_1, R8, 3)
CREATE_SAMPLER(SampleTex1, Tex1, LINEAR, MIRROR)

CREATE_TEXTURE(Tex2a, BUFFER_SIZE_2, RG16F, 8)
CREATE_SAMPLER(SampleTex2a, Tex2a, LINEAR, MIRROR)

CREATE_TEXTURE(Tex2b, BUFFER_SIZE_2, RG16F, 8)
CREATE_SAMPLER(SampleTex2b, Tex2b, LINEAR, MIRROR)

CREATE_TEXTURE(Tex2c, BUFFER_SIZE_2, RG16F, 8)
CREATE_SAMPLER(SampleTex2c, Tex2c, LINEAR, MIRROR)

CREATE_TEXTURE(OFlowTex, BUFFER_SIZE_2, RG16F, 1)
CREATE_SAMPLER(SampleOFlowTex, OFlowTex, LINEAR, MIRROR)

CREATE_TEXTURE(Tex3, BUFFER_SIZE_4, RG16F, 1)
CREATE_SAMPLER(SampleTex3, Tex3, LINEAR, MIRROR)

CREATE_TEXTURE(Tex4, BUFFER_SIZE_6, RG16F, 1)
CREATE_SAMPLER(SampleTex4, Tex4, LINEAR, MIRROR)

// Vertex shaders

VS2PS_Blur VS_HBlur(APP2VS Input)
{
    return GetVertexBlur(Input, 1.0 / BUFFER_SIZE_2, true);
}

VS2PS_Blur VS_VBlur(APP2VS Input)
{
    return GetVertexBlur(Input, 1.0 / BUFFER_SIZE_2, false);
}

VS2PS_Sobel VS_Sobel(APP2VS Input)
{
    return GetVertexSobel(Input, 1.0 / BUFFER_SIZE_2);
}

// Pixel shaders

float PS_Saturation(VS2PS_Quad Input) : SV_TARGET0
{
    float3 Color = tex2D(CShade_SampleColorTex, Input.Tex0).rgb;
    return SaturateRGB(Color);
}

float PS_HBlur_Prefilter(VS2PS_Blur Input) : SV_TARGET0
{
    return GetPixelBlur(Input, SampleTex1).r;
}

float PS_VBlur_Prefilter(VS2PS_Blur Input) : SV_TARGET0
{
    return GetPixelBlur(Input, SampleTex2a).r;
}

// Process spatial derivatives

float2 PS_Sobel(VS2PS_Sobel Input) : SV_TARGET0
{
    return GetPixelSobel(Input, SampleTex2c);
}

// Run Lucas-Kanade

float2 PS_PyLK_Level3(VS2PS_Quad Input) : SV_TARGET0
{
    float2 Vectors = 0.0;
    return GetPixelPyLK(Input.Tex0, Vectors, SampleTex2a, SampleTex2c, SampleTex2b, 2, true);
}

float2 PS_PyLK_Level2(VS2PS_Quad Input) : SV_TARGET0
{
    float2 Vectors = tex2D(SampleTex4, Input.Tex0).xy;
    return GetPixelPyLK(Input.Tex0, Vectors, SampleTex2a, SampleTex2c, SampleTex2b, 1, false);
}

float4 PS_PyLK_Level1(VS2PS_Quad Input) : SV_TARGET0
{
    float2 Vectors = tex2D(SampleTex3, Input.Tex0).xy;
    return float4(GetPixelPyLK(Input.Tex0, Vectors, SampleTex2a, SampleTex2c, SampleTex2b, 0, false), 0.0, _BlendFactor);
}

// Postfilter blur

// We use MRT to immeduately copy the current blurred frame for the next frame
float4 PS_HBlur_Postfilter(VS2PS_Blur Input, out float4 Copy : SV_TARGET0) : SV_TARGET1
{
    Copy = tex2D(SampleTex2b, Input.Tex0.xy);
    return float4(GetPixelBlur(Input, SampleOFlowTex).rg, 0.0, 1.0);
}

float4 PS_VBlur_Postfilter(VS2PS_Blur Input) : SV_TARGET0
{
    return float4(GetPixelBlur(Input, SampleTex2a).rg, 0.0, 1.0);
}

float4 PS_Display(VS2PS_Quad Input) : SV_TARGET0
{
    float2 InvTexSize = float2(ddx(Input.Tex0.x), ddy(Input.Tex0.y));

    float2 Vectors = tex2Dlod(SampleTex2b, float4(Input.Tex0.xy, 0.0, _MipBias)).xy;
    Vectors = DecodeVectors(Vectors, InvTexSize);

    float3 NVectors = normalize(float3(Vectors, 1.0));
    NVectors = saturate((NVectors * 0.5) + 0.5);

    return float4(NVectors, 1.0);
}

#define CREATE_PASS(VERTEX_SHADER, PIXEL_SHADER, RENDER_TARGET) \
    pass \
    { \
        VertexShader = VERTEX_SHADER; \
        PixelShader = PIXEL_SHADER; \
        RenderTarget0 = RENDER_TARGET; \
    }

technique CShade_OpticalFlow
{
    // Normalize current frame
    CREATE_PASS(VS_Quad, PS_Saturation, Tex1)

    // Prefilter blur
    CREATE_PASS(VS_HBlur, PS_HBlur_Prefilter, Tex2a)
    CREATE_PASS(VS_VBlur, PS_VBlur_Prefilter, Tex2b)

    // Calculate derivatives
    CREATE_PASS(VS_Sobel, PS_Sobel, Tex2a)

    // Bilinear Lucas-Kanade Optical Flow
    CREATE_PASS(VS_Quad, PS_PyLK_Level3, Tex4)
    CREATE_PASS(VS_Quad, PS_PyLK_Level2, Tex3)

    pass GetFineOpticalFlow
    {
        ClearRenderTargets = FALSE;
        BlendEnable = TRUE;
        BlendOp = ADD;
        SrcBlend = INVSRCALPHA;
        DestBlend = SRCALPHA;

        VertexShader = VS_Quad;
        PixelShader = PS_PyLK_Level1;
        RenderTarget0 = OFlowTex;
    }

    // Postfilter blur
    pass MRT_CopyAndBlur
    {
        VertexShader = VS_HBlur;
        PixelShader = PS_HBlur_Postfilter;
        RenderTarget0 = Tex2c;
        RenderTarget1 = Tex2a;
    }

    pass
    {
        VertexShader = VS_VBlur;
        PixelShader = PS_VBlur_Postfilter;
        RenderTarget0 = Tex2b;
    }

    // Display
    pass
    {
        SRGBWriteEnable = WRITE_SRGB;

        VertexShader = VS_Quad;
        PixelShader = PS_Display;
    }
}