using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
