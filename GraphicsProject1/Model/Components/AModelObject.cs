using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace GraphicsProject1.Model.Components
{
    internal abstract class AModelObject
    {
        public Vector3 position {  get; set; }
        public Vector3 scale { get; set; }
        public List<Vector4> verts;
        public List<Vector4> uvs;
        public Matrix4 rotationMat;
        public Matrix4 modelMatrix;

		// Stretching properties
		public List<Vector3> StretchVectors { get; set; } // Per-vertex stretch
		public float Elasticity { get; set; }
		public float Damping { get; set; }

        // Rendering
        public int textureID { get; set; }


        public AModelObject(Vector3 position, Vector3 scale, int textureID)
        {
            this.scale = scale;
            this.position = position;
            this.textureID = textureID;
        }

        public void SetPosition(Vector3 position)
        {
            position = position;
        }

        public void SetScale(Vector3 scale)
        {
            this.scale = scale;
        }

        // For if object is moving around screen
        public virtual void UpdateObject(double time, double delta)
        {
            
        }


		public virtual void Render(Camera camera)
        {
            // needs possible rotations and scale as well as the model render function
        }
    }
}
