using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Newtonsoft.Json.Linq;

namespace GraphicApllication
{
    internal class Material
    {
        private Vector4 _ambient;
        private Vector4 _diffuse;
        private Vector4 _specular;
        private float _shininess;
        public void LoadFromJson(string filepath)
        {
            string text = File.ReadAllText(filepath);
            var json = JObject.Parse(text);
            _ambient = new Vector4((float)json["ambient"][0], (float)json["ambient"][1], (float)json["ambient"][2], (float)json["ambient"][3]);
            _diffuse = new Vector4((float)json["diffuse"][0], (float)json["diffuse"][1], (float)json["diffuse"][2], (float)json["diffuse"][3]);
            _specular = new Vector4((float)json["specular"][0], (float)json["specular"][1], (float)json["specular"][2], (float)json["specular"][3]);
            _shininess = (float)json["shininess"];
        }
        public Vector4 Ambient
        {
            get => _ambient;
        }
        public Vector4 Diffuse
        {
            get => _diffuse;
        }
        public Vector4 Specular
        {
            get => _specular;
        }
        public float Shininess
        {
            get => _shininess;
        }
    }
}
