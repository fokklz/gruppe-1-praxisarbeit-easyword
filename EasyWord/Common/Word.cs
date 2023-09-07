using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWord.Common
{
    public class Word
    {
        /// <summary>
        /// english translation
        /// </summary>
        private string _english = "";
        /// <summary>
        /// german translation
        /// </summary>
        private string _german = "";
        /// <summary>
        /// knowledge level
        /// </summary>
        private int _bucket = 3;

        /// <summary>
        /// The Constructor, which will set the english and german translation of every word provided
        /// </summary>
        /// <param name="german"></param>
        /// <param name="english"></param>

        public Word(string german, string english)
        {
            _english = english;
            _german = german;
        }

        /// <summary>
        /// Get english translation
        /// </summary>
        public string English { get { return _english; } }
        /// <summary>
        /// Get german translation
        /// </summary>
        public string German { get { return _german; } }
        /// <summary>
        /// get/set bucket
        /// </summary>
        public int Bucket { get { return _bucket;} set { _bucket = value; } }

    }
}
