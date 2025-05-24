using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laboratorium10.Models
{
    public class FastaSequence
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Sequence { get; set; }

        public int Length => Sequence.Length;

        public double GCContent =>
            (Sequence.Count(c => c == 'G' || c == 'g' || c == 'C' || c == 'c') / (double)Sequence.Length) * 100;

        public int CodonCount => Sequence.Length / 3;

        public Dictionary<char, int> BaseCounts => Sequence
            .GroupBy(c => char.ToUpper(c))
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
