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
            List<byte> head = new List<byte>(headerSize);
            List<Pixel> img = new List<Pixel>(height * width);

            for (int k = 0; k < headerSize; k++)
            {
                head.Add(Convert.ToByte(header[k]));
            }

            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    img.Add(image[i, j].Negatif());
                }
            }
            List<byte> result = head.Concat(PixelToByte(img)).ToList();
            File.WriteAllBytes("./Images/Sortie.bmp", result.ToArray());
        }
        
  
        public List<byte> PixelToByte(List<Pixel> tab)
        {
            List<Byte> result = new List<byte> (tab.Count*3);
            for(int i = 0; i < tab.Count; i++)
            {
                result.Add(Convert.ToByte(tab[i].Rouge));
                result.Add(Convert.ToByte(tab[i].Vert));
                result.Add(Convert.ToByte(tab[i].Bleu));
            }
            return result.ToList();
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

        public byte[] Convertir_Int_To_Endian(int val)
        {
            byte[] result = new byte[4];
            return result;
        }


    }
}
