using Autocompletion;
using System.Diagnostics;
using System.Text;

namespace Autocompletion {
    public class Trie {
        public TrieNode Root { get; }
        public int size { get; set; }

        public Trie(TrieNode trieNode) {
            Root = trieNode;
        }

        public void Insert(string word) {
            size++;
            TrieNode current = Root;
            foreach (char ch in word) {
                if (!current.Children.ContainsKey(ch)) {
                    // current.Children.Add(ch, new TrieNode());
                    current.Children[ch] = new TrieNode();
                }
                current = current.Children[ch];
            }
            current.IsEndOfWord = true;
        }

        public void Delete(string word) {
            Delete(Root, word, 0);
        }

        private Boolean Delete(TrieNode current, string word, int index) {
            if (index == word.Length) {
                if (!current.IsEndOfWord) {
                    return false;
                }
                current.IsEndOfWord = false;
                return current.Children.Count == 0;
            }
            if (!current.Children.TryGetValue(word[index], out TrieNode node)) {
                return false;
            }

            bool shouldDeleteCurrentNode = Delete(node, word, index + 1) && !node.IsEndOfWord;
            if (shouldDeleteCurrentNode) {
                current.Children.Remove(word[index]);
                return current.Children.Count == 0;
            }
            return false;
        }

        public bool Search(string word) {
            TrieNode current = Root;
            foreach (char ch in word) {
                if (!current.Children.TryGetValue(ch, out TrieNode node)) {
                    return false;
                }
                current = node;
            }
            return current.IsEndOfWord;
        }

        public List<string> AutoComplete(string prefix) {
            List<string> result = new List<string>(10);
            TrieNode current = Root;

            foreach (char ch in prefix) {
                if (!current.Children.TryGetValue(ch, out TrieNode node)) {
                    return result;
                }
                current = node;
            }
            // Helper(current, result, prefix.Substring(0, prefix.Length - 1));
            // return result;

            StringBuilder stringBuilder = new StringBuilder(prefix);
            Helper(current, stringBuilder, result, 10);
            return result;
        }

        public void Helper(TrieNode node, StringBuilder stringBuilder, List<string> result, int max) {
            if (result.Count >= max) {
                return;
            }
            if (node.IsEndOfWord) {
                result.Add(stringBuilder.ToString());
            }
            foreach (var kvp in node.Children) {
                stringBuilder.Append(kvp.Key);
                Helper(kvp.Value, stringBuilder, result, max);
                stringBuilder.Length--;

                if (result.Count >= max) {
                    return;
                }
            }
        }

        public List<string> GetAllWords() {
            var result = new List<string>();
            var sb = new StringBuilder();
            DFS(Root, sb, result);
            return result;
        }

        private void DFS(TrieNode node, StringBuilder sb, List<string> result) {
            if (node.IsEndOfWord) {
                result.Add(sb.ToString());
            }

            foreach (var kvp in node.Children) {
                sb.Append(kvp.Key);
                DFS(kvp.Value, sb, result);
                sb.Length--;
            }
        }

        public List<string> StartsWith(string prefix) {
            List<string> result = new List<string>();

            TrieNode current = Root;
            foreach (char ch in prefix) {
                if (!current.Children.TryGetValue(ch, out TrieNode node)) {
                    return result;
                }
                current = node;
            }

            StringBuilder sbPrefix = new StringBuilder(prefix);
            foreach (var pair in current.Children) {
                CreateStrings(sbPrefix.Append(pair.Key), pair, result);
                sbPrefix.Remove(sbPrefix.Length - 1, 1);
            }
            return result;
        }

        private void CreateStrings(StringBuilder prefix, KeyValuePair<char, TrieNode> pair, List<string> result) {
            if (pair.Value.Children.Count == 0) {
                result.Add(prefix.ToString());
                return;
            }

            foreach (var child in pair.Value.Children) {
                CreateStrings(prefix.Append(child.Key), child, result);
                prefix.Remove(prefix.Length - 1, 1);
            }
        }

        public Object ToJson(TrieNode node, char? letter = null) {
            return new {
                letter = letter?.ToString(),
                end = node.IsEndOfWord,
                children = node.Children.Select(kvp => ToJson(kvp.Value, kvp.Key)).ToList()
            };
        }

        public TrieNode GetNodeForPrefix(string prefix) {
            TrieNode current = Root;
            foreach (char ch in prefix) {
                if (!current.Children.TryGetValue(ch, out var next)) {
                    return null;
                }
                current = next;
            }
            return current;
        }
    }
}
