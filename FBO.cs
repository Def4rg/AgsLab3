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
        public int IndexFBO
        {
            get => _fboIndex;
        }
        public int ColorTexture
        {
            get => _colorTexture;
        }
        public void Init(int width, int height, bool multiSamples)
        {
            _width = width;
            _height = height;
            _fboIndex = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fboIndex);

            _colorTexture = GL.GenTexture();
            _depthTexture = GL.GenTexture();
            if (!multiSamples)
            {
                GL.BindTexture(TextureTarget.Texture2D, _colorTexture);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, _width, _height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)null);

                GL.BindTexture(TextureTarget.Texture2D, _depthTexture);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent24, _width, _height, 0, PixelFormat.DepthComponent, PixelType.Float, (IntPtr)null);

                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _colorTexture, 0);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, _depthTexture, 0);
            }
            else
            {
                GL.GetInteger(GetPName.MaxColorTextureSamples, out _samples);

                GL.BindTexture(TextureTarget.Texture2DMultisample, _colorTexture);
                GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, _samples, PixelInternalFormat.Rgb8, _width, _height, true);

                GL.BindTexture(TextureTarget.Texture2DMultisample, _depthTexture);
                GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, _samples, PixelInternalFormat.DepthComponent24, _width, _height, true);

                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2DMultisample, _colorTexture, 0);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2DMultisample, _depthTexture, 0);
            }

            FramebufferErrorCode status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if(status != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("FBO creation failed");
            }
            Console.WriteLine(status);
            //GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        public void bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fboIndex);
            GL.Viewport(0, 0, _width, _height);
        }
        public void unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, _width, _height);
        }
        public void bindColorTexture(TextureUnit textureUnit = TextureUnit.Texture0)
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, _colorTexture);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
        public void bindDepthTexture(TextureUnit textureUnit = TextureUnit.Texture1)
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, _depthTexture);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
        public void ResolveToFBO(FBO fbo)
        {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _fboIndex);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, fbo._fboIndex);
            GL.BlitFramebuffer(0, 0, _width, _height, 0, 0, fbo._width, fbo._height, ClearBufferMask.ColorBufferBit /*| ClearBufferMask.DepthBufferBit*/, BlitFramebufferFilter.Nearest);
        }
    }
}
