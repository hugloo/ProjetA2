using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        Dictionary<string, int> Alphanum = new Dictionary<string, int>() {{"0", 0}, {"1", 1}, {"2", 2}, {"3", 3}, {"4", 4}, {"5", 5}, {"6", 6}, {"7", 7}, {"8", 8}, {"9", 9},
                                                    {"A", 10}, {"B", 11},  {"C", 12 }, {"D", 13}, {"E", 14}, {"F", 15}, {"G", 16}, {"H", 17}, {"I", 18}, {"J", 19},
                                                    {"K", 20}, {"L", 21}, {"M", 22}, {"N", 23}, {"O", 24}, {"P", 25}, {"Q", 26}, {"R", 27}, {"S", 28}, {"T", 29},
                                                    {"U", 30}, {"V", 31}, {"W", 32}, {"X", 33}, {"Y", 34}, {"Z", 35}, {" ", 36}, {"$", 37}, {"%", 38}, {"*", 39},
                                                    {"+", 40}, {"-", 41}, {".", 42}, {"/", 43}, {":", 44}};
        string IndicateurMode = "0010";


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

        #region Constructeurs et toString()
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
        /// <summary>
        /// fonction 
        /// </summary>
        /// <param name="entier"></param>
        /// <returns></returns>
        public byte[] Convertir_Int_To_Endian(int entier)
        {
            Console.WriteLine("ENTIERRRR = " + entier);
            byte[] result = BitConverter.GetBytes(entier);
            for (int i = 0; i < result.Length; i++) Console.Write(result[i] + " ");
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
        public double ConvertirDegrésRadians(double degrés)
        {
            return (Math.PI / 180) * degrés;
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
            byte[] largeur1 = Convertir_Int_To_Endian(largeur);
            byte[] weight = Convertir_Int_To_Endian(hauteur * largeur * 3 + headerSize);
            byte[] weightImg = Convertir_Int_To_Endian(hauteur * largeur * 3);
            for (int i = 0; i < 4; i++)
            {
                header[18 + i] = largeur1[i];
                header[22 + i] = hauteur1[i];
                header[2 + i] = weight[i];
                header[34 + i] = weightImg[i];
            }

            return header;
        }

        #endregion

        #region Modification images

        public Pixel[,] Agrandissement(int coef) //Nearest Neighbor Image Scaling ne marche qu'avec H2=W2
        {
            int h2 = image.GetLength(0) * coef;
            int w2 = image.GetLength(1) * coef;

            Pixel[,] result = new Pixel[h2, w2];
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
        public void Innovation()
        {
            Pixel[,] result = new Pixel[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    result[i, j] = image[i, j].Innovation();
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
            //ModifierHeader(result.GetLength(0), result.GetLength(1));
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
        public void Graph()
        {
            int r = 0;
            Pixel[,] result = new Pixel[270, 257];

            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    result[i, j] = new Pixel(0, 0, 0);
                }
            }

            for (int k = 0; k < 1; k++)
            {
                r = 0;
                for (int i = 0; i < image.GetLength(0); i++)
                {
                    for (int j = 0; j < image.GetLength(1); j++)
                    {
                        if (image[i, j].Rouge == k)
                        {
                            r++;
                        }
                    }
                    //Console.WriteLine();
                }
                // Console.WriteLine("r = " + (result.GetLength(0) - 1 - r) + " ");
                for (int n = result.GetLength(0) - 1 - r; n < result.GetLength(0); n++)
                {
                    //Console.Write(" n = " + n + " " + " k = " +  k);
                    result[n, k] = new Pixel(255, 0, 0);
                }
            }

            /*
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    Console.Write(result[i, j].Rouge + " ");
                    Console.Write(result[i, j].Vert + " ");
                    Console.Write(result[i, j].Bleu + " ");
                }
                Console.WriteLine();
            }*/

            ModifierHeader(result.GetLength(0), result.GetLength(1));
            Enregistrement(result);
        }
        /// <summary>
        /// Prend une taille d'image en entrée et fait une fractale bien centrée et proportionnée dessus avec le nombre d'itérations voulues
        /// </summary>
        /// <param name="hauteur">hauteur de l'image en pixel</param>
        /// <param name="largeur">largeur de l'image en pixel</param>
        /// <param name="itération_max">nombre d'itérations limites</param>
        /// <returns></returns>
        public Pixel[,] Fractale(int hauteur, int largeur, int itération_max)
        {
            //délimite la zone où l'on va dessiner
            double x1 = -2.1;
            double x2 = 0.6;
            double y1 = -1.2;
            double y2 = 1.2;
            //l'endroit où l'on zoom, si on change ces valeurs pour un gros nombre la fractale n'est plus centrée et on en voit qu'un morceau, elle est trop zoomé
            double zoom1 = largeur / (x2 - x1);
            double zoom2 = hauteur / (y2 - y1);
            Pixel[,] fractale = new Pixel[hauteur, largeur];
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    int itération = 0;
                    Complexe z = new Complexe(0, 0);
                    Complexe c = new Complexe(i / zoom1 + x1, j / zoom2 + y1);
                    while (itération < itération_max && z.Norme() < 2)
                    {
                        z = z.Multiplication(z);
                        z = z.Addition(c);
                        itération++;
                    }
                    if (itération == itération_max)
                    {
                        fractale[i, j] = new Pixel(0, 0, 0);
                    }
                    else
                    {
                        fractale[i, j] = new Pixel((int)itération * 255 / itération_max, 0, 0);
                    }
                }
            }
            ModifierHeader(largeur, hauteur);
            Enregistrement(fractale);
            return fractale;
        }
        /// <summary>
        /// Tourner une image à 90°
        /// </summary>
        /// <returns></returns>
        public Pixel[,] Rotation90()
        {
            Pixel[,] result = new Pixel[image.GetLength(1), image.GetLength(0)];

                for (int i = 0; i < image.GetLength(0); i++)
                {
                    for (int j = 0; j < image.GetLength(1); j++)
                    {
                        result[image.GetLength(1) - 1 - j, image.GetLength(0) - 1 - i] = image[i, j];
                    }
                }
            ModifierHeader(result.GetLength(1), result.GetLength(0));
            Enregistrement(result);
            return result;
        }
        public Pixel[,] CacherUneImage(MyImage image2)
        {
            if (image.GetLength(0) >= image2.image.GetLength(0) && image.GetLength(1) >= image2.image.GetLength(1))
            {
                for (int i = 0; i < image2.image.GetLength(0); i++)
                {
                    for (int j = 0; j < image2.image.GetLength(1); j++)
                    {
                        string rouge1 = Convert.ToString(image[i, j].Rouge, 2).PadLeft(8, '0');
                        string bleu1 = Convert.ToString(image[i, j].Bleu, 2).PadLeft(8, '0');
                        string vert1 = Convert.ToString(image[i, j].Vert, 2).PadLeft(8, '0');
                        string rouge2 = Convert.ToString(image2.image[i, j].Rouge, 2).PadLeft(8, '0');
                        string bleu2 = Convert.ToString(image2.image[i, j].Bleu, 2).PadLeft(8, '0');
                        string vert2 = Convert.ToString(image2.image[i, j].Vert, 2).PadLeft(8, '0');
                        string rougefinal = rouge1.Substring(0, 4) + rouge2.Substring(0, 4);
                        string bleufinal = bleu1.Substring(0, 4) + bleu2.Substring(0, 4);
                        string vertfinal = vert1.Substring(0, 4) + vert2.Substring(0, 4);
                        int val_rouge = Convert.ToInt32(rougefinal, 2);
                        int val_bleu = Convert.ToInt32(bleufinal, 2);
                        int val_vert = Convert.ToInt32(vertfinal, 2);
                        image[i, j] = new Pixel(val_rouge, val_vert, val_bleu);
                    }
                }
            }
            else
            {
                Console.WriteLine("Veuillez choisir une image à cacher plus petite ou de même taille que l'image qui cache");
            }
            Enregistrement(image);
            return image;
        }
        public Pixel[,] RetrouverUneImage()
        {
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    string rouge1 = Convert.ToString(image[i, j].Rouge, 2).PadLeft(8, '0');
                    string bleu1 = Convert.ToString(image[i, j].Bleu, 2).PadLeft(8, '0');
                    string vert1 = Convert.ToString(image[i, j].Vert, 2).PadLeft(8, '0');
                    string rougefinal = rouge1.Substring(4) + rouge1.Substring(0, 4);
                    string bleufinal = bleu1.Substring(4) + bleu1.Substring(0, 4);
                    string vertfinal = vert1.Substring(4) + vert1.Substring(0, 4);
                    int val_rouge = Convert.ToInt32(rougefinal, 2);
                    int val_bleu = Convert.ToInt32(bleufinal, 2);
                    int val_vert = Convert.ToInt32(vertfinal, 2);
                    image[i, j] = new Pixel(val_rouge, val_vert, val_bleu);
                }
            }
            ModifierHeader(image.GetLength(0), image.GetLength(1));
            Enregistrement(image);
            return image;
        }
        public Pixel[,] RotationQuelconque(double degrés)
        {
            double angle = ConvertirDegrésRadians(degrés);
            Pixel[,] resultat = new Pixel[(int)Math.Ceiling(Math.Sin(angle) * image.GetLength(1) + image.GetLength(0) * Math.Cos(angle)) + 1, (int)Math.Ceiling(Math.Sin(angle) * image.GetLength(0) + Math.Cos(angle) * image.GetLength(1)) + 1]; ;
            int référentiel_centre_x1 = 0;
            int référentiel_centre_y1 = 0;
            double référentiel_centre_x2 = 0;
            double référentiel_centre_y2 = 0;
            double nouvelle_position_x = 0;
            double nouvelle_position_y = 0;
                for(int i = 0; i < resultat.GetLength(0); i++)
                {
                    for(int j = 0; j < resultat.GetLength(1); j++)
                    {
                        resultat[i, j] = new Pixel(0, 0, 0);
                    }
                }
                for (int i = 0; i < image.GetLength(0); i++)
                {
                    for (int j = 0; j < image.GetLength(1)-1; j++)
                    {
                    référentiel_centre_x1 = i - image.GetLength(0)/2; //on se place au centre de l'image initiale et on regarde par rapport à là
                    référentiel_centre_y1 = j - image.GetLength(1)/2; //on se place au centre de l'image initiale et on regarde par rapport à là
                    référentiel_centre_x2 = référentiel_centre_x1 * Math.Cos(angle) - référentiel_centre_y1 * Math.Sin(angle); //avec matrice de rotation,
                    référentiel_centre_y2 = référentiel_centre_x1 * Math.Sin(angle) + référentiel_centre_y1 * Math.Cos(angle); //c'est l'emplacement du pixel dans l'image tournée mais par rapport au centre de la nouvelle image
                    nouvelle_position_x = référentiel_centre_x2 + (Math.Sin(angle) * image.GetLength(1) + image.GetLength(0) * Math.Cos(angle)) / 2; //on retourne dans le coin haut gauche de la nouvelle image
                    nouvelle_position_y = référentiel_centre_y2 + (Math.Sin(angle) * image.GetLength(0) + Math.Cos(angle) * image.GetLength(1)) / 2;
                    resultat[(int)Math.Ceiling(nouvelle_position_x),(int)Math.Ceiling(nouvelle_position_y)] = image[i, j];
                    //resultat[(int)((i - image.GetLength(0)/2) * Math.Cos(angle) - (j - image.GetLength(1)/2) * Math.Sin(angle) + (Math.Sin(angle) * image.GetLength(1) + image.GetLength(0) * Math.Cos(angle)) / 2), (int)((i - image.GetLength(0)/2) * Math.Sin(angle) + (j - image.GetLength(1)/2) * Math.Cos(angle) + (Math.Sin(angle) * image.GetLength(0) + Math.Cos(angle) * image.GetLength(1)) / 2)] = image[i, j];
                    }
                }
            ModifierHeader(resultat.GetLength(0), resultat.GetLength(1));
            Enregistrement(resultat);
            return resultat;
        }
        #region QR CODE

        /// <summary>
        /// Regroupe les éléments d'un string par 2 et traduit chaque couples en code binaire alphanumérique
        /// </summary>
        /// <param name="phrase">Le message porté par le QR code</param>
        /// <returns>un string contenant le code binaire pour chaque paire</returns>
        public string ConvertPhrase(string phrase)
        {
            string messageCode = "";
            string str = "";
            for (int i = 0; i < phrase.Length - 1; i = i + 2)
            {

                str = Convert.ToString(phrase[i]) + Convert.ToString(phrase[i + 1]);
                messageCode += ChaineToBinaire(str);
                if (phrase.Length % 2 != 0 && i == phrase.Length - 3) messageCode += ChaineToBinaire(Convert.ToString(phrase[i + 2]));
                Console.WriteLine(str);
                str = "";
            }
            return messageCode;
        }

        public bool QRCode(string phrase)
        {
            string longueur_phrase = Convert.ToString(phrase.Length, 2);
            if (phrase.Length <= 25)
            {
                longueur_phrase = Ajouter0(9 - longueur_phrase.Length, longueur_phrase);
            }
            string messageCorrige = Correction1(longueur_phrase, ConvertPhrase(phrase));
            Pixel[,] result = PlotQR(1);
            result = RemplirQR(result, messageCorrige);


            ModifierHeader(result.GetLength(0), result.GetLength(1));
            Enregistrement(result);

            return true;
        }

        /// <summary>
        /// Ajoute des 0 au début d'une chaine de caractère jusqu'a ce que celle ci atteigne une certaine longueur
        /// </summary>
        /// <param name="longueur">Longueur finale de la cahine</param>
        /// <param name="nb">chaine à laquelle on veut ajouter des 0</param>
        /// <returns></returns>
        public string Ajouter0(int longueur, string nb)
        {
            for (int i = 0; i < longueur; i++) nb = 0 + nb;
            return nb;
        }

        /// <summary>
        /// Convertit une chaine de caractère en en code binaire alphanumérique
        /// </summary>
        /// <param name="mot">chaine à convertir</param>
        /// <returns>convertion en binaire sur 11 bits d'une chaine de plus d'un caractère, 6 bits sinon</returns>
        public string ChaineToBinaire(string mot)
        {
            int nb = 0;

            for (int i = 0; i < mot.Length; i++)
            {
                nb += Alphanum[Convert.ToString(mot[mot.Length - i - 1])] * Convert.ToInt32(Math.Pow(45, i));
            }
            string result = Convert.ToString(nb, 2);
            Console.WriteLine(result);
            if (mot.Length > 1) result = Ajouter0(11 - result.Length, result);
            else if (mot.Length == 1) result = Ajouter0(6 - result.Length, result);

            Console.WriteLine(result);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nbChar"></param>
        /// <param name="messageCode"></param>
        /// <returns></returns>
        public string Correction1(string nbChar, string messageCode)
        {
            int i = 0;
            string messageCorige = "";
            messageCorige = IndicateurMode + nbChar + messageCode;

            while (messageCorige.Length < 152 && i < 4)
            {
                messageCorige += "0";
                i++;
            }
            while (messageCorige.Length % 8 != 0) messageCorige += "0";

            string lgMax = "1110110000010001";
            int count = 0;

            while (messageCorige.Length <= 152)
            {
                messageCorige += lgMax[count];
                count++;
                if (count == lgMax.Length) count = 0;
            }

            Console.WriteLine(messageCorige.Length);
            AfficherBit(messageCorige, 8);
            return messageCorige;
        }

        public void AfficherBit(string motBit, int nb)
        {
            for (int i = 0; i < motBit.Length - 1; i++)
            {
                if (i % nb == 0) Console.Write(" ");
                Console.Write(motBit[i]);
            }
            Console.WriteLine();
        }


        public Pixel[,] PlotQR(int version, int taille = 20)
        {
            Pixel[,] canva = Canvas(taille, taille);
            int x1 = 0, y1 = 0;
            int x2 = taille - 7, y2 = 0;
            int x3 = 0, y3 = taille - 7;

            //motifs de recherche
            PlotCarre(canva, 0, 6, x1, y1);
            PlotCarre(canva, 0, 6, x2, y2);
            PlotCarre(canva, 0, 6, x3, y3);

            for (int i = 0; i < 3; i++) //3 modules par 3 modules et d'un seul module noir au centre
            {
                for (int j = 0; j < 3; j++)
                {
                    canva[y1 + 2 + i, x1 + 2 + j] = new Pixel(0, 0, 0);
                    canva[y2 + 2 + i, x2 + 2 + j] = new Pixel(0, 0, 0);
                    canva[y3 + 2 + i, x3 + 2 + j] = new Pixel(0, 0, 0);

                }
            }


            for (int i = 8; i < x2; i++) //motifs de synchronisation 
            {
                if (i % 2 == 0)
                {
                    canva[i, 6] = new Pixel(0, 0, 0);
                    canva[6, i] = new Pixel(0, 0, 0);
                }
                else
                {
                    canva[i, 6] = new Pixel(1, 1, 1);
                    canva[6, i] = new Pixel(1, 1, 1);
                }

            }

            canva[(4 * version) + 9, 8] = new Pixel(0, 0, 0); //motif noir

            if (version == 2)
            {
                PlotCarre(canva, 0, 4, x1, y1);
                PlotCarre(canva, 0, 2, x2, y2);
                //canva[y3 + 2 + i, x3 + 2 + j] = new Pixel(0, 0, 0);
            }



            ModifierHeader(canva.GetLength(0), canva.GetLength(1));
            Enregistrement(canva);
            return canva;
        }

        public Pixel BitToPixel(char bit)
        {
            if (bit == '1') return new Pixel(0, 0, 0);
            else return new Pixel(1, 1, 1);
        }

        public Pixel[,] RemplirQR(Pixel[,] QR, string messageCorrige)
        {
            int c = 0;
            Console.WriteLine(messageCorrige);

            for (int i = 0; i < 12; i++)
            {
                for (int k = 0; k < 2; k++)
                {

                    QR[QR.GetLength(0) - 1 - i, QR.GetLength(0) - 1 - k] = BitToPixel(messageCorrige[c]);
                    c++;
                    Console.WriteLine((QR.GetLength(0) - 1 - i) + " " + (QR.GetLength(0) - 1 - k));
                }
            }
            Console.WriteLine("_");
            for (int i = 11; i >= 0; i--)
            {
                for (int k = 2; k < 4; k++)
                {

                    QR[QR.GetLength(0) - 1 - i, QR.GetLength(0) - 1 - k] = BitToPixel(messageCorrige[c]);
                    c++;
                    Console.WriteLine((QR.GetLength(0) - 1 - i) + " " + (QR.GetLength(0) - 1 - k));
                }
            }
            Console.WriteLine("_");

            return QR;
        }


        public Pixel[,] PlotCarre(Pixel[,] canva, int couleur, int taille, int x, int y)
        {
            for (int i = 0; i <= taille; i++)
            {
                canva[y, x + i] = new Pixel(couleur, couleur, couleur);
                canva[y + i, x] = new Pixel(couleur, couleur, couleur);

                canva[taille + y, x + i] = new Pixel(couleur, couleur, couleur);
                canva[y + i, taille + x] = new Pixel(couleur, couleur, couleur);

            }

            return canva;
        }

        public Pixel[,] Canvas(int hauteur, int largeur)
        {
            Pixel[,] result = new Pixel[hauteur, largeur];
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    result[i, j] = new Pixel(1, 1, 1);
                }
            }
            return result;
        }

        #endregion
    }
}


