namespace ConsoleApp1;

static class TestRunner
{
    public static void RunAll()
    {
        RunCase("ВАРІАНТ: Z = x1 + x3 + x6 -> max (всі змінні цілі)", Variant());
        RunCase("ПРИКЛАД 1: Z = x1 + 4x2 -> max (x1, x2 - цілі)", Example1());
        RunCase("ПРИКЛАД 2: Z = 4x1 + 5x2 + x3 -> max (x1, x2, x3 - цілі)", Example2());
    }

    private static void RunCase(string title, LpProblem p)
    {
        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine("  " + title);
        Console.WriteLine("========================================");
        LinearProgrammingSolver.Solve(p, new Protocol(), SolveMode.Integer);
    }

    private static LpProblem Variant() => new LpProblem
    {
        N = 6,
        C = new double[] { 1, 0, 1, 0, 0, 1 },
        Goal = ObjectiveGoal.Max,
        Constraints = new List<Constraint>
        {
            new Constraint { A = new double[] { 1, 1, 1, 1, 1, 3 }, Type = ConstraintType.LessOrEqual, B = 4 },
            new Constraint { A = new double[] { 1, -4, 0, 1, 10, -1 }, Type = ConstraintType.LessOrEqual, B = 5 },
            new Constraint { A = new double[] { 1, 3, 7, 1, 15, -1 }, Type = ConstraintType.LessOrEqual, B = 2 }
        },
        IntegerVars = new List<int> { 1, 2, 3, 4, 5, 6 }
    };

    private static LpProblem Example1() => new LpProblem
    {
        N = 2,
        C = new double[] { 1, 4 },
        Goal = ObjectiveGoal.Max,
        Constraints = new List<Constraint>
        {
            new Constraint { A = new double[] { 2, 1 }, Type = ConstraintType.LessOrEqual, B = 6 },
            new Constraint { A = new double[] { 1, 3 }, Type = ConstraintType.LessOrEqual, B = 4 }
        },
        IntegerVars = new List<int> { 1, 2 }
    };

    private static LpProblem Example2() => new LpProblem
    {
        N = 3,
        C = new double[] { 4, 5, 1 },
        Goal = ObjectiveGoal.Max,
        Constraints = new List<Constraint>
        {
            new Constraint { A = new double[] { 3, 2, 0 }, Type = ConstraintType.LessOrEqual, B = 10 },
            new Constraint { A = new double[] { 1, 4, 0 }, Type = ConstraintType.LessOrEqual, B = 11 },
            new Constraint { A = new double[] { 3, 3, 1 }, Type = ConstraintType.LessOrEqual, B = 13 }
        },
        IntegerVars = new List<int> { 1, 2, 3 }
    };
}
