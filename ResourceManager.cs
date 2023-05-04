using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicApllication
{
    internal class ResourceManager
    {
        private ResourceManager() { }
        private static List<Mesh> _meshes = new List<Mesh> ();
        private static List<Texture> _textures = new List<Texture> ();
        private static List<Material> _materials = new List<Material> ();
        private static Dictionary<string, int> _meshesDictionary = new Dictionary<string, int>();
        private static Dictionary<string, int> _texturesDictionary = new Dictionary<string, int>();
        private static Dictionary<string, int> _materialsDictionary = new Dictionary<string, int>();
        public Mesh GetMesh(int index)
        {
            return _meshes[index];
        }
        public Texture GetTexture(int index)
        {
            return _textures[index];
        }
        public Material GetMaterial(int index)
        {
            return _materials[index];
        }
        public int LoadMesh(string filepath)
        {
            if (_meshesDictionary.ContainsKey(filepath))return _meshesDictionary[filepath];
            Mesh mesh = new Mesh();
            mesh.Load(filepath);
            _meshes.Add(mesh);
            int index = _meshes.Count() - 1;
            _meshesDictionary.Add(filepath, index);
            return index;
        }
        public int LoadTexture(string filepath)
        {
            if (_texturesDictionary.ContainsKey(filepath))return _texturesDictionary[filepath]; 
            Texture texture = Texture.Load(filepath);
            _textures.Add(texture);
            int index = _textures.Count() - 1;
            _texturesDictionary.Add(filepath, index);
            return index;
        }
        public int LoadMaterial(string filepath)
        {
            if (_materialsDictionary.ContainsKey(filepath)) return _materialsDictionary[filepath];
            Material material = new Material();
            material.LoadFromJson(filepath);
            _materials.Add(material);
            int index = _materials.Count() - 1;
            _materialsDictionary.Add(filepath, index);
            return index;
        }
        public static ResourceManager Instance()
        {
            ResourceManager instance = new ResourceManager();
            return instance;
        }
    }
}
