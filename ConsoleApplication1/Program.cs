using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter expression");
            string InputString = Console.ReadLine();
            Console.WriteLine("Result DataTable: " + Calculator.ResultDataTable(InputString));
            Console.WriteLine("Result PostfixNotation: " + Calculator.resultPostfixNotation(InputString));
            Console.ReadKey();
        }
    }
}

public static class Calculator
{
    private static DataTable Table = new DataTable();
    private static List<string> standart_operators = new List<string>(new string[] { "(", ")", "+", "-", "*", "/", "^" });
    private static List<string> operators = new List<string>(standart_operators);

    public static double ResultDataTable(string InputString)
    {
        try
        {
            return Convert.ToDouble(Table.Compute(InputString, string.Empty));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return 0;
        }
    }

    private static IEnumerable<string> Separate(string input)
    {
        int pos = 0;
        while (pos < input.Length)
        {
            string s = string.Empty + input[pos];
            if (!standart_operators.Contains(input[pos].ToString()))
            {
                if (Char.IsDigit(input[pos]))
                    for (int i = pos + 1; i < input.Length &&
                        (Char.IsDigit(input[i]) || input[i] == ',' || input[i] == '.'); i++)
                        s += input[i];
                else if (Char.IsLetter(input[pos]))
                    for (int i = pos + 1; i < input.Length &&
                        (Char.IsLetter(input[i]) || Char.IsDigit(input[i])); i++)
                        s += input[i];
            }
            yield return s;
            pos += s.Length;
        }
    }

   private static byte GetPriority(string s)
    {
        switch (s)
        {
            case "(":
            case ")":
                return 0;
            case "+":
            case "-":
                return 1;
            case "*":
            case "/":
                return 2;
            case "^":
                return 3;
            default:
                return 4;
        }
    }

   public static string[] ConvertToPostfixNotation(string input)
    {
        List<string> outputSeparated = new List<string>();
        Stack<string> stack = new Stack<string>();
        foreach (string c in Separate(input))
        {
            if (operators.Contains(c))
            {
                if (stack.Count > 0 && !c.Equals("("))
                {
                    if (c.Equals(")"))
                    {
                        string s = stack.Pop();
                        while (s != "(")
                        {
                            outputSeparated.Add(s);
                            s = stack.Pop();
                        }
                    }
                    else if (GetPriority(c) > GetPriority(stack.Peek()))
                        stack.Push(c);
                    else
                    {
                        while (stack.Count > 0 && GetPriority(c) <= GetPriority(stack.Peek()))
                            outputSeparated.Add(stack.Pop());
                        stack.Push(c);
                    }
                }
                else
                    stack.Push(c);
            }
            else
                outputSeparated.Add(c);
        }
        if (stack.Count > 0)
            foreach (string c in stack)
                outputSeparated.Add(c);

        return outputSeparated.ToArray();
    }
   public static double resultPostfixNotation(string input)
    {
        Stack<string> stack = new Stack<string>();
        Queue<string> queue = new Queue<string>(ConvertToPostfixNotation(input));
        string str = queue.Dequeue();
        while (queue.Count >= 0)
        {
            if (!operators.Contains(str))
            {
                stack.Push(str);
                str = queue.Dequeue();
            }
            else
            {
                double summ = 0;
                switch (str)
                {

                    case "+":
                        {
                            double a = Convert.ToDouble(stack.Pop());
                            double b = Convert.ToDouble(stack.Pop());
                            summ = a + b;
                            break;
                        }
                    case "-":
                        {
                            double a = Convert.ToDouble(stack.Pop());
                            double b = Convert.ToDouble(stack.Pop());
                            summ = b - a;
                            break;
                        }
                    case "*":
                        {
                            double a = Convert.ToDouble(stack.Pop());
                            double b = Convert.ToDouble(stack.Pop());
                            summ = b * a;
                            break;
                        }
                    case "/":
                        {
                            double a = Convert.ToDouble(stack.Pop());
                            double b = Convert.ToDouble(stack.Pop());
                            summ = b / a;
                            break;
                        }
                    case "^":
                        {
                            double a = Convert.ToDouble(stack.Pop());
                            double b = Convert.ToDouble(stack.Pop());
                            summ = Convert.ToDouble(Math.Pow(Convert.ToDouble(b), Convert.ToDouble(a)));
                            break;
                        }
                }
                stack.Push(summ.ToString());
                if (queue.Count > 0)
                    str = queue.Dequeue();
                else
                    break;
            }
        }
        return Convert.ToDouble(stack.Pop());
    }
}