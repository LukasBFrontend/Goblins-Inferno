#ifndef STOCHASTIC_TERRAIN_LIT_INPUT_INCLUDED
#define STOCHASTIC_TERRAIN_LIT_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"

CBUFFER_START(UnityPerMaterial)

    float4 _MainTex_ST;
    half4 _BaseColor;
    half _Cutoff;

    UNITY_TEXTURE_STREAMING_DEBUG_VARS_FOR_TEX(_Control);

    float4 _Splat0_TexelSize;
    float4 _Splat1_TexelSize;
    float4 _Splat2_TexelSize;
    float4 _Splat3_TexelSize;

    UNITY_TEXTURE_STREAMING_DEBUG_VARS_FOR_TEX(_Splat0);
    UNITY_TEXTURE_STREAMING_DEBUG_VARS_FOR_TEX(_Splat1);
    UNITY_TEXTURE_STREAMING_DEBUG_VARS_FOR_TEX(_Splat2);
    UNITY_TEXTURE_STREAMING_DEBUG_VARS_FOR_TEX(_Splat3);

    // Stochastic sampling.
    half _StochasticStrength;

CBUFFER_END


#define _Surface 0.0


CBUFFER_START(_Terrain)

    half _NormalScale0;
    half _NormalScale1;
    half _NormalScale2;
    half _NormalScale3;

    half _Metallic0;
    half _Metallic1;
    half _Metallic2;
    half _Metallic3;

    half _Smoothness0;
    half _Smoothness1;
    half _Smoothness2;
    half _Smoothness3;

    half4 _DiffuseRemapScale0;
    half4 _DiffuseRemapScale1;
    half4 _DiffuseRemapScale2;
    half4 _DiffuseRemapScale3;

    half4 _MaskMapRemapOffset0;
    half4 _MaskMapRemapOffset1;
    half4 _MaskMapRemapOffset2;
    half4 _MaskMapRemapOffset3;

    half4 _MaskMapRemapScale0;
    half4 _MaskMapRemapScale1;
    half4 _MaskMapRemapScale2;
    half4 _MaskMapRemapScale3;

    float4 _Control_ST;
    float4 _Control_TexelSize;

    half _DiffuseHasAlpha0;
    half _DiffuseHasAlpha1;
    half _DiffuseHasAlpha2;
    half _DiffuseHasAlpha3;

    half _LayerHasMask0;
    half _LayerHasMask1;
    half _LayerHasMask2;
    half _LayerHasMask3;

    half _SmoothnessSource0;
    half _SmoothnessSource1;
    half _SmoothnessSource2;
    half _SmoothnessSource3;

    half4 _Splat0_ST;
    half4 _Splat1_ST;
    half4 _Splat2_ST;
    half4 _Splat3_ST;

    half _HeightTransition;
    half _NumLayersCount;

    float _TerrainBasemapDistance;

#ifdef UNITY_INSTANCING_ENABLED
    float4 _TerrainHeightmapRecipSize;
#endif

    float4 _TerrainHeightmapScale;

#ifdef SCENESELECTIONPASS
    int _ObjectId;
    int _PassValue;
#endif

CBUFFER_END


TEXTURE2D(_Control);
SAMPLER(sampler_Control);

TEXTURE2D(_Splat0);
SAMPLER(sampler_Splat0);

TEXTURE2D(_Splat1);
SAMPLER(sampler_Splat1);

TEXTURE2D(_Splat2);
SAMPLER(sampler_Splat2);

TEXTURE2D(_Splat3);
SAMPLER(sampler_Splat3);


TEXTURE2D(_Normal0);
SAMPLER(sampler_Normal0);

TEXTURE2D(_Normal1);
SAMPLER(sampler_Normal1);

TEXTURE2D(_Normal2);
SAMPLER(sampler_Normal2);

TEXTURE2D(_Normal3);
SAMPLER(sampler_Normal3);


TEXTURE2D(_Mask0);
SAMPLER(sampler_Mask0);

TEXTURE2D(_Mask1);
SAMPLER(sampler_Mask1);

TEXTURE2D(_Mask2);
SAMPLER(sampler_Mask2);

TEXTURE2D(_Mask3);
SAMPLER(sampler_Mask3);


TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

TEXTURE2D(_SpecGlossMap);
SAMPLER(sampler_SpecGlossMap);

TEXTURE2D(_MetallicTex);
SAMPLER(sampler_MetallicTex);


#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
#define ENABLE_TERRAIN_PERPIXEL_NORMAL
#endif


#ifdef UNITY_INSTANCING_ENABLED

TEXTURE2D(_TerrainHeightmapTexture);
TEXTURE2D(_TerrainNormalmapTexture);
SAMPLER(sampler_TerrainNormalmapTexture);

#endif


UNITY_INSTANCING_BUFFER_START(Terrain)

UNITY_DEFINE_INSTANCED_PROP(
    float4,
    _TerrainPatchInstanceData
)

UNITY_INSTANCING_BUFFER_END(Terrain)


#ifdef _ALPHATEST_ON

TEXTURE2D(_TerrainHolesTexture);
SAMPLER(sampler_TerrainHolesTexture);


float SampleTerrainHolesTexture(float2 uv)
{
    return SAMPLE_TEXTURE2D(
        _TerrainHolesTexture,
        sampler_TerrainHolesTexture,
        uv
    ).r;
}


void ClipHoles(float2 uv)
{
    float hole =
        SampleTerrainHolesTexture(uv);

    float epsilon = 0.0005f;

    clip(
        hole < epsilon
            ? -1
            : 1
    );
}

#endif


#ifdef _NORMALMAP

#define SampleLayerNormal(i) \
    UnpackNormalScale( \
        SAMPLE_TEXTURE2D( \
            _Normal##i, \
            sampler_Normal0, \
            splat##i##uv \
        ), \
        _NormalScale##i \
    )

#else

#define SampleLayerNormal(i) \
    half3(0.0, 0.0, 1.0)

#endif


#ifdef _MASKMAP

#define SampleLayerMasks(i) \
    (_MaskMapRemapOffset##i + \
     _MaskMapRemapScale##i * \
     lerp( \
         0.5h, \
         SAMPLE_TEXTURE2D( \
             _Mask##i, \
             sampler_Mask0, \
             splat##i##uv \
         ), \
         _LayerHasMask##i \
     ))

#else

#define SampleLayerMasks(i) \
    (_MaskMapRemapOffset##i + \
     _MaskMapRemapScale##i * 0.5h)

#endif


half4 SampleMetallicSpecGloss(
    float2 uv,
    half albedoAlpha
)
{
    half4 specGloss;

    specGloss =
        SAMPLE_TEXTURE2D(
            _MetallicTex,
            sampler_MetallicTex,
            uv
        );

    specGloss.a =
        albedoAlpha;

    return specGloss;
}


inline void InitializeStandardLitSurfaceData(
    float2 uv,
    out SurfaceData outSurfaceData
)
{
    outSurfaceData =
        (SurfaceData)0;

    half4 albedoSmoothness =
        SAMPLE_TEXTURE2D(
            _MainTex,
            sampler_MainTex,
            uv
        );

    outSurfaceData.alpha = 1;

    half4 specGloss =
        SampleMetallicSpecGloss(
            uv,
            albedoSmoothness.a
        );

    outSurfaceData.albedo =
        albedoSmoothness.rgb;

    outSurfaceData.metallic =
        specGloss.r;

    outSurfaceData.specular =
        half3(0.0h, 0.0h, 0.0h);

    outSurfaceData.smoothness =
        specGloss.a;

    outSurfaceData.normalTS =
        SampleNormal(
            uv,
            TEXTURE2D_ARGS(
                _BumpMap,
                sampler_BumpMap
            )
        );

    outSurfaceData.occlusion = 1;
    outSurfaceData.emission = 0;
}


void TerrainInstancing(
    inout float4 positionOS,
    inout float3 normal,
    inout float2 uv
)
{
#ifdef UNITY_INSTANCING_ENABLED

    float2 patchVertex =
        positionOS.xy;

    float4 instanceData =
        UNITY_ACCESS_INSTANCED_PROP(
            Terrain,
            _TerrainPatchInstanceData
        );

    float2 sampleCoords =
        (patchVertex.xy + instanceData.xy)
        * instanceData.z;

    float height =
        UnpackHeightmap(
            _TerrainHeightmapTexture.Load(
                int3(sampleCoords, 0)
            )
        );

    positionOS.xz =
        sampleCoords *
        _TerrainHeightmapScale.xz;

    positionOS.y =
        height *
        _TerrainHeightmapScale.y;

#ifdef ENABLE_TERRAIN_PERPIXEL_NORMAL

    normal =
        float3(0, 1, 0);

#else

    normal =
        _TerrainNormalmapTexture.Load(
            int3(sampleCoords, 0)
        ).rgb * 2 - 1;

#endif

    uv =
        sampleCoords *
        _TerrainHeightmapRecipSize.zw;

#endif
}


void TerrainInstancing(
    inout float4 positionOS,
    inout float3 normal
)
{
    float2 uv = { 0, 0 };

    TerrainInstancing(
        positionOS,
        normal,
        uv
    );
}


void TerrainInstancing(
    inout float4 positionOS
)
{
    float3 normal = { 0, 0, 0 };

    TerrainInstancing(
        positionOS,
        normal
    );
}

#endif
