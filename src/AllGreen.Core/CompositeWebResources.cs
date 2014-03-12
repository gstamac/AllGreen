using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AllGreen.Core
{
    public class CompositeWebResources : IWebResources
    {
        List<IWebResources> _ResourcesList = new List<IWebResources>();

        public void Add(IWebResources webResources)
        {
            _ResourcesList.Add(webResources);
        }

        public string GetContent(string path)
        {
            foreach (IWebResources webResources in _ResourcesList)
            {
                string content = webResources.GetContent(path);
                if (content != null) return content;
            }

            return null;
        }


        public string GetSystemFilePath(string path)
        {
            foreach (IWebResources webResources in _ResourcesList)
            {
                string content = webResources.GetSystemFilePath(path);
                if (content != null) return content;
            }

            return null;
        }
    }
}
