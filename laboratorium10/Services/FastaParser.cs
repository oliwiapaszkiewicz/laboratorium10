using laboratorium10.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laboratorium10.Services
{
    public class FastaParser
    {
        public static List<FastaSequence> ParseFastaFile(string filePath)
        {
            var sequences = new List<FastaSequence>();
            string[] lines = File.ReadAllLines(filePath);
            FastaSequence current = null;

            foreach (var line in lines)
            {
                if (line.StartsWith(">"))
                {
                    if (current != null)
                        sequences.Add(current);

                    var header = line.Substring(1);
                    var parts = header.Split(' ', 2);
                    current = new FastaSequence
                    {
                        Id = parts[0],
                        Description = parts.Length > 1 ? parts[1] : "",
                        Sequence = ""
                    };
                }
                else if (current != null)
                {
                    current.Sequence += line.Trim();
                }
            }

            if (current != null)
                sequences.Add(current);

            return sequences;
        }
    }
}
