using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace calcul
{
    public partial class MainWindow : Window
    {
        private string currentInput = "0";
        private double firstNumber = 0;
        private string currentOperator = "";
        private bool isNewInput = true;
        private string logFilePath = "calculator_log.txt";
        private bool isEngineerMode = false;

        public MainWindow()
        {
            InitializeComponent();
            UpdateColors();
            LoadHistory();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            string value = btn.Content.ToString();

            switch (value)
            {
                case "C": ClearAll(); break;
                case "⌫": Backspace(); break;
                case "=": CalculateResult(); break;
                case "±": ToggleSign(); break;
                case ",": AddComma(); break;
                case "x²": Square(); break;
                case "√": SquareRoot(); break;
                case "1/x": Reciprocal(); break;
                case "sin": Trigonometric(Math.Sin); break;
                case "cos": Trigonometric(Math.Cos); break;
                case "tan": Trigonometric(Math.Tan); break;
                case "xʸ": SetPowerOperator(); break;
                case "π": AddConstant(Math.PI); break;
                case "(": AddParenthesis('('); break;
                case ")": AddParenthesis(')'); break;
                case "%": Percentage(); break;
                default:
                    if (char.IsDigit(value[0]))
                        AddDigit(value);
                    else if (value == "+" || value == "-" || value == "*" || value == "/")
                        SetOperator(value);
                    break;
            }
        }

        private void AddDigit(string digit)
        {
            if (isNewInput)
            {
                currentInput = digit;
                isNewInput = false;
            }
            else
            {
                if (currentInput == "0" && digit != ",")
                    currentInput = digit;
                else
                    currentInput += digit;
            }
            UpdateDisplay();
        }

        private void AddComma()
        {
            if (!currentInput.Contains(","))
            {
                currentInput += ",";
                UpdateDisplay();
            }
        }

        private void ToggleSign()
        {
            if (double.TryParse(currentInput, out double num))
            {
                num = -num;
                currentInput = num.ToString();
                UpdateDisplay();
            }
        }

        private void SetOperator(string op)
        {
            if (double.TryParse(currentInput, out firstNumber))
            {
                currentOperator = op;
                isNewInput = true;
                if (InputBox != null)
                    InputBox.Text = firstNumber.ToString() + " " + op;
            }
        }

        private void CalculateResult()
        {
            if (string.IsNullOrEmpty(currentOperator))
                return;

            if (double.TryParse(currentInput, out double secondNumber))
            {
                double result = 0;
                string operation = "";

                switch (currentOperator)
                {
                    case "+": result = firstNumber + secondNumber; operation = "+"; break;
                    case "-": result = firstNumber - secondNumber; operation = "-"; break;
                    case "*": result = firstNumber * secondNumber; operation = "*"; break;
                    case "/":
                        if (secondNumber != 0)
                            result = firstNumber / secondNumber;
                        else
                        {
                            if (ResultBox != null)
                                ResultBox.Text = "Ошибка!";
                            return;
                        }
                        operation = "/"; break;
                    case "%": result = firstNumber % secondNumber; operation = "%"; break;
                    case "xʸ": result = Math.Pow(firstNumber, secondNumber); operation = "^"; break;
                }

                string historyEntry = $"{firstNumber} {operation} {secondNumber} = {result}";
                if (ResultBox != null)
                    ResultBox.Text = result.ToString();
                currentInput = result.ToString();
                currentOperator = "";
                isNewInput = true;
                if (InputBox != null)
                    InputBox.Text = "";
                AddToHistory(historyEntry);
            }
        }

        private void ClearAll()
        {
            currentInput = "0";
            firstNumber = 0;
            currentOperator = "";
            isNewInput = true;
            if (InputBox != null)
                InputBox.Text = "";
            if (ResultBox != null)
                ResultBox.Text = "0";
        }

        private void Backspace()
        {
            if (currentInput.Length > 1)
            {
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
                UpdateDisplay();
            }
            else
            {
                currentInput = "0";
                UpdateDisplay();
            }
        }

        private void Square()
        {
            if (double.TryParse(currentInput, out double num))
            {
                double result = num * num;
                if (ResultBox != null)
                    ResultBox.Text = result.ToString();
                AddToHistory($"{num}² = {result}");
                currentInput = result.ToString();
                isNewInput = true;
            }
        }

        private void SquareRoot()
        {
            if (double.TryParse(currentInput, out double num) && num >= 0)
            {
                double result = Math.Sqrt(num);
                if (ResultBox != null)
                    ResultBox.Text = result.ToString();
                AddToHistory($"√{num} = {result}");
                currentInput = result.ToString();
                isNewInput = true;
            }
            else
            {
                if (ResultBox != null)
                    ResultBox.Text = "Ошибка!";
            }
        }

        private void Reciprocal()
        {
            if (double.TryParse(currentInput, out double num) && num != 0)
            {
                double result = 1 / num;
                if (ResultBox != null)
                    ResultBox.Text = result.ToString();
                AddToHistory($"1/{num} = {result}");
                currentInput = result.ToString();
                isNewInput = true;
            }
            else
            {
                if (ResultBox != null)
                    ResultBox.Text = "Ошибка!";
            }
        }

        private void Trigonometric(Func<double, double> func)
        {
            if (double.TryParse(currentInput, out double num))
            {
                double result = func(num * Math.PI / 180);
                if (ResultBox != null)
                    ResultBox.Text = result.ToString();
                AddToHistory($"{func.Method.Name}({num}°) = {result}");
                currentInput = result.ToString();
                isNewInput = true;
            }
        }

        private void Percentage()
        {
            if (double.TryParse(currentInput, out double num))
            {
                double result = num / 100;
                if (ResultBox != null)
                    ResultBox.Text = result.ToString();
                currentInput = result.ToString();
                isNewInput = true;
            }
        }

        private void SetPowerOperator()
        {
            if (double.TryParse(currentInput, out firstNumber))
            {
                currentOperator = "xʸ";
                isNewInput = true;
                if (InputBox != null)
                    InputBox.Text = firstNumber.ToString() + " ^ ";
            }
        }

        private void AddConstant(double constant)
        {
            if (isNewInput)
            {
                currentInput = constant.ToString();
                isNewInput = false;
            }
            else
            {
                currentInput += constant.ToString();
            }
            UpdateDisplay();
        }

        private void AddParenthesis(char p)
        {
            currentInput += p;
            UpdateDisplay();
        }

        private void AddToHistory(string entry)
        {
            try
            {
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {entry}";
                File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            }
            catch { }
        }

        private void LoadHistory()
        {
            try
            {
                if (File.Exists(logFilePath))
                {
                    var lines = File.ReadAllLines(logFilePath);
                    if (lines.Length > 0)
                    {
                        string lastEntry = lines[lines.Length - 1];
                        Title = $"Калькулятор - Последнее: {lastEntry.Substring(20)}";
                    }
                }
            }
            catch { }
        }

        private void ClearHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(logFilePath))
                {
                    File.Delete(logFilePath);
                    MessageBox.Show("История вычислений очищена!", "Успех",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                    Title = "Калькулятор";
                }
                else
                {
                    MessageBox.Show("История пуста!", "Информация",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void ViewHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(logFilePath))
                {
                    string[] historyLines = File.ReadAllLines(logFilePath);
                    if (historyLines.Length > 0)
                    {
                        string historyText = "ИСТОРИЯ ВЫЧИСЛЕНИЙ \n";
                        for (int i = 0; i < historyLines.Length; i++)
                        {
                            historyText += (i + 1) + ". " + historyLines[i] + "\n";
                        }
                        MessageBox.Show(historyText, "История вычислений",
                                       MessageBoxButton.OK, MessageBoxImage.None);
                    }
                    else
                    {
                        MessageBox.Show("История пуста!", "Информация",
                                       MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Файл с историей ещё не создан.Выполните хотя бы одно вычисление.",
                                   "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении истории: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearInput_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        private void ModeRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radio)
            {
                if (radio.Name == "EngineerModeRadio")
                {
                    isEngineerMode = true;
                    if (SimpleGrid != null) SimpleGrid.Visibility = Visibility.Collapsed;
                    if (EngineerGrid != null) EngineerGrid.Visibility = Visibility.Visible;
                }
                else if (radio.Name == "SimpleModeRadio")
                {
                    isEngineerMode = false;
                    if (SimpleGrid != null) SimpleGrid.Visibility = Visibility.Visible;
                    if (EngineerGrid != null) EngineerGrid.Visibility = Visibility.Collapsed;
                }
            }

            UpdateColors();
            ClearAll();
        }

        private void UpdateColors()
        {
            if (ResultBox == null) return;

            if (isEngineerMode)
            {
                ResultBox.Foreground = new SolidColorBrush(Color.FromRgb(100, 200, 255));
            }
            else
            {
                ResultBox.Foreground = Brushes.White;
            }
        }

        private void UpdateDisplay()
        {
            if (ResultBox != null)
                ResultBox.Text = currentInput;
        }
    }
}