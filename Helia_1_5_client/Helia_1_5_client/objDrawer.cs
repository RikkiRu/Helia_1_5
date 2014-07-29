using Helia_tcp_contract;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Helia_1_5_client
{
    /// <summary>
    /// Служебный. Хранит данные.
    /// </summary>
    class modelData
    {
        public Bitmap[] Bitmaps = new Bitmap[1];
        public int[] Textures=new int[1];
        public string name;
        public int currentTexture = 0;
        public int ticks = 0;
        public int ticksNow = 0;

        public int texture
        {
            set
            {
                Textures[0] = value;
            }

            get
            {
                if (Textures.Length!=1)
                {
                    ticksNow++;
                    if (ticksNow > ticks)
                    {
                        ticksNow = 0;
                        currentTexture++;
                        if (currentTexture >= Textures.Length) currentTexture = 0;
                    }
                }
                return Textures[currentTexture];
            }
        }
        public Bitmap bm
        {
            get { return Bitmaps[0]; }
            set { Bitmaps[0] = value; }
        }

        public modelData(string name, int namesCount=1, int tiksSkip=0)
        {
            this.name = name;
            this.ticks = tiksSkip;

            if(namesCount>1)
            {
                Textures = new int[namesCount];
                Bitmaps = new Bitmap[namesCount];

                for(int i=0; i<namesCount; i++)
                {
                    Bitmaps[i] = new Bitmap(Image.FromFile(@"Data/Textures/" + name + @"/" +i.ToString() + ".png"));
                    Textures[i] = Render.parent.getTexture(Bitmaps[i]);
                }
            }

            else
            {
                this.bm = new Bitmap(Image.FromFile(@"Data/Textures/" + name + ".png"));
            }

            mainGen();
        }

        public modelData(Bitmap bm)
        {
            this.name = "";
            this.bm = bm;
            mainGen();
        }

        void mainGen()
        {
            //GL.GenTextures(1, out this.texture);
            this.texture = Render.parent.getTexture(this.bm);
            //Вы знаете сколько я мучался?! А оказывается openGL дергался из другого потока и не так генерил текстурки!!!
        }

        ~modelData()
        {
            try
            {
              if (OpenTK.Graphics.GraphicsContext.CurrentContext != null) GL.DeleteTextures(Textures.Length, Textures);
            }
            catch { }
        }
    }

    public struct vertexStruct
    {
        public float x;
        public float y;
    }
    public struct textureStruct
    {
        public float u;
        public float v;
    }

    /// <summary>
    /// Загрузка текстур тут - основной класс отрисовки
    /// </summary>
    class objDrawer
    {
        public static int sizeOfVertexStruct = sizeof(float) * 2;
        public static int sizeOfTextureStruct = sizeof(float) * 2;

        public static modelData[] texData; //хранит все текстуры
        public static textureStruct[] textStatic = new textureStruct[4];
        
        public vertexStruct[] vertexes = new vertexStruct[4];
        public float x;
        public float y;
        public float angle;
        public int texID;
        public Color colorMask = Color.White;

        public objDrawer(float sizeX, float sizeY, int texID)
        {
            this.texID = texID;

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
            GL.Color3(colorMask);
            GL.PushMatrix();
            GL.Translate(x, y, 0);
            GL.Rotate(angle, Vector3.UnitZ);
            GL.BindTexture(TextureTarget.Texture2D, texData[texID].texture);
            GL.VertexPointer(2, VertexPointerType.Float, sizeOfVertexStruct, vertexes);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, sizeOfTextureStruct, textStatic);
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);
            GL.PopMatrix();
        }

        public static void loadTextures()
        {
            textStatic[0].u = 0; textStatic[1].u = 1; textStatic[2].u = 1; textStatic[3].u = 0;
            textStatic[0].v = 0; textStatic[1].v = 0; textStatic[2].v = 1; textStatic[3].v = 1;

            texData = new modelData[30];
            texData[0] = new modelData("n_for");
            texData[1] = new modelData("pFlat");
            texData[2] = new modelData("n_grass");
            texData[3] = new modelData("none");
            texData[4] = new modelData("pOverlay");

            texData[5] = new modelData("planetIcon");
            texData[6] = new modelData("o2");
            texData[7] = new modelData("h2o");
            texData[8] = new modelData("smile");
            texData[9] = new modelData("man");

            texData[10] = new modelData("apple");
            texData[11] = new modelData("gears");
            texData[12] = new modelData("break");
            texData[13] = new modelData("book");
            texData[14] = new modelData("waterKaplya");

            texData[15] = new modelData("sun", 3, 20);
            texData[16] = new modelData("onePixel");
            texData[17] = new modelData("newPlayer");
            texData[18] = new modelData("select");
        }
    }

    class textDrawer
    {
        public modelData texData;
        public vertexStruct[] vertexes = new vertexStruct[4];
        public float x;
        public float y;
        public float angle;

        static PointF pNull = new PointF(0, 0);
        static Font fontM = new Font(FontFamily.GenericSansSerif, 48);

        public void changeText(string text, Color textColor, Color backgoundColor)
        {
            texData = new modelData(getTextBitmap(text, textColor, backgoundColor));
        }

        public Bitmap getTextBitmap (string text, Color fColor, Color back)
        {
            int length = text.Length;
            if (length == 0) length = 1;

            Bitmap res = new Bitmap(length * fontM.Height/2, fontM.Height);
            Graphics gr = Graphics.FromImage(res);
            
            SolidBrush solid = new SolidBrush(fColor);
            gr.Clear(back);

            gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            gr.DrawString(text, fontM, solid, pNull);
            return res;
        }

        public textDrawer(float sizeX, string text, Color textColor, Color backgoundColor, ContentAlignment aligin)
        {
            int length = text.Length;
            if(length==0) length=1;

            float sizeY = sizeX / (length) * 2;
            texData = new modelData(getTextBitmap(text, textColor, backgoundColor));

            switch (aligin)
            {
                default:
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
                    break;

                case ContentAlignment.TopLeft:
                    vertexes[0].x = 0;
                    vertexes[0].y = 0;
                    vertexes[1].x = sizeX;
                    vertexes[1].y = 0;
                    vertexes[2].x = sizeX;
                    vertexes[2].y = sizeY;
                    vertexes[3].x = 0;
                    vertexes[3].y = sizeY;
                    break;

                case ContentAlignment.BottomLeft:
                    vertexes[0].x = 0;
                    vertexes[0].y = -sizeY;
                    vertexes[1].x = sizeX;
                    vertexes[1].y = -sizeY;
                    vertexes[2].x = sizeX;
                    vertexes[2].y = 0;
                    vertexes[3].x = 0;
                    vertexes[3].y = 0;
                    break;

                case ContentAlignment.MiddleRight:
                    sizeY = sizeY / 2;
                    vertexes[0].x = -sizeX;
                    vertexes[0].y = -sizeY;
                    vertexes[1].x = 0;
                    vertexes[1].y = -sizeY;
                    vertexes[2].x = 0;
                    vertexes[2].y = sizeY;
                    vertexes[3].x = -sizeX;
                    vertexes[3].y = sizeY;
                    break;
            }
        }

        public void draw()
        {
            GL.Color3(Color.White);
            GL.PushMatrix();
            GL.Translate(x, y, 0);
            GL.Rotate(angle, Vector3.UnitZ);
            GL.BindTexture(TextureTarget.Texture2D, texData.texture);
            GL.VertexPointer(2, VertexPointerType.Float, objDrawer.sizeOfVertexStruct, vertexes);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, objDrawer.sizeOfTextureStruct, objDrawer.textStatic);
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);
            GL.PopMatrix();
        }
    }

    class dPlanet
    {
        public class dSector
        {
            public objDrawer building;
            public objDrawer nature;
            public objDrawer ownerOverlay;

            public dSector(sector x, Planet_nature p, int number, int count)
            {

                int texIDb = 3;
                int texIDn = 3;

                if (x.building != null)
                    switch (x.building.type)
                    {

                    }

                if (x.natureBuilding != null)
                    switch (x.natureBuilding.type)
                    {
                        case buildingsEnum.grass: texIDn = 2; break;
                        case buildingsEnum.forest: texIDn = 0; break;
                    }



                if (p.type != PlanetType.sun)
                {
                    building = new objDrawer(sizes.standartSizeSmall, sizes.standartSizeSmall, texIDb);
                    nature = new objDrawer(sizes.standartSizeSmall, sizes.standartSizeSmall, texIDn);
                    ownerOverlay = new objDrawer(sizes.standartSizeSmall, sizes.standartSizeSmall, 4);
                }
                else
                {
                    building = new objDrawer(0, 0, 16);
                    nature = new objDrawer(0, 0, 16);
                    ownerOverlay = new objDrawer(0, 0, 16);
                }

                building.x = p.x;
                nature.x = p.x;
                building.y = p.y;
                nature.y = p.y;
                ownerOverlay.x = p.x;
                ownerOverlay.y = p.y;

                float secAngle = 360 / count * number;


                building.angle = secAngle;
                nature.angle = secAngle;
                ownerOverlay.angle = secAngle;

                Player owner = Render.players.Where(c => c.name == x.owner).FirstOrDefault();
                if (owner != null)
                {
                    building.colorMask = owner.color;
                    ownerOverlay.colorMask = owner.color;
                }

                secAngle = secAngle / 57.0f;

                float xCorr = (float)Math.Sin((double)secAngle) * p.radius;
                float yCorr = (float)Math.Cos((double)secAngle) * p.radius;

                nature.x += xCorr;
                nature.y -= yCorr;

                building.x += xCorr;
                building.y -= yCorr;

                ownerOverlay.x += xCorr / 2;
                ownerOverlay.y -= yCorr / 2;
            }
        }

        public objDrawer ground;
        public string name;
        public dSector[] sectors;
        public textDrawer nameOfPlanet;
        public float radius;
        public PlanetType type;

        public void giveMeName()
        {
             if (type == PlanetType.sun)
            {
                nameOfPlanet = new textDrawer(radius, name, Color.Black, Color.Transparent, ContentAlignment.TopCenter);
                nameOfPlanet.x = ground.x;
                nameOfPlanet.y = ground.y;
            }

            else
            {
                nameOfPlanet = new textDrawer(radius * 2, name, Color.Blue, Color.Transparent, ContentAlignment.TopCenter);
                nameOfPlanet.x = ground.x;
                nameOfPlanet.y = ground.y + radius + 10;
            }
        }


        public dPlanet(Planet_nature x)
        {
            int texID=-1;

            this.radius = x.radius;
            this.type = x.type;

            switch (x.type)
            {
                case PlanetType.flat: texID = 1; break;
                case PlanetType.sun: texID = 15; break;
            }

            ground = new objDrawer(x.radius * 2, x.radius * 2, texID);
            this.name = x.name;

            ground.x = x.x;
            ground.y = x.y;

            if (x.sectors != null && x.sectors.Length > 0)
            {
                sectors = new dSector[x.sectors.Length];
                for (int i = 0; i < x.sectors.Length; i++)
                {
                    if(x.sectors[i]!=null)
                    sectors[i] = new dSector(x.sectors[i], x, i, x.sectors.Length);
                }
            }
        }
    }

    class dPLayers : Player
    {
        class dUnits
        {

        }
    }

   
}
