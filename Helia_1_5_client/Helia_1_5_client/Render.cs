using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Helia_tcp_contract;

namespace Helia_1_5_client
{
    class Render
    {
        float ortoX = 0;
        float ortoY = 0;

        public static List<dPlanet> planets = new List<dPlanet>();
        public static List<Player> players = new List<Player>();

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
            
            if (w < h) { GL.Ortho(0, ortoX, ortoY / aspect, 0, -1, 1); }
            else { GL.Ortho(0, ortoX * aspect, ortoY, 0, -1, 1); }
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

                for(int j=0; j<planets[i].sectors.Length; j++)
                {
                    planets[i].sectors[j].ownerOverlay.draw();
                    planets[i].sectors[j].nature.draw();
                    planets[i].sectors[j].building.draw();
                }
            }
        }
    }
}
