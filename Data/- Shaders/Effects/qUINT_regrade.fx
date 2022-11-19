/*=============================================================================

	ReShade 4 effect file
    github.com/martymcmodding

    Copyright (c) Pascal Gilcher. All rights reserved.

	Support me:
   		paypal.me/mcflypg
   		patreon.com/mcflypg

    ReGrade ALPHA 0.7

    changelog:

    0.1:    - initial release
    0.2:    - ported to refactored qUINT structure
            - removed RGB split from tone curve and lift gamma gain, replaced with
              with RGB selector
            - fixed bug with color remapper red affecting pure white
            - add more grading controls
            - remade lift gamma gain to fit convention
            - replaced histogram with McFly's '21 bomb ass histogram (TM)
            - added split toning
            - switched to internal LUT processing
    0.3:    - switched to LUT atlas for free arrangement of CC operations
            - added clipping mask
            - fixed bug with levels
            - closed UI sections by default
            - added vignette
            - improved color remap - still allows all hues, but now allows raising saturation
    0.4:    - changed LUT sampling to tetrahedral
            - flattened LUTs, improves performance greatly
    0.5:    - added color balance node
            - added dark wash feature
            - extended levels to in/out
            - added gamma to adjustments
    0.6:    - remade histogram UI
            - added dithering
    0.7:    - added compute histogram
            - integration with SOLARIS
            - remade hue controls entirely
            - added waveform mode for compute enabled platforms
            - fixed colorspace linear conversion in xyz, lab and oklab
            - remade split toning
            - moved waveform to Insight
    0.8:    - remade color balance 
            - added special transforms 
            - remade tone curve 

    * Unauthorized copying of this file, via any medium is strictly prohibited
 	* Proprietary and confidential

=============================================================================*/

/*=============================================================================
	Preprocessor settings
=============================================================================*/

#ifndef ENABLE_SOLARIS_REGRADE_PARITY
 #define ENABLE_SOLARIS_REGRADE_PARITY                 0   //[0 or 1]      If enabled, ReGrade takes HDR input from SOLARIS as color buffer instead. This allows HDR exposure, bloom and color grading to work nondestructively
#endif

/*=============================================================================
	UI Uniforms
=============================================================================*/

uniform int UIHELP <
	ui_type = "radio";
	ui_label = " ";	
	ui_text = "How to use ReGrade:\n\n"
    "ReGrade is a modular color grading platform.\n"
    "Pick any color grading operation in the slots below,\n"
    "then adjust its parameters in the respective section.\n\n"
    "CAUTION: You *can* pick operations multiple times, but they do NOT\n"
    "have separate controls, so it is advised to not use them twice.\n\n"
    "To use the histogram/waveforms, enable the respective\n"
    "technique in the technique window above."
   ;
	ui_category = ">>>> OVERVIEW / HELP (click me) <<<<";
	ui_category_closed = false;
>;

#define LABEL_NONE              "None"
#define LABEL_LEVELS            "Levels"
#define LABEL_ADJ               "Adjustments"
#define LABEL_LGG               "Lift Gamma Gain"
#define LABEL_WB                "White Balance"
#define LABEL_REMAP             "Color Remapping"
#define LABEL_TONECURVE         "Tone Curve"
#define LABEL_SPLIT             "Split Toning"
#define LABEL_CB                "Color Balance"
#define LABEL_SPECIAL           "Special Transforms"

#define CONCAT(a,b) a ## b
#define SECTION_PREFIX          "Parameters for "

#define SECTION_LEVELS          CONCAT(SECTION_PREFIX, LABEL_LEVELS) 
#define SECTION_ADJ             CONCAT(SECTION_PREFIX, LABEL_ADJ) 
#define SECTION_LGG             CONCAT(SECTION_PREFIX, LABEL_LGG) 
#define SECTION_WB              CONCAT(SECTION_PREFIX, LABEL_WB) 
#define SECTION_REMAP           CONCAT(SECTION_PREFIX, LABEL_REMAP) 
#define SECTION_TONECURVE       CONCAT(SECTION_PREFIX, LABEL_TONECURVE) 
#define SECTION_SPLIT           CONCAT(SECTION_PREFIX, LABEL_SPLIT) 
#define SECTION_CB              CONCAT(SECTION_PREFIX, LABEL_CB) 
#define SECTION_SPECIAL         CONCAT(SECTION_PREFIX, LABEL_SPECIAL) 

#define GRADE_ID_NONE           0
#define GRADE_ID_LEVELS         1
#define GRADE_ID_ADJ            2
#define GRADE_ID_LGG            3
#define GRADE_ID_WB             4
#define GRADE_ID_REMAP          5
#define GRADE_ID_TONECURVE      6
#define GRADE_ID_SPLIT          7
#define GRADE_ID_CB             8
#define GRADE_ID_SPECIAL        9

#define NUM_SECTIONS 9 //cannot use this to generate the nodes below! Increment and extend concat, add node

#define concat_all(a,b,c,d,e,f,g,h,i,j) a ## "\0" ## b ## "\0" ##c ## "\0" ##d ## "\0" ##e ## "\0" ##f ## "\0" ##g ## "\0" ##h ## "\0" ##i ## "\0" ##j ## "\0"  
#define CURR_ITEMS concat_all(LABEL_NONE, LABEL_LEVELS, LABEL_ADJ, LABEL_LGG, LABEL_WB, LABEL_REMAP, LABEL_TONECURVE, LABEL_SPLIT, LABEL_CB, LABEL_SPECIAL)

uniform int NODE1 <
	ui_type = "combo";
    ui_label = "Slot 1";
	ui_items = CURR_ITEMS;
    ui_category = "ORDER OF COLOR OPERATIONS";
> = 0;

uniform int NODE2 <
	ui_type = "combo";
    ui_label = "Slot 2";
	ui_items = CURR_ITEMS;
    ui_category = "ORDER OF COLOR OPERATIONS";
> = 0;

uniform int NODE3 <
	ui_type = "combo";
    ui_label = "Slot 3";
	ui_items = CURR_ITEMS;
    ui_category = "ORDER OF COLOR OPERATIONS";
> = 0;

uniform int NODE4 <
	ui_type = "combo";
    ui_label = "Slot 4";
	ui_items = CURR_ITEMS;
    ui_category = "ORDER OF COLOR OPERATIONS";
> = 0;

uniform int NODE5 <
	ui_type = "combo";
    ui_label = "Slot 5";
	ui_items = CURR_ITEMS;
    ui_category = "ORDER OF COLOR OPERATIONS";
> = 0;

uniform int NODE6 <
	ui_type = "combo";
    ui_label = "Slot 6";
	ui_items = CURR_ITEMS;
    ui_category = "ORDER OF COLOR OPERATIONS";
> = 0;

uniform int NODE7 <
	ui_type = "combo";
    ui_label = "Slot 7";
	ui_items = CURR_ITEMS;
    ui_category = "ORDER OF COLOR OPERATIONS";
> = 0;

uniform int NODE8 <
	ui_type = "combo";
    ui_label = "Slot 8";
	ui_items = CURR_ITEMS;
    ui_category = "ORDER OF COLOR OPERATIONS";
> = 0;

uniform int NODE9 <
	ui_type = "combo";
    ui_label = "Slot 9";
	ui_items = CURR_ITEMS;
    ui_category = "ORDER OF COLOR OPERATIONS";
> = 0;

uniform float INPUT_BLACK_LVL <
	ui_type = "drag";
	ui_min = 0.0; ui_max = 255.0;
	ui_step = 1.0;
	ui_label = "Black Level In";
    ui_category = SECTION_LEVELS;
    ui_category_closed = true;
> = 0.0;

uniform float INPUT_WHITE_LVL <
	ui_type = "drag";
	ui_min = 0.0; ui_max = 255.0;
	ui_step = 1.0;
	ui_label = "White Level In";
    ui_category = SECTION_LEVELS;
    ui_category_closed = true;
> = 255.0;

uniform float OUTPUT_BLACK_LVL <
	ui_type = "drag";
	ui_min = 0.0; ui_max = 255.0;
	ui_step = 1.0;
	ui_label = "Black Level Out";
    ui_category = SECTION_LEVELS;
    ui_category_closed = true;
> = 0.0;

uniform float OUTPUT_WHITE_LVL <
	ui_type = "drag";
	ui_min = 0.0; ui_max = 255.0;
	ui_step = 1.0;
	ui_label = "White Level Out";
    ui_category = SECTION_LEVELS;
    ui_category_closed = true;
> = 255.0;

uniform float GRADE_CONTRAST <
	ui_type = "drag";
	ui_min = -1.0; ui_max = 1.0;
	ui_label = "Contrast";
    ui_category = SECTION_ADJ;
    ui_category_closed = true;
> = 0.0;

uniform float GRADE_EXPOSURE <
	ui_type = "drag";
	ui_min = -4.0; ui_max = 4.0;
	ui_label = "Exposure";
    ui_category = SECTION_ADJ;
    ui_category_closed = true;
> = 0.0;

uniform float GRADE_GAMMA <
	ui_type = "drag";
	ui_min = -1.0; ui_max = 1.0;
	ui_label = "Gamma";
    ui_category = SECTION_ADJ;
    ui_category_closed = true;
> = 0.0;

uniform float GRADE_SATURATION <
	ui_type = "drag";
	ui_min = -1.0; ui_max = 1.0;
	ui_label = "Saturation";
    ui_category = SECTION_ADJ;
    ui_category_closed = true;
> = 0.0;

uniform float GRADE_VIBRANCE <
	ui_type = "drag";
	ui_min = -1.0; ui_max = 1.0;
	ui_label = "Vibrance";
    ui_category = SECTION_ADJ;
    ui_category_closed = true;
> = 0.0;

uniform float3 INPUT_LIFT_COLOR <
  	ui_type = "color";
  	ui_label="Lift";
  	ui_category = SECTION_LGG;
      ui_category_closed = true;    
> = float3(0.5, 0.5, 0.5);

uniform float3 INPUT_GAMMA_COLOR <
  	ui_type = "color";
  	ui_label="Gamma";
  	ui_category = SECTION_LGG;
      ui_category_closed = true;    
> = float3(0.5, 0.5, 0.5);

uniform float3 INPUT_GAIN_COLOR <
  	ui_type = "color";
  	ui_label="Gain";
  	ui_category = SECTION_LGG;
      ui_category_closed = true;    
> = float3(0.5, 0.5, 0.5);

uniform float INPUT_COLOR_TEMPERATURE <
	ui_type = "drag";
	ui_min = 1700.0; ui_max = 40000.0;
    ui_step = 10.0;
	ui_label = "Color Temperature";
    ui_category = SECTION_WB;
    ui_category_closed = true;
> = 6500.0;

uniform float3 COLOR_REMAP_RED <
  	ui_type = "drag";
    ui_min = -1.0;
    ui_max = 1.0;
  	ui_label="Hue | Saturation | Value   : Red";
  	ui_category = SECTION_REMAP;
      ui_category_closed = true;   
> = float3(0.0, 0.0, 0.0);

uniform float3 COLOR_REMAP_ORANGE <
  	ui_type = "drag";
    ui_min = -1.0;
    ui_max = 1.0;
  	ui_label="Hue | Saturation | Value   : Orange";
  	ui_category = SECTION_REMAP;
      ui_category_closed = true;    
> = float3(0.0, 0.0, 0.0);

uniform float3 COLOR_REMAP_YELLOW <
  	ui_type = "drag";
    ui_min = -1.0;
    ui_max = 1.0;
  	ui_label="Hue | Saturation | Value   : Yellow";
  	ui_category = SECTION_REMAP;
      ui_category_closed = true;     
> = float3(0.0, 0.0, 0.0);

uniform float3 COLOR_REMAP_GREEN <
  	ui_type = "drag";
    ui_min = -1.0;
    ui_max = 1.0;
  	ui_label="Hue | Saturation | Value   : Green";
  	ui_category = SECTION_REMAP;
      ui_category_closed = true;   
> = float3(0.0, 0.0, 0.0);

uniform float3 COLOR_REMAP_AQUA <
  	ui_type = "drag";
    ui_min = -1.0;
    ui_max = 1.0;
  	ui_label="Hue | Saturation | Value   : Aqua";
	ui_category = SECTION_REMAP;
    ui_category_closed = true;
> = float3(0.0, 0.0, 0.0);

uniform float3 COLOR_REMAP_BLUE <
  	ui_type = "drag";
    ui_min = -1.0;
    ui_max = 1.0;
  	ui_label="Hue | Saturation | Value   : Blue";
  	ui_category = SECTION_REMAP;
      ui_category_closed = true;    
> = float3(0.0, 0.0, 0.0);

uniform float3 COLOR_REMAP_MAGENTA <
  	ui_type = "drag";
    ui_min = -1.0;
    ui_max = 1.0;
  	ui_label="Hue | Saturation | Value   : Magenta";
  	ui_category = SECTION_REMAP;
      ui_category_closed = true;     
> = float3(0.0, 0.0, 0.0);

uniform float TONECURVE_SHADOWS <
	ui_type = "drag";
	ui_min = -1.00; ui_max = 1.00;
	ui_label = "Shadows";
    ui_category = SECTION_TONECURVE;
    ui_category_closed = true;
> = 0.00;

uniform float TONECURVE_DARKS <
	ui_type = "drag";
	ui_min = -1.00; ui_max = 1.00;
	ui_label = "Darks";
    ui_category = SECTION_TONECURVE;
    ui_category_closed = true;
> = 0.00;

uniform float TONECURVE_LIGHTS <
	ui_type = "drag";
	ui_min = -1.00; ui_max = 1.00;
	ui_label = "Lights";
    ui_category = SECTION_TONECURVE;
    ui_category_closed = true;
> = 0.00;

uniform float TONECURVE_HIGHLIGHTS <
	ui_type = "drag";
	ui_min = -1.00; ui_max = 1.00;
	ui_label = "Highlights";
    ui_category = SECTION_TONECURVE;
    ui_category_closed = true;
> = 0.00;

uniform float TONECURVE_DARKWASH_RANGE <
	ui_type = "drag";
	ui_min = 0.0; ui_max = 1.0;
	ui_label = "Dark Wash Range";
    ui_category = SECTION_TONECURVE;
    ui_category_closed = true;
> = 0.2;
uniform float TONECURVE_DARKWASH_INT <
	ui_type = "drag";
	ui_min = 0.0; ui_max = 1.0;
	ui_label = "Dark Wash Intensity";
    ui_category = SECTION_TONECURVE;
    ui_category_closed = true;
> = 0.0;

uniform float3 SPLITTONE_SHADOWS <
  	ui_type = "color";
  	ui_label="Shadow Tint";
  	ui_category = SECTION_SPLIT;
    ui_category_closed = true;    
> = float3(1.0, 1.0, 1.0);

uniform float3 SPLITTONE_HIGHLIGHTS <
  	ui_type = "color";
  	ui_label="Highlight Tint";
  	ui_category = SECTION_SPLIT;
    ui_category_closed = true;    
> = float3(1.0, 1.0, 1.0);

uniform float SPLITTONE_BALANCE <
	ui_type = "drag";
	ui_min = 0.00; ui_max = 1.00;
	ui_label = "Balance";
    ui_category = SECTION_SPLIT;
    ui_category_closed = true;
> = 0.5;

uniform float2 CB_SHAD <
  	ui_type = "drag";
  	ui_label="Hue / Saturation: Shadows";
    ui_min = 0.00; ui_max = 1.00;  
  	ui_category = SECTION_CB;
    ui_category_closed = true;    
> = float2(0.0, 0.0);

uniform float2 CB_MID <
  	ui_type = "drag";
  	ui_label="Hue / Saturation: Midtones";
    ui_min = 0.00; ui_max = 1.00;  
  	ui_category = SECTION_CB;
    ui_category_closed = true;    
> = float2(0.0, 0.0);

uniform float2 CB_HI <
  	ui_type = "drag";
  	ui_label="Hue / Saturation: Highlights";
    ui_min = 0.00; ui_max = 1.00;  
  	ui_category = SECTION_CB;
    ui_category_closed = true;    
> = float2(0.0, 0.0);

uniform float SPECIAL_BLEACHBP <
	ui_type = "drag";
	ui_min = 0.0; ui_max = 1.0;
	ui_label = "Bleach Bypass (Gamma Corrected)";
    ui_category = SECTION_SPECIAL;
    ui_category_closed = true;
> = 0.0;

uniform float2 SPECIAL_GAMMA_LUM_CHROM <
	ui_type = "drag";
	ui_min = -1.0; ui_max = 1.0;
	ui_label = "Gamma on Luma | Chroma";
    ui_category = SECTION_SPECIAL;
    ui_category_closed = true;
> = float2(0.0, 0.0);

uniform bool ENABLE_VIGNETTE <
	ui_label = "Enable Vignette Effect";
    ui_tooltip = "Allows two kinds of vignette effects:\n\n" 
    "Mechanical Vignette:   The insides of a camera lens occlude light at the corners of the field of view.\n\n"
    "Sensor Vignette:       Projection of light onto sensor plane causes a secondary vignette effect.\n"
    "                       Incident angle of light hitting the sensor and its travel distance affect light intensity.";
    ui_category = "Vignette";
    ui_category_closed = true;
> = false;

uniform float VIGNETTE_RADIUS_MECH <
	ui_type = "drag";
	ui_min = 0.0; ui_max = 1.0;
	ui_label = "Mechanical Vignette: Radius";
    ui_category = "Vignette";
    ui_category_closed = true;
> = 0.525;

uniform float VIGNETTE_BLURRYNESS_MECH <
	ui_type = "drag";
	ui_min = 0.0; ui_max = 1.0;
	ui_label = "Mechanical Vignette: Blurryness";
    ui_category = "Vignette";
    ui_category_closed = true;
> = 0.8;

uniform float VIGNETTE_RATIO <
	ui_type = "drag";
	ui_min = -1.0; ui_max = 1.0;
	ui_label = "Mechanical Vignette: Shape";
    ui_category = "Vignette";
    ui_category_closed = true;
> = 0.0;

uniform float VIGNETTE_RADIUS_SENSOR <
	ui_type = "drag";
	ui_min = 0.0; ui_max = 2.0;
	ui_label = "Sensor Vignette:     Scale";
    ui_category = "Vignette";
    ui_category_closed = true;
> = 1.0;

uniform int VIGNETTE_BLEND_MODE <
	ui_type = "combo";
    ui_label = "Vignette Blending Mode";
	ui_items = "Standard\0HDR simulation\0HDR simulation (protect tones)\0";
    ui_category = "Vignette";
    ui_category_closed = true;
> = 1;

uniform int DITHER_BIT_DEPTH <
	ui_type = "combo";
    ui_label = "Dithering";
	ui_items = " Off\0 6 Bit\0 8 Bit\0 10 Bit\0 12 Bit\0";
    ui_category = "Utility";
    ui_category_closed = true;
> = 2;

/*
uniform float4 tempF1 <
    ui_type = "drag";
    ui_min = -100.0;
    ui_max = 100.0;
> = float4(1,1,1,1);

uniform float4 tempF2 <
    ui_type = "drag";
    ui_min = -100.0;
    ui_max = 100.0;
> = float4(1,1,1,1);

uniform float4 tempF3 <
    ui_type = "drag";
    ui_min = -100.0;
    ui_max = 100.0;
> = float4(1,1,1,1);
*/

/*=============================================================================
	Textures, Samplers, Globals, Structs
=============================================================================*/

uniform float FRAMETIME < source = "frametime";  >;
uniform uint FRAMECOUNT  < source = "framecount"; >;
uniform bool OVERLAY_OPEN < source = "overlay_open"; >;

texture ColorInputTex : COLOR;
sampler ColorInput 	{ Texture = ColorInputTex; MinFilter=POINT; MipFilter=POINT; MagFilter=POINT; };

#define INTERNAL_LUT_SIZE           33

texture2D LUTAtlas	                    { Width = INTERNAL_LUT_SIZE * INTERNAL_LUT_SIZE;   Height = INTERNAL_LUT_SIZE * NUM_SECTIONS;     Format = RGBA16F;  	};
texture2D LUTFlattened	                { Width = INTERNAL_LUT_SIZE * INTERNAL_LUT_SIZE;   Height = INTERNAL_LUT_SIZE;     Format = RGB10A2;  	};
sampler2D sLUTAtlas		                { Texture = LUTAtlas;  };
sampler2D sLUTFlattened		            { Texture = LUTFlattened;  };

#if ENABLE_SOLARIS_REGRADE_PARITY != 0
texture2D ColorInputHDRTex			    { Width = BUFFER_WIDTH;   Height = BUFFER_HEIGHT;                Format = RGBA16F; };
sampler2D sColorInputHDR			    { Texture = ColorInputHDRTex;  };
#endif

struct VSOUT
{
	float4                  vpos        : SV_Position;
    float2                  uv          : TEXCOORD0;    
};

#include "qUINT\Global.fxh"
#include "qUINT\Colorspaces.fxh"
#include "qUINT\Whitebalance.fxh"

/*=============================================================================
	Functions
=============================================================================*/
/*
float3 color_remapper_old(in float3 rgb)
{
	float3 hsl = Colorspace::rgb_to_hsl(rgb);

	static const float hue_nodes[8] = {	 0.0, 1.0/12.0, 2.0/12.0, 4.0/12.0, 6.0/12.0, 8.0/12.0, 10.0/12.0, 1.0};

	float risingedges[7];
	for(int j = 0; j < 7; j++)
		risingedges[j] = linearstep(hue_nodes[j], hue_nodes[j+1], hsl.x);

	float3 remapped = 0;
    remapped += Colorspace::hsl_to_rgb(float3(frac(hue_nodes[0] + 0.5 * abs(COLOR_REMAP_RED.x) * COLOR_REMAP_RED.x),     COLOR_REMAP_RED.y + 1.0,     exp2(COLOR_REMAP_RED.z))     * float3(1.0, hsl.y, hsl.z)) * ((1.0 - risingedges[0]) + risingedges[6]); //red - yes, this needs a +
	remapped += Colorspace::hsl_to_rgb(float3(frac(hue_nodes[1] + 0.5 * abs(COLOR_REMAP_ORANGE.x) * COLOR_REMAP_ORANGE.x),  COLOR_REMAP_ORANGE.y + 1.0,  exp2(COLOR_REMAP_ORANGE.z))  * float3(1.0, hsl.y, hsl.z)) * ((1.0 - risingedges[1]) * risingedges[0]); //orange
	remapped += Colorspace::hsl_to_rgb(float3(frac(hue_nodes[2] + 0.5 * abs(COLOR_REMAP_YELLOW.x) * COLOR_REMAP_YELLOW.x),  COLOR_REMAP_YELLOW.y + 1.0,  exp2(COLOR_REMAP_YELLOW.z))  * float3(1.0, hsl.y, hsl.z)) * ((1.0 - risingedges[2]) * risingedges[1]); //yellow
	remapped += Colorspace::hsl_to_rgb(float3(frac(hue_nodes[3] + 0.5 * abs(COLOR_REMAP_GREEN.x) * COLOR_REMAP_GREEN.x),   COLOR_REMAP_GREEN.y + 1.0,   exp2(COLOR_REMAP_GREEN.z))   * float3(1.0, hsl.y, hsl.z)) * ((1.0 - risingedges[3]) * risingedges[2]); //green
	remapped += Colorspace::hsl_to_rgb(float3(frac(hue_nodes[4] + 0.5 * abs(COLOR_REMAP_AQUA.x) * COLOR_REMAP_AQUA.x),    COLOR_REMAP_AQUA.y + 1.0,    exp2(COLOR_REMAP_AQUA.z))    * float3(1.0, hsl.y, hsl.z)) * ((1.0 - risingedges[4]) * risingedges[3]); //aqua
	remapped += Colorspace::hsl_to_rgb(float3(frac(hue_nodes[5] + 0.5 * abs(COLOR_REMAP_BLUE.x) * COLOR_REMAP_BLUE.x),    COLOR_REMAP_BLUE.y + 1.0,    exp2(COLOR_REMAP_BLUE.z))    * float3(1.0, hsl.y, hsl.z)) * ((1.0 - risingedges[5]) * risingedges[4]); //blue
	remapped += Colorspace::hsl_to_rgb(float3(frac(hue_nodes[6] + 0.5 * abs(COLOR_REMAP_MAGENTA.x) * COLOR_REMAP_MAGENTA.x), COLOR_REMAP_MAGENTA.y + 1.0, exp2(COLOR_REMAP_MAGENTA.z)) * float3(1.0, hsl.y, hsl.z)) * ((1.0 - risingedges[6]) * risingedges[5]); //magenta
    //fix white
    remapped = lerp(rgb, remapped, smoothstep(0, 1.0/255, hsl.y));

	return remapped;
}
*/
float3 color_remapper(in float3 rgb)
{
    static const float hue_nodes[8] = {	 0.0, 1.0/12.0, 2.0/12.0, 4.0/12.0, 6.0/12.0, 8.0/12.0, 10.0/12.0, 1.0};

    float hue = Colorspace::rgb_to_hsl(rgb).x;

    float risingedges[7];
	for(int j = 0; j < 7; j++) risingedges[j] = linearstep(hue_nodes[j], hue_nodes[j+1], hue);
    
    float hueweights[7];    
    hueweights[0] = ((1.0 - risingedges[0]) + risingedges[6]); //this goes over the 2 pi boundary, so this needs special treatment
    for(int j = 1; j < 7; j++) hueweights[j] = ((1.0 - risingedges[j]) * risingedges[j - 1]);    

    float3 hue_modifiers[7] = {COLOR_REMAP_RED, COLOR_REMAP_ORANGE, COLOR_REMAP_YELLOW, COLOR_REMAP_GREEN, COLOR_REMAP_AQUA, COLOR_REMAP_BLUE, COLOR_REMAP_MAGENTA};

    float3 LChmod = 0;

    [loop]
    for(int hue = 0; hue < 7; hue++)
    {
        LChmod.z += abs(hue_modifiers[hue].x) * hue_modifiers[hue].x * hueweights[hue];
        LChmod.xy += hue_modifiers[hue].zy * hueweights[hue];    
    }

    float3 oklab = Colorspace::rgb_to_oklab(rgb);

    oklab.x *= exp2(LChmod.x * 0.33333); //~1/3 seems to create visual parity to HSL/HSV based tweaking, we don't want to break presets too much  

    float2 huesc; sincos(-3.14159265 * LChmod.z, huesc.x, huesc.y); 
    oklab.yz = mul(oklab.yz, float2x2(huesc.y, -huesc.x, huesc.x, huesc.y)); 

    oklab.yz = LChmod.y < 0 
             ? oklab.yz * (1 + LChmod.y) //reduce saturation -> saturation 0%-100%
             : normalize(oklab.yz) * pow(length(oklab.yz) * 2.0, exp2(-LChmod.y * 0.5)) * 0.5; //increase saturation -> vibrance

    return Colorspace::oklab_to_rgb(oklab);
}

float p(float x)  
{ 
    return x < 0.3333333 ? x * (-3.0 * x * x + 1.0) 
                         : 1.5 * x * (1.0 + x * (x - 2.0)); 
}

float shadows(float x)
{
    return p(saturate(x * 2.0)) * 0.5;
}

float darks(float x)
{
    return p(x);    
}

float lights(float x)
{
    return p(1.0 - x);
}

float highlights(float x)
{
    return lights(saturate(x * 2.0 - 1.0)) * 0.5;
}

float tonecurve(float x, float int_s, float int_d, float int_l, float int_h, float int_dw, float r_dw)
{    
    float s = x;

    x += shadows(s) * int_s;
    x += darks(x) * int_d;
    x += lights(s) * int_l;
    x += highlights(x) * int_h;

    float t = x / (r_dw + 1e-6);
    float dw = (10.0 * r_dw  - x) * t * t * (1.0 / 31.25) + r_dw;

    x = max(x, (dw - x) * int_dw + x);
	return x;
}

float3 input_remap( float3 x,   
					 float3 blacklevel, 
					 float3 whitelevel,
                     float3 blacklevel_out,
                     float3 whitelevel_out)
{
	x = linearstep(blacklevel, whitelevel + 1e-6, x);   
    x = lerp(blacklevel_out, whitelevel_out, x); 
	return x;
}

float3 lgg(float3 x, float3 lift, float3 gamma, float3 gain)
{
    //https://en.wikipedia.org/wiki/ASC_CDL
    x = pow(saturate((x * gain) + lift), gamma);
    return x;
}

float3 srgb_to_linear(float3 S)
{
    return S < 0.04045 ? S / 12.92 : pow(((S + 0.055) / 1.055), 2.4);
}

float3 linear_to_srgb(float3 L)
{
    return L <= 0.0031308 ? L * 12.92 : 1.055 * pow(L, rcp(2.4)) - 0.055;
}

float extended_overlay(float base, float blend)
{
    float sharpness = 7; //max ~100 without precision loss
    float poly_tri = 1 - pow(pow(base, sharpness) + pow(1 - base, sharpness), rcp(sharpness)); //form smooth triangle (vs hard triangle in regular overlay)
    return base + poly_tri * (blend * 2 - 1);
}

float3 splittone(float3 c)
{
    float cluma = dot(c, float3(0.299, 0.587, 0.114));
    float2 a = saturate(float2(cluma, 1 - cluma));
    a *= a;
    a *= a;
    //a *= a;
    float bal = lerp(1 - a.y, a.x, SPLITTONE_BALANCE);

    float3 tint_hsl = Colorspace::rgb_to_hsl(lerp(SPLITTONE_SHADOWS, SPLITTONE_HIGHLIGHTS, bal));
    tint_hsl.z = 0.5;
    float3 tint_rgb = Colorspace::hsl_to_rgb(tint_hsl);

    c.x = extended_overlay(c.x, tint_rgb.x);
    c.y = extended_overlay(c.y, tint_rgb.y);
    c.z = extended_overlay(c.z, tint_rgb.z);
    return c;
}

float3 color_balance(float3 c)
{
    /*
    float luma = Colorspace::linear_to_srgb(dot(Colorspace::srgb_to_linear(c), float3(0.2126729, 0.7151522, 0.0721750))).x; 

    float3 offsetSMH = float3(0, 0.5, 1);
    float3 widthSMH = float3(2.0, 1.0, 2.0);

    float3 weightSMH = saturate(1 - 2 * abs(luma - offsetSMH) / widthSMH);
    weightSMH = weightSMH * weightSMH * (3 - 2 * weightSMH);
    weightSMH *= weightSMH;

    float3 c_rgb_s = Colorspace::hsl_to_rgb(float3(CB_SHAD.x, CB_SHAD.y * CB_SHAD.y, 0.5));
    float3 c_rgb_m = Colorspace::hsl_to_rgb(float3(CB_MID.x, CB_MID.y * CB_MID.y, 0.5));
    float3 c_rgb_h = Colorspace::hsl_to_rgb(float3(CB_HI.x, CB_HI.y * CB_HI.y, 0.5));

    float3 c_oklab_s = Colorspace::rgb_to_oklab(c_rgb_s);
    float3 c_oklab_m = Colorspace::rgb_to_oklab(c_rgb_m);
    float3 c_oklab_h = Colorspace::rgb_to_oklab(c_rgb_h);

    float3 tint_oklab = float3(0.5, tempF1.y * (c_oklab_s.yz * weightSMH.x + c_oklab_m.yz * weightSMH.y + c_oklab_h.yz * weightSMH.z) / dot(weightSMH, 1));
    float3 tint_rgb =  Colorspace::oklab_to_rgb(tint_oklab);

    c *= tint_rgb;
    float luma_after = Colorspace::linear_to_srgb(dot(Colorspace::srgb_to_linear(c), float3(0.2126729, 0.7151522, 0.0721750))).x; 
    c *= luma / (luma_after + 1e-6);
*/
  
    //better use some perceptually well fitting estimate
    float luma = Colorspace::linear_to_srgb(dot(Colorspace::srgb_to_linear(c), float3(0.2126729, 0.7151522, 0.0721750))).x;

    float3 offsetSMH = float3(0, 0.5, 1);
    float3 widthSMH = float3(2.0, 1.0, 2.0);

    float3 weightSMH = saturate(1 - 2 * abs(luma - offsetSMH) / widthSMH);
    weightSMH = weightSMH * weightSMH * (3 - 2 * weightSMH);
    weightSMH *= weightSMH; //these do not sum up to 1.0, makes no sense in Lightroom either
    weightSMH.z *= 2.0;

    float3 tintcolorS = Colorspace::hsl_to_rgb(float3(frac(0.5 + CB_SHAD.x), 1, 0.5));
    float3 tintcolorM = Colorspace::hsl_to_rgb(float3(frac(0.5 + CB_MID.x), 1, 0.5));
    float3 tintcolorH = Colorspace::hsl_to_rgb(float3(frac(0.5 + CB_HI.x), 1, 0.5));

    return length(c) * normalize(pow(c, exp2(tintcolorS * weightSMH.x * CB_SHAD.y*CB_SHAD.y
                                            + tintcolorM * weightSMH.y * CB_MID.y*CB_MID.y
                                            + tintcolorH * weightSMH.z * CB_HI.y*CB_HI.y)));
    
    /*
    float3 c_hsl = Colorspace::rgb_to_hsl(c);

    float2 hue_cartesian; sincos(c_hsl.x * 6.283185307, hue_cartesian.y, hue_cartesian.x); hue_cartesian *= c_hsl.y;
    float3 splitw = float3(saturate(0.5 - c_hsl.z), 0.5, 0.5 - saturate(0.5 - c_hsl.z));
    float2 cyanred      = float2(1.0, 0.0) * dot(splitw, CB_BALANCE_CR);
    float2 blueyellow   = float2(0.5, 0.866025) * dot(splitw, CB_BALANCE_BL);
    float2 greenmagenta = float2(0.5, -0.866025) * dot(splitw, CB_BALANCE_GM);

    hue_cartesian += (cyanred + blueyellow + greenmagenta); 
    float newsat = saturate(length(hue_cartesian));

    float pi = 3.1415927;
    float newhue = frac(atan2(hue_cartesian.y, hue_cartesian.x) / 6.283185307);
    return Colorspace::hsl_to_rgb(float3(newhue, newsat, c_hsl.z));
    */
}

float3 adjustments(float3 col, float exposure, float contrast, float gamma, float vibrance, float saturation)
{
    col = srgb_to_linear(col);
    col *= exp2(exposure); //exposure in linear space - this makes no _visual_ difference but alters the response of the exposure curve
    col = linear_to_srgb(col);
    col = saturate(col);
    col = pow(col, exp2(-gamma));
    float3 contrasted = col - 0.5;
    contrasted = (contrasted / (0.5 + abs(contrasted))) + 0.5; //CJ.dk
    col = lerp(col, contrasted, contrast);
    col = Colorspace::rgb_to_hsl(col);
    col.y = pow(abs(col.y), exp2(-vibrance)) * (saturation + 1.0);
    col = Colorspace::hsl_to_rgb(col);
    return col;
}

float3 dither(in int2 pos, int bit_depth)
{
    const float2 magicdot = float2(0.75487766624669276, 0.569840290998);
    const float3 magicadd = float3(0, 0.025, 0.0125) * dot(magicdot, 1);
    
    const float lsb = exp2(bit_depth) - 1;

    float3 dither = frac(dot(pos, magicdot) + magicadd);
    dither = dither - 0.5;
    dither *= 0.9; //so if added to source color, it just does not spill over to next bucket
    dither /= lsb;    
    return dither;
}

//I'm really proud of this solution, it's orders of magnitude simpler than the ACES reference
float3 tetrahedral_volume_sampling(sampler volumesampler, float3 pn, int volume_size, int atlas_idx)
{
    float3 p = saturate(pn) * (volume_size - 1);

    //lo and hi corners inside lattice cell
    float3 corner000 = floor(p); float3 corner111 = ceil(p);

    //relative position inside lattice cell
    float3 delta = p - corner000;

/* //doesn't work?
    float3 g = step(delta.yzx, delta.xyz);
    float3 l = 1 - g;
    float3 maxaxis = min(g, l.zxy); //x largest? 100
    float3 minaxis =  1 - max(g.xyz, l.zxy);  //x smallest? 100
*/

//todo optimize?
    float3 maxaxis  = delta.x > delta.y && delta.x > delta.z ? float3(1,0,0) //x largest
                    : delta.y > delta.z ? float3(0,1,0) //y largest
                    : float3(0,0,1); //z largest
    float3 minaxis  = delta.x < delta.y && delta.x < delta.z ? float3(1,0,0) //x smallest
                    : delta.y < delta.z ? float3(0,1,0) //y smallest
                    : float3(0,0,1); //z smallest

                
    //3D coords of the 2 dynamic interpolants in the lattice    
    int3 cornermin = lerp(corner111, corner000, minaxis);
    int3 cornermax = lerp(corner000, corner111, maxaxis);
   
    //texture coords of the participating interpolants. LUT atlas extends vertically.
    int2 texturecoord000 = int2(corner000.x + corner000.z * volume_size, corner000.y + volume_size * atlas_idx);
    int2 texturecoordmax = int2(cornermax.x + cornermax.z * volume_size, cornermax.y + volume_size * atlas_idx);
    int2 texturecoordmin = int2(cornermin.x + cornermin.z * volume_size, cornermin.y + volume_size * atlas_idx);
    int2 texturecoord111 = int2(corner111.x + corner111.z * volume_size, corner111.y + volume_size * atlas_idx);

    float maxv = dot(maxaxis, delta);
    float minv = dot(minaxis, delta);
    float medv = dot(1 - maxaxis - minaxis, delta);

    float4 lattice_weights = float4(1, maxv, medv, minv);
    lattice_weights.xyz -= lattice_weights.yzw;
    
    return  tex2Dfetch(volumesampler, texturecoord000).rgb * lattice_weights.x            
          + tex2Dfetch(volumesampler, texturecoordmax).rgb * lattice_weights.y
          + tex2Dfetch(volumesampler, texturecoordmin).rgb * lattice_weights.z
          + tex2Dfetch(volumesampler, texturecoord111).rgb * lattice_weights.w;
}

float3 apply_vignette(float2 uv, float3 color)
{
    float2 viguv = uv * 2.0 - 1.0;
    viguv   -= viguv * saturate(float2(VIGNETTE_RATIO, -VIGNETTE_RATIO));
    viguv.x *= BUFFER_ASPECT_RATIO.y;

    float r = sqrt(dot(viguv, viguv) / dot(BUFFER_ASPECT_RATIO, BUFFER_ASPECT_RATIO));

    float vig = 1.0;
            
    float rf = r * VIGNETTE_RADIUS_SENSOR;
    float vigsensor = 1.0 + rf * rf; //cos is 1/sqrt(1+r*r) here, so 1/(1+r*r)^2 is cos^4
    vig *= rcp(vigsensor * vigsensor);
  
    float2 radii = VIGNETTE_RADIUS_MECH + float2(-VIGNETTE_BLURRYNESS_MECH * VIGNETTE_RADIUS_MECH * 0.2, 0);          

    float2 vsdf = r - radii;
    vig *= saturate(1 - vsdf.x / abs(vsdf.y - vsdf.x));

    switch(VIGNETTE_BLEND_MODE)
    {
        case 0:
            color *= vig; 
            break;
        case 1:
            color = color * rcp(1.03 - color);
            color *= vig;
            color = 1.03 * color * rcp(color + 1.0);
            break;
        case 2:
            float3 oldcolor = color;
            color = color * rcp(1.03 - color);
            color *= vig;
            color = 1.03 * color * rcp(color + 1.0);
            color = normalize(oldcolor + 1e-5) * length(color);
            break;
    }
    return color;
}

/*=============================================================================
	Shader Entry Points
=============================================================================*/

VSOUT VS_Basic(in uint id : SV_VertexID)
{
    VSOUT o;
    VS_FullscreenTriangle(id, o.vpos, o.uv);
    return o;
}

float3 specialfx(float3 c, float bleach_amt, float2 gamma_l_ch)
{
    //after removing all back and forth math involved with negative film process
    //at the end bleach bypass is just subtractive (multiplicative) mix with image luma

    //better use some perceptually well fitting estimate
    float luma = Colorspace::linear_to_srgb(dot(Colorspace::srgb_to_linear(c), float3(0.2126729, 0.7151522, 0.0721750))).x;
    c *= lerp(1.0, luma, bleach_amt);    
    c = pow(saturate(c), rcp(1.0 + bleach_amt * 0.5));

    float k = 1.73205; //sqrt(3)
    float l = length(c);
    float3 ch = c / (l + 1e-6);

    ch = pow(ch, exp2(gamma_l_ch.y));
    l = pow(l / k, exp2(-gamma_l_ch.x)) * k;

    c = normalize(ch + 1e-6) * l;    
    return c;
}

void PSMakeLUTs(in VSOUT i, out float3 o : SV_Target0)
{
    int gradeop = uint(floor(i.vpos.y)) / uint(INTERNAL_LUT_SIZE) + 1; //0 is "skip"

    float2 lut_xy = floor(i.vpos.xy); lut_xy.y %= INTERNAL_LUT_SIZE;
    float3 col = float3(lut_xy.x % INTERNAL_LUT_SIZE, lut_xy.y, floor(lut_xy.x / INTERNAL_LUT_SIZE));
    col = saturate(col / (INTERNAL_LUT_SIZE - 1.0));

    //doing one of the ops per atlas index - cycling over them into a single LUT compiles far too slow, atlas is actually faster
    switch(gradeop)
    {
        case GRADE_ID_LEVELS:       col = input_remap(col, INPUT_BLACK_LVL / 255.0, INPUT_WHITE_LVL / 255.0, OUTPUT_BLACK_LVL / 255.0, OUTPUT_WHITE_LVL / 255.0); break;
        case GRADE_ID_ADJ:          col = adjustments(col, GRADE_EXPOSURE, GRADE_CONTRAST, GRADE_GAMMA, GRADE_VIBRANCE, GRADE_SATURATION); break;
        case GRADE_ID_LGG:          col = lgg(col, INPUT_LIFT_COLOR - 0.5, 1.0 - INPUT_GAMMA_COLOR + 0.5, INPUT_GAIN_COLOR + 0.5); break;
        case GRADE_ID_WB:           col = Whitebalance::set_white_balance(col, INPUT_COLOR_TEMPERATURE); break;
        case GRADE_ID_REMAP:        col = color_remapper(col); break;
        case GRADE_ID_TONECURVE:    [unroll]for(int c = 0; c < 3; c++) col[c] = tonecurve(col[c], TONECURVE_SHADOWS, TONECURVE_DARKS, TONECURVE_LIGHTS, TONECURVE_HIGHLIGHTS, TONECURVE_DARKWASH_INT, TONECURVE_DARKWASH_RANGE); break;
        case GRADE_ID_SPLIT:        col = splittone(col); break;
        case GRADE_ID_CB:           col = color_balance(col); break;      
        case GRADE_ID_SPECIAL:      col = specialfx(col, SPECIAL_BLEACHBP, SPECIAL_GAMMA_LUM_CHROM); break;   
    };

    o = saturate(col);
}

void PSLUTFlatten(in VSOUT i, out float3 o : SV_Target0)
{
    const float2 lutsize = float2(INTERNAL_LUT_SIZE * INTERNAL_LUT_SIZE, INTERNAL_LUT_SIZE);
    const float2 invlutsize = rcp(lutsize);

    int op_sequence[NUM_SECTIONS] = {NODE1,NODE2,NODE3,NODE4,NODE5,NODE6,NODE7,NODE8, NODE9};

    float2 lut_xy = floor(i.vpos.xy); lut_xy.y %= INTERNAL_LUT_SIZE;
    float3 col = float3(lut_xy.x % INTERNAL_LUT_SIZE, lut_xy.y, floor(lut_xy.x / INTERNAL_LUT_SIZE));
    col = saturate(col / (INTERNAL_LUT_SIZE - 1.0));

    for(int op = 0; op < NUM_SECTIONS; op++)
    {
        uint curr_op = op_sequence[op];
        if(curr_op == GRADE_ID_NONE) continue;
        col = tetrahedral_volume_sampling(sLUTAtlas, col, INTERNAL_LUT_SIZE, curr_op - 1);
    }
    o = col;
}

void PSLUTApplyFlattened(in VSOUT i, out float3 o : SV_Target0)
{    
#if ENABLE_SOLARIS_REGRADE_PARITY != 0
    float3 col = tex2D(sColorInputHDR, i.uv).rgb; 
#else 
    float3 col = tex2D(ColorInput, i.uv).rgb;
#endif

    if(ENABLE_VIGNETTE) col = apply_vignette(i.uv, col);

    col = tetrahedral_volume_sampling(sLUTFlattened, col, INTERNAL_LUT_SIZE, 0); //no atlas, no LUT

    if(DITHER_BIT_DEPTH > 0) 
        col += dither(i.vpos.xy, DITHER_BIT_DEPTH * 2 + 4);
 
    o = col;    
}

/*=============================================================================
	Techniques
=============================================================================*/

technique qUINT_ReGrade
< ui_tooltip =
"                         >> qUINT::ReGrade <<"
"\n"
"\n"
"ReGrade is a comprehensive framework for color grading and -correction.\n"
"\n"
"It features many tools commonly used by photographers and video editors,\n"
"in a modular, intuitive and highly optimized framework.\n"
"Additionally, it is designed to work in tandem with SOLARIS, a novel approach\n"
"to bloom and HDR exposure adjustment.\n"
"\n"
"If using this feature, ake sure to place this effect right after SOLARIS,\n"
"with no other effects in between, as they will be ignored.\n";
>
{
	pass
	{
		VertexShader = VS_Basic;
		PixelShader = PSMakeLUTs;
        RenderTarget = LUTAtlas;
	}
    pass
	{
		VertexShader = VS_Basic;
		PixelShader = PSLUTFlatten;
        RenderTarget = LUTFlattened;
	}
    pass    
	{
		VertexShader = VS_Basic;
		PixelShader = PSLUTApplyFlattened;
	}
}