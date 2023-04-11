#include "Reshade.fxh"

namespace DHUniformityCorrection {

//// uniform

	uniform bool bSolidColor <
	    ui_category = "Debug";
		ui_label = "Solid color";
	> = false;
	
	uniform float fSolidColor <
	    ui_category = "Debug";
		ui_label = "Brightness";
		ui_type = "slider";
	    ui_min = 0.0;
	    ui_max = 1.0;
	    ui_step = 0.1;
	> = 0.5;
	
	uniform bool bGreuScale <
	    ui_category = "Correction";
		ui_label = "Grey scale";
	> = true;
	
	uniform float fCorrection <
	    ui_category = "Correction";
		ui_label = "Correction strength";
		ui_type = "slider";
	    ui_min = 0.0;
	    ui_max = 1.0;
	    ui_step = 0.05;
	> = 0.25;

	uniform int iMethod <
        ui_category = "Correction";
        ui_type = "combo";
        ui_label = "Method";
        ui_items = "Additive\0Additive normalized\0Brightness proportional\0";
    > = 0;

//// textures

	texture uniformityDefectTex < source = "dh_uniformity_correction.bmp"; > { Width = BUFFER_WIDTH; Height = BUFFER_HEIGHT; };
	sampler uniformityDefectSampler { Texture = uniformityDefectTex; };

//// Functions


//// PS

	void PS_Correction(float4 vpos : SV_Position, in float2 coords : TEXCOORD0, out float4 outPixel : SV_Target)
	{
		float3 color;
		if(bSolidColor) {
			color = fSolidColor;
		} else {
			color = tex2D(ReShade::BackBuffer,coords).rgb;
		}
		
		float3 defect = tex2D(uniformityDefectSampler,coords).rgb;
		if(bGreuScale) {
			defect = max(defect.r,max(defect.g,defect.b));
		}
	
		if(iMethod==0) {
			outPixel = float4(saturate(color*(1.0+fCorrection*(1.0-defect))),1.0);
		} else if(iMethod==1) {
			outPixel = float4(color*(1.0+fCorrection*(1.0-defect))/(1.0+fCorrection),1.0);
		} else if(iMethod==2) {
			float brightness = max(color.r,max(color.g,color.b));
			outPixel = float4(saturate(color*(1.0+fCorrection*(1.0-defect)*(1.0-brightness))),1.0);
		}
		
	}

//// Techniques

	technique DHUniformityCorrection <
	>
	{
		pass
		{
			VertexShader = PostProcessVS;
			PixelShader = PS_Correction;
		}
	}

}