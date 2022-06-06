using System.Text;
using static Math.WordOperationTable;

namespace Math
{
    public static class Words
    {
        public static List<double[]> GetWords(int bits)
        {
            List<double[]> words = new List<double[]>();
            for (int i = 0; i < System.Math.Pow(2, bits); i++)
            {
                words.Add(Convert.ToString(i, 2).PadLeft(bits, '0').ToBinary());
            }
            return words;
        }

        public static List<Matrix> ToMatrices(this List<double[]> words)
        {
            var wordMatrices = new List<Matrix>();
            for (int i = 0; i < words.Count; i++)
            {
                wordMatrices.Add(words[i].ToMatrix());
            }
            return wordMatrices;
        }

        public static double[] ToBinary(this string binaryString)
        {
            var binary = new double[binaryString.Length];
            for (int i = 0; i < binaryString.Length; i++)
            {
                binary[i] = Convert.ToDouble(binaryString[i] - '0');
            }
            return binary;
        }

        public static Matrix ToMatrix(this double[] word)
        {
            var rows = 1;
            var columns = word.Length;
            var word2d = new double[rows, columns];
            for (int i = 0; i < columns; i++)
            {
                word2d[0, i] = word[i];
            }
            return new Matrix(word2d);
        }

        public static Matrix OR(this Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.Rows != matrix2.Rows || matrix1.Columns != matrix2.Columns)
                throw new InvalidOperationException("OR operation can only performed on matrices with the same dimensions.");

            var result = new Matrix(matrix1.Rows, matrix1.Columns);

            for (int i = 0; i < matrix1.Rows; i++)
            {
                for (int j = 0; j < matrix2.Columns; j++)
                {
                    var matrix1_value = matrix1.Values[i, j];
                    var matrix2_value = matrix2.Values[i, j];
                    if ((matrix1_value != 1 && matrix1_value != 0) || (matrix2_value != 1 && matrix2_value != 0)) // make sure both values are binary
                        throw new InvalidOperationException("OR operation can only be performed on binary matrices");
                    result.Values[i, j] = Convert.ToDouble(Convert.ToInt32(matrix1_value) | Convert.ToInt32(matrix2_value));
                }
            }
            return result;
        }

        public static Matrix XOR(this Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.Rows != matrix2.Rows || matrix1.Columns != matrix2.Columns)
                throw new InvalidOperationException("OR operation can only performed on matrices with the same dimensions.");

            var result = new Matrix(matrix1.Rows, matrix1.Columns);

            for (int i = 0; i < matrix1.Rows; i++)
            {
                for (int j = 0; j < matrix2.Columns; j++)
                {
                    var matrix1_value = matrix1.Values[i, j];
                    var matrix2_value = matrix2.Values[i, j];
                    if ((matrix1_value != 1 && matrix1_value != 0) || (matrix2_value != 1 && matrix2_value != 0)) // make sure both values are binary
                        throw new InvalidOperationException("OR operation can only be performed on binary matrices");
                    result.Values[i, j] = Convert.ToDouble(Convert.ToInt32(matrix1_value) ^ Convert.ToInt32(matrix2_value));
                }
            }
            return result;
        }

        public static Matrix AND(this Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.Rows != matrix2.Rows || matrix1.Columns != matrix2.Columns)
                throw new InvalidOperationException("OR operation can only performed on matrices with the same dimensions.");

            var result = new Matrix(matrix1.Rows, matrix1.Columns);

            for (int i = 0; i < matrix1.Rows; i++)
            {
                for (int j = 0; j < matrix2.Columns; j++)
                {
                    var matrix1_value = matrix1.Values[i, j];
                    var matrix2_value = matrix2.Values[i, j];
                    if ((matrix1_value != 1 && matrix1_value != 0) || (matrix2_value != 1 && matrix2_value != 0)) // make sure both values are binary
                        throw new InvalidOperationException("OR operation can only be performed on binary matrices");
                    result.Values[i, j] = Convert.ToDouble(Convert.ToInt32(matrix1_value) & Convert.ToInt32(matrix2_value));
                }
            }
            return result;
        }

        public static int GetMinDistance(this Matrix matrix)
        {
            int distance = 0;
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    var value = matrix.Values[i, j];
                    if (value > 1)
                        throw new InvalidOperationException($"Min Distance operation only works on binary matrices. Found value: {value}");
                    if (value > 0)
                        distance += 1;
                }
            }
            return distance;
        }

        public static string Stringify(this List<Matrix> matrices)
        {
            var line = new StringBuilder();
            foreach (var matrix in matrices)
            {
                line.Append(matrix.Stringify() + ",");
            }
            var result = line.ToString();
            return result.Remove(result.Length - 1, 1);
        }

        public static string GetOperationString(Matrix matrix1, Matrix matrix2, Matrix result, Operation operation)
        {
            return $"{matrix1.Stringify()} {(operation == Operation.AND ? "*" : "+")} {matrix2.Stringify()} = {result.Stringify()}";
        }

        public static string ToTableString(this Dictionary<Matrix, Matrix> wordToWordDict)
        {
            var table = new StringBuilder();
            var firstPair = wordToWordDict.First();
            var tableWidth = firstPair.Key.ConsoleWidth + firstPair.Value.ConsoleWidth + 1;
            var wordLength = wordToWordDict.First().Key.ConsoleWidth;
            var firstColumnWidth = wordLength >= "Word".Length ? wordLength : "Word".Length;
            string firstColumnHeader = wordLength >= "Word".Length ? "Word".PadLeft(wordLength, ' ') : "Word";
            table.AppendLine($"{firstColumnHeader} | Codeword");
            for (int i = 0; i < tableWidth; i++)
            {
                table.Append("-");
            }
            table.AppendLine();
            foreach (var pair in wordToWordDict)
            {
                table.AppendLine($"{pair.Key.Stringify()} | {pair.Value.Stringify()}");
            }
            return table.ToString();
        }

        public static void WriteTable(this Dictionary<Matrix, Matrix> wordToWordDict)
        {
            Console.WriteLine(wordToWordDict.ToTableString());
        }

    }

    public static class WordOperationTable
    {
        public enum Operation
        {
            AND,
            OR,
            XOR
        }
        public static string OperationTable(List<Matrix> words1, List<Matrix> words2, Operation operation)
        {
            var betweenColumnPadding = 1;
            var table = new StringBuilder();
            table.AppendLine(GetFirstRow(words1[0].ConsoleWidth + 1, betweenColumnPadding, words2));
            for (int i = 0; i < words1.Count; i++)
            {
                table.Append(words1[i].Stringify() + "|");
                for (int j = 0; j < words2.Count; j++)
                {
                    table.Append("".PadLeft(betweenColumnPadding, ' '));
                    switch (operation)
                    {
                        case Operation.AND:
                            table.Append(words1[i].AND(words2[j]).ToBinaryMatrix().Stringify());
                            break;
                        case Operation.OR:
                            table.Append(words1[i].OR(words2[j]).ToBinaryMatrix().Stringify());
                            break;
                        case Operation.XOR:
                            table.Append(words1[i].XOR(words2[j]).ToBinaryMatrix().Stringify());
                            break;
                    }
                    if (j == words2.Count - 1)
                        table.Append(Environment.NewLine);
                }
            }
            return table.ToString();
        }



        public static void Write(List<Matrix> words1, List<Matrix> words2, Operation operation)
        {
            Console.WriteLine(OperationTable(words1, words2, operation));
        }

        private static string GetFirstRow(int firstColumnPadding, int betweenColumnPadding, List<Matrix> words)
        {
            var charCount = 0;
            var row = new StringBuilder();
            row.Append("".PadLeft(firstColumnPadding, ' '));
            charCount += firstColumnPadding;
            foreach (var word in words)
            {
                row.Append("".PadLeft(betweenColumnPadding, ' '));
                var matrix = word.Stringify();
                row.Append(matrix);
                charCount += betweenColumnPadding + matrix.Length;
            }
            row.Append(Environment.NewLine + "".PadLeft(charCount, '-'));
            return row.ToString();
        }
    }
}
