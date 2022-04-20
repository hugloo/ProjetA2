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
        byte[,] imgB;
        List<byte> header = new List<byte>(54);
        int headerSize = 54;
        int[,] flou = { { 0, 0, 0, 0, 0 }, { 0, 1, 1, 1, 0 }, { 0, 1, 1, 1, 0 }, { 0, 1, 1, 1, 0 }, { 0, 0, 0, 0, 0 } };


        #region Proprietes
        public int Height
        {
            get { return height; }
        }

        public int Width
        {
            get { return width; }
        }

        #endregion

        #region Consrtucteurs et toString()
        public MyImage(string fichier)
        {
            this.fichier = fichier;
            byte[] myfile = File.ReadAllBytes(fichier);

            width = Convertir_Endian_To_Int(myfile, 18);
            height = Convertir_Endian_To_Int(myfile, 22);

            // CREATION HEADER
            for (int i = 0; i < headerSize; i++)
            {
                header.Add(myfile[i]);
            }

            //Console.WriteLine("h = " + height + " w = " + width);

            // CREATION IMAGE

            imgB = new byte[height, width * 3];
            int a = headerSize;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width * 3; j++)
                {
                    imgB[i, j] = myfile[a];
                    a++;
                }
            }


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

        #endregion

        #region Convertions et autres

        public byte[] Convertir_Int_To_Endian(int entier)
        {
            byte[] result = BitConverter.GetBytes(entier);
            return result;
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

        public void Enregistrement(Pixel[,] result)
        {
            File.WriteAllBytes("./Images/Sortie.bmp", header.Concat(PixelToByte(result)).ToArray());
        }

        public List<Byte> PixelToByte(Pixel[,] tab)
        {
            List<Byte> result = new List<Byte>(tab.GetLength(0) * 3 * tab.GetLength(1));
            for (int i = 0; i < tab.GetLength(0); i++)
            {
                for (int j = 0; j < tab.GetLength(1); j++)
                {
                    result.Add(Convert.ToByte(tab[i, j].Rouge));
                    result.Add(Convert.ToByte(tab[i, j].Vert));
                    result.Add(Convert.ToByte(tab[i, j].Bleu));
                }

            }
            return result;
        }

        public List<byte> ModifierHeader(int hauteur, int largeur)
        {
            byte[] hauteur1 = Convertir_Int_To_Endian(hauteur);
            //Il manque la taille du fichier à modifier
            byte[] largeur1 = Convertir_Int_To_Endian(largeur);
            byte[] weight = Convertir_Int_To_Endian(height * 3 * width + headerSize);
            byte[] weightImg = Convertir_Int_To_Endian(height * 3 * width);
            for (int i = 0; i < 4; i++)
            {
                header[18 + i] = hauteur1[i];
                header[22 + i] = largeur1[i];
                header[2 + i] = weight[i];
                header[34 + i] = weightImg[i];
            }

            for (int i = 0; i < header.Count; i++)
            {
                Console.Write(header[i] + " ");
            }
            return header;
        }

        #endregion

        #region Modification images

        public void Miroir()
        {
            Pixel[,] img = new Pixel[height, width];


            for (int i = 0; i < image.GetLength(0); i++)
            {
                int c = 0;
                for (int j = image.GetLength(1) - 1; j >= 0; j--, c++)
                {
                    img[i, c] = image[i, j];
                }
            }

            Enregistrement(img);
        }

        public void Negative()
        {
            Pixel[,] result = new Pixel[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    result[i, j] = image[i, j].Negatif();
                }
            }
            Enregistrement(result);
        }

        public void NoirEtBlanc()
        {
            Pixel[,] img = new Pixel[height, width];

            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    img[i, j] = image[i, j].NoirBlanc();
                }
            }
            Enregistrement(img);
        }
        public void NuancesDeGris()
        {
            Pixel[,] img = new Pixel[height, width];

            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    img[i, j] = image[i, j].Gris();
                }
            }
            Enregistrement(img);
        }

        #endregion


        public Pixel[,] MatriceDeConvultion(int[,] convolution)
        {
            Pixel[,] matricefinale = null;
            int addition_rouge = 0;
            int addition_vert = 0;
            int addition_bleu = 0;
            if (image != null && convolution != null)
            {
                int ligne = image.GetLength(0);
                int colonne = image.GetLength(1);
                Console.WriteLine("ligne = " + ligne + "colonne = " + colonne);
                matricefinale = new Pixel[ligne, colonne];
                for (int i = 1; i < ligne-1; i++)
                {
                    for (int j = 1; j < colonne-1; j++)
                    {
                        addition_rouge = 0;
                        addition_vert = 0;
                        addition_bleu = 0;
                        for (int k = 0;k < 3; k++) //pour une matrice de convolution 3*3
                        {
                            for (int l = 0; l < 3; l++)
                            {
                                addition_rouge += convolution[k, l] * image[i - 1 + l, j - 1 + k].Rouge;
                                addition_bleu += convolution[k, l] * image[i - 1 + l, j - 1 + k].Bleu;
                                addition_vert += convolution[k, l] * image[i - 1 + l, j - 1 + k].Vert;
                            }
                        }
                            if (addition_rouge < 0)
                            {
                                addition_rouge = 0;
                            }
                            if (addition_rouge > 255)
                            {
                                addition_rouge = 255;
                            }
                            if (addition_bleu < 0)
                            {
                                addition_bleu = 0;
                            }
                            if (addition_bleu > 255)
                            {
                                addition_bleu = 255;
                            }
                            if (addition_vert < 0)
                            {
                                addition_vert = 0;
                            }
                            if (addition_vert > 255)
                            {
                                addition_vert = 255;
                            }
                        matricefinale[i, j] = new Pixel(addition_rouge, addition_vert, addition_bleu);
                    }
                }
                for (int i = 0; i < ligne; i++)
                {
                    for (int j = 0; j < colonne; j++)
                    {
                        if (i == 0 || i == ligne - 1 || j == 0 || j == colonne - 1)
                        {
                            matricefinale[i, j] = new Pixel(0, 0, 0);
                        }
                    }
                }
            }
            Enregistrement(matricefinale);
            return matricefinale;
        }

        public void Rotation90()
        {
            Pixel[,] result = new Pixel[image.GetLength(1), image.GetLength(0)];
            int c = 0;
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    Console.WriteLine("i = " + i + " j = " + j + " c = " + c);
                    result[j, image.GetLength(0) - i - 1] = image[i, j];

                    //Console.Write(result[c, j].Rouge + " ");
                    //Console.Write(result[c, j].Vert + " ");
                    //Console.Write(result[c, j].Bleu + " ");
                }
                Console.WriteLine();
            }
            ModifierHeader(result.GetLength(0), result.GetLength(1));
            Enregistrement(result);
        }

        public void Trans()
        {
            Pixel[,] result = new Pixel[image.GetLength(1), image.GetLength(0)];

            var rows = image.GetLength(0);
            var columns = image.GetLength(1);

            for (var c = 0; c < columns; c++)
            {
                for (var r = 0; r < rows; r++)
                {
                    result[c, r] = image[r, c];
                }
            }

            Pixel[,] result2 = new Pixel[image.GetLength(1), image.GetLength(0)];

            int i = 0;
            for (var c = 0; c < result.GetLength(0); c++)
            {
                for (var r = result.GetLength(1) - 1; r >= 0; r--, i++)
                {
                    result2[c, i] = image[i, r];
                }
                i = 0;
            }
            ModifierHeader(result.GetLength(0), result.GetLength(1));
            Enregistrement(result2);
        }
        public void Agrandir_Image(int val)
        {
            int l = header[4] * val;
            int h = header[8] * val;
            //ModifierHeader(h, l);
            List<byte> head = new List<byte>(headerSize);
            List<Pixel> img = new List<Pixel>(height * width * val * val);
            byte[] head1 = new byte[headerSize];
            for (int k = 0; k < headerSize; k++)
            {
                head.Add(Convert.ToByte(header[k]));
                head1[k] = Convert.ToByte(header[k]);
            }
            l = Convertir_Endian_To_Int(head1, 4);
            h = Convertir_Endian_To_Int(head1, 8);
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
            //List<byte> result = head.Concat(PixelToByte(img)).ToList();
            //File.WriteAllBytes("./Images/Sortie.bmp", result.ToArray());
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
                        if (i + compteur_ligne < image.GetLength(0) && j + compteur_colonne < image.GetLength(1))
                        {
                            img.Add(image[i + compteur_ligne, j + compteur_colonne]);
                            compteur_colonne += val;
                        }
                    }
                    compteur_colonne = 0;
                    compteur_ligne += val;
                }
                //List<byte> result = head.Concat(PixelToByte(img)).ToList();
                //File.WriteAllBytes("./Images/Sortie.bmp", result.ToArray());
            }
            else
            {
                Console.WriteLine("le quotient de rétrécissemnt n'est pas un mutiple de 2");
            }
        }
    }
}
