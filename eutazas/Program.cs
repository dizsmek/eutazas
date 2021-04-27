using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace eutazas
{
    class Utazas
    {
        public int Megallo { get; set; }

        public int Datum { get; set; }

        public int Ev { get; set; }
        public int Honap { get; set; }
        public int Nap { get; set; }
        public int Ora { get; set; }
        public int Perc { get; set; }
        
        public string Azon { get; set; }
        
        public string Tipus { get; set; }
        
        public bool Jegy { get; set; }

        public bool Kedvezmenyes { get; set; }
        public bool Ingyenes { get; set; }
        
        public Nullable<int> FelhasznalhatoJegyek { get; set; }

        public Nullable<int> LejaratDatum { get; set; }

        public Nullable<int> LejaratEv { get; set; }
        public Nullable<int> LejaratHonap { get; set; }
        public Nullable<int> LejaratNap { get; set; }

        public Utazas (string megallo, string datum, string azon, string tipus, string ervenyesseg)
        {
            bool jegy = tipus == "JGY";

            string[] datumArr = datum.Split('-');

            this.Megallo = Convert.ToInt32(megallo);

            this.Datum = Convert.ToInt32(datumArr[0]);

            this.Ev = Convert.ToInt32(datumArr[0].Substring(0, 4));
            this.Honap = Convert.ToInt32(datumArr[0].Substring(4, 2));
            this.Nap = Convert.ToInt32(datumArr[0].Substring(6));

            this.Ora = Convert.ToInt32(datumArr[1].Substring(0, 2));
            this.Perc = Convert.ToInt32(datumArr[1].Substring(2));

            this.Azon = azon;

            this.Tipus = tipus;

            this.Jegy = jegy;

            this.Kedvezmenyes = false;
            this.Ingyenes = false;

            if (jegy)
            {
                this.FelhasznalhatoJegyek = Convert.ToInt32(ervenyesseg);

                this.LejaratDatum = null;
                this.LejaratEv = null;
                this.LejaratHonap = null;
                this.LejaratNap = null;
            } else
            {
                this.FelhasznalhatoJegyek = null;

                this.LejaratDatum = Convert.ToInt32(ervenyesseg);
                this.LejaratEv = Convert.ToInt32(ervenyesseg.Substring(0, 4));
                this.LejaratHonap = Convert.ToInt32(ervenyesseg.Substring(4, 2));
                this.LejaratNap = Convert.ToInt32(ervenyesseg.Substring(6));

                switch (tipus)
                {
                    case "TAB":
                    case "NYB":
                        this.Kedvezmenyes = true;
                        break;

                    default:
                        this.Ingyenes = true;
                        break;
                }
            }
        }
    }

    class MainClass
    {
        public static List<Utazas> utazasok = new List<Utazas>();

        public static void Main(string[] args)
        {
            #region Feladat1
            StreamReader sr = new StreamReader(new FileStream("utasadat.txt", FileMode.Open));

            while (!sr.EndOfStream)
            {
                string[] line = sr.ReadLine().Split(' ');
                utazasok.Add(new Utazas(line[0], line[1], line[2], line[3], line[4]));
            }

            sr.Close();

            Console.WriteLine("1. Feladat: \nAz utasadat.txt fajl tartalma beolvasva.");
            #endregion

            Feladat2();
            Feladat3();
            Feladat4();
            Feladat5();
            Feladat7();
        }

        public static void Feladat2()
        {
            Console.WriteLine("\n2. Feladat");
            Console.WriteLine($"{utazasok.Count} ember probalt felszallni a buszra.");
        }

        public static void Feladat3()
        {
            Console.WriteLine("\n3. Feladat");

            int ervenytelenCounter = 0;

            foreach (Utazas utazas in utazasok)
            {
                if (utazas.Jegy)
                {
                    if (utazas.FelhasznalhatoJegyek == 0)
                    {
                        ervenytelenCounter += 1;
                    }
                } else
                {
                    if (utazas.Datum >= utazas.LejaratDatum)
                    {
                        ervenytelenCounter += 1;
                    }
                }
            }

            Console.WriteLine($"{ervenytelenCounter} felszallast kellett visszautasitani.");
        }

        public static void Feladat4()
        {
            Console.WriteLine("\n4. Feladat");

            int legforgalmasabbMegallo = utazasok
                .GroupBy(utazas => utazas.Megallo)
                .OrderByDescending(group => group.Count())
                .FirstOrDefault().OrderBy(utazas => utazas.Megallo)
                .First().Megallo;

            Console.WriteLine($"A {legforgalmasabbMegallo} sorszamu megalloban probalt felszallni a legtobb utas.");
        }

        public static void Feladat5()
        {
            Console.WriteLine("\n5. Feladat");

            int kedvezmenyesek = utazasok
                .Where(utazas => !utazas.Jegy && utazas.Datum > utazas.LejaratDatum && utazas.Kedvezmenyes)
                .Count();

            int ingyenesek = utazasok
                .Where(utazas => !utazas.Jegy && utazas.Datum > utazas.LejaratDatum && utazas.Ingyenes)
                .Count();

            Console.WriteLine($"{kedvezmenyesek} kedvezmenyes utas es {ingyenesek} ingyenes utas szallt fel.");
        }

        // Feladat 6
        public static int napokszama(int e1, int h1, int n1, int? e2, int? h2, int? n2)
        {
            int honap1 = (h1 + 9) % 12;
            int ev1 = e1 - honap1 / 10;
            int d1 = 365 * ev1 + ev1 / 4 - ev1 / 100 + ev1 / 400 + (h1 * 306 + 5) / 10 + n1 - 1;
            int honap2 = (int)(h2 + 9) % 12;
            int ev2 = (int)(e2 - honap2 / 10);
            int d2 = (int)(365 * ev2 + ev2 / 4 - ev2 / 100 + ev2 / 400 + (h2 * 306 + 5) / 10 + n2 - 1);

            return d2 - d1;
        }

        public static void Feladat7()
        {
            IEnumerable<Utazas> mindjartLejar = utazasok
                .Where(utazas => !utazas.Jegy)
                .Where(x => napokszama(x.Ev, x.Honap, x.Nap, x.LejaratEv, x.LejaratHonap, x.LejaratNap) <= 3);

            StreamWriter sr = new StreamWriter(new FileStream("figyelmeztetes.txt", FileMode.Create));
            foreach (Utazas utazas in mindjartLejar)
            {
                string sor = utazas.Azon;
                sor += $" {utazas.LejaratEv}-{utazas.LejaratHonap}-{utazas.LejaratNap}";
                sr.WriteLine(sor);
            }

            sr.Close();
        }
    }
}
