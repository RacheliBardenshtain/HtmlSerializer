using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    internal class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Attribute> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; }

        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }

        public HtmlElement(string name)
        {
            Name = name;
            Attributes = new List<Attribute>();
            Classes = new List<string>();
            Children = new List<HtmlElement>();
        }

        public void AddAttribute(string name, string value)
        {
            Attributes.Add(new Attribute(name, value));
        }

        public void AddClass(string classValue)
        {
            Classes.Add(classValue);
        }

        public void AddChild(HtmlElement child)
        {
            Children.Add(child);
            child.Parent = this;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<").Append(Name);

            if (Attributes.Count > 0)
            {
                foreach (var attribute in Attributes)
                {
                    builder.Append(" ").Append(attribute.ToString());
                }
            }

            if (Classes.Count > 0)
            {
                builder.Append(" class=\"");
                builder.Append(string.Join(" ", Classes));
                builder.Append("\"");
            }

            if (!string.IsNullOrEmpty(InnerHtml))
            {
                builder.Append(">").Append(InnerHtml).Append("</").Append(Name).Append(">");
            }
            else
            {
                builder.Append("/>");
            }

            return builder.ToString();
        }

        public HashSet<HtmlElement> FindElementsBySelector(Selector selector)
        {
            HashSet<HtmlElement> result = new HashSet<HtmlElement>();
            FindElementsRecursively(this, selector, result);
            return result;
        }

        private void FindElementsRecursively(HtmlElement currentElement, Selector selector, HashSet<HtmlElement> result)
        {
            var descendants = currentElement.Descendants();
            var filteredDescendants = descendants.Where(e => isIdentify(e, selector));
            filteredDescendants.ToList().ForEach(d=>result.Add(d));
            foreach (var descendant in filteredDescendants)
            {
                FindElementsRecursively(descendant, selector.Child, result);
            }
        }
        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);
            HtmlElement help;
            while (queue.Count > 0)
            {
                help = queue.Dequeue();
                yield return help;
                for (int i = 0; i < help.Children.Count; i++)
                {
                    queue.Enqueue(help.Children[i]);
                }
            }
        }
        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement htmlElement = this;

            while (htmlElement != null)
            {
                yield return htmlElement;
                htmlElement = htmlElement.Parent;
            }
        }

        public bool isIdentify(HtmlElement htmlElement, Selector selector)
        {
            if (selector != null)
            {

                if (selector.TagName != "" && selector.TagName != htmlElement.Name)
                    return false;
                if (selector.Id != "" && selector.Id != htmlElement.Id)
                    return false;
                if (htmlElement.Classes != null)
                {
                    if (!selector.Classes.All(c => htmlElement.Classes.Contains(c)))
                        return false;
                }
                return true;
            }
            return false;
        }
    }

    
}

   




    

