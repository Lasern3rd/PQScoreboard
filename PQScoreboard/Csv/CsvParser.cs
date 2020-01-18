using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace PQScoreboard
{
    public class CsvParser
    {
        private string buffer;
        private int pos;

        public static string[][] Parse(string buffer, bool withHeader, out string[] header)
        {
            CsvParser parser = new CsvParser(buffer);

            if (withHeader)
            {
                header = parser.Row();
            }
            else
            {
                header = null;
            }

            List<string[]> rows = new List<string[]>();
            while (!parser.Eof())
            {
                rows.Add(parser.Row());
            }

            return rows.ToArray();
        }

        private CsvParser(string buffer)
        {
            this.buffer = buffer;
            pos = 0;
        }

        private string[] Row()
        {
            bool eol = false;

            List<string> header = new List<string>();

            while (!eol)
            {
                header.Add(Field(out eol));
            }

            return header.ToArray();
        }

        private string Field(out bool eol)
        {
            eol = false;

            char c = buffer[pos];
            bool quoted = c == '"';
            int start = quoted ? ++pos : pos;

            if (quoted)
            {
                while (buffer[pos++] != '"') ;
                c = buffer[pos++];
                if (c != ',')
                {
                    if (c == '\n')
                    {
                        eol = true;
                    }
                    else
                    {
                        throw new ArgumentException("missing delimiter");
                    }
                }
            }
            else
            {
                while (!IsEndOfField(c = buffer[pos++])) ;
                eol = c == '\n';
            }

            return buffer.Substring(start, pos - start - (quoted ? 2 : 1));
        }

        private bool Eof()
        {
            return pos >= buffer.Length;
        }

        private static bool IsEndOfField(char c)
        {
            // meh
            return c == ',' || c == '\n';
        }
    }
}
