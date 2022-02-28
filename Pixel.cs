namespace LectureImage
{
    class Pixel
    {
        int rouge;
        int vert;
        int bleu;

        public int Rouge
        {
            get { return rouge; }
            set { value = rouge; }
        }

        public int Vert
        {
            get { return vert; }
            set { value = vert; }
        }
        public int Bleu
        {
            get { return bleu; }
            set { value = bleu; }
        }

        public Pixel(int rouge, int vert, int bleu)
        {
            this.rouge = rouge;
            this.vert = vert;
            this.bleu = bleu;
        }

        public string toString()
        {
            return " " + rouge + " " + vert + " " + bleu;
        }

        public Pixel Negatif()
        {
            return new Pixel(255 - rouge, 255 - vert, 255 - bleu);
        }
        public Pixel Gris()
        {
            int a = 0;
            if (rouge >= bleu && rouge >= vert)
            {
                if (vert <= bleu)
                {
                    a = (rouge + vert) / 2;
                }
                else if (bleu <= vert)
                {
                    a = (rouge + bleu) / 2;
                }
            }
            else
            {
                if (bleu >= rouge && bleu >= vert)
                {
                    if (vert <= rouge)
                    {
                        a = (bleu + vert) / 2;
                    }
                    else if (rouge <= vert)
                    {
                        a = (rouge + bleu) / 2;
                    }
                }
                else
                {
                    if (vert >= bleu && vert >= rouge)
                    {
                        if (rouge <= bleu)
                        {
                            a = (rouge + vert) / 2;
                        }
                        else if (bleu <= rouge)
                        {
                            a = (vert + bleu) / 2;
                        }
                    }
                }
            }
            return new Pixel(a, a, a);
        }
        public Pixel NoirBlanc()
        {
            if (rouge >= 128)
            {
                rouge = 255;
            }
            else
            {
                rouge = 0;
            }
            if (vert >= 128)
            {
                vert = 255;
            }
            else
            {
                vert = 0;
            }
            if (bleu >= 128)
            {
                bleu = 255;
            }
            else
            {
                bleu = 0;
            }
            return new Pixel(rouge, vert, bleu);
        }
    }
}
