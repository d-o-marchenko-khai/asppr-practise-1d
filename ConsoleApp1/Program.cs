using System.Globalization;
using System.Text;
using ConsoleApp1;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== Цілочислове лінійне програмування (метод Гоморі) ===");
    Console.WriteLine("1. Пошук опорного розв'язку");
    Console.WriteLine("2. Пошук оптимального розв'язку");
    Console.WriteLine("3. Максимум (мінімум) функції мети Z");
    Console.WriteLine("4. Пошук цілочислового розв'язку (метод Гоморі)");
    Console.WriteLine("5. Тестові обчислення");
    Console.WriteLine("0. Вихід");
    Console.Write("Оберіть пункт меню: ");

    string? choice = Console.ReadLine()?.Trim();
    Console.WriteLine();

    switch (choice)
    {
        case "1":
            Run(SolveMode.BasicOnly);
            break;
        case "2":
            Run(SolveMode.Optimal);
            break;
        case "3":
            Run(SolveMode.OptimalWithObjective);
            break;
        case "4":
            Run(SolveMode.Integer);
            break;
        case "5":
            TestRunner.RunAll();
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Невірний вибір.");
            break;
    }
}

static void Run(SolveMode mode)
{
    try
    {
        LpProblem problem = LpProblemInput.Read();
        LinearProgrammingSolver.Solve(problem, new Protocol(), mode);
    }
    catch (FormatException ex)
    {
        Console.WriteLine("Помилка вводу: " + ex.Message);
    }
}
