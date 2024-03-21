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
    public const int SorokSzama = 16;
    public const int OszlopokSzama = 15;
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
        // Reset the matrix to empty
        ToltseKiMatrixot(UresHely);

        // Mark the taken seats in the matrix
        foreach (var hely in lefoglaltHelyek)
        {
            matrix[hely.Sor, hely.Oszlop] = FoglaltHely;
        }

        // Print column numbers
        Console.Write("     "); // For alignment
        for (int j = 1; j <= matrix.GetLength(1); j++)
        {
            Console.Write(j.ToString().PadLeft(3));
        }
        Console.WriteLine();

        // Print line for separation
        Console.WriteLine("              " + new string('-', matrix.GetLength(1) * 2 - 2));

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            // Print row number
            Console.Write((i + 1).ToString().PadLeft(2) + "   ");

            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                Console.ForegroundColor = matrix[i, j] == UresHely ? ConsoleColor.White : ConsoleColor.Green;
                Console.Write(matrix[i, j].ToString().PadLeft(3));
                Console.ResetColor();
            }
            Console.WriteLine();
        }
    }

    public string FoglaljHelyet(int sorSzam, int oszlopSzam, string nev)
    {
        if (sorSzam >= 1 && sorSzam <= SorokSzama && oszlopSzam >= 1 && oszlopSzam <= OszlopokSzama)
        {
            sorSzam--;
            oszlopSzam--;

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

    public string ToroljHelyet(int sorSzam, int oszlopSzam)
    {
        if (sorSzam >= 1 && sorSzam <= SorokSzama && oszlopSzam >= 1 && oszlopSzam <= OszlopokSzama)
        {
            sorSzam--;
            oszlopSzam--;

            Hely torlendoHely = new Hely();
            torlendoHely.Sor = sorSzam;
            torlendoHely.Oszlop = oszlopSzam;

            if (!lefoglaltHelyek.Contains(torlendoHely))
            {
                return "Ez a hely nincs foglalva.";
            }

            matrix[sorSzam, oszlopSzam] = UresHely;
            lefoglaltHelyek.Remove(torlendoHely);
            MentsdLeFoglaltHelyekListajat();

            return "Helyfoglalás sikeresen törölve.";
        }
        else
        {
            return "Érvénytelen sor- vagy oszlopindex.";
        }
    }

    public string ToroljHelyet(string nev)
    {
        var torlendoHelyek = lefoglaltHelyek.Where(hely => hely.Nev == nev).ToList();
        if (torlendoHelyek.Count == 0)
        {
            return "Nincs ilyen névvel foglalt hely.";
        }

        foreach (var hely in torlendoHelyek)
        {
            matrix[hely.Sor, hely.Oszlop] = UresHely;
            lefoglaltHelyek.Remove(hely);
        }

        MentsdLeFoglaltHelyekListajat();

        return "Helyfoglalások sikeresen törölve.";
    }

    public bool FoglaltEezAHely(int sorSzam, int oszlopSzam)
    {
        return lefoglaltHelyek.Contains(new Hely { Sor = sorSzam - 1, Oszlop = oszlopSzam - 1 });
    }

    public bool FoglaltEezAHely(int sorSzam, int oszlopSzam, string nev)
    {
        // Ahol a nev egyezik és a hely foglalt
        return lefoglaltHelyek.Contains(new Hely { Sor = sorSzam - 1, Oszlop = oszlopSzam - 1, Nev = nev });
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
            Console.WriteLine("2. Helytörlés");
            Console.WriteLine("3. Foglalt helyek megtekintése");
            Console.WriteLine("4. Kilépés");
            ConsoleKeyInfo billentyu = Console.ReadKey(true);
            switch (billentyu.KeyChar)
            {
                case '1':
                    FoglaljHelyet();
                    break;

                case '2':
                    ToroljHelyet();
                    break;

                case '3':
                    ListazdFoglaltHelyeket();
                    break;

                case '4':
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
        Console.WriteLine("Nyomjon Entert a bemeneti módhoz, vagy Space-t a navigációs módhoz. Nyomjon Esc-et a visszalépéshez a menübe.");
        
        switch (Console.ReadKey(true).Key)
        {
            case ConsoleKey.Escape:
                return;

            case ConsoleKey.Enter:
                // bemenet mód
                while (true)
                {
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        break;

                    Console.WriteLine("Adja meg a sor- (1-16) és az oszlopszámot (1-15) szóközvel elválasztva a hely kiválasztásához (pl.: 3 5):");
                    string bemenet = Console.ReadLine();
                    Console.WriteLine("Adja meg a nevét: ");
                    string nev = Console.ReadLine();

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
                break;
            case ConsoleKey.Spacebar:
                // Navigational mode
                int currentRow = 0;
                int currentColumn = 0;

                bool exitted = false;

                Console.WriteLine("Adja meg a nevét: ");
                string _nev = Console.ReadLine();

                while (true)
                {
                    Console.Clear();
                    matrix.KiirMatrixot();
                    Console.SetCursorPosition(currentColumn * 3  + 6, currentRow + 2); // Adjust these values as needed

                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            if (currentRow > 0) currentRow--;
                            break;
                        case ConsoleKey.DownArrow:
                            if (currentRow < Matrix.SorokSzama - 1) currentRow++;
                            break;
                        case ConsoleKey.LeftArrow:
                            if (currentColumn > 0) currentColumn--;
                            break;
                        case ConsoleKey.RightArrow:
                            if (currentColumn < Matrix.OszlopokSzama - 1) currentColumn++;
                            break;
                        case ConsoleKey.Enter:
                            // Ha a hely már foglalt nem csinálunk semmit
                            // Ha a hely már foglalt de a saját nevünkön van akkor törölhetjük
                            // Ha a hely üres akkor foglalunk
                            // Ha a hely foglalt más néven nem csinálunk semmit

                            if (matrix.FoglaltEezAHely(currentRow + 1, currentColumn + 1, _nev))
                            {
                                matrix.ToroljHelyet(currentRow + 1, currentColumn + 1);
                            }

                            if (!matrix.FoglaltEezAHely(currentRow + 1, currentColumn + 1))
                            {
                                matrix.FoglaljHelyet(currentRow + 1, currentColumn + 1, _nev);
                            }

                            exitted = false;
                            break;
                        case ConsoleKey.Escape:
                            // Exit navigation mode
                            exitted = true;
                            break;
                    }
                if (exitted)
                    {
                        return;
                    }
                }
        }
    }

    static void ToroljHelyet()
    {
        Console.Clear();
        Console.WriteLine("1. Helyfoglalás törlése név alapján");
        Console.WriteLine("2. Helyfoglalás törlése sor- és oszlopszám alapján");
        string option = Console.ReadLine();

        matrix.KiirMatrixot();

        switch (option)
        {
            case "1":
                Console.WriteLine("Add meg a nevet:");
                string name = Console.ReadLine();

                matrix.ToroljHelyet(name);

                break;
            case "2":
                Console.WriteLine("Add meg a sor- és oszlopszámot:");
                string[] input = Console.ReadLine().Split(' ');
                int row = int.Parse(input[0]);
                int column = int.Parse(input[1]);

                matrix.ToroljHelyet(row, column);

                break;
            default:
                Console.WriteLine("Invalid option");
                break;
        }
    }

    static void ListazdFoglaltHelyeket()
    {
        Console.Clear();

        matrix.KiirMatrixot();
        Console.WriteLine();

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
