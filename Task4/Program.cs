using System;
using System.Collections.Generic;
using System.IO;

public class Student
{
    public int Id { get; }
    public string FullName { get; }
    public int Score { get; }

    public Student(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70) return "B";
        if (Score >= 60) return "C";
        if (Score >= 50) return "D";
        return "F";
    }
}

public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var students = new List<Student>();

        using (var reader = new StreamReader(inputFilePath))
        {
            string? line;
            int lineNumber = 0;

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                var parts = line.Split(',');

                if (parts.Length != 3)
                    throw new MissingFieldException($"Line {lineNumber}: Missing fields. Expected 3 values.");

                if (!int.TryParse(parts[0], out int id))
                    throw new FormatException($"Line {lineNumber}: Invalid ID format.");

                string fullName = parts[1].Trim();

                if (!int.TryParse(parts[2], out int score))
                    throw new InvalidScoreFormatException($"Line {lineNumber}: Score is not a valid integer.");

                students.Add(new Student(id, fullName, score));
            }
        }

        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (var writer = new StreamWriter(outputFilePath))
        {
            foreach (var student in students)
            {
                writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
            }
        }
    }
}

public static class Program
{
    public static void Main()
    {
        var processor = new StudentResultProcessor();

        Console.Write("Enter input file path: ");
        string inputPath = Console.ReadLine() ?? "";

        Console.Write("Enter output file path: ");
        string outputPath = Console.ReadLine() ?? "";

        try
        {
            var students = processor.ReadStudentsFromFile(inputPath);
            processor.WriteReportToFile(students, outputPath);
            Console.WriteLine("Report generated successfully.");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Error: The input file was not found.");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }
}

