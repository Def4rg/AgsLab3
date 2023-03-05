using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GraphicApllication.Shaders
{
    public class Shader
    {
        public  int Handle;
        private int VertexShader;
        private int FragmentShader;

        public void setUniform(string name, int value)
        {
            int uniformLocation = GL.GetUniformLocation(Handle, name);
            GL.Uniform1(uniformLocation, value);
        }
        public void setUniform(string name, float value)
        {
            int uniformLocation = GL.GetUniformLocation(Handle, name);
            GL.Uniform1(uniformLocation, value);
        }
        public void setUniform(string name, Vector2 value)
        {
            int uniformLocation = GL.GetUniformLocation(Handle, name);
            GL.Uniform2(uniformLocation, value.X, value.Y);
        }
        public void setUniform(string name, Vector4 value)
        {
            int uniformLocation = GL.GetUniformLocation(Handle, name);
            GL.Uniform4(uniformLocation, value.W, value.X, value.Y, value.Z);
        }
        public void setUniform(string name, Matrix4 value)
        {
            int uniformLocation = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(uniformLocation, true, ref value);
        }

        private bool disposedValue = false;

        public Shader(string vertexPath, string fragmentPath)
        {
            string VertexShaderSource = File.ReadAllText(vertexPath);
            string FragmentShaderSource = File.ReadAllText(fragmentPath);

            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);

            GL.CompileShader(VertexShader);
            GL.CompileShader(FragmentShader);

            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infolog = GL.GetShaderInfoLog(FragmentShader);
                Console.WriteLine(infolog);
            }

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out success);
            if (success == 0)
            {
                string infolog = GL.GetProgramInfoLog(Handle);
                Console.WriteLine(infolog);
            }
        }
        public void Use()
        {
            GL.UseProgram(Handle);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);
            }
        }
        ~Shader()
        {
            GL.DeleteProgram(Handle);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
