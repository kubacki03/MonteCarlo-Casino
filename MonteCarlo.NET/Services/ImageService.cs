using System;
using System.Text.RegularExpressions;
using Tesseract;
namespace MonteCarlo.NET.Services
{
    public class ImageService
    {
        public static Boolean SprawdzWiek(string imagePath)
        {
            string tessDataPath = "C:\\Users\\Kuba\\source\\repos\\montecarlo.net\\MonteCarlo.NET\\tessdata\\";  
            Console.WriteLine(System.IO.Directory.Exists(tessDataPath)); // Powinno zwrócić True

            try
            {
               
                using (var engine = new TesseractEngine(tessDataPath, "pol", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(imagePath))
                    {
                      
                        using (var page = engine.Process(img))
                        {
                            string recognizedText = page.GetText();
                            Console.WriteLine("Rozpoznany tekst:");
                            Console.WriteLine(recognizedText);


                            var dateMatch = Regex.Match(recognizedText, @"\b3\.\s*(\d{2}\.\d{2}\.\d{4})\b");
                            if (dateMatch.Success)
                            {
                              
                                string date = dateMatch.Groups[1].Value;
                                Console.WriteLine($"Znaleziono datę: {date}");
                                DateOnly data = DateOnly.Parse(date);
                                if (data.AddYears(18) <= DateOnly.FromDateTime(DateTime.Now))
                                {
                                    return true;
                                }
                                return false;
                            }
                            else
                            {
                                Console.WriteLine("Nie znaleziono daty w odpowiednim formacie.");
                               
                            }

                      
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
            }

            return false;
        }
    }
}
