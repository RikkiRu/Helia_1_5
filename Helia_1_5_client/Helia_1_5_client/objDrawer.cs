﻿using Helia_tcp_contract;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace Helia_1_5_client
{
    class modelData
    {
        public Bitmap bm;
        public int texture;
        public string name;

        public modelData(string name)
        {
            this.name = name;
            this.bm = new Bitmap(Image.FromFile(@"Data/Textures/" + name + ".png"));

            GL.GenTextures(1, out this.texture);
            GL.BindTexture(TextureTarget.Texture2D, this.texture);

            BitmapData data = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)SgisTextureEdgeClamp.ClampToEdgeSgis); // при фильтрации игнорируются тексели, выходящие за границу текстуры для s координаты
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)SgisTextureEdgeClamp.ClampToEdgeSgis); // при фильтрации игнорируются тексели, выходящие за границу текстуры для t координаты
            bm.UnlockBits(data);
        }
    }

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

    class objDrawer
    {
        static int sizeOfVertexStruct = sizeof(float) * 2;
        static int sizeOfTextureStruct = sizeof(float) * 2;

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

            texData = new modelData[2];
            texData[0] = new modelData("n_for");
            texData[1] = new modelData("pFlat");
        }
    }



    class dPlanet
    {
        public objDrawer ground;
        public string name;

        public dPlanet(Planet_nature x)
        {
            int texID=-1;
            switch (x.type)
            {
                case PlanetType.flat: texID = 1; break; 
            }

            ground = new objDrawer(x.radius * 2, x.radius * 2, texID);
            this.name = x.name;

            ground.x = x.x;
            ground.y = x.y;
        }
    }
}