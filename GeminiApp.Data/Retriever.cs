using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace GeminiApp.Data
{
    public class Retriever
    {
        private readonly List<string> _knowledgeBase;

        public Retriever()
        {
            _knowledgeBase = new List<string>();
        }

        public void AddDocument(string document)
        {
            _knowledgeBase.Add(document);
        }

        public string RetrieveRelevantDocument(string query)
        {
            foreach (var doc in _knowledgeBase)
            {
                if (doc.Contains(query))
                {
                    return doc;
                }
            }

            return string.Empty; // No match found
        }
    }
}
