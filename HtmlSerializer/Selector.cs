using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    internal class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }

        public Selector Child { get; set; }

        public Selector Parent { get; set; }

        public List<string> Classes { get; set; }

        public Selector()
        {
            Classes = new List<string>();
        }

        public static Selector ParseSelectorString(string selectorString)
        {
            Selector rootSelector = new Selector();
            Selector currentSelector = rootSelector;
            string[] parts = selectorString.Split(' ');

            foreach (string part in parts)
            {
                Console.WriteLine("part: "+part);
                string[] qualities = part.Split('#', '.');
                string level = part;
                foreach (string quality in qualities)
                {
                    Console.WriteLine("quality: "+quality);
                    Console.WriteLine("level: "+level);
                    if (level.StartsWith('#'))
                    {
                        currentSelector.Id = quality;
                        level = level.Substring(quality.Length + 1);
                    }

                    if (level.StartsWith('.'))
                    {
                        currentSelector.Classes.Add(quality);
                        level=level.Substring(quality.Length + 1);
                    }
                    else
                    {
                        if (HtmlHelper.Instance.AllTags.Contains(quality))
                        {
                            level = level.Substring(quality.Length);
                            currentSelector.TagName = quality;
                        }
                        else
                            Console.WriteLine("invalid parameter");
                    }
                }
                // Create a new selector for the child
                if (currentSelector.Parent == null)
                    rootSelector = currentSelector;
                Selector childSelector = new Selector();
                currentSelector.Child = childSelector;
                childSelector.Parent = currentSelector;
                currentSelector = childSelector;
            }
            currentSelector.Parent.Child = null;
            return rootSelector;
        }

        public override string ToString()
        {
            string str = $"TagName: {TagName} ,Id: {Id}  classes: ";
            if (Classes != null)
            {
                for (int i = 0; i < Classes.Count(); i++)
                {
                    str += Classes[i].ToString() + " ";
                }
            }
            return str;
        }
    }

}

