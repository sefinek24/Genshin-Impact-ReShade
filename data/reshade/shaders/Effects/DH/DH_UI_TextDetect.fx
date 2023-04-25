#include "Reshade.fxh"

namespace DH_UI_TextDetect {

// Uniforms
uniform int framecount < source = "framecount"; >;

uniform int iTextMaxThickness <
    ui_category = "Detection";
	ui_label = "Text max thicnkess";
	ui_type = "slider";
    ui_min = 1;
    ui_max = 16;
    ui_step = 1;
> = 3;

uniform float fMinTextBrightness <
    ui_category = "Detection";
	ui_label = "Min text brightness";
	ui_type = "slider";
    ui_min = 0.0;
    ui_max = 1.0;
    ui_step = 0.01;
> = 0.4;

uniform float fMaxBlackBrightness <
    ui_category = "Detection";
	ui_label = "Max black brightness";
	ui_type = "slider";
    ui_min = 0.0;
    ui_max = 1.0;
    ui_step = 0.01;
> = 0.1;

uniform int iTextOutline <
    ui_category = "Detection";
	ui_label = "Text outline";
	ui_type = "slider";
    ui_min = 0.0;
    ui_max = 8;
    ui_step = 1;
> = 1;

uniform bool bBrightnessAsRestore <
    ui_category = "Detection";
	ui_label = "Restore brightness";
> = true;


uniform bool bDebug = false;


// Textures

	texture dh_ui_savedTex { Width = BUFFER_WIDTH; Height = BUFFER_HEIGHT; };
	sampler dh_ui_savedSampler { Texture = dh_ui_savedTex; };

	texture dh_ui_textTex { Width = BUFFER_WIDTH; Height = BUFFER_HEIGHT; Format = R8; };
	sampler dh_ui_textSampler { Texture = dh_ui_textTex; };


// Functions

	bool inScreen(float v) {
		return v>=0 && v<=1;
	}

	bool inScreen(float2 coords) {
		return inScreen(coords.x) && inScreen(coords.y);
	}

	float getBrightness(float3 color) {
		return max(max(color.r,color.g),color.b);
	}

	float isText(float2 coords,float3 color) {
		float brightness = getBrightness(color);
		bool isLight =  brightness>=fMinTextBrightness;
		if(!isLight) {
			return 0.0;
		}

		float2 delta = 0.0;
		float maxDist2 = iTextMaxThickness*iTextMaxThickness;

		[loop]
		for(delta.x=-iTextMaxThickness;delta.x<=iTextMaxThickness;delta.x+=1.0) {
			for(delta.y=-iTextMaxThickness;delta.y<=iTextMaxThickness;delta.y+=1.0) {
				float dist2 = dot(delta,delta);
				if(dist2<=maxDist2) {
					float2 searchCoord = coords + delta*ReShade::PixelSize;
					if(inScreen(searchCoord)) {
						float3 searchColor = tex2Dlod(ReShade::BackBuffer,float4(searchCoord,0,0)).rgb;
						float searchBrightness = getBrightness(searchColor);
						if(searchBrightness<=fMaxBlackBrightness) return bBrightnessAsRestore?brightness:1.0;						
					}
				}
			}
		}

		return 0.0;
	}


// Pixel shaders

	void PS_save(in float4 position : SV_Position, in float2 coords : TEXCOORD, out float4 outText : SV_Target, out float4 outSaved : SV_Target1)
	{
		float4 sourceColor = tex2D(ReShade::BackBuffer,coords);
		outText = isText(coords,sourceColor.rgb);
		outSaved = sourceColor;
	}

	void PS_restore(in float4 position : SV_Position, in float2 coords : TEXCOORD, out float4 outPixel : SV_Target)
	{
		float text = tex2D(dh_ui_textSampler,coords).r;
		if(text==0 && iTextOutline>0) {
			float2 delta = 0;
			float maxDist2 = iTextOutline*iTextOutline;
			float dist = maxDist2+1;
			
			[loop]
			for(delta.x=-iTextOutline;delta.x<=iTextOutline;delta.x+=1.0) {
				for(delta.y=-iTextOutline;delta.y<=iTextOutline;delta.y+=1.0) {
					float dist2 = dot(delta,delta);
					if(dist2<=dist) {
						float2 searchCoord = coords + delta*ReShade::PixelSize;
						if(inScreen(searchCoord)) {
							float searchText = tex2Dlod(dh_ui_textSampler,float4(searchCoord,0,0)).r;
							if(searchText>0) {
								dist = dist2;
							}
						}
					}
				}
			}
			if(dist<maxDist2+1) {
				text = 1.0 - pow(dist/maxDist2,0.5);
			}
		}
		if(bDebug) { 
			outPixel = float4(text,0,0,1);
			return;
		}
		float3 color = tex2D(ReShade::BackBuffer,coords).rgb;
		float3 saved = tex2D(dh_ui_savedSampler,coords).rgb;
		outPixel = float4(color*(1.0-text)+saved*text,1.0);
	}
	
// Techniques

	technique DH_UI_TextDetect_before <
	>
	{
		pass
		{
			VertexShader = PostProcessVS;
			PixelShader = PS_save;
			RenderTarget = dh_ui_textTex;
			RenderTarget1 = dh_ui_savedTex;
		}
	}

	technique DH_UI_TextDetect_after <
	>
	{
		pass
		{
			VertexShader = PostProcessVS;
			PixelShader = PS_restore;
		}
	}

}