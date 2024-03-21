using System;
using System.Collections.Generic;
using System.IO;

struct Hely
{
    public int Sor;
    public int Oszlop;
    public string Nev;

    public override bool Equals(object obj)
    {
        if (obj is Hely other)
        {
            return Sor == other.Sor && Oszlop == other.Oszlop;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Sor, Oszlop);
    }
}

public class Matrix
{
    private const int SorokSzama = 16;
    private const int OszlopokSzama = 15;
    private const char UresHely = 'O';
    private const char FoglaltHely = 'X';

    private char[,] matrix;
    private List<Hely> lefoglaltHelyek;

    public Matrix()
    {
        matrix = new char[SorokSzama, OszlopokSzama];
        lefoglaltHelyek = new List<Hely>();
        ToltseKiMatrixot(UresHely);

        // Olvassa be az előzőleg foglalt helyeket a fájlból
        OlvasdBeFoglaltHelyeketFajlbol("data.txt");
    }

    private void ToltseKiMatrixot(char value)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                matrix[i, j] = value;
            }
        }
    }

    private void OlvasdBeFoglaltHelyeketFajlbol(string fajlNev)
    {
        if (File.Exists(fajlNev))
        {
            try
            {
                lefoglaltHelyek = File.ReadAllLines(fajlNev)
                .Select(sor =>
                {
                    string[] adatok = sor.Split(' ', '-');
                    return new Hely
                    {
                        Sor = int.Parse(adatok[0]) - 1,
                        Oszlop = int.Parse(adatok[1]) - 1,
                        Nev = adatok[2]
                    };
                }).ToList();
            }
            catch (Exception)
            {
                Console.WriteLine("Hibás formátum a fájlban. A fájl törlésre került.");
                File.Delete(fajlNev);
            }
        }
    }

    public void KiirMatrixot()
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                Console.ForegroundColor = matrix[i, j] == UresHely ? ConsoleColor.White : ConsoleColor.Green;
                Console.Write(matrix[i, j] + " ");
                Console.ResetColor();
            }
            Console.WriteLine();
        }
    }

    public string FoglaljHelyet(int sorSzam, int oszlopSzam, string nev)
    {
        if (sorSzam >= 1 && sorSzam <= SorokSzama && oszlopSzam >= 1 && oszlopSzam <= OszlopokSzama)
        {
            //sorSzam--;
            //oszlopSzam--;

            Hely ujHely;
            ujHely.Sor = sorSzam;
            ujHely.Oszlop = oszlopSzam;
            ujHely.Nev = nev;

            if (lefoglaltHelyek.Contains(ujHely))
            {
                return "Ez a hely már foglalt.";
            }

            matrix[sorSzam, oszlopSzam] = FoglaltHely;
            lefoglaltHelyek.Add(ujHely);
            MentsdLeFoglaltHelyekListajat();

            return "Hely sikeresen lefoglalva.";
        }
        else
        {
            return "Érvénytelen sor- vagy oszlopindex.";
        }
    }

    private void MentsdLeFoglaltHelyekListajat()
    {
        List<string> sorok = lefoglaltHelyek
        .Select(hely => $"{hely.Sor + 1} {hely.Oszlop + 1} - {hely.Nev}")
        .ToList();

        File.WriteAllLines("data.txt", sorok);
    }
}

class Program
{
    static Matrix matrix = new Matrix();
    static string fajlNev = "data.txt";

    static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. Helyfoglalás");
            Console.WriteLine("2. Foglalt helyek megtekintése");
            Console.WriteLine("3. Kilépés");
            ConsoleKeyInfo billentyu = Console.ReadKey(true);
            switch (billentyu.KeyChar)
            {
                case '1':
                    FoglaljHelyet();
                    break;

                case '2':
                    ListazdFoglaltHelyeket();
                    break;

                case '3':
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Érvénytelen választás. Nyomjon Entert a folytatáshoz, vagy Esc-et a kilépéshez.");
                    Console.ReadLine();
                    break;
            }
        }
    }

    static void FoglaljHelyet()
    {
        Console.Clear();
        Console.WriteLine("Helyfoglalás:");
        matrix.KiirMatrixot();
        Console.WriteLine("Nyomjon Esc-et a visszalépéshez a menübe.");
        while (true)
        {
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                break;

            Console.WriteLine("Adja meg a sor- (1-16) és az oszlopszámot (1-15) szóközvel elválasztva a hely kiválasztásához (pl.: 3 5):");
            string bemenet = Console.ReadLine();
            Console.WriteLine("Adja meg a nevét: ");
            string nev = Console.ReadLine();
            //File.AppendAllText(fajlNev, $"{bemenet} {nev}{Environment.NewLine}"); //mar nem kell

            // Rendszeres kifejezés a számok megtalálására a bemenetben
            var szamok = System.Text.RegularExpressions.Regex.Matches(bemenet, @"\d+");

            // Ha nincs pontosan két szám, folytatjuk a ciklust
            if (szamok.Count != 2)
            {
                Console.WriteLine("Érvénytelen bemenet. Kérem, adjon meg érvényes sor- és oszlopszámot.");
                continue;
            }

            // Számok kinyerése
            int sorSzam = int.Parse(szamok[0].Value);
            int oszlopSzam = int.Parse(szamok[1].Value);

            // Helyfoglalás
            string eredmeny = matrix.FoglaljHelyet(sorSzam, oszlopSzam, nev);
            Console.Clear(); // A konzol törlése a frissített mátrix megjelenítéséhez
            matrix.KiirMatrixot();
            Console.WriteLine(eredmeny);
        }
    }

    static void ListazdFoglaltHelyeket()
    {
        Console.Clear();
        Console.WriteLine("Fájlban lévő adatok:");
        if (File.Exists(fajlNev))
        {
            string[] sorok = File.ReadAllLines(fajlNev);
            foreach (var sor in sorok)
            {
                Console.WriteLine(sor);
            }
        }
        else
        {
            Console.WriteLine("A fájl nem létezik.");
        }
        Console.WriteLine("Nyomjon Entert a visszalépéshez a menübe.");
        Console.ReadLine();
    }
}
