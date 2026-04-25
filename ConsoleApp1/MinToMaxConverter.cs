using System.Globalization;

namespace ConsoleApp1;

static class MinToMaxConverter
{
    private static readonly CultureInfo Fmt = CultureInfo.InvariantCulture;

    public static LpProblem Convert(LpProblem p, Protocol protocol)
    {
        double[] cPrime = new double[p.N];
        for (int j = 0; j < p.N; j++)
            cPrime[j] = -p.C[j];

        protocol.WriteTitle("Перехід до задачі максимізації функції мети Z':");
        string expr = "";
        bool first = true;
        for (int j = 0; j < p.N; j++)
        {
            if (Math.Abs(cPrime[j]) < 1e-12) continue;
            string coeff = cPrime[j] < 0
                ? "(" + cPrime[j].ToString("F2", Fmt) + ")"
                : cPrime[j].ToString("F2", Fmt);
            string term = coeff + " * X[" + (j + 1) + "]";
            expr += first ? term : " + " + term;
            first = false;
        }
        protocol.WriteLine("Z' = " + expr + " -> max");

        return new LpProblem
        {
            N = p.N,
            C = cPrime,
            Constraints = p.Constraints,
            Goal = ObjectiveGoal.Max
        };
    }
}
