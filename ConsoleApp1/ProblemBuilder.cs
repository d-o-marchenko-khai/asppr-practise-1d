using System.Globalization;

namespace ConsoleApp1;

enum ConstraintType { LessOrEqual, GreaterOrEqual, Equal }
enum ObjectiveGoal { Max, Min }

class Constraint
{
    public double[] A { get; set; } = Array.Empty<double>();
    public ConstraintType Type { get; set; }
    public double B { get; set; }
}

class LpProblem
{
    public int N { get; set; }
    public double[] C { get; set; } = Array.Empty<double>();
    public List<Constraint> Constraints { get; set; } = new();
    public ObjectiveGoal Goal { get; set; }
    public List<int> IntegerVars { get; set; } = new();
}

static class ProblemBuilder
{
    private static readonly CultureInfo Fmt = CultureInfo.InvariantCulture;

    public static SimplexTable Build(LpProblem p, Protocol protocol)
    {
        int n = p.N;
        int m = p.Constraints.Count;

        double[,] normA = new double[m, n];
        double[] normB = new double[m];
        bool[] isEquality = new bool[m];

        for (int i = 0; i < m; i++)
        {
            Constraint c = p.Constraints[i];
            isEquality[i] = c.Type == ConstraintType.Equal;

            double sign = c.Type == ConstraintType.LessOrEqual ? -1.0 : 1.0;
            for (int j = 0; j < n; j++)
                normA[i, j] = sign * c.A[j];
            normB[i] = -sign * c.B;

            if (isEquality[i] && normB[i] < 0)
            {
                for (int j = 0; j < n; j++)
                    normA[i, j] = -normA[i, j];
                normB[i] = -normB[i];
            }
        }

        protocol.WriteTitle("Перепишемо систему обмежень:");
        for (int i = 0; i < m; i++)
        {
            string line = "";
            for (int j = 0; j < n; j++)
            {
                string term = FormatCoeff(normA[i, j]) + " * X[" + (j + 1) + "]";
                line += (j == 0 ? term : " + " + term);
            }
            string op = isEquality[i] ? "= 0" : ">= 0";
            line += " + " + FormatCoeff(normB[i]) + " " + op;
            protocol.WriteLine(line);
        }

        SimplexTable table = new SimplexTable(m, n);
        for (int j = 0; j < n; j++)
            table.TopLabels[j] = "-x" + (j + 1);

        int yCount = 0;
        for (int i = 0; i < m; i++)
            table.SideLabels[i] = isEquality[i] ? "0" : "y" + (++yCount);

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
                table.Matrix[i, j] = -normA[i, j];
            table.Matrix[i, table.FreeCol] = normB[i];
        }

        for (int j = 0; j < n; j++)
            table.Matrix[table.ZRow, j] = -p.C[j];
        table.Matrix[table.ZRow, table.FreeCol] = 0.0;

        return table;
    }

    private static string FormatCoeff(double v)
    {
        string formatted = v.ToString("F2", Fmt);
        return v < 0 ? "(" + formatted + ")" : formatted;
    }
}
