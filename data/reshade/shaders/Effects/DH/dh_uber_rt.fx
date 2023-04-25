#include "Reshade.fxh"

#ifndef DH_RENDER_SCALE
 #define DH_RENDER_SCALE 0.5
#endif

#define DEBUG_OFF 0
#define DEBUG_GI 1
#define DEBUG_AO 2
#define DEBUG_SSR 3
#define DEBUG_ROUGHNESS 4
#define DEBUG_DEPTH 5
#define DEBUG_NORMAL 6
#define DEBUG_SKY 7
#define DEBUG_MOTION 8

#define RT_HIT_SKY 0.42
#define RT_MISSED_CROSSED 0.5
#define RT_HIT 1.0
#define RT_MISSED 0.0

#define OPTIMIZATION_CROSS_TESTS 0
#define OPTIMIZATION_BIAISED_RT 1

#define NORMAL_FILTER 1

#define PI 3.14159265359
#define SQRT2 1.41421356237

// Can be used to fix wrong screen resolution
#define INPUT_WIDTH BUFFER_WIDTH
#define INPUT_HEIGHT BUFFER_HEIGHT

#define RENDER_WIDTH INPUT_WIDTH*DH_RENDER_SCALE
#define RENDER_HEIGHT INPUT_HEIGHT*DH_RENDER_SCALE

#define RENDER_SIZE int2(RENDER_WIDTH,RENDER_HEIGHT)

#define BUFFER_SIZE int2(INPUT_WIDTH,INPUT_HEIGHT)
#define BUFFER_SIZE3 int3(INPUT_WIDTH,INPUT_HEIGHT,INPUT_WIDTH*RESHADE_DEPTH_LINEARIZATION_FAR_PLANE*fDepthMultiplier/1024)
#define NOISE_SIZE 512

#define fRayHitDepthThreshold 0.350

#define getNormal(c) (tex2Dlod(normalSampler,float4(c.xy,0,0)).xyz-0.5)*2
#define getColor(c) tex2Dlod(ReShade::BackBuffer,float4(c,0,0))
#define getColorSamplerLod(s,c,l) tex2Dlod(s,float4(c.xy,0,l))
#define getColorSampler(s,c) tex2Dlod(s,float4(c.xy,0,0))

#define diffT(v1,v2,t) !any(max(abs(v1-v2)-t,0))
namespace DH_UBER_RT {

// Textures
    // Common textures
    texture roughnessTex { Width = INPUT_WIDTH; Height = INPUT_HEIGHT; Format = RGBA8; };
    sampler roughnessSampler { Texture = roughnessTex; };

    texture motionTex { Width = INPUT_WIDTH; Height = INPUT_HEIGHT; Format = RGBA32F; };
    sampler motionSampler { Texture = motionTex; };

    texture previousColorTex { Width = INPUT_WIDTH; Height = INPUT_HEIGHT; Format = RGBA8; };
    sampler previousColorSampler { Texture = previousColorTex; };

    texture depthTex { Width = INPUT_WIDTH; Height = INPUT_HEIGHT; Format = R32F; };
    sampler depthSampler { Texture = depthTex; };
    
    texture previousDepthTex { Width = INPUT_WIDTH; Height = INPUT_HEIGHT; Format = R32F; };
    sampler previousDepthSampler { Texture = previousDepthTex; };

    texture normalTex { Width = INPUT_WIDTH; Height = INPUT_HEIGHT; Format = RGBA8; };
    sampler normalSampler { Texture = normalTex; };
    
    texture resultTex { Width = BUFFER_WIDTH; Height = BUFFER_HEIGHT; Format = RGBA8; };
    sampler resultSampler { Texture = resultTex; };
    
    // RTGI textures
    texture giPassTex { Width = RENDER_WIDTH; Height = RENDER_HEIGHT; Format = RGBA8; MipLevels = 6; };
    sampler giPassSampler { Texture = giPassTex; MinLOD = 0.0f; MaxLOD = 5.0f;};
    
    texture giSmoothPassTex { Width = RENDER_WIDTH; Height = RENDER_HEIGHT; Format = RGBA8; MipLevels = 6; };
    sampler giSmoothPassSampler { Texture = giSmoothPassTex; MinLOD = 0.0f; MaxLOD = 5.0f;};
    
    texture giAccuTex { Width = INPUT_WIDTH; Height = INPUT_HEIGHT; Format = RGBA8; };
    sampler giAccuSampler { Texture = giAccuTex; };
    
    // AO textures
    texture aoPassTex { Width = RENDER_WIDTH; Height = RENDER_HEIGHT; Format = RGBA8; MipLevels = 6; };
    sampler aoPassSampler { Texture = aoPassTex; MinLOD = 0.0f; MaxLOD = 5.0f;};

    texture aoSmoothPassTex { Width = RENDER_WIDTH; Height = RENDER_HEIGHT; Format = RGBA8; MipLevels = 6; };
    sampler aoSmoothPassSampler { Texture = aoSmoothPassTex; MinLOD = 0.0f; MaxLOD = 5.0f; };

    texture aoAccuTex { Width = INPUT_WIDTH; Height = INPUT_HEIGHT; Format = RGBA8; MipLevels = 6; };
    sampler aoAccuSampler { Texture = aoAccuTex; MinLOD = 0.0f; MaxLOD = 5.0f;};

    // SSR textures
    texture ssrPassTex { Width = RENDER_WIDTH; Height = RENDER_HEIGHT; Format = RGBA8; MipLevels = 6; };
    sampler ssrPassSampler { Texture = ssrPassTex; MinLOD = 0.0f; MaxLOD = 5.0f; };

    texture ssrSmoothPassTex { Width = RENDER_WIDTH; Height = RENDER_HEIGHT; Format = RGBA8; MipLevels = 6; };
    sampler ssrSmoothPassSampler { Texture = ssrSmoothPassTex; MinLOD = 0.0f; MaxLOD = 5.0f; };
    
    texture ssrAccuTex { Width = INPUT_WIDTH; Height = INPUT_HEIGHT; Format = RGBA8; };
    sampler ssrAccuSampler { Texture = ssrAccuTex; };
    

// Internal Uniforms
    uniform float frametime < source = "frametime"; >;
    uniform int framecount < source = "framecount"; >;
    uniform int random < source = "random"; min = 0; max = NOISE_SIZE; >;

// Parameters
    
    uniform int iDebug <
        ui_category = "Debug";
        ui_type = "combo";
        ui_label = "Display";
        ui_items = "Output\0GI\0AO\0SSR\0Roughness\0Depth\0Normal\0Sky\0Motion\0";
    > = 0;

    // Commons
    uniform bool bRoughness <
        ui_type = "slider";
        ui_category = "Commons Roughness";
        ui_label = "Enable";
    > = true;
    uniform int iRoughnessRadius <
        ui_type = "slider";
        ui_category = "Commons Roughness";
        ui_label = "Radius";
        ui_min = 1; ui_max = 4;
        ui_step = 1;
    > = 2;
    uniform float fRoughnessIntensity <
        ui_type = "slider";
        ui_category = "Commons Roughness";
        ui_label = "Intensity";
        ui_min = 0; ui_max = 1.0;
        ui_step = 0.01;
    > = 0.02;
    
	uniform bool bMotionDetection <
        ui_type = "slider";
        ui_category = "Motion detection";
        ui_label = "Enable";
    > = true;
    uniform int iMotionRadius <
        ui_type = "slider";
        ui_category = "Motion detection";
        ui_label = "Radius";
        ui_min = 1; ui_max = 8;
        ui_step = 1;
    > = 6;

    uniform float fDepthMultiplier <
        ui_type = "slider";
        ui_category = "Common Depth";
        ui_label = "Depth multiplier";
        ui_min = 0.1; ui_max = 10;
        ui_step = 0.1;
    > = 1.0;

    uniform float fSkyDepth <
        ui_type = "slider";
        ui_category = "Common Depth";
        ui_label = "Sky Depth";
        ui_min = 0.0; ui_max = 1.0;
        ui_step = 0.01;
    > = 0.99;
    
    uniform float fWeaponDepth <
        ui_type = "slider";
        ui_category = "Common Depth";
        ui_label = "Weapon Depth ";
        ui_min = 0.0; ui_max = 1.0;
        ui_step = 0.001;
    > = 0.001;
    
// COMMMON RT

    uniform int iFrameAccu <
        ui_type = "slider";
        ui_category = "Common RT";
        ui_label = "Temporal accumulation";
        ui_min = 1; ui_max = 8;
        ui_step = 1;
    > = 4;
    
    uniform float fTemporalMaxDepthDiff <
        ui_type = "slider";
        ui_category = "Common RT";
        ui_label = "Max Depth diff";
        ui_min = 0.0; ui_max = 1.0;
        ui_step = 0.001;
    > = 0.5;
    
    uniform float fRTMaxDist <
        ui_type = "slider";
        ui_category = "Common RT";
        ui_label = "Max distance";
        ui_min = 0.001; ui_max = 2.0;
        ui_step = 0.001;
    > = 1.0;
    
    uniform float fRTBounce <
        ui_type = "slider";
        ui_category = "Common RT";
        ui_label = "Bounce strengh";
        ui_min = 0.0; ui_max = 1.0;
        ui_step = 0.001;
    > = 1.0;
    
    uniform float fRayStepMultiply <
        ui_type = "slider";
        ui_category = "Common RT";
        ui_label = "Step multiply";
        ui_min = 0.01; ui_max = 4.0;
        ui_step = 0.01;
    > = 1.3;


// GI
    
    uniform float fRayStepPrecision <
        ui_type = "slider";
        ui_category = "GI";
        ui_label = "Step Precision";
        ui_min = 100.0; ui_max = 4000;
        ui_step = 0.1;
    > = 2000;
    
    uniform float fSkyColor <
        ui_type = "slider";
        ui_category = "GI";
        ui_label = "Sky color";
        ui_min = 0; ui_max = 1.0;
        ui_step = 0.01;
    > = 0.2;
    
    uniform float fGISaturationWeight <
        ui_type = "slider";
        ui_category = "GI";
        ui_label = "Saturation weight";
        ui_min = 0.0; ui_max = 20.0;
        ui_step = 0.01;
    > = 20.0;

    // AO  
    uniform bool bAOProjected <
        ui_category = "AO";
        ui_label = "Project AO";
    > = true;
    
    uniform bool bAOProjectedLast <
        ui_category = "AO";
        ui_label = "Project AO last";
    > = true;
    
    uniform bool bAONoTemporal<
        ui_category = "AO";
        ui_label = "No temporal accumulation";
    > = false;

    uniform float fAOMultiplier <
        ui_type = "slider";
        ui_category = "AO";
        ui_label = "Multiplier";
        ui_min = 0.0; ui_max = 5;
        ui_step = 0.01;
    > = 1.0;
    
    uniform float fAOPow <
        ui_type = "slider";
        ui_category = "AO";
        ui_label = "Pow";
        ui_min = 0.0; ui_max = 5;
        ui_step = 0.01;
    > = 2.0;
    
    uniform int fAODistance <
        ui_type = "slider";
        ui_category = "AO";
        ui_label = "Distance";
        ui_min = 0; ui_max = 1000;
        ui_step = 0.1;
    > = 200;
    
    uniform float fAOProtect <
        ui_type = "slider";
        ui_category = "AO";
        ui_label = "Light protection";
        ui_min = 0.0; ui_max = 1.0;
        ui_step = 0.01;
    > = 0.85;
    


    // SSR

    uniform bool bSSROut <
        ui_category = "SSR";
        ui_label = "Stretch reflections";
        ui_min = 1.0; ui_max = 10.0;
        ui_step = 1.0;
    > = true;
    
    uniform float fSSRDistNoise <
        ui_type = "slider";
        ui_category = "SSR";
        ui_label = "Distance noise";
        ui_min = 1.0; ui_max = 10.0;
        ui_step = 1.0;
    > = 2.0;


    // Denoising
    uniform int iSmoothRadius <
        ui_type = "slider";
        ui_category = "Denoising";
        ui_label = "Radius";
        ui_min = 0; ui_max = 8;
        ui_step = 1;
    > = 2;
    
    uniform int iSmoothStep <
        ui_type = "slider";
        ui_category = "Denoising";
        ui_label = "Step";
        ui_min = 1; ui_max = 8;
        ui_step = 1;
    > = 3;
    
    uniform int iSmoothSSRStep <
        ui_type = "slider";
        ui_category = "Denoising";
        ui_label = "SSR Step";
        ui_min = 1; ui_max = 8;
        ui_step = 1;
    > = 1;
    
    uniform float fSmoothDistPow <
        ui_type = "slider";
        ui_category = "Denoising";
        ui_label = "Distance Weight";
        ui_min = 0.001; ui_max = 8.00;
        ui_step = 0.001;
    > = 1.4;
    
    uniform float fDepthWeight <
        ui_type = "slider";
        ui_category = "Denoising";
        ui_label = "Depth Weight";
        ui_min = 0.001; ui_max = 1.00;
        ui_step = 0.001;
    > = 0.001;

    uniform float fNormalWeight <
        ui_type = "slider";
        ui_category = "Denoising";
        ui_label = "Normal weight";
        ui_min = 0; ui_max = 4.0;
        ui_step = 0.001;
    > = 0.5;
    
    // Merging
        
    uniform float fDistanceFading <
        ui_type = "slider";
        ui_category = "Merging";
        ui_label = "Distance fading";
        ui_min = 0.0; ui_max = 1.0;
        ui_step = 0.01;
    > = 0.75;
        
    uniform float fGILightMerging <
        ui_type = "slider";
        ui_category = "Merging";
        ui_label = "GI Light";
        ui_min = 0.01; ui_max = 10.0;
        ui_step = 0.01;
    > = 1.0;
    uniform float fGIDarkMerging <
        ui_type = "slider";
        ui_category = "Merging";
        ui_label = "GI Dark";
        ui_min = 0.01; ui_max = 10.0;
        ui_step = 0.01;
    > = 2.5;
    
    uniform float fMergingSSR <
        ui_type = "slider";
        ui_category = "Merging";
        ui_label = "SSR";
        ui_min = 0; ui_max = 1.0;
        ui_step = 0.001;
    > = 0.5;
    
    uniform float fMergingRoughness <
        ui_type = "slider";
        ui_category = "Merging";
        ui_label = "SSR Roughness";
        ui_min = 0; ui_max = 10.0;
        ui_step = 0.001;
    > = 3.0;

// FUCNTIONS
// Color spaces
    float RGBCVtoHUE(in float3 RGB, in float C, in float V) {
        float3 Delta = (V - RGB) / C;
        Delta.rgb -= Delta.brg;
        Delta.rgb += float3(2,4,6);
        Delta.brg = step(V, RGB) * Delta.brg;
        float H;
        H = max(Delta.r, max(Delta.g, Delta.b));
        return frac(H / 6);
    }

    float3 RGBtoHSL(in float3 RGB) {
        float3 HSL = 0;
        float U, V;
        U = -min(RGB.r, min(RGB.g, RGB.b));
        V = max(RGB.r, max(RGB.g, RGB.b));
        HSL.z = ((V - U) * 0.5);
        float C = V + U;
        if (C != 0)
        {
            HSL.x = RGBCVtoHUE(RGB, C, V);
            HSL.y = C / (1 - abs(2 * HSL.z - 1));
        }
        return HSL;
    }
      
    float3 HUEtoRGB(in float H) 
    {
        float R = abs(H * 6 - 3) - 1;
        float G = 2 - abs(H * 6 - 2);
        float B = 2 - abs(H * 6 - 4);
        return saturate(float3(R,G,B));
    }
      
    float3 HSLtoRGB(in float3 HSL)
    {
        float3 RGB = HUEtoRGB(HSL.x);
        float C = (1 - abs(2 * HSL.z - 1)) * HSL.y;
        return (RGB - 0.5) * C + HSL.z;
    }

// Vector operations
    int getPixelIndex(float2 coords,int2 size) {
        int2 pxCoords = coords*size;
        pxCoords+= int2((framecount*random*SQRT2),(framecount*random*PI));
        
        return pxCoords.x+pxCoords.y*size.x;
    }

    float randomValue(inout uint seed) {
        seed = seed * 747796405 + 2891336453;
        uint result = ((seed>>((seed>>28)+4))^seed)*277803737;
        result = (result>>22)^result;
        return result/4294967295.0;
    }

    float randomNormalDistribution(inout uint seed) {
        float theta = 2*PI*randomValue(seed);
        float rho = sqrt(-2*log(randomValue(seed)));
        return rho*cos(theta);
    }

    float3 randomHemispehreVector(float2 coords) {
        uint seed = getPixelIndex(coords,RENDER_SIZE);
        float3 v = 0;
        v.x = randomNormalDistribution(seed);
        v.y = randomNormalDistribution(seed);
        v.z = randomNormalDistribution(seed);
        return normalize(v);
    }

    float maxOf3(float3 a) {
        return max(max(a.x,a.y),a.z);
    }
    
    float minOf3(float3 a) {
        return min(min(a.x,a.y),a.z);
    }
    
    float getBrightness(float3 color) {
    	return maxOf3(color);
    }
    
    float getPureness(float3 color) {
    	return 1.0+minOf3(color)-maxOf3(color);
    }
    
    float3 max3(float3 a,float3 b) {
        return float3(max(a.x,b.x),max(a.y,b.y),max(a.z,b.z));
    }
    
    float3 min3(float3 a,float3 b) {
        return float3(min(a.x,b.x),min(a.y,b.y),min(a.z,b.z));
    }
    
    float getMaxAo(sampler sourceAOSampler, float2 coords) {
	    float4 ao4 = tex2DgatherR(sourceAOSampler,coords);
	    return min(ao4.a,minOf3(ao4.xyz));
    }
    
    float getAvgAo(sampler sourceAOSampler, float2 coords) {
	    float4 ao4 = tex2DgatherR(sourceAOSampler,coords);
	    return (ao4.x+ao4.y*0.25+ao4.z*0.25+ao4.a)/2.5;
    }


// Screen
    float2 InputPixelSize() {
        float2 result = 1.0;
        return result/float2(INPUT_WIDTH,INPUT_HEIGHT);
    }
    
    float2 RenderPixelSize() {
        float2 result = 1.0;
        return result/float2(RENDER_WIDTH,RENDER_HEIGHT);
    }

    bool inScreen(float2 coords) {
        return coords.x>=0 && coords.x<=1
            && coords.y>=0 && coords.y<=1;
    }
    
    bool inScreen(float3 coords) {
        return coords.x>=0 && coords.x<=1
            && coords.y>=0 && coords.y<=1
            && coords.z>=0 && coords.z<=1;
    }

    float getDepth(float2 coords) {
        return getColorSampler(depthSampler,coords).x;
    }
    
    float getPreviousDepth(float2 coords) {
        return getColorSampler(previousDepthSampler,coords).x;
    }

    float3 getWorldPosition(float2 coords) {
        float depth = getDepth(coords);
        float3 result = float3((coords-0.5)*depth,depth);
        result *= BUFFER_SIZE3;
        return result;
    }

    float3 getScreenPosition(float3 wp) {
        float3 result = wp/BUFFER_SIZE3;
        result.xy /= result.z;
        return float3(result.xy+0.5,result.z);
    }

    float3 computeNormal(float2 coords,float3 offset) {
        float3 posCenter = getWorldPosition(coords);
        float3 posNorth  = getWorldPosition(coords - offset.zy);
        float3 posEast   = getWorldPosition(coords + offset.xz);
        return  normalize(cross(posCenter - posNorth, posCenter - posEast));
    }
    
    float3 getRayColor(float2 coords) {
        float3 color = getColor(coords).rgb;
        if(fRTBounce>0) {
        	float2 previousCoords = getColorSampler(motionSampler,coords).xy;
            float3 previous = getColorSampler(resultSampler,bMotionDetection?previousCoords:coords).rgb;
            color = fRTBounce*previous + (1.0-fRTBounce)*color;
        }
        return color;
    }
    
    float3 getGiColor(float2 coords) {
    	return getColorSampler(giAccuSampler,coords).rgb;
    }

// SSR 
    float getRoughness(float2 coords) {
        return abs(getColorSampler(roughnessSampler,coords).r-0.5)*2;
    }

// GI

// RT
    float4 trace(float3 refWp,float3 lightVector,float startDepth,bool ssr) {

		float3 sp = getScreenPosition(refWp);
        float3 startNormal = getNormal(sp.xy);
                
        float stepRatio = 1.001+fRayStepMultiply/10.0;
        
        float stepLength = 1.0/(ssr?200:fRayStepPrecision);
        float3 startIncrementVector = lightVector*stepLength;
        float3 incrementVector = startIncrementVector;
        float traceDistance = 0;
        float3 currentWp = refWp;
        
        float rayHitIncrement = 50.0*startDepth*fRayHitDepthThreshold/50.0;
        float rayHitDepthThreshold = rayHitIncrement;

        int crossed = 0;
        float deltaZ = 0.0;
        float deltaZbefore = 0.0;
        
        float3 lastCross;
        float bestCrossDist = 0.0;
        
        bool skyed = false;
        float3 lastSky;
        
        bool outSource = false;
        bool firstStep = true;
        
        bool startWeapon = startDepth<fWeaponDepth;
        float weaponLimit = fWeaponDepth*BUFFER_SIZE3.z;
        
        float3 screenCoords;
        do {
            currentWp += incrementVector;
            traceDistance += stepLength;
            
            screenCoords = getScreenPosition(currentWp);
            
            bool outScreen = !inScreen(screenCoords) && (!startWeapon || currentWp.z<weaponLimit);
            
            float3 screenWp = getWorldPosition(screenCoords.xy);
            
            deltaZ = screenWp.z-currentWp.z;
            
            float depth = getDepth(screenCoords.xy);
            bool isSky = depth>fSkyDepth;
            if(isSky) {
                skyed = true;
                lastSky = currentWp;
            }
            
            
            if(firstStep && deltaZ<0 && !ssr) {
                // wrong direction
                currentWp = refWp-incrementVector;
                startIncrementVector = reflect(incrementVector,startNormal);
                incrementVector = startIncrementVector;
                
                currentWp = refWp+incrementVector;
                //traceDistance = 0;

                firstStep = false; 
            } else if(outSource) { 

				bool hit = false;               
                
                if(!outScreen && sign(deltaZ)<sign(deltaZbefore)) {
                
                	float preciseRatio = 0.25;
                    float3 preciseIncrementVector = incrementVector;
                    float preciseLength = stepLength;
                    while(!hit && length(preciseIncrementVector)>length(incrementVector*0.1)) {
                        preciseIncrementVector *= preciseRatio;
                        preciseLength *= preciseRatio;
                        
                        bool recrossed=false;
                        int preciseStep = 0;
                        while(!recrossed && preciseStep<4) {
                            currentWp -= preciseIncrementVector;
                            screenCoords = getScreenPosition(currentWp);
			                screenWp = getWorldPosition(screenCoords.xy);
                            traceDistance -= preciseLength;
                            deltaZ = screenWp.z-currentWp.z;
                            recrossed = sign(deltaZ)==sign(deltaZbefore);
                            preciseStep++;
                        }
                        
						currentWp += preciseIncrementVector;
                        screenCoords = getScreenPosition(currentWp);
                        screenWp = getWorldPosition(screenCoords.xy);
                        traceDistance += preciseLength;
                        deltaZ = screenWp.z-currentWp.z;
                        
                        hit = abs(deltaZ)<=rayHitDepthThreshold;

                        if(preciseStep==4) break;
                    }
                    
                    float crossDist = abs(deltaZ);
                    if(crossed==0 || bAOProjectedLast || crossDist<bestCrossDist) {
                    	bestCrossDist = crossDist;
                    	lastCross = currentWp;
                    }
                    
                    crossed++;                    
                }
                
                if(outScreen) {
                    if(crossed) {
                        return float4(lastCross,RT_MISSED_CROSSED);
                    }
                    if(skyed) {
                        return float4(lastSky,RT_HIT_SKY);
                    }
                    
                    currentWp -= incrementVector;
                    return float4(currentWp, RT_MISSED);
                    
                } 
                if(hit || (OPTIMIZATION_CROSS_TESTS>0 && crossed>=OPTIMIZATION_CROSS_TESTS)) {
                    // hit !
                    return float4(crossed ? lastCross : currentWp, RT_HIT);
                }
            } else {
                if(outScreen) {
                    if(crossed) {
                        return float4(lastCross,RT_MISSED_CROSSED);
                    }
                    if(skyed) {
                        return float4(lastSky,RT_HIT_SKY);
                    }
                    currentWp -= incrementVector;
                    return float4(currentWp, RT_MISSED);
                }
                outSource = deltaZ>rayHitDepthThreshold*(pow(startDepth,0.5)*4);
                if(!outSource) {
                 //   float3 normal = getNormal(screenCoords);
                }
            }

            firstStep = false;
            
            deltaZbefore = deltaZ;
            
            stepLength *= stepRatio;
            if(rayHitDepthThreshold<fRayHitDepthThreshold) rayHitDepthThreshold +=rayHitIncrement;
            incrementVector *= stepRatio;

        } while(traceDistance<fRTMaxDist*INPUT_WIDTH*2.0*(ssr?1.0:0.001));

        return float4(0.5,0.5,0.5,RT_MISSED);

    }


// PS

    void PS_RoughnessDepthPass(float4 vpos : SV_Position, float2 coords : TexCoord, out float4 outRoughness : SV_Target0, out float4 outDepth : SV_Target1, out float4 outMotion : SV_Target2) {
        float depth = ReShade::GetLinearizedDepth(coords);
        float3 refColor = getColor(coords).rgb;
        
		if(!bRoughness) {
            outRoughness = float4(0.5,0.5,0.5,1.0);
            outDepth = float4(depth,depth,depth,1.0);
        } else {
            
            float refB = getBrightness(refColor);
            float roughness = 0.0;

            float tempA = 0;
            float tempB = 0;
            
            float3 previousX = refColor;
            float3 previousY = refColor;
            for(int d = 1;d<=iRoughnessRadius;d++) {
                float3 color = getColor(float2(coords.x+ReShade::PixelSize.x*d,coords.y)).rgb;
                float3 diff = previousX-color;
                float r = maxOf3(diff)/pow(d,0.5);
                tempA += abs(r);
                tempB = abs(r)>abs(tempB) ? r : tempB;
                previousX = color;
                
                color = getColor(float2(coords.x,coords.y+ReShade::PixelSize.y*d)).rgb;
                diff = previousY-color;
                r = maxOf3(diff)/pow(d,0.5);
                tempA += abs(r);
                tempB = abs(r)>abs(tempB) ? r : tempB;
                previousY = color;
            }
            previousX = refColor;
            previousY = refColor;
            for(int d = 1;d<=iRoughnessRadius;d++) {
                float3 color = getColor(float2(coords.x-ReShade::PixelSize.x*d,coords.y)).rgb;
                float3 diff = previousX-color;
                float r = maxOf3(diff)/pow(d,0.5);
                tempA += abs(r);
                tempB = abs(r)>abs(tempB) ? r : tempB;
                previousX = color;
                
                color = getColor(float2(coords.x,coords.y-ReShade::PixelSize.y*d)).rgb;
                diff = previousY-color;
                r = maxOf3(diff)/pow(d,0.5);
                tempA += abs(r);
                tempB = abs(r)>abs(tempB) ? r : tempB;
                previousY = color;
            }
            tempA /= iRoughnessRadius;
            tempA *= sign(tempB);
            roughness = tempA;
            
            //roughness /= 1+(iRoughnessRadius-1)*4;
            if(abs(roughness)>0.01 && depth<fSkyDepth) {
                depth += (log(2.0+depth)-log(2.0))*min(0.002,roughness*fRoughnessIntensity);
            }
            roughness = roughness/2+0.5;

            outRoughness = float4(roughness,roughness,roughness,1.0);
            outDepth = float4(depth,depth,depth,1.0);//1.0/iFrameAccu);
        }
        
        if(bMotionDetection) {
	    	float bestDist = 1000.0;
	    	float bestLength = iMotionRadius*8;
	    	float2 bestMotion = coords;
	        int2 delta;
	        for(delta.x=-iMotionRadius;delta.x<=iMotionRadius;delta.x++) {
	            for(delta.y=-iMotionRadius;delta.y<=iMotionRadius;delta.y++) {
	            	float2 currentCoords = coords+ReShade::PixelSize*delta*8;
	            	float currentDepth = getPreviousDepth(coords);
	            	float3 currentColor = getColorSampler(previousColorSampler,currentCoords).rgb;
	            	float3 diffColor = abs(currentColor-refColor);
	            	float length = 1+length(delta);
	            	float dist = (abs(depth-currentDepth)*0.5+maxOf3(diffColor));//*length;
	            	if(dist<bestDist || (dist==bestDist && length<bestLength)) {
	            		bestDist = dist;
	            		bestLength = length;
	            		bestMotion = currentCoords;
	            	}
	            }
	        }
	        outMotion = float4(bestMotion,bestDist,1.0);
        } else {
        	outMotion = float4(coords,0.0,1.0);
        }
        
    }

    void PS_NormalPass(float4 vpos : SV_Position, float2 coords : TexCoord, out float4 outNormal : SV_Target0) {
        
        float3 offset = float3(ReShade::PixelSize, 0.0);
        
        float3 normal = computeNormal(coords,offset);

        #if NORMAL_FILTER
            float depth = getDepth(coords);
            
            float3 normalTop = computeNormal(coords-offset.zy,offset);
            float3 normalBottom = computeNormal(coords+offset.zy,offset);
            float3 normalLeft = computeNormal(coords-offset.xz,offset);
            float3 normalRight = computeNormal(coords+offset.xz,offset);
            normal += normalTop+normalBottom+normalLeft+normalRight;
            normal/=5.0;
        #endif
        
        outNormal = float4(normal/2.0+0.5,1.0);
        
    }
    
    void PS_DisplayResult(in float4 position : SV_Position, in float2 coords : TEXCOORD, out float4 outPixel : SV_Target)
    {
        if(iDebug==DEBUG_OFF) {
            outPixel = getColorSampler(resultSampler,coords);
        } else if(iDebug==DEBUG_GI) {
            float3 gi = getGiColor(coords);
            float ao = getColorSampler(aoAccuSampler,coords).x;
            ao = pow(ao,fAOMultiplier);
            
            // Protect light from AO
            float b =  getBrightness(gi);
            ao = 1.0-(1.0-ao)*(1.0-b);
                
    		gi *= ao;
    		
            outPixel = float4(gi,1.0);
        } else if(iDebug==DEBUG_AO) {
            float3 ao = getColorSampler(aoAccuSampler,coords).r;
            float3 gi = getGiColor(coords);
            
            // Protect light from AO
            float b =  getBrightness(gi);
            ao = 1.0-(1.0-ao)*(1.0-b);

            outPixel = float4(ao,1);
        } else if(iDebug==DEBUG_SSR) {
            outPixel = getColorSampler(ssrAccuSampler,coords);
        } else if(iDebug==DEBUG_ROUGHNESS) {
            outPixel = getColorSampler(roughnessSampler,coords);
        } else if(iDebug==DEBUG_DEPTH) {
            float depth = getDepth(coords);
            outPixel = float4(depth,depth,depth,1);
        } else if(iDebug==DEBUG_NORMAL) {
            outPixel = getColorSampler(normalSampler,coords);
        } else if(iDebug==DEBUG_SKY) {
        	float depth = getColorSampler(depthSampler,coords).r;
        	float sky = depth>fSkyDepth;
            outPixel = float4(sky,sky,sky,1.0);
        } else if(iDebug==DEBUG_MOTION) {
        	float3 motion = getColorSampler(motionSampler,coords).rgb;
            outPixel = float4(motion,1.0);
        }
    }

// GI

    void PS_GILightPass(float4 vpos : SV_Position, float2 coords : TexCoord, out float4 outGI : SV_Target0, out float4 outAO : SV_Target1) {
        
        if(fGILightMerging==0.0 && fGILightMerging==0.0) {
            outGI = float4(0,0,0,1); 
            outAO = float4(1,0,0,1);
            return;
        }
        
        float depth = ReShade::GetLinearizedDepth(coords);
        if(depth>fSkyDepth) {
            outGI = getColor(coords);   
            outAO = float4(1,0,0,1);
            return;
        }
        
        float3 refWp = getWorldPosition(coords);
	    float3 normal = getNormal(coords);
	    
	    float4 hitPosition;
	    float3 screenCoords;
	    
	    float opacity = 1.0;
	    float aoOpacity = bAONoTemporal? 1.0 : 1.0/iFrameAccu;
        
        float3 randomVector = randomHemispehreVector(coords);

        float3 lightVector = reflect(refWp,randomVector);
        #if OPTIMIZATION_BIAISED_RT
        	float ratio = 1.0-dot(normal,float3(0,0,1));
			lightVector += float3(0,0,ratio*RESHADE_DEPTH_LINEARIZATION_FAR_PLANE/(2.0-depth));
		#endif
        
        
        hitPosition = trace(refWp,lightVector,depth,false);
        screenCoords = getScreenPosition(hitPosition.xyz);   
        
        float d;
        float3 giColor = 0;
        float2 previousCoords = bMotionDetection ? getColorSampler(motionSampler,coords).xy : coords;
        float3 previousGI = getGiColor(previousCoords);
		
		if(hitPosition.a==RT_HIT || hitPosition.a==RT_MISSED_CROSSED) {
        	giColor = getRayColor(screenCoords.xy).rgb;
            d = abs(distance(hitPosition.xyz,refWp));
        } else if(fSkyColor>0 && hitPosition.a==RT_HIT_SKY) {
        	giColor = getColor(screenCoords.xy).rgb*fSkyColor;
            d = abs(distance(hitPosition.xyz,refWp));
        } else if(hitPosition.a==RT_MISSED) {
        	giColor = previousGI;
            	
            d = abs(distance(hitPosition.xyz,refWp))*100;
        }
        
        float b = getBrightness(giColor);
        
		float giOpacity = 1.0/iFrameAccu;
		

		float previousB = getBrightness(previousGI);
		
		giOpacity *= pow(((b*0.9+0.1)+(1.0-previousB)),5.0);
		
		if(d>0) {
			giOpacity *= 100.0/d;
		} else if(d<0) {
			giOpacity = 1.0/iFrameAccu;
		}
		
		float previousDepth = getPreviousDepth(coords);
		float diffDepth = saturate(abs(depth-previousDepth)*10);		
		if(!bMotionDetection && diffDepth>fTemporalMaxDepthDiff) {
			giOpacity = 1.0-diffDepth;
			aoOpacity = 1.0-diffDepth;
		}
		//float r = diffDepth>fTemporalMaxDepthDiff*0.1?diffDepth*0.01:1.0;
		//giOpacity /= r;
		
		//giColor /= saturate(b*6);
		giColor = saturate(giColor);
		giOpacity = saturate(giOpacity);
		
		outGI = float4(giColor,giOpacity);

        //outGI = saturate(float4(hitPosition.rgb,1));
        if(hitPosition.a>=(bAOProjected?0.5:1.0)) {
          
         // if(screenCoords.z>=0.9) {
          //  outAO = float4(fAODistance>0?0:1,0,0,aoOpacity);
         // } else {
            if(d>fAODistance) {
                outAO = float4(1.0,1.0,1.0,aoOpacity);
            } else {
                float r = saturate(d/fAODistance);

                outAO = float4(1.0-pow(1.0-r,fAOPow),0,0,aoOpacity);
             //   outAO = float4(d/fAODistance,0,0,aoOpacity*(1.0-d/fAODistance));
            }
            
         // }
        } else {
            outAO = float4(1,1,1,aoOpacity);
        }
    }

// SSR
    void PS_SSRLightPass(float4 vpos : SV_Position, float2 coords : TexCoord, out float4 outColor : SV_Target0) {
        if(fMergingSSR==0.0) {
            outColor = float4(0,0,0,1);
            return;
        }
        
        float depth = getDepth(coords);
        if(depth>fSkyDepth) {
            outColor = float4(0,0,0,1);
        } else {
            float previousDepth = getPreviousDepth(coords);
            float opacity = abs(previousDepth-depth)>fTemporalMaxDepthDiff*10. ? 1.0 : 1.0/iFrameAccu;
        
            float3 targetWp = getWorldPosition(coords);
            float3 targetNormal = getNormal(coords);
    
            float3 lightVector = reflect(targetWp,targetNormal);
            
	        float3 randomVector = randomHemispehreVector(coords);
            lightVector += randomVector*fSSRDistNoise;
             
            lightVector = normalize(lightVector);
            
            float4 hitPosition = trace(targetWp,lightVector,depth,true);
            if(hitPosition.a==RT_MISSED && !bSSROut) {
                // no hit
                outColor = float4(0,0,0,opacity);
            } else {
                float3 screenCoords = getScreenPosition(hitPosition.xyz);
                float3 color = getRayColor(screenCoords.xy);
                float3 normal = getNormal(coords);
				float angle = max(0.25,1.0-dot(normal,normalize(float3(coords-0.5,1))));
                
                outColor = float4(color*angle,opacity);
            }
        }
    }
    
    void smooth(
    	int passNumber,
        sampler sourceGISampler,
        sampler sourceAOSampler,
        sampler sourceSSRSampler,
        float2 coords, out float4 outGI, out float4 outAO, out float4 outSSR,bool firstPass) {
        
        float refDepth = getDepth(coords);
        if(refDepth>fSkyDepth) {
            outGI = float4(getColor(coords).rgb,1.0);
            outAO = 1.0;
            outSSR = float4(0,0,0,1);
            return;
        }
        
        float3 refGI = getColorSampler(sourceGISampler,coords).rgb;
        float refBrightness = getBrightness(refGI);
       
        float opacity = 1.0/iFrameAccu;
        float aoOpacity = opacity;
        if(bAONoTemporal) {
        	aoOpacity = 1.0;
        }
        
        opacity *= 0.5+refBrightness*0.5;
        //aoOpacity *= 0.5+refBrightness*0.5;
        
        float refAo = getColorSampler(sourceAOSampler,coords).r;
        
        if(iSmoothRadius==0) {
            outGI = float4(refGI,opacity);
            outAO = float4(refAo,refAo,refAo,opacity);
            outSSR = float4(getColorSampler(sourceSSRSampler,coords).rgb,opacity);
            return;
        }
        
        int2 coordsInt = coords*RENDER_SIZE;
        
        float3 refNormal = getNormal(coords);
        
         
        float3 gi = 0;
        float giWeightSum = 1;
        
        float ao = 0;
        float aoWeightSum = 0;
        
        float3 ssr;
        float ssrWeightSum = 0;
        
        float2 pixelSize = RenderPixelSize();
        
        float2 minCoords = saturate(coords-iSmoothRadius*pixelSize);
        float2 maxCoords = saturate(coords+iSmoothRadius*pixelSize);
        float2 currentCoords = minCoords;
        
        int2 delta;
        
        
        for(delta.x=-iSmoothRadius;delta.x<=iSmoothRadius;delta.x++) {
            for(delta.y=-iSmoothRadius;delta.y<=iSmoothRadius;delta.y++) {
                
                float2 ssrCurrentCoords = coords+delta*pixelSize*(firstPass ? iSmoothSSRStep : 1);
                currentCoords = coords+delta*pixelSize*(firstPass ? iSmoothStep : 1);
                float dist = distance(0,delta);
                
                if(dist>iSmoothRadius) continue;
            
                
                float depth = getDepth(currentCoords);
                if(depth>fSkyDepth) continue;
                    
                    float3 normal = getNormal(currentCoords);
                        
                    // GI & AO
                    
                    float4 color = getColorSamplerLod(sourceGISampler,currentCoords,firstPass ? (iSmoothStep-1.0)*0.5 : 0.0);
                    float4 ssrColor = getColorSamplerLod(sourceSSRSampler,ssrCurrentCoords, firstPass ? (iSmoothSSRStep-1.0)*0.5 : 0.0);
                    float aoColor = getColorSamplerLod(sourceAOSampler,currentCoords,1.0+(firstPass ? (iSmoothSSRStep-1.0)*0.5:0.0)).r;
                   
                    float ssrWeight = maxOf3(ssrColor.rgb)>0 ? 1.0 : 0.0;
                    float aoWeight = 1.0;
                    float giWeight = color.a;                       
                    
                    float distWeight = abs(pow(1.0+iSmoothRadius/(dist+1),fSmoothDistPow));
                    
                    giWeight *= distWeight;
                    aoWeight *= distWeight;
                    ssrWeight *= distWeight;

                    
                    float b = getBrightness(color.rgb);
                    giWeight *= .9+b;
                    if(fGISaturationWeight>0) {
                    	giWeight += fGISaturationWeight*(maxOf3(color.rgb)-minOf3(color.rgb));
                    }
                    
                    if(true) { // Normal weight
                        float3 t = normal-refNormal;
                        float dist2 = max(dot(t,t), 0.0);
                        float nw = min(exp(-(dist2)/(fNormalWeight)), 1.0);
                        
                        giWeight *= nw*nw;
                        aoWeight *= nw*nw;
                        ssrWeight *= nw*nw;
                    }
                    
                    if(true) { // Depth weight
                        float3 t = depth-refDepth;
                        float dist2 = max(dot(t,t), 0.0);
                        float dw = min(exp(-(dist2)/(fDepthWeight)), 1.0);                                
                    
                        giWeight *= dw*dw;
                        aoWeight *= dw*dw;
                        ssrWeight *= dw*dw;
                    }
                     
					ao += aoColor.r*aoWeight;
		            aoWeightSum += aoWeight;
	                
                    gi += color.rgb*giWeight;
                    giWeightSum += giWeight;
                    
                    ssr += ssrColor.rgb * ssrWeight;
                    ssrWeightSum += ssrWeight;
                        
            } // end for y
        } // end for x

        //outAO = float4(resultAO,resultAO,resultAO,resultAO<1.0 ? 1.0-resultAO : 1.0);
        
        //result = float(foundSamples)/10;//saturate(fLightMult*result/weightSum);
        //result = saturate(fLightMult*result/weightSum);
        gi /= giWeightSum;
        ao /= aoWeightSum;
        ssr /= ssrWeightSum;
        
        if(bMotionDetection && passNumber==1) {
        	float3 previousCoords = getColorSampler(motionSampler,coords).xyz;
        	float3 previousGi = getGiColor(previousCoords.xy);
        	if(previousCoords.z>0) {
        	//	opacity = saturate(opacity+previousCoords.z);
        	}
        	outGI = float4(gi*opacity+previousGi*(1.0-opacity),1.0);
        
        	float previousAo = getColorSampler(aoAccuSampler,previousCoords).r;
        	
        	//ao = ao*aoOpacity+previousAo*(1-aoOpacity);
        	ao = min(ao,previousAo+0.1);
        	outAO = float4(ao,ao,ao,1.0);
        } else if(bMotionDetection && passNumber==2) {
        	outGI = float4(gi,1);
        	outAO = float4(ao,ao,ao,1);
        } else {
            outGI = float4(gi,opacity);
        	outAO = float4(ao,ao,ao,aoOpacity);
        }
        outSSR = float4(ssr,1);

    }
    
    void PS_SmoothPass(float4 vpos : SV_Position, float2 coords : TexCoord, out float4 outGI : SV_Target0, out float4 outAO : SV_Target1, out float4 outSSR : SV_Target2) {
        smooth(1,giPassSampler,aoPassSampler,ssrPassSampler,coords,outGI,outAO,outSSR, true);
    }
    
    void PS_AccuPass(float4 vpos : SV_Position, float2 coords : TexCoord, out float4 outGI : SV_Target0, out float4 outAO : SV_Target1, out float4 outSSR : SV_Target2) {
        smooth(2,giSmoothPassSampler,aoSmoothPassSampler,ssrSmoothPassSampler,coords,outGI,outAO,outSSR, false);
    }

    void PS_UpdateResult(in float4 position : SV_Position, in float2 coords : TEXCOORD, out float4 outResult : SV_Target, out float4 outDepth : SV_Target1, out float4 outPreviousColor : SV_Target2) {
        float depth = getDepth(coords);
        float3 color = getColor(coords).rgb;
        
        outPreviousColor = float4(color,1.0);
        
        if(depth>fSkyDepth) {
            outResult = float4(color,1.0);
            outDepth = float4(depth,depth,depth,1.0);
        } else {
            float3 colorHsl = RGBtoHSL(color);
            
            
            float3 gi = getGiColor(coords);
            float3 giHsl = RGBtoHSL(gi);
            
            float b =  getBrightness(gi);
        	float bColor = getBrightness(color);
            float3 ao = getColorSampler(aoAccuSampler,coords).x;
            ao = pow(ao,fAOMultiplier);
            
            // Protect light from AO
            ao = ao*(1.0-fAOProtect)+fAOProtect*(1.0-(1.0-ao)*(1.0-max(b,bColor)));
    		
            float originalColor = saturate(pow(abs(colorHsl.z*2.0-1.0),10));
            
            // Base color
            float3 result = 0;
            
            // GI
        	float pureness = getPureness(color);
        	float giPureness = getPureness(gi);
    		
            
            
        	float3 dark = saturate((color+gi)*(color/10+1.0));
        	float3 light = saturate((color+0.1)*gi);
        	result = 0;
        	
        	float3 rDark = pow(color,1.0/(fGIDarkMerging+0.001));
    		result += (1.0-bColor)*(rDark*dark+(1.-rDark)*color);
    		float3 rLight = giHsl.z*giHsl.y*colorHsl.z*fGILightMerging;
    		light = colorHsl;
    		light.x = giHsl.x;
    		light.z = 0.5;
    		light = HSLtoRGB(light);
    		result += bColor*(rLight*light+color*(1.0-rLight));
        	result = result*(1.0-originalColor)+originalColor*color;
        	
        	// Apply AO
			result *= ao;
        	
            // SSR
            float3 ssr = getColorSampler(ssrAccuSampler,coords).rgb;
            float roughness = getRoughness(coords);
            
            float3 fixedDarkHsl = colorHsl;
            fixedDarkHsl.z = 1.0-pow(1.0-colorHsl.z,fMergingRoughness);
            float3 fixedDark = HSLtoRGB(fixedDarkHsl);
            
            float ssrRatio2 = fixedDarkHsl.z/fMergingRoughness;
            float ssrRatio1 = max(0.0,1.0-(roughness+0.1)*fMergingRoughness);
            
            result += max(0,ssrRatio1-ssrRatio2)*ssr*fMergingSSR*(1.0-originalColor);
            
            // Distance fading
            if(fDistanceFading<1.0 && depth>fDistanceFading) {
                float diff = depth-fDistanceFading;
                float max = 1.0-fDistanceFading;
                float ratio = diff/max;
                result = result*(1.0-ratio)+color*ratio;
            }
            
            outResult = float4(result,1);
            outDepth = float4(depth,depth,depth,1.0);
        }
    }
    


// TEHCNIQUES 
    technique DH_UBER_RT {
        // Normal Roughness
        pass {
            VertexShader = PostProcessVS;
            PixelShader = PS_RoughnessDepthPass;
            RenderTarget = roughnessTex;
            RenderTarget1 = depthTex;
            RenderTarget2 = motionTex;
        }
        pass {
            VertexShader = PostProcessVS;
            PixelShader = PS_NormalPass;
            RenderTarget = normalTex;
        }

        // GI
        pass {
            VertexShader = PostProcessVS;
            PixelShader = PS_GILightPass;
            RenderTarget = giPassTex;
            RenderTarget1 = aoPassTex;
            
            ClearRenderTargets = false;
                        
            BlendEnable = true;
            BlendOp = ADD;
            SrcBlend = SRCALPHA;
            SrcBlendAlpha = ONE;
            DestBlend = INVSRCALPHA;
            DestBlendAlpha = ONE;
        }
        
        // SSR
        pass {
            VertexShader = PostProcessVS;
            PixelShader = PS_SSRLightPass;
            RenderTarget = ssrPassTex;
            
            ClearRenderTargets = false;
                        
            BlendEnable = true;
            BlendOp = ADD;
            SrcBlend = SRCALPHA;
            SrcBlendAlpha = ONE;
            DestBlend = INVSRCALPHA;
            DestBlendAlpha = ONE;
        }
        
        // Denoising
        pass {
            VertexShader = PostProcessVS;
            PixelShader = PS_SmoothPass;
            RenderTarget = giSmoothPassTex;
            RenderTarget1 = aoSmoothPassTex;
            RenderTarget2 = ssrSmoothPassTex;
            
            
            ClearRenderTargets = false;
                        
            BlendEnable = true;
            BlendOp = ADD;
            SrcBlend = SRCALPHA;
            SrcBlendAlpha = ONE;
            DestBlend = INVSRCALPHA;
            DestBlendAlpha = ONE;
        }
        pass {
            VertexShader = PostProcessVS;
            PixelShader = PS_AccuPass;
            RenderTarget = giAccuTex;
            RenderTarget1 = aoAccuTex;
            RenderTarget2 = ssrAccuTex;
            
            ClearRenderTargets = false;
                        
            BlendEnable = true;
            BlendOp = ADD;
            SrcBlend = SRCALPHA;
            SrcBlendAlpha = ONE;
            DestBlend = INVSRCALPHA;
            DestBlendAlpha = ONE;
        }
        
        // Merging
        pass {
            VertexShader = PostProcessVS;
            PixelShader = PS_UpdateResult;
            RenderTarget = resultTex;
            RenderTarget1 = previousDepthTex;
            RenderTarget2 = previousColorTex;
        }
        pass {
            VertexShader = PostProcessVS;
            PixelShader = PS_DisplayResult;
        }
    }
}