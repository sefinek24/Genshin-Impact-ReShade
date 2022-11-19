//Made by originalnicdor. 
//Check for updates here: https://github.com/originalnicodr/CorgiFX
//If you want to use multiple instances of the shader you have to rename the namespace and the name of the technique

	  ////////////
	 /// MENU ///
	////////////

namespace ColorMask
{
#include "ReShadeUI.fxh"
#include "ReShade.fxh"

uniform bool axisHueSelectON <
	ui_category = "Eyedropper";
    ui_category_closed = true;
	ui_label = "Sample Hue from";
> = false;

uniform bool axisLumaSelectON <
	ui_category = "Eyedropper";
	ui_label = "Sample Luma";
> = false;

uniform bool drawColorSelectON <
    ui_category = "Eyedropper";
	ui_label = "Draw eyedropper position";
    ui_tooltip = "Only visible if the eyedropper is used";
> = false;

uniform float2 axisColorSelectAxis <
    ui_category = "Eyedropper";
	ui_label = "Color eyedropper position";
	ui_type = "drag";
	ui_step = 0.001;
	ui_min = 0.000; ui_max = 1.000;
> = float2(0.5, 0.5);

uniform float3 axisDebugColor < 
    ui_category = "Eyedropper";
    ui_label = "Color of eyedropper axis";
    ui_type = "color";
> = float3(1.0, 0.0, 0.0);

uniform int MaskType <
    ui_category = "Masks settings";
    ui_type = "combo";
    ui_label = "Mask Type";
    ui_items = "Hue\0Luma\0Hue & Luma\0";
> = 2;

uniform int BlendM <
    ui_category = "Masks settings";
    ui_type = "combo";
    ui_label = "Blending Mode";
    ui_tooltip = "Select the blending mode used for both chroma and lunma masks to interct between them.";
    ui_items = "Add\0Multiply\0";
> = 0;

uniform float blackLevel <
    ui_category = "Masks settings";
    ui_tooltip = "Additional black level control over the final mask.";
    ui_label = "Black level";
    ui_type = "drag";
    ui_min = 0.0; ui_max = 1.0;
    ui_step = 0.001;
> = 0;

uniform float whiteLevel <
    ui_category = "Masks settings";
        ui_tooltip = "Additional white level control over the final mask.";
    ui_label = "White level";
    ui_type = "drag";
    ui_min = 0.0; ui_max = 1.0;
    ui_step = 0.001;
> = 1;

uniform bool MaskAfterEffects <
    ui_category = "Masks settings";
    ui_label = "Mask after effects are applied";
	ui_tooltip = "Apply the mask based on the resulting colors of applying the shaders between \nBeforeColorMask and AfterColorMask, instead of before.";
> = false;

uniform float hueTarget <
    ui_category = "Hue Mask";
    ui_type = "drag";
    ui_label = "Hue Target";
    ui_min = 0.0; ui_max = 1.0;
    ui_step = 0.001;
> = 0.0;

uniform float hueRange <
    ui_category = "Hue Mask";
    ui_type = "drag";
    ui_label = "Hue Range";
    ui_min = 0.0; ui_max = 1.0;
    ui_step = 0.001;
> = 0;

uniform float hueStep <
    ui_category = "Hue Mask";
    ui_type = "drag";
    ui_label = "Hue Step Smoothness";
    ui_min = 0.001; ui_max = 1.0;
    ui_step = 0.001;
> = 0.05;

uniform float hueMaskStrength <
    ui_category = "Hue Mask";
    ui_type = "drag";
    ui_label = "Hue Mask Opacity";
    ui_min = 0.0; ui_max = 1.0;
    ui_step = 0.001;
> = 1.0;

uniform bool InvertHueMask <
    ui_category = "Hue Mask";
    ui_label = "Invert hue mask";
> = false;

uniform float lumaTarget <
    ui_category = "Luma Mask";
    ui_type = "drag";
        ui_label = "Luma Target";
    ui_min = 0.0; ui_max = 1.0;
    ui_step = 0.001;
> = 0.0;

uniform float lumaRange <
    ui_category = "Luma Mask";
    ui_type = "drag";
    ui_label = "Luma Range";
    ui_min = 0.0; ui_max = 1.0;
    ui_step = 0.001;
> = 0.1;

uniform float lumaStep <
    ui_category = "Luma Mask";
    ui_type = "drag";
    ui_label = "Luma Step Smoothness";
    ui_min = 0.001; ui_max = 1.0;
    ui_step = 0.001;
> = 0.1;

uniform float lumaMaskStrength <
    ui_category = "Luma Mask";
    ui_type = "drag";
    ui_label = "Luma Mask Opacity";
    ui_min = 0.0; ui_max = 1.0;
    ui_step = 0.001;
> = 1.0;

uniform bool InvertLumaMask <
    ui_category = "Luma Mask";
    ui_label = "Invert luma mask";
> = false;

uniform bool showMask <
    ui_category = "Debug";
    ui_category_closed = true;
    ui_label = "Show Mask";
> = false;

uniform bool showDebugOverlay <
    ui_category = "Debug";
    ui_label = "Show Debug histogram";
> = false;

uniform float2 fUIOverlayPosTwo <
    ui_category = "Debug";
    ui_type = "slider";
    ui_label = "Overlay: Position";
    ui_min = 0.0; ui_max = 1.0;
    ui_step = 0.01;
> = float2(0.0, 0.0);

uniform int2 iUIOverlaySizeTwo <
    ui_category = "Debug";
    ui_type = "slider";
    ui_label = "Overlay: Size";
    ui_tooltip = "x: width\nz: height";
    ui_min = 50; ui_max = BUFFER_WIDTH;
    ui_step = 1;
> = int2(600, 100);

uniform float fUIOverlayOpacityTwo <
    ui_category = "Debug";
    ui_type = "slider";
    ui_label = "Overlay Opacity";
    ui_min = 0.0; ui_max = 1.0;
    ui_step = 0.01;
> = 1.0;

// First pass render target
texture BeforeTarget { Width = BUFFER_WIDTH; Height = BUFFER_HEIGHT; };
sampler BeforeSampler { Texture = BeforeTarget; };


	  /////////////////
	 /// FUNCTIONS ///
	/////////////////

//Original function made by prod80
float3 RGBToHCV( in float3 RGB )
{
    // Based on work by Sam Hocevar and Emil Persson
    float4 P         = ( RGB.g < RGB.b ) ? float4( RGB.bg, -1.0f, 2.0f/3.0f ) : float4( RGB.gb, 0.0f, -1.0f/3.0f );
    float4 Q1        = ( RGB.r < P.x ) ? float4( P.xyw, RGB.r ) : float4( RGB.r, P.yzx );
    float C          = Q1.x - min( Q1.w, Q1.y );
    float H          = abs(( Q1.w - Q1.y ) / ( 6.0f * C + 0.000001f ) + Q1.z );
    return float3( H, C, Q1.x );
}

float3 RGBToHSL( in float3 RGB )
{
    RGB.xyz          = max( RGB.xyz, 0.000001f );
    float3 HCV       = RGBToHCV(RGB);
    float L          = HCV.z - HCV.y * 0.5f;
    float S          = HCV.y / ( 1.0f - abs( L * 2.0f - 1.0f ) + 0.000001f);
    return float3( HCV.x, S, L );
}

float hue_smoothstep (float target, float range, float step, float x)
{

    float curvemin1 = target - range - step;
    float curvemax1 = target - range + step;
    float curvemin2 = target + range + step;
    float curvemax2 = target + range - step;

    if (curvemax2 < curvemax1){
        curvemax1 = curvemax2 = target;
    }

    float curve1 = smoothstep(curvemin1, curvemax1, x);
    float curve2 = smoothstep(curvemin2, curvemax2, x);
    float r = min(curve1, curve2);

    if (curvemin1 < 0){
        float curveminloop = 1 + curvemin1%1;
        float curvemaxloop = 1 + curvemax1;

        float rloop = smoothstep(curveminloop, curvemaxloop, x);
        r = max(r,rloop);
    }

    if (curvemin2 > 1){
        float curveminloop = curvemin2 - 1;
        float curvemaxloop = curvemax2 - 1;

        float rloop = smoothstep(curveminloop, curvemaxloop, x);
        r = max(r,rloop);
    }

    return r;
}


float hue_mask(float pixelHue){
    float target;

	if (axisHueSelectON) {
		float3 coloraxis = tex2D(BeforeSampler, axisColorSelectAxis).rgb;
		target = RGBToHCV(coloraxis).x;
	}
	else{
		target = hueTarget;
	}

    float result = hue_smoothstep(target, hueRange, hueStep, pixelHue);
    result = InvertHueMask ? 1- result : result;
    return result*hueMaskStrength;
}

float luma_mask(float pixelLuma){
    float target;

	if (axisLumaSelectON) {
		float3 coloraxis = tex2D(BeforeSampler, axisColorSelectAxis).rgb;
		target = RGBToHSL(coloraxis).z;
	}
	else{
		target = lumaTarget;
	}

    float curvemin1 = target - lumaRange - lumaStep;
    float curvemax1 = target - lumaRange + lumaStep;
    float curvemin2 = target + lumaRange + lumaStep;
    float curvemax2 = target + lumaRange - lumaStep;

    if (curvemax2 < curvemax1){
        curvemax1 = curvemax2 = target;
    }

    float curve1 = smoothstep(curvemin1, curvemax1, pixelLuma);
    float curve2 = smoothstep(curvemin2, curvemax2, pixelLuma);
    float result = min(curve1, curve2);
    result = InvertLumaMask ? 1 - result : result;
    return result*lumaMaskStrength;
}

float segment(float2 P, float2 A, float2 B, float r) 
{
    float2 g = B - A;
    float2 h = P - A;
    float d = length(h - g * clamp(dot(g, h) / dot(g,g), 0.0, 1.0));
    return smoothstep(r, 0.5*r, d);
}

float blendMasks(float hueMask, float lumaMask){
	float m;
    switch(BlendM){
        case 0:{m = hueMask + lumaMask;break;}
        case 1:{m = hueMask * lumaMask;break;}
    }
    return m;
}

float MapTwo(float value, float2 span_old, float2 span_new) {
	float span_old_diff;
    if (abs(span_old.y - span_old.x) < 1e-6)
		span_old_diff = 1e-6;
	else
		span_old_diff = span_old.y - span_old.x;
    return lerp(span_new.x, span_new.y, (clamp(value, span_old.x, span_old.y)-span_old.x)/(span_old_diff));
}

float3 HSVtoRGBTwo(float3 c) {
    const float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    const float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
}

// Debugger overlay from ColorIsolation by Daodan317081, credits to him.
float3 DrawDebugOverlayTwo(float3 background, float2 pos, int2 size, float opacity, int2 vpos, float2 texcoord) {
    float x, y, hue_value, luma_value, luma;
    float3 overlay, hsvStrip;

	const float2 overlayPos = pos * (ReShade::ScreenSize - size);

    if(all(vpos.xy >= overlayPos) && all(vpos.xy < overlayPos + size))
    {
        x = MapTwo(texcoord.x, float2(overlayPos.x, overlayPos.x + size.x) / BUFFER_WIDTH, float2(0.0, 1.0));
        y = MapTwo(texcoord.y, float2(overlayPos.y, overlayPos.y + size.y) / BUFFER_HEIGHT, float2(0.0, 1.0));
        hsvStrip = HSVtoRGBTwo(float3(x, 1.0, 1.0));
        luma = dot(hsvStrip, float3(0.2126, 0.7151, 0.0721));

        hue_value = MaskType != 1 ? hue_mask(x): 0;
        overlay = lerp(luma.rrr, hsvStrip, hue_value);

        luma_value = MaskType > 0 ? luma_mask(y): 0;
        if (y < blackLevel || y > whiteLevel){
            luma_value = 0;
        }

        overlay = lerp(overlay, float3(1,1,1), luma_value);
        
        overlay = lerp(overlay, 0.0.rrr, exp(-size.y * length(float2(x, 1.0 - y) - float2(x, hue_value))));
        background = lerp(background, overlay, opacity);
        
    }

    return background;
}

void BeforePS(float4 vpos : SV_Position, float2 texcoord : TEXCOORD, out float3 Image : SV_Target)
{
	// Grab screen texture
	Image = tex2D(ReShade::BackBuffer, texcoord).rgb;
}

float3 AfterPS(float4 vpos : SV_Position, float2 texcoord : TEXCOORD) : COLOR
{
    float3 beforeMaskColor = MaskAfterEffects ? tex2D(ReShade::BackBuffer, texcoord).rgb : tex2D(BeforeSampler, texcoord).rgb;
    float3 pixelHSL = RGBToHSL(beforeMaskColor);
    float mask;

    switch(MaskType){
        case 0:{mask = hue_mask(pixelHSL.x);break;}
        case 1:{mask = luma_mask(pixelHSL.z);break;}
        case 2:{mask = blendMasks(hue_mask(pixelHSL.x), luma_mask(pixelHSL.z));break;}
    }

    if (pixelHSL.z < blackLevel || pixelHSL.z > whiteLevel){
        mask = 0;
    }

    float3 color = lerp(beforeMaskColor, tex2D(ReShade::BackBuffer, texcoord).rgb, mask);

    //Debug stuff

    if(showMask){
        color = float3(mask, mask, mask);
    }

	if(drawColorSelectON){
        float xAxis = segment(texcoord, float2(axisColorSelectAxis.x, 0), float2(axisColorSelectAxis.x, 1), 0.001);
        float yAxis = segment(texcoord, float2(0, axisColorSelectAxis.y), float2(1, axisColorSelectAxis.y), 0.001);
        color = lerp(color, axisDebugColor, xAxis + yAxis);
	}

    if (showDebugOverlay){
        color = DrawDebugOverlayTwo(color, fUIOverlayPosTwo, iUIOverlaySizeTwo, fUIOverlayOpacityTwo, vpos.xy, texcoord);
    }

    return color;
}

	  //////////////
	 /// OUTPUT ///
	//////////////

technique BeforeColorMask < ui_tooltip = "Place this technique before the shaders you want to mask"; >
{
	pass
	{
		VertexShader = PostProcessVS;
		PixelShader = BeforePS;
		RenderTarget = BeforeTarget;
	}
}
technique AfterColorMask < ui_tooltip = "Place this technique after the shaders you want to mask"; >
{
	pass
	{
		VertexShader = PostProcessVS;
		PixelShader = AfterPS;
	}
}

}