using System.Globalization;

namespace ConsoleApp1;

enum GomoryStatus { Found, Inconsistent, Unbounded, IterationLimit }

static class GomorySolver
{
    private const double Eps = 1e-7;
    private const int MaxIterations = 50;
    private static readonly CultureInfo Fmt = CultureInfo.InvariantCulture;

    public static GomoryStatus Solve(SimplexTable table, LpProblem problem, Protocol protocol)
    {
        protocol.WriteTitle("Пошук оптимального цілочислового розв'язку (метод Гоморі):");

        int cutCount = 0;
        for (int iter = 0; iter < MaxIterations; iter++)
        {
            int row = SelectFractionalRow(table, problem);
            if (row < 0)
            {
                protocol.WriteLine("Усі цілочислові змінні мають цілі значення — задачу розв'язано.");
                return GomoryStatus.Found;
            }

            string label = table.SideLabels[row];
            double value = table.Matrix[row, table.FreeCol];
            protocol.WriteLine(
                "Знайдено розв'язок, у якому змінні мають дробову частину, " +
                $"максимальна дробова частина змінної: {label} = {value.ToString("F2", Fmt)}");

            cutCount++;
            AddGomoryCut(table, row, cutCount, protocol);

            BasicSolutionStatus basicStatus = BasicSolutionSolver.Solve(table, protocol);
            if (basicStatus == BasicSolutionStatus.Inconsistent)
            {
                protocol.WriteLine("Цілочислового розв'язку не існує.");
                return GomoryStatus.Inconsistent;
            }

            double[] basic = ExtractSolution(table, problem.N);
            protocol.WriteSolution("Знайдено опорний розв'язок", basic);

            OptimalSolutionStatus optStatus = OptimalSolutionSolver.SolveMax(table, protocol);
            if (optStatus == OptimalSolutionStatus.Unbounded)
                return GomoryStatus.Unbounded;

            double[] opt = ExtractSolution(table, problem.N);
            protocol.WriteSolution("Знайдено оптимальний розв'язок", opt);
        }

        protocol.WriteLine("Перевищено максимальну кількість ітерацій методу Гоморі.");
        return GomoryStatus.IterationLimit;
    }

    private static int SelectFractionalRow(SimplexTable table, LpProblem problem)
    {
        int bestRow = -1;
        double bestFrac = 0.0;
        for (int i = 0; i < table.Rows; i++)
        {
            string label = table.SideLabels[i];
            if (!label.StartsWith('x')) continue;
            if (!int.TryParse(label.Substring(1), out int idx)) continue;
            if (!problem.IntegerVars.Contains(idx)) continue;

            double v = table.Matrix[i, table.FreeCol];
            double f = FracPart(v);
            if (f <= Eps) continue;
            if (f > bestFrac + Eps)
            {
                bestFrac = f;
                bestRow = i;
            }
        }
        return bestRow;
    }

    private static void AddGomoryCut(SimplexTable table, int sourceRow, int cutNumber, Protocol protocol)
    {
        int cols = table.Cols;
        double[] cutCoefs = new double[cols];
        double[] fracCoefs = new double[cols];

        for (int j = 0; j < cols; j++)
        {
            double f = FracPart(table.Matrix[sourceRow, j]);
            fracCoefs[j] = f;
            cutCoefs[j] = -f;
        }
        double bFrac = FracPart(table.Matrix[sourceRow, table.FreeCol]);
        double cutFree = -bFrac;

        protocol.WriteTitle("Складено додаткове обмеження:");
        string expr = "";
        for (int j = 0; j < cols; j++)
        {
            string baseName = StripMinus(table.TopLabels[j]);
            string coeff = fracCoefs[j].ToString("F2", Fmt);
            string term = coeff + " * " + baseName;
            expr += j == 0 ? term : " + " + term;
        }
        string freeFmt = bFrac > Eps
            ? "(" + (-bFrac).ToString("F2", Fmt) + ")"
            : (0.0).ToString("F2", Fmt);
        protocol.WriteLine($"s{cutNumber} = " + expr + " + " + freeFmt + " >= 0");

        table.AddRow("s" + cutNumber, cutCoefs, cutFree);

        protocol.WriteTitle("Симплекс-таблиця з новим обмеженням:");
        protocol.WriteTable(table);
    }

    private static double FracPart(double v)
    {
        double f = v - Math.Floor(v);
        if (f > 1.0 - Eps) f = 0.0;
        if (f < Eps) f = 0.0;
        return f;
    }

    private static string StripMinus(string label) =>
        label.StartsWith('-') ? label.Substring(1) : label;

    private static double[] ExtractSolution(SimplexTable table, int n)
    {
        double[] x = new double[n];
        for (int i = 0; i < table.Rows; i++)
        {
            string label = table.SideLabels[i];
            if (label.StartsWith('x') &&
                int.TryParse(label.Substring(1), out int idx) &&
                idx >= 1 && idx <= n)
            {
                x[idx - 1] = table.Matrix[i, table.FreeCol];
            }
        }
        return x;
    }
}
