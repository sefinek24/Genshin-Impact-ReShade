
#include "shared/cGraphics.fxh"
#include "shared/cImageProcessing.fxh"

uniform float _Sigma <
    ui_type = "drag";
    ui_min = 0.0;
> = 1.0;

float4 GetGaussianBlur(float2 Tex, bool IsHorizontal)
{
    float2 Direction = IsHorizontal ? float2(1.0, 0.0) : float2(0.0, 1.0);
    float2 PixelSize = (1.0 / float2(BUFFER_WIDTH, BUFFER_HEIGHT)) * Direction;
    float KernelSize = _Sigma * 3.0;

    if(_Sigma == 0.0)
    {
        return tex2Dlod(SampleColorTex, float4(Tex, 0.0, 0.0));
    }
    else
    {
        // Sample and weight center first to get even number sides
        float TotalWeight = GetGaussianWeight(0.0, _Sigma);
        float4 OutputColor = tex2D(SampleColorTex, Tex) * TotalWeight;

        for(float i = 1.0; i < KernelSize; i += 2.0)
        {
            float LinearWeight = 0.0;
            float LinearOffset = GetGaussianOffset(i, _Sigma, LinearWeight);
            OutputColor += tex2Dlod(SampleColorTex, float4(Tex - LinearOffset * PixelSize, 0.0, 0.0)) * LinearWeight;
            OutputColor += tex2Dlod(SampleColorTex, float4(Tex + LinearOffset * PixelSize, 0.0, 0.0)) * LinearWeight;
            TotalWeight += LinearWeight * 2.0;
        }

        // Normalize intensity to prevent altered output
        return OutputColor / TotalWeight;
    }
}

float4 PS_HGaussianBlur(VS2PS_Quad Input) : SV_TARGET0
{
    return GetGaussianBlur(Input.Tex0, true);
}

float4 PS_VGaussianBlur(VS2PS_Quad Input) : SV_TARGET0
{
    return GetGaussianBlur(Input.Tex0, false);
}

technique cHorizontalBlur
{
    pass
    {
        #if BUFFER_COLOR_BIT_DEPTH == 8
            SRGBWriteEnable = TRUE;
        #endif

        VertexShader = VS_Quad;
        PixelShader = PS_HGaussianBlur;
    }
}

technique cVerticalBlur
{
    pass
    {
        #if BUFFER_COLOR_BIT_DEPTH == 8
            SRGBWriteEnable = TRUE;
        #endif

        VertexShader = VS_Quad;
        PixelShader = PS_VGaussianBlur;
    }
}
