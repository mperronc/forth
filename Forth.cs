using System;
using System.Text;
using System.Collections;

public static class Forth
{
    private class ForthProgram {
        private Stack Stack;
        public ForthProgram() {
            Stack = new Stack();
        }

        public void EvaluateToken(string tok) {
            int number;
            if (int.TryParse(tok, out number)) {
                Stack.Push(number);
            } else {
                switch (tok)
                {
                    case "+": BinaryOp((x, y) => y + x); break;
                    case "-": BinaryOp((x, y) => y - x); break;
                    case "*": BinaryOp((x, y) => y * x); break;
                    case "/": BinaryOp((x, y) => y / x); break;
                    case "dup": Dup(); break;
                    case "drop": Drop(); break;
                    case "swap": Swap(); break;
                    case "over": Over(); break;
                    default: throw new ArgumentException();
                }
            }
        }

        private void BinaryOp(Func<int, int, int> op) {
            int n1 = (int) Stack.Pop();
            int n2 = (int) Stack.Pop();
            Stack.Push(op(n1, n2));
        }

        private void Dup() {
            Stack.Push(Stack.Peek());
        }

        private void Drop() {
            Stack.Pop();
        }

        private void Swap() {
            int n1 = (int) Stack.Pop();
            int n2 = (int) Stack.Pop();
            Stack.Push(n1);
            Stack.Push(n2);
        }

        private void Over() {
            int n1 = (int) Stack.Pop();
            int n2 = (int) Stack.Peek();
            Stack.Push(n1);
            Stack.Push(n2);
        }

        public string ToString() {
            StringBuilder sb = new StringBuilder();
            object[] arr = Stack.ToArray();
            Array.Reverse(arr);
            foreach (int n in arr)
            {  
               sb.Append(n);
               sb.Append(" "); 
            }
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }


    public static string Evaluate(string[] instructions)
    {
        string[] tokens = instructions[0].Split(" ");

        ForthProgram program = new ForthProgram();

        foreach (string tok in tokens) {
            program.EvaluateToken(tok.ToLower());
        }

        return program.ToString();
    }
}