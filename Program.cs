using System;
using System.Collections.Generic;
using System.IO;

struct hely
{
    public int sor;
    public int oszlop;
}

public class Matrix
{
    private const int Rows = 16;
    private const int Columns = 15;
    private const char InitialValue = 'O';
    private const char SelectedValue = 'X';

    private char[,] matrix;
    private List<hely> list;

    public Matrix()
    {
        matrix = new char[Rows, Columns];
        list = new List<hely>();
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

                // Ellenőrizze, hogy van-e elég rész a konverzióhoz
                if (parts.Length < 2)
                {
                    Console.WriteLine("Hibás formátum a fájlban.");
                    continue;
                }

                int row = int.Parse(parts[0]);
                int column = int.Parse(parts[1]);
                SelectSeat(row, column);
            }
        }
    }

    public void PrintMatrix()
    {
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

    public void SelectSeat(int row, int column)
    {
        if (row >= 1 && row <= Rows && column >= 1 && column <= Columns)
        {
            // Ellenőrizzük, hogy a hely még szabad-e
            if (matrix[row - 1, column - 1] != InitialValue)
            {
                Console.WriteLine("Ez a hely már foglalt.");
                return;
            }

            // Ellenőrizzük, hogy a hely szerepel-e a listában
            hely foglaltHely;
            foglaltHely.sor = row;
            foglaltHely.oszlop = column;
            if (list.Contains(foglaltHely))
            {
                Console.WriteLine("Ez a hely már foglalt.");
                return;
            }

            // Ha a hely szabad és nincs a listában, akkor lefoglaljuk
            matrix[row - 1, column - 1] = SelectedValue;
            list.Add(foglaltHely); // Frissítjük a listát a lefoglalt helyekkel
        }
        else
        {
            throw new ArgumentOutOfRangeException("Invalid row or column number.");
        }
    }

}

class Program
{
    static void Main()
    {
        // Create the matrix
        Matrix matrix = new Matrix();

        // Show the initial matrix
        matrix.PrintMatrix();

        // Select a seat
        while (true)
        {
            Console.WriteLine("Enter row number (1-16) and column number (1-15) separated by space to select a seat (e.g., 3 5):");
            string input = Console.ReadLine();
            Console.WriteLine("Adja meg a nevét: ");
            string name = Console.ReadLine();
            File.AppendAllText("data.txt", input + " " + name + Environment.NewLine);

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
            try
            {
                matrix.SelectSeat(row, column);
                Console.Clear(); // Clear console to redraw matrix with updated selection
                matrix.PrintMatrix();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
