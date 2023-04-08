using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using System.Diagnostics;


namespace GraphicApllication
{ 
    public class Game : GameWindow
    {
        private double time;
        private Camera _camera = new Camera(new Vector3(0.0f, 1.0f, 30.0f), 4.0f / 3.0f);
        private List<Shader> _shader = new List<Shader>();
        private int index = 0;
        private List<GraphicObject> _graphicObjects = new ();
        private Light _light = new Light();
        Scene scene = new Scene();

        private Vector2 _lastPos;
        private bool _firstMove = true;

        private Mesh mesh = new Mesh();

        private FBO _filter0 = new FBO();
        private FBO _filter1 = new FBO();
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
            GL.ClearColor(0.4f, 0.4f, 0.4f, 0.4f);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.TextureColorTableSgi);
            GL.Enable(EnableCap.DepthClamp);
            _filter0.Init(800, 600, true);
            _filter0.unbind();
            _filter1.Init(800, 600, false);
            _filter1.unbind();
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

            _shader.Add(new Shader("data/Shaders/SimplePostProcessing.vsh", "data/Shaders/SimplePostProcessing.fsh"));
            _shader.Add(new Shader("data/Shaders/GreyPostProcessing.vsh", "data/Shaders/GreyPostProcessing.fsh"));
            _shader.Add(new Shader("data/Shaders/SepiaPostProcessing.vsh", "data/Shaders/SepiaPostProcessing.fsh"));
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _filter0.bind();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            RenderManager.Instance().Start();
            scene.Draw();
            RenderManager.Instance().Finish();
            _filter0.ResolveToFBO(_filter1);
            _filter0.unbind();

            _filter1.bindColorTexture();
            _shader[index].Use();
            _shader[index].setUniform("texture_0", 0);
            DrawBox();
            time += args.Time;
            if(time > 0.3f)
            {
                base.Title = $"Lab1 [FPS:{Convert.ToInt64(1 / args.Time)}][{scene.GetSceneDescription()}][{RenderManager.Instance().Texturecalls}][{RenderManager.Instance().Drawcalls}]";
                time = 0;
            }
            SwapBuffers();
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
            if (input.IsKeyPressed(Keys.KeyPad1))
            {
                index++;
                if(index >= _shader.Count)
                {
                    index = 0;
                }
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

            //GL.DeleteProgram(_shader.Handle);
            
            //_shader.Dispose();

            base.OnUnload();
        }
        private void DrawBox()
        {
            int VAO_Index = 0;
            int VBO_Index = 0;
            int VertexCount = 0;
            bool Init = true;
            if (Init)
            {
                Init = false;
                VBO_Index = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO_Index);
                float[] Verteces = {
                    -0.5f, +0.5f,
                    -0.5f, -0.5f,
                    +0.5f, +0.5f,
                    +0.5f, +0.5f,
                    -0.5f, -0.5f,
                    +0.5f, -0.5f
                };
                GL.BufferData(BufferTarget.ArrayBuffer, Verteces.Length * sizeof(float), Verteces, BufferUsageHint.StaticDraw);

                VAO_Index = GL.GenVertexArray();
                GL.BindVertexArray(VAO_Index);

                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO_Index);
                int location = 0;
                GL.VertexAttribPointer(location, 2, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(location);

                GL.BindVertexArray(0);

                VertexCount = 6;
            }
            GL.BindVertexArray(VAO_Index);
            GL.DrawArrays(PrimitiveType.Triangles, 0, VertexCount);
        }
    }
}
