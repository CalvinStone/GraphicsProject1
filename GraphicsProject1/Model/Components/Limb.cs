using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace GraphicsProject1.Model.Components
{
	// 4 limbs for temoc. 
	// Start off with same object rendered four times (for legs and arms as they have different lengths and rotations)
	internal class Limb
	{
		// Initial default limb build
		private List<Vector3> limbModelCoords;
		private List<Vector2> texCoords;
		private uint[] indices;
		private Vector3 axis;
		float angle;

		// Updated coordinates
		public List<Vector3> updatedModelCoords;


		public Limb(Vector3 posOffset, Matrix4 rotation, bool leftRight)
		{
			

		}

		private void BuildBaseLimb()
		{
			limbModelCoords = new List<Vector3>() 
			{ 
				// front face
				new Vector3(-0.5f, 0.5f, 0.5f), // topleft vert
				new Vector3(0.5f, 0.5f, 0.5f), // topright vert
				new Vector3(0.5f, -0.5f, 0.5f), // bottomright vert
				new Vector3(-0.5f, -0.5f, 0.5f), // bottomleft vert
				 // right face
				new Vector3(0.5f, 0.5f, 0.5f), // topleft vert
				new Vector3(0.5f, 0.5f, -0.5f), // topright vert
				new Vector3(0.5f, -0.5f, -0.5f), // bottomright vert
				new Vector3(0.5f, -0.5f, 0.5f), // bottomleft vert
				// back face
				new Vector3(0.5f, 0.5f, -0.5f), // topleft vert
				new Vector3(-0.5f, 0.5f, -0.5f), // topright vert
				new Vector3(-0.5f, -0.5f, -0.5f), // bottomright vert
				new Vector3(0.5f, -0.5f, -0.5f), // bottomleft vert
				// left face
				new Vector3(-0.5f, 0.5f, -0.5f), // topleft vert
				new Vector3(-0.5f, 0.5f, 0.5f), // topright vert
				new Vector3(-0.5f, -0.5f, 0.5f), // bottomright vert
				new Vector3(-0.5f, -0.5f, -0.5f), // bottomleft vert
				// top face
				new Vector3(-0.5f, 0.5f, -0.5f), // topleft vert
				new Vector3(0.5f, 0.5f, -0.5f), // topright vert
				new Vector3(0.5f, 0.5f, 0.5f), // bottomright vert
				new Vector3(-0.5f, 0.5f, 0.5f), // bottomleft vert
				// bottom face
				new Vector3(-0.5f, -0.5f, 0.5f), // topleft vert
				new Vector3(0.5f, -0.5f, 0.5f), // topright vert
				new Vector3(0.5f, -0.5f, -0.5f), // bottomright vert
				new Vector3(-0.5f, -0.5f, -0.5f), // bottomleft vert
			};

			axis = new Vector3(0f, 0f, 0f);

			texCoords = new List<Vector2>()
			{
				new Vector2(0f, 1f),
				new Vector2(1f, 1f),
				new Vector2(1f, 0f),
				new Vector2(0f, 0f),

				new Vector2(0f, 1f),
				new Vector2(1f, 1f),
				new Vector2(1f, 0f),
				new Vector2(0f, 0f),

				new Vector2(0f, 1f),
				new Vector2(1f, 1f),
				new Vector2(1f, 0f),
				new Vector2(0f, 0f),

				new Vector2(0f, 1f),
				new Vector2(1f, 1f),
				new Vector2(1f, 0f),
				new Vector2(0f, 0f),

				new Vector2(0f, 1f),
				new Vector2(1f, 1f),
				new Vector2(1f, 0f),
				new Vector2(0f, 0f),

				new Vector2(0f, 1f),
				new Vector2(1f, 1f),
				new Vector2(1f, 0f),
				new Vector2(0f, 0f),
			};

			indices = new uint[]
			{
				0, 1, 2,
				2, 3, 0,

				4, 5, 6,
				6, 7, 4,

				8, 9, 10,
				10, 11, 8,

				2, 13, 14,
				14, 15, 12,

				16, 17, 18,
				18, 19, 16,

				20, 21, 22,
				22, 23, 20
			};

			angle = 0f;
		}


	}
}
