using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.App_Code
{
    public static class Static
    {
        public static string GetMesAbreviaturaByIndex(int index)
        {
            switch (index)
            {
                case 1: return "ENE"; break;
                case 2: return "FEB"; break;
                case 3: return "MAR"; break;
                case 4: return "ABR"; break;
                case 5: return "MAY"; break;
                case 6: return "JUN"; break;
                case 7: return "JUL"; break;
                case 8: return "AGO"; break;
                case 9: return "SET"; break;
                case 10: return "OCT"; break;
                case 11: return "NOV"; break;
                case 12: return "DIC"; break;
                default:
                    break;
            }
            return string.Empty;
        }

        public static double getNumber(string str)
        {
            double n;
            if (double.TryParse(str, out n))
            {
                return n;
            }
            return 0.00;
        }
    }
}