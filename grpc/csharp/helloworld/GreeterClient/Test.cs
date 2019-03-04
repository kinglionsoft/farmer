using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GreeterClient
{
    /// <summary>
    /// 亚洲语言格式化
    /// </summary>
    internal static class KoreaTextFormater
    {
        public static void Run(int count)
        {
            var line = new string(' ', 300);// string.Join(" ", KoreaMap.Keys);

            Console.WriteLine($"文本字符数：{line.Length}");

            var sw = new Stopwatch();

            sw.Start();

            for(var i=0;i< count; i++)
            {
                Format("", line);
            }

            sw.Stop();

            Console.WriteLine($"替换{count}次，耗时：{sw.ElapsedMilliseconds} ms, {sw.ElapsedTicks} ticks");
        }


        public static string Format(string source, string destination)
        {
            var sb = new StringBuilder(destination);
            foreach (var item in KoreaMap)
            {
                sb.Replace(item.Key, item.Value);
            }

            return sb.ToString();
        }

        #region 将朝鲜语单词转换为韩语

        private static readonly Dictionary<string, string> KoreaMap
            = new Dictionary<string, string>
        {
            {"에네르기","에너지"},
            {"설치되여","설치되어"},
            {"휴대되여","휴대되어"},
            {"수정전","수정후"},
            {"랭음극","냉음극"},
            {"되였다","되었다"},
            {"로동자","노동자"},
            {"락동강","낙동강"},
            {"류사","유사"},
            {"리유","이유"},
            {"례를","예를"},
            {"색갈","색깔"},
            {"비률","비율"},
            {"리용","이용"},
            {"록색","녹색"},
            {"련결","연결"},
            {"루출","유출"},
            {"련루","연루"},
            {"략칭","약칭"},
            {"배렬","배열"},
            {"되여","되어"},
            {"로동","노동"},
            {"령도","영도"},
            {"로출","노출"},
            {"로인","노인"},
            {"량심","양심"},
            {"규률","규율"},
            {"녀자","여자"},
            {"선렬","선열"},
            {"력사","역사"},
            {"락원","낙원"},
            {"년금","연금"},
            {"녈반","열반"},
            {"념려","염려"},
            {"녕악","영악"},
            {"녕변","영변"},
            {"뇨도","요도"},
            {"뉴대","유대"},
            {"뉵혈","육혈"},
            {"니암","이암"},
            {"닉명","익명"},
            {"닉사","익사"},
            {"닐시","일시"},
            {"라렬","나열"},
            {"락랑","낙랑"},
            {"락양","낙양"},
            {"락하","낙하"},
            {"란초","난초"},
            {"람색","남색"},
            {"랍치","납치"},
            {"랑비","낭비"},
            {"래일","내일"},
            {"랭기","냉기"},
            {"략탈","약탈"},
            {"량민","양민"},
            {"려행","여행"},
            {"력학","역학"},
            {"렬등","열등"},
            {"렴치","염치"},
            {"렵총","엽총"},
            {"령리","영리"},
            {"례외","예외"},
            {"록음","녹음"},
            {"록용","녹용"},
            {"론쟁","논쟁"},
            {"롱담","농담"},
            {"뢰우","뇌우"},
            {"료녕","요령"},
            {"료리","요리"},
            {"룡천","용천"},
            {"루각","누각"},
            {"류출","유출"},
            {"륙지","육지"},
            {"륜리","윤리"},
            {"률법","율법"},
            {"륭성","융성"},
            {"륵골","늑골"},
            {"름름","늠름"},
            {"릉가","능가"},
            {"리과","이과"},
            {"리천","이천"},
            {"린접","인접"},
            {"림시","임시"},
            {"림야","임야"},
            {"립증","입증"},
            {"련락","연락"}
        };

        #endregion
    }
}
