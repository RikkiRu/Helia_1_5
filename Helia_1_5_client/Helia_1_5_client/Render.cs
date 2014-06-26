using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace Helia_1_5_client
{
    struct vertexStruct
    {
        public float x;
        public float y;
    }
    struct textureStruct
    {
        public float u;
        public float v;
    }

    class model
    {
        static int sizeOfVertexStruct = sizeof(float) * 2;
        static int sizeOfTextureStruct = sizeof(float) * 2;

        static Dictionary<string, Bitmap> bitmaps = new Dictionary<string,Bitmap>();
        public int texture;

        public vertexStruct[] vertexes = new vertexStruct[4];
        public static textureStruct[] textures=new textureStruct[4];

        public float sizeX;
        public float sizeY;

        public model(float sizeX, float sizeY, string texture)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;

            if (!bitmaps.ContainsKey(texture))
            {
                bitmaps.Add(texture, new Bitmap(Image.FromFile(@"Data/Textures/" + texture + ".png")));
            }

            GL.GenTextures(1, out this.texture);
            GL.BindTexture(TextureTarget.Texture2D, this.texture);
            BitmapData data = bitmaps[texture].LockBits(new Rectangle(0, 0, bitmaps[texture].Width, bitmaps[texture].Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)SgisTextureEdgeClamp.ClampToEdgeSgis); // при фильтрации игнорируются тексели, выходящие за границу текстуры для s координаты
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)SgisTextureEdgeClamp.ClampToEdgeSgis); // при фильтрации игнорируются тексели, выходящие за границу текстуры для t координаты
        
            bitmaps[texture].UnlockBits(data);

            sizeX = sizeX / 2;
            sizeY = sizeY / 2;

            vertexes[0].x = -sizeX;
            vertexes[0].y = -sizeY;
            vertexes[1].x = sizeX;
            vertexes[1].y = -sizeY;
            vertexes[2].x = sizeX;
            vertexes[2].y = sizeY;
            vertexes[3].x = -sizeX;
            vertexes[3].y = sizeY;
        }

        public void draw()
        {
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.VertexPointer(2, VertexPointerType.Float, sizeOfVertexStruct, vertexes);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, sizeOfTextureStruct, textures);
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);
        }
    }

    class Render
    {
        float ortoX = 0;
        float ortoY = 0;

        public Render()
        {
            model.textures[0].u = 0; model.textures[1].u = 1; model.textures[2].u = 1; model.textures[3].u = 0;
            model.textures[0].v = 0; model.textures[1].v = 0; model.textures[2].v = 1; model.textures[3].v = 1;

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
          }

        public void Resize(int w, int h)
        {
            ortoX = 100;
            ortoY = 100;
            if (h == 0) h = 1;
            float aspect = (float)w / (float)h;
            GL.Viewport(0, 0, w, h);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            if (w < h) { GL.Ortho(0, ortoX, ortoY / aspect, 0, -2, 2); ortoY = ortoY / aspect; }
            else { GL.Ortho(0, ortoX * aspect, ortoY, 0, -2, 2); ortoX = ortoX * aspect; }
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        model tree = new model(100, 100, "n_for");

        public void main()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadIdentity();

            tree.draw();
        }
    }
}
