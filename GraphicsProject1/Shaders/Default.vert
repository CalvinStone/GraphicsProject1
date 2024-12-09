#version 330 core
layout (location = 0) in vec4 aPosition;	// vertex coordinates
layout (location = 1) in vec2 aTexCoord;	// texture coordinates
layout (location = 2) in vec4 aBoneIndices; // Bone indices
layout (location = 3) in vec4 aBoneWeights; // Bone weights

out vec2 texCoord;

// uniform variables
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat4 boneTransforms[10];
uniform bool useBones; // Flag for bone influence

void main() 
{
	//vec4 finalPosition = (aPositionVec4 + vec4(aPosition, 1.0)) * model * view * projection
	//gl_Position = vec4(aPosition, 1.0) * model * view * projection; // coordinates

	
	vec4 transformedPosition = aPosition;
	/*
	if (useBones) {
		transformedPosition = vec4(0.0);
		for (int i = 0; i < 4; i++) {
			if (aBoneWeights[i] > 0.0) {
				transformedPosition += aPosition * aBoneWeights[i] * (boneTransforms[int(aBoneIndices[i])]);
				//transformedPosition += aBoneWeights[i] * (boneTransforms[int(aBoneIndices[i])] * aPosition);
			}
		}
	}
	*/
	

	//gl_Position = aPosition * model * view * projection;	//The normal line
	gl_Position = transformedPosition * model * view * projection;	// Experimental line
	texCoord = aTexCoord;
}