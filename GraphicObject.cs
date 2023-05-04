using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace GraphicApllication
{
    public enum GraphicObjectType
    {
        none, road, building, vehicle,
        big_nature, small_nature, big_prop, medium_prop, small_prop
    }
    public class GraphicObject
    {
        private GraphicObjectType _type;

        private float _angleX = 0;
        private float _angleY = 0;
        private float _angleZ = 0;

        public Vector3 _dimensions = new(); 
        public Vector4 _color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        public List<Vector4> Vertex = new List<Vector4>();


        private Matrix4 _modelMatrix = new Matrix4(
            new Vector4(1.0f, 0.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 1.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 0.0f, 1.0f, 0.0f),
            new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
        );
        private int _meshId;
        public Matrix4 ModelMatrix
        {
            get => _modelMatrix;
        }
        public int MeshId
        {
            set => _meshId = value;
            get => _meshId;
        }
        private int _textureId;
        public int TextureId
        {
            set => _textureId = value;
            get => _textureId;
        }
        private int _materialId;
        public int MaterialId
        {
            set => _materialId = value;
            get => _materialId;
        }
        public GraphicObject()
        {

        }
        public GraphicObjectType Type
        {
            get => _type;
            set => _type = value;
        }
        public Vector3 Dimensions
        {
            get => _dimensions;
            set
            {
                _dimensions = value;
                Vertex.Add(new Vector4(+_dimensions.X / 2, +_dimensions.Y / 2, +_dimensions.Z / 2, 1.0f));
                Vertex.Add(new Vector4(+_dimensions.X / 2, +_dimensions.Y / 2, -_dimensions.Z / 2, 1.0f));
                Vertex.Add(new Vector4(+_dimensions.X / 2, -_dimensions.Y / 2, +_dimensions.Z / 2, 1.0f));
                Vertex.Add(new Vector4(+_dimensions.X / 2, -_dimensions.Y / 2, -_dimensions.Z / 2, 1.0f));
                Vertex.Add(new Vector4(-_dimensions.X / 2, +_dimensions.Y / 2, +_dimensions.Z / 2, 1.0f));
                Vertex.Add(new Vector4(-_dimensions.X / 2, +_dimensions.Y / 2, -_dimensions.Z / 2, 1.0f));
                Vertex.Add(new Vector4(-_dimensions.X / 2, -_dimensions.Y / 2, +_dimensions.Z / 2, 1.0f));
                Vertex.Add(new Vector4(-_dimensions.X / 2, -_dimensions.Y / 2, -_dimensions.Z / 2, 1.0f));
            }
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public GraphicObject(Vector3 coord)
        {
            _modelMatrix.Row3 = new Vector4(coord, 1);
        }
        public GraphicObject(Vector3 coord, float angleX, float angleY, float angleZ)
        {
            if (angleX != 0) AngleX = angleX;
            if (angleY != 0) AngleY = angleY;
            if (angleZ != 0) AngleZ = angleZ;
            _modelMatrix.Row3 = new Vector4(coord, 1);
        }
        public float AngleX
        {
            get => MathHelper.RadiansToDegrees(_angleX);
            set
            {
                _angleX = MathHelper.DegreesToRadians(value);
                var rotationMatX = new Matrix4(
                    new Vector4(1.0f, 0.0f, 0.0f, 0.0f),
                    new Vector4(0.0f, (float)MathHelper.Cos(_angleX), -(float)MathHelper.Sin(_angleX), 0),
                    new Vector4(0.0f, (float)MathHelper.Sin(_angleX), (float)MathHelper.Cos(_angleX), 0.0f),
                    new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
                );
                _modelMatrix *= rotationMatX;
            }
        }
        public float AngleY
        {
            get => MathHelper.RadiansToDegrees(_angleY);
            set
            {
                _angleY = MathHelper.DegreesToRadians(value);
                var rotationMatY = new Matrix4(
                    new Vector4((float)MathHelper.Cos(_angleY), 0.0f, (float)MathHelper.Sin(_angleY), 0.0f),
                    new Vector4(0.0f, 1.0f, 0.0f, 0.0f),
                    new Vector4(-(float)MathHelper.Sin(_angleY), 0.0f, (float)MathHelper.Cos(_angleY), 0.0f),
                    new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
                );
                _modelMatrix *= rotationMatY;
            }
        }
        public float AngleZ
        {
            get => MathHelper.RadiansToDegrees(_angleZ);
            set
            {
                _angleZ = MathHelper.DegreesToRadians(value);
                var rotationMatZ = new Matrix4(
                    new Vector4((float)MathHelper.Cos(_angleZ), -(float)MathHelper.Sin(_angleZ), 0.0f, 0.0f),
                    new Vector4((float)MathHelper.Sin(_angleZ), (float)MathHelper.Cos(_angleZ), 0.0f, 0.0f),
                    new Vector4(0.0f, 0.0f, 1.0f, 0.0f),
                    new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
                );
                _modelMatrix *= rotationMatZ;
            }
        }
        public Vector4 Color
        {
            get => _color;
            set => _color = value;
        }
        public Vector3 Position
        {
            get => new Vector3(_modelMatrix.Row3);
            set => _modelMatrix.Row3 = new Vector4(value, 1.0f);
        }
        public void Draw()
        {
            ResourceManager.Instance().GetTexture(_textureId).Bind(TextureUnit.Texture0);
            ResourceManager.Instance().GetMesh(_meshId).DrawOne();
        }
    }
}
