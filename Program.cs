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
            MyImage img2 = new MyImage("./images/Test.bmp");
            Console.WriteLine(img.toString());
            img.CacherUneImage(img2);
            /*img.Fractale(1000,1000,100);


            //int[] pixels = { 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1 };
            //img.Graph();
            //byte[] tab = new byte[] { 97, 5, 0, 0 };
            //Console.WriteLine(img.Convertir_Endian_To_Int(tab, 0));
            //img.QRCode("HELLO WORLD");
            //img.Test();
            //img.Agrandissement(8);

            int[,] flou = { { 1, 1, 1}, { 1, 1, 1 }, { 1, 1, 1}, };
            int[,] bords = { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 }, };
            int[,] contraste = { { 0, -1, 0}, {-1, 5, -1 } , { 0, -1, 0 } };

            int[,] flou2 = { { 1/9, 1 / 9, 1 / 9 }, { 1 / 9, 1 / 9, 1 / 9 }, { 1 / 9, 1 / 9, 1 / 9 } };
            int[,] décalage_une_ligne_pixel_bas = { { 0, 1, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
            int[,] repoussage = { { -2, -1, 0}, { -1, 1, 1}, { 0, 1, 2 } };
            float[,] ker = { { 1/16, 1/8, 1/16 }, { 1 / 16, 1 / 4, 1 / 16 }, { 1 / 16, 1 / 8, 1 / 16 } };


            //img.MatriceDeConvultion(repoussage);

            //img.MatriceDeConvultion(flou);*/


            MyImage output = new MyImage("./images/sortie.bmp");

            Console.WriteLine(output.toString());
            //output.Agrandissement(8);
            //Console.WriteLine(output.toString());


            Console.ReadLine();
        }


        static void AfficherListe(string[] liste)
        {
            for (int i = 0; i < liste.Length; i++)
            {
                Console.Write(liste[i] + " ");
            }
        }

    }
}
