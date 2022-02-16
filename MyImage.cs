using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LectureImage
{
    class MyImage
    {
        string fichier;
        int height = 0; //number of lines
        int width = 0; //number of pixels on a line 
        Pixel[,] image;
        int[] header = new int[54];
        int headerSize = 54;

        public int Height
        {
            get { return height; }
        }

        public int Width
        {
            get { return width; }
        }


        public MyImage(string fichier)
        {
            this.fichier = fichier;
            byte[] myfile = File.ReadAllBytes(fichier);

            Console.WriteLine("taille = " + myfile.Length);

            width = Convertir_Endian_To_Int(myfile, 18);
            height = Convertir_Endian_To_Int(myfile, 22);

            // CREATION HEADER
            for (int i = 0; i < 53; i++)
            {
                header[i] = myfile[i];
            }

            //Console.WriteLine("h = " + height + " w = " + width);

            // CREATION IMAGE
            image = new Pixel[height, width];
            int c = headerSize;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    image[i, j] = new Pixel(myfile[c], myfile[c + 1], myfile[c + 2]);                   
                    c += 3;
                }
            }
        }

        public string toString()
        {
            string str = "";
            str += "width = " + width + "\nheight = " + height;

            str += "\nHEADER\n";

            for (int j = 0; j < headerSize; j++) // display header
            {
                str += header[j] + " ";
            }

            str += "\nIMAGE\n";

            for (int i = 0; i < height; i++) // display image
            {
                for (int j = 0; j < width; j++)
                {
                    str += image[i, j].toString();
                }
                str += "\n";
            }
            return str;
        }

        public void Negative()
        {
            Byte[] result = new byte[headerSize + height * width*3];
            Console.WriteLine("negative = " + result.Length);

            for (int k = 0; k < headerSize; k++)
            {
                result[k] = Convert.ToByte(header[k]);
                Console.WriteLine(result[k]);
            }
            int c = headerSize;
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    result[c] = Convert.ToByte(image[i, j].Rouge);                    
                }
            }
            File.WriteAllBytes("./Images/Sortie.bmp", result);
        }
        
        public int Convertir_Endian_To_Int(byte[] tab, int indice)
        {
            int result = 0;
            for (int i = 0; i < 4; i++)
            {
                result += tab[indice + i] * Convert.ToInt32(Math.Pow(256, i));
            }
            return result;
        }

        /*public void ChangerTailleImage(MyImage fichier)
        {
            Console.WriteLine("De quelle taille voulez vous avoir votre image ?");
            Console.WriteLine("Hauteur ?");
            int h = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Longueur ?");
            int l = Convert.ToInt32(Console.ReadLine());
        }
        */
        public byte[] Convertir_Int_To_Endian(int entier)
        {
            byte[] result = BitConverter.GetBytes(entier);
            return result;
        }
        public int[] ModifierHeader(int hauteur, int largeur)
        {
            byte[] hauteur1 = Convertir_Int_To_Endian(hauteur);
            //Il manque la taille du fichier à modifier
            byte[] largeur1 = Convertir_Int_To_Endian(largeur);
            for (int i = 0; i<4; i++)
            {
                header[i + 4] = hauteur1[i];
                header[i + 8] = largeur1[i];
            }
            return header;
        }
       /* public int[] ModifierImage(int hauteur, int largeur)
        {

        }
       */
    }
}
