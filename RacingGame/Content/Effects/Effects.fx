//=========================Constant variables
	//Matrices
float4x4 viewMatrix;
float4x4 reflectionViewMatrix;
float4x4 projectionMatrix;
float4x4 worldMatrix;

	//light data
float3 lightDirection;
float ambientIntensity;
bool enableLighting;

	//water data
float waveLength;
float waveHeight;

	//camera data
float3 cameraPosition;

	//moving water data
float time;
float windForce;
float3 windDirection;

	//fog data
float StartFog = 25;
float4 ColorFog = {0.96,0.96,0.96,1};

	//billboard data
float3 xAllowedRotDir;

//=========================Texture Sampers
Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xTexture0;
sampler TextureSampler0 = sampler_state { texture = <xTexture0> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = wrap; AddressV = wrap;};

Texture xTexture1;
sampler TextureSampler1 = sampler_state { texture = <xTexture1> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = wrap; AddressV = wrap;};

Texture xTexture2;
sampler TextureSampler2 = sampler_state { texture = <xTexture2> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xTexture3;
sampler TextureSampler3 = sampler_state { texture = <xTexture3> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xReflectionMap;
sampler ReflectionSampler = sampler_state { texture = <xReflectionMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xRefractionMap;
sampler RefractionSampler = sampler_state { texture = <xRefractionMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xWaterBumpMap;
sampler WaterBumpMapSampler = sampler_state { texture = <xWaterBumpMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

//=========================Mutitexturing technique
struct MultiTexturingVertexToPixel
{
	float4 Position			: POSITION;
	float4 Color			: COLOR0;
	float3 Normal			: TEXCOORD0;
	float2 TextureCoords	: TEXCOORD1;
	float4 LightDirection	: TEXCOORD2;
	float4 TextureWeights	: TEXCOORD3;
	float Depth				: TEXCOORD4;
	float Fog				: FOG;
};

struct MultiTexturingPixelToFrame
{
	float4 Color				: COLOR0;
};

MultiTexturingVertexToPixel MultiTexturedVS(float4 position : POSITION, float3 normal : NORMAL, float2 texcoords : TEXCOORD0, float4 texweights : TEXCOORD1)
{
	MultiTexturingVertexToPixel Output = (MultiTexturingVertexToPixel)0;
	
	float4x4 viewProjection = mul(viewMatrix, projectionMatrix);
	float4x4 worldViewProjection = mul(worldMatrix, viewProjection);
	
	Output.Position = mul(position, worldViewProjection);
    Output.Normal = mul(normalize(normal), worldMatrix);
    Output.TextureCoords = texcoords;
    Output.LightDirection.xyz = -lightDirection;
    Output.LightDirection.w = 1;    
    Output.TextureWeights = texweights;
    Output.Depth = Output.Position.z/Output.Position.w;
    
    //float4 PlayerPosWorld  = mul(cameraPosition, worldMatrix);
    //float DistFog = distance(cameraPosition.xy, PlayerPosWorld.xy);
    //Output.Fog = saturate(exp((StartFog-DistFog)*0.33));

    return Output;
}

MultiTexturingPixelToFrame MultiTexturedPS(MultiTexturingVertexToPixel PSIn)
{
	MultiTexturingPixelToFrame Output = (MultiTexturingPixelToFrame)0;  
	
	float lightFactor = 1;	
	if(enableLighting)
		lightFactor = saturate(saturate(dot(PSIn.Normal, PSIn.LightDirection)) + ambientIntensity);
		
	float blendDistance = 0.99f;
    float blendWidth = 0.005f;
    float blendFactor = clamp((PSIn.Depth-blendDistance)/blendWidth, 0, 1);
    
    float4 farColor;
    farColor = tex2D(TextureSampler0, PSIn.TextureCoords)*PSIn.TextureWeights.x;
    farColor += tex2D(TextureSampler1, PSIn.TextureCoords)*PSIn.TextureWeights.y;
    farColor += tex2D(TextureSampler2, PSIn.TextureCoords)*PSIn.TextureWeights.z;
    farColor += tex2D(TextureSampler3, PSIn.TextureCoords)*PSIn.TextureWeights.w;
    
    float4 nearColor;
    float2 nearTextureCoords = PSIn.TextureCoords*3;
    nearColor = tex2D(TextureSampler0, nearTextureCoords)*PSIn.TextureWeights.x;
    nearColor += tex2D(TextureSampler1, nearTextureCoords)*PSIn.TextureWeights.y;
    nearColor += tex2D(TextureSampler2, nearTextureCoords)*PSIn.TextureWeights.z;
    nearColor += tex2D(TextureSampler3, nearTextureCoords)*PSIn.TextureWeights.w;

    Output.Color = lerp(nearColor, farColor, blendFactor);
    //Output.Color = lerp(ColorFog, colorBase, PSIn.Fog);
    Output.Color *= lightFactor;
    
    return Output;
}

technique MultiTextured
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 MultiTexturedVS();
        PixelShader = compile ps_3_0 MultiTexturedPS();
    }
}

//=========================Water technique
struct WaterVertexToPixel
{
	float4 Position						: POSITION;
    float4 ReflectionMapSamplingPos		: TEXCOORD1;
    float2 BumpMapSamplingPos			: TEXCOORD2;
    float4 RefractionMapSamplingPos		: TEXCOORD3;
    float4 Position3D					: TEXCOORD4;
};

struct WaterPixelToFrame
{
	float4 Color						: COLOR0;
};

WaterVertexToPixel WaterVS(float4 position : POSITION, float2 tex: TEXCOORD)
{
	WaterVertexToPixel Output = (WaterVertexToPixel)0;
	
	float4x4 viewProjection = mul(viewMatrix, projectionMatrix);
	float4x4 worldViewProjection = mul(worldMatrix, viewProjection);
	float4x4 reflectionViewProjection = mul(reflectionViewMatrix, projectionMatrix);
	float4x4 worldReflectionViewProjection = mul(worldMatrix, reflectionViewProjection);
	
	Output.Position = mul(position, worldViewProjection);
    Output.ReflectionMapSamplingPos = mul(position, worldReflectionViewProjection);
    Output.BumpMapSamplingPos = tex/waveLength;

    Output.RefractionMapSamplingPos = mul(position, worldViewProjection);
    Output.Position3D = mul(position, worldMatrix);
    
    float3 windDir = normalize(windDirection);    
    float3 perpDir = cross(windDirection, float3(0,1,0));
    float ydot = dot(tex, windDirection.xz);
    float xdot = dot(tex, perpDir.xz);
    float2 moveVector = float2(xdot, ydot);
    moveVector.y += time*windForce;    
    Output.BumpMapSamplingPos = moveVector/waveLength;    

    return Output;
}

WaterPixelToFrame WaterPS(WaterVertexToPixel PSIn)
{
    WaterPixelToFrame Output = (WaterPixelToFrame)0;        
    
    float4 bumpColor = tex2D(WaterBumpMapSampler, PSIn.BumpMapSamplingPos);
    float2 perturbation = waveHeight*(bumpColor.rg - 0.5f)*2.0f;
    
    float2 ProjectedTexCoords;
    ProjectedTexCoords.x = PSIn.ReflectionMapSamplingPos.x/PSIn.ReflectionMapSamplingPos.w/2.0f + 0.5f;
    ProjectedTexCoords.y = -PSIn.ReflectionMapSamplingPos.y/PSIn.ReflectionMapSamplingPos.w/2.0f + 0.5f;        
    float2 perturbatedTexCoords = ProjectedTexCoords + perturbation;
    float4 reflectiveColor = tex2D(ReflectionSampler, perturbatedTexCoords);
    

    float2 ProjectedRefrTexCoords;
    ProjectedRefrTexCoords.x = PSIn.RefractionMapSamplingPos.x/PSIn.RefractionMapSamplingPos.w/2.0f + 0.5f;
    ProjectedRefrTexCoords.y = -PSIn.RefractionMapSamplingPos.y/PSIn.RefractionMapSamplingPos.w/2.0f + 0.5f;    
    float2 perturbatedRefrTexCoords = ProjectedRefrTexCoords + perturbation;    
    float4 refractiveColor = tex2D(RefractionSampler, perturbatedRefrTexCoords);
     
    float3 eyeVector = normalize(cameraPosition - PSIn.Position3D);
    float3 normalVector = float3(0,1,0);
    float fresnelTerm = dot(eyeVector, normalVector);    
    float4 combinedColor = lerp(reflectiveColor, refractiveColor, fresnelTerm);
     
    float4 dullColor = float4(0.3f, 0.3f, 0.5f, 1.0f);
     
    Output.Color = lerp(combinedColor, dullColor, 0.5f);
    
        
    return Output;
}

technique Water
{
    pass Pass0
    {
        VertexShader = compile vs_1_1 WaterVS();
        PixelShader = compile ps_2_0 WaterPS();
    }
}

//=========================Billboarding technique

Texture xBillboardTexture;

sampler textureSampler = sampler_state { texture = <xBillboardTexture> ; 
magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = CLAMP; AddressV = CLAMP;};

struct BBVertexToPixel
{
    float4 Position : POSITION;
    float2 TexCoord    : TEXCOORD0;
};

struct BBPixelToFrame
{
    float4 Color     : COLOR0;
};

BBVertexToPixel CylBillboardVS(float3 inPos: POSITION0, float2 inTexCoord: TEXCOORD0)
{
    BBVertexToPixel Output = (BBVertexToPixel)0;

    float3 center = mul(inPos, worldMatrix);
    float3 eyeVector = center - cameraPosition;

    float3 upVector = xAllowedRotDir;
    upVector = normalize(upVector);
    float3 sideVector = cross(eyeVector,upVector);
    sideVector = normalize(sideVector);

    float3 finalPosition = center;
    finalPosition += (inTexCoord.x-0.5f)*sideVector;
    finalPosition += (1.5f-inTexCoord.y*1.5f)*upVector;

    float4 finalPosition4 = float4(finalPosition, 1);

    float4x4 preViewProjection = mul (viewMatrix, projectionMatrix);
    Output.Position = mul(finalPosition4, preViewProjection);

    Output.TexCoord = inTexCoord;

    return Output;
}

BBPixelToFrame BillboardPS(BBVertexToPixel PSIn) : COLOR0
{
    BBPixelToFrame Output = (BBPixelToFrame)0;
    Output.Color = tex2D(textureSampler, PSIn.TexCoord);

    return Output;
}

technique CylBillboard
{
    pass Pass0
    {        
        VertexShader = compile vs_1_1 CylBillboardVS();
        PixelShader = compile ps_1_1 BillboardPS();        
    }
}