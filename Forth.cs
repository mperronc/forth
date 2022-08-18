using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public static class Forth
{
    private enum EvaluationState {
        Normal,
        ReadingUserDefinition,  // Reading what comes after the word until ;
        ReadingUserWord         // Reading the identifier after :
    }


    private class ForthProgram {
        private Stack Stack;

        private Dictionary<string, string[]> UserWords;

        private EvaluationState EvaluationState;

        private string CurrentUserWordDefinition;
        private List<string> UserWordDefinitionBuffer;


        public ForthProgram() {
            Stack = new Stack();
            UserWords = new Dictionary<string, string[]>();
            EvaluationState = EvaluationState.Normal;
            UserWordDefinitionBuffer = new List<string>();
        }

        public void EvaluateToken(string tok) {
            int number;
            if (EvaluationState == EvaluationState.Normal && UserWords.ContainsKey(tok)) {
                EvaluateUserWord(tok);
            }
            else if (EvaluationState == EvaluationState.ReadingUserWord) {
                RegisterUserWord(tok);
            }
            else if (tok != ";" && EvaluationState == EvaluationState.ReadingUserDefinition) {
                UserWordDefinitionBuffer.Add(tok);
            }
            else if (int.TryParse(tok, out number)) {
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
                    case ":": BeginRegisterUserWord(); break;
                    case ";": EndRegisterUserWord(); break;
                }
            }
        }

        private void BeginRegisterUserWord() {
            UserWordDefinitionBuffer.Clear();
            EvaluationState = EvaluationState.ReadingUserWord;
        }

        private void RegisterUserWord(string tok) {
            CurrentUserWordDefinition = tok;
            EvaluationState = EvaluationState.ReadingUserDefinition;
        }

        private void EndRegisterUserWord() {
            bool newWord = UserWords.TryAdd(CurrentUserWordDefinition, new string[UserWordDefinitionBuffer.Count]);
            if (newWord) {
                UserWordDefinitionBuffer.CopyTo(UserWords[CurrentUserWordDefinition]);
                EvaluationState = EvaluationState.Normal;
            } else {
                UserWords[CurrentUserWordDefinition] = new string[UserWordDefinitionBuffer.Count];
                UserWordDefinitionBuffer.CopyTo(UserWords[CurrentUserWordDefinition]);
                EvaluationState = EvaluationState.Normal;
            }

        }

        private void EvaluateUserWord(string word) {
            foreach (string tok in UserWords[word]) {
                EvaluateToken(tok);
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

        public override string ToString() {
            object[] reverseStack = Stack.ToArray();
            Array.Reverse(reverseStack);

            StringBuilder str = new StringBuilder("");

            if (reverseStack.Length > 0) {
                str.Append(reverseStack[0]);
            }

            for (int i = 1; i < reverseStack.Length; i++) {
                str.Append(" ");
                str.Append(reverseStack[i]);
            }

            return str.ToString();
        }
    }


    public static string Evaluate(string[] instructions)
    {
        ForthProgram program = new ForthProgram();

        foreach (string line in instructions) {
            string[] tokens = line.Split(" "); 
            foreach (string tok in tokens) {
                program.EvaluateToken(tok.ToLower());
            }
        }

        return program.ToString();
    }
}