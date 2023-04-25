
namespace SumAbsoluteDifferences
{
    #include "shared/cMacros.fxh"
    #include "shared/cGraphics.fxh"

    CREATE_TEXTURE(CurrentTex, BUFFER_SIZE_0, R8, 1)
    CREATE_SAMPLER(SampleCurrentTex, CurrentTex, LINEAR, CLAMP)

    CREATE_TEXTURE(PreviousTex, BUFFER_SIZE_0, R8, 1)
    CREATE_SAMPLER(SamplePreviousTex, PreviousTex, LINEAR, CLAMP)

    // Vertex shaders

    struct VS2PS_SAD
    {
        float4 HPos : SV_POSITION;
        float4 Tex0 : TEXCOORD0;
        float4 Tex1 : TEXCOORD1;
        float4 Tex2 : TEXCOORD2;
    };

    VS2PS_SAD VS_SAD(APP2VS Input)
    {
        float2 PixelSize = 1.0 / (float2(BUFFER_WIDTH, BUFFER_HEIGHT));

        VS2PS_Quad FSQuad = VS_Quad(Input);

        VS2PS_SAD Output;
        Output.HPos = FSQuad.HPos;
        Output.Tex0 = FSQuad.Tex0.xyyy + (float4(-1.0, 1.0, 0.0, -1.0) * PixelSize.xyyy);
        Output.Tex1 = FSQuad.Tex0.xyyy + (float4(0.0, 1.0, 0.0, -1.0) * PixelSize.xyyy);
        Output.Tex2 = FSQuad.Tex0.xyyy + (float4(1.0, 1.0, 0.0, -1.0) * PixelSize.xyyy);
        return Output;
    }

    // Pixel shaders

    float PS_Blit0(VS2PS_Quad Input) : SV_TARGET0
    {
        float3 Color = tex2D(SampleColorTex, Input.Tex0).rgb;
        return max(max(Color.r, Color.g), Color.b);
    }

    float4 PS_SAD(VS2PS_SAD Input) : SV_TARGET0
    {
        float4 OutputColor0 = 0.0;

        float2 SamplePos[9] =
        {
            Input.Tex0.xy, Input.Tex1.xy, Input.Tex2.xy,
            Input.Tex0.xz, Input.Tex1.xz, Input.Tex2.xz,
            Input.Tex0.xw, Input.Tex1.xw, Input.Tex2.xw
        };

        for(int i = 0; i < 9; i++)
        {
            float I0 = tex2D(SamplePreviousTex, SamplePos[i]).r;
            float I1 = tex2D(SampleCurrentTex, SamplePos[i]).r;
            float IT = I1 - I0;
            OutputColor0 += (abs(IT) * abs(IT));
        }

        return saturate(OutputColor0 / 9.0);
    }

    float4 PS_Blit1(VS2PS_Quad Input) : SV_TARGET0
    {
        return tex2D(SampleCurrentTex, Input.Tex0);
    }

    technique cSumSquaredDifferences
    {
        pass
        {
            VertexShader = VS_Quad;
            PixelShader = PS_Blit0;
            RenderTarget0 = CurrentTex;
        }

        pass
        {
            VertexShader = VS_SAD;
            PixelShader = PS_SAD;
            #if BUFFER_COLOR_BIT_DEPTH == 8
                SRGBWriteEnable = TRUE;
            #endif
        }

        pass
        {
            VertexShader = VS_Quad;
            PixelShader = PS_Blit1;
            RenderTarget0 = PreviousTex;
        }
    }
}
