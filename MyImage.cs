using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;

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
                for (int i = 1; i < ligne - 1; i++)
                {
                    for (int j = 1; j < colonne - 1; j++)
                    {
                        addition_rouge = 0;
                        addition_vert = 0;
                        addition_bleu = 0;
                        for (int k = 0; k < 3; k++) //pour une matrice de convolution 3*3
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

        public void Rotation(int angle)
        {
            Pixel[,] result = new Pixel[image.GetLength(1), image.GetLength(0)];

            int[,] rotationMat = { { (int)Math.Cos(angle), (int)-Math.Sin(angle) }, { (int)Math.Sin(angle), (int)Math.Cos(angle) } };

            int pivotX = (int)image.GetLength(1) / 2;
            int pivotY = (int)image.GetLength(0) / 2;

            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    int[,] xy_mat = { { pivotX }, { pivotY } };
                    int[,] rotatemat = Dot(rotationMat, xy_mat);

                    //newX = pivotX + (int)rotatemat.
                }
                Console.WriteLine();
            }
            ModifierHeader(result.GetLength(0), result.GetLength(1));
            Enregistrement(result);
        }

        public static int[,] Dot(int[,] matrix1, int[,] matrix2)
        {
            // cahing matrix lengths for better performance  
            var matrix1Rows = matrix1.GetLength(0);
            var matrix1Cols = matrix1.GetLength(1);
            var matrix2Rows = matrix2.GetLength(0);
            var matrix2Cols = matrix2.GetLength(1);

            // checking if product is defined  
            if (matrix1Cols != matrix2Rows)
                throw new InvalidOperationException
                  ("Product is undefined. n columns of first matrix must equal to n rows of second matrix");

            // creating the final product matrix  
            int[,] product = new int[matrix1Rows, matrix2Cols];

            for (int matrix1_row = 0; matrix1_row < matrix1Rows; matrix1_row++)
            {
                for (int matrix2_col = 0; matrix2_col < matrix2Cols; matrix2_col++)
                {
                    for (int matrix1_col = 0; matrix1_col < matrix1Cols; matrix1_col++)
                    {
                        product[matrix1_row, matrix2_col] += matrix1[matrix1_row, matrix1_col] * matrix2[matrix1_col, matrix2_col];
                    }
                }
            }

            return product;
        }
        public Pixel[,] Rotation90()
        {
            Pixel[,] result = new Pixel[image.GetLength(1), image.GetLength(0)];
            for(int i = 0; i < image.GetLength(0); i++)
            {
                for(int j = 0; j < image.GetLength(1); j++)
                {
                    result[image.GetLength(1) - 1 - j, image.GetLength(0) - 1 - i] = image[i, j];
                }
            }
            ModifierHeader(result.GetLength(1), result.GetLength(0));
            Enregistrement(result);
            return result;
        }
        public Pixel[,] resizePixels(int w2, int h2) //Nearest Neighbor Image Scaling ne marche qu'avec H2=W2
        {
            Pixel[,] result = new Pixel[w2, h2];
            int w1 = image.GetLength(1);
            int h1 = image.GetLength(0);

            double x_ratio = w1 / (double)w2;
            double y_ratio = h1 / (double)h2;
            int px, py;
            for (int i = 0; i < h2; i++)
            {
                for (int j = 0; j < w2; j++)
                {
                    px = Convert.ToInt32(Math.Floor(j * x_ratio));
                    py = Convert.ToInt32(Math.Floor(i * y_ratio));

                    result[i, j] = new Pixel(image[py, px].Rouge, image[py, px].Vert, image[py, px].Bleu);

                }
            }
            ModifierHeader(result.GetLength(0), result.GetLength(1));
            Enregistrement(result);
            return result;
        }
        public Pixel[,] Fractale(int hauteur, int largeur)
        {
            //on dessine la fractale en entière, les variables suivantes sont la zone où l'on dessine
            double x1 = -2.1;
            double x2 = 0.6;
            double y1 = -1.2;
            double y2 = 1.2;

            // taile de l'image
            double zoom1 = largeur / (x2 - x1);
            double zoom2 = hauteur / (y2 - y1);
            /*Pixel[] colors = new Pixel[256];
            for (int i = 0; i < 256; i++)
            {
                colors[i] = new Pixel((byte)((i >> 5) * 36), (byte)((i >> 3 & 7) * 36), (byte)((i & 3) * 85));
            }*/
            Pixel[,] fractale = new Pixel[hauteur, largeur];
            for (int i =0;i<hauteur; i++)
            {
                for(int j = 0; j < largeur; j++)
                {
                    int itération = 0;
                    Complexe z = new Complexe(0, 0);
                    Complexe c = new Complexe(i/zoom1 + x1, j/zoom2 + y1);
                    while (itération < 150 &&  z.Norme()< 2)
                    {
                        z = z.Multiplication(z);
                        z = z.Addition(c);
                        itération++;
                    }
                    //fractale[i, j] = colors[itération];
                    if(itération == 150)
                    {
                        fractale[i, j] = new Pixel(0, 255, 0);
                    }
                    else
                    {
                        fractale[i,j] = new Pixel(0, 0, 0);
                    }
                }
            }
            ModifierHeader(largeur, hauteur);
            Enregistrement(fractale);
            return fractale; 
        }
        
    }
}

