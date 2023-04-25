
#include "shared/cMacros.fxh"
#include "shared/cGraphics.fxh"

uniform float _TimeRate <
    ui_label = "Smoothing";
    ui_type = "drag";
    ui_tooltip = "Exposure time smoothing";
    ui_min = 0.0;
    ui_max = 1.0;
> = 0.95;

uniform float _ManualBias <
    ui_label = "Exposure";
    ui_type = "drag";
    ui_tooltip = "Optional manual bias ";
    ui_min = 0.0;
> = 2.0;

CREATE_TEXTURE(LumaTex, int2(256, 256), 9, R16F)

CREATE_SAMPLER(SampleLumaTex, LumaTex, LINEAR, CLAMP)

// Pixel shaders
// TODO: Add average, spot, and center-weighted metering with adjustable radius and slope

float4 PS_Blit(VS2PS_Quad Input) : SV_TARGET0
{
    float4 Color = tex2D(SampleColorTex, Input.Tex0);

    // OutputColor0.rgb = Output the highest brightness out of red/green/blue component
    // OutputColor0.a = Output the weight for temporal blending
    return float4(max(Color.r, max(Color.g, Color.b)).rrr, _TimeRate);
}

float4 PS_Exposure(VS2PS_Quad Input) : SV_TARGET0
{
    // Average Luma = Average value (1x1) for all of the pixels
    float AverageLuma = tex2Dlod(SampleLumaTex, float4(Input.Tex0, 0.0, 8.0)).r;
    float4 Color = tex2D(SampleColorTex, Input.Tex0);

    // KeyValue is an exposure compensation curve
    // Source: https://knarkowicz.wordpress.com/2016/01/09/automatic-exposure/
    float KeyValue = 1.03 - (2.0 / (log10(AverageLuma + 1.0) + 2.0));
    float ExposureValue = log2(KeyValue / AverageLuma) + _ManualBias;
    return Color * exp2(ExposureValue);
}

technique cAutoExposure
{
    pass
    {
        ClearRenderTargets = FALSE;
        BlendEnable = TRUE;
        BlendOp = ADD;
        SrcBlend = INVSRCALPHA;
        DestBlend = SRCALPHA;

        VertexShader = VS_Quad;
        PixelShader = PS_Blit;
        RenderTarget = LumaTex;
    }

    pass
    {
        #if BUFFER_COLOR_BIT_DEPTH == 8
            SRGBWriteEnable = TRUE;
        #endif

        VertexShader = VS_Quad;
        PixelShader = PS_Exposure;
    }
}
