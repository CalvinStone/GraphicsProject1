using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGLTF.Schema2;
using OpenTK.Mathematics;

namespace GraphicsProject1.Model
{
	// Takes blender glb file and extracts necessary components from it
	internal class GLBExtractor
	{
		public List<Vector2> glbUVs;
		public List<Vector4> glbVerts;
		public List<uint> glbIndices;

		// Bone info
		public List<Vector4> bones;
		public List<Vector4> boneIndices;
		public List<Vector4> boneWeights;
		public List<Matrix4> initialBoneMatrices;
		public List<int> boneParents;
		public Matrix4[] boneTransforms;

		public GLBExtractor(string filePath)
		{
			var model = ModelRoot.Load(filePath);
			glbVerts = new List<Vector4>();
			glbUVs = new List<Vector2>();
			glbIndices = new List<uint>();

			bones = new List<Vector4>();
			boneIndices = new List<Vector4>();
			boneWeights = new List<Vector4>();

			initialBoneMatrices = new List<Matrix4>();
			boneParents = new List<int>();


			foreach (var mesh in model.LogicalMeshes)
			{
				foreach (var primitive in mesh.Primitives)
				{
					// Vertices
					var vertexPositions = primitive.GetVertexAccessor("POSITION").AsVector3Array();

					foreach (var position in vertexPositions)
					{
						glbVerts.Add(new Vector4(position.X, position.Y, position.Z, 1.0f));
					}

					// UVs
					var texCoordAccessor = primitive.GetVertexAccessor("TEXCOORD_0");
					if (texCoordAccessor != null) // Check if the attribute exists
					{
						var texCoords = texCoordAccessor.AsVector2Array();
						foreach (var uv in texCoords)
						{
							glbUVs.Add(new Vector2(uv.X,1.0f - uv.Y));
						}
					}

					// Indices
					var indices = primitive.GetIndices();
					glbIndices.AddRange(indices);

					// Bones and weights
					var jointAccessor = primitive.GetVertexAccessor("JOINTS_0");
					var weightAccessor = primitive.GetVertexAccessor("WEIGHTS_0");
					if (jointAccessor != null && weightAccessor != null)
					{
						var joints = jointAccessor.AsVector4Array();
						var weights = weightAccessor.AsVector4Array();
						foreach (var joint in joints)
						{
							boneIndices.Add(new Vector4(joint.X, joint.Y, joint.Z, joint.W));
						}
						foreach (var weight in weights)
						{
							boneWeights.Add(new Vector4(weight.X, weight.Y, weight.Z, weight.W));
						}
					}


					
					// Texture Data
					foreach (var material in model.LogicalMaterials)
					{
						var texture = material.FindChannel("BaseColor")?.Texture;	// Has multiple channels but only need the BaseColor
						if (texture != null)
						{
							var image = texture.PrimaryImage;
							var imageBytes = image.Content.Content.ToArray();

							string fileName = Path.Combine("../../../Textures", $"{material.Name ?? "texture"}_{image.Name ?? "basecolor"}.png");

							if (!File.Exists(fileName))
							{
								File.WriteAllBytes(fileName, imageBytes);			// Saves image as png
								Console.WriteLine($"Texture saved to: {fileName}");	// Confirms in console if saved
							}
								
						}
					}
					
				}

				foreach (var scene in model.LogicalScenes)
				{
					foreach (var node in scene.VisualChildren)
					{
						if (node.Skin != null)
						{
							ExtractSkinData(node.Skin);
						}
					}
				}
			}


			// Some of this needs to be moved to the game class
			Matrix4[] inverseBindPoseMat = GetInverseBindPoseMatrices(model);

			boneTransforms =  CalculateBoneTransforms(model, inverseBindPoseMat);

			
		}

		private Matrix4[] GetInverseBindPoseMatrices(ModelRoot model)
		{
			if (model.LogicalSkins.Count == 0)
			{
				throw new Exception("No skinning data found in the model.");
			}

			var skin = model.LogicalSkins[0]; // Assuming the model has one skin
			var inverseBindMatrices = new Matrix4[skin.Joints.Count];

			for (int i = 0; i < skin.Joints.Count; i++)
			{
				var matrix = skin.InverseBindMatrices[i];
				inverseBindMatrices[i] = new Matrix4(
					matrix.M11, matrix.M12, matrix.M13, matrix.M14,
					matrix.M21, matrix.M22, matrix.M23, matrix.M24,
					matrix.M31, matrix.M32, matrix.M33, matrix.M34,
					matrix.M41, matrix.M42, matrix.M43, matrix.M44
				);
			}

			return inverseBindMatrices;
		}

		private Matrix4[] CalculateBoneTransforms(ModelRoot model, Matrix4[] inverseBindPoseMatrices)
		{
			var skin = model.LogicalSkins[0]; // Assuming the model has one skin
			var jointNodes = skin.Joints.ToList(); // Convert to IList

			if (jointNodes == null || jointNodes.Count == 0)
			{
				throw new Exception("No joint nodes found in the skin.");
			}

			int boneCount = jointNodes.Count;

			// Build the parent-child relationship
			int[] boneParents = BuildParentChildRelationshipsForJoints(model, jointNodes);

			// Initialize currentBoneMatrices and worldMatrices
			Matrix4[] currentBoneMatrices = new Matrix4[boneCount];
			Matrix4[] worldMatrices = new Matrix4[boneCount];
			Matrix4[] boneTransforms = new Matrix4[boneCount];

			for (int i = 0; i < boneCount; i++)
			{
				var node = jointNodes[i];

				if (node == null)
				{
					throw new Exception($"Joint node at index {i} is null.");
				}

				// Extract local transform from the joint node
				Matrix4 localTransform = new Matrix4(
					node.LocalMatrix.M11, node.LocalMatrix.M12, node.LocalMatrix.M13, node.LocalMatrix.M14,
					node.LocalMatrix.M21, node.LocalMatrix.M22, node.LocalMatrix.M23, node.LocalMatrix.M24,
					node.LocalMatrix.M31, node.LocalMatrix.M32, node.LocalMatrix.M33, node.LocalMatrix.M34,
					node.LocalMatrix.M41, node.LocalMatrix.M42, node.LocalMatrix.M43, node.LocalMatrix.M44
				);

				currentBoneMatrices[i] = localTransform;

				// Calculate world matrix
				if (boneParents[i] == -1) // Root bone
				{
					worldMatrices[i] = currentBoneMatrices[i];
				}
				else
				{
					worldMatrices[i] = worldMatrices[boneParents[i]] * currentBoneMatrices[i];
				}

				// Combine with inverse bind pose matrix
				boneTransforms[i] = worldMatrices[i] * inverseBindPoseMatrices[i];
			}

			return boneTransforms;
		}

		private int[] BuildParentChildRelationshipsForJoints(ModelRoot model, IList<Node> jointNodes)
		{
			var nodeIndexMap = CreateNodeIndexMapping(model); // Map all nodes to their indices
			int[] boneParents = new int[jointNodes.Count];

			// Initialize all parents to -1 (default for root nodes)
			for (int i = 0; i < boneParents.Length; i++) boneParents[i] = -1;

			for (int i = 0; i < jointNodes.Count; i++)
			{
				var node = jointNodes[i];

				if (node == null)
				{
					throw new Exception($"Joint node at index {i} is null.");
				}

				// Check if the node has a parent in the joint list
				if (node.VisualParent != null && nodeIndexMap.TryGetValue(node.VisualParent, out int parentIndex))
				{
					int parentInJointNodes = jointNodes.IndexOf(node.VisualParent);
					if (parentInJointNodes != -1)
					{
						boneParents[i] = parentInJointNodes; // Get parent index in jointNodes
					}
				}
			}

			return boneParents;
		}

		private Dictionary<Node, int> CreateNodeIndexMapping(ModelRoot model)
		{
			var nodeIndexMap = new Dictionary<Node, int>();

			for (int i = 0; i < model.LogicalNodes.Count; i++)
			{
				var node = model.LogicalNodes[i];
				if (node == null)	// Check if validating
				{
					Console.WriteLine($"Node at index {i} is null.");
					continue;
				}

				nodeIndexMap[node] = i;
			}

			return nodeIndexMap;
		}

		private void ExtractSkinData(Skin skin)
		{
			foreach (var joint in skin.Joints)
			{
				var jointMatrix = joint.WorldMatrix;
				var translation = new Vector3(jointMatrix.M41, jointMatrix.M42, jointMatrix.M43);
				bones.Add(new Vector4(translation.X, translation.Y, translation.Z, 1.0f));
			}
		}

	}
}
