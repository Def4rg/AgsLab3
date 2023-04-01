using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using System.Diagnostics;

namespace GraphicApllication
{
    internal class Scene
    {
        private Camera _camera;
        private Light _light;
        private JObject _document;
        private List<GraphicObject>[] _graphicsObjects = new List<GraphicObject>[2];
        private int _renderedObjectsCount;
        private GraphicObject _createGraphicObject(string model)
        {
            var graphicObject = new GraphicObject();
            switch ((string)_document[model]["type"])
            {
                case "none":
                    graphicObject.Type = GraphicObjectType.none;
                    break;
                case "road":
                    graphicObject.Type = GraphicObjectType.road;
                    break;
                case "building":
                    graphicObject.Type = GraphicObjectType.building;
                    break;
                case "vehicle":
                    graphicObject.Type = GraphicObjectType.vehicle;
                    break;
                case "big nature":
                    graphicObject.Type = GraphicObjectType.big_nature;
                    break;
                case "small nature":
                    graphicObject.Type = GraphicObjectType.small_nature;
                    break;
                case "big prop":
                    graphicObject.Type = GraphicObjectType.big_prop;
                    break;
                case "medium prop":
                    graphicObject.Type = GraphicObjectType.medium_prop;
                    break;
                case "small prop":
                    graphicObject.Type = GraphicObjectType.small_prop;
                    break;
            }
            graphicObject.Dimensions = new Vector3((float)_document[model]["dimensions"][0], (float)_document[model]["dimensions"][1], (float)_document[model]["dimensions"][2]);
            graphicObject.MeshId = ResourceManager.Instance().LoadMesh((string)_document[model]["mesh"]);
            graphicObject.TextureId = ResourceManager.Instance().LoadTexture((string)_document[model]["texture"]);
            graphicObject.MaterialId = ResourceManager.Instance().LoadMaterial((string)_document[model]["material"]);
            return graphicObject;
        }
        private bool _lodTest(GraphicObject graphicObject)
        {
            float distance = Vector3.Distance(_camera.Position, graphicObject.Position);
            switch (graphicObject.Type)
            {
                case GraphicObjectType.vehicle:
                    if (distance > 500) return false;
                    break;
                case GraphicObjectType.big_nature:
                    if (distance > 350) return false;
                    break;
                case GraphicObjectType.small_nature:
                    if (distance > 200) return false;
                    break;
                case GraphicObjectType.big_prop:
                    if (distance > 400) return false;
                    break;
                case GraphicObjectType.medium_prop:
                    if (distance > 300) return false;
                    break;
                case GraphicObjectType.small_prop:
                    if (distance > 200) return false;
                    break;
            }
            return true;
        }
        private bool _frustrumCullingTest(GraphicObject graphicObject)
        {
            Matrix4 viewMat = _camera.GetViewMatrix();
            viewMat.Transpose();
            Matrix4 projectionMat = _camera.GetProjectionMatrix();
            projectionMat.Transpose();
            Matrix4 modelMat = graphicObject.ModelMatrix;
            modelMat.Transpose();
            Matrix4 PVM = projectionMat * viewMat * modelMat;
            Vector4[] obb_points = new Vector4[8];
            for (int i = 0; i < 8; i++)
            {
                obb_points[i] = PVM * graphicObject.Vertex[i];
            }
            bool outside = false, outside_positive_plane, outside_negative_plane;
            for (int i = 0; i < 3; i++)
            {
                outside_positive_plane =
                    obb_points[0][i] > obb_points[0].W &&
                    obb_points[1][i] > obb_points[1].W &&
                    obb_points[2][i] > obb_points[2].W &&
                    obb_points[3][i] > obb_points[3].W &&
                    obb_points[4][i] > obb_points[4].W &&
                    obb_points[5][i] > obb_points[5].W &&
                    obb_points[6][i] > obb_points[6].W &&
                    obb_points[7][i] > obb_points[7].W;
                outside_negative_plane =
                    obb_points[0][i] < -obb_points[0].W &&
                    obb_points[1][i] < -obb_points[1].W &&
                    obb_points[2][i] < -obb_points[2].W &&
                    obb_points[3][i] < -obb_points[3].W &&
                    obb_points[4][i] < -obb_points[4].W &&
                    obb_points[5][i] < -obb_points[5].W &&
                    obb_points[6][i] < -obb_points[6].W &&
                    obb_points[7][i] < -obb_points[7].W;
                outside = outside || outside_positive_plane || outside_negative_plane;
            }
            return !outside;
        }
        public void Init(string filepath)
        {
            _document = JObject.Parse(File.ReadAllText(filepath));
            for(int index = 0; index < _graphicsObjects.Length; index++)
            {
                _graphicsObjects[index] = new List<GraphicObject>();
            }
        }
        public void LoadFromJson(string filepath)
        {
            int index = 0;
            JArray json = JArray.Parse(File.ReadAllText(filepath));
            foreach (JObject elem in json)
            {
                GraphicObject graphicObject = new GraphicObject();
                graphicObject = _createGraphicObject(elem.GetValue("model").ToString());
                graphicObject.AngleY = (float)elem.GetValue("angle");
                graphicObject.Position = new Vector3((float)elem.GetValue("position")[0], (float)elem.GetValue("position")[1], (float)elem.GetValue("position")[2]);
                _graphicsObjects[index].Add(graphicObject);
                index++;
                if (index >= _graphicsObjects.Length) index = 0;
            }

        }
        public Camera Camera
        {
            set => _camera = value;
        }
        public Light Light
        {
            set => _light = value;
        }
        public void Draw()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            _renderedObjectsCount = 0;
            var Threads = new Thread[2];
            List<GraphicObject> queue1 = new List<GraphicObject>();
            List<GraphicObject> queue2 = new List<GraphicObject>();
            Thread task = new Thread(() =>
            {
                foreach (var elem in _graphicsObjects[0])
                {
                    if (!_lodTest(elem)) continue;
                    if (!_frustrumCullingTest(elem)) continue;
                    _renderedObjectsCount++;
                    queue1.Add(elem);
                }
            });
            Threads[0] = task;
            task.Start();
            Thread task1 = new Thread(() =>
            {
                foreach (var elem in _graphicsObjects[1])
                {
                    if (!_lodTest(elem)) continue;
                    if (!_frustrumCullingTest(elem)) continue;
                    _renderedObjectsCount++;
                    queue2.Add(elem);
                }
            });
            Threads[1] = task1;
            task1.Start();

            for (int index = 0; index < 2; index++)
            {
                Threads[index].Join();
            }
            sw.Stop();
            //Console.WriteLine(sw.ElapsedMilliseconds);
            RenderManager.Instance().Queue = queue1.Concat(queue2).ToList();
        }
        public string GetSceneDescription()
        {
            return $"{_renderedObjectsCount}/{_graphicsObjects[0].Count}";
        }
    }
}
