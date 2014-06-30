using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helia_tcp_contract;
using System.Drawing;

namespace Helia_1_5_client
{
    class playerView
    {
        public static float x;
        public static float y;

        public static float speedX = 1;
        public static float speedY = 1;

        public static Planet_nature currentNature;
        public static Planet_government currentGov;

        public static int countIntGuiText = 15;
        public static float guiSizeElements = 5;
        public static float scaleSpeed = 1.2f;

        public static float scale = 1.0f;

        public static void setGuiInfo()
        {
            if(currentNature==null) Render.guiElementsText[0].changeText(Render.changeText("Космос", countIntGuiText, ContentAlignment.TopRight), Color.White, Color.Black);
            else
            {
                Render.guiElementsText[0].changeText(Render.changeText(currentNature.name, countIntGuiText, ContentAlignment.TopRight), Color.White, Color.Black);
            }
        }
    }
}
