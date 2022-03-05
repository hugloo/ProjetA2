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
            MyImage img = new MyImage("./images/Test.bmp");
            img.ChangerTailleImage(2);
            MyImage output = new MyImage("./images/sortie.bmp");

            //Console.WriteLine(img.toString());
            /*
            <<<<<<< HEAD
                        int[,] flou = { { 0, 0, 0, 0, 0 }, { 0, 1, 1, 1, 0 }, { 0, 1, 1, 1, 0 }, { 0, 1, 1, 1, 0 }, { 0, 0, 0, 0, 0 } };
                        int[,] i = { { 1/9, 1 / 9, 1 / 9 }, { 1 / 9, 1 / 9, 1 / 9 }, { 1 / 9, 1 / 9, 1 / 9 } };
                        int[,] t = { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } , { 0, 0, 0, 0, 0 } };
                        int[,] test = { { 100, 100, 100, 100, 100 }, { 100, 100, 100, 100, 100 }, { 100, 100, 150, 100, 100 }, { 100, 100, 100, 100, 100 }, { 100, 100, 100, 100, 100 } };
                        int[,] iy = { { -1, -1, -1}, { -1, 8, -1}, { -1, -1, -1 } };


                        img.MatriceDeConvultion(flou);
            =======
                        img.Agrandir_Image(2);
            >>>>>>> 23b73272d14345d872e6aad87dd2e10809224e11

                        MyImage output = new MyImage("./images/sortie.bmp");

                        Console.WriteLine(output.toString()); */

            /*
            byte[] myfile = File.ReadAllBytes("./Images/sortie.bmp");
            //myfile est un vecteur composé d'octets représentant les métadonnées et les données de l'image

            //Métadonnées du fichier
            Console.WriteLine("\n Header \n");
            for (int i = 0; i < 14; i++)
                Console.Write(myfile[i] + " ");
            //Métadonnées de l'image
            Console.WriteLine("\n HEADER INFO \n");
            for (int i = 14; i < 54; i++)
                Console.Write(myfile[i] + " ");
            //L'image elle-même
            Console.WriteLine("\n IMAGE \n");
            for (int i = 54; i < myfile.Length; i = i + 60)
            {
                for (int j = i; j < i + 60; j++)
                {
                    Console.Write(myfile[j] + " ");
                }
                Console.WriteLine();
            }
            */

            Console.ReadLine();
        }


    }
}
