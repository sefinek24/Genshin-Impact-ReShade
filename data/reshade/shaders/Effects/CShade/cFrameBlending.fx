
#include "shared/cGraphics.fxh"

uniform float _BlendFactor <
    ui_label = "Blend Factor";
    ui_type = "slider";
    ui_min = 0.0;
    ui_max = 1.0;
> = 0.5;

CREATE_TEXTURE(BlendTex, BUFFER_SIZE_0, RGBA8, 1)

CREATE_SRGB_SAMPLER(SampleBlendTex, BlendTex, 1, CLAMP)

// Pixel shaders

float4 PS_Blend(VS2PS_Quad Input) : SV_TARGET0
{
    // Copy backbuffer to a that continuously blends with its previous result 
    return float4(tex2D(CShade_SampleColorTex, Input.Tex0).rgb, _BlendFactor);
}

float4 PS_Display(VS2PS_Quad Input) : SV_TARGET0
{
    // Display the buffer
    return tex2D(SampleBlendTex, Input.Tex0);
}

technique CShade_FrameBlending
{
    pass
    {
        ClearRenderTargets = FALSE;
        BlendEnable = TRUE;
        BlendOp = ADD;
        SrcBlend = INVSRCALPHA;
        DestBlend = SRCALPHA;
        SRGBWriteEnable = WRITE_SRGB;

        VertexShader = VS_Quad;
        PixelShader = PS_Blend;
        RenderTarget0 = BlendTex;
    }

    pass
    {
        SRGBWriteEnable = WRITE_SRGB;

        VertexShader = VS_Quad;
        PixelShader = PS_Display;
    }
}
