using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LectureImage
{
    class Complexe
    {
        double reel;
        double imaginaire;
        public double Entier
        {
            get { return reel; }
            set { value = reel; }
        }
        public double Imaginaire
        {
            get { return imaginaire; }
            set { value = imaginaire; }
        }
        public Complexe(double reel, double imaginaire)
        {
            this.reel = reel;
            this.imaginaire = imaginaire;
        }
        public string toString()
        {
            return " " + reel + " + " + imaginaire + "i ";
        }
        public Complexe Addition(Complexe z)
        {
            Complexe somme = new Complexe(0,0);
            somme.reel = reel + z.reel;
            somme.imaginaire = imaginaire + z.imaginaire;
            return somme;
        }
        public Complexe Soustraction(Complexe z)
        {
            Complexe difference = new Complexe(0,0);
            difference.reel = reel - z.reel;
            difference.imaginaire = imaginaire - z.imaginaire;
            return difference;
        }
        public Complexe Multiplication(Complexe z)
        {
            Complexe produit = new Complexe(0, 0);
            produit.reel = reel * z.reel - imaginaire * z.imaginaire;
            produit.imaginaire = imaginaire * z.reel + reel * z.imaginaire;
            return produit;
        }
        public double Norme()
        {
            double norme = 0;
            norme = Math.Sqrt(reel * reel + imaginaire*imaginaire);
            return norme; 
        }
    }
}
