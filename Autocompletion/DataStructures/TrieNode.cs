using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Autocompletion.DataStructures {

    public class TrieNode {

        public Dictionary<char, TrieNode> Children { get; set; }
        public bool IsEndOfWord { get; set; }

        public TrieNode() {
            Children = new Dictionary<char, TrieNode>();
            IsEndOfWord = false;
        }
    }
}
