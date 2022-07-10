using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace filmBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var bot = new FilmBot();
            bot.Start();
            Thread.Sleep(-1);
        }
    }
}
