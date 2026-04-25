namespace ConsoleApp1;

enum NullRowStatus { Done, Inconsistent }

static class NullRowEliminator
{
    private const double Eps = 1e-9;

    public static NullRowStatus Eliminate(SimplexTable table, Protocol protocol)
    {
        bool hasAny = HasZeroRow(table);
        if (!hasAny) return NullRowStatus.Done;

        protocol.WriteTitle("Видалення нуль-рядків:");

        while (true)
        {
            int zeroRow = FindZeroRow(table);
            if (zeroRow < 0)
            {
                protocol.WriteLine("Всі нуль-рядки видалено.");
                return NullRowStatus.Done;
            }

            int pivotCol = FindPositiveInRow(table, zeroRow);
            if (pivotCol < 0)
            {
                double b = table.Matrix[zeroRow, table.FreeCol];
                if (Math.Abs(b) < Eps)
                {
                    table.RemoveRow(zeroRow);
                    protocol.WriteLine("Нуль-рядок вироджений — видалено.");
                    protocol.WriteTable(table);
                    continue;
                }
                protocol.WriteLine("Система обмежень є суперечливою.");
                return NullRowStatus.Inconsistent;
            }

            int pivotRow = SelectPivotRow(table, pivotCol);
            if (pivotRow < 0)
            {
                protocol.WriteLine("Не вдалося обрати розв'язувальний рядок.");
                return NullRowStatus.Inconsistent;
            }

            protocol.WritePivot(table.SideLabels[pivotRow], table.TopLabels[pivotCol]);

            bool pivotWasZeroRow = table.SideLabels[pivotRow] == "0";

            JordanElimination.Eliminate(table.Matrix, pivotRow, pivotCol,
                table.Rows + 1, 0, table.Cols + 1);
            table.SwapLabels(pivotRow, pivotCol);

            if (pivotWasZeroRow)
                table.RemoveColumn(pivotCol);

            protocol.WriteTable(table);
        }
    }

    private static bool HasZeroRow(SimplexTable table)
    {
        for (int i = 0; i < table.Rows; i++)
            if (table.SideLabels[i] == "0") return true;
        return false;
    }

    private static int FindZeroRow(SimplexTable table)
    {
        for (int i = 0; i < table.Rows; i++)
            if (table.SideLabels[i] == "0") return i;
        return -1;
    }

    private static int FindPositiveInRow(SimplexTable table, int row)
    {
        for (int j = 0; j < table.Cols; j++)
            if (table.Matrix[row, j] > Eps) return j;
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
