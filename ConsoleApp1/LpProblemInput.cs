using System.Globalization;

namespace ConsoleApp1;

static class LpProblemInput
{
    public static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine()?.Trim(), out int v) && v > 0)
                return v;
            Console.WriteLine("Очікується додатне ціле число.");
        }
    }

    public static LpProblem Read()
    {
        int n = ReadInt("Введіть кількість змінних n: ");
        int m = ReadInt("Введіть кількість обмежень m: ");

        ObjectiveGoal goal = ReadGoal();

        Console.WriteLine($"Введіть {n} коефіцієнтів функції мети Z (через пробіл):");
        double[] c = ReadRow(n);

        List<Constraint> constraints = new();
        Console.WriteLine("Введіть обмеження у форматі: a1 a2 ... an  <=|>=|=  b");
        for (int i = 0; i < m; i++)
        {
            Console.Write($"  Обмеження {i + 1}: ");
            string line = Console.ReadLine()?.Trim() ?? "";
            constraints.Add(ParseConstraint(line, n));
        }

        List<int> integerVars = ReadIntegerVars(n);

        return new LpProblem
        {
            N = n,
            C = c,
            Constraints = constraints,
            Goal = goal,
            IntegerVars = integerVars
        };
    }

    private static List<int> ReadIntegerVars(int n)
    {
        Console.WriteLine($"Введіть номери цілочислових змінних через пробіл (1..{n}), або порожньо якщо немає, або 'all' для всіх:");
        string line = Console.ReadLine()?.Trim() ?? "";
        if (line.Length == 0) return new List<int>();
        if (line.Equals("all", StringComparison.OrdinalIgnoreCase))
            return Enumerable.Range(1, n).ToList();

        string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        List<int> result = new();
        foreach (string p in parts)
        {
            if (!int.TryParse(p, out int idx) || idx < 1 || idx > n)
                throw new FormatException($"Некоректний індекс змінної: {p}");
            if (!result.Contains(idx)) result.Add(idx);
        }
        result.Sort();
        return result;
    }

    private static ObjectiveGoal ReadGoal()
    {
        while (true)
        {
            Console.Write("Напрямок оптимізації (max / min): ");
            string s = Console.ReadLine()?.Trim().ToLowerInvariant() ?? "";
            if (s == "max") return ObjectiveGoal.Max;
            if (s == "min") return ObjectiveGoal.Min;
            Console.WriteLine("Очікується max або min.");
        }
    }

    private static double[] ReadRow(int n)
    {
        while (true)
        {
            string[] parts = (Console.ReadLine() ?? "").Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != n)
            {
                Console.WriteLine($"Очікується {n} чисел. Повторіть ввід:");
                continue;
            }
            double[] row = new double[n];
            bool ok = true;
            for (int j = 0; j < n; j++)
            {
                if (!TryParse(parts[j], out row[j])) { ok = false; break; }
            }
            if (!ok) { Console.WriteLine("Некоректне число. Повторіть ввід:"); continue; }
            return row;
        }
    }

    private static Constraint ParseConstraint(string line, int n)
    {
        string[] tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length != n + 2)
            throw new FormatException(
                $"Очікується {n} коефіцієнтів + знак + вільний член (всього {n + 2} токенів).");

        double[] a = new double[n];
        for (int j = 0; j < n; j++)
            a[j] = ParseStrict(tokens[j]);

        ConstraintType type = tokens[n] switch
        {
            "<=" => ConstraintType.LessOrEqual,
            ">=" => ConstraintType.GreaterOrEqual,
            "=" => ConstraintType.Equal,
            _ => throw new FormatException("Очікується знак <=, >= або =.")
        };

        double b = ParseStrict(tokens[n + 1]);
        return new Constraint { A = a, Type = type, B = b };
    }

    private static bool TryParse(string s, out double v) =>
        double.TryParse(s.Replace(',', '.'), NumberStyles.Float,
            CultureInfo.InvariantCulture, out v);

    private static double ParseStrict(string s)
    {
        if (!TryParse(s, out double v))
            throw new FormatException($"Некоректне число: {s}");
        return v;
    }
}
