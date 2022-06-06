using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeHomeQuiz2Project
{
    internal class Options
    {
        [Option('w', "wordBitDepth", Required = false, Default = 0, HelpText = "The number of bits in a word")]
        public int WordBitDepth { get; set; }

        [Option('c', "codewordBitDepth", Required = false, Default = 0, HelpText = "The number of bits in a codeword")]
        public int CodewordBitDepth { get; set; }

        [Option('e', "encodingMatrix", Required = false, Default = "", HelpText = "Comma seperated values representing the matrix to use for encoding. Dimensions must match wordBitDepth x codewordBitDepth.")]
        public string EncodingMatrix { get; set; }
    }
}
