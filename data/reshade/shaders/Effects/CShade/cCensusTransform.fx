
#include "shared/cGraphics.fxh"

struct VS2PS_Census
{
    float4 HPos : SV_POSITION;
    float4 Tex0 : TEXCOORD0;
    float4 Tex1 : TEXCOORD1;
    float4 Tex2 : TEXCOORD2;
};

VS2PS_Census VS_Census(APP2VS Input)
{
    // Sample locations:
    // [0].xy [1].xy [2].xy
    // [0].xz [1].xz [2].xz
    // [0].xw [1].xw [2].xw

    const float2 PixelSize = 1.0 / float2(BUFFER_WIDTH, BUFFER_HEIGHT);

    // Get fullscreen texcoord and vertex position
    VS2PS_Quad FSQuad = VS_Quad(Input);

    VS2PS_Census Output;
    Output.HPos = FSQuad.HPos;
    Output.Tex0 = FSQuad.Tex0.xyyy + (float4(-1.0, 1.0, 0.0, -1.0) * PixelSize.xyyy);
    Output.Tex1 = FSQuad.Tex0.xyyy + (float4(0.0, 1.0, 0.0, -1.0) * PixelSize.xyyy);
    Output.Tex2 = FSQuad.Tex0.xyyy + (float4(1.0, 1.0, 0.0, -1.0) * PixelSize.xyyy);
    return Output;
}

float GetGreyScale(float4 Color)
{
    return max(max(Color.r, Color.g), Color.b);
}

float4 PS_Census(VS2PS_Census Input) : SV_TARGET0
{
    float4 OutputColor0 = 0.0;
    const int Neighbors = 8;
    float SampleNeighbor[Neighbors];

    float CenterSample = GetGreyScale(tex2D(SampleColorTex, Input.Tex1.xz));
    SampleNeighbor[0] = GetGreyScale(tex2D(SampleColorTex, Input.Tex0.xy));
    SampleNeighbor[1] = GetGreyScale(tex2D(SampleColorTex, Input.Tex1.xy));
    SampleNeighbor[2] = GetGreyScale(tex2D(SampleColorTex, Input.Tex2.xy));
    SampleNeighbor[3] = GetGreyScale(tex2D(SampleColorTex, Input.Tex0.xz));
    SampleNeighbor[4] = GetGreyScale(tex2D(SampleColorTex, Input.Tex2.xz));
    SampleNeighbor[5] = GetGreyScale(tex2D(SampleColorTex, Input.Tex0.xw));
    SampleNeighbor[6] = GetGreyScale(tex2D(SampleColorTex, Input.Tex1.xw));
    SampleNeighbor[7] = GetGreyScale(tex2D(SampleColorTex, Input.Tex2.xw));

    // Generate 8-bit integer from the 8-pixel neighborhood
    for(int i = 0; i < Neighbors; i++)
    {
        float Comparison = step(SampleNeighbor[i], CenterSample);
        OutputColor0 += ldexp(Comparison, i);
    }

	// Convert the 8-bit integer to float, and average the results from each channel
    return OutputColor0 * (1.0 / (exp2(8) - 1));
}

technique cCensusTransform
{
    pass
    {
        #if BUFFER_COLOR_BIT_DEPTH == 8
            SRGBWriteEnable = TRUE;
        #endif

        VertexShader = VS_Census;
        PixelShader = PS_Census;
    }
}
