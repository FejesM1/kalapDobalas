using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using kalapDobalas.Models;

namespace KalaplengetoVerseny
{
    class Program
    {
        static string kapcsolatString = "server=localhost;user=root;password=;database=szelesbalas_kalap;";
        static string htmlFajlNeve = "statisztika.html";

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== SZELESBALASI VERSENY NYILVANTARTO ===");
                Console.WriteLine("1. Uj versenyzo felvetele");
                Console.WriteLine("2. Eredmeny rogzitese (Pont es Ido)");
                Console.WriteLine("3. Rangsor frissitese a TV-n (HTML generalas)");
                Console.WriteLine("4. Versenyzo torlese");
                Console.WriteLine("0. Kilepes");
                Console.WriteLine("=========================================");
                Console.Write("Valasszon menupontot: ");

                string valasztas = Console.ReadLine();

                if (valasztas == "1")
                {
                    UjVersenyzoFelvetele();
                }
                else if (valasztas == "2")
                {
                    EredmenyRogzitese();
                }
                else if (valasztas == "3")
                {
                    AdatokBetolteseEsHtmlGeneralas();
                }
                else if (valasztas == "4")
                {
                    VersenyzoTorlese();
                }
                else if (valasztas == "0")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Ervenytelen valasztas! Nyomjon Entert.");
                    Console.ReadLine();
                }
            }
        }

        static void UjVersenyzoFelvetele()
        {
            Console.WriteLine("\n--- Uj versenyzo ---");
            Console.Write("Kerem a nevet: ");
            string nev = Console.ReadLine();

            string sql = $"INSERT INTO versenyzok (nev, pont1, ido1, pont2, ido2, pont3, ido3) VALUES ('{nev}', 0, 99.99, 0, 99.99, 0, 99.99)";

            AdatbazisParancsVegrehajtas(sql);
            Console.WriteLine("[OK] Versenyzo felveve. Nyomjon Entert.");
            Console.ReadLine();
        }

        static void EredmenyRogzitese()
        {
            ListaKiirasaKonzolra();

            Console.WriteLine("\n--- Eredmeny beirasa ---");
            Console.Write("Adja meg a versenyzo ID-jet (szamat): ");
            string idStr = Console.ReadLine();

            Console.Write("Melyik kor? (1, 2 vagy 3): ");
            string kor = Console.ReadLine();

            if (kor != "1" && kor != "2" && kor != "3")
            {
                Console.WriteLine("[HIBA] Csak 1, 2 vagy 3 lehet a kor szama.");
                Console.ReadLine();
                return;
            }

            Console.Write("Elert pontszam (0-10): ");
            string pont = Console.ReadLine();

            Console.Write("Elert ido (pl. 3,45): ");
            string ido = Console.ReadLine().Replace(".", ",");

            string sql = $"UPDATE versenyzok SET pont{kor} = {pont}, ido{kor} = {ido} WHERE id = {idStr}";

            AdatbazisParancsVegrehajtas(sql);
            Console.WriteLine($"[OK] A(z) {kor}. kor eredmenyei mentve. Nyomjon Entert.");
            Console.ReadLine();
        }

        static void AdatokBetolteseEsHtmlGeneralas()
        {
            Console.WriteLine("\nAdatok betoltese es szamolasa...");
            List<Versenyzo> versenyzokLista = new List<Versenyzo>();

            try
            {
                using (MySqlConnection kapcsolat = new MySqlConnection(kapcsolatString))
                {
                    kapcsolat.Open();
                    string sql = "SELECT * FROM versenyzok";
                    MySqlCommand parancs = new MySqlCommand(sql, kapcsolat);
                    MySqlDataReader olvaso = parancs.ExecuteReader();

                    while (olvaso.Read())
                    {
                        Versenyzo v = new Versenyzo();
                        v.Id = Convert.ToInt32(olvaso["id"]);
                        v.Nev = olvaso["nev"].ToString();

                        v.Pont1 = Convert.ToInt32(olvaso["pont1"]);
                        v.Ido1 = Convert.ToDouble(olvaso["ido1"]);

                        v.Pont2 = Convert.ToInt32(olvaso["pont2"]);
                        v.Ido2 = Convert.ToDouble(olvaso["ido2"]);

                        v.Pont3 = Convert.ToInt32(olvaso["pont3"]);
                        v.Ido3 = Convert.ToDouble(olvaso["ido3"]);

                        SzamitasElvegzese(v);

                        versenyzokLista.Add(v);
                    }
                }
            }
            catch (Exception hiba)
            {
                Console.WriteLine("[HIBA] Nem sikerult kapcsolodni az adatbazishoz!");
                Console.WriteLine(hiba.Message);
                Console.ReadLine();
                return;
            }

            var rendezettLista = versenyzokLista
                .OrderByDescending(x => x.LegjobbPontszam)
                .ThenBy(x => x.LegjobbIdo)
                .ThenBy(x => x.Nev)
                .ToList();

            try
            {
                string htmlTartalom = File.ReadAllText(htmlFajlNeve);

                StringBuilder ujTablazatSorok = new StringBuilder();
                int helyezes = 1;

                foreach (var v in rendezettLista)
                {
                    ujTablazatSorok.AppendLine("<tr>");
                    ujTablazatSorok.AppendLine($"   <td class='helyezes'>{helyezes}.</td>");
                    ujTablazatSorok.AppendLine($"   <td>{v.Nev}</td>");
                    ujTablazatSorok.AppendLine($"   <td>{v.Pont1}</td>");
                    ujTablazatSorok.AppendLine($"   <td>{v.Pont2}</td>");
                    ujTablazatSorok.AppendLine($"   <td>{v.Pont3}</td>");
                    ujTablazatSorok.AppendLine($"   <td style='font-weight:bold; color:red;'>{v.LegjobbPontszam}</td>");
                    ujTablazatSorok.AppendLine("</tr>");
                    helyezes++;
                }

                string kezdoJel = "<!-- START_EREDMENY -->";
                string vegJel = "<!-- END_EREDMENY -->";

                int kezdetIndex = htmlTartalom.IndexOf(kezdoJel);
                int vegIndex = htmlTartalom.IndexOf(vegJel);

                if (kezdetIndex != -1 && vegIndex != -1)
                {
                    string eleje = htmlTartalom.Substring(0, kezdetIndex + kezdoJel.Length);
                    string vege = htmlTartalom.Substring(vegIndex);

                    string teljesUjHtml = eleje + "\n" + ujTablazatSorok.ToString() + "\n" + vege;

                    File.WriteAllText(htmlFajlNeve, teljesUjHtml);
                    Console.WriteLine("[OK] A statisztika.html sikeresen frissitve!");
                }
                else
                {
                    Console.WriteLine("[HIBA] Nem talalom a jeloloket () a HTML fajlban.");
                }
            }
            catch (Exception hiba)
            {
                Console.WriteLine("[HIBA] Fájl hiba: " + hiba.Message);
            }

            Console.WriteLine("Nyomjon Entert a folytatashoz.");
            Console.ReadLine();
        }

        static void AdatbazisParancsVegrehajtas(string sql)
        {
            try
            {
                using (MySqlConnection kapcsolat = new MySqlConnection(kapcsolatString))
                {
                    kapcsolat.Open();
                    MySqlCommand parancs = new MySqlCommand(sql, kapcsolat);
                    parancs.ExecuteNonQuery();
                }
            }
            catch (Exception hiba)
            {
                Console.WriteLine("[HIBA] Az adatbazis muvelet sikertelen: " + hiba.Message);
            }
        }

        static void SzamitasElvegzese(Versenyzo v)
        {
            int maxPont = Math.Max(v.Pont1, Math.Max(v.Pont2, v.Pont3));
            v.LegjobbPontszam = maxPont;

            double legjobbIdo = 99.99;

            if (v.Pont1 == maxPont && v.Ido1 < legjobbIdo) legjobbIdo = v.Ido1;
            if (v.Pont2 == maxPont && v.Ido2 < legjobbIdo) legjobbIdo = v.Ido2;
            if (v.Pont3 == maxPont && v.Ido3 < legjobbIdo) legjobbIdo = v.Ido3;

            v.LegjobbIdo = legjobbIdo;
        }

        static void ListaKiirasaKonzolra()
        {
            Console.WriteLine("\nJelenlegi versenyzok:");
            try
            {
                using (MySqlConnection kapcsolat = new MySqlConnection(kapcsolatString))
                {
                    kapcsolat.Open();
                    string sql = "SELECT id, nev FROM versenyzok";
                    MySqlCommand parancs = new MySqlCommand(sql, kapcsolat);
                    MySqlDataReader olvaso = parancs.ExecuteReader();

                    while (olvaso.Read())
                    {
                        Console.WriteLine($"ID: {olvaso["id"]} - Nev: {olvaso["nev"]}");
                    }
                }
            }
            catch (Exception) { }
        }

        static void VersenyzoTorlese()
        {
            ListaKiirasaKonzolra();
            Console.Write("\nTorlendo ID: ");
            string id = Console.ReadLine();
            string sql = $"DELETE FROM versenyzok WHERE id = {id}";
            AdatbazisParancsVegrehajtas(sql);
            Console.WriteLine("[OK] Torolve.");
            Console.ReadLine();
        }
    }
}
