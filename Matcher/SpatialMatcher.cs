using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zxcvbn.Matcher
{
    public class SpatialMatcher : IMatcher
    {
        const string SpatialPattern = "spatial";

        Lazy<List<SpatialGraph>> spatialGraphs = new Lazy<List<SpatialGraph>>(() => GenerateSpatialGraphs());

        public IEnumerable<Match> MatchPassword(string password)
        {
            return spatialGraphs.Value.SelectMany((g) => SpatialMatch(g, password)).ToList();
        }

        private List<Match> SpatialMatch(SpatialGraph graph, string password)
        {
            var matches = new List<Match>();

            var i = 0;
            while (i < password.Length - 1)
            {
                var j = i + 1;
                for (; j < password.Length && graph.IsCharAdjacent(password[j - 1], password[j]); ++j) ;

                // Only consider runs of greater than two
                if (j - i > 2) 
                {
                    matches.Add(new Match()
                    {
                        Pattern = SpatialPattern,
                        i = i,
                        j = j - 1,
                        Token = password.Substring(i, j - i)
                    });
                }

                i = j;
            }

            return matches;
        }


        // In the JS version these are precomputed, but for now we'll generate them here when they are first needed.
        private static List<SpatialGraph> GenerateSpatialGraphs()
        {
            // Kwyboard layouts directly from zxcvbn's build_kayboard_adjacency_graph.py script
            const string qwerty = @"
`~ 1! 2@ 3# 4$ 5% 6^ 7& 8* 9( 0) -_ =+
    qQ wW eE rR tT yY uU iI oO pP [{ ]} \|
        aA sS dD fF gG hH jJ kK lL ;: '""
        zZ xX cC vV bB nN mM ,< .> /?
";

            const string dvorak = @"
`~ 1! 2@ 3# 4$ 5% 6^ 7& 8* 9( 0) [{ ]}
    '"" ,< .> pP yY fF gG cC rR lL /? =+ \|
        aA oO eE uU iI dD hH tT nN sS -_
        ;: qQ jJ kK xX bB mM wW vV zZ
";

            const string keypad = @"
    / * -
7 8 9 +
4 5 6
1 2 3
    0 .
";

            const string mac_keypad = @"
    = / *
7 8 9 -
4 5 6 +
1 2 3
    0 .
";

            return new List<SpatialGraph> { new SpatialGraph("qwerty", qwerty, true),
                    new SpatialGraph("dvorak", dvorak, true),
                    new SpatialGraph("keypad", keypad, false),
                    new SpatialGraph("mac_kaypad", mac_keypad, false)
                };
        }

        // See build_keyboard_adjacency_graph.py in zxcvbn for how these are generated
        // TODO: this is a quick and dirty adaptation could be spruced up a bit
        private class SpatialGraph
        {
            public string Name { get; private set; }
            private Dictionary<char, List<string>> AdjacencyGraph { get; set; }

            public SpatialGraph(string name, string layout, bool slanted)
            {
                this.Name = name;
                BuildGraph(layout, slanted);                
            }


            /// <summary>
            /// Returns true when testAdjacent is in c's adjacency list
            /// </summary>
            public bool IsCharAdjacent(char c, char testAdjacent)
            {
                if (AdjacencyGraph.ContainsKey(c)) return AdjacencyGraph[c].Any(s => s.Contains(testAdjacent));
                return false;
            }

            private Point[] GetSlantedAdjacent(Point c)
            {
                int x = c.x;
                int y = c.y;

                return new Point[] { new Point(x - 1, y), new Point(x, y - 1), new Point(x + 1, y - 1), new Point(x + 1, y), new Point(x, y + 1), new Point(x - 1, y + 1) };
            }

            private Point[] GetAlignedAdjacent(Point c)
            {
                int x = c.x;
                int y = c.y;

                return new Point[] { new Point(x - 1, y), new Point(x - 1, y - 1), new Point(x, y - 1), new Point(x + 1, y - 1), new Point(x + 1, y), new Point(x + 1, y + 1), new Point(x, y + 1), new Point(x - 1, y + 1) };
            }

            private void BuildGraph(string layout, bool slanted)
            {

                var tokens =  layout.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                var tokenSize = tokens[0].Length;

                // Put the characters in each keyboard cell into the map agains t their coordinates
                var positionTable = new Dictionary<Point, string>();
                var lines = layout.Split('\n');
                for (int y = 0; y < lines.Length; ++y)
                {
                    var line = lines[y];
                    var slant = slanted ? y - 1 : 0;

                    foreach (var token in line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var x = (line.IndexOf(token) - slant) / (tokenSize + 1);
                        var p = new Point(x, y);
                        positionTable[p] = token;
                    }
                }

                AdjacencyGraph = new Dictionary<char, List<string>>();
                foreach (var pair in positionTable)
                {
                    var p = pair.Key;
                    foreach (var c in pair.Value)
                    {
                        AdjacencyGraph[c] = new List<string>();
                        var adjacentPoints = slanted ? GetSlantedAdjacent(p) : GetAlignedAdjacent(p);

                        foreach (var adjacent in adjacentPoints)
                        {
                            if (positionTable.ContainsKey(adjacent)) AdjacencyGraph[c].Add(positionTable[adjacent]);
                        }
                    }
                }
                
            }
        }

        // Instances of Point or Pair in the standard library are in UI assemblies, so define our own version to reduce dependencies
        private struct Point
        {
            public int x;
            public int y;

            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public override string ToString()
            {
                return "{" + x + ", " + y + "}";
            }
        }
    }
}
