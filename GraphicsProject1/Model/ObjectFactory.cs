using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsProject1.Model.Components;
using OpenTK.Mathematics;

namespace GraphicsProject1.Model
{
	// Set an ID to make a certain type of face so the object factory will auto make the eyes and face
	internal class ObjectFactory
	{
		// Eye Info
		public List<Vector4> eyeVerts;
		public List<Vector2> eyeUVs;
		public Vector3 eyeOffset1;
		public Vector3 eyeOffset2;

		// Head/body info
		public List<Vector4> headVerts;
		public List<Vector2> headUVs;
		public List<uint> headIndices;
		public List<Vector4> bones;
		public List<Vector4> boneIndices;
		public List<Vector4> boneWeights;
		public Matrix4[] boneTransforms;

		public ObjectFactory()
		{
			eyeVerts = new List<Vector4>();
			eyeUVs = new List<Vector2>();
			headVerts = new List<Vector4>();
			headUVs = new List<Vector2>();
			headIndices = new List<uint>();

			CreateHead();
		}

		public void CreateEye(Vector3 pos, bool lr)
		{
			
			Eye newEye = new Eye(pos, new Vector3(0.5f, 0.5f, 0.5f), 1, 2, lr);

			eyeVerts = newEye.verts;
			eyeUVs = newEye.uvs;
		}

		private void CreateHead()
		{
			//Head newHead = new Head("TemocHead1.glb");
			Head newHead = new Head(0.1f);

			Vector3 rotationAxis = Vector3.UnitX;
			float angleInRadians = MathHelper.DegreesToRadians(-90f);
			Quaternion rotation = Quaternion.FromAxisAngle(rotationAxis, angleInRadians);

			headVerts = newHead.faceVerts;
			Vector4 tempAdd = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
			for (int i = 0; i < headVerts.Count; i++)
			{
				headVerts[i] = tempAdd + headVerts[i];
				headVerts[i] = Vector4.Transform(headVerts[i], rotation);		// Rotates the body model so it is // to camera
			}

			headUVs = newHead.faceUVs;
			headIndices = newHead.faceIndices;
			bones = newHead.bones;
			boneIndices = newHead.boneIndices;
			boneWeights = newHead.boneWeights;
			boneTransforms = newHead.boneTransforms;
		}

		public void Dispose()
		{
			

		}

	}
}
