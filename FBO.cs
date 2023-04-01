using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace GraphicApllication
{
    internal class FBO
    {
        private int _fboIndex;
        private int _colorTexture;
        private int _depthTexture;
        private int _width;
        private int _height;
        private int _samples;
        public void Init(int width, int height, bool multiSamples = false)
        {
            _fboIndex = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fboIndex);
            if (multiSamples)
            {
                _colorTexture = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2DMultisample, _colorTexture);
                GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, 8, PixelInternalFormat.Rgba8, 800, 600, false);

                _depthTexture = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2DMultisample, _depthTexture);
                GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, 8, PixelInternalFormat.DepthComponent24, 800, 600, false);

                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2DMultisample, _colorTexture, 0);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2DMultisample, _colorTexture, 0);
            }
            else
            {
                _colorTexture = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2DMultisample, _colorTexture);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, 800, 600, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)0);

                _depthTexture = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2DMultisample, _depthTexture);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent24, 800, 600, 0, PixelFormat.DepthComponent, PixelType.Float, (IntPtr)0);

                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _colorTexture, 0);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, _colorTexture, 0);
            }

            FramebufferErrorCode status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if(status != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("FBO creation failed");
            }
        }
        public void bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fboIndex);
            GL.Viewport(0, 0, 800, 600);
        }
        public void unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, 800, 600);
        }
        void bindColorTexture(TextureUnit textureUnit = TextureUnit.Texture0)
        {

        }
        void bindDepthTexture(TextureUnit textureUnit = TextureUnit.Texture1)
        {

        }
    }
}
