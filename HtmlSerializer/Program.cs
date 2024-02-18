// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Channels;
using System.Security.Cryptography.X509Certificates;

namespace HtmlSerializer
{

    class Program
    {
       

        public static void PrintTree(HtmlElement element)
        {
            // Print the current element
            Console.WriteLine(element.ToString());

            // Recursively print the children
            foreach (var child in element.Children)
            {
                PrintTree(child);
            }
        }
        async static Task<string> Load(string url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            var html = await response.Content.ReadAsStringAsync();
            return html;
        }
        public static void printSelector(Selector selector)
        {
            if (selector == null)
                return;
            Console.WriteLine(selector.ToString());
            printSelector(selector.Child);
        }

        static async Task Main(string[] args)
        {
            var html = await Load("https://learn.malkabruk.co.il/");
           // var regex = new Regex(@"\s+");
            var htmlLines = new Regex("<(.*?)>").Split(html).Select(line => Regex.Replace(line, @"\s+", " ")) .Where(x => x.Length > 1);
            HtmlElement rootElement = null;
            HtmlElement currentElement = null;
            List<string> lines = htmlLines.ToList();
            foreach (var line in lines)
            {
                var firstWord = line.Split(' ')[0];
                if (firstWord == "/html")
                {
                    break;
                }

                if (firstWord.StartsWith("/"))
                {
                    if(currentElement.Parent!= null) 
                    currentElement = currentElement?.Parent;
                }
                else if (HtmlHelper.Instance.AllTags.Contains(firstWord))
                {
                    // Opening tag - create a new HtmlElement
                    var newElement = new HtmlElement(firstWord);

                    // Parse attributes
                    var attributeMatch = Regex.Match(line, "([a-zA-Z]+)=\"([^\"]+)\"");
                    while (attributeMatch.Success)
                    {
                        var attributeName = attributeMatch.Groups[1].Value;
                        var attributeValue = attributeMatch.Groups[2].Value;
                        newElement.AddAttribute(attributeName, attributeValue);

                        // Check if the attribute is a class
                        if (attributeName.ToLower() == "class")
                        {
                            newElement.Classes.AddRange(attributeValue.Split(' '));
                        }

                        attributeMatch = attributeMatch.NextMatch();
                    }

                    // Set Name and Id
                    newElement.Name = firstWord;
                    newElement.Id = newElement.Attributes.Find(attr => attr.Name.ToLower() == "id")?.Value;

                    // Update Parent and Children
                    if (currentElement != null)
                    {
                        currentElement.AddChild(newElement);
                        newElement.Parent = currentElement;
                    }
                    else
                    {
                        // If there's no current element, this is the root element
                        rootElement = newElement;
                    }

                    currentElement = newElement;

                    // Check if it's a self-closing tag
                    if (line.EndsWith("/") || HtmlHelper.Instance.SelfClosingTags.Contains(firstWord))
                    {
                        // Move back to the parent for self-closing tags
                        if(currentElement.Parent != null) 
                            currentElement = currentElement.Parent;
                    }
                }
                else
                {
                    // Inner text content
                    if (currentElement != null)
                    {
                        currentElement.InnerHtml+=line;
                    }
                }
            }
            
            PrintTree(rootElement);
            Selector root = Selector.ParseSelectorString("div div a.home-hero-button1 button");
            printSelector(root);
            HashSet<HtmlElement> result = new HashSet<HtmlElement>();
            result=rootElement.FindElementsBySelector(root);
            await Console.Out.WriteLineAsync("------res--------");
            Console.WriteLine(result.Count());
            result.ToList().ForEach(r=>Console.WriteLine(r.ToString()));
        }
    }
}

