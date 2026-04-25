namespace ConsoleApp1;

class SimplexTable
{
    public double[,] Matrix { get; private set; }
    public string[] TopLabels { get; private set; }
    public string[] SideLabels { get; private set; }
    public int Rows { get; private set; }
    public int Cols { get; private set; }

    public SimplexTable(int rows, int cols)
    {
        Rows = rows;
        Cols = cols;
        Matrix = new double[rows + 1, cols + 1];
        TopLabels = new string[cols];
        SideLabels = new string[rows];
    }

    public int ZRow => Rows;
    public int FreeCol => Cols;

    public void SwapLabels(int pivotRow, int pivotCol)
    {
        string rowName = SideLabels[pivotRow];
        string colName = StripMinus(TopLabels[pivotCol]);
        SideLabels[pivotRow] = colName;
        TopLabels[pivotCol] = "-" + rowName;
    }

    public void RemoveRow(int row)
    {
        int newRows = Rows - 1;
        double[,] m = new double[newRows + 1, Cols + 1];
        string[] side = new string[newRows];

        int dest = 0;
        for (int i = 0; i < Rows; i++)
        {
            if (i == row) continue;
            for (int j = 0; j <= Cols; j++)
                m[dest, j] = Matrix[i, j];
            side[dest] = SideLabels[i];
            dest++;
        }
        for (int j = 0; j <= Cols; j++)
            m[newRows, j] = Matrix[Rows, j];

        Matrix = m;
        SideLabels = side;
        Rows = newRows;
    }

    public void AddRow(string label, double[] coefs, double freeTerm)
    {
        int newRows = Rows + 1;
        double[,] m = new double[newRows + 1, Cols + 1];
        string[] side = new string[newRows];

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j <= Cols; j++)
                m[i, j] = Matrix[i, j];
            side[i] = SideLabels[i];
        }

        for (int j = 0; j < Cols; j++)
            m[Rows, j] = coefs[j];
        m[Rows, Cols] = freeTerm;
        side[Rows] = label;

        for (int j = 0; j <= Cols; j++)
            m[newRows, j] = Matrix[Rows, j];

        Matrix = m;
        SideLabels = side;
        Rows = newRows;
    }

    public void RemoveColumn(int col)
    {
        int newCols = Cols - 1;
        double[,] m = new double[Rows + 1, newCols + 1];
        string[] top = new string[newCols];

        for (int i = 0; i <= Rows; i++)
        {
            int dest = 0;
            for (int j = 0; j < Cols; j++)
            {
                if (j == col) continue;
                m[i, dest] = Matrix[i, j];
                dest++;
            }
            m[i, newCols] = Matrix[i, Cols];
        }

        int d = 0;
        for (int j = 0; j < Cols; j++)
        {
            if (j == col) continue;
            top[d++] = TopLabels[j];
        }

        Matrix = m;
        TopLabels = top;
        Cols = newCols;
    }

    private static string StripMinus(string label) =>
        label.StartsWith('-') ? label.Substring(1) : label;
}
