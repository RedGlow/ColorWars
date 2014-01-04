sampler2D implicitInputSampler : register(S0);
float value : register(C0);

/* from http://chilliant.blogspot.de/2010/11/rgbhsv-in-hlsl.html */
float3 Hue(float H)
{
    float R = abs(H * 6 - 3) - 1;
    float G = 2 - abs(H * 6 - 2);
    float B = 2 - abs(H * 6 - 4);
    return saturate(float3(R,G,B));
}

float3 HSVtoRGB(in float3 HSV)
{
    return ((Hue(HSV.x) - 1) * HSV.y + 1) * HSV.z;
}

float4 main(float2 uv : TEXCOORD) : COLOR {
	// gets distance of the point from the center (dx, dy).
	float dx = uv[0] - 0.5f;
	float dy = uv[1] - 0.5f;
	// by default, take the underlying image
	float4 color = tex2D(implicitInputSampler, uv);
	// check if we are inside the wheel
	float d2 = dx * dx + dy * dy;
	if(d2 < 0.25) {
		// hue, computed as an angle, transformed inside the range [0;1] and rotated to correct
		float h = -atan2(dx, dy) / (2 * 3.1415) + 0.25;
		if(h < 0) h = h + 1;
		// saturation: distance from the center, transformed from [0;0.5] to [0;1]
		float s = sqrt(d2) * 2;
		// transform HSV to RGB
		float3 hsv = {h, s, value};
		float3 rgb = HSVtoRGB(hsv);
		// copy on output
		color[0] = rgb[0];
		color[1] = rgb[1];
		color[2] = rgb[2];
		color[3] = 1.0f;
	}
	return color;
}