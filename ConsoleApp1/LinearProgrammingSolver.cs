using System.Globalization;

namespace ConsoleApp1;

enum SolveMode { BasicOnly, Optimal, OptimalWithObjective, Integer }

static class LinearProgrammingSolver
{
    private static readonly CultureInfo Fmt = CultureInfo.InvariantCulture;

    public static void Solve(LpProblem problem, Protocol protocol, SolveMode mode)
    {
        protocol.WriteTitle("Постановка задачі:");
        PrintProblem(problem, protocol);

        bool wasMin = problem.Goal == ObjectiveGoal.Min;
        LpProblem working = wasMin
            ? MinToMaxConverter.Convert(problem, protocol)
            : problem;
        working.IntegerVars = problem.IntegerVars;

        SimplexTable table = ProblemBuilder.Build(working, protocol);
        protocol.WriteTitle("Вхідна симплекс-таблиця:");
        protocol.WriteTable(table);

        NullRowStatus nullStatus = NullRowEliminator.Eliminate(table, protocol);
        if (nullStatus == NullRowStatus.Inconsistent)
            return;

        BasicSolutionStatus basicStatus = BasicSolutionSolver.Solve(table, protocol);
        if (basicStatus == BasicSolutionStatus.Inconsistent)
            return;

        double[] basic = ExtractSolution(table, working.N);
        protocol.WriteSolution("Знайдено опорний розв'язок", basic);

        if (mode == SolveMode.BasicOnly)
            return;

        OptimalSolutionStatus optimalStatus = OptimalSolutionSolver.SolveMax(table, protocol);
        if (optimalStatus == OptimalSolutionStatus.Unbounded)
            return;

        double[] optimal = ExtractSolution(table, working.N);
        double zPrime = table.Matrix[table.ZRow, table.FreeCol];

        protocol.WriteSolution("Знайдено оптимальний розв'язок", optimal);

        if (mode == SolveMode.OptimalWithObjective || mode == SolveMode.Integer)
        {
            if (wasMin)
                protocol.WriteObjective("Min (Z)", -zPrime);
            else
                protocol.WriteObjective("Max (Z)", zPrime);
        }

        if (mode != SolveMode.Integer || working.IntegerVars.Count == 0)
            return;

        GomoryStatus gStatus = GomorySolver.Solve(table, working, protocol);
        if (gStatus != GomoryStatus.Found)
            return;

        double[] intOpt = ExtractSolution(table, working.N);
        double zInt = table.Matrix[table.ZRow, table.FreeCol];

        protocol.WriteSolution("Цілочисловий оптимальний розв'язок", intOpt);
        if (wasMin)
            protocol.WriteObjective("Min (Z)", -zInt);
        else
            protocol.WriteObjective("Max (Z)", zInt);
    }

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

    private static void PrintProblem(LpProblem p, Protocol protocol)
    {
        string goal = p.Goal == ObjectiveGoal.Max ? "max" : "min";
        protocol.WriteLine("Z = " + FormatLinear(p.C) + " -> " + goal);
        protocol.WriteLine("при обмеженнях:");
        foreach (Constraint c in p.Constraints)
        {
            string op = c.Type switch
            {
                ConstraintType.LessOrEqual => "<=",
                ConstraintType.GreaterOrEqual => ">=",
                _ => "="
            };
            protocol.WriteLine(FormatLinear(c.A) + " " + op + " " + c.B.ToString("F2", Fmt));
        }

        protocol.WriteLine($"x[j] >= 0, j = 1..{p.N}");

        if (p.IntegerVars.Count > 0)
        {
            string ints = string.Join(", ", p.IntegerVars.Select(i => "x" + i));
            protocol.WriteLine("Цілі числа: " + ints);
        }
    }

    private static string FormatLinear(double[] c)
    {
        string s = "";
        bool first = true;
        for (int j = 0; j < c.Length; j++)
        {
            double v = c[j];
            if (Math.Abs(v) < 1e-12) continue;
            string sign = v < 0 ? "-" : (first ? "" : "+");
            double abs = Math.Abs(v);
            string coeff = Math.Abs(abs - 1) < 1e-12 ? "" : abs.ToString("F2", Fmt);
            s += (first ? sign : " " + sign + " ") + coeff + "x" + (j + 1);
            first = false;
        }
        return s.Length == 0 ? "0" : s;
    }
}
