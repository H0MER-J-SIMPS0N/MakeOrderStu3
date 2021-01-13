using MakeOrderStu3.Models.QuestionnaireClasses;
using MakeOrderStu3.Singletones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MakeOrderStu3.Models
{
    public static class Validate
    {
        public static bool Eta(string contractCode, Nomenclature nomenclature)
        {
            return new Regex(@"^(C\d{9})$").IsMatch(contractCode) 
                && nomenclature != null
                && EtaList.Get(contractCode)
                    .Where(y => y.Id == nomenclature.Id
                        && y.Status == "stopped"
                        && y.EffectivePeriod.Start < DateTime.Now
                        && y.EffectivePeriod.End > DateTime.Now)
                    .Count() == 0;
        }

        public static bool Answer(object answer, string type)
        {            
            if (type == "string")
            {
                return !string.IsNullOrWhiteSpace((string)answer);
            }
            else if (type == "integer")
            {
                return int.TryParse((string)answer, out _);
            }
            else if (type == "boolean")
            {
                return ((string)answer).ToUpper() == "Y" || ((string)answer).ToUpper() == "N";
            }
            else if (type == "date")
            {
                return DateTime.TryParse((string)answer, out _);
            }
            else if (type == "decimal")
            {
                return decimal.TryParse((string)answer, out _);
            }
            else if (type == "choice")
            {
                QuestionnaireOption x = answer as QuestionnaireOption;
                return x != null;
            }
            return true;
        }
    }
}
