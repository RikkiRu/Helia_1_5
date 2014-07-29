using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Helia_tcp_contract;
using System.Text;

namespace Helia_1_5_client
{
    class Render
    {
        public static FormGame parent;

        public static float ortoX = 0;
        public static float ortoY = 0;
        public static float halfOrtoX = 0;
        public static float halfOrtoY = 0;

        public static List<dPlanet> planets = new List<dPlanet>();
        public static List<Player> players = new List<Player>();
        public static List<objDrawer> objects = new List<objDrawer>();

        public static objDrawer[] guiElemnts = new objDrawer[10];
        public static textDrawer[] guiElementsText = new textDrawer[10];

        public static textDrawer guiTextHeader;
        public static textDrawer guiTextDown;
        public static objDrawer drawSelect = new objDrawer(0, 0, 18); //Отрисовка выбора планеты

        public static string changeText(string text, int sizeOut=100, ContentAlignment aligin = ContentAlignment.TopLeft)
        {
            char[] textBuffer = new char[sizeOut];

            int size = text.Length;
            int correction = 0;
            

            if (aligin == ContentAlignment.TopRight) correction = sizeOut - size;

            for (int i = 0; i < textBuffer.Length; i++ )
            {
                if (i < size + correction && i >= correction)
                {
                    textBuffer[i] = text[i - correction];
                }
                else textBuffer[i] = ' ';
            }

            string res = new string(textBuffer);
            return res;
        }


        public static void writeBottom(string text)
        {
            guiTextDown.changeText(changeText(text), Color.White, Color.Black);
        }
        public static void writeTop(string text)
        {
            guiTextHeader.changeText(changeText(text), Color.Black, Color.Transparent);
        }
        public static void loadGUIelements()
        {
            for(int i=0; i<10; i++)
            {
                guiElemnts[i] = new objDrawer(playerView.guiSizeElements, playerView.guiSizeElements, i + 5); //5-14 текстуры... магочисла...
                guiElementsText[i] = new textDrawer(20, changeText("", playerView.countIntGuiText, ContentAlignment.TopRight), Color.Black, Color.Transparent, ContentAlignment.MiddleRight);
            }
        }

        public Render()
        {
            planets.Clear();
            players.Clear();
            objects.Clear();

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            objDrawer.loadTextures();
            loadGUIelements();
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
            
            if (w < h) { ortoY = ortoY / aspect; }
            else { ortoX = ortoX * aspect; }

            GL.Ortho(0, ortoX, ortoY, 0, -1, 1);

            halfOrtoX = ortoX / 2;
            halfOrtoY = ortoY / 2;

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

           
            guiTextHeader = new textDrawer(ortoX, changeText("Добро пожаловать в игру, "+FormGame.username+"!"), Color.Black, Color.Transparent, ContentAlignment.TopLeft);
            guiTextDown = new textDrawer(ortoX, changeText("Оповещения"), Color.White, Color.Black, ContentAlignment.BottomLeft);
            guiTextHeader.x = guiTextDown.x = 0;
            guiTextHeader.y = 0;
            guiTextDown.y = ortoY;

            for(int i=0; i<guiElemnts.Length; i++)
            {
                guiElemnts[i].x = ortoX - playerView.guiSizeElements;
                guiElemnts[i].y = i * playerView.guiSizeElements+playerView.guiSizeElements;

                guiElementsText[i].x = ortoX - playerView.guiSizeElements*2;
                guiElementsText[i].y = i * playerView.guiSizeElements + playerView.guiSizeElements;
            }
        }


        public void main()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadIdentity();

            GL.PushMatrix();

            GL.Translate(halfOrtoX, halfOrtoY, 0);
            GL.Scale(playerView.scale, playerView.scale, 1);
            GL.Translate(-halfOrtoX, -halfOrtoY, 0);

            GL.Translate(playerView.x, playerView.y, 0);


            for (int i = 0; i < planets.Count; i++)
            {
                planets[i].ground.draw();

                if (planets[i].sectors != null && planets[i].sectors.Length > 0)
                {
                    for (int j = 0; j < planets[i].sectors.Length; j++)
                    {
                        if (planets[i].sectors[j] != null)
                        {
                            planets[i].sectors[j].ownerOverlay.draw();
                            planets[i].sectors[j].nature.draw();
                            planets[i].sectors[j].building.draw();
                            if(planets[i].nameOfPlanet!=null) planets[i].nameOfPlanet.draw();
                        }
                    }
                }
            }

            for (int i = 0; i < objects.Count; i++ )
            {
                objects[i].draw();
            }


            GL.PopMatrix();

            for (int i = 0; i < guiElemnts.Length; i++ )
            {
                guiElemnts[i].draw();
                guiElementsText[i].draw();
            }

            guiTextHeader.draw();
            guiTextDown.draw();

        
        }
    }
}
