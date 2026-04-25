using System.Globalization;

namespace ConsoleApp1;

class Protocol
{
    private static readonly CultureInfo Fmt = CultureInfo.InvariantCulture;

    public void WriteLine(string text = "") => Console.WriteLine(text);

    public void WriteTitle(string text)
    {
        Console.WriteLine();
        Console.WriteLine(text);
    }

    public void WriteTable(SimplexTable table)
    {
        const int w = 9;

        Console.Write(new string(' ', 6));
        foreach (string label in table.TopLabels)
            Console.Write(label.PadLeft(w));
        Console.Write("1".PadLeft(w));
        Console.WriteLine();

        for (int i = 0; i < table.Rows; i++)
        {
            Console.Write((table.SideLabels[i] + " =").PadRight(6));
            for (int j = 0; j <= table.Cols; j++)
                Console.Write(FormatCell(table.Matrix[i, j]).PadLeft(w));
            Console.WriteLine();
        }

        Console.Write("Z =".PadRight(6));
        for (int j = 0; j <= table.Cols; j++)
            Console.Write(FormatCell(table.Matrix[table.Rows, j]).PadLeft(w));
        Console.WriteLine();
    }

    public void WritePivot(string rowLabel, string colLabel)
    {
        Console.WriteLine($"Розв'язувальний рядок: {rowLabel}");
        Console.WriteLine($"Розв'язувальний стовпець: {colLabel}");
    }

    public void WriteSolution(string label, double[] x)
    {
        string parts = string.Join("; ", x.Select(v => v.ToString("F2", Fmt)));
        Console.WriteLine($"{label}: X = ({parts})");
    }

    public void WriteObjective(string label, double value)
    {
        Console.WriteLine($"{label} = {value.ToString("F2", Fmt)}");
    }

    private static string FormatCell(double v)
    {
        if (Math.Abs(v) < 1e-10) v = 0.0;
        return v.ToString("F2", Fmt);
    }
}
