using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NCalc; // veendu, et NCalc on NuGetist lisatud

namespace SciCalc.ViewModels
{
    [INotifyPropertyChanged]
    internal partial class CalculatorPageViewModel
    {
        [ObservableProperty]
        private string inputText = string.Empty;

        [ObservableProperty]
        private string calculatedResult = "0";

        private bool isSciOpWaiting = false;

        // AC
        [RelayCommand]
        private void Reset()
        {
            CalculatedResult = "0";
            InputText = string.Empty;
            isSciOpWaiting = false;
        }

        // =
        [RelayCommand]
        private void Calculate()
        {
            if (InputText.Length == 0) return;
            if (isSciOpWaiting) { InputText += ")"; isSciOpWaiting = false; }

            try
            {
                var inputString = NormalizeInputString();
                var expression = new Expression(inputString);
                var result = expression.Evaluate();
                CalculatedResult = result?.ToString() ?? "NaN";
            }
            catch
            {
                CalculatedResult = "NaN";
            }
        }

        private string NormalizeInputString()
        {
            var map = new Dictionary<string, string>
            {
                { "×", "*" }, { "÷", "/" },
                { "SIN", "Sin" }, { "COS", "Cos" }, { "TAN", "Tan" },
                { "ASIN", "Asin" }, { "ACOS", "Acos" }, { "ATAN", "Atan" },
                { "LOG", "Log" }, { "EXP", "Exp" }, { "LOG10", "Log10" },
                { "POW", "Pow" }, { "SQRT", "Sqrt" }, { "ABS", "Abs" },
            };

            var s = InputText;
            foreach (var kv in map) s = s.Replace(kv.Key, kv.Value);
            return s;
        }

        // ⌫
        [RelayCommand]
        private void Backspace()
        {
            if (InputText.Length == 0) return;
            InputText = InputText[..^1];
        }

        // 0–9 ja .
        [RelayCommand]
        private void NumberInput(string key)
        {
            InputText += key;
        }

        // +, -, ×, ÷
        [RelayCommand]
        private void MathOperator(string op)
        {
            if (isSciOpWaiting) { InputText += ")"; isSciOpWaiting = false; }
            InputText += $" {op} ";
        }

        // (, ), ,
        [RelayCommand]
        private void RegionOperator(string op)
        {
            if (isSciOpWaiting) { InputText += ")"; isSciOpWaiting = false; }
            InputText += op;
        }

        // SIN, COS, ...
        [RelayCommand]
        private void ScientificOperator(string op)
        {
            InputText += $"{op}(";
            isSciOpWaiting = true; // jätame ootele kuni vajutatakse operaator või '='
        }
    }
}
