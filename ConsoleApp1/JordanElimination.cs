namespace ConsoleApp1;

static class JordanElimination
{
    public static void Eliminate(double[,] matrix, int pivotRow, int pivotCol,
        int rowCount, int colStart, int colEnd)
    {
        double pivot = matrix[pivotRow, pivotCol];

        double[] savedRow = new double[colEnd - colStart];
        double[] savedCol = new double[rowCount];

        for (int j = colStart; j < colEnd; j++)
            savedRow[j - colStart] = matrix[pivotRow, j];
        for (int i = 0; i < rowCount; i++)
            savedCol[i] = matrix[i, pivotCol];

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = colStart; j < colEnd; j++)
            {
                if (i == pivotRow && j == pivotCol)
                    matrix[i, j] = 1.0 / pivot;
                else if (i == pivotRow)
                    matrix[i, j] = savedRow[j - colStart] / pivot;
                else if (j == pivotCol)
                    matrix[i, j] = -savedCol[i] / pivot;
                else
                    matrix[i, j] = matrix[i, j] - savedCol[i] * savedRow[j - colStart] / pivot;
            }
        }
    }
}
