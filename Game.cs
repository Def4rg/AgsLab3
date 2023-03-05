using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using GraphicApllication.Shaders;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;


namespace GraphicApllication
{
    public class Game : GameWindow
    {

        private Camera _camera = new Camera(new Vector3(0.0f, 1.0f, 5.0f), 4.0f / 3.0f);
        private Shader shader;
        private List<GraphicObject> GraphicObjects = new ();

        private Vector2 _lastPos;
        private bool _firstMove = true;

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

            shader = new Shader("Shaders/Example.vsh", "Shaders/Example.fsh");
            int n = 10;
            for (int i = 0; i < n; i++)
            {
                GraphicObjects.Add(new GraphicObject(
                    new Vector3(
                        5 * (float)MathHelper.Cos(MathHelper.DegreesToRadians(n / (i + 1))),
                        0, 
                        5 * (float)MathHelper.Sin(MathHelper.DegreesToRadians(n / (i + 1)))
                    )
                    )
                );
            }
            shader.Use();
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            base.Title = $"Lab1 FPS:{Convert.ToInt64(1 / args.Time)}";
            GL.Clear(ClearBufferMask.ColorBufferBit);
            shader.Use();
            Matrix4 modelMatrix = new Matrix4(
                new Vector4(1.0f, 0.0f, 0.0f, 0.0f),
                new Vector4(0.0f, 1.0f, 0.0f, 0.0f),
                new Vector4(0.0f, 0.0f, 1.0f, 0.0f),
                new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
                );
            shader.setUniform("color", new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            shader.setUniform("modelMatrix", modelMatrix);
            shader.setUniform("viewMatrix", _camera.GetViewMatrix());
            shader.setUniform("projectionMatrix", _camera.GetProjectionMatrix());

            foreach (GraphicObject elem in GraphicObjects)
            {
                elem.Draw();
            }
            SwapBuffers();
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            const float cameraSpeed = 1.5f;
            const float sensitivity = 0.2f;


            KeyboardState input = KeyboardState;
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

            var mouse = MouseState;

            if (_firstMove) // This bool variable is initially set to true.
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

            GL.DeleteProgram(shader.Handle);
            
            shader.Dispose();

            base.OnUnload();
        }
    }
}
