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
        public void NoirEtBlanc()
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
                    img.Add(image[i, j].NoirBlanc());
                }
            }
            List<byte> result = head.Concat(PixelToByte(img)).ToList();
            File.WriteAllBytes("./Images/Sortie.bmp", result.ToArray());
        }
        public void NuancesDeGris()
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
                    img.Add(image[i, j].Gris());
                }
            }
            List<byte> result = head.Concat(PixelToByte(img)).ToList();
            File.WriteAllBytes("./Images/Sortie.bmp", result.ToArray());
        }
        public void Miroir()
        {
            List<byte> head = new List<byte>(headerSize);
            List<Pixel> img = new List<Pixel>(height * width);

            for (int k = 0; k < headerSize; k++)
            {
                head.Add(Convert.ToByte(header[k]));
            }

            for (int i = 0 ; i < image.GetLength(0); i++)
            {
                for (int j = image.GetLength(1) - 1; j >= 0; j--)
                {
                    img.Add(image[i, j]);
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

        /*public void ChangerTailleImage(MyImage fichier)
        {
            Console.WriteLine("De quelle taille voulez vous avoir votre image ? (en octet)");
            Console.WriteLine("Hauteur ?");
            int h = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Longueur ?");
            int l = Convert.ToInt32(Console.ReadLine());
            ModifierHeader(h, l);
            int vérification = l % 3;
            while(vérification != 0)
            {
                Console.WriteLine("Taille invalide, veuillez donner une longueur multiple de 3");
                l = Convert.ToInt32(Console.ReadLine());
                vérification = l % 3;
            }
            int pixels_rajoutés = 0;
            Pixel[,] pixel2 = null;
            if (vérification > image.GetLength(1))
            {
                pixels_rajoutés = (vérification - image.GetLength(1)) / 2;
                pixel2 = new Pixel[image.GetLength(0), image.GetLength(1) + 2 * pixels_rajoutés];
                for (int i = 0; i < pixel2.GetLength(0); i++)
                {
                    for (int j = 0; j < pixel2.GetLength(1); j++)
                    {
                        if (j <= pixels_rajoutés)
                        {
                            pixel2[i, j] = image[i, 1];
                        }
                        else
                        {
                            if (j >= image.GetLength(1))
                            {
                                pixel2[i, j] = image[i, image.GetLength(1)-1];
                            }
                            else
                            {
                                pixel2[i, j + pixels_rajoutés] = image[i, j];
                            }
                        }
                    }
                }
            }
            else
            {
                if(vérification == image.GetLength(1))
                {
                    pixel2 = new Pixel[image.GetLength(0), image.GetLength(1)];
                    for (int i = 0; i <pixel2.GetLength(0); i++)
                    {
                        for (int j = 0; j < pixel2.GetLength(1); j++)
                        {
                            pixel2[i, j] = image[i, j];
                        }
                    }
                }
                else
                {
                    pixels_rajoutés = (((vérification - image.GetLength(1)) / 2)^2)^(1/2);
                    pixel2 = new Pixel[image.GetLength(0), image.GetLength(1) - 2 * pixels_rajoutés];
                    for (int i = 0; i < pixel2.GetLength(0); i++)
                    {
                        for (int j = 0 ; j < pixel2.GetLength(1); j++)
                        {
                            pixel2[i, j] = image[i, j + pixels_rajoutés]; 
                        }
                    }
                }
            }
            pixels_rajoutés = 0;
            Pixel[,] pixel3 = null;
            if (h > image.GetLength(0))
            {
                pixels_rajoutés = (h - pixel2.GetLength(0)) / 2;
                pixel3 = new Pixel[pixel2.GetLength(0) + 2 * pixels_rajoutés, pixel2.GetLength(1)];
                for (int i = 0; i < pixel3.GetLength(0); i++)
                {
                    for (int j = 0; j < pixel3.GetLength(1); j++)
                    {
                        if (i <= pixels_rajoutés)
                        {
                            pixel3[i, j] = pixel2[0, j];
                        }
                        else
                        {
                            if (i >= pixel2.GetLength(0))
                            {
                                pixel3[i, j] = pixel2[pixel2.GetLength(0) - 1, j];
                            }
                            else
                            {
                                pixel3[i + pixels_rajoutés, j] = pixel2[i, j];
                            }
                        }
                    }
                }
            }
            else
            {
                if (h == pixel2.GetLength(0))
                {
                    pixel3 = new Pixel[pixel2.GetLength(0), pixel2.GetLength(1)];
                    for (int i = 0; i < pixel2.GetLength(0); i++)
                    {
                        for (int j = 0; j < pixel2.GetLength(1); j++)
                        {
                            pixel3[i, j] = pixel2[i, j];
                        }
                    }
                }
                else
                {
                    pixels_rajoutés = (((h - pixel2.GetLength(0)) / 2) ^ 2) ^ (1 / 2);
                    pixel2 = new Pixel[image.GetLength(0), image.GetLength(1) - 2 * pixels_rajoutés];
                    for (int i = 0; i < pixel2.GetLength(0); i++)
                    {
                        for (int j = 0; j < pixel2.GetLength(1); j++)
                        {
                            pixel2[i, j] = image[i + pixels_rajoutés, j];
                        }
                    }
                }
            }
            //rajouter pour convertir cette matrice (pixel3) pour pouvoir la lire comme une image
        }
        */
        public void Agrandir_Image(int val)
        {
            List<byte> head = new List<byte>(headerSize);
            List<Pixel> img = new List<Pixel>(height * width * val * val);
            byte[] head1 = new byte[headerSize];
            for (int k = 0; k < headerSize; k++)
            {
                head.Add(Convert.ToByte(header[k]));
                head1[k] = Convert.ToByte(header[k]);
            }
            int l = Convertir_Endian_To_Int(head1, 4);
            int h = Convertir_Endian_To_Int (head1, 8);
            l = l * val;
            h = h * val;
            byte[] l1 = Convertir_Int_To_Endian(l);
            byte[] h1 = Convertir_Int_To_Endian(h);
            for (int k = 0; k < 4; k++)
            {
                head[k + 4] = l1[k];
                head[k + 8] = h1[k];
            }
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    
                    {
                        img.Add(image[i, j]);
                    }

                }
            }
            List<byte> result = head.Concat(PixelToByte(img)).ToList();
            File.WriteAllBytes("./Images/Sortie.bmp", result.ToArray());
        }
        public void Retrécir_Image(int val)
        {
            if (val % 2 == 0)
            {
                List<byte> head = new List<byte>(headerSize);
                List<Pixel> img = new List<Pixel>(height * width * val * val);
                byte[] head1 = new byte[headerSize];
                for (int k = 0; k < headerSize; k++)
                {
                    head.Add(Convert.ToByte(header[k]));
                    head1[k] = Convert.ToByte(header[k]);
                }
                int l = Convertir_Endian_To_Int(head1, 4);
                int h = Convertir_Endian_To_Int(head1, 8);
                l = l * val;
                h = h * val;
                byte[] l1 = Convertir_Int_To_Endian(l);
                byte[] h1 = Convertir_Int_To_Endian(h);
                for (int k = 0; k < 4; k++)
                {
                    head[k + 4] = l1[k];
                    head[k + 8] = h1[k];
                }
                int compteur_ligne = 0;
                int compteur_colonne = 0;
                for (int i = 0; i < image.GetLength(0); i++)
                {
                    for (int j = 0; j < image.GetLength(1); j++)
                    {
                        if(i + compteur_ligne < image.GetLength(0) && j + compteur_colonne < image.GetLength(1))
                        {
                            img.Add(image[i + compteur_ligne, j + compteur_colonne]);
                            compteur_colonne += val;
                        }
                    }
                    compteur_colonne = 0;
                    compteur_ligne += val;
                }
                List<byte> result = head.Concat(PixelToByte(img)).ToList();
                File.WriteAllBytes("./Images/Sortie.bmp", result.ToArray());
            }
            else
            {
                Console.WriteLine("le quotient de rétrécissemnt n'est pas un mutiple de 2");
            }
        }
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
