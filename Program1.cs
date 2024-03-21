using System;
using System.Collections.Generic;
using System.IO;

struct Hely : IEquatable<Hely>
{
    public int sor;
    public int oszlop;
    public string name;

    public bool Equals(Hely other)
    {
        return sor == other.sor && oszlop == other.oszlop;
    }
}

public class Matrix
{
    private const int Rows = 16;
    private const int Columns = 15;
    private const char InitialValue = 'O';
    private const char SelectedValue = 'X';

    private char[,] matrix;
    private List<Hely> list;

    public Matrix()
    {
        matrix = new char[Rows, Columns];
        list = new List<Hely>();
        FillMatrix(InitialValue);

        // Olvassuk be az előzőleg foglalt helyeket a fájlból
        ReadReservedSeatsFromFile("data.txt");
    }

    private void FillMatrix(char value)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                matrix[i, j] = value;
            }
        }
    }

    private void ReadReservedSeatsFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(' ', '-');

                if (parts.Length < 3)
                {
                    Console.WriteLine("Hibás formátum a fájlban.");
                    continue;
                }

                int row = int.Parse(parts[0]);
                int column = int.Parse(parts[1]);
                string name = parts[2];
                SelectSeat(row, column, name);
            }
        }
    }

    public void PrintMatrix()
    {
        // Print a line of dashes
        Console.WriteLine(new string('-', Columns));

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                Console.ForegroundColor = matrix[i, j] == InitialValue ? ConsoleColor.White : ConsoleColor.Green;
                Console.Write(matrix[i, j] + " ");
                Console.ResetColor();
            }
            Console.WriteLine();
        }
    }

    public string SelectSeat(int row, int column, string name)
    {
        if (row >= 1 && row <= Rows && column >= 1 && column <= Columns)
        {
            row--;
            column--;

            Hely selectedSeat;
            selectedSeat.sor = row;
            selectedSeat.oszlop = column;
            selectedSeat.name = name;

            if (list.Contains(selectedSeat))
            {
                return "This seat is already taken.";
            }

            matrix[row, column] = SelectedValue;
            list.Add(selectedSeat);

            return "Seat successfully reserved.";
        }
        else
        {
            return "Invalid row or column number.";
        }
    }

}

class Program
{
    static Matrix matrix = new Matrix();
    static string filePath = "data.txt";

    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. Hely foglalás");
            Console.WriteLine("2. Lefoglalt helyek megtekintése");
            Console.WriteLine("3. Kilépés");
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.KeyChar)
            {
                case '1':
                    ReserveSeat();
                    break;

                case '2':
                    ViewReservedSeats();
                    break;

                case '3':
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Érvénytelen választás. Nyomjon Enter-t a folytatáshoz.");
                    Console.ReadLine();
                    break;
            }
        }
    }

    static void ReserveSeat()
    {
        Console.Clear();
        Console.WriteLine("Hely foglalás:");
        matrix.PrintMatrix();
        Console.WriteLine("Nyomjon Esc-et a visszalépéshez a menübe.");
        while (true)
        {
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                break;

            Console.WriteLine("Enter row number (1-16) and column number (1-15) separated by space to select a seat (e.g., 3 5):");
            string input = Console.ReadLine();
            Console.WriteLine("Adja meg a nevét: ");
            string name = Console.ReadLine();
            File.AppendAllText(filePath, $"{input} {name}{Environment.NewLine}");

            // Use regular expressions to find all numbers in the input
            var matches = System.Text.RegularExpressions.Regex.Matches(input, @"\d+");

            // If there are not exactly two numbers, continue with the next iteration
            if (matches.Count != 2)
            {
                Console.WriteLine("Invalid input. Please enter valid row and column numbers.");
                continue;
            }

            // Parse the numbers
            int row = int.Parse(matches[0].Value);
            int column = int.Parse(matches[1].Value);

            // Select the seat
            string result = matrix.SelectSeat(row, column, name);
            Console.WriteLine(result);
            Console.Clear(); // Clear console to redraw matrix with updated selection
            matrix.PrintMatrix();
        }
    }

    static void ViewReservedSeats()
    {
        Console.Clear();
        Console.WriteLine("Fájlban lévő adatok:");
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }
        else
        {
            Console.WriteLine("A fájl nem létezik.");
        }
        Console.WriteLine("Nyomjon Enter-t a visszalépéshez a menübe.");
        Console.ReadLine();
    }
}
