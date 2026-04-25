namespace ConsoleApp1;

enum OptimalSolutionStatus { Found, Unbounded }

static class OptimalSolutionSolver
{
    private const double Eps = 1e-9;

    public static OptimalSolutionStatus SolveMax(SimplexTable table, Protocol protocol)
    {
        protocol.WriteTitle("Пошук оптимального розв'язку:");

        while (true)
        {
            int pivotCol = FindNegativeInZ(table);
            if (pivotCol < 0)
            {
                protocol.WriteLine("У Z-рядку немає від'ємних елементів — оптимум знайдено.");
                return OptimalSolutionStatus.Found;
            }

            int pivotRow = SelectPivotRow(table, pivotCol);
            if (pivotRow < 0)
            {
                protocol.WriteLine("Функція мети Z не обмежена зверху.");
                return OptimalSolutionStatus.Unbounded;
            }

            protocol.WritePivot(table.SideLabels[pivotRow], table.TopLabels[pivotCol]);
            JordanElimination.Eliminate(table.Matrix, pivotRow, pivotCol,
                table.Rows + 1, 0, table.Cols + 1);
            table.SwapLabels(pivotRow, pivotCol);
            protocol.WriteTable(table);
        }
    }

    private static int FindNegativeInZ(SimplexTable table)
    {
        for (int j = 0; j < table.Cols; j++)
            if (table.Matrix[table.ZRow, j] < -Eps)
                return j;
        return -1;
    }

    private static int SelectPivotRow(SimplexTable table, int pivotCol)
    {
        int best = -1;
        double bestRatio = double.PositiveInfinity;

        for (int i = 0; i < table.Rows; i++)
        {
            double a = table.Matrix[i, pivotCol];
            if (a <= Eps) continue;

            double b = table.Matrix[i, table.FreeCol];
            if (b < -Eps) continue;

            double ratio = b / a;
            if (ratio < bestRatio - Eps)
            {
                bestRatio = ratio;
                best = i;
            }
        }
        return best;
    }
}
