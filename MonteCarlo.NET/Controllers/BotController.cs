using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]  
public class BotController : ControllerBase
{
    Dictionary<string, string> pairs = new Dictionary<string, string>();
    public BotController()
    {

        pairs.Add("Ile trzeba mieć lat", "Aby grać w gry w MonteCarlo należy mieć ukończone 18 lat");
        pairs.Add("Jak założyć konto", "Aby założyć konto w MonteCarlo, wystarczy kliknąć przycisk 'Zarejestruj się' na stronie głównej i wypełnić formularz rejestracyjny. Pamiętaj, że musisz mieć ukończone 18 lat, aby się zarejestrować.");
        pairs.Add("Jak wpłacić pieniądze na konto", "Aby wpłacić pieniądze na konto, należy zalogować się na swoje konto, przejść do sekcji 'Saldo' i wybrać opcję 'Wpłać'. Wybierz preferowaną metodę płatności i postępuj zgodnie z instrukcjami.");
        pairs.Add("Jakie metody płatności są dostępne", "W MonteCarlo akceptujemy różne metody płatności, takie jak karty kredytowe, przelewy bankowe oraz płatności za pomocą systemów takich jak PayPal czy Stripe.");
        pairs.Add("Czy mogę wypłacić pieniądze na swoje konto bankowe", "Tak, aby wypłacić pieniądze na swoje konto bankowe, należy przejść do sekcji 'Saldo', wybrać opcję 'Wypłać' i wprowadzić dane konta bankowego. Pamiętaj, że minimalna kwota wypłaty to 10zł.");
        pairs.Add("Czy mogę grać na MonteCarlo z zagranicy", "Tak, możesz grać w MonteCarlo z zagranicy, jednak przed dokonaniem wpłaty lub wypłaty sprawdź, czy Twoja metoda płatności jest obsługiwana w Twoim kraju.");
        pairs.Add("Co zrobić, jeśli zapomnialem hasła", "Jeśli zapomniałeś hasła, wystarczy kliknąć 'Nie pamiętam hasła' na stronie logowania. Otrzymasz wiadomość e-mail z instrukcjami, jak zresetować hasło.");
        pairs.Add("Co lepsze java czy c#", "Ofc ze java");
        pairs.Add("Czy moje dane są bezpieczne", "Tak, bezpieczeństwo danych naszych użytkowników jest dla nas priorytetem. Korzystamy z najnowszych technologii szyfrowania, aby zapewnić pełną ochronę Twoich danych.");
        pairs.Add("Jakie gry są dostępne w MonteCarlo", "W MonteCarlo oferujemy szeroki wybór gier, w tym automaty, ruletkę, poker i inne gry kasynowe. Znajdziesz je w sekcji 'Gry' na naszej stronie.");

    }

 

    [HttpGet]
    [Route("getResponse")]
    public string getResponse(string message)
    {
        var response = getBestResponse(message);  
        return response;
    }


    public string getBestResponse(string message)
    {
        string bestResponse = "Sprobuj ponownie";
        var max = 0.0;
        foreach (var item in pairs)
        {
            var match = GetJaroWinklerDistance(item.Key, message);
            Console.WriteLine("wynik " + match);
            if (match > max)
            {
                max = match;
                bestResponse = item.Value;
            }
        }
       

        return bestResponse;
    }

    public static double GetJaroWinklerDistance(string s1, string s2)
    {
        if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
            return 0.0;

        int m = 0; 
        int t = 0; 
        int maxDistance = Math.Max(s1.Length, s2.Length) / 2 - 1;

        bool[] s1Matches = new bool[s1.Length];
        bool[] s2Matches = new bool[s2.Length];

        for (int i = 0; i < s1.Length; i++)
        {
            int start = Math.Max(0, i - maxDistance);
            int end = Math.Min(i + maxDistance + 1, s2.Length);

            for (int j = start; j < end; j++)
            {
                if (!s2Matches[j] && s1[i] == s2[j])
                {
                    s1Matches[i] = true;
                    s2Matches[j] = true;
                    m++;
                    break;
                }
            }
        }

        if (m == 0)
            return 0.0;

        int k = 0;
        for (int i = 0; i < s1.Length; i++)
        {
            if (s1Matches[i])
            {
                while (!s2Matches[k])
                {
                    k++;
                }

                if (s1[i] != s2[k])
                {
                    t++;
                }

                k++;
            }
        }

        t /= 2;

        double jaro = ((double)m / s1.Length + (double)m / s2.Length + (double)(m - t) / m) / 3.0;

        const double prefixScalingFactor = 0.1;
        int prefixLength = 0;

        for (int i = 0; i < Math.Min(4, Math.Min(s1.Length, s2.Length)); i++)
        {
            if (s1[i] == s2[i])
            {
                prefixLength++;
            }
            else
            {
                break;
            }
        }

        return jaro + prefixScalingFactor * prefixLength * (1 - jaro);
    }


  
}
