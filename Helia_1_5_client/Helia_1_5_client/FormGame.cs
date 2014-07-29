// This code was written for the OpenTK library and has been released
// to the Public Domain.
// It is provided "as is" without express or implied warranty of any kind.

using System;
using System.Drawing;
using System.Windows.Forms;

using OpenTK.Graphics.OpenGL;
using Helia_tcp_contract;
using System.Threading;
using System.Drawing.Imaging;
using OpenTK;

namespace Helia_1_5_client
{

    public partial class FormGame : Form
    {

        public FormGame()
        {
            InitializeComponent();
        }

        Render render;
        ConnectionClient connect;
        public static string username="user2";

        bool keyPw;
        bool keyPs;
        bool keyPa;
        bool keyPd;

        private void FormGame_Load(object sender, EventArgs e)
        {
            Render.parent = this;

            render = new Render();
            render.Resize(glControl1.Width, glControl1.Height);

            connect = new ConnectionClient();
            connect.connect();
            connect.send(new CommandServer(typeOfCommandServer.getAll, username));

            this.timer1.Enabled = true;
            Application.Idle += Application_Idle;
        }

        public void startTimer()
        {
            foreach(var a in Render.planets)
            {
                Invoke(new Action(()=> a.giveMeName()));
            }

        }

        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                mouseLoop();
                Thread.Sleep(5);
            }
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            try
            {
                render.Resize(glControl1.Width, glControl1.Height);
            }
            catch { }
        }

        private void mouseLoop()
        {
            if (keyPa) playerView.x += playerView.speedX;
            if (keyPd) playerView.x -= playerView.speedX;
            if (keyPw) playerView.y += playerView.speedY;
            if (keyPs) playerView.y -= playerView.speedY;

            render.main();
            glControl1.SwapBuffers();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            playerView.setGuiInfo();
        }

        private void FormGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            connect.send(new CommandServer(typeOfCommandServer.KillMe, username));
            connect.disconnect();
        }


        public int getTexture(Bitmap bm)
        {
            int x=0;
            Invoke(new Action(()=>GL.GenTextures(1, out x)));
            Invoke(new Action(()=>GL.BindTexture(TextureTarget.Texture2D, x)));
            Invoke(new Action(()=>GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear)));
            Invoke(new Action(()=>GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear)));
            Invoke(new Action(()=>GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)SgisTextureEdgeClamp.ClampToEdgeSgis)));
            Invoke(new Action(()=>GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)SgisTextureEdgeClamp.ClampToEdgeSgis)));
            BitmapData data = null;
            Invoke(new Action(()=> data = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb)));
            Invoke(new Action(()=>GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0)));
            Invoke(new Action(()=> bm.UnlockBits(data)));
            return x;
        }

        public void drawTexture(int tId, float x, float y, float angle, vertexStruct[] vertexes)
        {
            Invoke(new Action(()=>GL.Color3(Color.White)));
            Invoke(new Action(()=>GL.PushMatrix()));
            Invoke(new Action(()=>GL.Translate(x, y, 0)));
            Invoke(new Action(()=>GL.Rotate(angle, Vector3.UnitZ)));
            Invoke(new Action(()=>GL.BindTexture(TextureTarget.Texture2D, tId)));
            Invoke(new Action(()=>GL.VertexPointer(2, VertexPointerType.Float, objDrawer.sizeOfVertexStruct, vertexes)));
            Invoke(new Action(()=>GL.TexCoordPointer(2, TexCoordPointerType.Float, objDrawer.sizeOfTextureStruct, objDrawer.textStatic)));
            Invoke(new Action(()=>GL.DrawArrays(PrimitiveType.Quads, 0, 4)));
            Invoke(new Action(()=>GL.PopMatrix()));
        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W: keyPw = true;
                    break;
                case Keys.S: keyPs = true;
                    break;
                case Keys.A: keyPa = true;
                    break;
                case Keys.D: keyPd = true;
                    break;

                case Keys.Q:
                    playerView.scale = playerView.scale * playerView.scaleSpeed;
                    playerView.speedX = playerView.speedX / playerView.scaleSpeed;
                    playerView.speedY = playerView.speedY / playerView.scaleSpeed;
                    break;

                case Keys.E:
                    playerView.scale = playerView.scale / playerView.scaleSpeed;
                    playerView.speedX = playerView.speedX * playerView.scaleSpeed;
                    playerView.speedY = playerView.speedY * playerView.scaleSpeed;
                    break;

                case Keys.Enter:
                    break;
            }
        }

        private void glControl1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W: keyPw = false;
                    break;
                case Keys.S: keyPs = false;
                    break;
                case Keys.A: keyPa = false;
                    break;
                case Keys.D: keyPd = false;
                    break;
            }
        }

        private void glControl1_MouseClick(object sender, MouseEventArgs e)
        {
            float realX = e.X;
            float realY = e.Y;

            realX += Render.ortoX / playerView.scale;

            realX = realX * (Render.ortoX) / (glControl1.Width) - playerView.x;
            realY = realY * (Render.ortoY) / (glControl1.Height) - playerView.y;
            //realX = realX / (playerView.scale * 2);
            //realY = realY / (playerView.scale * 2);

            this.Text = string.Format("x:{0} y:{1}", realX, realY);

            // Рисуем выбор планеты
            foreach (var a in Render.planets)
            {
                float rad = a.radius;
                if (realX > a.ground.x - rad && realX < a.ground.x + rad && realY > a.ground.y - rad && realY < a.ground.y + rad)
                {
                    a.ground.colorMask = Color.Red;
                }
            }

            objDrawer od = new objDrawer(10, 10, 8);
            od.x = realX;
            od.y = realY;
            Render.objects.Add(od);
        }
    }
}
