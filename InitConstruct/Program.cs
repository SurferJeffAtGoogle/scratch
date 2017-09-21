using System;

namespace InitConstruct
{
    class C 
    {
        private string _s;
        public string S
        {
            get { return _s;}
            set 
            { 
                Console.WriteLine("Setting S to " + value);
                _s  = value;
            }
        }
        
        public C(string s) { 
            _s = s; 
            Console.WriteLine("Constructing C with " + s);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var c = new C("A") {
                S = "B"
            };
            Console.WriteLine("c.S: " + c.S);
        }
    }
}
