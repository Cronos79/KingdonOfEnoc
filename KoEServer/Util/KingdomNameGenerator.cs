using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoEServer.Util
{
    public class KingdomNameGenerator
    {
        public List<string> kingdomNames;

        public KingdomNameGenerator()
        {
            kingdomNames = new List<string>();
        }

        public string GetRandomKingdomName()
        {
            if (kingdomNames == null || !kingdomNames.Any())
            {
                throw new InvalidOperationException("Kingdom names have not been generated or the list is empty.");
            }

            Random random = new Random();
            int index = random.Next(kingdomNames.Count);
            string selectedName = kingdomNames[index];

            // Remove the name from the list to prevent reuse
            kingdomNames.RemoveAt(index);

            return selectedName;
        }

        public void GenerateKingdomNames()
        {
            int realCount = 0;
            string[] prefixes = { "Kingdom of", "Realm of", "Empire of", "City of", "Village of", "Fortress of", "Province of", "Sanctuary of", "Dominion of" };
            string[] names = {
                "Eldoria", "Draconia", "Enoc", "Valoria", "Arcania", "Thaloria", "Zerathia", "Caladorn", "Mythralis", "Solara",
                "Avaris", "Brithal", "Cindralis", "Durnovia", "Eryndor", "Falthar", "Gildoria", "Havenmoor", "Ironton", "Jorvaskr",
                "Kaeloria", "Lunaris", "Mythgard", "Nythoria", "Osthaven", "Pyrelis", "Quenndral", "Ravenspire", "Sylvaris", "Tarnath",
                "Ultharion", "Vyrndal", "Wyndoria", "Xandralis", "Ylvaris", "Zorathia", "Aeloria", "Brynthar", "Cyradon", "Dreloria",
                "Eryndel", "Frosthaven", "Gryndor", "Halvaris", "Isilmar", "Jorath", "Kryndor", "Lythoria", "Myrathis", "Nyvaris",
                "Aelthar", "Baldoria", "Cyrathis", "Dornath", "Eryndral", "Fayloria", "Gryndal", "Halthar", "Isiloria", "Jorvalis",
                "Kryndal", "Lunathis", "Mythral", "Nythalor", "Ostvalis", "Pyrelor", "Quenndor", "Raventhorn", "Sylvarin", "Tarnathis",
                "Ultharion", "Vyrndor", "Wyndralis", "Xandoria", "Ylvarin", "Zorathis", "Aelthoria", "Brithalis", "Cindralor", "Durnathis",
                "Eryndalis", "Falthoria", "Gildorin", "Havenlor", "Irontalis", "Jorvalin", "Kaelthar", "Lunathor", "Mythralis", "Nythorin",
                "Osthavenor", "Pyrelith", "Quenndalis", "Ravenshade", "Sylvarith", "Tarnalor", "Ultharionis", "Vyrndalis", "Wyndalor",
                "Xandralith", "Ylvarith", "Zorathalis", "Aeltharion", "Brithalor", "Cindralith", "Durnalor", "Eryndalith", "Faltharion",
                "Gildorion", "Havenalor", "Irontarion", "Jorvalith", "Kaeltharion", "Lunatharion", "Mythralion", "Nytharion", "Osthavenion",
                "Pyrelion", "Quenndarion", "Ravenshadeion", "Sylvarion", "Tarnalith", "Ultharionion", "Vyrndarion", "Wyndarion",
                "Xandralion", "Ylvarion", "Zoratharion", "Aeltharionis", "Britharionis", "Cindralionis", "Durnalionis", "Eryndalionis",
                "Faltharionis", "Gildorionis", "Havenarionis", "Irontarionis", "Jorvalionis", "Kaeltharionis", "Lunatharionis", "Mythralionis",
                "Nytharionis", "Osthavenionis", "Pyrelionis", "Quenndarionis", "Ravenshadeionis", "Sylvarionis", "Tarnalithionis",
                "Ultharionionis", "Vyrndarionis", "Wyndarionis", "Xandralionis", "Ylvarionis", "Zoratharionis", "Aeltharionith", "Britharionith",
                "Cindralionith", "Durnalionith", "Eryndalionith", "Faltharionith", "Gildorionith", "Havenarionith", "Irontarionith",
                "Jorvalionith", "Kaeltharionith", "Lunatharionith", "Mythralionith", "Nytharionith", "Osthavenionith", "Pyrelionith",
                "Quenndarionith", "Ravenshadeionith", "Sylvarionith", "Tarnalithionith", "Ultharionionith", "Vyrndarionith", "Wyndarionith",
                "Xandralionith", "Ylvarionith", "Zoratharionith"
};

            int count = 0;
            Random random = new Random();

            while (count < 2000)
            {
                string prefix = prefixes[random.Next(prefixes.Length)];
                string name = names[random.Next(names.Length)];

                string kingdomName = $"{prefix} {name}";
                if (!kingdomNames.Contains(kingdomName))
                {
                    kingdomNames.Add(kingdomName);
                    count++;
                }
                realCount++;
                if (realCount > 50000)
                {
                    break;
                }
            }

            Console.WriteLine($"Kingdom names generated: {kingdomNames.Count}. It took {realCount} tries.");
        }
    }
}
