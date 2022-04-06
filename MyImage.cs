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
        byte[] header = new byte[54];
        int headerSize = 54;
        int[,] flou = { { 0, 0, 0, 0, 0 }, { 0, 1, 1, 1, 0 }, { 0, 1, 1, 1, 0 }, { 0, 1, 1, 1, 0 }, { 0, 0, 0, 0, 0 } };

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
        public int[,] MatriceConvolution(int[,] kernel)
        {
            int[,] resultat = null;
            int ajustementligne = 0;
            int ajustementcolonne = 0;
            int addition = 0;
            if (imgB != null && kernel != null)
            {
                resultat = new int[imgB.GetLength(0), imgB.GetLength(1)];
                for (int i = 0; i < imgB.GetLength(0); i++)
                {
                    for (int j = 0; j < image.GetLength(1); j++)
                    {
                        if (i == 0 || j == 0 || j == image.GetLength(1) || i == image.GetLength(0))
                        {
                            resultat[i, j] = 255;
                        }
                        else
                        {
                            addition = 0;
                            for (int k = 0; k < kernel.GetLength(0); k++)
                            {
                                for (int l = 0; l < kernel.GetLength(1); l++)
                                {
                                    if (l == 0)
                                    {
                                        ajustementcolonne = -1;
                                    }
                                    if (l == 2)
                                    {
                                        ajustementcolonne = 1;
                                    }
                                    if (k == 0)
                                    {
                                        ajustementligne = -1;
                                    }
                                    if (k == 2)
                                    {
                                        ajustementligne = 1;
                                    }
                                    addition += kernel[k, l] * imgB[i + ajustementligne, j + ajustementcolonne]; }
                            }
                            if (addition < 0)
                            {
                                resultat[i, j] = 0;
                            }
                            else if (addition > 255)
                            {
                                resultat[i, j] = 255;
                            }
                            else resultat[i, j] = addition;
                        }
                    }
                }
                List<byte> result = new List<byte>();
                for (int i = 0; i < resultat.GetLength(0); i++)
                {
                    for (int j = 0; j < resultat.GetLength(1); j++)
                    {
                        Console.Write(resultat[i, j] + " ");
                        result.Add(Convert.ToByte(resultat[i, j]));
                    }
                    Console.WriteLine();
                }
                File.WriteAllBytes("./Images/Sortie.bmp", header.Concat(result).ToList().ToArray());
            }

            for (int i = 0; i < resultat.GetLength(0); i++)
            {
                for (int j = 0; j < resultat.GetLength(1); j++)
                {
                    Console.Write(resultat[i, j] + " ");
                }
                Console.WriteLine();
            }

            return resultat;
        }
        public int[,] MatriceDeConvultion(int[,] convultion)
        {
            int[,] matricefinal = null;
            if (imgB != null && convultion != null)
            {
                int ligne = imgB.GetLength(0);
                int colonne = imgB.GetLength(1);
                Console.WriteLine("ligne = " + ligne + "colonne = " + colonne);
                matricefinal = new int[ligne, colonne];
                for (int i = 0; i < ligne; i++)
                {
                    for (int j = 0; j < colonne; j++)
                    {
                        int addition = 0;
                        for (int k = -1; k <= 1; k++)
                        {
                            for (int l = -1; l <= 1; l++)
                            {
                                int position1 = i + k;
                                int position2 = j + l;
                                if (position1 < 0 || position2 < 0 || position1 >= ligne || position2 >= colonne)
                                {
                                }
                                else
                                {
                                    addition = addition + convultion[k + 1, l + 1] * imgB[position1, position2];
                                }
                            }
                        }
                        if (addition < 0)
                        {
                            matricefinal[i, j] = 0;
                        }
                        else if (addition  > 255)
                        {
                            matricefinal[i, j] = 255;
                        }
                        else matricefinal[i, j] = addition;

                    }
                }
                List<byte> result = new List<byte>();
                for (int i = 0; i < matricefinal.GetLength(0); i++)
                {
                    for (int j = 0; j < matricefinal.GetLength(1); j++)
                    {
                        Console.Write(matricefinal[i, j] + " ");
                        result.Add(Convert.ToByte(matricefinal[i, j]));
                    }
                    Console.WriteLine();
                }
                File.WriteAllBytes("./Images/Sortie.bmp", header.Concat(result).ToList().ToArray());
            }

            for (int i = 0; i < matricefinal.GetLength(0); i++)
            {
                for (int j = 0; j < matricefinal.GetLength(1); j++)
                {
                    Console.Write(matricefinal[i, j] + " ");
                }
                Console.WriteLine();
            }          

            return matricefinal;
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
            List<Pixel> img = new List<Pixel>(height * width);

            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    img.Add(image[i, j].Negatif());
                }
            }

            List<byte> result = header.Concat(PixelToByte(img)).ToList();
            File.WriteAllBytes("./Images/Sortie.bmp", result.ToArray());
        }
        public void NoirEtBlanc()
        {
            List<Pixel> img = new List<Pixel>(height * width);

            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    img.Add(image[i, j].NoirBlanc());
                }
            }
            List<byte> result = header.Concat(PixelToByte(img)).ToList();
            File.WriteAllBytes("./Images/Sortie.bmp", result.ToArray());
        }
        public void NuancesDeGris()
        {
            List<Pixel> img = new List<Pixel>(height * width);

            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    img.Add(image[i, j].Gris());
                }
            }
            List<byte> result = header.Concat(PixelToByte(img)).ToList();
            File.WriteAllBytes("./Images/Sortie.bmp", result.ToArray());
        }
        public void Miroir()
        {
            List<Pixel> img = new List<Pixel>(height * width);

            for (int i = 0 ; i < image.GetLength(0); i++)
            {
                for (int j = image.GetLength(1) - 1; j >= 0; j--)
                {
                    img.Add(image[i, j]);
                }
            }
            List<byte> result = header.Concat(PixelToByte(img)).ToList();
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
        public void ChangerTailleImage(int valeur)
        {
            int new_height = 0;
            int new_width = 0;
            Pixel[,] new_img = null;
            if (valeur <= 0 )
            {
                Console.WriteLine("Veuillez donner une valeur positive différente de 0");
                int new_val = Convert.ToInt32(Console.ReadLine());
                ChangerTailleImage(new_val);
            }
            else
            {
                if (valeur < 1)
                {
                    new_height = height * valeur;
                    new_width = width * valeur;
                    ModifierHeader(new_height, new_width);
                    ModifierHeader(new_height, new_width);
                    new_img = new Pixel[height * valeur, width * valeur];
                    for (int i = 0; i < new_img.GetLength(0); i++)
                    {
                        for (int j = 0; j < new_img.GetLength(1); j++)
                        {
                                new_img[i, j] = image[i / valeur, j / valeur];
                        }
                    }
                }
                if (valeur >= 1)
                {
                    new_height = height * valeur;
                    new_width = width * valeur;
                    ModifierHeader(new_height, new_width);
                    new_img = new Pixel[new_height, new_width];
                    int compteur_ligne = 0;
                    int compteur_colonne = 0;
                    for (int i = 0; i < image.GetLength(0); i++)
                    {
                        for (int j = 0; j < image.GetLength(1); j++)
                        {
                            for (int k = 0; k < valeur; k++)
                            {
                                new_img[i + k, j] = image[i, j];
                                new_img[i, j + k] = image[i, j];
                            }
                            compteur_colonne += valeur;
                        }
                        compteur_ligne += valeur;
                    }
                }
            }
            List<Pixel> img = new List<Pixel>(new_height * width);

            for (int i = 0; i < new_img.GetLength(0); i++)
            {
                for (int j = 0; j < new_img.GetLength(1); j++)
                {
                    img.Add(new_img[i, j]);
                }
            }
            List<byte> result = header.Concat(PixelToByte(img)).ToList();
            File.WriteAllBytes("./Images/Sortie.bmp", result.ToArray());
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
        /*public void Agrandir_Image(int val)
        {
            int l = header[4] * val;
            int h = header[8] * val;
            ModifierHeader(h, l);
            List<byte> head = new List<byte>(headerSize);
            List<Pixel> img = new List<Pixel>(height * width * val * val);
            byte[] head1 = new byte[headerSize];
            for (int k = 0; k < headerSize; k++)
            {
                head.Add(Convert.ToByte(header[k]));
                head1[k] = Convert.ToByte(header[k]);
            }
            l = Convertir_Endian_To_Int(head1, 4);
            h = Convertir_Endian_To_Int (head1, 8);
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
        */
        public byte[] Convertir_Int_To_Endian(int entier)
        {
            byte[] result = BitConverter.GetBytes(entier);
            return result;
        }

        
        public byte[] ModifierHeader(int hauteur, int largeur)
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
       
    }
}
