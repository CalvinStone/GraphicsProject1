using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using GraphicsProject1.Graphics;
using GraphicsProject1.Model;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics.Contracts;


namespace GraphicsProject1
{
	
	internal class RenderScene
	{
		// Render pipeline
		private VAO eyeVAO1;
		private VAO eyeVAO2;
		private VAO headVAO1;
		private ShaderProgram program;
		private Texture textureHead;
		private Texture textureEye;
		public ObjectFactory factory;
		private IBO headIndicesIBO;
		private IBO eyeIndicesIBO;
		private MouseCasting mouseCast;
		private List<int> triIndi;
		private int dominantBone;
		private Vector2 initialMousePosition;

		public RenderScene(int width, int height)
		{
			LoadScene(width, height);

		}

		public void LoadScene(int screenWidth, int screenHeight)
		{
			eyeVAO1 = new VAO();
			eyeVAO2 = new VAO();
			headVAO1 = new VAO();
			
			factory = new ObjectFactory();


			// Eye1 mesh
			factory.CreateEye(new Vector3(-1.2f, 17.3f, 2.1f), false);


			VBO eye1VBOverts = new VBO(factory.eyeVerts);
			eyeVAO1.LinkToVAO(0, 4, eye1VBOverts);

			VBO eyeVBOuvs = new VBO(factory.eyeUVs);
			eyeVAO1.LinkToVAO(1, 2, eyeVBOuvs);

			List<uint> eyeIndices = new List<uint>();
			uint tempInt = 0;
			foreach (Vector4 vert in factory.eyeVerts)
			{
				eyeIndices.Add(tempInt);
				tempInt++;
			}

			eyeIndicesIBO = new IBO(eyeIndices);

			// Eye2 mesh reuses mesh 1 info
			factory.CreateEye(new Vector3(1.2f, 17.2f, 2.1f), true);

			VBO eye2VBOverts = new VBO(factory.eyeVerts);
			eyeVAO2.LinkToVAO(0, 4, eye2VBOverts);
			eyeVAO2.LinkToVAO(1, 2, eyeVBOuvs);


			// Head Mesh
			VBO headVBOVerts = new VBO(factory.headVerts);
			headVAO1.LinkToVAO(0, 4, headVBOVerts);

			VBO headVBOuvs = new VBO(factory.headUVs);
			headVAO1.LinkToVAO(1, 2, headVBOuvs);

			VBO headVBOBonesIndices = new VBO(factory.boneIndices);
			headVAO1.LinkToVAO(2, 4, headVBOBonesIndices);

			VBO headVBOBoneWeights = new VBO(factory.boneWeights);
			headVAO1.LinkToVAO(3, 4, headVBOBoneWeights);

			headIndicesIBO = new IBO(factory.headIndices);

			mouseCast = new MouseCasting(screenWidth, screenHeight, factory.headVerts, factory.headIndices);


			// Load textures
			textureEye = new Texture("material_Image_1.png");
			textureHead = new Texture("body_Image_0.png");
		}		

		// Will be rendered with bones flag false
		public void RenderEyes(ShaderProgram program)
		{
			program.Bind();

			eyeVAO1.Bind();
			textureEye.Bind();
			eyeIndicesIBO.Bind();
			GL.DrawElements(PrimitiveType.Triangles, factory.eyeVerts.Count, DrawElementsType.UnsignedInt, 0);
			eyeVAO1.Unbind();

			eyeVAO2.Bind();
			textureEye.Bind();
			eyeIndicesIBO.Bind();
			GL.DrawElements(PrimitiveType.Triangles, factory.eyeVerts.Count, DrawElementsType.UnsignedInt, 0);
		}

		// Will be rendered with bones flag true
		public void RenderHead(ShaderProgram program)
		{
			headVAO1.Bind();
			textureHead.Bind();
			headIndicesIBO.Bind();
			GL.DrawElements(PrimitiveType.Triangles, factory.headIndices.Count, DrawElementsType.UnsignedInt, 0);
		}

		public void Update(MouseState mouse, Matrix4 view, Matrix4 projection)
		{

			if (mouse.IsButtonPressed(MouseButton.Left))	// If mouse just clicked
			{
				var result = mouseCast.Check(mouse, view, projection);
				Console.WriteLine("Button is pressed");
				if (result.hit)
				{
					List<int> triIndi = result.triIndices;
					int dominantBone = FindDominantBone(factory.boneWeights, factory.boneIndices, triIndi);
					Vector2 initialMousePosition = mouse.Position;
				}
			}
			else if (mouse.IsButtonDown(MouseButton.Left))	// If mouse is held
			{
				Vector2 mouseDelta = mouse.Delta;
				float sensitivity = 0.01f;

				// Map mouse delta to 3D deformation
				Vector3 stretchDirectionX = new Vector3(1, 0, 0);
				Vector3 stretchDirectionY = new Vector3(0, 1, 0);
				Vector3 stretchDirectionZ = new Vector3(0, 0, 1);
				Vector3 deformation = (stretchDirectionX * mouseDelta.X * sensitivity) +
									  (stretchDirectionY * mouseDelta.Y * sensitivity);

				List<int> affectedVertices = GetAffectedVertices(dominantBone, factory.boneWeights, factory.boneIndices);


				// Apply deformation to vertices
				
				foreach (int vertexIndex in affectedVertices) // GPU Based
				{
					float weight = factory.boneWeights[vertexIndex][dominantBone];
					if (weight > 0)		// Kept throwing errors by not adding things up this way for some reason
					{
						// Convert deformation to Vector3
						Vector3 deformation3 = new Vector3(deformation.X, deformation.Y, deformation.Z) * weight;
						Vector4 weightedVert = factory.headVerts[vertexIndex];
						weightedVert.X = weightedVert.X + deformation3.X;
						weightedVert.Y = weightedVert.Y + deformation3.Y;
						weightedVert.Z = weightedVert.Z + deformation3.Z;

						factory.headVerts[vertexIndex] = weightedVert;
					}
				}
				
				/*
				foreach (int vertexIndex in affectedVertices) //cpu based
				{
					Vector4 weights = factory.boneWeights[vertexIndex];
					Vector4 indices = factory.boneIndices[vertexIndex];

					Vector3 newPosition = Vector3.Zero;

					// Apply bone influences
					for (int i = 0; i < 4; i++)
					{
						int boneIndex = (int)indices[i];
						if (weights[i] > 0)
						{
							Vector4 transformedVertex = boneTransforms[boneIndex] * new Vector4(vertices[vertexIndex], 1.0f);
							newPosition += new Vector3(transformedVertex.X, transformedVertex.Y, transformedVertex.Z) * weights[i];
						}
					}

					vertices[vertexIndex] = newPosition;
				}
				*/

				// Update vertex buffer
				VBO stretchedVerts = new VBO(factory.headVerts);
				headVAO1.LinkToVAO(0, 4, stretchedVerts);
			}
		
		}


		// Used to find bone that most strongly affects grabbed area
		private int FindDominantBone(List<Vector4> boneWeights, List<Vector4> boneIndices, List<int> triangleIndices)
		{
			float maxWeight = 0f;
			int dominantBone = -1;

			// Iterate over the three vertices of the triangle
			foreach (int vertexIndex in triangleIndices)
			{
				Vector4 weights = boneWeights[vertexIndex]; 
				Vector4 indices = boneIndices[vertexIndex]; 

				// Check each of the 4 weights for this vertex
				for (int i = 0; i < 4; i++)
				{
					if (weights[i] > maxWeight)
					{
						maxWeight = weights[i];
						dominantBone = (int)indices[i]; // Update the dominant bone index
					}
				}
			}

			return dominantBone;
		}

		List<int> GetAffectedVertices(int dominantBone, List<Vector4> boneWeights, List<Vector4> boneIndices)
		{
			List<int> affectedVertices = new List<int>();

			for (int i = 0; i < boneWeights.Count; i++) // Iterate over all vertices
			{
				Vector4 weights = boneWeights[i];
				Vector4 indices = boneIndices[i];

				// Check if the dominant bone influences this vertex
				for (int j = 0; j < 4; j++) // Check all 4 possible bone weights for this vertex
				{
					if ((int)indices[j] == dominantBone && weights[j] > 0.0f)
					{
						affectedVertices.Add(i);
						break; // Skip to the next vertex once we find a match
					}
				}
			}

			return affectedVertices;
		}

		public void Delete()
		{
			eyeVAO1.Delete();
			headVAO1.Delete();
			textureEye.Delete();
			textureHead.Delete();
		}
	}
}
