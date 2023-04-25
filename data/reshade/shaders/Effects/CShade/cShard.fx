
#include "shared/cGraphics.fxh"

uniform float _Weight <
    ui_type = "drag";
> = 1.0;

struct VS2PS_Shard
{
    float4 HPos : SV_POSITION;
    float4 Tex0 : TEXCOORD0;
    float4 Tex1 : TEXCOORD1;
};

VS2PS_Shard VS_Shard(APP2VS Input)
{
    float2 PixelSize = float2(1.0 / int2(BUFFER_WIDTH, BUFFER_HEIGHT));

    VS2PS_Quad FSQuad = VS_Quad(Input);
    VS2PS_Shard Output;

    Output.HPos = FSQuad.HPos;
    Output.Tex0 = FSQuad.Tex0.xyxy;
    Output.Tex1 = FSQuad.Tex0.xyxy + float4(-PixelSize, PixelSize);
    return Output;
}

float4 PS_Shard(VS2PS_Shard Input) : SV_TARGET0
{
    float4 OriginalSample = tex2D(SampleColorTex, Input.Tex0.xy);
    float4 BlurSample = 0.0;
    BlurSample += tex2D(SampleColorTex, Input.Tex1.xw) * 0.25;
    BlurSample += tex2D(SampleColorTex, Input.Tex1.zw) * 0.25;
    BlurSample += tex2D(SampleColorTex, Input.Tex1.xy) * 0.25;
    BlurSample += tex2D(SampleColorTex, Input.Tex1.zy) * 0.25;
    return OriginalSample + (OriginalSample - BlurSample) * _Weight;
}

technique cShard
{
    pass
    {
        #if BUFFER_COLOR_BIT_DEPTH == 8
            SRGBWriteEnable = TRUE;
        #endif

        VertexShader = VS_Shard;
        PixelShader = PS_Shard;
    }
}
