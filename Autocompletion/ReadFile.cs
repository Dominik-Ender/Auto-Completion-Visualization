namespace Autocompletion {
    public class ReadFile {

        String line;

        public async Task StartReading(Trie trie) {
            try {
                StreamReader streamReader = new StreamReader("words.txt");

                line = streamReader.ReadLine();

                while ((line = streamReader.ReadLine()) != null) {
                    // Console.Write(line + ", ");
                    // line = streamReader.ReadLine();

                    string[] words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    
                    foreach(var word in words) {
                        trie.Insert(word.Trim());
                    }


                }

                var allWords = trie.GetAllWords();
                File.WriteAllLines("test.txt", allWords);

                // Console.WriteLine("TRIE SIZE: " + trie.size);

                streamReader.Close();
            } catch (Exception exception) {
                Console.WriteLine("Exception: " + exception.Message);
            } finally {
                Console.WriteLine("Executing finally block");
            }
        }
    }
}
