using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsProject1.Model.Renderables;
using OpenTK.Mathematics;

namespace GraphicsProject1.Model.Components
{
	internal class Eye
	{
		public Vector3 position { get; set; }
		public Vector3 scale { get; set; }
		public List<Vector4> verts;
		public List<Vector2> uvs;
		public Matrix4 rotationMat;
		public Matrix4 modelMatrix;
		public int textureID;

		// Type 1 is full spheres and seperate iris while Type 2 is textured on eye with seperate iris
		public Eye(Vector3 position, Vector3 scale, int textureID, int recursionLevel, bool lr)
		{
			this.scale = scale;
			this.position = position;
			this.textureID = textureID;

			rotationMat = Matrix4.Identity;
			modelMatrix = Matrix4.Identity;


			List<TexturedVertex> eyeTVerts = new List<TexturedVertex>();
			eyeTVerts = CreateSphere(recursionLevel);

			verts = new List<Vector4>();
			uvs = new List<Vector2>();

			// Takes textured coordinate and breaks it up into renderable coordinates
			foreach (TexturedVertex v in eyeTVerts)
			{
				verts.Add(v.pos);
				uvs.Add(v.textureCoordinate);
			}

			// Applies necessary transformations
			Vector3 rotationAxisY = Vector3.UnitY;
			float angleInRadiansY = MathHelper.DegreesToRadians(-90f);
			Quaternion rotationY = Quaternion.FromAxisAngle(rotationAxisY, angleInRadiansY);

			Vector3 rotationAxisZ = Vector3.UnitZ;
			float d2g = 30.0f;
			if (lr)
				d2g = d2g * -1;
			float angleInRadiansZ = MathHelper.DegreesToRadians(d2g);
			Quaternion rotationZ = Quaternion.FromAxisAngle(rotationAxisZ, angleInRadiansZ);

			for (int i = 0; i < verts.Count; i++)
			{
				verts[i] = Vector4.Transform(verts[i], rotationY);
				verts[i] = Vector4.Transform(verts[i], rotationZ);
				verts[i] = ((verts[i].X * scale.X) + position.X, (verts[i].Y * scale.Y) + position.Y, (verts[i].Z * scale.Z) + position.Z, verts[i].W);
				
			}
		}

		public void SetPosition(Vector3 position)
		{
			this.position = position;
		}

		public void SetScale(Vector3 scale)
		{
			this.scale = scale;
		}

		private List<TexturedVertex> CreateSphere(int recursionLevel)
		{
			SphereFactory sphereFactory = new SphereFactory();
			// vertices and tex coordinates the same for both eyes, only the offsets and rotation matices different
			List<TexturedVertex> eyeSet = sphereFactory.Create(recursionLevel);

			return eyeSet;
		}
	}
}
