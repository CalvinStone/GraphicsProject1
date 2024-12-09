using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using SharpGLTF.Schema2;

namespace GraphicsProject1.Model.Components
{
	// Start off with two faces and 2-3 hairstyles
	internal class Head
	{
		// Normal model info
		public List<Vector4> faceVerts;
		//private string face1FilePath;
		public List<Vector2> faceUVs;
		public List<uint> faceIndices;
		private float scale;

		// Bones info for stretching
		public List<Vector4> bones;
		public List<Vector4> boneIndices;
		public List<Vector4> boneWeights;
		public Matrix4[] boneTransforms;

		public Head(float scale)
		{
			faceVerts = new List<Vector4>();
			faceUVs = new List<Vector2>();
			faceIndices = new List<uint>();
			this.scale = scale;
			bones = new List<Vector4>();
			boneIndices = new List<Vector4>();
			boneWeights = new List<Vector4>();
			

			GenerateFaceDefault(scale);
			
		}

		/*
		public Head(string filePath)
		{
			faceVerts = new List<Vector4>();
			faceVerts = GenerateFace();
		}
		*/

		private void GenerateFaceDefault(float scale)
		{
			GLBExtractor glbInfo = new GLBExtractor("../../../Model/gltf/kermitBody1.glb");

			foreach (Vector4 v in glbInfo.glbVerts)
			{
				faceVerts.Add(v * (new Vector4(scale, scale, scale, 1)));
			}
			faceUVs = glbInfo.glbUVs;
			faceIndices = glbInfo.glbIndices;

			// Bones
			bones = glbInfo.bones;
			boneIndices = glbInfo.boneIndices;
			boneWeights = glbInfo.boneWeights;
			boneTransforms = glbInfo.boneTransforms;
		}


		/*
		private static List<Vector4> GenerateFace(string filepath)
		{
			List<Vector4> newFace = new List<Vector4>();
			GLBExtractor faceVerts = new GLBExtractor("../../../Model/gltf/" + filepath);
			foreach (Vector3 v in faceVerts)
			{

			}

			return newFace;
		}
		*/
	}
}
