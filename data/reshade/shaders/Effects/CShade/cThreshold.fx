
#include "shared/cGraphics.fxh"

uniform float _Threshold <
    ui_type = "drag";
    ui_min = 0.0;
    ui_label = "Threshold";
> = 0.8;

uniform float _Smooth <
    ui_type = "drag";
    ui_min = 0.0;
    ui_label = "Smoothing";
> = 0.5;

uniform float _Saturation <
    ui_type = "drag";
    ui_min = 0.0;
    ui_label = "Saturation";
> = 1.0;

uniform float _Intensity <
    ui_type = "drag";
    ui_min = 0.0;
    ui_label = "Intensity";
> = 1.0;

// Vertex shaders

float Med3(float x, float y, float z)
{
    return max(min(x, y), min(max(x, y), z));
}

// Pixel shaders

float4 PS_Threshold(VS2PS_Quad Input) : SV_TARGET0
{
    const float Knee = mad(_Threshold, _Smooth, 1e-5f);
    const float3 Curve = float3(_Threshold - Knee, Knee * 2.0, 0.25 / Knee);
    float4 Color = tex2D(SampleColorTex, Input.Tex0);

    // Under-threshold
    float Brightness = Med3(Color.r, Color.g, Color.b);
    float ResponseCurve = clamp(Brightness - Curve.x, 0.0, Curve.y);
    ResponseCurve = Curve.z * ResponseCurve * ResponseCurve;

    // Combine and apply the brightness response curve
    Color = Color * max(ResponseCurve, Brightness - _Threshold) / max(Brightness, 1e-10);
    Brightness = Med3(Color.r, Color.g, Color.b);
    return saturate(lerp(Brightness, Color, _Saturation) * _Intensity);
}

technique cThreshold
{
    pass
    {
        #if BUFFER_COLOR_BIT_DEPTH == 8
            SRGBWriteEnable = TRUE;
        #endif

        VertexShader = VS_Quad;
        PixelShader = PS_Threshold;
    }
}