// This code was written for the OpenTK library and has been released
// to the Public Domain.
// It is provided "as is" without express or implied warranty of any kind.

using System;
using System.Drawing;
using System.Windows.Forms;

using OpenTK.Graphics.OpenGL;
using Helia_tcp_contract;
using System.Threading;

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
        public static string username="user1";

        private void FormGame_Load(object sender, EventArgs e)
        {
            render = new Render();
            connect = new ConnectionClient();
            connect.connect();
            connect.send(new CommandServer(typeOfCommandServer.getAll, username));
            render.Resize(glControl1.Width, glControl1.Height);
            timer1.Enabled = true;
            //connect.send(new CommandServer(typeOfCommandServer.ping, 0));
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            try
            {
                render.Resize(glControl1.Width, glControl1.Height);
            }
            catch { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            render.main();
            glControl1.SwapBuffers();
        }

        private void FormGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            connect.send(new CommandServer(typeOfCommandServer.KillMe, username));
            connect.disconnect();
        }
    }
}
