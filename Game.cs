using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using Newtonsoft.Json.Linq;
using System.Diagnostics;


namespace GraphicApllication
{ 
    public class Game : GameWindow
    {
        private Camera _camera = new Camera(new Vector3(0.0f, 1.0f, 30.0f), 4.0f / 3.0f);
        private Shader _shader;
        private List<GraphicObject> _graphicObjects = new ();
        private Light _light = new Light();
        Scene scene = new Scene();

        private Vector2 _lastPos;
        private bool _firstMove = true;

        private Mesh mesh = new Mesh();
        public Game(int width, int height, string title) :
            base(GameWindowSettings.Default, new NativeWindowSettings()
            {
                Size = (width, height),
                Title = title
            })
        { }
        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.TextureColorTableSgi);
            GL.Enable(EnableCap.DepthClamp);
            _light.Ambient = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            _light.Diffuse = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            _light.Specular = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            RenderManager.Instance().Init();
            RenderManager.Instance().SetCamera(_camera);
            RenderManager.Instance().SetLight(_light);
            scene.Init("models.json");
            scene.LoadFromJson("data/scenes/big_scene.json");
            scene.Camera = _camera;
            scene.Light = _light;
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Stopwatch stopwatch = new Stopwatch();
            RenderManager.Instance().Start();
            //Console.Clear();
            stopwatch.Start();
            scene.Draw();
            stopwatch.Stop();
            Console.WriteLine($"SCENE_DRAW:{stopwatch.ElapsedMilliseconds}");
            stopwatch.Start();
            RenderManager.Instance().Finish();
            stopwatch.Stop();
            Console.WriteLine($"REN_FINISH:{stopwatch.ElapsedMilliseconds}");
            SwapBuffers();
            base.Title = $"Lab1 [FPS:{Convert.ToInt64(1 / args.Time)}][{scene.GetSceneDescription()}][{RenderManager.Instance().Texturecalls}][{RenderManager.Instance().Drawcalls}]";
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            const float cameraSpeed = 4.5f;
            const float sensitivity = 0.2f;


            KeyboardState input = KeyboardState;
            MouseState mouse = MouseState;
            if (input.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)args.Time;
            }
            if (input.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)args.Time;
            }
            if (input.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)args.Time;
            }
            if (input.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)args.Time;
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)args.Time;
            }
            if (input.IsKeyDown(Keys.LeftControl))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)args.Time;
            }

            if (mouse.IsButtonDown(MouseButton.Left))
            {
                if (_firstMove)
                {
                    _lastPos = new Vector2(mouse.X, mouse.Y);
                    _firstMove = false;
                }
                else
                {
                    // Calculate the offset of the mouse position
                    var deltaX = mouse.X - _lastPos.X;
                    var deltaY = mouse.Y - _lastPos.Y;
                    _lastPos = new Vector2(mouse.X, mouse.Y);

                    // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                    _camera.Yaw += deltaX * sensitivity;
                    _camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
                }
            }
            else
            {
                _firstMove = true;
            }
        }

        protected override void OnResize(ResizeEventArgs args)
        {
            base.OnResize(args);

            GL.Viewport(0, 0, args.Width, args.Height);
        }
        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteProgram(_shader.Handle);
            
            _shader.Dispose();

            base.OnUnload();
        }
    }
}
