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

    
#define iRayPreciseHit 8
#define iRayPreciseHitSteps 4
#define fRayHitDepthThreshold 0.350

#define getNormal(c) (tex2Dlod(normalSampler,float4(c.xy,0,0)).xyz-0.5)*2
#define getColor(c) tex2Dlod(ReShade::BackBuffer,float4(c,0,0))
#define getColorSamplerLod(s,c,l) tex2Dlod(s,float4(c.xy,0,l))
#define getColorSampler(s,c) tex2Dlod(s,float4(c.xy,0,0))
#define getBrightness(c) maxOf3(c)

#define diffT(v1,v2,t) !any(max(abs(v1-v2)-t,0))
namespace DH_UBER_RT {

// Textures
    // Common textures
    texture blueNoiseTex < source ="dh_rt_noise.png" ; > { Width = NOISE_SIZE; Height = NOISE_SIZE; MipLevels = 1; Format = RGBA8; };
    sampler blueNoiseSampler { Texture = blueNoiseTex;  AddressU = REPEAT;  AddressV = REPEAT;  AddressW = REPEAT;};

    texture roughnessTex { Width = INPUT_WIDTH; Height = INPUT_HEIGHT; Format = RGBA8; };
    sampler roughnessSampler { Texture = roughnessTex; };

    texture previousRoughnessTex { Width = INPUT_WIDTH; Height = INPUT_HEIGHT; Format = RGBA8; };
    sampler previousRoughnessSampler { Texture = previousRoughnessTex; };

    texture depthTex { Width = INPUT_WIDTH; Height = INPUT_HEIGHT; Format = R32F; };
    sampler depthSampler { Texture = depthTex; };
    
    texture previousDepthTex { Width = INPUT_WIDTH; Height = INPUT_HEIGHT; Format = R32F; };
    sampler previousDepthSampler { Texture = previousDepthTex; };

    texture normalTex { Width = INPUT_WIDTH; Height = INPUT_HEIGHT; Format = RGBA8; };
    sampler normalSampler { Texture = normalTex; };
    
    texture resultTex { Width = BUFFER_WIDTH; Height = BUFFER_HEIGHT; Format = RGBA8; };
    sampler resultSampler { Texture = resultTex; };
    
    // RTGI textures
    texture giPassTex { Width = RENDER_WIDTH; Height = RENDER_HEIGHT; Format = RGBA8; MipLevels = 4; };
    sampler giPassSampler { Texture = giPassTex; MinLOD = 0.0f; MaxLOD = 3.0f;};
    
    texture giSmoothPassTex { Width = RENDER_WIDTH; Height = RENDER_HEIGHT; Format = RGBA8; MipLevels = 4; };
    sampler giSmoothPassSampler { Texture = giSmoothPassTex; MinLOD = 0.0f; MaxLOD = 3.0f;};
    
    texture giAccuTex { Width = INPUT_WIDTH; Height = INPUT_HEIGHT; Format = RGBA8; };
    sampler giAccuSampler { Texture = giAccuTex; };
    
    // AO textures
    texture aoPassTex { Width = RENDER_WIDTH; Height = RENDER_HEIGHT; Format = RGBA8; MipLevels = 4; };
    sampler aoPassSampler { Texture = aoPassTex; MinLOD = 0.0f; MaxLOD = 3.0f;};

    texture aoSmoothPassTex { Width = RENDER_WIDTH; Height = RENDER_HEIGHT; Format = RGBA8; MipLevels = 4; };
    sampler aoSmoothPassSampler { Texture = aoSmoothPassTex; MinLOD = 0.0f; MaxLOD = 3.0f; };

    texture aoAccuTex { Width = INPUT_WIDTH; Height = INPUT_HEIGHT; Format = RGBA8; MipLevels = 4; };
    sampler aoAccuSampler { Texture = aoAccuTex; MinLOD = 0.0f; MaxLOD = 3.0f; };

    // SSR textures
    texture ssrPassTex { Width = RENDER_WIDTH; Height = RENDER_HEIGHT; Format = RGBA8; MipLevels = 4; };
    sampler ssrPassSampler { Texture = ssrPassTex; MinLOD = 0.0f; MaxLOD = 3.0f; };

    texture ssrSmoothPassTex { Width = RENDER_WIDTH; Height = RENDER_HEIGHT; Format = RGBA8; MipLevels = 4; };
    sampler ssrSmoothPassSampler { Texture = ssrSmoothPassTex; MinLOD = 0.0f; MaxLOD = 3.0f; };
    
    texture ssrAccuTex { Width = INPUT_WIDTH; Height = INPUT_HEIGHT; Format = RGBA8; };
    sampler ssrAccuSampler { Texture = ssrAccuTex; };
    

// Internal Uniforms
    uniform float frametime < source = "frametime"; >;
    uniform int framecount < source = "framecount"; >;
    uniform int random < source = "random"; min = 0; max = NOISE_SIZE; >;

// Parameters

    uniform bool bTest = true;

    
    uniform int iDebug <
        ui_category = "Debug";
        ui_type = "combo";
        ui_label = "Display";
        ui_items = "Output\0GI\0AO\0SSR\0Roughness\0Depth\0Normal\0";
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

    uniform bool bNormalFilter <
        ui_type = "slider";
        ui_category = "Common Depth";
        ui_label = "Normal filter";
    > = true;

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
    > = 3;
    
    uniform float fTemporalMaxDepthDiff <
        ui_type = "slider";
        ui_category = "Common RT";
        ui_label = "Max Depth diff";
        ui_min = 0.0; ui_max = 1.0;
        ui_step = 0.001;
    > = 0.1;
    
    uniform float fRTMaxDist <
        ui_type = "slider";
        ui_category = "Common RT";
        ui_label = "Max distance";
        ui_min = 0.001; ui_max = 2.0;
        ui_step = 0.001;
    > = 0.8;
    
    uniform float fRTBounce <
        ui_type = "slider";
        ui_category = "Common RT";
        ui_label = "Bounce strengh";
        ui_min = 0.0; ui_max = 1.0;
        ui_step = 0.001;
    > = 0.25;
    
    uniform float fRayStepMultiply <
        ui_type = "slider";
        ui_category = "Common RT";
        ui_label = "Step multiply";
        ui_min = 0.01; ui_max = 4.0;
        ui_step = 0.01;
    > = 1.0;


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
    > = 0.08;
    
    uniform float fGIMissColor <
        ui_type = "slider";
        ui_category = "GI";
        ui_label = "Miss color";
        ui_min = 0.0; ui_max = 1.0;
        ui_step = 0.01;
    > = 0.05;
    
    uniform float fGISaturationWeight <
        ui_type = "slider";
        ui_category = "GI";
        ui_label = "Saturation weight";
        ui_min = 0.0; ui_max = 20.0;
        ui_step = 0.01;
    > = 20.0;

    // AO

    uniform float fAOMultiplier <
        ui_type = "slider";
        ui_category = "AO";
        ui_label = "Multiplier";
        ui_min = 0.0; ui_max = 5;
        ui_step = 0.01;
    > = 1.5;
    
    uniform float fAOPow <
        ui_type = "slider";
        ui_category = "AO";
        ui_label = "Pow";
        ui_min = 0.0; ui_max = 5;
        ui_step = 0.01;
    > = 1.0;
    
    uniform int fAODistance <
        ui_type = "slider";
        ui_category = "AO";
        ui_label = "Distance";
        ui_min = 0; ui_max = 1000;
        ui_step = 0.1;
    > = 1000.0;


    // SSR


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
    > = 4;
    
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
    

    uniform bool bDepthWeight <
        ui_category = "Denoising";
        ui_label = "Depth weight";
    > = true;
    
    uniform float fDepthWeight <
        ui_type = "slider";
        ui_category = "Denoising";
        ui_label = "Depth Weight";
        ui_min = 0.001; ui_max = 1.00;
        ui_step = 0.001;
    > = 0.010;
    
    uniform bool bNormalWeight <
        ui_category = "Denoising";
        ui_label = "Normal weight";
    > = true;

    uniform float fNormalWeight <
        ui_type = "slider";
        ui_category = "Denoising";
        ui_label = "Normal weight";
        ui_min = 0; ui_max = 4.0;
        ui_step = 0.001;
    > = 1.5;
    
    // Merging
        
    uniform float fDistanceFading <
        ui_type = "slider";
        ui_category = "Merging";
        ui_label = "Distance fading";
        ui_min = 0.0; ui_max = 1.0;
        ui_step = 0.01;
    > = 0.75;
    
    uniform float fMergingColor <
        ui_type = "slider";
        ui_category = "Merging";
        ui_label = "Color power";
        ui_min = 0; ui_max = 20.0;
        ui_step = 0.001;
    > = 1.5;
    
    uniform float fMergingGI <
        ui_type = "slider";
        ui_category = "Merging";
        ui_label = "GI";
        ui_min = 0; ui_max = 10.0;
        ui_step = 0.001;
    > = 2.0;
    
    uniform bool bAOProtectLight <
        ui_category = "Merging";
        ui_label = "Protect light from AO";
    > = true;
    
    uniform float fMergingSSR <
        ui_type = "slider";
        ui_category = "Merging";
        ui_label = "SSR";
        ui_min = 0; ui_max = 10.0;
        ui_step = 0.001;
    > = 1.5;
    
    uniform float fMergingRoughness <
        ui_type = "slider";
        ui_category = "Merging";
        ui_label = "Roughness";
        ui_min = 0; ui_max = 10.0;
        ui_step = 0.001;
    > = 2.5;

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
    float maxOf3(float3 a) {
        return max(max(a.x,a.y),a.z);
    }
    
    float minOf3(float3 a) {
        return min(min(a.x,a.y),a.z);
    }
    
    float3 max3(float3 a,float3 b) {
        return float3(max(a.x,b.x),max(a.y,b.y),max(a.z,b.z));
    }
    
    float3 min3(float3 a,float3 b) {
        return float3(min(a.x,b.x),min(a.y,b.y),min(a.z,b.z));
    }

    float3 getNormalJitter(float2 coords) {
        int2 offset = int2((framecount*random*SQRT2),(framecount*random*PI))%NOISE_SIZE;
        float3 jitter = normalize(tex2D(blueNoiseSampler,(offset+coords*BUFFER_SIZE)%(NOISE_SIZE)/(NOISE_SIZE)).rgb-0.5-float3(0.25,0,0));
        return normalize(jitter);
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
            float3 previous = getColorSampler(resultSampler,coords).rgb;
            color = fRTBounce*previous + (1.0-fRTBounce)*color;
        }
        return color;
    }

// SSR 
    float getRoughness(float2 coords) {
        return abs(getColorSampler(roughnessSampler,coords).r-0.5)*2;
    }

// GI

// RT
    float4 trace(float3 refWp,float3 lightVector,float startDepth,bool ssr) {

        float3 startNormal = getNormal(getScreenPosition(refWp));
                
        float stepRatio = 1.001+fRayStepMultiply/10.0;
        
        float stepLength = 1.0/(ssr?200:fRayStepPrecision);
        float3 incrementVector = lightVector*stepLength;
        float traceDistance = 0;
        float3 currentWp = refWp;
        
        float rayHitIncrement = 50.0*startDepth*fRayHitDepthThreshold/50.0;
        float rayHitDepthThreshold = rayHitIncrement;

        bool crossed = false;
        float deltaZ = 0;
        float deltaZbefore = 0;
        float3 lastCross;
        
        bool skyed = false;
        float3 lastSky;
        
        bool outSource = false;
        bool firstStep = true;
        
        bool startWeapon = startDepth<fWeaponDepth;
        float weaponLimit = fWeaponDepth*BUFFER_SIZE3.z;
        
        
        do {
            currentWp += incrementVector;
            traceDistance += stepLength;
            
            float3 screenCoords = getScreenPosition(currentWp);
            
            bool outScreen = !inScreen(screenCoords) && (!startWeapon || currentWp.z<weaponLimit);
            
            float3 screenWp = getWorldPosition(screenCoords.xy);
            
            deltaZ = screenWp.z-currentWp.z;
            
            float depth = getDepth(screenCoords.xy);
            bool isSky = depth>fSkyDepth;
            if(isSky) {
                skyed = true;
                lastSky = currentWp;
            }
            
            
            if(firstStep && deltaZ<0) {
                // wrong direction
                currentWp = refWp-incrementVector;
                incrementVector = reflect(incrementVector,startNormal);
                
                currentWp = refWp+incrementVector;
                //traceDistance = 0;

                firstStep = false; 
            } else if(outSource) {                
                
                if(!outScreen && sign(deltaZ)!=sign(deltaZbefore)) {
                    // search more precise
                    float preciseRatio = 1.0/iRayPreciseHitSteps;
                    float3 preciseIncrementVector = incrementVector;
                    float preciseLength = stepLength;
                    for(int precisePass=0;precisePass<iRayPreciseHit;precisePass++) {
                        preciseIncrementVector *= preciseRatio;
                        preciseLength *= preciseRatio;
                        
                        int preciseStep=0;
                        bool recrossed=false;
                        while(!recrossed && preciseStep<iRayPreciseHitSteps-1) {
                            currentWp -= preciseIncrementVector;
                            traceDistance -= preciseLength;
                            deltaZ = screenWp.z-currentWp.z;
                            recrossed = sign(deltaZ)==sign(deltaZbefore);
                            preciseStep++;
                        }
                        if(recrossed) {
                            currentWp += preciseIncrementVector;
                            traceDistance += preciseLength;
                            deltaZ = screenWp.z-currentWp.z;
                        }
                    }
                    
                    lastCross = currentWp;
                    crossed = true;
                    
                    
                }
                if(outScreen) {
                    if(crossed) {
                        return float4(lastCross,0.5);
                    }
                    if(skyed) {
                        return float4(lastSky,fSkyColor);
                    }
                    
                    return float4(currentWp, 0.1);
                    
                } 
                if(abs(deltaZ)<=rayHitDepthThreshold) {
                    // hit !
                    return float4(crossed ? lastCross : currentWp, 1.0);
                }
            } else {
                if(outScreen) {
                    if(crossed) {
                        return float4(lastCross,0.5);
                    }
                    if(skyed) {
                        return float4(lastSky,fSkyColor);
                    }
                    currentWp -= incrementVector;
                    return float4(currentWp, 0.1);
                }
                outSource = deltaZ>rayHitDepthThreshold;
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

        return float4(0.5,0.5,0.5,0);

    }



// PS

    void PS_RoughnessDepthPass(float4 vpos : SV_Position, float2 coords : TexCoord, out float4 outRoughness : SV_Target0, out float4 outDepth : SV_Target1) {
        float depth = ReShade::GetLinearizedDepth(coords);
        if(!bRoughness) {
            outRoughness = float4(0.5,0.5,0.5,1.0);
            outDepth = float4(depth,depth,depth,1.0);
        } else {
            
            float3 refColor = getColor(coords).rgb;
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
    }

    void PS_NormalPass(float4 vpos : SV_Position, float2 coords : TexCoord, out float4 outNormal : SV_Target0) {
        
        float3 offset = float3(ReShade::PixelSize, 0.0);
        
        float3 normal = computeNormal(coords,offset);

        if(bNormalFilter) {
            float depth = getDepth(coords);
            
            float3 normalTop = computeNormal(coords-offset.zy,offset);
            float3 normalBottom = computeNormal(coords+offset.zy,offset);
            float3 normalLeft = computeNormal(coords-offset.xz,offset);
            float3 normalRight = computeNormal(coords+offset.xz,offset);
            normal += normalTop+normalBottom+normalLeft+normalRight;
            normal/=5.0;
        }
        
        outNormal = float4(normal/2.0+0.5,1.0);
        
    }
    
    void PS_DisplayResult(in float4 position : SV_Position, in float2 coords : TEXCOORD, out float4 outPixel : SV_Target)
    {
        if(iDebug==DEBUG_OFF) {
            outPixel = getColorSampler(resultSampler,coords);
        } else if(iDebug==DEBUG_GI) {
            float3 gi = getColorSampler(giAccuSampler,coords).rgb;
            float ao = getColorSampler(aoAccuSampler,coords).x;
            ao = pow(ao,fAOMultiplier);
            if(bAOProtectLight) {
                float b =  getBrightness(gi);
                ao = 1.0-(1.0-ao)*(1.0-b);
            }
            gi *= ao;
            outPixel = float4(gi,1.0);
        } else if(iDebug==DEBUG_AO) {
            float ao = getColorSampler(aoAccuSampler,coords).r;
            if(bAOProtectLight) {
                float3 gi = getColorSampler(giAccuSampler,coords).rgb;
                float b =  getBrightness(gi);
                ao = 1.0-(1.0-ao)*(1.0-b);
            }
            outPixel = float4(ao,ao,ao,1);
        } else if(iDebug==DEBUG_SSR) {
            outPixel = getColorSampler(ssrAccuSampler,coords);
        } else if(iDebug==DEBUG_ROUGHNESS) {
            outPixel = getColorSampler(roughnessSampler,coords);
        } else if(iDebug==DEBUG_DEPTH) {
            float depth = getDepth(coords);
            outPixel = float4(depth,depth,depth,1);
        } else if(iDebug==DEBUG_NORMAL) {
            outPixel = getColorSampler(normalSampler,coords);
        }
    }

// GI
    void PS_GILightPass(float4 vpos : SV_Position, float2 coords : TexCoord, out float4 outGI : SV_Target0, out float4 outAO : SV_Target1) {
        
        if(fMergingGI==0.0) {
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
        
        float3 targetWp = getWorldPosition(coords);
        
        float3 randomVector = getNormalJitter(coords);

        float3 lightVector = reflect(targetWp,randomVector);
        
        float opacity = 1.0;
        float aoOpacity = 1.0/iFrameAccu;
        
        float4 hitPosition = trace(targetWp,lightVector,depth,false);
        float3 screenCoords = getScreenPosition(hitPosition.xyz);        
        
        
        float3 color = 0;
        if(hitPosition.a<=0.1) {
            color = fGIMissColor*getColorSamplerLod(resultSampler,coords,3).rgb;
        } else {
            color = getRayColor(screenCoords.xy).rgb;
        }
        
        float b = getBrightness(color);
        
        float d = abs(distance(hitPosition.xyz,targetWp));
                    
        float distance = 1.0+0.02*d;

        float previousDepth = getPreviousDepth(coords);

        float giOpacity = abs(previousDepth-depth)>fTemporalMaxDepthDiff*10. 
            ? 1.0 
            : max(b/iFrameAccu,hitPosition.a*hitPosition.a/iFrameAccu);
            
        outGI = saturate(float4(saturate(color*pow(iFrameAccu,0.5)),giOpacity));
        if(hitPosition.a>=1.0) {
          
         // if(screenCoords.z>=0.9) {
          //  outAO = float4(fAODistance>0?0:1,0,0,aoOpacity);
         // } else {
            if(d>fAODistance) {
                outAO = float4(1.0,0,0,aoOpacity);
            } else {
                float r = saturate(pow(d/fAODistance,fAOPow));

                outAO = float4(r,0,0,pow(1.0-r,0.5)*aoOpacity);
             //   outAO = float4(d/fAODistance,0,0,aoOpacity*(1.0-d/fAODistance));
            }
            
         // }
        } else {
        /*
          if(d>fAODistance) {
            outAO = float4(1,0,0,0.5*aoOpacity);
            } else {
                outAO = float4(d/fAODistance,0,0,0.5*aoOpacity);
            }
            
        }
        */
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
            
            float3 randomVector = getNormalJitter(coords);
            lightVector += randomVector;
             
            lightVector = normalize(lightVector);
            
            float4 hitPosition = trace(targetWp,lightVector,depth,true);
            if(hitPosition.a==0) {
                // no hit
                outColor = float4(0,0,0,opacity);
            } else {
                float3 screenCoords = getScreenPosition(hitPosition.xyz);
                float3 color = getRayColor(screenCoords.xy);
                float b = getBrightness(color.rgb);
                
                outColor = float4(hitPosition.a*color,opacity);        
            }
        }
    }
    
    void smooth(
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
        
        float opacity = 1.0/iFrameAccu;
        
        if(iSmoothRadius==0) {
            outGI = float4(getColorSampler(sourceGISampler,coords).rgb,opacity);
            float ao = getColorSampler(sourceAOSampler,coords).r;
            outAO = float4(ao,ao,ao,opacity);
            outSSR = float4(getColorSampler(sourceSSRSampler,coords).rgb,opacity);
            return;
        }
        
        int2 coordsInt = coords*RENDER_SIZE;
        
        float3 refNormal = getNormal(coords);
        
        float3 refGI = getColorSampler(sourceGISampler,coords).rgb;
        float refBrightness = getBrightness(refGI);
        
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
                float dist2 = distance(0,delta);
                
                if(dist2>iSmoothRadius) continue;
                
                if(firstPass && iSmoothStep>1) {
                    int2 coordsInt = coords*RENDER_SIZE;
                    float2 randDelta = 0;
                    float r = random+coordsInt.x+coordsInt.y*RENDER_WIDTH;
                    if(r%4==0) {
                        currentCoords.x += sign(delta.x)*pixelSize.x;
                    } else if(r%4==1) {
                        currentCoords.y += sign(delta.y)*pixelSize.y;
                    } else if(r%4==2) {
                        currentCoords.x += sign(delta.x)*pixelSize.x;
                        currentCoords.y += sign(delta.y)*pixelSize.y;
                    }
                }
            
                
                float depth = getDepth(currentCoords);
                if(depth>fSkyDepth) continue;
                
                if(abs(depth-refDepth)<=0.1) {
                    
                    float3 normal = getNormal(currentCoords);
                    if(diffT(normal,refNormal,1.0)) {
                        
                        // GI & AO
                        
                        float4 color = getColorSamplerLod(sourceGISampler,currentCoords,firstPass ? (iSmoothStep-1.0)*0.5 : 0.0);
                        float4 ssrColor = getColorSamplerLod(sourceSSRSampler,ssrCurrentCoords, firstPass ? (iSmoothSSRStep-1.0)*0.5 : 0.0);
                        
                        float ssrWeight = maxOf3(ssrColor.rgb)>0 ? 1.0 : 0.0;
                        float aoWeight = color.a;
                        float giWeight = color.a;                       
                        
                        float distWeight = pow(1.0+iSmoothRadius/(dist2+1),fSmoothDistPow);
                        
                        giWeight *= distWeight;
                        aoWeight *= distWeight;
                        ssrWeight *= distWeight;

                        
                        float b = getBrightness(color.rgb);
                        giWeight *= .5+b;
                        if(fGISaturationWeight>0) {
                        	giWeight += color.a*fGISaturationWeight*(maxOf3(color.rgb)-minOf3(color.rgb));
                        	//if(!bTest) ssrWeight += fGISaturationWeight*(maxOf3(ssrColor.rgb)-minOf3(ssrColor.rgb));
                        }
                        
                        if(bNormalWeight) {
                            float3 t = normal-refNormal;
                            float dist2 = max(dot(t,t), 0.0);
                            float nw = min(exp(-(dist2)/fNormalWeight), 1.0);
                            
                            giWeight *= nw*nw;
                            aoWeight *= nw*nw;
                            ssrWeight *= nw*nw;
                        }
                        
                        if(bDepthWeight) {
                            float3 t = depth-refDepth;
                            float dist2 = max(dot(t,t), 0.0);
                            float dw = min(exp(-(dist2)/fDepthWeight), 1.0);                                
                        
                            giWeight *= dw*dw;
                            aoWeight *= dw*dw;
                            ssrWeight *= dw*dw;
                        }
                        
                        // hit
                        float4 aoColor = getColorSamplerLod(sourceAOSampler,currentCoords,1.0);
                        
                        ao += aoColor.r*aoWeight;
                        aoWeightSum += aoWeight;
                        
                        gi += color.rgb*giWeight;
                        giWeightSum += giWeight;
                        
                        if(bTest) {
                        	//ssrWeight = 1.;
                        }
                        
                        ssr += ssrColor.rgb * ssrWeight;
                        ssrWeightSum += ssrWeight;
                        
                        
                    } // end normal
                } // end depth  
            } // end for y
        } // end for x

        //outAO = float4(resultAO,resultAO,resultAO,resultAO<1.0 ? 1.0-resultAO : 1.0);
        
        //result = float(foundSamples)/10;//saturate(fLightMult*result/weightSum);
        //result = saturate(fLightMult*result/weightSum);
        gi /= giWeightSum;
        ao /= aoWeightSum;
        ssr /= ssrWeightSum;
        outGI = float4(gi,opacity);
        outAO = float4(ao,ao,ao,opacity);
        outSSR = float4(ssr,1);
    }
    
    void PS_SmoothPass(float4 vpos : SV_Position, float2 coords : TexCoord, out float4 outGI : SV_Target0, out float4 outAO : SV_Target1, out float4 outSSR : SV_Target2) {
        smooth(giPassSampler,aoPassSampler,ssrPassSampler,coords,outGI,outAO,outSSR, true);
    }
    
    void PS_AccuPass(float4 vpos : SV_Position, float2 coords : TexCoord, out float4 outGI : SV_Target0, out float4 outAO : SV_Target1, out float4 outSSR : SV_Target2) {
        smooth(giSmoothPassSampler,aoSmoothPassSampler,ssrSmoothPassSampler,coords,outGI,outAO,outSSR, false);
    }
    


    void PS_UpdateResult(in float4 position : SV_Position, in float2 coords : TEXCOORD, out float4 outResult : SV_Target, out float4 outDepth : SV_Target1) {
        float depth = getDepth(coords);
        float3 color = getColor(coords).rgb;
        
        if(depth>fSkyDepth) {
            outResult = float4(color,1.0);
            outDepth = float4(depth,depth,depth,1.0);
        } else {
            float3 colorHsl = RGBtoHSL(color);
            
            
            float3 gi = getColorSampler(giAccuSampler,coords).rgb;
            float3 giHsl = RGBtoHSL(gi);
            
            float b =  getBrightness(gi);
            float ao = getColorSampler(aoAccuSampler,coords).x;
            ao = pow(ao,fAOMultiplier);
            if(bAOProtectLight) {
                ao = 1.0-(1.0-ao)*(1.0-b);
            }
            gi *= ao;
            
            float3 ssr = getColorSampler(ssrAccuSampler,coords).rgb;
            float roughness = getRoughness(coords);
            
            float3 fixLightHsl = colorHsl;
            fixLightHsl.z = pow(fixLightHsl.z,fMergingColor);
            
            float3 fixDarkHsl = colorHsl;
            fixDarkHsl.z = pow(1.0-fixDarkHsl.z,fMergingColor);
            
            float3 fixedColor = HSLtoRGB(fixLightHsl);//+HSLtoRGB(fixDarkHsl))/;
            float originalColor = saturate(fixLightHsl.z+fixDarkHsl.z);
            fixedColor = color*originalColor;
            
            float3 fixedDarkHsl = colorHsl;
            fixedDarkHsl.z = 1.0-pow(1.0-colorHsl.z,fMergingRoughness);
            float3 fixedDark = HSLtoRGB(fixedDarkHsl);
            
            float ssrRatio2 = fixedDarkHsl.z/fMergingRoughness;
            float ssrRatio1 = max(0.0,1.0-(roughness+0.1)*fMergingRoughness);
            
            float3 result = color*originalColor
                //+(1.0-originalColor)
                +(
                    (1.-colorHsl.z)*max(0.1,color)*(gi*fMergingGI)
                    +max(0,ssrRatio1-ssrRatio2)*ssr*fMergingSSR*(1.0-originalColor)
                    )
                ;
                
            //result /= fMergingColor+fMergingGI+fMergingSSR;
            
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
        }
        pass {
            VertexShader = PostProcessVS;
            PixelShader = PS_DisplayResult;
        }
    }
}