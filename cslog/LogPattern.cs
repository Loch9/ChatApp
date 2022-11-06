using System;
using System.Collections.Generic;

namespace cslog
{
    public class LogPattern
    {
        public string Pattern;

        /// <summary>
        /// Create a pattern to log with.
        ///
        /// Key:
        ///
        /// !v  The actual text to log.
        /// !t  The time in HH:mm:ss format (24 hour).
        /// !T  The time in HH:mm:ss format (12 hour).
        /// !n  The name of the logger.
        /// !d  The date in DD:MM:YYYY format.
        /// !D  The short date in DD:MM:YY format.
        /// !a  The day of the week (e.g. Monday).
        /// !A  The short day of the week (e.g. Mon).
        /// !m  The month (e.g September).
        /// !M  The short month (e.g. Sep).
        /// !k  The month 01-12.
        /// !K  The day of month 01-31.
        /// !h  Hours in 24 hour format.
        /// !H  Hours in 12 hour format.
        /// !p  Minutes 00-59.
        /// !P  Seconds 00-59.
        /// !q  AM/PM.
        /// !r  The 24 hour time in HH:MM.
        /// !b  The log level
        /// !@  Start colour.
        /// !*  End colour.
        /// !!  The '!' sign.
        /// </summary>
        /// <param name='pattern'></param>
        public LogPattern(string pattern)
        {
            Pattern = pattern;
        }

        public string Parse(string message, LogLevel logLevel, Logger fromLogger)
        {
            string parsed = "";

            char skipChar = '\u200b';
            for (int i = 0; i < Pattern.Length; i++)
            {
                if (Pattern.ToCharArray()[i] == '!')
                {
                    switch (Pattern.ToCharArray()[i + 1])
                    {
                        case 'v':
                        {
                            parsed += message;
                            skipChar = 'v';
                            break;
                        }
                        case 't':
                        {
                            DateTime time = DateTime.Now;
                            parsed += time.ToString("HH:mm:ss");
                            skipChar = 't';
                            break;
                        }
                        case 'T':
                        {
                            DateTime time = DateTime.Now;
                            parsed += time.ToString("hh:mm:ss");
                            skipChar = 'T';
                            break;
                        }
                        case 'n':
                        {
                            parsed += fromLogger.Name;
                            skipChar = 'n';
                            break;
                        }
                        case 'd':
                        {
                            DateTime date = DateTime.Now;
                            parsed += date.ToString("dd/MM/yyyy");
                            skipChar = 'd';
                            break;
                        }
                        case 'D':
                        {
                            DateTime date = DateTime.Now;
                            parsed += date.ToString("dd/MM/yy");
                            skipChar = 'D';
                            break;
                        }
                        case 'a':
                        {
                            DateTime date = DateTime.Now;
                            parsed += date.ToString("dddd");
                            skipChar = 'a';
                            break;
                        }
                        case 'A':
                        {
                            DateTime date = DateTime.Now;
                            parsed += date.ToString("ddd");
                            skipChar = 'A';
                            break;
                        }
                        case 'm':
                        {
                            DateTime date = DateTime.Now;
                            parsed += date.ToString("MMMM");
                            skipChar = 'm';
                            break;
                        }
                        case 'M':
                        {
                            DateTime date = DateTime.Now;
                            parsed += date.ToString("MMM");
                            skipChar = 'M';
                            break;
                        }
                        case 'k':
                        {
                            DateTime date = DateTime.Now;
                            parsed += date.ToString("M");
                            skipChar = 'k';
                            break;
                        }
                        case 'K':
                        {
                            DateTime date = DateTime.Now;
                            parsed += date.ToString("dd");
                            skipChar = 'K';
                            break;
                        }
                        case 'h':
                        {
                            DateTime time = DateTime.Now;
                            parsed += time.ToString("HH");
                            skipChar = 'h';
                            break;
                        }
                        case 'H':
                        {
                            DateTime time = DateTime.Now;
                            parsed += time.ToString("hh");
                            skipChar = 'H';
                            break;
                        }
                        case 'p':
                        {
                            DateTime time = DateTime.Now;
                            parsed += time.ToString("mm");
                            skipChar = 'p';
                            break;
                        }
                        case 'P':
                        {
                            DateTime time = DateTime.Now;
                            parsed += time.ToString("ss");
                            skipChar = 'P';
                            break;
                        }
                        case 'q':
                        {
                            DateTime time = DateTime.Now;
                            parsed += time.ToString("tt");
                            skipChar = 'q';
                            break;
                        }
                        case 'r':
                        {
                            DateTime time = DateTime.Now;
                            parsed += time.ToString("t");
                            skipChar = 'r';
                            break;
                        }
                        case 'b':
                        {
                            parsed += logLevel.ToString().ToUpper();
                            skipChar = 'b';
                            break;
                        }
                        case '@':
                        {
                            parsed += GetColour(logLevel);
                            skipChar = '@';
                            break;
                        }
                        case '*':
                        {
                            parsed += "\u001b[0m";
                            skipChar = '*';
                            break;
                        }
                        case '!':
                        {
                            parsed += "!";
                            skipChar = '!';
                            break;
                        }
                        default:
                            break;
                    }
                }
                else if (Pattern.ToCharArray()[i] != skipChar)
                {
                    parsed += Pattern.ToCharArray()[i];
                }
            }

            return parsed;
        }

        public string GetColour(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.None:
                    return "";
                case LogLevel.Debug:
                    return "";
                case LogLevel.Info:
                    return "\u001b[0;32m";
                case LogLevel.Warning:
                    return "\u001b[0;33m";
                case LogLevel.Error:
                    return "\u001b[0;31m";
                case LogLevel.Fatal:
                    return "\u001b[41m\u001b[37;1m";
                default:
                    return "";
            }
        }

        public static implicit operator String(LogPattern pattern)
        {
            return pattern.Pattern;
        }

        public static implicit operator LogPattern(String str)
        {
            return new LogPattern(str);
        }
    }
}
