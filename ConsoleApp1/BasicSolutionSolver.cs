namespace ConsoleApp1;

enum BasicSolutionStatus { Found, Inconsistent }

static class BasicSolutionSolver
{
    private const double Eps = 1e-9;

    public static BasicSolutionStatus Solve(SimplexTable table, Protocol protocol)
    {
        protocol.WriteTitle("Пошук опорного розв'язку:");

        while (true)
        {
            int pivotRow = FindNegativeFreeTerm(table);
            if (pivotRow < 0)
            {
                protocol.WriteLine("Усі вільні члени невід'ємні — опорний розв'язок знайдено.");
                return BasicSolutionStatus.Found;
            }

            int pivotCol = FindNegativeInRow(table, pivotRow);
            if (pivotCol < 0)
            {
                protocol.WriteLine("Система обмежень є суперечливою.");
                return BasicSolutionStatus.Inconsistent;
            }

            int selectedRow = SelectPivotRow(table, pivotCol);
            if (selectedRow < 0)
            {
                protocol.WriteLine("Не вдалося обрати розв'язувальний рядок.");
                return BasicSolutionStatus.Inconsistent;
            }

            protocol.WritePivot(table.SideLabels[selectedRow], table.TopLabels[pivotCol]);
            JordanElimination.Eliminate(table.Matrix, selectedRow, pivotCol,
                table.Rows + 1, 0, table.Cols + 1);
            table.SwapLabels(selectedRow, pivotCol);
            protocol.WriteTable(table);
        }
    }

    private static int FindNegativeFreeTerm(SimplexTable table)
    {
        for (int i = 0; i < table.Rows; i++)
            if (table.Matrix[i, table.FreeCol] < -Eps)
                return i;
        return -1;
    }

    private static int FindNegativeInRow(SimplexTable table, int row)
    {
        for (int j = 0; j < table.Cols; j++)
            if (table.Matrix[row, j] < -Eps)
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
            double b = table.Matrix[i, table.FreeCol];

            bool sameSign = (a > Eps && b >= -Eps) || (a < -Eps && b <= Eps);
            if (!sameSign) continue;
            if (Math.Abs(a) < Eps) continue;

            double ratio = b / a;
            if (ratio < -Eps) continue;
            if (ratio < bestRatio - Eps)
            {
                bestRatio = ratio;
                best = i;
            }
        }
        return best;
    }
}
