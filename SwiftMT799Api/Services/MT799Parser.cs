using SwiftMT799Api.Models;
using System.Reflection.PortableExecutable;

namespace SwiftMT799Api.Services
{
    public class MT799Parser
    {
        public MT799Message Parse(string fileContent)
        {
            var message = new MT799Message();

            Dictionary<string, string> Field = ParseBlocks(fileContent);

            message.UpdateFields(Field);

            return message;
        }

        static Dictionary<string, string> ParseBlocks(string input)
        {
            List<string> blocks = new List<string>();
            Stack<int> bracketStack = new Stack<int>();
            Dictionary<string, string> Field = new Dictionary<string, string>();

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (c == '{')
                {
                    bracketStack.Push(i);
                }
                else if (c == '}')
                {
                    if (bracketStack.Count > 0)
                    {
                        int startIndex = bracketStack.Pop();

                        string block = input.Substring(startIndex, i - startIndex + 1);

                        if (bracketStack.Count == 0)
                        {
                            block = block.Replace("{", "");
                            block = block.Replace("}", "");
                            block = block.Trim();
                            if (block[0].ToString() == "4" || block[0].ToString() == "5")
                            {
                                var Things = ExtractFields(block.Substring(2));
                                foreach (var element in Things)
                                {
                                    Field.Add(element.Key, element.Value);
                                }
                            }
                            Field.Add(block[0].ToString(), block.Substring(2));

                        }
                    }
                }
            }

            return Field;
        }

        static Dictionary<string, string> ExtractFields(string input)
        {
            Dictionary<string, string> fields = new Dictionary<string, string>();
            string[] fieldLabels = { "20:", "21:", "79:", "MAC:", "CHK:" };

            for (int i = 0; i < fieldLabels.Length; i++)
            {
                int startIndex = input.IndexOf(fieldLabels[i]);
                if (startIndex != -1)
                {
                    startIndex += fieldLabels[i].Length;

                    int endIndex = input.Length;

                    for (int j = i + 1; j < fieldLabels.Length; j++)
                    {
                        int nextFieldIndex = input.IndexOf(fieldLabels[j], startIndex);
                        if (nextFieldIndex != -1)
                        {
                            endIndex = nextFieldIndex;
                            break;
                        }
                    }

                    string fieldContent = input.Substring(startIndex, endIndex - startIndex).Trim();

                    fieldContent = fieldContent.Replace(":", "").Trim();

                    fields[fieldLabels[i].Trim(':')] = fieldContent;
                }
            }

            return fields;
        }
    }
}
