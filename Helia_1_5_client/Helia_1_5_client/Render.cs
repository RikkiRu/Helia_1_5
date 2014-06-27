using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace Helia_1_5_client
{
    class Render
    {
        float ortoX = 0;
        float ortoY = 0;

        public static List<dPlanet> planets=new List<dPlanet>();

        public Render()
        {
            planets.Clear();

            objDrawer.loadTextures();

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


        public void main()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadIdentity();

            for(int i=0; i<planets.Count; i++)
            {
                planets[i].ground.draw();
            }
        }
    }
}
