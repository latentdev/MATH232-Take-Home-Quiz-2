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
        private Dictionary<Matrix, Matrix> _wordToCodeword = new Dictionary<Matrix, Matrix>();
        private Dictionary<Matrix, Matrix> _codewordToWord = new Dictionary<Matrix, Matrix>();

        #region Quiz Questions

        internal void Question1()
        {
            _wordBitDepth = GetBitDepth("word");
            _codewordBitDepth = GetBitDepth("codeword");
            _encodingMatrix = GetEncodingMatrix(_wordBitDepth, _codewordBitDepth);
            var words = Words.GetWords(_wordBitDepth).ToMatrices();
            
            foreach (var word in words)
            {
                var codeword = word.Multiply(_encodingMatrix).ToBinaryMatrix();
                _wordToCodeword.Add(word, codeword);
                _codewordToWord.Add(codeword,word);
            }
            _wordToCodeword.WriteTable();
        }

        public void Question2()
        {
            var matrices = _wordToCodeword.Values.ToList();
            var numOfBits = matrices.First().Columns;
            var bits = new double[1, numOfBits];
            for (int i = 0; i < numOfBits; i++)
            {
               bits[0, i] = 1;
            }
            var multiplicativeIdentity = new Matrix(bits);
            var hasIdentity = HasIdentity(matrices, multiplicativeIdentity);
            Console.WriteLine($"Identity: {(hasIdentity ? $"yes {multiplicativeIdentity.Stringify()}" : "no")}");
            List<Matrix> elementsWithoutInverses;
            Dictionary<Matrix,Matrix> elementsWithInverses;
            var hasInverses = HasInverses(matrices, multiplicativeIdentity, out elementsWithoutInverses, out elementsWithInverses);
            Console.Write($"Inverse: {(hasInverses?"yes":"no")}");
            if(!hasInverses)
            {
                Console.WriteLine($" Elements without an inverse: {elementsWithoutInverses.Stringify()}");
            }
            else
            {
                Console.WriteLine("Inverses: ");
                foreach (var operands in elementsWithInverses)
                {
                    Console.WriteLine(Words.GetOperationString(operands.Key, operands.Value, operands.Key.AND(operands.Value), Operation.AND));
                }
            }

        }

        #endregion

        #region Private Functions
        
        //private bool IsGroup(bool hasIdentity, bool hasInverses, bool isClosed)
        //{

        //}

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
            foreach (var matrix in matrices)
            {
                var product = element.AND(matrix).ToBinaryMatrix();
                var isInverse = product.Equals(identity);
                if (isInverse)
                {
                    inverse = product;
                    return true;//Found an inverse.
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

        #endregion
    }
}
