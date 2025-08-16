using Autocompletion.DataStructures;

namespace Autocompletion.WordImport {
    public class ReadFile {

        //string line;

        public async Task StartReading(Trie trie) {
            try {
                string projectDir = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;
                string filePath = Path.Combine(projectDir, "WordImport/words.txt");

                if(!File.Exists(filePath)) {
                    Console.WriteLine($"File not found: {filePath}");
                    return;
                }

                using StreamReader streamReader = new StreamReader(filePath);

                string? line;

                //StreamReader streamReader = new StreamReader("words.txt");

                //line = streamReader.ReadLine();

                while ((line = await streamReader.ReadLineAsync()) != null) {
                    string[] words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    
                    foreach(var word in words) {
                        trie.Insert(word.Trim());
                    }
                }

                var allWords = trie.GetAllWords();

                streamReader.Close();
            } catch (Exception exception) {
                Console.WriteLine("Exception: " + exception.Message);
            } finally {
            }
        }
    }
}
