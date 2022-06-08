using CommandLine;
using Math;
using System.Text;
using static Math.WordOperationTable;

namespace TakeHomeQuiz2Project
{
    internal class TakeHomeQuiz2
    {
        private int _wordBitDepth = 0;
        private int _codewordBitDepth = 0;
        private Matrix _encodingMatrix;
        private bool _isClosed = false;
        private bool _isGroup = false;
        private Dictionary<Matrix, Matrix> _wordToCodeword = new Dictionary<Matrix, Matrix>();
        private Dictionary<Matrix, Matrix> _codewordToWord = new Dictionary<Matrix, Matrix>();
        private Dictionary<Matrix, Tuple<Matrix, Matrix>> _codeWordLookupTable = new Dictionary<Matrix, Tuple<Matrix, Matrix>>();
        private ConsoleColor _defaultForeground = Console.ForegroundColor;
        private ConsoleColor _defaultBackgroundColor = Console.BackgroundColor;
        private ConsoleColor _questionForegroundColor = ConsoleColor.White;
        private ConsoleColor _questionBackgroundColor = ConsoleColor.DarkGreen;

        #region Quiz Questions

        internal void Setup()
        {
            WriteHeader("[Setup]");
            _wordBitDepth = GetBitDepth("word");
            _codewordBitDepth = GetBitDepth("codeword");
            _encodingMatrix = GetEncodingMatrix(_wordBitDepth, _codewordBitDepth);
        }

        internal void Setup(int wordBitDepth, int codewordBitDepth, Matrix encodingMatrix)
        {
            WriteHeader("[Setup]");
            _wordBitDepth = wordBitDepth;
            _codewordBitDepth = codewordBitDepth;
            _encodingMatrix = encodingMatrix;
            var header = "Word Bit Depth:";
            Console.WriteLine($"{header.PadLeft(header.Length + 4, ' ')} {_wordBitDepth}");
            Console.WriteLine($"Codeword Bit Depth: {_codewordBitDepth}\n");
            Console.WriteLine($"Encoding Matrix:\n{_encodingMatrix.Stringify()}\n");
            Console.Write("Press enter to begin Take Home Quiz 2...");
            Console.ReadLine();
        }

        internal void Question1()
        {
            WriteHeader("[Question 1]");
            var words = Words.GetWords(_wordBitDepth).ToMatrices();
            
            foreach (var word in words)
            {
                var codeword = word.Multiply(_encodingMatrix).ToBinaryMatrix();
                _wordToCodeword.Add(word, codeword);
                _codewordToWord.Add(codeword,word);
            }
            _wordToCodeword.WriteTable();
            Console.Write("Press enter to continue to Question 2...");
            Console.ReadLine();
            Console.WriteLine();
        }

        internal void Question2()
        {
            WriteHeader("[Question 2]");
            var matrices = _wordToCodeword.Values.ToList();

            //Check if identity exists
            var numOfBits = matrices.First().Columns;
            var bits = new double[1, numOfBits];
            for (int i = 0; i < numOfBits; i++)
            {
               bits[0, i] = 0;
            }
            var identity = new Matrix(bits);
            var hasIdentity = HasIdentity(matrices, identity);
            Console.WriteLine($"Identity: {(hasIdentity ? $"yes {identity.Stringify()}" : "no")}");

            //Check if each element has an inverse
            List<Matrix> elementsWithoutInverses;
            Dictionary<Matrix,Matrix> elementsWithInverses;
            var hasInverses = HasInverses(matrices, identity, out elementsWithoutInverses, out elementsWithInverses);
            Console.Write($"Inverses: {(hasInverses?"yes":"no")}");
            if(!hasInverses)
            {
                Console.WriteLine($" Elements without an inverse: {elementsWithoutInverses.Stringify()}");
            }
            else
            {
                Console.WriteLine();
                foreach (var operands in elementsWithInverses)
                {
                    Words.PerformOperation(operands.Key, operands.Value, Operation.XOR, true);
                }
            }

            //Check if set is closed
            List<Tuple<Matrix, Matrix>> notInSet = new List<Tuple<Matrix, Matrix>>();
            _isClosed = IsClosed(matrices, ref notInSet);

            if(_isClosed)
            {
                Console.WriteLine($"Closed: yes");
                WriteAllTableOperations(matrices);
            }
            else
            {
                Console.WriteLine($"Closed: no\nOperations that produced an element not in the set:");
                foreach (var operands in notInSet)
                {
                    operands.Item1.PerformOperation(operands.Item2, Operation.XOR, true);
                }
            }

            _isGroup = (hasIdentity && hasInverses && _isClosed);
            if(_isGroup)
            {
                Console.WriteLine("\nIt is a group because the set:\n -Has an identity element\n -Each element has an inverse\n -The set is closed\n");
            }
            else
            {
                Console.WriteLine("\nIt is not a group because the set:");
                if (!hasIdentity)
                    Console.WriteLine(" -Does not have an identity element");
                if (!hasInverses)
                    Console.WriteLine(" -There are elements that do not have inverses");
                if (!_isClosed)
                    Console.WriteLine(" -The set is not closed");
                Console.WriteLine();
                Console.WriteLine("Because set is not a group Questions 3-5 aren't solvable.");
                Console.WriteLine("Thanks for trying out the application!");
                Environment.Exit(0);
            }
            Console.Write("Press enter to continue to Question 3...");
            Console.ReadLine();
            Console.WriteLine();
        }

        internal void Question3()
        {
            WriteHeader("[Question 3]");
            var distances = new List<int>();
            if(_isClosed)
            {
                Console.WriteLine("Because set is closed all possible combinations are represented in the set.");
                foreach (var matrix in _codewordToWord.Keys.ToList())
                {
                    var distance = matrix.GetDistance();
                    distances.Add(distance);
                    matrix.WriteLine($" Distance: {distance}");
                }            
                distances.RemoveAll(x => x == 0);//remove all zero distances
                Console.WriteLine($"\nMinimum Distance (k): {distances.Min()}");
                Console.WriteLine($"Minimum number of errors that can be detected (k-1): {distances.Min() - 1}");
                Console.WriteLine($"Minimum number of errors that can be corrected ((k-1)/2): {(distances.Min() - 1) / 2}");
            }
            else
            {
                Console.WriteLine("Set is not closed. Error detection cannot occur.");
            }
            Console.WriteLine();
            Console.Write("Press enter to continue to Question 4...");
            Console.ReadLine();
            Console.WriteLine();
        }

        internal void Question4()
        {
            WriteHeader("[Question 4]");
            var possibleCombinations = System.Math.Pow(2, _codewordBitDepth);
            var numberOfCosetLeadersRequired =(int) possibleCombinations / _wordToCodeword.Values.Count;
            Console.WriteLine($"Possible combination of 7 bits: {possibleCombinations}");
            Console.WriteLine($"Number of distinct cosets required: {numberOfCosetLeadersRequired}");
            var cosetLeaders = new List<Matrix>();
            _codeWordLookupTable = GetTableOfCosets(ref cosetLeaders, numberOfCosetLeadersRequired);
            WordOperationTable.Write(cosetLeaders, _wordToCodeword.Values.ToList(),Operation.XOR);
            Console.WriteLine();
            Console.Write("Press enter to continue to Question 5...");
            Console.ReadLine();
            Console.WriteLine();
        }

        internal void Question5()
        {
            WriteHeader("[Question 5]");
            string input = "";
            do
            {
                input = GetCodeWord();
                if (input.ToLower().Equals("x"))
                {
                    Console.WriteLine("Thanks for trying out the application!");
                    return;
                }
                input = string.Join<char>(",", input);
                var codeword = new Matrix(1,_codewordBitDepth, input);
                Console.WriteLine($"Looking up {codeword.Stringify()}");
                var entry = _codeWordLookupTable[codeword];
                var word = _codewordToWord[entry.Item2];
                Console.WriteLine($"{codeword.Stringify()} -> {entry.Item2.Stringify()} -> {word.Stringify()}");
                
            }
            while (true); //Reached the end of the program. Loop till user wants to quit.
        }


        #endregion

        #region Private Functions

        internal void ParseArguments(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<Options>(args)
                    .WithParsed(RunQuiz)
                    .WithNotParsed(HandleParseError);
            }
            catch (Exception ae) //was ArgumentException
            {
                Console.WriteLine($"{ae.Message}\n\n{ae.StackTrace}");
            }
        }

        /// <summary>
        /// Handles any errors in parsing startup arguments.
        /// </summary>
        /// <param name="errors"></param>
        private static void HandleParseError(IEnumerable<Error> errors)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Error occured parsing arguments");
            foreach (var error in errors)
            {
                stringBuilder.AppendLine($"Error: {error.Tag}");
            }
            stringBuilder.AppendLine("Application will now exit.");
            Console.WriteLine(stringBuilder.ToString());
        }

        private void RunQuiz(Options args)
        {
            if (args.WordBitDepth != 0 && args.CodewordBitDepth != 0 && !args.EncodingMatrix.Equals(String.Empty)) // Arguments were provided
            {
                var matrix = new Matrix(args.WordBitDepth, args.CodewordBitDepth, args.EncodingMatrix);
                Setup(args.WordBitDepth, args.CodewordBitDepth, matrix);
            }
            else
            {
                Setup();
            }
            Question1();
            Question2();
            Question3();
            Question4();
            Question5();
        }

        private string GetCodeWord()
        {
            var inputIsValid = false;
            string input = "";
            do
            {
                Console.Write($"Please enter a {_codewordBitDepth} bit codeword to look up or enter x to exit:");
                input = Console.ReadLine();
                if(input.ToLower().Equals("x"))
                    return input;
                inputIsValid = input.IsValidWord(_codewordBitDepth);
                if (!inputIsValid)
                {
                    Console.WriteLine($"Invalid codeword entered. Please try again.");
                }
            } while (!inputIsValid);
            return input;
        }

        private Dictionary<Matrix,Tuple<Matrix,Matrix>> GetTableOfCosets(ref List<Matrix> cosetLeaders, int numOfCosetLeadersRequired)
        {
            int value = 0;
            int depth = _codewordBitDepth;
            var allPotentialCosetLeaders = new List<Matrix>();
            for (int i = 1; i <= _codewordBitDepth; i++)
            {
                var startingValue = (int)System.Math.Pow(2, i)-1;
                allPotentialCosetLeaders.AddRange(GenerateCosetsLeaders(_codewordBitDepth, startingValue, depth--));
                value += 1;
            }
            //Get initial coset lookup table
            var index = _codewordBitDepth;
            cosetLeaders.AddRange(allPotentialCosetLeaders.GetRange(0,index++)); //Load initial coset leaders
            var operationLookupTable = cosetLeaders.GetOperationTableLookup(_wordToCodeword.Values.ToList(), Operation.XOR);
            while(cosetLeaders.Count < numOfCosetLeadersRequired)
            {
                if(!operationLookupTable.ContainsKey(allPotentialCosetLeaders[index]))
                {
                    cosetLeaders.Add(allPotentialCosetLeaders[index]);
                    operationLookupTable = cosetLeaders.GetOperationTableLookup(_wordToCodeword.Values.ToList(), Operation.XOR);
                }
                index++;
            }

            return operationLookupTable;
        }

        private List<Matrix> GenerateCosetsLeaders(int bits, int startingValue, int depth)
        {
            var values = new List<double[]>();
            for (int i = 0; i < depth; i++)
            {
                values.Add(Convert.ToString(startingValue, 2).PadLeft(bits, '0').ToBinary());
                startingValue = startingValue << 1;//shift all bits to the left 1
            }
            return values.ToMatrices();
        }

        private bool IsClosed(List<Matrix> matrices, ref List<Tuple<Matrix,Matrix>> notInSet)
        {
            if (matrices.Count == 0)
                return false;
            var operands = new List<Tuple<Matrix,Matrix>>();
            var results = new List<Matrix>();
            var matricesCopy = matrices.ToList(); // make a copy of the list
            foreach (var matrix in matrices)
            {
                results.AddRange(matrix.OperateOnList(matricesCopy, ref operands, Operation.XOR, false));
                matricesCopy.Remove(matrix);
            }
            var elementsNotInSet = GetElementsNotInSet(matrices, results, operands);
            if (elementsNotInSet.Count > 0)
            {
                foreach (var element in elementsNotInSet)
                {
                    notInSet.Add(element.Item1);
                }
                return false;
            }
            else
                return true;
        }

        private List<Tuple<Tuple<Matrix,Matrix>,Matrix>> GetElementsNotInSet(List<Matrix> matrices, List<Matrix> results, List<Tuple<Matrix,Matrix>> operands)
        {
            var elementsNotInSet = new List<Tuple<Tuple<Matrix,Matrix>,Matrix>>();
            for(int i = 0; i< results.Count; i++)
            {
                if (!matrices.Contains(results[i]))
                    elementsNotInSet.Add(Tuple.Create(operands[i], results[i]));
            }
            return elementsNotInSet;

        }

        public void WriteAllTableOperations(List<Matrix> matrices)
        {
            var operands = new List<Tuple<Matrix, Matrix>>();
            var results = new List<Matrix>();
            var matricesCopy = matrices.ToList(); // make a copy of the list
            foreach (var matrix in matrices)
            {
                results.AddRange(matrix.OperateOnList(matricesCopy, ref operands, Operation.XOR, true));
                matricesCopy.Remove(matrix);
            }
        }

        private bool HasIdentity(List<Matrix> matrices, Matrix identity)
        {
            if (matrices != null)
                if (matrices.Count > 0)
                {
                    var numOfBits = matrices.First().Columns;

                    foreach (var matrix in matrices)
                    {
                        if(matrix.Equals(identity))
                            return true;
                    }
                }
            return false;
        }

        private bool HasInverses(List<Matrix> matrices, Matrix identity, out List<Matrix> elementsWithNoInverses, out Dictionary<Matrix,Matrix> elementsWithInverses)
        {
            elementsWithNoInverses = new List<Matrix>();
            elementsWithInverses = new Dictionary<Matrix, Matrix>();
            Matrix inverse;
            foreach (var matrix in matrices)
            {
                var hasInverse = HasInverse(matrix, matrices, identity, out inverse);
                if (!hasInverse)
                {
                    elementsWithNoInverses.Add(matrix);
                    //return false;//didn't find an inverse for this element. Therefore not all elements have inverses and it fails.
                }
                elementsWithInverses.Add(matrix, inverse);
            }
            return elementsWithNoInverses.Count >0 ? false : true;//Found inverses for all elements.
        }

        private bool HasInverse(Matrix element, List<Matrix> matrices, Matrix identity, out Matrix inverse)
        {
            bool foundInverse = false;
            foreach (var matrix in matrices)
            {
                var product = element.XOR(matrix).ToBinaryMatrix();
                var isInverse = product.Equals(identity);
                if (isInverse)
                {
                    inverse = matrix;
                    return true;
                }
            }
            inverse = identity;
            return false;//Looped through all the matrices and didn't find an inverse.
        }

        private int GetBitDepth(string name)
        {
            Console.Write($"Please enter {name} bit depth(int): ");
            var bitDepth = 0;
            do
            {
                try
                {
                    bitDepth = Convert.ToInt32(Console.ReadLine());
                    if (bitDepth <= 0)
                        throw new Exception();
                }
                catch (Exception ex)
                {
                    Console.Write($"Invalid input. Bit depth must be an integer greater than 0.\nPlease enter {name} bit depth(int): ");
                }
            } while (bitDepth <= 0);
            return bitDepth;
        }

        private Matrix GetEncodingMatrix(int rows, int columns)
        {
            Matrix encodingMatrix = null;
            string proceed = "n";
            do
            {

                Console.WriteLine($"A {rows}x{columns} encoding matrix is required.");
                var matrixString = "";
                for (int i = 0; i < rows; i++)
                {
                    var rowCountString = GetRowCountString(i + 1);
                    Console.Write($"Enter {rowCountString} row of encoding matrix values seperated by a comma(1,2,3): ");
                    var rowString = Console.ReadLine();
                    if (i != rows - 1)
                    {
                        rowString += ",";
                    }
                    matrixString += rowString;
                }
                Console.WriteLine();
                encodingMatrix = new Matrix(GetValueArray(rows, columns, matrixString));
                encodingMatrix.WriteLine();
                Console.Write("Is this the correct encoding matrix? Press y if correct and any other key to try again:");
                Console.ReadLine();
            } while (!proceed.ToLower().Equals("y") && encodingMatrix == null);
            Console.WriteLine();
            return encodingMatrix;
        }

        private double[,] GetValueArray(int rows, int columns, string matrixString)
        {
            var values = matrixString.Replace(" ", "").Split(',');
            if (rows * columns != values.Length)
                throw new InvalidOperationException($"Number of values provided does not equal the number of values required by the encoding matrix. Provided: {values.Length} Required: {rows * columns}");
            var valuesArray = new double[rows, columns];
            int index = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    valuesArray[i, j] = Convert.ToDouble(values[index++]);
                }
            }
            return valuesArray;
        }

        private string GetRowCountString(int rowCount)
        {
            if (rowCount <= 0)
                throw new ArgumentException($"Count values can only be integers greater than 0. Received: {rowCount}");
            switch (rowCount)
            {
                case 1:
                    return "1st";
                case 2:
                    return "2nd";
                case 3:
                    return "3rd";
                default:
                    return $"{rowCount}th";
            }
        }

        private void WriteHeader(string header)
        {
            Console.ForegroundColor = _questionForegroundColor;
            Console.BackgroundColor = _questionBackgroundColor;
            Console.Write(header);
            Console.ForegroundColor = _defaultForeground;
            Console.BackgroundColor = _defaultBackgroundColor;
            Console.WriteLine();
            Console.WriteLine();
        }

        #endregion
    }
}
