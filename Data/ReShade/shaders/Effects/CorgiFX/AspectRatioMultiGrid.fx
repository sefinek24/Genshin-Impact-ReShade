// Based and inspired on AspectRatioComposition by Daodan317081 (https://github.com/Daodan317081/reshade-shaders)
// and AspectRatioSuite by luluco250 (https://github.com/luluco250/FXShaders/)

//Made by originalnicodr

namespace AspectRatioMultiGrid
{

#include "ReShade.fxh"

#ifndef CUSTOM_ASPECT_RATIO_MAX
	#define CUSTOM_ASPECT_RATIO_MAX 25
#endif


#ifndef ASPECT_RATIO_LIST_VALUES
#define ASPECT_RATIO_LIST_VALUES 2, 3/2., 4/3., 5/4., 21/9., 1, 4/5., 3/4., 2/3.
#endif


#ifndef ASPECT_RATIO_LIST_UI_VALUES
#define ASPECT_RATIO_LIST_UI_VALUES " 2\0 3/2\0 4/3\0 5/4\0 21/9\0 1\0 4/5\0 3/4\0 2/3\0"
#endif

static const float ASPECT_RATIOS[] = {ASPECT_RATIO_LIST_VALUES, 0.0};

/******************************************************************************
	Uniforms
******************************************************************************/

uniform int ARMode <
	ui_category = "Aspect Ratio";
	ui_label = "Aspect Ratio Mode";
	ui_tooltip = "Select the way you want to choose the aspect ratio being used.";
	ui_type = "combo";
	ui_items = "Off\0List\0Custom\0";
> = 0;

uniform int ARFromList<
	ui_category = "Aspect Ratio";
	ui_label = "Aspect Ratio from list";
	ui_tooltip = "To edit the values on this list change the preprocessor definitions \nfor 'ASPECT_RATIO_LIST_VALUES' and 'ASPECT_RATIO_LIST_UI_VALUES'. \nThe former should have the values separated by a comma \n(and if they are a division add a dot at the end to signal its a float) \nand the latter should have the values separated by a '\\0' and a space.";
	ui_type = "combo";
	ui_items = ASPECT_RATIO_LIST_UI_VALUES;
> = 0;

#ifdef CUSTOM_ASPECT_RATIO_FLOAT
uniform float fUIAspectRatio <
	ui_category = "Aspect Ratio";
	ui_label = "Custom Aspect Ratio";
	ui_tooltip = "To control aspect ratio with an int2\nremove 'CUSTOM_ASPECT_RATIO_FLOAT' from preprocessor";
	ui_type = "slider";
	ui_min = 0.0; ui_max = 25.0;
	ui_step = 0.01;
> = 1.0;
#else
uniform int2 iUIAspectRatio <
	ui_category = "Aspect Ratio";
	ui_label = "Custom Aspect Ratio";
	ui_tooltip = "To control aspect ratio with a float\nadd 'CUSTOM_ASPECT_RATIO_FLOAT' to preprocessor.\nOptional: 'CUSTOM_ASPECT_RATIO_MAX=xyz'";
	ui_type = "slider";
	ui_min = 0; ui_max = CUSTOM_ASPECT_RATIO_MAX;
> = int2(16, 9);
#endif

uniform float3 ARColor <
	ui_category = "Aspect Ratio";
	ui_label = "Bars Color";
    ui_type = "color";
> = float3(0.0, 0.0, 0.0);

uniform float AROpacity <
	ui_category = "Aspect Ratio";
	ui_label = "Bars Opacity";
    ui_type = "slider";
    ui_min = 0.0; ui_max = 1.0;
    ui_step = 0.01;
> = 1.0;

uniform float3 gridColor <
	ui_category = "Grid";
	ui_label = "Color";
    ui_type = "color";
> = float3(0.0, 0.0, 0.0);

uniform bool gridInverseColor <
	ui_category = "Grid";
	ui_label = "Grid Inverse Color";
	ui_tooltip = "Lets you inverse the color of the pixels behind the grid to \nfully contrast the grid against the game's screen.";
> = false;

uniform float gridOpacity <
	ui_category = "Grid";
	ui_label = "Opacity";
    ui_type = "slider";
    ui_min = 0.0; ui_max = 1.0;
    ui_step = 0.01;
> = 1.0;

uniform float  gridLinesWidth<
	ui_category = "Grid";
	ui_label = "Lines width";
	ui_min = 0.001; ui_max = 0.1;
	ui_type = "slider";
	ui_step = 0.001;
> = 0.001;

/*
uniform bool ResizeMode <
	ui_category = "Grid";
	ui_label = "Resize mode";
	ui_tooltip = "Resize mode: 0 is clamp to screen (so resizing of overlay, no golden ratio by definition),\n1: resize to either full with or full height while keeping aspect ratio: golden ratio by definition in lined area.";
> = true;
*/

uniform bool RuleofThirds <
	ui_category = "Grid";
	ui_label = "Rule of thirds";
> = false;

uniform bool RuleofFifths <
	ui_category = "Grid";
	ui_label = "Rule of fifths";
> = false;

uniform bool Triangles1 <
	ui_category = "Grid";
	ui_label = "Triangles - 1";
> = false;

uniform bool Triangles2 <
	ui_category = "Grid";
	ui_label = "Triangles - 2";
> = false;

uniform bool Diagonals <
	ui_category = "Grid";
> = false;

uniform bool Diamond <
	ui_category = "Grid";
> = false;

uniform bool Special1 <
	ui_category = "Grid";
	ui_label = "Crosses - 1";
> = false;

uniform bool Special2 <
	ui_category = "Grid";
	ui_label = "Crosses - 2";
> = false;

uniform bool Special3 <
	ui_category = "Grid";
	ui_label = "Crosses - 3";
> = false;

uniform bool FibonacciBottomRight <
	ui_category = "Grid";
	ui_label = "Golden Ratio - bottom right";
> = false;

uniform bool FibonacciBottomLeft <
	ui_category = "Grid";
	ui_label = "Golden Ratio - bottom left";
> = false;

uniform bool FibonacciTopRight <
	ui_category = "Grid";
	ui_label = "Golden Ratio - top right";
> = false;

uniform bool FibonacciTopLeft <
	ui_category = "Grid";
	ui_label = "Golden Ratio - top left";
> = false;

uniform bool CustomGrid <
	ui_category = "Custom Grid";
	ui_label = "Custom Grid";
	ui_category_closed = true;
> = false;

uniform float4 CustomGridLine1 <
	ui_category = "Custom Grid";
	ui_label = "Line 1";
	ui_type = "slider";
	ui_step = 0.001;
	ui_min = 0; ui_max = 1;
> = float4(0, 0, 0.667, 1);

uniform float4 CustomGridLine2 <
	ui_category = "Custom Grid";
	ui_label = "Line 2";
	ui_type = "slider";
	ui_step = 0.001;
	ui_min = 0; ui_max = 1;
> = float4(1, 0, 0.333, 1);

uniform float4 CustomGridLine3 <
	ui_category = "Custom Grid";
	ui_label = "Line 3";
	ui_type = "slider";
	ui_step = 0.001;
	ui_min = 0; ui_max = 1;
> = float4(1, 1, 0.333, 0);

uniform float4 CustomGridLine4 <
	ui_category = "Custom Grid";
	ui_label = "Line 4";
	ui_type = "slider";
	ui_step = 0.001;
	ui_min = 0; ui_max = 1;
> = float4(0, 1, 0.666, 0);

uniform bool UseWhiteBackground <
	ui_category = "Custom Grid";
	ui_label = "White Background";
	ui_tooltip = "If you need more than 4 lines for a custom grid you can quickly \nmake one by turning this on, taking a pic of the resulting image, \nusing that image as a custom grid, and combining it again \nwith other custom grid by changing the positions of the lines above.";
> = false;

uniform bool CustomGridImage <
	ui_category = "Custom Grid";
	ui_label = "Custom Grid Image";
	ui_tooltip = "To change the image being used change the 'customGrid.png' file on \nthe texture folder or change the name on the texture being used in \nthe preprocessor definition 'CUSTOM_GRID_IMAGE'. \nMake sure the image used has a white background and black lines.";
> = false;

/******************************************************************************
	Functions
******************************************************************************/

//Credits to gPlatl: https://www.shadertoy.com/view/MlcGDB
float segment(float2 P, float2 A, float2 B, float r) 
{
    float2 g = B - A;
    float2 h = P - A;
    float d = length(h - g * clamp(dot(g, h) / dot(g,g), 0.0, 1.0));
    return smoothstep(r, 0.5*r, d);
}

float triangles(float2 texcoord){
	float diagonal = segment(texcoord, float2(0, 0), float2(1, 1), gridLinesWidth);
    float line1 = segment(texcoord, float2(0, 1), float2(1/4., 1/4.), gridLinesWidth);
	float line2 = segment(texcoord, float2(1, 0), float2(3/4., 3/4.), gridLinesWidth);
	return diagonal + line1 + line2;
}

float diagonals(float2 texcoord){
	float diagonal1 = segment(texcoord, float2(0, 0), float2(1, 1), gridLinesWidth);
	float diagonal2 = segment(texcoord, float2(1, 0), float2(0, 1), gridLinesWidth);
	return diagonal1 + diagonal2;
}

float special(float2 texcoord){
	float d1 = segment(texcoord, float2(0, 0), float2(4/9., 1), gridLinesWidth);
	float d2 = segment(texcoord, float2(0, 1), float2(4/9., 0), gridLinesWidth);
	float d3 = segment(texcoord, float2(1, 0), float2(5/9., 1), gridLinesWidth);
	float d4 = segment(texcoord, float2(1, 1), float2(5/9., 0), gridLinesWidth);
	return d1 + d2 + d3 + d4;
}


float special2(float2 texcoord){
	float d1 = segment(texcoord, float2(0, 0), float2(0.5, 1), gridLinesWidth);
	float d2 = segment(texcoord, float2(0.5, 1), float2(1, 0), gridLinesWidth);
	float d3 = segment(texcoord, float2(0, 1), float2(0.5, 0), gridLinesWidth);
	float d4 = segment(texcoord, float2(0.5, 0), float2(1, 1), gridLinesWidth);
	return d1 + d2 + d3 + d4;
}

float special3(float2 texcoord){
	float d1 = segment(texcoord, float2(0, 0), float2(1, 0.5), gridLinesWidth);
	float d2 = segment(texcoord, float2(0, 0.5), float2(1, 0), gridLinesWidth);
	float d3 = segment(texcoord, float2(0, 0.5), float2(1, 1), gridLinesWidth);
	float d4 = segment(texcoord, float2(0, 1), float2(1, 0.5), gridLinesWidth);
	return d1 + d2 + d3 + d4;
}


float diamond(float2 texcoord){
	float d1 = segment(texcoord, float2(0, 0.5), float2(0.5, 1), gridLinesWidth);
	float d2 = segment(texcoord, float2(0.5, 1), float2(1, 0.5), gridLinesWidth);
	float d3 = segment(texcoord, float2(1, 0.5), float2(0.5, 0), gridLinesWidth);
	float d4 = segment(texcoord, float2(0.5, 0), float2(0, 0.5), gridLinesWidth);
	return d1 + d2 + d3 + d4;
}

float ruleOfThirds(float2 texcoord){
	float ly1 = segment(texcoord, float2(1/3., 0), float2(1/3., 1), gridLinesWidth);
	float ly2 = segment(texcoord, float2(2/3., 0), float2(2/3., 1), gridLinesWidth);
	float lx1 = segment(texcoord, float2(0, 1/3.), float2(1, 1/3.), gridLinesWidth);
	float lx2 = segment(texcoord, float2(0, 2/3.), float2(1, 2/3.), gridLinesWidth);
	return ly1 + ly2 + lx1 + lx2;
}

float ruleOfFifths(float2 texcoord){
	float ly1 = segment(texcoord, float2(1/5., 0), float2(1/5., 1), gridLinesWidth);
	float ly2 = segment(texcoord, float2(2/5., 0), float2(2/5., 1), gridLinesWidth);
	float ly3 = segment(texcoord, float2(3/5., 0), float2(3/5., 1), gridLinesWidth);
	float ly4 = segment(texcoord, float2(4/5., 0), float2(4/5., 1), gridLinesWidth);
	float lx1 = segment(texcoord, float2(0, 1/5.), float2(1, 1/5.), gridLinesWidth);
	float lx2 = segment(texcoord, float2(0, 2/5.), float2(1, 2/5.), gridLinesWidth);
	float lx3 = segment(texcoord, float2(0, 3/5.), float2(1, 3/5.), gridLinesWidth);
	float lx4 = segment(texcoord, float2(0, 4/5.), float2(1, 4/5.), gridLinesWidth);
	return ly1 + ly2 + ly3 + ly4 + lx1 + lx2 + lx3 + lx4;
}

float customGrid(float2 texcoord){
	float l1 = segment(texcoord, CustomGridLine1.zw, CustomGridLine1.xy, gridLinesWidth);
	float l2 = segment(texcoord, CustomGridLine2.zw, CustomGridLine2.xy, gridLinesWidth);
	float l3 = segment(texcoord, CustomGridLine3.zw, CustomGridLine3.xy, gridLinesWidth);
	float l4 = segment(texcoord, CustomGridLine4.zw, CustomGridLine4.xy, gridLinesWidth);
	return l1 + l2 + l3 + l4;
}

#ifndef CUSTOM_GRID_IMAGE
#define CUSTOM_GRID_IMAGE "customGrid.png"//Put your image file name here or remplace the original file
#endif

texture	customGridTex <source= CUSTOM_GRID_IMAGE; > { Width = 1920; Height = 1080; MipLevels = 1; Format = RGBA8; };

sampler	customGridSampler
{
	Texture = customGridTex;
	AddressU = BORDER;
	AddressV = BORDER;
};

float customGridImage(float2 texcoord){
	return 1 - tex2D(customGridSampler, texcoord).r;
}

texture	goldenRatioTex <source= "golden_ratio.png"; > { Width = 1156; Height = 715; MipLevels = 1; Format = RGBA8; };

sampler	goldenRatioSampler
{
	Texture = goldenRatioTex;
	AddressU = BORDER;
	AddressV = BORDER;
};

//Based on Otis_inf GoldenRatio.fx
/*
float fibonacciOtis(float2 texcoord){
	float phiValue = ((1.0 + sqrt(5.0))/2.0);
	float aspectRatio = (float)iUIAspectRatio.x/(float)iUIAspectRatio.y;

	int fakeBufferHeight = BUFFER_HEIGHT;
	int fakeBufferWidth = BUFFER_WIDTH;

	if(aspectRatio < BUFFER_ASPECT_RATIO)
	{
		fakeBufferWidth = BUFFER_HEIGHT * aspectRatio;
	}
	else
	{
		fakeBufferHeight = BUFFER_WIDTH / aspectRatio;
	}

	float idealWidth = fakeBufferHeight * phiValue;
	float idealHeight = fakeBufferWidth / phiValue;
	float4 sourceCoordFactor = float4(1.0, 1.0, 1.0, 1.0);

	if(ResizeMode){
		if(aspectRatio < phiValue)
		{
			// display spirals at full width, but resize across height
			sourceCoordFactor = float4(1.0, fakeBufferHeight/idealHeight, 1.0, idealHeight/fakeBufferHeight);
		}
		else
		{
			// display spirals at full height, but resize across width
			sourceCoordFactor = float4(fakeBufferWidth/idealWidth, 1.0, idealWidth/fakeBufferWidth, 1.0);
		}
	}
	
	return tex2D(goldenRatioSampler, float2((texcoord.x * sourceCoordFactor.x) - ((1.0-sourceCoordFactor.z)/2.0),
														(texcoord.y * sourceCoordFactor.y) - ((1.0-sourceCoordFactor.w)/2.0))).a;
}
*/

float fibonacci(float2 texcoord){
	return tex2D(goldenRatioSampler, texcoord).a;
}

float DrawAR(float aspectRatio, float2 texcoord){
	float2 vpos = texcoord*BUFFER_SCREEN_SIZE;
	float borderSize;
	float retVal = 0;

	if(aspectRatio < BUFFER_ASPECT_RATIO)
	{
		borderSize = (BUFFER_WIDTH - BUFFER_HEIGHT * aspectRatio) / 2.0;

		if(vpos.x < borderSize || vpos.x > (BUFFER_WIDTH - borderSize))
			retVal = 1;
	}
	else
	{
		borderSize = (BUFFER_HEIGHT - BUFFER_WIDTH / aspectRatio) / 2.0;

		if(vpos.y < borderSize || vpos.y > (BUFFER_HEIGHT - borderSize))
			retVal = 1;
	}
	
	return retVal;
}

float2 texcoordRemapAR(float aspectRatio, float2 texcoord){
	float2 vpos = texcoord*BUFFER_SCREEN_SIZE;
	float borderSize;

	if(aspectRatio < BUFFER_ASPECT_RATIO)
	{
		borderSize = (BUFFER_WIDTH - BUFFER_HEIGHT * aspectRatio) / 2.0;

		float w = BUFFER_HEIGHT * aspectRatio;
		float x = texcoord.x*BUFFER_WIDTH/w - borderSize/w;
		return float2(x,texcoord.y);
	}
	else
	{
		borderSize = (BUFFER_HEIGHT - BUFFER_WIDTH / aspectRatio) / 2.0;

		float h = BUFFER_WIDTH / aspectRatio;
		float y = texcoord.y*BUFFER_HEIGHT/h - borderSize/h;
		return float2(texcoord.x, y);
	}
}

float getAR(){
	float ar;
	switch(ARMode){
		case 0:{
			ar = BUFFER_ASPECT_RATIO;
		}break;
		case 1:{
			ar = ASPECT_RATIOS[ARFromList];
		}break;
		case 2:{
			ar = (float)iUIAspectRatio.x/(float)iUIAspectRatio.y;
		}break;
	}
	return ar;
}


float3 AspectRatioMultiGrid_PS(float4 vpos : SV_Position, float2 texcoord : TexCoord) : SV_Target
{
    float3 color = UseWhiteBackground ? float3(1,1,1) : tex2D(ReShade::BackBuffer, texcoord).rgb;
	float3 realGridColor = gridInverseColor ? 1 - color : gridColor;

	float ar = getAR();
	float2 remappedTexcoord = texcoordRemapAR(ar, texcoord);

	float lines = 0;
	if (RuleofThirds) lines = lines + ruleOfThirds(remappedTexcoord);
	if (RuleofFifths) lines = lines + ruleOfFifths(remappedTexcoord);
	if (Triangles1) lines = lines + triangles(remappedTexcoord);
	if (Triangles2) lines = lines + triangles(float2(1-remappedTexcoord.x,remappedTexcoord.y));
	if (Diagonals) lines = lines + diagonals(remappedTexcoord);
	if (Diamond) lines = lines + diamond(remappedTexcoord);
	if (Special1) lines = lines + special(remappedTexcoord);
	if (Special2) lines = lines + special2(remappedTexcoord);
	if (Special3) lines = lines + special3(remappedTexcoord);
	if (FibonacciBottomRight) lines = lines + fibonacci(remappedTexcoord);
	if (FibonacciBottomLeft) lines = lines + fibonacci(float2(1-remappedTexcoord.x,remappedTexcoord.y));
	if (FibonacciTopRight) lines = lines + fibonacci(float2(remappedTexcoord.x,1-remappedTexcoord.y));
	if (FibonacciTopLeft) lines = lines + fibonacci(float2(1-remappedTexcoord.x,1-remappedTexcoord.y));
	if (CustomGrid) lines = lines + customGrid(remappedTexcoord);
	if (CustomGridImage) lines = lines + customGridImage(remappedTexcoord);

	color = lerp(color, realGridColor, min(gridOpacity, lines));

	if (ARMode != 0){
		float aspectRatioBars = DrawAR(ar, texcoord);
		color = lerp(color, ARColor, min(AROpacity, aspectRatioBars));
	}

	return color;
}

technique AspectRatioMultiGrid
{
	pass
	{
		VertexShader = PostProcessVS;
		PixelShader = AspectRatioMultiGrid_PS;
	}
}

}
