using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GraphicsProject1
{
	// Checks space to see if mouse is over a bone. Needs bounding box possibly.
	internal class MouseCasting
	{
		private Vector2 mouseNDC;
		private Vector4 nearPoint;
		private Vector4 farPoint;
		private int screenWidth;
		private int screenHeight;

		// For triangle checking
		List<Vector3> verts;
		List<int> indices;
		Vector3 rayOrigin;
		Vector3 rayDirection;
		Vector3 v0;
		Vector3 v1;
		Vector3 v2;


		public MouseCasting(int w, int h, List<Vector4> verts4, List<uint> uIndices)
		{
			mouseNDC = new Vector2();
			screenWidth = w;
			screenHeight = h;
			verts = new List<Vector3>();

			foreach (Vector4 v in verts4)
			{
				verts.Add(new Vector3(v.X, v.Y, v.Z));
			}

			indices = new List<int>();
			foreach (uint u in uIndices)
			{
				indices.Add((int)u);
			}

			rayOrigin = new Vector3(0f, 0f, 0f);
			rayDirection = new Vector3(0f, 0f, 0f);
		}

		public (List<int> triIndices, bool hit) Check(MouseState mouse, Matrix4 view, Matrix4 projection)
		{
			
			GenerateMouseRay(mouse, view, projection);
			var result = RayCastMesh(rayOrigin, rayDirection);

			List<int> triangleIndices = new List<int>();

			if (result.hit)
			{
				v0 = verts[indices[result.triangleIndex]];
				v1 = verts[indices[result.triangleIndex + 1]];
				v2 = verts[indices[result.triangleIndex + 2]];
				Console.WriteLine("Ray hit valid triangle");
				triangleIndices.Add(indices[result.triangleIndex]);
				triangleIndices.Add(indices[result.triangleIndex + 1]);
				triangleIndices.Add(indices[result.triangleIndex + 2]);
			}

			return (triangleIndices, result.hit);
		}

		public void GenerateMouseRay(MouseState mouse, Matrix4 view, Matrix4 projection)
		{
			mouseNDC = new Vector2(
				(2.0f * mouse.X) / screenWidth - 1.0f,
				1.0f - (2.0f * mouse.Y) / screenHeight
			);

			Vector4 nearPoint = new Vector4(mouseNDC.X, mouseNDC.Y, -1.0f, 1.0f);
			Vector4 farPoint = new Vector4(mouseNDC.X, mouseNDC.Y, 1.0f, 1.0f);


			Matrix4 invProjectionView = Matrix4.Invert(projection * view);

			Vector4 nearWorld = invProjectionView * nearPoint;
			Vector4 farWorld = invProjectionView * farPoint;


			rayOrigin = new Vector3(nearWorld.X / nearWorld.W, nearWorld.Y / nearWorld.W, nearWorld.Z / nearWorld.W);
			rayDirection = Vector3.Normalize(
				new Vector3(farWorld.X / farWorld.W, farWorld.Y / farWorld.W, farWorld.Z / farWorld.W) - rayOrigin
			);
		}

		public (bool hit, Vector3 intersectionPoint, int triangleIndex) RayCastMesh(Vector3 rayOrigin, Vector3 rayDirection)
		{
			float closestT = float.MaxValue;
			Vector3 closestIntersection = Vector3.Zero;
			int closestTriangleIndex = -1;

			for (int i = 0; i < indices.Count; i += 3)
			{
				// Get the vertices of the triangle
				Vector3 v0 = verts[indices[i]];
				Vector3 v1 = verts[indices[i + 1]];
				Vector3 v2 = verts[indices[i + 2]];

				// Test for intersection
				if (RayIntersectsTriangle(rayOrigin, rayDirection, v0, v1, v2, out float t, out Vector3 intersectionPoint))
				{
					// Check if this is the closest intersection
					if (t < closestT)
					{
						closestT = t;
						closestIntersection = intersectionPoint;
						closestTriangleIndex = i;
					}
				}
			}

			// Return the closest intersection
			return (closestTriangleIndex != -1, closestIntersection, closestTriangleIndex);
		}

		// Moller-Trumbore intersection algorithm
		private bool RayIntersectsTriangle(Vector3 rayOrigin, Vector3 rayDirection, Vector3 v0, Vector3 v1, Vector3 v2, 
			out float t, out Vector3 intersectionPoint)
		{
			const float EPSILON = 1e-8f;
			t = 0;
			intersectionPoint = Vector3.Zero;

			Vector3 edge1 = v1 - v0;
			Vector3 edge2 = v2 - v0;
			Vector3 h = Vector3.Cross(rayDirection, edge2);
			float a = Vector3.Dot(edge1, h);

			if (a > -EPSILON && a < EPSILON) return false; // Parallel, no intersection

			float f = 1.0f / a;
			Vector3 s = rayOrigin - v0;
			float u = f * Vector3.Dot(s, h);
			if (u < 0.0f || u > 1.0f) return false;

			Vector3 q = Vector3.Cross(s, edge1);
			float v = f * Vector3.Dot(rayDirection, q);
			if (v < 0.0f || u + v > 1.0f) return false;

			t = f * Vector3.Dot(edge2, q);
			if (t > EPSILON) // Intersection point is along the ray
			{
				intersectionPoint = rayOrigin + rayDirection * t;
				return true;
			}
			return false;
		}
	}
}
