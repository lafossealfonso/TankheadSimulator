#ifndef WIREFRAME_SHADER_CGINC
#define WIREFRAME_SHADER_CGINC


#define WireframeShaderDistanceToEdge(a,b,c) length(cross(a - b, a - c))
#define WireframeShaderMaskSmooth(x) x = saturate(x * x * (3 - 2 * x));



float WireframeShaderReadTriangleMassFromUV(float3 uv, float thickness, float smoothness, float diameter)
{
	float3 fw = abs(ddx(uv)) + abs(ddy(uv));
	float3 s = fw * (1 + smoothness * 10) + 1e-6;
	float3 l = uv - thickness.xxx;


	float3 a1 = smoothstep(0, s, l);	
	float w1 = min(min(a1.x, a1.y), a1.z);	
	
	float3 a2 = l / s;
	float w2 = smoothstep(0.0, 1.0, min(a2.x, min(a2.y, a2.z)) + 0.5);


    return 1 - saturate(lerp(w2, w1, diameter));
}

void WireframeShaderCalculateTriangleMass(float3 vertex1, float3 vertex2, float3 vertex3, out float3 mass1, out float3 mass2, out float3 mass3)
{
	#if defined(WIREFRAME_NORMALIZE_EDGES_ON)

		float d1 = distance(vertex1, vertex2);
		float d2 = distance(vertex2, vertex3);
		float d3 = distance(vertex3,  vertex1);		

		float4 b = float4(0, 
		                  WireframeShaderDistanceToEdge(vertex3, vertex1, vertex2) / d1, 
						  WireframeShaderDistanceToEdge(vertex1, vertex2, vertex3) / d2, 
						  WireframeShaderDistanceToEdge(vertex2, vertex1, vertex3) / d3);
		b /= min(b.y, min(b.z, b.w));


		mass1 = b.xzx;
		mass2 = b.xxw;
		mass3 = b.yxx;

		#ifdef WIREFRAME_TRY_QUAD_ON
			
			mass1.x = ((d1 > d2) && (d1 > d3)) ? 10000 : 0;
			mass1.z = ((d3 >= d1) && (d3 > d2)) ? 10000 : 0;
			mass2.y = ((d2 >= d1) && (d2 >= d3)) ? 10000 : 0;

		#endif

	#else

		#if defined(WIREFRAME_TRY_QUAD_ON)

			float d1 = distance(vertex1, vertex2);
			float d2 = distance(vertex2, vertex3);
			float d3 = distance(vertex3, vertex1);

			float3 offset = 0;
			if (d1 > d2 && d1 > d3)
				offset.y = 1;
			else if (d2 > d3 && d2 > d1)
				offset.x = 1;
			else
				offset.z = 1;
			
			mass1 = float3(1, 0, 0) + offset;
			mass2 = float3(0, 0, 1) + offset;
			mass3 = float3(0, 1, 0) + offset;

		#else

			mass1 = float3(1, 0, 0);
			mass2 = float3(0, 1, 0);
			mass3 = float3(0, 0, 1);

		#endif

	#endif
}

float WireframeShaderMaskPlane(float3 planePosition, float3 planeNormal, float3 vertexPositionWS, float edgeFalloff, float invert)
{
	float edge = dot(planeNormal, (vertexPositionWS - planePosition));

    edge = saturate(edge / max(0.01, edgeFalloff) * lerp(1, -1, invert));


    //WireframeShaderMaskSmooth(edge);

    return edge;
}

float WireframeShaderMaskSphere(float3 spherePosition, float sphereRadius, float3 vertexPositionWS, float edgeFalloff, float invert)
{
	float d = distance(vertexPositionWS, spherePosition);

    edgeFalloff = max(0.01, edgeFalloff);
    float offset = lerp(0, edgeFalloff, invert);

    float edge = saturate(max(0, d - offset - sphereRadius + edgeFalloff) / edgeFalloff);
                        

    //WireframeShaderMaskSmooth(edge);

    return lerp(1- edge, edge, invert);
}

float WireframeShaderDistanceFade(float3 cameraPositionWS, float3 vertexPositionWS, float startDistance, float endDistance)
{
	float distanceToCamera = distance(cameraPositionWS, vertexPositionWS);

	float distanceFade = (endDistance - distanceToCamera) / (endDistance - startDistance); 

	return saturate(distanceFade);
}




//Unity Shader Graph///////////////////////////////////////////////////////////////////////////////////
void WireframeShaderReadTriangleMassFromUV_float(float3 uv, float thickness, float smoothness, float diameter, out float value)
{
	value = WireframeShaderReadTriangleMassFromUV(uv, thickness, smoothness, diameter);
}

void WireframeShaderReadTriangleMassFromUV_half(half3 uv,  half thickness, half smoothness, half diameter, out half value)
{
	value = WireframeShaderReadTriangleMassFromUV(uv, thickness, smoothness, diameter);
}

void WireframeShaderMaskPlane_float(float3 planePosition, float3 planeNormal, float3 vertexPositionWS, float edgeFalloff, float invert, out float value)
{
	value = WireframeShaderMaskPlane(planePosition, planeNormal, vertexPositionWS, edgeFalloff, invert);
}

void WireframeShaderMaskPlane_half(half3 planePosition, half3 planeNormal, half3 vertexPositionWS, half edgeFalloff, half invert, out half value)
{
	value = WireframeShaderMaskPlane(planePosition, planeNormal, vertexPositionWS, edgeFalloff, invert);
}

void WireframeShaderMaskSphere_float(float3 spherePosition, float sphereRadius, float3 vertexPositionWS, float edgeFalloff, float invert, out float value)
{
	value = WireframeShaderMaskSphere(spherePosition, sphereRadius, vertexPositionWS, edgeFalloff, invert);
}

void WireframeShaderMaskSphere_half(half3 spherePosition, half sphereRadius, half3 vertexPositionWS, half edgeFalloff, half invert, out half value)
{
	value = WireframeShaderMaskSphere(spherePosition, sphereRadius, vertexPositionWS, edgeFalloff, invert);
}

void WireframeShaderDistanceFade_float(float3 cameraPositionWS, float3 vertexPositionWS, float startDistance, float endDistance, out float value)
{
	value = WireframeShaderDistanceFade(cameraPositionWS, vertexPositionWS, startDistance, endDistance);
}

void WireframeShaderDistanceFade_half(half3 cameraPositionWS, half3 vertexPositionWS, half startDistance, half endDistance, out half value)
{
	value = WireframeShaderDistanceFade(cameraPositionWS, vertexPositionWS, startDistance, endDistance);
}

#endif
