using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiApp.Data.Models
{
    public class Question<T>
    {
        public T Content { get; set; }
        public Question(T content)
        {
            Content = content;
        }
    }
}
