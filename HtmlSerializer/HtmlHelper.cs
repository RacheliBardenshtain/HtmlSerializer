using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace HtmlSerializer
{
    internal class HtmlHelper
    {
        private readonly static HtmlHelper _instance = new HtmlHelper();

        public static HtmlHelper Instance => _instance;
        public string[] AllTags { get; set; }

        public string[] SelfClosingTags { get; set; }


        private HtmlHelper()
        {
            try
            {
                var content = File.ReadAllText("JsonFiles/AllTags.json");
                AllTags = JsonSerializer.Deserialize<string[]>(content);

                var content2 = File.ReadAllText("JsonFiles/SelfClosingTags.json");
                SelfClosingTags = JsonSerializer.Deserialize<string[]>(content2);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
       
    }
}