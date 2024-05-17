using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace NutzerVerwaltung
{
    public class Nutzer
    {
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string Geburtstag { get; set; }

        public static List<Nutzer> NutzerListe { get; set; } = new List<Nutzer>();

        public Nutzer()
        { }

        public override string ToString()
        {
            return $"Vorname: {Vorname}, Nachname: {Nachname}, Geburtstag: {Geburtstag}";
        }

        public static void NutzerAusDateiLaden(string dateiPfad)
        {
            if (File.Exists(dateiPfad))
            {
                string[] zeilen = File.ReadAllLines(dateiPfad);
                foreach (string zeile in zeilen)
                {
                    string[] daten = zeile.Split(',');
                    if (daten.Length == 3)
                    {
                        Nutzer nutzer = new Nutzer
                        {
                            Vorname = daten[0],
                            Nachname = daten[1],
                            Geburtstag = daten[2]
                        };
                        NutzerListe.Add(nutzer);
                    }
                }
            }
        }

        public static void NutzerInDateiSpeichern(string dateiPfad)
        {
            using (StreamWriter writer = new StreamWriter(dateiPfad, false))
            {
                foreach (Nutzer nutzer in NutzerListe)
                {
                    writer.WriteLine($"{nutzer.Vorname},{nutzer.Nachname},{nutzer.Geburtstag}");
                }
            }
        }
    }

    public static class Menue
    {
        private static readonly string Dateiname = "nutzerdaten.txt";
        private static readonly string Dateipfad = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Dateiname);

        public static void Start()
        {
            if (!File.Exists(Dateipfad))
            {
                File.Create(Dateipfad).Close(); // erstellt die datei, falls sie noch nicht existieren sollte
            }

            Nutzer.NutzerAusDateiLaden(Dateipfad);
            bool beenden = false;

            while (!beenden)
            {
                Console.WriteLine("Willkommen im Nutzer-Verwaltungssystem!");
                Console.WriteLine("1. Nutzer hinzufügen");
                Console.WriteLine("2. Nutzer anzeigen");
                Console.WriteLine("3. Beenden");
                Console.Write("Bitte wählen Sie eine Option: ");

                string eingabe = Console.ReadLine();

                switch (eingabe)
                {
                    case "1":
                        NutzerHinzufuegen();
                        Nutzer.NutzerInDateiSpeichern(Dateipfad); // Direkte speicherung der Daten
                        break;

                    case "2":
                        NutzerAnzeigen();
                        break;

                    case "3":
                        beenden = true;
                        break;

                    default:
                        Console.WriteLine("Ungültige Auswahl. Bitte versuchen Sie es erneut.");
                        break;
                }

                Console.WriteLine("\n\n");
            }
        }

        public static void NutzerHinzufuegen()
        {
            Nutzer neuerNutzer = new Nutzer();

            while (true)
            {
                Console.Write("Vorname: ");
                string vornameEingabe = Console.ReadLine();

                if (IstGueltigerName(vornameEingabe))
                {
                    neuerNutzer.Vorname = vornameEingabe;
                    break;
                }
                else
                {
                    Console.WriteLine("Falsche Eingabe. Bitte geben Sie nur Buchstaben ein.");
                }
            }

            while (true)
            {
                Console.Write("Nachname: ");
                string nachnameEingabe = Console.ReadLine();

                if (IstGueltigerName(nachnameEingabe))
                {
                    neuerNutzer.Nachname = nachnameEingabe;
                    break;
                }
                else
                {
                    Console.WriteLine("Falsche Eingabe. Bitte geben Sie nur Buchstaben ein.");
                }
            }

            while (true)
            {
                Console.Write("Geburtstag (dd.mm.yyyy): ");
                string geburtstagEingabe = Console.ReadLine();

                if (IstGueltigesGeburtsdatum(geburtstagEingabe))
                {
                    neuerNutzer.Geburtstag = geburtstagEingabe;
                    break;
                }
                else
                {
                    Console.WriteLine("Falsche Eingabe, bitte geben Sie ihr Geburtsdatum im Format dd.mm.yyyy ein.");
                }
            }

            Nutzer.NutzerListe.Add(neuerNutzer);

            Console.WriteLine("Nutzer erfolgreich hinzugefügt!");
        }

        private static bool IstGueltigerName(string eingabe)
        {
            foreach (char c in eingabe)
            {
                if (!char.IsLetter(c))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IstGueltigesGeburtsdatum(string eingabe)
        {
            if (Regex.IsMatch(eingabe, @"^(0[1-9]|[12][0-9]|3[01])\.(0[1-9]|1[0-2])\.\d{4}$"))
            {
                string[] teile = eingabe.Split('.');
                int tag = int.Parse(teile[0]);
                int monat = int.Parse(teile[1]);
                int jahr = int.Parse(teile[2]);

                if (tag >= 1 && tag <= 31 && monat >= 1 && monat <= 12 && jahr >= 1)
                {
                    try
                    {
                        DateTime datum = new DateTime(jahr, monat, tag);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public static void NutzerAnzeigen()
        {
            Console.WriteLine("Liste der Nutzer:");

            foreach (Nutzer nutzer in Nutzer.NutzerListe)
            {
                Console.WriteLine(nutzer);
            }

            if (Nutzer.NutzerListe.Count == 0)
            {
                Console.WriteLine("Keine Nutzer vorhanden.");
            }
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            Menue.Start();
        }
    }
}