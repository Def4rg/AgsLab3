using OpenTK.Mathematics;

namespace GraphicApllication
{
    internal class Light
    {
        private Vector4 _direction;
        private Vector4 _position;
        private Vector4 _ambient;
        private Vector4 _diffuse;
        private Vector4 _specular;
        public Vector4 Direction
        {
            get => _direction;
            set
            {
                _direction = value;
            }
        }
        public Vector4 Position
        {
            get => _position;
            set
            {
                _position = value;
            }
        }
        public Vector4 Ambient
        {
            get => _ambient;
            set
            {
                _ambient = value;
            }
        }
        public Vector4 Diffuse
        {
            get => _diffuse;
            set
            {
                _diffuse = value;
            }
        }
        public Vector4 Specular
        {
            get => _specular;
            set
            {
                _specular = value;
            }
        }
    }
}
