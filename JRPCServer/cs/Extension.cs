using System;

using System.Text;

namespace JRPCServer
{
    using System.Globalization;

    static class Extension
    {

        public static string GetJQueryDateFormat(this CultureInfo culture)
        {
            string cs;
            string jquery;
            GetDateFormats(culture, out cs, out jquery);
            return jquery;

        }

        ///========================================================================
        ///  Method : ConvertDateFormat
        /// 
        /// <summary>
        ///   Takes a culture and returns matching C# and jQuery date format
        ///   strings. If possible the C# string will be the ShortDatePattern for
        ///   the supplied culture.
        /// </summary>
        /// <see href="http://stackoverflow.com/a/1244587/1153203"/>
        ///========================================================================
        private static void GetDateFormats(CultureInfo xiCulture, out string xoCSharpFormat, out string xoJQueryFormat)
        {
            //=======================================================================
            // Start by assigning formats that are hopefully unambiguous in case we
            // can't do better.
            //=======================================================================
            xoCSharpFormat = "yyyy-MM-dd";
            xoJQueryFormat = "yy-mm-dd";

            if (xiCulture.IsNeutralCulture)
            {
                try
                {
                    xiCulture = CultureInfo.CreateSpecificCulture(xiCulture.Name);
                }
                catch
                {
                    //===================================================================
                    // Some cultures are neutral and don't have specific cultures.
                    // There's not much we can do here.
                    //===================================================================
                    return;
                }
            }

            string lCSharpFormat = xiCulture.DateTimeFormat.ShortDatePattern;

            //=======================================================================
            // Handle:
            //  C#     jQuery  Meaning
            //  d      d       Day of month (no leading 0)
            //  dd     dd      Day of month (leading 0)
            //  M      m       Month of year (no leading 0)
            //  MM     mm      Month of year (leading 0)
            //  yy     y       Two digit year
            //  yyyy   yy      Not an exact match but good enough:
            //                 C# means: The year in four or five digits (depending on
            //                 the calendar used), including the century. Pads with
            //                 leading zeros to get four digits. Thai Buddhist and
            //                 Korean calendars have five-digit years. Users
            //                 selecting the "yyyy" pattern see all five digits
            //                 without leading zeros for calendars that have five
            //                 digits. Exception: the Japanese and Taiwan calendars
            //                 always behave as if "yy" is selected.
            //                 jQuery means: four digit year
            //
            //  Copy '.', '-', ' ', '/' verbatim
            //  Bail out if we find anything else and return standard date format for
            //  both.
            //=======================================================================
            StringBuilder lJQueryFormat = new StringBuilder();
            bool lError = false;
            for (int ii = 0; ii < lCSharpFormat.Length; ++ii)
            {
                Char lCurrentChar = lCSharpFormat[ii];

                switch (lCurrentChar)
                {
                    case 'd':
                        //=================================================================
                        // d or dd is OK, ddd is not
                        //=================================================================
                        if (ii < (lCSharpFormat.Length - 1) &&
                          lCSharpFormat[ii + 1] == 'd')
                        {
                            if (ii < (lCSharpFormat.Length - 2) &&
                            lCSharpFormat[ii + 2] == 'd')
                            {
                                //=============================================================
                                // ddd
                                //=============================================================
                                lError = true;
                            }
                            else
                            {
                                //=============================================================
                                // dd
                                //=============================================================
                                lJQueryFormat.Append("dd");
                                ii++;
                            }
                        }
                        else
                        {
                            //===============================================================
                            // d
                            //===============================================================
                            lJQueryFormat.Append('d');
                        }
                        break;
                    case 'M':
                        //=================================================================
                        // M or MM is OK, MMM is not
                        //=================================================================
                        if (ii < (lCSharpFormat.Length - 1) &&
                          lCSharpFormat[ii + 1] == 'M')
                        {
                            if (ii < (lCSharpFormat.Length - 2) &&
                            lCSharpFormat[ii + 2] == 'M')
                            {
                                //=============================================================
                                // MMM
                                //=============================================================
                                lError = true;
                            }
                            else
                            {
                                //=============================================================
                                // MM
                                //=============================================================
                                lJQueryFormat.Append("mm");
                                ii++;
                            }
                        }
                        else
                        {
                            //===============================================================
                            // M
                            //===============================================================
                            lJQueryFormat.Append('m');
                        }
                        break;
                    case 'y':
                        //=================================================================
                        // yy or yyyy is OK, y, yyy, or yyyyy is not
                        //=================================================================
                        if (ii < (lCSharpFormat.Length - 1) &&
                          lCSharpFormat[ii + 1] == 'y')
                        {
                            if (ii < (lCSharpFormat.Length - 2) &&
                              lCSharpFormat[ii + 2] == 'y')
                            {
                                if (ii < (lCSharpFormat.Length - 3) &&
                                  lCSharpFormat[ii + 3] == 'y')
                                {
                                    if (ii < (lCSharpFormat.Length - 4) &&
                                      lCSharpFormat[ii + 4] == 'y')
                                    {
                                        //=========================================================
                                        // yyyyy
                                        //=========================================================
                                        lError = true;
                                    }
                                    else
                                    {
                                        //=========================================================
                                        // yyyy
                                        //=========================================================
                                        lJQueryFormat.Append("yy");
                                        ii = ii + 3;
                                    }
                                }
                                else
                                {
                                    //===========================================================
                                    // yyy
                                    //===========================================================
                                    lError = true;
                                }
                            }
                            else
                            {
                                //=============================================================
                                // yy
                                //=============================================================
                                lJQueryFormat.Append("y");
                                ii++;
                            }
                        }
                        else
                        {
                            //===============================================================
                            // y
                            //===============================================================
                            lError = true;
                        }
                        break;
                    case '.':
                    case '-':
                    case ' ':
                    case '/':
                        lJQueryFormat.Append(lCurrentChar);
                        break;
                    default:
                        lError = true;
                        break;
                }

                if (lError)
                {
                    break;
                }
            }

            //=======================================================================
            // If we didn't get an error return the culture specific formats
            //=======================================================================
            if (!lError)
            {
                xoCSharpFormat = lCSharpFormat;
                xoJQueryFormat = lJQueryFormat.ToString();
            }
        }
    }
}
