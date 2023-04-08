using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;

namespace GraphicApllication
{
    internal class RenderManager
    {
        private static List<Shader> _shaders = new List<Shader>();
        private static List<GraphicObject> _graphicObjects = new List<GraphicObject>();
        private static int _listNum = 0;
        private static Camera _camera;
        private static ResourceManager _instance = ResourceManager.Instance();
        private static Light _light;
        private static int _drawcalls = 0;
        private static int _texturecalls = 0;
        public int Drawcalls
        {
            get => _drawcalls;
        }
        public List<GraphicObject> Queue
        {
            set => _graphicObjects = value;
        }
        public int Texturecalls
        {
            get => _texturecalls;
        }
        private RenderManager() { }
        public static RenderManager Instance()
        {
            RenderManager instance = new RenderManager();
            return instance;
        }
        public void Init()
        {
            _shaders.Add(new Shader("data/Shaders/DiffuseTextureInstanced.vsh", "data/Shaders/DiffuseTextureInstanced.fsh"));
        }
        public void Start()
        {
            _graphicObjects.Clear();
            _drawcalls = 0;
            _texturecalls = 0;
        }
        public void SetCamera(Camera camera)
        {
            _camera = camera;
        }
        public void SetLight(Light light)
        {
            _light = light;
        }
        public void AddToRenderQueue(GraphicObject graphicObject)
        {
            _graphicObjects.Add(graphicObject);
        }
        public void Finish()
        {
            if (_graphicObjects.Count == 0)
            {
                return;
            }
            _graphicObjects.Sort((x, y) =>
            {
                if (x.TextureId > y.TextureId)
                {
                    return 1;
                }
                else if (x.TextureId < y.TextureId)
                {
                    return -1;
                }
                else
                {
                    if (x.MeshId > y.MeshId)
                    {
                        return 1;
                    }
                    else if (x.MeshId < y.MeshId)
                    {
                        return -1;
                    }
                }
                return 0;
            });
            List<Matrix4> models = new List<Matrix4>();
            _shaders[0].Use();
            _shaders[0].setUniform("viewMatrix", _camera.GetViewMatrix());
            _shaders[0].setUniform("projectionMatrix", _camera.GetProjectionMatrix());
            _shaders[0].setUniform("lAmbient", _light.Ambient);
            _shaders[0].setUniform("lDiffuse", _light.Diffuse);
            _shaders[0].setUniform("lSpecular", _light.Specular);
            _shaders[0].setUniform("lPosition", new Vector4(0.0f, 100.0f, 0.0f, 20.0f));
            _shaders[0].setUniform("color", _graphicObjects[0].Color);
            _shaders[0].setUniform("mAmbient", _instance.GetMaterial(_graphicObjects[0].MaterialId).Ambient);
            _shaders[0].setUniform("mDiffuse", _instance.GetMaterial(_graphicObjects[0].MaterialId).Diffuse);
            _shaders[0].setUniform("mSpecular", _instance.GetMaterial(_graphicObjects[0].MaterialId).Specular);
            _shaders[0].setUniform("mShininess", _instance.GetMaterial(_graphicObjects[0].MaterialId).Shininess);
            _instance.GetTexture(_graphicObjects[0].TextureId).Bind(TextureUnit.Texture0);
            for (int index = 1; index < _graphicObjects.Count(); index++)
            {
                if (_graphicObjects[index - 1].MaterialId != _graphicObjects[index].MaterialId)
                {
                    _shaders[0].setUniform("mAmbient", _instance.GetMaterial(_graphicObjects[index].MaterialId).Ambient);
                    _shaders[0].setUniform("mDiffuse", _instance.GetMaterial(_graphicObjects[index].MaterialId).Diffuse);
                    _shaders[0].setUniform("mSpecular", _instance.GetMaterial(_graphicObjects[index].MaterialId).Specular);
                    _shaders[0].setUniform("mShininess", _instance.GetMaterial(_graphicObjects[index].MaterialId).Shininess);
                }
                models.Add(_graphicObjects[index - 1].ModelMatrix);
                if (models.Count == 200 || _graphicObjects[index - 1].MeshId != _graphicObjects[index].MeshId || _graphicObjects[index - 1].MaterialId != _graphicObjects[index].MaterialId || _graphicObjects[index - 1].TextureId != _graphicObjects[index].TextureId)
                {
                    for (int dentindex = 0; dentindex < models.Count(); dentindex++)
                    {
                        _shaders[0].setUniform($"modelMatrix[{dentindex}]", models[dentindex]);
                    }
                    _instance.GetMesh(_graphicObjects[index - 1].MeshId).DrawMany(models.Count);
                    _drawcalls++;
                    models.Clear();
                }
                if (_graphicObjects[index - 1].TextureId != _graphicObjects[index].TextureId)
                {
                    _instance.GetTexture(_graphicObjects[index].TextureId).Bind(TextureUnit.Texture0);
                    _texturecalls++;
                }
            }
            models.Add(_graphicObjects[_graphicObjects.Count() - 1].ModelMatrix);
            for (int dentindex = 0; dentindex < models.Count(); dentindex++)
            {
                _shaders[0].setUniform($"modelMatrix[{dentindex}]", models[dentindex]);
            }
            _instance.GetMesh(_graphicObjects[_graphicObjects.Count() - 1].MeshId).DrawMany(models.Count());
            _drawcalls++;
            //_shaders[0].Dispose();
        }
    }
}
