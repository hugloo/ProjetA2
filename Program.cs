using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace LectureImage
{
    class Program
    {

        static void Main(string[] args)
        {
            MyImage img = new MyImage("./images/coco.bmp");
            Console.WriteLine(img.toString());

            int[] pixels = { 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1 };
            img.resizePixels(400, 403);




            int[,] flou = { { 1, 1, 1}, { 1, 1, 1 }, { 1, 1, 1}, };
            int[,] bords = { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 }, };
            int[,] contraste = { { 0, -1, 0}, {-1, 5, -1 } , { 0, -1, 0 } };

            int[,] flou2 = { { 1/9, 1 / 9, 1 / 9 }, { 1 / 9, 1 / 9, 1 / 9 }, { 1 / 9, 1 / 9, 1 / 9 } };
            int[,] décalage_une_ligne_pixel_bas = { { 0, 1, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
            int[,] repoussage = { { -2, -1, 0}, { -1, 1, 1}, { 0, 1, 2 } };
            float[,] ker = { { 1/16, 1/8, 1/16 }, { 1 / 16, 1 / 4, 1 / 16 }, { 1 / 16, 1 / 8, 1 / 16 } };

            //img.MatriceDeConvultion(repoussage);

            MyImage output = new MyImage("./images/sortie.bmp");

            //Console.WriteLine(output.toString());

            Console.ReadLine();
        }


    }
}
