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
        }

        public int Vert
        {
            get { return vert; }
        }
        public int Bleu
        {
            get { return bleu; }
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

        public void Negatif()
        {
        rouge = 255 - rouge;
        vert = 255 - vert;
        bleu = 255 - bleu;
        }
        
    }
}
