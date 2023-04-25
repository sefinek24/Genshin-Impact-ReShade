
#include "shared/cGraphics.fxh"

struct VS2PS_Median
{
    float4 HPos : SV_POSITION;
    float4 Tex0 : TEXCOORD0;
    float4 Tex1 : TEXCOORD1;
    float4 Tex2 : TEXCOORD2;
};

VS2PS_Median VS_Median(APP2VS Input)
{
    float2 PixelSize = 1.0 / (float2(BUFFER_WIDTH, BUFFER_HEIGHT));

    VS2PS_Quad FSQuad = VS_Quad(Input);

    VS2PS_Median Output;
    Output.HPos = FSQuad.HPos;
    Output.Tex0 = FSQuad.Tex0.xyyy + (float4(-1.0, 1.0, 0.0, -1.0) * PixelSize.xyyy);
    Output.Tex1 = FSQuad.Tex0.xyyy + (float4(0.0, 1.0, 0.0, -1.0) * PixelSize.xyyy);
    Output.Tex2 = FSQuad.Tex0.xyyy + (float4(1.0, 1.0, 0.0, -1.0) * PixelSize.xyyy);
    return Output;
}

// Math functions: https://github.com/microsoft/DirectX-Graphics-Samples/blob/master/MiniEngine/Core/Shaders/DoFMedianFilterCS.hlsl

float4 Max3(float4 A, float4 B, float4 C)
{
    return max(max(A, B), C);
}

float4 Min3(float4 A, float4 B, float4 C)
{
    return min(min(A, B), C);
}

float4 Med3(float4 A, float4 B, float4 C)
{
    return clamp(A, min(B, C), max(B, C));
}

float4 Med9(float4 X0, float4 X1, float4 X2,
            float4 X3, float4 X4, float4 X5,
            float4 X6, float4 X7, float4 X8)
{
    float4 A = Max3(Min3(X0, X1, X2), Min3(X3, X4, X5), Min3(X6, X7, X8));
    float4 B = Min3(Max3(X0, X1, X2), Max3(X3, X4, X5), Max3(X6, X7, X8));
    float4 C = Med3(Med3(X0, X1, X2), Med3(X3, X4, X5), Med3(X6, X7, X8));
    return Med3(A, B, C);
}

float4 PS_Median(VS2PS_Median Input) : SV_TARGET0
{
    // Sample locations:
    // [0].xy [1].xy [2].xy
    // [0].xz [1].xz [2].xz
    // [0].xw [1].xw [2].xw
    float4 Sample[9];
    Sample[0] = tex2D(CShade_SampleColorTex, Input.Tex0.xy);
    Sample[1] = tex2D(CShade_SampleColorTex, Input.Tex1.xy);
    Sample[2] = tex2D(CShade_SampleColorTex, Input.Tex2.xy);
    Sample[3] = tex2D(CShade_SampleColorTex, Input.Tex0.xz);
    Sample[4] = tex2D(CShade_SampleColorTex, Input.Tex1.xz);
    Sample[5] = tex2D(CShade_SampleColorTex, Input.Tex2.xz);
    Sample[6] = tex2D(CShade_SampleColorTex, Input.Tex0.xw);
    Sample[7] = tex2D(CShade_SampleColorTex, Input.Tex1.xw);
    Sample[8] = tex2D(CShade_SampleColorTex, Input.Tex2.xw);
    return Med9(Sample[0], Sample[1], Sample[2],
                Sample[3], Sample[4], Sample[5],
                Sample[6], Sample[7], Sample[8]);
}

technique CShade_Median
{
    pass
    {
        SRGBWriteEnable = WRITE_SRGB;

        VertexShader = VS_Median;
        PixelShader = PS_Median;
    }
}
